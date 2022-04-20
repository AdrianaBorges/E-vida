using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.GenPops {
	public partial class PopEmailTemplate : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				if (string.IsNullOrEmpty(Request["TIPO"])) {
					this.ShowError("Requisição inválida! Tipo inválido!");
					btnEnviar.Visible = false;
					return;
				}

				TipoTemplateEmail tipo = (TipoTemplateEmail)Convert.ToInt32(Request["TIPO"]);
				Tipo = tipo;

				List<TemplateEmailVO> lstTemplates = TemplateEmailBO.Instance.ListarTemplates();
				dpdTemplate.DataSource = lstTemplates;
				dpdTemplate.DataBind();
				dpdTemplate.Items.Insert(0, new ListItem("SELECIONE", ""));

				if (lstTemplates == null || lstTemplates.Count == 0) {
					this.ShowError("Não existem templates de e-mail cadastrados para o tipo " + TemplateEmailEnumTradutor.TraduzTipo(tipo));
					btnEnviar.Visible = false;
				}

				switch (tipo) {
					case TipoTemplateEmail.PROTOCOLO_FATURA: mtvTemplate.ActiveViewIndex = 0;
						break;
					default:
						break;
				}
			}
		}

		public TipoTemplateEmail Tipo {
			get { return (TipoTemplateEmail)ViewState["TIPO"]; }
			set { ViewState["TIPO"] = value; }
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		private void Enviar() {
			GerarEmail(true);
		}

		private void Visualizar() {
			GerarEmail(false);
		}

		private void GerarEmail(bool enviar = false) {
			if (string.IsNullOrEmpty(dpdTemplate.SelectedValue)) {
				this.ShowError("Selecione o template de e-mail para envio!");
				return;
			}

			int idTemplate = Convert.ToInt32(dpdTemplate.SelectedValue);

			SortedDictionary<string, string> parametros = BuildParametros();
			if (parametros == null)
				return;

			string conteudo = TemplateEmailBO.Instance.GerarEmail(idTemplate, parametros, enviar);
			if (!enviar)
				pnlVisualizacao.Visible = true;
			
			if (pnlVisualizacao.Visible)
				this.RegisterScript("UPD_FRAME", "updVisualizacao('" + this.frmVisualizacao.ClientID + "', \"" + conteudo + "\");");

			if (enviar) {
				this.ShowInfo("Email enviado com sucesso!");
			}
		}

		private SortedDictionary<string, string> BuildParametros() {
			SortedDictionary<string, string> parametros = new SortedDictionary<string, string>();
			if (Tipo == TipoTemplateEmail.PROTOCOLO_FATURA) {
				int id;
				if (!Int32.TryParse(Request["ID"], out id)) {
					this.ShowError("Não é possível identificar o protocolo de fatura.");
					return null;
				}
				parametros.Add("MENSAGEM", txtMsgGenerico.Text);
				parametros.Add("ID", id.ToString());
			}
			return parametros;
		}

		protected void btnEnviar_Click(object sender, EventArgs e) {
			try {
				Enviar();
			} catch (Exception ex) {
				this.ShowError("Erro ao enviar email. ", ex);
			}
		}

		protected void btnVisualizar_Click(object sender, EventArgs e) {
			try {
				Visualizar();
			} catch (Exception ex) {
				this.ShowError("Erro ao gerar visualização. ", ex);
			}
		}

		
	}
}