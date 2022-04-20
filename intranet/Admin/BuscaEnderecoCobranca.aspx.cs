using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Admin
{
    public partial class BuscaEnderecoCobranca : RelatorioExcelPageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    dpdPreenchido.Items.Insert(0, new ListItem("TODOS", ""));
                    dpdPreenchido.Items.Insert(1, new ListItem("SIM", "S"));
                    dpdPreenchido.Items.Insert(2, new ListItem("NÃO", "N"));
                }
                catch (Exception ex)
                {
                    this.ShowError("Erro ao carregar a página", ex);
                }
            }
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_ENDERECO_COBRANCA; }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar dados", ex);
            }

        }

        private void Buscar()
        {
            string cpf = String.Empty;
            string nome = String.Empty;
            string preenchido = String.Empty;

            if (!string.IsNullOrEmpty(txtCpf.Text.Trim()))
            {
                cpf = txtCpf.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtNome.Text.Trim()))
            {
                nome = txtNome.Text.Trim();
            }

            preenchido = dpdPreenchido.SelectedValue;

            DataTable dt = PClienteBO.Instance.Pesquisar(cpf, nome, preenchido);
            this.ShowPagingGrid(gdvRelatorio, dt, null);

            lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                /*

                StatusIndisponibilidadeRede status = (StatusIndisponibilidadeRede)Convert.ToInt32(vo["id_situacao"]);
                TableCell cellVencimento = row.Cells[11];
                TableCell cellDiasAtraso = row.Cells[12];
                Label lblResponsavel = (Label)row.FindControl("lblResponsavel");
                ImageButton btnEditar = (ImageButton)row.FindControl("btnEditar");
                LinkButton btnEncerrar = (LinkButton)row.FindControl("btnEncerrar");
                LinkButton btnReabrirSolicitacao = (LinkButton)row.FindControl("btnReabrirSolicitacao");
                Label lblProcedencia = (Label)row.FindControl("lblProcedencia");
                Label lblPendencia = (Label)row.FindControl("lblPendencia");
                LinkButton btnExecutarCobranca = (LinkButton)row.FindControl("btnExecutarCobranca");
                EncaminhamentoIndisponibilidadeRede setor = (EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(vo["ID_SETOR_ENCAMINHAMENTO"]);

                row.Cells[9].Text = IndisponibilidadeRedeEnumTradutor.TraduzPrioridade((PrioridadeIndisponibilidadeRede)Convert.ToInt32(vo["NR_PRIORIDADE"]));
                row.Cells[13].Text = IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento((EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(vo["ID_SETOR_ENCAMINHAMENTO"]));
                row.Cells[15].Text = IndisponibilidadeRedeEnumTradutor.TraduzStatus(status);

                #region[CÁLCULO DO VENCIMENTO E ATRASO]

                StatusIndisponibilidadeRede situacao = (StatusIndisponibilidadeRede)Convert.ToInt32(vo["ID_SITUACAO"]);
                int cdIndisponibilidade = Convert.ToInt32(vo["CD_INDISPONIBILIDADE"]);
                DateTime dataSolicitacao = Convert.ToDateTime(vo["DT_SOLICITACAO"]);
                DateTime dataSituacao = Convert.ToDateTime(vo["DT_SITUACAO"]);
                DateTime dataAtendimento = (vo["DT_ATENDIMENTO"] != DBNull.Value ? Convert.ToDateTime(vo["DT_ATENDIMENTO"]) : DateTime.MinValue);
                DateTime dataReferencia = DateTime.Now;
                int diasEstimados = (vo["NR_DIAS_PRAZO"] != DBNull.Value) ? Convert.ToInt32(vo["NR_DIAS_PRAZO"]) : 0;
                Double horasUsadas = (vo["NR_HORAS"] != DBNull.Value) ? Convert.ToInt32(vo["NR_HORAS"]) : 0;
                DateTime dataVencimento;
                int diasAtraso;

                if (situacao == StatusIndisponibilidadeRede.ENCERRADO)
                {
                    dataReferencia = dataSituacao;
                }

                if (dataAtendimento != DateTime.MinValue)
                {
                    dataReferencia = dataAtendimento;
                }

                Double horasPendencia = 0;
                DateTime? inicioPendencia = null;

                dataVencimento = DateUtil.SomarDiasUteis(dataSolicitacao, diasEstimados);

                DataTable dtPendencias = IndisponibilidadeRedeBO.Instance.ListarPendencias(cdIndisponibilidade);
                foreach (DataRow linha in dtPendencias.Rows)
                {
                    DateTime dataEvento = Convert.ToDateTime(linha["DATA"]);
                    string tipoEvento = Convert.ToString(linha["EVENTO"]);

                    if (tipoEvento == "PENDENTE")
                    {
                        if (inicioPendencia == null)
                        {
                            if (dataEvento < dataVencimento)
                            {

                                inicioPendencia = dataEvento;

                            }

                        }
                    }
                    else if (tipoEvento == "RESOLVIDO")
                    {
                        if (inicioPendencia != null)
                        {
                            if (dataEvento >= dataVencimento)
                            {
                                horasPendencia += DateUtil.CalcularHorasDiasUteis((DateTime)inicioPendencia, dataVencimento);
                            }
                            else
                            {
                                horasPendencia += DateUtil.CalcularHorasDiasUteis((DateTime)inicioPendencia, dataEvento);
                            }
                        }

                        inicioPendencia = null;
                    }
                }

                if (inicioPendencia != null)
                {
                    horasPendencia += DateUtil.CalcularHorasDiasUteis((DateTime)inicioPendencia, dataVencimento);
                }

                dataVencimento = DateUtil.SomarDiasUteis(dataVencimento, (Int32)Math.Floor(horasPendencia / 24));

                if (dataReferencia <= dataVencimento || status == StatusIndisponibilidadeRede.ENCERRADO)
                {
                    diasAtraso = 0;
                }
                else
                {
                    TimeSpan intervalo = dataReferencia - dataVencimento;
                    diasAtraso = intervalo.Days;
                }

                row.Cells[11].Text = dataVencimento.ToShortDateString();
                row.Cells[12].Text = diasAtraso.ToString();

                if (row.Cells[12].Text.Trim() == "0") row.Cells[12].Text = "";

                #endregion

                if (diasAtraso <= 0)
                {
                    cellVencimento.ForeColor = System.Drawing.Color.Black;
                    cellDiasAtraso.Text = "";
                }
                else if (diasAtraso <= 3)
                {
                    cellVencimento.ForeColor = System.Drawing.Color.Blue;
                }
                else
                {
                    cellVencimento.ForeColor = System.Drawing.Color.Red;
                }

                #region[Controle do botão Encerrar]

                btnEncerrar.Visible = false;
                if (status != StatusIndisponibilidadeRede.ABERTO && status != StatusIndisponibilidadeRede.ENCERRADO)
                {
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO))
                    {
                        btnEncerrar.Visible = true;
                    }
                    else if (hasPermissionRegional() && isRegional(setor))
                    {
                        btnEncerrar.Visible = true;
                    }
                }

                #endregion

                #region[Controle do botão Reabrir Solicitação]

                btnReabrirSolicitacao.Visible = false;

                // Se o protocolo estiver encerrado
                if (status == StatusIndisponibilidadeRede.ENCERRADO)
                {
                    // Se o usuário tem permissão de diretoria ou credenciamento
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_DIRETORIA) || UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO))
                    {
                        btnReabrirSolicitacao.Visible = true;
                    }
                }

                #endregion

                lblResponsavel.Text = "- NENHUM - ";
                UsuarioVO resp = null;
                if (vo["CD_USUARIO_ATUANTE"] != DBNull.Value)
                {
                    resp = UsuarioBO.Instance.GetUsuarioById(Convert.ToInt32(vo["CD_USUARIO_ATUANTE"]));
                    lblResponsavel.Text = resp.Nome + " (" + resp.Login + ")";
                    lblResponsavel.ToolTip = string.Empty;
                }
                else
                {
                    lblResponsavel.ToolTip = "Nenhum usuário do setor assumiu na etapa atual.";
                }

                if (vo["DS_PROCEDENCIA"] != DBNull.Value)
                {
                    String procedencia = Convert.ToString(vo["DS_PROCEDENCIA"]);

                    if (procedencia == "P")
                    {
                        lblProcedencia.Text = "PROCEDENTE";
                    }
                    else if (procedencia == "I")
                    {
                        lblProcedencia.Text = "IMPROCEDENTE";
                    }
                }
                else
                {
                    lblProcedencia.Text = "";
                }

                TipoPendenciaIndisponibilidadeRede? tipoPendencia = null;
                if (vo["TP_PENDENCIA"] != DBNull.Value)
                {
                    tipoPendencia = (TipoPendenciaIndisponibilidadeRede)Convert.ToInt32(vo["TP_PENDENCIA"]);
                }
                lblPendencia.Text = tipoPendencia != null ? IndisponibilidadeRedeEnumTradutor.TraduzTipoPendencia(tipoPendencia.Value) : "";

                if (vo["FL_ACOMPANHANTE"] != DBNull.Value)
                {
                    if (Convert.ToChar(vo["FL_ACOMPANHANTE"]) == 'S')
                    {
                        row.Cells[18].Text = "SIM";
                    }
                    else
                    {
                        row.Cells[18].Text = "NÃO";
                    }
                }
                else
                {
                    row.Cells[18].Text = "";
                }

                btnExecutarCobranca.Visible = false;
                if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO) || hasPermissionRegional())
                {
                    DateTime? dtPendencia = tipoPendencia == null ? new DateTime?() : Convert.ToDateTime(vo["DT_PENDENCIA"]);
                    bool isSameUsuario = resp != null && resp.Id == UsuarioLogado.Id;
                    if (IndisponibilidadeRedeBO.Instance.PodeExecutarCobranca(status, tipoPendencia, dtPendencia, isSameUsuario))
                    {
                        btnExecutarCobranca.Visible = true;
                    }
                }

                */
            }
        }

        protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (this.IsPagingCommand(sender, e)) return;

                if ("CmdExecutarCobranca".Equals(e.CommandName))
                {
                    /*
                    int id = Convert.ToInt32(GetKeyCommand(e, gdvRelatorio).Value);
                    IndisponibilidadeRedeBO.Instance.ExecutarCobranca(id, UsuarioLogado.Usuario);
                    this.ShowInfo("Encaminhado ao setor do financeiro para execução da cobrança!");
                     */
                }

                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar dados", ex);
            }
        }

    }
}