using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Internal
{
	public partial class Internal : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			
		}

		protected void scriptManager_OnAsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e) {
			ScriptManager1.AsyncPostBackErrorMessage = e.Exception.Message;
			eVidaGeneralLib.Util.EVidaLog log = new eVidaGeneralLib.Util.EVidaLog(typeof(Internal));
			log.Error("Erro não tratado: " + e.Exception.Message, e.Exception);
		}
	}
}