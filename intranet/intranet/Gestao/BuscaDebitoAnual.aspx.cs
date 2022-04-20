using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Gestao {
	public partial class BuscaDebitoAnual : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int anoAtual = DateTime.Now.Year;
				for (int i = anoAtual - 5; i < anoAtual; ++i) {
					dpdAno.Items.Add(new ListItem(i.ToString(), i.ToString()));
				}
				dpdAno.Items.Insert(0, new ListItem("SELECIONE", ""));

				List<HcPlanoVO> lst = LocatorDataBO.Instance.ListarPlanos();
				//int idx = lst.FindIndex(x => x.CdPlano.Equals(Constantes.PLANO_MAIS_VIDA_CEA.ToString()));
				//if (idx >= 0)
				//	lst.RemoveAt(idx);

				dpdPlano.DataSource = lst;
				dpdPlano.DataBind();
				dpdPlano.Items.Insert(0, new ListItem("SELECIONE", ""));

				List<HcEmpresaVO> lstEmpresa = LocatorDataBO.Instance.ListarEmpresas();
				dpdEmpresa.DataSource = lstEmpresa;
				dpdEmpresa.DataBind();
				dpdEmpresa.Items.Insert(0, new ListItem("SELECIONE", ""));
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_DEBITO_ANUAL; }
		}

		private void ClearResults() {
			lblCount.Visible = false;
			gdvRelatorio.Visible = false;
			pnlGrid.Update();
		}

		private void Buscar() {
			ClearResults();
			if (string.IsNullOrEmpty(dpdAno.SelectedValue)) {
				this.ShowError("Informe o ano para consulta!");
				return;
			}
			int ano = Int32.Parse(dpdAno.SelectedValue);
			long? matricula = null;
			int? status = null;
			int? empresa = null;
			bool? apenasQuitados = null;

			if (!string.IsNullOrEmpty(txtMatricula.Text)) {
				long l;
				if (!Int64.TryParse(txtMatricula.Text, out l)) {
					this.ShowError("A matrícula deve ser numérica!");
					return;
				}
				matricula = l;
			}
			if (!string.IsNullOrEmpty(dpdEmpresa.SelectedValue)) {
				empresa = Int32.Parse(dpdEmpresa.SelectedValue);
			}

			if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
				status = Int32.Parse(dpdSituacao.SelectedValue);
			}

			if (!string.IsNullOrEmpty(dpdPendencia.SelectedValue)) {
				apenasQuitados = "S".Equals(dpdPendencia.SelectedValue);
			}

			DataTable dt = DeclaracaoAnualDebitoBO.Instance.Pesquisar(ano, dpdPlano.SelectedValue, empresa, matricula, status, apenasQuitados);
			
			lblCount.Text = dt.Rows.Count + " registros encontrados.";
			lblCount.Visible = true;
			this.ShowPagingGrid(gdvRelatorio, dt, null);

			gdvRelatorio.Visible = true;
			pnlGrid.Update();
		}

		private void RefreshBusca() {
			Buscar();
		}

		private void Enviar(int cdBeneficiario, int ano) {
			DeclaracaoAnualDebitoVO vo = new DeclaracaoAnualDebitoVO();
			vo.AnoRef = ano;
			vo.CodBeneficiario = cdBeneficiario;

			ReportDeclaracaoAnualDebito rpt = new ReportDeclaracaoAnualDebito(ReportDir, UsuarioLogado);
			byte[] bytes = rpt.GerarNovoRelatorio(cdBeneficiario, ano);

			DeclaracaoAnualDebitoBO.Instance.Enviar(vo, UsuarioLogado.Id, bytes);
			this.ShowInfo("Declaracao enviada com sucesso!");
			RefreshBusca();
		}

		private void EnviarSelecao() {
			List<int> lst = new List<int>();
			int ano = 0;
			foreach (GridViewRow row in gdvRelatorio.Rows) {
				if (row.RowType == DataControlRowType.DataRow) {
					CheckBox chkSel = (CheckBox)row.FindControl("chkSelecionar");
					if (chkSel.Enabled && chkSel.Checked) {
						DeclaracaoAnualDebitoVO vo = new DeclaracaoAnualDebitoVO();
						DataKey key = gdvRelatorio.DataKeys[row.RowIndex];
						int cdBeneficiario = Convert.ToInt32(key["CD_BENEFICIARIO"]);
						ano = Convert.ToInt32(key["ANO"]);
						lst.Add(cdBeneficiario);
					}
				}
			}
			if (lst.Count == 0) {
				this.ShowError("Por favor, selecione pelo menos um item na página para envio em lote!");
				return;
			}
			DeclaracaoAnualDebitoBO.Instance.Enviar(lst, ano, UsuarioLogado.Id);
			this.ShowInfo("Solicitação em lote enviada com sucesso! - " + lst.Count + " - registros");
			RefreshBusca();
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar dados!", ex);
			}
		}

		protected void chkHeader_CheckedChanged(object sender, EventArgs e) {
			CheckBox chkHeader = (CheckBox)sender;
			foreach (GridViewRow row in gdvRelatorio.Rows) {
				CheckBox chkSel = (CheckBox)row.FindControl("chkSelecionar");
				if (chkSel.Enabled)
					chkSel.Checked = chkHeader.Checked;
			}
		}

		protected void btnEnviarSelecao_Click(object sender, EventArgs e) {
			try {
				EnviarSelecao();
			} catch (Exception ex) {
				this.ShowError("Erro ao enviar seleção!", ex);
			}
		}

		protected void btnEnviar_Click(object sender, EventArgs e) {
			Button btn = (Button)sender;
			try {
				GridViewRow row = (GridViewRow)btn.NamingContainer;
				DataKey key = gdvRelatorio.DataKeys[row.RowIndex];
				int cdBeneficiario = Convert.ToInt32(key["CD_BENEFICIARIO"]);
				int ano = Convert.ToInt32(key["ANO"]);
				Enviar(cdBeneficiario, ano);
			} catch (Exception ex) {
				this.ShowError("Erro ao enviar declaração!", ex);
			}
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				Button btnEnviar = (Button)row.FindControl("btnEnviar");
				CheckBox chkSelecionar = (CheckBox)row.FindControl("chkSelecionar");
				DataRowView drv = (DataRowView)row.DataItem;

				TableCell cellStatus = row.Cells[4];

				int qtdPendente = Convert.ToInt32(drv["QTD_PENDENTE"]);
				int ano = Convert.ToInt32(drv["ANO"]);
				if (qtdPendente > 0) {
					btnEnviar.Enabled = false;
					btnEnviar.ToolTip = "O beneficiário possui débitos pendentes em " + ano;
					chkSelecionar.Enabled = false;
				} else {
					btnEnviar.Enabled = true;
					chkSelecionar.Enabled = true;
				}

				if (drv["CD_STATUS"] != DBNull.Value)
					cellStatus.Text = DeclaracaoAnualDebitoEnumTradutor.TraduzStatus((StatusDeclaracaoAnualDebito)Convert.ToInt32(drv["CD_STATUS"]));
				else
					cellStatus.Text = "NÃO SOLICITADO";
			}
		}

	}
}