using eVidaGeneralLib.BO;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioCreditoBeneficiario : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
			}
			this.Form.DefaultButton = btnBuscar.UniqueID;
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_CREDITO_BENEFICIARIO; }
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_beneficiario", 30);

			ExportExcel("RelatorioReembolsoAtend", defs, sourceTable);
		}

		private void BuscarRelatorio() {
			DateTime dtInicio;
			DateTime dtFim;
			string cartao = txtCartao.Text;

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}

			DataTable dtAcessos = RelatorioBO.Instance.CreditosBeneficiario(dtInicio, dtFim, cartao);

			btnExportar.Visible = dtAcessos.Rows.Count > 0;
			lblCount.Visible = true;
			lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

			this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);

		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				BuscarRelatorio();
			} catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio: ", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			} catch (Exception ex) {
				this.ShowError("Erro ao exportar!");
				Log.Error("Erro ao exportar para excel!", ex);
			}
		}
	}
}