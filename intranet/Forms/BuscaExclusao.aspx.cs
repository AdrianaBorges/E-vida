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
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Forms {
	public partial class BuscaExclusao : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdSituacao.Items.Add(new ListItem("TODOS", ""));
					foreach(int i in Enum.GetValues(typeof(StatusExclusao))) {
						dpdSituacao.Items.Add(new ListItem(ExclusaoEnumTradutor.TraduzStatus((StatusExclusao)i), i.ToString()));
					}

					if (!HasPermission(Modulo.FORM_EXCLUSAO_CREATE)) {
						btnNovo.Visible = false;
					}

				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página: ", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.EXCLUSAO; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
				LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
				LinkButton btnSolDoc = row.FindControl("btnSolDoc") as LinkButton;

				bool hasAprovPermission = HasPermission(Modulo.FORM_EXCLUSAO_APROV_NEG);
				StatusExclusao status = (StatusExclusao)Convert.ToInt32(vo["cd_status"]);
				string motivo = Convert.ToString(vo["ds_motivo_cancelamento"]);

				bool canAprovarNegar = (status == StatusExclusao.AGUARDANDO_DOCUMENTACAO || status == StatusExclusao.PENDENTE) && hasAprovPermission;
				btnNegar.Visible = btnAprovar.Visible = canAprovarNegar;
				btnSolDoc.Visible = status == StatusExclusao.PENDENTE && hasAprovPermission;

				row.Cells[5].Text = ExclusaoEnumTradutor.TraduzStatus(status);
				if (!string.IsNullOrEmpty(motivo)) {
					row.Cells[5].Text += " - " + motivo;
				}
			}
		}

		private void AlterarStatusLinha(GridViewRow row, int index, StatusExclusao status) {
			TableCell cellDataAlt = row.Cells[4];
			TableCell cellStatus = row.Cells[5];

			int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_SOLICITACAO"]);
			LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
			LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
			LinkButton btnSolDoc = row.FindControl("btnSolDoc") as LinkButton;

			if (status == StatusExclusao.APROVADO) {
				byte[] anexo = null;
				if (ParametroUtil.EmailEnabled) {
					ReportExclusao rpt = new ReportExclusao(ReportDir, UsuarioLogado);
					ExclusaoVO vo = FormExclusaoBO.Instance.GetById(cdProtocolo);
					anexo = rpt.GerarRelatorio(vo);
				}
				FormExclusaoBO.Instance.Aprovar(cdProtocolo, UsuarioLogado.Id, anexo);
				cellStatus.Text = ExclusaoEnumTradutor.TraduzStatus(status);
				this.ShowInfo("O formulário foi aprovado com sucesso!");
				
			} else if (status == StatusExclusao.NEGADO) {
				ExclusaoVO vo = FormExclusaoBO.Instance.GetById(cdProtocolo);
				cellStatus.Text = ExclusaoEnumTradutor.TraduzStatus(status) + " - " + vo.MotivoCancelamento;
			} else if (status == StatusExclusao.AGUARDANDO_DOCUMENTACAO) {
				FormExclusaoBO.Instance.AguardarDocumentacao(cdProtocolo, UsuarioLogado.Id);
				cellStatus.Text = ExclusaoEnumTradutor.TraduzStatus(status);
				this.ShowInfo("Situação alterada com sucesso!");
			}
			cellDataAlt.Text = DateTime.Now.ToString();

			btnSolDoc.Visible = false;
			if (status == StatusExclusao.AGUARDANDO_DOCUMENTACAO) return;

			btnAprovar.Visible = false;
			btnNegar.Visible = false;
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (this.IsPagingCommand(sender, e)) return;

				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);

					// Retrieve the row that contains the button 
					// from the Rows collection.
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdAprovar") {
						AlterarStatusLinha(row, index, StatusExclusao.APROVADO);
					} else if (e.CommandName == "CmdCancelar") {
						AlterarStatusLinha(row, index, StatusExclusao.NEGADO);
					} else if (e.CommandName == "CmdDoc") {
						AlterarStatusLinha(row, index, StatusExclusao.AGUARDANDO_DOCUMENTACAO);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}
		
		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				long? matricula = null;
				int? cdProtocolo = null;
				StatusExclusao? status = null;
				int iValue;
				long lValue;

				if (!string.IsNullOrEmpty(txtMatricula.Text)) {
					if (Int64.TryParse(txtMatricula.Text, out lValue)) {
						matricula = lValue;
					}
				}

				if (!string.IsNullOrEmpty(txtProtocolo.Text))
					if (Int32.TryParse(txtProtocolo.Text, out iValue)) {
						cdProtocolo = iValue;
					}

				if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
					if (Int32.TryParse(dpdSituacao.SelectedValue, out iValue)) {
						status = (StatusExclusao)iValue;
					}
				}

				DataTable dt = FormExclusaoBO.Instance.Pesquisar(matricula, cdProtocolo, status);
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

			defs["CD_STATUS"][0].Transformer = x => ExclusaoEnumTradutor.TraduzStatus((StatusExclusao)Convert.ToInt32(x));

			ExportExcel("SolicitacoesExclusao", defs, sourceTable);
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