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
	public partial class RelatorioServicoPrestador : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				IEnumerable<object> lstLocal = PLocatorDataBO.Instance.ListarRegionais().Select(x => new
				{
					Codigo = x.Key,
					Descricao = "(" + x.Key + ") " +x.Value
				});
				chkRegional.DataSource = lstLocal;
				chkRegional.DataBind();
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_SERVICO_PRESTADOR; }
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = GetRelatorioTable();
			if (sourceTable == null) {
				this.ShowError("Os dados da pesquisa expiraram! Realize a busca novamente!");
				return;
			}

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_razao_social", 40);
			defs.SetWidth("nm_beneficiario", 40);
			ExportExcel("RelatorioServicoPrestador", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			Log.Debug("Buscar relatorio");
			List<string> lstMascara = new List<string>();
			DateTime? dtInicial = null;
			DateTime? dtFinal = null;
			List<int> lstRegional = new List<int>();
			string tipo = "S";
			try {
				if (!string.IsNullOrEmpty(txtInicio.Text) || !string.IsNullOrEmpty(txtFim.Text)) {
					if (string.IsNullOrEmpty(txtInicio.Text) || string.IsNullOrEmpty(txtFim.Text)) {
						this.ShowError("Para utilização de período, informar data início e fim!");
						return;
					}
					DateTime dt1, dt2;
					if (!DateTime.TryParse(txtInicio.Text, out dt1)) {
						this.ShowError("Data inicial inválida!");
						return;
					}
					if (!DateTime.TryParse(txtFim.Text, out dt2)) {
						this.ShowError("Data final inválida!");
						return;
					}
					if (dt1 > dt2) {
						this.ShowError("Data inicial deve ser menor ou igual data final!");
						return;
					}
					dtInicial = dt1;
					dtFinal = dt2;
				}

				DataTable dt = GetTableServico();
				if (dt != null) {
					foreach (DataRow dr in dt.Rows) {
						lstMascara.Add((string)dr["cd_mascara"]);
					}
				}
				if (lstMascara.Count == 0) {
					this.ShowError("Informe pelo menos um serviço!");
					return;
				}

				foreach (ListItem item in chkRegional.Items) {
					if (item.Selected)
						lstRegional.Add(Int32.Parse(item.Value));
				}

				tipo = rblTipoRelatorio.SelectedValue;

				DataTable dtAcessos = RelatorioBO.Instance.BuscarServicoPrestador(tipo, dtInicial, dtFinal, lstMascara, lstRegional);

				btnExportar.Visible = dtAcessos.Rows.Count > 0;
				lblCount.Visible = true;
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

				bool sintetico = tipo.Equals("S");
				gdvRelatorio.Columns[6].Visible = sintetico;
				gdvRelatorio.Columns[7].Visible = sintetico;
				for (int i = 8; i < gdvRelatorio.Columns.Count; i++) {
					gdvRelatorio.Columns[i].Visible = !sintetico;
				}

				this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
				pnlGrid.Update();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}

		protected void gdvServicos_RowCommand(object sender, GridViewCommandEventArgs e) {
			if (e.CommandName.Equals("RemoverServico")) {
				try {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);
					RemoverServico(index);
				}
				catch (Exception ex) {
					this.ShowError("Erro ao remover filtro!", ex);
				}

			}
		}

		protected void btnLimparServico_Click(object sender, EventArgs e) {
			try {
				LimparServicos();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao limpar seleção!", ex);
			}
		}

		protected void btnAdicionarServico_Click(object sender, EventArgs e) {
			try {
				AdicionarServico(hidCodServico.Value);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao adicionar o serviço!", ex);
			}
		}

		#region serviços

		private DataTable GetTableServico() {

			DataTable dt = ViewState["GDV_SERVICO"] as DataTable;
			if (dt == null) {
				dt = new DataTable();
                dt.Columns.Add(new DataColumn("cd_mascara", typeof(string)));
				dt.Columns.Add(new DataColumn("nm_servico", typeof(string)));
				dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };

				ViewState["GDV_SERVICO"] = dt;
			}
			return dt;
		}

		private void BindGridServico() {
			DataTable dt = GetTableServico();
			gdvServicos.DataSource = dt;
			gdvServicos.DataBind();
			if (dt != null && dt.Rows.Count > 0)
				btnLimparServico.Visible = true;
			else
				btnLimparServico.Visible = false;
		}

		private void LimparServicos() {
			DataTable dt = GetTableServico();

			dt.Rows.Clear();

			BindGridServico();
		}

		private void AdicionarServico(string cdMascara) {
			DataTable dt = GetTableServico();

			if (dt.Rows.Find(cdMascara) != null) {
				this.ShowError("O serviço já está no filtro");
			} else {
				DataTable dtSrv = PLocatorDataBO.Instance.BuscarServicos(cdMascara, null, false);
				if (dtSrv == null || dtSrv.Rows.Count == 0) {
					this.ShowError("Serviço [" + cdMascara + "] inexistente!");
				} else {					
					DataRow dr = dt.NewRow();
					DataRow drv = dtSrv.Rows[0];
					dr["cd_mascara"] = drv.Field<string>("cd_mascara");
					dr["nm_servico"] = drv.Field<string>("ds_servico");
					dt.Rows.Add(dr);					
				}
			}
			BindGridServico();
		}

		private void RemoverServico(int index) {
			DataTable dt = GetTableServico();
			dt.Rows.RemoveAt(index);
			BindGridServico();
		}

		#endregion
	}
}