using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using eVida.Web.Security;

namespace eVida.Web.Controls {
	public static class PageHelper {
		private const string SESSION_USUARIO = "USUARIO";

		public static bool IsLogged(HttpRequest request, HttpSessionState session) {
			return (GetUsuarioLogado(session) != null ||
						request.Cookies[FormsAuthentication.FormsCookieName] != null);
		}

		public static IUsuarioLogado GetUsuarioLogado(HttpSessionState session) {
			return (IUsuarioLogado)session[SESSION_USUARIO];
		}

		public static bool CheckLogin<T>(AllPageBase<T> pageBase) where T:IUsuarioLogado {
			Func<string, IUsuarioLogado> convert = delegate(string username)
			{
				return pageBase.GetUsuario(username);
			};
			return CheckLogin(pageBase.Request, pageBase.Response, pageBase.Session, convert);
		}

		public static bool CheckLogin(HttpRequest request, HttpResponse response, HttpSessionState session,
			Func<string, IUsuarioLogado> funcGetUsuario) {
			bool redoLogin = false;
			if (IsLogged(request, session)) {
				if (GetUsuarioLogado(session) == null) {
					redoLogin = !RefreshAuthentication(request, response, session, funcGetUsuario);
				}
			} else {
				redoLogin = true;
			}

			if (redoLogin) {
				return false;
			}
			return true;
		}

		public static bool RefreshAuthentication(HttpRequest request, HttpResponse response, HttpSessionState session,
			Func<string, IUsuarioLogado> funcGetUsuario) {
			try {
				HttpCookie authCookie = request.Cookies[FormsAuthentication.FormsCookieName];
				FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
				string username = ticket.UserData;

				IUsuarioLogado uVO = funcGetUsuario(username);
				if (uVO == null)
					return false;

				SaveAuthentication(uVO, session, response);
				return true;
			}
			catch (Exception ex) {
				FormsAuthentication.SignOut();
				new eVidaGeneralLib.Util.EVidaLog(typeof(PageHelper)).Error("Erro ao atualizar autenticacao", ex);
			}
			return false;
		}

		public static void SaveAuthentication(IUsuarioLogado uVO, HttpSessionState session, HttpResponse response, bool saveCookie = false) {
			session[SESSION_USUARIO] = uVO;
			HttpContext.Current.Items[SESSION_USUARIO] = uVO.Username;
			response.Cookies.Add(new HttpCookie(SESSION_USUARIO, uVO.Username));

			if (saveCookie) {
				FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "LOGIN_" + uVO.GetType().Name + "_" + uVO.Username,
					DateTime.Now, DateTime.Now.AddDays(1), true, uVO.Username);
				//Encrypt the authentication ticket
				string encrypetedTicket = FormsAuthentication.Encrypt(ticket);
				HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypetedTicket);
				//Set the cookie's expiration time to the tickets expiration time
				authCookie.Expires = ticket.Expiration;
				//Set the cookie in the Response
				response.Cookies.Add(authCookie);
			}

		}

		public static void DoLogout(HttpSessionState session) {
			session.Remove(SESSION_USUARIO);
			HttpContext.Current.Items.Remove(SESSION_USUARIO);
			HttpContext.Current.Response.Cookies.Remove(SESSION_USUARIO);
			FormsAuthentication.SignOut();
		}

	}
}
