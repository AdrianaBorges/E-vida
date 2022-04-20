using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailViagem : EmailProvider {

		#region Viagem

		internal static void SendViagemCriacao(SolicitacaoViagemVO vo, EmpregadoEvidaVO responsavel) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$DESTINATARIO$$", responsavel.Nome);
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));

			string body = GenerateEmailBody(EmailType.ENVIO_VIAGEM_CRIACAO, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(CreateMailAddress(responsavel.Email, responsavel.Nome));

			GenericSendEmail(EmailType.ENVIO_VIAGEM_CRIACAO, col, body);
		}

		internal static void SendViagemCriacaoExterno(SolicitacaoViagemVO vo, List<EmpregadoEvidaVO> diretores) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$DESTINATARIO$$", "Diretor");
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));

			string body = GenerateEmailBody(EmailType.ENVIO_VIAGEM_CRIACAO, parms);

			MailAddressCollection col = new MailAddressCollection();
			foreach (EmpregadoEvidaVO diretor in diretores) {
				if (!string.IsNullOrEmpty(diretor.Email))
					col.Add(CreateMailAddress(diretor.Email, diretor.Nome));	
			}
			

			GenericSendEmail(EmailType.ENVIO_VIAGEM_CRIACAO, col, body);
		}

		internal static void SendViagemSolAprovNegarCoordenador(SolicitacaoViagemVO vo, UsuarioVO usuario, bool aprovado, string mensagem) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));
			if (!aprovado) {
				parms.Add("$$MOTIVO$$", mensagem);
			}

			EmailType tipo = aprovado ? EmailType.ENVIO_VIAGEM_SOL_APROV_COORDENADOR : EmailType.ENVIO_VIAGEM_SOL_NEGAR_COORDENADOR;

			string body = GenerateEmailBody(tipo, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(CreateMailAddress(usuario.Email, usuario.Nome));

			GenericSendEmail(tipo, col, body);
		}

		internal static void SendViagemSolAprovNegarDiretoria(SolicitacaoViagemVO vo, UsuarioVO usuario, bool aprovado, string mensagem) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));
			if (!aprovado) {
				parms.Add("$$MOTIVO$$", mensagem);
			}

			EmailType tipo = aprovado ? EmailType.ENVIO_VIAGEM_SOL_APROV_DIRETORIA : EmailType.ENVIO_VIAGEM_SOL_NEGAR_DIRETORIA;

			string body = GenerateEmailBody(tipo, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(CreateMailAddress(usuario.Email, usuario.Nome));

			GenericSendEmail(tipo, col, body);
		}

		internal static void SendViagemPcAprovNegarFinanceiro(SolicitacaoViagemVO vo, UsuarioVO usuario, bool aprovado, string mensagem) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));
			if (!aprovado) {
				parms.Add("$$MOTIVO$$", mensagem);
			}

			EmailType tipo = aprovado ? EmailType.ENVIO_VIAGEM_PC_APROV_FINANCEIRO : EmailType.ENVIO_VIAGEM_PC_NEGAR_FINANCEIRO;

			string body = GenerateEmailBody(tipo, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(CreateMailAddress(usuario.Email, usuario.Nome));

			GenericSendEmail(tipo, col, body);
		}

        internal static void SendViagemPcAprovNegarDiretoria(SolicitacaoViagemVO vo, UsuarioVO usuario, bool aprovado, string mensagem)
        {
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
            parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));
            if (!aprovado)
            {
                parms.Add("$$MOTIVO$$", mensagem);
            }

            EmailType tipo = aprovado ? EmailType.ENVIO_VIAGEM_PC_APROV_DIRETORIA : EmailType.ENVIO_VIAGEM_PC_NEGAR_DIRETORIA;

            string body = GenerateEmailBody(tipo, parms);

            MailAddressCollection col = new MailAddressCollection();
            col.Add(CreateMailAddress(usuario.Email, usuario.Nome));

            GenericSendEmail(tipo, col, body);
        }

		internal static void SendViagemPcPendenteDiretoria(SolicitacaoViagemVO vo, List<EmpregadoEvidaVO> diretores, byte[] relatorio) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$DESTINATARIO$$", "Diretor");
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));

			EmailData dados = new EmailData(EmailType.ENVIO_VIAGEM_PC_PENDENTE_DIRETORIA);
			string body = GenerateEmailBody(dados.Tipo, parms);
			dados.Body = body;

			foreach (EmpregadoEvidaVO diretor in diretores) {
				if (!string.IsNullOrEmpty(diretor.Email))
					dados.AddTo(CreateMailAddress(diretor.Email, diretor.Nome));
			}

			dados.Add(CreatePdfAttachment(relatorio, "RELATORIO_VIAGEM_" + vo.Id));
			GenericSendEmail(dados);
		}

        internal static void SendViagemRealizarCompra(SolicitacaoViagemVO vo, List<UsuarioVO> lstSecretaria)
        {
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));

            EmailType tipo = EmailType.ENVIO_VIAGEM_COMPRA_PASSAGEM;
            string body = GenerateEmailBody(tipo, parms);

            MailAddressCollection col = new MailAddressCollection();

            // Adiciona o e-mail da secretaria
            AddUusarios(col, lstSecretaria);

            // Adiciona o e-mail do financeiro
            col.Add(GetMailToFinanceiro());

            // Adiciona o e-mail do solicitante
            UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioSolicitante);
            if (!string.IsNullOrEmpty(usuario.Email))
                col.Add(CreateMailAddress(usuario.Email, usuario.Nome));

            GenericSendEmail(tipo, col, body);
        }

		internal static void SendViagemCompraEfetuada(SolicitacaoViagemVO vo) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));

			EmailType tipo = EmailType.ENVIO_VIAGEM_COMPRA_EFETUADA;
			string body = GenerateEmailBody(tipo, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(GetMailToFinanceiro());

			GenericSendEmail(tipo, col, body);
		}

		internal static void SendViagemFinanceiroPago(SolicitacaoViagemVO vo, UsuarioVO usuario) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO));
			parms.Add("$$STATUS$$", SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao));

			EmailType tipo = EmailType.ENVIO_VIAGEM_FINANCEIRO_COMPROVANTE;
			string body = GenerateEmailBody(tipo, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(CreateMailAddress(usuario.Email, usuario.Nome));

			GenericSendEmail(tipo, col, body);
		}
		
		#endregion

	}
}
