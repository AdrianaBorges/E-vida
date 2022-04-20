using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.GenPops {
	public partial class PopGenericCancel : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			string tipoFormulario = Request["TIPO"];
			this.btnCancelar.Visible = false;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");				
				return;
			}

			if (string.IsNullOrEmpty(tipoFormulario)) {
				this.ShowError("Tipo de requisição está inválido!");
				return;
			}
			btnCancelar.Visible = true;
			Bind(cdProtocolo, tipoFormulario);
		}

		public int Id { 
			get { return Convert.ToInt32(ViewState["ID"]); }
			set { ViewState["ID"] = value; } 
		}

		public string Tipo {
			get { return Convert.ToString(ViewState["TIPO"]); }
			set { ViewState["TIPO"] = value; }
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		private void Bind(int cdProtocolo, string tipoFormulario) {
			Id = cdProtocolo;
			Tipo = tipoFormulario;


			if ("ALGUM".Equals(Tipo)) {

			} else {
				litProtocolo.Text = cdProtocolo.ToString();
			}
		}

        private void CancelarViagem()
        {
            ViagemBO.Instance.CancelarSolicitacao(Id, txtCancelamento.Text.Trim());
            this.RegisterScript("CANCELAMENTO", "setCancelamento();");
        }

		protected void btnCancelar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(Tipo)) {
				this.ShowError("A requisição do botão está inválida!");
				return;
			}

			if (txtCancelamento.Visible) {
				if (string.IsNullOrEmpty(txtCancelamento.Text)) {
					this.ShowError("Informe o motivo de cancelamento!");
					return;
				}
			}

			if ("VIAGEM".Equals(Tipo)) {
				CancelarViagem();
			}
		}
	}
}