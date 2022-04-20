using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class DeclaracaoUniversitarioDAO {

		private static int NextId(EvidaDatabase db) {
			string sql = ("SELECT SQ_EV_DECLARACAO_UNIVERSITARIO.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);

			return (int)idSolicitacao;
		}

		internal static void Salvar(DeclaracaoUniversitarioVO vo, EvidaDatabase evdb) {

            string sql = "INSERT INTO EV_DECLARACAO_UNIVERSITARIO (CD_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BI3_CODIGO_BENEF, NM_ARQUIVO, " +
                " DT_CRIACAO, CD_STATUS) " +
                " VALUES (:id, :codint, :codemp, :matric, :tipreg, :cdPlanoBenef, :nmArquivo, LOCALTIMESTAMP, :status)";

			vo.CodSolicitacao = NextId(evdb);

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CodSolicitacao);
            db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Tipreg.Trim());
            db.AddInParameter(dbCommand, ":cdPlanoBenef", DbType.String, vo.CodPlanoBeneficiario.Trim());
			db.AddInParameter(dbCommand, ":nmArquivo", DbType.String, vo.NomeArquivo);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusDeclaracaoUniversitario.PENDENTE);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			
		}

		internal static DeclaracaoUniversitarioVO GetById(int id, EvidaDatabase db) {

            string sql = "SELECT CD_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BI3_CODIGO_BENEF, NM_ARQUIVO, " +
                "	DT_CRIACAO, CD_STATUS, DT_ALTERACAO, CD_USUARIO_ALTERACAO, DS_MOTIVO_CANCELAMENTO " +
                " FROM  EV_DECLARACAO_UNIVERSITARIO " +
                " WHERE CD_SOLICITACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

			List<DeclaracaoUniversitarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			if (lst != null && lst.Count > 0) {
				return lst[0];
			}
			return null;
		}

		private static DeclaracaoUniversitarioVO FromDataRow(DataRow dr) {
			DeclaracaoUniversitarioVO vo = new DeclaracaoUniversitarioVO();
			vo.CodSolicitacao = Convert.ToInt32(dr["CD_SOLICITACAO"]);
            vo.Codint = dr.Field<string>("BA1_CODINT");
            vo.Codemp = dr.Field<string>("BA1_CODEMP");
            vo.Matric = dr.Field<string>("BA1_MATRIC");
            vo.Tipreg = dr.Field<string>("BA1_TIPREG");
            vo.CodPlanoBeneficiario = dr.Field<String>("BI3_CODIGO_BENEF");
			vo.NomeArquivo = dr.Field<String>("NM_ARQUIVO");
			vo.DataCriacao = dr.Field<DateTime>("DT_CRIACAO");
			vo.Status = (StatusDeclaracaoUniversitario) dr.Field<decimal>("CD_STATUS");
			vo.CodUsuarioAlteracao = (int?)dr.Field<decimal?>("CD_USUARIO_ALTERACAO");
			vo.DataAlteracao = dr.Field<DateTime?>("DT_ALTERACAO");
			vo.MotivoCancelamento = dr.Field<string>("DS_MOTIVO_CANCELAMENTO");
			return vo;
		}

		internal static DataTable PesquisarDeclaracoes(int? cdProtocolo, string cdFuncionario, DateTime? dtInicial, DateTime? dtFinal, StatusDeclaracaoUniversitario? status, EvidaDatabase db) {
            string sql = "SELECT SOL.cd_solicitacao, SOL.BA1_CODINT, SOL.BA1_CODEMP, SOL.BA1_MATRIC, SOL.BA1_TIPREG, SOL.dt_criacao, SOL.cd_status, SOL.DT_ALTERACAO, SOL.CD_USUARIO_ALTERACAO " +
                " , dep.BA1_NOMUSR, sol.ds_motivo_cancelamento " +
                " FROM EV_DECLARACAO_UNIVERSITARIO sol, VW_PR_FAMILIA func, VW_PR_USUARIO dep " +
                " WHERE trim(sol.BA1_CODINT) = trim(func.BA3_CODINT) and trim(sol.BA1_CODEMP) = trim(func.BA3_CODEMP) and trim(sol.BA1_MATRIC) = trim(func.BA3_MATRIC) " +
                "	AND trim(sol.BA1_CODINT) = trim(dep.BA1_CODINT) " +
                "	AND trim(sol.BA1_CODEMP) = trim(dep.BA1_CODEMP) " +
                "	AND trim(sol.BA1_MATRIC) = trim(dep.BA1_MATRIC) " +
                "	AND trim(sol.BA1_TIPREG) = trim(dep.BA1_TIPREG) ";

            List<Parametro> lstParams = new List<Parametro>();
            if (cdProtocolo != null)
            {
                lstParams.Add(new Parametro() { Name = ":cdProtocolo", Tipo = DbType.Int32, Value = cdProtocolo.Value });
                sql += " AND sol.cd_solicitacao = :cdProtocolo ";
            }
            if (!string.IsNullOrEmpty(cdFuncionario))
            {
                lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = cdFuncionario });
                sql += " AND trim(func.BA3_MATEMP) = trim(:matemp) ";
            }
            if (status != null)
            {
                lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = (int)status.Value });
                sql += " AND sol.cd_STATUS = :status ";
            }
            if (dtInicial != null)
            {
                lstParams.Add(new Parametro() { Name = ":dtInicial", Tipo = DbType.Date, Value = dtInicial.Value });
                sql += " AND sol.DT_CRIACAO >= :dtInicial ";
            }
            if (dtFinal != null)
            {
                lstParams.Add(new Parametro() { Name = ":dtFinal", Tipo = DbType.Date, Value = dtFinal.Value });
                sql += " AND sol.DT_CRIACAO <= :dtFinal ";
            }
            sql += " ORDER BY cd_solicitacao DESC ";   
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void Cancelar(int cdProtocolo, string motivo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_DECLARACAO_UNIVERSITARIO SET CD_STATUS = :status, DS_MOTIVO_CANCELAMENTO = :motivo, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			lstParams.Add(new Parametro(":motivo", DbType.String, motivo));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusDeclaracaoUniversitario.RECUSADO));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static void Aprovar(int cdProtocolo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_DECLARACAO_UNIVERSITARIO SET CD_STATUS = :status, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusDeclaracaoUniversitario.APROVADO));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}
	}
}
