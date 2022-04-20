using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.SCL;

namespace eVidaIntranet.Classes {
	public abstract class PageBase : AllPageBase<UsuarioIntranetVO> {

		protected void Page_Load(object sender, EventArgs e) {
			Log.Debug("Acessando " + Request.HttpMethod + " " + Request.RawUrl);
			try {
				if (!IsLoginPage()) {
					if (PageHelper.CheckLogin(this)) {
						if (HasPermission(this.Modulo))
							PageLoad(sender, e);
						else {
							ShowAcessoNegado();
						}
					} else {
						ShowLoginPage();
					}
				} else {
					PageLoad(sender, e);
				}
			}
			catch (Exception ex) {
				if (!(ex is ThreadAbortException))
					this.ShowError("Erro ao carregar a página", ex);
			}
		}

		protected abstract void PageLoad(object sender, EventArgs e);

		protected abstract Modulo Modulo { get; }

		public override bool IsLoginPage() {
			return this.Modulo == eVidaGeneralLib.VO.Modulo.LOGIN;
		}
		
		public override UsuarioIntranetVO GetUsuario(string username) {
			
			UsuarioVO vo = UsuarioBO.Instance.GetUsuarioByLogin(username);
			List<Modulo> lstModulo = UsuarioBO.Instance.ListarModulosUsuario(vo.Id);
			eVidaGeneralLib.VO.SCL.SclUsuarioVO sclVO = UsuarioBO.Instance.GetUsuarioScl(username);

			UsuarioIntranetVO uVO = new UsuarioIntranetVO()
			{
				Usuario = vo,
				Permissoes = lstModulo,
				UsuarioScl = sclVO
			};
			return uVO;
		}

		protected List<SclUsuarioDominioVO> GetValoresDominios(SclUsuarioDominio dominio) {
			List<SclUsuarioDominioVO> lst = UsuarioLogado.UsuarioScl.Dominios;
			if (lst != null) {
				return lst.FindAll(x => x.IdDominio == dominio);
			}
			return new List<SclUsuarioDominioVO>();
		}

		protected void ShowAcessoNegado() {
			string parms = "?MODULO=" + this.Modulo;
			if (this is PopUpPageBase)
				Response.Redirect("~/Internal/PopUpNegado.aspx" + parms, true);
			else
				Response.Redirect("~/Internal/Negado.aspx" + parms, true);
		}

		protected void ShowLoginPage() {
			if (this is PopUpPageBase) {
				Response.Clear();
				Response.Write("Sessão expirada neste popup!");
			} else {
				Session[typeof(Login).ToString()] = this.Request.RawUrl;
				Response.Redirect("~/Login.aspx", true);
			}
		}
	}

}