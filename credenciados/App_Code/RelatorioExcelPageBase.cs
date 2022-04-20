using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;
using eVida.Web.Controls;

namespace eVidaCredenciados.Classes {

	public abstract class RelatorioExcelPageBase : PageBase {
		protected ExcelColumnDefinitionCollection GetDefinitionsFromGrid(GridView gdv) {
			return ExcelPageHelper.GetDefinitionsFromGrid(gdv);
		}

		protected void ExportExcel(String reportName, ExcelColumnDefinitionCollection definitions, DataTable data) {
			ExportExcel(reportName, definitions.GetAll(), data);
		}

		protected void ExportExcel(String reportName, List<ExcelColumnDefinition> definitions, DataTable data) {
			ExcelPageHelper helper = new ExcelPageHelper(this.Context);
			helper.ExportExcel(reportName, definitions, data);

			string expUrl = ResolveUrl("~/excel.evida");
			RegisterScript("RELATORIO_STR", "window.open('" + expUrl + "?name=" + reportName + "');");
		}

		protected DataTable GetRelatorioTable() {
			RelatorioData data = this.Session["RELATORIO"] as RelatorioData;
			DataTable sourceTable = null;
			if (data == null) {
				sourceTable = this.Session["RELATORIO"] as DataTable;
			} else {
				if (data.CanBeUsed(this.Context))
					sourceTable = data.Table;
			}
			return sourceTable;
		}
		protected void gdvRelatorio_Sorting(object sender, GridViewSortEventArgs e) {
			DataTable sourceTable = GetRelatorioTable();
			if (sourceTable == null)
				return;

			this.ShowGrid((GridView)sender, sourceTable, null, e.SortExpression);
		}

		protected void gdvRelatorio_PageIndexChanging(object sender, GridViewPageEventArgs e) {
			DataTable sourceTable = GetRelatorioTable();
			if (sourceTable == null)
				return;
			this.ShowGrid((GridView)sender, sourceTable, e.NewPageIndex, null);
		}
	}
}