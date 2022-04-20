using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms
{

    [Serializable]
    public class PermissaoViagemVS
    {
        public bool ChangeSolicitacao { get; set; }
        public bool Coordenador { get; set; }
        public bool Secretaria { get; set; }
        public bool Diretor { get; set; }
        public bool DiretorSolicitacao { get; set; }
        public bool DiretorChangePrestacaoConta { get; set; }
        public bool FinanceiroCompra { get; set; }
        public bool FinanceiroPrestacaoConta { get; set; }
    }

    public partial class FormViagem : FormPageBase
    {

        decimal vlTotalDespDet = 0;

        #region[LOAD]

        protected override Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.VIAGEM; }
        }

        public int? Id
        {
            get { return ViewState["ID"] == null ? new int?() : Convert.ToInt32(ViewState["ID"]); }
            set { ViewState["ID"] = value; }
        }

        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                IEnumerable<HcBancoVO> lstBanco = LocatorDataBO.Instance.ListarBancoISA().Select(x => new HcBancoVO()
                {
                    Id = x.Id,
                    Nome = "(" + x.Id.ToString("0000") + ") " + x.Nome
                });
                dpdBanco.DataSource = lstBanco.OrderBy(x => x.Nome);
                dpdBanco.DataBind();
                dpdBanco.Items.Insert(0, new ListItem("SELECIONE", ""));

                mtvViagem.ActiveViewIndex = 0;

                if (!string.IsNullOrEmpty(Request["ID"]))
                {
                    int id = 0;
                    if (!Int32.TryParse(Request["ID"], out id))
                    {
                        this.ShowError("Identificador de requisição inexistente!");
                        return;
                    }
                    Bind(id);

                }
                else
                {
                    dpdTipoSolicitacao.SelectedValue = "0";
                    ChangeTipoSolicitacao(false);

                    if (!HasPermission(eVidaGeneralLib.VO.Modulo.VIAGEM_RESPONSAVEL))
                    {
                        dpdTipoSolicitacao.Enabled = false;
                        txtMatricula.Enabled = false;

                        long? matricula = UsuarioLogado.Usuario.Matricula;
                        if (matricula == null || matricula.Value == 0)
                        {
                            this.ShowError("Seu usuário não possui matrícula E-VIDA associada. Entre em contato com suporte para atualização cadastral!");
                            btnSalvarSolicitacao.Enabled = false;
                            return;
                        }

                        EmpregadoEvidaVO empregado = EmpregadoEvidaBO.Instance.GetByMatricula(matricula.Value);
                        if (empregado == null)
                        {
                            this.ShowError("Sua matrícula [" + matricula + "] não foi encontrada no protheus!");
                            btnSalvarSolicitacao.Enabled = false;
                            return;
                        }

                        BindFuncionario(empregado);
                    }
                    else
                    {
                        dpdTipoSolicitacao.Enabled = true;
                    }
                    btnSalvarSolicitacao.Visible = true;
                    btnIncluirCurso.Visible = true;
                    tbCurso.Visible = true;
                }
            }

        }

        private void Bind(int id)
        {
            Id = id;

            SolicitacaoViagemVO vo = ViagemBO.Instance.GetById(id);
            if (vo == null)
            {
                this.ShowError("ID não encontrado!");
                return;
            }

            lblDataSolicitacao.Text = vo.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss");
            lblProtocolo.Text = vo.Id.ToString(SolicitacaoViagemVO.FORMATO_PROTOCOLO);
            lblSituacao.Text = SolicitacaoViagemEnumTradutor.TraduzStatus(vo.Situacao);
            UsuarioVO usuarioSolicitante = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioSolicitante);
            lblSolicitadoPor.Text = usuarioSolicitante.Login + " - " + usuarioSolicitante.Nome;
            lblDataSituacao.Text = vo.DataSituacao.ToString("dd/MM/yyyy HH:mm:ss");

            btnRelatorio.OnClientClick = "return openRelViagem('" + Id + "');";

            ChangeTipoSolicitacao(vo.IsExterno);
            if (vo.IsExterno)
            {
                txtMatricula.Text = string.Empty;
                txtMatricula.Enabled = false;
                txtMatricula.Visible = false;
                dpdTipoViagem.SelectedValue = string.Empty;
                dpdTipoSolicitacao.SelectedValue = "1";
            }
            else
            {
                dpdTipoSolicitacao.SelectedValue = "0";
                txtMatricula.Text = vo.Empregado.Matricula.ToString();
                txtMatricula.Enabled = HasPermission(eVidaGeneralLib.VO.Modulo.VIAGEM_RESPONSAVEL);
                txtMatricula.Visible = true;
                dpdTipoViagem.SelectedValue = vo.TipoViagem == null ? string.Empty : ((int)vo.TipoViagem.Value).ToString();
            }
            dpdTipoViagem.Enabled = false;
            dpdTipoSolicitacao.Enabled = false;
            txtMatricula.Enabled = false;

            CheckPermissions(vo);

            BindAprovacao(vo.AprovSolicitacaoCoordenador, dpdAvalSolCoordenador, txtJusSolCoordenador);
            BindAprovacao(vo.AprovSolicitacaoDiretoria, dpdAvalSolDiretoria, txtJusSolDiretoria);
            BindAprovacao(vo.AprovPrestacaoDiretoria, dpdAvalPcDiretoria, txtJusPcDiretoria);
            BindAprovacao(vo.AprovPrestacaoFinanceiro, dpdAvalPcFinanceiro, txtJusPcFinanceiro);

            BindDadosSolicitacao(vo.Solicitacao);
            BindDadosCompra(vo.Solicitacao, vo.Compra);
            BindDadosFinanceiro(vo);
            BindDadosPrestacaoConta(vo.Solicitacao, vo.Compra, vo.PrestacaoConta);

            CarregarArquivos();
        }

        private void BindAprovacao(SolicitacaoViagemAprovacaoVO aprovacaoVO, DropDownList dpdAval, TextBox txtJustificativa)
        {
            dpdAval.SelectedValue = aprovacaoVO == null ? "" : (aprovacaoVO.Aprovado ? "S" : "N");
            txtJustificativa.Text = aprovacaoVO == null ? "" : aprovacaoVO.Justificativa;
        }

        private void BindDadosSolicitacao(SolicitacaoViagemInfoSolicitacaoVO vo)
        {
            txtNomeFuncionario.Text = vo.Nome;

            txtCpf.Text = FormatUtil.FormatCpf(vo.Cpf);
            txtRg.Text = vo.Rg;
            txtDataNascimento.Text = vo.DataNascimento.ToString("dd/MM/yyyy");

            txtTelContato.Text = vo.Telefone;
            txtRamal.Text = vo.Ramal;
            txtCargo.Text = vo.Cargo;

            txtObjetivoViagem.Text = vo.Objetivo;
            dpdBanco.Text = vo.Banco != null ? vo.Banco.Id.ToString() : string.Empty;
            txtAgencia.Text = vo.Agencia;
            txtConta.Text = vo.ContaCorrente;
            txtValorAdiantamento.Text = vo.ValorAdiantamento != null ? vo.ValorAdiantamento.Value.ToString("#.00") : "";
            txtJusAdiantamento.Text = vo.JustificativaAdiantamento;

            foreach (ListItem item in chkMeioTransporte.Items)
            {
                item.Selected = (vo.MeioTransporte != null && vo.MeioTransporte.Contains((MeioTransporteViagem)Int32.Parse(item.Value)));
            }

            chkUsaDiaria.Checked = vo.UsaValorDiaria;

            gdvItinerario.DataSource = vo.Itinerarios;
            gdvItinerario.DataBind();
        }

        private void BindDadosCompra(SolicitacaoViagemInfoSolicitacaoVO infoVO, SolicitacaoViagemInfoCompraVO vo)
        {
            List<ViagemValorVariavelVO> lstValoresDiaria = ViagemHelper.CalcularDiarias(vo.Passagens);
            string strValorDiaria = ViagemHelper.GetStringValoresDiaria(lstValoresDiaria);
            lblValorDiaria.Text = strValorDiaria;

            txtValorCurso.Text = FormatUtil.FormatDecimalForm(vo.ValorCurso);

            gdvHospedagem.DataSource = vo.Hoteis;
            gdvHospedagem.DataBind();

            BindTraslado(vo.Passagens);
        }

        private void BindTraslado(List<SolicitacaoViagemItinerarioVO> passagens)
        {
            dpdMeioTransporte.Items.Clear();
            dpdMeioTransporte.Items.Add(new ListItem("SELECIONE", ""));
            foreach (MeioTransporteViagem meio in Enum.GetValues(typeof(MeioTransporteViagem)))
            {
                dpdMeioTransporte.Items.Add(new ListItem(meio.ToString(), ((int)meio).ToString()));
            }

            gdvVoo.DataSource = passagens;
            gdvVoo.DataBind();

            List<ViagemValorVariavelVO> lstValores = ViagemHelper.CalcularDiarias(passagens);
            if (lstValores.Count > 0)
            {
                decimal qtdDiarias = ViagemHelper.CalcularQtdTotalDiarias(lstValores);
                txtDiarias.Text = FormatUtil.FormatDecimalForm(qtdDiarias);
                txtValorTotalDiarias.Text = FormatUtil.FormatDecimalForm(ViagemHelper.CalcularVlrTotalDiarias(lstValores, IsUtilizaDiaria));
                lblValorDiaria.Text = ViagemHelper.GetStringValoresDiaria(lstValores);
            }
            else
            {
                txtDiarias.Text = "0";
                txtValorTotalDiarias.Text = "0";
                lblValorDiaria.Text = "-";
            }
        }

        private bool IsUtilizaDiaria
        {
            get
            {
                return chkUsaDiaria.Checked;
            }
        }

        private void BindDadosFinanceiro(SolicitacaoViagemVO vo)
        {
            SolicitacaoViagemInfoSolicitacaoVO infoVO = vo.Solicitacao;
            SolicitacaoViagemInfoCompraVO infoCompraVO = vo.Compra;

            decimal valorPago = 0;

            decimal realValor = 0;
            realValor = infoVO.ValorAdiantamento != null ? infoVO.ValorAdiantamento.Value : 0;

            if (IsUtilizaDiaria)
            {
                List<ViagemValorVariavelVO> lstValoresDiaria = ViagemHelper.CalcularDiarias(infoCompraVO.Passagens);
                realValor += ViagemHelper.CalcularVlrTotalDiarias(lstValoresDiaria, IsUtilizaDiaria);
            }

            if (infoCompraVO.ValorPago == null)
            {
                valorPago = realValor;
            }
            else
            {
                valorPago = infoCompraVO.ValorPago.Value;
            }

            txtValorPago.Text = FormatUtil.FormatDecimalForm(valorPago);
            if (realValor != valorPago)
            {
                lblNovoValorPago.Visible = true;
                lblNovoValorPago.Text = "NOVO VALOR: " + realValor.ToString("C");
            }
            else
            {
                lblNovoValorPago.Visible = false;
            }

            gdvCompFinVoo.DataSource = infoCompraVO.Passagens;
            gdvCompFinVoo.DataBind();

            gdvCompFinHotel.DataSource = infoCompraVO.Hoteis;
            gdvCompFinHotel.DataBind();
        }

        private void BindDadosPrestacaoConta(SolicitacaoViagemInfoSolicitacaoVO infoVO, SolicitacaoViagemInfoCompraVO infoCompraVO, SolicitacaoViagemInfoPrestacaoContaVO vo)
        {
            decimal? valorHospedagem = vo.ValorHospedagem;
            decimal? valorPassagem = vo.ValorPassagens;

            if (valorHospedagem == null && infoCompraVO.Hoteis != null)
            {
                valorHospedagem = infoCompraVO.Hoteis.Sum(x => x.Valor == null ? 0 : x.Valor.Value);
            }
            if (valorPassagem == null && infoCompraVO.Passagens != null)
            {
                valorPassagem = infoCompraVO.Passagens.Sum(x => x.Valor == null ? 0 : x.Valor.Value);
            }

            txtPcHospedagem.Text = valorHospedagem == null ? "" : valorHospedagem.Value.ToString("#.00");
            txtPcPassagem.Text = valorPassagem == null ? "" : valorPassagem.Value.ToString("#.00");
            txtPcAdiantamento.Text = infoVO.ValorAdiantamento == null ? "" : infoVO.ValorAdiantamento.Value.ToString("#.00");

            dpdDespDetGrupo.Items.Clear();
            dpdDespDetGrupo.Items.Add(new ListItem("SELECIONE", ""));
            foreach (GrupoDespesaPrestContaViagem grupo in Enum.GetValues(typeof(GrupoDespesaPrestContaViagem)))
            {
                dpdDespDetGrupo.Items.Add(new ListItem(SolicitacaoViagemEnumTradutor.TraduzGrupoDespPrestConta(grupo), ((int)grupo).ToString()));
            }
            txtResumoViagem.Text = vo.ResumoViagem;

            BindGridDespDet(vo.DespesasDetalhadas);
        }

        private void BindFuncionario(EmpregadoEvidaVO funcVO)
        {
            if (funcVO != null)
            {
                txtMatricula.Text = funcVO.Matricula.ToString();
                txtNomeFuncionario.Text = funcVO.Nome;
                txtCpf.Text = funcVO.Cpf != null ? FormatUtil.FormatCpf(funcVO.Cpf) : "";
                txtRg.Text = funcVO.Rg;
                txtCargo.Text = funcVO.Funcao;
                txtDataNascimento.Text = funcVO.DataNascimento.ToString("dd/MM/yyyy");
            }
            else
            {
                txtMatricula.Text = string.Empty;
                txtDataNascimento.Text = txtCargo.Text = txtRg.Text = txtCpf.Text = txtNomeFuncionario.Text = "";
            }
        }

        private void ChangeTipoSolicitacao(bool isExterna)
        {
            txtMatricula.Enabled = !isExterna;
            txtNomeFuncionario.Enabled = isExterna;
            txtRg.Enabled = isExterna;
            txtCpf.Enabled = isExterna;
            txtDataNascimento.Enabled = isExterna;
            txtCargo.Enabled = isExterna;
            dpdTipoViagem.Enabled = !isExterna;
            trCurso.Visible = !isExterna;
            txtMatricula.Visible = !isExterna;
            chkUsaDiaria.Enabled = isExterna;
            if (isExterna)
            {
                txtMatricula.Text = string.Empty;
                dpdTipoViagem.SelectedValue = string.Empty;
                chkUsaDiaria.Visible = true;
            }
            else
            {
                chkUsaDiaria.Visible = false;
                chkUsaDiaria.Checked = true;
            }
        }

        private void CheckPermissions(SolicitacaoViagemVO vo)
        {
            PermissaoViagemVS permissao = new PermissaoViagemVS();
            Permissoes = permissao;

            pnlAvalSolCoordenador.Enabled = false;
            pnlAvalSolDiretoria.Enabled = false;
            pnlComplementar.Enabled = false;
            pnlFinanceiro.Enabled = false;
            pnlPrestacaoConta.Enabled = false;
            pnlAvalPcFinanceiro.Enabled = false;
            pnlAvalPcDiretoria.Enabled = false;

            btnSalvarSolicitacao.Visible = false;
            tbFormItinerarios.Visible = false;
            btnSalvarSolicitacao.Text = "EDITAR/REENVIAR SOLICITAÇÃO";
            btnRemoverCurso.Visible = false;
            btnIncluirCurso.Visible = false;

            btnSalvarComplementar.Visible = false;
            btnSalvarFinanceiro.Visible = false;
            btnIncluirComprovante.Visible = false;
            btnPagamentoRecebido.Visible = false;
            btnSalvarPrestacaoConta.Visible = false;
            btnIncluirArq.Visible = false;
            CanEditFile = false;
            btnAvalPcFinanceiro.Visible = false;
            btnAvalPcDiretoria.Visible = false;
            btnSalvarSolDiretoria.Visible = false;
            btnSalvarSolCoordenador.Visible = false;

            pnlConfirmarPagamento.Enabled = false;
            btnPagamentoRecebido.Visible = false;
            btnIncluirVoo.Visible = false;
            btnIncluirHotel.Visible = false;
            btnAddDespDet.Visible = false;
            btnIncluirRelatorioViagem.Visible = false;

            tabCompra.Enabled = tabPrestacaoConta.Enabled = false;

            if (vo.Situacao == StatusSolicitacaoViagem.SOLICITACAO_PENDENTE || vo.Situacao == StatusSolicitacaoViagem.SOLICITACAO_REPROVADO_COORDENADOR
                 || vo.Situacao == StatusSolicitacaoViagem.SOLICITACAO_REPROVADO_DIRETORIA)
            {
                if (vo.CodUsuarioSolicitante == UsuarioLogado.Id)
                {
                    btnSalvarSolicitacao.Visible = true;
                    tbFormItinerarios.Visible = true;
                    btnIncluirCurso.Visible = true;
                    btnCancelar.Visible = true;
                    btnCancelar.OnClientClick = "return openCancelar(this, '" + Id.Value + "');";
                    permissao.ChangeSolicitacao = true;
                }
            }

            CheckPermissionCoordenador(vo);
            CheckPermissionDiretor(vo);
            CheckPermissionSecretaria(vo);
            CheckPermissionFinanceiro(vo);

            if (vo.CodUsuarioSolicitante == UsuarioLogado.Id && vo.Situacao == StatusSolicitacaoViagem.PAGAMENTO_ADIANTAMENTO_EFETUADO)
            {
                lblPagamentoRecebido2.Visible = false;
                btnPagamentoRecebido.Visible = true;
                pnlConfirmarPagamento.Enabled = true;
            }

            if (vo.Situacao == StatusSolicitacaoViagem.PAGAMENTO_ADIANTAMENTO_CONFERIDO || vo.Situacao == StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_DIRETORIA
                || vo.Situacao == StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_FINANCEIRO)
            {
                if (vo.CodUsuarioSolicitante == UsuarioLogado.Id)
                {
                    pnlPrestacaoConta.Enabled = true;
                    btnIncluirArq.Visible = true;
                    btnSalvarPrestacaoConta.Visible = true;
                    btnIncluirRelatorioViagem.Visible = true;
                    btnAddDespDet.Visible = true;
                    CanEditFile = true;
                }
            }

            if (vo.Compra.ValorPago == null)
            {
                lblPagamentoRecebido2.Visible = true;
                lblPagamentoRecebido2.Text = "Financeiro ainda não enviou os comprovantes.";
            }
            else
            {
                if (vo.Compra.RecebimentoConfirmado)
                {
                    tabPrestacaoConta.Enabled = true;
                    lblPagamentoRecebido2.Visible = true;
                    lblPagamentoRecebido2.Text = "Pagamento confirmado.";
                }
                else
                {
                    lblPagamentoRecebido2.Visible = false;
                }
            }

            if (vo.AprovSolicitacaoDiretoria != null && vo.AprovSolicitacaoDiretoria.Aprovado)
            {
                tabCompra.Enabled = true;
            }
        }

        protected PermissaoViagemVS Permissoes
        {
            get { return ViewState["PERMISSAO"] as PermissaoViagemVS; }
            set { ViewState["PERMISSAO"] = value; }
        }

        public bool CanEditFile
        {
            get { return ViewState["CAN_EDT_FILE"] == null ? false : Convert.ToBoolean(ViewState["CAN_EDT_FILE"]); }
            set { ViewState["CAN_EDT_FILE"] = value; }
        }

        private void CheckPermissionCoordenador(SolicitacaoViagemVO vo)
        {
            lblSuperiorImediato.Text = string.Empty;

            if (vo.IsExterno)
            {
                lblSuperiorImediato.Text = "AUTOMÁTICO";
                return; // Coordenador não interage com solicitacao externa
            }

            EmpregadoEvidaVO coordenadorTotvs = null;

            if (vo.AprovSolicitacaoCoordenador != null)
            {
                if (vo.AprovSolicitacaoCoordenador.IdUsuario == 0)
                { // aprovação automatica
                    lblSuperiorImediato.Text = "AUTOMÁTICO";
                }
                else
                {
                    UsuarioVO coordenador = UsuarioBO.Instance.GetUsuarioById(vo.AprovSolicitacaoCoordenador.IdUsuario);
                    lblSuperiorImediato.Text = coordenador.Login + " - " + coordenador.Matricula + " - " + coordenador.Nome;
                }
            }
            else
            {
                coordenadorTotvs = EmpregadoEvidaBO.Instance.GetCoordenador(vo.Empregado.Matricula, vo.Empregado.CodFuncao);
                if (coordenadorTotvs == null)
                {
                    this.ShowError("Não foi possível encontrar o superior associado à matrícula " + vo.Empregado.Matricula + " no Protheus. Acionar suporte!");
                }
                else
                {
                    lblSuperiorImediato.Text = coordenadorTotvs.Matricula + " - " + coordenadorTotvs.Nome;
                }
            }

            if (vo.Situacao == StatusSolicitacaoViagem.SOLICITACAO_PENDENTE)
            {
                if (UsuarioLogado.HasPermission(Modulo.VIAGEM_COORDENADOR) && UsuarioLogado.Usuario.Matricula != null)
                {
                    if (coordenadorTotvs != null && coordenadorTotvs.Matricula == UsuarioLogado.Usuario.Matricula.Value)
                    {
                        pnlAvalSolCoordenador.Enabled = true;
                        btnSalvarSolCoordenador.Visible = true;
                        Permissoes.Coordenador = true;
                    }
                }
            }
        }

        private void CheckPermissionDiretor(SolicitacaoViagemVO vo)
        {
            lblDiretorResponsavelSol.Text = lblDiretorResponsavelPc.Text = string.Empty;

            string diretorSol = null;
            string diretorPc = null;
            EmpregadoEvidaVO diretorTotvs = null;

            if (vo.AprovSolicitacaoDiretoria != null)
            {
                UsuarioVO diretor = UsuarioBO.Instance.GetUsuarioById(vo.AprovSolicitacaoDiretoria.IdUsuario);
                diretorSol = diretor.Login + " - " + diretor.Matricula + " - " + diretor.Nome;
            }
            if (vo.AprovPrestacaoDiretoria != null)
            {
                UsuarioVO diretor = UsuarioBO.Instance.GetUsuarioById(vo.AprovPrestacaoDiretoria.IdUsuario);
                diretorPc = diretor.Login + " - " + diretor.Matricula + " - " + diretor.Nome;
            }
            if (vo.IsExterno)
            {
                diretorSol = diretorSol == null ? "TODOS" : diretorSol;
                diretorPc = diretorPc == null ? "TODOS" : diretorPc;
            }
            else
            {
                diretorTotvs = EmpregadoEvidaBO.Instance.GetDiretor(vo.Empregado.Matricula, vo.Empregado.CodFuncao);
                if (diretorTotvs == null)
                {
                    this.ShowError("Não foi possível encontrar o diretor associado à matrícula " + vo.Empregado.Matricula + " no Protheus. Acionar suporte!");
                }
                else
                {
                    diretorSol = diretorSol == null ? diretorTotvs.Matricula + " - " + diretorTotvs.Nome : diretorSol;
                    diretorPc = diretorPc == null ? diretorTotvs.Matricula + " - " + diretorTotvs.Nome : diretorPc;
                }
            }
            lblDiretorResponsavelPc.Text = diretorPc;
            lblDiretorResponsavelSol.Text = diretorSol;

            if (UsuarioLogado.HasPermission(Modulo.VIAGEM_DIRETORIA))
            {
                bool isDiretor = false;
                if (vo.IsExterno)
                {
                    isDiretor = true;
                }
                else
                {
                    if (UsuarioLogado.Usuario.Matricula != null)
                    {
                        if (diretorTotvs != null && diretorTotvs.Matricula == UsuarioLogado.Usuario.Matricula.Value)
                        {
                            isDiretor = true;
                        }
                    }
                }
                if (isDiretor)
                {
                    Permissoes.Diretor = true;
                    if (vo.Situacao == StatusSolicitacaoViagem.SOLICITACAO_APROVADO_COORDENADOR)
                    {
                        pnlAvalSolDiretoria.Enabled = true;
                        btnSalvarSolDiretoria.Visible = true;
                        Permissoes.DiretorSolicitacao = true;
                    }
                    else if (vo.Situacao == StatusSolicitacaoViagem.PRESTACAO_CONTA_CONFERIDA)
                    {
                        pnlAvalPcDiretoria.Enabled = true;
                        btnAvalPcDiretoria.Visible = true;
                        Permissoes.DiretorChangePrestacaoConta = true;
                    }
                }
            }
            if (vo.Situacao == StatusSolicitacaoViagem.PRESTACAO_CONTA_APROVADA)
            {
                pnlAvalPcDiretoria.Enabled = true;
                btnAvalPcDiretoria.Visible = false;
            }
        }

        private void CheckPermissionSecretaria(SolicitacaoViagemVO vo)
        {
            if (UsuarioLogado.HasPermission(Modulo.VIAGEM_SECRETARIA) && CanChangePassagem(vo))
            {
                pnlComplementar.Enabled = true;
                btnIncluirVoo.Visible = true;
                btnSalvarComplementar.Visible = true;
                btnIncluirHotel.Visible = true;
                Permissoes.Secretaria = true;
            }
        }

        private void CheckPermissionFinanceiro(SolicitacaoViagemVO vo)
        {
            lblFinanceiroPc.Text = string.Empty;

            if (vo.AprovPrestacaoFinanceiro != null)
            {
                UsuarioVO financeiro = UsuarioBO.Instance.GetUsuarioById(vo.AprovPrestacaoFinanceiro.IdUsuario);
                lblFinanceiroPc.Text = financeiro.Login + " - " + financeiro.Nome;
            }

            if (!UsuarioLogado.HasPermission(Modulo.VIAGEM_FINANCEIRO))
            {
                btnIncluirCompPcFinReemb.Visible = false;
                btnAvalPcFinanceiro.Visible = false;
                return;
            }

            if (vo.Compra != null && vo.Compra.Passagens.Count > 0 && CanChangePassagem(vo))
            {
                pnlFinanceiro.Enabled = true;
                btnSalvarFinanceiro.Visible = vo.Situacao == StatusSolicitacaoViagem.COMPRA_EFETUADA;
                btnIncluirComprovante.Visible = true;
                if (vo.Compra.ValorPago != null)
                    btnRemoverComprovante.Visible = true;
                Permissoes.FinanceiroCompra = true;
            }
            if (vo.Situacao == StatusSolicitacaoViagem.PRESTACAO_CONTA_PENDENTE)
            {
                pnlAvalPcFinanceiro.Enabled = true;
                btnAvalPcFinanceiro.Visible = true;
                btnIncluirCompPcFinReemb.Visible = true;
                Permissoes.FinanceiroPrestacaoConta = true;
            }
            else
            {
                btnIncluirCompPcFinReemb.Visible = false;
            }
        }

        private bool CanChangePassagem(SolicitacaoViagemVO vo)
        {
            bool prestacaoIniciada = (vo.PrestacaoConta != null && vo.PrestacaoConta.DespesasDetalhadas != null && vo.PrestacaoConta.DespesasDetalhadas.Count > 0);
            if (prestacaoIniciada) return false;
            return vo.AprovSolicitacaoDiretoria != null && vo.AprovSolicitacaoDiretoria.Aprovado;
        }

        private void BindGridDespDet(List<SolicitacaoViagemDespesaDetalhadaVO> lstDesp)
        {
            ViewState["DESP_DET"] = lstDesp;

            gdvDespDet.DataSource = lstDesp;
            gdvDespDet.DataBind();

            decimal total = 0;
            if (lstDesp != null)
            {
                total = lstDesp.Sum(x => x.Valor);
            }
            litDespDetValorTotal.Text = total.ToString("C");

            decimal adiantamento = Convert.ToDecimal(txtValorAdiantamento.Text);
            decimal valorFinal = adiantamento - total;
            txtValorReemb.Text = Math.Abs(valorFinal).ToString("C");
            if (valorFinal <= 0)
            {
                lblParecerValorReemb.Text = "REEMBOLSO";
            }
            else
            {
                lblParecerValorReemb.Text = "DEVOLVER";
            }

            BindGridDespDetFin(lstDesp);
        }

        protected void gdvDespDet_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            List<SolicitacaoViagemDespesaDetalhadaVO> lstDesp = (List<SolicitacaoViagemDespesaDetalhadaVO>)ViewState["DESP_DET"];
            gdvDespDet.PageIndex = e.NewPageIndex;
            gdvDespDet.DataSource = lstDesp;
            gdvDespDet.DataBind();

        }

        private void BindGridDespDetFin(List<SolicitacaoViagemDespesaDetalhadaVO> lstDesp)
        {
            List<SolicitacaoViagemDespesaDetalhadaVO> lstDespFin = null;
            if (lstDesp != null)
                lstDespFin = lstDesp.OrderBy(x => x.Data).OrderBy(x => x.DataConferido == null ? DateTime.MinValue : x.DataConferido).ToList();
            gdvDespDetFin.DataSource = lstDespFin;
            gdvDespDetFin.DataBind();
        }

        protected void gdvDespDetFin_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                SolicitacaoViagemDespesaDetalhadaVO vo = (SolicitacaoViagemDespesaDetalhadaVO)row.DataItem;

                TableCell cellTipo = row.Cells[2];
                TableCell cellSubTipo = row.Cells[3];
                Label lblDataConferido = (Label)row.FindControl("lblDataConferido");
                ImageButton btnDespDetConferido = (ImageButton)row.FindControl("btnDespDetConferido");
                ImageButton btnDespDetReverter = (ImageButton)row.FindControl("btnDespDetReverter");

                cellTipo.Text = SolicitacaoViagemEnumTradutor.TraduzGrupoDespPrestConta(vo.GrupoDespesa);
                cellSubTipo.Text = SolicitacaoViagemEnumTradutor.TraduzTipoDespPrestConta(vo.TipoDespesa);
                if (vo.TipoDespesa == TipoDespesaPrestContaViagem.OUTROS)
                {
                    cellSubTipo.Text += " - " + vo.DescricaoTipoDespesa;
                }

                if (vo.DataConferido != null)
                {
                    btnDespDetConferido.Visible = false;
                    lblDataConferido.Text = vo.DataConferido.Value.ToShortDateString();
                    btnDespDetReverter.Visible = pnlAvalPcFinanceiro.Enabled;
                }
                else
                {
                    btnDespDetReverter.Visible = false;
                }
            }
        }

        protected void gdvDespDetFin_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            List<SolicitacaoViagemDespesaDetalhadaVO> lstDesp = (List<SolicitacaoViagemDespesaDetalhadaVO>)ViewState["DESP_DET"];
            gdvDespDetFin.PageIndex = e.NewPageIndex;
            BindGridDespDetFin(lstDesp);

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

        private void CarregarArquivos()
        {
            dvArquivos.Visible = true;

            List<SolicitacaoViagemArquivoVO> lstArquivos = ViagemBO.Instance.ListarArquivos(Id.Value);
            if (lstArquivos == null)
                lstArquivos = new List<SolicitacaoViagemArquivoVO>();

            List<ArquivoTelaVO> lstArqs = lstArquivos.Select(x => ArquivoVO2Tela(x)).ToList();
            Arquivos = lstArqs;

            ltvArquivo.DataSource = FilterArquivo(lstArqs, TipoArquivoViagem.COMPROVANTE_DESPESA);
            ltvArquivo.DataBind();

            updArquivos.Update();

            hidComprovante.Value = string.Empty;
            ArquivoTelaVO arqVO = FilterArquivo(lstArqs, TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA).FirstOrDefault();
            if (arqVO != null)
            {
                hidComprovante.Value = arqVO.Id.ToString();
                litComprovante.Text = arqVO.NomeTela;
                tbComprovante.Visible = true;
            }

            hidRelatorioViagem.Value = string.Empty;
            arqVO = FilterArquivo(lstArqs, TipoArquivoViagem.RELATORIO_VIAGEM).FirstOrDefault();
            if (arqVO != null)
            {
                hidRelatorioViagem.Value = arqVO.Id.ToString();
                ltRelatorioViagem.Text = arqVO.NomeTela;
                tbRelatorioViagem.Visible = true;
            }

            hidCurso.Value = string.Empty;
            ltCurso.Text = string.Empty;
            tbCurso.Visible = false;
            arqVO = FilterArquivo(lstArqs, TipoArquivoViagem.CURSO).FirstOrDefault();
            if (arqVO != null)
            {
                hidCurso.Value = arqVO.Id.ToString();
                ltCurso.Text = arqVO.NomeTela;
                tbCurso.Visible = true;
                btnRemoverCurso.Visible = btnIncluirCurso.Visible;
            }

            hidCompPcFinReemb.Value = string.Empty;
            ltCompPcFinReemb.Text = string.Empty;
            tbPcFinReemb.Visible = false;
            arqVO = FilterArquivo(lstArqs, TipoArquivoViagem.COMPROVANTE_REEMBOLSO).FirstOrDefault();
            if (arqVO != null)
            {
                hidCompPcFinReemb.Value = arqVO.Id.ToString();
                ltCompPcFinReemb.Text = arqVO.NomeTela;
                tbPcFinReemb.Visible = true;
                btnRemoverCompPcFinReemb.Visible = Permissoes.FinanceiroPrestacaoConta;
            }
        }

        private static ArquivoTelaVO ArquivoVO2Tela(SolicitacaoViagemArquivoVO x)
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

        private IEnumerable<ArquivoTelaVO> FilterArquivo(IEnumerable<ArquivoTelaVO> lstArqs, TipoArquivoViagem tipo)
        {
            return lstArqs.Where(x => x.Parameters["TP_ARQUIVO"].Equals(((int)tipo).ToString()));
        }

        /*
        private void AddArquivo2(string idDesp, string fisico, string original) {
            int idDespesa = Int32.Parse(idDesp);

            ArquivoTelaVO vo = new ArquivoTelaVO() {
                NomeFisico = fisico,
                NomeTela = idDespesa + "_" + original,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoViagem.COMPROVANTE_DESPESA).ToString());

            int idArquivo = ViagemBO.Instance.SalvarArquivoDespesa(Id.Value, idDespesa, vo);

            foreach (GridViewRow row in gdvDespDet.Rows) {
                if (row.RowType == DataControlRowType.DataRow) {
                    HiddenField hidIdArquivo = (HiddenField)row.FindControl("hidIdArquivo");
                    HiddenField hidId = (HiddenField)row.FindControl("hidId");
                    if (hidId.Value.Equals(idDesp)) {
                        hidIdArquivo.Value = idArquivo.ToString();
                    }
                }
            }

            CarregarArquivos();
            this.ShowInfo("Arquivo salvo com sucesso no sistema!");
        }*/

        #endregion

        #region[INFORMAÇÕES GERAIS]

        protected void Tab_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ShowTab(btn);
        }

        private void ShowTab(Button sender)
        {
            if (sender == tabCompra)
            {
                ShowTabCompra();
            }
            else if (sender == tabPrestacaoConta)
            {
                ShowTabPrestacaoConta();
            }
            else
            {
                ShowTabSolicitacao();
            }
        }

        private void ShowTabCompra()
        {
            mtvViagem.ActiveViewIndex = 1;
            AdjustTabButtons();
        }

        private void ShowTabPrestacaoConta()
        {
            mtvViagem.ActiveViewIndex = 2;
            AdjustTabButtons();
        }

        private void ShowTabSolicitacao()
        {
            mtvViagem.ActiveViewIndex = 0;
            AdjustTabButtons();
        }

        private void AdjustTabButtons()
        {
            tabSolicitacao.CssClass = mtvViagem.ActiveViewIndex == 0 ? "tabSelected" : "tabNoSelected";
            tabCompra.CssClass = mtvViagem.ActiveViewIndex == 1 ? "tabSelected" : "tabNoSelected";
            tabPrestacaoConta.CssClass = mtvViagem.ActiveViewIndex == 2 ? "tabSelected" : "tabNoSelected";
        }

        #endregion

        #region[1. SOLICITAÇÃO -> 1.1 - DADOS DA SOLICITAÇÃO]

        protected void dpdTipoSolicitacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChangeTipoSolicitacao(Int32.Parse(dpdTipoSolicitacao.SelectedValue) == 1);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao alterar o tipo de solicitação!", ex);
            }
        }

        protected void txtMatricula_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strMatricula = txtMatricula.Text;
                if (string.IsNullOrEmpty(strMatricula))
                {
                    this.ShowError("Informe uma matrícula.");
                    return;
                }
                long matricula;
                if (!Int64.TryParse(strMatricula, out matricula))
                {
                    this.ShowError("A matrícula deve ser numérica!");
                    return;
                }
                EmpregadoEvidaVO funcVO = EmpregadoEvidaBO.Instance.GetByMatricula(matricula);
                if (funcVO == null)
                {
                    this.ShowError("Matrícula não encontrada!");
                }
                BindFuncionario(funcVO);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar matrícula.", ex);
            }
        }

        protected void btnRemoverCurso_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Id == null)
                {
                    if (ViewState["ARQ_CURSO"] == null)
                    {
                        this.ShowError("Erro na tela. Curso não enviado para remoção. Recarregar a tela e reiniciar cadastro");
                        return;
                    }
                    else
                    {
                        ViewState["ARQ_CURSO"] = null;
                        btnRemoverCurso.Visible = false;
                        ltCurso.Text = string.Empty;
                        hidCurso.Value = string.Empty;
                    }
                }
                else
                {
                    IEnumerable<ArquivoTelaVO> lstAtual = Arquivos;
                    lstAtual = FilterArquivo(lstAtual, TipoArquivoViagem.CURSO);
                    if (lstAtual.Count() == 0)
                    {
                        this.ShowError("Erro de estado da tela, não deveria aparecer a opção de remover o arquivo, pois não existe!");
                        return;
                    }
                    ArquivoTelaVO telaVO = lstAtual.First();
                    SolicitacaoViagemArquivoVO vo = ArquivoTela2VO(telaVO);
                    ViagemBO.Instance.RemoverArquivo(vo);
                    this.ShowInfo("Arquivo de curso removido com sucesso!");
                    CarregarArquivos();
                }
            }
            catch (Exception)
            {
                this.ShowError("Erro ao remover o arquivo do curso!");
            }
        }

        private SolicitacaoViagemArquivoVO ArquivoTela2VO(ArquivoTelaVO telaVO)
        {
            SolicitacaoViagemArquivoVO vo = new SolicitacaoViagemArquivoVO();
            if (!string.IsNullOrEmpty(telaVO.Id))
            {
                vo.IdArquivo = Int32.Parse(telaVO.Id);
            }
            vo.IdViagem = Id.Value;
            vo.NomeArquivo = telaVO.NomeTela;
            vo.TipoArquivo = (TipoArquivoViagem)Convert.ToInt32(telaVO.Parameters["TP_ARQUIVO"]);
            return vo;
        }

        protected void btnIncluirCurso_Click(object sender, EventArgs e)
        {
            try
            {
                ArquivoTelaVO telaVO = new ArquivoTelaVO()
                {
                    IsNew = true,
                    NomeFisico = hidArqFisico.Value,
                    NomeTela = hidArqOrigem.Value,
                    Parameters = new Dictionary<string, string>()
                };
                telaVO.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoViagem.CURSO).ToString());
                if (Id != null)
                {
                    ViagemBO.Instance.SalvarArquivoCurso(Id.Value, telaVO);
                    CarregarArquivos();
                }
                else
                {
                    ViewState["ARQ_CURSO"] = telaVO;
                    btnRemoverCurso.Visible = true;
                    ltCurso.Text = telaVO.NomeTela;

                    hidCurso.Value = string.Empty;
                }
                this.ShowInfo("Arquivo enviado com sucesso!");
            }
            catch (Exception)
            {
                this.ShowError("Erro ao incluir o arquivo do curso!");
            }
        }

        protected void btnAddItinerario_Click(object sender, EventArgs e)
        {
            try
            {
                AddItinerario();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir itinerario!", ex);
            }
        }

        private void AddItinerario()
        {
            if (string.IsNullOrEmpty(txtSolDe.Text))
            {
                this.ShowError("Informe a origem do itinerário!");
                return;
            }
            if (string.IsNullOrEmpty(txtSolPara.Text))
            {
                this.ShowError("Informe o destino do itinerário!");
                return;
            }
            if (string.IsNullOrEmpty(txtSolDataIda.Text))
            {
                this.ShowError("Informe a data de ida do itinerário!");
                return;
            }
            if (string.IsNullOrEmpty(txtSolDataVolta.Text))
            {
                this.ShowError("Informe a data de volta do itinerário!");
                return;
            }

            SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
            vo.DataPartida = DateTime.Parse(txtSolDataIda.Text);
            vo.DataRetorno = DateTime.Parse(txtSolDataVolta.Text);
            vo.Destino = txtSolPara.Text.ToUpper();
            vo.Origem = txtSolDe.Text.ToUpper();
            vo.TipoRegistro = TipoItinerarioSolicitacaoViagem.ITINERARIO;

            List<SolicitacaoViagemItinerarioVO> lstItinerario = ReadItinerario();
            lstItinerario.Add(vo);
            gdvItinerario.DataSource = lstItinerario;
            gdvItinerario.DataBind();

            txtSolDe.Text = txtSolPara.Text = txtSolDataIda.Text = txtSolDataVolta.Text = string.Empty;
        }

        private List<SolicitacaoViagemItinerarioVO> ReadItinerario()
        {
            List<SolicitacaoViagemItinerarioVO> lst = new List<SolicitacaoViagemItinerarioVO>();
            foreach (GridViewRow row in gdvItinerario.Rows)
            {
                SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
                HiddenField hidDe = (HiddenField)row.FindControl("hidDe");
                HiddenField hidPara = (HiddenField)row.FindControl("hidPara");
                HiddenField hidDataIda = (HiddenField)row.FindControl("hidDataIda");
                HiddenField hidDataVolta = (HiddenField)row.FindControl("hidDataVolta");

                vo.TipoRegistro = TipoItinerarioSolicitacaoViagem.ITINERARIO;
                vo.DataPartida = DateTime.Parse(hidDataIda.Value);
                vo.DataRetorno = DateTime.Parse(hidDataVolta.Value);
                vo.Destino = hidPara.Value;
                vo.Origem = hidDe.Value;

                lst.Add(vo);
            }
            return lst;
        }

        protected void btnRemoverItinerario_Click(object sender, ImageClickEventArgs e)
        {
            GridViewRow row = (GridViewRow)(sender as ImageButton).NamingContainer;
            RemoverItinerario(row.RowIndex);
        }

        private void RemoverItinerario(int rowNumber)
        {
            List<SolicitacaoViagemItinerarioVO> lstItinerario = ReadItinerario();
            if (rowNumber < lstItinerario.Count)
            {
                lstItinerario.RemoveAt(rowNumber);
            }

            gdvItinerario.DataSource = lstItinerario;
            gdvItinerario.DataBind();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                CancelarSolicitacao();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao cancelar solicitação!", ex);
            }
        }

        private void CancelarSolicitacao()
        {
            Bind(Id.Value);
        }

        protected void btnSalvarSolicitacao_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarSolicitacao();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar solicitação!", ex);
            }
        }

        private void SalvarSolicitacao()
        {
            string strMatricula = txtMatricula.Text;
            long cpf;
            decimal vlAdiantamento = 0;
            DateTime dataNascimento;
            long matricula = 0;
            bool isExterna = dpdTipoSolicitacao.SelectedValue.Equals("1");

            if (!isExterna)
            {
                if (string.IsNullOrEmpty(strMatricula))
                {
                    this.ShowError("Informe a matrícula!");
                    return;
                }
                if (!Int64.TryParse(strMatricula, out matricula))
                {
                    this.ShowError("A matrícula deve ser numérica!");
                    return;
                }
            }
            if (string.IsNullOrEmpty(txtNomeFuncionario.Text))
            {
                this.ShowError("Informe nome do viajante!");
                return;
            }
            if (string.IsNullOrEmpty(txtCpf.Text))
            {
                this.ShowError("Informe o CPF!");
                return;
            }
            string strCpf = FormatUtil.UnformatCpf(txtCpf.Text);
            if (!Int64.TryParse(strCpf, out cpf))
            {
                this.ShowError("O CPF deve ser numérico!");
                return;
            }
            if (string.IsNullOrEmpty(txtRg.Text))
            {
                this.ShowError("Informe o RG!");
                return;
            }
            if (string.IsNullOrEmpty(txtDataNascimento.Text))
            {
                this.ShowError("Informe a data de nascimento!");
                return;
            }
            if (!DateTime.TryParse(txtDataNascimento.Text, out dataNascimento))
            {
                this.ShowError("A data de nascimento está inválida!");
                return;
            }
            if (string.IsNullOrEmpty(txtTelContato.Text))
            {
                this.ShowError("Informe o telefone de contato!");
                return;
            }
            //if (string.IsNullOrEmpty(txtRamal.Text)) {
            //	this.ShowError("Informe o ramal de contato!");
            //	return;
            //}
            if (string.IsNullOrEmpty(txtCargo.Text) && isExterna)
            {
                this.ShowError("Informe o cargo ou função do funcionário/viajante!");
                return;
            }

            if (string.IsNullOrEmpty(txtObjetivoViagem.Text))
            {
                this.ShowError("Informe o objetivo da viagem!");
                return;
            }

            if (string.IsNullOrEmpty(dpdBanco.SelectedValue) || string.IsNullOrEmpty(txtAgencia.Text) || string.IsNullOrEmpty(txtConta.Text))
            {
                this.ShowError("Informe os dados bancários!");
                return;
            }
            if (string.IsNullOrEmpty(txtValorAdiantamento.Text))
            {
                this.ShowError("Informe o valor de adiantamento!");
                return;
            }
            if (!Decimal.TryParse(txtValorAdiantamento.Text, out vlAdiantamento))
            {
                this.ShowError("Valor de adiantamento inválido!");
                return;
            }
            if (vlAdiantamento != 0 && string.IsNullOrEmpty(txtJusAdiantamento.Text))
            {
                this.ShowError("Para solicitação de adiantamento é necessário informar uma justificativa!");
                return;
            }

            if (!ValidateUtil.IsValidLength(txtJusAdiantamento.Text, 2000))
            {
                this.ShowError("A justificativa de adiantamento pode ter no máximo 2000 caracteres");
                return;
            }

            List<MeioTransporteViagem> lstMeioTransporte = new List<MeioTransporteViagem>();
            foreach (ListItem item in chkMeioTransporte.Items)
            {
                if (item.Selected)
                {
                    lstMeioTransporte.Add((MeioTransporteViagem)Int32.Parse(item.Value));
                }
            }
            if (lstMeioTransporte.Count == 0)
            {
                this.ShowError("Informe os meios de transporte desejados!");
                return;
            }

            SolicitacaoViagemVO solVO = new SolicitacaoViagemVO();
            if (!isExterna)
            {
                solVO.Empregado = EmpregadoEvidaBO.Instance.GetByMatricula(matricula);
                if (solVO.Empregado == null)
                {
                    this.ShowError("Empregado matrícula " + matricula + " não encontrado!");
                    return;
                }
            }

            SolicitacaoViagemInfoSolicitacaoVO vo = new SolicitacaoViagemInfoSolicitacaoVO();
            solVO.Solicitacao = vo;
            solVO.IsExterno = isExterna;
            solVO.TipoViagem = string.IsNullOrEmpty(dpdTipoViagem.SelectedValue) ? new TipoViagem?() : (TipoViagem)(Convert.ToInt32(dpdTipoViagem.SelectedValue));

            vo.Nome = txtNomeFuncionario.Text;
            vo.Cpf = cpf;
            vo.Rg = txtRg.Text;
            vo.DataNascimento = dataNascimento;
            vo.Telefone = txtTelContato.Text;
            vo.Ramal = txtRamal.Text;
            vo.Cargo = txtCargo.Text;

            vo.Objetivo = txtObjetivoViagem.Text;
            vo.MeioTransporte = lstMeioTransporte;

            vo.Banco = new HcBancoVO()
            {
                Id = Int32.Parse(dpdBanco.SelectedValue)
            };
            vo.Agencia = txtAgencia.Text;
            vo.ContaCorrente = txtConta.Text;
            vo.ValorAdiantamento = vlAdiantamento;
            vo.JustificativaAdiantamento = txtJusAdiantamento.Text;

            bool usarValorDiaria = chkUsaDiaria.Checked;

            vo.Itinerarios = ReadItinerario();

            if (vo.Itinerarios == null || vo.Itinerarios.Count == 0)
            {
                this.ShowError("Informe ao menos um itinerário para a viagem!");
                return;
            }


            if (!solVO.IsExterno)
            {
                usarValorDiaria = true;
                if (solVO.TipoViagem == TipoViagem.CURSO)
                {
                    if (Id == null && ViewState["ARQ_CURSO"] == null)
                    {
                        this.ShowError("Informe comprovante de inscrição ou aprovação para o curso!");
                        return;
                    }
                    if (Id != null && string.IsNullOrEmpty(hidCurso.Value))
                    {
                        this.ShowError("Informe comprovante de inscrição ou aprovação para o curso!");
                        return;
                    }
                }
            }


            if (Id == null)
            {
                solVO.CodUsuarioSolicitante = UsuarioLogado.Id;
                solVO.Solicitacao.UsaValorDiaria = usarValorDiaria;
                ArquivoTelaVO arqTela = (ArquivoTelaVO)ViewState["ARQ_CURSO"];
                ViagemBO.Instance.CriarSolicitacao(solVO, arqTela);
                this.ShowInfo("Solicitação criada com sucesso!");
            }
            else
            {
                solVO.Id = Id.Value;
                solVO.Solicitacao.UsaValorDiaria = usarValorDiaria;
                ViagemBO.Instance.SalvarSolicitacao(solVO);
                this.ShowInfo("Alterações realizadas com sucesso!");
            }

            Bind(solVO.Id);
        }

        #endregion

        #region[1. SOLICITAÇÃO -> 1.2 - AVAL DO SUPERIOR IMEDIATO]

        protected void btnSalvarSolCoordenador_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarAvalSolCoordenador();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar aval do coordenador.", ex);
            }
        }

        private void SalvarAvalSolCoordenador()
        {
            if (string.IsNullOrEmpty(dpdAvalSolCoordenador.SelectedValue))
            {
                this.ShowError("Informe o aval!");
                return;
            }

            bool aprovado = dpdAvalSolCoordenador.SelectedValue.Equals("S");
            string justificativa = txtJusSolCoordenador.Text;
            if (!aprovado && string.IsNullOrEmpty(justificativa))
            {
                this.ShowError("Para reprovação, informe a justificativa!");
                return;
            }
            ViagemBO.Instance.AprovarSolicitacaoCoordenador(Id.Value, aprovado, justificativa, UsuarioLogado.Id);
            this.ShowInfo("Solicitação " + (aprovado ? "APROVADA" : "REPROVADA") + " com sucesso!");

            Bind(Id.Value);
        }

        #endregion

        #region[1. SOLICITAÇÃO -> 1.3 - AVAL DA DIRETORIA]

        protected void btnSalvarSolDiretoria_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarAvalSolDiretoria();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar aval da diretoria.", ex);
            }
        }

        private void SalvarAvalSolDiretoria()
        {
            if (string.IsNullOrEmpty(dpdAvalSolDiretoria.SelectedValue))
            {
                this.ShowError("Informe o aval!");
                return;
            }
            if (dpdAvalSolDiretoria.SelectedValue.Equals("N") && string.IsNullOrEmpty(txtJusSolDiretoria.Text))
            {
                this.ShowError("Para reprovação, informe a justificativa!");
                return;
            }
            bool aprovado = dpdAvalSolDiretoria.SelectedValue.Equals("S");
            ViagemBO.Instance.AprovarSolicitacaoDiretoria(Id.Value, aprovado, txtJusSolDiretoria.Text, UsuarioLogado.Id);
            this.ShowInfo("Solicitação " + (aprovado ? "APROVADA" : "REPROVADA") + " com sucesso!");

            Bind(Id.Value);
        }

        #endregion

        #region[2. COMPRA/PAGAMENTO -> 2.1 - INFORMAÇÕES COMPLEMENTARES]

        protected void dpdMeioTransporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeMeioTransporte();
        }

        private void ChangeMeioTransporte()
        {
            litValorTraslado.Text = "Valor do traslado";
            if (!string.IsNullOrEmpty(dpdMeioTransporte.SelectedValue))
            {
                MeioTransporteViagem meioTransporte = (MeioTransporteViagem)Int32.Parse(dpdMeioTransporte.SelectedValue);
                if (meioTransporte == MeioTransporteViagem.CARRO_PROPRIO)
                {
                    litValorTraslado.Text = "KM Rodados";
                }
            }
        }

        protected void btnAddVoo_Click(object sender, EventArgs e)
        {
            try
            {
                AddVoo();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir voo!", ex);
            }
        }

        private void AddVoo()
        {
            DateTime dtPartida;
            DateTime dtChegada;
            MeioTransporteViagem meioTransporte;

            string kmRodado = "";

            if (string.IsNullOrEmpty(dpdMeioTransporte.SelectedValue))
            {
                this.ShowError("Informe o meio de transporte!");
                return;
            }
            meioTransporte = (MeioTransporteViagem)Int32.Parse(dpdMeioTransporte.SelectedValue);

            if (string.IsNullOrEmpty(txtVooDataPartida.Text))
            {
                this.ShowError("Informe a data de partida!");
                return;
            }
            if (string.IsNullOrEmpty(txtVooDataChegada.Text))
            {
                this.ShowError("Informe a data de chegada!");
                return;
            }
            if (string.IsNullOrEmpty(txtVooHoraIda.Text))
            {
                this.ShowError("Informe a hora de ida da partida!");
                return;
            }

            if (string.IsNullOrEmpty(txtVooHoraChegada.Text))
            {
                this.ShowError("Informe a hora de chegada da partida!");
                return;
            }

            if (!DateTime.TryParse(txtVooDataPartida.Text + " " + txtVooHoraIda.Text, out dtPartida))
            {
                this.ShowError("Horário de ida da partida inválido!");
                return;
            }

            if (!DateTime.TryParse(txtVooDataChegada.Text + " " + txtVooHoraChegada.Text, out dtChegada))
            {
                this.ShowError("Horário de chegada da partida inválido!");
                return;
            }
            if (dtChegada < dtPartida)
            {
                this.ShowError("A data/horário de chegada deve ser maior que a data/horário de partida!");
                return;
            }
            if (string.IsNullOrEmpty(txtVooOrigem.Text))
            {
                this.ShowError("Informe a origem do traslado!");
                return;
            }
            if (string.IsNullOrEmpty(txtVooDestino.Text))
            {
                this.ShowError("Informe o destino do traslado!");
                return;
            }

            decimal valor;
            if (!Decimal.TryParse(txtVooValor.Text, out valor))
            {
                if (meioTransporte == MeioTransporteViagem.CARRO_PROPRIO)
                    this.ShowError("Informe a quantidade de KM rodados!");
                else
                    this.ShowError("Informe o valor do traslado!");
                return;
            }

            if (meioTransporte == MeioTransporteViagem.CARRO_PROPRIO)
            {
                kmRodado = valor.ToString();
                ParametroVariavelVO paramKm = ParametroVariavelBO.Instance.GetParametro(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_KM, dtPartida);
                if (paramKm == null)
                {
                    this.ShowError("Não existe parâmetro de KM configurado para a data " + dtPartida.ToShortDateString());
                    return;
                }
                valor = valor * Decimal.Parse(paramKm.Value);
            }

            SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
            vo.IdViagem = Id.Value;
            vo.DataPartida = dtPartida;
            vo.DataRetorno = dtChegada;
            vo.Valor = valor;
            vo.TipoRegistro = TipoItinerarioSolicitacaoViagem.PASSAGEM;
            vo.Complemento = ((int)meioTransporte).ToString() + ";" + kmRodado;
            vo.Origem = txtVooOrigem.Text.Upper();
            vo.Destino = txtVooDestino.Text.Upper();

            List<SolicitacaoViagemItinerarioVO> lstItinerario = ViagemBO.Instance.IncluirItinerario(vo);
            BindTraslado(lstItinerario);

            dpdMeioTransporte.SelectedValue = txtVooDataPartida.Text = txtVooHoraIda.Text = txtVooHoraChegada.Text =
                txtVooDataChegada.Text = txtVooValor.Text = txtVooDestino.Text = txtVooOrigem.Text = string.Empty;
            ChangeMeioTransporte();
        }

        protected void gdvVoo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                SolicitacaoViagemItinerarioVO vo = (SolicitacaoViagemItinerarioVO)row.DataItem;

                Literal litMeioTransporte = (Literal)row.FindControl("litMeioTransporte");
                ImageButton btnRemoverVoo = (ImageButton)row.FindControl("btnRemoverVoo");

                string[] compls = vo.Complemento.Split(new char[] { ';' });
                MeioTransporteViagem meio = (MeioTransporteViagem)Int32.Parse(compls[0]);
                litMeioTransporte.Text = meio.ToString();

                btnRemoverVoo.Visible = pnlComplementar.Enabled && vo.IdArquivo == null;
            }
        }

        protected void btnRemoverVoo_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)(sender as ImageButton).NamingContainer;
                RemoverVoo(row.RowIndex);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover voo!", ex);
            }
        }

        private void RemoverVoo(int rowNumber)
        {
            List<int> lstItinerario = ReadVooIds();
            if (rowNumber < lstItinerario.Count)
            {
                int id = lstItinerario[rowNumber];
                List<SolicitacaoViagemItinerarioVO> lstVoo = ViagemBO.Instance.RemoverItinerario(Id.Value, TipoItinerarioSolicitacaoViagem.PASSAGEM, id);
                BindTraslado(lstVoo);
            }
        }

        private List<int> ReadVooIds()
        {
            List<int> lst = new List<int>();
            foreach (GridViewRow row in gdvVoo.Rows)
            {
                SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
                HiddenField hidId = (HiddenField)row.FindControl("hidId");
                int id = Int32.Parse(hidId.Value);
                lst.Add(id);
            }
            return lst;
        }

        protected void btnAddHotel_Click(object sender, EventArgs e)
        {
            try
            {
                AddHotel();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir hotel!", ex);
            }
        }

        private void AddHotel()
        {
            decimal valor;
            if (string.IsNullOrEmpty(txtHospHotel.Text))
            {
                this.ShowError("Informe o nome do hotel!");
                return;
            }
            if (string.IsNullOrEmpty(txtHospCheckIn.Text))
            {
                this.ShowError("Informe a data de checkin!");
                return;
            }
            if (string.IsNullOrEmpty(txtHospCheckOut.Text))
            {
                this.ShowError("Informe data de checkout!");
                return;
            }
            if (string.IsNullOrEmpty(txtHospValor.Text))
            {
                this.ShowError("Informe o valor da hospedagem!");
                return;
            }
            if (!Decimal.TryParse(txtHospValor.Text, out valor))
            {
                this.ShowError("Valor de hospedagem inválido!");
                return;
            }
            SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
            vo.IdViagem = Id.Value;
            vo.DataPartida = DateTime.Parse(txtHospCheckIn.Text);
            vo.DataRetorno = DateTime.Parse(txtHospCheckOut.Text);
            vo.Origem = txtHospHotel.Text.ToUpper();
            vo.Valor = valor;
            vo.TipoRegistro = TipoItinerarioSolicitacaoViagem.HOTEL;

            List<SolicitacaoViagemItinerarioVO> lstHotel = ViagemBO.Instance.IncluirItinerario(vo);
            gdvHospedagem.DataSource = lstHotel;
            gdvHospedagem.DataBind();

            txtHospHotel.Text = txtHospCheckIn.Text = txtHospCheckOut.Text = txtHospValor.Text = string.Empty;
        }

        protected void gdvHospedagem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                SolicitacaoViagemItinerarioVO vo = (SolicitacaoViagemItinerarioVO)row.DataItem;

                ImageButton btnRemoverHotel = (ImageButton)row.FindControl("btnRemoverHotel");

                btnRemoverHotel.Visible = pnlComplementar.Enabled && vo.IdArquivo == null;
            }
        }

        protected void btnRemoverHotel_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)(sender as ImageButton).NamingContainer;
                RemoverHotel(row.RowIndex);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover hotel!", ex);
            }
        }

        private void RemoverHotel(int rowNumber)
        {
            List<int> lstItinerario = ReadHotelIds();
            if (rowNumber < lstItinerario.Count)
            {
                int id = lstItinerario[rowNumber];
                List<SolicitacaoViagemItinerarioVO> lstHotel = ViagemBO.Instance.RemoverItinerario(Id.Value, TipoItinerarioSolicitacaoViagem.HOTEL, id);
                gdvHospedagem.DataSource = lstHotel;
                gdvHospedagem.DataBind();
            }

        }

        private List<int> ReadHotelIds()
        {
            List<int> lst = new List<int>();
            foreach (GridViewRow row in gdvHospedagem.Rows)
            {
                SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
                HiddenField hidId = (HiddenField)row.FindControl("hidId");

                int id = Int32.Parse(hidId.Value);
                lst.Add(id);
            }
            return lst;
        }

        protected void btnSalvarComplementar_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarCompra();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar e encaminhar para financeiro.", ex);
            }
        }

        private void SalvarCompra()
        {
            decimal qtdDiaria;
            decimal vlCurso = 0;

            if (!Decimal.TryParse(txtDiarias.Text, out qtdDiaria))
            {
                this.ShowError("Quantidade de diárias inválido!");
                return;
            }

            bool vlCursoOk = Decimal.TryParse(txtValorCurso.Text, out vlCurso);
            if (!string.IsNullOrEmpty(dpdTipoViagem.SelectedValue))
            {
                int tipo = Convert.ToInt32(dpdTipoViagem.SelectedValue);

                if (tipo == (int)TipoViagem.CURSO)
                {
                    if (string.IsNullOrEmpty(txtValorCurso.Text))
                    {
                        this.ShowError("A viagem é do tipo CURSO, favor informar o valor do curso!");
                        return;
                    }
                    else if (!vlCursoOk)
                    {
                        this.ShowError("Valor do curso inválido!");
                        return;
                    }
                }
            }

            if (!vlCursoOk) vlCurso = 0;


            List<int> lstPassagens = ReadVooIds();
            List<int> lstHoteis = ReadHotelIds();

            if (lstPassagens.Count == 0)
            {
                this.ShowError("Informe pelo menos um traslado!");
                return;
            }
            if (lstHoteis.Count == 0)
            {
                this.ShowError("Informe pelo menos um hotel!");
                return;
            }


            SolicitacaoViagemInfoCompraVO vo = new SolicitacaoViagemInfoCompraVO();
            vo.IdViagem = Id.Value;
            vo.ValorCurso = vlCurso;

            ViagemBO.Instance.SalvarInfoCompra(vo);
            this.ShowInfo("Solicitação encaminhada ao financeiro com sucesso!");

            Bind(Id.Value);
        }

        #endregion

        #region[2. COMPRA/PAGAMENTO -> 2.2 - DADOS DO FINANCEIRO]

        protected void btnRemoverComprovante_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                RemoverComprovantePagamento();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover comprovante.", ex);
            }
        }

        private void RemoverComprovantePagamento()
        {
            ViagemBO.Instance.RemoverComprovantePagamento(Id.Value);
            this.ShowInfo("Comprovante removido com sucesso!");
            Bind(Id.Value);
        }

        protected void btnIncluirComprovante_Click(object sender, EventArgs e)
        {
            try
            {
                AddComprovantePagamento(hidArqFisico.Value, hidArqOrigem.Value);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir ou alterar o comprovante.", ex);
            }
        }

        private void AddComprovantePagamento(string fisico, string original)
        {
            string ext = System.IO.Path.GetExtension(original);
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = "COMPROVANTE" + ext,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA).ToString());
            decimal valorPago = 0;
            Decimal.TryParse(txtValorPago.Text, out valorPago);

            ViagemBO.Instance.SalvarComprovantePagamento(Id.Value, vo, valorPago);

            this.ShowInfo("Comprovante salvo com sucesso no sistema!");
            Bind(Id.Value);
        }

        protected void gdvCompFinVoo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                SolicitacaoViagemItinerarioVO vo = (SolicitacaoViagemItinerarioVO)row.DataItem;

                Literal litMeioTransporte = (Literal)row.FindControl("litMeioTransporte");
                Button btnAddCompVoo = (Button)row.FindControl("btnAddCompVoo");
                LinkButton lnkCompVoo = (LinkButton)row.FindControl("lnkCompVoo");
                Label lblNew = (Label)row.FindControl("lblNew");

                string[] compls = vo.Complemento.Split(new char[] { ';' });
                MeioTransporteViagem meio = (MeioTransporteViagem)Int32.Parse(compls[0]);
                litMeioTransporte.Text = meio.ToString();

                lnkCompVoo.Visible = false;
                if (vo.IdArquivo != null)
                {
                    lnkCompVoo.Visible = true;
                    lnkCompVoo.OnClientClick = "return openDownload('" + Id.Value + "', " + ((int)TipoArquivoViagem.COMPROVANTE_PAGTO_TRASLADO) + ", '" + vo.IdArquivo.Value + "', false);";
                    btnAddCompVoo.Text = "Alterar";
                }
                else
                {
                    btnAddCompVoo.Text = "Incluir";
                    lblNew.Visible = true;
                }

                btnAddCompVoo.Visible = pnlFinanceiro.Enabled;
            }
        }

        protected void btnAddCompVoo_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)(sender as Button).NamingContainer;
            int id = Convert.ToInt32(gdvCompFinVoo.DataKeys[row.RowIndex][0]);
            AddComprovanteItinerario(hidArqFisico.Value, hidArqOrigem.Value, TipoItinerarioSolicitacaoViagem.PASSAGEM, id);
        }

        private void AddComprovanteItinerario(string fisico, string original, TipoItinerarioSolicitacaoViagem tipo, int id)
        {
            string ext = System.IO.Path.GetExtension(original);
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = "COMPROVANTE_" + ((int)tipo) + "_" + id + ext,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            ViagemBO.Instance.IncluirComprovanteItinerario(Id.Value, tipo, id, vo);

            this.ShowInfo("Comprovante salvo com sucesso no sistema!");

            Bind(Id.Value);
        }

        protected void gdvCompFinHotel_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                SolicitacaoViagemItinerarioVO vo = (SolicitacaoViagemItinerarioVO)row.DataItem;

                Button btnAddCompHotel = (Button)row.FindControl("btnAddCompHotel");
                LinkButton lnkCompHotel = (LinkButton)row.FindControl("lnkCompHotel");
                Label lblNew = (Label)row.FindControl("lblNew");

                lnkCompHotel.Visible = false;
                if (vo.IdArquivo != null)
                {
                    lnkCompHotel.Visible = true;
                    lnkCompHotel.OnClientClick = "return openDownload('" + Id.Value + "', " + ((int)TipoArquivoViagem.COMPROVANTE_PAGTO_HOTEL) + ", '" + vo.IdArquivo.Value + "', false);";
                    btnAddCompHotel.Text = "Alterar";
                }
                else
                {
                    lblNew.Visible = true;
                    btnAddCompHotel.Text = "Incluir";
                }

                btnAddCompHotel.Visible = pnlFinanceiro.Enabled;
            }
        }

        protected void btnAddCompHotel_Click(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)(sender as Button).NamingContainer;
            int id = Convert.ToInt32(gdvCompFinHotel.DataKeys[row.RowIndex][0]);
            AddComprovanteItinerario(hidArqFisico.Value, hidArqOrigem.Value, TipoItinerarioSolicitacaoViagem.HOTEL, id);
        }

        protected void btnSalvarFinanceiro_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarFinanceiro();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover comprovante.", ex);
            }
        }

        private void SalvarFinanceiro()
        {
            decimal vlPago;
            if (!Decimal.TryParse(txtValorPago.Text, out vlPago))
            {
                this.ShowError("Informe o valor pago!");
                return;
            }
            if (string.IsNullOrEmpty(hidComprovante.Value))
            {
                this.ShowError("É necessário informar o comprovante de pagamento para prosseguir!");
                return;
            }

            SolicitacaoViagemInfoCompraVO vo = new SolicitacaoViagemInfoCompraVO();
            vo.IdViagem = Id.Value;
            vo.ValorPago = vlPago;

            ViagemBO.Instance.SalvarFinanceiroPagamento(vo);
            this.ShowInfo("Dados do financeiro salvos com sucesso!");

            Bind(Id.Value);
        }

        #endregion

        #region[2. COMPRA/PAGAMENTO -> 2.3 - CONFIRMAÇÃO DE PAGAMENTO/RECEBIMENTO]

        protected void btnPagamentoRecebido_Click(object sender, EventArgs e)
        {
            try
            {
                ConfirmarPagamentoRecebido();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao confirmar pagamento.", ex);
            }
        }

        private void ConfirmarPagamentoRecebido()
        {

            ViagemBO.Instance.ConfirmarPagamentoRecebido(Id.Value);
            this.ShowInfo("Confirmação realizada! Aguardando prestação de contas!");
            Bind(Id.Value);
        }

        #endregion

        #region[3. PRESTAÇÃO DE CONTAS -> 3.1 - PRESTAÇÃO DE CONTAS]

        protected void btnAddDespDet_Click(object sender, EventArgs e)
        {
            try
            {
                AddDespDet();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir despesa!", ex);
            }
        }

        private void AddDespDet()
        {
            decimal valor;
            int iValue;
            GrupoDespesaPrestContaViagem grupoDespesa;
            TipoDespesaPrestContaViagem tipoDespesa;
            if (string.IsNullOrEmpty(txtDespDet.Text))
            {
                this.ShowError("Informe a descrição da despesa!");
                return;
            }
            if (string.IsNullOrEmpty(txtDespDetData.Text))
            {
                this.ShowError("Informe a data da despesa!");
                return;
            }
            if (string.IsNullOrEmpty(txtDespDetValor.Text))
            {
                this.ShowError("Informe o valor da despesa!");
                return;
            }
            if (!Decimal.TryParse(txtDespDetValor.Text, out valor))
            {
                this.ShowError("Valor de despesa inválido!");
                return;
            }
            if (string.IsNullOrEmpty(dpdDespDetGrupo.SelectedValue))
            {
                this.ShowError("Informe o tipo da despesa!");
                return;
            }
            if (!Int32.TryParse(dpdDespDetGrupo.SelectedValue, out iValue))
            {
                this.ShowError("Opção de Tipo de despesa inválida!");
                return;
            }
            grupoDespesa = (GrupoDespesaPrestContaViagem)iValue;
            if (string.IsNullOrEmpty(dpdDespDetTipo.SelectedValue))
            {
                this.ShowError("Informe o sub-tipo da despesa!");
                return;
            }
            if (!Int32.TryParse(dpdDespDetTipo.SelectedValue, out iValue))
            {
                this.ShowError("Opção de sub-tipo de despesa inválida!");
                return;
            }
            tipoDespesa = (TipoDespesaPrestContaViagem)iValue;

            if (tipoDespesa == TipoDespesaPrestContaViagem.OUTROS)
            {
                if (string.IsNullOrEmpty(txtDespDetOutros.Text))
                {
                    this.ShowError("Ao selecionar o sub-tipo OUTROS, deve informar o nome.");
                    return;
                }
            }

            if (string.IsNullOrEmpty(txtDespDetIdent.Text))
            {
                this.ShowError("Informe o identificador do recibo/cupom/nota!");
                return;
            }

            SolicitacaoViagemDespesaDetalhadaVO vo = new SolicitacaoViagemDespesaDetalhadaVO();
            vo.Data = DateTime.Parse(txtDespDetData.Text);
            vo.Descricao = txtDespDet.Text.ToUpper();
            vo.Valor = valor;
            vo.IdViagem = Id.Value;
            vo.GrupoDespesa = grupoDespesa;
            vo.TipoDespesa = tipoDespesa;
            vo.DescricaoTipoDespesa = txtDespDetOutros.Text.Upper();
            vo.Identificador = txtDespDetIdent.Text;

            List<SolicitacaoViagemDespesaDetalhadaVO> lst = ViagemBO.Instance.IncluirDespesaDetalhada(vo);

            this.ShowInfo("Despesa incluída com sucesso!");

            BindGridDespDet(lst);

            txtDespDetIdent.Text = txtDespDetOutros.Text = dpdDespDetGrupo.SelectedValue = txtDespDet.Text = txtDespDetData.Text = txtDespDetValor.Text = string.Empty;
            dpdDespDetTipo.Items.Clear();
        }

        protected void dpdDespDetGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            GrupoDespesaPrestContaViagem? grupo = null;
            if (!string.IsNullOrEmpty(dpdDespDetGrupo.SelectedValue))
            {
                grupo = (GrupoDespesaPrestContaViagem)Int32.Parse(dpdDespDetGrupo.SelectedValue);
            }
            ChangeTipoDespesa(grupo);
        }

        protected void ChangeTipoDespesa(GrupoDespesaPrestContaViagem? grupo)
        {
            dpdDespDetTipo.Items.Clear();
            if (grupo != null)
            {
                TipoDespesaPrestContaViagem[] lstTipoDespesa = SolicitacaoViagemEnumTradutor.GetListaTipoDespesa(grupo.Value);
                dpdDespDetTipo.Items.Add(new ListItem("SELECIONE", ""));
                foreach (TipoDespesaPrestContaViagem tipo in lstTipoDespesa)
                {
                    dpdDespDetTipo.Items.Add(new ListItem(SolicitacaoViagemEnumTradutor.TraduzTipoDespPrestConta(tipo), ((int)tipo).ToString()));
                }
            }
        }

        protected void dpdDespDetTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDespDetOutros.Enabled = false;
            if (!string.IsNullOrEmpty(dpdDespDetTipo.SelectedValue))
            {
                TipoDespesaPrestContaViagem tipo = (TipoDespesaPrestContaViagem)Int32.Parse(dpdDespDetTipo.SelectedValue);
                if (tipo == TipoDespesaPrestContaViagem.OUTROS)
                    txtDespDetOutros.Enabled = true;
            }
        }

        protected void gdvDespDet_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                SolicitacaoViagemDespesaDetalhadaVO vo = (SolicitacaoViagemDespesaDetalhadaVO)row.DataItem;

                TableCell cellTipo = row.Cells[2];
                TableCell cellSubTipo = row.Cells[3];
                ImageButton btnRemoverDespDet = (ImageButton)row.FindControl("btnRemoverDespDet");

                cellTipo.Text = SolicitacaoViagemEnumTradutor.TraduzGrupoDespPrestConta(vo.GrupoDespesa);
                cellSubTipo.Text = SolicitacaoViagemEnumTradutor.TraduzTipoDespPrestConta(vo.TipoDespesa);
                if (vo.TipoDespesa == TipoDespesaPrestContaViagem.OUTROS)
                {
                    cellSubTipo.Text += " - " + vo.DescricaoTipoDespesa;
                }
                vlTotalDespDet += vo.Valor;
                if (vo.DataConferido != null)
                {
                    btnRemoverDespDet.Visible = false;
                }
            }
            else if (row.RowType == DataControlRowType.Header)
            {
                vlTotalDespDet = 0;
            }
            else if (row.RowType == DataControlRowType.Footer)
            {
                row.Cells[1].Text = "TOTAL";
                row.Cells[6].Text = vlTotalDespDet.ToString("C");
            }

        }

        protected void btnRemoverDespDet_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)(sender as ImageButton).NamingContainer;
                RemoverDespDet(row.RowIndex);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover despesa!", ex);
            }
        }

        private void RemoverDespDet(int rowNumber)
        {
            List<int> lstIds = ReadDespDetIds();
            if (rowNumber < lstIds.Count)
            {
                int id = lstIds[rowNumber];
                List<SolicitacaoViagemDespesaDetalhadaVO> lstItinerario = ViagemBO.Instance.RemoverDespesaDetalhada(Id.Value, id);

                BindGridDespDet(lstItinerario);
            }
            else
            {
                this.ShowError("Indice de remoção inválido!");
            }
        }

        private List<int> ReadDespDetIds()
        {
            List<int> lst = new List<int>();
            foreach (GridViewRow row in gdvDespDet.Rows)
            {
                SolicitacaoViagemDespesaDetalhadaVO vo = new SolicitacaoViagemDespesaDetalhadaVO();
                HiddenField hidId = (HiddenField)row.FindControl("hidId");

                int id = Int32.Parse(hidId.Value);
                lst.Add(id);
            }
            return lst;
        }

        protected void btnIncluirRelatorioViagem_Click(object sender, EventArgs e)
        {
            string fisico = hidArqFisico.Value;
            string original = hidArqOrigem.Value;
            string ext = System.IO.Path.GetExtension(original);
            ArquivoTelaVO telaVO = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = "RELATORIO" + ext,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            telaVO.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoViagem.RELATORIO_VIAGEM).ToString());


            ViagemBO.Instance.SalvarRelatorioViagem(Id.Value, telaVO);

            this.ShowInfo("Relatório salvo com sucesso no sistema!");

            CarregarArquivos();
        }

        protected void btnRemoverArquivo_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
                RemoverArquivoDespesa(row);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover item da lista", ex);
            }
        }

        private void RemoverArquivoDespesa(ListViewDataItem row)
        {
            int idx = row.DataItemIndex;

            IEnumerable<ArquivoTelaVO> lstAtual = Arquivos;
            lstAtual = FilterArquivo(lstAtual, TipoArquivoViagem.COMPROVANTE_DESPESA);
            ArquivoTelaVO telaVO = lstAtual.ElementAt(idx);
            if (!telaVO.IsNew)
            {
                SolicitacaoViagemArquivoVO vo = ArquivoTela2VO(telaVO);
                ViagemBO.Instance.RemoverArquivo(vo);
            }
            this.ShowInfo("Arquivo removido com sucesso!");
            CarregarArquivos();
        }

        protected void btnIncluirArquivo_Click(object sender, EventArgs e)
        {
            AddArquivoDespesa(hidArqFisico.Value, hidArqOrigem.Value);
        }

        private void AddArquivoDespesa(string fisico, string original)
        {
            List<ArquivoTelaVO> lstAtual = Arquivos;
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = original,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoViagem.COMPROVANTE_DESPESA).ToString());
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
                SalvarAddArquivoDespesa();
            }
            else
            {
                this.ShowInfo("Arquivo adicionado em tela! Arquivos só serão salvos quando formulário enviado!");
            }
        }

        private void SalvarAddArquivoDespesa()
        {
            ArquivoTelaVO telaVO = Arquivos.Last(x => x.IsNew);
            ViagemBO.Instance.SalvarArquivoDespesa(Id.Value, telaVO);

            CarregarArquivos();
            this.ShowInfo("Arquivo salvo com sucesso no sistema!");
        }

        protected void btnSalvarPrestacaoConta_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarPrestacaoConta();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar e encaminhar para o financeiro.", ex);
            }
        }

        private void SalvarPrestacaoConta()
        {

            decimal vlPcHospedagem;
            decimal vlPcPassagem;

            if (!Decimal.TryParse(txtPcHospedagem.Text, out vlPcHospedagem))
            {
                this.ShowError("Informe a despesa de hospedagem!");
                return;
            }
            if (!Decimal.TryParse(txtPcPassagem.Text, out vlPcPassagem))
            {
                this.ShowError("Informe a despesa das passagens!");
                return;
            }

            if (ltvArquivo.Items.Count == 0)
            {
                this.ShowError("Informe pelo menos um comprovante de despesa!");
                return;
            }

            if (string.IsNullOrEmpty(hidRelatorioViagem.Value))
            {
                this.ShowError("Informe o relatório detalhado da viagem!");
                return;
            }

            if (string.IsNullOrEmpty(txtResumoViagem.Text))
            {
                this.ShowError("Informe o resumo da viagem!");
                return;
            }

            if (!ValidateUtil.IsValidLength(txtResumoViagem.Text, 1000))
            {
                this.ShowError("O resumo pode ter no máximo 1000 caracteres");
                return;
            }

            List<int> lstDespDet = ReadDespDetIds();
            if (lstDespDet.Count == 0)
            {
                this.ShowError("Informe pelo menos uma despesa detalhada!");
                return;
            }

            SolicitacaoViagemInfoPrestacaoContaVO vo = new SolicitacaoViagemInfoPrestacaoContaVO();
            vo.IdViagem = Id.Value;
            vo.ValorHospedagem = vlPcHospedagem;
            vo.ValorPassagens = vlPcPassagem;
            vo.ResumoViagem = txtResumoViagem.Text;

            ViagemBO.Instance.SalvarPrestacaoConta(vo);
            this.ShowInfo("Prestação de conta realizada com sucesso!");
            Bind(Id.Value);
        }

        #endregion

        #region[3. PRESTAÇÃO DE CONTAS -> 3.2 - CONFERÊNCIA DO FINANCEIRO]

        protected void btnDespDetConferido_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)(sender as ImageButton).NamingContainer;
                MarcarDespDetConferido(row.RowIndex);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao marcar despesa!", ex);
            }
        }

        private void MarcarDespDetConferido(int rowNumber)
        {
            List<int> lstIds = ReadDespDetFinIds();
            if (rowNumber < lstIds.Count)
            {
                int id = lstIds[rowNumber];
                List<SolicitacaoViagemDespesaDetalhadaVO> lstItinerario = ViagemBO.Instance.MarcarDespDetConferido(Id.Value, id, true);

                BindGridDespDet(lstItinerario);
            }
            else
            {
                this.ShowError("Indice de remoção inválido!");
            }
        }

        protected void btnDespDetReverter_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)(sender as ImageButton).NamingContainer;
                MarcarDespDetReverter(row.RowIndex);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao reverter conferido!", ex);
            }
        }

        private void MarcarDespDetReverter(int rowNumber)
        {
            List<int> lstIds = ReadDespDetFinIds();
            if (rowNumber < lstIds.Count)
            {
                int id = lstIds[rowNumber];
                List<SolicitacaoViagemDespesaDetalhadaVO> lstItinerario = ViagemBO.Instance.MarcarDespDetConferido(Id.Value, id, false);

                BindGridDespDet(lstItinerario);
            }
            else
            {
                this.ShowError("Indice de remoção inválido!");
            }
        }

        private List<int> ReadDespDetFinIds()
        {
            List<int> lst = new List<int>();
            foreach (GridViewRow row in gdvDespDetFin.Rows)
            {
                SolicitacaoViagemDespesaDetalhadaVO vo = new SolicitacaoViagemDespesaDetalhadaVO();
                HiddenField hidId = (HiddenField)row.FindControl("hidId");

                int id = Int32.Parse(hidId.Value);
                lst.Add(id);
            }
            return lst;
        }

        protected void btnRemoverCompPcFinReemb_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                RemoverComprovanteReembolso();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao remover comprovante.", ex);
            }
        }

        private void RemoverComprovanteReembolso()
        {
            ViagemBO.Instance.RemoverComprovanteReembolso(Id.Value);
            this.ShowInfo("Comprovante removido com sucesso!");
            Bind(Id.Value);
        }

        protected void btnIncluirCompPcFinReemb_Click(object sender, EventArgs e)
        {
            try
            {
                AddComprovanteReembolso(hidArqFisico.Value, hidArqOrigem.Value);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao incluir ou alterar o comprovante.", ex);
            }
        }

        private void AddComprovanteReembolso(string fisico, string original)
        {
            string ext = System.IO.Path.GetExtension(original);
            ArquivoTelaVO vo = new ArquivoTelaVO()
            {
                NomeFisico = fisico,
                NomeTela = "COMPROVANTE" + ext,
                IsNew = true,
                Parameters = new Dictionary<string, string>()
            };
            vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoViagem.COMPROVANTE_REEMBOLSO).ToString());
            decimal valorPago = 0;
            Decimal.TryParse(txtValorPago.Text, out valorPago);

            ViagemBO.Instance.SalvarComprovanteReembolso(Id.Value, vo);

            this.ShowInfo("Comprovante salvo com sucesso no sistema!");
            Bind(Id.Value);
        }

        protected void btnAvalPcFinanceiro_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarAvalPcFinanceiro();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar aval do financeiro.", ex);
            }
        }

        private void SalvarAvalPcFinanceiro()
        {
            if (string.IsNullOrEmpty(dpdAvalPcFinanceiro.SelectedValue))
            {
                this.ShowError("Informe o aval!");
                return;
            }
            byte[] relatorio = null;
            bool aprovado = dpdAvalPcFinanceiro.SelectedValue.Equals("S");
            string justificativa = txtJusPcFinanceiro.Text;
            if (!aprovado && string.IsNullOrEmpty(justificativa))
            {
                this.ShowError("Para reprovação, informe a justificativa!");
                return;
            }
            if (aprovado)
            {
                SolicitacaoViagemVO vo = ViagemBO.Instance.GetById(Id.Value);
                List<SolicitacaoViagemDespesaDetalhadaVO> lstDespDet = vo.PrestacaoConta.DespesasDetalhadas;
                if (lstDespDet != null && lstDespDet.Count > 0)
                {
                    if (lstDespDet.Where(x => x.DataConferido == null).Count() > 0)
                    {
                        this.ShowError("Não é possível aprovar prestação de conta, pois existem despesas que ainda não foram conferidas!");
                        return;
                    }
                }

                decimal valorReemb = Decimal.Parse(txtValorReemb.Text, NumberStyles.Currency);
                if (valorReemb != 0 && string.IsNullOrEmpty(hidCompPcFinReemb.Value))
                {
                    this.ShowError("É necessário informar o comprovante de reembolso/devolução!");
                    return;
                }

                ReportViagem rpt = new ReportViagem(ReportDir, UsuarioLogado);
                relatorio = rpt.GerarRelatorio(vo);
            }
            ViagemBO.Instance.AprovarPrestacaoContaFinanceiro(Id.Value, aprovado, justificativa, UsuarioLogado.Id, relatorio);
            this.ShowInfo("Prestação de Contas " + (aprovado ? "APROVADA" : "REPROVADA") + " com sucesso!");

            Bind(Id.Value);
        }

        #endregion

        #region[3. PRESTAÇÃO DE CONTAS -> 3.3 - AVAL DA DIRETORIA]

        protected void btnAvalPcDiretoria_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarAvalPcDiretoria();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar aval da diretoria.", ex);
            }
        }

        private void SalvarAvalPcDiretoria()
        {
            if (string.IsNullOrEmpty(dpdAvalPcDiretoria.SelectedValue))
            {
                this.ShowError("Informe o aval!");
                return;
            }

            bool aprovado = dpdAvalPcDiretoria.SelectedValue.Equals("S");
            string justificativa = txtJusPcDiretoria.Text;
            if (!aprovado && string.IsNullOrEmpty(justificativa))
            {
                this.ShowError("Para reprovação, informe a justificativa!");
                return;
            }
            ViagemBO.Instance.AprovarPrestacaoContaDiretoria(Id.Value, aprovado, justificativa, UsuarioLogado.Id);
            string adicional = aprovado ? "Fluxo da solicitação de viagem finalizado! " : "";
            this.ShowInfo("Prestação de Contas " + (aprovado ? "APROVADA" : "REPROVADA") + " com sucesso! " + adicional);

            Bind(Id.Value);
        }

        #endregion

    }
}