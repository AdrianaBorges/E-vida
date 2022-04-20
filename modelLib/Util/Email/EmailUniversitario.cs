using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailUniversitario : EmailProvider{

		#region Universitario
		internal static void SendAprovacaoDeclaracaoUniversitario(DeclaracaoUniversitarioVO vo, PFamiliaVO funcVO) {

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(funcVO.Codint, funcVO.Codemp, funcVO.Matric);

            string nome = titular.Nomusr;

            if (string.IsNullOrEmpty(titular.Email))
            {
                LogNotEmailFuncionario(EmailType.ENVIO_UNIVERSITARIO_APROV, titular);
				return;
			}

            MailAddress toAddress = new MailAddress(titular.Email.ToLower(), nome);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.CodSolicitacao.ToString());

			string body = GenerateEmailBody(EmailType.ENVIO_UNIVERSITARIO_APROV, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(toAddress);

			GenericSendEmail(EmailType.ENVIO_UNIVERSITARIO_APROV, col, body);
		}

		internal static void SendCancelamentoDeclaracaoUniversitario(DeclaracaoUniversitarioVO vo, PFamiliaVO funcVO) {

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(funcVO.Codint, funcVO.Codemp, funcVO.Matric);

            string nome = titular.Nomusr;

            if (string.IsNullOrEmpty(titular.Email))
            {
                LogNotEmailFuncionario(EmailType.ENVIO_UNIVERSITARIO_CANCEL, titular);
				return;
			}

            MailAddress toAddress = new MailAddress(titular.Email.ToLower(), nome);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.CodSolicitacao.ToString());
			parms.Add("$$MOTIVO$$", vo.MotivoCancelamento);

			string body = GenerateEmailBody(EmailType.ENVIO_UNIVERSITARIO_CANCEL, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(toAddress);

			GenericSendEmail(EmailType.ENVIO_UNIVERSITARIO_CANCEL, col, body);
		}
		#endregion

	}
}
