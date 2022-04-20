using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Forms {
	public partial class BuscaReciprocidade : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdSituacao.Items.Add(new ListItem("TODOS", ""));
					foreach(int i in Enum.GetValues(typeof(StatusReciprocidade))) {
						dpdSituacao.Items.Add(new ListItem(((StatusReciprocidade)i).ToString(), i.ToString()));
					}

					btnNovo.Visible = this.HasPermission(Modulo.RECIPROCIDADE_GESTAO);
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RECIPROCIDADE_VIEW; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
				ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
				ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;
				LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
                Image imgSituacao = row.FindControl("imgSituacao") as Image;

				StatusReciprocidade status = (StatusReciprocidade)Convert.ToInt32(vo["cd_status"]);
                string motivo = (vo["ds_motivo_cancelamento"] == null) ? "" : Convert.ToString(vo["ds_motivo_cancelamento"]);

				if (this.HasPermission(Modulo.RECIPROCIDADE_GESTAO)) {
					btnAprovar.Visible = status == StatusReciprocidade.ENVIADO;
					btnEditar.Visible = status == StatusReciprocidade.PENDENTE;
					btnNegar.Visible = status == StatusReciprocidade.ENVIADO || status == StatusReciprocidade.PENDENTE;
				} else {
					btnAprovar.Visible = btnEditar.Visible = btnNegar.Visible = false;
				}
                
                row.Cells[6].Text = status.ToString();
                if (!string.IsNullOrEmpty(motivo))
                {
                    row.Cells[6].Text += " - " + motivo;
                }

                // Rotina de escolha da imagem da situação
                SituacaoReciprocidade situacao = (SituacaoReciprocidade)Convert.ToInt32(vo["cd_situacao"]);
                string urlImg = "";
                switch (situacao)
                {
                    case SituacaoReciprocidade.NORMAL:
                        urlImg = "~/img/progress_ok.png";
                        break;
                    case SituacaoReciprocidade.ALERTA:
                        urlImg = "~/img/progress_alert.png";
                        break;
                    case SituacaoReciprocidade.CRITICA:
                        urlImg = "~/img/progress_fail.png";
                        break;
                    default:
                        break;
                }
                imgSituacao.ImageUrl = urlImg;
                imgSituacao.ToolTip = ReciprocidadeEnumTradutor.TraduzSituacao(situacao);

			}
		}

		private void AlterarStatusLinha(GridViewRow row, int index, StatusReciprocidade status) {
			TableCell cellStatus = row.Cells[5];

			int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_SOLICITACAO"]);
			LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
			ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
			LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;

			if (status == StatusReciprocidade.APROVADO) {				
				cellStatus.Text = status.ToString();
				TableCell cellUsuarioAprovacao = row.Cells[8];
				TableCell cellDataAprovacao = row.Cells[9];
				cellUsuarioAprovacao.Text = UsuarioLogado.Usuario.Nome;
				cellDataAprovacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
			} else if (status == StatusReciprocidade.NEGADO) {
				ReciprocidadeVO vo = ReciprocidadeBO.Instance.GetById(cdProtocolo);
				cellStatus.Text = status.ToString() + " - " + vo.MotivoCancelamento;
			}
			btnAprovar.Visible = false;
			btnEditar.Visible = false;
			btnNegar.Visible = false;
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					if (IsPagingCommand(sender, e))
						return;
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);

					// Retrieve the row that contains the button 
					// from the Rows collection.
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdAprovar") {
						AlterarStatusLinha(row, index, StatusReciprocidade.APROVADO);
					} else if (e.CommandName == "CmdCancelar") {
						AlterarStatusLinha(row, index, StatusReciprocidade.NEGADO);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}
		
		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				string matricula = null;
				int? cdProtocolo = null;
				StatusReciprocidade? status = null;
				int iValue;
				long lValue;
				string protocoloAns = null;

				if (!string.IsNullOrEmpty(txtMatricula.Text)) {
					if (Int64.TryParse(txtMatricula.Text, out lValue)) {
                        matricula = txtMatricula.Text;
					}
				}

				if (!string.IsNullOrEmpty(txtProtocolo.Text))
					if (Int32.TryParse(txtProtocolo.Text, out iValue)) {
						cdProtocolo = iValue;
					}

				if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
					if (Int32.TryParse(dpdSituacao.SelectedValue, out iValue)) {
						status = (StatusReciprocidade)iValue;
					}
				}

				protocoloAns = txtProtocoloANS.Text;

				DataTable dt = ReciprocidadeBO.Instance.Pesquisar(matricula, cdProtocolo, protocoloAns, status);
				this.ShowPagingGrid(gdvRelatorio, dt, null);

				lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
				btnExportar.Visible = dt.Rows.Count > 0;
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar os dados!", ex);
			}
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);
			
			defs["CD_STATUS"][0].Transformer = x => ((StatusReciprocidade)Convert.ToInt32(x)).ToString();

			ExportExcel("SolicitacoesReciprocidade", defs, sourceTable);
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar!", ex);
			}
		}

	}
}