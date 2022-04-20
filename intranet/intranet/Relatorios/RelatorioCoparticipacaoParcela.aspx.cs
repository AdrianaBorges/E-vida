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
	public partial class RelatorioCoparticipacaoParcela : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				txtInicio.Text = "01/" + DateTime.Now.ToString("MM/yyyy");
				txtFim.Text = "01/" + DateTime.Now.ToString("MM/yyyy");
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_COPARTICIPACAO; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("dt_ano_mes_ref", 8);
			defs.SetWidth("cd_grupo_lancto", 20);
			defs.SetWidth("ds_categoria", 25);
			defs.SetWidth("nm_beneficiario", 40);

			ExportExcel("RelatorioCoparticipacaoParcela", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dtInicio;
			DateTime dtFim;
			List<string> lstSituacao = new List<string>();

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}
			if (dtFim < dtInicio) {
				this.ShowError("A data final deve ser maior que a data inicial!");
				return;
			}
			foreach (ListItem item in chkListSituacao.Items) {
				if (item.Selected)
					lstSituacao.Add(item.Value);
			}

			if (lstSituacao.Count == 0) {
				this.ShowError("Por favor, selecione pelo menos uma das situações desejadas!");
				return;
			}
			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarCoparticipacao(dtInicio, dtFim, lstSituacao, txtCartaoTitular.Text, true);
				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
			} catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio: ", ex);
			}
		}

	}
}