using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Admin {
	public partial class TemplateEmail : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdTipo.Items.Add(new ListItem("SELECIONE", ""));
				foreach (TipoTemplateEmail tipo in Enum.GetValues(typeof(TipoTemplateEmail))) {
					dpdTipo.Items.Add(new ListItem(TemplateEmailEnumTradutor.TraduzTipo(tipo), ((int)tipo).ToString()));
				}
				dpdTipo.DataBind();

				if (!string.IsNullOrEmpty(Request.QueryString["ID"])) {
					int id = Int32.Parse(Request.QueryString["ID"]);
					Bind(id);
				} else {
					
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_TEMPLATE_EMAIL; }
		}

		private int? Id {
			get { return ViewState["ID"] != null ? (int)ViewState["ID"] : new int?(); }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int id) {
			try {
				TemplateEmailVO vo = TemplateEmailBO.Instance.GetById(id);
				if (vo == null) {
					this.ShowError("Template inexistente!");
					return;
				}
				Id = id;

				this.ltId.Text = vo.Id.ToString();
				this.txtNome.Text = vo.Nome;
				this.txtTexto.Text = vo.Texto;
				dpdTipo.SelectedValue = ((int)vo.Tipo).ToString();
				BindTags();
			} catch (Exception ex) {
				this.ShowError("Erro ao carregar dados do template! ", ex);
			}
		}

		private void BindTags() {
			string[] tags = new string[] { };
			if (!string.IsNullOrEmpty(dpdTipo.SelectedValue)) {
				txtTexto.Enabled = true;
				TipoTemplateEmail tipo = (TipoTemplateEmail)Convert.ToInt32(dpdTipo.SelectedValue);
				tags = TemplateEmailBO.Instance.GetTagsByTipo(tipo);
			} else {
				txtTexto.Enabled = false;
			}
			rptTag.DataSource = tags;
			rptTag.DataBind();
		}

		private void Salvar() {
			if (string.IsNullOrEmpty(txtNome.Text)) {
				this.ShowError("Informe o nome do template!");
				return;
			}
			if (string.IsNullOrEmpty(dpdTipo.SelectedValue)) {
				this.ShowError("Selecione o tipo do template");
				return;
			}

			TemplateEmailVO vo = new TemplateEmailVO();
			if (Id != null)
				vo.Id = Id.Value;
			vo.Nome = txtNome.Text;
			vo.Texto = txtTexto.Text;
			vo.Tipo = (TipoTemplateEmail)Convert.ToInt32(dpdTipo.SelectedValue);

			TemplateEmailBO.Instance.Salvar(vo, UsuarioLogado.Id);

			this.ShowInfo("Template salvo com sucesso!");
			Bind(vo.Id);
		}
		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar o template!", ex);
			}
		}

		protected void dpdTipo_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				BindTags();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar tags do tipo de template.", ex);
			}
		}


	}
}