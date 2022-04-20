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
	internal class ListReports : ReportBase<string,DefaultReportBinder> {
		public ListReports(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}
		public override void DoAll(HttpContext context) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("<table style=\"width:300px\">");
			sb.AppendLine("<tr><th>Report</th></tr>");
			foreach (ReportHandler.EnumRelatorio en in ReportHandler.GetValues<ReportHandler.EnumRelatorio>()) {
				sb.AppendLine("<tr><td>");
				sb.AppendFormat("<a href=\"../relatorio.evida?REPORT={0}&GEN=true\">{0}</a>", en.ToString());
				sb.AppendLine("</td></tr>");
			}
			sb.AppendLine("</table>");

			context.Response.Clear();
			context.Response.ClearHeaders();
			context.Response.ClearContent();
			context.Response.BufferOutput = true;
			context.Response.CacheControl = "No-cache";
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			context.Response.ContentType = "text/html";

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
