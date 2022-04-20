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
	public partial class CriarUsuarioScl : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_USUARIO_SCL; }
		}

		private bool ValidateRequiredFields() {
			List<ItemError> lstError = new List<ItemError>();
			if (string.IsNullOrEmpty(txtLogin.Text)) {
				this.AddErrorMessage(lstError, txtLogin, "Informe o login!");
			}
			if (string.IsNullOrEmpty(txtNome.Text)) {
				this.AddErrorMessage(lstError, txtNome, "Informe o nome!");
			}
			if (string.IsNullOrEmpty(dpdPerfil.SelectedValue)) {
				this.AddErrorMessage(lstError, txtSenha, "Informe o perfil!");
			}
			if (string.IsNullOrEmpty(txtDominio.Text)) {
				this.AddErrorMessage(lstError, txtDominio, "Informe o valor de domínio!");
			}
			if (string.IsNullOrEmpty(txtSenha.Text)) {
				this.AddErrorMessage(lstError, txtSenha, "Informe a senha!");
			}
			if (string.IsNullOrEmpty(txtConfSenha.Text)) {
				this.AddErrorMessage(lstError, txtConfSenha, "Informe a confirmação de senha!");
			}

			if (lstError.Count > 0) {
				this.ShowErrorList(lstError);
				return false;
			}
			return true;
		}

		private bool ValidateFields() {
			if (!ValidateRequiredFields())
				return false;

			string senha = txtSenha.Text;
			string confSenha = txtConfSenha.Text;

			if (!senha.Equals(confSenha, StringComparison.InvariantCultureIgnoreCase)) {
				this.ShowError("As senhas informadas não conferem!");
				return false;
			}
			return true;
		}

		private void Salvar() {

			if (!ValidateFields()) {
				return;
			}

			string login = txtLogin.Text;
			string vlDominio = txtDominio.Text;

			SclUsuarioVO oldVO = UsuarioBO.Instance.GetUsuarioScl(login);
			if (oldVO != null) {
				this.ShowError("Este login já está em uso!");
				return;
			}

			SclUsuarioDominio dominio = SclUsuarioDominio.GAL_NRCARTEIRA_BENEF;
			if (dpdPerfil.SelectedValue.Equals("1")) {
				dominio = SclUsuarioDominio.GAL_NRCARTEIRA_BENEF;
			} else {
				dominio = SclUsuarioDominio.GAL_CNPJ_PRESTADOR;
			}

			oldVO = UsuarioBO.Instance.GetUsuarioSclByDominio(dominio, vlDominio);
			if (oldVO != null) {
				this.ShowError("Este domínio já está sendo utilizado pelo usuário " + oldVO.Login);
				return;
			}

			SclUsuarioVO vo = new SclUsuarioVO();
			vo.Login = login;
			vo.Nome = txtNome.Text;
			vo.Ativo = true;
			vo.DataCadastro = DateTime.Now;
			vo.DataExpiracaoSenha = null;
			vo.Dominios = new List<SclUsuarioDominioVO>();
			vo.Dominios.Add(new SclUsuarioDominioVO() {
				IdDominio = dominio,
				CdAutorizacaoI = vlDominio
			});
			vo.Perfis = new List<SclPerfilUsuarioVO>();
			vo.Perfis.Add(new SclPerfilUsuarioVO() {
				CdSistema = "GAL",
				CdGrupo = Int32.Parse(dpdPerfil.SelectedValue)
			});

			UsuarioBO.Instance.CriarUsuarioScl(vo, txtSenha.Text, UsuarioLogado.Usuario);

			btnSalvar.Enabled = false;
			this.ShowInfo("Usuario criado com sucesso!");
			this.RegisterScript("goToEdit", "currLogin ='" + vo.Login.ToUpper() + "';");
		}

		protected void dpdPerfil_SelectedIndexChanged(object sender, EventArgs e) {
			lblDominio.Text = "-";
			string strPerfil = dpdPerfil.SelectedValue;

			if (!string.IsNullOrEmpty(strPerfil)) {
				string strDominio = "-";
				switch (strPerfil) {
					case "1": strDominio = SclUsuarioDominio.GAL_NRCARTEIRA_BENEF.ToString(); break;
					case "2": 
					case "4":
						strDominio = SclUsuarioDominio.GAL_CNPJ_PRESTADOR.ToString(); break;
					default:
						break;
				}
				lblDominio.Text = strDominio;
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao criar novo usuário!", ex);
			}
		}
	}
}