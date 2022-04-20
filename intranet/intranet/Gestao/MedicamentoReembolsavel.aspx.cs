using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Gestao {
	public partial class MedicamentoReembolsavel : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<PPlanoVO> lstPlano = PlanoBO.Instance.ListarPlanos();
				chkLstPlano.DataSource = lstPlano;
				chkLstPlano.DataBind();

				String id = Request.QueryString["ID"];
				if (!string.IsNullOrEmpty(id)) {
					if (HasPermission(Modulo.GESTAO_MEDICAMENTO_REEMBOLSAVEL_EDICAO)) {
						btnSalvar.Visible = btnIncluirArq.Visible = CanEditFile;
						btnLocPrincipio.Visible = CanEditFile;
					}
					Bind(id);
				} else {
					this.ShowError("Serviço não selecionado!!!");
					this.Form.Disabled = true;
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_MEDICAMENTO_REEMBOLSAVEL; }
		}

		protected string Id {
			get { return (string)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		public bool CanEditFile { get { return HasPermission(Modulo.GESTAO_MEDICAMENTO_REEMBOLSAVEL_EDICAO); } }


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

		private void BindPrincipio(int idPrincipio) {
			PrincipioAtivoVO principio = MedicamentoReembolsavelBO.Instance.GetPrincipioById(idPrincipio);
			hidPrincipio.Value = idPrincipio.ToString();
			lblPrincipio.Text = principio.Descricao;
		}

		private void Bind(string id) {
			try {
				Id = id;
				string mascara = Id;
				PServicoVO vo = ServicoBO.Instance.GetByMascara(mascara);
				if (vo == null) {
					this.ShowError("Medicamento/Material inexistente!");
					return;
				}
				Id = id;
				this.ltMascara.Text = mascara;
				this.ltDescricao.Text = vo.Descricao;

				dvArquivos.Visible = true;
				MedicamentoReembolsavelVO medVO = MedicamentoReembolsavelBO.Instance.GetById(mascara);
				if (medVO != null) {
					UsuarioVO uAlteracao = UsuarioBO.Instance.GetUsuarioById(medVO.IdUusarioAlteracao);
					ltAlteradoPor.Text = uAlteracao.Login + " - " + uAlteracao.Nome + " - " + medVO.DataAlteracao.ToString();
					UsuarioVO uCriacao = UsuarioBO.Instance.GetUsuarioById(medVO.IdUsuarioCriacao);
					ltCriadoPor.Text = uCriacao.Login + " - " + uCriacao.Nome + " - " + medVO.DataCriacao.ToString();

					BindPrincipio(medVO.IdPrincipioAtivo);
					txtObs.Text = medVO.Obs;

					dpdReembolsavel.SelectedValue = medVO.Reembolsavel ? "S" : "N";
					dpdUsoContinuo.SelectedValue = medVO.UsoContinuo ? "S" : "N";

					foreach (ListItem item in chkLstPlano.Items) {
						item.Selected = (medVO.Planos != null && medVO.Planos.Contains(item.Value));
					}

					CarregarArquivos();
				} else {
					ltvArquivo.DataSource = new List<ArquivoTelaVO>();
					ltvArquivo.DataBind();
				}

			} catch (Exception ex) {
				this.ShowError("Erro ao carregar dados! ", ex);
			}
		}

		private MedicamentoReembolsavelVO FillVO() {
			string mascara = Id;

			List<string> lstPlanos = new List<string>();
			foreach (ListItem item in chkLstPlano.Items) {
				if (item.Selected) lstPlanos.Add(item.Value);
			}

			MedicamentoReembolsavelVO vo = new MedicamentoReembolsavelVO();
			vo.Mascara = mascara;
			vo.Descricao = ltDescricao.Text;
			vo.Obs = txtObs.Text;
			vo.Planos = lstPlanos;
			vo.IdPrincipioAtivo = Convert.ToInt32(hidPrincipio.Value);
			vo.Reembolsavel = dpdReembolsavel.SelectedValue.Equals("S");
			vo.UsoContinuo = dpdUsoContinuo.SelectedValue.Equals("S");
			return vo;
		}

		private void Salvar() {
			if (string.IsNullOrEmpty(hidPrincipio.Value)) {
				this.ShowError("Informe o princípio ativo!");
				return;
			}
			MedicamentoReembolsavelVO vo = FillVO();
			if (vo == null) {
				this.ShowError("Erro ao ler campos da tela!");
				return;
			}
			if (vo.Planos.Count == 0) {
				this.ShowError("Informe pelo menos um plano!");
				return;
			}

			bool replicar = "true".Equals(hidReplica.Value);

			MedicamentoReembolsavelBO.Instance.Salvar(vo, replicar, UsuarioLogado.Id);
			this.ShowInfo("Registro salvo com sucesso!");
			Bind(Id);			
		}

		private void CarregarArquivos() {
			string mascara = Id;

			List<MedicamentoReembolsavelArqVO> lstArquivos = MedicamentoReembolsavelBO.Instance.ListarArquivos(mascara);
			if (lstArquivos == null)
				lstArquivos = new List<MedicamentoReembolsavelArqVO>();

			List<ArquivoTelaVO> lstArqs = lstArquivos.Select(x => ArquivoVO2Tela(x)).ToList();
			Arquivos = lstArqs;

			ltvArquivo.DataSource = lstArqs;
			ltvArquivo.DataBind();

			updArquivos.Update();
		}

		private static ArquivoTelaVO ArquivoVO2Tela(MedicamentoReembolsavelArqVO x) {
			ArquivoTelaVO vo = new ArquivoTelaVO() {
				Id = x.IdArquivo.ToString(),
				NomeTela = x.NomeArquivo,
				IsNew = false,
				Parameters = new Dictionary<string, string>()
			};
			return vo;
		}

		private MedicamentoReembolsavelArqVO ArquivoTela2VO(ArquivoTelaVO telaVO) {
			MedicamentoReembolsavelArqVO vo = new MedicamentoReembolsavelArqVO();
			if (!string.IsNullOrEmpty(telaVO.Id)) {
				vo.IdArquivo = Int32.Parse(telaVO.Id);
			}
			string mascara = Id;
			vo.Mascara = mascara;
			vo.NomeArquivo = telaVO.NomeTela;
			return vo;
		}
		private void RemoverArquivo(ListViewDataItem row) {
			int idx = row.DataItemIndex;

			IEnumerable<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO telaVO = lstAtual.ElementAt(idx);
			if (!telaVO.IsNew) {
				MedicamentoReembolsavelArqVO vo = ArquivoTela2VO(telaVO);
				MedicamentoReembolsavelBO.Instance.RemoverArquivo(vo);
			}
			this.ShowInfo("Arquivo removido com sucesso!");
			CarregarArquivos();
		}

		private void SalvarAddArquivo() {
			ArquivoTelaVO telaVO = Arquivos.Last(x => x.IsNew);

			MedicamentoReembolsavelVO vo = FillVO();

			MedicamentoReembolsavelBO.Instance.IncluirArquivoMedicamento(vo, UsuarioLogado.Id, telaVO);

			CarregarArquivos();
			this.ShowInfo("Arquivo salvo com sucesso no sistema!");
		}

		private void AddArquivo(string fisico, string original) {
			List<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO vo = new ArquivoTelaVO() {
				NomeFisico = fisico,
				NomeTela = original,
				IsNew = true,
				Parameters = new Dictionary<string, string>()
			};
			bool contains = lstAtual.FindIndex(x => x.NomeTela.Equals(original, StringComparison.InvariantCultureIgnoreCase)) >= 0;
			if (!contains) {
				lstAtual.Add(vo);
			} else {
				this.ShowError("Este arquivo já existe na listagem! Por favor, exclua o antigo ou renomeie o arquivo novo!");
				return;
			}

			ltvArquivo.DataSource = lstAtual;
			ltvArquivo.DataBind();

			if (Id != null) {
				SalvarAddArquivo();
			} else {
				this.ShowInfo("Arquivo adicionado em tela! Arquivos só serão salvos quando formulário enviado!");
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar o registro!", ex);
			}
		}


		protected void btnRemoverArquivo_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
				RemoverArquivo(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover arquivo da lista", ex);
			}
		}

		protected void btnIncluirArq_Click(object sender, EventArgs e) {
			AddArquivo(hidArqFisico.Value, hidArqOrigem.Value);
		}

		protected void btnVoltar_Click(object sender, EventArgs e) {
			Response.Redirect("./BuscaMedicamentoReembolsavel.aspx");
		}

		protected void btnLocPrincipio_Click(object sender, ImageClickEventArgs e) {
			string value = hidPrincipio.Value;
			if (NOT_FOUND_LOCATOR.Equals(value)) {
				hidPrincipio.Value = string.Empty;
			} else {
				int id = 0;
				if (!Int32.TryParse(value, out id)) {
					this.ShowError("Princípio inválido!");
					return;
				}
				BindPrincipio(id);
			}
		}

	}
}