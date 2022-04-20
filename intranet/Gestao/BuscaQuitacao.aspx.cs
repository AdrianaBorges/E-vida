using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Gestao {
	public partial class BuscaQuitacao : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
				foreach (string name in monthNames) {
					if (!string.IsNullOrEmpty(name))
						dpdMes.Items.Add(new ListItem(name.ToUpper(), (dpdMes.Items.Count + 1).ToString()));
				}
				dpdMes.Items.Insert(0, new ListItem("TODOS", ""));
				dpdMes.SelectedValue = DateTime.Now.Month.ToString();

				for (int i = DateTime.Now.Year - 5; i <= DateTime.Now.Year; ++i) {
					dpdAno.Items.Add(new ListItem(i + "", i + ""));
				}
				dpdAno.Items.Insert(0, new ListItem("TODOS", ""));
				dpdAno.SelectedValue = DateTime.Now.Year.ToString();

				dpdStatus.Items.Add(new ListItem("TODOS", ""));
				dpdStatus.Items.Add(new ListItem(TraduzStatus(ArquivoSapVO.ST_IMPORTADO), ArquivoSapVO.ST_IMPORTADO));
				dpdStatus.Items.Add(new ListItem(TraduzStatus(ArquivoSapVO.ST_QUITADO), ArquivoSapVO.ST_QUITADO));
				dpdStatus.Items.Add(new ListItem(TraduzStatus(ArquivoSapVO.ST_CANCELADO), ArquivoSapVO.ST_CANCELADO));

				dpdTipoArquivo.Items.Add(new ListItem("TODOS", ""));
				foreach (TipoArquivoSapEnum tipoArq in Enum.GetValues(typeof(TipoArquivoSapEnum))) {
					dpdTipoArquivo.Items.Add(new ListItem(ArquivoSapEnumTradutor.TraduzTipoArquivo(tipoArq), tipoArq.ToString()));
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_QUITACAO; }
		}

		private string TraduzStatus(string cdStatus) {
			switch (cdStatus) {
				case ArquivoSapVO.ST_CANCELADO: return "CANCELADO";
				case ArquivoSapVO.ST_QUITADO: return "QUITADO";
				case ArquivoSapVO.ST_IMPORTADO: return "IMPORTADO";
			}
			return "-";
		}

		protected void btnPesquisar_Click(object sender, EventArgs e) {
			int? ano = null;
			int? mes = null;
			string status = null;
			TipoArquivoSapEnum? tipoArquivo = null;

			if (!string.IsNullOrEmpty(dpdAno.SelectedValue))
				ano = Int32.Parse(dpdAno.SelectedValue);
			if (!string.IsNullOrEmpty(dpdMes.SelectedValue))
				mes = Int32.Parse(dpdMes.SelectedValue);
			if (!string.IsNullOrEmpty(dpdTipoArquivo.SelectedValue)) {
				tipoArquivo = (TipoArquivoSapEnum)Enum.Parse(typeof(TipoArquivoSapEnum), dpdTipoArquivo.SelectedValue);
			}
			status = dpdStatus.SelectedValue;

			DataTable dt = QuitacaoBO.Instance.Pesquisar(tipoArquivo, ano, mes, status);
			gdvArquivos.DataSource = dt;
			gdvArquivos.DataBind();

			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("./Quitacao.aspx");
		}

		protected void gdvArquivos_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				DataRowView drv = row.DataItem as DataRowView;
				row.Cells[6].Text = TraduzStatus(Convert.ToString(drv["CD_STATUS"]));
			}
		}
	}
}