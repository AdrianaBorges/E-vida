using eVidaGeneralLib.BO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Admin {
	public partial class BuscarUsuariosScl : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_USUARIO_SCL; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				DataTable dt = UsuarioBO.Instance.PesquisarUsuariosScl(txtLogin.Text, txtNome.Text);

				this.ShowPagingGrid(gdvUsuarios, dt, "NM_USUARIO");

			} catch (Exception ex) {
				this.ShowError("Erro ao consultar usuários!", ex);
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("./CriarUsuarioScl.aspx");
		}
	}
}