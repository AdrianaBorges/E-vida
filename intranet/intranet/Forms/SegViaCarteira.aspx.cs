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
using eVidaGeneralLib.VO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Forms
{
    public partial class SegViaCarteira : RelatorioExcelPageBase
    {

        protected override void PageLoad(object sender, EventArgs e)
        {

        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return Modulo.GESTAO_SEG_VIA; }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {

            DateTime? dtInicial = null;
            DateTime? dtFinal = null;
            int? cdProtocolo = null;
            long? cdFuncionario = null;
            StatusSegVia? status = null;
            string protocoloAns = null;

            try
            {
                if (!string.IsNullOrEmpty(txtInicio.Text))
                {
                    DateTime data;
                    if (!DateTime.TryParse(txtInicio.Text, out data))
                    {
                        this.ShowError("A data inicial está inválida!");
                        return;
                    }
                    dtInicial = data;
                }

                if (!string.IsNullOrEmpty(txtFim.Text))
                {
                    DateTime data;
                    if (!DateTime.TryParse(txtFim.Text, out data))
                    {
                        this.ShowError("A data final está inválida!");
                        return;
                    }
                    dtFinal = data;
                }

                if (!string.IsNullOrEmpty(txtProtocolo.Text))
                {
                    int iValue;
                    if (!Int32.TryParse(txtProtocolo.Text, out iValue))
                    {
                        this.ShowError("O protocolo deve ser um número!");
                        return;
                    }
                    cdProtocolo = iValue;
                }

                if (!string.IsNullOrEmpty(txtMatricula.Text))
                {
                    long lValue;
                    if (!Int64.TryParse(txtMatricula.Text, out lValue))
                    {
                        this.ShowError("A matrícula deve ser um número!");
                        return;
                    }
                    cdFuncionario = lValue;
                }

                if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue))
                {
                    char c = dpdSituacao.SelectedValue[0];
                    int iValue = (int)c;
                    status = (StatusSegVia)Enum.Parse(typeof(StatusSegVia), iValue + "");
                }
                if (!string.IsNullOrEmpty(txtProtocoloANS.Text))
                {
                    protocoloAns = txtProtocoloANS.Text;
                }

                DataTable dt = SegViaCarteiraBO.Instance.PesquisarSegViaCarteira(cdProtocolo, txtMatricula.Text, protocoloAns, dtInicial, dtFinal, status);
                this.ShowPagingGrid(gdvRelatorio, dt, null);

                lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";

                btnExportar.Visible = dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar as solicitações.", ex);
            }

        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                LinkButton btnFinalizar = (LinkButton)row.FindControl("btnAprovar");
                LinkButton btnCancelar = (LinkButton)row.FindControl("btnNegar");
                TableCell cellStatus = row.Cells[7];
                StatusSegVia status = (StatusSegVia)Convert.ToChar(vo["TP_STATUS"]);
                cellStatus.Text = SolicitacaoSegViaCarteiraEnumTradutor.TraduzStatus(status);
                if (status == StatusSegVia.PENDENTE)
                {
                    btnFinalizar.Visible = true;
                    btnCancelar.Visible = true;
                    btnFinalizar.OnClientClick = "return confirm('Deseja realmente finalizar esta solicitação?')";
                }
                else
                {
                    btnFinalizar.Visible = false;
                    btnCancelar.Visible = false;

                    if (status == StatusSegVia.CANCELADO)
                    {
                        string motivo = Convert.ToString(vo["DS_MOTIVO_CANCELAMENTO"]);

                        cellStatus.Text += " - " + motivo;
                        cellStatus.ToolTip = motivo;
                    }
                }

                // ---------------------- Arquivos ------------------------------
                ImageButton btnArquivo = row.FindControl("btnArquivo") as ImageButton;
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

        protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.IsPagingCommand(sender, e)) return;
            if (e.CommandName == "CmdFinalizar" || e.CommandName == "CmdCancelar")
            {
                // Retrieve the row index stored in the CommandArgument property.
                int index = Convert.ToInt32(e.CommandArgument);

                // Retrieve the row that contains the button 
                // from the Rows collection.
                GridViewRow row = gdvRelatorio.Rows[index];

                if (e.CommandName == "CmdFinalizar")
                {
                    AlterarStatusLinha(row, index, StatusSegVia.FINALIZADO);
                }
                else if (e.CommandName == "CmdCancelar")
                {
                    AlterarStatusLinha(row, index, StatusSegVia.CANCELADO);
                }

            }
        }

        private void AlterarStatusLinha(GridViewRow row, int index, StatusSegVia status)
        {
            LinkButton btnFinalizar = (LinkButton)row.FindControl("btnAprovar");
            LinkButton btnCancelar = (LinkButton)row.FindControl("btnNegar");
            TableCell cellStatus = row.Cells[6];

            int cdProtocolo = Convert.ToInt32(gdvRelatorio.DataKeys[index]["CD_SOLICITACAO"]);

            string strStatus = SolicitacaoSegViaCarteiraEnumTradutor.TraduzStatus(status);
            if (status == StatusSegVia.FINALIZADO)
            {
                SegViaCarteiraBO.Instance.Finalizar(cdProtocolo, UsuarioLogado.Usuario);
            }
            else if (status == StatusSegVia.CANCELADO)
            {
                SolicitacaoSegViaCarteiraVO vo = SegViaCarteiraBO.Instance.GetById(cdProtocolo);
                strStatus += " - " + vo.MotivoCancelamento;
            }
            btnCancelar.Visible = false;
            btnFinalizar.Visible = false;
            cellStatus.Text = strStatus;
        }

        private void ExportarExcel()
        {
            DataTable sourceTable = this.GetRelatorioTable();

            ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

            defs.SetWidth("BA1_NOMUSR", 30);

            defs["TP_STATUS"][0].Transformer = x => SolicitacaoSegViaCarteiraEnumTradutor.TraduzStatus((StatusSegVia)Convert.ToChar(x));

            ExportExcel("RelatorioSegViaCarteira", defs, sourceTable);
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                ExportarExcel();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao exportar para excel!", ex);
            }
        }

    }
}