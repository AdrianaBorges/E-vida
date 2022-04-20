using eVidaGeneralLib.Util;
using eVidaGeneralLib.Util.Email;
using eVidaGeneralLib.VO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet {
	public partial class ShwCnfg : System.Web.UI.Page {
		protected void Page_Load(object sender, EventArgs e) {
			NameValueCollection appSettings = ConfigurationManager.AppSettings;

			Response.Write("<table id=\"tbConfig\" border=1 width=\"100%\">");
			Response.Write("<tr><td><b>Key</b></td><td><b>Value</b></td></tr>");
			foreach (string key in appSettings.AllKeys) {
				Response.Write("<tr>");
				Response.Write("<td>" + key + "</td>");
				Response.Write("<td>" + appSettings[key] + "</td>");
				Response.Write("</tr>");
			}
			Response.Write("</table>");

			string emByPass = ConfigurationManager.AppSettings["EMAIL_BYPASS"];
			if (string.IsNullOrEmpty(emByPass)) {
				Response.Write("Email bypass não configurado");
			} else {
				string[] dominios = emByPass.Split(';');
				Response.Write("Domínios sem validação:<br/>");
				foreach (string dom in dominios) {
					Response.Write(dom + "<BR/>");
				}
			}

			List<Dictionary<string,string>> lst = EmailProvider.ListConfig();

			Response.Write("<table id=\"tbConfigEmail\" border=1 width=\"100%\" style=\"border-collapse:collapse\">");
			bool hasHeader = false;
			foreach (Dictionary<string,string> emailConfig in lst) {
				if (!hasHeader) {
					Response.Write("<tr>");
					foreach (string key in emailConfig.Keys){
						Response.Write("<th><b>" + key +  "</b></th>");
					}
					Response.Write("</tr>");
					hasHeader = true;
				}
				Response.Write("<tr>");
				foreach (string key in emailConfig.Keys) {
					Response.Write("<td>" + emailConfig[key] + "</td>");
				}
				Response.Write("</tr>");
			}
			Response.Write("</table>");
			if (lst.Count == 0) {
				Response.Write("SEM EMAILS <br />");
			}

			Response.Write("<br /><table id=\"tbConfigEmail\" border=1 width=\"100%\" style=\"border-collapse:collapse\">");
			Response.Write("<tr><th><b>Key</b></th><th><b>ID</b></th><th><b>Value</b></th></tr>");
			List<ParametroVO> lstParams = ParametroUtil.ListAll();

			foreach (ParametroVO p in lstParams) {
				Response.Write("<tr>");
				Response.Write("<td>" + ((eVidaGeneralLib.Util.ParametroUtil.ParametroType) p.Id) + "</td>");
				Response.Write("<td>" + p.Id + "</td>");
				Response.Write("<td>" + p.Value + "</td>");
				Response.Write("</tr>");
			}
			Response.Write("</table>");

			Response.Write("<br /><table id=\"tbConfigEmail\" border=1 width=\"100%\" style=\"border-collapse:collapse\">");
			Response.Write("<tr><th><b>Key</b></th><th><b>Obj</b></th><th><b>Size</b></th></tr>");
			Dictionary<string, object> lstCache = CacheHelper.GetAllCache();

			foreach (string key in lstCache.Keys) {
				int size = -1;
				object value = lstCache[key];
				if (value is IList) {
					size = ((IList)value).Count;
				} else if (value is System.Data.DataTable) {
					size = ((System.Data.DataTable)value).Rows.Count;
				}
				Response.Write("<tr>");
				Response.Write("<td>" + key + "</td>");
				Response.Write("<td>" + lstCache[key] + "</td>");
				Response.Write("<td>" + size + "</td>");
				Response.Write("</tr>");
			}
			Response.Write("</table>");

		}
	}
}