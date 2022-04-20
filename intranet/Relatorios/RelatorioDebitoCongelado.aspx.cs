using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.HC;
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
	public partial class RelatorioDebitoCongelado : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<HcPlanoVO> lstPlano = LocatorDataBO.Instance.ListarPlanos();
				dpdPlano.DataSource = lstPlano.Select(x => new HcPlanoVO() {
					CdPlano = x.CdPlano,
					DsPlano = x.CdPlano + " - " + x.DsPlano
				});
				dpdPlano.DataBind();
				dpdPlano.Items.Insert(0, new ListItem("TODOS", ""));
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_DEBITO_CONGELADO; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dataReferencia;
			string plano = null;

			if (!DateTime.TryParse(txtMesReferencia.Text, out dataReferencia)) {
				this.ShowError("Informe uma data de referência correta!");
				return;
			}

			try {
				plano = dpdPlano.SelectedValue;
				DataTable dtAcessos = RelatorioBO.Instance.DebitosCongelados(dataReferencia, plano);

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
			} catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_beneficiario", 40);

			ExportExcel("DebitosCongelados", defs, sourceTable);
		}
	}
}