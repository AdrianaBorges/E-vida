using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;

namespace eVida.Console.Report {
	public class ReportIrReembolso : ReportBase<ReportIrReembolso.ParamsVO> {

		public class ParamsVO {
			public long CdFuncionario { get; set; }
			public int AnoRef { get; set; }
			public int CdEmpresa { get; set; }
		}

		public ReportIrReembolso(string reportDir)
			: base(reportDir) {
		}

		protected override string GerarNome(ReportIrReembolso.ParamsVO vo) {
			return "REEMBOLSO_" + vo.CdFuncionario + "_" + vo.AnoRef;
		}

		protected override string DefaultRpt(ReportIrReembolso.ParamsVO vo) {
			return "rptIrReembolso";
		}

		protected override void FillReport(ReportIrReembolso.ParamsVO vo, ReportParams repParam, RelatorioHelper helper) {
			HcFuncionarioVO func = FuncionarioBO.Instance.GetByMatricula(vo.CdEmpresa, vo.CdFuncionario);

			string local = "-";
			string lotacao = func.CdLotacao;

			if (func.CdLocal != null) {
				local = func.CdLocal + " - " + LocatorDataBO.Instance.GetRegional(func.CdLocal.Value);
			}
			if (!string.IsNullOrEmpty(func.CdLotacao)) {
				lotacao += " - " + LocatorDataBO.Instance.GetLotacao(func.CdLotacao).DsLotacao;
			} else {
				lotacao = "-";
			}

			DateTime dataManifesto = new DateTime(vo.AnoRef + 1, 3, 1);
			dataManifesto = DateUtil.PreviousBusinessDay(dataManifesto);

			repParam.Params.Add("AnoRef", vo.AnoRef + "");
			repParam.Params.Add("NomeTitular", func.Nome);
            repParam.Params.Add("Cpf", FormatUtil.FormatCpf(func.Cpf));
			repParam.Params.Add("Matricula", func.CdFuncionario.ToString());
			repParam.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(dataManifesto)); 

			repParam.Params.Add("Inicio", (new DateTime(vo.AnoRef, 1, 1)).ToShortDateString());
			repParam.Params.Add("Fim", (new DateTime(vo.AnoRef, 12, 31)).ToShortDateString());
			repParam.Params.Add("Local", local);
			repParam.Params.Add("Lotacao", lotacao);

			DataTable dt = ExtratoIrBeneficiarioBO.Instance.RelatorioReembolsoIr(func.CdEmpresa, func.CdFuncionario, vo.AnoRef);
			repParam.DataSources.Add("dsMensalidade", dt);
		}
	}
}