using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;
using eVidaGeneralLib.VO.HC;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioContabilizacao: RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

				DataTable dt = LocatorDataBO.Instance.ListarCategorias().Copy();
				dt.Columns.Add("Description", typeof(string), "'(' + cd_categoria + ') ' + ds_categoria");
				chkCategoria.DataSource = dt;
				chkCategoria.DataBind();

				List<HcEmpresaVO> lstEmpresa = LocatorDataBO.Instance.ListarEmpresas();
				foreach (HcEmpresaVO empresa in lstEmpresa) {
					chkEmpresa.Items.Add( new ListItem("(" + empresa.Id + ") " + empresa.Nome, empresa.Id.ToString()) );
				}

				List<eVidaGeneralLib.VO.HC.HcPlanoVO> lst = LocatorDataBO.Instance.ListarPlanos();
				lst = (from p in lst select new eVidaGeneralLib.VO.HC.HcPlanoVO() {
					DsPlano = "(" + p.CdPlano + ") " + p.DsPlano,
					CdPlano = p.CdPlano
				}).ToList();
				chkPlano.DataSource = lst;
				chkPlano.DataBind();
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_CONTABILIZACAO_EVIDA_ELN; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nr_cnpj_cpf", 12);
			defs.SetWidth("nm_razao_social", 40);

			ExportExcel("RelatorioContabilizacaoEVida", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dtInicio;
			DateTime dtFim;
			string sistema = null;
			string situacao = null;
			List<int> lstEmpresa = new List<int>();
			List<int> lstPlano = new List<int>();
			List<int> lstCategoria = new List<int>();

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}
			if (!string.IsNullOrEmpty(dpdSistema.SelectedValue))
				sistema = dpdSistema.SelectedValue;
			if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue))
				situacao = dpdSituacao.SelectedValue;

			foreach (ListItem item in chkEmpresa.Items) {
				if (item.Selected)
					lstEmpresa.Add(Int32.Parse(item.Value));
			}
			foreach (ListItem item in chkPlano.Items) {
				if (item.Selected)
					lstPlano.Add(Int32.Parse(item.Value));
			}
			foreach (ListItem item in chkCategoria.Items) {
				if (item.Selected)
					lstCategoria.Add(Int32.Parse(item.Value));
			}

			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarContabilizacao(sistema,situacao,
					lstEmpresa, lstPlano, lstCategoria,
					dtInicio, dtFim);

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, "cd_funcionario");
			}
			catch (Exception ex) {
				Log.Error("Erro ao consultar o relatorio", ex);
				this.ShowError("Erro ao consultar o relatorio: " + ex.Message);
			}
		}
	}
}