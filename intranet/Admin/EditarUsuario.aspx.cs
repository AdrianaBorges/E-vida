using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Admin
{
    public partial class EditarUsuario : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<KeyValuePair<int, string>> lst = AdministracaoBO.Instance.ListarTodosPerfis();

				chkPerfil.DataSource = lst;
				chkPerfil.DataBind();

				List<KeyValuePair<string, string>> lstRegionais = PLocatorDataBO.Instance.ListarRegioes();
				dpdRegional.DataSource = lstRegionais.Select(x => new {
					Key = x.Key,
					Value = x.Value + " (" + x.Key + ")"
				});
				dpdRegional.DataBind();
				dpdRegional.Items.Insert(0, new ListItem("NENHUMA", ""));

				btnPermissao.Visible = false;
				btnAssinatura.Visible = false;
				btnRemover.Visible = false;

				if (!string.IsNullOrEmpty(Request.QueryString["ID"])) {
					string login = Request.QueryString["ID"];
                    Bind(login);
				} else {
					//Response.Redirect("./GerenciarUsuario.aspx");
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_USUARIO; }
		}

		private void Bind(string login) {
			try {
				UsuarioVO vo = UsuarioBO.Instance.GetUsuarioByLogin(login);
				
                if (vo == null) {
					
                    //SclUsuarioVO sclVO = UsuarioBO.Instance.GetUsuarioScl(login);
					//if (sclVO == null) {
					//	this.ShowError("Usuário inexistente!");
					//	return;
					//}
					//vo = new UsuarioVO();
					//vo.Login = sclVO.Login;
					//vo.Nome = sclVO.Nome;

                    vo = new UsuarioVO();
                    string nome_usuario = "";
                    nome_usuario = ConsultaUsuarioAD(login.Trim());

                    if (string.IsNullOrEmpty(nome_usuario.Trim()))
                    {
                        this.ShowError("O usuário " + login.Trim() + " não existe no Active Directory.");
                        btnSalvar.Visible = false;
                        return;
                    }
                    else 
                    {
                        vo.Login = login.Trim();
                        vo.Nome = nome_usuario.Trim();
                    }

				}

				this.lblId.Text = vo.Id != 0 ? vo.Id.ToString() : "";
                this.txtEmail.Text = vo.Email != null ? vo.Email : "";
                this.txtLogin.Text = vo.Login != null ? vo.Login : "";
				this.txtNome.Text = vo.Nome != null ? vo.Nome : "";
                this.txtCargo.Text = vo.Cargo != null ? vo.Cargo : "";
				this.txtMatricula.Text = vo.Matricula != null ? vo.Matricula.ToString() : "";
				this.dpdRegional.SelectedValue = vo.Regional != null ? vo.Regional.ToString() : "";

				List<Perfil> lstPerfil = UsuarioBO.Instance.GetPerfilByUsuario(vo.Id);

				foreach (ListItem item in chkPerfil.Items) {
					Perfil p = (Perfil)Int32.Parse(item.Value);
					item.Selected = (lstPerfil != null && lstPerfil.Contains(p));
				}

				btnAssinatura.Visible = vo.Id != 0;
				trAss.Visible = vo.Id != 0;
				btnRemover.Visible = vo.Id != 0;
				btnPermissao.Visible = vo.Id != 0;

				/*if (vo.Id != 0 && vo.Matricula != null && vo.Matricula.Value != 0) {
					EmpregadoEvidaVO empregado = EmpregadoEvidaBO.Instance.GetByMatricula(vo.Matricula.Value);
					if (empregado != null) {
						lblEmpregado.Text = empregado.Nome;
					} else {
						lblEmpregado.Text = "-";
					}
				}*/
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar dados do usuário! ", ex);
			}
		}

		private void RemoverUsuario() {
			UsuarioVO vo = new UsuarioVO();
			string login = Request.QueryString["ID"];
			vo.Login = login;

			try {
				UsuarioBO.Instance.RemoverUsuario(vo);
			} catch (Exception ex) {
				if (ex.Message.Contains("ORA-02292")) {
					throw new Exception("O usuário possui vínculo interno na INTRANET. Não pode ser removido!", ex);
				} else {
					throw new Exception(ex.Message, ex);
				}
			}
			this.ShowInfo("Usuário removido da INTRANET com sucesso! Usuário ainda existe no Protheus!");
			Bind(vo.Login);
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			UsuarioVO vo = new UsuarioVO();
			try {
				List<ItemError> lstErros = PreencheValida(vo);
				if (lstErros.Count == 0) {
					List<Perfil> lstPerfil = GetSelectedPerfil();
					UsuarioBO.Instance.SalvarUsuario(vo, lstPerfil);
					this.ShowInfo("Usuário salvo com sucesso!");
					this.lblId.Text = vo.Id.ToString();
					Bind(vo.Login);
				} else {
					ShowErrorList(lstErros);
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar o usuário!", ex);
			}
		}

		private List<ItemError> PreencheValida(UsuarioVO vo) {
			List<ItemError> lst = new List<ItemError>();

			string login = Request.QueryString["ID"];
			vo.Login = login;

			vo.Email = txtEmail.Text.Trim();
			if (string.IsNullOrEmpty(vo.Email)) {
				AddErrorMessage(lst, txtEmail, "Informe o e-mail do usuário!");
			} else {
				if (!FormatUtil.IsValidEmail(vo.Email)) {
					AddErrorMessage(lst, txtEmail, "O e-mail do usuário está em um formato inválido!");
				}
			}

			vo.Nome = txtNome.Text.ToUpper();
			if (string.IsNullOrEmpty(vo.Nome)) {
				AddErrorMessage(lst, txtNome, "Informe o nome do usuário!");
			} else {
				if (!ValidateUtil.IsValidName(vo.Nome)) {
					//AddErrorMessage(lst, txtNome, "O nome do usuário está em um formato incorreto!");
				}
			}
			
			vo.Cargo = txtCargo.Text.ToUpper();
			if (string.IsNullOrEmpty(vo.Cargo)) {
				AddErrorMessage(lst, txtCargo, "Informe o cargo/função do usuário");
			}

			if (!string.IsNullOrEmpty(txtMatricula.Text)) {
				long lv = 0;
				vo.Matricula = null;
				if (!Int64.TryParse(txtMatricula.Text, out lv)) {
					AddErrorMessage(lst, txtMatricula, "A matrícula E-VIDA do usuário deve ser numérico (MATRÍCULA NO PROTHEUS)!");
				} else {
					/*EmpregadoEvidaVO empregado = EmpregadoEvidaBO.Instance.GetByMatricula(lv);
					if (empregado == null) {
						AddErrorMessage(lst, txtMatricula, "A matrícula " + lv + " não existe na base do Protheus (ZF1)!");
					}*/
					vo.Matricula = lv;
				}
			}

			vo.Regional = null;
			if (!string.IsNullOrEmpty(dpdRegional.SelectedValue)) {
				vo.Regional = dpdRegional.SelectedValue;
			}

			List<Perfil> lstPerfil = GetSelectedPerfil();
			if (lstPerfil.Count == 0) {
				//AddErrorMessage(lst, chkPerfil, "Informe pelo menos um perfil do usuário!");
			}

			return lst;
		}

		private List<Perfil> GetSelectedPerfil() {
			List<Perfil> lst = new List<Perfil>();
			foreach (ListItem item in chkPerfil.Items) {
				if (item.Selected)
					lst.Add((Perfil)Int32.Parse(item.Value));
			}
			return lst;
		}

		protected void btnAssinatura_Click(object sender, EventArgs e) {
			try {
				string fisico = hidArqFisico.Value;
				string original = hidArqOrigem.Value;				
				string login = Request.QueryString["ID"];
				UsuarioBO.Instance.AlterarAssinatura(login, fisico);
				this.ShowInfo("Assinatura alterada com sucesso!");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao alterar assinatura!", ex);
			}
		}

		protected void btnRemover_Click(object sender, EventArgs e) {
			try {
				RemoverUsuario();
			} catch (Exception ex) {
				this.ShowError("Erro ao remover usuário!", ex);
			}
		}

		protected void txtMatricula_TextChanged(object sender, EventArgs e) {
			try {
				lblEmpregado.Text = string.Empty;
				/*if (!string.IsNullOrEmpty(txtMatricula.Text)) {
					long lv = 0;
					if (!Int64.TryParse(txtMatricula.Text, out lv)) {
						this.ShowError("A matrícula E-VIDA do usuário deve ser numérico (MATRÍCULA NO PROTHEUS)!");
						return;
					} else {
						EmpregadoEvidaVO empregado = EmpregadoEvidaBO.Instance.GetByMatricula(lv);
						if (empregado == null) {
							this.ShowError("A matrícula " + lv + " não existe na base do Protheus (ZF1)!");
							return;
						}

						lblEmpregado.Text = empregado.Nome;
					}
				}*/
			} catch (Exception ex) {
				this.ShowError("Erro ao validar a matrícula!", ex);
			}
		}

        private string ConsultaUsuarioAD(string usuario) {

            var nome = "";
            var acesso = new DirectoryEntry("LDAP://10.0.0.6", "adriana.borges", "Lum#2508");

            var pesquisa = new DirectorySearcher();

            pesquisa.SearchRoot = acesso;
            pesquisa.SearchScope = SearchScope.Subtree;
            pesquisa.Filter = "(ObjectClass=user)";
            pesquisa.Filter = "(&(ObjectClass=user)(samaccountname=" + usuario + "))";

            foreach (SearchResult filtro in pesquisa.FindAll())
            {
                nome = filtro.Properties["cn"][0].ToString();
            }
            return nome;

        }
	}
}