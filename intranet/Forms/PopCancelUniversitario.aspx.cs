using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Forms {
	public partial class PopCancelUniversitario : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");

				this.btnSalvar.Visible = false;
				return;
			}
			litProtocolo.Text = cdProtocolo.ToString(DeclaracaoUniversitarioVO.FORMATO_PROTOCOLO);
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.UNIVERSITARIO; }
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCancelamento.Text)) {
				this.ShowError("Informe o motivo de cancelamento!");
				return;
			}
			try {
				int id = Int32.Parse(Request["ID"]);
				DeclaracaoUniversitarioBO.Instance.Cancelar(id, txtCancelamento.Text.ToUpper(), UsuarioLogado.Id);
				this.RegisterScript("aqui", "setCancelamento(" + id + ")");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao realizar cancelamento!", ex);
			}
		}
	}
}