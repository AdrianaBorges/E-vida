using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace eVidaGeneralLib.Util.Email
{
    public class PEmailAdesao : EmailProvider
    {
        static string EMAIL_TESTE = ConfigurationManager.AppSettings["EMAIL_TESTE"];

        #region Adesao
        internal static void SendAdesaoValidacao(VO.Adesao.PDeclaracaoVO vo, bool isValido, string motivo)
        {
            if (string.IsNullOrEmpty(vo.Email)) return;

            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("$$COD_SOLICITACAO$$", vo.Numero.ToString(VO.Adesao.PDeclaracaoVO.FORMATO_PROTOCOLO));
            parms.Add("$$RESPOSTA$$", isValido ? "VALIDADA" : "INVALIDADA");
            parms.Add("$$OBSERVACAO$$", motivo);

            string body = GenerateEmailBody(EmailType.ENVIO_ADESAO_VALIDACAO, parms);

            MailAddressCollection col = new MailAddressCollection();

            if (string.IsNullOrEmpty(EMAIL_TESTE))
            {
                col.Add(GetMailToCadastro());
            }
            else
            {
                col.Add(EMAIL_TESTE);
            }

            GenericSendEmail(EmailType.ENVIO_ADESAO_VALIDACAO, col, body);
        }
        #endregion

    }
}
