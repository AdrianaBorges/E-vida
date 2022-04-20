using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailCanalGestante : EmailProvider {

		#region Canal Gestante
		internal static void SendCanalGestanteBeneficiario(CanalGestanteVO vo, CanalGestanteBenefVO canalBenefVO, VO.Protheus.PUsuarioVO benefVO, string path) {

			if (string.IsNullOrEmpty(canalBenefVO.Email)) return;

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString() + " / " + vo.DataSolicitacao.Year);
			parms.Add("$$NOME_BENEFICIARIO$$", benefVO.Nomusr);

			EmailData dados = new EmailData(EmailType.ENVIO_CANAL_GESTANTE_BENEF);
            dados.AddTo(new MailAddress(canalBenefVO.Email, benefVO.Nomusr));

			dados.Add(CreateGenericAttachment(path));
			
			string body = GenerateEmailBody(EmailType.ENVIO_CANAL_GESTANTE_BENEF, parms);
			dados.Body = body;

			GenericSendEmail(dados);
		}
        internal static void SendCanalGestanteSolEsclarecimentoBenef(CanalGestanteVO vo, CanalGestanteBenefVO canalBenefVO, VO.Protheus.PUsuarioVO benefVO)
        {
			if (string.IsNullOrEmpty(canalBenefVO.Email)) return;

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString() + " / " + vo.DataSolicitacao.Year);
			parms.Add("$$NOME_BENEFICIARIO$$", benefVO.Nomusr);
			parms.Add("$$TIPO_CONTATO$$", vo.TipoContato);
			parms.Add("$$MENSAGEM$$", vo.Pendencia);

			MailAddressCollection col = new MailAddressCollection();
            col.Add(new MailAddress(canalBenefVO.Email, benefVO.Nomusr));

			string body = GenerateEmailBody(EmailType.ENVIO_CANAL_GESTANTE_ESC_BENEF, parms);

			GenericSendEmail(EmailType.ENVIO_CANAL_GESTANTE_ESC_BENEF, col, body);
		}
		#endregion

	}
}
