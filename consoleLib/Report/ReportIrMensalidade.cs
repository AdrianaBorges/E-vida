using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;

namespace eVida.Console.Report {

	public class ReportIrMensalidade : ReportBase<ReportIrMensalidade.ParamsVO> {

		public class ParamsVO {
			public long CdFuncionario { get; set; }
			public int AnoRef { get; set; }
			public int CdEmpresa { get; set; }
		}

		public ReportIrMensalidade(string reportDir)
			: base(reportDir) {
		}

		protected override string GerarNome(ReportIrMensalidade.ParamsVO vo) {
			return "MENSALIDADE_COPARTICIPACAO_" + vo.CdFuncionario + "_" + vo.AnoRef;
		}

		protected override string DefaultRpt(ReportIrMensalidade.ParamsVO vo) {
			return "rptIrMensalidade";
		}

		protected override void FillReport(ReportIrMensalidade.ParamsVO vo, ReportParams repParams, RelatorioHelper helper) {
			HcFuncionarioVO func = FuncionarioBO.Instance.GetByMatricula(vo.CdEmpresa, vo.CdFuncionario);

			DateTime dataManifesto = new DateTime(vo.AnoRef + 1, 3, 1);
			dataManifesto = DateUtil.PreviousBusinessDay(dataManifesto);

			repParams.Params.Add("AnoRef", vo.AnoRef + "");
			repParams.Params.Add("NomeTitular", func.Nome);
			repParams.Params.Add("Matricula", func.CdFuncionario.ToString());
            repParams.Params.Add("Cpf", FormatUtil.FormatCpf(func.Cpf));
			repParams.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(dataManifesto)); 

			DataTable dt = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidade(func.CdEmpresa, func.CdFuncionario, vo.AnoRef);
			dt = ExtratoIrBeneficiarioBO.Instance.TotalizarMensalidade(dt);
			repParams.DataSources.Add("dsMensalidade", dt);
		}
	}
}