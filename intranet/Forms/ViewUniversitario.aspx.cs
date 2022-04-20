using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms {
	public partial class ViewUniversitario : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			try {
				if (!IsPostBack) {
					int id = 0;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Solicitação inválida!");
						return;
					}

					DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioBO.Instance.GetById(id);
					Bind(vo);
				}
			}
			catch (Exception ex) {
				this.ShowError(ex);
				Log.Error("Erro ao abrir tela de declaração universitária!", ex);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.UNIVERSITARIO; }
		}

		private void Bind(DeclaracaoUniversitarioVO vo) {

			PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			txtEmailTitular.Text = titular.Email;
			txtNomeTitular.Text = titular.Nomusr;
			txtCartao.Text = titular.Matant;

			PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);

			txtNomeDep.Text = benef.Nomusr;
            if (benef.Datnas != null)
            {
                DateTime dataNascimento = DateTime.ParseExact(benef.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture);
                txtIdade.Text = DateUtil.CalculaIdade(dataNascimento).ToString();
            }
				
			txtParentesco.Text = PLocatorDataBO.Instance.GetParentesco(benef.Graupa);
			txtPlano.Text = PLocatorDataBO.Instance.GetProdutoSaude(vo.CodPlanoBeneficiario).Descri;
		}

	}
}