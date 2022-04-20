using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Security;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO;
using eVida.Web.Report;

namespace eVidaIntranet.IR {
	public partial class IrBeneficiarios : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdAnoRef.Items.Add(new ListItem((DateTime.Now.Year - 3).ToString(), (DateTime.Now.Year - 3).ToString()));
				dpdAnoRef.Items.Add(new ListItem((DateTime.Now.Year-2).ToString(), (DateTime.Now.Year-2).ToString()));
				dpdAnoRef.Items.Add(new ListItem((DateTime.Now.Year-1).ToString(), (DateTime.Now.Year-1).ToString()));
				dpdAnoRef.SelectedValue = (DateTime.Now.Year - 1).ToString();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.IR_BENEFICIARIO; }
		}

		private void Buscar() {
			btnExportar.Visible = false;
			gdvMensalidade.Visible = false;
			gdvReembolso.Visible = false;
			gdvTotalMensalidade.Visible = false;

			string cartao = txtCartao.Text;

			if (string.IsNullOrEmpty(cartao)) {
				this.ShowError("Por favor informe o cartão do titular!");
				return;
			}

			HcBeneficiarioVO benefVO = BeneficiarioBO.Instance.GetBeneficiarioByCartao(cartao);
			if (benefVO == null) {
				this.ShowError("Beneficiário não encontrado");
				return;
			}

			int ano = Int32.Parse(dpdAnoRef.SelectedValue);
			string tipo = dpdTipo.SelectedValue;
			UsuarioIntranetVO usuario = UsuarioLogado;

			DataTable dtAcessos = null;
			bool byFile = false;
			bool hasData = false;

			if (tipo.Equals("M")) {
				string filePath = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidadeFile(cartao, ano);
				if (string.IsNullOrEmpty(filePath)) {
					dtAcessos = ExtratoIrBeneficiarioBO.Instance.RelatorioMensalidade(benefVO.CdEmpresa, benefVO.CdFuncionario, ano);
					if (dtAcessos.Rows.Count > 0) {
						DataTable dtTotal = ExtratoIrBeneficiarioBO.Instance.TotalizarMensalidade(dtAcessos);
						gdvTotalMensalidade.DataSource = dtTotal;
						gdvTotalMensalidade.DataBind();

						gdvMensalidade.DataSource = dtAcessos;
						gdvMensalidade.DataBind();

						//gdvTotalMensalidade.Visible = true;
						//gdvMensalidade.Visible = true;
						hasData = true;
					}
				} else {
					byFile = true;
					hasData = true;
				}
			} else {
				dtAcessos = ExtratoIrBeneficiarioBO.Instance.RelatorioReembolsoIr(benefVO.CdEmpresa, benefVO.CdFuncionario, ano);
				gdvReembolso.DataSource = dtAcessos;
				gdvReembolso.DataBind();
				if (dtAcessos.Rows.Count > 0) {
					gdvReembolso.Visible = true;
					hasData = true;
				}
			}
			
			if (!hasData) {
				this.ShowInfo("Não foram encontrados registros!");
			} else {
				hidMatricula.Value = benefVO.CdFuncionario.ToString();
				hidEmpresa.Value = benefVO.CdEmpresa.ToString();
				btnExportar.Visible = true;

				if (byFile) {
					btnExportar.OnClientClick = "return openPdfFile();";
					base.RegisterScript("pop", "openPdfFile()");
				} else {
					btnExportar.OnClientClick = "return openPdf();";
					base.RegisterScript("pop", "openPdf()");
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