using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.GenPops {
	public partial class PopPrincipioAtivo : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

				if (!HasPermission(eVidaGeneralLib.VO.Modulo.GESTAO_PRINCIPIO_ATIVO_EDICAO)) {
					gdv.Columns[3].Visible = false;
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}

		private void Salvar() {
			if (!HasPermission(eVidaGeneralLib.VO.Modulo.GESTAO_PRINCIPIO_ATIVO_EDICAO)) {
				this.ShowError("Permissão negada!");
				return;
			}
			if (string.IsNullOrEmpty(txtDescricaoCadastro.Text)) {
				this.ShowError("Informe o nome do princípio ativo!");
				return;
			}

			PrincipioAtivoVO vo = new PrincipioAtivoVO();
			vo.Descricao = txtDescricaoCadastro.Text;
			MedicamentoReembolsavelBO.Instance.CriarPrincipio(vo, UsuarioLogado.Id);

			this.ShowInfo("Princípio Ativo salvo com sucesso!");

			txtCodigo.Text = vo.Id.ToString();
			txtDescricao.Text = string.Empty;
			Buscar();
		}

		private void Remover(int id) {
			try {
				MedicamentoReembolsavelBO.Instance.RemoverPrincipioAtivo(id);
				this.ShowInfo("Princípio ativo removido com sucesso!");
			} catch (Exception ex) {
				this.ShowError("Erro ao remover princípio", ex);
			}
			Buscar();
		}

		private void Buscar() {
			ClearForm();
			tbCadastro.Visible = false;
			int? codigo = null;
			string descricao = null;

			if (string.IsNullOrEmpty(txtCodigo.Text) && string.IsNullOrEmpty(txtDescricao.Text)) {
				this.ShowError("Por favor, informe pelo menos um campo de filtro!");
				return;
			}
			if (!string.IsNullOrEmpty(txtDescricao.Text) && txtDescricao.Text.Length < 3) {
				this.ShowError("Por favor, para a descrição informe pelo menos 3 caracteres!");
				return;
			}

			descricao = txtDescricao.Text;
			if (!string.IsNullOrEmpty(txtCodigo.Text)) {
				int id = 0;
				if (!Int32.TryParse(txtCodigo.Text, out id)) {
					this.ShowError("O código deve ser numérico!");
					return;
				}
				codigo = id;
			}

			DataTable dt = MedicamentoReembolsavelBO.Instance.PesquisarPrincipio(codigo, descricao);
			if (dt.Rows.Count > 300) {
				dt = dt.AsEnumerable().Take(300).CopyToDataTable();
				this.ShowInfo("Foram retornados apenas os 300 primeiros resultados da pesquisa. Por favor informe mais detalhes!");
			}
			gdv.DataSource = dt;
			gdv.DataBind();

			if (dt.Rows.Count == 0) {
				this.ShowInfo("Não foram encontrados registros com este filtro!");				
				tbCadastro.Visible = HasPermission(eVidaGeneralLib.VO.Modulo.GESTAO_PRINCIPIO_ATIVO_EDICAO);
			}
		}

		private void ClearForm() {
			txtDescricaoCadastro.Text = string.Empty;
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar", ex);
			}
		}

		protected void gdv_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;
				LinkButton btnExcluir = row.FindControl("btnExcluir") as LinkButton;

				int qtd = Convert.ToInt32(vo["qtd"]);

				btnExcluir.Visible = HasPermission(Modulo.GESTAO_PRINCIPIO_ATIVO_EDICAO) && qtd == 0;
			}
		}

		protected void gdv_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName == "CmdSelecionar") {
				string id = GetRowId(e);
				this.RegisterScript("SERVICO", "setLocatorValue('" + id + "');");
			} else if ("CmdExcluir".Equals(e.CommandName)) {
				string id = GetRowId(e);

				Remover(Int32.Parse(id));
			}
		}

		private string GetRowId(GridViewCommandEventArgs e) {
			// Retrieve the row index stored in the CommandArgument property.
			int index = Convert.ToInt32(e.CommandArgument);

			// Retrieve the row that contains the button 
			// from the Rows collection.
			GridViewRow row = gdv.Rows[index];

			string id = Convert.ToString(gdv.DataKeys[index]["cd_principio_ativo"]);
			return id;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar!", ex);
			}
		}

	}
}