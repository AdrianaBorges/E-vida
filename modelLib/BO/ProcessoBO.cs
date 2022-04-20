using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.BO {
	public class ProcessoBO {
		private static ProcessoBO instance = new ProcessoBO();

		public static ProcessoBO Instance { get { return instance; } }

		public long RegistrarProcesso(ControleProcessoEnum processo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();

				try {
					ControleProcessoDAO.AbortarAnterior(processo, transaction, db);
					long id = ControleProcessoDAO.RegistrarProcesso(processo, transaction, db);

					transaction.Commit();
					return id;
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

		public ControleProcessoVO GetAnteriorSucesso(ControleProcessoEnum processo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					return ControleProcessoDAO.GetAnteriorSucesso(processo, db);	
				}
				finally {
					connection.Close();
				}
			}
		}
		
		public void SucessoProcesso(long idProcesso, ControleProcessoEnum processo, int qtd, string parametros) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();

				try {
					ControleProcessoDAO.FinalizarProcesso(idProcesso, processo, qtd, StatusControleProcesso.SUCESSO, parametros, transaction, db);
					transaction.Commit();
				}
				catch (Exception ex) {
					transaction.Rollback();
					throw ex;
				}
				finally {
					connection.Close();
				}
			}
		}

		public void ErroProcesso(long idProcesso, ControleProcessoEnum processo, Exception ex) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();

				try {
					ControleProcessoDAO.FinalizarProcesso(idProcesso, processo, 0, StatusControleProcesso.ERRO, ex.Message, transaction, db);
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
