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
	public partial class BuscaUniversitario : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdFiltroSituacao.Items.Add(new ListItem("TODOS", ""));
					foreach (int i in Enum.GetValues(typeof(StatusDeclaracaoUniversitario))) {
						dpdFiltroSituacao.Items.Add(new ListItem(DeclaracaoUniversitarioEnumTradutor.TraduzStatus((StatusDeclaracaoUniversitario)i), i.ToString()));
					}

				}
				catch (Exception ex) {
					Log.Error("Erro ao carregar a página", ex);
					this.ShowError("Erro ao carregar a página: " + ex.Message);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.UNIVERSITARIO; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
				LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;

				StatusDeclaracaoUniversitario status = (StatusDeclaracaoUniversitario)Convert.ToInt32(vo["CD_STATUS"]);
				string motivo = Convert.ToString(vo["ds_motivo_cancelamento"]);

				btnAprovar.Visible = status == StatusDeclaracaoUniversitario.PENDENTE;
				btnNegar.Visible = status == StatusDeclaracaoUniversitario.PENDENTE;

				row.Cells[4].Text = DeclaracaoUniversitarioEnumTradutor.TraduzStatus(status);

				if (!string.IsNullOrEmpty(motivo)) {
					row.Cells[4].Text += " - " + motivo;
				}
			}
		}

		private void AlterarStatusLinha(GridViewRow row, int index, StatusDeclaracaoUniversitario status) {
			TableCell cellDataAlt = row.Cells[3];
			TableCell cellStatus = row.Cells[4];

			int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_SOLICITACAO"]);
			LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
			LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;

			if (status == StatusDeclaracaoUniversitario.APROVADO) {
				DeclaracaoUniversitarioBO.Instance.Aprovar(cdProtocolo, UsuarioLogado.Id);
				cellStatus.Text = DeclaracaoUniversitarioEnumTradutor.TraduzStatus(status);

				this.ShowInfo("O formulário foi aprovado com sucesso!");

			} else if (status == StatusDeclaracaoUniversitario.RECUSADO) {
				DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioBO.Instance.GetById(cdProtocolo);
				cellStatus.Text = DeclaracaoUniversitarioEnumTradutor.TraduzStatus(status) + " - " + vo.MotivoCancelamento;
			}
			cellDataAlt.Text = DateTime.Now.ToString();
			btnAprovar.Visible = false;
			btnNegar.Visible = false;
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);

					// Retrieve the row that contains the button 
					// from the Rows collection.
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdAprovar") {
						AlterarStatusLinha(row, index, StatusDeclaracaoUniversitario.APROVADO);
					} else if (e.CommandName == "CmdCancelar") {
						AlterarStatusLinha(row, index, StatusDeclaracaoUniversitario.RECUSADO);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}

		protected void btnPesquisar_Click(object sender, EventArgs e) {
			string strStatus = dpdFiltroSituacao.SelectedValue;

			StatusDeclaracaoUniversitario? status = null;
			string matricula = null;
			int? cdProtocolo = null;

			if (!string.IsNullOrEmpty(strStatus))
				status = (StatusDeclaracaoUniversitario)Int32.Parse(strStatus);
			int iValue;
			long lValue;

			if (!string.IsNullOrEmpty(txtMatricula.Text)) {
				if (Int64.TryParse(txtMatricula.Text, out lValue)) {
                    matricula = txtMatricula.Text;
				}
			}

			if (!string.IsNullOrEmpty(txtProtocolo.Text)) {
				if (Int32.TryParse(txtProtocolo.Text, out iValue)) {
					cdProtocolo = iValue;
				}
			}

			DataTable dt = DeclaracaoUniversitarioBO.Instance.PesquisarDeclaracoes(cdProtocolo, matricula, null, null, status);
			this.ShowPagingGrid(gdvRelatorio, dt, "cd_solicitacao DESC");

			lblCount.Text = "Foram encontradas " + dt.Rows.Count + " solicitações!";
			btnExportar.Visible = dt.Rows.Count > 0;
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs["CD_STATUS"][0].Transformer = x => DeclaracaoUniversitarioEnumTradutor.TraduzStatus((StatusDeclaracaoUniversitario)Convert.ToInt32(x));

			ExportExcel("DeclaracoesUniversitario", defs, sourceTable);
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