using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportDeclaracaoAnualDebitoBinder : IReportBinder {
		private DeclaracaoAnualDebitoInfoVO vo;

		public ReportDeclaracaoAnualDebitoBinder(DeclaracaoAnualDebitoInfoVO info) {
			vo = info;
			if (string.IsNullOrEmpty(vo.NomeBeneficiario)) {
				DeclaracaoAnualDebitoBO.Instance.FillInfo(vo);
			}
		}


		public ReportBinderParams GetData() {
			ReportBinderParams repParam = new ReportBinderParams();

			repParam.UseExternalImages = true;

			DateTime dtCriacao = new DateTime(vo.AnoRef + 1, 5, 31);
			dtCriacao = DateTime.Now;

			repParam.Params.Add("Local", "Brasília/DF");
			repParam.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(dtCriacao));

			string conteudo = DeclaracaoAnualDebitoBO.Instance.BuildConteudo(vo);

			repParam.Params.Add("Conteudo", conteudo);
			return repParam;
		}

		public string GerarNome() {
			return "DECLARACAO_QUITACAO_" + vo.CdBeneficiario + "_" + vo.AnoRef;
		}

		public string DefaultRpt() {
			return "rptDeclaracaoDebitoAnual";
		}
	}
}
