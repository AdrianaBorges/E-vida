using eVida.Web.Report;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Forms {
	public partial class DeclaracaoAnualDebito : FormPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			
		}

		private void Gerar() {

			DeclaracaoAnualDebitoVO vo = new DeclaracaoAnualDebitoVO();
			vo.AnoRef = DateTime.Now.Year-1;
			vo.CodBeneficiario = UsuarioLogado.Titular.CdBeneficiario;

			if (!DeclaracaoAnualDebitoBO.Instance.CheckDebitoAno(vo.CodBeneficiario, vo.AnoRef)) {
				this.ShowError("Declaração não disponível entrar em contato com financeiro@e-vida.org.br");
				return;
			}

			ReportDeclaracaoAnualDebito rpt = new ReportDeclaracaoAnualDebito(ReportDir, UsuarioLogado);
			byte[] bytes = rpt.GerarNovoRelatorio(vo.CodBeneficiario, vo.AnoRef);

			DeclaracaoAnualDebitoBO.Instance.Enviar(vo, null, bytes);
			this.ShowInfo("Declaracao enviada com sucesso!");
		}

		protected void btnGerar_Click(object sender, EventArgs e) {
			try {
				Gerar();
			} catch (Exception ex) {
				this.ShowError("Erro ao gerar declaração!", ex);
			}
		}

	}
}