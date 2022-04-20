using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Internal {
	public partial class Internal : System.Web.UI.MasterPage {
		protected void Page_Load(object sender, EventArgs e) {

		}

		protected void scriptManager_OnAsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e) {
			eVidaGeneralLib.Util.EVidaLog log = new eVidaGeneralLib.Util.EVidaLog(typeof(Internal));
			log.Error("Erro nao tratado: " + e.Exception);
			ScriptManager1.AsyncPostBackErrorMessage = e.Exception.Message;
		}
	}
}