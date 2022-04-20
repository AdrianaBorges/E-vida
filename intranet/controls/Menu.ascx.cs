using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.controls {
	public partial class Menu : UserControlBase {
		protected void Page_Load(object sender, EventArgs e) {
			if (this.UsuarioLogado == null)
				this.Visible = false;
			
		}

		protected void btnSair_Click(object sender, EventArgs e) {
			this.Session.Abandon();
			FormsAuthentication.SignOut();
			Response.Redirect("~/Login.aspx");
		}
	}
}