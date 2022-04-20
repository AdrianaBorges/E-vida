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
	public class EmailExclusao : EmailProvider{


		#region Exclusao
		internal static void SendAprovacaoExclusaoFuncionario(ExclusaoVO vo, PFamiliaVO funcVO) {

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(funcVO.Codint, funcVO.Codemp, funcVO.Matric);

            string nome = titular.Nomusr;

			if (string.IsNullOrEmpty(titular.Email)) {
				LogNotEmailFuncionario(EmailType.ENVIO_EXCLUSAO_APROV_FUNCIONARIO, titular);
				return;
			}

			MailAddress toAddress = new MailAddress(titular.Email.ToLower(), nome);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.CodSolicitacao.ToString());

			string body = GenerateEmailBody(EmailType.ENVIO_EXCLUSAO_APROV_FUNCIONARIO, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(toAddress);

			GenericSendEmail(EmailType.ENVIO_EXCLUSAO_APROV_FUNCIONARIO, col, body);
		}

        internal static void SendAprovacaoExclusaoFinanceiro(ExclusaoVO vo, PFamiliaVO funcVO, byte[] anexo)
        {
            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(funcVO.Codint, funcVO.Codemp, funcVO.Matric);

            string nome = titular.Nomusr;

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.CodSolicitacao.ToString());

			EmailData dados = new EmailData(EmailType.ENVIO_EXCLUSAO_APROV_FINANCEIRO);

			string body = GenerateEmailBody(EmailType.ENVIO_EXCLUSAO_APROV_FINANCEIRO, parms);
			dados.Body = body;
			
			dados.AddTo(GetMailToFinanceiro());
			
			string arquivo = "SOL_EXCLUSAO_" + vo.CodSolicitacao;
			dados.Add(CreatePdfAttachment(anexo, arquivo));

			GenericSendEmail(dados);
		}

		internal static void SendCancelamentoExclusaoFuncionario(ExclusaoVO vo, PFamiliaVO funcVO) {
            
            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(funcVO.Codint, funcVO.Codemp, funcVO.Matric);

            string nome = titular.Nomusr;

            if (string.IsNullOrEmpty(titular.Email))
            {
				LogNotEmailFuncionario(EmailType.ENVIO_EXCLUSAO_CANCELAMENTO, titular);
				return;
			}

            MailAddress toAddress = new MailAddress(titular.Email.ToLower(), nome);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.CodSolicitacao.ToString());
			parms.Add("$$MOTIVO$$", vo.MotivoCancelamento);

			string body = GenerateEmailBody(EmailType.ENVIO_EXCLUSAO_CANCELAMENTO, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(toAddress);

			GenericSendEmail(EmailType.ENVIO_EXCLUSAO_CANCELAMENTO, col, body);
		}
		#endregion
		
	}
}
