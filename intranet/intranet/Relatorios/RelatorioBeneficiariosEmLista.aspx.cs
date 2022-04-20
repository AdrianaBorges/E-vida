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
using SkyReport.ExcelExporter;
using eVidaGeneralLib.ReadWriteCsv;

namespace eVidaIntranet.Relatorios
{
    public partial class RelatorioBeneficiariosEmLista : RelatorioExcelPageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                IEnumerable<object> lstLocal = PLocatorDataBO.Instance.ListarRegioes().Select(x => new
                {
                    Codigo = x.Key,
                    Descricao = x.Value
                });
                chkRegional.DataSource = lstLocal;
                chkRegional.DataBind();

                chkUf.DataSource = PLocatorDataBO.Instance.ListarUf();
                chkUf.DataBind();

                chkParentesco.DataSource = PLocatorDataBO.Instance.ListaParentescos();
                chkParentesco.DataBind();

                chkPlano.DataSource = PLocatorDataBO.Instance.ListarProdutoSaude().Select(x => new
                {
                    Codigo = x.Codigo,
                    Descricao = x.Descri + " (" + x.Codigo + ") "
                });
                chkPlano.DataBind();
            }
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.RELATORIO_BENEFICIARIOS_LISTA; }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> lstRegional = new List<string>();
            List<string> lstPlano = new List<string>();
            List<string> lstGrauParentesco = new List<string>();
            List<string> lstUf = new List<string>();
            bool? deficienteFisico = null;
            bool? estudante = null;

            foreach (ListItem item in chkRegional.Items)
            {
                if (item.Selected)
                    lstRegional.Add(item.Value);
            }
            foreach (ListItem item in chkPlano.Items)
            {
                if (item.Selected)
                    lstPlano.Add(item.Value);
            }
            foreach (ListItem item in chkUf.Items)
            {
                if (item.Selected)
                    lstUf.Add(item.Value);
            }
            foreach (ListItem item in chkParentesco.Items)
            {
                if (item.Selected)
                    lstGrauParentesco.Add(item.Value);
            }

            if (!string.IsNullOrEmpty(dpdDeficienteFisico.SelectedValue))
            {
                deficienteFisico = dpdDeficienteFisico.SelectedValue.Equals("S");
            }
            if (!string.IsNullOrEmpty(dpdEstudante.SelectedValue))
            {
                estudante = dpdEstudante.SelectedValue.Equals("S");
            }

            try
            {
                DataTable dtAcessos = RelatorioBO.Instance.BuscarBeneficiariosEmLista(lstRegional, lstPlano, lstUf, lstGrauParentesco, deficienteFisico, estudante);

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

                TableCell cellEmail = row.Cells[1];
                if (vo["BA1_EMAIL"].ToString().Trim() != "")
                {
                    cellEmail.Text = vo["BA1_EMAIL"].ToString().ToLower();
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
                var nomeArquivo = "~/Export/RelatorioBeneficiariosEmLista.csv";

                // Write sample data to CSV file
                using (CsvFileWriter writer = new CsvFileWriter(Server.MapPath(nomeArquivo)))
                {

                    // Títulos das colunas
                    if (sourceTable.Rows.Count > 0)
                    {
                        CsvRow row = new CsvRow();
                        row.Add("name");
                        row.Add("email");
                        writer.WriteRow(row);
                    }

                    foreach (DataRow dr in sourceTable.Rows)
                    {
                        CsvRow row = new CsvRow();
                        row.Add(dr["BA1_NOMUSR"].ToString());

                        if (dr["BA1_EMAIL"].ToString().Trim() != "")
                        {
                            row.Add(dr["BA1_EMAIL"].ToString().ToLower());
                        }
                        else
                        {
                        }

                        writer.WriteRow(row);
                    }
                }

                // Baixa o arquivo
                RegisterScript("BENEFICIARIOS_CSV", "window.open('" + ResolveUrl(nomeArquivo) + "');");
            }
            catch (Exception ex)
            {
                this.ShowError("Houve um erro ao gerar o arquivo .CSV: " + ex.ToString());
            }
        }

    }
}