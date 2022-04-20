using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using eVida.Web.Security;

namespace eVida.Web.Controls {
	public class UserControlBase<T> : UserControl where T:IUsuarioLogado{

		protected const string SESSION_USUARIO = "USUARIO";
		private MessageControl<T> msgControl;
		private eVidaGeneralLib.Util.EVidaLog log = null;

		public MessageControl<T> MsgControl {
			get {
				if (msgControl == null) msgControl = new MessageControl<T>((AllPageBase<T>)this.Page);
				return msgControl;
			}
		}

		protected void ShowError(string msg) {
			MsgControl.ShowError(msg);
		}

		protected void ShowInfo(string msg) {
			MsgControl.ShowInfo(msg);
		}

		protected T UsuarioLogado {
			get { return ((AllPageBase<T>)this.Page).UsuarioLogado; }
		}

		protected eVidaGeneralLib.Util.EVidaLog Log {
			get {
				if (log == null) {
					log = new eVidaGeneralLib.Util.EVidaLog(this.GetType());
				}
				return log;
			}
		}
	}
}
