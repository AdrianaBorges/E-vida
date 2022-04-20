using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Reuniao {
	public partial class ArquivosConselho : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<ConselhoVO> lstConselho = ConselhoBO.Instance.ListarConselhos();
				dpdConselho.DataSource = lstConselho;
				dpdConselho.DataBind();
				dpdConselho.Items.Insert(0, new ListItem("TODOS", ""));

				/*bool canChange = HasPermission(Modulo.GERENCIAR_REUNIAO);
				dpdConselho.Enabled = canChange;

				if (!HasPermission(Modulo.GERENCIAR_REUNIAO)) {
					ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByUsuario(UsuarioLogado.Id);
					if (conselho == null) {
						this.ShowError("Você deve pertencer a um conselho ou ter permissão para gerenciar reunião para visualizar os arquivos!");
						return;
					} else {
						dpdConselho.SelectedValue = conselho.Codigo;
					}
				} else {
					dpdConselho.SelectedValue = string.Empty;
				}*/
				Bind();

			}
		}

		protected override Modulo Modulo {
			get { return Modulo.VISUALIZAR_REUNIAO; }
		}

		protected string CurrentConselho {
			get { return (string)ViewState["CONS"]; }
			set { ViewState["CONS"] = value; }
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

		private void Bind() {
			BindArquivos();
		}

		private void BindArquivos() {
			List<ArquivoConselhoVO> lstArquivos = null;

			CurrentConselho = dpdConselho.SelectedValue;

			if (!string.IsNullOrEmpty(CurrentConselho)) {
				lstArquivos = ConselhoBO.Instance.ListarArquivosConselho(CurrentConselho);
			} else {
				lstArquivos = ConselhoBO.Instance.ListarArquivosTodosConselhos();
			}
			if (lstArquivos == null)
				lstArquivos = new List<ArquivoConselhoVO>();

			IEnumerable<ArquivoConselhoVO> lstArquivosFiltrado = lstArquivos.Where(x => Filtrar(x));

			List<ArquivoTelaVO> lstArqs = lstArquivosFiltrado.Select(x =>
				CreateArqTelaVO(x)).ToList();			

			Arquivos = lstArqs;

			ltvArquivo.EditIndex = -1;
			ltvArquivo.DataSource = lstArqs;
			ltvArquivo.DataBind();
			if (ltvArquivo.Items.Count > 0)
				ltvArquivo.FindControl("colConselho").Visible = string.IsNullOrEmpty(CurrentConselho);

			updArquivos.Update();
		}

		private bool Filtrar(ArquivoConselhoVO x) {
			string nome = txtTitulo.Text;
			string descricao = txtDescricao.Text;

			if (!string.IsNullOrEmpty(nome)) {
				if (x.NomeArquivo.IndexOf(nome, StringComparison.InvariantCultureIgnoreCase) < 0)
					return false;
			}
			if (!string.IsNullOrEmpty(descricao)) {
				if (x.Descricao.IndexOf(descricao, StringComparison.InvariantCultureIgnoreCase) < 0)
					return false;
			}
			return true;
		}

		private ArquivoTelaVO CreateArqTelaVO(ArquivoConselhoVO x) {
			ArquivoTelaVO vo = new ArquivoTelaVO() {
				Id = x.IdArquivo.ToString(),
				Descricao = x.Descricao,
				NomeTela = x.NomeArquivo,
				IsNew = false,
				Parameters = new Dictionary<string, string>()
			};
			vo.Parameters.Add("CONSELHO", x.CodConselho);
			vo.Parameters.Add("NOME_CONSELHO", TranslateConselho(x.CodConselho));
			return vo;
		}

		protected void btnAtualizar_Click(object sender, EventArgs e) {
			Bind();
		}

		private string TranslateConselho(string cod) {
			ListItem item = dpdConselho.Items.FindByValue(cod);
			if (item != null) {
				return cod + " - " + item.Text;
			}
			return cod;
		}
	}
}