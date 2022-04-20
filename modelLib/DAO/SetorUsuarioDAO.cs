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
	internal class SetorUsuarioDAO {
		static EVidaLog log = new EVidaLog(typeof(SetorUsuarioDAO));

		private static int GetNextId(DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT NVL(MAX(ID_SETOR),0) FROM EV_SETOR_USUARIO ");

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (int)idSolicitacao + 1;
		}

		internal static List<SetorUsuarioVO> ListarConselhos(Database db) {
			string sql = "SELECT L.* FROM EV_SETOR_USUARIO L" +
				" ORDER BY L.NM_SETOR ";
			List<SetorUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowSetor);
			return lst;
		}

		private static SetorUsuarioVO FromDataRowSetor(DataRow dr) {
			SetorUsuarioVO vo = new SetorUsuarioVO();
			vo.Id = Convert.ToInt32(dr["id_setor"]);
			vo.Nome = dr.Field<string>("nm_setor");
			return vo;
		}

		internal static void Salvar(SetorUsuarioVO vo, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (vo.Id == 0) {
				vo.Id = GetNextId(transaction, db);
				dbCommand = CreateInsert(vo, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Nome);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static DbCommand CreateInsert(SetorUsuarioVO vo, Database db) {
			string sql = "INSERT INTO EV_SETOR_USUARIO (ID_SETOR, NM_SETOR) " +
				"	VALUES (:id, :nome) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(SetorUsuarioVO vo, Database db) {
			string sql = "UPDATE EV_SETOR_USUARIO SET NM_SETOR = :nome " +
				"	WHERE ID_SETOR = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void Excluir(SetorUsuarioVO vo, DbTransaction transaction, Database db) {

			string sql = "DELETE FROM EV_REL_SETOR_USUARIO WHERE ID_SETOR = :id ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);

			sql = "DELETE FROM EV_SETOR_USUARIO WHERE ID_SETOR = :id ";
			dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}
		
		internal static List<UsuarioVO> ListarUsuariosBySetor(int idSetor, Database db) {
			string sql = "SELECT U.* FROM EV_USUARIO U" +
				" WHERE EXISTS (SELECT 1 FROM EV_REL_SETOR_USUARIO CU WHERE CU.ID_USUARIO = U.ID_USUARIO AND ID_SETOR = :id) " +
				" ORDER BY U.NM_USUARIO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = idSetor });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow, lstParams);
			return lst;
		}

		internal static List<UsuarioVO> ListarUsuariosNaoAssociados(int idPerfil, int idSetor, Database db) {
			string sql = "SELECT U.* FROM EV_USUARIO U" +
				" WHERE EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO PU WHERE PU.ID_USUARIO = U.ID_USUARIO AND PU.ID_PERFIL = :idPerfil) " +
				" AND NOT EXISTS (SELECT 1 FROM EV_REL_SETOR_USUARIO CU WHERE CU.ID_USUARIO = U.ID_USUARIO AND ID_SETOR = :id) " +
				" ORDER BY U.NM_USUARIO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = idSetor });
			lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = idPerfil });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow, lstParams);
			return lst;
		}

		internal static void AddUsuarios(List<int> lstIds, int idSetor, DbTransaction transaction, Database db) {
			string sql = "INSERT INTO EV_REL_SETOR_USUARIO (ID_SETOR, ID_USUARIO) VALUES (:id, :idUsuario)";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, idSetor);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32);

			foreach (int id in lstIds) {
				db.SetParameterValue(dbCommand, ":idUsuario", id);
				BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
			}
		}

		internal static void DelUsuarios(List<int> lstIds, int idSetor, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_REL_SETOR_USUARIO WHERE ID_SETOR = :id AND id_usuario = :idUsuario";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, idSetor);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32);

			foreach (int id in lstIds) {
				db.SetParameterValue(dbCommand, ":idUsuario", id);
				BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
			}
		}

	}
}
