using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaCredenciados.controls {
	public partial class Header : UserControlBase {
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				UsuarioCredenciadoVO uVO = UsuarioLogado;
				if (uVO != null) {
					divHeaderInfo2.Visible = false;
                    PRedeAtendimentoVO u = uVO.RedeAtendimento;
                    string strDoc = u.Tippe.Equals(PConstantes.PESSOA_FISICA) ? (u.Cpfcgc != null ? FormatUtil.FormatCpf(u.Cpfcgc) : "") : (u.Cpfcgc != null ? FormatUtil.FormatCnpj(u.Cpfcgc) : "");
					ltNome.Text = u.Nome + " (" + strDoc + ")";
					
				} else {
					lnkInicial.Enabled = false;
					divHeaderInfo.Visible = false;
				}
			}
		}

		protected void btnSair_Click(object sender, EventArgs e) {
			PageHelper.DoLogout(Session);
			Response.Redirect("~/Login.aspx");
		}
	}
}