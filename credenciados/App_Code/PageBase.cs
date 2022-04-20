using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaCredenciados.Classes {
	public abstract class PageBase : AllPageBase<UsuarioCredenciadoVO> {

		protected void Page_Load(object sender, EventArgs e) {
			Log.Debug("Acessando " + Request.RawUrl);
			if (!IsLoginPage()) {
				if (PageHelper.CheckLogin(this)) {
					PageLoad(sender, e);
				} else {
					Session[typeof(Login).ToString()] = this.Request.RawUrl;
					Response.Redirect("~/Login.aspx", true);
				}
			} else {
				if (!IsPostBack) {
					PageHelper.DoLogout(Session);
				}
				PageLoad(sender, e);
			}			
		}

		protected abstract void PageLoad(object sender, EventArgs e);

		public override bool IsLoginPage() {
			return this.Request.RawUrl.IndexOf("Login.aspx", StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

        public override UsuarioCredenciadoVO GetUsuario(string codigo_credenciado)
        {
            string[] dados_credenciado = codigo_credenciado.Split('|');
            string username = dados_credenciado[0];
            string codigo = dados_credenciado[1];

            #region[CREDENCIADO NO ISA]

            long cpfCnpj;
			if (!Int64.TryParse(username, out cpfCnpj))
				return null;

			HcVCredenciadoVO voh = CredenciadoBO.Instance.GetByDoc(cpfCnpj);
			ConfiguracaoIrVO configIr = ConfiguracaoIrBO.Instance.GetConfiguracao();

			UsuarioCredenciadoVO uVO = new UsuarioCredenciadoVO()
			{
				Credenciado = voh,
				ConfiguracaoIr = configIr
			};

            #endregion

            #region[CREDENCIADO NO PROTHEUS]

            PRedeAtendimentoVO vop = PRedeAtendimentoBO.Instance.GetByDoc(codigo);

            uVO.RedeAtendimento = vop;

            #endregion

            return uVO;
		}
	}

}