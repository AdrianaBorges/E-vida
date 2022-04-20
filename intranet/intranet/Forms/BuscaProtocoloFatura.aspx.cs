using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
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
	public partial class BuscaProtocoloFatura : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdSituacao.Items.Add(new ListItem("TODOS", ""));
				/*foreach (int i in Enum.GetValues(typeof(StatusProtocoloFatura))) {
					dpdSituacao.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzStatus((StatusProtocoloFatura)i), i.ToString()));
				}*/
                foreach (int i in Enum.GetValues(typeof(FaseProtocoloFatura)))
                {
                    dpdSituacao.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzFase((FaseProtocoloFatura)i), i.ToString()));
                }

                List<PClasseRedeAtendimentoVO> lstEspecialidade = PLocatorDataBO.Instance.ListarClasseRedeAtendimento();
                dpdNatureza.DataSource = lstEspecialidade;
				dpdNatureza.DataBind();
				dpdNatureza.Items.Insert(0, new ListItem("TODAS", ""));

				List<KeyValuePair<string, string>> lstRegioes = PLocatorDataBO.Instance.ListarRegioes();
                dpdRegional.DataSource = lstRegioes;
				dpdRegional.DataBind();
				dpdRegional.Items.Insert(0, new ListItem("TODAS", ""));

				dpdControle.Items.Add(new ListItem("TODOS", ""));
				dpdControle.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzControle(ProtocoloFaturaVO.CONTROLE_OK), ProtocoloFaturaVO.CONTROLE_OK.ToString()));
				dpdControle.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzControle(ProtocoloFaturaVO.CONTROLE_ALERTA), ProtocoloFaturaVO.CONTROLE_ALERTA.ToString()));
				dpdControle.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzControle(ProtocoloFaturaVO.CONTROLE_ATRASADO), ProtocoloFaturaVO.CONTROLE_ATRASADO.ToString()));
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.PROTOCOLO_FATURA; }
		}
		
		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

            defs["BAU_CPFCGC"][0].Transformer = x => FormatUtil.TryFormatCpfCnpj(x);
			/*defs.Add(new ExcelColumnDefinition() {
				HeaderText = "Situação",
				ColumnName = "ID_SITUACAO",
				StyleName = ExcelStyleDefinition.TEXT,
				Transformer = x => ProtocoloFaturaEnumTradutor.TraduzStatus((StatusProtocoloFatura)Convert.ToInt32(x))
			});*/
            defs.Add(new ExcelColumnDefinition()
            {
                HeaderText = "Situação",
                ColumnName = "BCI_FASE",
                StyleName = ExcelStyleDefinition.TEXT,
                Transformer = x => ProtocoloFaturaEnumTradutor.TraduzFase((FaseProtocoloFatura)Convert.ToInt32(x))
            });
			defs.Add(new ExcelColumnDefinition() {
				HeaderText = "Unidade",
                ColumnName = "BAU_REGMUN",
				StyleName = ExcelStyleDefinition.TEXT,
				Transformer = x => GetRegional(x)
			});
			defs.Add(new ExcelColumnDefinition() {
				HeaderText = "Pendência",
				ColumnName = "DS_PENDENCIA",
				StyleName = ExcelStyleDefinition.TEXT
			});

			ExportExcel("ProtocoloFatura", defs, sourceTable);
		}

		private void Buscar() {
			string nrProtocolo = null;
			string nrCpfCnpj = null;
			//StatusProtocoloFatura? status = null;
            FaseProtocoloFatura? fase = null;
			string razaoSocial = txtRazaoSocial.Text;
			string docFiscal = txtDocFiscal.Text;
			DateTime? dtEmissao = null;
			DateTime? dtEntradaInicio = null;
			DateTime? dtEntradaFim = null;
			decimal? vlApresentado = null;
			DateTime? dtVencimentoInicio = null;
			DateTime? dtVencimentoFim = null;
			DateTime? dtFinalizacaoInicio = null;
			DateTime? dtFinalizacaoFim = null;
			DateTime? dtExpedicaoInicio = null;
			DateTime? dtExpedicaoFim = null;
			string cdNatureza = null;
            string cdRegional = null;
			int? controle = null;

			long lValue;
			DateTime dtValue;
			decimal dValue;
			
			txtCpfCnpj.Text = FormatUtil.OnlyNumbers(txtCpfCnpj.Text);

			if (!string.IsNullOrEmpty(txtCpfCnpj.Text)) {
				if (Int64.TryParse(txtCpfCnpj.Text, out lValue)) {
                    nrCpfCnpj = txtCpfCnpj.Text;
				} else {
					this.ShowError("O CPF/CNPJ deve ser numérico!");
					return;
				}
			}

			txtProtocolo.Text = txtProtocolo.Text.Trim();
			if (!string.IsNullOrEmpty(txtProtocolo.Text))
				nrProtocolo = txtProtocolo.Text;
				

			if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
				//status = (StatusProtocoloFatura)Convert.ToInt32(dpdSituacao.SelectedValue);
                fase = (FaseProtocoloFatura)Convert.ToInt32(dpdSituacao.SelectedValue);
			}
			if (!string.IsNullOrEmpty(txtDataEmissao.Text)) {
				if (!DateTime.TryParse(txtDataEmissao.Text, out dtValue)) {
					this.ShowError("A data de emissão está inválida!");
					return;
				}
				dtEmissao = dtValue;				
			}
			if (!string.IsNullOrEmpty(txtValor.Text)) {
				if (!Decimal.TryParse(txtValor.Text, out dValue)) {
					this.ShowError("O valor está inválido!");
					return;
				}
				vlApresentado = dValue;
			}
			if (!string.IsNullOrEmpty(txtDataEntradaInicio.Text)) {
				if (!DateTime.TryParse(txtDataEntradaInicio.Text, out dtValue)) {
					this.ShowError("A data de entrada inicial está inválida!");
					return;
				}
				dtEntradaInicio = dtValue; 				
			}
			if (!string.IsNullOrEmpty(txtDataEntradaFim.Text)) {
				if (!DateTime.TryParse(txtDataEntradaFim.Text, out dtValue)) {
					this.ShowError("A data de entrada final está inválida!");
					return;
				}
				dtEntradaFim = dtValue;				
			}
			if (dtEntradaInicio != null || dtEntradaFim != null) {
				if (dtEntradaInicio == null || dtEntradaFim == null) {
					this.ShowError("Para a entrada deve ser informado início e fim do período!");
					return;
				} else {
					if (dtEntradaInicio > dtEntradaFim) {
						this.ShowError("A data início do período de entrada deve ser menor que a data fim!");
						return;
					}
				}
			}
			if (!string.IsNullOrEmpty(txtVencimentoInicio.Text)) {
				if (!DateTime.TryParse(txtVencimentoInicio.Text, out dtValue)) {
					this.ShowError("A data de vencimento inicial está inválida!");
					return;
				}
				dtVencimentoInicio = dtValue;
			}
			if (!string.IsNullOrEmpty(txtVencimentoFim.Text)) {
				if (!DateTime.TryParse(txtVencimentoFim.Text, out dtValue)) {
					this.ShowError("A data de vencimento final está inválida!");
					return;
				}
				dtVencimentoFim = dtValue;
			}
			if (dtVencimentoInicio != null || dtVencimentoFim != null) {
				if (dtVencimentoInicio == null || dtVencimentoFim == null) {
					this.ShowError("Para o vencimento deve ser informado início e fim do período!");
					return;
				} else {
					if (dtVencimentoInicio > dtVencimentoFim) {
						this.ShowError("A data início do período do vencimento deve ser menor que a data fim!");
						return;
					}
				}
			}

			if (!string.IsNullOrEmpty(txtFinalizacaoInicio.Text)) {
				if (!DateTime.TryParse(txtFinalizacaoInicio.Text, out dtValue)) {
					this.ShowError("A data de finalização inicial está inválida!");
					return;
				}
				dtFinalizacaoInicio = dtValue;
			}
			if (!string.IsNullOrEmpty(txtFinalizacaoFim.Text)) {
				if (!DateTime.TryParse(txtFinalizacaoFim.Text, out dtValue)) {
					this.ShowError("A data de finalização final está inválida!");
					return;
				}
				dtFinalizacaoFim = dtValue;
			}
			if (dtFinalizacaoInicio != null || dtFinalizacaoFim != null) {
				if (dtFinalizacaoInicio == null || dtFinalizacaoFim == null) {
					this.ShowError("Para a finalização deve ser informado início e fim do período!");
					return;
				} else {
					if (dtFinalizacaoInicio > dtFinalizacaoFim) {
						this.ShowError("A data início do período de finalização deve ser menor que a data fim!");
						return;
					}
				}
			}

			if (!string.IsNullOrEmpty(txtExpedicaoInicio.Text)) {
				if (!DateTime.TryParse(txtExpedicaoInicio.Text, out dtValue)) {
					this.ShowError("A data de expedição inicial está inválida!");
					return;
				}
				dtExpedicaoInicio = dtValue;
			}
			if (!string.IsNullOrEmpty(txtExpedicaoFim.Text)) {
				if (!DateTime.TryParse(txtExpedicaoFim.Text, out dtValue)) {
					this.ShowError("A data de expedição final está inválida!");
					return;
				}
				dtExpedicaoFim = dtValue;
			}
			if (dtExpedicaoInicio != null || dtExpedicaoFim != null) {
				if (dtExpedicaoInicio == null || dtExpedicaoFim == null) {
					this.ShowError("Para a expedição deve ser informado início e fim do período!");
					return;
				} else {
					if (dtExpedicaoInicio > dtExpedicaoFim) {
						this.ShowError("A data início do período de expedição deve ser menor que a data fim!");
						return;
					}
				}
			}

			if (!string.IsNullOrEmpty(dpdNatureza.SelectedValue)) {
				cdNatureza = dpdNatureza.SelectedValue;
			}
			if (!string.IsNullOrEmpty(dpdRegional.SelectedValue)) {
				cdRegional = dpdRegional.SelectedValue;
			}

			if (!string.IsNullOrEmpty(dpdControle.SelectedValue)) {
				controle = Int32.Parse(dpdControle.SelectedValue);
			}

			List<int> lstAnalistasResp = new List<int>();
			DataTable dtAnalResp = GetTableUsuario();
			if (dtAnalResp != null) {
				foreach (DataRow dr in dtAnalResp.Rows) {
					lstAnalistasResp.Add((int)dr["cd_usuario"]);
				}
			}

            String protDuplicados = "";
            String strDuplicados = "";

            DataTable dtDuplicados = ProtocoloFaturaBO.Instance.ObterDuplicados();
            if (dtDuplicados != null)
            {
                foreach(DataRow linha in dtDuplicados.Rows)
                {
                    if (!String.IsNullOrEmpty(protDuplicados))
                    {
                        protDuplicados += ", ";
                        strDuplicados += ", ";
                    }

                    protDuplicados += linha[0];
                    strDuplicados += linha[0] + " (Locais de Digitação: " + linha[1] + ")";
                }
            }

            ProtocoloFaturaBO.Instance.Mesclar(protDuplicados);

			DataTable dt = ProtocoloFaturaBO.Instance.Pesquisar(nrProtocolo, nrCpfCnpj, razaoSocial, lstAnalistasResp, docFiscal, 
				dtEmissao, vlApresentado, dtEntradaInicio, dtEntradaFim, fase, dtVencimentoInicio, dtVencimentoFim, 
				dtFinalizacaoInicio, dtFinalizacaoFim, dtExpedicaoInicio, dtExpedicaoFim, cdNatureza, cdRegional, controle);

			this.ShowPagingGrid(gdvRelatorio, dt, "cd_protocolo_fatura DESC");

			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";

            lblDuplicados.Text = "";
            if (!String.IsNullOrEmpty(strDuplicados.Trim()))
            {
                lblDuplicados.Text = " Os seguintes PEGs estão duplicados no Protheus: " + strDuplicados + ".";
            }

			btnExportar.Visible = dt.Rows.Count > 0;
		}

		private void AlterarStatusLinha(GridViewRow row, int index, StatusProtocoloFatura status) {

			//ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
			LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
			//Literal litSituacao = row.FindControl("litSituacao") as Literal;

			//int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_PROTOCOLO_FATURA"]);

			//ProtocoloFaturaVO vo = ProtocoloFaturaBO.Instance.GetById(cdProtocolo);
			//if (status == StatusProtocoloFatura.CANCELADO) {
			//	litSituacao.Text = ProtocoloFaturaEnumTradutor.TraduzStatus(status) + " - " + vo.MotivoCancelamento;
			//}
			btnNegar.Visible = false;
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar registros!", ex);
			}
		}

		protected void btnLimpar_Click(object sender, EventArgs e) {
			txtProtocolo.Text = string.Empty;

			txtCpfCnpj.Text = string.Empty;
			txtDataEmissao.Text = string.Empty;
			txtDataEntradaInicio.Text = string.Empty;
			txtDataEntradaFim.Text = string.Empty;
			txtDocFiscal.Text = string.Empty;
			txtProtocolo.Text = string.Empty;
			txtRazaoSocial.Text = string.Empty;
			txtValor.Text = string.Empty;
			txtVencimentoFim.Text = string.Empty;
			txtVencimentoInicio.Text = string.Empty;
			dpdNatureza.SelectedValue = string.Empty;
			dpdSituacao.SelectedValue = string.Empty;
			dpdRegional.SelectedValue = string.Empty;
			txtExpedicaoInicio.Text = string.Empty;
			txtExpedicaoFim.Text = string.Empty;

			LimparUsuarios();
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
				ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;
				LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
				Literal litSituacao = row.FindControl("litSituacao") as Literal;
				Image imgControle = row.FindControl("imgControle") as Image;
				Literal litUnidade = row.FindControl("litUnidade") as Literal;
				Literal litPendencia = row.FindControl("litPendencia") as Literal;
				Literal litResposta = row.FindControl("litResposta") as Literal;

                string motivo = Convert.ToString(vo["ds_motivo_cancelamento"]);
                StatusProtocoloFatura status = (StatusProtocoloFatura)Convert.ToInt32(vo["ID_SITUACAO"]);
                //litSituacao.Text = ProtocoloFaturaEnumTradutor.TraduzStatus(status);
                //if (!string.IsNullOrEmpty(motivo)) {
                //	litSituacao.Text += " - " + motivo;
                //}

                FaseProtocoloFatura fase = (FaseProtocoloFatura)Convert.ToInt32(vo["BCI_FASE"]);
                litSituacao.Text = ProtocoloFaturaEnumTradutor.TraduzFase(fase);

				btnPdf.Visible = status != StatusProtocoloFatura.PROTOCOLADO;
				if (vo["analista_ID_USUARIO"] != DBNull.Value) {
					if (UsuarioLogado.Id == Convert.ToInt32(vo["analista_ID_USUARIO"])) {
						btnNegar.Visible = (status != StatusProtocoloFatura.CANCELADO && status != StatusProtocoloFatura.FINALIZADO);
					} else {
						btnNegar.Visible = false;
					}
				} else {
					btnNegar.Visible = false;
				}

				DateTime dtVencimento = Convert.ToDateTime(vo["dt_vencimento"]);
				DateTime? dtFinalizacao = vo["dt_finalizacao"] != DBNull.Value ? Convert.ToDateTime(vo["dt_finalizacao"]) : new DateTime?();
				string urlImg = "";
				switch (ProtocoloFaturaEnumTradutor.CalcularControle(dtVencimento, dtFinalizacao)) {
					case ProtocoloFaturaVO.CONTROLE_OK:
						urlImg = "~/img/progress_ok.png";
						break;
					case ProtocoloFaturaVO.CONTROLE_ALERTA:
						urlImg = "~/img/progress_alert.png";
						break;
					case ProtocoloFaturaVO.CONTROLE_ATRASADO:
						urlImg = "~/img/progress_fail.png";
						break;
					default:
						break;
				}
				imgControle.ImageUrl = urlImg;

				litUnidade.Text = GetRegional(vo["BAU_REGMUN"]);

				litPendencia.Text = Convert.ToString(vo["ds_pendencia"]);

				btnEditar.Visible = status != StatusProtocoloFatura.CANCELADO;
			}
		}

		private string GetRegional(object o) {
			if (o == DBNull.Value)
				return "";
			string cdRegiao = Convert.ToString(o);
            if (cdRegiao != "")
                return PLocatorDataBO.Instance.GetRegiao(cdRegiao);
			return "";
		}

		#region Analista Responsavel

		private DataTable GetTableUsuario() {

			DataTable dt = ViewState["GDV_USUARIO"] as DataTable;
			if (dt == null) {
				dt = new DataTable();
				dt.Columns.Add(new DataColumn("cd_usuario", typeof(int)));
				dt.Columns.Add(new DataColumn("nm_usuario", typeof(string)));
				ViewState["GDV_USUARIO"] = dt;
			}
			return dt;
		}

		private void BindGridUsuario() {
			DataTable dt = GetTableUsuario();
			gdvUsuarios.DataSource = dt;
			gdvUsuarios.DataBind();
			if (dt != null && dt.Rows.Count > 0)
				btnLimparUsuario.Visible = true;
			else
				btnLimparUsuario.Visible = false;
		}

		private void LimparUsuarios() {
			DataTable dt = GetTableUsuario();

			dt.Rows.Clear();

			BindGridUsuario();
		}

		private void AdicionarUsuario(int id) {
			DataTable dt = GetTableUsuario();
			AdicionarUsuarioUnico(dt, id);
			BindGridUsuario();
		}

		private void AdicionarUsuarioUnico(DataTable dt, int id) {
			DataView dv = new DataView(dt);
			dv.RowFilter = " CD_USUARIO = " + id;
			if (dv.Count > 0) {
				this.ShowInfo("Usuário já existe na lista de filtros!");
				return;
			}

			UsuarioVO vo = UsuarioBO.Instance.GetUsuarioById(id);
			if (vo != null) {
				DataRow dr = dt.NewRow();
				dr["cd_usuario"] = id;		
				dr["nm_usuario"] = vo.Nome.ToUpper();
				dt.Rows.Add(dr);
			} else {
				this.ShowError("Usuário não encontrado!");
			}
		}
		
		private void RemoverUsuario(int index) {
			DataTable dt = GetTableUsuario();
			dt.Rows.RemoveAt(index);
			BindGridUsuario();
		}

		protected void gdvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName.Equals("RemoverUsuario")) {
				try {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);
					RemoverUsuario(index);
				} catch (Exception ex) {
					this.ShowError("Erro ao exportar!", ex);
				}

			}
		}

		#endregion

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (IsPagingCommand(sender, e)) return;

				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					int index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdNegar") {
						AlterarStatusLinha(row, index, StatusProtocoloFatura.CANCELADO);
					}
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}

		protected void btnLocCred_Click(object sender, ImageClickEventArgs e) {
			try {
				string credValue = hidCredenciado.Value;
				if (string.IsNullOrEmpty(credValue)) {
					txtCpfCnpj.Text = txtRazaoSocial.Text = string.Empty;
					txtCpfCnpj.Enabled = txtRazaoSocial.Enabled = true;
				} else {
                    PRedeAtendimentoVO credVO = PRedeAtendimentoBO.Instance.GetById(credValue);
					txtCpfCnpj.Text = credVO.Cpfcgc;
					txtRazaoSocial.Text = credVO.Nome;
					txtCpfCnpj.Enabled = txtRazaoSocial.Enabled = false;
				}

			} catch (Exception ex) {
				this.ShowError("Erro ao selecionar credenciado!", ex);
			}
		}

		protected void btnClrCred_Click(object sender, ImageClickEventArgs e) {
			txtCpfCnpj.Text = txtRazaoSocial.Text = string.Empty;
			txtCpfCnpj.Enabled = txtRazaoSocial.Enabled = true;
			hidCredenciado.Value = string.Empty;
		}
		
		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			} catch (Exception ex) {
				this.ShowError("Erro ao exportar!", ex);
			}
		}

		protected void btnAdicionarUsuario_Click(object sender, EventArgs e) {
			try {
				int id;
				if (!Int32.TryParse(hidCodAnalista.Value, out id)) {
					return;
				}

				AdicionarUsuario(id);
			} catch (Exception ex) {
				this.ShowError("Erro ao adicionar usuário nos filtros!", ex);
			}
		}

		protected void btnLimparUsuario_Click(object sender, EventArgs e) {
			try {
				LimparUsuarios();
			} catch (Exception ex) {
				this.ShowError("Erro ao limpar usuários nos filtros!", ex);
			}
		}
	}
}
