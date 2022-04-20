using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioMensalidade : RelatorioExcelPageBase {
		const string PLANO_FAMILIA = "MENSALIDADE 22";
		const string PLANO_MELHOR_IDADE = "MENSALIDADE 21";
		const string PLANO_PPRS = "MENSALIDADE 20";
		
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
				foreach (string name in monthNames) {
					if (!string.IsNullOrEmpty(name))
						dpdMes.Items.Add(new ListItem(name.ToUpper(), (dpdMes.Items.Count + 1).ToString()));
				}
				dpdMes.SelectedValue = DateTime.Now.Month.ToString();

				for (int i = DateTime.Now.Year - 4; i <= DateTime.Now.Year + 4; ++i) {
					dpdAno.Items.Add(new ListItem(i + "", i + ""));
				}
				dpdAno.SelectedValue = DateTime.Now.Year.ToString();

				DataTable dtCategorias = LocatorDataBO.Instance.ListarCategorias();
				DataView categorias = (from row in dtCategorias.AsEnumerable().OrderBy(x => x["ds_categoria"])				
						select row).AsDataView();
				chkListCat.DataSource = categorias;
				chkListCat.DataValueField = "cd_categoria";
				chkListCat.DataTextField = dtCategorias.Columns[1].ColumnName;
				chkListCat.RepeatColumns = 3;
				chkListCat.DataBind();

				chkListPlano.DataSource = LocatorDataBO.Instance.ListarPlanos().Select(x => new {
					Codigo = "MENSALIDADE " + x.CdPlano,
					Descricao = x.DsPlano + " (" + x.CdPlano + ")"
				});
				chkListPlano.DataBind();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_MENSALIDADE; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("dt_ano_mes_ref", 8);
			defs.SetWidth("cd_grupo_lancto", 20);
			defs.SetWidth("cd_categoria", 25);
			defs.SetWidth("tp_beneficiario", 8);
			defs.SetWidth("nm_beneficiario", 40);
            defs.SetWidth("cd_alternativo", 15);

			ExportExcel("RelatorioMensalidade", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			DateTime dataReferencia;
			List<string> lstPlanos = new List<string>();
			List<int> lstCategoria = new List<int>();

			if (!DateTime.TryParse("01/" + dpdMes.SelectedValue + "/" + dpdAno.SelectedValue, out dataReferencia)) {
				this.ShowError("Informe uma data de referência correta!");
				return;
			}
			foreach (ListItem item in chkListPlano.Items) {
				if (item.Selected)
					lstPlanos.Add(item.Value);
			}
			foreach (ListItem item in chkListCat.Items) {
				if (item.Selected)
					lstCategoria.Add(Int32.Parse(item.Value));
			}
			if (lstPlanos.Count == 0 || lstCategoria.Count == 0) {
				this.ShowError("Por favor, selecione pelo menos um plano e uma categoria!");
				return;
			}
			try {
				DataTable dtAcessos = RelatorioBO.Instance.BuscarMensalidade(dataReferencia, lstPlanos, lstCategoria);
				Translate(dtAcessos);
				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				
				this.ShowPagingGrid(gdvRelatorio, dtAcessos, "cd_funcionario");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio: ", ex);
			}
		}

		private void Translate(DataTable dtAcessos) {
			foreach (DataRow dr in dtAcessos.Rows) {
				string cdGrupo = Convert.ToString(dr["cd_grupo_lancto"]);
				dr["cd_grupo_lancto"] = FindPlanName(cdGrupo);
				int categoria = Convert.ToInt32(dr["cd_categoria"]);
				dr["cd_categoria"] = FindCategoryName(categoria);
			}
		}

		private string FindPlanName(string codigo) {
			foreach (ListItem item in chkListPlano.Items) {
				if (item.Value.Equals(codigo))
					return item.Text;
			}
			return " - not found -";
		}

		private string FindCategoryName(int codigo) {
			foreach (ListItem item in chkListCat.Items) {
				if (Int32.Parse(item.Value) == codigo)
					return item.Text;
			}
			return " - not found -";
		}
	}
}