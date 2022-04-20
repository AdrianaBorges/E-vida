using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailAutorizacao : EmailProvider {

		static EVidaLog log = new EVidaLog(typeof(EmailAutorizacao));

		#region Autorizacao

        private static MailAddressCollection GetAutorizacaoSendTo(VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred, UsuarioVO criador)
        {
            MailAddress toTitular = GetMailTo(benef);
            MailAddress toCred = GetMailTo(cred);
            MailAddress toCriador = GetMailTo(criador);
            MailAddressCollection col = new MailAddressCollection();
            if (toTitular != null)
                col.Add(toTitular);
            if (toCred != null)
                col.Add(toCred);
            if (toCriador != null)
                col.Add(toCriador);
            return col;
        }

		internal static void SendAutorizacaoCotacao(AutorizacaoVO vo, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred, UsuarioVO criador, UsuarioVO gestor) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());
			parms.Add("$$NM_GESTOR$$", gestor.Nome);

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_COTACAO, parms);

			MailAddressCollection col = GetAutorizacaoSendTo(benef, cred, criador);
			if (col.Count == 0) {
				log.Error("Nenhum email foi obtido para envio!");
				return;
			}

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_COTACAO, col, body);
		}

		internal static void SendAutorizacaoAnalise(AutorizacaoVO vo, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred, UsuarioVO criador, UsuarioVO gestor) {

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());
			parms.Add("$$NM_GESTOR$$", gestor.Nome);

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_ANALISE, parms);

			MailAddressCollection col = GetAutorizacaoSendTo(benef, cred, criador);
			if (col.Count == 0) {
				log.Error("Nenhum email foi obtido para envio!");
				return;
			}

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_ANALISE, col, body);
		}

		internal static bool SendAutorizacaoSolDocumento(AutorizacaoVO vo, string strConteudo, VO.Protheus.PUsuarioVO benef,
			VO.Protheus.PRedeAtendimentoVO cred, UsuarioVO criador, UsuarioVO gestor) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());

			if (!string.IsNullOrEmpty(strConteudo))
				strConteudo = strConteudo.Replace("\n", "<br />");
			parms.Add("$$CONTEUDO$$", strConteudo);
			parms.Add("$$NM_GESTOR$$", gestor.Nome);
			parms.Add("$$URL_SITE_BENEFICIARIOS$$", ParametroUtil.UrlSiteBeneficiarios);

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_SOL_DOC, parms);

			MailAddressCollection col = GetAutorizacaoSendTo(benef, cred, criador);
			if (col.Count == 0) {
				log.Error("Nenhum email foi obtido para envio!");
				return false;
			}

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_SOL_DOC, col, body);
			return true;
		}

		internal static void SendAutorizacaoCancelar(AutorizacaoVO vo, string motivo, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred,
			UsuarioVO criador, UsuarioVO gestor) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());

			if (!string.IsNullOrEmpty(motivo))
				motivo = motivo.Replace("\n", "<br />");
			parms.Add("$$CONTEUDO$$", motivo);
			parms.Add("$$NM_GESTOR$$", gestor.Nome);

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_CANCEL, parms);

			MailAddressCollection col = GetAutorizacaoSendTo(benef, cred, criador);
			if (col.Count == 0) {
				log.Error("Nenhum email foi obtido para envio!");
				return;
			}

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_CANCEL, col, body);
			return;
		}

		internal static void SendAutorizacaoNegar(AutorizacaoVO vo, int? idNeg, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred,
			UsuarioVO criador, UsuarioVO gestor, byte[] anexoNeg) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());
			parms.Add("$$NM_GESTOR$$", gestor.Nome);

			EmailType tipo = EmailType.ENVIO_AUTORIZACAO_NEGAR;
			if (idNeg == null)
				tipo = EmailType.ENVIO_AUTORIZACAO_NEGAR_CRED;

			EmailData dados = new EmailData(tipo);
			dados.Body = GenerateEmailBody(tipo, parms);

			if (idNeg != null) {
				dados.AddTo(GetAutorizacaoSendTo(benef, cred, criador));
				dados.Add(CreatePdfAttachment(anexoNeg, "NEGATIVA_" + idNeg));
			} else {
				dados.AddTo(GetAutorizacaoSendTo(null, cred, null));
			}

			GenericSendEmail(dados);
		}

		internal static void SendAutorizacaoAprov(AutorizacaoVO vo, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred,
			UsuarioVO criador, UsuarioVO gestor, string dirDestino, List<AutorizacaoTissVO> lstArquivos) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());
			parms.Add("$$NM_GESTOR$$", gestor.Nome);

			EmailData dados = new EmailData(EmailType.ENVIO_AUTORIZACAO_APROV);

			dados.Body = GenerateEmailBody(dados.Tipo, parms);

			dados.AddTo(GetAutorizacaoSendTo(benef, cred, criador));

			foreach (AutorizacaoTissVO arquivo in lstArquivos) {
				string fullName = Path.Combine(dirDestino, arquivo.NomeArquivo);
				if (File.Exists(fullName)) {
					dados.Add(CreateGenericAttachment(fullName));
				} else {
					throw new InvalidOperationException("Arquivo TISS não encontrado: " + arquivo.NomeArquivo);
				}
			}

			GenericSendEmail(dados);
		}

		private static string AggregateAutorizacaoIds(IEnumerable<AutorizacaoVO> lst, bool onlyOdonto) {
			string str = string.Empty;

			if (lst.Count() > 0) {
				if (onlyOdonto)
					lst = lst.Where(x => x.Tipo == TipoAutorizacao.ODONTO);
				if (lst.Count() > 0)
					str = lst.Select(x => x.Id.ToString()).Aggregate((x, y) => x + "," + y);
			}
			return str;
		}

		internal static void SendAutorizacaoAlerta(IEnumerable<AutorizacaoVO> lstProtocoloNovo, IEnumerable<AutorizacaoVO> lstProtocoloAlerta, IEnumerable<AutorizacaoVO> lstProtocoloFora, bool onlyOdonto) {
			string strAlerta = string.Empty;
			string strFora = string.Empty;
			string strNovo = string.Empty;

			strNovo = AggregateAutorizacaoIds(lstProtocoloNovo, onlyOdonto);
			strAlerta = AggregateAutorizacaoIds(lstProtocoloAlerta, onlyOdonto);
			strFora = AggregateAutorizacaoIds(lstProtocoloFora, onlyOdonto);

			if (string.IsNullOrEmpty(strNovo) && string.IsNullOrEmpty(strAlerta) && string.IsNullOrEmpty(strFora))
				return;

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$PROTOCOLO_NOVO$$", strNovo);
			parms.Add("$$PROTOCOLO_ALERTA$$", strAlerta);
			parms.Add("$$PROTOCOLO_FORA$$", strFora);
			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_ALERTA, parms);

			MailAddressCollection col = new MailAddressCollection();
			if (!onlyOdonto)
				col.Add(GetMailToAutoriza());
			else
				col.Add(new MailAddress(ParametroUtil.EmailPericiaOdonto, "Odonto"));

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_ALERTA, col, body);
		}
		
        internal static void SendAutorizacaoCriacao(AutorizacaoVO vo, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred, UsuarioVO usuarioCriacao) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());

			string solicitante = "";
			if (vo.Origem == OrigemAutorizacao.GESTOR) {
				solicitante = "GESTOR - " + usuarioCriacao.Nome;
			} else if (vo.Origem == OrigemAutorizacao.BENEF) {
				solicitante = "BENEFICIÁRIO - " + benef.Nomusr;
			} else if (vo.Origem == OrigemAutorizacao.CRED) {
				solicitante = "CREDENCIADO - " + cred.Nome;
			}

            VO.Protheus.PUsuarioVO titular = BO.Protheus.PUsuarioBO.Instance.GetTitular(benef.Codint, benef.Codemp, benef.Matric);

			parms.Add("$$NM_SOLICITANTE$$", solicitante);
			parms.Add("$$NM_TITULAR$$", titular.Nomusr);
			parms.Add("$$NM_BENEFICIARIO$$", benef.Nomusr);

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_CRIADA, parms);

			MailAddressCollection col = GetAutorizacaoSendTo(benef, cred, usuarioCriacao);
			col.Add(GetMailToAutoriza());

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_CRIADA, col, body);
			return;
		}

		internal static void SendAutorizacaoAlteracao(AutorizacaoVO vo, VO.Protheus.PUsuarioVO benef, VO.Protheus.PRedeAtendimentoVO cred, UsuarioVO usuario) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());
			OrigemAutorizacao origemOperacao = vo.OrigemAlteracao;
			string solicitante = "";
			if (origemOperacao == OrigemAutorizacao.GESTOR) {
				solicitante = "GESTOR - " + usuario.Nome;
			} else if (origemOperacao == OrigemAutorizacao.BENEF) {
				solicitante = "BENEFICIÁRIO - " + benef.Nomusr;
			} else if (origemOperacao == OrigemAutorizacao.CRED) {
				solicitante = "CREDENCIADO - " + cred.Nome;
			}
			parms.Add("$$NM_SOLICITANTE$$", solicitante);
			parms.Add("$$DT_ALTERACAO$$", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
			parms.Add("$$STATUS$$", AutorizacaoTradutorHelper.TraduzStatus(vo.Status));

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_ALTERADA, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(GetMailToAutoriza());
			if (vo.Tipo == TipoAutorizacao.ODONTO)
				col.Add(new MailAddress(ParametroUtil.EmailPericiaOdonto, "Odonto"));

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_ALTERADA, col, body);
			return;
		}
		
        internal static void SendAutorizacaoPericia(AutorizacaoVO vo) {
			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$COD_SOLICITACAO$$", vo.Id.ToString());

			parms.Add("$$URL_SITE_INTRANET$$", ParametroUtil.UrlSiteIntranet);
			parms.Add("$$DT_ALTERACAO$$", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
			parms.Add("$$STATUS$$", AutorizacaoTradutorHelper.TraduzStatus(vo.Status));

			string body = GenerateEmailBody(EmailType.ENVIO_AUTORIZACAO_PERICIA, parms);

			MailAddressCollection col = new MailAddressCollection();
			string email = ParametroUtil.EmailPericiaMedica;
			if (vo.Tipo == TipoAutorizacao.ODONTO)
				email = ParametroUtil.EmailPericiaOdonto;
			MailAddress auditor = new MailAddress(email, "AUDITOR");
			col.Add(auditor);

			GenericSendEmail(EmailType.ENVIO_AUTORIZACAO_PERICIA, col, body);
			return;
		}

		#endregion

	}
}
