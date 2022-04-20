using eVida.Web.Controls;
using eVida.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace eVidaBeneficiarios.Classes.Externo {
	public abstract class PageBase : AllPageBase<UsuarioNoLoginRequiredVO> {

		protected void Page_Load(object sender, EventArgs e) {
			Log.Debug("Acessando " + Request.RawUrl);
			try {
				if (!IsLoginPage()) {
					if (PageHelper.CheckLogin(this)) {
						PageLoad(sender, e);
					} else {
						Response.Redirect(GetInitialPage(), false);
					}
				} else {
					PageLoad(sender, e);
				}
			} catch (Exception ex) {
				if (!(ex is ThreadAbortException))
					this.ShowError("Erro ao carregar a página", ex);
			}

		}

		protected abstract void PageLoad(object sender, EventArgs e);
		protected abstract string GetInitialPage();

		
		public override UsuarioNoLoginRequiredVO GetUsuario(string username) {
			return null;
		}

	}
}