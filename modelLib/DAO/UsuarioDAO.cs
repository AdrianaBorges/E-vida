using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class UsuarioDAO {
        const string FIELDS_USUARIO = "U.ID_USUARIO, U.CD_USUARIO, U.NM_USUARIO, U.DS_EMAIL, U.DT_ULT_LOGIN, U.DS_CARGO, U.CD_MATRICULA, U.BIB_CODREG ";
		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT SQ_EV_USUARIO.nextval FROM DUAL";

			decimal idSimulacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			
			return (int)idSimulacao;
		}

		internal static UsuarioVO GetByLogin(string login, EvidaDatabase db) {
			string sql = "SELECT " + FIELDS_USUARIO +
				" FROM EV_USUARIO U " +
				" WHERE upper(trim(U.CD_USUARIO)) = upper(trim(:login)) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			if (lst != null && lst.Count > 0) {
				return lst[0];
			}
			return null;
		}
		
		internal static void RegistrarLogin(int idUsuario, EvidaDatabase evdb) {
			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand("UPDATE EV_USUARIO " +
				" SET DT_ULT_LOGIN = localtimestamp " +
				" WHERE id_usuario = :idUsuario");

			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static List<Modulo> ListarModulosUsuario(int idUsuario, EvidaDatabase db) {
			string sql = "SELECT DISTINCT ID_MODULO FROM EV_PERFIL_MODULO PM, EV_PERFIL_USUARIO U " +
				" WHERE U.ID_PERFIL = PM.ID_PERFIL AND U.ID_USUARIO = :idUsuario";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idUsuario", Tipo = DbType.Int32, Value = idUsuario });

			Func<DataRow, Modulo> convert = delegate(DataRow dr) { return (Modulo)Enum.Parse(typeof(Modulo), Convert.ToString(dr["id_modulo"])); };

			List<Modulo> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);

			return lst;
		}
		
		internal static UsuarioVO FromDataRow(DataRow dr) {
			UsuarioVO vo = new UsuarioVO();
			vo.Id = Convert.ToInt32(dr["id_usuario"]);
			vo.Login = Convert.ToString(dr["cd_usuario"]);
			vo.Nome = Convert.ToString(dr["nm_usuario"]);
			vo.Email = Convert.ToString(dr["ds_email"]);
			vo.Cargo = Convert.ToString(dr["ds_cargo"]);
			vo.UltimoLogin = dr["DT_ULT_LOGIN"] != DBNull.Value ? Convert.ToDateTime(dr["DT_ULT_LOGIN"]) : new DateTime?();
			vo.Matricula = BaseDAO.GetNullableLong(dr, "CD_MATRICULA");
            vo.Regional = Convert.ToString(dr["BIB_CODREG"]);
			return vo;
		}

        /*
		internal static DataTable PesquisarUsuarios(string login, string nome, int? idPerfil, string idRegional, EvidaDatabase db) {
            string sql = "SELECT U.ID_USUARIO, NVL(U.CD_USUARIO,SU.CD_USUARIO) CD_USUARIO, NVL(U.NM_USUARIO,SU.NM_USUARIO)NM_USUARIO, " +
              "		U.DS_EMAIL, U.DT_ULT_LOGIN, U.CD_MATRICULA, U.BIB_CODREG " +
              "	FROM	EV_USUARIO U, SCL_USUARIO SU " +
              "	WHERE	SU.CD_USUARIO = U.CD_USUARIO(+) AND SU.ID_ATIVO = 1";

            List<Parametro> lstParams = new List<Parametro>();

            if (!string.IsNullOrEmpty(login))
            {
                sql += " AND UPPER(NVL(U.CD_USUARIO,SU.CD_USUARIO)) = :login ";
                lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND UPPER(NVL(U.NM_USUARIO,SU.NM_USUARIO)) LIKE :nome ";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }
            if (idPerfil != null && idPerfil.HasValue)
            {
                if (idPerfil == -1)
                {
                    sql += " AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+)) ";
                }
                else if (idPerfil == -2)
                {
                    sql += " AND NOT EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+)) ";
                }
                else
                {
                    sql += " AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+) AND P.ID_PERFIL = :idPerfil) ";
                    lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = idPerfil });
                }
            }
            if (idRegional != null)
            {
                sql += " AND U.BIB_CODREG = :idRegional ";
                lstParams.Add(new Parametro() { Name = ":idRegional", Tipo = DbType.String, Value = idRegional });
            }
            sql += " AND ROWNUM < 400 ";
            sql += " order by U.CD_USUARIO, SU.CD_USUARIO";   
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}
        */

        internal static DataTable PesquisarUsuarios(string login, string nome, int? idPerfil, string idRegional, EvidaDatabase db)
        {
            string sql = "SELECT U.ID_USUARIO, U.CD_USUARIO, U.NM_USUARIO, U.DS_EMAIL, U.DT_ULT_LOGIN, U.CD_MATRICULA, U.BIB_CODREG " +
              "	FROM EV_USUARIO U " +
              "	WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();

            if (!string.IsNullOrEmpty(login))
            {
                sql += " AND UPPER(U.CD_USUARIO) = :login ";
                lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND UPPER(U.NM_USUARIO) LIKE :nome ";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }
            if (idPerfil != null && idPerfil.HasValue)
            {
                if (idPerfil == -1)
                {
                    sql += " AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+)) ";
                }
                else if (idPerfil == -2)
                {
                    sql += " AND NOT EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+)) ";
                }
                else
                {
                    sql += " AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+) AND P.ID_PERFIL = :idPerfil) ";
                    lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = idPerfil });
                }
            }
            if (idRegional != null)
            {
                sql += " AND U.BIB_CODREG = :idRegional ";
                lstParams.Add(new Parametro() { Name = ":idRegional", Tipo = DbType.String, Value = idRegional });
            }
            sql += " AND ROWNUM < 400 ";
            sql += " order by U.CD_USUARIO";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

		internal static DataTable PesquisarUsuariosInterno(string login, string nome, int? idPerfil, EvidaDatabase db) {
			string sql = "SELECT " + FIELDS_USUARIO +
				"	FROM	EV_USUARIO U " +
				"	WHERE	1 = 1 ";

			List<Parametro> lstParams = new List<Parametro>();

			if (!string.IsNullOrEmpty(login)) {
				sql += " AND UPPER(U.CD_USUARIO) = :login ";
				lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
			}
			if (!string.IsNullOrEmpty(nome)) {
                sql += " AND upper(trim(U.NM_USUARIO)) LIKE upper(trim(:nome)) ";
				lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
			}
			if (idPerfil != null && idPerfil.HasValue) {
				if (idPerfil == -1) {
					sql += " AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+)) ";
				} else if (idPerfil == -2) {
					sql += " AND NOT EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+)) ";
				} else {
					sql += " AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO P WHERE U.ID_USUARIO = P.ID_USUARIO(+) AND P.ID_PERFIL = :idPerfil) ";
					lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = idPerfil });
				}
			}
			sql += " AND ROWNUM < 400 ";
			sql += " order by U.CD_USUARIO";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void SalvarUsuario(UsuarioVO usuario, List<Perfil> lstPerfil, EvidaDatabase evdb) {
			string sql = null;
            if (usuario.Id == 0)
            {
                sql = "INSERT INTO EV_USUARIO (ID_USUARIO, CD_USUARIO, NM_USUARIO, DS_EMAIL, DS_CARGO, CD_MATRICULA, BIB_CODREG) " +
                " VALUES (:idUsuario, :login, :nome, :email, :cargo, :matricula, :regional) ";
                usuario.Id = NextId(evdb);
            }
            else
            {
                sql = "UPDATE EV_USUARIO SET CD_USUARIO = :login, NM_USUARIO = :nome, DS_EMAIL = :email" +
                  " , DS_CARGO = :cargo, CD_MATRICULA = :matricula, BIB_CODREG = :regional " +
                  " WHERE ID_USUARIO = :idUsuario";
            }    

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, usuario.Id);
			db.AddInParameter(dbCommand, ":login", DbType.String, usuario.Login.ToUpper());
			db.AddInParameter(dbCommand, ":nome", DbType.String, usuario.Nome);
			db.AddInParameter(dbCommand, ":email", DbType.String, usuario.Email);
			db.AddInParameter(dbCommand, ":cargo", DbType.String, usuario.Cargo);
			db.AddInParameter(dbCommand, ":matricula", DbType.Int64, usuario.Matricula);
			db.AddInParameter(dbCommand, ":regional", DbType.String, usuario.Regional);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			SalvarPerfilUsuario(usuario, lstPerfil, evdb);
		}

		private static void SalvarPerfilUsuario(UsuarioVO usuario, List<Perfil> lstPerfil, EvidaDatabase evdb) {
			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand("DELETE FROM EV_PERFIL_USUARIO WHERE ID_USUARIO = :idUsuario");

			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, usuario.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			dbCommand = db.GetSqlStringCommand("INSERT INTO EV_PERFIL_USUARIO (ID_PERFIL, ID_USUARIO) VALUES (:idPerfil, :idUsuario)");

			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, usuario.Id);
			db.AddInParameter(dbCommand, ":idPerfil", DbType.Int32);
			foreach (Perfil p in lstPerfil) {
				db.SetParameterValue(dbCommand, ":idPerfil", (int)p);
				BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			}
		}

		internal static List<UsuarioVO> ListarUsuarios(EvidaDatabase db) {
			string sql = "SELECT * " +
				" FROM EV_USUARIO U ";

			List<Parametro> lstParams = new List<Parametro>();

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}

		internal static UsuarioVO GetUsuarioById(int id, EvidaDatabase db) {
			string sql = "SELECT " + FIELDS_USUARIO +
				" FROM EV_USUARIO U " +
				" WHERE U.ID_USUARIO = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			if (lst != null && lst.Count > 0) {
				return lst[0];
			}
			return null;
		}

		/*
		internal static List<UsuarioVO> GetUsuariosByPerfil(Perfil perfil, Database db) {
			string sql = "SELECT U.ID_USUARIO, U.CD_USUARIO, U.NM_USUARIO, U.DS_EMAIL, U.DT_ULT_LOGIN, U.DS_CARGO " +
				" FROM EV_USUARIO U, EV_PERFIL_USUARIO PU " +
				" WHERE PU.ID_USUARIO = U.ID_USUARIO AND PU.ID_PERFIL = :idPerfil ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = (int)perfil });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}
		*/

		internal static UsuarioVO GetUsuarioScl(string login, EvidaDatabase db) {
			string sql = "SELECT 0 ID_USUARIO, CD_USUARIO, NM_USUARIO, NULL DS_EMAIL, NULL DT_ULT_LOGIN, NULL DS_CARGO, NULL CD_MATRICULA " +
				" FROM SCL_USUARIO U " +
				" WHERE U.CD_USUARIO = :login AND ID_ATIVO = 1";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			if (lst != null && lst.Count > 0) {
				return lst[0];
			}
			return null;
		}

		internal static List<UsuarioVO> GetUsuariosByModulo(Modulo modulo, EvidaDatabase db) {
			string sql = "SELECT DISTINCT " + FIELDS_USUARIO +
				" FROM EV_USUARIO U, EV_PERFIL_MODULO PM, EV_PERFIL_USUARIO PU " +
				" WHERE PU.ID_PERFIL = PM.ID_PERFIL AND PU.ID_USUARIO = U.ID_USUARIO " +
				"		AND PM.ID_MODULO = :modulo" ;

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":modulo", Tipo = DbType.Int32, Value = (int)modulo });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}

		internal static List<Perfil> GetPerfilByUsuario(int idUsuario, EvidaDatabase db) {
			string sql = "SELECT DISTINCT ID_PERFIL FROM EV_PERFIL_USUARIO U " +
				" WHERE U.ID_USUARIO = :idUsuario";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idUsuario", Tipo = DbType.Int32, Value = idUsuario });

			Func<DataRow, Perfil> convert = delegate(DataRow dr) { return (Perfil)Convert.ToInt32(dr["id_perfil"]); };

			List<Perfil> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);

			return lst;
		}

		internal static void RemoverUsuario(UsuarioVO usuario, EvidaDatabase evdb) {
			Database db = evdb.Database;

			DbCommand dbCommand = db.GetSqlStringCommand("DELETE FROM EV_PERFIL_USUARIO WHERE ID_USUARIO = :idUsuario");
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, usuario.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			dbCommand = db.GetSqlStringCommand("DELETE FROM EV_USUARIO WHERE ID_USUARIO = :idUsuario");
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, usuario.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
