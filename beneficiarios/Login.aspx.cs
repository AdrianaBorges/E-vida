using System;
using System.Collections.Generic;
using System.Threading;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.VO.SCL;

namespace eVidaBeneficiarios {
	public partial class Login : PageBase {
		
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string nextUrl = Session[typeof(Login).ToString()] as string;
				if (nextUrl == null)
					lblLoginExpirado.Visible = false;
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
                #region[BENEFICIÁRIO NO ISA]

                /*eVidaGeneralLib.BO.BeneficiarioBO boh = eVidaGeneralLib.BO.BeneficiarioBO.Instance;
                HcBeneficiarioVO voh = boh.LogarBeneficiario(login, senha);

                if (voh == null)
                {
                    this.ShowError("Usuario ou Senha inválidos!");
                    return;
                }
                if (voh.CdFuncionario == 0)
                {
                    this.ShowError("Seus dados estão incompletos, favor entrar em contato com o suporte!");
                    return;
                }*/

                HcBeneficiarioVO voh = new HcBeneficiarioVO();

                #endregion

                #region[BENEFICIÁRIO NO PROTHEUS]

                /*PUsuarioBO bop = PUsuarioBO.Instance;
                PUsuarioVO vop = bop.LogarBeneficiario(login, senha);

                if (vop == null)
                {
                    this.ShowError("Usuario ou Senha inválidos!");
                    return;
                }

                if (vop.Matemp.Trim() == "")
                {
                    this.ShowError("Seus dados estão incompletos, favor entrar em contato com o suporte!");
                    return;
                }*/

                PUsuarioPortalVO vop = PUsuarioPortalBO.Instance.LogarBeneficiario(login, senha);
                if (vop == null)
                {
                    this.ShowError("Usuario ou Senha inválidos!");
                    return;
                }

                if (string.IsNullOrEmpty(vop.Matric.Trim()))
                {
                    this.ShowError("Seus dados estão incompletos, favor entrar em contato com o suporte!");
                    return;
                }

                #endregion

                // O código do beneficiário é uma concatenação dos códigos do beneficiário logado tanto no ISA quanto no Protheus.
                string codigo_beneficiario = voh.CdBeneficiario + "|" + vop.Codint.Trim() + "|" + vop.Codemp.Trim() + "|" + vop.Matric.Trim() + "|" + vop.Tipreg.Trim();

                UsuarioBeneficiarioVO usuario = GetUsuario(codigo_beneficiario);
                if (usuario == null)
                {
                    this.ShowError("Não foi possível obter algumas informações de seu usuário, favor entrar em contato com o suporte!");
                    return;
                }

                PageHelper.SaveAuthentication(usuario, Session, Response, true);

			}
			catch (Exception ex) {
				this.ShowError("Erro ao autenticar usuário! ", ex);
				return;
			}
			string nextUrl = Session["NEXT_URL"] as string;
			if (nextUrl == null) {
				nextUrl = "~/Forms/DadosPessoais.aspx";
			}
			Response.Redirect(nextUrl);
			
		}

        /*
         protected void btnEsqueciSenha_Click(object sender, EventArgs e) {
            Response.Redirect("~/EsqueciSenha.aspx");
        }*/

    }
}