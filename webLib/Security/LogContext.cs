using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eVida.Web.Security {
	public class LogContext {
		public override string ToString() {
			if (HttpContext.Current == null)
				return "NO-CTX";
			if (HttpContext.Current.Session == null) {
				if (HttpContext.Current.Items["USUARIO"] != null) {
					return "NS" + HttpContext.Current.Items["USUARIO"];
				} else {
					if (HttpContext.Current.Request.Cookies["USUARIO"] != null) {
						return "NSC" + HttpContext.Current.Request.Cookies["USUARIO"].Value;
					}
				}
				return "NO-SESSION";
			}			
			IUsuarioLogado vo = HttpContext.Current.Session["USUARIO"] as IUsuarioLogado;
			if (vo != null)
				return vo.Username;
			return "-";
		}
	}
}
