using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class ExclusaoDAO {

		private static int NextId(EvidaDatabase db) {
			string sql = ("SELECT SQ_EV_SOLICITACAO_EXCLUSAO.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);

			return (int)idSolicitacao;
		}

		private static ExclusaoVO FromDataRow(DataRow dr) {
			ExclusaoVO vo = new ExclusaoVO();
			vo.CodSolicitacao = Convert.ToInt32(dr["CD_SOLICITACAO"]);
			vo.Codint = Convert.ToString(dr["BA3_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA3_CODEMP"]);
            vo.Matric = Convert.ToString(dr["BA3_MATRIC"]);
			vo.DataAlteracao = dr["DT_ALTERACAO"] != DBNull.Value ? Convert.ToDateTime(dr["DT_ALTERACAO"]) : new DateTime?();
			vo.DataCriacao = Convert.ToDateTime(dr["DT_CRIACAO"]);
			vo.Local = Convert.ToString(dr["DS_LOCAL"]);
			vo.Status = (StatusExclusao) Convert.ToInt32(dr["CD_STATUS"]);
			vo.CodUsuarioAlteracao = dr["CD_USUARIO_ALTERACAO"] != DBNull.Value ? Convert.ToInt32(dr["CD_USUARIO_ALTERACAO"]) : new int?();
			vo.MotivoCancelamento = Convert.ToString(dr["DS_MOTIVO_CANCELAMENTO"]);
			vo.NomeBeneficiarios = Convert.ToString(dr["ds_beneficiario"]);
			vo.Observacao = Convert.ToString(dr["ds_obs"]);
			return vo;
		}

        internal static DataTable BuscarExclusao(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "SELECT cd_solicitacao, BA3_CODINT, BA3_CODEMP, BA3_MATRIC, DS_LOCAL, DT_CRIACAO, cd_STATUS, DT_ALTERACAO, CD_USUARIO_ALTERACAO, " +
                "	ds_motivo_cancelamento, ds_beneficiario, ds_obs, ds_protocolo " +
                " FROM EV_FORM_EXCLUSAO sol " +
                " WHERE trim(sol.BA3_CODINT) = trim(:codint) AND trim(sol.BA3_CODEMP) = trim(:codemp) AND trim(sol.BA3_MATRIC) = trim(:matric) " +
                " ORDER BY cd_solicitacao DESC ";

			List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void Salvar(ExclusaoVO vo, List<ExclusaoBenefVO> lst, EvidaDatabase evdb) {

            string sql = "INSERT INTO EV_FORM_EXCLUSAO (CD_SOLICITACAO, BA3_CODINT, BA3_CODEMP, BA3_MATRIC, DS_LOCAL, DT_CRIACAO, CD_STATUS, DS_BENEFICIARIO, CD_USUARIO_ALTERACAO, DS_OBS, DS_PROTOCOLO) " +
                " VALUES (:id, :codint, :codemp, :matric, :local, LOCALTIMESTAMP, :status, :nomes, :cdUsuario, :obs, :protocolo)";

			vo.CodSolicitacao = NextId(evdb);

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));
            lstParams.Add(new Parametro(":codint", DbType.String, vo.Codint.Trim()));
            lstParams.Add(new Parametro(":codemp", DbType.String, vo.Codemp.Trim()));
            lstParams.Add(new Parametro(":matric", DbType.String, vo.Matric.Trim()));
			lstParams.Add(new Parametro(":local", DbType.String, vo.Local));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)vo.Status));			
			lstParams.Add(new Parametro(":nomes", DbType.String, vo.NomeBeneficiarios));
						
			lstParams.Add(new Parametro(":cdUsuario", DbType.Int32, vo.CodUsuarioAlteracao));
			lstParams.Add(new Parametro(":obs", DbType.AnsiString, vo.Observacao));
            lstParams.Add(new Parametro(":protocolo", DbType.String, vo.Protocolo));


            BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);

            sql = "INSERT INTO EV_FORM_EXCLUSAO_BENEF (CD_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, IN_TITULAR, IN_DEP_FAMILIA, BI3_CODIGO, DS_PROTOCOLO) " +
                "	VALUES (:id, :codint, :codemp, :matric, :tipreg, :titular, :depFamilia, :cdPlano, :protocolo) ";

			List<Parametro> lstParamBenef = new List<Parametro>();
			lstParamBenef.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));
			lstParamBenef.Add(new ParametroVar(":codint", DbType.String));
            lstParamBenef.Add(new ParametroVar(":codemp", DbType.String));
            lstParamBenef.Add(new ParametroVar(":matric", DbType.String));
            lstParamBenef.Add(new ParametroVar(":tipreg", DbType.String));
			lstParamBenef.Add(new ParametroVar(":titular", DbType.Int32));
			lstParamBenef.Add(new ParametroVar(":depFamilia", DbType.Int32));			
			lstParamBenef.Add(new ParametroVar(":cdPlano", DbType.String));
            lstParamBenef.Add(new ParametroVar(":protocolo", DbType.String));


            List<ParametroVarRow> lstrows = new List<ParametroVarRow>();
			foreach (ExclusaoBenefVO bVO in lst) {
				ParametroVarRow varRow = new ParametroVarRow(lstParamBenef);
                varRow[":codint"] = bVO.Codint.Trim();
                varRow[":codemp"] = bVO.Codemp.Trim();
                varRow[":matric"] = bVO.Matric.Trim();
                varRow[":tipreg"] = bVO.Tipreg.Trim();
				varRow[":titular"] = bVO.IsTitular ? 1 : 0;
				varRow[":depFamilia"] = bVO.IsDepFamilia ? 1 : 0;
                varRow[":cdPlano"] = bVO.CodPlano.Trim();
                varRow[":protocolo"] = bVO.Protocolo.Trim(); // é aqui que quebra

                lstrows.Add(varRow);
			}		
			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParamBenef, lstrows, evdb);
		}

		internal static ExclusaoVO GetById(int idSolicitacao, EvidaDatabase db) {

            string sql = "SELECT CD_SOLICITACAO, BA3_CODINT, BA3_CODEMP, BA3_MATRIC, DS_LOCAL, DT_CRIACAO, CD_STATUS, DT_ALTERACAO, CD_USUARIO_ALTERACAO, " +
                "	ds_motivo_cancelamento, ds_beneficiario, ds_obs, ds_protocolo " +
                " FROM  EV_FORM_EXCLUSAO " +
                " WHERE CD_SOLICITACAO = :id";  

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = idSolicitacao });

			List<ExclusaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			if (lst != null && lst.Count > 0) {
				return lst[0];
			}
			return null;
		}

		internal static DataTable BuscarBeneficiarios(int idSolicitacao, EvidaDatabase db) {

            string sql = " SELECT func.BA3_CODINT, func.BA3_CODEMP, func.BA3_MATRIC, benef.BA1_CODINT, benef.BA1_CODEMP, benef.BA1_MATRIC, benef.BA1_TIPREG, benef.BA1_MATVID, benef.BA1_NOMUSR, " +
                "		benef.BA1_DATNAS, " +
                "		parentesco.BRP_CODIGO, parentesco.BRP_DESCRI, p.BI3_DESCRI, benef.BA1_MATANT, benef.BA1_DTVLCR, " +
                "		sol.cd_solicitacao, sol.dt_criacao, solben.BI3_CODIGO, solben.in_titular, solben.in_dep_familia, sol.ds_protocolo " +
                " FROM VW_PR_USUARIO benef, " +
                "		VW_PR_GRAU_PARENTESCO parentesco, " +
                "		VW_PR_PRODUTO_SAUDE p, VW_PR_FAMILIA func, " +
                "		EV_FORM_EXCLUSAO sol, EV_FORM_EXCLUSAO_BENEF solben " +
                " WHERE sol.cd_solicitacao = solben.cd_solicitacao " +
                "		AND trim(sol.BA3_CODINT) = trim(func.BA3_CODINT) and trim(sol.BA3_CODEMP) = trim(func.BA3_CODEMP) and trim(sol.BA3_MATRIC) = trim(func.BA3_MATRIC) " +
                "		AND trim(solben.BA1_CODINT) = trim(benef.BA1_CODINT) " +
                "		AND trim(solben.BA1_CODEMP) = trim(benef.BA1_CODEMP) " +
                "		AND trim(solben.BA1_MATRIC) = trim(benef.BA1_MATRIC) " +
                "		AND trim(solben.BA1_TIPREG) = trim(benef.BA1_TIPREG) " +
                "		and trim(p.BI3_CODIGO) = trim(solben.BI3_CODIGO) " +
                "		and parentesco.BRP_CODIGO (+) = benef.BA1_GRAUPA " +
                "		and sol.cd_solicitacao = :id " +
                " ORDER BY nvl(benef.BA1_MATVID, '')";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = idSolicitacao });

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void Aprovar(int cdProtocolo, int idUsuario, EvidaDatabase evdb) {
			string sql = "UPDATE EV_FORM_EXCLUSAO SET CD_STATUS = :status, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, cdProtocolo);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusExclusao.APROVADO);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, idUsuario);
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static DataTable Pesquisar(long? matricula, int? cdProtocolo, StatusExclusao? status, EvidaDatabase db) {

            string sql = "SELECT cd_solicitacao, sol.BA3_CODINT, sol.BA3_CODEMP, sol.BA3_MATRIC, f.BA3_MATEMP, DS_LOCAL, DT_CRIACAO, cd_STATUS, DT_ALTERACAO, CD_USUARIO_ALTERACAO, " +
                      "	ds_motivo_cancelamento, ds_beneficiario, ds_obs, ds_protocolo " +
                      " FROM EV_FORM_EXCLUSAO sol, VW_PR_FAMILIA f " +
                      " WHERE 1 = 1 " +
                      " AND upper(trim(sol.BA3_CODINT)) = upper(trim(f.BA3_CODINT)) and upper(trim(sol.BA3_CODEMP)) = upper(trim(f.BA3_CODEMP)) and upper(trim(sol.BA3_MATRIC)) = upper(trim(f.BA3_MATRIC)) ";

			List<Parametro> lstParams = new List<Parametro>();
            if (matricula != null)
            {
                sql += " AND trim(f.BA3_MATEMP) = trim(:matemp) ";
                lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matricula });
            }
			if (cdProtocolo != null) {
				sql += " AND sol.cd_solicitacao = :cdProtocolo ";
				lstParams.Add(new Parametro() { Name = ":cdProtocolo", Tipo = DbType.Int32, Value = cdProtocolo });
			}
			if (status != null) {
				sql += " AND sol.cd_STATUS = :status ";
				lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = (int)status });
			}
			sql += " ORDER BY cd_solicitacao DESC ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void Cancelar(int cdProtocolo, string motivo, int idUsuario, EvidaDatabase evdb) {
			string sql = "UPDATE EV_FORM_EXCLUSAO SET CD_STATUS = :status, DS_MOTIVO_CANCELAMENTO = :motivo, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, cdProtocolo);
			db.AddInParameter(dbCommand, ":motivo", DbType.String, motivo);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusExclusao.NEGADO);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, idUsuario);
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void AguardarDocumentacao(int cdProtocolo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_FORM_EXCLUSAO SET CD_STATUS = :status, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusExclusao.AGUARDANDO_DOCUMENTACAO));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}
	}
}
