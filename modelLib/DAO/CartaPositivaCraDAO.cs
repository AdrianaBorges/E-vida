using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	public class CartaPositivaCraDAO {
		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT SQ_EV_CARTA_POSITIVA_CRA.nextval FROM DUAL";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		internal static DataTable Pesquisar(int? id, string protocoloCra, string matricula, string carteira, CartaPositivaCraStatus? status, EvidaDatabase db) {

            string sql = "SELECT A.CD_SOLICITACAO, A.TP_SOLICITACAO, A.DS_PROTOCOLO_CRA, A.DT_SOLICITACAO, A.CD_USUARIO_CRIACAO, A.CD_STATUS, " +
                "	A.DT_ALTERACAO, A.DT_APROVACAO, A.DS_MOTIVO_CANCELAMENTO, " +
                " B.BA1_NOMUSR, B.BA1_MATANT, P.BI3_CODIGO, P.BI3_DESCRI, " +
                "	CRED.BAU_CODIGO, CRED.BAU_NOME, CRED.BAU_CPFCGC, " +
                "	UC.CD_USUARIO || ' - ' || UC.NM_USUARIO usuario_criacao, " +
                "	UA.CD_USUARIO || ' - ' || UA.NM_USUARIO usuario_aprovacao, " +
                "	UALT.CD_USUARIO || ' - ' || UALT.NM_USUARIO usuario_alteracao " +
                " FROM EV_CARTA_POSITIVA_CRA A, VW_PR_USUARIO B, VW_PR_PRODUTO_SAUDE P, EV_USUARIO UC, EV_USUARIO UA, EV_USUARIO UALT, " +
                "	VW_PR_REDE_ATENDIMENTO cred " +
                " WHERE trim(A.BA1_CODINT) = trim(B.BA1_CODINT) AND trim(A.BA1_CODEMP) = trim(B.BA1_CODEMP) AND trim(A.BA1_MATRIC) = trim(B.BA1_MATRIC) AND trim(A.BA1_TIPREG) = trim(B.BA1_TIPREG) AND trim(A.BI3_CODIGO) = trim(P.BI3_CODIGO) " +
                "	AND uc.id_usuario = a.cd_usuario_criacao " +
                "	AND trim(cred.BAU_CODIGO) = trim(a.BAU_CODIGO) " +
                "	AND ua.id_usuario (+)= a.cd_usuario_aprovacao " +
                "	AND ualt.id_usuario (+)= a.cd_usuario_alteracao ";

            List<Parametro> lst = new List<Parametro>();
            if (id != null)
            {
                sql += " and A.CD_SOLICITACAO = :id ";
                lst.Add(new Parametro(":id", DbType.Int32, id.Value));
            }
            if (!string.IsNullOrEmpty(matricula))
            {
                sql += " and upper(trim(b.BA1_MATEMP)) like upper(trim(:matemp)) ";
                lst.Add(new Parametro(":matemp", DbType.String, matricula));
            }
            if (!string.IsNullOrEmpty(protocoloCra))
            {
                sql += " and A.DS_PROTOCOLO_CRA = :protocoloCra ";
                lst.Add(new Parametro(":protocoloCra", DbType.String, protocoloCra));
            }
            if (!string.IsNullOrEmpty(carteira))
            {
                sql += " and upper(trim(B.BA1_MATANT)) like upper(trim(:carteira)) ";
                lst.Add(new Parametro(":carteira", DbType.String, carteira));
            }
            if (status != null)
            {
                sql += " AND  a.cd_status = :status ";
                lst.Add(new Parametro(":status", DbType.Int32, (int)status.Value));
            } 

			return BaseDAO.ExecuteDataSet(db, sql, lst);
		}

		internal static CartaPositivaCraVO GetById(int id, EvidaDatabase evdb) {
			string sql = "SELECT * FROM EV_CARTA_POSITIVA_CRA A" +
				"		WHERE a.cd_solicitacao = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			List<CartaPositivaCraVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static CartaPositivaCraVO FromDataRow(DataRow dr) {
			CartaPositivaCraVO vo = new CartaPositivaCraVO();

			vo.Beneficiario = new VO.Protheus.PUsuarioVO() {
                Codint = dr.Field<string>("BA1_CODINT"),
                Codemp = dr.Field<string>("BA1_CODEMP"),
                Matric = dr.Field<string>("BA1_MATRIC"),
                Tipreg = dr.Field<string>("BA1_TIPREG"),
			};
			vo.CdPlano = dr.Field<string>("BI3_CODIGO");
			vo.Contato = dr.Field<string>("DS_CONTATO_CREDENCIADO");
			vo.Credenciado = new VO.Protheus.PRedeAtendimentoVO() {
                Codigo = dr.Field<string>("BAU_CODIGO")
			};
			vo.DataAlteracao = BaseDAO.GetNullableDate(dr, "dt_alteracao").Value;
			vo.DataCriacao = BaseDAO.GetNullableDate(dr, "dt_criacao").Value;
			vo.DataSolicitacao = BaseDAO.GetNullableDate(dr, "DT_SOLICITACAO").Value;
			vo.DataAprovacao = BaseDAO.GetNullableDate(dr, "DT_APROVACAO");

			vo.Id = BaseDAO.GetNullableInt(dr, "CD_SOLICITACAO").Value;
			vo.IdUsuarioAlteracao = BaseDAO.GetNullableInt(dr, "CD_USUARIO_ALTERACAO").Value;
			vo.IdUsuarioCriacao = BaseDAO.GetNullableInt(dr, "CD_USUARIO_CRIACAO").Value;
			vo.IdUsuarioAprovacao = BaseDAO.GetNullableInt(dr, "CD_USUARIO_APROVACAO");

			vo.ProtocoloCra = dr.Field<string>("DS_PROTOCOLO_CRA");

			vo.Tipo = BaseDAO.GetNullableEnum<CartaPositivaCraTipo>(dr, "TP_SOLICITACAO").Value;

			vo.Status = BaseDAO.GetNullableEnum<CartaPositivaCraStatus>(dr, "CD_STATUS").Value;

			vo.MotivoCancelamento = dr.Field<string>("DS_MOTIVO_CANCELAMENTO");

			return vo;
		}

		internal static void Salvar(CartaPositivaCraVO vo, EvidaDatabase db) {
			List<Parametro> lstParam = new List<Parametro>();
			string sql = vo.Id == 0 ? CreateInsert(lstParam) : CreateUpdate();

			if (vo.Id == 0)
				vo.Id = NextId(db);

			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":tipo", DbType.Int32, (int)vo.Tipo));
			lstParam.Add(new Parametro(":protocoloCra", DbType.String, vo.ProtocoloCra));
            lstParam.Add(new Parametro(":codint", DbType.String, vo.Beneficiario.Codint.Trim()));
            lstParam.Add(new Parametro(":codemp", DbType.String, vo.Beneficiario.Codemp.Trim()));
            lstParam.Add(new Parametro(":matric", DbType.String, vo.Beneficiario.Matric.Trim()));
            lstParam.Add(new Parametro(":tipreg", DbType.String, vo.Beneficiario.Tipreg.Trim()));
            lstParam.Add(new Parametro(":cdCred", DbType.String, vo.Credenciado.Codigo.Trim()));
			lstParam.Add(new Parametro(":contato", DbType.String, vo.Contato));
            lstParam.Add(new Parametro(":plano", DbType.String, vo.CdPlano.Trim()));
			lstParam.Add(new Parametro(":cdUsuario", DbType.Int32, vo.IdUsuarioAlteracao));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		private static string CreateInsert(List<Parametro> lstParam) {
            string sql = "INSERT INTO EV_CARTA_POSITIVA_CRA (CD_SOLICITACAO, DT_SOLICITACAO, TP_SOLICITACAO, DS_PROTOCOLO_CRA, " +
                "	BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BAU_CODIGO, DS_CONTATO_CREDENCIADO, BI3_CODIGO, " +
                "	CD_USUARIO_CRIACAO, DT_CRIACAO, CD_USUARIO_ALTERACAO, DT_ALTERACAO, CD_STATUS) " +
                "	VALUES (:id, LOCALTIMESTAMP, :tipo, :protocoloCra, :codint, :codemp, :matric, :tipreg, :cdCred, :contato, :plano, " +
                "	:cdUsuario, LOCALTIMESTAMP, :cdUsuario, LOCALTIMESTAMP, :status )";

			lstParam.Add(new Parametro(":status", DbType.Int32, (int)CartaPositivaCraStatus.PENDENTE));

			return sql;
		}

		private static string CreateUpdate() {
            string sql = "UPDATE EV_CARTA_POSITIVA_CRA SET TP_SOLICITACAO = :tipo, DS_PROTOCOLO_CRA = :protocoloCra, " +
                "	BA1_CODINT = :codint, BA1_CODEMP = :codemp, BA1_MATRIC = :matric, BA1_TIPREG = :tipreg, BAU_CODIGO = :cdCred, DS_CONTATO_CREDENCIADO = :contato, " +
                "	BI3_CODIGO = :plano, CD_USUARIO_ALTERACAO = :cdUsuario, DT_ALTERACAO = LOCALTIMESTAMP " +
                "	WHERE CD_SOLICITACAO = :id";
			return sql;
		}

		internal static void Cancelar(int id, string motivoCancelamento, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_CARTA_POSITIVA_CRA SET CD_STATUS = :status, DS_MOTIVO_CANCELAMENTO = :motivo, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP, CD_USUARIO_APROVACAO = NULL, DT_APROVACAO = NULL where CD_SOLICITACAO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":motivo", DbType.String, motivoCancelamento));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)CartaPositivaCraStatus.CANCELADO));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void Aprovar(int id, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_CARTA_POSITIVA_CRA SET CD_STATUS = :status, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP, CD_USUARIO_APROVACAO = :usuario, DT_APROVACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)CartaPositivaCraStatus.APROVADO));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

	}
}
