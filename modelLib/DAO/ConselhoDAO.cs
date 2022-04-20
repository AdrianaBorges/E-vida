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

namespace eVidaGeneralLib.DAO {
	internal class ConselhoDAO {
		static EVidaLog log = new EVidaLog(typeof(ConselhoDAO));
		
		internal static List<ConselhoVO> ListarConselhos(Database db) {
			string sql = "SELECT L.* FROM EV_CONSELHO L" +
				" ORDER BY L.CD_CONSELHO, L.NM_CONSELHO ";
			List<ConselhoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowConselho);
			return lst;
		}

		internal static ConselhoVO GetConselhoByUsuario(int idUsuario, Database db) {
			string sql = "SELECT L.* FROM EV_CONSELHO L" +
				" WHERE EXISTS (SELECT 1 FROM EV_CONSELHO_USUARIO CU WHERE CU.CD_CONSELHO = L.CD_CONSELHO AND CU.ID_USUARIO = :idUsuario) ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idUsuario", Tipo = DbType.String, Value = idUsuario });

			List<ConselhoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowConselho, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static ConselhoVO FromDataRowConselho(DataRow dr) {
			ConselhoVO vo = new ConselhoVO();
			vo.Codigo = dr.Field<string>("cd_conselho");
			vo.Nome = dr.Field<string>("nm_conselho");
			return vo;
		}
		
		internal static void Salvar(ConselhoVO vo, bool isNew, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (isNew) {
				dbCommand = CreateInsert(vo, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":id", DbType.String, vo.Codigo);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Nome);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static DbCommand CreateInsert(ConselhoVO vo, Database db) {
			string sql = "INSERT INTO EV_CONSELHO (CD_CONSELHO, NM_CONSELHO) " +
				"	VALUES (:id, :nome) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(ConselhoVO vo, Database db) {
			string sql = "UPDATE EV_CONSELHO SET NM_CONSELHO = :nome " +
				"	WHERE CD_CONSELHO = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void Excluir(ConselhoVO vo, DbTransaction transaction, Database db) {

			string sql = "DELETE FROM EV_CONSELHO_USUARIO WHERE cd_conselho = :id ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, vo.Codigo);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);

			sql = "DELETE FROM EV_CONSELHO WHERE cd_conselho = :id ";
			dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, vo.Codigo);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static List<UsuarioVO> ListarUsuariosSemConselho(int perfil, Database db) {
			string sql = "SELECT U.* FROM EV_USUARIO U" +
				" WHERE NOT EXISTS (SELECT 1 FROM EV_CONSELHO_USUARIO CU WHERE CU.ID_USUARIO = U.ID_USUARIO) " +
				" AND EXISTS (SELECT 1 FROM EV_PERFIL_USUARIO PU WHERE PU.ID_USUARIO = U.ID_USUARIO AND PU.ID_PERFIL = :idPerfil) " +
				" ORDER BY U.NM_USUARIO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idPerfil", Tipo = DbType.Int32, Value = perfil });
			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow, lstParams);
			return lst;
		}

		internal static List<UsuarioVO> ListarUsuariosByConselho(string codigo, Database db) {
			string sql = "SELECT U.* FROM EV_USUARIO U" +
				" WHERE EXISTS (SELECT 1 FROM EV_CONSELHO_USUARIO CU WHERE CU.ID_USUARIO = U.ID_USUARIO AND CD_CONSELHO = :id) " +
				" ORDER BY U.NM_USUARIO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.String, Value = codigo });

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow, lstParams);
			return lst;
		}

		internal static void AddUsuarios(List<int> lstIds, string conselhoId, DbTransaction transaction, Database db) {
			string sql = "INSERT INTO EV_CONSELHO_USUARIO (CD_CONSELHO, ID_USUARIO) VALUES (:id, :idUsuario)";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, conselhoId);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32);

			foreach (int id in lstIds) {
				db.SetParameterValue(dbCommand, ":idUsuario", id);
				BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
			}
		}

		internal static void DelUsuarios(List<int> lstIds, string conselhoId, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_CONSELHO_USUARIO WHERE cd_conselho = :id AND id_usuario = :idUsuario";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.String, conselhoId);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32);

			foreach (int id in lstIds) {
				db.SetParameterValue(dbCommand, ":idUsuario", id);
				BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);	
			}
		}

		#region Arquivos

		private static ArquivoConselhoVO FromDataRowArquivo(DataRow dr) {
			ArquivoConselhoVO vo = new ArquivoConselhoVO();
			vo.IdArquivo = Convert.ToInt32(dr["cd_arquivo"]);
			vo.CodConselho = Convert.ToString(dr["cd_conselho"]);
			vo.NomeArquivo = Convert.ToString(dr["nm_arquivo"]);
			vo.Descricao = Convert.ToString(dr["ds_arquivo"]);
			return vo;
		}

		internal static List<ArquivoConselhoVO> ListarArquivosConselho(string codConselho, Database db) {
			string sql = "SELECT A.* " +
				" FROM EV_CONSELHO_ARQUIVO A " +
				(!string.IsNullOrEmpty(codConselho) ? " WHERE A.cd_conselho = :codConselho " : "") +
				" order by a.cd_conselho ASC, a.CD_ARQUIVO DESC";
			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(codConselho))
				lstParams.Add(new Parametro() { Name = ":codConselho", Tipo = DbType.String, Value = codConselho });

			List<ArquivoConselhoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParams);
			return lst;
		}

		internal static void SalvarArquivo(ArquivoConselhoVO vo, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (vo.IdArquivo == 0) {
				dbCommand = CreateInsert(vo, db);
				vo.IdArquivo = GetNextArquivoId(vo.CodConselho, transaction, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32, vo.IdArquivo);
			db.AddInParameter(dbCommand, ":codConselho", DbType.String, vo.CodConselho);
			db.AddInParameter(dbCommand, ":descricao", DbType.String, vo.Descricao);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static int GetNextArquivoId(string codConselho, DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT NVL(MAX(CD_ARQUIVO),0) FROM EV_CONSELHO_ARQUIVO WHERE cd_conselho = :codConselho");
			db.AddInParameter(dbCommand, ":codConselho", DbType.String, codConselho);

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (int)idSolicitacao + 1;
		}

		private static DbCommand CreateInsert(ArquivoConselhoVO vo, Database db) {
			string sql = "INSERT INTO EV_CONSELHO_ARQUIVO (cd_conselho, CD_ARQUIVO, NM_ARQUIVO, DS_ARQUIVO) " +
				"	VALUES (:codConselho, :idArquivo, :nome, :descricao) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.NomeArquivo);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(ArquivoConselhoVO vo, Database db) {
			string sql = "UPDATE EV_CONSELHO_ARQUIVO SET DS_ARQUIVO = :descricao " +
				"	WHERE cd_conselho = :codConselho AND CD_ARQUIVO = :idArquivo";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void ExcluirArquivo(ArquivoConselhoVO vo, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_CONSELHO_ARQUIVO " +
				"	WHERE cd_conselho = :codConselho AND CD_ARQUIVO = :idArquivo";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":codConselho", DbType.String, vo.CodConselho);
			db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32, vo.IdArquivo);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		#endregion
	}
}
