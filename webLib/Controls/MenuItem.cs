using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Security;
using eVidaGeneralLib.VO;

namespace eVida.Web.Controls {
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:MenuItem runat=server></{0}:MenuItem>")]
	public class MenuItem : WebControl {
				
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		public string Url {
			get {
				String s = (String)ViewState["URL"];
				return ((s == null) ? String.Empty : s);
			}

			set {
				ViewState["URL"] = value;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Label {
			get {
				String s = (String)ViewState["LABEL"];
				return ((s == null) ? String.Empty : s);
			}

			set {
				ViewState["LABEL"] = value;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		public Modulo? Modulo {
			get {
				Modulo? s = (Modulo?)ViewState["MODULO"];
				return s;
			}

			set {
				ViewState["MODULO"] = value;
			}
		}

		[PersistenceMode(PersistenceMode.InnerProperty)]
		public List<MenuItem> Items { get; set; }

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Text {
			get {
				String s = (String)ViewState["Text"];
				return ((s == null) ? String.Empty : s);
			}

			set {
				ViewState["Text"] = value;
			}
		}

		private bool HasPermission() {
			if (Modulo != null) {
				IUsuarioLogado usuario = PageHelper.GetUsuarioLogado(this.Context.Session);
				if (usuario == null)
					return false;
				return usuario.HasPermission(this.Modulo.Value);
			} else {
				return true;
			}
		}

		protected override void RenderContents(HtmlTextWriter output) {
			if (this.Modulo != null) {
				if (!HasPermission()) {
					this.Visible = false;
				}
			}

			if (this.Visible) {
				string html = this.WriteHtml();
				output.Write(html);
			}
		}


		protected string WriteHtml() {
			if (!this.Visible)
				return null;
			if (this.Modulo != null) {
				if (!HasPermission()) {
					return null;
				}
			}

			bool isParent = string.IsNullOrEmpty(this.Url);

			StringBuilder sbChildren = new StringBuilder();

			bool hasVisibileChild = false;
			if (Items != null) {
				foreach (MenuItem item in Items) {
					string sub = item.WriteHtml();
					if (sub != null) {
						hasVisibileChild = true;
						sbChildren.Append(sub);
					}
				}
			}

			if (isParent && !hasVisibileChild)
				return null;

			StringBuilder sb = new StringBuilder();
			sb.Append("<li>");

			if (isParent)
				//sb.Append(Label);
				sb.Append("<a href=\"#\" class=\"menuPai\">").Append(Label).Append("</a>");
			else
				sb.Append("<a href=\"").Append(ResolveUrl(Url)).Append("\">").Append(Label).Append("</a>");

			if (hasVisibileChild) {
				sb.Append("<ul>");
				sb.Append(sbChildren);
				sb.Append("</ul>");
			}
			sb.Append("</li>");


			return sb.ToString();
		}

	}
}
