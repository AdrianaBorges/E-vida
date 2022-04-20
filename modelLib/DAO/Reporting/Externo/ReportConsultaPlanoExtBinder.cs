using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting.Externo {
	public class ReportConsultaPlanoExtBinder : IReportBinder {
		[Serializable]
		public class ParamsVO {
			public DataTable Dados { get; set; }
			public PBeneficiarioVO Beneficiario { get; set; }
			public PPlanoVO Plano { get; set; }
		}

		private ParamsVO vo;

		public ReportConsultaPlanoExtBinder(ParamsVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

			DataTable dt = vo.Dados;

			repParams.Params.Add("nomeBenef", vo.Beneficiario.Nome);
			repParams.Params.Add("cartao", vo.Beneficiario.CdAlternativo);
			repParams.Params.Add("Plano", vo.Plano.CodIsa.Trim() + " - " + vo.Plano.Nome.Trim());
			repParams.Params.Add("Data", DateTime.Now.ToShortDateString());
			repParams.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(DateTime.Now));
			/*
			if (dt.Rows.Count > 1000) {
				DataTable dtClone = dt.Clone();
				for (int i = 0; i < 1000; ++i) {
					dtClone.Rows.Add(dt.Rows[i]);
				}
			}*/
			repParams.DataSources.Add("dsConsulta", dt);
			return repParams;
		}

		public string GerarNome() {
			string nome = "PROCEDIMENTOS_" + vo.Plano.Codigo.Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
			return nome;
		}

		public string DefaultRpt() {
			return "externo/rptConsultaPlanoExt";
		}
	}
}
