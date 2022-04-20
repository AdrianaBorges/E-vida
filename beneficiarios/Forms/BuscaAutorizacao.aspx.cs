using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaBeneficiarios.Forms {
	public partial class BuscaAutorizacao : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					BindAll();
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar tela. ", ex);
				}
			}
		}

		private void BindAll() {
            DataTable dt = AutorizacaoBO.Instance.BuscarAutorizacaoByBeneficiario(UsuarioLogado.UsuarioTitular.Codint, UsuarioLogado.UsuarioTitular.Codemp, UsuarioLogado.UsuarioTitular.Matric, UsuarioLogado.UsuarioTitular.Tipreg);
			this.ShowGrid(gdvRelatorio, dt, null, "cd_autorizacao DESC");
			lblCount.Text = "Você possui " + dt.Rows.Count + " solicitações vinculadas!";
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView rvo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;
				ImageButton btnCancelar = (ImageButton)row.FindControl("btnCancelar");
				ImageButton btnEditar = (ImageButton)row.FindControl("btnEditar");
				ImageButton btnPdf = (ImageButton)row.FindControl("btnPdf");
				ImageButton btnRevalidar = (ImageButton)row.FindControl("btnRevalidar");
				TableCell cellTiss = row.Cells[3];
				TableCell cellStatus = row.Cells[7];

				btnRevalidar.Visible = false;

				AutorizacaoVO vo = (AutorizacaoVO)rvo["OBJ"];
				StatusAutorizacao status = vo.Status;
				OrigemAutorizacao origem = vo.Origem;
				cellStatus.Text = AutorizacaoTradutorHelper.TraduzStatus(status);
				if (status == StatusAutorizacao.CANCELADA) {
					cellStatus.Text += " - " + vo.MotivoCancelamento;
				}
				btnCancelar.Visible = status == StatusAutorizacao.ENVIADA && origem == OrigemAutorizacao.BENEF;

				if (btnCancelar.Visible || status == StatusAutorizacao.SOLICITANDO_DOC) {
					btnEditar.ImageUrl = ResolveClientUrl("~/img/ico_editar.gif");
				} else {
					btnEditar.ImageUrl = ResolveClientUrl("~/img/lupa.gif");
				}

				btnPdf.Visible = false;
				if (status == StatusAutorizacao.APROVADA) {
					List<AutorizacaoTissVO> lstTiss = AutorizacaoBO.Instance.ListTiss(vo.Id);
					if (lstTiss != null && lstTiss.Count > 0) {
						btnPdf.Visible = true;
						btnPdf.OnClientClick = "return openPdf(" + vo.Id + ", '" + lstTiss[0].NomeArquivo + "');";
						cellTiss.Text = lstTiss.Select(x => x.NrAutorizacaoTiss.ToString()).Aggregate((x, y) => x + ", " + y);
					}
					if (AutorizacaoBO.Instance.PodeRevalidar(vo)) {
						btnRevalidar.Visible = true;
					}
				} else if (status == StatusAutorizacao.NEGADA) {
					if (vo.CodNegativa != null) {
						btnPdf.Visible = true;
						btnPdf.OnClientClick = "return openNegPdf(" + vo.CodNegativa.Value + ");";
					}
				}

			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			Response.Redirect("~/Forms/Autorizacao.aspx");
		}

		protected void btnCancelar_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btnCancelar = (ImageButton)sender;
				int idAutorizacao = Int32.Parse(btnCancelar.CommandArgument);

				AutorizacaoBO.Instance.Cancelar(idAutorizacao, AutorizacaoTradutorHelper.CANCELADO_BENEFICIARIO, null);
				this.ShowError("Solicitação cancelada!");
				BindAll();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao cancelar", ex);
			}
		}

	}
}