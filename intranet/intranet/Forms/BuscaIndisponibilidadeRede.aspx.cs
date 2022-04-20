using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.Util;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class BuscaIndisponibilidadeRede : RelatorioExcelPageBase {
		
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdUf.DataSource = LocatorDataBO.Instance.ListarUf().Select(x => new ListItem(x.Nome, x.Sigla));
					dpdUf.DataBind();
					dpdUf.Items.Insert(0, new ListItem("TODOS", ""));

					dpdSituacao.Items.Add(new ListItem("TODOS", ""));
					dpdSituacao.Items.Add(new ListItem("[APENAS EM ANDAMENTO]", "-1"));
					foreach (StatusIndisponibilidadeRede i in Enum.GetValues(typeof(StatusIndisponibilidadeRede))) {
						dpdSituacao.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzStatus(i), ((int)i).ToString()));
					}
					dpdSituacao.SelectedValue = "-1";

                    dpdPendencia.Items.Add(new ListItem("TODOS", ""));
                    dpdPendencia.Items.Add(new ListItem("[SEM PENDÊNCIA]", "-1"));
                    foreach (TipoPendenciaIndisponibilidadeRede i in Enum.GetValues(typeof(TipoPendenciaIndisponibilidadeRede)))
                    {
                        if (i != TipoPendenciaIndisponibilidadeRede.RECEBIDO && i != TipoPendenciaIndisponibilidadeRede.RESOLVIDO)
                        {
                            dpdPendencia.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzTipoPendencia(i), ((int)i).ToString()));
                        }
                    }

                    List<EncaminhamentoIndisponibilidadeRede> setor = new List<EncaminhamentoIndisponibilidadeRede>();
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO) ||
                        UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_DIRETORIA) ||
                        UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_REGIONAL) ||
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
                        setor.Add(EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.FINANCEIRO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.DIRETORIA);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.FATURAMENTO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.AUTORIZACAO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.REGIONAL);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.CADASTRO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.BENEFICIARIO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.SEFAT_CM);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.SEFAT_RB);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_ARARAQUARA);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_BELEM);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_BOAVISTA);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_CUIABA);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_IMPERATRIZ);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_MACAPA);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_MANAUS);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_MARABA);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_PALMAS);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_PORTOVELHO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_RIOBRANCO);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_SAOLUIS);
                        setor.Add(EncaminhamentoIndisponibilidadeRede.PAT_TUCURUI);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_FINANCEIRO))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.FINANCEIRO);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_FATURAMENTO))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.FATURAMENTO);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_AUTORIZACAO))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.AUTORIZACAO);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CADASTRO))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.CADASTRO);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_BENEFICIARIO))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.BENEFICIARIO);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SEFAT_CM))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.SEFAT_CM);
                    }
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_SEFAT_RB))
                    {
                        setor.Add(EncaminhamentoIndisponibilidadeRede.SEFAT_RB);
                    }

					if (setor.Count > 1) {
						dpdSetorAtual.Items.Add(new ListItem("TODOS", ""));
					}
					foreach (EncaminhamentoIndisponibilidadeRede set in setor) {
						dpdSetorAtual.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(set), set.ToString()));
					}
					if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO)) {
						dpdSetorAtual.SelectedValue = EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO.ToString();
					}

					btnNovo.Visible = UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_CREDENCIAMENTO) || hasPermissionRegional();

				} catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INDISPONIBILIDADE_REDE; }
		}

        private void Buscar()
        {
            long? matricula = null;
            int? cdProtocolo = null;
            string protocoloAns = null;
            List<StatusIndisponibilidadeRede> status = new List<StatusIndisponibilidadeRede>();
            int? pendencia = null;
            string uf = null;
            int? idMunicipio = null;
            int iValue;
            long lValue;
            string procedencia = "";
            bool? acompanhante = null;

            if (!string.IsNullOrEmpty(txtMatricula.Text))
            {
                if (Int64.TryParse(txtMatricula.Text, out lValue))
                {
                    matricula = lValue;
                }
            }

            if (!string.IsNullOrEmpty(txtProtocolo.Text))
                if (Int32.TryParse(txtProtocolo.Text, out iValue))
                {
                    cdProtocolo = iValue;
                }
                else
                {
                    this.ShowError("O protocolo deve ser numérico!");
                }

            if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue))
            {
                if (Int32.TryParse(dpdSituacao.SelectedValue, out iValue))
                {
                    if (iValue == -1)
                    {
                        foreach (StatusIndisponibilidadeRede i in Enum.GetValues(typeof(StatusIndisponibilidadeRede)))
                        {
                            if (i != StatusIndisponibilidadeRede.ENCERRADO)
                            {
                                status.Add(i);
                            }
                        }
                    }
                    else
                    {
                        status.Add((StatusIndisponibilidadeRede)iValue);
                    }
                }
            }
            if (!string.IsNullOrEmpty(txtProtocoloAns.Text))
            {
                protocoloAns = txtProtocoloAns.Text;
            }

            List<EncaminhamentoIndisponibilidadeRede> setor = new List<EncaminhamentoIndisponibilidadeRede>();

            if (string.IsNullOrEmpty(dpdSetorAtual.SelectedValue))
            {
                foreach (ListItem item in dpdSetorAtual.Items)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        setor.Add(IndisponibilidadeRedeEnumTradutor.TraduzSetor(item.Value));
                    }
                }
            }
            else
            {
                setor.Add(IndisponibilidadeRedeEnumTradutor.TraduzSetor(dpdSetorAtual.SelectedValue));
            }

            uf = dpdUf.SelectedValue;
            idMunicipio = null;
            if (!string.IsNullOrEmpty(dpdMunicipio.SelectedValue))
            {
                idMunicipio = Convert.ToInt32(dpdMunicipio.SelectedValue);
            }

            if (!string.IsNullOrEmpty(dpdPendencia.SelectedValue))
            {
                pendencia = Convert.ToInt32(dpdPendencia.SelectedValue);
            }

            if (!string.IsNullOrEmpty(dpdProcedencia.SelectedValue))
            {
                procedencia = dpdProcedencia.SelectedValue;
            }

            if (!string.IsNullOrEmpty(dpdAcompanhante.SelectedValue))
            {
                acompanhante = dpdAcompanhante.SelectedValue.Equals("S");
            }

            DataTable dt = IndisponibilidadeRedeBO.Instance.Pesquisar(matricula, cdProtocolo, protocoloAns, status, setor, uf, idMunicipio, pendencia, procedencia, acompanhante);
            this.ShowPagingGrid(gdvRelatorio, dt, null);

            lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
            btnExportar.Visible = dt.Rows.Count > 0;
        }	

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar dados", ex);
			}

		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs["NR_PRIORIDADE"][0].Transformer = x => IndisponibilidadeRedeEnumTradutor.TraduzPrioridade((PrioridadeIndisponibilidadeRede)Convert.ToInt32(x));
			defs["ID_SETOR_ENCAMINHAMENTO"][0].Transformer = x => IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento((EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(x));
			defs["ID_SITUACAO"][0].Transformer = x => IndisponibilidadeRedeEnumTradutor.TraduzStatus((StatusIndisponibilidadeRede)Convert.ToInt32(x));
			defs["VENCIMENTO"][0].StyleName = ExcelStyleDefinition.DATE;
			defs["NR_PROTOCOLO_ANS"][0].Width = 20;
			defs.Add(new ExcelColumnDefinition() {
				ColumnName = "TP_PENDENCIA",
				HeaderText = "Pendência",
				StyleName = ExcelStyleDefinition.TEXT,
				Width = 20,
				Transformer = x => x != null ? IndisponibilidadeRedeEnumTradutor.TraduzTipoPendencia((TipoPendenciaIndisponibilidadeRede)Convert.ToInt32(x)) : ""
			});
            defs.Add(new ExcelColumnDefinition()
            {
                ColumnName = "DS_PROCEDENCIA",
                HeaderText = "Procedência",
                StyleName = ExcelStyleDefinition.TEXT,
                Width = 20,
                Transformer = x => x != null ? (x.ToString() == "P" ? "PROCEDENTE" : "IMPROCEDENTE") : ""
            });

            defs["DIAS_ATRASO"][0].Transformer = x => Convert.ToInt32(x) == 0 ? "" : x;

			ExportExcel("IndisponibilidadeRede", defs, sourceTable);
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			} catch (Exception ex) {
				this.ShowError("Erro ao exportar!", ex);
			}
		}

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

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
            }
        }

        private bool isRegional(EncaminhamentoIndisponibilidadeRede setor)
        {
            bool flag = false;

            if(setor == EncaminhamentoIndisponibilidadeRede.REGIONAL ||
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

        private bool hasPermissionRegional() {

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

        protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (this.IsPagingCommand(sender, e)) return;

                if ("CmdExecutarCobranca".Equals(e.CommandName))
                {
                    int id = Convert.ToInt32(GetKeyCommand(e, gdvRelatorio).Value);
                    IndisponibilidadeRedeBO.Instance.ExecutarCobranca(id, UsuarioLogado.Usuario);
                    this.ShowInfo("Encaminhado ao setor do financeiro para execução da cobrança!");
                }

                if ("CmdReabrirSolicitacao".Equals(e.CommandName))
                {
                    string mensagem = "";
                    int id = Convert.ToInt32(GetKeyCommand(e, gdvRelatorio).Value);

                    IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(id);
                    vo.SetorEncaminhamento = EncaminhamentoIndisponibilidadeRede.DIRETORIA;

                    // Se o usuário logado é um DIRETOR
                    if (UsuarioLogado.HasPermission(Modulo.INDISPONIBILIDADE_REDE_DIRETORIA))
                    {
                        // Encaminha para a diretoria e assume a solicitação
                        IndisponibilidadeRedeBO.Instance.AssumirSolicitacao(vo, true, UsuarioLogado.Usuario);
                        mensagem = "Você reabriu a solicitação, encaminhou para a diretoria e assumiu a responsabilidade atual da mesma. Caso necessário, realize os ajustes e encaminhe-a para outro setor.";
                    }
                    else
                    {
                        // Encaminha a solicitação para a diretoria
                        IndisponibilidadeRedeBO.Instance.EncaminharSolicitacao(vo);
                        mensagem = "Você reabriu a solicitação e encaminhou a mesma para a diretoria.";
                    }

                    IndisponibilidadeRedeBO.Instance.ApagarEncerramento(vo);

                    this.ShowInfo(mensagem);
                }

                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar dados", ex);
            }
        }	

		protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				dpdMunicipio.Items.Clear();
				string uf = dpdUf.SelectedValue;
				if (!string.IsNullOrEmpty(uf)) {
					DataTable dtMunicipios = PLocatorDataBO.Instance.BuscarMunicipiosProtheus(uf);
					dpdMunicipio.Items.AddRange(dtMunicipios.AsEnumerable().Select(x =>
                        new ListItem(Convert.ToString(x["BID_DESCRI"]), Convert.ToString(x["BID_CODMUN"]))).ToArray());
					dpdMunicipio.Items.Insert(0, new ListItem("TODOS", ""));
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao selecionar UF.", ex);
			}
		}

	}
}