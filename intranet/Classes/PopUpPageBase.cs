using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eVida.Web.Controls;
using eVida.Web.Security;

namespace eVidaIntranet.Classes {
	public abstract class PopUpPageBase : PageBase {

		protected void CallDefaultCallback(int id) {
			CallDefaultCallback(id.ToString());
		}

		protected void CallDefaultCallback(string id) {
			this.RegisterScript("callback", "parent.locatorCallback('" + id + "');");
		}
	}
}