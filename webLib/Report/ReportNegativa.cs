using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	public class ReportNegativa : ReportBase<FormNegativaVO, ReportNegativaBinder> {
		private bool reanalise = false;

		public ReportNegativa(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override FormNegativaVO GerarDados(HttpRequest request) {
			int id;
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				if (!string.IsNullOrEmpty(request["RN"]))
					reanalise = true;
				FormNegativaVO vo = Buscar(id);
				return vo;
			}
		}

		protected override void FillReport(FormNegativaVO vo, ReportParams repParams, RelatorioHelper helper) {
			helper.EnableExternalImages = true;

			ReportNegativaBinder rpt = new ReportNegativaBinder(vo, reanalise);
			ReportBinderParams repBinderParams = rpt.GetData();

			FillFromBinder(repParams, repBinderParams);

			if (!reanalise) {
				repParams.Params.Add("TotPagina", "");
				byte[] b = helper.GenerateReport(repParams);

				int pages = helper.GetTotalPages(b);
				//int pages = helper.GetTotalPages(repParams);
				repParams.Params["TotPagina"] = pages + "";
			}
		}


		private static FormNegativaVO Buscar(int id) {
			FormNegativaVO vo = FormNegativaBO.Instance.GetById(id);
			return vo;
		}


		protected override ReportNegativaBinder CreateBinder(FormNegativaVO vo) {
			return new ReportNegativaBinder(vo, reanalise);
		}
	}
}