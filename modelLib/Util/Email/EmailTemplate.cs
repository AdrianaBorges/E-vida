using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailTemplate : EmailProvider {

		#region Template
		internal static void SendTemplate(List<KeyValuePair<string, string>> destinatarios, string mensagem, string assunto) {
			MailAddressCollection col = new MailAddressCollection();
			foreach (KeyValuePair<string, string> dest in destinatarios) {
				col.Add(new MailAddress(dest.Key, dest.Value));
			}
			if (!string.IsNullOrEmpty(assunto)) {
				if (!assunto.StartsWith("[")) {
					assunto = "[E-VIDA] " + assunto;
				}
			}
			GenericSendEmail(EmailType.ENVIO_TEMPLATE, col, mensagem, assunto, null);
		}
		#endregion

	}
}
