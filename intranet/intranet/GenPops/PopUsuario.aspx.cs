using eVidaGeneralLib.BO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.GenPops {
	public partial class PopUsuario : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			if (txtLogin.Text.Length < 3) {
				this.ShowError("Por favor, informe pelo menos 3 caracteres para filtro!");
				return;
			}
			DataTable dt = UsuarioBO.Instance.PesquisarUsuariosInterno(txtLogin.Text, null, null);
			gdvUsuario.DataSource = dt;
			gdvUsuario.DataBind();

			if (dt.Rows.Count == 0) {
				this.ShowInfo("Não foram encontrados usuários com este filtro!");
			}
		}

		protected void gdvUsuario_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdvUsuario.Rows[index];

				string id = Convert.ToString(gdvUsuario.DataKeys[index]["ID_USUARIO"]);

				this.RegisterScript("USUARIO", "setUsuario(" + id + ");");
			}
		}

	}
}