using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.HC;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioCustoInternacao : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<HcPlanoVO> lst = LocatorDataBO.Instance.ListarPlanos();
				lst = (from p in lst
					   select new HcPlanoVO()
					   {
						   DsPlano = "(" + p.CdPlano + ") " + p.DsPlano,
						   CdPlano = p.CdPlano
					   }).ToList();
				chkPlano.DataSource = lst;
				chkPlano.DataBind();
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_CUSTO_INTERNACAO; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_razao_social", 40);
			defs.SetWidth("nm_beneficiario", 40);
			ExportExcel("RelatorioCustoTotalInternacao", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dtRef = DateTime.MinValue;
			string cartao = null;
			string nomeBenef = null;
			int nroAutorizacao = -1;
			List<int> lstPlano = new List<int>();

			if (!string.IsNullOrEmpty(txtAnoMesRef.Text)) {
				if (!DateTime.TryParse(txtAnoMesRef.Text, out dtRef)) {
					this.ShowError("A data informada é inválida!");
					return;
				}
			}
			if (!string.IsNullOrEmpty(txtNumAutorizacao.Text)) {
				if (!Int32.TryParse(txtNumAutorizacao.Text, out nroAutorizacao)) {
					this.ShowError("A autorização deve ser numérica!");
					return;
				}
			}
			foreach (ListItem item in chkPlano.Items) {
				if (item.Selected)
					lstPlano.Add(Int32.Parse(item.Value));
			}

			cartao = txtNumCartao.Text;
			nomeBenef = txtNomeBenef.Text.ToUpper();

			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarCustoInternacao(
					dtRef != DateTime.MinValue ? dtRef : new DateTime?(), cartao, nomeBenef,
					nroAutorizacao != -1 ? nroAutorizacao : new int?(), lstPlano);

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
				pnlGrid.Update();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}

		protected void btnBuscarBeneficiario_Click(object sender, ImageClickEventArgs e) {
			try {
				if (!string.IsNullOrEmpty(hidCodBenef.Value)) {
					int codBenef = Int32.Parse(hidCodBenef.Value);

					HcBeneficiarioVO benef = BeneficiarioBO.Instance.GetBeneficiario(codBenef);
					txtNumCartao.Text = benef.CdAlternativo;
					txtNomeBenef.Text = benef.NmBeneficiario;
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar cartao!", ex);
			}
		}
	}
}