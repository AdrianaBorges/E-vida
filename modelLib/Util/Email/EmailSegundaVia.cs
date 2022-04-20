using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailSegundaVia : EmailProvider {

		#region Segunda Via

		internal static void SendEmailSegVia(SolicitacaoSegViaCarteiraVO vo, PUsuarioVO funcVO, byte[] anexo) {
			string nome = funcVO.Nomusr;

			if (string.IsNullOrEmpty(funcVO.Email)) {
				LogNotEmailFuncionario(EmailType.SEGUNDA_VIA_CARTEIRA, funcVO);
				return;
			}
			EmailData dados = new EmailData(EmailType.SEGUNDA_VIA_CARTEIRA);
			
			MailAddress toAddress = new MailAddress(funcVO.Email, nome);
			dados.AddTo(toAddress);

			string arquivo = "SOL_2aVia_" + vo.CdSolicitacao;
			dados.Add(CreatePdfAttachment(anexo, arquivo));			

			GenericSendEmail(dados);
		}

        internal static void SendCancelamentoSegViaCarteiraFuncionario(SolicitacaoSegViaCarteiraVO vo, PUsuarioVO funcVO)
        {
			string nome = funcVO.Nomusr;

			if (string.IsNullOrEmpty(funcVO.Email)) {
				LogNotEmailFuncionario(EmailType.SEGUNDA_VIA_CARTEIRA, funcVO);
				return;
			}

			MailAddress toAddress = new MailAddress(funcVO.Email.ToLower(), nome);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.CdSolicitacao.ToString());
			parms.Add("$$MOTIVO$$", vo.MotivoCancelamento);

			string body = GenerateEmailBody(EmailType.ENVIO_SEGUNDA_VIA_CANCEL, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(toAddress);

			GenericSendEmail(EmailType.ENVIO_SEGUNDA_VIA_CANCEL, col, body);
		}

		#endregion
	}
}
