using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.Relatorios {
	public partial class PopUsuarioAtendimento : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			if (txtNome.Text.Length < 3) {
				this.ShowError("Por favor, informe pelo menos 3 caracteres para filtro!");
				return;
			}
			List<string> ids = new List<string>();
			ids.Add(txtNome.Text);
			DataTable dt = RelatorioBO.Instance.BuscarUserUpdateAtendimento(ids);
			gdvUsuario.DataSource = dt;
			gdvUsuario.DataBind();

			if (dt.Rows.Count == 0) {
				this.ShowInfo("Não foram encontrados usuários com este filtro!");
				this.btnSelecao.Visible = false;
			} else {
				this.btnSelecao.Visible = true;
			}
		}

		protected void gdvUsuario_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdvUsuario.Rows[index];
				
				string cdUsuario = Convert.ToString(gdvUsuario.DataKeys[index]["CD_USUARIO"]);

				string nome = row.Cells[3].Text;

				this.RegisterScript("USUARIO", "addUsuario('" + cdUsuario + "','" + HttpUtility.UrlEncode(nome) + "');");
			}
		}

		protected void btnSelecao_Click(object sender, EventArgs e) {
			StringBuilder sb = new StringBuilder();
			foreach (GridViewRow row in gdvUsuario.Rows) {
				CheckBox chk = (CheckBox)row.Cells[0].FindControl("chkSingle");
				if (chk.Checked)
					sb.Append("|").Append(gdvUsuario.DataKeys[row.RowIndex]["CD_USUARIO"]);
			}
			if (sb.Length == 0) {
				ShowError("Por favor, selecione pelo menos um checkbox para a múltipla seleção!");
				return;
			}

			this.RegisterScript("MULT", "addUsuarios('" + sb.ToString() + "')");
		}
	}
}