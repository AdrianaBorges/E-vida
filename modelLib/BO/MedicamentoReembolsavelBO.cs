using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class MedicamentoReembolsavelBO {
		EVidaLog log = new EVidaLog(typeof(MedicamentoReembolsavelBO));

		private static MedicamentoReembolsavelBO instance = new MedicamentoReembolsavelBO();

		public static MedicamentoReembolsavelBO Instance { get { return instance; } }

		public MedicamentoReembolsavelVO GetById(string cdMascara) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return MedicamentoReembolsavelDAO.GetById(cdMascara, db);
			}
		}

        public DataTable Pesquisar(string cdMascara, string descricao, string reembolsavel, string continuo, string plano)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return MedicamentoReembolsavelDAO.Pesquisar(cdMascara, descricao, reembolsavel, continuo, plano, false, db);
            }
        }

		public void Salvar(MedicamentoReembolsavelVO vo, bool replicar, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				MedicamentoReembolsavelDAO.Salvar(vo, idUsuario, db);

				if (replicar) {
					MedicamentoReembolsavelDAO.Replicar(vo, idUsuario, db);
				}

				connection.Commit();
			}
		}

		#region Principio Ativo

		public DataTable PesquisarPrincipio(int? codigo, string descricao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PrincipioAtivoDAO.Pesquisar(codigo, descricao, db);
			}
		}

		public PrincipioAtivoVO GetPrincipioById(int idPrincipio) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PrincipioAtivoDAO.GetById(idPrincipio, db);
			}
		}

		public void CriarPrincipio(PrincipioAtivoVO vo, int idUsuario) {

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				List<PrincipioAtivoVO> lst = PrincipioAtivoDAO.GetByDescricao(vo.Descricao, db);
				if (lst != null && lst.Count > 0) {
					throw new EvidaException("Já existe um princípio ativo com esta descrição!");
				}

				PrincipioAtivoDAO.Salvar(vo, idUsuario, db);

				connection.Commit();
			}
		}

		public void RemoverPrincipioAtivo(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				List<MedicamentoReembolsavelVO> lstMed = MedicamentoReembolsavelDAO.ListByPrincipioAtivo(id, db);
				if (lstMed != null && lstMed.Count > 0) {
					throw new EvidaException("O princípio possui medicamentos associados. Não pode ser excluído!");
				}

				PrincipioAtivoDAO.Excluir(id, db);

				connection.Commit();
			}
		}

		#endregion

		#region Arquivos

		public List<MedicamentoReembolsavelArqVO> ListarArquivos(string cdMascara) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return MedicamentoReembolsavelDAO.ListarArquivos(cdMascara, db);
			}
		}

		private List<MedicamentoReembolsavelArqVO> SalvarArquivos(string cdMascara, int idUsuario, List<ArquivoTelaVO> lstArquivos, EvidaDatabase db) {
			List<MedicamentoReembolsavelArqVO> lstNewFiles = PrepareFiles(cdMascara, idUsuario, lstArquivos);
			MedicamentoReembolsavelDAO.CriarArquivos(cdMascara, idUsuario, lstNewFiles, db);
			MoveFiles(cdMascara, lstArquivos);
			return lstNewFiles;
		}

		private List<MedicamentoReembolsavelArqVO> PrepareFiles(string cdMascara, int idUsuario, List<ArquivoTelaVO> lstArquivos) {
			List<MedicamentoReembolsavelArqVO> lstNewFiles = new List<MedicamentoReembolsavelArqVO>();
			foreach (ArquivoTelaVO arq in lstArquivos) {
				MedicamentoReembolsavelArqVO fileVO = new MedicamentoReembolsavelArqVO();
				if (!string.IsNullOrEmpty(arq.Id)) {
					fileVO.IdArquivo = Int32.Parse(arq.Id);
				}
				fileVO.Mascara = cdMascara;
				fileVO.NomeArquivo = arq.NomeTela;

				if (arq.IsNew) {
					fileVO.DataEnvio = DateTime.Now;
					fileVO.IdUsuarioEnvio = idUsuario;

					string diskFile = arq.NomeFisico;
					if (!FileUtil.HasTempFile(diskFile)) {
						throw new Exception("Arquivo enviado [" + arq.NomeTela + "] não existe em disco (" + diskFile + ")!");
					}
					lstNewFiles.Add(fileVO);
				}
				arq.InternalVO = fileVO;
			}
			return lstNewFiles;
		}

		public string GetFileDiskId(MedicamentoReembolsavelArqVO arq) {
			return GetFileDiskId(arq.IdArquivo);
		}

		private string GetFileDiskId(ArquivoTelaVO arq) {
			int id = 0;
			if (!string.IsNullOrEmpty(arq.Id)) {
				id = Int32.Parse(arq.Id);
			} else {
				id = ((MedicamentoReembolsavelArqVO)arq.InternalVO).IdArquivo;
			}
			return GetFileDiskId(id);
		}

		private string GetFileDiskId(int idArq) {
			return "MED" + "_" + idArq;
		}

		public string GetFolderId(string cdMascara) {
			return cdMascara;
		}

		private void MoveFiles(string cdMascara, List<ArquivoTelaVO> lstArquivos) {
			string folder = GetFolderId(cdMascara);

			String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.MEDICAMENTO_REEMBOLSAVEL, folder);
			foreach (ArquivoTelaVO arq in lstArquivos) {
				FileUtil.MoverArquivo(GetFileDiskId(arq), null, arq.NomeFisico, dirDestino, arq.NomeTela);
			}
		}

		private void DeleteFiles(string cdMascara, IEnumerable<MedicamentoReembolsavelArqVO> lstDel) {
			string folder = GetFolderId(cdMascara);

			if (lstDel != null && lstDel.Count() > 0) {
				String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.MEDICAMENTO_REEMBOLSAVEL, folder);
				foreach (MedicamentoReembolsavelArqVO arq in lstDel) {
					FileUtil.RemoverArquivo(GetFileDiskId(arq), dirDestino, arq.NomeArquivo);
				}
			}
		}

		public void IncluirArquivoMedicamento(MedicamentoReembolsavelVO medVO, int idUsuario, ArquivoTelaVO arq) {
			if (log.IsDebugEnabled)
				log.Debug("Salvando arquivo " + medVO.Mascara + " - " + arq.NomeTela + " - " + arq.NomeFisico);

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				MedicamentoReembolsavelVO oldVO = MedicamentoReembolsavelDAO.GetById(medVO.Mascara, db);
				if (oldVO == null)
					MedicamentoReembolsavelDAO.Salvar(medVO, idUsuario, db);

				List<ArquivoTelaVO> lst = new List<ArquivoTelaVO>();
				lst.Add(arq);
				SalvarArquivos(medVO.Mascara, idUsuario, lst, db);
				connection.Commit();
			}
		}

		public void RemoverArquivo(MedicamentoReembolsavelArqVO arq) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				RemoverArquivo(arq, true, db);
				connection.Commit();
			}
		}

		private void RemoverArquivo(MedicamentoReembolsavelArqVO arq, bool delete, EvidaDatabase db) {
			if (log.IsDebugEnabled)
				log.Debug("Removendo arquivo " + arq.Mascara + " - " + arq.NomeArquivo);

			if (delete)
				MedicamentoReembolsavelDAO.ExcluirArquivo(arq, db);

			List<MedicamentoReembolsavelArqVO> lst = new List<MedicamentoReembolsavelArqVO>();
			lst.Add(arq);
			DeleteFiles(arq.Mascara, lst);
		}

		#endregion


	}
}
