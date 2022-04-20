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
	public class CanalGestanteDAO {

		#region Protocolo

		private static int NextProtocolo(EvidaDatabase db) {
			string sql = ("SELECT SQ_EV_CANAL_GESTANTE.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);

			return (int)idSolicitacao;
		}

		internal static DataTable Pesquisar(int? idProtocolo, string cartao, StatusCanalGestante? status, int? controle, Database db) {
            string sql = "SELECT a.*, cb.cd_alternativo, b.BA1_NOMUSR benef_nm_beneficiario " +
                "	FROM EV_CANAL_GESTANTE A, VW_PR_USUARIO b, EV_CANAL_GESTANTE_BENEF cb" +
                "		WHERE trim(b.BA1_CODINT) = trim(a.BA1_CODINT) AND trim(b.BA1_CODEMP) = trim(a.BA1_CODEMP) AND trim(b.BA1_MATRIC) = trim(a.BA1_MATRIC) AND trim(b.BA1_TIPREG) = trim(a.BA1_TIPREG) " +
                "       AND trim(cb.BA1_CODINT) = trim(a.BA1_CODINT) AND trim(cb.BA1_CODEMP) = trim(a.BA1_CODEMP) AND trim(cb.BA1_MATRIC) = trim(a.BA1_MATRIC) AND trim(cb.BA1_TIPREG) = trim(a.BA1_TIPREG)";
            
            List<Parametro> lstParam = new List<Parametro>();

            if (idProtocolo != null)
            {
                sql += " AND A.CD_PROTOCOLO = :id ";
                lstParam.Add(new Parametro(":id", DbType.Int32, idProtocolo));
            }
            if (!string.IsNullOrEmpty(cartao))
            {
                sql += " AND trim(cb.cd_alternativo) = trim(:cartao) ";
                lstParam.Add(new Parametro(":cartao", DbType.String, cartao));
            }
            if (status != null)
            {
                sql += " AND A.ID_SITUACAO = :status ";
                lstParam.Add(new Parametro(":status", DbType.Int32, (int)status));
            }
            if (controle != null)
            {
                if (controle == CanalGestanteVO.CONTROLE_OK)
                {
                    sql += " AND NVL(trunc(a.dt_finalizacao),:data) < trunc(a.dt_solicitacao)+" + CanalGestanteVO.DIAS_ALERTA;
                }
                else if (controle == CanalGestanteVO.CONTROLE_ALERTA)
                {
                    sql += " AND NVL(trunc(a.dt_finalizacao),:data) between trunc(a.dt_solicitacao)+" + CanalGestanteVO.DIAS_ALERTA + " AND trunc(a.dt_solicitacao)+" + CanalGestanteVO.DIAS_CRITICO;
                }
                else
                {
                    sql += " AND NVL(trunc(a.dt_finalizacao),:data) > trunc(a.dt_solicitacao)+" + CanalGestanteVO.DIAS_CRITICO;
                }

                lstParam.Add(new Parametro(":data", DbType.Date, DateTime.Now.Date));
            }	
			
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);
			return dt;
		}

		internal static CanalGestanteVO GetById(int id, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_CANAL_GESTANTE A" +
				"		WHERE a.CD_PROTOCOLO = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			List<CanalGestanteVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowProtocolo, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}
		
		private static CanalGestanteVO FromDataRowProtocolo(DataRow dr) {
			CanalGestanteVO vo = new CanalGestanteVO();
			vo.Id = Convert.ToInt32(dr["CD_PROTOCOLO"]);
			vo.DataSolicitacao = BaseDAO.GetNullableDate(dr, "DT_SOLICITACAO").Value;
			vo.Codint = Convert.ToString(dr["BA1_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA1_CODEMP"]);
            vo.Matric = Convert.ToString(dr["BA1_MATRIC"]);
            vo.Tipreg = Convert.ToString(dr["BA1_TIPREG"]);

			vo.Status = (StatusCanalGestante)Convert.ToInt32(dr["ID_SITUACAO"]);

			vo.Pendencia = Convert.ToString(dr["DS_PENDENCIA"]);
			vo.TipoContato = Convert.ToString(dr["TP_CONTATO"]);

			vo.CodUsuarioFinalizacao = BaseDAO.GetNullableInt(dr, "CD_USUARIO_FINALIZACAO");
			vo.DataFinalizacao = BaseDAO.GetNullableDate(dr, "DT_FINALIZACAO");
			
			vo.Resposta = Convert.ToString(dr["DS_RESPOSTA"]);

            vo.CdCredenciado = Convert.ToString(dr["BAU_CODIGO"]);
            vo.NrSeqAnsProfissional = Convert.ToString(dr["BB0_CODIGO"]);

			return vo;
		}

        internal static List<CanalGestanteVO> ListarProtocolosBenef(string codint, string codemp, string matric, string tipreg, Database db)
        {
            string sql = "SELECT * FROM EV_CANAL_GESTANTE A" +
                    "		WHERE trim(a.BA1_CODINT) = trim(:codint) and trim(a.BA1_CODEMP) = trim(:codemp) and trim(a.BA1_MATRIC) = trim(:matric) and trim(a.BA1_TIPREG) = trim(:tipreg) "; 

			List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":codint", DbType.String, codint));
            lstParam.Add(new Parametro(":codemp", DbType.String, codemp));
            lstParam.Add(new Parametro(":matric", DbType.String, matric));
            lstParam.Add(new Parametro(":tipreg", DbType.String, tipreg));

			List<CanalGestanteVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowProtocolo, lstParam);
			return lst;
		}

        internal static int GerarProtocolo(string codint, string codemp, string matric, string tipreg, string cdCredenciado, string nrSeqProfissional, EvidaDatabase evdb)
        {
            string sql = "INSERT INTO EV_CANAL_GESTANTE (CD_PROTOCOLO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, DT_SOLICITACAO, ID_SITUACAO, BAU_CODIGO, BB0_CODIGO) " +
                    " VALUES (:id, :codint, :codemp, :matric, :tipreg, LOCALTIMESTAMP, :situacao, :cdCredenciado, :nrSeqProf) ";		

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			int id = NextProtocolo(evdb);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":codint", DbType.String, codint);
            db.AddInParameter(dbCommand, ":codemp", DbType.String, codemp);
            db.AddInParameter(dbCommand, ":matric", DbType.String, matric);
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, tipreg);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, (int)StatusCanalGestante.GERANDO);
			db.AddInParameter(dbCommand, ":cdCredenciado", DbType.String, cdCredenciado);
			db.AddInParameter(dbCommand, ":nrSeqProf", DbType.String, nrSeqProfissional);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			return id;
		}

		internal static void AtualizarGerado(CanalGestanteVO vo, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_CANAL_GESTANTE SET ID_SITUACAO = :situacao, DT_FINALIZACAO = LOCALTIMESTAMP " +
				" WHERE CD_PROTOCOLO = :id ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, (int)StatusCanalGestante.FINALIZADO);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static void SolicitarEsclarecimento(int id, string tipoContato, string mensagem, EvidaDatabase evdb) {
			string sql = "UPDATE EV_CANAL_GESTANTE SET ID_SITUACAO = :situacao, DT_FINALIZACAO = null, DS_PENDENCIA = :mensagem, TP_CONTATO = :contato " +
				" WHERE CD_PROTOCOLO = :id ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, (int)StatusCanalGestante.PENDENTE);
			db.AddInParameter(dbCommand, ":mensagem", DbType.String, mensagem);
			db.AddInParameter(dbCommand, ":contato", DbType.String, tipoContato);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void Finalizar(CanalGestanteVO vo, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_CANAL_GESTANTE SET ID_SITUACAO = :situacao, DT_FINALIZACAO = LOCALTIMESTAMP, DS_RESPOSTA = :mensagem, CD_USUARIO_FINALIZACAO = :idUsuario " +
				" WHERE CD_PROTOCOLO = :id ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":situacao", DbType.Int32, (int)StatusCanalGestante.FINALIZADO);
			db.AddInParameter(dbCommand, ":mensagem", DbType.String, vo.Resposta);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, vo.CodUsuarioFinalizacao.Value);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		#endregion

		#region Config

		internal static List<string> ListarUfConfig(int ano, Database db) {
			string sql = "SELECT SG_UF FROM EV_CANAL_GESTANTE_CONFIG_CRED WHERE NR_ANO = :ano " +
				"	UNION " +
				" SELECT BB0_ESTADO FROM EV_CANAL_GESTANTE_CONFIG_PROF WHERE NR_ANO = :ano " +
				" ORDER BY 1";
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":ano", DbType.Int32, ano));

			List<KeyValuePair<string,string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<string>("sg_uf", "sg_uf"), lstParam);
			if (lst != null)
				return lst.Select(x => x.Key).ToList();
			return null;
		}

		internal static CanalGestanteConfigVO GetConfig(int ano, Database db) {
			string sql = "SELECT * FROM EV_CANAL_GESTANTE_CONFIG A" +
				"		WHERE a.NR_ANO = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, ano));

			List<CanalGestanteConfigVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowConfig, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static CanalGestanteConfigVO FromDataRowConfig(DataRow dr) {
			CanalGestanteConfigVO vo = new CanalGestanteConfigVO();
			vo.Ano = Convert.ToInt32(dr["NR_ANO"]);
			vo.PartoNormal = BaseDAO.GetNullableDecimal(dr, "PR_PARTO_NORMAL").Value;
			vo.DataAlteracao = BaseDAO.GetNullableDate(dr, "DT_ALTERACAO").Value;

			vo.CodUsuarioAlteracao = BaseDAO.GetNullableInt(dr, "CD_USUARIO_ALTERACAO").Value;
			return vo;
		}

		internal static void SalvarConfig(CanalGestanteConfigVO vo, bool novo, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (novo) {
				dbCommand = CreateInsert(vo, transaction, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.Ano);
			db.AddInParameter(dbCommand, ":partoNormal", DbType.Decimal, vo.PartoNormal);
			db.AddInParameter(dbCommand, ":cdUsuario", DbType.Int32, vo.CodUsuarioAlteracao);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static DbCommand CreateInsert(CanalGestanteConfigVO vo, DbTransaction transaction, Database db) {
			string sql = "INSERT INTO EV_CANAL_GESTANTE_CONFIG (NR_ANO, PR_PARTO_NORMAL, PR_PARTO_CESAREA, CD_USUARIO_ALTERACAO, DT_ALTERACAO) " +
				" VALUES (:ano, :partoNormal, 100-:partoNormal, :cdUsuario, LOCALTIMESTAMP) "; ;
			
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

		private static DbCommand CreateUpdate(CanalGestanteConfigVO vo, Database db) {
			string sql = "UPDATE EV_CANAL_GESTANTE_CONFIG SET PR_PARTO_NORMAL = :partoNormal, PR_PARTO_CESAREA = 100-:partoNormal, " +
				"	CD_USUARIO_ALTERACAO = :cdUsuario, DT_ALTERACAO = LOCALTIMESTAMP " +
				"	WHERE NR_ANO = :ano";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

		internal static List<CanalGestanteConfigCredVO> GetConfigCred(int ano, Database db) {
			string sql = "SELECT * FROM EV_CANAL_GESTANTE_CONFIG_CRED A" +
				"		WHERE a.NR_ANO = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, ano));

			List<CanalGestanteConfigCredVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowConfigCred, lstParam);
			return lst;
		}

		internal static List<VO.Protheus.PRedeAtendimentoVO> ListarInfoCredenciadoConfig(int ano, Database db) {
            string sql = "SELECT v.*, null as nr_formula, null as tp_dias FROM VW_PR_REDE_ATENDIMENTO V WHERE trim(BAU_CODIGO) IN " +
                " (SELECT trim(BAU_CODIGO) FROM EV_CANAL_GESTANTE_CONFIG_CRED A WHERE A.NR_ANO = :ano) ";		

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":ano", DbType.Int32, ano));
            List<VO.Protheus.PRedeAtendimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, DAO.Protheus.PRedeAtendimentoDAO.FromDataRowRedeAtend, lstParams);

			return lst;
		}

		private static CanalGestanteConfigCredVO FromDataRowConfigCred(DataRow dr) {
			CanalGestanteConfigCredVO vo = new CanalGestanteConfigCredVO();
			vo.Ano = Convert.ToInt32(dr["NR_ANO"]);
			vo.CodCredenciado = Convert.ToString(dr["BAU_CODIGO"]);
			vo.Uf = Convert.ToString(dr["SG_UF"]);
			vo.PartoNormal = BaseDAO.GetNullableDecimal(dr, "PR_PARTO_NORMAL").Value;
			return vo;
		}

		internal static void SalvarConfig(int ano, IEnumerable<CanalGestanteConfigCredVO> lst, bool novo, DbTransaction transaction, Database db) {
			DbCommand dbCommandIns = CreateInsertCred(db);
			DbCommand dbCommandDel = CreateDeleteCredAll(db);
			if (!novo) {
				db.AddInParameter(dbCommandDel, ":ano", DbType.Int32, ano);
				BaseDAO.ExecuteNonQuery(dbCommandDel, transaction, db);
			}

			if (lst != null && lst.Count() > 0) {
				db.AddInParameter(dbCommandIns, ":ano", DbType.Int32, ano);
				db.AddInParameter(dbCommandIns, ":credenciado", DbType.String);
				db.AddInParameter(dbCommandIns, ":uf", DbType.String);
				db.AddInParameter(dbCommandIns, ":partoNormal", DbType.Decimal);

				foreach (CanalGestanteConfigCredVO vo in lst) {
					db.SetParameterValue(dbCommandIns, ":credenciado", vo.CodCredenciado.Trim());
                    db.SetParameterValue(dbCommandIns, ":uf", vo.Uf.Trim());
					db.SetParameterValue(dbCommandIns, ":partoNormal", vo.PartoNormal);

					BaseDAO.ExecuteNonQuery(dbCommandIns, transaction, db);
				}
			}
		}

		private static DbCommand CreateInsertCred(Database db) {
            string sql = "INSERT INTO EV_CANAL_GESTANTE_CONFIG_CRED (NR_ANO, BAU_CODIGO, SG_UF, PR_PARTO_NORMAL) " +
                " VALUES (:ano, :credenciado, :uf, :partoNormal) ";		

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

		private static DbCommand CreateDeleteCredAll(Database db) {
			string sql = "DELETE FROM EV_CANAL_GESTANTE_CONFIG_CRED " +
				"	WHERE NR_ANO = :ano";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

		internal static List<CanalGestanteConfigProfVO> GetConfigProf(int ano, Database db) {
			string sql = "SELECT * FROM EV_CANAL_GESTANTE_CONFIG_PROF A" +
				"		WHERE a.NR_ANO = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, ano));

			List<CanalGestanteConfigProfVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowConfigProf, lstParam);
			return lst;
		}

		internal static List<VO.Protheus.PProfissionalSaudeVO> ListarInfoProfissionalConfig(int ano, Database db) {
            string sql = "SELECT * FROM VW_PR_PROFISSIONAL_SAUDE P WHERE trim(BB0_CODIGO) IN " +
                " (SELECT trim(BB0_CODIGO) FROM EV_CANAL_GESTANTE_CONFIG_PROF A WHERE A.NR_ANO = :ano) ";		

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":ano", DbType.Int32, ano));
            List<VO.Protheus.PProfissionalSaudeVO> lst = BaseDAO.ExecuteDataSet(db, sql, VO.Protheus.PProfissionalSaudeVO.FromDataRow, lstParams);

			return lst;
		}

		private static CanalGestanteConfigProfVO FromDataRowConfigProf(DataRow dr) {
			CanalGestanteConfigProfVO vo = new CanalGestanteConfigProfVO();
			vo.Ano = Convert.ToInt32(dr["NR_ANO"]);
			vo.Codigo = Convert.ToString(dr["BB0_CODIGO"]);
            vo.Numcr = Convert.ToString(dr["BB0_NUMCR"]);
            vo.Codsig = Convert.ToString(dr["BB0_CODSIG"]);
            vo.Estado = Convert.ToString(dr["BB0_ESTADO"]);
			vo.PartoNormal = BaseDAO.GetNullableDecimal(dr, "PR_PARTO_NORMAL").Value;
			return vo;
		}

		internal static void SalvarConfig(int ano, IEnumerable<CanalGestanteConfigProfVO> lst, bool novo, DbTransaction transaction, Database db) {
			DbCommand dbCommandIns = CreateInsertProf(db);
			DbCommand dbCommandDel = CreateDeleteProfAll(db);
			if (!novo) {
				db.AddInParameter(dbCommandDel, ":ano", DbType.Int32, ano);
				BaseDAO.ExecuteNonQuery(dbCommandDel, transaction, db);
			}

			if (lst != null && lst.Count() > 0) {
				db.AddInParameter(dbCommandIns, ":ano", DbType.Int32, ano);
				db.AddInParameter(dbCommandIns, ":prof", DbType.String);
				db.AddInParameter(dbCommandIns, ":nroConselho", DbType.String);
				db.AddInParameter(dbCommandIns, ":codConselho", DbType.String);
				db.AddInParameter(dbCommandIns, ":uf", DbType.String);
				db.AddInParameter(dbCommandIns, ":partoNormal", DbType.Decimal);

				foreach (CanalGestanteConfigProfVO vo in lst) {
					db.SetParameterValue(dbCommandIns, ":prof", vo.Codigo);
					db.SetParameterValue(dbCommandIns, ":nroConselho", vo.Numcr);
                    db.SetParameterValue(dbCommandIns, ":codConselho", vo.Codsig);
                    db.SetParameterValue(dbCommandIns, ":uf", vo.Estado);
					db.SetParameterValue(dbCommandIns, ":partoNormal", vo.PartoNormal);

					BaseDAO.ExecuteNonQuery(dbCommandIns, transaction, db);
				}
			}
		}

		private static DbCommand CreateInsertProf(Database db) {
            string sql = "INSERT INTO EV_CANAL_GESTANTE_CONFIG_PROF (NR_ANO, BB0_CODIGO, BB0_NUMCR, BB0_CODSIG, BB0_ESTADO, PR_PARTO_NORMAL) " +
				" VALUES (:ano, :prof, :nroConselho, :codConselho, :uf, :partoNormal) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

		private static DbCommand CreateDeleteProfAll(Database db) {
			string sql = "DELETE FROM EV_CANAL_GESTANTE_CONFIG_PROF " +
				"	WHERE NR_ANO = :ano ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

		#endregion

		#region Beneficiario

		private static CanalGestanteBenefVO FromDataRowBenef(DataRow dr) {
			CanalGestanteBenefVO vo = new CanalGestanteBenefVO();
			vo.Codint = Convert.ToString(dr["BA1_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA1_CODEMP"]);
            vo.Matric = Convert.ToString(dr["BA1_MATRIC"]);
            vo.Tipreg = Convert.ToString(dr["BA1_TIPREG"]);
			vo.CodAlternativo = Convert.ToString(dr["CD_ALTERNATIVO"]);
			vo.DataDownloadCartao = BaseDAO.GetNullableDate(dr, "DT_DOWNLOAD_CARTAO");
			vo.DataDownloadInfo = BaseDAO.GetNullableDate(dr, "DT_DOWNLOAD_INFO");
			vo.DataDownloadPartograma = BaseDAO.GetNullableDate(dr, "DT_DOWNLOAD_PARTOGRAMA");
			vo.DataDownloadCartao = BaseDAO.GetNullableDate(dr, "DT_DOWNLOAD_CARTAO");

			vo.DataNascimento = BaseDAO.GetNullableDate(dr, "DT_NASCIMENTO").Value;
			vo.Email = Convert.ToString(dr["ds_email"]);
			vo.PlanoVinculado = Convert.ToString(dr["BI3_CODIGO"]);
			vo.Telefone = Convert.ToString(dr["ds_telefone"]);
			
			return vo;
		}

        internal static CanalGestanteBenefVO GetInfoBenef(string codint, string codemp, string matric, string tipreg, EvidaDatabase db)
        {
			string sql = "SELECT * FROM EV_CANAL_GESTANTE_BENEF A" +
                "		WHERE trim(a.BA1_CODINT) = trim(:codint) and trim(a.BA1_CODEMP) = trim(:codemp) and trim(a.BA1_MATRIC) = trim(:matric) and trim(a.BA1_TIPREG) = trim(:tipreg) ";

			List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":codint", DbType.String, codint));
            lstParam.Add(new Parametro(":codemp", DbType.String, codemp));
            lstParam.Add(new Parametro(":matric", DbType.String, matric));
            lstParam.Add(new Parametro(":tipreg", DbType.String, tipreg));

			List<CanalGestanteBenefVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowBenef, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static void SalvarBenef(CanalGestanteBenefVO vo, bool novo, EvidaDatabase evdb) {
			DbCommand dbCommand = null;
			Database db = evdb.Database;
			if (novo) {
				dbCommand = CreateInsert(vo, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Tipreg.Trim());
			db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email);
			db.AddInParameter(dbCommand, ":telefone", DbType.String, vo.Telefone);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static DbCommand CreateInsert(CanalGestanteBenefVO vo, Database db) {
            string sql = "INSERT INTO EV_CANAL_GESTANTE_BENEF (BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, CD_ALTERNATIVO, DT_NASCIMENTO, BI3_CODIGO, DS_EMAIL, " +
                " DS_TELEFONE) " +
                " VALUES (:codint, :codemp, :matric, :tipreg, :cdAlternativo, :nascimento, :plano, :email, :telefone) ";	

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":cdAlternativo", DbType.String, vo.CodAlternativo);
			db.AddInParameter(dbCommand, ":nascimento", DbType.Date, vo.DataNascimento);
            db.AddInParameter(dbCommand, ":plano", DbType.String, vo.PlanoVinculado.Trim());

			return dbCommand;
		}

		private static DbCommand CreateUpdate(CanalGestanteBenefVO vo, Database db) {
            string sql = "UPDATE EV_CANAL_GESTANTE_BENEF SET DS_EMAIL = :email, DS_TELEFONE = :telefone " +
                "	WHERE trim(BA1_CODINT) = trim(:codint) and trim(BA1_CODEMP) = trim(:codemp) and trim(BA1_MATRIC) = trim(:matric) and trim(BA1_TIPREG) = trim(:tipreg) ";		

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			return dbCommand;
		}

        internal static void MarcarDownload(string codint, string codemp, string matric, string tipreg, string tipo, EvidaDatabase evdb)
        {
			string campo = "DT_DOWNLOAD_INFO";
			if (CanalGestanteBenefVO.CARTA_INFO.Equals(tipo)) {
				campo = "DT_DOWNLOAD_INFO";
			} else if (CanalGestanteBenefVO.CARTAO_GES.Equals(tipo)) {
				campo = "DT_DOWNLOAD_CARTAO";
			} else if (CanalGestanteBenefVO.PARTOGRAMA.Equals(tipo)) {
				campo = "DT_DOWNLOAD_PARTOGRAMA";
			}
			string sql = "UPDATE EV_CANAL_GESTANTE_BENEF SET " + campo + " = LOCALTIMESTAMP " +
                "	WHERE trim(BA1_CODINT) = trim(:codint) and trim(BA1_CODEMP) = trim(:codemp) and trim(BA1_MATRIC) = trim(:matric) and trim(BA1_TIPREG) = trim(:tipreg) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":codint", DbType.String, codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, tipreg.Trim());
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		#endregion

	}
}

