using eVida.Web.Security;
using eVidaGeneralLib.Reporting;
using SkyReport.ReportingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eVida.Web.Report.CustomHandlers {
	internal class JsReport : ReportBase<string, DefaultReportBinder> {
		public JsReport(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}
		protected override string GerarNome(string vo) {
			return null;
		}

		protected override string DefaultRpt(string vo) {
			return null;
		}

		protected override void FillReport(string vo, ReportParams parameters, RelatorioHelper helper) {
			return;
		}
		
		public override void DoAll(HttpContext context) {
			StringBuilder sb = new StringBuilder();

			foreach (ReportHandler.EnumRelatorio en in ReportHandler.GetValues<ReportHandler.EnumRelatorio>()) {
				sb.AppendFormat("var {0} = '{1}';", ReportHandler.GetJsVarName(en), en.ToString());
				sb.AppendLine("");
			}

			context.Response.Clear();
			context.Response.ClearHeaders();
			context.Response.ClearContent();
			context.Response.BufferOutput = true;
			context.Response.CacheControl = "No-cache";
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			context.Response.ContentType = "text/javascript";

			context.Response.Write(sb.ToString());
		}
		protected override DefaultReportBinder CreateBinder(string vo) {
			return new DefaultReportBinder();
		}

		internal override string GerarDados(HttpRequest request) {
			throw new NotImplementedException();
		}
	}
}
