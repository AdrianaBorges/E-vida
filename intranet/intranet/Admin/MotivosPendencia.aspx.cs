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
	public partial class MotivosPendencia : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

				dpdTipo.Items.Add(new ListItem("SELECIONE", ""));
				foreach (TipoMotivoPendencia motivo in Enum.GetValues(typeof(TipoMotivoPendencia))) {
					dpdTipo.Items.Add(new ListItem(MotivoPendenciaEnumTradutor.TraduzTipo(motivo), ((int)motivo).ToString()));
				}

				BindAll();
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.ADMINISTRACAO_MOTIVO_PENDENCIA; }
		}

		private int? EditId {
			get { return (int?)ViewState["ED_ID"]; }
			set { ViewState["ED_ID"] = value; }
		}

		private void BindAll() {
			List<MotivoPendenciaVO> lstLocais = MotivoPendenciaBO.Instance.ListarAllMotivos();

			ltvMotivosPendencia.DataSource = lstLocais;
			ltvMotivosPendencia.DataBind();

			updMotivos.Update();
		}

		private void ClearForm() {
			litId.Text = string.Empty;
			dpdTipo.Enabled = true;
			dpdTipo.SelectedValue = string.Empty;
			txtNome.Text = string.Empty;
			EditId = null;
			btnNovo.Visible = false;
		}

		private void SalvarMotivo() {
			string tipo = dpdTipo.SelectedValue;
			string nome = txtNome.Text;

			if (string.IsNullOrEmpty(tipo)) {
				this.ShowError("Selecione o tipo do motivo!");
				return;
			}
			if (string.IsNullOrEmpty(nome)) {
				this.ShowError("Informe o nome do motivo!");
				return;
			}

			MotivoPendenciaVO vo = new MotivoPendenciaVO();			
			vo.Tipo = (TipoMotivoPendencia) Convert.ToInt32(tipo);
			vo.Nome = nome;

			if (EditId != null) {
				vo.Id = EditId.Value;
				if (!MotivoPendenciaBO.Instance.CheckAlteracao(vo)) {
					this.ShowError("Este motivo já está associado a uma entidade, não é possível alterar o tipo.");
					return;
				}
			}

			MotivoPendenciaBO.Instance.Salvar(vo);
			this.ShowInfo("Motivo salvo com sucesso!");

			ClearForm();
			BindAll();
		}

		private void RemoverMotivo(ListViewItem row) {
			int id = (int)ltvMotivosPendencia.DataKeys[row.DataItemIndex]["Id"];

			if (MotivoPendenciaBO.Instance.IsMotivoUtilizado(id)) {
				this.ShowError("Este motivo já está associado a uma entidade!");
				return;
			}
			MotivoPendenciaBO.Instance.Excluir(id);
			this.ShowInfo("Motivo removido com sucesso!");

			ClearForm();
			BindAll();
		}

		private void Editar(ListViewItem row) {
			int id = (int)ltvMotivosPendencia.DataKeys[row.DataItemIndex]["Id"];

			MotivoPendenciaVO vo = MotivoPendenciaBO.Instance.GetById(id);

			litId.Text = vo.Id.ToString();
			txtNome.Text = vo.Nome;
			dpdTipo.SelectedValue = ((int)vo.Tipo).ToString();

			EditId = id;
			btnNovo.Visible = true;
			
			updUf.Update();
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarMotivo();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar motivo.", ex);
			}
		}

		protected void bntExcluir_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem ufRow = (ListViewItem)btn.NamingContainer;

				RemoverMotivo(ufRow);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover motivo!", ex);
			}
		}

		protected void btnEditar_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem ufRow = (ListViewItem)btn.NamingContainer;

				Editar(ufRow);
			} catch (Exception ex) {
				this.ShowError("Erro ao montar formulário para edição de motivo!", ex);
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			ClearForm();
		}


	}
}