using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailNegativa : EmailProvider {
		#region Negativa
		internal static void SendEmailNegativa(FormNegativaVO vo, bool reanalise, List<UsuarioVO> usuarios) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO));

			EmailData emailData = new EmailData(reanalise ? EmailType.ENVIO_REANALISE_NEGATIVA_SOL : EmailType.SOLICITACAO_NEGATIVA);
			emailData.Body = GenerateEmailBody(emailData.Tipo, parms);

			foreach (UsuarioVO u in usuarios) {
				emailData.AddTo(new MailAddress(u.Email, u.Nome));
			}

			GenericSendEmail(emailData);
		}

		internal static void SendEmailAprovNegativa(FormNegativaVO vo, bool reanalise) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO));

			EmailData emailData = new EmailData(reanalise ? EmailType.ENVIO_REANALISE_NEGATIVA_APROV : EmailType.ENVIO_NEGATIVA_APROV);
			emailData.Body = GenerateEmailBody(emailData.Tipo, parms);

			emailData.AddTo(GetMailToAutoriza());

			GenericSendEmail(emailData);
		}
		internal static void SendEmailReanaliseDev(FormNegativaVO vo, UsuarioVO usuario) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO));
			parms.Add("$$MOTIVO_DEVOLUCAO$$", vo.Reanalise.ObservacaoDevolucao);

			EmailData emailData = new EmailData(EmailType.ENVIO_REANALISE_NEGATIVA_DEV);
			emailData.Body = GenerateEmailBody(emailData.Tipo, parms);

			emailData.AddTo(GetMailTo(usuario));

			GenericSendEmail(emailData);
		}
		#endregion
	}
}
