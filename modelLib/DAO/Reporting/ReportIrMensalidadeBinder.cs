using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportIrMensalidadeBinder : IReportBinder {

		public class ParamsVO {
			public string CartaoTitular { get; set; }
			public int AnoRef { get; set; }
		}

		private ParamsVO vo;
		public ReportIrMensalidadeBinder(ParamsVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

            PUsuarioVO benef = PUsuarioBO.Instance.GetUsuarioByCartao(vo.CartaoTitular);

			//DateTime dataManifesto = new DateTime(vo.AnoRef + 1, 3, 1);
			//dataManifesto = DateUtil.PreviousBusinessDay(dataManifesto);
			//if (dataManifesto.Year == 2017) {
			//	dataManifesto = DateUtil.PreviousBusinessDay(dataManifesto);
			//}
			VO.ConfiguracaoIrVO configIR = ConfiguracaoIrBO.Instance.GetConfiguracao();
			DateTime dataManifesto = new DateTime(vo.AnoRef + 1, 2, configIR.DayIrBeneficiario);

			repParams.Params.Add("AnoRef", vo.AnoRef + "");
			repParams.Params.Add("NomeTitular", benef.Nomusr);
			repParams.Params.Add("Matricula", benef.Matemp);
            repParams.Params.Add("Cpf", !string.IsNullOrEmpty(benef.Cpfusr.Trim()) ? FormatUtil.FormatCpf(benef.Cpfusr.Trim()) : "NÃO CADASTRADO");
            repParams.Params.Add("EnderecoEVIDA", configIR.EnderecoEVIDA);
			repParams.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(dataManifesto));

            DataTable dt = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidadeTable(benef.Codint, benef.Codemp, benef.Matric, vo.AnoRef);
			dt = ExtratoIrBeneficiarioBO.Instance.TotalizarMensalidade(dt);
			repParams.DataSources.Add("dsMensalidade", dt);
			return repParams;
		}

		public string GerarNome() {
			return "MENSALIDADE_COPARTICIPACAO_" + vo.AnoRef + "_" + vo.CartaoTitular.Trim();
		}

		public string DefaultRpt() {
			return "rptIrMensalidade";
		}
	}
}
