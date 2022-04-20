using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.Util;

namespace eVidaBeneficiarios.controls {
	public partial class Menu : UserControlBase {
		private int[] CATEGORIA_IR = new int[] { 3, 16, 24, 27, 28, 31, 32 };
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				if (this.UsuarioLogado == null)
					this.Visible = false;
				else {
					/*menuIr.Visible = false;
					if (this.UsuarioLogado.Plano != null) {
						string cdPlano = this.UsuarioLogado.Plano.CdPlano;
						if ("21".Equals(cdPlano) || "22".Equals(cdPlano)) {
							menuIr.Visible = true;
						}
					}
					if (this.UsuarioLogado.BeneficiarioCategoria != null) {
						int cdCategoria = this.UsuarioLogado.BeneficiarioCategoria.CdCategoria;
						if (CATEGORIA_IR.Contains(cdCategoria)) {
							menuIr.Visible = true;
						}
					}*/
					//menuIr.Visible = this.UsuarioLogado.ConfiguracaoIr.EnableIrBeneficiario;
				}
				//mnGalenus.Url = UrlGalenus;
			}
		}

		public string UrlGalenus {
			get { return ParametroUtil.UrlGalenus; }
		}

		protected void btnSair_Click(object sender, EventArgs e) {
			this.Session.Abandon();
			Response.Redirect("~/Login.aspx");
		}
	}
}