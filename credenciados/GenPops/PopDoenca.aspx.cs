using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.BO;

namespace eVidaCredenciados.GenPops {
	public partial class PopDoenca : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {

		}
		
		protected void btnBuscar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCodigo.Text) && string.IsNullOrEmpty(txtDescricao.Text)) {
				this.ShowError("Por favor, informe pelo menos um campo de filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtDescricao.Text) && txtDescricao.Text.Length < 3) {
				this.ShowError("Por favor, para a descrição informe pelo menos 3 caracteres!");
				return;
			}
			DataTable dt = PLocatorDataBO.Instance.BuscarDoencas(txtCodigo.Text, txtDescricao.Text);
			if (dt.Rows.Count > 300) {
				dt = dt.AsEnumerable().Take(300).CopyToDataTable();
				this.ShowInfo("Foram retornados apenas os 300 primeiros resultados da pesquisa. Por favor informe mais detalhes!");
			}
			gdv.DataSource = dt;
			gdv.DataBind();

			if (dt.Rows.Count == 0)
				this.ShowInfo("Não foram encontrados doenças com este filtro!");
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdv.Rows[index];

				string id = Convert.ToString(gdv.DataKeys[index]["BA9_CODDOE"]);

				string nome = row.Cells[2].Text;
				nome = HttpUtility.HtmlDecode(nome);
				nome = HttpUtility.JavaScriptStringEncode(nome);
				this.RegisterScript("SERVICO", "setDoenca('" + id + "','" + nome + "');");
			}
		}
	}
}