using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class ArquivoSapDAO {

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT nvl(MAX(ID_ARQUIVO),0)+1 FROM EV_ARQUIVO_SAP";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		private static int NextSeq(TipoArquivoSapEnum tipoArquivo, DateTime dataFolha, EvidaDatabase db) {
			string sql = "SELECT nvl(MAX(NR_SEQUENCIAL),0)+1 FROM EV_ARQUIVO_SAP WHERE DT_FOLHA = :data AND TP_ARQUIVO = :tpArquivo";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":data", DbType.Date, dataFolha));
			lstParam.Add(new Parametro(":tpArquivo", DbType.String, tipoArquivo));
			decimal seq = (decimal)BaseDAO.ExecuteScalar(db, sql, lstParam);
			return (int)seq;
		}

		internal static List<ArquivoSapVerbaVO> ListarVerbas(EvidaDatabase db) {
			string sql = "SELECT * FROM ev_verba_quitacao";

			List<ArquivoSapVerbaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowVerba);
			return lst;
		}

		public static ArquivoSapVerbaVO FromDataRowVerba(DataRow dr) {
			ArquivoSapVerbaVO vo = new ArquivoSapVerbaVO();
			vo.GrupoLancamento = dr.Field<string>("cd_grupo_lancto");
			vo.TipoArq = (TipoArquivoSapEnum)Enum.Parse(typeof(TipoArquivoSapEnum), Convert.ToString(dr["TP_ARQUIVO"]));
			vo.Verba = BaseDAO.GetNullableInt(dr, "cd_verba").Value;
			return vo;
		}

		internal static void Importar(ArquivoSapVO vo, UsuarioVO usuario, EvidaDatabase db) {
			int id = NextId( db);
			int seq = NextSeq(vo.TipoArquivo, vo.DataFolha, db);

			string sql = "INSERT INTO EV_ARQUIVO_SAP (ID_ARQUIVO, DT_FOLHA, NR_SEQUENCIAL, NM_ARQUIVO, DT_IMPORTACAO, DT_RECEBIMENTO, " +
				" CD_STATUS, ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO, TP_ARQUIVO) " +
				" VALUES (:id, :dtFolha, :seq, :nome, LOCALTIMESTAMP, :dtReceb, " +
				" :status, :usuario, LOCALTIMESTAMP, NULL, NULL, :tpArquivo)";

			vo.DataImportacao = DateTime.Now;
			vo.IdArquivo = id;
			vo.Seq = seq;
			vo.Status = "IM";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int64, vo.IdArquivo));
			lstParam.Add(new Parametro(":dtFolha", DbType.Date, vo.DataFolha));
			lstParam.Add(new Parametro(":seq", DbType.Int32, vo.Seq));
			lstParam.Add(new Parametro(":nome", DbType.String, vo.Nome));
			lstParam.Add(new Parametro(":dtReceb", DbType.Date, vo.DataRecebimento));
			lstParam.Add(new Parametro(":status", DbType.String, vo.Status));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, usuario.Id));
			lstParam.Add(new Parametro(":tpArquivo", DbType.String, vo.TipoArquivo.ToString()));
			
			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
			
			InsertItems(vo, db);
		}

		private static void InsertItems(ArquivoSapVO vo, EvidaDatabase db) {

			string sql = "INSERT INTO EV_ARQUIVO_SAP_ITEM (ID_ARQUIVO, NR_SEQUENCIAL_ITEM, CD_EMPRESA, CD_FUNCIONARIO, DT_REFERENCIA, " +
				" CD_VERBA, VL_QUITACAO) " +
				" VALUES (:id, :seqItem, :empresa, :matricula, :data, :verba, :valor) ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int64, vo.IdArquivo));
			lstParam.Add(new ParametroVar(":seqItem", DbType.Int32));
			lstParam.Add(new ParametroVar(":empresa", DbType.Int32));
			lstParam.Add(new ParametroVar(":matricula", DbType.Int64));
			lstParam.Add(new ParametroVar(":data", DbType.Date));
			lstParam.Add(new ParametroVar(":verba", DbType.Int32));
			lstParam.Add(new ParametroVar(":valor", DbType.Double));


			List<ParametroVarRow> lstRows = new List<ParametroVarRow>();
			foreach (ArquivoSapItemVO bVO in vo.Items) {
				ParametroVarRow row = new ParametroVarRow(lstParam);
				row.Set(":seqItem", bVO.SeqItem);
				row.Set(":empresa", bVO.CdEmpresa);
				row.Set(":matricula", bVO.CdFuncionario);
				row.Set(":data", bVO.DataReferencia);
				row.Set(":verba", bVO.Verba);
				row.Set(":valor", bVO.Valor);
				
				lstRows.Add(row);
			}
			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParam, lstRows, db);
		}

		internal static DataTable PesquisarArquivos(TipoArquivoSapEnum? tipoArquivo, int? ano, int? mes, string status, EvidaDatabase db) {
			string sql = "SELECT F.ID_ARQUIVO, F.NM_ARQUIVO, F.DT_FOLHA, F.NR_SEQUENCIAL, DT_IMPORTACAO, F.TP_ARQUIVO, " +
				" F.DT_RECEBIMENTO, F.CD_STATUS, F.ID_USUARIO_CRIACAO " +
				" FROM EV_ARQUIVO_SAP F " +
				" WHERE 1 = 1 ";

			List<Parametro> lstParam = new List<Parametro>();
			if (mes != null && ano != null) {
				sql += " AND F.DT_FOLHA = :dataFolha";
				DateTime dataFolha = new DateTime(ano.Value, mes.Value, 1);
				lstParam.Add(new Parametro(":dataFolha", DbType.Date, dataFolha));
			} else if (mes != null) {				
				sql += " AND to_char(F.DT_FOLHA, 'MM') = to_char(:dataFolha, 'MM') ";
				DateTime dataFolha = new DateTime(DateTime.Now.Year, mes.Value, 1);
				lstParam.Add(new Parametro(":dataFolha", DbType.Date, dataFolha));
			} else if (ano != null) {
				sql += " AND to_char(F.DT_FOLHA, 'rrrr') = to_char(:dataFolha, 'rrrr') ";
				DateTime dataFolha = new DateTime(ano.Value, 1, 1);
				lstParam.Add(new Parametro(":dataFolha", DbType.Date, dataFolha));
			}
			if (!string.IsNullOrEmpty(status)) {
				sql += " AND F.CD_STATUS = :status";
				lstParam.Add(new Parametro(":status", DbType.String, status));
			}
			if (tipoArquivo != null) {
				sql += " AND F.TP_ARQUIVO = :tpArquivo";
				lstParam.Add(new Parametro(":tpArquivo", DbType.String, tipoArquivo.ToString()));
			}

			sql += " ORDER BY F.DT_FOLHA DESC, F.NR_SEQUENCIAL DESC ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);

			return dt;
		}

		internal static ArquivoSapVO GetById(long id, EvidaDatabase db) {
			string sql = " select * " +
				" FROM EV_ARQUIVO_SAP A " +
				" WHERE A.ID_ARQUIVO = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int64, id));

			List<ArquivoSapVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static ArquivoSapVO FromDataRow(DataRow dr) {
			ArquivoSapVO vo = new ArquivoSapVO();

			vo.IdArquivo = Convert.ToInt32(dr["ID_ARQUIVO"]);
			vo.DataFolha = Convert.ToDateTime(dr["DT_FOLHA"]);
			vo.Seq = Convert.ToInt32(dr["NR_SEQUENCIAL"]);
			vo.DataImportacao = Convert.ToDateTime(dr["DT_IMPORTACAO"]);
			vo.DataRecebimento = Convert.ToDateTime(dr["DT_RECEBIMENTO"]);
			vo.Nome = Convert.ToString(dr["NM_ARQUIVO"]);
			vo.Status = Convert.ToString(dr["CD_STATUS"]);
			vo.TipoArquivo = (TipoArquivoSapEnum)Enum.Parse(typeof(TipoArquivoSapEnum), Convert.ToString(dr["TP_ARQUIVO"]));
			return vo;
		}

		private static string SqlInconsistencia(bool useRight) {
			string sql =
				@"WITH 
					VQ AS (SELECT CD_GRUPO_LANCTO, CD_VERBA FROM EV_VERBA_QUITACAO WHERE TP_ARQUIVO = :tipoArq),
					pisa AS (select b.cd_empresa, b.cd_funcionario, SUM(p.vl_parcela) vl_parcela, p.cd_grupo_lancto
							from isa_hc.hc_parcela_debito_benef p, isa_hc.hc_beneficiario b
							WHERE p.dt_ano_mes_ref = :dataFolha           
								and p.cd_grupo_lancto IN (SELECT CD_GRUPO_LANCTO FROM VQ)
								and b.cd_beneficiario = p.cd_beneficiario 
							GROUP BY b.cd_empresa, b.cd_funcionario, p.cd_grupo_lancto
							),
						ITEM AS (SELECT AI.CD_EMPRESA, AI.CD_FUNCIONARIO, AI.VL_QUITACAO, VQ.CD_GRUPO_LANCTO, AI.CD_VERBA
							FROM EV_ARQUIVO_SAP_ITEM AI, VQ
							WHERE ai.cd_verba = vq.cd_verba 
								AND AI.ID_ARQUIVO = :id),
						SAP AS (SELECT AI.CD_EMPRESA, AI.CD_FUNCIONARIO, SUM(AI.VL_QUITACAO) vl_sap, cd_grupo_lancto
							FROM ITEM AI
							group by ai.cd_empresa, ai.cd_funcionario, cd_grupo_lancto
							),
						VERBA AS (SELECT AI.CD_EMPRESA, AI.CD_FUNCIONARIO, SUM(AI.VL_QUITACAO) VL_SAP, CD_VERBA
							FROM ITEM AI
							GROUP BY CD_EMPRESA, CD_FUNCIONARIO, CD_VERBA)
					SELECT d.cd_empresa, d.cd_funcionario, d.vl_isa, d.vl_sap, d.vl_diff, ai.cd_verba, ai.vl_sap vl_quitacao, D.CD_GRUPO_LANCTO          
					FROM (
					SELECT nvl(pisa.cd_empresa, sap.cd_empresa) cd_empresa, nvl(pisa.cd_funcionario, sap.cd_funcionario) cd_funcionario,
							pisa.vl_parcela vl_isa, sap.vl_sap, nvl(sap.vl_sap,0)-nvl(pisa.vl_parcela,0) vl_diff, SAP.CD_GRUPO_LANCTO
					FROM pisa, sap
					where pisa.cd_funcionario (+)= sap.cd_funcionario and pisa.cd_empresa (+)= sap.cd_empresa
							AND pisa.cd_grupo_lancto (+) = sap.cd_grupo_lancto 
					UNION
					SELECT nvl(pisa.cd_empresa, sap.cd_empresa) cd_empresa, nvl(pisa.cd_funcionario, sap.cd_funcionario) cd_funcionario,
							pisa.vl_parcela vl_isa, sap.vl_sap, nvl(sap.vl_sap,0)-nvl(pisa.vl_parcela,0) vl_diff, SAP.CD_GRUPO_LANCTO
					FROM pisa, sap
					where pisa.cd_funcionario = sap.cd_funcionario(+) and pisa.cd_empresa = sap.cd_empresa(+) $$USE RIGHT$$
					) D, VERBA ai, VQ
					WHERE nvl(d.vl_diff,-1) <> 0
							AND ai.cd_funcionario = d.cd_funcionario AND ai.cd_empresa = d.cd_empresa
							AND VQ.CD_VERBA = AI.CD_VERBA AND VQ.CD_GRUPO_LANCTO = D.CD_GRUPO_LANCTO
					ORDER BY 1, 2, 6";
			string replacement = "";
			if (!useRight) {
				replacement = "AND 1 = 0";
			}

			return sql.Replace("$$USE RIGHT$$", replacement);
		}

		internal static DataTable RelatorioInconsistencia(long idArquivo, EvidaDatabase db) {
			ArquivoSapVO vo = GetById(idArquivo, db);
			DateTime dtFolha = vo.DataFolha;
			bool useRight = vo.TipoArquivo == TipoArquivoSapEnum.PPRS;
			string sql = SqlInconsistencia(useRight);

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":dataFolha", DbType.Date, dtFolha));
			lstParam.Add(new Parametro(":id", DbType.Int32, idArquivo));
			lstParam.Add(new Parametro(":tipoArq", DbType.String, vo.TipoArquivo.ToString()));

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);

			return dt;
		}

		internal static DataTable RelatorioInconsistencia2(long idArquivo, EvidaDatabase db) {
			ArquivoSapVO vo = GetById(idArquivo, db);
			DateTime dtFolha = vo.DataFolha;
			string sql = null;

			if (vo.TipoArquivo == TipoArquivoSapEnum.PPRS) {
				sql = "SELECT nvl(pisa.cd_empresa, sap.cd_empresa) cd_empresa, nvl(pisa.cd_funcionario, sap.cd_funcionario) cd_funcionario, " +
					"	pisa.vl_parcela vl_isa, sap.vl_sap, nvl(sap.vl_sap,0)-nvl(pisa.vl_parcela,0) vl_diff " +
					"	FROM " +
					"		(select b.cd_empresa, b.cd_funcionario, p.vl_parcela " +
					"		from isa_hc.hc_parcela_debito_benef p, isa_hc.hc_beneficiario b " +
					"		WHERE p.dt_ano_mes_ref = :dataFolha and p.cd_grupo_lancto in ('MENSALIDADE 20') " +
					"			and b.cd_beneficiario = p.cd_beneficiario " +
					"		) pisa, " +
					"		(SELECT AI.CD_EMPRESA, AI.CD_FUNCIONARIO, SUM(VL_QUITACAO) vl_sap " +
					"		FROM ev_arquivo_sap a, ev_arquivo_sap_item ai " +
					"		WHERE a.id_arquivo = ai.id_arquivo and ai.cd_verba IN (8102, 8105, 8109) AND AI.ID_ARQUIVO = :id " +
					"		group by ai.cd_empresa, ai.cd_funcionario " +
					"		) sap " +
					"	where pisa.cd_funcionario (+)= sap.cd_funcionario and pisa.cd_empresa (+)= sap.cd_empresa" +

					"	UNION " +

					"	SELECT nvl(sap.cd_empresa, pisa.cd_empresa) cd_empresa, nvl(sap.cd_funcionario, pisa.cd_funcionario) cd_funcionario, " +
					"	pisa.vl_parcela vl_isa, sap.vl_sap, nvl(sap.vl_sap,0)-nvl(pisa.vl_parcela,0) vl_diff " +
					"	FROM " +
					"		(select b.cd_empresa, b.cd_funcionario, p.vl_parcela " +
					"		from isa_hc.hc_parcela_debito_benef p, isa_hc.hc_beneficiario b " +
					"		WHERE p.dt_ano_mes_ref = :dataFolha and p.cd_grupo_lancto in ('MENSALIDADE 20') " +
					"			and b.cd_beneficiario = p.cd_beneficiario " +
					"		) pisa, " +
					"		(SELECT ai.cd_empresa, AI.CD_FUNCIONARIO, SUM(VL_QUITACAO) vl_sap " +
					"		FROM ev_arquivo_sap a, ev_arquivo_sap_item ai " +
					"		WHERE a.id_arquivo = ai.id_arquivo and ai.cd_verba IN (8102, 8105, 8109) AND AI.ID_ARQUIVO = :id " +
					"		group by ai.cd_empresa, ai.cd_funcionario " +
					"		) sap " +
					"	where pisa.cd_funcionario = sap.cd_funcionario(+) and pisa.cd_empresa = sap.cd_empresa(+) ";


				sql = "SELECT d.cd_empresa, d.cd_funcionario, d.vl_isa, d.vl_sap, d.vl_diff, ai.cd_verba, ai.vl_quitacao " +
					" FROM (" + sql + ") d, " +
					"	(SELECT ai.cd_verba, ai.vl_quitacao, ai.cd_empresa, ai.cd_funcionario FROM EV_ARQUIVO_SAP_ITEM ai " +
					"	WHERE ai.cd_verba IN (8102, 8105, 8109) AND ai.id_arquivo = :id) ai " +
					" WHERE ai.cd_funcionario = d.cd_funcionario AND ai.cd_empresa = d.cd_empresa" +
					"	AND nvl(d.vl_diff,-1) <> 0";
			} else {
				sql = "SELECT nvl(pisa.cd_empresa, sap.cd_empresa) cd_empresa, nvl(pisa.cd_funcionario, sap.cd_funcionario) cd_funcionario, " +
					"	pisa.vl_parcela vl_isa, sap.vl_sap, nvl(sap.vl_sap,0)-nvl(pisa.vl_parcela,0) vl_diff, " +
					"	nvl(pisa.cd_grupo_lancto, sap.cd_grupo_lancto) cd_grupo_lancto, sap.cd_verba, sap.vl_sap vl_quitacao " +
					"	FROM " +
					"		(select b.cd_empresa, b.cd_funcionario, SUM(p.vl_parcela) vl_parcela, p.cd_grupo_lancto " +
					"		from isa_hc.hc_parcela_debito_benef p, isa_hc.hc_beneficiario b " +
					"		WHERE p.dt_ano_mes_ref = :dataFolha and p.cd_grupo_lancto in ('MENSALIDADE 22', '22 COPARTICIPACAO') " +
					"			and b.cd_beneficiario = p.cd_beneficiario " +
					"		group by b.cd_empresa, b.cd_funcionario, p.cd_grupo_lancto " +
					"		) pisa, " +
					"		(SELECT AI.CD_EMPRESA, AI.CD_FUNCIONARIO, SUM(VL_QUITACAO) vl_sap, ai.cd_verba, DECODE(ai.cd_verba, 8107, 'MENSALIDADE 22', 8114, '22 COPARTICIPACAO') cd_grupo_lancto " +
					"		FROM ev_arquivo_sap a, ev_arquivo_sap_item ai " +
					"		WHERE a.id_arquivo = ai.id_arquivo and ai.cd_verba IN (8107, 8114) AND AI.ID_ARQUIVO = :id " +
					"		group by ai.cd_empresa, ai.cd_funcionario, ai.cd_verba " +
					"		) sap " +
					"	where pisa.cd_funcionario (+)= sap.cd_funcionario  and pisa.cd_empresa (+)= sap.cd_empresa " +
					"		and nvl(pisa.vl_parcela,0) - nvl(sap.vl_sap,0) <> 0 " +
					"		and pisa.cd_grupo_lancto (+) = sap.cd_grupo_lancto ";
			}

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":dataFolha", DbType.Date, dtFolha));
			lstParam.Add(new Parametro(":id", DbType.Int32, idArquivo));

			sql += " ORDER BY cd_empresa, cd_funcionario, cd_verba ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);

			return dt;
		}

		internal static bool ExisteParcelaPosterior(TipoArquivoSapEnum tpArquivo, DateTime dataFolha, EvidaDatabase db) {
			string sql = null;
			
			/*if (tpArquivo == TipoArquivoSapEnum.PPRS)
				sql = "select count(1) from isa_hc.hc_parcela_debito_benef p where p.cd_grupo_lancto in ('MENSALIDADE 20') and p.dt_ano_mes_ref > :dataFolha";
			else
				sql = "select count(1) from isa_hc.hc_parcela_debito_benef p where p.cd_grupo_lancto in ('MENSALIDADE 22', '22 COPARTICIPACAO') and p.dt_ano_mes_ref > :dataFolha";*/

			sql = @"SELECT COUNT(1) FROM ISA_HC.HC_PARCELA_DEBITO_BENEF P 
					WHERE p.dt_ano_mes_ref > :dataFolha 
						AND P.CD_GRUPO_LANCTO IN (SELECT CD_GRUPO_LANCTO FROM EV_VERBA_QUITACAO WHERE TP_ARQUIVO = :tipoArq)";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":dataFolha", DbType.Date, dataFolha));
			lstParam.Add(new Parametro(":tipoArq", DbType.String, tpArquivo.ToString()));

			decimal d = (decimal)BaseDAO.ExecuteScalar(db, sql, lstParam);
			return d > 0;
		}

		internal static void Quitar(long id, UsuarioVO usuario, EvidaDatabase db) {
			QuitarCancelar(id, usuario, true, db);
		}

		internal static void Cancelar(long id, UsuarioVO usuario, EvidaDatabase db) {
			QuitarCancelar(id, usuario, false, db);
		}

		private static void QuitarCancelar(long id, UsuarioVO usuario, bool quitar, EvidaDatabase db) {
			const string sqlProcCanc = "BEGIN PC_QUITAR_CANC_ARQUIVO_SAP(:id, :idUsuario, :login, :op); END;";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int64, id));
			lstParam.Add(new Parametro(":idUsuario", DbType.Int32, usuario.Id));
			lstParam.Add(new Parametro(":login", DbType.String, usuario.Login));
			lstParam.Add(new Parametro(":op", DbType.String, quitar ? "QUITAR" : "CANCELAR"));

			BaseDAO.ExecuteNonQuery(sqlProcCanc, lstParam, db);
		}

	}
}
