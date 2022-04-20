using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.DAO {
	internal class AutorizacaoProvisoriaDAO {

		#region Plantao Social

		private static int GetNextPlantaoId(DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT NVL(MAX(CD_PLANTAO_SOCIAL),0) FROM EV_PLANTAO_SOCIAL_MUNICIPIO ");

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (int)idSolicitacao+1;
		}

		internal static PlantaoSocialLocalVO GetPlantaoById(int id, Database db) {
			string sql = "SELECT L.* FROM EV_PLANTAO_SOCIAL_MUNICIPIO L WHERE CD_PLANTAO_SOCIAL = " + id;
			List<PlantaoSocialLocalVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowPlantaoSocialLocal);
			if (lst == null || lst.Count == 0)
				return null;
			return lst[0];
		}

		public static List<PlantaoSocialLocalVO> ListPlantaoSocialLocal(Database db) {
            string sql = "SELECT L.* FROM EV_PLANTAO_SOCIAL_MUNICIPIO L" +
                " ORDER BY L.BID_EST, L.BID_DESCRI ";    
			List<PlantaoSocialLocalVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowPlantaoSocialLocal);
			return lst;
		}

		private static PlantaoSocialLocalVO FromDataRowPlantaoSocialLocal(DataRow dr) {
			PlantaoSocialLocalVO vo = new PlantaoSocialLocalVO();
			vo.Id = Convert.ToInt32(dr["cd_plantao_social"]);
            vo.Uf = dr.Field<string>("BID_EST");
            vo.Cidade = dr.Field<string>("BID_DESCRI");
            vo.CodMunicipio = dr.Field<string>("BID_CODMUN");
			vo.Telefone = dr.Field<string>("ds_telefone");
			return vo;
		}

		internal static void Excluir(PlantaoSocialLocalVO vo, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_PLANTAO_SOCIAL_MUNICIPIO WHERE cd_plantao_social = :id ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static void Salvar(PlantaoSocialLocalVO vo, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (vo.Id == 0) {
				dbCommand = CreateInsert(vo, db);
				vo.Id = GetNextPlantaoId(transaction, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":telefone", DbType.String, vo.Telefone);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static DbCommand CreateInsert(PlantaoSocialLocalVO vo, Database db) {
            string sql = "INSERT INTO EV_PLANTAO_SOCIAL_MUNICIPIO (CD_PLANTAO_SOCIAL, BID_EST, BID_CODMUN, BID_DESCRI, DS_TELEFONE) " +
                "	VALUES (:id, :uf, :idMunicipio, :nome, :telefone) "; 

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":uf", DbType.String, vo.Uf.Trim());
			db.AddInParameter(dbCommand, ":idMunicipio", DbType.String, vo.CodMunicipio.Trim());
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Cidade.Trim());
			return dbCommand;
		}

		private static DbCommand CreateUpdate(PlantaoSocialLocalVO vo, Database db) {
			string sql = "UPDATE EV_PLANTAO_SOCIAL_MUNICIPIO SET DS_TELEFONE = :telefone " +
				"	WHERE CD_PLANTAO_SOCIAL = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static bool IsLocalUtilizado(PlantaoSocialLocalVO vo, Database db) {
			string sql = "SELECT COUNT(1) FROM EV_AUTORIZACAO_PROVISORIA A " +
				" WHERE A.CD_PLANTAO_SOCIAL = :idPlantao";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idPlantao", Tipo = DbType.Int32, Value = vo.Id });

			int count = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
			return count > 0;
		}

		internal static bool ExistePlantao(string uf, string codMunicipio, Database db) {
            string sql = "SELECT COUNT(1) FROM EV_PLANTAO_SOCIAL_MUNICIPIO A " +
                " WHERE A.BID_EST = :uf AND trim(a.BID_CODMUN) = trim(:idMunicipio) ";   

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });
			lstParams.Add(new Parametro() { Name = ":idMunicipio", Tipo = DbType.String, Value = codMunicipio });

			int count = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
			return count > 0;
		}

		#endregion

		internal static AutorizacaoProvisoriaVO GetById(int id, Database db) {
            string sql = "SELECT A.*, L.BID_EST, L.DS_TELEFONE, L.BID_DESCRI, L.BID_CODMUN, benef.BA1_CODINT, benef.BA1_CODEMP, benef.BA1_MATRIC, benef.BA1_TIPREG " +
                " FROM EV_AUTORIZACAO_PROVISORIA A, EV_PLANTAO_SOCIAL_MUNICIPIO L, VW_PR_USUARIO benef " +
                " WHERE L.cd_plantao_social = a.cd_plantao_social " +
                "	AND trim(benef.BA1_CODINT) = trim(A.BA1_CODINT) " +
                "	AND trim(benef.BA1_CODEMP) = trim(A.BA1_CODEMP) " +
                "	AND trim(benef.BA1_MATRIC) = trim(A.BA1_MATRIC) " +
                "	AND trim(benef.BA1_TIPREG) = trim(A.BA1_TIPREG) " +
                "	AND A.CD_SOLICITACAO = :id";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

			List<AutorizacaoProvisoriaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst == null || lst.Count == 0)
				return null;
			return lst[0];
		}

		private static AutorizacaoProvisoriaVO FromDataRow(DataRow dr) {
			AutorizacaoProvisoriaVO vo = new AutorizacaoProvisoriaVO();
			vo.Codint = Convert.ToString(dr["BA1_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA1_CODEMP"]);
            vo.Matric = Convert.ToString(dr["BA1_MATRIC"]);
            vo.Tipreg = Convert.ToString(dr["BA1_TIPREG"]);
			vo.CodSolicitacao = Convert.ToInt32(dr["cd_solicitacao"]);
			vo.Status = (StatusAutorizacaoProvisoria)Convert.ToInt32(dr["cd_status"]);

			vo.DataCriacao = Convert.ToDateTime(dr["dt_criacao"]);
			vo.FimVigencia = Convert.ToDateTime(dr["dt_fim_vigencia"]);
			vo.CodUsuarioAlteracao = BaseDAO.GetNullableInt(dr, "cd_usuario_alteracao");
			vo.CodUsuarioCriacao = Convert.ToInt32(dr["cd_usuario_criacao"]);
			vo.DataAlteracao = BaseDAO.GetNullableDate(dr, "dt_alteracao");
			vo.CodUsuarioAprovacao = BaseDAO.GetNullableInt(dr, "cd_usuario_aprovacao");
			vo.DataAprovacao = BaseDAO.GetNullableDate(dr, "dt_aprovacao");

			vo.MotivoCancelamento = Convert.ToString(dr["ds_motivo_cancelamento"]);

			vo.Local = FromDataRowPlantaoSocialLocal(dr);
            vo.Plano = new VO.Protheus.PProdutoSaudeVO()
			{
				Codigo = Convert.ToString(dr["BI3_CODIGO"])
			};
			vo.Procedimentos = FormatUtil.StringToList(Convert.ToString(dr["ds_procedimento"])).Select(x => Convert.ToInt32(x)).ToList();

			vo.IsReciprocidade = dr.IsNull("fl_reciprocidade") ? false : Convert.ToBoolean(dr["fl_reciprocidade"]);
			vo.Coberturas = FormatUtil.StringToList(Convert.ToString(dr["ds_cobertura"]));

			vo.Observacao = Convert.ToString(dr["ds_observacao"]);
			vo.Abrangencia = (AbrangenciaAutorizacaoProvisoria)Convert.ToChar(dr["tp_abrangencia"]);

			return vo;
		}

		internal static DataTable Pesquisar(string matemp, int? cdProtocolo, StatusAutorizacaoProvisoria? status, Database db) {
            string sql = "SELECT cd_solicitacao, DT_CRIACAO, DT_FIM_VIGENCIA, CD_STATUS, DT_APROVACAO, DS_MOTIVO_CANCELAMENTO, " +
                "	b.BA1_NOMUSR benef_NM_BENEFICIARIO, p.BI3_DESCRI plano_DS_PLANO, L.SG_UF, L.DS_MUNICIPIO, " +
                "	criador.cd_usuario criador_cd_usuario " +
                " FROM EV_AUTORIZACAO_PROVISORIA sol, VW_PR_USUARIO b, VW_PR_PRODUTO_SAUDE p, EV_PLANTAO_SOCIAL_MUNICIPIO L, EV_USUARIO criador " +
                " WHERE trim(sol.BA1_CODINT) = trim(b.BA1_CODINT) AND trim(sol.BA1_CODEMP) = trim(b.BA1_CODEMP) AND trim(sol.BA1_MATRIC) = trim(b.BA1_MATRIC) AND trim(sol.BA1_TIPREG) = trim(b.BA1_TIPREG) AND trim(sol.BI3_CODIGO) = trim(p.BI3_CODIGO) AND sol.CD_PLANTAO_SOCIAL = L.CD_PLANTAO_SOCIAL " +
                "	AND criador.id_usuario = sol.cd_usuario_criacao ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(matemp))
            {
                sql += " AND trim(B.BA1_MATEMP) = trim(:matemp) ";
                lstParams.Add(new Parametro(":matemp", DbType.String, matemp));
            }
            if (cdProtocolo != null)
            {
                sql += " AND sol.cd_solicitacao = :cdProtocolo ";
                lstParams.Add(new Parametro() { Name = ":cdProtocolo", Tipo = DbType.Int32, Value = cdProtocolo });
            }
            if (status != null)
            {
                sql += " AND sol.cd_status = :status";
                lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = (int)status });
            }
            sql += " ORDER BY cd_solicitacao DESC "; 
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		private static int NextId(Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT SQ_EV_AUTORIZACAO_PROVISORIA.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand);

			return (int)idSolicitacao;
		}

		internal static void Salvar(AutorizacaoProvisoriaVO vo, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (vo.CodSolicitacao == 0) {
				dbCommand = CreateInsert(vo, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CodSolicitacao);
			db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Tipreg.Trim());
            db.AddInParameter(dbCommand, ":cdPlano", DbType.String, vo.Plano.Codigo.Trim());
			db.AddInParameter(dbCommand, ":fimVigencia", DbType.Date, vo.FimVigencia);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, vo.CodUsuarioCriacao);
			db.AddInParameter(dbCommand, ":idPlantao", DbType.String, vo.Local.Id);

			string procs = FormatUtil.ListToString(vo.Procedimentos);
			db.AddInParameter(dbCommand, ":procedimentos", DbType.String, procs);

			db.AddInParameter(dbCommand, ":isReciprocidade", DbType.Boolean, vo.IsReciprocidade);
			string cobertura = FormatUtil.ListToString(vo.Coberturas);
			db.AddInParameter(dbCommand, ":cobertura", DbType.String, cobertura);

			db.AddInParameter(dbCommand, ":obs", DbType.String, vo.Observacao);
			db.AddInParameter(dbCommand, ":abrangencia", DbType.StringFixedLength, (char)vo.Abrangencia);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

        internal static int SalvarIdentity(AutorizacaoProvisoriaVO vo, DbTransaction transaction, Database db)
        {
            DbCommand dbCommand = null;
            if (vo.CodSolicitacao == 0)
            {
                dbCommand = CreateInsert(vo, db);
            }
            else
            {
                dbCommand = CreateUpdate(vo, db);
            }

            db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CodSolicitacao);
            db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Tipreg.Trim());
            db.AddInParameter(dbCommand, ":cdPlano", DbType.String, vo.Plano.Codigo.Trim());
            db.AddInParameter(dbCommand, ":fimVigencia", DbType.Date, vo.FimVigencia);
            db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, vo.CodUsuarioCriacao);
            db.AddInParameter(dbCommand, ":idPlantao", DbType.String, vo.Local.Id);

            string procs = FormatUtil.ListToString(vo.Procedimentos);
            db.AddInParameter(dbCommand, ":procedimentos", DbType.String, procs);

            db.AddInParameter(dbCommand, ":isReciprocidade", DbType.Boolean, vo.IsReciprocidade);
            string cobertura = FormatUtil.ListToString(vo.Coberturas);
            db.AddInParameter(dbCommand, ":cobertura", DbType.String, cobertura);

            db.AddInParameter(dbCommand, ":obs", DbType.String, vo.Observacao);
            db.AddInParameter(dbCommand, ":abrangencia", DbType.StringFixedLength, (char)vo.Abrangencia);

            BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);

            return vo.CodSolicitacao;
        }	

		private static DbCommand CreateInsert(AutorizacaoProvisoriaVO vo, Database db) {

            string sql = "INSERT INTO EV_AUTORIZACAO_PROVISORIA (CD_SOLICITACAO, DT_CRIACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BI3_CODIGO, " +
                "	DT_FIM_VIGENCIA, DS_PROCEDIMENTO, CD_PLANTAO_SOCIAL, CD_USUARIO_CRIACAO, CD_STATUS, " +
                "	FL_RECIPROCIDADE, DS_COBERTURA, DS_OBSERVACAO, TP_ABRANGENCIA) " +
                " VALUES (:id, LOCALTIMESTAMP, :codint, :codemp, :matric, :tipreg, :cdPlano, " +
                "	:fimVigencia, :procedimentos, :idPlantao, :idUsuario, 0, :isReciprocidade, :cobertura, :obs, :abrangencia) ";

			vo.CodSolicitacao = NextId(db);

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(AutorizacaoProvisoriaVO vo, Database db) {

            string sql = "UPDATE EV_AUTORIZACAO_PROVISORIA SET BA1_CODINT = :codint, BA1_CODEMP = :codemp, BA1_MATRIC = :matric, BA1_TIPREG = :tipreg, BI3_CODIGO = :cdPlano, " +
                "	DT_FIM_VIGENCIA = :fimVigencia, DS_PROCEDIMENTO = :procedimentos, CD_PLANTAO_SOCIAL = :idPlantao, " +
                "	FL_RECIPROCIDADE = :isReciprocidade, DS_COBERTURA = :cobertura, DS_OBSERVACAO = :obs, " +
                "	TP_ABRANGENCIA = :abrangencia, " +
                "	CD_USUARIO_ALTERACAO = :idUsuario, DT_ALTERACAO = LOCALTIMESTAMP " +
                "	WHERE CD_SOLICITACAO = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void Aprovar(int cdProtocolo, int idUsuario, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_AUTORIZACAO_PROVISORIA SET CD_STATUS = :status, " +
				" CD_USUARIO_APROVACAO = :idUsuario, DT_APROVACAO = LOCALTIMESTAMP " +
				" WHERE CD_SOLICITACAO = :id ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusAutorizacaoProvisoria.APROVADO);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, cdProtocolo);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static void Cancelar(int cdProtocolo, string motivo, int idUsuario, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_AUTORIZACAO_PROVISORIA SET CD_STATUS = :status, " +
				" CD_USUARIO_ALTERACAO = :idUsuario, DT_ALTERACAO = LOCALTIMESTAMP, " +
				" DS_MOTIVO_CANCELAMENTO = :motivo " +
				" WHERE CD_SOLICITACAO = :id ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusAutorizacaoProvisoria.NEGADO);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
			db.AddInParameter(dbCommand, ":motivo", DbType.String, motivo);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, cdProtocolo);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

	}
}
