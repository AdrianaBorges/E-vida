using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Admin {
	public partial class Conselhos : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdPerfil.DataSource = AdministracaoBO.Instance.ListarPerfisConselho();
				dpdPerfil.DataBind();
				dpdPerfil.Items.Insert(0, new ListItem("SELECIONE", ""));

				BindAll();
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.ADMINISTRACAO_CONSELHO; }
		}

		protected string EditId {
			get { return (string)ViewState["ED_ID"]; }
			set { ViewState["ED_ID"] = value; }
		}

		private void BindAll() {
			List<ConselhoVO> lstConselhos = ConselhoBO.Instance.ListarConselhos();

			ltvConselho.DataSource = lstConselhos;
			ltvConselho.DataBind();
			updConselho.Update();
			ClearForm();
		}
		
		private void ClearForm() {
			txtCodigo.Enabled = true;
			txtCodigo.Text = string.Empty;
			txtNome.Enabled = true;
			txtNome.Text = string.Empty;
			EditId = null;
			btnNovo.Visible = false;
			updCadastro.Update();

			tbUsuarios.Visible = false;
			updUsuarios.Update();
		}

		#region Conselho

		private void SalvarConselho() {
			string codigo = txtCodigo.Text.ToUpper();
			string nome = txtNome.Text;

			if (string.IsNullOrEmpty(codigo)) {
				this.ShowError("Informe o código do órgão!");
				return;
			}
			if (string.IsNullOrEmpty(nome)) {
				this.ShowError("Informe o nome do órgão!");
				return;
			}

			ConselhoVO vo = new ConselhoVO();
			vo.Codigo = codigo;
			vo.Nome = nome;

			if (EditId == null) {
				if (ConselhoBO.Instance.ExisteConselho(vo.Codigo, vo.Nome)) {
					this.ShowError("Já existe um órgão com este código ou nome.");
					return;
				}

			}

			ConselhoBO.Instance.Salvar(vo, EditId == null);
			this.ShowInfo("Órgão salvo com sucesso!");

			BindAll();
		}

		private void RemoverConselho(ListViewItem row) {
			string id = (string)ltvConselho.DataKeys[row.DataItemIndex]["Codigo"];

			ConselhoVO vo = new ConselhoVO();
			vo.Codigo = id;

			if (ConselhoBO.Instance.IsConselhoUtilizado(vo)) {
				this.ShowError("Este órgão é utilizado dentro do sistema, não é possível excluí-lo!");
				return;
			}
			ConselhoBO.Instance.Excluir(vo);
			this.ShowInfo("Órgão removido com sucesso!");

			BindAll();
		}

		private void Editar(ListViewItem row) {
			string id = (string)ltvConselho.DataKeys[row.DataItemIndex]["Codigo"];

			ConselhoVO vo = ConselhoBO.Instance.GetConselhoByCodigo(id);
			txtCodigo.Text = vo.Codigo;
			txtNome.Text = vo.Nome;
			EditId = vo.Codigo;
			btnNovo.Visible = true;

			txtCodigo.Enabled = false;

			updCadastro.Update();

			CarregarUsuarios();
			CarregarArquivos();
		}


		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarConselho();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar órgão.", ex);
			}
		}

		protected void bntExcluir_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem row = (ListViewItem)btn.NamingContainer;

				RemoverConselho(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover órgão!", ex);
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

			if (!string.IsNullOrEmpty(dpdPerfil.SelectedValue)) {
				idPerfil = Int32.Parse(dpdPerfil.SelectedValue);
			}

			List<UsuarioVO> lstUsuarios = ConselhoBO.Instance.ListarUsuariosSemConselho(idPerfil);
			List<UsuarioVO> lstUsuariosConselho = ConselhoBO.Instance.ListarUsuariosByConselho(EditId);

			if (idPerfil != 0 && (lstUsuarios == null || lstUsuarios.Count == 0)) {
				this.ShowInfo("Todos os usuários deste perfil estão associados à um órgão.");
			}

			ltbUsuario.Items.Clear();
			ltbUsuario.DataSource = lstUsuarios;
			ltbUsuario.DataBind();

			ltbUsuarioConselho.Items.Clear();
			ltbUsuarioConselho.DataSource = lstUsuariosConselho;
			ltbUsuarioConselho.DataBind();

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
				this.ShowError("Selecione os usuários para incluir no órgão!");
				return;
			}
			if (string.IsNullOrEmpty(EditId)) {
				this.ShowError("Informe o órgão para adição!");
				return;
			}

			ConselhoBO.Instance.AddUsuarios(lstIds, EditId);
			this.ShowInfo("Usuários incluídos com sucesso!");
			CarregarUsuarios();
		}

		private void DelUsuarios() {

			List<int> lstIds = new List<int>();
			foreach (ListItem item in ltbUsuarioConselho.Items) {
				if (item.Selected)
					lstIds.Add(Int32.Parse(item.Value));
			}
			if (lstIds.Count == 0) {
				this.ShowError("Selecione os usuários para remover do órgão!");
				return;
			}
			if (string.IsNullOrEmpty(EditId)) {
				this.ShowError("Informe o órgão para remoção!");
				return;
			}

			ConselhoBO.Instance.DelUsuarios(lstIds, EditId);
			this.ShowInfo("Usuários retirados com sucesso!");
			CarregarUsuarios();
		}

		protected void btnAddUsuario_Click(object sender, EventArgs e) {
			try {
				AddUsuarios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao incluir usuários no órgão.", ex);
			}
		}

		protected void btnDelUsuario_Click(object sender, EventArgs e) {
			try {
				DelUsuarios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao retirar usuários do órgão.", ex);
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

		#region Arquivos

		public List<ArquivoTelaVO> Arquivos {
			get {
				if (ViewState["ARQUIVOS"] == null) {
					Arquivos = new List<ArquivoTelaVO>();
				}
				return ViewState["ARQUIVOS"] as List<ArquivoTelaVO>;
			}
			set {
				ViewState["ARQUIVOS"] = value;
			}
		}

		private void CarregarArquivos() {
			dvArquivos.Visible = true;

			List<ArquivoConselhoVO> lstArquivos = ConselhoBO.Instance.ListarArquivosConselho(EditId);
			if (lstArquivos == null)
				lstArquivos = new List<ArquivoConselhoVO>();

			List<ArquivoTelaVO> lstArqs = lstArquivos.Select(x =>
				new ArquivoTelaVO() {
					Id = x.IdArquivo.ToString(),
					Descricao = x.Descricao,
					NomeTela = x.NomeArquivo,
					IsNew = false
				}).ToList();
			Arquivos = lstArqs;

			ltvArquivo.EditIndex = -1;
			ltvArquivo.DataSource = lstArqs;
			ltvArquivo.DataBind();

			updArquivos.Update();
			btnIncluirArquivo.Visible = true;
		}

		private ArquivoConselhoVO ArquivoTela2VO(ArquivoTelaVO telaVO) {
			ArquivoConselhoVO vo = new ArquivoConselhoVO();
			string codConselho = EditId;
			if (!string.IsNullOrEmpty(telaVO.Id)) {
				vo.IdArquivo = Int32.Parse(telaVO.Id);
			}
			vo.CodConselho = EditId;
			vo.Descricao = telaVO.Descricao;
			vo.NomeArquivo = telaVO.NomeTela;
			return vo;
		}

		private void AddArquivo(string fisico, string original) {
			List<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO vo = new ArquivoTelaVO() {
				NomeFisico = fisico,
				NomeTela = original,
				Descricao = original,
				IsNew = true
			};
			bool contains = lstAtual.FindIndex(x => x.NomeTela.Equals(original, StringComparison.InvariantCultureIgnoreCase)) >= 0;
			if (!contains) {
				lstAtual.Add(vo);
			} else {
				this.ShowError("Este arquivo já existe na listagem! Por favor, exclua o antigo ou renomeie o arquivo novo!");
				return;
			}
			ltvArquivo.EditIndex = lstAtual.Count - 1;
			ltvArquivo.DataSource = lstAtual;
			ltvArquivo.DataBind();
			btnIncluirArquivo.Visible = false;
		}

		private void RemoverArquivo(ListViewDataItem row) {
			int idx = row.DataItemIndex;

			List<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO telaVO = lstAtual[idx];

			int cdReuniao = Convert.ToInt32(ViewState["ID"]);
			if (!telaVO.IsNew) {
				ArquivoConselhoVO vo = ArquivoTela2VO(telaVO);
				ConselhoBO.Instance.RemoverArquivo(vo);
			}
			CarregarArquivos();
		}

		private void SalvarArquivo(ListViewDataItem row) {
			int idx = row.DataItemIndex;

			ArquivoTelaVO telaVO = Arquivos[idx];
			ArquivoConselhoVO vo = ArquivoTela2VO(telaVO);

			TextBox txtDescricao = (TextBox)row.FindControl("txtDescricao");
			vo.Descricao = txtDescricao.Text;

			ConselhoBO.Instance.SalvarArquivo(vo, telaVO.NomeFisico);

			CarregarArquivos();
		}

		protected void bntRemoverArquivo_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
				RemoverArquivo(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover item da lista", ex);
			}
		}

		protected void btnIncluirArquivo_Click(object sender, EventArgs e) {
			AddArquivo(hidArqFisico.Value, hidArqOrigem.Value);
		}

		protected void btnSalvarArquivo_Click(object sender, EventArgs e) {
			try {
				Button btn = (Button)sender;
				ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
				SalvarArquivo(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao adicionar arquivo lista", ex);
			}
		}

		protected void bntEditarArquivo_Click(object sender, EventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
				ltvArquivo.EditIndex = row.DataItemIndex;

				List<ArquivoTelaVO> lstAtual = Arquivos;
				ltvArquivo.DataSource = lstAtual;
				ltvArquivo.DataBind();

			} catch (Exception ex) {
				this.ShowError("Erro ao adicionar arquivo lista", ex);
			}

		}

		#endregion

	}
}