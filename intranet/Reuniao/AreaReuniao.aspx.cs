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
	public partial class AreaReuniao : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<ConselhoVO> lstConselho = ConselhoBO.Instance.ListarConselhos();
				dpdConselho.DataSource = lstConselho;
				dpdConselho.DataBind();

				dpdConselho.Items.Insert(0, new ListItem("SELECIONE", ""));

				bool canChange = CheckPermissions();
				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id = Convert.ToInt32(Request["ID"]);
					Bind(id, canChange);
				} else {
					if (!canChange) {
						this.ShowError("Você não possui permissão para criar nova reunião!");
						return;
					}
				}

			}
		}

		protected override Modulo Modulo {
			get { return Modulo.VISUALIZAR_REUNIAO; }
		}

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

		protected bool HasGerencia() {
			return HasPermission(Modulo.GERENCIAR_REUNIAO);
		}

		private bool CheckPermissions() {
			bool canChange = HasGerencia();

			txtTitulo.Enabled = canChange;
			txtData.Enabled = canChange;
			txtDescricao.Enabled = canChange;
			dpdConselho.Enabled = canChange;
			btnSalvar.Visible = canChange;

			return canChange;
		}

		private void Bind(int id, bool canChange) {
			ReuniaoVO vo = ReuniaoBO.Instance.GetById(id);

			/*
			if (!canChange) {
				ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByUsuario(UsuarioLogado.Id);
				if (conselho == null || !vo.CodConselho.Equals(conselho.Codigo)) {
					this.ShowError("Você não faz parte do conselho desta reunião!");
					return;
				}
			}
			*/
			ViewState["ID"] = id;
			txtTitulo.Text = vo.Titulo;
			txtDescricao.Text = vo.Descricao;
			txtData.Text = vo.Data.ToShortDateString();
			dpdConselho.SelectedValue = vo.CodConselho;

			btnSalvar.Text = "Alterar reunião";

			BindArquivos(id, canChange);
		}

		private void BindArquivos(int id, bool canChange) {
			dvArquivos.Visible = true;

			List<ArquivoReuniaoVO> lstArquivos = ReuniaoBO.Instance.ListarArquivosByReuniao(id);
			if (lstArquivos == null)
				lstArquivos = new List<ArquivoReuniaoVO>();

			List<ArquivoTelaVO> lstArqs = lstArquivos.Select(x =>
				new ArquivoTelaVO()
				{
					Id = x.IdArquivo.ToString(),
					Descricao = x.Descricao,
					NomeTela = x.NomeArquivo,
					IsNew = false
				}).ToList();
			Arquivos = lstArqs;

			ltvArquivo.EditIndex = -1;
			ltvArquivo.DataSource = lstArqs;
			ltvArquivo.DataBind();

			btnIncluirArquivo.Visible = canChange;
		}

		private void SalvarReuniao() {
			DateTime data;

			if (string.IsNullOrEmpty(txtTitulo.Text)) {
				this.ShowError("Informe o título da reunião!");
				return;
			}

			if (string.IsNullOrEmpty(txtDescricao.Text)) {
				this.ShowError("Informe a descrição da reunião!");
				return;
			}

			if (string.IsNullOrEmpty(txtData.Text)) {
				this.ShowError("Informe a data da reunião!");
				return;
			} else {
				if (!DateTime.TryParse(txtData.Text, out data)) {
					this.ShowError("A data informada é inválida!");
					return;
				}
			}

			if (string.IsNullOrEmpty(dpdConselho.SelectedValue)) {
				this.ShowError("Informe o conselho da reunião!");
				return;
			}

			ReuniaoVO vo = new ReuniaoVO();
			bool isNew = ViewState["ID"] == null;
			if (!isNew) {
				vo.Id = Convert.ToInt32(ViewState["ID"]);
			}
			vo.CodConselho = dpdConselho.SelectedValue;
			vo.Data = data;
			vo.Descricao = txtDescricao.Text;
			vo.Titulo = txtTitulo.Text;

			ReuniaoBO.Instance.Salvar(vo, UsuarioLogado.Usuario);

			if (isNew)
				this.ShowInfo("Reunião criada com sucesso!");
			else
				this.ShowInfo("Reunião alterada com sucesso!");

			Bind(vo.Id, CheckPermissions());
		}

		#region Gestão Arquivos

		private ArquivoReuniaoVO ArquivoTela2VO(ArquivoTelaVO telaVO) {
			ArquivoReuniaoVO vo = new ArquivoReuniaoVO();
			int cdReuniao = Convert.ToInt32(ViewState["ID"]);
			if (!string.IsNullOrEmpty(telaVO.Id)) {
				vo.IdArquivo = Int32.Parse(telaVO.Id);
			}
			vo.CodReuniao = cdReuniao;
			vo.Descricao = telaVO.Descricao; 
			vo.NomeArquivo = telaVO.NomeTela;
			return vo;
		}

		private void AddArquivo(string fisico, string original) {
			List<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO vo = new ArquivoTelaVO()
			{
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
				ArquivoReuniaoVO vo = ArquivoTela2VO(telaVO);
				ReuniaoBO.Instance.RemoverArquivo(vo);
			}
			BindArquivos(cdReuniao, HasGerencia());
		}

		private void SalvarArquivo(ListViewDataItem row) {
			int idx = row.DataItemIndex;

			ArquivoTelaVO telaVO = Arquivos[idx];
			ArquivoReuniaoVO vo = ArquivoTela2VO(telaVO);

			TextBox txtDescricao = (TextBox)row.FindControl("txtDescricao");
			vo.Descricao = txtDescricao.Text;

			ReuniaoBO.Instance.SalvarArquivo(vo, telaVO.NomeFisico);

			BindArquivos(vo.CodReuniao, HasGerencia());
		}

		#endregion

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarReuniao();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar reuniao.", ex);
			}
		}

		protected void bntRemoverArquivo_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
				RemoverArquivo(row);
			}
			catch (Exception ex) {
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
			}
			catch (Exception ex) {
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
				this.ShowError("Erro ao disponibilizar para edição", ex);
			}

		}

	}
}