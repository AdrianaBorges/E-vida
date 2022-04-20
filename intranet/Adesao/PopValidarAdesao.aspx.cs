using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.Adesao;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Adesao {
	public partial class PopValidarAdesao : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");

				this.btnSalvar.Visible = false;
				return;
			}
			litProtocolo.Text = cdProtocolo.ToString(PDeclaracaoVO.FORMATO_PROTOCOLO);
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADESAO; }
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(dpdValidacao.SelectedValue)) {
				this.ShowError("Informe se deseja validar ou invalidar a proposta!");
				return;
			}

			bool isValido = "S".Equals(dpdValidacao.SelectedValue);

			if (!isValido) {
				if (string.IsNullOrEmpty(txtMotivo.Text)) {
					this.ShowError("Informe o motivo da invalidação da proposta!");
					return;
				}
			}
			int id = Int32.Parse(Request["ID"]);
			PAdesaoBO.Instance.MarcarValidada(id, isValido, txtMotivo.Text.ToUpper(), UsuarioLogado.Id);
			this.RegisterScript("aqui", "setDone(" + id + ")");
		}

        protected void dpdValidacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtMotivo.Text = "";
            lblMotivo.Text = "Informe uma observação referente à validação:";
            btnSalvar.Text = "Validar proposta";

            if(dpdValidacao.SelectedValue == "N")
            {
                lblMotivo.Text = "Informe o motivo da invalidação da proposta:";
                btnSalvar.Text = "Invalidar proposta";
            }
        }

	}
}