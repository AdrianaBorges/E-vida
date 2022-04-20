using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms
{
    public partial class FormExclusao : FormPageBase {

		[Serializable]
		private class ExclusaoBenefTelaVO : ExclusaoBenefVO {
			public string Nome { get; set; }
			public string Parentesco { get; set; }
			public string Plano { get; set; }
			public string Nascimento { get; set; }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				btnSalvar.Visible = true;
                if (!string.IsNullOrEmpty(Request["ID"]))
                {
                    int id = 0;
                    if (!Int32.TryParse(Request["ID"], out id))
                    {
                        this.ShowError("Identificador de requisição inexistente!");
                        return;
                    }

                    ExclusaoVO vo = FormExclusaoBO.Instance.GetById(id);
                    if (vo == null || vo.Status == StatusExclusao.APROVADO || vo.Status == StatusExclusao.NEGADO)
                    {
                        this.ShowError("Identificador de requisição inválido ou finalizado!");
                        return;
                    }
                    else
                    {
                        Bind(vo);
                    }

                }
                else 
                {
                    ltData.Text = FormatUtil.FormatDataHoraExtenso(DateTime.Now);
                }
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.FORM_EXCLUSAO_CREATE; }
		}

		private List<ExclusaoBenefTelaVO> Beneficiarios {
			get {
				return ViewState["BENEFICIARIOS"] as List<ExclusaoBenefTelaVO>;
			}
			set {
				ViewState["BENEFICIARIOS"] = value;
			}
		}

		private List<ExclusaoBenefTelaVO> CarregarTodosBeneficiarios(string codint, string codemp, string matric) {
			List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
            DataTable dtBenef = PUsuarioBO.Instance.BuscarUsuarios(codint, codemp, matric);
			foreach (DataRow dr in dtBenef.Rows) {
                string tipreg = Convert.ToString(dr["BA1_TIPREG"]);
				string nmBeneficiario = Convert.ToString(dr["BA1_NOMUSR"]);
				string codPlano = Convert.ToString(dr["BI3_CODIGO"]);

				lst.Add(new ExclusaoBenefTelaVO() {
                    Codint = Convert.ToString(dr["BA1_CODINT"]),
                    Codemp = Convert.ToString(dr["BA1_CODEMP"]),
                    Matric = Convert.ToString(dr["BA1_MATRIC"]),
                    Tipreg = Convert.ToString(dr["BA1_TIPREG"]),
                    Cdusuario = Convert.ToString(dr["BA1_CODINT"]).Trim() + "|" + Convert.ToString(dr["BA1_CODEMP"]).Trim() + "|" + Convert.ToString(dr["BA1_MATRIC"]).Trim() + "|" + Convert.ToString(dr["BA1_TIPREG"]).Trim(),
                    CodPlano = Convert.ToString(dr["BI3_CODIGO"]),
					IsDepFamilia = false,
                    IsTitular = Convert.ToString(dr["BA1_TIPUSU"]).Trim() == "T",
                    Nome = Convert.ToString(dr["BA1_NOMUSR"]),
                    Plano = Convert.ToString(dr["BI3_DESCRI"]),
                    Parentesco = Convert.ToString(dr["BRP_DESCRI"]),
                    Nascimento = Convert.ToString(dr["BA1_DATNAS"])
				});
			}

			dtBenef = ResponsavelBO.Instance.BuscarDependentes(codint, codemp, matric, TipoResponsavel.FAMILIA);
			foreach (DataRow dr in dtBenef.Rows) {
                string tipreg = Convert.ToString(dr["BA1_TIPREG"]);
                string nmBeneficiario = Convert.ToString(dr["BA1_NOMUSR"]);
                string codPlano = Convert.ToString(dr["BI3_CODIGO"]);

                ExclusaoBenefVO vo = lst.FirstOrDefault(x =>
                    x.Codint == codint && x.Codemp == codemp && x.Matric == matric && x.Tipreg == tipreg);

				if (vo == null) {
					lst.Add(new ExclusaoBenefTelaVO() {
                        Codint = Convert.ToString(dr["BA1_CODINT"]),
                        Codemp = Convert.ToString(dr["BA1_CODEMP"]),
                        Matric = Convert.ToString(dr["BA1_MATRIC"]),
                        Tipreg = Convert.ToString(dr["BA1_TIPREG"]),
                        Cdusuario = Convert.ToString(dr["BA1_CODINT"]).Trim() + "|" + Convert.ToString(dr["BA1_CODEMP"]).Trim() + "|" + Convert.ToString(dr["BA1_MATRIC"]).Trim() + "|" + Convert.ToString(dr["BA1_TIPREG"]).Trim(),
                        CodPlano = Convert.ToString(dr["BI3_CODIGO"]),
						IsDepFamilia = true,
						IsTitular = false,
                        Nome = Convert.ToString(dr["BA1_NOMUSR"]),
                        Plano = Convert.ToString(dr["BI3_DESCRI"]),
                        Parentesco = Convert.ToString(dr["BRP_DESCRI"]),
                        Nascimento = Convert.ToString(dr["BA1_DATNAS"])
					});
				}
			}

			return lst;
		}

		private void BindTitular() {
			PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
			if (titular == null) {
				this.ShowError("Titular não encontrado!");
				return;
			}
            Beneficiarios = CarregarTodosBeneficiarios(titular.Codint, titular.Codemp, titular.Matric);

			txtEmailTitular.Text = titular.Email;
			txtCartao.Text = titular.Matant;
			txtNomeTitular.Text = titular.Nomusr;

			gdvDependentes.DataSource = Beneficiarios;
			gdvDependentes.DataBind();
			gdvDependentes.Visible = true;

			ltData.Text = FormatUtil.FormatDataExtenso(DateTime.Now);
			
			LimparDependentes();
			btnLimparDep.Visible = true;
		}

		private void Bind(ExclusaoVO vo) {
            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
			txtCartao.Text = titular.Matant;
			BindTitular();

			litProtocolo.Value = vo.CodSolicitacao.ToString();

			txtLocal.Text = vo.Local;

			ltData.Text = FormatUtil.FormatDataHoraExtenso(vo.DataCriacao);

			DataTable dtBenef = FormExclusaoBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
			List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
			foreach (DataRow dr in dtBenef.Rows) {
				lst.Add(new ExclusaoBenefTelaVO() {
                    Codint = Convert.ToString(dr["BA1_CODINT"]),
                    Codemp = Convert.ToString(dr["BA1_CODEMP"]),
                    Matric = Convert.ToString(dr["BA1_MATRIC"]),
                    Tipreg = Convert.ToString(dr["BA1_TIPREG"]),
                    CodPlano = Convert.ToString(dr["BI3_CODIGO"]),
					IsDepFamilia = Convert.ToInt32(dr["in_dep_familia"]) == 1,
					IsTitular = Convert.ToInt32(dr["in_titular"]) == 1,
                    Nome = Convert.ToString(dr["BA1_NOMUSR"]),
                    Plano = Convert.ToString(dr["BI3_DESCRI"]),
                    Parentesco = Convert.ToString(dr["BRP_DESCRI"]),
                    Nascimento = Convert.ToString(dr["BA1_DATNAS"])
				});
			}
			gdvDependentes.DataSource = lst;
			gdvDependentes.DataBind();

			btnNova.Visible = true;
			btnLimparDep.Visible = false;
			tbSelecao.Visible = false;
			btnPdf.Visible = true;
		}

		private void ChangeTitular() {
			dpdDependente.Items.Clear();
			txtParentesco.Text = string.Empty;
			txtNomeTitular.Text = string.Empty;
			txtPlano.Text = string.Empty;
			txtEmailTitular.Text = string.Empty;
			txtIdade.Text = string.Empty;
			gdvDependentes.Visible = false;
			gdvDependentes.DataSource = new List<ExclusaoBenefTelaVO>();
			gdvDependentes.DataBind();

			BindTitular();
		}

		private string GetParentesco(ExclusaoBenefTelaVO vo) {
			if (vo.IsTitular) {
				return "TITULAR";
			} else {
				if (vo.IsDepFamilia) {
					return "DEPENDENTE FAMÍLIA";
				} else {
					return vo.Parentesco;
				}
			}
		}

		private void LimparDependentes() {
			foreach (GridViewRow row in gdvDependentes.Rows) {
				row.Visible = false;
			}
			RebindDependentes();
		}

        private void IncluirDependente(string parametro)
        {
			foreach (GridViewRow row in gdvDependentes.Rows) {

                string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);

                if (cd_usuario == parametro)
                {
					ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == parametro);
					if (vo.IsTitular) {
						this.ShowInfo("Ao selecionar o titular, todos os dependentes são incluídos automaticamente!");
					}
					row.Visible = true;
					break;
				}
			}
			RebindDependentes();
		}

		private void MostrarDependente() {
			if (string.IsNullOrEmpty(dpdDependente.SelectedValue)) {
				txtIdade.Text = "";
				txtParentesco.Text = "";
				txtPlano.Text = "";
			} else {

                string cd_usuario = dpdDependente.SelectedValue;

                ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);

                DateTime dataNascimento = DateTime.ParseExact(vo.Nascimento, "yyyyMMdd", CultureInfo.InvariantCulture);

                txtIdade.Text = DateUtil.CalculaIdade(dataNascimento).ToString();
				txtParentesco.Text = GetParentesco(vo);
				txtPlano.Text = vo.Plano;
			}
		}

		private void RebindDependentes() {
			string codemp_titular = "";
            string codint_titular = "";
            string matric_titular = "";
            string tipreg_titular = "";
			List<KeyValuePair<string, string>> lstBenef = new List<KeyValuePair<string, string>>();

			foreach (GridViewRow row in gdvDependentes.Rows) {
				if (row.Visible) {

                    string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);

                    ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);
					if (vo.IsTitular) {
						codemp_titular = vo.Codemp;
                        codint_titular = vo.Codint;
                        matric_titular = vo.Matric;
                        tipreg_titular = vo.Tipreg;
						break;
					}
				}
			}

			int i = 0;
			foreach (GridViewRow row in gdvDependentes.Rows) {
				if (matric_titular != "")
					row.Visible = true;

                string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);
                string[] dados_beneficiario = cd_usuario.Split('|');
                string codint = dados_beneficiario[0];
                string codemp = dados_beneficiario[1];
                string matric = dados_beneficiario[2];
                string tipreg = dados_beneficiario[3];

				if (row.Visible) {
					i++;
					Label lblRowNum = (Label)row.FindControl("lblRowNum");
					lblRowNum.Text = i.ToString();

					row.CssClass = (i % 2 == 0) ? "tbDependenteAlt" : "tbDependente";

					ImageButton btn = (ImageButton)row.FindControl("lnkRemoverDependente");
					btn.Visible = matric_titular == "" || matric == matric_titular;
				} else {
                    ExclusaoBenefTelaVO item = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);
					lstBenef.Add(new KeyValuePair<string, string>(item.Cdusuario, item.Nome));
				}
			}

			dpdDependente.DataSource = lstBenef;
			dpdDependente.DataBind();
			dpdDependente.Items.Insert(0, new ListItem("SELECIONE TITULAR/DEPENDENTE", ""));
		}

		private bool ValidateRequired() {
			List<ItemError> lst = new List<ItemError>();
			if (string.IsNullOrEmpty(txtCartao.Text)) {
				this.AddErrorMessage(lst, txtCartao, "Informe o cartão do titular!");
			}
			if (string.IsNullOrEmpty(txtEmailTitular.Text)) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe o e-mail!");
			}
			if (string.IsNullOrEmpty(txtLocal.Text)) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe o local!");
			}
			List<ExclusaoBenefTelaVO> lstBenef = GridToList();
			if (lstBenef.Count == 0) {
				this.AddErrorMessage(lst, gdvDependentes, "Informe pelo menos um beneficiário!");
			}
			if (string.IsNullOrEmpty(txtObs.Text)) {
				this.AddErrorMessage(lst, txtObs, "Para salvar o formulário de exclusão, é necessário preencher a observação!");
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
			} else if (!ValidateUtil.IsValidDomain(txtEmailTitular.Text)) {
				this.AddErrorMessage(lst, txtEmailTitular, "Informe o domínio do e-mail informado é inválido!");
			}

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}
			return true;
		}

		private List<ExclusaoBenefTelaVO> GridToList() {
			List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
			foreach (GridViewRow row in gdvDependentes.Rows) {
				if (row.Visible) {
                    string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);
                    ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);
					lst.Add(vo);
				}
			}
			return lst;
		}

		private void Salvar() {
			if (!ValidateFields()) {
				return;
			}

			ExclusaoVO vo = new ExclusaoVO();
			PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
			vo.Codint = titular.Codint;
            vo.Codemp = titular.Codemp;
			vo.Matric = titular.Matric;
			vo.Observacao = txtObs.Text;
			vo.CodUsuarioAlteracao = UsuarioLogado.Id;

			vo.Local = txtLocal.Text;
			vo.Status = StatusExclusao.PENDENTE;

			List<ExclusaoBenefTelaVO> lst = GridToList();

			List<string> nomes = lst.Select(x => x.Nome).ToList();
			string strNome = FormatUtil.ListToString(nomes);
			vo.NomeBeneficiarios = strNome;

			FormExclusaoBO.Instance.Salvar(vo, lst.Select(x => (ExclusaoBenefVO)x).ToList(), txtEmailTitular.Text.Trim());

			this.ShowInfo("A solicitação de exclusão somente será processada caso este formulário seja impresso, assinado e entregue ao representante da E-VIDA na regional ou diretamente na E-VIDA em Brasília-DF.");
			this.RegisterScript("IMPRIMIR", "openPdf()");
			this.btnSalvar.Visible = false;
			this.btnNova.Visible = true;

			try {
				vo = FormExclusaoBO.Instance.GetById(vo.CodSolicitacao);
				Bind(vo);
			} catch (Exception ex) {
				this.ShowError("Erro ao recarregar formulário!", ex);
			}
		}

		protected void gdvDependentes_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName.Equals("Excluir")) {
				int rowIndex = Convert.ToInt32(e.CommandArgument);
				gdvDependentes.Rows[rowIndex].Visible = false;
				RebindDependentes();
			}
		}

		protected void gdvDependentes_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				ExclusaoBenefTelaVO vo = (ExclusaoBenefTelaVO)row.DataItem;

				Label lblRowNum = (Label)row.FindControl("lblRowNum");

				row.Cells[2].Text = GetParentesco(vo);

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
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar formulário!", ex);
			}
		}

		protected void btnLimparDep_Click(object sender, EventArgs e) {
			try {
				LimparDependentes();
			} catch (Exception ex) {
				this.ShowError("Erro ao carregar beneficiários!", ex);
			}
		}

		protected void dpdDependente_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				MostrarDependente();
			} catch (Exception ex) {
				this.ShowError("Erro ao selecionar beneficiário!", ex);
			}
		}

		protected void btnAdicionar_Click(object sender, EventArgs e) {
			try {
				if (!string.IsNullOrEmpty(dpdDependente.SelectedValue)) {

                    string cd_usuario = dpdDependente.SelectedValue;

                    IncluirDependente(cd_usuario);
					MostrarDependente();
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao adicionar beneficiário!", ex);
			}
		}

		protected void txtCartao_TextChanged(object sender, EventArgs e) {
			try {
				ChangeTitular();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar titular!", ex);
			}
		}

	}
}