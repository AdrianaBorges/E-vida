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
using SkyReport.ExcelExporter;
using eVidaGeneralLib.Reporting;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioFaturamentoUsuario : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			return;
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_FATURAMENTO_USUARIO; }
		}

		private void ExportarExcel() {
			DataTable sourceTable = GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_usuario", 30);

			ExportExcel("RelatorioAutorizacoes", defs, sourceTable);
		}

		private void Buscar() {
			DateTime dtInicio;
			DateTime dtFim;

			if (!DateTime.TryParse(txtDataInicial.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}

			if (!DateTime.TryParse(txtDataFinal.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}

			List<string> lstUsuarios = new List<string>();
			DataTable dt = GetTableUsuario();
			if (dt != null) {
				foreach (DataRow dr in dt.Rows) {
					lstUsuarios.Add((string)dr["cd_usuario"]);
				}
			}

			bool byItem = dpdContabilizacao.SelectedValue.Equals("1");

			DataTable dtAcessos = RelatorioBO.Instance.BuscarFaturamentoUsuario(byItem, dtInicio, dtFim, lstUsuarios);

			btnExportar.Visible = btnGraficoProdDia.Visible = btnGraficoProd.Visible = dtAcessos.Rows.Count > 0;
			lblCount.Visible = true;
			lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

			if (byItem) {
				gdvRelatorio.Columns[0].Visible = true;
				gdvRelatorio.Columns[2].Visible = true;
				gdvRelatorio.Columns[3].HeaderText = "QTD ITENS";

				btnGraficoProtocolo.Visible = dtAcessos.Rows.Count > 0;
			} else {
				gdvRelatorio.Columns[0].Visible = false;
				gdvRelatorio.Columns[2].Visible = false;
				gdvRelatorio.Columns[3].HeaderText = "QTD GUIAS";

				btnGraficoProtocolo.Visible = false;
			}

			this.ViewState["dtInicio"] = dtInicio;
			this.ViewState["dtFim"] = dtFim;

			this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
		}

		private DataTable GetTableUsuario() {

			DataTable dt = ViewState["GDV_USUARIO"] as DataTable;
			if (dt == null) {
				dt = new DataTable();
				dt.Columns.Add(new DataColumn("cd_usuario", typeof(string)));
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

		private void AdicionarUsuario(string cd, string nome) {
			DataTable dt = GetTableUsuario();

			if (nome.Equals("--")) {
				AdicionarUsuarioMultiplo(dt, cd);
			} else {
				AdicionarUsuarioUnico(dt, cd, nome);
			}

			BindGridUsuario();
		}

		private void AdicionarUsuarioUnico(DataTable dt, string cd, string nome) {
			DataView dv = new DataView(dt);
			dv.RowFilter = " CD_USUARIO = '" + cd.ToUpper() + "'";
			if (dv.Count > 0) {
				this.ShowInfo("Usuário já existe na lista de filtros!");
				return;
			}

			DataRow dr = dt.NewRow();
			dr["cd_usuario"] = cd.ToUpper();
			dr["nm_usuario"] = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(nome)).ToUpper();
			dt.Rows.Add(dr);
		}

		private void AdicionarUsuarioMultiplo(DataTable dt, string ids) {
			List<string> lstIds = FormatUtil.StringToList(ids);

			DataTable dt2 = RelatorioBO.Instance.BuscarUserUpdateAtendimento(lstIds);
			DataView dv = new DataView(dt);
			bool existe = false;
			foreach (DataRow dr2 in dt2.Rows) {
				string cd = Convert.ToString(dr2["cd_usuario"]).ToUpper();
				dv.RowFilter = " CD_USUARIO = '" + cd + "'";
				if (dv.Count > 0) {
					existe = true;
				} else {
					DataRow dr = dt.NewRow();
					dr["cd_usuario"] = cd;
					dr["nm_usuario"] = Convert.ToString(dr2["nm_usuario"]).ToUpper();
					dt.Rows.Add(dr);
				}
			}

			if (existe) {
				this.ShowInfo("Alguns usuários já estavam selecionados!");
			}
		}

		private void RemoverUsuario(int index) {
			DataTable dt = GetTableUsuario();
			dt.Rows.RemoveAt(index);
			BindGridUsuario();
		}

		#region Eventos

		protected void btnGraficoProd_Click(object sender, EventArgs e) {
			Button btn = sender as Button;

			DateTime dtInicio = (DateTime)ViewState["dtInicio"];
			DateTime dtFim = (DateTime)ViewState["dtFim"];
			DataTable dt = GetRelatorioTable();

			string url = "";
			url += "dtInicio=" + dtInicio.ToShortDateString().ToBase64String();
			url += "&dtFim=" + dtFim.ToShortDateString().ToBase64String();

			url += "&hc=" + dt.GetHashCode();
			url += "&tr=" + (btn == btnGraficoProdDia ? "DIA" : "ALL");

			string tipo = ReportHandler.EnumRelatorio.FATURAMENTO_USUARIO.ToString();
			ReportProducaoFaturamentoBinder.ParamsVO dados = new ReportProducaoFaturamentoBinder.ParamsVO();
			dados.Dados = dt;
			dados.Fim = dtFim;
			dados.Inicio = dtInicio;
			dados.Tipo = (btn == btnGraficoProdDia ? "DIA" : "ALL");
			ReportProducaoFaturamento.SaveDados(Request, Session, dados);

			base.RegisterScript("GRAFICO", String.Format("openReport('{0}','{1}');", tipo, url));
		}

		protected void btnLimparUsuario_Click(object sender, EventArgs e) {
			try {
				LimparUsuarios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao limpar usuários!", ex);
			}
		}

		protected void btnAdicionarUsuario_Click(object sender, EventArgs e) {
			try {
				AdicionarUsuario(hidCdUsuario.Value, hidNmUsuario.Value);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao adicionar o usuário!", ex);
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar dados!", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar para excel!", ex);
			}
		}

		protected void gdvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName.Equals("RemoverUsuario")) {
				try {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);
					RemoverUsuario(index);
				}
				catch (Exception ex) {
					this.ShowError("Erro ao remover usuario!", ex);
				}
				
			}
		}

		#endregion

		private DataTable ConverterTableProtocolo(DataTable origem) {
			DataTable dest = new DataTable();

			dest.Columns.Add(new DataColumn("qtd_item", typeof(int)));
			dest.Columns.Add(new DataColumn("tp_sistema_atend", typeof(string)));
			dest.Columns.Add(new DataColumn("user_update", typeof(string)));
			dest.Columns.Add(new DataColumn("nm_usuario", typeof(string)));
			dest.Columns.Add(new DataColumn("tp_origem", typeof(string)));

			Dictionary<string, KeyValuePair<DataRow, SortedSet<string>>> mapping = new Dictionary<string, KeyValuePair<DataRow, SortedSet<string>>>();
			foreach (DataRow drOrigem in origem.Rows) {
				string protocolo = Convert.ToString(drOrigem["nr_protocolo"]);
				string tpSistema = Convert.ToString(drOrigem["tp_sistema_atend"]);
				string userUpdate  = Convert.ToString(drOrigem["user_update"]);
				string nomeUsuario  = Convert.ToString(drOrigem["nm_usuario"]);
				string tpOrigem  = Convert.ToString(drOrigem["tp_origem"]);

				string key = tpSistema + "_" + userUpdate + "_" + tpOrigem;

				DataRow drDest = null;
				if (mapping.ContainsKey(key)) {
					KeyValuePair<DataRow, SortedSet<string>> mapped = mapping[key];
					SortedSet<string> lstProtocolos = mapped.Value;
					drDest = mapped.Key;
					if (!lstProtocolos.Contains(protocolo)) {
						drDest["qtd_item"] = Convert.ToInt32(drDest["qtd_item"]) + 1;
						lstProtocolos.Add(protocolo);
					}					
				} else {
					drDest = dest.NewRow();
					drDest["qtd_item"] = 1;
					drDest["tp_sistema_atend"] = tpSistema;
					drDest["user_update"] = userUpdate;
					drDest["nm_usuario"] = nomeUsuario;
					drDest["tp_origem"] = tpOrigem;
					dest.Rows.Add(drDest);

					KeyValuePair<DataRow, SortedSet<string>> mapped = new KeyValuePair<DataRow, SortedSet<string>>(drDest, new SortedSet<string>());
					mapped.Value.Add(protocolo);
					mapping.Add(key, mapped);
				}
			}
			return dest;
		}

		protected void btnGraficoProtocolo_Click(object sender, EventArgs e) {
			DataTable dt = GetRelatorioTable();
			if (dt == null) {
				this.ShowError("Dados de sessão forma expirados. Efetua a busca novamente!");
				return;
			}
			if (!dt.Columns.Contains("nr_protocolo")) {
				this.ShowError("Gráfico pode ser gerado apenas para busca por ITEMS!");
				return;
			}
			dt = ConverterTableProtocolo(dt);

			DateTime dtInicio = (DateTime)ViewState["dtInicio"];
			DateTime dtFim = (DateTime)ViewState["dtFim"];

			string url = "";
			url += "dtInicio=" + dtInicio.ToShortDateString().ToBase64String();
			url += "&dtFim=" + dtFim.ToShortDateString().ToBase64String();

			url += "&hc=" + dt.GetHashCode();
			url += "&tr=PROTOCOLO";

			string tipo = ReportHandler.EnumRelatorio.FATURAMENTO_USUARIO.ToString();

			ReportProducaoFaturamentoBinder.ParamsVO dados = new ReportProducaoFaturamentoBinder.ParamsVO();
			dados.Dados = dt;
			dados.Fim = dtFim;
			dados.Inicio = dtInicio;
			dados.Tipo = "PROTOCOLO";
			ReportProducaoFaturamento.SaveDados(Request, Session, dados);
			base.RegisterScript("GRAFICO", String.Format("openReport('{0}','{1}');", tipo, url));
		}

	}
}