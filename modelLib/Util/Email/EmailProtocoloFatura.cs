using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailProtocoloFatura : EmailProvider {
		static EVidaLog log = new EVidaLog(typeof(EmailProtocoloFatura));

		internal static void SendAlerta(UsuarioVO usuario, IEnumerable<ProtocoloFaturaVO> lstProtocoloAlerta) {
			string strAlerta = string.Empty;
			
			strAlerta = AggregateIds(lstProtocoloAlerta);

			if (string.IsNullOrEmpty(strAlerta))
				return;

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$NOME_USUARIO$$", usuario.Nome);
			parms.Add("$$PROTOCOLO_ALERTA$$", strAlerta);
			string body = GenerateEmailBody(EmailType.ENVIO_PROTOCOLO_FATURA_ALERTA, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(GetMailTo(usuario));

			GenericSendEmail(EmailType.ENVIO_PROTOCOLO_FATURA_ALERTA, col, body);
		}
		private static string AggregateIds(IEnumerable<ProtocoloFaturaVO> lst) {
			string str = string.Empty;

			if (lst.Count() > 0) {
				str = lst.Select(x => x.Id.ToString()).Aggregate((x, y) => x + "," + y);
			}
			return str;
		}
	}
}
