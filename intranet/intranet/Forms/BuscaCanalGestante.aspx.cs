using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
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
	public partial class BuscaCanalGestante : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdSituacao.Items.Add(new ListItem("TODOS", ""));
				foreach (int i in Enum.GetValues(typeof(StatusCanalGestante))) {
					StatusCanalGestante sts = (StatusCanalGestante)i;
					if (sts == StatusCanalGestante.GERANDO)
						continue;
					dpdSituacao.Items.Add(new ListItem(CanalGestanteEnumTradutor.TraduzStatus(sts), i.ToString()));
				}
				
				dpdControle.Items.Add(new ListItem("TODOS", ""));
				dpdControle.Items.Add(new ListItem(CanalGestanteEnumTradutor.TraduzControle(CanalGestanteVO.CONTROLE_OK), CanalGestanteVO.CONTROLE_OK.ToString()));
				dpdControle.Items.Add(new ListItem(CanalGestanteEnumTradutor.TraduzControle(CanalGestanteVO.CONTROLE_ALERTA), CanalGestanteVO.CONTROLE_ALERTA.ToString()));
				dpdControle.Items.Add(new ListItem(CanalGestanteEnumTradutor.TraduzControle(CanalGestanteVO.CONTROLE_ATRASADO), CanalGestanteVO.CONTROLE_ATRASADO.ToString()));
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.CANAL_GESTANTE; }
		}

		private void Buscar() {
			int? idProtocolo = null;
			string cartao = null;
			StatusCanalGestante? status = null;
			int? controle = null;

			if (!string.IsNullOrEmpty(txtProtocolo.Text)) {
				int i;
				if (!Int32.TryParse(txtProtocolo.Text, out i)) {
					this.ShowError("O protocolo deve ser numérico!");
					return;
				}
				idProtocolo = i;
			}
			cartao = txtCartao.Text.Trim();
			if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
				status = (StatusCanalGestante)Int32.Parse(dpdSituacao.SelectedValue);
			}
			if (!string.IsNullOrEmpty(dpdControle.SelectedValue)) {
				controle = Int32.Parse(dpdControle.SelectedValue);
			}

			if (idProtocolo == null && string.IsNullOrEmpty(cartao) && status == null) {
				this.ShowError("Informe pelo menos um filtro!");
				return;
			}

			DataTable dt = CanalGestanteBO.Instance.Pesquisar(idProtocolo, cartao, status, controle);
			this.ShowPagingGrid(gdvRelatorio, dt, null);

			btnExportar.Visible = dt.Rows.Count > 0;
			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);
			defs.Add(new ExcelColumnDefinition() {
				HeaderText = "Situação",
				ColumnName = "ID_SITUACAO",
				StyleName = ExcelStyleDefinition.TEXT,
				Transformer = x => CanalGestanteEnumTradutor.TraduzStatus((StatusCanalGestante)Convert.ToInt32(x)),
				Width = 30
			});
			defs.SetWidth("DS_PENDENCIA", 40);
			defs.SetWidth("benef_NM_BENEFICIARIO", 40);
			ExportExcel("CanalGestante", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar busca.", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			} catch (Exception ex) {
				this.ShowError("Erro ao exportar!", ex);
			}
		}
		
		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				Literal litSituacao = (Literal)row.FindControl("litSituacao");
				Image imgControle = (Image)row.FindControl("imgControle");
				LinkButton btnFinalizar = (LinkButton)row.FindControl("btnFinalizar");

				StatusCanalGestante sts = (StatusCanalGestante)Convert.ToInt32(vo["ID_SITUACAO"]);
				DateTime dtSolicitacao = Convert.ToDateTime(vo["DT_SOLICITACAO"]);
				litSituacao.Text = CanalGestanteEnumTradutor.TraduzStatus(sts);

				btnFinalizar.Visible = sts != StatusCanalGestante.FINALIZADO;

				DateTime? dtFinalizacao = vo["DT_FINALIZACAO"] == DBNull.Value ? new DateTime?() : Convert.ToDateTime(vo["DT_FINALIZACAO"]);
				int controle = CanalGestanteEnumTradutor.CalcularControle(dtSolicitacao, dtFinalizacao);

				string urlImg = "";
				switch (controle) {
					case CanalGestanteVO.CONTROLE_OK:
						urlImg = "~/img/progress_ok.png";
						break;
					case CanalGestanteVO.CONTROLE_ALERTA:
						urlImg = "~/img/progress_alert.png";
						break;
					case CanalGestanteVO.CONTROLE_ATRASADO:
						urlImg = "~/img/progress_fail.png";
						break;
					default:
						break;
				}

				imgControle.ImageUrl = urlImg;
			}
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			string cmdName = e.CommandName;

			if (this.IsPagingCommand(sender, e)) return;

			if ("CmdFinalizar".Equals(cmdName)) {
				Buscar();
			}
		}
	}
}