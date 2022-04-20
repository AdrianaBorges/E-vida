using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;

namespace eVidaBeneficiarios.GenPops {
	public partial class PopCredenciado : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				EnableEmpty = string.Equals("true", Request["enableEmpty"], StringComparison.InvariantCultureIgnoreCase);
				IsHospital = string.Equals("true", Request["hospital"], StringComparison.InvariantCultureIgnoreCase);
			}
		}

		public bool EnableEmpty {
			get { return Convert.ToBoolean(ViewState["ENABLE_EMPTY"]); }
			set { ViewState["ENABLE_EMPTY"] = value; }
		}

		public bool IsHospital {
			get { return Convert.ToBoolean(ViewState["HOSPITAL"]); }
			set { ViewState["HOSPITAL"] = value; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			long? cpfCnpj =null;
			if (string.IsNullOrEmpty(txtCpfCnpj.Text) && string.IsNullOrEmpty(txtRazaoSocial.Text)) {
				this.ShowError("Por favor, informe pelo menos um campo de filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtCpfCnpj.Text)) {
				long l = 0;
				if (!Int64.TryParse(txtCpfCnpj.Text, out l)) {
					this.ShowError("O CPF/CNPJ deve ser numérico");
					return;
				}
				cpfCnpj = l;
			}

            DataTable dt = PRedeAtendimentoBO.Instance.Pesquisar(txtRazaoSocial.Text, txtCpfCnpj.Text, IsHospital);
			if (dt.Rows.Count > 300) {
				dt = dt.AsEnumerable().Take(300).CopyToDataTable();
				this.ShowInfo("Foram retornados apenas os 300 primeiros resultados da pesquisa. Por favor informe mais detalhes!");
			}
			gdv.DataSource = dt;
			gdv.DataBind();

			btnEmpty.Visible = dt.Rows.Count == 0 && EnableEmpty;
			if (dt.Rows.Count == 0)
				this.ShowInfo("Não foram encontrados credenciados com este filtro!");
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdv.Rows[index];

				string id = Convert.ToString(gdv.DataKeys[index]["BAU_CODIGO"]);

				string nome = row.Cells[3].Text;
				nome = HttpUtility.HtmlDecode(nome);
				nome = HttpUtility.JavaScriptStringEncode(nome);
				this.RegisterScript("SERVICO", "setCredenciado('" + id + "','" + nome + "');");
			}
		}

		protected void btnEmpty_Click(object sender, EventArgs e) {
			this.RegisterScript("SERVICO", "setCredenciado('" + NOT_FOUND_LOCATOR + "');");
		}
	}
}