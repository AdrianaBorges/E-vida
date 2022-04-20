using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class FormExclusaoBO {
		EVidaLog log = new EVidaLog(typeof(FormExclusaoBO));

		private static FormExclusaoBO instance = new FormExclusaoBO();

		public static FormExclusaoBO Instance { get { return instance; } }

		public DataTable BuscarExclusao(string codint, string codemp, string matric) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
                return ExclusaoDAO.BuscarExclusao(codint, codemp, matric, db);
			}
		}

		public void Salvar(ExclusaoVO vo, List<ExclusaoBenefVO> lst, string email) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando solicitação " + vo.Protocolo + vo.Codint + " - " + vo.Codemp + " - " + vo.Matric + " - " +
                              vo.Protocolo + " - "+ email + " - intra: " + vo.CodUsuarioAlteracao);

				PFamiliaVO funcionario = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);
				if (funcionario == null)
					throw new InvalidOperationException("Funcionário inexistente");

                PUsuarioVO titular = PUsuarioDAO.GetTitular(vo.Codint, vo.Codemp, vo.Matric, db);

                if (!email.Equals(titular.Email, StringComparison.InvariantCultureIgnoreCase))
                {
                    PUsuarioDAO.AlterarEmailTitular(vo.Codint, vo.Codemp, vo.Matric, email, db);
				}

				if (lst == null || lst.Count == 0) {
					throw new Exception("Não há beneficiários selecionados!");
				}

				ExclusaoDAO.Salvar(vo, lst, db);

				connection.Commit();
			}
		}

		public ExclusaoVO GetById(int idSolicitacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();

				ExclusaoVO vo = ExclusaoDAO.GetById(idSolicitacao, db);
				return vo;
			}
		}

		public DataTable BuscarBeneficiarios(int idSolicitacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ExclusaoDAO.BuscarBeneficiarios(idSolicitacao, db);
			}
		}

		public void Aprovar(int cdProtocolo, int idUsuario, byte[] anexo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Aprovando solicitação. Solicitacao: " + cdProtocolo);

				ExclusaoDAO.Aprovar(cdProtocolo, idUsuario, db);

				ExclusaoVO vo = ExclusaoDAO.GetById(cdProtocolo, db);
                PFamiliaVO funcVO = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);

				EnviarEmailAprovacao(vo, funcVO, anexo);

				connection.Commit();
			}
		}

		public void EnviarEmailAprovacao(ExclusaoVO vo, PFamiliaVO funcVO, byte[] anexo) {
			EmailUtil.Exclusao.SendAprovacaoExclusaoFuncionario(vo, funcVO);
			EmailUtil.Exclusao.SendAprovacaoExclusaoFinanceiro(vo, funcVO, anexo);
			
		}

		public DataTable Pesquisar(long? matricula, int? cdProtocolo, StatusExclusao? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ExclusaoDAO.Pesquisar(matricula, cdProtocolo, status, db);
			}
		}

		public void Cancelar(int cdProtocolo, string motivo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Cancelando solicitação. Solicitacao: " + cdProtocolo);

				ExclusaoDAO.Cancelar(cdProtocolo, motivo, idUsuario, db);

				ExclusaoVO vo = ExclusaoDAO.GetById(cdProtocolo, db);
                PFamiliaVO funcVO = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);

				EmailUtil.Exclusao.SendCancelamentoExclusaoFuncionario(vo, funcVO);

				connection.Commit();
			}
		}

		public void AguardarDocumentacao(int cdProtocolo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Aguardando documentacao. Solicitacao: " + cdProtocolo);

				ExclusaoDAO.AguardarDocumentacao(cdProtocolo, idUsuario, db);

				connection.Commit();
			}

		}
	}
}
