using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;
using eVida.Web.Controls;

namespace eVidaIntranet.Classes {

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

		protected bool IsPagingCommand(object sender, GridViewCommandEventArgs e) {
			return "Page".Equals(e.CommandName, StringComparison.InvariantCultureIgnoreCase);
		}

		protected DataKey GetKeyCommand(GridViewCommandEventArgs e, GridView gdv) {
			if (e.CommandArgument == null) return null;

			// Retrieve the row index stored in the CommandArgument property.
			int index = 0;

			if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out index)) {
				throw new InvalidOperationException("GetKeyCommand apenas pode ser usado quando o CommandArgumento é o index da linha");
			}

			// Retrieve the row that contains the button from the Rows collection.
			DataKey keys = gdv.DataKeys[index];
			return keys;
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

		protected void ShowPagingGrid(GridView gdv, DataTable sourceTable, string sortExpression) {
			this.Session["RELATORIO"] = RelatorioData.Create(sourceTable, this.Context);
			this.ViewState["sortExpression"] = null;
			this.ViewState["pageIndex"] = null;
			this.ShowGrid(gdv, sourceTable, null, sortExpression);
		}


		protected E? ChangeCellEnumDrv<E>(GridView gdv, GridViewRow row, string dataField, Func<E, string> tradutor) where E : struct {
			DataRowView drv = (DataRowView)row.DataItem;
			TableCell cell = GetTableCell(gdv, row, dataField);
			return ChangeCellEnum(drv, cell, dataField, tradutor);
		}

		protected E? ChangeCellEnum<E>(DataRowView drv, TableCell cell, string dataField, Func<E, string> tradutor) where E : struct {
			E? enValue = EnumFromListItem<E>(Convert.ToString(drv[dataField]));
			if (enValue == null)
				cell.Text = "";
			cell.Text = tradutor(enValue.Value);
			return enValue;
		}

		protected TableCell GetTableCell(GridView gdv, GridViewRow row, string dataField) {
			int idx = 0;
			foreach (DataControlField field in gdv.Columns) {
				if (field is BoundField) {
					string confDf = ((BoundField)field).DataField;
					if (dataField.Equals(confDf)) {
						return row.Cells[idx];
					}
				}
				idx++;
			}
			throw new InvalidOperationException("Column " + dataField + " not found on GetCell!");
		}
	}
}