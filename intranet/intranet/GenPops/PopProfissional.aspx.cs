using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.GenPops {
	public partial class PopProfissional : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdTipoConselho.DataSource = PLocatorDataBO.Instance.ListarConselhoProfissional();
				dpdTipoConselho.DataBind();
				dpdTipoConselho.Items.Insert(0, new ListItem("TODOS", ""));

				dpdUfConselho.DataSource = PLocatorDataBO.Instance.ListarUf();
				dpdUfConselho.DataBind();
				dpdUfConselho.Items.Insert(0, new ListItem("TODOS", ""));

				EnableEmpty = string.Equals("true", Request["enableEmpty"], StringComparison.InvariantCultureIgnoreCase);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		public bool EnableEmpty {
			get { return Convert.ToBoolean(ViewState["ENABLE_EMPTY"]); }
			set { ViewState["ENABLE_EMPTY"] = value; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			int? codigo = null;
			if (string.IsNullOrEmpty(txtConselho.Text) && string.IsNullOrEmpty(txtNome.Text) 
				&& string.IsNullOrEmpty(dpdTipoConselho.SelectedValue) && string.IsNullOrEmpty(dpdUfConselho.SelectedValue)) {
				this.ShowError("Por favor, informe pelo menos um campo de filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtConselho.Text)) {
				int i = 0;
				if (!Int32.TryParse(txtConselho.Text, out i)) {
					this.ShowError("O conselho deve ser numérico!");
					return;
				}
				codigo = i;
			}
			IEnumerable<PProfissionalSaudeVO> dt = PLocatorDataBO.Instance.BuscarProfissionais(codigo.ToString(), txtNome.Text, dpdUfConselho.SelectedValue, dpdTipoConselho.SelectedValue);
			if (dt != null && dt.Count() > 300) {
				dt = dt.Take(300);
				this.ShowInfo("Foram retornados apenas os 300 primeiros resultados da pesquisa. Por favor informe mais detalhes!");
			}
			gdv.DataSource = dt;
			gdv.DataBind();

			if (dt == null || dt.Count() == 0) {
				btnEmpty.Visible = EnableEmpty;
				this.ShowInfo("Não foram encontrados profissionais com este filtro!");
			} else {
				btnEmpty.Visible = false;
			}
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdv.Rows[index];

                string seq = Convert.ToString(gdv.DataKeys[index]["Codigo"]);

				PProfissionalSaudeVO prof = PLocatorDataBO.Instance.GetProfissional(seq);

				string id = prof.Numcr;
				id += ";" + prof.Codsig;
				id += ";" + prof.Estado;
				string nome = row.Cells[1].Text;
				nome = HttpUtility.HtmlDecode(nome);
				nome = HttpUtility.JavaScriptStringEncode(nome);
				this.RegisterScript("SERVICO", "setProfissional('" + id + "','" + nome + "');");
			}
		}

		protected void btnEmpty_Click(object sender, EventArgs e) {
			this.RegisterScript("SERVICO", "setProfissional('" + NOT_FOUND_LOCATOR + "');");
		}

		protected void gdv_RowDataBound(object sender, GridViewRowEventArgs e) {
            
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
                PProfissionalSaudeVO vo = (PProfissionalSaudeVO)row.DataItem;
				string str = string.Empty;
                str += vo.Numcr + " - " + vo.Codsig + " - " + vo.Estado;
                row.Cells[2].Text = str;
			}
		}
	}
}