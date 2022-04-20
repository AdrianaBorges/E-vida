using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Externo {
	public partial class Header : UserControlBase<UsuarioNoLoginRequiredVO> {
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				UsuarioNoLoginRequiredVO uVO = UsuarioLogado;
				if (uVO != null) {
					divHeaderInfo2.Visible = false;
				} else {
					lnkInicial.Enabled = false;
					divHeaderInfo.Visible = false;
				}
			}
		}

		public void UpdateBeneficiario(PBeneficiarioVO vo, PPlanoVO plano) {
			ltNome.Text = vo.Nome;
			ltCartao.Text = vo.CdAlternativo;			
			ltPlano.Text = plano.CodIsa + " - " + plano.Nome;
			ltValidadeCarteira.Text = vo.DtValidadeCarteira.ToShortDateString();
			divHeaderInfo2.Visible = false;
			divHeaderInfo.Visible = true;
			lnkInicial.Enabled = false;
		}
	}
}