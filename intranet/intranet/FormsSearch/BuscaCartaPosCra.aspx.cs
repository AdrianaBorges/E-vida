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

namespace eVidaIntranet.FormsSearch {
	public partial class BuscaCartaPosCra : RelatorioExcelPageBase {
		
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {				
				PopulateByEnum<CartaPositivaCraStatus>(dpdSituacao.Items, CartaPositivaCraEnumTradutor.TraduzStatus);
				dpdSituacao.Items.Insert(0, new ListItem("TODAS", ""));
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.CARTA_POSITIVA_CRA; }
		}

		private void Buscar() {
			long? matricula = null;
			int? cdProtocolo = null;
			string protocoloCra = null;
			string carteira = null;
			CartaPositivaCraStatus? status = null;
			int iValue;
			long lValue;

			if (!string.IsNullOrEmpty(txtMatricula.Text)) {
				if (Int64.TryParse(txtMatricula.Text, out lValue)) {
					matricula = lValue;
				}
			}

			if (!string.IsNullOrEmpty(txtId.Text))
				if (Int32.TryParse(txtId.Text, out iValue)) {
					cdProtocolo = iValue;
				} else {
					this.ShowError("O identificador deve ser numérico!");
				}

			status = EnumFromListItem<CartaPositivaCraStatus>(dpdSituacao.SelectedValue);

			protocoloCra = txtProtocoloCra.Text;
			carteira = txtCarteirinha.Text;

			DataTable dt = CartaPositivaCraBO.Instance.Pesquisar(cdProtocolo, protocoloCra, matricula.ToString(), carteira, status);
			this.ShowPagingGrid(gdvRelatorio, dt, null);

			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
			//btnExportar.Visible = dt.Rows.Count > 0;
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar dados", ex);
			}

		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (IsPagingCommand(sender, e)) return;

				if ("CmdAprovar".Equals(e.CommandName)) {
					int id = Convert.ToInt32(GetKeyCommand(e, gdvRelatorio).Value);
					CartaPositivaCraBO.Instance.AprovarSolicitacao(id, UsuarioLogado.Usuario);
					this.ShowInfo("Aprovação realizada com sucesso!");

				} else if ("CmdCancelar".Equals(e.CommandName)) {
					/*int id = Convert.ToInt32(GetKeyCommand(e, gdvRelatorio).Value);
					CartaPositivaCraBO.Instance.CancelarSolicitacao(id, UsuarioLogado.Usuario);
					this.ShowInfo("Cancelamento realizado com sucesso!");*/
				}
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao executar a operação.", ex);
			}
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				DataRowView drv = (DataRowView) row.DataItem;

				CartaPositivaCraStatus status = ChangeCellEnumDrv<CartaPositivaCraStatus>(gdvRelatorio, row, "CD_STATUS", CartaPositivaCraEnumTradutor.TraduzStatus).Value;

				LinkButton btnAprovar = (LinkButton)row.FindControl("btnAprovar");
				LinkButton btnCancelar = (LinkButton)row.FindControl("btnCancelar");
				ImageButton btnPdf = (ImageButton)row.FindControl("btnPdf");

				bool hasPermAprov = HasPermission(Modulo.CARTA_POSITIVA_CRA_APROVAR);

				btnAprovar.Visible = btnCancelar.Visible = btnPdf.Visible = false;

				if (status == CartaPositivaCraStatus.PENDENTE) {
					btnAprovar.Visible = hasPermAprov;
					btnCancelar.Visible = hasPermAprov;
				} else if (status == CartaPositivaCraStatus.APROVADO) {
					btnCancelar.Visible = hasPermAprov;
					btnPdf.Visible = true;
				} else if (status == CartaPositivaCraStatus.CANCELADO) {
					GetTableCell(gdvRelatorio, row, "CD_STATUS").Text += " - " + drv["DS_MOTIVO_CANCELAMENTO"];
				}
			}
		}
	}
}