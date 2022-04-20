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
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class FormNegativaBO {
		EVidaLog log = new EVidaLog(typeof(FormNegativaBO));

		private static FormNegativaBO instance = new FormNegativaBO();

		public static FormNegativaBO Instance { get { return instance; } }
		
		public void Salvar(FormNegativaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
                    log.Debug("Salvando solicitação. Codint: " + vo.Codint + ", Codemp: " + vo.Codemp + ", Matric: " + vo.Matric + ", Tipreg: " + vo.Tipreg + " Solicitacao: " + vo.CodSolicitacao);

				FormNegativaDAO.Salvar(vo, db);

				connection.Commit();
			}
		}

		public void Salvar(FormNegativaReanaliseVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Salvando reanálise. Solicitacao: " + vo.Id);

				FormNegativaDAO.Salvar(vo, db);

				connection.Commit();
			}
		}

		public DataTable Pesquisar(string nomeBeneficiario, int cdProtocolo, string protocoloAns, string tipoRede, string cdMascara, string dsServico,
			FormNegativaStatus? status, FormNegativaReanaliseStatus? statusReanalise) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return FormNegativaDAO.Pesquisar(nomeBeneficiario, cdProtocolo, protocoloAns, tipoRede, cdMascara, dsServico, status, statusReanalise, db);
			}
		}

        public List<FormNegativaVO> ListByBeneficiario(string codint, string codemp, string matric, string tipreg)
        {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
                return FormNegativaDAO.ListByBeneficiario(codint, codemp, matric, tipreg, db);
			}
		}

		public FormNegativaVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return FormNegativaDAO.GetById(id, db);
			}
		}

		public FormNegativaInfoAdicionalVO GetInfoAdicional(FormNegativaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();

                PUsuarioVO benefVO = PUsuarioDAO.GetRowById(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);

				if (benefVO == null) return null;

				FormNegativaInfoAdicionalVO infoVO = new FormNegativaInfoAdicionalVO();
				infoVO.NomeBeneficiario = benefVO.Nomusr;
				infoVO.Cartao = benefVO.Matant;
                infoVO.Cpf = benefVO.Cpfusr != null ? FormatUtil.FormatCpf(benefVO.Cpfusr) : "";
                infoVO.DataNascimento = (benefVO.Datnas == null) ? DateTime.MinValue : DateTime.ParseExact(benefVO.Datnas, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                PFamiliaProdutoVO benefPlanoVO = PFamiliaProdutoDAO.GetByFamiliaData(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, vo.DataCriacao.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture), db.Database);
				if (benefPlanoVO != null) {

                    // Solicitado pelo Sr. Eurico, do Cadastro
                    // Se o plano da família for o 0001 e o plano do usuário for o 0004, a consulta considera o plano do usuário. Para os demais casos, a consulta considera o plano da família.
                    PUsuarioVO usuario = PUsuarioDAO.GetRowById(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);
                    if (usuario != null)
                    {
                        if (benefPlanoVO.Codpla != null && usuario.Codpla != null)
                        {
                            if (benefPlanoVO.Codpla == "0001" && usuario.Codpla == "0004")
                            {
                                benefPlanoVO.Codpla = "0004";
                            }
                        }
                    }

                    infoVO.DataAdesao = (benefPlanoVO.Datcar == null) ? DateTime.MinValue : DateTime.ParseExact(benefPlanoVO.Datcar, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

					PProdutoSaudeVO planoVO = PLocatorDataBO.Instance.GetProdutoSaude(benefPlanoVO.Codpla);
					infoVO.Plano = planoVO;
				}

				infoVO.NrContrato = FormNegativaDAO.GetNroContrato(benefVO, db);

				return infoVO;
			}
		}

		public void Cancelar(int id, string motivo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Cancelando solicitação. Solicitacao: " + id);

				FormNegativaDAO.Cancelar(id, motivo, idUsuario, db);

				connection.Commit();
			}
		}

		public void Aprovar(int id, int idUsuario, bool reanalise) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Aprovando solicitação. Solicitacao: " + id + " Reanalise: " + reanalise);

				string assinatura = UsuarioBO.Instance.GetAssinatura(idUsuario);
				if (string.IsNullOrEmpty(assinatura)) {
					throw new EvidaException("O usuário não possui assinatura cadastrada!");
				}

				FormNegativaVO vo = FormNegativaDAO.GetById(id, db);
				FormNegativaDAO.Finalizar(id, idUsuario, reanalise, db);

				EnviarEmailAprovacao(vo, reanalise);

				connection.Commit();
			}
		}

		public FormNegativaVO DevolverReanalise(int id, int idUsuario, string motivo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Devolvendo reanálise. Solicitacao: " + id);
				
				FormNegativaVO vo = FormNegativaDAO.GetById(id, db);
				FormNegativaDAO.DevolverReanalise(id, idUsuario, motivo, db);

				vo = FormNegativaDAO.GetById(id, db);
				UsuarioVO criador = UsuarioDAO.GetUsuarioById(vo.Reanalise.IdUsuario, db);

				EmailUtil.Negativa.SendEmailReanaliseDev(vo, criador);

				connection.Commit();
				return vo;
			}
		}

		private void EnviarEmailAprovacao(FormNegativaVO vo, bool reanalise) {
			EmailUtil.Negativa.SendEmailAprovNegativa(vo, reanalise);
		}

        public void EnviarEmailSolicitacao(FormNegativaVO vo, bool reanalise)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();

                List<UsuarioVO> lst = UsuarioDAO.GetUsuariosByModulo(Modulo.RECEBER_EMAIL_NEGATIVA, db);

                EmailUtil.Negativa.SendEmailNegativa(vo, reanalise, lst);
            }
        }	

		public MotivoGlosaVO GetMotivoGlosaById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return FormNegativaDAO.GetMotivoById(id, db);
			}
		}

		public List<string> ListarGruposMotivo() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return FormNegativaDAO.ListarGruposMotivo(db);
			}
		}

		public List<MotivoGlosaVO> BuscarMotivoGlosa(int? codigo, string grupo, string descricao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return FormNegativaDAO.BuscarMotivosGlosa(codigo, grupo, descricao, db);
			}
		}

	}
}
