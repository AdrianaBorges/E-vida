using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Security;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;

namespace eVidaBeneficiarios.IR {
	public partial class ExtratoReembolsoTable : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<int> anos = UsuarioLogado.ConfiguracaoIr.Anos;
				if (anos.Count == 0)
					anos.Add(DateTime.Now.Year - 1);
				dpdAno.DataSource = anos;
				dpdAno.DataBind();
				dpdAno.SelectedValue = (anos[anos.Count-1]).ToString();
			}
		}

		private void Buscar() {
			btnExportar.Visible = false;
			gdvRelatorio.Visible = false;

			int ano = Int32.Parse(dpdAno.SelectedValue);

            DataTable dtAcessos = ExtratoIrBeneficiarioBO.Instance.RelatorioReembolsoIrTable(UsuarioLogado.UsuarioTitular.GetCarteira(), ano);

			this.SaveRelatorioData(dtAcessos);
			gdvRelatorio.DataSource = dtAcessos;
			gdvRelatorio.DataBind();

			if (dtAcessos.Rows.Count == 0) {
				this.ShowInfo("Não foram encontrados registros!");
			} else {
				btnExportar.Visible = true;
				gdvRelatorio.Visible = true;
				base.RegisterScript("pop", "openPdf()");
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			}
			catch (Exception ex) {
				ShowError("Erro ao buscar os dados", ex);
			}
		}
	}
}