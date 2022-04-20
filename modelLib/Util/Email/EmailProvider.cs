using eVidaGeneralLib.DAO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace eVidaGeneralLib.Util.Email {
	public class EmailProvider {

		static EVidaLog log = new EVidaLog(typeof(EmailProvider));
        static string EMAIL_TESTE = ConfigurationManager.AppSettings["EMAIL_TESTE"];

		public class EmailData {
			public EmailData(EmailType tipo) {
				Tipo = tipo;
			}
			public EmailType Tipo { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
			public List<Attachment> Attachments { get; private set; }
			public MailAddressCollection To { get; private set; }
			public MailAddress Sender { get; set; }

			public void Add(Attachment att) {
				if (att == null) return;

				if (Attachments == null)
					Attachments = new List<Attachment>();
				Attachments.Add(att);
			}
			public void AddTo(MailAddress to) {
				if (To == null)
					To = new MailAddressCollection();
				To.Add(to);
			}
			public void AddTo(MailAddressCollection to) {
				if (To == null)
					To = new MailAddressCollection();
				if (to != null) {
					foreach (MailAddress item in to) {
						To.Add(item);
					}
				}
			}
		}

		public enum EmailType {
			SEGUNDA_VIA_CARTEIRA = 1,
			SOLICITACAO_NEGATIVA = 2,
			ENVIO_RECIPROCIDADE_EMPRESA = 3,
			ENVIO_RECIPROCIDADE_FUNCIONARIO = 4,
			ENVIO_RECIPROCIDADE_APROV_FUNCIONARIO = 5,
            ENVIO_RECIPROCIDADE_ALERTA = 202,
			ENVIO_EXCLUSAO_APROV_FUNCIONARIO = 6,
			ENVIO_EXCLUSAO_APROV_FINANCEIRO = 7,
			ENVIO_EXCLUSAO_CANCELAMENTO = 8,
			ENVIO_UNIVERSITARIO_APROV = 9,
			ENVIO_UNIVERSITARIO_CANCEL = 10,
			ENVIO_SEGUNDA_VIA_CANCEL = 11,
			ENVIO_NEGATIVA_APROV = 12,
			ENVIO_AUTORIZACAO_CRIADA = 13,
			ENVIO_AUTORIZACAO_ANALISE = 14,
			ENVIO_AUTORIZACAO_SOL_DOC = 15,
			ENVIO_AUTORIZACAO_CANCEL = 16,
			ENVIO_AUTORIZACAO_NEGAR = 17,
			ENVIO_AUTORIZACAO_APROV = 18,
			ENVIO_AUTORIZACAO_COTACAO = 19,
			ENVIO_AUTORIZACAO_NEGAR_CRED = 20,
			ENVIO_AUTORIZACAO_ALERTA = 21,
			ENVIO_AUTORIZACAO_ALTERADA = 22,
			ENVIO_AUTORIZACAO_PERICIA = 23,
			ENVIO_AUTORIZACAO_PROVISORIA_CRIADA = 24,
			ENVIO_AUTORIZACAO_PROVISORIA_APROV = 25,
			ENVIO_REUNIAO = 26,
			ENVIO_INDISPONIBILIDADE_REDE_ENCAMINHAR = 27,
			ENVIO_INDISPONIBILIDADE_REDE_CRIADA = 28,
			ENVIO_INDISPONIBILIDADE_REDE_ENCERRADA = 29,
			ENVIO_REANALISE_NEGATIVA_SOL = 40,
			ENVIO_REANALISE_NEGATIVA_APROV = 41,
			ENVIO_REANALISE_NEGATIVA_DEV = 42,

			ENVIO_DECLARACAO_ANUAL_DEBITO = 30,
            ENVIO_INDISPONIBILIDADE_REDE_ALERTA = 31,

			ENVIO_ADESAO_VALIDACAO = 50,

			ENVIO_VIAGEM_CRIACAO = 60,
			ENVIO_VIAGEM_SOL_APROV_COORDENADOR = 61,
			ENVIO_VIAGEM_SOL_APROV_DIRETORIA = 62,
			ENVIO_VIAGEM_SOL_NEGAR_COORDENADOR = 63,
			ENVIO_VIAGEM_SOL_NEGAR_DIRETORIA = 64,
			ENVIO_VIAGEM_COMPRA_PASSAGEM = 65,
			ENVIO_VIAGEM_COMPRA_EFETUADA = 66,
			ENVIO_VIAGEM_FINANCEIRO_COMPROVANTE = 67,
			ENVIO_VIAGEM_PC_APROV_FINANCEIRO = 68,
			ENVIO_VIAGEM_PC_NEGAR_FINANCEIRO = 69,
			ENVIO_VIAGEM_PC_PENDENTE_DIRETORIA = 70,
			ENVIO_VIAGEM_PC_APROV_DIRETORIA = 71,
			ENVIO_VIAGEM_PC_NEGAR_DIRETORIA = 72,

			ENVIO_TEMPLATE = 100,
			ENVIO_PROTOCOLO_FATURA_ALERTA = 120,

			ENVIO_CANAL_GESTANTE_BENEF = 200,
			ENVIO_CANAL_GESTANTE_ESC_BENEF = 201,
            ENVIO_ARQUIVO_OPTUM = 205
		}
		private static EmailConfigVO GetConfig(EmailType tipo) {
			return Configurator.GetConfig((int)tipo);
		}

		public static List<Dictionary<string,string>> ListConfig() {
			List<EmailConfigVO> lstConfig = Configurator.ListAllConfig();
			List<Dictionary<string,string>> lstOut = new List<Dictionary<string,string>>();
			
			System.Reflection.PropertyInfo[] properties = typeof(EmailConfigVO).GetProperties();
			foreach (EmailConfigVO vo in lstConfig) {
				Dictionary<string, string> value = new Dictionary<string, string>();
				
				foreach (System.Reflection.PropertyInfo property in properties) {
					value.Add(property.Name, Convert.ToString(property.GetValue(vo, null)));
				}
				lstOut.Add(value);
			}
			return lstOut;
		}

		internal static string GenerateEmailTitle(EmailType tipo) {
			EmailConfigVO vo = GetConfig(tipo);
			if (vo != null)
				return vo.Assunto;

			log.Error("Subject de email não encontrado para " + tipo);
			return "eVida - SEM ASSUNTO - " + tipo;
		}

		internal static string GenerateEmailBody(EmailType tipo, Dictionary<string, string> parms = null) {
			string value = null;

			EmailConfigVO vo = GetConfig(tipo);
			if (vo != null)
				value = vo.Arquivo;
			else {
				log.Error("Config nao encontrada: " + tipo);
			}

			return LoadReplaceBody(value, parms);
		}

		internal static string GetSender(EmailType tipo) {
			string value = null;

			EmailConfigVO vo = GetConfig(tipo);
			if (vo != null)
				value = vo.Email;

			if (string.IsNullOrEmpty(value))
				return null;

			return value;
		}

		protected static string LoadReplaceBody(string file, Dictionary<string, string> parms) {
			if (String.IsNullOrEmpty(file) || !File.Exists(file)) {
				log.Error("Arquivo " + file + " não encontrado");

				string ret = "";
				if (parms != null) {
					foreach (string key in parms.Keys) {
						ret += "\n" + key + " = " + parms[key];
					}
				} else {
					ret = " DEFINIÇÃO DE CORPO DE EMAIL NÃO ENCONTRADO ";
				}

				return ret;
			}

			string fullFile = File.ReadAllText(file);
			StringBuilder sb = new StringBuilder(fullFile);
			if (parms != null) {
				foreach (string key in parms.Keys) {
					if (log.IsDebugEnabled) {
						log.Debug(key + " = " + parms[key]);
					}
					sb.Replace(key, parms[key]);
				}
			}
			return sb.ToString();
		}

		protected static void GenericSendEmail(EmailData email) {
			GenericSendEmail(email.Tipo, email.To, email.Body, email.Subject, email.Attachments, email.Sender);
		}
		
		protected static void GenericSendEmail(EmailType type, MailAddressCollection to, string body = null, string subject = null, List<Attachment> attachments = null, MailAddress sender = null) {
			if (!ParametroUtil.EmailEnabled) return;

			log.Debug("Sending mail: " + type + " (" + (int)type + ") " + body + " " + subject);
			string logMail = string.Empty;

			if (to == null || to.Count == 0) {
				log.Info("Não existem destinatários: " + type);
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
				if (subject == null)
					subject = GenerateEmailTitle(type);
				if (body == null)
					body = GenerateEmailBody(type);

				message.Subject = subject;
				message.Body = body;
				message.IsBodyHtml = true;
				if (attachments != null)
					foreach (Attachment at in attachments) {
						message.Attachments.Add(at);
					}

				SmtpClient client = new SmtpClient();
				if (sender == null) {
					string senderAddr = GetSender(type);
					if (!string.IsNullOrEmpty(senderAddr)) {
						message.Sender = new MailAddress(senderAddr.Trim());
						message.From = message.Sender;
					}
				} else {
					message.Sender = sender;
					message.From = message.Sender;
				}

				if (log.IsDebugEnabled) {
					log.Debug("Sending client email: " +
						" Subject: " + subject +
						" Host: " + client.Host + " Port: " + client.Port +
						" Sender: " + (message.Sender != null ? message.Sender.Address : "-") +
						" TO: " + logMail + "\n" +
						" Body: " + body);
				}
				client.Send(message);
			}
			if (log.IsDebugEnabled)
				log.Debug("Client email sent: " + logMail);
		}

		protected static void Release(MailMessage message) {
			if (message.Attachments != null) {
				foreach (Attachment a in message.Attachments) {
					a.Dispose();
				}
				message.Attachments.Dispose();
			}
			message.Dispose();
		}

		protected static Attachment CreatePdfAttachment(byte[] anexo, string nome) {
			nome = nome.ToLower().EndsWith(".pdf") ? nome : nome + ".pdf";
			return new Attachment(new MemoryStream(anexo), nome, "application/pdf");
		}

		protected static Attachment CreateGenericAttachment(string serverDir, string file) {
			if (string.IsNullOrEmpty(file)) return null;
			string fullName = Path.Combine(serverDir, file);
			return CreateGenericAttachment(fullName);
		}

		protected static Attachment CreateGenericAttachment(string fullPath) {
			if (string.IsNullOrEmpty(fullPath)) return null;

			if (File.Exists(fullPath)) {
				return new Attachment(fullPath);
			}
			return null;
		}
		protected static MailAddress GetMailToAutoriza() {
			return CreateMailAddress(ParametroUtil.EmailAutoriza, "Autoriza");
		}
		protected static MailAddress GetMailToCadastro() {
			return CreateMailAddress(ParametroUtil.EmailCadastro, "Cadastro");
		}
		protected static MailAddress GetMailToCredenciamento() {
			return CreateMailAddress(ParametroUtil.EmailCredenciamento, "Credenciamento");
		}
		protected static MailAddress GetMailToFaturamento() {
			return CreateMailAddress(ParametroUtil.EmailFaturamento, "Faturamento");
		}
		protected static MailAddress GetMailToFinanceiro() {
			return CreateMailAddress(ParametroUtil.EmailFinanceiro, "Financeiro");
		}
		protected static MailAddress GetMailToReembolso() {
			return CreateMailAddress(ParametroUtil.EmailReembolso, "Reembolso");
		}
		protected static MailAddress GetMailToDiretoria() {
			return CreateMailAddress(ParametroUtil.EmailDiretoria, "Diretoria");
		}

		protected static MailAddress GetMailTo(VO.HC.HcBeneficiarioVO benef) {
			string nomeTitular = string.Empty;
			string emailTitular = string.Empty;

			if (benef != null) {
				nomeTitular = benef.NmTitular;
				emailTitular = benef.Email;
				if (string.IsNullOrEmpty(benef.Email)) {
					log.Info("Email de beneficiario nao cadastrado. MATRICULA " + benef.CdFuncionario);
				}
			}

			MailAddress to = null;
			if (!string.IsNullOrEmpty(emailTitular))
				to = CreateMailAddress(emailTitular.ToLower(), nomeTitular);
			return to;
		}
		protected static MailAddress GetMailTo(VO.HC.HcVCredenciadoVO cred) {
			string nomeCred = string.Empty;
			string emailCred = string.Empty;
			if (cred != null) {
				nomeCred = cred.RazaoSocial;
				emailCred = cred.Email;
				if (string.IsNullOrEmpty(emailCred))
					log.Info("Email de credenciado não cadastrado. CREDENCIADO: " + cred.CdCredenciado);
			}
			MailAddress toCred = null;
			if (!string.IsNullOrEmpty(emailCred))
				toCred = CreateMailAddress(emailCred.ToLower(), nomeCred);
			return toCred;
		}

        protected static MailAddress GetMailTo(VO.Protheus.PUsuarioVO benef)
        {
            string nomeTitular = string.Empty;
            string emailTitular = string.Empty;

            if (benef != null)
            {
                nomeTitular = benef.Nomusr.Trim();
                emailTitular = benef.Email.Trim();
                if (string.IsNullOrEmpty(benef.Email.Trim()))
                {
                    log.Info("Email de beneficiario nao cadastrado. CODINT: " + benef.Codint + ", CODEMP: " + benef.Codemp + ", MATRIC: " + benef.Matric + ", TIPREG: " + benef.Tipreg);
                }
            }

            MailAddress to = null;
            if (!string.IsNullOrEmpty(emailTitular))
                to = CreateMailAddress(emailTitular.ToLower(), nomeTitular);
            return to;
        }

        protected static MailAddress GetMailTo(VO.Protheus.PRedeAtendimentoVO cred)
        {
            string nomeCred = string.Empty;
            string emailCred = string.Empty;
            if (cred != null)
            {
                nomeCred = cred.Nome.Trim();
                emailCred = cred.Email.Trim();
                if (string.IsNullOrEmpty(emailCred.Trim()))
                    log.Info("Email de credenciado não cadastrado. CREDENCIADO: " + cred.Codigo);
            }
            MailAddress toCred = null;
            if (!string.IsNullOrEmpty(emailCred))
                toCred = CreateMailAddress(emailCred.ToLower(), nomeCred);
            return toCred;
        }

		protected static MailAddress GetMailTo(UsuarioVO usuario) {
			string nome = string.Empty;
			string email = string.Empty;
			if (usuario != null) {
				nome = usuario.Nome;
				email = usuario.Email;
				if (string.IsNullOrEmpty(email))
					log.Error("Email de usuário não cadastrado. USUARIO: " + usuario.Id);
			}
			MailAddress toMail = null;
			if (!string.IsNullOrEmpty(email))
				toMail = CreateMailAddress(email.ToLower(), nome);
			return toMail;
		}

		protected static void LogNotEmailFuncionario(EmailType tipo, PUsuarioVO funcVO) {
			if (string.IsNullOrEmpty(funcVO.Email)) {
				if (log.IsDebugEnabled) {
					log.Debug(tipo + " Email de funcionário não encontrado " + funcVO.GetLogId());
				}
			}
		}

		protected static void AddUusarios(MailAddressCollection col, IEnumerable<UsuarioVO> lstUsuarios) {
			if (lstUsuarios != null) {
				foreach (UsuarioVO u in lstUsuarios) {
					if (!string.IsNullOrEmpty(u.Email))
						col.Add(CreateMailAddress(u.Email, u.Nome));
				}
			}
		}

		protected static MailAddress CreateMailAddress(string mailAddr, string name) {
			try {
				return new MailAddress(mailAddr.ToLower(), name);
			} catch (Exception ex) {
				throw new Exception("Email inválido: " + mailAddr, ex);
			}
		}
	}
}
