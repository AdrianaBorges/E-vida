using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.GenPops {
	public partial class PopMotivoGlosa : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdGrupo.DataSource = FormNegativaBO.Instance.ListarGruposMotivo();
				dpdGrupo.DataBind();
				dpdGrupo.Items.Insert(0, new ListItem("TODOS", ""));
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCodigo.Text) && string.IsNullOrEmpty(txtDescricao.Text) && string.IsNullOrEmpty(dpdGrupo.SelectedValue)) {
				this.ShowError("Por favor, informe pelo menos um campo de filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtDescricao.Text) && txtDescricao.Text.Length < 3) {
				this.ShowError("Por favor, para a descrição informe pelo menos 3 caracteres!");
				return;
			}
			int? codigo = null;
			if (!string.IsNullOrEmpty(txtCodigo.Text)) {
				int i;
				if (!Int32.TryParse(txtCodigo.Text, out i)) {
					this.ShowError("O código deve ser numérico!");
					return;
				}
			}

			List<MotivoGlosaVO> dt = FormNegativaBO.Instance.BuscarMotivoGlosa(codigo, dpdGrupo.SelectedValue, txtDescricao.Text.ToUpper());
			
			gdv.DataSource = dt;
			gdv.DataBind();

			if (dt == null || dt.Count == 0)
				this.ShowInfo("Não foram encontrados motivos de glosa com este filtro!");
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				// Retrieve the row index stored in the CommandArgument property.
				int index = Convert.ToInt32(e.CommandArgument);

				// Retrieve the row that contains the button 
				// from the Rows collection.
				GridViewRow row = gdv.Rows[index];

				string id = Convert.ToString(gdv.DataKeys[index]["Id"]);
				this.RegisterScript("SERVICO", "setReturn('" + id + "');");
			}
		}
	}
}