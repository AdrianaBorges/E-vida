using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using eVidaGeneralLib.VO;

namespace consoleLib.Report {
	public class ReportDeclaracaoAnualDebito : ReportBase<DeclaracaoAnualDebitoInfoVO> {

		public ReportDeclaracaoAnualDebito(string reportDir)
			: base(reportDir) {
		}

		protected override string GerarNome(DeclaracaoAnualDebitoInfoVO vo) {
			return "DECLARACAO_QUITACAO_" + vo.CdBeneficiario + "_" + vo.AnoRef;
		}

		protected override string DefaultRpt(DeclaracaoAnualDebitoInfoVO vo) {
			return "rptDeclaracaoDebitoAnual";
		}
		
		public byte[] GerarNovoRelatorio(int cdBeneficiario, int ano) {
			DeclaracaoAnualDebitoInfoVO info = new DeclaracaoAnualDebitoInfoVO();
			info.AnoRef = ano;
			info.CdBeneficiario = cdBeneficiario;

			DeclaracaoAnualDebitoBO.Instance.FillInfo(info);

			return GerarRelatorio(info);
		}

		protected override void FillReport(DeclaracaoAnualDebitoInfoVO vo, ReportParams repParam, RelatorioHelper helper) {
			helper.EnableExternalImages = true;

			DateTime dtCriacao = new DateTime(vo.AnoRef + 1, 5, 31);
			dtCriacao = DateTime.Now;

			repParam.Params.Add("Local", "Brasília/DF");
			repParam.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(dtCriacao));

			string conteudo = DeclaracaoAnualDebitoBO.Instance.BuildConteudo(vo);

			repParam.Params.Add("Conteudo", conteudo);
		}

	}
}
