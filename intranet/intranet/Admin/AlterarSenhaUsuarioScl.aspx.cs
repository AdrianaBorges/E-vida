using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.SCL;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Admin {
	public partial class AlterarSenhaUsuarioScl : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string login = Request["login"];
				if (string.IsNullOrEmpty(login)) {
					this.ShowError("Para alteração de senha deve ser selecionado um usuário válido!");
					btnSalvar.Enabled = false;
					return;
				}

				SclUsuarioVO vo = UsuarioBO.Instance.GetUsuarioScl(login);
				if (vo == null) {
					this.ShowError("Usuário não encontrado!");
					btnSalvar.Enabled = false;
					return;
				}

				Bind(vo);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_USUARIO_SCL; }
		}

		private string Login {
			get { return (string)ViewState["LOGIN"]; }
			set { ViewState["LOGIN"] = value; }
		}

		private void Bind(SclUsuarioVO vo) {
			Login = vo.Login;
			lblLogin.Text = vo.Login;
			lblNome.Text = vo.Nome;
			chkAtivo.Checked = vo.Ativo;
			lblDataCadastro.Text = vo.DataCadastro.ToString("dd/MM/yyyy");
			txtDataExpiracao.Text = vo.DataExpiracaoSenha != null ? vo.DataExpiracaoSenha.Value.ToString("dd/MM/yyyy") : "";

			lblUserUpdate.Text = vo.UserUpdate;
			lblDateUpdate.Text = vo.DateUpdate;

			gdvDominio.DataSource = vo.Dominios;
			gdvDominio.DataBind();

			gdvPerfil.DataSource = vo.Perfis;
			gdvPerfil.DataBind();
		}

		private void Alterar() {
			string senha = txtSenha.Text;
			string confSenha = txtConfSenha.Text;
			bool ativo = chkAtivo.Checked;
			DateTime? dtExpiracao = null;

			if (!string.IsNullOrEmpty(senha)) {
				if (string.IsNullOrEmpty(confSenha)) {
					this.ShowError("Informe a confirmação da senha!");
					return;
				}
				if (!senha.Equals(confSenha, StringComparison.InvariantCultureIgnoreCase)) {
					this.ShowError("As senhas informadas não conferem!");
					return;
				}
			}

			if (!string.IsNullOrEmpty(txtDataExpiracao.Text)) {
				DateTime dt;
				if (!DateTime.TryParse(txtDataExpiracao.Text, out dt)) {
					this.ShowError("A Data de Expiração é inválida!");
					return;
				}
				dtExpiracao = dt;
			}


			UsuarioBO.Instance.AlterarUsuarioScl(Login, senha, dtExpiracao, ativo, UsuarioLogado.Usuario);

			this.ShowInfo("Dados alterados com sucesso!");
			txtSenha.Text = "";
			txtConfSenha.Text = "";

			SclUsuarioVO vo = UsuarioBO.Instance.GetUsuarioScl(Login);
			Bind(vo);
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Alterar();
			} catch (Exception ex) {
				this.ShowError("Erro ao alterar dados do usuário!", ex);
			}
		}
	}
}