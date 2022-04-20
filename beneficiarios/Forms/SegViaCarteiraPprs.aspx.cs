using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaBeneficiarios.Forms
{
    public partial class SegViaCarteiraPprs : FormPageBase
    {

        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PUsuarioVO titular = UsuarioLogado.UsuarioTitular;
                txtEmailTitular.Text = titular.Email;
                txtNomeTitular.Text = titular.Nomusr;
                txtCartao.Text = titular.Matant;

                DataTable dt = SegViaCarteiraBO.Instance.ListarDadosSegViaCarteira(titular);
                dt.Columns.Add(new DataColumn("Motivo", typeof(int)));
                dt.Columns.Add(new DataColumn("Checked", typeof(bool)));

                DataTable dtPendente = SegViaCarteiraBO.Instance.BuscarBeneficiariosPendentes(titular);
                foreach (DataRow drPendente in dtPendente.Rows)
                {
                    DataRow[] drs = dt.Select("BA1_CODINT = " + Convert.ToString(drPendente["BA1_CODINT"]).Trim() + " and BA1_CODEMP = " + Convert.ToString(drPendente["BA1_CODEMP"]).Trim() + " and BA1_MATRIC = " + Convert.ToString(drPendente["BA1_MATRIC"]).Trim() + " and BA1_TIPREG = " + Convert.ToString(drPendente["BA1_TIPREG"]).Trim());
                    if (drs.Length > 0)
                    {
                        foreach (DataRow dr2 in drs)
                        {
                            dt.Rows.Remove(dr2);
                        }
                    }
                }

                gdvDependentes.DataSource = dt;
                gdvDependentes.DataBind();

                if (dt.Rows.Count == 0)
                {
                    this.ShowInfo("Todos os beneficiários associados a você já possuem solicitação de segunda via pendente!");
                    btnSalvar.Visible = false;
                    return;
                }
            }

        }

        protected void gdvDependentes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                Label lblRowNum = (Label)row.FindControl("lblRowNum");
                DropDownList dpdMotivo = (DropDownList)row.FindControl("dpdMotivo");
                CheckBox chk = (CheckBox)row.FindControl("chk");

                lblRowNum.Text = (row.DataItemIndex + 1).ToString();
                dpdMotivo.SelectedValue = Convert.ToString(vo["Motivo"]);
                chk.Checked = vo["Checked"] != DBNull.Value ? Convert.ToBoolean(vo["Checked"]) : false;
            }
        }

        private MotivoSegVia? ColetarTipoMotivo()
        {
            MotivoSegVia? motivo = null;
            List<SolicitacaoSegViaCarteiraBenefVO> lst = GridToList();
            foreach (SolicitacaoSegViaCarteiraBenefVO sol in lst)
            {
                motivo = (MotivoSegVia)sol.Motivo;
                if (motivo == MotivoSegVia.ROUBO_FURTO)
                    return motivo;
            }
            return motivo;
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!FormatUtil.IsValidEmail(txtEmailTitular.Text.Trim()))
            {
                this.ShowError("Informe o e-mail!");
                this.SetFocus(txtEmailTitular);
                return;
            }
            MotivoSegVia? motivo = ColetarTipoMotivo();
            if (motivo == null)
            {
                this.ShowError("Informe pelo menos uma solicitação de 2ª Via!");
                return;
            }
            if (string.IsNullOrEmpty(txtLocal.Text))
            {
                this.ShowError("Informe o local!");
                this.SetFocus(txtLocal);
                return;
            }
            List<SolicitacaoSegViaCarteiraBenefVO> lst = GridToList();
            DataTable dt = SegViaCarteiraBO.Instance.BuscarBeneficiariosPendentes(UsuarioLogado.UsuarioTitular);
            StringBuilder sb = new StringBuilder();
            foreach (SolicitacaoSegViaCarteiraBenefVO benef in lst)
            {
                DataRow[] drs = dt.Select("BA1_CODINT = " + benef.Codint.Trim() + " and BA1_CODEMP = " + benef.Codemp.Trim() + " and BA1_MATRIC = " + benef.Matric.Trim() + " and BA1_TIPREG = " + benef.Tipreg.Trim());
                if (drs.Length > 0)
                {
                    sb.AppendLine(string.Format("O beneficiário {0} possui a solicitação {1} pendente!\n", drs[0]["BA1_NOMUSR"], drs[0]["cd_solicitacao"]));
                }
            }
            if (sb.Length > 0)
            {
                this.ShowError(sb.ToString());
                return;
            }

            if (motivo == MotivoSegVia.QUEBRA || motivo == MotivoSegVia.PERDA)
            {
                this.RegisterScript("confirm", "ConfirmQuebraPerda()");
                return;
            }

            if (ExigirAnexo())
            {
                if (Arquivos.Count == 0)
                {
                    this.ShowError("Anexe o Boletim de Ocorrência do Roubo/Furto.");
                    return;
                }

                if (Arquivos.Count > 1)
                {
                    this.ShowError("Anexe apenas 1 Boletim de Ocorrência do Roubo/Furto. Remova os demais.");
                    return;
                }
            }

            Salvar(false);
        }

        private void Salvar(bool fromConfirm)
        {
            SolicitacaoSegViaCarteiraVO vo = new SolicitacaoSegViaCarteiraVO();
            vo.Criacao = DateTime.Now;
            vo.Codint = UsuarioLogado.UsuarioTitular.Codint.Trim();
            vo.Codemp = UsuarioLogado.UsuarioTitular.Codemp.Trim();
            vo.Matric = UsuarioLogado.UsuarioTitular.Matric.Trim();
            vo.Local = txtLocal.Text.Trim();
            vo.Status = (char)StatusSegVia.PENDENTE;

            List<SolicitacaoSegViaCarteiraBenefVO> lst = GridToList();

            try
            {
                #region[SALVAR SOLICITAÇÃO]

                SegViaCarteiraBO.Instance.Salvar(vo, lst, txtEmailTitular.Text.Trim(), Arquivos);
                lblProtocolo.Text = "Protocolo gerado: " + string.Format("{0:0000000000}", vo.CdSolicitacao) + "<br/> " +
                    "Protocolo ANS: " + vo.ProtocoloAns;

                #endregion

                #region[ENVIAR EMAIL]

                bool erroEmail = false;
                try
                {
                    if (ParametroUtil.EmailEnabled)
                    {
                        Report2aViaCarteira rpt = new Report2aViaCarteira(ReportDir, UsuarioLogado);
                        vo = SegViaCarteiraBO.Instance.GetById(vo.CdSolicitacao);
                        byte[] anexo = rpt.GerarRelatorio(vo);
                        PUsuarioVO funcVO = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
                        SegViaCarteiraBO.Instance.EnviarEmailCriacao(vo, funcVO, anexo);
                    }
                }
                catch (Exception ex)
                {
                    erroEmail = true;
                    Log.Error("Erro ao enviar o email", ex);
                }

                #endregion

                #region[ENVIAR AUTORIZAÇÃO PROVISÓRIA]

                bool erroAutorizacao = false;
                try
                {
                    foreach (SolicitacaoSegViaCarteiraBenefVO solBenef in lst)
                    {

                        // Salvar autorização provisória
                        AutorizacaoProvisoriaVO autorizacao = CriarAutorizacaoProvisoria(solBenef.Codint.Trim(), solBenef.Codemp.Trim(), solBenef.Matric.Trim(), solBenef.Tipreg.Trim());
                        int cdSolicitacao = AutorizacaoProvisoriaBO.Instance.SalvarIdentity(autorizacao);

                        // Aprovar autorização provisória
                        autorizacao = AutorizacaoProvisoriaBO.Instance.GetById(cdSolicitacao);
                        ReportAutorizacaoProvisoria rpt = new ReportAutorizacaoProvisoria(ReportDir, UsuarioLogado);
                        byte[] anexo = rpt.GerarRelatorio(autorizacao);
                        // O usuário 524 é o Márcio Souza
                        AutorizacaoProvisoriaBO.Instance.Aprovar(autorizacao, 524, anexo);
                    }
                }
                catch (Exception ex)
                {
                    erroAutorizacao = true;
                    Log.Error("Erro ao gerar a autorização provisória.", ex);
                }

                #endregion

                #region[ATUALIZAR TELA]

                this.btnSalvar.Visible = false;
                this.btnPdf.Visible = true;
                this.btnNova.Visible = true;
                btnPdf.Attributes["onclick"] = "return openPdf(" + vo.CdSolicitacao + ")";

                if (!fromConfirm)
                    //this.ShowInfo("Solicitação enviada com sucesso! Imprimir este formulário, assinar, anexar cópia do Boletim de Ocorrência Policial e entregar na E-VIDA ou na área de benefícios da regional da Eletronorte.");
                    this.ShowInfo("Solicitação enviada com sucesso! Não é necessário imprimir o formulário!");
                else
                    this.ShowInfo("Solicitação enviada com sucesso! Não é necessário imprimir o formulário!");

                if (erroEmail)
                    this.ShowInfo("Houve um erro ao enviar o formulário ao seu e-mail. Por favor imprima-o se necessário!");

                if (erroAutorizacao)
                    this.ShowInfo("Houve um erro ao enviar a autorização provisória.");

                ViewState["ID"] = vo.CdSolicitacao;

                #endregion
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar a solicitação. ", ex);
            }
        }

        protected void hidSalvar_ValueChanged(object sender, EventArgs e)
        {
            Salvar(true);
        }

        private AutorizacaoProvisoriaVO CriarAutorizacaoProvisoria(string codint, string codemp, string matric, string tipreg)
        {
            AutorizacaoProvisoriaVO vo = new AutorizacaoProvisoriaVO();

            vo.CodSolicitacao = 0;

            PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(codint.Trim(), codemp.Trim(), matric.Trim(), tipreg.Trim());
            vo.Codint = benef.Codint.Trim();
            vo.Codemp = benef.Codemp.Trim();
            vo.Matric = benef.Matric.Trim();
            vo.Tipreg = benef.Tipreg.Trim();

            vo.CodUsuarioCriacao = 524; // Verificar se o usuário 524 é o do Márcio Souza
            vo.FimVigencia = DateTime.Now.AddDays(15);

            PlantaoSocialLocalVO plantaoBSB = new PlantaoSocialLocalVO();
            plantaoBSB.Id = 14;
            plantaoBSB.Uf = "DF";
            plantaoBSB.CodMunicipio = "5300108";
            plantaoBSB.Cidade = "BRASÍLIA";
            plantaoBSB.Telefone = "(61) 9968-0322";

            vo.Local = Locais.Find(x => x.Cidade.Trim().Equals(txtLocal.Text.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (vo.Local == null)
            {
                vo.Local = plantaoBSB;
            }

            PFamiliaVO familia = PFamiliaBO.Instance.GetByMatricula(vo.Codint.Trim(), vo.Codemp.Trim(), vo.Matric.Trim());
            PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(familia.Codpla.Trim());
            vo.Plano = plano;

            vo.IsReciprocidade = false;

            List<string> lstCobertura = new List<string>();
            lstCobertura.Add("AMB");
            lstCobertura.Add("OBS");
            lstCobertura.Add("ODO");
            lstCobertura.Add("UrE");
            vo.Coberturas = lstCobertura;

            vo.Abrangencia = AbrangenciaAutorizacaoProvisoria.NACIONAL;

            List<Int32> lstProcedimento = new List<Int32>();
            lstProcedimento.Add(6);
            vo.Procedimentos = lstProcedimento;

            vo.Observacao = "Necessário Autorização Prévia para Cirurgia, Intern., TC, RM, Cintilografia, Polissonografia, Terapias, Fisio buco-maxilo-facial, Trat. Dermat., Odonto, Monit. Epilepsia, DIU, Quimio, Sedação Profunda, Painel Hibrid. Mol., RPG, Hidroterapia e Home Care.";

            return vo;
        }

        private List<PlantaoSocialLocalVO> Locais
        {
            get
            {
                List<PlantaoSocialLocalVO> lst = (List<PlantaoSocialLocalVO>)Session["PLANTAO_LOCAL"];
                if (lst == null)
                {
                    lst = AutorizacaoProvisoriaBO.Instance.ListarPlantaoSocialLocal();
                    Locais = lst;
                }
                return lst;
            }
            set { Session["PLANTAO_LOCAL"] = value; }
        }

        private List<SolicitacaoSegViaCarteiraBenefVO> GridToList()
        {
            List<SolicitacaoSegViaCarteiraBenefVO> lst = new List<SolicitacaoSegViaCarteiraBenefVO>();
            foreach (GridViewRow row in gdvDependentes.Rows)
            {

                string motivo = (row.FindControl("dpdMotivo") as DropDownList).SelectedValue;
                if (!string.IsNullOrEmpty(motivo))
                {
                    CheckBox chk = row.FindControl("chk") as CheckBox;
                    if (chk.Checked)
                    {
                        SolicitacaoSegViaCarteiraBenefVO benef = new SolicitacaoSegViaCarteiraBenefVO();

                        string cd_beneficiario = Convert.ToString(gdvDependentes.DataKeys[row.RowIndex]["cd_beneficiario"]);
                        string[] dados_beneficiario = cd_beneficiario.Split('|');
                        benef.Codint = dados_beneficiario[0].Trim();
                        benef.Codemp = dados_beneficiario[1].Trim();
                        benef.Matric = dados_beneficiario[2].Trim();
                        benef.Tipreg = dados_beneficiario[3].Trim();
                        
                        benef.Motivo = motivo[0];
                        lst.Add(benef);
                    }
                }
            }
            return lst;
        }

        protected void btnNova_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Forms/SegViaCarteiraPprs.aspx");
        }

        #region[ARQUIVOS]

        public List<ArquivoTelaVO> Arquivos
        {
            get
            {
                if (ViewState["ARQUIVOS"] == null)
                {
                    Arquivos = new List<ArquivoTelaVO>();
                }
                return ViewState["ARQUIVOS"] as List<ArquivoTelaVO>;
            }
            set
            {
                ViewState["ARQUIVOS"] = value;
            }
        }

        public int? Id
        {
            get { return ViewState["ID"] == null ? new int?() : Convert.ToInt32(ViewState["ID"]); }
            set { ViewState["ID"] = value; }
        }

        protected void btnIncluirArquivo_Click(object sender, EventArgs e)
        {
            AddArquivo(hidArqFisico.Value, hidArqOrigem.Value);
        }

        private void AddArquivo(string fisico, string original)
        {
            List<ArquivoTelaVO> lstAtual = Arquivos;
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = original,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoSolicitacaoSegViaCarteira.BOLETIM_OCORRENCIA).ToString());
            bool contains = lstAtual.FindIndex(x => x.NomeTela.Equals(original, StringComparison.InvariantCultureIgnoreCase)) >= 0;
            if (!contains)
            {
                lstAtual.Add(vo);
            }
            else
            {
                this.ShowError("Este arquivo já existe na listagem! Por favor, exclua o antigo ou renomeie o arquivo novo!");
                return;
            }

            ltvArquivo.DataSource = lstAtual;
            ltvArquivo.DataBind();

            if (Id != null)
            {
                //SalvarAddArquivo();
            }
            else
            {
                this.ShowInfo("Arquivo adicionado em tela! Arquivos só serão salvos quando formulário enviado!");
            }
        }

        protected void btnRemoverArquivo_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
                RemoverArquivo(row);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover item da lista", ex);
            }
        }

        private void RemoverArquivo(ListViewDataItem row)
        {
            int idx = row.DataItemIndex;

            List<ArquivoTelaVO> lstAtual = Arquivos;
            ArquivoTelaVO telaVO = lstAtual[idx];
            if (!telaVO.IsNew)
            {
                SolicitacaoSegViaCarteiraArquivoVO vo = ArquivoTela2VO(telaVO);
                SegViaCarteiraBO.Instance.RemoverArquivo(vo);
                CarregarArquivos();
            }
            else
            {
                List<ArquivoTelaVO> lstBefore = Arquivos;
                lstBefore.RemoveAt(idx);
                Arquivos = lstBefore;
                BindArquivos();
            }

        }

        private SolicitacaoSegViaCarteiraArquivoVO ArquivoTela2VO(ArquivoTelaVO telaVO)
        {
            SolicitacaoSegViaCarteiraArquivoVO vo = new SolicitacaoSegViaCarteiraArquivoVO();
            if (!string.IsNullOrEmpty(telaVO.Id))
            {
                vo.IdArquivo = Int32.Parse(telaVO.Id);
            }
            vo.IdSegViaCarteira = Id.Value;
            vo.NomeArquivo = telaVO.NomeTela;
            return vo;
        }

        private static ArquivoTelaVO ArquivoVO2Tela(SolicitacaoSegViaCarteiraArquivoVO x)
        {
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                Id = x.IdArquivo.ToString(),
                NomeTela = x.NomeArquivo,
                IsNew = false,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)x.TipoArquivo).ToString());
            return vo;
        }

        private void CarregarArquivos()
        {
            dvArquivos.Visible = true;

            List<ArquivoTelaVO> lstBefore = Arquivos;

            List<SolicitacaoSegViaCarteiraArquivoVO> lstArquivos = SegViaCarteiraBO.Instance.ListarArquivos(Id.Value);
            if (lstArquivos == null)
                lstArquivos = new List<SolicitacaoSegViaCarteiraArquivoVO>();

            List<ArquivoTelaVO> lstArqs = lstArquivos.Select(x => ArquivoVO2Tela(x)).ToList();

            Arquivos = lstArqs;

            BindArquivos();
        }

        private void BindArquivos()
        {
            List<ArquivoTelaVO> lstArqs = Arquivos;

            ltvArquivo.DataSource = lstArqs.Where(x => x.Parameters["TP_ARQUIVO"].Equals(((int)TipoArquivoSolicitacaoSegViaCarteira.BOLETIM_OCORRENCIA).ToString()));
            ltvArquivo.DataBind();

            updArquivos.Update();
            btnIncluirArquivo.Visible = true;
        }

        protected void dpdMotivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AvaliarPainelAnexo();
        }

        protected void chk_CheckedChanged(object sender, EventArgs e)
        {
            AvaliarPainelAnexo();
        }

        private void AvaliarPainelAnexo()
        {
            if (ExigirAnexo())
                pnlAnexo.Visible = true;
            else
                pnlAnexo.Visible = false;
        }

        private Boolean ExigirAnexo()
        {

            Boolean exigir = false;

            foreach (GridViewRow row in gdvDependentes.Rows)
            {
                DropDownList dpdMotivoBenef = (DropDownList)row.FindControl("dpdMotivo") as DropDownList;
                CheckBox chkBenef = row.FindControl("chk") as CheckBox;

                if (dpdMotivoBenef.Text == "R" && chkBenef.Checked)
                    exigir = true;
            }

            return exigir;
        }

        #endregion

    }
}