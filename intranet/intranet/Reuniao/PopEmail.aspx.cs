using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Reuniao {
	public partial class PopEmail : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int id;
				if (!Int32.TryParse(Request["ID"], out id)) {
					this.ShowError("Identificador inválido!");
					btnEnviar.Visible = false;
					return;
				}
				ReuniaoVO vo = ReuniaoBO.Instance.GetById(id);
				ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByCodigo(vo.CodConselho);
				ltConselho.Text = conselho.Codigo + " - " + conselho.Nome;
				txtEmail.Text = vo.Email;
				txtAssunto.Text = vo.AssuntoEmail;
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GERENCIAR_REUNIAO; }
		}

		protected void btnEnviar_Click(object sender, EventArgs e) {
			try {
				if (string.IsNullOrEmpty(txtAssunto.Text)) {
					this.ShowError("Informe o assunto do e-mail!");
					return;
				}
				if (string.IsNullOrEmpty(txtEmail.Text)) {
					this.ShowError("Informe o conteúdo do e-mail!");
					return;
				}
				int id = Int32.Parse(Request["ID"]);
				ReuniaoBO.Instance.EnviarEmail(id, txtAssunto.Text, txtEmail.Text);
				this.ShowInfo("Email enviado com sucesso!");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao enviar e-mail para conselho!", ex);
			}
		}
	}
}