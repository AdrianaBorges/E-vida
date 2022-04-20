using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO.TemplateEmail {

	internal class TemplateEmailGeradorProtocoloFatura : TemplateEmailGerador {

		internal static readonly string[] TAG_PROTOCOLO_FATURA = new string[] { "CD_CREDENCIADO", "RAZAO_SOCIAL", "CPF_CNPJ", "MENSAGEM", "DATA_EMISSAO", "DATA_ENTRADA", "DATA_VENCIMENTO", 
			"DOC_FISCAL", "GLOSA", 
			"VALOR_APRESENTADO", "VALOR_PROCESSADO", "MOTIVO_CANCELAMENTO", "PENDENCIA", 
			"ANALISTA_RESPONSAVEL_NOME", "ANALISTA_RESPONSAVEL_EMAIL"};

		private ProtocoloFaturaVO protocolo;

		internal TemplateEmailGeradorProtocoloFatura(TemplateEmailVO templateVO, EvidaDatabase db, SortedDictionary<string, string> parametros)
			: base(templateVO, db, parametros) {
			int id = Convert.ToInt32(parametros["ID"]);
			protocolo = ProtocoloFaturaBO.Instance.GetById(id);
		}

		protected override ControleEmailVO GetControleEmail() {
			ControleEmailVO controleVO = new ControleEmailVO();
			controleVO.Tipo = TipoControleEmail.PROTOCOLO_FATURA;
			controleVO.Referencia = new List<string>();
			controleVO.Referencia.Add(protocolo.Id.ToString());
			controleVO.Referencia.Add(template.Id.ToString());
			return controleVO;
		}

		protected override List<KeyValuePair<string, string>> GetDestinatarios() {
			string email = protocolo.RedeAtendimento.Email;
			string nome = protocolo.RedeAtendimento.Nome;
			KeyValuePair<string, string> keyMail = new KeyValuePair<string, string>(email, nome);

			if (string.IsNullOrEmpty(email)) {
				throw new EvidaException("E-mail não cadastrado para o credenciado " + nome);
			}

			KeyValuePair<string, string> keyMailNf = new KeyValuePair<string, string>(ParametroUtil.EmailNotaFiscal, "Nota Fiscal");

			KeyValuePair<string, string> keyMailResp = new KeyValuePair<string,string>("","");
			if (protocolo.CdUsuarioResponsavel != null) {
				UsuarioVO respVO = UsuarioBO.Instance.GetUsuarioById(protocolo.CdUsuarioResponsavel.Value);
				if (respVO != null && !string.IsNullOrEmpty(respVO.Email)) {
					keyMailResp = new KeyValuePair<string, string>(respVO.Email, respVO.Nome);
				}
			}


			List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
			lst.Add(keyMail);
			lst.Add(keyMailNf);
			if (!string.IsNullOrEmpty(keyMailResp.Key)) lst.Add(keyMailResp);
			return lst;
		}

		protected override SortedDictionary<string, string> GetValuesTag() {
			string mensagem = parametros["MENSAGEM"];
			string cdCredenciado = protocolo.RedeAtendimento.Codigo;
			string razaoSocial = protocolo.RedeAtendimento.Nome;
			string cpfCnpj = protocolo.RedeAtendimento.Cpfcgc;

			UsuarioVO analistaVO = null;
			if (protocolo.CdUsuarioResponsavel != null)
				analistaVO = UsuarioBO.Instance.GetUsuarioById(protocolo.CdUsuarioResponsavel.Value);
			string pendencia = " ";
			if (protocolo.CdPendencia != null)
				pendencia = MotivoPendenciaBO.Instance.GetById(protocolo.CdPendencia.Value).Nome;

			SortedDictionary<string, string> outParams = new SortedDictionary<string, string>();
            outParams.Add("CD_CREDENCIADO", cdCredenciado);
            outParams.Add("RAZAO_SOCIAL", razaoSocial);
            outParams.Add("CPF_CNPJ", cpfCnpj);
			outParams.Add("MENSAGEM", mensagem);

			outParams.Add("DATA_ENTRADA", protocolo.DataEntrada.ToString("dd/MM/yyyy"));
			outParams.Add("DATA_EMISSAO", protocolo.DataEmissao != null ? protocolo.DataEmissao.Value.ToString("dd/MM/yyyy") : " ");
			outParams.Add("DATA_VENCIMENTO", protocolo.DataVencimento.ToString("dd/MM/yyyy"));
			outParams.Add("DOC_FISCAL", protocolo.DocumentoFiscal);

			outParams.Add("VALOR_PROCESSADO", protocolo.ValorProcessado != null ? protocolo.ValorProcessado.Value.ToString("C") : " ");
			outParams.Add("VALOR_APRESENTADO", protocolo.ValorApresentado != null ? protocolo.ValorApresentado.Value.ToString("C") : " ");
			outParams.Add("GLOSA", protocolo.ValorGlosa != null ? protocolo.ValorGlosa.Value.ToString("C") : " ");

			outParams.Add("MOTIVO_CANCELAMENTO", protocolo.MotivoCancelamento);
			outParams.Add("PENDENCIA", pendencia);

			outParams.Add("ANALISTA_RESPONSAVEL_NOME", analistaVO != null ? analistaVO.Nome : " ");
			outParams.Add("ANALISTA_RESPONSAVEL_EMAIL", analistaVO != null ? analistaVO.Email : " ");

			return outParams;
		}
	}
}
