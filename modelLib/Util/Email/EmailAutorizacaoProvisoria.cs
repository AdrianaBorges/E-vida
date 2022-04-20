using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailAutorizacaoProvisoria : EmailProvider{

		#region Autorizacao Provisoria
		internal static void SendEmailSolAutorizacaoProvisoria(AutorizacaoProvisoriaVO vo, List<UsuarioVO> usuarios) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.CodSolicitacao.ToString(AutorizacaoProvisoriaVO.FORMATO_PROTOCOLO));
			//parms.Add("$$LINK$$", GenerateDeclaracaoLink(declaracao.Id, declaracao.Produto));

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_PROVISORIA_CRIADA, parms);

			MailAddressCollection col = new MailAddressCollection();
			if (usuarios != null) {
				foreach (UsuarioVO u in usuarios) {
					col.Add(new MailAddress(u.Email, u.Nome));
				}
			}
			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_PROVISORIA_CRIADA, col, body);
		}

		internal static void SendEmailAprovAutorizacaoProvisoria(AutorizacaoProvisoriaVO vo, VO.Protheus.PUsuarioVO benefVO, UsuarioVO usuario, byte[] anexo) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.CodSolicitacao.ToString(AutorizacaoProvisoriaVO.FORMATO_PROTOCOLO));

			EmailData dados = new EmailData(EmailType.ENVIO_AUTORIZACAO_PROVISORIA_APROV);

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_PROVISORIA_APROV, parms);
			dados.Body = body;

			dados.AddTo(GetMailToCadastro());

			MailAddress mailBenef = GetMailTo(benefVO);
			if (mailBenef != null) {
				dados.AddTo(mailBenef);
			}
			MailAddress mailUsuario = GetMailTo(usuario);
			if (mailUsuario != null) {
				dados.AddTo(mailUsuario);
			}

			dados.Add(CreatePdfAttachment(anexo, "SOL_PROVISORIA_" + vo.CodSolicitacao));

			GenericSendEmail(dados);
		}
		#endregion

	}
}
