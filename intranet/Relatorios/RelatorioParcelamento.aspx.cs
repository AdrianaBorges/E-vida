using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioParcelamento : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				BuscarRelatorio();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_PARCELAMENTOS; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();
			if (sourceTable == null) {
				BuscarRelatorio();
			}
			sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("dt_ano_mes_ref", 8);
			defs.SetWidth("cd_grupo_lancto", 20);
			defs.SetWidth("nm_beneficiario", 40);

			ExportExcel("RelatorioParcelamento", defs, sourceTable);
		}

		private void BuscarRelatorio() {
			DataTable dtAcessos = RelatorioBO.Instance.BuscarParcelamento();
			btnExportar.Visible = dtAcessos.Rows.Count > 0;
			lblCount.Visible = true;
			lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

			this.ShowPagingGrid(gdvRelatorio, dtAcessos, "CD_ALTERNATIVO");
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {			
			try {
				BuscarRelatorio();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio: ", ex);
			}
		}

	}
}