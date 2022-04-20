using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.SCL;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.SCL;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;
using System.DirectoryServices;

namespace eVidaGeneralLib.BO {
	public class UsuarioBO {
		private static UsuarioBO instance = new UsuarioBO();

		public static UsuarioBO Instance { get { return instance; } }

		private const string CACHE_USUARIO = "CACHE_USUARIOS";

		public UsuarioVO LogarFuncionario(string login, string senha) {
			UsuarioVO objReturn = null;

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

                try {
                    DirectoryEntry objAD = new DirectoryEntry("LDAP://10.0.0.6", login, senha);
                    string nome = objAD.Name;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw new DirectoryServicesCOMException(ex.Message);
                }
                
                //SclUsuarioVO vo = SclUsuarioDAO.Logar(login, senha, db);

				//if (vo == null)
				//	return null;
				//else {
					objReturn = UsuarioDAO.GetByLogin(login, db);
					if (objReturn == null) {
						objReturn = new UsuarioVO();
						objReturn.Login = login;
						return objReturn;
					}
				//}

				UsuarioDAO.RegistrarLogin(objReturn.Id, db);
				connection.Commit();

			}
			return objReturn;
		}

		public List<Modulo> ListarModulosUsuario(int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.ListarModulosUsuario(idUsuario, db);
			}
		}

		public DataTable PesquisarUsuarios(string login, string nome, int? idPerfil, string idRegional) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.PesquisarUsuarios(login, nome, idPerfil, idRegional, db);
			}
		}

		public DataTable PesquisarUsuariosInterno(string login, string nome, int? idPerfil) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.PesquisarUsuariosInterno(login, nome, idPerfil, db);
			}
		}

		public List<UsuarioVO> ListarUsuarios() {
			List<UsuarioVO> lst = CacheHelper.GetFromCache<List<UsuarioVO>>(CACHE_USUARIO);
			if (lst == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					lst = UsuarioDAO.ListarUsuarios(db);
					CacheHelper.AddOnCache(CACHE_USUARIO, lst);
				}
			}
			return lst;
		}

		public UsuarioVO GetUsuarioById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.GetUsuarioById(id, db);
			}
		}

		public void SalvarUsuario(UsuarioVO vo, List<Perfil> lstPerfil) {
			CacheHelper.RemoveFromCache(CACHE_USUARIO);

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				
				UsuarioVO old = UsuarioDAO.GetByLogin(vo.Login, db);
				if (old != null)
					vo.Id = old.Id;
				UsuarioDAO.SalvarUsuario(vo, lstPerfil, db);

				connection.Commit();
			}
		}

		public void RemoverUsuario(UsuarioVO vo) {
			CacheHelper.RemoveFromCache(CACHE_USUARIO);

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				UsuarioVO old = UsuarioDAO.GetByLogin(vo.Login, db);
				if (old != null) {
					vo.Id = old.Id;
					UsuarioDAO.RemoverUsuario(vo, db);
				}
				connection.Commit();
			}
		}

		public string GetAssinatura(int idUsuario) {
			string fileName = FileUtil.GetFileFromPathWithoutExtension(FileUtil.FileDir.ASSINATURA, idUsuario.ToString(), "ass");
			return fileName;
		}

		public void AlterarAssinatura(string login, string diskFile) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				UsuarioVO old = UsuarioDAO.GetByLogin(login, db);
				if (old == null)
					throw new InvalidOperationException("Apenas usuários cadastrados podem ter assinatura!");

				string prefix = UploadConfigManager.GetPrefix(UploadFilePrefix.ASSINATURA, Sistema.INTRANET, old.Id.ToString());
				string oldFile = FileUtil.GetFileFromPathWithoutExtension(FileUtil.FileDir.ASSINATURA, old.Id + "", "ass");
				string dir = FileUtil.GetRepositoryDir(FileUtil.FileDir.ASSINATURA);
				string oldExtension = null;
				string newExtension = System.IO.Path.GetExtension(diskFile);

				if (!string.IsNullOrEmpty(oldFile)) {
					oldExtension = System.IO.Path.GetExtension(oldFile);
					if (!newExtension.Equals(oldExtension, StringComparison.InvariantCultureIgnoreCase)) {
						System.IO.File.Delete(oldFile);
					}
				}

				FileUtil.MoverArquivo(old.Id.ToString(), null, diskFile, dir, "ass" + newExtension);

				connection.Commit();
			}
		}

		public bool AlreadyContainsLogin(string login, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				UsuarioVO vo = UsuarioDAO.GetByLogin(login, db);
				if (vo == null)
					return false;
				if (vo.Id == idUsuario)
					return false;
				return true;
			}
		}

		public UsuarioVO GetUsuarioByLogin(string login) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.GetByLogin(login, db);
			}
		}

		public List<UsuarioVO> GetUsuariosByModulo(Modulo modulo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.GetUsuariosByModulo(modulo, db);
			}
		}

		public List<Perfil> GetPerfilByUsuario(int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return UsuarioDAO.GetPerfilByUsuario(idUsuario, db);
			}
		}
		
		#region SCL

		public SclUsuarioVO GetUsuarioScl(string login) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				SclUsuarioVO vo = SclUsuarioDAO.GetUsuarioScl(login, db);
				if (vo != null) {
					vo.Dominios = SclUsuarioDAO.GetDominios(vo.Login, db);
					vo.Perfis = SclUsuarioDAO.GetPerfis(vo.Login, db);
				}
				return vo;
			}
		}

		public DataTable PesquisarUsuariosScl(string login, string nome) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return SclUsuarioDAO.PesquisarUsuariosScl(login, nome, db);
			}
		}

		public void AlterarUsuarioScl(string login, string senha, DateTime? dtExpiracao, bool isAtivo, UsuarioVO usuarioLogado) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				SclUsuarioDAO.AlterarUsuario(login, senha, dtExpiracao, isAtivo, usuarioLogado, db);
				connection.Commit();
			}
		}

		public SclUsuarioVO GetUsuarioSclByDominio(SclUsuarioDominio dominio, string vlDominio) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return SclUsuarioDAO.GetUsuarioSclByDominio(dominio, vlDominio, db);
			}
		}

		public void CriarUsuarioScl(SclUsuarioVO vo, string senha, UsuarioVO usuarioLogado) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				SclUsuarioDAO.CriarUsuarioScl(vo, senha, usuarioLogado, db);
				connection.Commit();
			}
		}

		#endregion

	}
}
