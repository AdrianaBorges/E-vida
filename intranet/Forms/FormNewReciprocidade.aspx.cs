using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms {
	public partial class FormNewReciprocidade : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdUf.DataSource = PConstantes.Uf.Values;
				dpdUf.DataBind();
				dpdUf.Items.Insert(0, new ListItem("-", ""));
				
				btnIncluirDep.Visible = btnSalvar.Visible = false;

				ltData.Text = FormatUtil.FormatDataExtenso(DateTime.Now);
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RECIPROCIDADE_GESTAO; }
		}

		private void Bind(PUsuarioVO vo) {
			txtEmailTitular.Text = vo.Email;
			txtNomeTitular.Text = vo.Nomusr;

			CarregarBeneficiarios(vo);

			btnSalvar.Visible = true;
		}

		private void Bind(ReciprocidadeVO vo) {
			litProtocolo.Value = vo.CodSolicitacao.ToString();

			dpdUf.SelectedValue = vo.Endereco.Uf;
			PopularMunicipios();
			dpdMunicipio.SelectedValue = vo.Endereco.Cidade;
			dpdUf.Enabled = false;

			txtFim.Text = vo.Fim.ToString("dd/MM/yyyy");
			txtInicio.Text = vo.Inicio.ToString("dd/MM/yyyy");
			txtLocal.Text = vo.Local;

			ltData.Text = FormatUtil.FormatDataExtenso(vo.DataCriacao);

			DataTable dtBenef = ReciprocidadeBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
			gdvDependentes.DataSource = dtBenef;
			gdvDependentes.DataBind();

			btnIncluirDep.Visible = false;
			txtCartao.Enabled = false;
			txtLocal.Enabled = false;

			btnEnviar.Visible = true;
		}

		private void CarregarBeneficiarios(PUsuarioVO titular) {
            DataTable dtBenef = PUsuarioBO.Instance.BuscarUsuarios(titular.Codint, titular.Codemp, titular.Matric);
			gdvDependentes.DataSource = dtBenef;
			gdvDependentes.DataBind();
		}

		private void PopularMunicipios() {
			DataTable dt = PLocatorDataBO.Instance.BuscarMunicipios(dpdUf.SelectedValue);
			dpdMunicipio.DataSource = dt;
			dpdMunicipio.DataBind();
			dpdMunicipio.Items.Insert(0, new ListItem("SELECIONE", ""));
		}

		private bool ValidateRequired() {
			List<ItemError> lst = new List<ItemError>();
			if (string.IsNullOrEmpty(txtCartao.Text)) {
				this.AddErrorMessage(lst, txtCartao, "Informe o cartão do titular!");
			}
			if (string.IsNullOrEmpty(txtInicio.Text)) {
				this.AddErrorMessage(lst, txtInicio, "Informe a data de início do período!");
			}
			if (string.IsNullOrEmpty(txtFim.Text)) {
				this.AddErrorMessage(lst, txtFim, "Informe a data de fim do período!");
			}
			if (string.IsNullOrEmpty(dpdUf.SelectedValue)) {
				this.AddErrorMessage(lst, dpdUf, "Informe o estado para atendimento!");
			}
			if (string.IsNullOrEmpty(dpdMunicipio.SelectedValue)) {
				this.AddErrorMessage(lst, dpdMunicipio, "Informe o município para atendimento!");
			}
			if (string.IsNullOrEmpty(txtLocal.Text)) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe o local!");
			}
			List<ReciprocidadeBenefVO> lstBenef = GridToList();
			if (lstBenef.Count == 0) {
				this.AddErrorMessage(lst, gdvDependentes, "Informe pelo menos um beneficiário!");
			}

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}

			return true;
		}

		private bool ValidateFields() {
			if (!ValidateRequired())
				return false;

			List<ItemError> lst = new List<ItemError>();

			DateTime dtInicio = DateTime.MinValue;
			DateTime dtFim = DateTime.MinValue;

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.AddErrorMessage(lst, txtInicio, "Informe uma data inicial válida!");
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.AddErrorMessage(lst, txtFim, "Informe uma data final válida!");
			} else {
				if (dtInicio > dtFim) {
					this.AddErrorMessage(lst, txtFim, "A data de início deve ser menor que a data fim!");
				}
			}
			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}
			return true;
		}

		private List<ReciprocidadeBenefVO> GridToList() {
			List<ReciprocidadeBenefVO> lst = new List<ReciprocidadeBenefVO>();
			foreach (GridViewRow row in gdvDependentes.Rows) {
				if (row.Visible) {

                    string cd_beneficiario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["cd_beneficiario"]);
                    string[] dados_beneficiario = cd_beneficiario.Split('|');
                    string codint = dados_beneficiario[0];
                    string codemp = dados_beneficiario[1];
                    string matric = dados_beneficiario[2];
                    string tipreg = dados_beneficiario[3];

					ReciprocidadeBenefVO benef = new ReciprocidadeBenefVO();
					benef.Codint = codint;
                    benef.Codemp = codemp;
                    benef.Matric = matric;
                    benef.Tipreg = tipreg;
					benef.IsTitular = row.Cells[3].Text.Equals("TITULAR");
					benef.TipoPlano = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["BI3_TIPO"]);
                    benef.InicioPlano = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["BA1_DATCAR"]);
					lst.Add(benef);
				}
			}
			return lst;
		}

		private void Salvar() {
			if (!ValidateFields()) {
				return;
			}

			ReciprocidadeVO vo = new ReciprocidadeVO();
            PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
			if (titular == null) {
				this.ShowError("Titular não encontrado pelo cartão!");
				return;
			}
			vo.Codint = titular.Codint;
            vo.Codemp = titular.Codemp;
			vo.Matric = titular.Matric;

			vo.Endereco = new EnderecoVO();
			vo.Endereco.Cidade = dpdMunicipio.SelectedValue;
			vo.Endereco.Uf = dpdUf.SelectedValue;

			vo.Fim = DateTime.Parse(txtFim.Text);
			vo.Inicio = DateTime.Parse(txtInicio.Text);

			vo.Local = txtLocal.Text;
			vo.Status = StatusReciprocidade.PENDENTE;

			List<ReciprocidadeBenefVO> lst = GridToList();

			DataTable dtConcorrencia = ReciprocidadeBO.Instance.ChecarConcorrencia(
				vo.Codint, vo.Codemp, vo.Matric, lst, vo.Inicio, vo.Fim, vo.Endereco.Cidade, vo.Endereco.Uf);
			if (dtConcorrencia.Rows.Count > 0) {
				DataRow dr = dtConcorrencia.Rows[0];
				this.ShowError("O beneficiário " + dr["BA1_NOMUSR"] + " já possui " +
					" solicitação de reciprocidade de PROTOCOLO " +
					Convert.ToInt32(dr["CD_SOLICITACAO"]).ToString(ReciprocidadeVO.FORMATO_PROTOCOLO) +
					" em processamento para o período " +
					Convert.ToDateTime(dr["dt_inicio"]).ToShortDateString() + " a " +
					Convert.ToDateTime(dr["dt_fim"]).ToShortDateString() + " para a localidade " +
					vo.Endereco.Cidade + " - " + vo.Endereco.Uf);
				return;
			}

			ReciprocidadeBO.Instance.Salvar(vo, lst, txtEmailTitular.Text);

			this.ShowInfo("Formulário salvo com sucesso!");

			this.btnSalvar.Visible = false;

			try {
				vo = ReciprocidadeBO.Instance.GetById(vo.CodSolicitacao);
				Bind(vo);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao recarregar formulário!", ex);
			}
		}

		protected void gdvDependentes_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName.Equals("Excluir")) {
				int rowIndex = Convert.ToInt32(e.CommandArgument);
				gdvDependentes.Rows[rowIndex].Visible = false;

				int i = 0;
				foreach (GridViewRow row in gdvDependentes.Rows) {
					if (row.Visible) {
						i++;
						Label lblRowNum = (Label)row.FindControl("lblRowNum");
						lblRowNum.Text = i.ToString();

					}
				}
			}
		}

		protected void txtCartao_TextChanged(object sender, EventArgs e) {
			try {
				if (!string.IsNullOrEmpty(txtCartao.Text)) {
					PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
					if (titular == null) {
						this.ShowError("Beneficiário não encontrado!");
						return;
					}
					if (!titular.Tipusu.Equals(PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO)) {
						this.ShowError("O beneficiário não é o titular!");
						return;
					}
					Bind(titular);
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar o titular! ", ex);
			}
		}
		
		protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				PopularMunicipios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar municípios! ", ex);
			}
		}

		protected void gdvDependentes_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)row.DataItem;

				Label lblRowNum = (Label)row.FindControl("lblRowNum");

				if (Convert.ToString(vo["BA1_TIPUSU"]) == "D") {
					row.Cells[3].Text = "DEPENDENTE";
				} else {
					row.Cells[2].Text = "-";
					row.Cells[3].Text = "TITULAR";
				}

                row.Cells[4].Text = DateUtil.FormatDateYMDToDMY(Convert.ToString(vo["BA1_DATNAS"]));

				if (!string.IsNullOrEmpty(Convert.ToString(vo["BA1_CPFUSR"]).Trim())) {
                    row.Cells[5].Text = vo["BA1_CPFUSR"] != DBNull.Value ? FormatUtil.FormatCpf(Convert.ToString(vo["BA1_CPFUSR"])) : "";
				}

				lblRowNum.Text = (row.DataItemIndex + 1).ToString();

				if (!string.IsNullOrEmpty(litProtocolo.Value)) {
					ImageButton btnRemover = row.FindControl("lnkRemoverDependente") as ImageButton;
					btnRemover.Visible = false;
				}
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar formulário! " + ex.Message);
				Log.Error("Erro ao salvar formulário.", ex);
			}
		}

		protected void btnIncluirDep_Click(object sender, EventArgs e) {
			try {
				PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
				if (titular == null) {
					this.ShowError("Titular não encontrado pelo cartão!");
					return;
				}				
				CarregarBeneficiarios(titular);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar beneficiários!", ex);
			}
		}

	}
}