using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Forms {
	public partial class BuscaIndisponibilidadeRede : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
                    DataTable dt = IndisponibilidadeRedeBO.Instance.BuscarIndisponibilidadeRede(UsuarioLogado.Codint, UsuarioLogado.Codemp, UsuarioLogado.Matric);
					this.ShowGrid(gdvRelatorio, dt, null, "cd_indisponibilidade DESC");
					lblCount.Text = "Você já realizou " + dt.Rows.Count + " solicitações!";
				} catch (Exception ex) {
					this.ShowError("Erro ao carregar tela. ", ex);
				}
			}
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				row.Cells[4].Text = IndisponibilidadeRedeEnumTradutor.TraduzPrioridade((PrioridadeIndisponibilidadeRede)Convert.ToInt32(vo["NR_PRIORIDADE"]));
				row.Cells[6].Text = IndisponibilidadeRedeEnumTradutor.TraduzStatus((StatusIndisponibilidadeRede)Convert.ToInt32(vo["id_situacao"]));

			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("~/Forms/IndisponibilidadeRede.aspx");
		}
	}
}