using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class RamalBO {
		EVidaLog log = new EVidaLog(typeof(RamalBO));
		private static RamalBO instance = new RamalBO();

		public static RamalBO Instance { get { return instance; } }

		private const string CACHE_RAMAL = "CACHE_RAMAL";
		
		public List<RamalVO> ListarRamais() {
			List<RamalVO> lst = CacheHelper.GetFromCache<List<RamalVO>>(CACHE_RAMAL);

			if (lst == null) {
				Database db = DatabaseFactory.CreateDatabase();
				using (DbConnection connection = db.CreateConnection()) {
					connection.Open();

					try {
						lst = RamalDAO.ListarRamais(db);
						//CacheHelper.AddOnCache(CACHE_RAMAL, lst);
					} finally {
						connection.Close();
					}
				}
			}
			return lst;
		}

		public RamalVO GetRamalByNro(int nroRamal) {
			List<RamalVO> lst = ListarRamais();
			if (lst != null) {
				foreach (RamalVO ramal in lst) {
					if (ramal.NrRamal == nroRamal)
						return ramal;
				}
			}
			return null;
		}

		public List<UsuarioVO> ListarUsuariosByRamal(int nroRamal) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return RamalDAO.ListarUsuariosByRamal(nroRamal, db);
				} finally {
					connection.Close();
				}
			}
		}
		public List<UsuarioVO> ListarUsuariosNaoAssociados(int idPerfil, int idSetor) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return RamalDAO.ListarUsuariosNaoAssociados(idPerfil, idSetor, db);
				} finally {
					connection.Close();
				}
			}
		}
		public bool ExisteRamal(int nroRamal) {
			List<RamalVO> lst = ListarRamais();
			if (lst != null) {
				foreach (RamalVO ramal in lst) {
					if (ramal.NrRamal == nroRamal)
						return true;
				}
			}
			return false;
		}
		public bool ExisteRamal(string alias) {
			List<RamalVO> lst = ListarRamais();
			if (lst != null) {
				foreach (RamalVO ramal in lst) {
					if (alias.Equals(ramal.Alias, StringComparison.InvariantCultureIgnoreCase))
						return true;
				}
			}
			return false;
		}
		public bool IsRamalUtilizado(int nroRamal) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return false;
				} finally {
					connection.Close();
				}
			}
		}

		public void Salvar(RamalVO vo, bool isNew) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando ramal " + vo.NrRamal + " - " + vo.Tipo + " - " + vo.IdSetor + " - " + isNew);

					RamalDAO.Salvar(vo, isNew, transaction, db);
					if (!isNew)
						RamalDAO.ClearUsuarios(vo.NrRamal, transaction, db);
					RamalDAO.AddUsuarios(vo.Usuarios, vo.NrRamal, transaction, db);
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void Excluir(RamalVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Removendo ramal " + vo.NrRamal);

					RamalDAO.Excluir(vo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

	}
}
