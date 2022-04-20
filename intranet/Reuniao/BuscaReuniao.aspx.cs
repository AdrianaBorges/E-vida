using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Reuniao {
	public partial class BuscaReuniao : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					List<ConselhoVO> lstConselho = ConselhoBO.Instance.ListarConselhos();
					dpdConselho.DataSource = lstConselho;
					dpdConselho.DataBind();
					dpdConselho.Items.Insert(0, new ListItem("TODOS", ""));

					bool canChange = HasPermission(Modulo.GERENCIAR_REUNIAO);
					btnNovo.Visible = canChange;
					/*dpdConselho.Enabled = canChange;
					if (!canChange) {
						ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByUsuario(UsuarioLogado.Id);
						if (conselho != null) {
							dpdConselho.SelectedValue = conselho.Codigo;
						} else {
							this.ShowError("Você deve pertencer a um conselho ou ter permissão para gerenciar reunião!");
							btnBuscar.Visible = false;
						}
					}*/
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.VISUALIZAR_REUNIAO; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				LinkButton btnExcluir = (LinkButton)row.FindControl("btnExcluir");
				ImageButton btnEmail = (ImageButton)row.FindControl("btnEmail");

				bool canChange = HasPermission(Modulo.GERENCIAR_REUNIAO);
				btnEmail.Visible = btnExcluir.Visible = canChange;

			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar os dados!", ex);
			}
		}

		private void Buscar() {
			string cdConselho = dpdConselho.SelectedValue;
			string titulo = txtTitulo.Text;
			string descricao = txtDescricao.Text;
			DateTime? inicio = null;
			DateTime? fim = null;

			if (!string.IsNullOrEmpty(txtInicio.Text)) {
				DateTime d;
				if (!DateTime.TryParse(txtInicio.Text, out d)) {
					this.ShowError("A data de início está inválida!");
					return;
				}
				inicio = d;
			}
			if (!string.IsNullOrEmpty(txtFim.Text)) {
				DateTime d;
				if (!DateTime.TryParse(txtFim.Text, out d)) {
					this.ShowError("A data de fim está inválida!");
					return;
				}
				fim = d;
			}

			if (inicio != null || fim != null) {
				if (inicio == null || fim == null) {
					this.ShowError("Para seleção de período informe data de início e fim!");
					return;
				} else if (inicio > fim) {
					this.ShowError("A data de início deve ser menor ou igual a data fim!");
					return;
				}
			}

			DataTable dt = ReuniaoBO.Instance.Pesquisar(cdConselho, titulo, descricao, inicio, fim);

			this.ShowPagingGrid(gdvRelatorio, dt, "DT_REUNIAO DESC");			

			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
			//btnExportar.Visible = dt.Rows.Count > 0;
		}

		private void Excluir(GridViewRow row, int index) {
			int cdReuniao = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_REUNIAO"]);
			ReuniaoVO vo = new ReuniaoVO();
			vo.Id = cdReuniao;
			List<ArquivoReuniaoVO> lstArquivos = ReuniaoBO.Instance.ListarArquivosByReuniao(cdReuniao);
			if (lstArquivos != null && lstArquivos.Count > 0) {
				this.ShowError("A reunião possui arquivos associados, não pode ser excluída!");
				return;
			}
			ReuniaoBO.Instance.Excluir(vo);
			this.ShowInfo("Reunião excluída com sucesso!");
			Buscar();
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			ExportExcel("Reunioes", defs, sourceTable);
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar para excel!", ex);
			}
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (this.IsPagingCommand(sender, e)) return;

				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);

					// Retrieve the row that contains the button 
					// from the Rows collection.
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdExcluir") {
						Excluir(row, index);
					} else {
						this.ShowError("Comando não reconhecido: " + e.CommandName);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}

	}
}