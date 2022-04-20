using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Security;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;

namespace eVida.Web.Controls {
	public class MessageControl<T> where T:IUsuarioLogado {

		private int counter = 0;
		private AllPageBase<T> page;

		public MessageControl(AllPageBase<T> p) {
			page = p;
		}

		public void ShowError(string msg) {
			ShowMessage(msg, true);
		}

		public void ShowInfo(string msg) {
			ShowMessage(msg, false);
		}

		public void SetFocus(WebControl ctrl) {
			if (ctrl != null)
				RegisterScript("ctrFocus = '" + ctrl.ClientID + "';", "SetFocus");
		}

		public void ClearMessage() {
			//RegisterScript("clearStartupMessage(); ", "ClearInfo");
		}
		
		private void ShowMessage(string msg, bool erro) {
			if (counter == 0) {
				//RegisterScript("clearStartupMessage(); ", "ClearInfo");
			}
			msg = msg.Replace('\'', '\"');
			msg = msg.Replace("\n\r", "<br>");
			msg = msg.Replace("\r\n", "<br>");
			msg = msg.Replace('\n', ' ');
			msg = msg.Replace('\r', ' ');

			if (counter > 0)
				msg = "<br>" + msg;

			string script = "showStartupMessage('" + msg + "', " + (erro ? "true" : "false") + ");";
			counter++;
			RegisterScript(script, "ShowInfo"+counter);
		}

		public void RegisterScript(string script, string key) {
			ScriptManager sm = (ScriptManager)FindControl("ScriptManager1");
			if (sm != null) {
				//UpdatePanel up = (UpdatePanel)FindControl("content");
				//if (up != null) {
				//ScriptManager.RegisterClientScriptBlock(up, up.GetType(), key, script, true);
				ScriptManager.RegisterClientScriptBlock(page, page.GetType(), key, script, true);
				return;
				//}
			}

			page.ClientScript.RegisterStartupScript(this.GetType(), key, script, true);
		}

		private Control FindControl(string id) {
			Control ctl = this.page;
			LinkedList<Control> ctls = new LinkedList<Control>();

			while (ctl != null) {
				if (ctl.ID == id)
					return ctl;
				foreach (Control child in ctl.Controls) {
					if (child.ID == id)
						return child;
					if (child.HasControls())
						ctls.AddLast(child);
				}
				if (ctls.Count > 0) {
					ctl = ctls.First.Value;
					ctls.Remove(ctl);
				}
				else {
					ctl = null;
				}
			}
			return null;
		}
		public void ShowError(Exception ex) {
			string msg = ex.Message;
			if (!(ex is EvidaException))
				msg = "Erro inesperado! " + msg + " - " + ex.Source;

			EVidaLog logger = new EVidaLog(this.GetType());
			logger.Error(msg, ex);

			ShowError(msg);
		}
	}
}