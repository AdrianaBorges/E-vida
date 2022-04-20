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
	public partial class BuscarTemplatesEmail : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

				dpdTipo.Items.Add(new ListItem("TODOS", ""));
				foreach (TipoTemplateEmail tipo in Enum.GetValues(typeof(TipoTemplateEmail))) {
					dpdTipo.Items.Add(new ListItem(TemplateEmailEnumTradutor.TraduzTipo(tipo), ((int)tipo).ToString()));
				}
				dpdTipo.DataBind();
			}
		}


		protected override Modulo Modulo {
			get { return Modulo.ADMINISTRACAO_TEMPLATE_EMAIL; }
		}

		private void Buscar() {
			IEnumerable<TemplateEmailVO> lstTemplates = TemplateEmailBO.Instance.ListarTemplates();
			if (lstTemplates != null) {
				TipoTemplateEmail? tipo =  null;
				if (!string.IsNullOrEmpty(dpdTipo.SelectedValue)) {
					tipo = (TipoTemplateEmail)Convert.ToInt32(dpdTipo.SelectedValue);
				}
				if (tipo != null)
					lstTemplates = lstTemplates.Where(x => x.Tipo == tipo);

				if (!string.IsNullOrEmpty(txtNome.Text)) {
					lstTemplates = lstTemplates.Where(x => x.Nome.ToUpper().Contains(txtNome.Text.ToUpper()));
				}
			} else {
				lstTemplates = new List<TemplateEmailVO>();
			}

			gdvRelatorio.DataSource = lstTemplates;
			gdvRelatorio.DataBind();
			lblCount.Text = "Foram encontrados " + lstTemplates.Count() + " registros.";
		}

		private void RemoverTemplate(GridViewRow row) {
			int id = (int)gdvRelatorio.DataKeys[row.DataItemIndex]["Id"];

			TemplateEmailVO vo = TemplateEmailBO.Instance.GetById(id);
			TemplateEmailBO.Instance.Excluir(vo);
			this.ShowInfo("Template removido com sucesso!");
			Buscar();
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {

		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar busca", ex);
			}
		}

		protected void bntExcluir_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				GridViewRow ufRow = (GridViewRow)btn.NamingContainer;

				RemoverTemplate(ufRow);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover template!", ex);
			}
		}

	}
}