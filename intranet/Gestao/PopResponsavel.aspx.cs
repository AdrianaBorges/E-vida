using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.Gestao {
	public partial class PopResponsavel : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			long matricula = 0;
			if (string.IsNullOrEmpty(txtMatricula.Text) && string.IsNullOrEmpty(txtNome.Text)) {
				this.ShowError("Por favor, informe pelo menos um filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtNome.Text) && txtNome.Text.Length < 3) {
				this.ShowError("Por favor, informe pelo menos 3 caracteres para filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtMatricula.Text) && !Int64.TryParse(txtMatricula.Text, out matricula)) {
				this.ShowError("A matrícula deve ser numérica!");
				return;
			}
			
			DataTable dt = ResponsavelBO.Instance.BuscarPossiveisResponsaveis(matricula, txtNome.Text);
			gdvResponsavel.DataSource = dt;
			gdvResponsavel.DataBind();

			if (dt.Rows.Count == 0)
				this.ShowInfo("Não foram encontrados usuários com este filtro!");
		}

		protected void gdvResponsavel_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdvResponsavel.Rows[index];

				string cdFuncionario = Convert.ToString(gdvResponsavel.DataKeys[index]["CD_FUNCIONARIO"]);
				string cdBeneficiario = row.Cells[2].Text;
				string nome = row.Cells[3].Text;
				nome = HttpUtility.HtmlDecode(nome);
				nome = HttpUtility.JavaScriptStringEncode(nome);

				this.RegisterScript("SELECIONAR", "setResponsavel(" + cdBeneficiario + "," +
					cdFuncionario + ", '" + nome + "');");
			}
		}
	}
}