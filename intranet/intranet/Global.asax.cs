using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using eVida.Web.Security;
using eVidaIntranet.Classes;
using eVidaGeneralLib.VO;
using System.Diagnostics;
using System.Web.UI;
using System.Globalization;
using System.Threading;

namespace eVidaIntranet {
	public class Global : System.Web.HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			eVidaGeneralLib.Util.EVidaLog.Initialize();
			eVidaGeneralLib.Util.EVidaLog.Context = new LogContext();

			string JQueryVer = "1.8.2";
			ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
				new ScriptResourceDefinition {
					Path = "~/Scripts/jquery-" + JQueryVer + ".min.js",
					DebugPath = "~/Scripts/jquery-" + JQueryVer + ".js",
					CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".min.js",
					CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".js",
					CdnSupportsSecureConnection = true,
					LoadSuccessExpression = "window.jQuery"
				});
		}

		protected void Session_Start(object sender, EventArgs e) {

		}

		protected void Application_BeginRequest(object sender, EventArgs e) 
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
		}
		protected void Application_EndRequest() {
			string msg = string.Format("{0}:{1}",
				 (DateTime.Now - HttpContext.Current.Timestamp).TotalMilliseconds,
				 HttpContext.Current.Request.RawUrl);
			Trace.WriteLine(msg);
			try {
				eVidaGeneralLib.Util.EVidaLog log = new eVidaGeneralLib.Util.EVidaLog(typeof(Global));
				log.Info(msg);
			} catch {

			}
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {

		}

		protected void Application_Error(object sender, EventArgs e) {
			
		}

		protected void Session_End(object sender, EventArgs e) {

		}

		protected void Application_End(object sender, EventArgs e) {

		}
	}
}