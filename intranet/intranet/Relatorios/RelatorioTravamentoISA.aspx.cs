using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioTravamentoISA : RelatorioExcelPageBase {

        static bool PODE_ENCERRAR = false;

		protected override void PageLoad(object sender, EventArgs e) {

            if (!IsPostBack)
            {
                // Verifica se o usuário logado é um administrador
                List<Perfil> lista_perfil = UsuarioBO.Instance.GetPerfilByUsuario(UsuarioLogado.Id);
                if (lista_perfil.Contains(Perfil.ADMINISTRADOR))
                {
                    PODE_ENCERRAR = true;
                }
            }
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_TRAVAMENTO_ISA; }
		}

		private void Buscar() {
			DataTable dtAcessos = RelatorioBO.Instance.BuscarTravamentoISA();

			//btnExportar.Visible = dtAcessos.Rows.Count > 0;
			lblCount.Visible = true;
			if (dtAcessos.Rows.Count > 0)
				lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " travamentos.";
			else
				lblCount.Text = "Sem travamentos no momento.";

			this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
		}

		private void Kill(string argument) {
			SistemaBO.Instance.KillDbSession(argument);
			this.ShowInfo("Sessão finalizada com sucesso!");
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {

			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nm_titular", 40);
			defs.SetWidth("nm_beneficiario", 40);

			ExportExcel("TravamentoISA", defs, sourceTable);
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (this.IsPagingCommand(sender, e)) return;
				if ("CmdKill".Equals(e.CommandName)) {
					Kill(Convert.ToString(e.CommandArgument));
				}
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar dados", ex);
			}
		}

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                Button btnKill = (Button)row.FindControl("btnKill");
                btnKill.Visible = PODE_ENCERRAR;
            }
        }
	}
}