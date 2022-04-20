using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailReciprocidade : EmailProvider {

		static EVidaLog log = new EVidaLog(typeof(EmailReciprocidade));

		#region Reciprocidade

        internal static void SendEmailReciprocidadeEmpresa(ReciprocidadeVO vo, PUsuarioVO titular, POperadoraSaudeVO empresa, byte[] anexo)
        {
            List<PContatoOperadoraVO> lista_contato = ReciprocidadeBO.Instance.ListarContatoOperadora(empresa.Codint.Trim());
            if(lista_contato == null){
                log.Debug("Email de empresa não cadastrado. CÓDIGO " + empresa.Codint);
                return;
            }
            else if (lista_contato.Count <= 0)
            {
                log.Debug("Email de empresa não cadastrado. CÓDIGO " + empresa.Codint);
                return;
            }

			EmailData dados = new EmailData(EmailType.ENVIO_RECIPROCIDADE_EMPRESA);
            
            foreach (PContatoOperadoraVO contato in lista_contato)
            {
                MailAddress toAddress = new MailAddress(contato.Email.Trim(), empresa.Nomint.Trim());
                dados.AddTo(toAddress);
            }

			string arquivo = "SOL_RECIPROCIDADE_" + vo.CodSolicitacao;
			dados.Add(CreatePdfAttachment(anexo, arquivo + ".pdf"));

			dados.Subject = "[E-VIDA] Solicitação de Reciprocidade TITULAR: " + titular.Nomusr + " CARTÃO: " + titular.Matant;

			GenericSendEmail(dados);
		}

        internal static void SendEmailReciprocidadeFuncionario(ReciprocidadeVO vo, PUsuarioVO titular, POperadoraSaudeVO empresa)
        {
            string nome = titular.Nomusr;

			if (string.IsNullOrEmpty(titular.Email)) {
				LogNotEmailFuncionario(EmailType.ENVIO_RECIPROCIDADE_FUNCIONARIO, titular);
				return;
			}

            MailAddress toAddress = new MailAddress(titular.Email.ToLower(), nome);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$NOME_EMPRESA$$", empresa.Nomint.Trim());
			parms.Add("$$COD_SOLICITACAO$$", vo.CodSolicitacao.ToString());

			string body = GenerateEmailBody(EmailType.ENVIO_RECIPROCIDADE_FUNCIONARIO, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(toAddress);

			GenericSendEmail(EmailType.ENVIO_RECIPROCIDADE_FUNCIONARIO, col, body);
		}

        internal static void SendEmailReciprocidadeAprovacaoFuncionario(ReciprocidadeVO vo, PUsuarioVO funcVO, POperadoraSaudeVO empresa, string serverDir)
        {
			string nome = funcVO.Nomusr;

			if (string.IsNullOrEmpty(funcVO.Email)) {
				LogNotEmailFuncionario(EmailType.ENVIO_RECIPROCIDADE_APROV_FUNCIONARIO, funcVO);
				return;
			}

			EmailData dados = new EmailData(EmailType.ENVIO_RECIPROCIDADE_APROV_FUNCIONARIO);
			
			MailAddress toAddress = new MailAddress(funcVO.Email.ToLower(), nome);
			dados.AddTo(toAddress);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$NOME_EMPRESA$$", empresa.Nomint.Trim());
			parms.Add("$$LINK_EMPRESA$$", empresa.Site.Trim());
			parms.Add("$$OBSERVACAO_APROVACAO$$", vo.ObservacaoAprovacao);

			string body = GenerateEmailBody(EmailType.ENVIO_RECIPROCIDADE_APROV_FUNCIONARIO, parms);
			dados.Body = body;

			if (!string.IsNullOrEmpty(vo.ArquivoAprovacao)) {
				dados.Add(CreateGenericAttachment(serverDir, vo.ArquivoAprovacao));
			}

			GenericSendEmail(dados);
		}

        internal static void SendReciprocidadeAlerta(UsuarioVO usuario, IEnumerable<ReciprocidadeVO> lstReciprocidadeAlerta, IEnumerable<ReciprocidadeVO> lstReciprocidadeCritica)
        {
            string strAlerta = string.Empty;
            string strCritica = string.Empty;
            strAlerta = AggregateIds(lstReciprocidadeAlerta);
            strCritica = AggregateIds(lstReciprocidadeCritica);
            if (string.IsNullOrEmpty(strAlerta) && string.IsNullOrEmpty(strCritica))
                return;

            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("$$NOME_USUARIO$$", usuario.Nome);
            parms.Add("$$PROTOCOLO_ALERTA$$", (string.IsNullOrEmpty(strAlerta) == false) ? strAlerta : "nenhuma");
            parms.Add("$$PROTOCOLO_CRITICA$$", (string.IsNullOrEmpty(strCritica) == false) ? strCritica : "nenhuma");
            string body = GenerateEmailBody(EmailType.ENVIO_RECIPROCIDADE_ALERTA, parms);

            MailAddressCollection col = new MailAddressCollection();
            col.Add(GetMailTo(usuario));

            GenericSendEmail(EmailType.ENVIO_RECIPROCIDADE_ALERTA, col, body);
        }

        private static string AggregateIds(IEnumerable<ReciprocidadeVO> lst)
        {
            string str = string.Empty;

            if (lst.Count() > 0)
            {
                str = lst.Select(x => x.CodSolicitacao.ToString()).Aggregate((x, y) => x + "," + y);
            }
            return str;
        }	

		#endregion
		
	}
}
