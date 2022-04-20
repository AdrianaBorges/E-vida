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
	public partial class SetoresUsuario : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdPerfil.DataSource = AdministracaoBO.Instance.ListarTodosPerfis();
				dpdPerfil.DataBind();
				dpdPerfil.Items.Insert(0, new ListItem("SELECIONE", ""));

				BindAll();
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.ADMINISTRACAO_SETOR_USUARIO; }
		}

		protected string EditId {
			get { return (string)ViewState["ED_ID"]; }
			set { ViewState["ED_ID"] = value; }
		}

		private void BindAll() {
			List<SetorUsuarioVO> lstSetores = SetorUsuarioBO.Instance.ListarSetores();

			ltvSetor.DataSource = lstSetores;
			ltvSetor.DataBind();
			updSetor.Update();
			ClearForm();
		}

		private void ClearForm() {
			lblCodigo.Text = string.Empty;
			txtNome.Enabled = true;
			txtNome.Text = string.Empty;
			EditId = null;
			btnNovo.Visible = false;
			updCadastro.Update();

			tbUsuarios.Visible = false;
			updUsuarios.Update();
		}

		#region Setor

		private void SalvarSetor() {
			string nome = txtNome.Text;

			if (string.IsNullOrEmpty(nome)) {
				this.ShowError("Informe o nome do setor!");
				return;
			}

			SetorUsuarioVO vo = new SetorUsuarioVO();
			vo.Nome = nome;

			if (!string.IsNullOrEmpty(EditId)) {
				vo.Id = Int32.Parse(EditId);
			}
			if (SetorUsuarioBO.Instance.ExisteSetor(vo.Id, vo.Nome)) {
				this.ShowError("Já existe um setor com este nome.");
				return;
			}
			SetorUsuarioBO.Instance.Salvar(vo);
			this.ShowInfo("Setor salvo com sucesso!");

			BindAll();
		}

		private void RemoverSetor(ListViewItem row) {
			int id = Convert.ToInt32(ltvSetor.DataKeys[row.DataItemIndex]["Id"]);
			
			if (SetorUsuarioBO.Instance.IsSetorUtilizado(id)) {
				this.ShowError("Este setor é utilizado dentro do sistema, não é possível excluí-lo!");
				return;
			}
			SetorUsuarioVO vo = SetorUsuarioBO.Instance.GetSetorById(id);
			if (vo == null) {
				this.ShowError("Setor inexistente!");
				return;
			}
			SetorUsuarioBO.Instance.Excluir(vo);
			this.ShowInfo("Setor removido com sucesso!");

			BindAll();
		}

		private void Editar(ListViewItem row) {
			int id = Convert.ToInt32(ltvSetor.DataKeys[row.DataItemIndex]["Id"]);

			SetorUsuarioVO vo = SetorUsuarioBO.Instance.GetSetorById(id);
			lblCodigo.Text = vo.Id.ToString();
			txtNome.Text = vo.Nome;
			EditId = vo.Id.ToString();
			btnNovo.Visible = true;

			updCadastro.Update();

			CarregarUsuarios();
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarSetor();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar setor.", ex);
			}
		}

		protected void bntExcluir_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem row = (ListViewItem)btn.NamingContainer;

				RemoverSetor(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover setor!", ex);
			}
		}

		protected void btnEditar_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem row = (ListViewItem)btn.NamingContainer;

				Editar(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao montar formulário para edição!", ex);
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			ClearForm();
		}

		#endregion

		#region Usuarios

		private void CarregarUsuarios() {
			int idPerfil = 0;
			int idSetor = 0;

			if (!string.IsNullOrEmpty(dpdPerfil.SelectedValue)) {
				idPerfil = Int32.Parse(dpdPerfil.SelectedValue);
			}
			if (!string.IsNullOrEmpty(EditId))
				idSetor = Int32.Parse(EditId);

			List<UsuarioVO> lstUsuarios = SetorUsuarioBO.Instance.ListarUsuariosNaoAssociados(idPerfil, idSetor);
			List<UsuarioVO> lstUsuariosConselho = SetorUsuarioBO.Instance.ListarUsuariosBySetor(idSetor);

			if (lstUsuarios == null || lstUsuarios.Count == 0) {
				if (idPerfil != 0)
					this.ShowInfo("Não existem usuários neste perfil para associação.");
			}

			ltbUsuario.Items.Clear();
			ltbUsuario.DataSource = lstUsuarios;
			ltbUsuario.DataBind();

			ltbUsuarioSetor.Items.Clear();
			ltbUsuarioSetor.DataSource = lstUsuariosConselho;
			ltbUsuarioSetor.DataBind();

			tbUsuarios.Visible = true;
			updUsuarios.Update();
		}

		private void AddUsuarios() {
			int idSetor = 0;
			if (string.IsNullOrEmpty(EditId)) {
				this.ShowError("Informe o setor para adição!");
				return;
			}

			idSetor = Int32.Parse(EditId);

			List<int> lstIds = new List<int>();
			foreach (ListItem item in ltbUsuario.Items) {
				if (item.Selected)
					lstIds.Add(Int32.Parse(item.Value));
			}
			if (lstIds.Count == 0) {
				this.ShowError("Selecione os usuários para incluir no setor!");
				return;
			}

			SetorUsuarioBO.Instance.AddUsuarios(lstIds, idSetor);
			this.ShowInfo("Usuários incluídos com sucesso!");
			CarregarUsuarios();
		}

		private void DelUsuarios() {
			int idSetor = 0;
			if (string.IsNullOrEmpty(EditId)) {
				this.ShowError("Informe o setor para adição!");
				return;
			}

			idSetor = Int32.Parse(EditId);

			List<int> lstIds = new List<int>();
			foreach (ListItem item in ltbUsuarioSetor.Items) {
				if (item.Selected)
					lstIds.Add(Int32.Parse(item.Value));
			}
			if (lstIds.Count == 0) {
				this.ShowError("Selecione os usuários para remover do setor!");
				return;
			}

			SetorUsuarioBO.Instance.DelUsuarios(lstIds, idSetor);
			this.ShowInfo("Usuários retirados com sucesso!");
			CarregarUsuarios();
		}

		protected void btnAddUsuario_Click(object sender, EventArgs e) {
			try {
				AddUsuarios();
			} catch (Exception ex) {
				this.ShowError("Erro ao incluir usuários no setor.", ex);
			}
		}

		protected void btnDelUsuario_Click(object sender, EventArgs e) {
			try {
				DelUsuarios();
			} catch (Exception ex) {
				this.ShowError("Erro ao retirar usuários do setor.", ex);
			}
		}

		protected void dpdPerfil_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				CarregarUsuarios();
			} catch (Exception ex) {
				this.ShowError("Erro ao listar usuários do perfil.", ex);
			}
		}

		#endregion Usuarios

	}
}