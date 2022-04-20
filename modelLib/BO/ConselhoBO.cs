using eVidaGeneralLib.DAO;
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

namespace eVidaGeneralLib.BO {
	public class ConselhoBO {

		EVidaLog log = new EVidaLog(typeof(ConselhoBO));
		private static ConselhoBO instance = new ConselhoBO();

		public static ConselhoBO Instance { get { return instance; } }

		public List<ConselhoVO> ListarConselhos() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ConselhoDAO.ListarConselhos(db);
				} finally {
					connection.Close();
				}
			}
		}

		public ConselhoVO GetConselhoByCodigo(string codigo) {
			List<ConselhoVO> lst = ListarConselhos();
			if (lst != null) {
				foreach (ConselhoVO conselho in lst) {
					if (conselho.Codigo.Equals(codigo, StringComparison.InvariantCultureIgnoreCase))
						return conselho;
				}
			}
			return null;
		}

		public ConselhoVO GetConselhoByUsuario(int idUsuario) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ConselhoDAO.GetConselhoByUsuario(idUsuario, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<UsuarioVO> ListarUsuariosSemConselho(int idPerfil) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ConselhoDAO.ListarUsuariosSemConselho(idPerfil, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<UsuarioVO> ListarUsuariosByConselho(string codigo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ConselhoDAO.ListarUsuariosByConselho(codigo, db);
				} finally {
					connection.Close();
				}
			}
		}

		public bool ExisteConselho(string codigo, string nome) {
			List<ConselhoVO> lst = ListarConselhos();
			if (lst != null) {
				foreach (ConselhoVO conselho in lst) {
					if (conselho.Codigo.Equals(codigo, StringComparison.InvariantCultureIgnoreCase)
						|| conselho.Nome.Equals(nome, StringComparison.InvariantCultureIgnoreCase))
						return true;
				}
			}
			return false;
		}

		public bool IsConselhoUtilizado(ConselhoVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					DataTable dt = ReuniaoDAO.Pesquisar(vo.Codigo, null, null, null, null, db);
					if (dt != null && dt.Rows.Count > 0)
						return true;
					return false;
				} finally {
					connection.Close();
				}
			}
		}

		public void Salvar(ConselhoVO vo, bool isNew) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando conselho " + vo.Codigo + " - " + vo.Nome + " - " + isNew);

					ConselhoDAO.Salvar(vo, isNew, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void Excluir(ConselhoVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Removendo conselho " + vo.Codigo);

					ConselhoDAO.Excluir(vo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void AddUsuarios(List<int> lstIds, string conselhoId) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Adicionando usuários ao conselho " + conselhoId);

					ConselhoDAO.AddUsuarios(lstIds, conselhoId, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void DelUsuarios(List<int> lstIds, string conselhoId) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Removendo usuários do conselho " + conselhoId);

					ConselhoDAO.DelUsuarios(lstIds, conselhoId, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		#region Arquivos

		public List<ArquivoConselhoVO> ListarArquivosTodosConselhos() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ConselhoDAO.ListarArquivosConselho(null, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<ArquivoConselhoVO> ListarArquivosConselho(string codConselho) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ConselhoDAO.ListarArquivosConselho(codConselho, db);
				} finally {
					connection.Close();
				}
			}
		}

		public void SalvarArquivo(ArquivoConselhoVO vo, string nomeFisico) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando arquivo " + vo.CodConselho + " - " + vo.NomeArquivo + " - " + nomeFisico);

					ConselhoDAO.SalvarArquivo(vo, transaction, db);

					if (!string.IsNullOrEmpty(nomeFisico)) {
						String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.CONSELHO);
						FileUtil.MoverArquivo(vo.CodConselho.ToString(), null, nomeFisico, dirDestino, vo.NomeArquivo);
					}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void RemoverArquivo(ArquivoConselhoVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Excluindo arquivo " + vo.CodConselho + " - " + vo.IdArquivo + " - " + vo.NomeArquivo);

					ConselhoDAO.ExcluirArquivo(vo, transaction, db);

					String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.CONSELHO);
					FileUtil.RemoverArquivo(vo.CodConselho.ToString(), dirDestino, vo.NomeArquivo);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		#endregion


	}
}
