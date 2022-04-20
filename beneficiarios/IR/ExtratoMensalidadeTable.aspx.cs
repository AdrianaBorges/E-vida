using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;

namespace eVidaBeneficiarios.IR {
	public partial class ExtratoMensalidadeTable : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<int> anos = UsuarioLogado.ConfiguracaoIr.Anos;
				if (anos != null && anos.Count > 0)
					anos.Sort();
				else {
					anos = new List<int>();
					anos.Add(DateTime.Now.Year - 1);
				}
				dpdAno.DataSource = anos;
				dpdAno.DataBind();

				dpdAno.SelectedValue = (anos[anos.Count-1]).ToString();
			}
		}

		private void Buscar() {
			btnExportar.Visible = false;
			gdvRelatorio.Visible = false;

			int ano = Int32.Parse(dpdAno.SelectedValue);
			hidAno.Value = string.Empty;

			string filePath = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidadeFile(UsuarioLogado.Usuario.GetCarteira(), ano);
			bool hasDados = false;
			DataTable dtAcessos = null;
			if (string.IsNullOrEmpty(filePath)) {
                dtAcessos = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidadeTable(UsuarioLogado.UsuarioTitular.Codint, UsuarioLogado.UsuarioTitular.Codemp, UsuarioLogado.UsuarioTitular.Matric, ano);

				this.SaveRelatorioData(dtAcessos);
				gdvRelatorio.DataSource = dtAcessos;
				gdvRelatorio.DataBind();

				hasDados = dtAcessos.Rows.Count > 0;
			} else {
				hasDados = true;
			}

			if (!hasDados) {
				this.ShowInfo("Não foram encontrados registros!");
			} else {
				hidAno.Value = ano.ToString();

				btnExportar.Visible = true;
				
				if (dtAcessos != null) {
					DataTable dtTotal = ExtratoIrBeneficiarioBO.Instance.TotalizarMensalidade(dtAcessos);
					gdvTotal.DataSource = dtTotal;
					gdvTotal.DataBind();

					base.RegisterScript("pop", "openPdf()");
					btnExportar.OnClientClick = "return openPdf()";
				} else {
					base.RegisterScript("pop", "openPdfFile()");
					btnExportar.OnClientClick = "return openPdfFile()";
				}
				

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