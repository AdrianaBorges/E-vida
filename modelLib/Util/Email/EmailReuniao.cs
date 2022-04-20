using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailReuniao : EmailProvider{

		internal static void SendReuniao(ReuniaoVO vo, ConselhoVO conselho, List<UsuarioVO> lstUsuarios) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$NM_REUNIAO$$", vo.Titulo);
			parms.Add("$$NM_CONSELHO$$", conselho.Codigo + " - " + conselho.Nome);
			parms.Add("$$DT_REUNIAO$$", vo.Data.ToShortDateString());
			parms.Add("$$TEXTO$$", vo.Email);

			string body = GenerateEmailBody(EmailType.ENVIO_REUNIAO, parms);

			MailAddressCollection col = new MailAddressCollection();
			AddUusarios(col, lstUsuarios);

			GenericSendEmail(EmailType.ENVIO_REUNIAO, col, body, "[E-VIDA] " + vo.AssuntoEmail);
		}
	}
}
