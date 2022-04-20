using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class AdministracaoBO {
		private static AdministracaoBO instance = new AdministracaoBO();

		public static AdministracaoBO Instance { get { return instance; } }

		public List<KeyValuePair<int, string>> ListarTodosPerfis() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AdministracaoDAO.ListarTodosPerfis(db);
			}
		}

        public List<KeyValuePair<int, string>> ListarPerfisConselho()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return AdministracaoDAO.ListarPerfisConselho(db);
            }
        }

		public List<ModuloVO> ListarTodosModulos() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AdministracaoDAO.ListarTodosModulos(db);
			}
		}

		public ModuloVO GetModulo(Modulo m) {
			List<ModuloVO> lst = ListarTodosModulos();
			return lst.Find(x => x.Value == m);
		}

		public List<Modulo> ListarModulosPerfil(int idPerfil) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AdministracaoDAO.ListarModulosPerfil(idPerfil, db);
			}
		}

		public void SalvarModulosPerfil(int idPerfil, List<Modulo> lst) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				List<Modulo> lstAtual = AdministracaoDAO.ListarModulosPerfil(idPerfil, db);

				List<Modulo> lstAdicionar = new List<Modulo>();
				List<Modulo> lstRemover = new List<Modulo>();

				if (lstAtual == null || lstAtual.Count == 0) {
					lstAdicionar = lst;
				} else if (lst.Count == 0) {
					lstRemover = lstAtual;
				} else {
					foreach (Modulo m in lst) {
						if (lstAtual.Contains(m)) {
							lstAtual.Remove(m);
						} else {
							lstAdicionar.Add(m);
						}
					}
					lstRemover = lstAtual;
				}

				AdministracaoDAO.InserirModulosPerfil(idPerfil, lstAdicionar, db);
				AdministracaoDAO.RemoverModulosPerfil(idPerfil, lstRemover, db);

				connection.Commit();
			}
		}

		public List<CategoriaModuloVO> ListarTodasCategorias() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AdministracaoDAO.ListarTodasCategorias(db);
			}
		}
	}
}
