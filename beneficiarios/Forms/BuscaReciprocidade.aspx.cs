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
	public partial class BuscaReciprocidade : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
                    DataTable dt = ReciprocidadeBO.Instance.BuscarReciprocidade(UsuarioLogado.Codint, UsuarioLogado.Codemp, UsuarioLogado.Matric);
					this.ShowGrid(gdvRelatorio, dt, null, "cd_solicitacao DESC");
					lblCount.Text = "Você já realizou " + dt.Rows.Count + " solicitações!";
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar tela.", ex);

				}
			}
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				row.Cells[3].Text += " / " + Convert.ToString(vo["ds_uf"]);
				row.Cells[4].Text = ((StatusReciprocidade)Convert.ToInt32(vo["cd_status"])).ToString();

			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("~/Forms/Reciprocidade.aspx");
		}

	}
}