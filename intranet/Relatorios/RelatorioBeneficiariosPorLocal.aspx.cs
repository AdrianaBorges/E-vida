using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioBeneficiariosPorLocal : RelatorioExcelPageBase {

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ScriptManager manager = (ScriptManager)Master.FindControl("ScriptManager1");
            manager.AsyncPostBackTimeout = 36000;   // O script pode rodar por até 10 horas, sem dar timeout
        }

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				IEnumerable<object> lstLocal = PLocatorDataBO.Instance.ListarRegioes().Select(x => new {
					Codigo = x.Key,
					Descricao = x.Value
				});
				chkRegional.DataSource = lstLocal;
				chkRegional.DataBind();

				chkUf.DataSource = PLocatorDataBO.Instance.ListarUf();
				chkUf.DataBind();

				chkParentesco.DataSource = PLocatorDataBO.Instance.ListaParentescos();
				chkParentesco.DataBind();
				//chkParentesco.Items.Insert(0, new ListItem("TITULAR", "01"));

				chkPlano.DataSource = PLocatorDataBO.Instance.ListarProdutoSaude().Select(x => new {
					Codigo = x.Codigo,
                    Descricao = x.Descri + " (" + x.Codigo + ") "
				});
				chkPlano.DataBind();
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_BENEFICIARIOS_LOCAL; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_titular", 40);
			defs.SetWidth("nm_beneficiario", 40);
			ExportExcel("RelatorioBeneficiariosPorLocal", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
            List<string> lstRegional = new List<string>();
            List<string> lstPlano = new List<string>();
            List<string> lstGrauParentesco = new List<string>();
			List<string> lstUf = new List<string>();
			bool? deficienteFisico = null;
			bool? estudante = null;
			string tipoRelatorio = rblTipoRelatorio.SelectedValue;

			foreach (ListItem item in chkRegional.Items) {
				if (item.Selected)
					lstRegional.Add(item.Value);
			}
			foreach (ListItem item in chkPlano.Items) {
				if (item.Selected)
					lstPlano.Add(item.Value);
			}
			foreach (ListItem item in chkUf.Items) {
				if (item.Selected)
					lstUf.Add(item.Value);
			}
			foreach (ListItem item in chkParentesco.Items) {
				if (item.Selected)
					lstGrauParentesco.Add(item.Value);
			}

			if (!string.IsNullOrEmpty(dpdDeficienteFisico.SelectedValue)) {
				deficienteFisico = dpdDeficienteFisico.SelectedValue.Equals("S");
			}
			if (!string.IsNullOrEmpty(dpdEstudante.SelectedValue)) {
				estudante = dpdEstudante.SelectedValue.Equals("S");
			}

			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarBeneficiariosPorLocal(tipoRelatorio, lstRegional, lstPlano, lstUf, lstGrauParentesco, deficienteFisico, estudante);

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";
				
				bool sintetico = tipoRelatorio.Equals("S");
				int[] colAnalitico = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 14, 15, 16 };
				int[] colSintetico = new int[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };

				foreach (int col in colAnalitico) {
					gdvRelatorio.Columns[col].Visible = !sintetico;
				}
				foreach (int col in colSintetico) {
					gdvRelatorio.Columns[col].Visible = sintetico;
				}

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
				pnlGrid.Update();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}
	}
}