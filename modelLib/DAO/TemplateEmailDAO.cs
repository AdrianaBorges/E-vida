using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
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
	internal class TemplateEmailDAO {
		private static EVidaLog log = new EVidaLog(typeof(TemplateEmailDAO));

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT NVL(MAX(CD_TEMPLATE_EMAIL),0)+1 FROM EV_TEMPLATE_EMAIL ";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		internal static TemplateEmailVO GetById(int id, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_TEMPLATE_EMAIL WHERE cd_template_email = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			List<TemplateEmailVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static List<TemplateEmailVO> ListarTemplates(EvidaDatabase db) {
			string sql = "SELECT * FROM EV_TEMPLATE_EMAIL";

			List<TemplateEmailVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
			return lst;
		}

		internal static TemplateEmailVO FromDataRow(DataRow dr) {
			TemplateEmailVO vo = new TemplateEmailVO();
			vo.Id = Convert.ToInt32(dr["CD_TEMPLATE_EMAIL"]);
			vo.Nome = Convert.ToString(dr["ds_template_email"]);
			vo.Texto = Convert.ToString(dr["ds_texto"]);
			vo.Tipo = (TipoTemplateEmail)Convert.ToInt32(dr["tp_template_email"]);
			vo.CdUsuarioAlteracao = BaseDAO.GetNullableInt(dr, "cd_usuario_alteracao");
			vo.CdUsuarioCriacao = BaseDAO.GetNullableInt(dr, "cd_usuario_criacao").Value;
			vo.DataAlteracao = BaseDAO.GetNullableDate(dr, "dt_alteracao");
			vo.DataCriacao = BaseDAO.GetNullableDate(dr, "dt_criacao").Value;
			return vo;
		}

		internal static void Salvar(TemplateEmailVO vo, int cdUsuario, EvidaDatabase evdb) {
			DbCommand dbCommand = null;
			if (vo.Id == 0) {
				dbCommand = CreateInsert(vo, evdb);
			} else {
				dbCommand = CreateUpdate(vo, evdb);
			}

			Database db = evdb.Database;
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":tipo", DbType.Int32, (int)vo.Tipo);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Nome);
			db.AddInParameter(dbCommand, ":texto", DbType.AnsiString, vo.Texto);

			db.AddInParameter(dbCommand, ":cdUsuario", DbType.Int32,cdUsuario);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static DbCommand CreateInsert(TemplateEmailVO vo, EvidaDatabase db) {
			string sql = "INSERT INTO EV_TEMPLATE_EMAIL (CD_TEMPLATE_EMAIL, DT_CRIACAO, CD_USUARIO_CRIACAO, DS_TEMPLATE_EMAIL, " +
				"	DS_TEXTO, TP_TEMPLATE_EMAIL) " +
				" VALUES (:id, LOCALTIMESTAMP, :cdUsuario, :nome, " +
				"	:texto, :tipo)";

			vo.Id = NextId(db);

			DbCommand dbCommand = db.Database.GetSqlStringCommand(sql);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(TemplateEmailVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_TEMPLATE_EMAIL SET DS_TEMPLATE_EMAIL = :nome," +
				"	DS_TEXTO = :texto, TP_TEMPLATE_EMAIL = :tipo, " +
				"	CD_USUARIO_ALTERACAO = :cdUsuario, DT_ALTERACAO = LOCALTIMESTAMP " +
				"	WHERE CD_TEMPLATE_EMAIL = :id";

			DbCommand dbCommand = db.Database.GetSqlStringCommand(sql);

			return dbCommand;
		}

		internal static void Excluir(TemplateEmailVO vo, EvidaDatabase db) {
			string sql = "DELETE FROM EV_TEMPLATE_EMAIL " +
				"	WHERE CD_TEMPLATE_EMAIL = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}
	}
}
