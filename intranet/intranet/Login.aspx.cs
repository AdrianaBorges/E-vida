using System;
using System.Collections.Generic;
using System.Threading;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaIntranet {
	public partial class Login : PageBase {

		protected override Modulo Modulo {
			get { return Modulo.LOGIN; }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string nextUrl = (string)Session[typeof(eVidaIntranet.Login).ToString()];
				if (nextUrl == null)
					lblLoginExpirado.Visible = false;
				Session.Clear();
				ViewState["NEXT_URL"] = nextUrl;
			}
		}

		protected void btnLogin_Click(object sender, EventArgs e) {

			string login = txtLogin.Text;
			string senha = txtSenha.Text;

			if (string.IsNullOrEmpty(login)) {
				this.ShowError("Usuário é obrigatório!");
				return;
			}

			if (string.IsNullOrEmpty(senha)) {
				this.ShowError("Senha é obrigatório!");
				return;
			}

			//Thread.Sleep(1000);

			try {
				UsuarioBO bo = UsuarioBO.Instance;
                UsuarioVO vo = bo.LogarFuncionario(login, senha);

				if (vo == null) {
					this.ShowError("Usuario ou Senha inválidos!");
					return;
				}
				if (vo.Id == 0) {
					this.ShowError("Você não possui permissão de acesso ao sistema!");
					return;
				}

				List<Modulo> lst = bo.ListarModulosUsuario(vo.Id);
				if (lst == null || lst.Count == 0) {
					this.ShowError("Você não possui módulos associados no sistema!");
					return;
				}

				UsuarioIntranetVO usuario = new UsuarioIntranetVO();
				usuario.Usuario = vo;
				usuario.Permissoes = lst;
				usuario.UsuarioScl = UsuarioBO.Instance.GetUsuarioScl(vo.Login);

				PageHelper.SaveAuthentication(usuario, Session, Response, true);
			}
			catch (Exception ex) {

                // A exceção está sendo exibida em inglês. É preciso mostrá-la em pt-BR.
				//this.ShowError("Erro ao autenticar usuário! ", ex);
                string msg = ex.ToString();
                msg = "";

                this.ShowError("Erro ao autenticar usuário! - Nome de usuário ou senha incorretos." + msg);
				return;
			}
			string nextUrl = (string)ViewState["NEXT_URL"];
			if (string.IsNullOrEmpty(nextUrl))
				Response.Redirect("~/Internal/Inicial.aspx");
			else
				Response.Redirect(nextUrl);
			
		}
		
		protected void btnEsqueciSenha_Click(object sender, EventArgs e) {
			Response.Redirect("~/EsqueciSenha.aspx");
		}
		
	}
}