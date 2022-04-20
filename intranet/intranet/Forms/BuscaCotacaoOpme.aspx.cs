using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Forms {
	public partial class BuscaCotacaoOpme : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdFiltroSituacao.Items.Add(new ListItem("TODOS", ""));
					foreach (StatusCotacaoOpme i in Enum.GetValues(typeof(StatusCotacaoOpme))) {
						dpdFiltroSituacao.Items.Add(new ListItem(CotacaoOpmeTradutorHelper.TraduzStatus(i), i.ToString()));
					}
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO; }
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {

		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {

		}

		protected void btnBuscar_Click(object sender, EventArgs e) {

		}
	}
}