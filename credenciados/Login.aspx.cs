using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaCredenciados {
	public partial class Login : PageBase {
		
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string nextUrl = Session[typeof(Login).ToString()] as string;
				if (nextUrl == null)
					lblLoginExpirado.Visible = false;
				//Session.Clear();
				if (nextUrl != null)
					Session["NEXT_URL"] = nextUrl;
			}
		}

		protected void btnLogin_Click(object sender, EventArgs e) {
			string login = txtLogin.Text;
			string senha = txtSenha.Text;
			
			lblLoginExpirado.Visible = false;

			if (string.IsNullOrEmpty(login)) {
				this.ShowError("O login é obrigatório!");
				return;
			} 

			if (string.IsNullOrEmpty(senha)) {
				this.ShowError("A senha é obrigatória!");
				return;
			}

			//Thread.Sleep(1000);

			try
            {

                #region[CREDENCIADO NO ISA]

                /*CredenciadoBO boh = CredenciadoBO.Instance;
				HcVCredenciadoVO voh = boh.LogarCredenciado(login, senha);

				if (voh == null) {
					this.ShowError("Usuario ou Senha inválidos!");
					return;
				}
				if (voh.CdCredenciado == 0) {
					this.ShowError("Seus dados estão incompletos ou você não é um credenciado, favor entrar em contato com o suporte!");
					return;
                }*/

                HcVCredenciadoVO voh = new HcVCredenciadoVO();

                #endregion

                #region[CREDENCIADO NO PROTHEUS]

                /*PRedeAtendimentoBO bop = PRedeAtendimentoBO.Instance;
                PRedeAtendimentoVO vop = bop.LogarRedeAtendimento(login, senha);

                if (vop == null)
                {
                    this.ShowError("Usuario ou Senha inválidos!");
                    return;
                }
                if (string.IsNullOrEmpty(vop.Codigo))
                {
                    this.ShowError("Seus dados estão incompletos ou você não é um credenciado, favor entrar em contato com o suporte!");
                    return;
                }*/

                PUsuarioPortalVO vop = PUsuarioPortalBO.Instance.LogarCredenciado(login, senha);
                if (vop == null)
                {
                    this.ShowError("Usuario ou Senha inválidos!");
                    return;
                }

                if (string.IsNullOrEmpty(vop.Codigo.Trim()))
                {
                    this.ShowError("Seus dados estão incompletos ou você não é um credenciado, favor entrar em contato com o suporte!");
                    return;
                }

                #endregion

                PRedeAtendimentoVO redeAtendimento = PRedeAtendimentoBO.Instance.GetById(vop.Codigo.Trim());

                UsuarioCredenciadoVO uVO = new UsuarioCredenciadoVO();
				uVO.Credenciado = voh;
                uVO.RedeAtendimento = redeAtendimento;
				uVO.ConfiguracaoIr = ConfiguracaoIrBO.Instance.GetConfiguracao();

				PageHelper.SaveAuthentication(uVO, Session, Response, true);
			}
			catch (Exception ex) {
				Log.Error("Erro ao realizar login!", ex);
				this.ShowError("Erro ao autenticar usuário! " + ex.Message);
				return;
			}
			string nextUrl = Session["NEXT_URL"] as string;
			if (nextUrl == null) {
				nextUrl = "~/Internal/Inicial.aspx";
			}
			Response.Redirect(nextUrl);
			
		}
		
		/*protected void btnEsqueciSenha_Click(object sender, EventArgs e) {
			Response.Redirect("~/EsqueciSenha.aspx");
		}*/

	}
}