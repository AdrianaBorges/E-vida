using eVida.Web.Controls;
using eVida.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eVidaCredenciados.Classes.Elegibilidade {

	public abstract class PageBase : AllPageBase<UsuarioNoLoginRequiredVO> {

		protected void Page_Load(object sender, EventArgs e) {
			Log.Debug("Acessando " + Request.RawUrl);
			try {
				if (!IsLoginPage()) {
					if (PageHelper.CheckLogin(this)) {
						PageLoad(sender, e);
					} else {
					}
				} else {
					PageLoad(sender, e);
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao carregar página!", ex);
			}

		}

		protected abstract void PageLoad(object sender, EventArgs e);


		public override bool IsLoginPage() {
			return this.Request.RawUrl.IndexOf("Elegibilidade/ConsultaElegibilidade.aspx", StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		public override UsuarioNoLoginRequiredVO GetUsuario(string username) {
			return new UsuarioNoLoginRequiredVO();
		}

	}
}