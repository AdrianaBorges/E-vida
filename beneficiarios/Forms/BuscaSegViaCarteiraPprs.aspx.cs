using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.BO;
using eVidaBeneficiarios.Classes;
using System.Data;

namespace eVidaBeneficiarios.Forms
{
    public partial class BuscaSegViaCarteiraPprs : PageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = SegViaCarteiraBO.Instance.BuscarSolicitacoes(UsuarioLogado.UsuarioTitular);
                this.ShowGrid(gdvRelatorio, dt, null, "cd_solicitacao DESC");
                lblCount.Text = "Você já realizou " + dt.Rows.Count + " solicitações!";
            }
        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                ImageButton btnArquivo = row.FindControl("btnArquivo") as ImageButton;

                row.Cells[3].Text = Convert.ToChar(vo["TP_STATUS"]) == (char)StatusSegVia.PENDENTE ? "PENDENTE" : "FINALIZADO";

                // ---------------------- Arquivos ------------------------------

                int cd_solicitacao = Convert.ToInt32(vo["CD_SOLICITACAO"]);
                List<SolicitacaoSegViaCarteiraArquivoVO> lstArquivos = SegViaCarteiraBO.Instance.ListarArquivos(cd_solicitacao);

                if (lstArquivos != null && lstArquivos.Count > 0)
                {
                    btnArquivo.OnClientClick = "return openDownload(" + cd_solicitacao + "," + lstArquivos[0].IdArquivo + ", false)";
                }
                else
                {
                    btnArquivo.Visible = false;
                }

                // --------------------------------------------------------------
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Forms/SegViaCarteiraPprs.aspx");
        }

    }
}