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
	public partial class RelatorioBoletosNaoQuitados : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_BOLETO_NAO_QUITADOS; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			
			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarBoletosPendentes();

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_titular", 40);
			defs.SetWidth("nm_beneficiario", 40);

			ExportExcel("RelatorioBoletosNaoQuitados", defs, sourceTable);
		}
	}
}