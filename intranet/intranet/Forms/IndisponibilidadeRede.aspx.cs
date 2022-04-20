using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class IndisponibilidadeRede : FormPageBase
    {
        #region[Variáveis]

        protected override Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.INDISPONIBILIDADE_REDE; }
        }

        public int? Id
        {
            get { return ViewState["ID"] == null ? new int?() : Convert.ToInt32(ViewState["ID"]); }
            set { ViewState["ID"] = value; }
        }

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

        #endregion

        #region[Load]

        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<EspecialidadeVO> lstEspecialidade = IndisponibilidadeRedeBO.Instance.ListarEspecialidades();
                dpdEspecialidade.DataSource = lstEspecialidade;
                dpdEspecialidade.DataBind();
                dpdEspecialidade.Items.Insert(0, new ListItem("SELECIONE", ""));

                IEnumerable<HcBancoVO> lstBanco = PLocatorDataBO.Instance.ListarBancoISA().Select(x => new HcBancoVO()
                {
                    Id = x.Id,
                    Nome = "(" + x.Id.ToString("0000") + ") " + x.Nome
                });
                dpdBanco.DataSource = lstBanco.OrderBy(x => x.Nome);
                dpdBanco.DataBind();
                dpdBanco.Items.Insert(0, new ListItem("SELECIONE", ""));

                IEnumerable<Constantes.Uf> lstUf = LocatorDataBO.Instance.ListarUf();
                dpdUf.DataSource = lstUf;
                dpdUf.DataBind();
                dpdUf.Items.Insert(0, new ListItem("SELECIONE", ""));

                dpdAvalAutorizacao.Items.Add(new ListItem("SELECIONE", ""));
                dpdAvalDiretoria.Items.Add(new ListItem("SELECIONE", ""));
                dpdAvalFaturamento.Items.Add(new ListItem("SELECIONE", ""));

                foreach (AvalIndisponibilidadeRede aval in IndisponibilidadeRedeEnumTradutor.AVAL_AUTORIZACAO)
                {
                    dpdAvalAutorizacao.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzAval(aval), aval.ToString()));
                }
                foreach (AvalIndisponibilidadeRede aval in IndisponibilidadeRedeEnumTradutor.AVAL_DIRETORIA)
                {
                    dpdAvalDiretoria.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzAval(aval), aval.ToString()));
                }
                foreach (AvalIndisponibilidadeRede aval in IndisponibilidadeRedeEnumTradutor.AVAL_FATURAMENTO)
                {
                    dpdAvalFaturamento.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzAval(aval), aval.ToString()));
                }

                dpdSetor.Items.Add(new ListItem("SELECIONE", ""));
                foreach (object obj in Enum.GetValues(typeof(EncaminhamentoIndisponibilidadeRede)))
                {
                    // Os setores REGIONAL e FATURAMENTO foram substituídos, portanto não poderão mais receber direcionamentos de protocolos (SGA 15038448)
                    EncaminhamentoIndisponibilidadeRede setorDestino = (EncaminhamentoIndisponibilidadeRede)obj;
                    if (setorDestino != EncaminhamentoIndisponibilidadeRede.REGIONAL && setorDestino != EncaminhamentoIndisponibilidadeRede.FATURAMENTO)
                        dpdSetor.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(setorDestino), setorDestino.ToString()));
                }

                if (!string.IsNullOrEmpty(Request["ID"]))
                {
                    int id = 0;
                    if (!Int32.TryParse(Request["ID"], out id))
                    {
                        this.ShowError("Identificador de requisição inexistente!");
                        return;
                    }

                    IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(id);
                    if (vo == null)
                    {
                        this.ShowError("Identificador de requisição inválido!");
                        return;
                    }

                    Bind(vo);

                }
                else
                {
                    btnSalvarSolicitacao.Visible = true;
                    pnlAnexos.Visible = false;
                }
            }

        }

        private void Bind(int id)
        {
            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(id);
            Bind(vo);
        }

        private void Bind(IndisponibilidadeRedeVO vo)
        {
            Id = vo.Id;
            BindCabecalho(vo);

            BindDadosSolicitacao(vo);

            CarregarArquivos();

            BindCredenciamento(vo);

            BindFinanceiro(vo);

            BindDiretoria(vo);

            BindAutorizacao(vo);

            BindFaturamento(vo);

            BindCadastro(vo);

            CheckPermissions(vo);
        }

        private void BindCabecalho(IndisponibilidadeRedeVO vo)
        {
            lblProtocolo.Text = vo.Id.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO);
            lblProtocoloANS.Text = vo.ProtocoloAns;
            lblDataSolicitacao.Text = vo.DataCriacao.ToShortDateString();
            lblSituacao.Text = IndisponibilidadeRedeEnumTradutor.TraduzStatus(vo.Situacao);
            if (vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO)
                lblSituacao.Text += " - " + vo.MotivoEncerramento;

            lblPrazo.Text = vo.DiasPrazo + " dias";// - " + .ToShortDateString();

            lblAtraso.Visible = false;
            DateTime dtLimite = DateTime.Now.Date;
            if (vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO)
            {
                dtLimite = vo.DataSituacao.Date;
            }
            if (vo.DataCriacao.AddDays(vo.DiasPrazo) < dtLimite)
            {
                lblAtraso.Visible = true;
            }

            if (lblAtraso.Text.Trim() == "0" || vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO) lblAtraso.Visible = false;

            lblSetor.Text = IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(vo.SetorEncaminhamento);
            lblResponsavel.Text = "- NENHUM - ";
            if (vo.CodUsuarioAtuante != null)
            {
                UsuarioVO resp = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAtuante.Value);
                lblResponsavel.Text = resp.Nome + " (" + resp.Login + ")";
                lblResponsavel.ToolTip = string.Empty;
            }
            else
            {
                lblResponsavel.ToolTip = "Nenhum usuário do setor assumiu na etapa atual.";
            }
        }

        private void BindDadosSolicitacao(IndisponibilidadeRedeVO vo)
        {
            PUsuarioVO titular = PUsuarioBO.Instance.GetUsuario(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);
            txtCartao.Text = titular.Matant;

            CarregarBeneficiarios(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric);

            string cd_usuario = vo.Usuario.Codint.Trim() + "|" + vo.Usuario.Codemp.Trim() + "|" + vo.Usuario.Matric.Trim() + "|" + vo.Usuario.Tipreg.Trim();

            dpdBeneficiario.SelectedValue = cd_usuario;
            txtEmail.Text = vo.EmailContato;
            txtTelContato.Text = vo.TelefoneContato;

            dpdEspecialidade.SelectedValue = vo.IdEspecialidade.ToString();
            CarregarPrioridades();
            dpdPrioridade.SelectedValue = vo.Prioridade.ToString();

            if (vo.CpfCnpjCred != null)
            {
                string strCpfCnpj = vo.CpfCnpjCred.ToString();
                if (strCpfCnpj.Length > 11)
                    txtCpfCnpj.Text = FormatUtil.FormatCnpj(strCpfCnpj);
                else
                    txtCpfCnpj.Text = FormatUtil.FormatCpf(strCpfCnpj);
            }
            else
            {
                txtCpfCnpj.Text = "";
            }

            txtRazaoSocial.Text = vo.RazaoSocialCred;

            txtEnderecoPrestador.Text = vo.EnderecoPrestador;
            txtTelefonePrestador.Text = vo.TelefonePrestador;

            if (vo.ValorSolicitacao != null)
            {
                txtValor.Text = FormatUtil.FormatDecimalForm(vo.ValorSolicitacao);
            }

            dpdUf.SelectedValue = vo.Uf;
            dpdUf_SelectedIndexChanged(dpdUf, EventArgs.Empty);
            dpdMunicipio.SelectedValue = vo.IdLocalidade != null ? vo.IdLocalidade.ToString() : string.Empty;

            txtTratativaGuia.Text = vo.TratativaGuiaMedico;
            txtTratativaReciprocidade.Text = vo.TratativaReciprocidade;

            if (vo.DataAtendimento != null)
            {
                txtDataAtendimento.Text = DateTime.Parse(vo.DataAtendimento.ToString()).ToShortDateString();
                txtAtendente.Text = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAtendente.Value).Nome + " (" + UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAtendente.Value).Login + ")";
            }

            BindObs(vo);
            BindOrcamento(vo);

        }

        private void CarregarBeneficiarios(string codint, string codemp, string matric)
        {
            List<PUsuarioVO> lstUsuarios = PUsuarioBO.Instance.ListarUsuarios(codint, codemp, matric);
            // nova autorizacao, filtrar apenas usuários ativos
            if (Id == null)
            {
                lstUsuarios = new List<PUsuarioVO>(lstUsuarios);
                lstUsuarios.RemoveAll(x => (x.Datblo != "        " && Int32.Parse(x.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))) && x.Tipusu != "T");
            }
            dpdBeneficiario.DataSource = lstUsuarios;
            dpdBeneficiario.DataBind();
            dpdBeneficiario.Items.Insert(0, new ListItem("SELECIONE", ""));
        }

        private void CarregarPrioridades()
        {
            dpdPrioridade.Items.Clear();
            if (!string.IsNullOrEmpty(dpdEspecialidade.SelectedValue))
            {
                EspecialidadeVO vo = IndisponibilidadeRedeBO.Instance.GetEspecialidadeById(Int32.Parse(dpdEspecialidade.SelectedValue));

                dpdPrioridade.Items.Add(new ListItem("", "SELECIONE"));
                List<PrioridadeIndisponibilidadeRede> lstPrioridades = IndisponibilidadeRedeBO.Instance.GetPrioridadesEspecialidade(vo);
                foreach (PrioridadeIndisponibilidadeRede prioridade in lstPrioridades)
                {
                    dpdPrioridade.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzPrioridade(prioridade), prioridade.ToString()));
                }
            }
        }

        private void BindObs(IndisponibilidadeRedeVO vo)
        {
            trObsNovo.Visible = false;
            trObsEdit.Visible = true;
            List<IndisponibilidadeRedeObsVO> lst = IndisponibilidadeRedeBO.Instance.ListarObs(Id.Value);
            if (lst == null)
                lst = new List<IndisponibilidadeRedeObsVO>();
            gdvObs.DataSource = lst.Where(x => x.TipoObs == IndisponibilidadeRedeObsVO.TIPO_EXTERNO);
            gdvObs.DataBind();
            gdvObs.Visible = true;
            txtObsEdit.Text = string.Empty;

            gdvPendencia.DataSource = lst.Where(x => x.TipoObs == IndisponibilidadeRedeObsVO.TIPO_INTERNO);
            gdvPendencia.DataBind();
            gdvPendencia.Visible = true;
            txtPendenciaEdit.Text = string.Empty;
            dpdPendencia.SelectedValue = string.Empty;

            trTratativa.Visible = true;

            dpdPendencia.Items.Clear();
            dpdPendencia.Items.Add(new ListItem("[NÃO ALTERAR]", ""));
            if (HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO))
            {
                foreach (TipoPendenciaIndisponibilidadeRede tipo in Enum.GetValues(typeof(TipoPendenciaIndisponibilidadeRede)))
                {
                    if (vo.Pendencia != null && (tipo == TipoPendenciaIndisponibilidadeRede.RECEBIDO || tipo == TipoPendenciaIndisponibilidadeRede.RESOLVIDO)
                        || vo.Pendencia == null && (tipo != TipoPendenciaIndisponibilidadeRede.RECEBIDO && tipo != TipoPendenciaIndisponibilidadeRede.RESOLVIDO))
                        dpdPendencia.Items.Add(CreateListItem(tipo));
                }
            }
            if (dpdPendencia.Items.Count == 1)
            {
                dpdPendencia.Enabled = false;
            }
            else
            {
                dpdPendencia.Enabled = true;
            }
        }

        private ListItem CreateListItem(TipoPendenciaIndisponibilidadeRede tipo)
        {
            return new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzTipoPendencia(tipo), ((int)tipo).ToString());
        }

        private void BindOrcamento(IndisponibilidadeRedeVO vo)
        {
            List<IndisponibilidadeRedeOrcamentoVO> lstOrcamento = IndisponibilidadeRedeBO.Instance.ListarOrcamento(vo.Id);
            IndisponibilidadeRedeOrcamentoVO orc1 = null;
            IndisponibilidadeRedeOrcamentoVO orc2 = null;
            IndisponibilidadeRedeOrcamentoVO orc3 = null;
            if (lstOrcamento != null)
            {
                if (lstOrcamento.Count > 0)
                    orc1 = lstOrcamento[0];
                if (lstOrcamento.Count > 1)
                    orc2 = lstOrcamento[1];
                if (lstOrcamento.Count > 2)
                    orc3 = lstOrcamento[2];
            }
            txtOrcCnpj1.Text = orc1 != null ? orc1.CpfCnpj : string.Empty;
            txtOrcEmail1.Text = orc1 != null ? orc1.Email : string.Empty;
            txtOrcNome1.Text = orc1 != null ? orc1.NomePrestador : string.Empty;
            txtOrcValor1.Text = orc1 != null ? FormatUtil.FormatDecimalForm(orc1.Valor) : string.Empty;
            txtOrcTel1.Text = orc1 != null ? orc1.Telefone : string.Empty;

            txtOrcCnpj2.Text = orc2 != null ? orc2.CpfCnpj : string.Empty;
            txtOrcEmail2.Text = orc2 != null ? orc2.Email : string.Empty;
            txtOrcNome2.Text = orc2 != null ? orc2.NomePrestador : string.Empty;
            txtOrcValor2.Text = orc2 != null ? FormatUtil.FormatDecimalForm(orc2.Valor) : string.Empty;
            txtOrcTel2.Text = orc2 != null ? orc2.Telefone : string.Empty;

            txtOrcCnpj3.Text = orc3 != null ? orc3.CpfCnpj : string.Empty;
            txtOrcEmail3.Text = orc3 != null ? orc3.Email : string.Empty;
            txtOrcNome3.Text = orc3 != null ? orc3.NomePrestador : string.Empty;
            txtOrcValor3.Text = orc3 != null ? FormatUtil.FormatDecimalForm(orc3.Valor) : string.Empty;
            txtOrcTel3.Text = orc3 != null ? orc3.Telefone : string.Empty;

        }

        private void CarregarArquivos()
        {
            dvArquivos.Visible = true;

            List<ArquivoTelaVO> lstBefore = Arquivos;

            List<IndisponibilidadeRedeArquivoVO> lstArquivos = IndisponibilidadeRedeBO.Instance.ListarArquivos(Id.Value);
            if (lstArquivos == null)
                lstArquivos = new List<IndisponibilidadeRedeArquivoVO>();

            List<ArquivoTelaVO> lstArqs = lstArquivos.Select(x => ArquivoVO2Tela(x)).ToList();
            /*
            if (lstBefore != null) {
                foreach (ArquivoTelaVO arqTela in lstArqs) {
                    ArquivoTelaVO oldVO = lstBefore.Find(x => x.NomeTela.Equals(arqTela.NomeTela, StringComparison.InvariantCultureIgnoreCase));
                    if (oldVO != null) {
                        arqTela.IsNew = oldVO.IsNew;
                    }
                }
            }*/
            Arquivos = lstArqs;

            BindArquivos();
        }

        private static ArquivoTelaVO ArquivoVO2Tela(IndisponibilidadeRedeArquivoVO x)
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

        private void BindArquivos()
        {
            List<ArquivoTelaVO> lstArqs = Arquivos;

            ltvArquivo.DataSource = lstArqs.Where(x => x.Parameters["TP_ARQUIVO"].Equals(((int)TipoArquivoIndisponibilidadeRede.BENEFICIARIO).ToString())
                || x.Parameters["TP_ARQUIVO"].Equals(((int)TipoArquivoIndisponibilidadeRede.CREDENCIAMENTO).ToString()));
            ltvArquivo.DataBind();

            updArquivos.Update();
            btnIncluirArquivo.Visible = true;
        }

        private void BindCredenciamento(IndisponibilidadeRedeVO vo)
        {
            dpdBanco.SelectedValue = vo.Banco;
            txtAgencia.Text = vo.Agencia;
            txtConta.Text = vo.ContaCorrente;
            txtValorFinanceiro.Text = FormatUtil.FormatDecimalForm(vo.ValorFinanceiro);
            txtFavorecido.Text = vo.Favorecido;
            txtCodigoServico.Text = vo.CodigoServicoFinanceiro;

            if (vo.AvalFaturamento != null)
                dpdAvalFaturamento.SelectedValue = vo.AvalFaturamento.Value.ToString();
            else
                dpdAvalFaturamento.SelectedValue = string.Empty;

            CarregarHistorico();
            CarregarHorasSetores();
        }

        private void CarregarHistorico()
        {
            List<IndisponibilidadeRedeHistoricoVO> lstHistorico = IndisponibilidadeRedeBO.Instance.ListarHistorico(Id.Value);
            gdvHistorico.DataSource = lstHistorico;
            gdvHistorico.DataBind();
        }

        protected void gdvHistorico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GridViewRow row = e.Row;
                    IndisponibilidadeRedeHistoricoVO vo = (IndisponibilidadeRedeHistoricoVO)row.DataItem;

                    TableCell cellSituacao = row.Cells[1];
                    TableCell cellAcao = row.Cells[2];

                    cellSituacao.Text = IndisponibilidadeRedeEnumTradutor.TraduzStatus(vo.StatusDestino);

                    string acao = "";
                    if (vo.StatusDestino == StatusIndisponibilidadeRede.ABERTO)
                    {
                        acao = "ABERTO";
                    }
                    else if (vo.StatusDestino == StatusIndisponibilidadeRede.ENCERRADO)
                    {
                        acao = "ENCERRADO POR " + UsuarioBO.Instance.GetUsuarioById(vo.CodUsuario.Value).Nome;
                    }
                    else if (vo.CodUsuario == null)
                    {
                        acao = "ENCAMINHADO PARA " + IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(vo.Setor);
                    }
                    else
                    {
                        acao = "ASSUMIDO POR " + UsuarioBO.Instance.GetUsuarioById(vo.CodUsuario.Value).Nome;
                    }
                    cellAcao.Text = acao;
                }
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao mostra grid de histórico.", ex);
            }
        }

        private void CarregarHorasSetores()
        {
            List<IndisponibilidadeRedeHorasSetorVO> lstHorasSetor = IndisponibilidadeRedeBO.Instance.ListarHorasSetor(Id.Value);
            gdvHorasSetor.DataSource = lstHorasSetor;
            gdvHorasSetor.DataBind();
        }

        protected void gdvHorasSetor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GridViewRow row = e.Row;
                    IndisponibilidadeRedeHorasSetorVO vo = (IndisponibilidadeRedeHorasSetorVO)row.DataItem;

                    TableCell cellSetor = row.Cells[0];
                    TableCell cellHoras = row.Cells[1];

                    cellSetor.Text = IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(vo.Setor);
                    cellHoras.Text = DateUtil.FormatHours(vo.Horas);
                }
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao mostrar grid de horas por setor.", ex);
            }
        }

        private void BindFinanceiro(IndisponibilidadeRedeVO vo)
        {
            txtObsFinanceiro.Text = vo.ObservacaoFinanceiro;
            txtObsFinanceiro2.Text = vo.ObservacaoFinanceiroBaixa;
            txtValorExecucao.Text = FormatUtil.FormatDecimalForm(vo.ValorExecucao);
            txtDataExecucao.Text = FormatUtil.FormatDataForm(vo.DataExecucao);

            lblStatusFinanceiro.Text = vo.SituacaoFinanceiro == null ? string.Empty : IndisponibilidadeRedeEnumTradutor.TraduzStatusFinanceiro(vo.SituacaoFinanceiro.Value);

            tbComprovante.Visible = false;
            if (Arquivos != null)
            {
                ArquivoTelaVO arqVO = Arquivos.FirstOrDefault(x => x.Parameters["TP_ARQUIVO"].Equals(((int)TipoArquivoIndisponibilidadeRede.COMPROVANTE).ToString()));
                if (arqVO != null)
                {
                    hidComprovante.Value = arqVO.Id.ToString();
                    ltComprovante.Text = arqVO.NomeTela;
                    tbComprovante.Visible = true;
                }
            }

            if (vo.SituacaoFinanceiro != SituacaoFinanceiroIndisponibilidadeRede.EXECUTAR_COBRANCA)
            {
                if (vo.ProtocoloFaturamento != null)
                {
                    txtObsFinanceiro.Enabled = false;
                    txtObsFinanceiro2.Enabled = true;
                }
                else
                {
                    txtObsFinanceiro.Enabled = true;
                    txtObsFinanceiro2.Enabled = false;
                }

                txtDataExecucao.Enabled = false;
                txtValorExecucao.Enabled = false;
                btnExecutarCobranca.Visible = false;
                btnSalvarFinanceiro.Visible = true;
            }
            else
            {
                txtObsFinanceiro.Enabled = txtObsFinanceiro2.Enabled = false;
                btnIncluirComprovante.Visible = false;

                txtDataExecucao.Enabled = true;
                txtValorExecucao.Enabled = true;
                btnExecutarCobranca.Visible = true;
                btnSalvarFinanceiro.Visible = false;
            }


        }

        private void BindDiretoria(IndisponibilidadeRedeVO vo)
        {
            if (vo.AvalDiretoria != null)
                dpdAvalDiretoria.SelectedValue = vo.AvalDiretoria.Value.ToString();
            else
                dpdAvalDiretoria.SelectedValue = string.Empty;
            txtComplementoDiretoria.Text = vo.ComplementoDiretoria;
        }

        private void BindAutorizacao(IndisponibilidadeRedeVO vo)
        {
            if (vo.AvalAutorizacao != null)
                dpdAvalAutorizacao.SelectedValue = vo.AvalAutorizacao.Value.ToString();
            else
                dpdAvalAutorizacao.SelectedValue = string.Empty;

            txtComplementoAutorizacao.Text = vo.ComplementoAutorizacao;

            if (vo.Acompanhante != null)
                dpdAcompanhante.SelectedValue = Convert.ToBoolean(vo.Acompanhante) ? "S" : "N";
            else
                dpdAcompanhante.SelectedValue = string.Empty;
        }

        private void BindFaturamento(IndisponibilidadeRedeVO vo)
        {
            txtProtocoloFaturamento.Text = vo.ProtocoloFaturamento == null ? string.Empty : vo.ProtocoloFaturamento.ToString();
            litResponsavelFaturamento.Text = vo.CodUsuarioFaturamento == null ? string.Empty : UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioFaturamento.Value).Login;
            txtObsFaturamento.Text = vo.ObservacaoFaturamento;
        }

        private void BindCadastro(IndisponibilidadeRedeVO vo)
        {
            txtObsCadastro.Text = vo.ObservacaoCadastro;
        }

        private void CheckPermissions(IndisponibilidadeRedeVO vo)
        {
            #region[Permissões]

            EncaminhamentoIndisponibilidadeRede setor = vo.SetorEncaminhamento;

            bool hasCredenciamento = false;
            bool hasFinanceiro = false;
            bool hasDiretoria = false;
            bool hasFaturamento = false;
            bool hasAutorizacao = false;
            bool hasRegional = false;
            bool hasCadastro = false;
            bool hasBeneficiario = false;

            // Flag que indica que está assumido pelo usuário atual
            bool assumido = false;

            hasCredenciamento = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO);
            hasFinanceiro = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_FINANCEIRO);
            hasDiretoria = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_DIRETORIA);
            hasFaturamento = hasPermissionFaturamento();
            hasAutorizacao = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_AUTORIZACAO);
            hasRegional = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE);
            hasCadastro = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CADASTRO);
            hasBeneficiario = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_BENEFICIARIO);

            #endregion

            #region[Visualização dos botões]

            btnAssumir.Visible = false;

            // Se ainda não foi encerrado
            if (vo.Situacao != StatusIndisponibilidadeRede.ENCERRADO)
            {

                // Verifica se o usuário já assumiu
                assumido = vo.CodUsuarioAtuante != null && vo.CodUsuarioAtuante.Value == UsuarioLogado.Id;

                // Se ninguém assumiu
                if (vo.CodUsuarioAtuante == null)
                {
                    // Se o usuário logado tem permissão neste setor, pode assumir
                    btnAssumir.Visible = IsSameSetor(setor);
                    // Se o usuário logado é SUPER
                }
                else if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SUPER))
                {
                    // Se o usuário logado ainda não assumiu, pode assumir
                    btnAssumir.Visible = vo.CodUsuarioAtuante.Value != UsuarioLogado.Id;
                }
            }

            // O usuário logado só pode encaminhar se ele tiver assumido
            btnEncaminhar.Visible = assumido;

            // Só pode encaminhar se o botão estiver habilitado
            // Pescuma pediu em 14/02/2019 que todos os setores pudessem encaminhar para todos os setores
            //dpdSetor.Visible = btnEncaminhar.Visible && setor == EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO;
            dpdSetor.Visible = btnEncaminhar.Visible;

            btnSalvarSolicitacao.Visible = IsCredenciamentoOrRegional(setor, hasCredenciamento, hasRegional) && assumido;
            btnSalvarObs.Visible = IsCredenciamentoOrRegional(setor, hasCredenciamento, hasRegional) && assumido;
            btnSalvarPendencia.Visible = (IsCredenciamentoOrRegional(setor, hasCredenciamento, hasRegional) || (setor == EncaminhamentoIndisponibilidadeRede.BENEFICIARIO && hasBeneficiario)) && assumido;
            btnSalvarAutorizacao.Visible = setor == EncaminhamentoIndisponibilidadeRede.AUTORIZACAO && hasAutorizacao && assumido;
            btnSalvarDiretoria.Visible = setor == EncaminhamentoIndisponibilidadeRede.DIRETORIA && hasDiretoria && assumido;
            btnSalvarFaturamento.Visible = isFaturamento(setor) && hasFaturamento && assumido;
            btnSalvarFinanceiro.Visible = !txtDataExecucao.Enabled && setor == EncaminhamentoIndisponibilidadeRede.FINANCEIRO && hasFinanceiro && assumido;
            btnExecutarCobranca.Visible = txtDataExecucao.Enabled && setor == EncaminhamentoIndisponibilidadeRede.FINANCEIRO && hasFinanceiro && assumido;
            btnSalvarCadastro.Visible = setor == EncaminhamentoIndisponibilidadeRede.CADASTRO && hasCadastro && assumido;

            // Teddy pediu em 02/05/2018 que o campo anexo estivesse habilitado para todos os setores
            //btnIncluirArquivo.Visible = (IsCredenciamentoOrRegional(setor, hasCredenciamento, hasRegional) || (setor == EncaminhamentoIndisponibilidadeRede.CADASTRO && hasCadastro)) && assumido;
            btnIncluirArquivo.Visible = (IsCredenciamentoOrRegional(setor, hasCredenciamento, hasRegional)
                || (setor == EncaminhamentoIndisponibilidadeRede.AUTORIZACAO && hasAutorizacao)
                || (setor == EncaminhamentoIndisponibilidadeRede.DIRETORIA && hasDiretoria)
                || (isFaturamento(setor) && hasFaturamento)
                || (setor == EncaminhamentoIndisponibilidadeRede.FINANCEIRO && hasFinanceiro)
                || (setor == EncaminhamentoIndisponibilidadeRede.CADASTRO && hasCadastro)
                || (setor == EncaminhamentoIndisponibilidadeRede.BENEFICIARIO && hasBeneficiario)
                ) && assumido;

            btnIncluirComprovante.Visible = !txtDataExecucao.Enabled && setor == EncaminhamentoIndisponibilidadeRede.FINANCEIRO && hasFinanceiro && assumido;
            btnRemoverComprovante.Visible = setor == EncaminhamentoIndisponibilidadeRede.FINANCEIRO && hasFinanceiro && assumido;

            btnSalvarDadosBancarios.Visible = IsCredenciamentoOrRegional(setor, hasCredenciamento, hasRegional) && assumido;

            foreach (ListViewItem item in ltvArquivo.Items)
            {
                if (item.ItemType == ListViewItemType.DataItem)
                {
                    ImageButton btnRemoverArquivo = (ImageButton)item.FindControl("btnRemoverArquivo");
                    btnRemoverArquivo.Visible = setor == EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO && hasCredenciamento && assumido;
                }
            }
            
            #endregion

            #region[Visualização dos painéis]

            pnlAnexos.Visible = true;
            pnlSolicitacao.Visible = true;
            pnlCredenciamento.Visible = true;

            if (hasCredenciamento || hasFinanceiro || hasDiretoria || hasFaturamento || hasAutorizacao || hasRegional || hasCadastro || hasBeneficiario)
            {
                pnlAutorizacao.Visible = true;
                pnlDiretoria.Visible = true;
                pnlFaturamento.Visible = true;
                pnlFinanceiro.Visible = true;
                pnlCadastro.Visible = true;
            }
            else
            {
                this.ShowError("Você não possui nenhuma permissão especial de Indisponibilidade de Rede. (CREDENCIAMENTO, FINANCEIRO, DIRETORIA, FATURAMENTO, AUTORIZACAO, CADASTRO, REGIONAL, BENEFICIÁRIO)");
            }

            #endregion

            #region[Habilitação dos painéis]

            pnlSolicitacao.Enabled = true;
            pnlCredenciamento.Enabled = true;
            pnlDiretoria.Enabled = true;
            pnlAutorizacao.Enabled = true;
            pnlFaturamento.Enabled = true;
            pnlFinanceiro.Enabled = true;
            pnlCadastro.Enabled = true;

            if (!btnSalvarSolicitacao.Visible)
            {
                pnlSolicitacao.Enabled = false;
            }
            if (!btnSalvarDadosBancarios.Visible && !btnSalvarPendencia.Visible)
            {
                pnlCredenciamento.Enabled = false;
            }
            if (!btnSalvarDiretoria.Visible)
            {
                pnlDiretoria.Enabled = false;
            }
            if (!btnSalvarAutorizacao.Visible)
            {
                pnlAutorizacao.Enabled = false;
            }
            if (!btnSalvarFaturamento.Visible)
            {
                pnlFaturamento.Enabled = false;
            }
            if (!btnSalvarFinanceiro.Visible && !btnExecutarCobranca.Visible)
            {
                pnlFinanceiro.Enabled = false;
            }
            if (!btnSalvarCadastro.Visible)
            {
                pnlCadastro.Enabled = false;
            }

            #endregion
        }

        private bool isFaturamento(EncaminhamentoIndisponibilidadeRede setor)
        {
            bool flag = false;

            if (setor == EncaminhamentoIndisponibilidadeRede.FATURAMENTO ||
                setor == EncaminhamentoIndisponibilidadeRede.SEFAT_CM ||
                setor == EncaminhamentoIndisponibilidadeRede.SEFAT_RB)
            {
                flag = true;
            }

            return flag;
        }

        private bool isRegional(EncaminhamentoIndisponibilidadeRede setor)
        {
            bool flag = false;

            if (setor == EncaminhamentoIndisponibilidadeRede.REGIONAL ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_ARARAQUARA ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_BELEM ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_BOAVISTA ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_CUIABA ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_IMPERATRIZ ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_MACAPA ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_MANAUS ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_MARABA ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_PALMAS ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_PORTOVELHO ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_RIOBRANCO ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_SAOLUIS ||
                setor == EncaminhamentoIndisponibilidadeRede.PAT_TUCURUI)
            {
                flag = true;
            }

            return flag;
        }

        private bool hasPermissionFaturamento()
        {
            bool flag = false;

            if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_FATURAMENTO) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SEFAT_CM) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SEFAT_RB))
            {
                flag = true;
            }

            return flag;
        }

        private bool hasPermissionRegional()
        {
            bool flag = false;

            if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_REGIONAL) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_ARARAQUARA) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_BELEM) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_BOAVISTA) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_CUIABA) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_IMPERATRIZ) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_MACAPA) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_MANAUS) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_MARABA) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_PALMAS) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_PORTOVELHO) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_RIOBRANCO) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_SAOLUIS) ||
                UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_TUCURUI))
            {
                flag = true;
            }

            return flag;
        }

        // Verifica se o usuário logado tem permissão neste setor
        private bool IsSameSetor(EncaminhamentoIndisponibilidadeRede setor)
        {
            switch (setor)
            {
                case EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO);
                case EncaminhamentoIndisponibilidadeRede.DIRETORIA:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_DIRETORIA);
                case EncaminhamentoIndisponibilidadeRede.FINANCEIRO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_FINANCEIRO);
                case EncaminhamentoIndisponibilidadeRede.FATURAMENTO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_FATURAMENTO);
                case EncaminhamentoIndisponibilidadeRede.AUTORIZACAO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_AUTORIZACAO);
                case EncaminhamentoIndisponibilidadeRede.REGIONAL:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_REGIONAL);
                case EncaminhamentoIndisponibilidadeRede.CADASTRO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CADASTRO);
                case EncaminhamentoIndisponibilidadeRede.BENEFICIARIO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_BENEFICIARIO);
                case EncaminhamentoIndisponibilidadeRede.SEFAT_CM:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SEFAT_CM);
                case EncaminhamentoIndisponibilidadeRede.SEFAT_RB:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SEFAT_RB);
                case EncaminhamentoIndisponibilidadeRede.PAT_ARARAQUARA:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_ARARAQUARA);
                case EncaminhamentoIndisponibilidadeRede.PAT_BELEM:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_BELEM);
                case EncaminhamentoIndisponibilidadeRede.PAT_BOAVISTA:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_BOAVISTA);
                case EncaminhamentoIndisponibilidadeRede.PAT_CUIABA:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_CUIABA);
                case EncaminhamentoIndisponibilidadeRede.PAT_IMPERATRIZ:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_IMPERATRIZ);
                case EncaminhamentoIndisponibilidadeRede.PAT_MACAPA:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_MACAPA);
                case EncaminhamentoIndisponibilidadeRede.PAT_MANAUS:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_MANAUS);
                case EncaminhamentoIndisponibilidadeRede.PAT_MARABA:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_MARABA);
                case EncaminhamentoIndisponibilidadeRede.PAT_PALMAS:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_PALMAS);
                case EncaminhamentoIndisponibilidadeRede.PAT_PORTOVELHO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_PORTOVELHO);
                case EncaminhamentoIndisponibilidadeRede.PAT_RIOBRANCO:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_RIOBRANCO);
                case EncaminhamentoIndisponibilidadeRede.PAT_SAOLUIS:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_SAOLUIS);
                case EncaminhamentoIndisponibilidadeRede.PAT_TUCURUI:
                    return UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_PAT_TUCURUI);
                default:
                    throw new Exception("Setor inválido: " + setor);
            }
        }

        private bool IsCredenciamentoOrRegional(EncaminhamentoIndisponibilidadeRede setor, bool hasCredenciamento, bool hasRegional)
        {
            if (setor == EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO && hasCredenciamento) return true;
            if (isRegional(setor) && hasRegional) return true;
            return false;
        }

        private void BindCred(string idCred)
        {
            txtCpfCnpj.Enabled = false;
            txtRazaoSocial.Enabled = false;
            if (!String.IsNullOrEmpty(idCred))
            {
                PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idCred);
                txtCpfCnpj.Text = vo.Tippe == Constantes.PESSOA_JURIDICA ? (vo.Cpfcgc != null ? FormatUtil.FormatCnpj(vo.Cpfcgc) : "") : (vo.Cpfcgc != null ? FormatUtil.FormatCpf(vo.Cpfcgc) : "");
                txtRazaoSocial.Text = vo.Nome;
            }
            else
            {
                txtCpfCnpj.Text = "";
                txtRazaoSocial.Text = "";
            }
        }

        #endregion

        #region[1 - Informações Gerais]

        protected void btnAssumir_Click(object sender, EventArgs e)
        {
            try
            {
                AssumirSolicitacao();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao assumir solicitação.", ex);
            }
        }

        private void AssumirSolicitacao()
        {
            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            bool isSuper = HasPermission(Modulo.INDISPONIBILIDADE_REDE_SUPER);
            bool forcado = false;
            if (vo.CodUsuarioAtuante != null)
            {
                if (!isSuper)
                {
                    this.ShowError("A solicitação já foi assumida por outro usuário.");
                    return;
                }
                else
                {
                    forcado = true;
                    vo.SetorEncaminhamento = EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO;
                }
            }
            else if (!IsSameSetor(vo.SetorEncaminhamento))
            {
                this.ShowError("Você não possui permissão de assumir solicitações do setor: " + IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(vo.SetorEncaminhamento));
                return;
            }

            IndisponibilidadeRedeBO.Instance.AssumirSolicitacao(vo, forcado, UsuarioLogado.Usuario);
            this.ShowInfo("Você assumiu a responsabilidade atual da solicitação com sucesso! Caso necessário, realize os ajustes e encaminhe para outro setor.");
            Bind(vo.Id);
        }

        protected void btnEncaminhar_Click(object sender, EventArgs e)
        {
            try
            {
                EncaminharSolicitacao();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao encaminhar solicitação.", ex);
            }
        }

        private void EncaminharSolicitacao()
        {
            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!IsSameSetor(vo.SetorEncaminhamento))
            {
                this.ShowError("Você não possui permissão de encaminhar solicitações do setor: " + IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(vo.SetorEncaminhamento));
                return;
            }

            if (vo.CodUsuarioAtuante == null)
            {
                this.ShowError("Apenas solicitações assumidas podem ser encaminhadas!");
                return;
            }

            if (vo.CodUsuarioAtuante.Value != UsuarioLogado.Id)
            {
                this.ShowError("Apenas o usuário responsável no momento pode encaminhar a solicitação!");
                return;
            }

            if (string.IsNullOrEmpty(dpdSetor.SelectedValue))
            {
                this.ShowError("Informe o setor a ser encaminhada a solicitação!");
                return;
            }
            EncaminhamentoIndisponibilidadeRede destino = IndisponibilidadeRedeEnumTradutor.TraduzSetor(dpdSetor.SelectedValue);

            if (destino == EncaminhamentoIndisponibilidadeRede.FINANCEIRO || destino == EncaminhamentoIndisponibilidadeRede.FATURAMENTO)
            {
                AvalIndisponibilidadeRede? avalDiretoria = vo.AvalDiretoria;
                if (avalDiretoria == null || avalDiretoria != AvalIndisponibilidadeRede.DIRETORIA_DEFERIDO)
                {
                    this.ShowError("A solicitação não pode ser encaminhada ao Financeiro ou Faturamento sem o aval positivo da Diretoria!");
                    return;
                }
                if (isFaturamento(destino))
                {
                    if (vo.AvalFaturamento == null)
                    {
                        this.ShowError("Antes de encaminhar ao setor FATURAMENTO é necessário escolher o Tipo em Tratativas do Credenciamento!");
                        return;
                    }
                }
            }
            if (destino == EncaminhamentoIndisponibilidadeRede.FINANCEIRO)
            {
                if (vo.AvalFaturamento == null)
                {
                    this.ShowError("Antes de encaminhar ao setor FINANCEIRO é necessário escolher o Tipo em Tratativas do Credenciamento!");
                    return;
                }
                if (vo.SituacaoFinanceiro == null)
                {
                    vo.SituacaoFinanceiro = SituacaoFinanceiroIndisponibilidadeRede.PAGAMENTO_SOLICITADO;
                }
            }
            else if (destino == EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO && vo.SetorEncaminhamento == EncaminhamentoIndisponibilidadeRede.FINANCEIRO)
            {
                if (tbComprovante.Visible && vo.SituacaoFinanceiro == SituacaoFinanceiroIndisponibilidadeRede.PAGAMENTO_SOLICITADO)
                {
                    vo.SituacaoFinanceiro = SituacaoFinanceiroIndisponibilidadeRede.PAGAMENTO_REALIZADO;
                }
            }

            vo.SetorEncaminhamento = destino;

            IndisponibilidadeRedeBO.Instance.EncaminharSolicitacao(vo);
            this.ShowInfo("Solicitação encaminhada para o setor " + IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(destino) + " com sucesso!");
            Bind(vo.Id);
        }

        #endregion

        #region[2 - Dados da Solicitação]

        protected void txtCartao_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dpdBeneficiario.Items.Clear();
                PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
                if (titular == null)
                {
                    this.ShowError("Titular não encontrado!");
                    return;
                }

                txtEmail.Text = titular.Email;
                CarregarBeneficiarios(titular.Codint, titular.Codemp, titular.Matric);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar matrícula.", ex);
            }
        }

        protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e)
        {
            dpdMunicipio.Items.Clear();
            if (!string.IsNullOrEmpty(dpdUf.SelectedValue))
            {
                dpdMunicipio.DataSource = PLocatorDataBO.Instance.BuscarMunicipiosProtheus(dpdUf.SelectedValue);
                dpdMunicipio.DataBind();
            }
            dpdMunicipio.Items.Insert(0, new ListItem("SELECIONE", ""));
        }

        protected void dpdEspecialidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CarregarPrioridades();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao carregar prioridade!", ex);
            }
        }

        protected void btnSalvarSolicitacao_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarSolicitacao();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar!", ex);
            }
        }

        private void SalvarSolicitacao()
        {
            if (!ValidateFields())
            {
                return;
            }

            IndisponibilidadeRedeVO vo = new IndisponibilidadeRedeVO();
            if (Id != null)
            {
                vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
                if (!CheckAssumido(vo))
                {
                    return;
                }
            }

            string[] dados_usuario = dpdBeneficiario.SelectedValue.Split('|');
            string codint = dados_usuario[0];
            string codemp = dados_usuario[1];
            string matric = dados_usuario[2];
            string tipreg = dados_usuario[3];

            vo.Usuario = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
            vo.EmailContato = txtEmail.Text;
            vo.TelefoneContato = txtTelContato.Text;
            vo.Prioridade = IndisponibilidadeRedeEnumTradutor.TraduzPrioridade(dpdPrioridade.SelectedValue);
            vo.IdEspecialidade = Int32.Parse(dpdEspecialidade.SelectedValue);

            if (!string.IsNullOrEmpty(txtRazaoSocial.Text))
                vo.RazaoSocialCred = txtRazaoSocial.Text;
            if (!string.IsNullOrEmpty(txtCpfCnpj.Text))
                vo.CpfCnpjCred = Int64.Parse(FormatUtil.UnformatCnpj(txtCpfCnpj.Text));

            vo.EnderecoPrestador = txtEnderecoPrestador.Text;
            vo.TelefonePrestador = txtTelefonePrestador.Text;

            if (!string.IsNullOrEmpty(txtValor.Text))
            {
                vo.ValorSolicitacao = Decimal.Parse(txtValor.Text);
            }
            else
            {
                vo.ValorSolicitacao = null;
            }

            vo.IdLocalidade = Convert.ToInt32(dpdMunicipio.SelectedValue);
            vo.Uf = dpdUf.SelectedValue;

            vo.TratativaGuiaMedico = txtTratativaGuia.Text;
            vo.TratativaReciprocidade = txtTratativaReciprocidade.Text;

            #region[ATENDIMENTO]

            DateTime dtAtendimento;
            if (DateTime.TryParse(txtDataAtendimento.Text.Trim(), out dtAtendimento))
            {

                if (dtAtendimento != vo.DataAtendimento)
                {
                    vo.DataAtendimento = dtAtendimento;
                    vo.CodUsuarioAtendente = UsuarioLogado.Id;
                }
            }

            #endregion

            if (Id == null)
            {
                IndisponibilidadeRedeBO.Instance.CriarSolicitacao(vo, txtObsNovo.Text, IndisponibilidadeRedeObsVO.ORIGEM_INTRANET, null);
                this.ShowInfo("Solicitação criada com sucesso!");
            }
            else
            {
                vo.Id = Id.Value;
                List<IndisponibilidadeRedeOrcamentoVO> lstOrcamento = ReadOrcamento();

                IndisponibilidadeRedeBO.Instance.SalvarSolicitacao(vo, lstOrcamento);
                this.ShowInfo("Alterações em 'Dados da Solicitação' realizadas com sucesso!");
            }

            Bind(vo.Id);
        }

        private List<IndisponibilidadeRedeOrcamentoVO> ReadOrcamento()
        {
            List<IndisponibilidadeRedeOrcamentoVO> lstOrcamento = new List<IndisponibilidadeRedeOrcamentoVO>();
            IndisponibilidadeRedeOrcamentoVO orc = new IndisponibilidadeRedeOrcamentoVO();
            orc.CpfCnpj = txtOrcCnpj1.Text;
            orc.Email = txtOrcEmail1.Text;
            orc.NomePrestador = txtOrcNome1.Text;
            orc.Telefone = txtOrcTel1.Text;
            orc.Valor = string.IsNullOrEmpty(txtOrcValor1.Text) ? new decimal?() : Convert.ToDecimal(txtOrcValor1.Text);
            lstOrcamento.Add(orc);

            orc = new IndisponibilidadeRedeOrcamentoVO();
            orc.CpfCnpj = txtOrcCnpj2.Text;
            orc.Email = txtOrcEmail2.Text;
            orc.NomePrestador = txtOrcNome2.Text;
            orc.Telefone = txtOrcTel2.Text;
            orc.Valor = string.IsNullOrEmpty(txtOrcValor2.Text) ? new decimal?() : Convert.ToDecimal(txtOrcValor2.Text);
            lstOrcamento.Add(orc);

            orc = new IndisponibilidadeRedeOrcamentoVO();
            orc.CpfCnpj = txtOrcCnpj3.Text;
            orc.Email = txtOrcEmail3.Text;
            orc.NomePrestador = txtOrcNome3.Text;
            orc.Telefone = txtOrcTel3.Text;
            orc.Valor = string.IsNullOrEmpty(txtOrcValor3.Text) ? new decimal?() : Convert.ToDecimal(txtOrcValor3.Text);
            lstOrcamento.Add(orc);
            return lstOrcamento;
        }

        private bool ValidateFields()
        {
            if (!ValidateRequired())
                return false;

            List<ItemError> lst = new List<ItemError>();

            if (!string.IsNullOrEmpty(txtCpfCnpj.Text))
            {
                long cnpj = 0;
                string str = FormatUtil.UnformatCnpj(txtCpfCnpj.Text);
                if (!Int64.TryParse(str, out cnpj))
                {
                    this.AddErrorMessage(lst, txtCpfCnpj, "O CPF/CNPJ informado está em um formato inválido!");
                }
            }

            if (!string.IsNullOrEmpty(txtValor.Text))
            {
                decimal d;
                if (!Decimal.TryParse(txtValor.Text, out d))
                {
                    this.AddErrorMessage(lst, txtValor, "O valor deve ser numérico!");
                }
            }

            if (!string.IsNullOrEmpty(txtDataAtendimento.Text.Trim()))
            {
                DateTime dtAtendimento;
                if (!DateTime.TryParse(txtDataAtendimento.Text, out dtAtendimento))
                {
                    this.AddErrorMessage(lst, txtValor, "Data de atendimento inválida!");
                    this.ShowError("Data de atendimento inválida!");
                }
            }

            if (!string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                if (!FormatUtil.IsValidEmail(txtEmail.Text.Trim()))
                {
                    this.AddErrorMessage(lst, txtEmail, "Email inválido!");
                    this.ShowError("Email inválido!");
                }
            }

            if (lst.Count > 0)
            {
                this.ShowErrorList(lst);
                return false;
            }
            return true;
        }

        private bool ValidateRequired()
        {
            List<ItemError> lst = new List<ItemError>();

            if (string.IsNullOrEmpty(dpdBeneficiario.SelectedValue))
            {
                this.AddErrorMessage(lst, dpdBeneficiario, "Informe o beneficiário!");
            }
            if (string.IsNullOrEmpty(txtTelContato.Text))
            {
                this.AddErrorMessage(lst, txtTelContato, "Informe o telefone de contato!");
            }
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                this.AddErrorMessage(lst, txtEmail, "Informe o e-mail!");
            }
            if (string.IsNullOrEmpty(dpdEspecialidade.SelectedValue))
            {
                this.AddErrorMessage(lst, dpdEspecialidade, "Informe a especialidade!");
            }
            if (string.IsNullOrEmpty(dpdPrioridade.SelectedValue))
            {
                this.AddErrorMessage(lst, dpdPrioridade, "Informe a prioridade!");
            }
            if (string.IsNullOrEmpty(dpdUf.SelectedValue))
            {
                this.AddErrorMessage(lst, dpdUf, "Informe a UF!");
            }
            if (string.IsNullOrEmpty(dpdMunicipio.SelectedValue))
            {
                this.AddErrorMessage(lst, dpdMunicipio, "Informe o município!");
            }

            if (lst.Count > 0)
            {
                this.ShowErrorList(lst);
                return false;
            }

            return true;
        }

        private bool CheckAssumido(IndisponibilidadeRedeVO vo)
        {
            if (vo.CodUsuarioAtuante == null)
            {
                this.ShowError("A solicitação ainda não foi assumida. Não é possível salver alterações!");
                return false;
            }
            if (vo.CodUsuarioAtuante != UsuarioLogado.Id)
            {
                this.ShowError("A solicitação foi assumida por outro usuário. Não é possível salver alterações!");
                return false;
            }
            return true;
        }

        #endregion

        #region[3 - Anexos]

        protected void bntRemoverArquivo_Click(object sender, ImageClickEventArgs e)
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
                IndisponibilidadeRedeArquivoVO vo = ArquivoTela2VO(telaVO);
                IndisponibilidadeRedeBO.Instance.RemoverArquivo(vo);
            }
            CarregarArquivos();
        }

        private IndisponibilidadeRedeArquivoVO ArquivoTela2VO(ArquivoTelaVO telaVO)
        {
            IndisponibilidadeRedeArquivoVO vo = new IndisponibilidadeRedeArquivoVO();
            if (!string.IsNullOrEmpty(telaVO.Id))
            {
                vo.IdArquivo = Int32.Parse(telaVO.Id);
            }
            vo.IdIndisponibilidade = Id.Value;
            vo.NomeArquivo = telaVO.NomeTela;
            return vo;
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
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoIndisponibilidadeRede.CREDENCIAMENTO).ToString());
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
                SalvarAddArquivo();
            }
            else
            {
                this.ShowInfo("Arquivo adicionado em tela! Arquivos só serão salvos quando formulário enviado!");
            }
        }

        private void SalvarAddArquivo()
        {
            ArquivoTelaVO telaVO = Arquivos.Last(x => x.IsNew);
            IndisponibilidadeRedeBO.Instance.SalvarArquivo(Id.Value, telaVO);

            CarregarArquivos();
            this.ShowInfo("Arquivo salvo com sucesso no sistema!");
        }

        #endregion

        #region[4 - Tratativas do Credenciamento]

        protected void btnSalvarDadosBancarios_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarCredenciamento();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar dados bancários.", ex);
            }
        }

        private void SalvarCredenciamento()
        {

            decimal valor;

            if (string.IsNullOrEmpty(dpdBanco.SelectedValue))
            {
                this.ShowError("Informe o banco!");
                return;
            }
            if (string.IsNullOrEmpty(txtAgencia.Text))
            {
                this.ShowError("Informe a agencia!");
                return;
            }
            if (string.IsNullOrEmpty(txtConta.Text))
            {
                this.ShowError("Informe a conta!");
                return;
            }

            if (string.IsNullOrEmpty(txtFavorecido.Text))
            {
                this.ShowError("Informe o favorecido!");
                return;
            }
            if (string.IsNullOrEmpty(txtValorFinanceiro.Text))
            {
                this.ShowError("Informe o valor financeiro!");
                return;
            }

            if (!Decimal.TryParse(txtValorFinanceiro.Text, out valor))
            {
                this.ShowError("Informe um valor numérico financeiro!");
                return;
            }
            if (string.IsNullOrEmpty(dpdAvalFaturamento.SelectedValue))
            {
                this.ShowError("Informe o tipo!");
                return;
            }

            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!CheckAssumido(vo))
            {
                return;
            }

            AvalIndisponibilidadeRede aval = IndisponibilidadeRedeEnumTradutor.TraduzAval(dpdAvalFaturamento.SelectedValue);

            vo.Banco = dpdBanco.SelectedValue;
            vo.Agencia = txtAgencia.Text;
            vo.ContaCorrente = txtConta.Text;
            vo.Favorecido = txtFavorecido.Text;
            vo.ValorFinanceiro = valor;

            vo.AvalFaturamento = aval;
            vo.CodigoServicoFinanceiro = txtCodigoServico.Text;

            IndisponibilidadeRedeBO.Instance.SalvarCredenciamento(vo);
            this.ShowInfo("Dados bancários salvos com sucesso!");

            Bind(vo.Id);
        }

        protected void btnSalvarObs_Click(object sender, EventArgs e)
        {
            try
            {
                bool interno = (sender == btnSalvarPendencia);
                TipoPendenciaIndisponibilidadeRede? pendencia = null;
                if (interno)
                {
                    if (!string.IsNullOrEmpty(dpdPendencia.SelectedValue))
                    {
                        pendencia = (TipoPendenciaIndisponibilidadeRede)Convert.ToInt32(dpdPendencia.SelectedValue);
                    }
                }
                SalvarObs(interno, pendencia);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar a observação.", ex);
            }
        }

        private void SalvarObs(bool interno, TipoPendenciaIndisponibilidadeRede? pendencia)
        {
            if (Id == null)
            {
                this.ShowError("Observação deve ser inserida em um protocolo já existente.");
                return;
            }
            string texto = interno ? txtPendenciaEdit.Text : txtObsEdit.Text;
            if (string.IsNullOrEmpty(texto))
            {
                this.ShowError("Informe a observação a ser inserida!");
                return;
            }
            IndisponibilidadeRedeObsVO vo = new IndisponibilidadeRedeObsVO();
            vo.IdIndisponibilidade = Id.Value;
            vo.CodUsuario = UsuarioLogado.Id;
            vo.Observacao = texto;
            vo.Origem = IndisponibilidadeRedeObsVO.ORIGEM_INTRANET;
            vo.TipoObs = interno ? IndisponibilidadeRedeObsVO.TIPO_INTERNO : IndisponibilidadeRedeObsVO.TIPO_EXTERNO;
            vo.Pendencia = pendencia;
            IndisponibilidadeRedeBO.Instance.IncluirObs(vo);
            if (interno)
                this.ShowInfo("Pendência/Observação incluída com sucesso!");
            else
                this.ShowInfo("Observação Incluída com sucesso!");

            IndisponibilidadeRedeVO redeVO = IndisponibilidadeRedeBO.Instance.GetById(vo.IdIndisponibilidade);
            BindObs(redeVO);
        }

        #endregion

        #region[5 - Tratativas de Autorização]

        protected void btnSalvarAutorizacao_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarAutorizacao();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar dados da autorização.", ex);
            }
        }

        private void SalvarAutorizacao()
        {
            if (string.IsNullOrEmpty(dpdAvalAutorizacao.SelectedValue))
            {
                this.ShowError("Informe o aval");
                return;
            }
            AvalIndisponibilidadeRede aval = IndisponibilidadeRedeEnumTradutor.TraduzAval(dpdAvalAutorizacao.SelectedValue);
            if (aval == AvalIndisponibilidadeRede.AUTORIZACAO_COMPLEMENTO)
            {
                if (string.IsNullOrEmpty(txtComplementoAutorizacao.Text))
                {
                    this.ShowError("Informe o complemento!");
                    return;
                }
            }

            if (string.IsNullOrEmpty(dpdAcompanhante.SelectedValue))
            {
                this.ShowError("Informe se necessita ou não de acompanhante!");
                return;
            }

            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!CheckAssumido(vo))
            {
                return;
            }
            vo.AvalAutorizacao = aval;
            vo.ComplementoAutorizacao = txtComplementoAutorizacao.Text;
            vo.Acompanhante = dpdAcompanhante.SelectedValue.Equals("S");

            IndisponibilidadeRedeBO.Instance.SalvarAutorizacao(vo);
            this.ShowInfo("Aval da autorização salvo com sucesso!");
            Bind(vo.Id);
        }

        #endregion

        #region[6 - Aval do Superior Imediato]

        protected void btnSalvarDiretoria_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarDiretoria();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar dados da diretoria.", ex);
            }
        }

        private void SalvarDiretoria()
        {
            if (string.IsNullOrEmpty(dpdAvalDiretoria.SelectedValue))
            {
                this.ShowError("Informe o aval");
                return;
            }
            AvalIndisponibilidadeRede aval = IndisponibilidadeRedeEnumTradutor.TraduzAval(dpdAvalDiretoria.SelectedValue);
            if (aval == AvalIndisponibilidadeRede.DIRETORIA_COMPLEMENTO)
            {
                if (string.IsNullOrEmpty(txtComplementoDiretoria.Text))
                {
                    this.ShowError("Informe o complemento!");
                    return;
                }
            }

            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!CheckAssumido(vo))
            {
                return;
            }
            vo.AvalDiretoria = aval;
            vo.ComplementoDiretoria = txtComplementoDiretoria.Text;

            IndisponibilidadeRedeBO.Instance.SalvarDiretoria(vo);
            this.ShowInfo("Aval da diretoria salvo com sucesso!");
            Bind(vo.Id);
        }

        #endregion

        #region[7 - Tratativas do Financeiro]

        protected void btnSalvarFinanceiro_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarFinanceiro();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar dados do financeiro.", ex);
            }
        }

        private void SalvarFinanceiro()
        {
            if (txtObsFinanceiro.Enabled)
            {
                if (string.IsNullOrEmpty(txtObsFinanceiro.Text))
                {
                    this.ShowInfo("Informe algum complemento na tratativa financeira sobre o pagamento!");
                    return;
                }
            }
            if (txtObsFinanceiro2.Enabled)
            {
                if (string.IsNullOrEmpty(txtObsFinanceiro2.Text))
                {
                    this.ShowInfo("Informe algum complemento na tratativa financeira sobre a baixa!");
                    return;
                }
            }
            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!CheckAssumido(vo))
            {
                return;
            }
            vo.ObservacaoFinanceiro = txtObsFinanceiro.Text;
            vo.ObservacaoFinanceiroBaixa = txtObsFinanceiro2.Text;

            IndisponibilidadeRedeBO.Instance.SalvarFinanceiro(vo);
            this.ShowInfo("Tratativas do financeiro salvos com sucesso!");

            Bind(vo.Id);
        }

        protected void btnRemoverComprovante_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                RemoverComprovante();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover comprovante.", ex);
            }
        }

        private void RemoverComprovante()
        {
            IndisponibilidadeRedeBO.Instance.RemoverComprovante(Id.Value);
            this.ShowInfo("Comprovante removido com sucesso!");
            Bind(Id.Value);
        }

        protected void btnIncluirComprovante_Click(object sender, EventArgs e)
        {
            try
            {
                AddComprovante(hidArqFisico.Value, hidArqOrigem.Value);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir ou alterar o comprovante.", ex);
            }
        }

        private void AddComprovante(string fisico, string original)
        {
            string ext = System.IO.Path.GetExtension(original);
            List<ArquivoTelaVO> lstAtual = Arquivos;
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = "COMPROVANTE" + ext,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoIndisponibilidadeRede.COMPROVANTE).ToString());

            IndisponibilidadeRedeBO.Instance.SalvarComprovante(Id.Value, vo);

            this.ShowInfo("Comprovante salvo com sucesso no sistema!");

            Bind(Id.Value);
        }

        protected void btnExecutarCobranca_Click(object sender, EventArgs e)
        {
            try
            {
                ExecutarCobranca();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar.", ex);
            }
        }

        private void ExecutarCobranca()
        {
            DateTime dtExecucao;
            decimal vlExecucao;

            if (!DateTime.TryParse(txtDataExecucao.Text, out dtExecucao))
            {
                this.ShowError("A data de execução é obrigatório e deve ser válida!");
                return;
            }
            if (!Decimal.TryParse(txtValorExecucao.Text, out vlExecucao))
            {
                this.ShowError("O valor de execução é obrigatório e deve ser um número válido!");
                return;
            }

            IndisponibilidadeRedeBO.Instance.ExecutarCobranca(Id.Value, dtExecucao, vlExecucao);
            this.ShowError("Informações da execução da cobrança salvas com sucesso!");
            Bind(Id.Value);
        }

        #endregion

        #region[8 - Tratativas do Faturamento/Reembolso]

        protected void btnSalvarFaturamento_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarFaturamento();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar dados do faturamento/reembolso.", ex);
            }
        }

        private void SalvarFaturamento()
        {
            decimal? protocolo = null;
            if (!string.IsNullOrEmpty(txtProtocoloFaturamento.Text))
            {
                decimal l;
                if (!Decimal.TryParse(txtProtocoloFaturamento.Text, out l))
                {
                    this.ShowError("O protocolo deve ser numérico!");
                    return;
                }
                protocolo = l;
            }

            if (txtObsFaturamento.Enabled)
            {
                if (string.IsNullOrEmpty(txtObsFaturamento.Text))
                {
                    this.ShowInfo("Informe alguma observação na tratativa do faturamento!");
                    return;
                }
            }

            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!CheckAssumido(vo))
            {
                return;
            }
            if (protocolo == null)
            {
                this.ShowError("Informe o protocolo gerado!");
                return;
            }

            vo.ProtocoloFaturamento = protocolo;
            vo.ObservacaoFaturamento = txtObsFaturamento.Text;

            IndisponibilidadeRedeBO.Instance.SalvarFaturamento(vo);
            this.ShowInfo("Tratativa do faturamento/reembolso salvo com sucesso!");
            Bind(vo.Id);
        }


        #endregion

        #region[9 - Tratativas do Cadastro]

        protected void btnSalvarCadastro_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarCadastro();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar dados do cadastro.", ex);
            }
        }

        private void SalvarCadastro()
        {
            if (txtObsCadastro.Enabled)
            {
                if (string.IsNullOrEmpty(txtObsCadastro.Text))
                {
                    this.ShowInfo("Informe alguma observação na tratativa do cadastro!");
                    return;
                }
            }
            IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Id.Value);
            if (!CheckAssumido(vo))
            {
                return;
            }
            vo.ObservacaoCadastro = txtObsCadastro.Text;

            IndisponibilidadeRedeBO.Instance.SalvarCadastro(vo);
            this.ShowInfo("Tratativas do cadastro salvas com sucesso!");

            Bind(vo.Id);
        }

        #endregion

	}
}