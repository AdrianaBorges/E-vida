using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Admin {
	public partial class EditarEmpresaReciprocidade : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				
				chkLstArea.DataSource = Constantes.Uf.Values;
				chkLstArea.DataBind();

				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador da requisição inválido!");
					} else {
						EmpresaReciprocidadeVO vo = ReciprocidadeBO.Instance.GetEmpresaById(Int32.Parse(Request["ID"]));
						Bind(vo);
					}
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_EMP_RECIPROCIDADE; }
		}

		private List<string> GetCheckedArea() {
			List<string> lstAreas = new List<string>();
			foreach (ListItem item in chkLstArea.Items) {
				if (item.Selected)
					lstAreas.Add(item.Value);
			}
			return lstAreas;
		}

		private void BindCheckedArea(List<string> lst) {
			foreach (ListItem item in chkLstArea.Items) {
				item.Selected = lst.Contains(item.Value);
			}
		}

		private void Bind(EmpresaReciprocidadeVO vo) {
			ViewState["id"] = vo.Codigo;

			txtCodigo.Text = vo.Codigo.ToString("00");
			txtCodigo.Enabled = false;

			txtNome.Text = vo.Nome;
			BindCheckedArea(vo.AreaAtuacao);

			txtCep.Text = FormatUtil.FormatCep(vo.Endereco.Cep);
			txtEndereco.Text = vo.Endereco.Rua;
			txtNumero.Text = vo.Endereco.NumeroEndereco;
			txtCidade.Text = vo.Endereco.Cidade;
			txtBairro.Text = vo.Endereco.Bairro;
			txtComplemento.Text = vo.Endereco.Complemento;
			txtUf.Text = vo.Endereco.Uf;

			txtEmail.Text = vo.Email[0];
			for (int i = 2; i <= 6; ++i) {
				TextBox txtEmailAlt = (TextBox)conteudo.FindControl("txtEmail" + (i));
				txtEmailAlt.Text = vo.Email.Count >= i ? vo.Email[i-1] : "";
			}

			txtContato.Text = vo.Contato;
			txtAreaContato.Text = vo.AreaContato;
			txtFuncaoContato.Text = vo.FuncaoContato;

			txtFax.Text = "";
			txtFax2.Text = "";
			if (vo.Fax != null && vo.Fax.Count > 0) {
				txtFax.Text = vo.Fax[0];
				txtFax2.Text = vo.Fax.Count > 1 ? vo.Fax[1] : "";
			}

			txtTelefone.Text = "";
			txtTelefone2.Text = "";
			txtTelefone3.Text = "";
			if (vo.Telefone != null && vo.Telefone.Count > 0) {
				txtTelefone.Text = vo.Telefone[0];
				txtTelefone2.Text = vo.Telefone.Count > 1 ? vo.Telefone[1] : "";
				txtTelefone3.Text = vo.Telefone.Count > 2 ? vo.Telefone[2] : "";
			}
			
			txtUrlGuia.Text = vo.UrlGuia;

			this.btnSalvar.Text = "Alterar";
			this.btnNova.Visible = true;
		}

		private bool ValidateRequired() {
			if (string.IsNullOrEmpty(txtCodigo.Text)) {
				this.ShowError("Informe o código!");
				SetFocus(txtCodigo);
				return false;
			}
			if (string.IsNullOrEmpty(txtNome.Text)) {
				this.ShowError("Informe o nome!");
				SetFocus(txtNome);
				return false;
			}
			if (string.IsNullOrEmpty(txtCep.Text)) {
				this.ShowError("Informe o CEP!");
				SetFocus(txtCep);
				return false;
			}
			if (string.IsNullOrEmpty(txtEndereco.Text)) {
				this.ShowError("Informe a rua!");
				SetFocus(txtEndereco);
				return false;
			}
			if (string.IsNullOrEmpty(txtNumero.Text)) {
				this.ShowError("Informe o número do endereço!");
				SetFocus(txtNumero);
				return false;
			}
			if (string.IsNullOrEmpty(txtEmail.Text)) {
				this.ShowError("Informe o e-mail principal!");
				SetFocus(txtEmail);
				return false;
			}
			if (string.IsNullOrEmpty(txtTelefone.Text)) {
				this.ShowError("Informe o telefone principal para contato!");
				SetFocus(txtTelefone);
				return false;
			}
			if (string.IsNullOrEmpty(txtUrlGuia.Text)) {
				this.ShowError("Informe a URL do Guia Médico!");
				SetFocus(txtUrlGuia);
				return false;
			}

			List<string> lstAreas = GetCheckedArea();
			if (lstAreas.Count == 0) {
				this.ShowError("Informe as áreas de atuação!");
				SetFocus(chkLstArea);
				return false;
			}

			return true;
		}

		private bool ValidateFields() {
			if (!ValidateRequired())
				return false;

			if (!FormatUtil.IsValidEmail(txtEmail.Text.Trim())) {
				this.ShowError("Informe um e-mail principal válido!");
				SetFocus(txtEmail);
				return false;
			}
			for (int i = 2; i <= 6; ++i) {
				TextBox txtEmailAlt = (TextBox)conteudo.FindControl("txtEmail" + i);
				if (!string.IsNullOrEmpty(txtEmailAlt.Text) && !FormatUtil.IsValidEmail(txtEmailAlt.Text.Trim())) {
					this.ShowError("O e-mail " + i + " informado não é váldio!");
					SetFocus(txtEmailAlt);
					return false;
				}
			}
			return true;
		}

		private void Salvar() {
			if (!ValidateFields())
				return;

			int id = Int32.Parse(txtCodigo.Text);
			bool newId = false;
			if (ViewState["id"] == null) {
				newId = true;
				// Novo registro
				EmpresaReciprocidadeVO oldVO = ReciprocidadeBO.Instance.GetEmpresaById(id);
				if (oldVO != null) {
					this.ShowError("Já existe uma empresa cadastrada com este código!");
					return;
				}
			}

			EmpresaReciprocidadeVO vo = new EmpresaReciprocidadeVO();
			vo.Codigo = id;
			vo.Nome = txtNome.Text.ToUpper();
			vo.AreaAtuacao = GetCheckedArea();
			vo.Endereco = new EnderecoVO()
			{
				Cep = Int32.Parse(FormatUtil.UnformatCep(txtCep.Text)),
				Rua = txtEndereco.Text,
				NumeroEndereco = txtNumero.Text,
				Cidade = txtCidade.Text,
				Bairro = txtBairro.Text,
				Complemento = txtComplemento.Text,
				Uf = txtUf.Text
			};
			vo.Email = new List<string>();
			vo.Email.Add(txtEmail.Text.Trim());
			for (int i = 2; i <= 6; i++) {
				TextBox txtEmailAlt = (TextBox)conteudo.FindControl("txtEmail" + i);
				if (!string.IsNullOrEmpty(txtEmailAlt.Text)) {
					vo.Email.Add(txtEmailAlt.Text.Replace("|", ""));
				}
			}

			vo.Contato = txtContato.Text;
			vo.AreaContato = txtAreaContato.Text;
			vo.FuncaoContato = txtFuncaoContato.Text;
			vo.Fax = new List<string>();
			if (!string.IsNullOrEmpty(txtFax.Text))
				vo.Fax.Add(txtFax.Text.Replace("|", ""));
			if (!string.IsNullOrEmpty(txtFax2.Text))
				vo.Fax.Add(txtFax2.Text.Replace("|", ""));

			vo.Telefone = new List<string>();
			vo.Telefone.Add(txtTelefone.Text);
			if (!string.IsNullOrEmpty(txtTelefone2.Text))
				vo.Telefone.Add(txtTelefone2.Text.Replace("|", ""));
			if (!string.IsNullOrEmpty(txtTelefone3.Text))
				vo.Telefone.Add(txtTelefone3.Text.Replace("|", ""));

			vo.UrlGuia = txtUrlGuia.Text;

			ReciprocidadeBO.Instance.SalvarEmpresa(vo, newId);
			this.ShowInfo("Empresa salva com sucesso!");
			try {
				vo = ReciprocidadeBO.Instance.GetEmpresaById(id);
				Bind(vo);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar dados da empresa! " + ex.Message);
				Log.Error("Erro ao carregar dados da empresa!", ex);
			}
		}

		protected void txtCep_TextChanged(object sender, EventArgs e) {
			try {
				string strCep = txtCep.Text;

				if (Log.IsDebugEnabled)
					Log.Debug("BuscarCep: " + strCep);

				int nuCep;
				strCep = FormatUtil.UnformatCep(strCep);
				CepVO cepVO = new CepVO();
				if (!string.IsNullOrEmpty(strCep) && Int32.TryParse(strCep, out nuCep)) {
					cepVO = LocatorDataBO.Instance.BuscarCep(nuCep);
					if (cepVO == null) {
						this.ShowError("CEP não encontrado!");
						cepVO = new CepVO();
					}
				}

				txtBairro.Text = cepVO.Bairro;
				txtCidade.Text = cepVO.Cidade;
				txtEndereco.Text = cepVO.Rua;
				txtUf.Text = cepVO.Uf;

				txtEndereco.Enabled = true;

				txtNumero.Focus();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar o CEP!");
				Log.Error("Erro ao buscar o CEP!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar empresa! " + ex.Message);
				Log.Error("Erro ao salvar empresa!", ex);
			}
		}

	}
}