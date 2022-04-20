using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.BO;

namespace eVidaCredenciados.GenPops {
	public partial class PopServico : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				if (string.IsNullOrEmpty(Request["SHOWT"])) {
					ShowTabela = true;
				} else {
					ShowTabela = "TRUE".Equals(Request["SHOWT"], StringComparison.InvariantCultureIgnoreCase);
				}
			}
		}

		private bool ShowTabela {
			get { return ViewState["SHOW_TABELA"] != null ? (bool)ViewState["SHOW_TABELA"] : true; }
			set { ViewState["SHOW_TABELA"] = value; }
		}
		
		protected void btnBuscar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCodigo.Text) && string.IsNullOrEmpty(txtDescricao.Text)) {
				this.ShowError("Por favor, informe pelo menos um campo de filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtDescricao.Text) && txtDescricao.Text.Length < 3) {
				this.ShowError("Por favor, para a descrição informe pelo menos 3 caracteres!");
				return;
			}
            DataTable dt = PLocatorDataBO.Instance.BuscarServicos(txtCodigo.Text, txtDescricao.Text, ShowTabela);
			if (dt.Rows.Count > 300) {
				dt = dt.AsEnumerable().Take(300).CopyToDataTable();
				this.ShowInfo("Foram retornados apenas os 300 primeiros resultados da pesquisa. Por favor informe mais detalhes!");
			}
            gdv.DataKeyNames = new string[] { "CD_SERVICO" };
            gdv.Columns[2].Visible = false;

            gdv.DataSource = dt;
            gdv.DataBind();

			if (dt.Rows.Count == 0)
				this.ShowInfo("Não foram encontrados serviços com este filtro!");
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				//GridViewRow row = gdv.Rows[index];

				string cod = Convert.ToString(gdv.DataKeys[index][0]);
				this.RegisterScript("SERVICO", "setServico('" + cod + "');");
			}
		}
	}
}