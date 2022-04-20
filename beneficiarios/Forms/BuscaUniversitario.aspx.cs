using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaBeneficiarios.Forms {
	public partial class BuscaUniversitario : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
                DataTable dt = DeclaracaoUniversitarioBO.Instance.PesquisarDeclaracoes(null, UsuarioLogado.UsuarioTitular.Matemp, null, null, null);
				this.ShowGrid(gdvRelatorio, dt, null, "cd_solicitacao DESC");
				lblCount.Text = "Você já realizou " + dt.Rows.Count + " solicitações!";
			}
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				StatusDeclaracaoUniversitario status = (StatusDeclaracaoUniversitario)Convert.ToInt32(vo["CD_STATUS"]);
				row.Cells[2].Text = DeclaracaoUniversitarioEnumTradutor.TraduzStatus(status);

				if (status == StatusDeclaracaoUniversitario.RECUSADO) {
					row.Cells[2].Text += " - " + Convert.ToString(vo["DS_MOTIVO_CANCELAMENTO"]);
				}
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("~/Forms/Universitario.aspx");
		}

		protected void btnPesquisar_Click(object sender, EventArgs e) {
			string strStatus = dpdFiltroSituacao.SelectedValue;

			StatusDeclaracaoUniversitario? status = null;
			if (!string.IsNullOrEmpty(strStatus))
				status = (StatusDeclaracaoUniversitario)Int32.Parse(strStatus);

            DataTable dt = DeclaracaoUniversitarioBO.Instance.PesquisarDeclaracoes(null, UsuarioLogado.UsuarioTitular.Matemp, null, null, status);
			this.ShowGrid(gdvRelatorio, dt, null, "cd_solicitacao DESC");
			lblCount.Text = "Foram encontradas " + dt.Rows.Count + " solicitações!";
		}

	}
}