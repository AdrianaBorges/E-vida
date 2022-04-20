using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class AdministracaoDAO {

		internal static List<KeyValuePair<int,string>> ListarTodosPerfis(EvidaDatabase db) {
			string sql = "SELECT ID_PERFIL, NM_PERFIL FROM EV_PERFIL P ORDER BY NM_PERFIL ";

			Func<DataRow, KeyValuePair<int,string>> convert = delegate(DataRow dr) { 
				return new KeyValuePair<int,string>(Convert.ToInt32(dr["ID_PERFIL"]), Convert.ToString(dr["NM_PERFIL"]));
			};

			List<KeyValuePair<int, string>> lst = BaseDAO.ExecuteDataSet(db, sql, convert);

			return lst;
		}

        internal static List<KeyValuePair<int, string>> ListarPerfisConselho(EvidaDatabase db)
        {
            string sql = "SELECT ID_PERFIL, NM_PERFIL FROM EV_PERFIL P WHERE ID_PERFIL IN (3, 9, 10, 13, 16, 17) ORDER BY NM_PERFIL ";

            Func<DataRow, KeyValuePair<int, string>> convert = delegate(DataRow dr)
            {
                return new KeyValuePair<int, string>(Convert.ToInt32(dr["ID_PERFIL"]), Convert.ToString(dr["NM_PERFIL"]));
            };

            List<KeyValuePair<int, string>> lst = BaseDAO.ExecuteDataSet(db, sql, convert);

            return lst;
        }

		internal static List<ModuloVO> ListarTodosModulos(EvidaDatabase db) {
			string sql = "SELECT ID_MODULO, NM_MODULO, ID_CATEGORIA FROM EV_MODULO M ORDER BY NM_MODULO";

			Func<DataRow, ModuloVO> convert = delegate(DataRow dr)
			{
				return new ModuloVO()
				{
					Id = Convert.ToInt32(dr["ID_MODULO"]),
					Nome = Convert.ToString(dr["NM_MODULO"]),
					Value = (Modulo)Convert.ToInt32(dr["ID_MODULO"]),
					IdCategoria = Convert.ToInt32(dr["ID_CATEGORIA"])
				};
			};

			List<ModuloVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);

			return lst;
		}

		internal static List<CategoriaModuloVO> ListarTodasCategorias(EvidaDatabase db) {
			string sql = "SELECT ID_CATEGORIA, NM_CATEGORIA FROM EV_CATEGORIA_MODULO M ORDER BY NM_CATEGORIA";

			Func<DataRow, CategoriaModuloVO> convert = delegate(DataRow dr)
			{
				return new CategoriaModuloVO()
				{
					Id = Convert.ToInt32(dr["ID_CATEGORIA"]),
					Nome = Convert.ToString(dr["NM_CATEGORIA"])
				};
			};

			List<CategoriaModuloVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);

			return lst;
		}

		internal static List<Modulo> ListarModulosPerfil(int idPerfil, EvidaDatabase db) {
			string sql = "SELECT ID_MODULO FROM EV_PERFIL_MODULO PM " +
				" WHERE PM.ID_PERFIL = :idPerfil";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = idPerfil });

			Func<DataRow, Modulo> convert = delegate(DataRow dr) { return (Modulo)Enum.Parse(typeof(Modulo), Convert.ToString(dr["id_modulo"])); };

			List<Modulo> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);

			return lst;
		}

		internal static void InserirModulosPerfil(int idPerfil, List<Modulo> lstAdicionar, EvidaDatabase db) {
			string sql = "INSERT INTO EV_PERFIL_MODULO (ID_PERFIL, ID_MODULO) VALUES (:idPerfil, :idModulo) ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":idPerfil", DbType.Int32, idPerfil));
			lstParams.Add(new ParametroVar(":idModulo", DbType.Int32));

			List<ParametroVarRow> lstRows = new List<ParametroVarRow>();
			foreach (Modulo m in lstAdicionar) {
				ParametroVarRow row = new ParametroVarRow(lstParams);
				row.Set(":idModulo", (int)m);
				lstRows.Add(row);
			}
			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParams, lstRows, db);
		}

		internal static void RemoverModulosPerfil(int idPerfil, List<Modulo> lstRemover, EvidaDatabase db) {
			string sql = "DELETE FROM EV_PERFIL_MODULO WHERE ID_PERFIL = :idPerfil AND ID_MODULO = :idModulo ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":idPerfil", DbType.Int32, idPerfil));
			lstParams.Add(new ParametroVar(":idModulo", DbType.Int32));
			
			List<ParametroVarRow> lstRows = new List<ParametroVarRow>();
			foreach (Modulo m in lstRemover) {
				ParametroVarRow row = new ParametroVarRow(lstParams);
				row.Set(":idModulo", (int)m);
				lstRows.Add(row);
			}
			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParams, lstRows, db);
		}

	}
}
