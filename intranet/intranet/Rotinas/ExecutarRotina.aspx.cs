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

namespace eVidaIntranet.Rotinas {
	public partial class ExecutarRotina : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<RotinaVO> lstRotina = RotinaBO.Instance.ListarRotinas();
				if (lstRotina != null)
					lstRotina = lstRotina.FindAll(x => this.HasPermission(x.Modulo));
				dpdRotina.DataSource = lstRotina;
				dpdRotina.DataBind();
				dpdRotina.Items.Insert(0, new ListItem("SELECIONE", ""));
				if (lstRotina == null || lstRotina.Count == 0) {
					this.ShowError("Você não possui acesso a nenhuma rotina de execução!");
				}
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.EXECUTAR_ROTINAS; }
		}

		private void SelecionarRotina(int idRotina) {
			RotinaVO vo = RotinaBO.Instance.GetById(idRotina);

			lblNome.Text = vo.Nome;
			lblDescricao.Text = vo.Descricao;

			BindHistorico(idRotina);
		}

		private void BindHistorico(int idRotina) {

			DataTable dt = RotinaBO.Instance.PesquisarHistorico(idRotina);
			this.ShowGrid(gdvRelatorio, dt, null, "DT_CRIACAO DESC");
			btnExecutar.Visible = true;
			btnAtualizar.Visible = true;
			gdvRelatorio.Visible = true;
			dvHistorico.Visible = true;
		}

		private void Executar(int idRotina) {
			if (HasPendente(idRotina)) {
				this.ShowError("A rotina já possui uma solicitação pendente!");
			} else {
				RotinaBO.Instance.SolicitarExecucao(idRotina, UsuarioLogado.Id);
				this.ShowInfo("Solicitação enviada com sucesso!");
			}
			BindHistorico(idRotina);
		}

		private bool HasPendente(int idRotina) {
			return RotinaBO.Instance.HasPendente(idRotina);
		}

		protected void btnExecutar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(dpdRotina.SelectedValue)) {
				this.ShowError("Selecione uma rotina para execução");
				return;
			}

			Executar(Convert.ToInt32(dpdRotina.SelectedValue));
		}

		protected void dpdRotina_SelectedIndexChanged(object sender, EventArgs e) {
			btnExecutar.Visible = false;
			gdvRelatorio.Visible = false;
			btnAtualizar.Visible = false;
			dvHistorico.Visible = false;

			lblNome.Text = string.Empty;
			lblDescricao.Text = string.Empty;
			if (string.IsNullOrEmpty(dpdRotina.SelectedValue)) return;

			int idRotina = Convert.ToInt32(dpdRotina.SelectedValue);
			SelecionarRotina(idRotina);
		}

		protected void btnAtualizar_Click(object sender, EventArgs e) {

			if (string.IsNullOrEmpty(dpdRotina.SelectedValue)) {
				this.ShowError("Selecione uma rotina");
				return;
			}

			int idRotina = Convert.ToInt32(dpdRotina.SelectedValue);
			BindHistorico(idRotina);
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				TableCell cellStatus = row.Cells[4];

				string erroSQL = Convert.ToString(((DataRowView)row.DataItem)["ds_erro_sql"]);
				if (!string.IsNullOrEmpty(erroSQL)) {
					cellStatus.Text += "<BR>" + erroSQL;
				}
			}
		}
	}
}