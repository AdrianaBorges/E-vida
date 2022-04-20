using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Telefonia {
	public partial class Ramais : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdPerfil.DataSource = AdministracaoBO.Instance.ListarTodosPerfis();
				dpdPerfil.DataBind();
				dpdPerfil.Items.Insert(0, new ListItem("SELECIONE", ""));

				dpdSetor.DataSource = SetorUsuarioBO.Instance.ListarSetores();
				dpdSetor.DataBind();
				dpdSetor.Items.Insert(0, new ListItem("SELECIONE", ""));

				BindAll();
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.TELEFONIA_RAMAL; }
		}

		protected string EditId {
			get { return (string)ViewState["ED_ID"]; }
			set { ViewState["ED_ID"] = value; }
		}

		private void BindAll() {
			List<RamalVO> lstSetores = RamalBO.Instance.ListarRamais();

			ltvRamal.DataSource = lstSetores;
			ltvRamal.DataBind();
			updRamal.Update();
			ClearForm();
		}

		private void ClearForm() {
			txtRamal.Text = string.Empty;
			txtRamal.Enabled = true;						
			EditId = null;

			dpdTipo.SelectedValue = "SETOR";
			mtvTipoRamal.ActiveViewIndex = 0;

			btnNovo.Visible = false;
			updCadastro.Update();

			tbUsuarios.Visible = false;
			updRamal.Update();
		}

		#region Ramal
		private void SalvarRamal() {
			string strNroRamal = txtRamal.Text;
			int nroRamal;
			List<int> lstIdUsuarios = new List<int>();
			int idSetor = 0;

			if (string.IsNullOrEmpty(strNroRamal)) {
				this.ShowError("Informe o número do ramal!");
				return;
			}
			if (!Int32.TryParse(strNroRamal, out nroRamal)) {
				this.ShowError("O ramal deve ser numérico!");
				return;
			}

			if (dpdTipo.SelectedValue.Equals("SETOR")) {
				if (string.IsNullOrEmpty(dpdSetor.SelectedValue)) {
					this.ShowError("Informe o setor associado ao ramal!");
					return;
				}
				if (!Int32.TryParse(dpdSetor.SelectedValue, out idSetor)) {
					this.ShowError("Seleção de setor inválida!");
					return;
				}
			} else {
				foreach (ListItem item in ltbUsuarioRamal.Items) {
					lstIdUsuarios.Add(Int32.Parse(item.Value));
				}
				if (lstIdUsuarios.Count == 0) {
					this.ShowError("Selecione os usuários para associar ao ramal!");
					return;
				}
			}

			RamalVO vo = new RamalVO();
			vo.NrRamal = nroRamal;
			vo.Tipo = dpdTipo.SelectedValue;
			vo.Alias = txtAlias.Text;
			if (idSetor != 0)
				vo.IdSetor = idSetor;

			bool isNew = true;
			if (!string.IsNullOrEmpty(EditId)) {
				isNew = false;
			} else {
				if (RamalBO.Instance.ExisteRamal(nroRamal)) {
					this.ShowError("Já existe um ramal com este número.");
					return;
				}
				if (!string.IsNullOrEmpty(vo.Alias) && RamalBO.Instance.ExisteRamal(vo.Alias)) {
					this.ShowError("Já existe um ramal com este alias.");
					return;
				}
			}

			vo.Usuarios = lstIdUsuarios;

			RamalBO.Instance.Salvar(vo, isNew);
			this.ShowInfo("Setor salvo com sucesso!");

			BindAll();
		}

		private void RemoverRamal(ListViewItem row) {
			int id = Convert.ToInt32(ltvRamal.DataKeys[row.DataItemIndex]["NrRamal"]);

			if (RamalBO.Instance.IsRamalUtilizado(id)) {
				this.ShowError("Este ramal é utilizado dentro do sistema, não é possível excluí-lo!");
				return;
			}
			RamalVO vo = RamalBO.Instance.GetRamalByNro(id);
			if (vo == null) {
				this.ShowError("Ramal inexistente!");
				return;
			}
			RamalBO.Instance.Excluir(vo);
			this.ShowInfo("Ramal removido com sucesso!");

			BindAll();
		}

		private void Editar(ListViewItem row) {
			int id = Convert.ToInt32(ltvRamal.DataKeys[row.DataItemIndex]["NrRamal"]);

			RamalVO vo = RamalBO.Instance.GetRamalByNro(id);
			EditId = vo.NrRamal.ToString();
			txtRamal.Text = vo.NrRamal.ToString();
			txtRamal.Enabled = false;

			txtAlias.Text = vo.Alias;

			dpdTipo.SelectedValue = vo.Tipo;
			if ("SETOR".Equals(vo.Tipo)) {
				mtvTipoRamal.ActiveViewIndex = 0;
				dpdSetor.SelectedValue = vo.IdSetor.ToString();
			} else {
				mtvTipoRamal.ActiveViewIndex = 1;
				CarregarUsuarios();
			}

			btnNovo.Visible = true;

			updCadastro.Update();

		}


		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarRamal();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar ramal.", ex);
			}
		}

		protected void bntExcluir_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem row = (ListViewItem)btn.NamingContainer;

				RemoverRamal(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover ramal!", ex);
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
			int idRamal = 0;

			if (!string.IsNullOrEmpty(dpdPerfil.SelectedValue)) {
				idPerfil = Int32.Parse(dpdPerfil.SelectedValue);
			}
			if (!string.IsNullOrEmpty(EditId))
				idRamal = Int32.Parse(EditId);

			List<UsuarioVO> lstUsuarios = RamalBO.Instance.ListarUsuariosNaoAssociados(idPerfil, idRamal);
			List<UsuarioVO> lstUsuariosConselho = RamalBO.Instance.ListarUsuariosByRamal(idRamal);

			if (lstUsuarios == null || lstUsuarios.Count == 0) {
				if (idPerfil != 0)
					this.ShowInfo("Não existem usuários neste perfil para associação.");
			}

			ltbUsuario.Items.Clear();
			ltbUsuario.DataSource = lstUsuarios;
			ltbUsuario.DataBind();

			ltbUsuarioRamal.Items.Clear();
			ltbUsuarioRamal.DataSource = lstUsuariosConselho;
			ltbUsuarioRamal.DataBind();

			tbUsuarios.Visible = true;
			updUsuarios.Update();
		}

		private void AddUsuarios() {
			List<int> lstIds = new List<int>();
			foreach (ListItem item in ltbUsuario.Items) {
				if (item.Selected)
					lstIds.Add(Int32.Parse(item.Value));
			}
			if (lstIds.Count == 0) {
				this.ShowError("Selecione os usuários para incluir no ramal!");
				return;
			}
			foreach (int id in lstIds) {
				ListItem item = ltbUsuario.Items.FindByValue(id.ToString());
				ltbUsuario.Items.Remove(item);
				ltbUsuarioRamal.Items.Add(item);
			}
		}

		private void DelUsuarios() {
			List<int> lstIds = new List<int>();
			foreach (ListItem item in ltbUsuarioRamal.Items) {
				if (item.Selected)
					lstIds.Add(Int32.Parse(item.Value));
			}
			if (lstIds.Count == 0) {
				this.ShowError("Selecione os usuários para remover do ramal!");
				return;
			}

			foreach (int id in lstIds) {
				ListItem item = ltbUsuario.Items.FindByValue(id.ToString());
				ltbUsuarioRamal.Items.Remove(item);
				ltbUsuario.Items.Add(item);
			}
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

		protected void dpdTipo_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				if (dpdTipo.SelectedValue.Equals("SETOR"))
					mtvTipoRamal.ActiveViewIndex = 0;
				else
					mtvTipoRamal.ActiveViewIndex = 1;
				CarregarUsuarios();
				updCadastro.Update();
			} catch (Exception ex) {
				this.ShowError("Erro ao trocar tipo.", ex);
			}
		}

		protected void ltvRamal_ItemDataBound(object sender, ListViewItemEventArgs e) {
			ListViewItem item = e.Item;
			if (item.ItemType == ListViewItemType.DataItem) {
				Literal ltAssociacao = (Literal)item.FindControl("ltAssociacao");
				RamalVO vo = (RamalVO)item.DataItem;

				if (vo.Tipo.Equals("SETOR")) {
					SetorUsuarioVO setor = SetorUsuarioBO.Instance.GetSetorById(vo.IdSetor.Value);
					if (setor != null)
						ltAssociacao.Text = setor.Nome;
					else
						ltAssociacao.Text = "SETOR DESCONHECIDO";
				} else {
					List<UsuarioVO> lstUsuarios = UsuarioBO.Instance.ListarUsuarios();
					List<UsuarioVO> lstToBind = lstUsuarios.FindAll(x => vo.Usuarios.Contains(x.Id));

					ltAssociacao.Text = lstToBind.Select(x => x.Nome).Aggregate((x, y) => x + ", " + y);
				}
			}
		}

	}
}