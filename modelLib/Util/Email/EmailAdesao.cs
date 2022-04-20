using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailAdesao : EmailProvider {

		#region Adesao
		internal static void SendAdesaoValidacao(VO.Adesao.DeclaracaoVO vo, bool isValido, string motivo) {
			if (string.IsNullOrEmpty(vo.Email)) return;

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Numero.ToString(VO.Adesao.DeclaracaoVO.FORMATO_PROTOCOLO));
			parms.Add("$$RESPOSTA$$", isValido ? "VALIDADA" : "INVALIDADA");
			parms.Add("$$OBSERVACAO$$", motivo);

			string body = GenerateEmailBody(EmailType.ENVIO_ADESAO_VALIDACAO, parms);

			MailAddressCollection col = new MailAddressCollection();

			//col.Add(new MailAddress(vo.Email, vo.NomeRequerente));
			col.Add(GetMailToCadastro());

			GenericSendEmail(EmailType.ENVIO_ADESAO_VALIDACAO, col, body);
		}
		#endregion

	}
}
