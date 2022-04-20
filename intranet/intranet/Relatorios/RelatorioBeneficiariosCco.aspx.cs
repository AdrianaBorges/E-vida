using eVidaGeneralLib.BO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioBeneficiariosCco : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_BENEFICIARIOS_CCO; }
		}

		private void Buscar() {
			if (string.IsNullOrEmpty(txtCco.Text)) {
				this.ShowError("Informe o CCO!");
				return;
			}
			DataTable dt = RelatorioBO.Instance.BuscarBeneficiariosCco(txtCco.Text);
			gdvRelatorio.DataSource = dt;
			gdvRelatorio.DataBind();
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar consulta.", ex);
			}
		}

	}
}