using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaBeneficiarios.Forms {
	public partial class Reciprocidade : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
                PUsuarioVO titular = UsuarioLogado.UsuarioTitular;

				dpdUf.DataSource = PConstantes.Uf.Values;
				dpdUf.DataBind();
				dpdUf.Items.Insert(0, new ListItem("-", ""));

				txtEmailTitular.Text = titular.Email;
				txtCartao.Text = titular.GetCarteira();
				txtNomeTitular.Text = titular.Nomusr;

				btnIncluirDep.Visible = btnSalvar.Visible = false;

				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id = 0;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador de requisição inexistente!");
						return;						
					}

					ReciprocidadeVO vo = ReciprocidadeBO.Instance.GetById(id);
					if (vo == null || !CheckBeneficiario(vo.Codint, vo.Codemp, vo.Matric)) {
						this.ShowError("Identificador de requisição inválido!");
						return;						
					}
					Bind(vo);

				} else {
					ltData.Text = FormatUtil.FormatDataExtenso(DateTime.Now);
					CarregarBeneficiarios();
					btnIncluirDep.Visible = btnSalvar.Visible = true;
				}
			}

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

			btnNova.Visible = true;
			btnIncluirDep.Visible = false;
		}

		private void CarregarBeneficiarios() {
			DataTable dtBenef = PUsuarioBO.Instance.BuscarUsuarios(UsuarioLogado.Codint, UsuarioLogado.Codemp, UsuarioLogado.Matric);
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
			if (string.IsNullOrEmpty(txtEmailTitular.Text)) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe o e-mail!");
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
			if (!FormatUtil.IsValidEmail(txtEmailTitular.Text.Trim())) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe um e-mail válido!");
			} else if (!ValidateUtil.IsValidDomain(txtEmailTitular.Text.Trim())) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe o domínio do e-mail informado é inválido!");
			}

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
			vo.Codint = UsuarioLogado.Codint;
            vo.Codemp = UsuarioLogado.Codemp;
			vo.Matric = UsuarioLogado.Matric;

			vo.Endereco = new EnderecoVO();
			vo.Endereco.Cidade = dpdMunicipio.SelectedValue;
			vo.Endereco.Uf = dpdUf.SelectedValue;

			vo.Fim = DateTime.Parse(txtFim.Text);
			vo.Inicio = DateTime.Parse(txtInicio.Text);

			vo.Local = txtLocal.Text;
			vo.Status = StatusReciprocidade.PENDENTE;

			List<ReciprocidadeBenefVO> lst = GridToList();

			DataTable dtConcorrencia = ReciprocidadeBO.Instance.ChecarConcorrencia(vo.Codint, vo.Codemp, vo.Matric, lst, vo.Inicio, vo.Fim, vo.Endereco.Cidade, vo.Endereco.Uf);
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

			ReciprocidadeBO.Instance.Salvar(vo, lst, txtEmailTitular.Text.Trim());

			this.ShowInfo("Formulário salvo com sucesso!");

			this.btnSalvar.Visible = false;
			this.btnNova.Visible = true;

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

				if (!string.IsNullOrEmpty(Convert.ToString(vo["BA1_CPFUSR"]))) {
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
				this.ShowError("Erro ao salvar formulário! ", ex);
			}
		}

		protected void btnIncluirDep_Click(object sender, EventArgs e) {
			try {
				CarregarBeneficiarios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar beneficiários! ", ex);
			}
		}
	}
}