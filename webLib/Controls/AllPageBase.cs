using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Security;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;

namespace eVida.Web.Controls {
	public class RelatorioData {
		public DataTable Table { get; set; }
		public string UrlPageSource { get; set; }

		public bool CanBeUsed(HttpContext ctx) {
			if (string.IsNullOrEmpty(UrlPageSource)) return false;
			if (ctx.Request.Url.AbsolutePath.Equals(UrlPageSource, StringComparison.InvariantCultureIgnoreCase)) return true;
			return false;
		}

		public static RelatorioData Create(DataTable table, HttpContext ctx) {
			RelatorioData r = new RelatorioData();
			r.Table = table;
			r.UrlPageSource = ctx.Request.Url.AbsolutePath;
			return r;
		}
	}

	public abstract class AllPageBase<T> : Page where T:IUsuarioLogado{

		protected const string NOT_FOUND_LOCATOR = "$NOT_FOUND$";
		protected const string SESSION_USUARIO = "USUARIO";
		private MessageControl<T> msgControl;
		private eVidaGeneralLib.Util.EVidaLog log = null;

		protected class ItemError {
			public WebControl Control { get; set; }
			public string Message { get; set; }
		}

		public abstract T GetUsuario(string username);

		public abstract bool IsLoginPage();

		protected void Page_PreLoad(object sender, EventArgs e) {
			MsgControl.ClearMessage();
		}

		protected eVidaGeneralLib.Util.EVidaLog Log {
			get {
				if (log == null) {
					log = new eVidaGeneralLib.Util.EVidaLog(this.GetType());
				}
				return log;
			}
		}

		public T UsuarioLogado {
			get { return (T)Session[SESSION_USUARIO]; }
		}

		private MessageControl<T> MsgControl {
			get {
				if (msgControl == null) msgControl = new MessageControl<T>(this);
				return msgControl;
			}
		}

		protected string ReportDir {
			get {
				string dir = ParametroUtil.ReportRdlcFolder;
				if (string.IsNullOrEmpty(dir))
					dir = Request.MapPath("~/rpts");
				return dir;
			}
		}

		public bool HasPermission(Modulo modulo) {
			IUsuarioLogado usuario = UsuarioLogado;
			if (usuario == null)
				return false;
			return usuario.HasPermission(modulo);
		}

		protected void ShowError(string msg) {
			MsgControl.ShowError(msg);
		}

		protected void ShowError(string msg, Exception ex) {
			MsgControl.ShowError(msg + " - " + ex.Message);
			Log.Error(msg, ex);
		}

		protected void ShowInfo(string msg) {
			MsgControl.ShowInfo(msg);
		}

		protected void ShowError(Exception ex) {
			MsgControl.ShowError(ex);
		}

		protected void SetFocus(WebControl ctr) {
			MsgControl.SetFocus(ctr);
		}

		protected void RegisterScript(string key, string script) {
			MsgControl.RegisterScript(script, key);
		}

		protected void AddErrorMessage(List<ItemError> lst, WebControl ctr, string msg) {
			lst.Add(new ItemError()
			{
				Control = ctr,
				Message = msg
			});
		}

		protected void ShowErrorList(IEnumerable<ItemError> lstMsg) {
			WebControl ctrFocus = null;
			if (lstMsg.Count() > 0) {
				StringBuilder sb = new StringBuilder();
				foreach (ItemError m in lstMsg) {
					if (!string.IsNullOrEmpty(m.Message))
						sb.AppendLine(m.Message);
					if (ctrFocus == null)
						ctrFocus = m.Control;
				}
				if (sb.Length > 0) {
					this.ShowError(sb.ToString());
				}
			}
			SetFocus(ctrFocus);
		}

		protected DataView Sort(DataTable dt, string sortField) {
			DataView view = new DataView(dt);
			string sortExpression = "";
			if (this.ViewState["sortExpression"] != null) {
				sortExpression = ViewState["sortExpression"].ToString().Trim();
			}
			string[] sortData = sortExpression.Split(' ');
			if (sortField == sortData[0]) {
				if (sortData[1] == "ASC") {
					view.Sort = sortField + " " + "DESC";
					this.ViewState["sortExpression"] = sortField + " " + "DESC";
				} else {
					view.Sort = sortField + " " + "ASC";
					this.ViewState["sortExpression"] = sortField + " " + "ASC";
				}
			} else {
				view.Sort = sortField + " " + "ASC";
				this.ViewState["sortExpression"] = sortField + " " + "ASC";
			}

			return view;
		}

		protected void ShowGrid(GridView gdv, DataTable sourceTable, int? newPageIndex, string sortExpression) {
			if (sourceTable == null)
				return;

			DataView dv = new DataView(sourceTable);
			if (newPageIndex.HasValue)
				this.ViewState["pageIndex"] = newPageIndex.Value;
			else if (this.ViewState["pageIndex"] == null)
				this.ViewState["pageIndex"] = 0;

			if (string.IsNullOrEmpty(sortExpression) && this.ViewState["sortExpression"] != null)
				dv.Sort = (string)this.ViewState["sortExpression"];
			else {
				if (sortExpression != null) {
					if (sortExpression.EndsWith(" ASC", StringComparison.InvariantCultureIgnoreCase) ||
						sortExpression.EndsWith(" DESC", StringComparison.InvariantCultureIgnoreCase)) {
						this.ViewState["sortExpression"] = sortExpression;
						dv.Sort = sortExpression;
					} else {
						dv = Sort(sourceTable, sortExpression);
					}
				}
			}

			gdv.PageIndex = (int)this.ViewState["pageIndex"];
			gdv.DataSource = dv;
			gdv.DataBind();

		}

		public string CreateJsFunctionGrid(IDataItemContainer row, string jsFn, params string[] idCols) {
			if (row is GridViewRow) {
				return CreateJsFunctionGrid(row as GridViewRow, jsFn, idCols);
			}
			return "alert('Not implemented'); return false;";
		}

		public string CreateJsFunctionGrid(GridViewRow row, string jsFn, params string[] idCols) {
			string id = "";
			if (idCols == null || idCols.Length == 0) {
				GridView gridView = (GridView)row.NamingContainer;
				idCols = gridView.DataKeyNames;
			}
			foreach (string idCol in idCols) {
				if (!string.IsNullOrEmpty(id)) id += ";";
				id += Eval(idCol);
			}
			return "return " + jsFn + "(this,'" + id + "'," + row.RowIndex + ")";
		}

		public void PopulateByEnum<E>(ListItemCollection lstItems, Func<E, string> tradutor) where E : struct {
			foreach (E item in Enum.GetValues(typeof(E))) {
				lstItems.Add(CreateListItemByEnum<E>(item, tradutor));
			}
		}

		public ListItem CreateListItemByEnum<E>(E value, Func<E, string> tradutor) {
			return new ListItem(tradutor(value), Convert.ToInt32(value).ToString());
		}

		public static E? EnumFromListItem<E>(string value) where E : struct {
			if (string.IsNullOrEmpty(value))
				return null;
			int iValue = Int32.Parse(value);
			return (E)Enum.Parse(typeof(E), iValue.ToString(), true);
		} 
	}
}