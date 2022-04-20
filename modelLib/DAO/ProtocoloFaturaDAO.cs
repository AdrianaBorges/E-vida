using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class ProtocoloFaturaDAO {

        private static string FIELDS = " CD_PROTOCOLO_FATURA, DT_ENTRADA, NR_PROTOCOLO, BAU_CODIGO, NR_ANO_ENTRADA, DT_EMISSAO, DT_ENTRADA, DT_VENCIMENTO, " +
        " DS_DOC_FISCAL, VL_APRESENTADO, VL_GLOSA, VL_PROCESSADO, CD_USUARIO_CRIACAO, CD_USUARIO_ANALISTA, ID_SITUACAO, " +
        " NVL(BCI_FASE, 0) BCI_FASE, DS_MOTIVO_CANCELAMENTO, CD_PENDENCIA, DT_CRIACAO, DT_FINALIZACAO, DT_EXPEDICAO "; 

		private static int NextId(EvidaDatabase evdb) {
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, "SELECT SQ_EV_PROTOCOLO_FATURA.nextval FROM DUAL");

			return (int)idSolicitacao;
		}

		internal static DataTable Pesquisar(string nrProtocolo, string nrCpfCnpj, string razaoSocial, List<int> lstAnalistasResp, string docFiscal, DateTime? dtEmissao, decimal? vlApresentado,
            DateTime? dtEntradaInicio, DateTime? dtEntradaFim, /*StatusProtocoloFatura? status,*/ FaseProtocoloFatura? fase, DateTime? dtVencimentoInicio, DateTime? dtVencimentoFim, 
			DateTime? dtFinalizacaoInicio, DateTime? dtFinalizacaoFim, DateTime? dtExpedicaoInicio, DateTime? dtExpedicaoFim, 
			string cdNatureza, string cdRegional, int? controle, EvidaDatabase evdb) {

                string sql = "SELECT P.CD_PROTOCOLO_FATURA, P.NR_PROTOCOLO, C.BAU_CPFCGC, C.BAU_NOME, P.DS_DOC_FISCAL, P.DT_EMISSAO, P.DT_ENTRADA, P.VL_APRESENTADO, " +
                "   P.DT_VENCIMENTO, P.VL_PROCESSADO, P.VL_GLOSA, P.CD_PENDENCIA, P.DT_FINALIZACAO, P.DT_EXPEDICAO, CRIADOR.CD_USUARIO criador_CD_USUARIO, " +
                "   ANALISTA.ID_USUARIO analista_ID_USUARIO, ANALISTA.CD_USUARIO analista_CD_USUARIO, P.ID_SITUACAO, NVL(P.BCI_FASE, 0) BCI_FASE, P.DS_MOTIVO_CANCELAMENTO, A.BAG_DESCRI, " +
                "   (select BAU_REGMUN from VW_PR_REDE_ATENDIMENTO RA where to_char(P.DT_CRIACAO, 'YYYYMMDD') BETWEEN trim(RA.BAU_DTINCL) AND NVL(RA.BAU_DATBLO, to_char(P.DT_CRIACAO, 'YYYYMMDD')) and trim(RA.BAU_CODIGO) = trim(p.BAU_CODIGO)) BAU_REGMUN, " +
                "   (SELECT ds_motivo_pendencia from EV_MOTIVO_PENDENCIA pf WHERE pf.cd_motivo_pendencia = p.cd_pendencia) ds_pendencia, " +
                "   P.E2_VALOR, P.E2_ISS, P.E2_IRRF, P.E2_DESCONT, P.E2_COFINS, P.E2_PIS, P.E2_CSLL, to_date(replace(P.E2_BAIXA, '        ', null), 'yyyymmdd') as E2_BAIXA, P.E2_VALLIQ " +
                "   FROM EV_PROTOCOLO_FATURA P, EV_USUARIO CRIADOR, EV_USUARIO ANALISTA, VW_PR_REDE_ATENDIMENTO C, VW_PR_PEG E, VW_PR_CLASSE_REDE_ATENDIMENTO A " +
                "   WHERE P.CD_USUARIO_CRIACAO = CRIADOR.ID_USUARIO " +
                "   AND P.CD_USUARIO_ANALISTA = ANALISTA.ID_USUARIO(+) " +
                "   AND trim(P.BAU_CODIGO) = trim(C.BAU_CODIGO) " +
                "   AND substr(trim(P.NR_PROTOCOLO), 1, 8) = trim(E.BCI_CODPEG(+)) " +
                "   AND trim(E.BCI_TIPPRE) = trim(A.BAG_CODIGO(+)) ";

			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(nrProtocolo)) {
				sql += " AND p.NR_PROTOCOLO = :protocolo ";
				lstParams.Add(new Parametro() { Name = ":protocolo", Tipo = DbType.String, Value = nrProtocolo });
			}
            if (!string.IsNullOrEmpty(nrCpfCnpj))
            {
                sql += " AND trim(c.BAU_CPFCGC) = trim(:cpfCnpj) ";
				lstParams.Add(new Parametro() { Name = ":cpfCnpj", Tipo = DbType.String, Value = nrCpfCnpj });
			}
			if (!string.IsNullOrEmpty(razaoSocial)) {
                sql += " AND upper(trim(c.BAU_NOME)) LIKE upper(trim(:razaoSocial)) ";
				lstParams.Add(new Parametro() { Name = ":razaoSocial", Tipo = DbType.String, Value = "%" + razaoSocial.ToUpper() + "%"});
			}
			if (lstAnalistasResp != null && lstAnalistasResp.Count > 0) {
				sql += " AND P.CD_USUARIO_ANALISTA IN (0";
				foreach (int idUsuario in lstAnalistasResp) {
					sql += "," + idUsuario;
				}
				sql += ")";
				//lstParams.Add(new Parametro() { Name = ":analista", Tipo = DbType.String, Value = "%" + analista.ToUpper() + "%" });
			}
			if (!string.IsNullOrEmpty(docFiscal)) {
                sql += " AND upper(trim(p.DS_DOC_FISCAL)) LIKE upper(trim(:docFiscal)) ";
				lstParams.Add(new Parametro() { Name = ":docFiscal", Tipo = DbType.String, Value = "%" + docFiscal.ToUpper() + "%" });
			}
			if (dtEmissao != null) {
				sql += " AND p.dt_emissao = :dtEmissao ";
				lstParams.Add(new Parametro() { Name = ":dtEmissao", Tipo = DbType.Date, Value = dtEmissao.Value });
			}
			if (vlApresentado != null) {
				sql += " AND p.vl_apresentado = :vlApresentado ";
				lstParams.Add(new Parametro() { Name = ":vlApresentado", Tipo = DbType.Decimal, Value = vlApresentado.Value });
			}
			if (dtEntradaInicio != null && dtEntradaFim != null) {
				sql += " AND p.dt_entrada between :dtEntradaInicio AND :dtEntradaFim ";
				lstParams.Add(new Parametro() { Name = ":dtEntradaInicio", Tipo = DbType.Date, Value = dtEntradaInicio.Value });
				lstParams.Add(new Parametro() { Name = ":dtEntradaFim", Tipo = DbType.Date, Value = dtEntradaFim.Value });
			}
			/*if (status != null) {
				sql += " AND p.id_situacao = :status ";
				lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = (int)status.Value });
			}*/
            if (fase != null)
            {
                sql += " AND p.BCI_FASE = :fase ";
                lstParams.Add(new Parametro() { Name = ":fase", Tipo = DbType.Int32, Value = (int)fase.Value });
            }
			if (dtVencimentoInicio != null && dtVencimentoFim != null) {
				sql += " AND p.DT_VENCIMENTO between :dtVencimentoInicio AND :dtVencimentoFim ";
				lstParams.Add(new Parametro() { Name = ":dtVencimentoInicio", Tipo = DbType.Date, Value = dtVencimentoInicio.Value });
				lstParams.Add(new Parametro() { Name = ":dtVencimentoFim", Tipo = DbType.Date, Value = dtVencimentoFim.Value });
			}
			if (dtFinalizacaoInicio != null && dtFinalizacaoFim != null) {
				sql += " AND p.DT_FINALIZACAO between :dtFinalizacaoInicio AND :dtFinalizacaoFim ";
				lstParams.Add(new Parametro() { Name = ":dtFinalizacaoInicio", Tipo = DbType.Date, Value = dtFinalizacaoInicio.Value });
				lstParams.Add(new Parametro() { Name = ":dtFinalizacaoFim", Tipo = DbType.Date, Value = dtFinalizacaoFim.Value });
			}
			if (dtExpedicaoInicio != null && dtExpedicaoFim != null) {
				sql += " AND p.DT_EXPEDICAO between :dtExpedicaoInicio AND :dtExpedicaoFim ";
				lstParams.Add(new Parametro() { Name = ":dtExpedicaoInicio", Tipo = DbType.Date, Value = dtExpedicaoInicio.Value });
				lstParams.Add(new Parametro() { Name = ":dtExpedicaoFim", Tipo = DbType.Date, Value = dtExpedicaoFim.Value });
			}
            if (!string.IsNullOrEmpty(cdNatureza))
            {
                sql += " AND trim(E.BCI_TIPPRE) = trim(:cdNatureza) ";
                lstParams.Add(new Parametro() { Name = ":cdNatureza", Tipo = DbType.String, Value = cdNatureza });
            }
			if (cdRegional != null) {
				sql += " AND EXISTS (SELECT 1 FROM VW_PR_REDE_ATENDIMENTO CR " +
                       " WHERE trim(CR.BAU_REGMUN) = trim(:cdRegional) " +
                       " AND trim(CR.BAU_CODIGO) = trim(C.BAU_CODIGO) " +
                       " AND to_char(P.DT_CRIACAO, 'YYYYMMDD') BETWEEN trim(CR.BAU_DTINCL) AND NVL(CR.BAU_DATBLO, to_char(P.DT_CRIACAO, 'YYYYMMDD'))) ";
                lstParams.Add(new Parametro() { Name = ":cdRegional", Tipo = DbType.String, Value = cdRegional });
			}
			if (controle != null) {
				lstParams.Add(new Parametro() { Name = ":data", Tipo = DbType.Date, Value = DateTime.Now.Date });
				if (controle == ProtocoloFaturaVO.CONTROLE_ATRASADO) {
					sql += " AND p.dt_vencimento < NVL(p.dt_finalizacao, :data) ";
				} else if (controle == ProtocoloFaturaVO.CONTROLE_ALERTA) {
					sql += " AND NVL(p.dt_finalizacao, :data) BETWEEN p.dt_vencimento-" + ProtocoloFaturaVO.DIAS_ALERTA + " AND p.dt_vencimento ";
				} else if (controle == ProtocoloFaturaVO.CONTROLE_OK) {
					sql += " AND NVL(p.dt_finalizacao, :data) < p.dt_vencimento-" + ProtocoloFaturaVO.DIAS_ALERTA;
				}
			}

			sql += " ORDER BY CD_PROTOCOLO_FATURA desc ";
			DataTable dt = BaseDAO.ExecuteDataSet(evdb, sql, lstParams);

			return dt;
		}

		internal static List<ProtocoloFaturaVO> FindAllActiveNotify(EvidaDatabase db) {
			string sql = "SELECT * FROM EV_PROTOCOLO_FATURA p" +
				"		WHERE p.id_situacao not in (:status1, :status2) AND p.CD_USUARIO_ANALISTA is not null ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":status1", DbType.Int32, StatusProtocoloFatura.FINALIZADO));
			lstParam.Add(new Parametro(":status2", DbType.Int32, StatusProtocoloFatura.CANCELADO));
			lstParam.Add(new Parametro(":data", DbType.Date, DateTime.Now.Date));

			sql += " AND (p.dt_vencimento < NVL(p.dt_finalizacao, :data) ";
			sql += " OR NVL(p.dt_finalizacao, :data) BETWEEN p.dt_vencimento-" + ProtocoloFaturaVO.DIAS_ALERTA + " AND p.dt_vencimento )";

			return BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);
		}

		internal static ProtocoloFaturaVO GetById(int id, EvidaDatabase evdb) {
			string sql = "SELECT " + FIELDS +
                " FROM EV_PROTOCOLO_FATURA A" +
				" WHERE a.CD_PROTOCOLO_FATURA = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			List<ProtocoloFaturaVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

        internal static bool ExisteProtocoloFatura(string codpeg, EvidaDatabase evdb)
        {
            string sql = "SELECT COUNT(*) " +
                " FROM EV_PROTOCOLO_FATURA " +
                " WHERE NR_PROTOCOLO like '%' || :codpeg || '%' ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":codpeg", DbType.String, codpeg));

            int count = Convert.ToInt32(BaseDAO.ExecuteScalar(evdb, sql, lstParam));
            return count > 0;
        }

		private static ProtocoloFaturaVO FromDataRow(DataRow dr) {
			ProtocoloFaturaVO vo = new ProtocoloFaturaVO();
			vo.Id = Convert.ToInt32(dr["CD_PROTOCOLO_FATURA"]);
			vo.DataEntrada = Convert.ToDateTime(dr["DT_ENTRADA"]);

			vo.NrProtocolo = Convert.ToString(dr["NR_PROTOCOLO"]);

            vo.RedeAtendimento = new VO.Protheus.PRedeAtendimentoVO()
            {
                Codigo = dr.IsNull("BAU_CODIGO") ? "" : dr["BAU_CODIGO"].ToString()
            };

			vo.AnoEntrada = Convert.ToInt32(dr["NR_ANO_ENTRADA"]);

			vo.DataEmissao = BaseDAO.GetNullableDate(dr, "DT_EMISSAO");
			vo.DataEntrada = BaseDAO.GetNullableDate(dr, "DT_ENTRADA").Value;
			vo.DataVencimento = BaseDAO.GetNullableDate(dr, "DT_VENCIMENTO").Value;
			vo.DocumentoFiscal = Convert.ToString(dr["DS_DOC_FISCAL"]);

			vo.ValorApresentado = BaseDAO.GetNullableDecimal(dr, "VL_APRESENTADO");
			vo.ValorGlosa = BaseDAO.GetNullableDecimal(dr, "VL_GLOSA");
			vo.ValorProcessado = BaseDAO.GetNullableDecimal(dr, "VL_PROCESSADO");

			vo.CdUsuarioCriacao = Convert.ToInt32(dr["CD_USUARIO_CRIACAO"]);
			vo.CdUsuarioResponsavel = BaseDAO.GetNullableInt(dr, "CD_USUARIO_ANALISTA");

			vo.Situacao = (StatusProtocoloFatura)Convert.ToInt32(dr["ID_SITUACAO"]);
            vo.Fase = (FaseProtocoloFatura)Convert.ToInt32(dr["BCI_FASE"]);
			vo.MotivoCancelamento = Convert.ToString(dr["DS_MOTIVO_CANCELAMENTO"]);
			vo.CdPendencia = BaseDAO.GetNullableInt(dr, "CD_PENDENCIA");

			vo.DataCriacao = BaseDAO.GetNullableDate(dr, "DT_CRIACAO").Value;
			vo.DataFinalizacao = BaseDAO.GetNullableDate(dr, "DT_FINALIZACAO");
			vo.DataExpedicao = BaseDAO.GetNullableDate(dr, "DT_EXPEDICAO");

			return vo;
		}

		internal static void Gerar(ProtocoloFaturaVO vo, EvidaDatabase evdb) {
			
			string sql = "INSERT INTO EV_PROTOCOLO_FATURA (CD_PROTOCOLO_FATURA, NR_PROTOCOLO, DT_ENTRADA, NR_ANO_ENTRADA, BAU_CODIGO, " +
                "	DS_DOC_FISCAL, VL_APRESENTADO, DT_EMISSAO, DT_VENCIMENTO, CD_USUARIO_CRIACAO, ID_SITUACAO, DT_CRIACAO, VL_PROCESSADO, VL_GLOSA, DT_FINALIZACAO, BCI_FASE) " +
                "	VALUES (:id, :nrProtocolo, :entrada, :ano, :cdRedeAtendimento, :docFiscal, :vlApresentado, :emissao, :vencimento, :usuario, :situacao, LOCALTIMESTAMP, :vlProcessado, :vlGlosa, :finalizacao, :fase) ";

            vo.Id = NextId(evdb);
            vo.NrProtocolo = vo.NrProtocolo.PadLeft(8, '0') + "/" + DateTime.Now.Year;

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":entrada", DbType.Date, vo.DataEntrada);
			db.AddInParameter(dbCommand, ":nrProtocolo", DbType.String, vo.NrProtocolo);
			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.AnoEntrada);
            db.AddInParameter(dbCommand, ":cdRedeAtendimento", DbType.String, vo.RedeAtendimento.Codigo.Trim().PadLeft(6, '0'));
			db.AddInParameter(dbCommand, ":docFiscal", DbType.String, vo.DocumentoFiscal);
			db.AddInParameter(dbCommand, ":vlApresentado", DbType.Decimal, vo.ValorApresentado);
			db.AddInParameter(dbCommand, ":emissao", DbType.Date, vo.DataEmissao);
			db.AddInParameter(dbCommand, ":vencimento", DbType.Date, vo.DataVencimento);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, vo.CdUsuarioCriacao);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, StatusProtocoloFatura.PROTOCOLADO);
            db.AddInParameter(dbCommand, ":vlProcessado", DbType.Decimal, vo.ValorProcessado);
            db.AddInParameter(dbCommand, ":vlGlosa", DbType.Decimal, vo.ValorGlosa);
            db.AddInParameter(dbCommand, ":finalizacao", DbType.Date, vo.DataFinalizacao);
            db.AddInParameter(dbCommand, ":fase", DbType.Int32, (int) vo.Fase);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void Salvar(ProtocoloFaturaVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_PROTOCOLO_FATURA SET DT_ENTRADA = :entrada, NR_ANO_ENTRADA = :ano, BAU_CODIGO = :cdRedeAtendimento, " +
				"	DS_DOC_FISCAL = :docFiscal, VL_APRESENTADO = :vlApresentado, DT_EMISSAO = :emissao, ID_SITUACAO = :situacao, " +
				"	CD_USUARIO_ANALISTA = :analista, VL_PROCESSADO = :vlProcessado, VL_GLOSA = :vlGlosa, DT_VENCIMENTO = :vencimento, " +
				"	DT_FINALIZACAO = :finalizacao, CD_PENDENCIA = :cdPendencia, DT_EXPEDICAO = :dtExpedicao " +
				"	WHERE CD_PROTOCOLO_FATURA = :id ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":entrada", DbType.Date, vo.DataEntrada);
			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.AnoEntrada);
            db.AddInParameter(dbCommand, ":cdRedeAtendimento", DbType.String, vo.RedeAtendimento.Codigo.Trim().PadLeft(6, '0'));
			db.AddInParameter(dbCommand, ":docFiscal", DbType.String, vo.DocumentoFiscal);
			db.AddInParameter(dbCommand, ":vlApresentado", DbType.Decimal, vo.ValorApresentado);
			db.AddInParameter(dbCommand, ":vlGlosa", DbType.Decimal, vo.ValorGlosa);
			db.AddInParameter(dbCommand, ":vlProcessado", DbType.Decimal, vo.ValorProcessado);
			db.AddInParameter(dbCommand, ":emissao", DbType.Date, vo.DataEmissao);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, (int)vo.Situacao);
			db.AddInParameter(dbCommand, ":analista", DbType.Int32, vo.CdUsuarioResponsavel);
			db.AddInParameter(dbCommand, ":cdPendencia", DbType.Int32, vo.CdPendencia);
			db.AddInParameter(dbCommand, ":vencimento", DbType.Date, vo.DataVencimento);
			db.AddInParameter(dbCommand, ":finalizacao", DbType.Date, vo.DataFinalizacao);
			db.AddInParameter(dbCommand, ":dtExpedicao", DbType.Date, vo.DataExpedicao);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void Cancelar(int id, string motivo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_PROTOCOLO_FATURA SET DS_MOTIVO_CANCELAMENTO = :motivo, ID_SITUACAO = :situacao, DT_CANCELAMENTO = LOCALTIMESTAMP " +
				"	WHERE CD_PROTOCOLO_FATURA = :id ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":motivo", DbType.String, motivo);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, (int)StatusProtocoloFatura.CANCELADO);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

        internal static void Mesclar(string duplicados, EvidaDatabase evdb)
        {
            StringBuilder sql = new StringBuilder();

            string strDuplicados = "";
            if(!string.IsNullOrEmpty(duplicados.Trim())){
                strDuplicados = " where BCI_CODPEG not in (" + duplicados + ")";
            }

            sql.Append(" merge into ev_protocolo_fatura a ");
            sql.Append(" using (select * from VW_PR_PEG " + strDuplicados + ") b ");
            sql.Append(" on (substr(trim(a.NR_PROTOCOLO), 1, 8) = trim(b.BCI_CODPEG) and b.BCI_YREDOC <> '        ' and b.BCI_YVEDOC <> '        ') ");
            sql.Append(" when matched then update set ");
            sql.Append(" a.DT_ENTRADA = decode(b.bci_yredoc, '        ', null, to_date(trim(b.bci_yredoc), 'yyyyMMdd')), ");
            sql.Append(" a.DS_DOC_FISCAL = b.bci_ydoc, ");
            sql.Append(" a.VL_APRESENTADO = b.bci_yvldoc, ");
            sql.Append(" a.DT_EMISSAO = decode(b.bci_yemiss, '        ', null, to_date(trim(b.bci_yemiss), 'yyyyMMdd')), ");
            sql.Append(" a.DT_VENCIMENTO = decode(b.bci_yvedoc, '        ', null, to_date(trim(b.bci_yvedoc), 'yyyyMMdd')), ");
            sql.Append(" a.VL_PROCESSADO = b.bci_vlrgui, ");
            sql.Append(" a.VL_GLOSA = b.bci_vlrglo, ");
            sql.Append(" a.DT_FINALIZACAO = decode(b.bci_dthrlb, '        ', null, to_date(substr(trim(b.bci_dthrlb), 1, 8), 'yyyyMMdd - hh24:mi:ss')), ");
            sql.Append(" a.BAU_CODIGO = b.bci_codrda, ");
            sql.Append(" a.BCI_FASE = b.bci_fase ");

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql.ToString());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        // Exclui da Intranet os protocolos que foram excluídos no Protheus
        internal static void ExcluirCancelados(EvidaDatabase evdb)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" delete from ev_protocolo_fatura a ");
            sql.Append(" where a.bau_codigo is not null ");
            sql.Append(" and not exists (select * from vw_pr_peg b where substr(trim(a.NR_PROTOCOLO), 1, 8) = trim(b.BCI_CODPEG)) ");

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql.ToString());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

		internal static bool HasPendenciaUtilizada(int id, EvidaDatabase db) {
			string sql = "SELECT count(1) FROM EV_PROTOCOLO_FATURA A" +
				"		WHERE a.CD_PENDENCIA = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			object o = BaseDAO.ExecuteScalar(db, sql, lstParam);
			if (o == DBNull.Value)
				return false;
			return Convert.ToInt32(o) >= 1;
		}

		internal static void Assumir(int id, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_PROTOCOLO_FATURA SET CD_USUARIO_ANALISTA = :analista, DT_ALTERACAO = LOCALTIMESTAMP " +
				"	WHERE CD_PROTOCOLO_FATURA = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":analista", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

        internal static DataTable ObterDuplicados(EvidaDatabase db)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select distinct a.BCI_CODPEG, listagg(a.BCI_CODLDP || ' - ' || evida.F_TRADUZ_LOCAL_DIGITACAO(a.BCI_CODLDP), ', ') within group (order by a.BCI_CODLDP || ' - ' || evida.F_TRADUZ_LOCAL_DIGITACAO(a.BCI_CODLDP)) BCI_CODLDP ");
            sql.Append(" from VW_PR_PEG a ");
            sql.Append(" inner join VW_PR_PEG b ");
            sql.Append(" on a.BCI_CODPEG = b.BCI_CODPEG ");
            sql.Append(" and a.BCI_MES <> b.BCI_MES ");
            sql.Append(" and a.BCI_YREDOC <> '        ' ");
            sql.Append(" and a.BCI_YVEDOC <> '        ' ");
            sql.Append(" and b.BCI_YREDOC <> '        ' ");
            sql.Append(" and b.BCI_YVEDOC <> '        ' ");
            sql.Append(" group by a.BCI_CODPEG ");

            return BaseDAO.ExecuteDataSet(db, sql.ToString());
        }

	}
}
