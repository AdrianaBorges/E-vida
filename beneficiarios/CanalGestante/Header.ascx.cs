using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.CanalGestante {
	public partial class Header : UserControlBase<UsuarioCanalGestanteVO> {
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				UsuarioCanalGestanteVO uVO = UsuarioLogado;
				if (uVO != null) {
					divHeaderInfo2.Visible = false;

                    PUsuarioVO benefVO = uVO.Usuario;

					ltNome.Text = benefVO.Nomusr;
					ltCartao.Text = benefVO.Matant;
                    PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(uVO.FamiliaProduto.Codpla.Trim());
                    ltPlano.Text = plano.Codigo + " - " + plano.Descri;

				} else {
					lnkInicial.Enabled = false;
					divHeaderInfo.Visible = false;
				}
			}
		}
	}
}