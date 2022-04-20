using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Forms {
	public partial class BuscaAutorizacaoProvisoria : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdSituacao.Items.Add(new ListItem("TODOS", ""));
				foreach (int i in Enum.GetValues(typeof(StatusAutorizacaoProvisoria))) {
					dpdSituacao.Items.Add(new ListItem(AutorizacaoProvisoriaEnumTradutor.TraduzSituacao((StatusAutorizacaoProvisoria)i), i.ToString()));
				}
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.AUTORIZACAO_PROVISORIA; }
		}

		private void Buscar() {
			string matricula = null;
			int? cdProtocolo = null;
			StatusAutorizacaoProvisoria? status = null;
			int iValue;
			long lValue;

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
				status = (StatusAutorizacaoProvisoria)Convert.ToInt32(dpdSituacao.SelectedValue);
			}

			DataTable dt = AutorizacaoProvisoriaBO.Instance.Pesquisar(matricula, cdProtocolo, status);
			this.ShowPagingGrid(gdvRelatorio, dt, null);

			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";

            btnExportar.Visible = gdvRelatorio.Rows.Count > 0;
		}

		private void AlterarStatusLinha(GridViewRow row, int index, StatusAutorizacaoProvisoria status) {

			LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
			ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
			ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;
			LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
			TableCell cellStatus = row.Cells[7];

			int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_SOLICITACAO"]);

			AutorizacaoProvisoriaVO vo = AutorizacaoProvisoriaBO.Instance.GetById(cdProtocolo);
			if (status == StatusAutorizacaoProvisoria.APROVADO) {
				ReportAutorizacaoProvisoria rpt = new ReportAutorizacaoProvisoria(ReportDir, UsuarioLogado);
				byte[] anexo = rpt.GerarRelatorio(vo);
				AutorizacaoProvisoriaBO.Instance.Aprovar(vo, UsuarioLogado.Id, anexo);
				this.ShowInfo("Autorização Provisória gerada com sucesso!");
				cellStatus.Text = AutorizacaoProvisoriaEnumTradutor.TraduzSituacao(status);
				btnNegar.Visible = HasPermission(Modulo.APROVAR_AUTORIZACAO_PROVISORIA);
			} else if (status == StatusAutorizacaoProvisoria.NEGADO) {
				cellStatus.Text = AutorizacaoProvisoriaEnumTradutor.TraduzSituacao(status) + " - " + vo.MotivoCancelamento;				
			}
			btnAprovar.Visible = false;
			btnNegar.Visible = false;
			btnPdf.Visible = status == StatusAutorizacaoProvisoria.APROVADO;
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar registros!", ex);
			}
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
				ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
				ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;
				LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
				TableCell cellStatus = row.Cells[7];

				StatusAutorizacaoProvisoria status = (StatusAutorizacaoProvisoria)Convert.ToInt32(vo["cd_status"]);
				string motivo = Convert.ToString(vo["ds_motivo_cancelamento"]);

				btnAprovar.Visible = status == StatusAutorizacaoProvisoria.PENDENTE;
				btnPdf.Visible = status == StatusAutorizacaoProvisoria.APROVADO;

				btnAprovar.Visible &= HasPermission(Modulo.APROVAR_AUTORIZACAO_PROVISORIA);

				btnNegar.Visible = false;
				if (status == StatusAutorizacaoProvisoria.PENDENTE)
					btnNegar.Visible = true;
				else if (status == StatusAutorizacaoProvisoria.APROVADO)
					btnNegar.Visible = HasPermission(Modulo.APROVAR_AUTORIZACAO_PROVISORIA);

				btnNegar.Visible = (status == StatusAutorizacaoProvisoria.PENDENTE);

				cellStatus.Text = AutorizacaoProvisoriaEnumTradutor.TraduzSituacao(status);
				if (!string.IsNullOrEmpty(motivo)) {
					cellStatus.Text += " - " + motivo;
				}
			}
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (this.IsPagingCommand(sender, e)) return;

				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					int index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdAprovar") {
						AlterarStatusLinha(row, index, StatusAutorizacaoProvisoria.APROVADO);
					} else if (e.CommandName == "CmdNegar") {
						AlterarStatusLinha(row, index, StatusAutorizacaoProvisoria.NEGADO);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            DataTable sourceTable = GetRelatorioTable();

            ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);
            defs["CD_STATUS"].ForEach(x => x.Transformer = y => AutorizacaoProvisoriaEnumTradutor.TraduzSituacao((StatusAutorizacaoProvisoria)Convert.ToInt32(y)));
            defs.SetWidth("benef_NM_BENEFICIARIO", 40);

            defs.Add(new ExcelColumnDefinition()
            {
                HeaderText = "LOCAL",
                ColumnName = ExcelColumnDefinition.FULL_ROW,
                StyleName = ExcelStyleDefinition.TEXT,
				Transformer = x => ((DataRow)x)["DS_MUNICIPIO"] + " - " + ((DataRow)x)["SG_UF"],
                Width = 27
            });

            ExportExcel("AutorizacaoProvisoria", defs, sourceTable);
        }

	}
}