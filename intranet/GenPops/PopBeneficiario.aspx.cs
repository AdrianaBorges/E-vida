using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.GenPops {
	public partial class PopBeneficiario : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				if (string.IsNullOrEmpty(txtCartao.Text) && string.IsNullOrEmpty(txtNome.Text)) {
					this.ShowError("Por favor, informe pelo menos um campo de filtro!");
					return;
				}
				if (!string.IsNullOrEmpty(txtNome.Text) && txtNome.Text.Length < 3) {
					this.ShowError("Por favor, para o nome informe pelo menos 3 caracteres!");
					return;
				}
				DataTable dt = PLocatorDataBO.Instance.BuscarUsuarios(txtCartao.Text, txtNome.Text);

				gdv.DataSource = dt;
				gdv.DataBind();

				if (dt.Rows.Count == 0)
					this.ShowInfo("Não foram encontrados beneficiários com este filtro!");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar os registros!", ex);
			}
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdv.Rows[index];

                string cod = Convert.ToString(gdv.DataKeys[index]["CD_BENEFICIARIO"]);

                this.RegisterScript("SERVICO", "setBeneficiario('" + cod + "');");
			}
		}
	}
}