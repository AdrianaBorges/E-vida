using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email
{
    public class EmailOptum : EmailProvider
    {
        internal static void SendInformativoTranferencia()
        {
            Dictionary<string, string> parms = new Dictionary<string, string>();
            string body = GenerateEmailBody(EmailType.ENVIO_ARQUIVO_OPTUM, parms);

            List<MailAddress> lista_destinatario = new List<MailAddress>();
            //lista_destinatario.Add(new MailAddress("izabella.guimaraes@e-vida.org.br", "Izabella Pacheco Guimarães"));
            //lista_destinatario.Add(new MailAddress("wellington.santos@optum.com.br", "Wellington Santos"));
            //lista_destinatario.Add(new MailAddress("ana.oliveira@optum.com.br", "Ana Luiza de Oliveira Santos"));
            //lista_destinatario.Add(new MailAddress("pqv@e-vida.org.br"));

            // Destinatários provisórios
            //lista_destinatario.Add(new MailAddress("thiago.pescuma@e-vida.org.br", "Thiago Pescuma"));
            //lista_destinatario.Add(new MailAddress("tiago.souza@e-vida.org.br", "Tiago Souza"));
            //lista_destinatario.Add(new MailAddress("adriana.borges@evbl.com.br", "Ricardo Ataide"));

            lista_destinatario.Add(new MailAddress("adriana.borges@evbl.com.br", "Adriana Borges"));

            MailAddressCollection col = new MailAddressCollection();
            foreach(MailAddress endereco in lista_destinatario){
                col.Add(endereco);
            }

            GenericSendEmail(EmailType.ENVIO_ARQUIVO_OPTUM, col, body);
        }
    }
}
