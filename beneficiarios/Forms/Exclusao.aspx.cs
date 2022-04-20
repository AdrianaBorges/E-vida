using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace eVidaBeneficiarios.Forms
{
    public partial class Exclusao : FormPageBase
    {
        [Serializable]
        private class ExclusaoBenefTelaVO : ExclusaoBenefVO
        {
            public string Nome { get; set; }
            public string Parentesco { get; set; }
            public string Plano { get; set; }
            public string Nascimento { get; set; }
        }

        protected override void PageLoad(object sender, EventArgs e) {
            if (!IsPostBack)
            {
                PUsuarioVO titular = UsuarioLogado.UsuarioTitular;

                txtEmailTitular.Text = titular.Email;
                txtMatricula.Text = titular.Matant;
                txtNomeTitular.Text = titular.Nomusr;
                //txtProtocolo.Text = titular.Protocolo;

                btnLimparDep.Visible = btnSalvar.Visible = false;

                if (!string.IsNullOrEmpty(Request["ID"]))
                {
                    int id = 0;
                    if (!Int32.TryParse(Request["ID"], out id))
                    {
                        this.ShowError("Identificador de requisição inexistente!");
                        return;
                    }

                    ExclusaoVO vo = FormExclusaoBO.Instance.GetById(id);
                    if (vo == null || vo.Codint != UsuarioLogado.Codint || vo.Codemp != UsuarioLogado.Codemp ||
                        vo.Matric != UsuarioLogado.Matric)
                    {
                        this.ShowError("Identificador de requisição inválido!");
                        return;
                    }
                    else
                    {
                        Bind(vo);
                    }

                }
                else
                {
                    gdvDependentes.DataSource = Beneficiarios;
                    gdvDependentes.DataBind();
                    ltData.Text = FormatUtil.FormatDataHoraExtenso(DateTime.Now);
                    LimparDependentes();
                    btnLimparDep.Visible = btnSalvar.Visible = true;
                }
            }

        }

        private List<ExclusaoBenefTelaVO> Beneficiarios
        {
            get
            {
                if (ViewState["BENEFICIARIOS"] == null)
                {
                    ViewState["BENEFICIARIOS"] = CarregarTodosBeneficiarios();
                }

                return ViewState["BENEFICIARIOS"] as List<ExclusaoBenefTelaVO>;
            }
        }

        private List<ExclusaoBenefTelaVO> CarregarTodosBeneficiarios()
        {
            List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
            DataTable dtBenef =
                PUsuarioBO.Instance.BuscarUsuarios(UsuarioLogado.Codint, UsuarioLogado.Codemp, UsuarioLogado.Matric);
            foreach (DataRow dr in dtBenef.Rows)
            {
                string tipreg = Convert.ToString(dr["BA1_TIPREG"]);
                string nmBeneficiario = Convert.ToString(dr["BA1_NOMUSR"]);
                string codPlano = Convert.ToString(dr["BI3_CODIGO"]);

                lst.Add(new ExclusaoBenefTelaVO()
                {
                    Codint = Convert.ToString(dr["BA1_CODINT"]),
                    Codemp = Convert.ToString(dr["BA1_CODEMP"]),
                    Matric = Convert.ToString(dr["BA1_MATRIC"]),
                    Tipreg = Convert.ToString(dr["BA1_TIPREG"]),
                    Cdusuario = Convert.ToString(dr["BA1_CODINT"]).Trim() + "|" +
                                Convert.ToString(dr["BA1_CODEMP"]).Trim() + "|" +
                                Convert.ToString(dr["BA1_MATRIC"]).Trim() + "|" +
                                Convert.ToString(dr["BA1_TIPREG"]).Trim(),
                    CodPlano = Convert.ToString(dr["BI3_CODIGO"]),
                    IsDepFamilia = false,
                    IsTitular = Convert.ToString(dr["BA1_TIPUSU"]).Trim() == "T",
                    Nome = Convert.ToString(dr["BA1_NOMUSR"]),
                    Plano = Convert.ToString(dr["BI3_DESCRI"]),
                    Parentesco = Convert.ToString(dr["BRP_DESCRI"]),
                    Nascimento = Convert.ToString(dr["BA1_DATNAS"]),
                });
            }

            dtBenef = ResponsavelBO.Instance.BuscarDependentes(UsuarioLogado.Codint, UsuarioLogado.Codemp,
                UsuarioLogado.Matric, TipoResponsavel.FAMILIA);
            foreach (DataRow dr in dtBenef.Rows)
            {
                string tipreg = Convert.ToString(dr["BA1_TIPREG"]);
                string nmBeneficiario = Convert.ToString(dr["BA1_NOMUSR"]);
                string codPlano = Convert.ToString(dr["BI3_CODIGO"]);

                ExclusaoBenefVO vo = lst.FirstOrDefault(x =>
                    x.Codint == UsuarioLogado.Codint && x.Codemp == UsuarioLogado.Codemp &&
                    x.Matric == UsuarioLogado.Matric && x.Tipreg == tipreg);

                if (vo == null)
                {
                    lst.Add(new ExclusaoBenefTelaVO()
                    {
                        Codint = Convert.ToString(dr["BA1_CODINT"]),
                        Codemp = Convert.ToString(dr["BA1_CODEMP"]),
                        Matric = Convert.ToString(dr["BA1_MATRIC"]),
                        Tipreg = Convert.ToString(dr["BA1_TIPREG"]),
                        Cdusuario = Convert.ToString(dr["BA1_CODINT"]).Trim() + "|" +
                                    Convert.ToString(dr["BA1_CODEMP"]).Trim() + "|" +
                                    Convert.ToString(dr["BA1_MATRIC"]).Trim() + "|" +
                                    Convert.ToString(dr["BA1_TIPREG"]).Trim(),
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

        private void Bind(ExclusaoVO vo)
        {
            litProtocolo.Value = vo.CodSolicitacao.ToString();

            txtLocal.Text = vo.Local;

            ltData.Text = String.Format("{0:dd \\de MMMM \\de yyyy HH:mm}", vo.DataCriacao);

            DataTable dtBenef = FormExclusaoBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
            List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
            foreach (DataRow dr in dtBenef.Rows)
            {
                lst.Add(new ExclusaoBenefTelaVO()
                {
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
            btnCancelar.Visible = false;
            btnPdf.Visible = true;
        }

        private string GetParentesco(ExclusaoBenefTelaVO vo)
        {
            if (vo.IsTitular)
            {
                return "TITULAR";
            }
            else
            {
                if (vo.IsDepFamilia)
                {
                    return "DEPENDENTE FAMÍLIA";
                }
                else
                {
                    return vo.Parentesco;
                }
            }
        }

        private void LimparDependentes()
        {
            foreach (GridViewRow row in gdvDependentes.Rows)
            {
                row.Visible = false;
            }

            RebindDependentes();
        }

        private void IncluirDependente(string parametro)
        {
            foreach (GridViewRow row in gdvDependentes.Rows)
            {

                string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);

                if (cd_usuario == parametro)
                {
                    ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == parametro);
                    if (vo.IsTitular)
                    {
                        this.ShowInfo("Ao selecionar o titular, todos os dependentes são incluídos automaticamente!");
                    }

                    row.Visible = true;
                    break;
                }
            }

            RebindDependentes();
        }

        private void MostrarDependente()
        {
            if (string.IsNullOrEmpty(dpdDependente.SelectedValue))
            {
                txtIdade.Text = "";
                txtParentesco.Text = "";
                txtPlano.Text = "";
            }
            else
            {

                string cd_usuario = dpdDependente.SelectedValue;

                ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);

                DateTime dataNascimento = DateTime.ParseExact(vo.Nascimento, "yyyyMMdd", CultureInfo.InvariantCulture);

                txtIdade.Text = DateUtil.CalculaIdade(dataNascimento).ToString();
                txtParentesco.Text = GetParentesco(vo);
                txtPlano.Text = vo.Plano;
            }
        }

        private void RebindDependentes()
        {
            string codint_titular = "";
            string codemp_titular = "";
            string matric_titular = "";
            string tipreg_titular = "";
            List<KeyValuePair<string, string>> lstBenef = new List<KeyValuePair<string, string>>();

            foreach (GridViewRow row in gdvDependentes.Rows)
            {
                if (row.Visible)
                {

                    string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);

                    ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);
                    if (vo.IsTitular)
                    {
                        codint_titular = vo.Codint;
                        codemp_titular = vo.Codemp;
                        matric_titular = vo.Matric;
                        tipreg_titular = vo.Tipreg;
                        break;
                    }
                }
            }

            int i = 0;
            foreach (GridViewRow row in gdvDependentes.Rows)
            {
                if (matric_titular != "")
                    row.Visible = true;

                string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);
                string[] dados_beneficiario = cd_usuario.Split('|');
                string codint = dados_beneficiario[0];
                string codemp = dados_beneficiario[1];
                string matric = dados_beneficiario[2];
                string tipreg = dados_beneficiario[3];

                if (row.Visible)
                {
                    i++;
                    Label lblRowNum = (Label) row.FindControl("lblRowNum");
                    lblRowNum.Text = i.ToString();

                    row.CssClass = (i % 2 == 0) ? "tbDependenteAlt" : "tbDependente";

                    ImageButton btn = (ImageButton) row.FindControl("lnkRemoverDependente");
                    btn.Visible = matric_titular == "" || matric == matric_titular;
                }
                else
                {
                    ExclusaoBenefTelaVO item = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);
                    lstBenef.Add(new KeyValuePair<string, string>(item.Cdusuario, item.Nome));
                }
            }

            dpdDependente.DataSource = lstBenef;
            dpdDependente.DataBind();
            dpdDependente.Items.Insert(0, new ListItem("SELECIONE TITULAR/DEPENDENTE", ""));
        }

        private bool ValidateRequired()
        {
            List<ItemError> lst = new List<ItemError>();
            if (string.IsNullOrEmpty(txtEmailTitular.Text))
            {
                this.AddErrorMessage(lst, txtEmailTitular, "Informe o e-mail!");
            }

            if (string.IsNullOrEmpty(txtLocal.Text))
            {
                this.AddErrorMessage(lst, txtEmailTitular, "Informe o local!");
            }

            List<ExclusaoBenefTelaVO> lstBenef = GridToList();
            if (lstBenef.Count == 0)
            {
                this.AddErrorMessage(lst, gdvDependentes, "Informe pelo menos um beneficiário!");
            }

            if (lst.Count > 0)
            {
                this.ShowErrorList(lst);
                return false;
            }

            return true;
        }

        private bool ValidateFields()
        {
            if (!ValidateRequired())
                return false;

            List<ItemError> lst = new List<ItemError>();
            if (!FormatUtil.IsValidEmail(txtEmailTitular.Text.Trim()))
            {
                this.AddErrorMessage(lst, txtEmailTitular, "Informe um e-mail válido!");
            }
            else if (!ValidateUtil.IsValidDomain(txtEmailTitular.Text.Trim()))
            {
                this.AddErrorMessage(lst, txtEmailTitular, "Informe o domínio do e-mail informado é inválido!");
            }

            if (lst.Count > 0)
            {
                this.ShowErrorList(lst);
                return false;
            }

            return true;
        }

        private List<ExclusaoBenefTelaVO> GridToList()
        {
            List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
            foreach (GridViewRow row in gdvDependentes.Rows)
            {
                if (row.Visible)
                {
                    string cd_usuario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["Cdusuario"]);
                    ExclusaoBenefTelaVO vo = Beneficiarios.Find(x => x.Cdusuario == cd_usuario);
                    lst.Add(vo);
                }
            }

            return lst;
        }

        private void Salvar()
        {
            if (!ValidateFields())
            {
                return;
            }

            ExclusaoVO vo = new ExclusaoVO();
            vo.Codint = UsuarioLogado.Codint;
            vo.Codemp = UsuarioLogado.Codemp;
            vo.Matric = UsuarioLogado.Matric;

            vo.Local = txtLocal.Text;
            vo.Status = StatusExclusao.PENDENTE;

            List<ExclusaoBenefTelaVO> lst = GridToList();
            
            List<string> nomes = lst.Select(x => x.Nome).ToList();
            string strNome = FormatUtil.ListToString(nomes);
            vo.NomeBeneficiarios = strNome;

            var dados = GetDados("Adriana Borges", "adriana.borges@evbl.com", "123456", "025.477.187-40", "(21)9999-9999", "teste", "34", "87", "15", "testete", "testeteste");

            string script = string.Format("getProtocolo('{0}','{1}')", dados, hdnProtocolo.ClientID);

            RegisterScript("GetProtocolo", script);

            string valorHiddenField = Request.Form[hdnProtocolo.UniqueID];



            if (!string.IsNullOrEmpty(hdnProtocolo.Value))
            {
                //Seu código
            }


            string protocolo = hdnProtocolo.Value;
            txtProtocolo.Text = protocolo;


            foreach (var lista in lst)
            {
                lista.Protocolo = txtProtocolo.Text;
            }

            vo.Protocolo = protocolo;
            // aqui ele faz a chamada
            FormExclusaoBO.Instance.Salvar(vo, lst.Select(x => (ExclusaoBenefVO)x).ToList(), txtEmailTitular.Text.Trim());

            this.ShowInfo("A solicitação de exclusão somente será processada caso este formulário seja impresso, assinado e entregue ao representante da E-VIDA na regional ou diretamente na E-VIDA em Brasília-DF.");
            this.RegisterScript("IMPRIMIR", "openPdf()");
            this.btnSalvar.Visible = false;
            this.btnNova.Visible = true;

            try
            {
                vo = FormExclusaoBO.Instance.GetById(vo.CodSolicitacao);
                Bind(vo);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao recarregar formulário!", ex);
            }

        }


        public async Task<HttpResponseMessage> SendRequestAsync(string adaptiveUri, string xmlRequest)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent httpConent = new StringContent(xmlRequest, Encoding.UTF8);

                HttpResponseMessage responseMessage = null;
                try
                {
                    responseMessage = await httpClient.PostAsync(adaptiveUri, httpConent);
                }
                catch (Exception ex)
                {
                    if (responseMessage == null)
                    {
                        responseMessage = new HttpResponseMessage();
                    }
                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                    responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendRequest failed: {0}", ex);
                }
                return responseMessage;
            }
        }

      

        protected void gdvDependentes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Excluir"))
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                gdvDependentes.Rows[rowIndex].Visible = false;
                RebindDependentes();
            }
        }

        protected void gdvDependentes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                ExclusaoBenefTelaVO vo = (ExclusaoBenefTelaVO) row.DataItem;

                Label lblRowNum = (Label) row.FindControl("lblRowNum");

                row.Cells[2].Text = GetParentesco(vo);

                lblRowNum.Text = (row.DataItemIndex + 1).ToString();

                if (!string.IsNullOrEmpty(litProtocolo.Value))
                {
                    ImageButton btnRemover = row.FindControl("lnkRemoverDependente") as ImageButton;
                    btnRemover.Visible = false;
                }
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Salvar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar formulário!", ex);
            }
        }

        protected void btnLimparDep_Click(object sender, EventArgs e)
        {
            try
            {
                LimparDependentes();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao carregar beneficiários!", ex);
            }
        }

        protected void dpdDependente_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MostrarDependente();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao selecionar beneficiário!", ex);
            }
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(dpdDependente.SelectedValue))
                {

                    string cd_usuario = dpdDependente.SelectedValue;

                    IncluirDependente(cd_usuario);
                    MostrarDependente();
                }
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao adicionar beneficiário!", ex);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                txtEmailTitular.Text = UsuarioLogado.Titular.Email;
                txtLocal.Text = "";
                LimparDependentes();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao executar cancelamento!", ex);
            }
        }

        private string GetDados(string name, string email, string code, string cpf, string phone, string address, string group, string category, string type, string subject, string text)
        {
            return
                $"source=37&name={name}&email={email}&code={code}&cpf={cpf}&phone={phone}&address={address}&group={@group}&category={category}&type={type}&subject={subject}&text={text}";
        }

        public new void RegisterScript(string key, string script)
        {
            //var sm = (ScriptManager)FindControl("scriptManagerMaster");

            //if (sm != null)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), key, script, true);
            //    return;
            //}

            this.ClientScript.RegisterStartupScript(this.GetType(), key, script, true);
        }

        protected void btnExibir_Click(object sender, EventArgs e)
        {

            ExclusaoVO vo = new ExclusaoVO();
            vo.Codint = UsuarioLogado.Codint;
            vo.Codemp = UsuarioLogado.Codemp;
            vo.Matric = UsuarioLogado.Matric;

            vo.Local = txtLocal.Text;
            vo.Status = StatusExclusao.PENDENTE;

            List<ExclusaoBenefTelaVO> lst = GridToList();

            List<string> nomes = lst.Select(x => x.Nome).ToList();
            string strNome = FormatUtil.ListToString(nomes);
            vo.NomeBeneficiarios = strNome;

            var dados = GetDados("Adriana Borges", "adriana.borges@evbl.com", "123456", "025.477.187-40", "(21)9999-9999", "teste", "34", "87", "15", "testete", "testeteste");

            string script = string.Format("getProtocolo('{0}','{1}')", dados, hdnProtocolo.ClientID);

            RegisterScript("GetProtocolo", script);

            txtProtocolo.Text = Request.Form[hdnProtocolo.UniqueID];


        }
    }
}