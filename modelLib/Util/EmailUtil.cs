using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.Util.Email;

namespace eVidaGeneralLib.Util {
	public static class EmailUtil {
		
		static EVidaLog log = new EVidaLog(typeof(EmailUtil));
        static string EMAIL_TESTE = ConfigurationManager.AppSettings["EMAIL_TESTE"];

		public class Adesao : EmailAdesao { }
        public class PAdesao : PEmailAdesao { }
		public class Autorizacao : EmailAutorizacao { }
		public class AutorizacaoProvisoria : EmailAutorizacaoProvisoria { }
		public class CanalGestante : EmailCanalGestante { }
		public class DeclaracaoAnual : EmailDeclaracaoAnual { }
		public class Exclusao : EmailExclusao { }
		public class IndisponibilidadeRede : EmailIndisponibilidadeRede { }
		public class ProtocoloFatura : EmailProtocoloFatura { }
		public class Negativa : EmailNegativa { }
		public class Reciprocidade : EmailReciprocidade { }
		public class Reuniao : EmailReuniao { }
		public class SegundaVia : EmailSegundaVia { }
		public class Template : EmailTemplate { }
		public class Universitario : EmailUniversitario { }
		public class Viagem : EmailViagem { }

        public class Optum : EmailOptum { }

		internal static void SendControleEmail(ControleEmailVO vo) {
			if (!ParametroUtil.EmailEnabled) return;

			log.Debug("Sending controle mail: " + vo.Id);
			string logMail = string.Empty;

			List<MailAddress> to = new List<MailAddress>();
			foreach (KeyValuePair<string,string> keyMail in vo.Destinatarios) {
				to.Add(new MailAddress(keyMail.Key, keyMail.Value));
			}

			if (to.Count == 0) {
				log.Info("Não existem destinatários");
				return;
			}

			using (MailMessage message = new MailMessage()) {
				foreach (MailAddress mail in to) {

                    if (string.IsNullOrEmpty(EMAIL_TESTE))
                    {
                        message.To.Add(mail);
                    }
                    else
                    {
                        message.To.Add(new MailAddress(EMAIL_TESTE));
                    }

					logMail += (mail.Address + ", ");
				}
				string subject = vo.Titulo;
				string body = vo.Conteudo;

				message.Subject = subject;
				message.Body = body;
				message.IsBodyHtml = true;

				if (vo.Anexos.Count > 0) {
					foreach (string anexo in vo.Anexos) {
						Attachment at = new Attachment(anexo);
						message.Attachments.Add(at);
					}
				}
				SmtpClient client = new SmtpClient();

				if (!string.IsNullOrEmpty(vo.Sender.Key)) {
					string nameSender = vo.Sender.Value;
					if (string.IsNullOrEmpty(nameSender)) {
						nameSender = vo.Sender.Key;
					}
					message.Sender = new MailAddress(vo.Sender.Key, nameSender);
					message.From = message.Sender;
				}

				if (log.IsDebugEnabled) {
					log.Debug("Sending client email (controle): " +
						" Subject: " + subject +
						" Host: " + client.Host + " Port: " + client.Port +
						" Sender: " + (message.Sender != null ? message.Sender.Address : "-") +
						" TO: " + logMail + "\n" +
						" Body: " + body);
				}
				client.Send(message);
			}
			if (log.IsDebugEnabled)
				log.Debug("Client email sent: " + logMail + " Controle: " + vo.Id);
		}

	}
}
