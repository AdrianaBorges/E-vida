using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet
{
	public partial class Site : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			
		}

		protected void scriptManager_OnAsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e) {
			ScriptManager1.AsyncPostBackErrorMessage = e.Exception.Message;
		}
	}
}