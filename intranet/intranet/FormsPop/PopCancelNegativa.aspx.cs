using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.FormsPop {
	public partial class PopCancelNegativa : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");

				this.btnSalvar.Visible = false;
				return;
			}
			litProtocolo.Text = cdProtocolo.ToString("000000000");
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.NEGATIVA; }
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCancelamento.Text)) {
				this.ShowError("Informe o motivo de cancelamento!");
				return;
			}
			FormNegativaBO.Instance.Cancelar(Int32.Parse(Request["ID"]), txtCancelamento.Text.ToUpper(), UsuarioLogado.Id);
			this.RegisterScript("aqui", "setCancelamento(" + Int32.Parse(Request["ID"])  + ")");
		}
	}
}