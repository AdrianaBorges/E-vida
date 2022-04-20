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
	public class SetorUsuarioBO {
		EVidaLog log = new EVidaLog(typeof(SetorUsuarioBO));
		private static SetorUsuarioBO instance = new SetorUsuarioBO();

		public static SetorUsuarioBO Instance { get { return instance; } }

		private const string CACHE_SETOR_USUARIO = "CACHE_SETOR_USUARIO";

		public List<SetorUsuarioVO> ListarSetores() {
			List<SetorUsuarioVO> lst = CacheHelper.GetFromCache<List<SetorUsuarioVO>>(CACHE_SETOR_USUARIO);
			if (lst == null) {
				Database db = DatabaseFactory.CreateDatabase();
				using (DbConnection connection = db.CreateConnection()) {
					connection.Open();

					try {
						lst = SetorUsuarioDAO.ListarConselhos(db);
						CacheHelper.AddOnCache(CACHE_SETOR_USUARIO, lst);
					} finally {
						connection.Close();
					}
				}
			}
			return lst;
		}

		public SetorUsuarioVO GetSetorById(int id) {
			List<SetorUsuarioVO> lst = ListarSetores();
			if (lst != null) {
				foreach (SetorUsuarioVO conselho in lst) {
					if (conselho.Id == id)
						return conselho;
				}
			}
			return null;
		}

		public List<UsuarioVO> ListarUsuariosBySetor(int idSetor) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return SetorUsuarioDAO.ListarUsuariosBySetor(idSetor, db);
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
					return SetorUsuarioDAO.ListarUsuariosNaoAssociados(idPerfil, idSetor, db);
				} finally {
					connection.Close();
				}
			}
		}
		public bool ExisteSetor(int id, string nome) {
			List<SetorUsuarioVO> lst = ListarSetores();
			if (lst != null) {
				foreach (SetorUsuarioVO conselho in lst) {
					if (conselho.Id != id
						&& conselho.Nome.Equals(nome, StringComparison.InvariantCultureIgnoreCase))
						return true;
				}
			}
			return false;
		}
		public bool IsSetorUtilizado(int idSetor) {
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

		public void Salvar(SetorUsuarioVO vo) {
			CacheHelper.RemoveFromCache(CACHE_SETOR_USUARIO);
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando setor " + vo.Id + " - " + vo.Nome);

					SetorUsuarioDAO.Salvar(vo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void Excluir(SetorUsuarioVO vo) {
			CacheHelper.RemoveFromCache(CACHE_SETOR_USUARIO);
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Removendo setor " + vo.Id);

					SetorUsuarioDAO.Excluir(vo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}


		public void AddUsuarios(List<int> lstIds, int idSetor) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Adicionando usuários ao setor " + idSetor);

					SetorUsuarioDAO.AddUsuarios(lstIds, idSetor, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void DelUsuarios(List<int> lstIds, int idSetor) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Removendo usuários do setor " + idSetor);

					SetorUsuarioDAO.DelUsuarios(lstIds, idSetor, transaction, db);

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
