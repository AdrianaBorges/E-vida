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
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class AutorizacaoProvisoriaBO {
		EVidaLog log = new EVidaLog(typeof(AutorizacaoProvisoriaBO));

		private static AutorizacaoProvisoriaBO instance = new AutorizacaoProvisoriaBO();

		public static AutorizacaoProvisoriaBO Instance { get { return instance; } }

		#region Plantao Social

		public PlantaoSocialLocalVO GetPlantaoById(int id) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return AutorizacaoProvisoriaDAO.GetPlantaoById(id, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public List<PlantaoSocialLocalVO> ListarPlantaoSocialLocal() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return AutorizacaoProvisoriaDAO.ListPlantaoSocialLocal(db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public void Salvar(PlantaoSocialLocalVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando plantão social " + vo.Uf + " - " + vo.Cidade + " - " + vo.Telefone);

					AutorizacaoProvisoriaDAO.Salvar(vo, transaction, db);
					
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

		public void Excluir(PlantaoSocialLocalVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Removendo plantão social " + vo.Uf + " - " + vo.Cidade);

					AutorizacaoProvisoriaDAO.Excluir(vo, transaction, db);

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

		public bool IsLocalUtilizado(PlantaoSocialLocalVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return AutorizacaoProvisoriaDAO.IsLocalUtilizado(vo, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public bool ExistePlantao(string uf, string codMunicipio) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return AutorizacaoProvisoriaDAO.ExistePlantao(uf, codMunicipio, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		#endregion


		#region Autorização

		public AutorizacaoProvisoriaVO GetById(int id) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return AutorizacaoProvisoriaDAO.GetById(id, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable Pesquisar(string matricula, int? cdProtocolo, StatusAutorizacaoProvisoria? status) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return AutorizacaoProvisoriaDAO.Pesquisar(matricula, cdProtocolo, status, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public void Salvar(AutorizacaoProvisoriaVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
                        log.Debug("Salvando autorização " + vo.CodSolicitacao + " - BENEF CODINT:" + vo.Codint + " - BENEF CODEMP:" + vo.Codemp + " - BENEF MATRIC:" + vo.Matric + " - BENEF TIPREG:" + vo.Tipreg);

					AutorizacaoProvisoriaDAO.Salvar(vo, transaction, db);

					//EnviarEmailSolicitacao(vo);

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

        public int SalvarIdentity(AutorizacaoProvisoriaVO vo)
        {
            int codigo = 0;
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();
                try
                {
                    if (log.IsDebugEnabled)
                        log.Debug("Salvando autorização " + vo.CodSolicitacao + " - BENEF CODINT:" + vo.Codint + " - BENEF CODEMP:" + vo.Codemp + " - BENEF MATRIC:" + vo.Matric + " - BENEF TIPREG:" + vo.Tipreg);

                    codigo = AutorizacaoProvisoriaDAO.SalvarIdentity(vo, transaction, db);

                    //EnviarEmailSolicitacao(vo);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return codigo;
        }	

		public void Aprovar(AutorizacaoProvisoriaVO vo, int idUsuario, byte[] anexo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Aprovando autorização " + vo.CodSolicitacao + " - Usuario:" + idUsuario);

					AutorizacaoProvisoriaDAO.Aprovar(vo.CodSolicitacao, idUsuario, transaction, db);

					vo = AutorizacaoProvisoriaDAO.GetById(vo.CodSolicitacao, db);
					PUsuarioVO funcVO = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
					UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(idUsuario);

					EnviarEmailAprovacao(vo, funcVO, usuario, anexo);

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

		public void Cancelar(int cdProtocolo, string motivo, int idUsuario) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Cancelando autorização " + cdProtocolo + " - Usuario:" + idUsuario);

					AutorizacaoProvisoriaDAO.Cancelar(cdProtocolo, motivo, idUsuario, transaction, db);

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

		private void EnviarEmailAprovacao(AutorizacaoProvisoriaVO vo, VO.Protheus.PUsuarioVO benefVO, UsuarioVO usuario, byte[] anexo) {
			EmailUtil.AutorizacaoProvisoria.SendEmailAprovAutorizacaoProvisoria(vo, benefVO, usuario, anexo);
		}

		public void EnviarEmailSolicitacao(AutorizacaoProvisoriaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();

				List<UsuarioVO> lst = UsuarioDAO.GetUsuariosByModulo(Modulo.APROVAR_AUTORIZACAO_PROVISORIA, db);
				EmailUtil.AutorizacaoProvisoria.SendEmailSolAutorizacaoProvisoria(vo, lst);
			}
		}

		#endregion

	}
}
