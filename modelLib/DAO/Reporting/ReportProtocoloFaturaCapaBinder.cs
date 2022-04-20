using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportProtocoloFaturaCapaBinder : IReportBinder {
		private ProtocoloFaturaVO vo;
		public ReportProtocoloFaturaCapaBinder(ProtocoloFaturaVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.UseExternalImages = true;

			repParams.Params.Add("CodProtocolo", vo.Id.ToString(ProtocoloFaturaVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NrProtocolo", vo.NrProtocolo);
			repParams.Params.Add("CpfCnpj", FormatUtil.FormatCpfCnpj(vo.RedeAtendimento.Tippe, vo.RedeAtendimento.Cpfcgc));
			repParams.Params.Add("RazaoSocial", vo.RedeAtendimento.Nome);

			repParams.Params.Add("DataEntrada", vo.DataEntrada.ToString("dd/MM/yyyy"));
			repParams.Params.Add("AnoEntrada", vo.AnoEntrada.ToString());
			repParams.Params.Add("DocFiscal", !string.IsNullOrEmpty(vo.DocumentoFiscal) ? vo.DocumentoFiscal : " ");
			repParams.Params.Add("DataEmissao", vo.DataEmissao != null ? vo.DataEmissao.Value.ToString("dd/MM/yyyy") : " ");
			repParams.Params.Add("ValorApresentado", vo.ValorApresentado != null ? vo.ValorApresentado.Value.ToString("C") : " ");
			repParams.Params.Add("DataVencimento", vo.DataVencimento.ToString("dd/MM/yyyy"));

			repParams.Params.Add("Email", StringOrDefault(vo.RedeAtendimento.Email));
			string regional = " ";
            KeyValuePair<string, string> keyRegiao = PRedeAtendimentoBO.Instance.GetRegiaoRedeAtendimento(vo.RedeAtendimento.Codigo, vo.DataCriacao);
            if (!string.IsNullOrEmpty(keyRegiao.Value))
            {
                regional = keyRegiao.Value;
			}
			repParams.Params.Add("Unidade", regional);

			string analista = " ";
			if (vo.CdUsuarioResponsavel != null) {
				UsuarioVO analistaVO = UsuarioBO.Instance.GetUsuarioById(vo.CdUsuarioResponsavel.Value);
				analista = analistaVO.Login;
			}
			repParams.Params.Add("AnalistaResp", analista);
            
            /*string natureza = " ";
            List<PEspecialidadeVO> lista_especialidade = PLocatorDataBO.Instance.ListarEspecialidade(vo.RedeAtendimento.Codigo);
            if (lista_especialidade.Count > 0)
            {
                foreach (PEspecialidadeVO especialidade in lista_especialidade)
                {
                    natureza += (natureza != "" ? ", " + especialidade.Descri : especialidade.Descri);
                }
            }*/

            string natureza = "";
            PRedeAtendimentoVO credenciado = PRedeAtendimentoBO.Instance.GetById(vo.RedeAtendimento.Codigo);
            if (credenciado != null)
            {
                PClasseRedeAtendimentoVO classerede = PLocatorDataBO.Instance.GetClasseRedeAtendimento(credenciado.Tippre);
                if (classerede != null)
                {
                    natureza = classerede.Descri;
                }
            }

			repParams.Params.Add("Natureza", natureza);

            string ddd = (vo.RedeAtendimento.Ddd != null ? "(" + vo.RedeAtendimento.Ddd + ")" : "");
            string tel = (vo.RedeAtendimento.Tel != null ? vo.RedeAtendimento.Tel : "");

			repParams.Params.Add("Fone1", ddd + tel);
			repParams.Params.Add("Fone2", " ");
			return repParams;
		}

		public string GerarNome() {
			if (vo.Id == Int32.MinValue)
				return "Capa_Protocolo_Fatura_BRANCO";
			return "Capa_Protocolo_Fatura_" + vo.Id;
		}

		public string DefaultRpt() {
			return "rptProtocoloFaturaCapa";
		}
		protected string StringOrDefault(string str) {
			if (string.IsNullOrEmpty(str)) return " ";
			return str;
		}

	}
}
