using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Adesao
{

    public partial class IntegrarAdesao : FormPageBase
    {
        protected override Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.ADESAO; }
        }

        public int Id
        {
            get { return (int)ViewState["ID"]; }
            set { ViewState["ID"] = value; }
        }

        public bool RequireMotivoDesligamento
        {
            get { return (bool)ViewState["REQ_MOT_DES"]; }
            set { ViewState["REQ_MOT_DES"] = value; }
        }

        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["ID"]))
                {
                    this.ShowError("Para integração deve ser selecionado um formulário!");
                    return;
                }
                int cdProtocolo;
                if (!Int32.TryParse(Request["ID"], out cdProtocolo))
                {
                    this.ShowError("Protocolo inválido!");
                    return;
                }

                List<KeyValuePair<string, string>> lstMotivosFamilia = PLocatorDataBO.Instance.ListarMotivosDesligamentoFamilia();
                dpdMotivoDesligamentoFamilia.DataSource = lstMotivosFamilia;
                dpdMotivoDesligamentoFamilia.DataBind();
                dpdMotivoDesligamentoFamilia.Items.Insert(0, new ListItem("SELECIONE", ""));

                List<KeyValuePair<string, string>> lstMotivosUsuario = PLocatorDataBO.Instance.ListarMotivosDesligamentoUsuario();
                dpdMotivoDesligamentoUsuario.DataSource = lstMotivosUsuario;
                dpdMotivoDesligamentoUsuario.DataBind();
                dpdMotivoDesligamentoUsuario.Items.Insert(0, new ListItem("SELECIONE", ""));

                RequireMotivoDesligamento = false;

                // Preenche a tela com os dados da declaração atual
                Bind(cdProtocolo);
            }
        }

        // Preenche a tela com os dados da declaração atual
        private void Bind(int cdProtocolo)
        {
            // Obtém a declaração atual
            PDeclaracaoVO decVO = PAdesaoBO.Instance.GetById(cdProtocolo);
            if (decVO == null)
            {
                this.ShowError("Formulário não encontrado! (" + cdProtocolo + ")");
                return;
            }

            // Verifica se o produto é válido para a empresa
            if (!PAdesaoBO.Instance.IsValidForIntegracao(decVO.Empresa, decVO.Produto))
            {
                throw new Exception("Este produto não está mapeado na integração! Empresa: " + decVO.Empresa + " produto: " + decVO.Produto);
            }

            Id = decVO.Numero;
            litEmpresa.Text = decVO.Empresa.ToString();
            litProduto.Text = PDados.Produto.Find(decVO.Produto).Descricao;
            litProtocolo.Text = decVO.Numero.ToString();

            DataTable dt = PLocatorDataBO.Instance.ListarCategorias((int)decVO.Empresa).Copy();
            dpdCategoria.DataSource = dt;
            dpdCategoria.DataBind();
            dpdCategoria.Items.Insert(0, new ListItem("SELECIONE", ""));

            #region[PREENCHIMENTO DOS DADOS DO TITULAR NA TELA]

            // Busca a Família da proposta no Protheus
            PFamiliaVO familia = PFamiliaBO.Instance.GetByMatriculaTitular(decVO.Empresa, decVO.Titular.Matemp, decVO.Titular);

            // Lista de usuários
            List<PUsuarioVO> lstUsuarios = null;

            // Usuário titular
            PUsuarioVO titular = null;

            if(familia != null){

                // Busca a lista de usuários da família no Protheus
                lstUsuarios = PUsuarioBO.Instance.ListarUsuarios(familia.Codint, familia.Codemp, familia.Matric);

                // Obtém o usuário titular
                if (lstUsuarios != null)
                    titular = lstUsuarios.Find(x => x.Tipusu.Equals(PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO));

            }

            if (titular != null)
            {
                litIdBeneficiario.Text = titular.Matant.ToString().Trim();
            }
            else
            {
                litIdBeneficiario.Text = "(NOVO)";
            }

            litMatricula.Text = decVO.Titular.Matemp.Trim();
            litNomeTitular.Text = decVO.Titular.Nome.Trim();
            litCpfTitular.Text = decVO.Titular.Cpf.Trim();

            #endregion

            if (decVO.InicioPlano != DateTime.MinValue)
                txtDataInicio.Text = decVO.InicioPlano.ToString("dd/MM/yyyy");

            // Se a declaração já está integrada
            if (decVO.DataIntegracao != null)
            {
                txtDataInicio.Text = decVO.InicioPlanoIntegracao.Value.ToString("dd/MM/yyyy");
                txtDataInicio.Enabled = false;
                btnIntegrar.Visible = false;
                dpdCategoria.SelectedValue = decVO.CdCategoria.Value.ToString();
                dpdMotivoDesligamentoFamilia.SelectedValue = decVO.CdMotivoDesligamentoFamilia != null ? decVO.CdMotivoDesligamentoFamilia.ToString() : "";
                dpdMotivoDesligamentoUsuario.SelectedValue = decVO.CdMotivoDesligamentoUsuario != null ? decVO.CdMotivoDesligamentoUsuario.ToString() : "";
                string idPlano = decVO.PlanoIntegracao;
                PProdutoSaudeVO produtoSaude = PLocatorDataBO.Instance.GetProdutoSaude(idPlano);
                if (produtoSaude == null)
                {
                    this.ShowError("Plano não encontrado: " + idPlano);
                }
                litPlano.Text = "(" + produtoSaude.Codigo + ") " + produtoSaude.Descri;
                hidPlano.Value = produtoSaude.Codigo;
                lblIntegrada.Text = "A proposta foi integrada em: " + decVO.DataIntegracao.Value.ToString("dd/MM/yyyy HH:mm");
                dpdCarencia.SelectedValue = decVO.CarenciaIntegracao;
            }
            // Se a declaração ainda não está integrada
            else
            {
                
                int idPlano = PDados.Produto.Find(decVO.Produto).ToProtheus();
                PProdutoSaudeVO produtoSaude = PLocatorDataBO.Instance.GetProdutoSaude(idPlano.ToString());
                if (produtoSaude == null)
                {
                    this.ShowError("Plano não encontrado: " + idPlano);
                }
                litPlano.Text = "(" + produtoSaude.Codigo + ") " + produtoSaude.Descri;
                hidPlano.Value = produtoSaude.Codigo;
                
                if (decVO.Titular.Admissao != null)
                {
                    if (decVO.Titular.Admissao.Value.AddDays(30) >= decVO.Criacao.Date)
                    {
                        dpdCarencia.SelectedValue = "NOR";
                    }
                    else
                    {
                        dpdCarencia.SelectedValue = "IT";
                    }
                }

                if (!PAdesaoBO.Instance.IsValidPlano(decVO.Empresa, idPlano))
                {
                    btnIntegrar.Visible = false;
                    throw new InvalidOperationException("O plano " + produtoSaude.Descri + " não é válido para a empresa " + decVO.Empresa.ToString());
                }

                if (decVO.Titular.Lotacao != null) {

                    if (decVO.Empresa == PDados.Empresa.CEA)
                    {
                        if (idPlano == PConstantes.PLANO_MAIS_VIDA_CEA)
                        {
                            if (decVO.Titular.Lotacao.Equals("CEA FEDERAL") || decVO.Titular.Lotacao.Equals("CFED"))
                            {
                                dpdCategoria.SelectedValue = "0003-000000000001-000000002";     // Combinação da chave primária composta: CODEMP-NUMCON-SUBCON
                            }
                            else if (decVO.Titular.Lotacao.Equals("CEA CELETISTA") || decVO.Titular.Lotacao.Equals("CCLT"))
                            {
                                dpdCategoria.SelectedValue = "0003-000000000001-000000001";     // Combinação da chave primária composta: CODEMP-NUMCON-SUBCON
                            }
                            else
                            {
                                dpdCategoria.SelectedValue = "0003-000000000001-000000001";     // Combinação da chave primária composta: CODEMP-NUMCON-SUBCON
                            }

                            SelecionarCategoria();
                        }
                    }
                    else if (decVO.Empresa == PDados.Empresa.AMAZONASGT)
                    {
                    }
                
                }

                DateTime dtInicio = decVO.InicioPlano;
                if (dtInicio == DateTime.MinValue)
                    dtInicio = DateTime.Now.AddMonths(1);
                txtDataInicio.Text = "01/" + dtInicio.Month.ToString("00") + "/" + dtInicio.Year.ToString();
            }
            
        }

        protected void btnIntegrar_Click(object sender, EventArgs e)
        {
            // Nova integração
            PIntegracaoAdesaoVO integracao = null;

            try
            {
                DateTime dtInicio;
                string cdCategoria = "";
                string plano = hidPlano.Value;
                string cdMotivoDesligamentoFamilia = "";
                string cdMotivoDesligamentoUsuario = "";
                string tpCarencia = dpdCarencia.SelectedValue;

                #region[VALIDAÇÃO DA TELA]

                // Validação dos campos da tela da integração
                if (!DateTime.TryParse(txtDataInicio.Text, out dtInicio))
                {
                    this.ShowError("Informe uma data de início válida!");
                    return;
                }
                if (string.IsNullOrEmpty(dpdCategoria.SelectedValue))
                {
                    this.ShowError("Informe o subcontrato para integração!");
                    return;
                }
                else {
                    cdCategoria = dpdCategoria.SelectedValue;
                }
                if (string.IsNullOrEmpty(plano))
                {
                    this.ShowError("Plano não selecionado!");
                    return;
                }
                if (RequireMotivoDesligamento)
                {
                    if (string.IsNullOrEmpty(dpdMotivoDesligamentoFamilia.SelectedValue))
                    {
                        this.ShowError("Para os beneficiários em questão, será necessário informar o motivo de desligamento (família) do plano anterior.");
                        return;
                    }
                    cdMotivoDesligamentoFamilia = dpdMotivoDesligamentoFamilia.SelectedValue;

                    if (string.IsNullOrEmpty(dpdMotivoDesligamentoUsuario.SelectedValue))
                    {
                        this.ShowError("Para os beneficiários em questão, será necessário informar o motivo de desligamento (usuário) do plano anterior.");
                        return;
                    }
                    cdMotivoDesligamentoUsuario = dpdMotivoDesligamentoUsuario.SelectedValue;
                }
                if (string.IsNullOrEmpty(tpCarencia))
                {
                    this.ShowError("Informe o tipo de carência!");
                    return;
                }

                #endregion

                // Valida e retorna uma nova integração
                integracao = PAdesaoBO.Instance.ValidarIntegracao(Id, dtInicio, cdCategoria, plano, cdMotivoDesligamentoFamilia, cdMotivoDesligamentoUsuario, tpCarencia);

            }
            catch (EvidaException ex)
            {
                this.ShowError("Não é possível realizar a integração para este formulário: ", ex);
                return;
            }
            catch (Exception ex)
            {
                this.ShowError("Erro na validação da integração!", ex);
                return;
            }

            if (integracao != null)
            {
                try
                {
                    // Realiza a integração
                    PAdesaoBO.Instance.RealizarIntegracao(integracao, UsuarioLogado.Id);

                    #region[ATUALIZAÇÃO DA TELA]

                    // Atualiza a tela
                    this.ShowInfo("Integração realizada com sucesso!");
                    btnIntegrar.Visible = false;
                    txtDataInicio.Enabled = false;
                    btnVoltar.Visible = true;

                    //litIdBeneficiario.Text = integracao.UsuarioTitular.Matant.ToString();
                    lblIntegrada.Text = "A proposta foi integrada em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    if (integracao.Declaracao.Dependentes != null)
                    {
                        gdvDependentes.DataSource = integracao.Declaracao.Dependentes;
                        gdvDependentes.DataBind();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    this.ShowError("Erro ao realizar integração!", ex);
                    return;
                }
            }
        }

        protected void gdvDependentes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                PDeclaracaoDependenteVO vo = (PDeclaracaoDependenteVO)row.DataItem;

                Literal litIdBeneficiario = (Literal)row.FindControl("litIdBeneficiario");
                Literal litIdDependente = (Literal)row.FindControl("litIdDependente");
                Literal litParentesco = (Literal)row.FindControl("litParentesco");
                litIdBeneficiario.Text = vo.UsuarioDependente.Matric != "" ? vo.UsuarioDependente.Matant : "(NOVO)";
                litIdDependente.Text = vo.VidaDependente.Matvid != "" ? vo.VidaDependente.Nomusr : "(NOVO)";
                litParentesco.Text = vo.Parentesco.Descricao;
            }
        }

        protected void dpdCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelecionarCategoria();
        }

        private void SelecionarCategoria() {

            RequireMotivoDesligamento = false;

            if (!string.IsNullOrEmpty(dpdCategoria.SelectedValue))
            {
                String[] array_categoria = dpdCategoria.SelectedValue.Split('-');
                String codemp = array_categoria[0];
                String conemp = array_categoria[1];
                String subcon = array_categoria[2];

                // Obtém a declaração atual
                PDeclaracaoVO decVO = PAdesaoBO.Instance.GetById(Id);
                if (decVO != null)
                {

                    #region[VERIFICA SE EXISTE UMA FAMÍLIA COM ESTE CONTRATO BLOQUEADA NO PROTHEUS]

                    // Busca uma Família com este contrato no Protheus
                    PFamiliaVO familiaCompativel = PFamiliaBO.Instance.GetByContratoTitular(decVO.Empresa, decVO.Titular.Matemp, decVO.Titular, conemp, subcon);
                    if (familiaCompativel != null)
                    {
                        // Se a família está bloqueada
                        if (!string.IsNullOrEmpty(familiaCompativel.Motblo.Trim()))
                        {
                            RequireMotivoDesligamento = true;
                        }
                    }

                    #endregion

                    #region[VERIFICA SE EXISTE UMA FAMÍLIA COM OUTRO CONTRATO DESBLOQUEADA NO PROTHEUS]

                    // Busca a Família vigente no Protheus
                    PFamiliaVO familiaAtual = PFamiliaBO.Instance.GetAtualByMatriculaTitular(decVO.Empresa, decVO.Titular.Matemp, decVO.Titular);
                    if (familiaAtual != null && (familiaAtual.Conemp.Trim() != conemp.Trim() || familiaAtual.Subcon.Trim() != subcon.Trim()))
                    {
                        RequireMotivoDesligamento = true;
                    }

                    #endregion

                }

            }

            if (!RequireMotivoDesligamento)
            {
                dpdMotivoDesligamentoFamilia.SelectedValue = "";
                dpdMotivoDesligamentoUsuario.SelectedValue = "";
            }

            dpdMotivoDesligamentoFamilia.Enabled = RequireMotivoDesligamento;
            dpdMotivoDesligamentoUsuario.Enabled = RequireMotivoDesligamento;
            litMensagemPlanoExistenteFamilia.Visible = RequireMotivoDesligamento;
            litMensagemPlanoExistenteUsuario.Visible = RequireMotivoDesligamento;
            litMensagemSemPlanoExistenteFamilia.Visible = !RequireMotivoDesligamento;
            litMensagemSemPlanoExistenteUsuario.Visible = !RequireMotivoDesligamento;        
        }

    }
}