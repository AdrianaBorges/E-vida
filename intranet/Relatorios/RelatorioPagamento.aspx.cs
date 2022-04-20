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
	public partial class RelatorioPagamento : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				IEnumerable<object> lstLocal = LocatorDataBO.Instance.ListarRegionais().Select(x => new {
					Codigo = x.Key,
					Descricao = "(" + x.Key + ") " + x.Value
				});
				chkRegional.DataSource = lstLocal;
				chkRegional.DataBind();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_PROVISAO; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();
			try {
				ExcelColumnDefinitionCollection defs = ExcelPageHelper.GetDefinitionsFromGrid(gdvRelatorio);

				if (gdvRelatorio.Columns[0].Visible) {
					defs.SetWidth("cd_empresa", 7);
					defs.SetWidth("ds_empresa", 40);
				}
				
				defs.SetWidth("nr_cnpj_cpf", 12);
				defs.SetWidth("nm_razao_social", 40);
				defs.SetWidth("ds_natureza", 20);
				defs.SetWidth("nr_docto_complementar", 20);
				defs.SetWidth("vl_docto_complementar", 20);

				ExportExcel("RelatorioPagamento", defs, sourceTable);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar!", ex);
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dtInicio;
			DateTime dtFim;
			string tipoPessoaCred;
			List<int> lstRegional = new List<int>();

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}

			bool agrupado = rblTipoRelatorio.SelectedValue.Equals("A");
			tipoPessoaCred = dpdTipoPessoa.SelectedValue;

			foreach (ListItem item in chkRegional.Items) {
				if (item.Selected)
					lstRegional.Add(Int32.Parse(item.Value));
			}
			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarPagamento(agrupado, dtInicio, dtFim, tipoPessoaCred, lstRegional);

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				gdvRelatorio.Columns[0].Visible = !agrupado;
				gdvRelatorio.Columns[1].Visible = !agrupado;

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, "nm_razao_social");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio: ", ex);
			}
		}

	}
}