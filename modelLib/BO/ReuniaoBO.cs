using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.BO {
	public class ReuniaoBO {

		EVidaLog log = new EVidaLog(typeof(ReuniaoBO));
		private static ReuniaoBO instance = new ReuniaoBO();

		public static ReuniaoBO Instance { get { return instance; } }

		public ReuniaoVO GetById(int id) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ReuniaoDAO.GetById(id, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable Pesquisar(string cdConselho, string titulo, string descricao, DateTime? inicio, DateTime? fim) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ReuniaoDAO.Pesquisar(cdConselho, titulo, descricao, inicio, fim, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<ReuniaoVO> ListReuniaoAno(List<string> lstConselho, int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ReuniaoDAO.ListReuniaoAno(lstConselho, ano, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public void Salvar(ReuniaoVO vo, UsuarioVO criador) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando reuniao " + vo.Id + " - " + vo.Titulo);

					ReuniaoDAO.Salvar(vo, criador, transaction, db);

					transaction.Commit();
				}
				catch {
					transaction.Rollback();
					throw;
				}
				finally {
					connection.Close();
				}
			}
		}

		public void Excluir(ReuniaoVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Excluindo reuniao " + vo.Id);

					ReuniaoDAO.Excluir(vo, transaction, db);

					transaction.Commit();
				}
				catch {
					transaction.Rollback();
					throw;
				}
				finally {
					connection.Close();
				}
			}
		}

		#region Arquivos

		public List<ArquivoReuniaoVO> ListarArquivosByReuniao(int id) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ReuniaoDAO.ListarArquivosByReuniao(id, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public void SalvarArquivo(ArquivoReuniaoVO vo, string nomeFisico) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando arquivo " + vo.CodReuniao + " - " + vo.NomeArquivo);

					ReuniaoDAO.SalvarArquivo(vo, transaction, db);

					if (!string.IsNullOrEmpty(nomeFisico)) {
						String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.REUNIAO);
						FileUtil.MoverArquivo(vo.CodReuniao.ToString(), null, nomeFisico, dirDestino, vo.NomeArquivo);
					}

					transaction.Commit();
				}
				catch {
					transaction.Rollback();
					throw;
				}
				finally {
					connection.Close();
				}
			}
		}

		public void RemoverArquivo(ArquivoReuniaoVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Excluindo arquivo " + vo.CodReuniao + " - " + vo.IdArquivo + " - " + vo.NomeArquivo);

					ReuniaoDAO.ExcluirArquivo(vo, transaction, db);

					String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.REUNIAO);
					FileUtil.RemoverArquivo(vo.CodReuniao.ToString(), dirDestino, vo.NomeArquivo);

					transaction.Commit();
				}
				catch {
					transaction.Rollback();
					throw;
				}
				finally {
					connection.Close();
				}
			}
		}

		#endregion

		public void EnviarEmail(int id, string assunto, string texto) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Enviar email " + id + " - " + assunto + " - " + texto);

					ReuniaoDAO.RegistrarEmail(id, assunto, texto, transaction, db);

					ReuniaoVO vo = ReuniaoDAO.GetById(id, db, transaction);
					ConselhoVO conselho = ConselhoDAO.ListarConselhos(db).First(x => x.Codigo.Equals(vo.CodConselho));
					List<UsuarioVO> lstUsuarios = ConselhoDAO.ListarUsuariosByConselho(vo.CodConselho, db);

					EmailUtil.Reuniao.SendReuniao(vo, conselho, lstUsuarios);

					transaction.Commit();
				}
				catch {
					transaction.Rollback();
					throw;
				}
				finally {
					connection.Close();
				}
			}
		}

	}
}
