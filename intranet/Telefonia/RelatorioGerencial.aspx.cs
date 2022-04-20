using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;
using eVidaGeneralLib.Reporting;

namespace eVidaIntranet.Telefonia {
	public partial class RelatorioGerencial : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				chkSetor.DataSource = SetorUsuarioBO.Instance.ListarSetores();
				chkSetor.DataBind();

				List<RamalVO> lstRamal = RamalBO.Instance.ListarRamais();
				if (lstRamal != null) {
					foreach (RamalVO ramal in lstRamal) {
						string text = ramal.NrRamal.ToString();
						if (!string.IsNullOrEmpty(ramal.Alias)) {
							text += " (" + ramal.Alias + ")";
						}
						chkRamal.Items.Add(new ListItem(text, ramal.NrRamal.ToString()));
					}
				}
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.TELEFONIA_RELATORIO_GERENCIAL; }
		}

		private void Buscar() {
			DateTime? dtInicial = null;
			DateTime? dtFinal = null;

			List<int> lstSetores = new List<int>();
			List<int> lstRamais = new List<int>();
			List<string> lstEstados = new List<string>();
			string direcao = null;

			if (string.IsNullOrEmpty(txtDataInicial.Text) || string.IsNullOrEmpty(txtDataFinal.Text)) {
				this.ShowError("Informe o período!");
				return;
			}

			DateTime data;
			if (!string.IsNullOrEmpty(txtDataFinal.Text)) {
				if (!DateTime.TryParse(txtDataFinal.Text, out data)) {
					this.ShowError("Data final inválida!");
					return;
				}
				dtFinal = data;
			}
			if (!string.IsNullOrEmpty(txtDataInicial.Text)) {
				if (!DateTime.TryParse(txtDataInicial.Text, out data)) {
					this.ShowError("Data inicial inválida!");
					return;
				}
				dtInicial = data;
			}

			direcao = dpdTipo.SelectedValue;

			foreach (ListItem item in chkSetor.Items) {
				if (item.Selected) {
					lstSetores.Add(Int32.Parse(item.Value));
				}
			}
			foreach (ListItem item in chkRamal.Items) {
				if (item.Selected) {
					lstRamais.Add(Int32.Parse(item.Value));
				}
			}
			foreach (ListItem item in chkEstado.Items) {
				if (item.Selected) {
					lstEstados.Add(item.Value);
				}
			}
			ViewState["dtInicio"] = dtInicial;
			ViewState["dtFim"] = dtFinal;
			ViewState["direcao"] = direcao;

			DataTable dt = BilhetagemBO.Instance.Pesquisar(dtInicial, dtFinal, direcao, lstSetores, lstRamais, lstEstados);
			this.ShowPagingGrid(gdvRelatorio, dt, null);

			if (dt.Rows.Count > 0) {
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
				btnExportar.Visible = true;
				btnGrafico.Visible = true;
			} else {
				lblCount.Visible = true;
				lblCount.Text = "Não foram encontrados bilhetes para os ramais selecionados.";
				btnExportar.Visible = false;
				btnGrafico.Visible = false;
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar dados!", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = GetRelatorioTable();
			if (sourceTable == null) {
				this.ShowError("Os dados da pesquisa expiraram! Realize a busca novamente!");
				return;
			}

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			ExportExcel("RelatorioTelefonia", defs, sourceTable);
		}

		protected void btnGrafico_Click(object sender, EventArgs e) {
			Button btn = sender as Button;

			DateTime dtInicio = (DateTime)ViewState["dtInicio"];
			DateTime dtFim = (DateTime)ViewState["dtFim"];
			string direcao = (string)ViewState["direcao"];
			DataTable dt = GetRelatorioTable();

			string url = "";
			url += "dtInicio=" + dtInicio.ToShortDateString().ToBase64String();
			url += "&dtFim=" + dtFim.ToShortDateString().ToBase64String();

			url += "&hc=" + dt.GetHashCode();
			url += "&dir=" + direcao;

			string tipo = ReportHandler.EnumRelatorio.GERENCIAL_BILHETAGEM.ToString();
			ReportBilhetagemBinder.ParamsVO dados = new ReportBilhetagemBinder.ParamsVO();
			dados.Dados = dt;
			dados.Direcao = direcao;
			dados.Fim = dtFim;
			dados.Inicio = dtInicio;
			ReportBilhetagem.SaveDados(Request, Session, dados);
			base.RegisterScript("GRAFICO", String.Format("openReport('{0}','{1}');", tipo, url));
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				TableCell cellROrigem = row.Cells[3];
				TableCell cellRDestino = row.Cells[4];

				DataRowView dr = (DataRowView)row.DataItem;
				string origem = Convert.ToString(dr["RORIGEM_NR_RAMAL"]);
				string destino = Convert.ToString(dr["RDESTINO_NR_RAMAL"]);

				if (!string.IsNullOrEmpty(origem)) {
					cellROrigem.Font.Bold = true;
				}

				if (!string.IsNullOrEmpty(destino)) {
					cellRDestino.Font.Bold = true;
				}
			}
		}
	}
}