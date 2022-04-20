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
	internal class RamalDAO {
		static EVidaLog log = new EVidaLog(typeof(RamalDAO));

		internal static List<RamalVO> ListarRamais(Database db) {
			string sql = "SELECT L.* FROM EV_RAMAL L" +
				" ORDER BY L.NR_RAMAL ";
			List<RamalVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowRamal);
			return lst;
		}

		private static RamalVO FromDataRowRamal(DataRow dr) {
			RamalVO vo = new RamalVO();
			vo.NrRamal = Convert.ToInt32(dr["nr_ramal"]);
			vo.Tipo= dr.Field<string>("tp_ramal");
			vo.IdSetor = BaseDAO.GetNullableInt(dr, "id_setor");
			vo.Usuarios = FormatUtil.StringToList(dr.Field<string>("ds_usuario")).Select(x => Int32.Parse(x)).ToList();
			vo.Alias = dr.Field<string>("ds_alias");
			return vo;
		}

		internal static void Salvar(RamalVO vo, bool isNew, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (isNew) {
				dbCommand = CreateInsert(vo, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.NrRamal);
			db.AddInParameter(dbCommand, ":tipo", DbType.String, vo.Tipo);
			db.AddInParameter(dbCommand, ":idSetor", DbType.Int32, vo.IdSetor);
			db.AddInParameter(dbCommand, ":alias", DbType.String, vo.Alias);
			db.AddInParameter(dbCommand, ":usuarios", DbType.String, FormatUtil.ListToString(vo.Usuarios));

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static DbCommand CreateInsert(RamalVO vo, Database db) {
			string sql = "INSERT INTO EV_RAMAL (NR_RAMAL, TP_RAMAL, ID_SETOR, DS_USUARIO, DS_ALIAS) " +
				"	VALUES (:id, :tipo, :idSetor, :usuarios, :alias) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(RamalVO vo, Database db) {
			string sql = "UPDATE EV_RAMAL SET TP_RAMAL = :tipo, ID_SETOR = :idSetor, DS_USUARIO = :usuarios, DS_ALIAS = :alias " +
				"	WHERE NR_RAMAL = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void Excluir(RamalVO vo, DbTransaction transaction, Database db) {

			string sql = "DELETE FROM EV_REL_RAMAL_USUARIO WHERE NR_RAMAL = :id ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.NrRamal);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);

			sql = "DELETE FROM EV_RAMAL WHERE NR_RAMAL = :id ";
			dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.NrRamal);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static List<UsuarioVO> ListarUsuariosByRamal(int nrRamal, Database db) {
			string sql = "SELECT U.* FROM EV_USUARIO U" +
				" WHERE EXISTS (SELECT 1 FROM EV_REL_RAMAL_USUARIO CU WHERE CU.ID_USUARIO = U.ID_USUARIO AND NR_RAMAL = :id) " +
				" ORDER BY U.NM_USUARIO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = nrRamal });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow, lstParams);
			return lst;
		}

		internal static List<UsuarioVO> ListarUsuariosNaoAssociados(int idPerfil, int nrRamal, Database db) {
			string sql = "SELECT U.* FROM EV_USUARIO U" +
				" WHERE EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO PU WHERE PU.ID_USUARIO = U.ID_USUARIO AND PU.ID_PERFIL = :idPerfil) " +
				" AND NOT EXISTS (SELECT 1 FROM EV_REL_RAMAL_USUARIO CU WHERE CU.ID_USUARIO = U.ID_USUARIO AND NR_RAMAL = :nrRamal) " +
				" ORDER BY U.NM_USUARIO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":nrRamal", Tipo = DbType.Int32, Value = nrRamal });
			lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = idPerfil });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow, lstParams);
			return lst;
		}

		internal static void AddUsuarios(List<int> lstIds, int nrRamal, DbTransaction transaction, Database db) {
			string sql = "INSERT INTO EV_REL_RAMAL_USUARIO (NR_RAMAL, ID_USUARIO) VALUES (:id, :idUsuario)";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, nrRamal);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32);

			foreach (int id in lstIds) {
				db.SetParameterValue(dbCommand, ":idUsuario", id);
				BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
			}
		}

		internal static void ClearUsuarios(int nrRamal, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_REL_RAMAL_USUARIO WHERE NR_RAMAL = :id";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, nrRamal);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);			
		}

	}
}
