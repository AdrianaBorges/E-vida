using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes.CanalGestante;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.BO;

namespace eVidaBeneficiarios.CanalGestante {
	public partial class PopSolEsclarecimento : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {

			if (!IsPostBack) {
				string sID = Request["ID"];
				if (string.IsNullOrEmpty(sID)) {
					this.ShowError("ID de requisição invalido!");
					return;
				}
				int id;
				if (!Int32.TryParse(sID, out id)) {
					this.ShowError("ID inválido.");
					return;
				}

				dpdCanalResposta.Items.Add(new ListItem("SELECIONE", ""));
				dpdCanalResposta.Items.Add(new ListItem("CARTA", CanalGestanteVO.TIPO_CONTATO_CARTA));
				dpdCanalResposta.Items.Add(new ListItem("E-MAIL", CanalGestanteVO.TIPO_CONTATO_EMAIL));
				dpdCanalResposta.Items.Add(new ListItem("PRESENCIALMENTE", CanalGestanteVO.TIPO_CONTATO_PRESENCIAL));

				Bind(id);
			}
		}

		public int Id {
			get { return (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int id) {
			this.Id = id;
			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(id);
			litProtocolo.Text = vo.Id.ToString() + "/" + vo.DataSolicitacao.Year;
			btnEnviar.Visible = true;

		}

		private void Enviar() {
			int id = Id;
			string tipoContato = dpdCanalResposta.SelectedValue;
			string mensagem = txtDuvida.Text;

			if (string.IsNullOrEmpty(tipoContato)) {
				this.ShowError("Informe o tipo de contato desejado!");
				return;
			}
			if (string.IsNullOrEmpty(mensagem)) {
				this.ShowError("Informe o esclarecimento desejado!");
				return;
			}

			CanalGestanteBO.Instance.SolicitarEsclarecimento(id, tipoContato, mensagem);
			
			btnEnviar.Visible = false;
			this.ShowInfo("EM BREVE A E-VIDA ENTRARÁ EM CONTATO PARA OS DEVIDOS ESCLARECIMENTOS, FAVOR AGUARDAR.");

			this.RegisterScript("OK", "isMsgSalvaOK = true;");
		}

		protected void btnEnviar_Click(object sender, EventArgs e) {
			try {
				Enviar();
			} catch (Exception ex) {
				this.ShowError("Erro ao enviar solicitação!", ex);
			}
		}

	}
}