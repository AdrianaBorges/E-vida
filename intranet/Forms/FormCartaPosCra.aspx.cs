using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class FormCartaPosCra : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				lblDataSolicitacao.Text = DateTime.Now.ToShortDateString();
				lblAprovador.Text = "USUÁRIO APROVAÇÃO";
				lblCargoAprovador.Text = string.Empty;

				//if (string.IsNullOrEmpty(UsuarioLogado.Usuario.Cargo)) {
				//	this.ShowError("Seu cargo não está cadastrado no sistema. Entre em contato com o suporte!");
				//	btnSalvar.Visible = false;
				//	return;
				//}
				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id;
					if (Int32.TryParse(Request["ID"], out id)) {
						Bind(id);
					}
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.CARTA_POSITIVA_CRA; }
		}

		public int? Id {
			get { return ViewState["ID"] == null ? new int?() : Convert.ToInt32(ViewState["ID"]); }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int id) {
			CartaPositivaCraVO vo = CartaPositivaCraBO.Instance.GetById(id);
			if (vo == null) {
				this.ShowError("Solicitação inválida!");
				this.Form.Disabled = true;
				return;
			}
			Id = id;
			lblId.Text = id.ToString();

			txtProtocoloCra.Text = vo.ProtocoloCra;
			rblTipo.SelectedValue = ((int)vo.Tipo).ToString();
            BindBenef(vo.Beneficiario.Codint, vo.Beneficiario.Codemp, vo.Beneficiario.Matric, vo.Beneficiario.Tipreg);
			BindCred(vo.Credenciado.Codigo);

			txtContatoCred.Text = vo.Contato;

			UsuarioVO usuarioCriacao = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuarioCriacao);
			UsuarioVO usuarioAlteracao = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuarioAlteracao);
			UsuarioVO usuarioAprovador = vo.IdUsuarioAprovacao != null ? UsuarioBO.Instance.GetUsuarioById(vo.IdUsuarioAprovacao.Value) :  null;

			if (usuarioAprovador != null) {
				lblAprovador.Text = usuarioAprovador.Nome;
				lblCargoAprovador.Text = usuarioAprovador.Cargo;
			} else {
				lblAprovador.Text = "USUÁRIO APROVAÇÃO";
				lblCargoAprovador.Text = string.Empty;
			}

			lblSituacao.Text = CartaPositivaCraEnumTradutor.TraduzStatus(vo.Status);

			lblCriadoPor.Text = vo.DataCriacao.ToShortDateString() + " - " + usuarioCriacao.Login + " - " + usuarioCriacao.Nome;
			lblAlteradoPor.Text = vo.DataAlteracao.ToShortDateString() + " - " + usuarioAlteracao.Login + " - " + usuarioAlteracao.Nome;

			lblDataSolicitacao.Text = vo.DataSolicitacao.ToShortDateString();

			btnPdf.Visible = vo.Status == CartaPositivaCraStatus.APROVADO;
			btnPdf.OnClientClick = "return openCartaPdf(" + vo.Id + ");";

			if (vo.Status != CartaPositivaCraStatus.PENDENTE) {
				btnSalvar.Visible = false;
				this.ShowInfo("Não é possível editar o formulário pois foi finalizado!");
			}
		}

		private void CarregarBeneficiarios(PUsuarioVO titular) {
			List<PUsuarioVO> lstBenefs = PUsuarioBO.Instance.ListarUsuarios(titular.Codint, titular.Codemp, titular.Matric);
			// nova autorizacao, filtrar apenas beneficiarios ativos
			if (Id == null) {
				lstBenefs = new List<PUsuarioVO>(lstBenefs);
                lstBenefs.RemoveAll(x => (x.Datblo != "        " && Int32.Parse(x.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))) && x.Tipusu != "T");
			}
			dpdBeneficiario.DataSource = lstBenefs;
			dpdBeneficiario.DataBind();
			dpdBeneficiario.Items.Insert(0, new ListItem("SELECIONE", ""));
		}

		private void BindBenef(string codint, string codemp, string matric, string tipreg) {
			PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
			if (dpdBeneficiario.Items.Count == 0) {
                PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(benef.Codint, benef.Codemp, benef.Matric);
				CarregarBeneficiarios(titular);
				txtCartaoTitular.Text = titular.Matant;
			}

            string cd_usuario = codint.Trim() + "|" + codemp.Trim() + "|" + matric.Trim() + "|" + tipreg.Trim();

            dpdBeneficiario.SelectedValue = cd_usuario;

			PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(codint, codemp, matric, tipreg);
			PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla);

			lblFiliacao.Text = benef.Mae;
            lblDataNascimento.Text = DateUtil.FormatDateYMDToDMY(benef.Datnas);
			lblPlano.Text = plano.Codigo + " - " + plano.Descri;
			hidPlano.Value = plano.Codigo;

            lblCpf.Text = benef.Cpfusr != null ? FormatUtil.FormatCpf(benef.Cpfusr) : "";
			lblCartao.Text = benef.Matant;

            lblVigencia.Text = DateUtil.FormatDateYMDToDMY(benefPlano.Datcar) + " - " + (String.IsNullOrEmpty(benefPlano.Datblo.Trim()) != true ? DateUtil.FormatDateYMDToDMY(benefPlano.Datblo.Trim()) : "");
		}

		private void BindCred(string idCred) {
			if (idCred != "") {
				PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idCred);
                txtCpfCnpj.Text = vo.Tippe == PConstantes.PESSOA_JURIDICA ? (vo.Cpfcgc != null ? FormatUtil.FormatCnpj(vo.Cpfcgc) : "") : (vo.Cpfcgc != null ? FormatUtil.FormatCpf(vo.Cpfcgc) : "");
				txtRazaoSocial.Text = vo.Nome;
				txtCpfCnpj.Enabled = false;
				txtRazaoSocial.Enabled = false;
				hidCredenciado.Value = idCred.ToString();
			} else {
				txtCpfCnpj.Text = "";
				txtRazaoSocial.Text = "";
				txtCpfCnpj.Enabled = true;
				txtRazaoSocial.Enabled = true;
				hidCredenciado.Value = string.Empty;
			}
		}

		private bool ValidateRequired() {
			if (string.IsNullOrEmpty(rblTipo.SelectedValue)) {
				this.ShowError("Selecione o tipo de carta!");
				return false;
			}
			if (string.IsNullOrEmpty(txtProtocoloCra.Text)) {
				this.ShowError("Informe o protocolo registrado CRA!");
				return false;
			}
			if (string.IsNullOrEmpty(txtCartaoTitular.Text)) {
				this.ShowError("Informe o cartão do titular!");
				return false;
			}
			if (string.IsNullOrEmpty(dpdBeneficiario.SelectedValue)) {
				this.ShowError("Selecione o beneficiário!");
				return false;
			}
			if (string.IsNullOrEmpty(hidCredenciado.Value)) {
				this.ShowError("Informe o credenciado!");
				return false;
			}
			if (string.IsNullOrEmpty(txtContatoCred.Text)) {
				this.ShowError("Informe o contato do credenciado!");
				return false;
			}
			return true;
		}

		private bool ValidateFields() {
			if (!ValidateRequired()) return false;
			return true;
		}

		private void Salvar() {
			if (!ValidateFields())
				return;

			CartaPositivaCraVO vo = new CartaPositivaCraVO();
			if (Id != null) {
				vo.Id = Id.Value;
			}

			vo.ProtocoloCra = txtProtocoloCra.Text;

            string[] dados_beneficiario = dpdBeneficiario.SelectedValue.Split('|');
            string codint = dados_beneficiario[0];
            string codemp = dados_beneficiario[1];
            string matric = dados_beneficiario[2];
            string tipreg = dados_beneficiario[3];

			vo.Beneficiario = new PUsuarioVO() {
				Codint = codint,
                Codemp = codemp,
                Matric = matric,
                Tipreg = tipreg
			};
			vo.CdPlano = hidPlano.Value;
			vo.Contato = txtContatoCred.Text;
			vo.Credenciado = new PRedeAtendimentoVO() {
				Codigo = hidCredenciado.Value
			};
			vo.Tipo = (CartaPositivaCraTipo)Int32.Parse(rblTipo.SelectedValue);

			CartaPositivaCraBO.Instance.SalvarSolicitacao(vo, UsuarioLogado.Usuario);
			this.ShowInfo("Solicitação salva com sucesso!");
			Bind(vo.Id);
		}

		#region Locators

		protected void btnLocCred_Click(object sender, ImageClickEventArgs e) {
			string value = hidCredenciado.Value;
			if (NOT_FOUND_LOCATOR.Equals(value)) {
				hidCredenciado.Value = string.Empty;
				txtCpfCnpj.Enabled = true;
				txtRazaoSocial.Enabled = true;
				this.SetFocus(txtCpfCnpj);
			} else {
				int idCred = 0;
				if (!Int32.TryParse(value, out idCred)) {
					this.ShowError("Credenciado inválido!");
					return;
				}
				BindCred(value);
			}
		}

		protected void btnClrCred_Click(object sender, ImageClickEventArgs e) {
			hidCredenciado.Value = string.Empty;
			BindCred("");
		}

		protected void txtCpfCnpj_TextChanged(object sender, EventArgs e) {
			long cpfCnpj = 0;
			if (string.IsNullOrEmpty(txtCpfCnpj.Text)) {
				btnClrCred_Click(sender, null);
			} else {
				txtCpfCnpj.Text = FormatUtil.UnformatCnpj(txtCpfCnpj.Text);
				if (!Int64.TryParse(txtCpfCnpj.Text, out cpfCnpj)) {
					this.ShowError("O CPF/CNPJ deve ser numérico!");
					return;
				}
				DataTable dt = PRedeAtendimentoBO.Instance.Pesquisar(null, cpfCnpj.ToString(), false);
				if (dt == null || dt.Rows.Count == 0) {
					if (!txtRazaoSocial.Enabled)
						this.ShowError("Credenciado não encontrado! Verifique o cpf/cnpj ou utilize o sistema de busca!");
					return;
				}

				string cdCredenciado = Convert.ToString(dt.Rows[0]["BAU_CODIGO"]);
				BindCred(cdCredenciado);
			}
		}

		#endregion

		protected void txtCartao_TextChanged(object sender, EventArgs e) {
			try {
				dpdBeneficiario.Items.Clear();
				PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartaoTitular.Text);
				if (titular == null) {
					this.ShowError("Titular não encontrado!");
					return;
				}

				CarregarBeneficiarios(titular);
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar matrícula.", ex);
			}
		}
		
		protected void dpdBeneficiario_SelectedIndexChanged(object sender, EventArgs e) {
			try {

                string[] dados_beneficiario = dpdBeneficiario.SelectedValue.Split('|');
                string codint = dados_beneficiario[0];
                string codemp = dados_beneficiario[1];
                string matric = dados_beneficiario[2];
                string tipreg = dados_beneficiario[3];

				BindBenef(codint, codemp, matric, tipreg);
			} catch (Exception ex) {
				this.ShowError("Erro ao selecionar beneficiário!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar!", ex);
			}
		}

	}
}