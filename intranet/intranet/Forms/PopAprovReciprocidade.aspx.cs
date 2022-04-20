using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.Forms {
	public partial class PopAprovReciprocidade : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");
				this.btnSalvar.Visible = false;
				return;
			}

			hidRnd.Value = cdProtocolo.ToString();
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RECIPROCIDADE_GESTAO; }
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			int id = Int32.Parse(Request["ID"]);

			string arquivo = (!this.fileName.Value.Equals(ofileName.Value)) ? fileName.Value : "";
			//string dir = Server.MapPath("~/arquivos/reciprocidade");
			ReciprocidadeBO.Instance.Aprovar(id, txtObservacao.Text.ToUpper(), arquivo, UsuarioLogado.Id);

			this.RegisterScript("aqui", "setAprovacao(" + id + ")");
		}
		
	}
}