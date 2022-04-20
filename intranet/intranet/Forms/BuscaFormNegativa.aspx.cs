using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Forms {
	public partial class BuscaFormNegativa : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					if (!this.HasPermission(eVidaGeneralLib.VO.Modulo.APROVAR_NEGATIVA))
						gdvRelatorio.Columns[6].Visible = false;

					dpdSituacao.Items.Add(new ListItem("TODOS", ""));
					dpdSituacao.Items.Add(new ListItem("SOB ANÁLISE", ((int)FormNegativaStatus.SOB_ANALISE).ToString()));
					dpdSituacao.Items.Add(new ListItem("APROVADO", ((int)FormNegativaStatus.APROVADO).ToString()));
					dpdSituacao.Items.Add(new ListItem("CANCELADO", ((int)FormNegativaStatus.CANCELADO).ToString()));

					dpdSituacaoReanalise.Items.Add(new ListItem("TODOS", ""));
					dpdSituacaoReanalise.Items.Add(new ListItem("SOB ANÁLISE", ((int)FormNegativaReanaliseStatus.SOB_ANALISE).ToString()));
					dpdSituacaoReanalise.Items.Add(new ListItem("FINALIZADO", ((int)FormNegativaReanaliseStatus.FINALIZADO).ToString()));
					dpdSituacaoReanalise.Items.Add(new ListItem("DEVOLVIDO", ((int)FormNegativaReanaliseStatus.DEVOLVIDO).ToString()));

				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.NEGATIVA; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				LinkButton btnFinalizar = row.FindControl("btnAprovar") as LinkButton;
				LinkButton btnCancelar = row.FindControl("btnCancelar") as LinkButton;
				LinkButton btnReanalise = row.FindControl("btnReanalise") as LinkButton;
				ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;
				ImageButton btnPrint = row.FindControl("btnPrint") as ImageButton;
				TableCell cellStatus = row.Cells[3];
				TableCell cellStatusReanalise = row.Cells[4];

				string status = Convert.ToString(vo["CD_STATUS"]);

				btnFinalizar.OnClientClick = "return confirm('Deseja realmente aprovar esta solicitação?')";
				btnPrint.Visible = false;
				btnPdf.Visible = false;
				btnReanalise.Visible = false;
				if (status == FormNegativaStatus.SOB_ANALISE.ToString()) {
					cellStatus.Text = "SOB ANÁLISE";
					btnFinalizar.Visible = true;
				} else if (status == FormNegativaStatus.APROVADO.ToString()) {
					cellStatus.Text = "APROVADO";
					btnFinalizar.Visible = false;
					btnCancelar.Visible = this.HasPermission(eVidaGeneralLib.VO.Modulo.APROVAR_NEGATIVA);
					btnPrint.Visible = true;
					btnPdf.Visible = true;
					btnReanalise.Visible = true;
					if (vo["CD_STATUS_REANALISE"] != DBNull.Value) {
						FormNegativaReanaliseStatus statusReanalise = (FormNegativaReanaliseStatus)Convert.ToInt32(vo["cd_status_reanalise"]);
						if (statusReanalise == FormNegativaReanaliseStatus.DEVOLVIDO) {
							cellStatusReanalise.Text = "DEVOLVIDO";
						} else if (statusReanalise == FormNegativaReanaliseStatus.SOB_ANALISE) {
							cellStatusReanalise.Text = "SOB ANÁLISE";
						} else {
							cellStatusReanalise.Text = "FINALIZADO";
						}
					}
				} else {
					string motivo = Convert.ToString(vo["DS_MOTIVO_CANCELAMENTO"]);
					cellStatus.Text = "CANCELADO - " + motivo;
					cellStatus.ToolTip = motivo;
					btnFinalizar.Visible = false;
					btnCancelar.Visible = false;
				}

			}
		}

		private void AlterarStatusLinha(GridViewRow row, int index, FormNegativaStatus status) {
			LinkButton btnFinalizar = row.FindControl("btnAprovar") as LinkButton;
			LinkButton btnCancelar = row.FindControl("btnCancelar") as LinkButton;
			LinkButton btnReanalise = row.FindControl("btnReanalise") as LinkButton;
			TableCell cellStatus = row.Cells[3];

			int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_SOLICITACAO"]);

			btnReanalise.Visible = false;
			if (status == FormNegativaStatus.APROVADO) {
				FormNegativaBO.Instance.Aprovar(cdProtocolo, UsuarioLogado.Id, false);
				cellStatus.Text = status.ToString();

				btnReanalise.Visible = true;
				btnCancelar.Visible = this.HasPermission(eVidaGeneralLib.VO.Modulo.APROVAR_NEGATIVA);				
			} else if (status == FormNegativaStatus.CANCELADO) {
				FormNegativaVO vo = FormNegativaBO.Instance.GetById(cdProtocolo);
				cellStatus.Text = status.ToString() + " - " + vo.MotivoCancelamento;

				btnCancelar.Visible = false;
			}

			btnFinalizar.Visible = false;
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (IsPagingCommand(sender, e)) return;

				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);

					// Retrieve the row that contains the button 
					// from the Rows collection.
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdFinalizar") {
						AlterarStatusLinha(row, index, FormNegativaStatus.APROVADO);
					} else if (e.CommandName == "CmdCancelar") {
						AlterarStatusLinha(row, index, FormNegativaStatus.CANCELADO);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! ", ex);
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("./FormNegativa.aspx");
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				int cdProtocolo = 0;

				FormNegativaReanaliseStatus? statusReanalise = null;
				FormNegativaStatus? status = null;
				string protocoloAns = txtProtocoloANS.Text;
				string nomeBenef = txtNomeBeneficiario.Text;

				Int32.TryParse(txtProtocolo.Text, out cdProtocolo);

				if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
					status = (FormNegativaStatus)Convert.ToInt32(dpdSituacao.SelectedValue);
				}

				if (!string.IsNullOrEmpty(dpdSituacaoReanalise.SelectedValue)) {
					statusReanalise = (FormNegativaReanaliseStatus)Convert.ToInt32(dpdSituacaoReanalise.SelectedValue);
				}

				DataTable dt = FormNegativaBO.Instance.Pesquisar(nomeBenef, cdProtocolo, protocoloAns, dpdTipo.SelectedValue,
					txtMascara.Text, txtDesServico.Text, status, statusReanalise);
				gdvRelatorio.DataSource = dt;
				gdvRelatorio.DataBind();

				lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar os dados!", ex);
			}
		}

	}
}