using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using System.Web.Security;
using eVidaGeneralLib.VO;
using eVida.Web.Security;

namespace eVidaIntranet.controls {
	public partial class Header : UserControlBase {
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				UsuarioIntranetVO uVO = UsuarioLogado;
				if (uVO != null) {
					UsuarioVO u = uVO.Usuario;
					ltNome.Text = u.Nome + " (" + u.Login + ")";
					if (u.UltimoLogin.HasValue) {
						ltUltAcesso.Text = u.UltimoLogin.Value.ToString("dd/MM/yyyy HH:mm:ss");
					}
					else {
						ltUltAcesso.Text = "-";
					}
					divHelp.Visible = false;
					//if (System.IO.File.Exists(Server.MapPath("~/Help/")))
				}
				else {
					divHelp.Visible = false;
					lnkInicial.Enabled = false;
					divMenu.Visible = false;
				}
			}
		}

		protected void btnSair_Click(object sender, EventArgs e) {
			FormsAuthentication.SignOut();
			Session.Abandon();
			Response.Redirect("~/Login.aspx");
		}
	}
}