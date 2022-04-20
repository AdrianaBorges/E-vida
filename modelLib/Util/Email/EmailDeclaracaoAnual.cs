using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailDeclaracaoAnual : EmailProvider {
		#region Declaracao Anual
		internal static string GetMailTitleDeclaracaoAnualDebito() {
			return EmailType.ENVIO_DECLARACAO_ANUAL_DEBITO.GenerateEmailTitle();
		}
		internal static string GetMailSenderDeclaracaoAnualDebito() {
			return EmailType.ENVIO_DECLARACAO_ANUAL_DEBITO.GetSender();
		}

		internal static string CreateBodyDeclaracaoAnualDebito(HcBeneficiarioVO benefVO) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$NOME_BENEFICIARIO$$", benefVO.NmBeneficiario);
			string body = GenerateEmailBody(EmailType.ENVIO_DECLARACAO_ANUAL_DEBITO, parms);
			return body;
		}

		internal static void SendDeclaracaoAnualDebito(DeclaracaoAnualDebitoVO vo, HcBeneficiarioVO benefVO, byte[] anexo) {
			if (string.IsNullOrEmpty(benefVO.Email)) return;

			EmailData dados = new EmailData(EmailType.ENVIO_DECLARACAO_ANUAL_DEBITO);
			dados.AddTo(new MailAddress(benefVO.Email, benefVO.NmBeneficiario));
			
			dados.Add(CreatePdfAttachment(anexo, "DECLARACAO_ANUAL_DEBITO_" + vo.CodBeneficiario + "_" + vo.AnoRef));

			string body = CreateBodyDeclaracaoAnualDebito(benefVO);
			dados.Body = body;

			GenericSendEmail(dados);
		}
		#endregion

	}
}
