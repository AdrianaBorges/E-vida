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
using eVidaGeneralLib.VO.Protheus;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioReciprocidade : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<POperadoraSaudeVO> lst = ReciprocidadeBO.Instance.ListarOperadoras();
				dpdEmpresa.DataSource = lst;
				dpdEmpresa.DataBind();
				dpdEmpresa.Items.Insert(0, new ListItem("TODAS", ""));
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_RECIPROCIDADE; }
		}

		private bool TryGetLong(string value, out long? outLong) {
			outLong = null;
			if (string.IsNullOrEmpty(value))
				return true;
			long oL;
			if (!Int64.TryParse(value, out oL)) {
				return false;
			}
			outLong = oL;
			return true;
		}

		private string StatusConverter(object o) {
			switch ((StatusReciprocidade)Convert.ToInt32(o)) {
				case StatusReciprocidade.APROVADO: return "APROVADO";
				case StatusReciprocidade.ENVIADO: return "ENVIADO";
				default: return ((StatusReciprocidade)Convert.ToInt32(o)).ToString();
			}
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

            defs.SetWidth("BA1_NOMUSR", 30);
            defs.SetWidth("BA0_NOMINT", 30);

			defs["cd_status"][0].Transformer = x => StatusConverter(x);

			ExportExcel("RelatorioReciprocidade", defs, sourceTable);
		}

		private void Buscar() {
			DateTime dtInicio;
			DateTime dtFim;
			string cdEmpresa = null;
			StatusReciprocidade? status = null;
			long? cdMatricula = null;
			string titular = null;
			string dependente = null;

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}
			if (!string.IsNullOrEmpty(dpdEmpresa.SelectedValue))
				cdEmpresa = dpdEmpresa.SelectedValue;

			if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
				status = (StatusReciprocidade) Convert.ToInt32(dpdSituacao.SelectedValue);
			}

			if (!TryGetLong(txtMatricula.Text, out cdMatricula)) {
				this.ShowError("A matrícula deve ser numérica!");
				return;
			}

			titular = txtTitular.Text;
			dependente = txtDependente.Text;

			DataTable dtAcessos = ReciprocidadeBO.Instance.RelatorioReciprocidade
				(cdMatricula, null, titular, dependente, cdEmpresa, dtInicio, dtFim, status);

			btnExportar.Visible = dtAcessos.Rows.Count > 0;
			lblCount.Visible = true;
			lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

			this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);

		}

		private bool TryGetInt(string value, out int? outInt) {
			outInt = null;
			if (string.IsNullOrEmpty(value))
				return true;
			int oL;
			if (!Int32.TryParse(value, out oL)) {
				return false;
			}
			outInt = oL;
			return true;
		}

		#region Eventos

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				DataRowView dv = row.DataItem as DataRowView;
				row.Cells[7].Text = StatusConverter(dv["cd_status"]);
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try { Buscar(); }
			catch (Exception ex) {
				Log.Error("Erro ao consultar o relatorio", ex);
				this.ShowError("Erro ao consultar o relatorio: " + ex.Message);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar!");
				Log.Error("Erro ao exportar para excel!", ex);
			}
		}

		#endregion

	}
}