using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;

namespace eVidaIntranet.Gestao {
	public partial class BuscaBenefResponsavel : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_RESPONSAVEL; }
		}

		protected void btnPesquisar_Click(object sender, EventArgs e) {
			try {
				long matricula = 0;

				if (string.IsNullOrEmpty(txtMatricula.Text) &&
					string.IsNullOrEmpty(txtNome.Text) &&
					string.IsNullOrEmpty(txtAlternativo.Text)) {
						ShowError("Informe pelo menos um filtro");
						return;
				}

				if (!string.IsNullOrEmpty(txtMatricula.Text) && !Int64.TryParse(txtMatricula.Text, out matricula)) {
					ShowError("Informe uma matrícula numérica!");
					return;
				}

				if (!string.IsNullOrEmpty(txtNome.Text) && txtNome.Text.Length < 3) {
					ShowError("Informe pelo menos 3 caracteres para realizar a busca pelo nome!");
					return;
				}

				DataTable dt = ResponsavelBO.Instance.BuscarBeneficiarios(matricula, txtNome.Text, txtAlternativo.Text);
				gdvBeneficiario.DataSource = dt;
				gdvBeneficiario.DataBind();
				if (dt.Rows.Count == 0) {
					lblCount.Text = "Não foram encontrados beneficiários do plano família com os filtros informados!";
				} else {
					lblCount.Text = " Foram encontrados " + dt.Rows.Count + " registros ";
				}
			}
			catch (Exception ex) {
				ShowError("Erro ao pesquisar os beneficiários. ", ex);
			}
		}

	}
}