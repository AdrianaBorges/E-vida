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

namespace eVidaIntranet.FormsPop {
	public partial class PopAutorizacaoHistorico : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int cdProtocolo;
				if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
					this.ShowError("A requisição está inválida!");

					//this.btnSalvar.Visible = false;
					return;
				}
				Bind(cdProtocolo);
			}
		}

		protected override Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO; }
		}

		public int Id {
			get { return (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int cdProtocolo) {
			Id = cdProtocolo;

			//btnSalvar.Visible = false;
			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(cdProtocolo);
			
			litProtocolo.Text = vo.Id.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);

			DataTable dt = AutorizacaoBO.Instance.ListarHistorico(cdProtocolo);
			gdvHistorico.DataSource = dt;
			gdvHistorico.DataBind();
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				TableCell cellStatus = row.Cells[1];
				StatusAutorizacao status = (StatusAutorizacao)Convert.ToInt32(vo["ST_AUTORIZACAO"]);
				cellStatus.Text = AutorizacaoTradutorHelper.TraduzStatus(status);
			}
		}

	}
}