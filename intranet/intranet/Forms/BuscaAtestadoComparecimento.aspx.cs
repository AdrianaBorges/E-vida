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
using eVidaGeneralLib.VO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Forms {
	public partial class BuscaAtestadoComparecimento : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdSituacao.Items.Add(new ListItem("TODOS", ""));
					foreach(int i in Enum.GetValues(typeof(StatusAtestadoComparecimento))) {
						dpdSituacao.Items.Add(new ListItem(((StatusAtestadoComparecimento)i).ToString(), i.ToString()));
					}

				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ATESTADO_COMPARECIMENTO; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
				ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;

				StatusAtestadoComparecimento status = (StatusAtestadoComparecimento)Convert.ToInt32(vo["cd_status"]);

				row.Cells[5].Text = status.ToString();

				btnEditar.Visible = status == StatusAtestadoComparecimento.PENDENTE;
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				long? matricula = null;
				int? cdProtocolo = null;
				StatusAtestadoComparecimento? status = null;
				int iValue;
				long lValue;

				if (!string.IsNullOrEmpty(txtMatricula.Text)) {
					if (Int64.TryParse(txtMatricula.Text, out lValue)) {
						matricula = lValue;
					}
				}

				if (!string.IsNullOrEmpty(txtProtocolo.Text))
					if (Int32.TryParse(txtProtocolo.Text, out iValue)) {
						cdProtocolo = iValue;
					}

				if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
					if (Int32.TryParse(dpdSituacao.SelectedValue, out iValue)) {
						status = (StatusAtestadoComparecimento)iValue;
					}
				}

                DataTable dt = AtestadoComparecimentoBO.Instance.Pesquisar(txtMatricula.Text, cdProtocolo, status);
				this.ShowPagingGrid(gdvRelatorio, dt, null);

				lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
				btnExportar.Visible = dt.Rows.Count > 0;
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar os dados!", ex);
			}
		}

		private void ExportarExcel() {
			DataTable sourceTable = GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);
			
			defs["CD_STATUS"][0].Transformer = x => ((StatusAtestadoComparecimento)Convert.ToInt32(x)).ToString();

			ExportExcel("AtestadosComparecimento", defs, sourceTable);
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar para excel!", ex);
			}
		}

	}
}