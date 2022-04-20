using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioBoletosVencimento : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_BOLETO_VENCIMENTO; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dataReferencia;

			if (!DateTime.TryParse(txtVencimento.Text, out dataReferencia)) {
				this.ShowError("Informe uma data de vencimento correta!");
				return;
			}

			try {
				while (gdvRelatorio.Columns.Count > 13) {
					gdvRelatorio.Columns.RemoveAt(13);
				}

				List<string> lstCategorias;
				DataTable dtAcessos = RelatorioBO.Instance.BuscarBoletosVencimento(dataReferencia, out lstCategorias);

				if (lstCategorias != null && lstCategorias.Count > 0) {
					lstCategorias.Sort();
					foreach (string cat in lstCategorias) {
						BoundField nameColumn = new BoundField();
						nameColumn.HeaderText = cat;
						nameColumn.DataField = cat;
						nameColumn.DataFormatString = "{0:C}";
						nameColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
						nameColumn.ItemStyle.Wrap = false;
						gdvRelatorio.Columns.Add(nameColumn);
					}
				}

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, "cd_funcionario");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("ds_email", 40);
			defs.SetWidth("ds_categoria", 25);
			defs.SetWidth("nm_titular", 40);

			ExportExcel("RelatorioBoletos", defs, sourceTable);
		}
	}
}