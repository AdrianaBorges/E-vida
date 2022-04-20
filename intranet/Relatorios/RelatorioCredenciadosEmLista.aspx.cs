using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using SkyReport.ExcelExporter;
using eVidaGeneralLib.ReadWriteCsv;

namespace eVidaIntranet.Relatorios
{
    public partial class RelatorioCredenciadosEmLista : RelatorioExcelPageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /*IEnumerable<HcNaturezaVO> lstNatureza = LocatorDataBO.Instance.ListarNatureza().Select(x => new HcNaturezaVO()
                {
                    CdNatureza = x.CdNatureza,
                    DsNatureza = x.DsNatureza
                });
                dpdNatureza.DataSource = lstNatureza;
                dpdNatureza.DataBind();
                dpdNatureza.Items.Insert(0, new ListItem("Todas", ""));*/

                chkUf.DataSource = LocatorDataBO.Instance.ListarUf();
                chkUf.DataBind();

            }

        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.RELATORIO_CREDENCIADOS_LISTA; }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {

            List<string> lstUf = new List<string>();
            /*int nValue;
            int? cdNatureza = null;
            string sistema = null;*/
            string status = null;

            foreach (ListItem item in chkUf.Items)
            {
                if (item.Selected)
                    lstUf.Add(item.Value);
            }
            
            /*if (!string.IsNullOrEmpty(dpdNatureza.SelectedValue))
            {
                if (Int32.TryParse(dpdNatureza.SelectedValue, out nValue))
                {
                    cdNatureza = nValue;
                }
            }

            if (!string.IsNullOrEmpty(dpdSistema.SelectedValue))
                sistema = dpdSistema.SelectedValue;*/

            if (!string.IsNullOrEmpty(dpdStatus.SelectedValue))
                status = dpdStatus.SelectedValue;

            try
            {
                DataTable dtAcessos = RelatorioBO.Instance.BuscarCredenciadosEmLista(lstUf, status);

                btnExportar.Visible = dtAcessos.Rows.Count > 0;
                lblCount.Visible = true;
                lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

                this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);
                pnlGrid.Update();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao consultar o relatorio", ex);
            }
        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                TableCell cellNome = row.Cells[0];
                if (vo["BAU_NOME"].ToString().Trim() != "")
                {
                    cellNome.Text = vo["BAU_NOME"].ToString().ToUpper();
                }


                TableCell cellEmail = row.Cells[1];
                if (vo["BAU_EMAIL"].ToString().Trim() != "")
                {
                    cellEmail.Text = vo["BAU_EMAIL"].ToString().ToLower();
                }
                else {
                    
                    cellEmail.Text = "EMAIL NÃO CADASTRADO";
                    cellEmail.ControlStyle.ForeColor = System.Drawing.Color.Red;
                }
                    
            }
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable sourceTable = this.GetRelatorioTable();

                // Gera um nome de arquivo
                var nomeArquivo = "~/Export/RelatorioCredenciadosEmLista.csv";

                // Write sample data to CSV file
                using (CsvFileWriter writer = new CsvFileWriter(Server.MapPath(nomeArquivo)))
                {
                    foreach (DataRow dr in sourceTable.Rows)
                    {
                        CsvRow row = new CsvRow();
                        row.Add(dr["BAU_NOME"].ToString());

                        if (dr["BAU_EMAIL"].ToString().Trim() != "")
                        {
                            row.Add(dr["BAU_EMAIL"].ToString().ToLower());
                        }
                        else
                        {

                        }

                        writer.WriteRow(row);
                    }
                }

                // Baixa o arquivo
                RegisterScript("CREDENCIADOS_CSV", "window.open('" + ResolveUrl(nomeArquivo) + "');");
            }
            catch (Exception ex)
            {
                this.ShowError("Houve um erro ao gerar o arquivo .CSV: " + ex.ToString());
            }
        }

    }
}