using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class DeclaracaoUniversitarioBO {
		EVidaLog log = new EVidaLog(typeof(DeclaracaoUniversitarioBO));

		private static DeclaracaoUniversitarioBO instance = new DeclaracaoUniversitarioBO();

		public static DeclaracaoUniversitarioBO Instance { get { return instance; } }

		public DataTable PesquisarDeclaracoes(int? cdProtocolo, string cdFuncionario, DateTime? dtInicial, DateTime? dtFinal, StatusDeclaracaoUniversitario? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return DeclaracaoUniversitarioDAO.PesquisarDeclaracoes(cdProtocolo, cdFuncionario, dtInicial, dtFinal, status, db);
			}
		}

		public void Salvar(DeclaracaoUniversitarioVO vo, string email) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
                    log.Debug("Salvando solicitação " + vo.Codint + " - " + vo.Codemp + " - " + vo.Matric + " - " + email);

				if (!string.IsNullOrEmpty(vo.NomeArquivo)) {
					string serverDir = FileUtil.GetRepositoryDir(FileUtil.FileDir.DECLARACAO_UNIVERSITARIO);
					FileUtil.MoverArquivo(serverDir, vo.NomeArquivo);
				}

				PFamiliaVO funcionario = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);
				if (funcionario == null)
					throw new InvalidOperationException("Funcionário inexistente");

                PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(funcionario.Codint, funcionario.Codemp, funcionario.Matric);

                if (!titular.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                {
					PUsuarioDAO.AlterarEmailTitular(vo.Codint, vo.Codemp, vo.Matric, email, db);
				}

				DeclaracaoUniversitarioDAO.Salvar(vo, db);

				connection.Commit();
			}
		}

		public DeclaracaoUniversitarioVO GetById(int idSolicitacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioDAO.GetById(idSolicitacao, db);
				return vo;
			}
		}

		public void Aprovar(int cdProtocolo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Aprovando solicitação. Solicitacao: " + cdProtocolo);

				DeclaracaoUniversitarioDAO.Aprovar(cdProtocolo, idUsuario, db);

				DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioDAO.GetById(cdProtocolo, db);
				PFamiliaVO funcVO = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);

				EnviarEmailAprovacao(vo, funcVO);

				connection.Commit();
			}
		}

		public void EnviarEmailAprovacao(DeclaracaoUniversitarioVO vo, PFamiliaVO funcVO) {
			EmailUtil.Universitario.SendAprovacaoDeclaracaoUniversitario(vo, funcVO);
		}

		public void Cancelar(int cdProtocolo, string motivo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Cancelando solicitação. Solicitacao: " + cdProtocolo);

				DeclaracaoUniversitarioDAO.Cancelar(cdProtocolo, motivo, idUsuario, db);

				DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioDAO.GetById(cdProtocolo, db);
				PFamiliaVO funcVO = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);

				EmailUtil.Universitario.SendCancelamentoDeclaracaoUniversitario(vo, funcVO);

				connection.Commit();
			}
		}

	}
}
