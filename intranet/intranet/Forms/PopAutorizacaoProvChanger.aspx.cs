using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Forms {
	public partial class PopAutorizacaoProvChanger : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");

				this.btnSalvar.Visible = false;
				return;
			}
			litProtocolo.Text = cdProtocolo.ToString(AutorizacaoProvisoriaVO.FORMATO_PROTOCOLO);
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO_PROVISORIA; }
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCancelamento.Text)) {
				this.ShowError("Informe o motivo de cancelamento!");
				return;
			}
			int id = Int32.Parse(Request["ID"]);
			AutorizacaoProvisoriaBO.Instance.Cancelar(id, txtCancelamento.Text.ToUpper(), UsuarioLogado.Id);
			this.RegisterScript("aqui", "setCancelamento(" + id + ")");
		}
	}
}