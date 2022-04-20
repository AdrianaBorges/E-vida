using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaBeneficiarios.Forms {
	public partial class Universitario : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
                txtEmailTitular.Text = UsuarioLogado.UsuarioTitular.Email;
                txtNomeTitular.Text = UsuarioLogado.UsuarioTitular.Nomusr;
                txtCartao.Text = UsuarioLogado.UsuarioTitular.Matant;

				List<PUsuarioVO> lstBeneficiarios = UsuarioLogado.Usuarios;

				if (lstBeneficiarios != null) {
					lstBeneficiarios = lstBeneficiarios.FindAll(x => x.Tipusu != "T"
						&& (PConstantes.GRAU_PARENTESCO_FILHO_FILHA.Equals(Int32.Parse(x.Graupa).ToString())
                         || PConstantes.GRAU_PARENTESCO_ENTEADO_ENTEADA.Equals(Int32.Parse(x.Graupa).ToString())));
				}

				if (lstBeneficiarios == null || lstBeneficiarios.Count == 0) {
					this.ShowInfo("Não existem dependentes que sejam " +
						PLocatorDataBO.Instance.GetParentesco(PConstantes.GRAU_PARENTESCO_FILHO_FILHA) + " ou " +
						PLocatorDataBO.Instance.GetParentesco(PConstantes.GRAU_PARENTESCO_ENTEADO_ENTEADA));

					btnSalvar.Visible = false;					
				}

				dpdDependente.DataSource = lstBeneficiarios;
				dpdDependente.DataBind();

				dpdDependente.Items.Insert(0, new ListItem("SELECIONE O DEPENDENTE",""));

				hidRnd.Value = UsuarioLogado.Matemp.ToString();
			}
		}

		private void Salvar() {
			this.file.Text = fileName.Value;
			if (!FormatUtil.IsValidEmail(txtEmailTitular.Text.Trim())) {
				this.ShowError("Informe o e-mail!");
				this.SetFocus(txtEmailTitular);
				return;
			}

			if (string.IsNullOrEmpty(dpdDependente.SelectedValue)) {
				this.ShowError("Informe o dependente!");
				this.SetFocus(dpdDependente);
				return;
			}
			string arquivo = (!this.fileName.Value.Equals(ofileName.Value)) ? fileName.Value : "";
			if (string.IsNullOrEmpty(arquivo)) {
				this.ShowError("Informe o arquivo da declaração universitária!");
				return;
			}

			DeclaracaoUniversitarioVO vo = new DeclaracaoUniversitarioVO();
			vo.NomeArquivo = arquivo;
			vo.Codint = UsuarioLogado.Codint;
            vo.Codemp = UsuarioLogado.Codemp;
			vo.Matric = UsuarioLogado.Matric;

            string[] dados_beneficiario = dpdDependente.SelectedValue.Split('|');

            vo.Tipreg = dados_beneficiario[3];
			vo.CodPlanoBeneficiario = (string)this.ViewState["PLANO_BENEF"];

			DeclaracaoUniversitarioBO.Instance.Salvar(vo, txtEmailTitular.Text.Trim());

			this.ShowInfo("Sua solicitação foi enviada com sucesso para o setor de cadastro da E-VIDA. Assim que o cadastro for processado você receberá um e-mail confirmando.");

			tbArquivo.Rows[0].Cells[0].Width = "0px";
			this.btnSalvar.Visible = false;
			this.btnCancelar.Visible = false;
		}

		private void SelecionarDependente() {
			string parentesco = string.Empty;
			int idade = 0;
			string plano = string.Empty;
			if (string.IsNullOrEmpty(dpdDependente.SelectedItem.Value)) {

			} else {

                string cd_usuario = dpdDependente.SelectedItem.Value;
                string[] dados_beneficiario = cd_usuario.Split('|');
                string codint = dados_beneficiario[0];
                string codemp = dados_beneficiario[1];
                string matric = dados_beneficiario[2];
                string tipreg = dados_beneficiario[3];

                PUsuarioVO beneficiario = UsuarioLogado.Usuarios.Find(x => x.Cdusuario == cd_usuario);
				parentesco = PLocatorDataBO.Instance.GetParentesco(beneficiario.Graupa);

				PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(codint, codemp, matric, tipreg);
				if (benefPlano != null) {
					this.ViewState["PLANO_BENEF"] = benefPlano.Codpla;
					plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla).Descri;
				}

				if (beneficiario.Datnas != null) {
                    DateTime dataNascimento = DateTime.ParseExact(beneficiario.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture);
                    idade = DateUtil.CalculaIdade(dataNascimento);
				}
			}

			txtParentesco.Text = parentesco;
			txtIdade.Text = idade.ToString();
			txtPlano.Text = plano;
		}
		
		protected void dpdDependente_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				SelecionarDependente();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao selecionar dependente!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar declaracao. ", ex);
			}

		}

		protected void btnCancelar_Click(object sender, EventArgs e) {
			Response.Redirect("./Universitario.aspx");
		}
	}
}