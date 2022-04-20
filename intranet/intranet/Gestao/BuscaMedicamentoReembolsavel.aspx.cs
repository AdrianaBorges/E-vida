using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Gestao {
	public partial class BuscaMedicamentoReembolsavel : RelatorioExcelPageBase {

	    protected override void PageLoad(object sender, EventArgs e) 
        {
            if (!IsPostBack)
            {

                try
                {

                    List<PPlanoVO> lstPlanos = PlanoBO.Instance.ListarPlanos();
                    dpdPlano.Items.Insert(0, new ListItem("TODOS", ""));
                    if (lstPlanos != null)
                    {
                        lstPlanos.Sort((x, y) => x.Nome.CompareTo(y.Nome));
                        foreach (PPlanoVO plano in lstPlanos)
                        {
                            dpdPlano.Items.Add(new ListItem(plano.Nome, plano.Codigo));
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.ShowError("Erro ao carregar a página", ex);
                }
            }
	    }

		protected override Modulo Modulo {
			get { return Modulo.GESTAO_MEDICAMENTO_REEMBOLSAVEL; }
		}

        private void Buscar()
        {
            string cdMascara = null;
            string descricao = null;
            string reembolsavel = null;
            string continuo = null;
            string plano = null;

            if (!string.IsNullOrEmpty(txtSDescricao.Text))
            {
                descricao = txtSDescricao.Text;
            }
            if (!string.IsNullOrEmpty(txtSMascara.Text))
            {
                cdMascara = txtSMascara.Text;
            }
            if (!string.IsNullOrEmpty(dpdReembolsavel.SelectedValue))
            {
                reembolsavel = dpdReembolsavel.SelectedValue;
            }
            if (!string.IsNullOrEmpty(dpdContinuo.SelectedValue))
            {
                continuo = dpdContinuo.SelectedValue;
            }
            if (!string.IsNullOrEmpty(dpdPlano.SelectedValue))
            {
                plano = dpdPlano.SelectedValue;
            }

            DataTable dt = MedicamentoReembolsavelBO.Instance.Pesquisar(cdMascara, descricao, reembolsavel, continuo, plano);
            if (dt != null)
            {
                if (dt.Rows.Count > 300)
                {
                    this.ShowInfo("Apenas os 300 primeiros registros foram retornados! Refine sua busca!");
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);
                }
                lblCount.Text = "Exibindo " + dt.Rows.Count + " registros!";
            }
            else
            {
                lblCount.Text = "Sem registros para o filtro!";
            }

            this.ShowPagingGrid(gdvRelatorio, dt, null);
            btnExportar.Visible = dt.Rows.Count > 0;
            //lblCount.Text = "Foram encontrados " + lstTemplates.Count() + " registros.";
        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow row = e.Row;

                // Flag Reembolsável
                if (row.Cells[3].Text.Trim() == "0")
                {
                    row.Cells[3].Text = "NÃO";
                }
                else if (row.Cells[3].Text.Trim() == "1")
                {
                    row.Cells[3].Text = "SIM";
                }

                // Flag Contínuo
                if (row.Cells[4].Text.Trim() == "0")
                {
                    row.Cells[4].Text = "NÃO";
                }
                else if (row.Cells[4].Text.Trim() == "1")
                {
                    row.Cells[4].Text = "SIM";
                }

                // Planos
                if (row.Cells[5].Text.Trim() != "&nbsp;")
                {
                    String textoPlanos = "";
                    String[] planos = row.Cells[5].Text.Trim().Split('|');
                    foreach (String plano in planos)
                    {
                        if (plano.Trim() != "")
                        {
                            PPlanoVO vo = PlanoBO.Instance.GetPlano("0001", plano);

                            if (textoPlanos != "")
                            {
                                textoPlanos += "<br />";
                            }

                            textoPlanos += vo.Nome;
                        }
                    }

                    row.Cells[5].Text = textoPlanos;

                }

            }
        }

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar busca", ex);
			}
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

        private void ExportarExcel()
        {

            DataTable sourceTable = this.GetRelatorioTable();

            ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

            defs["CK_REEMBOLSAVEL"].ForEach(x => x.Transformer = y => TransformarFlag(y));
            defs["CK_USO_CONTINUO"].ForEach(x => x.Transformer = y => TransformarFlag(y));
            defs["DS_LST_PLANO"].ForEach(x => x.Transformer = y => TransformarPlano(y));

            defs.SetWidth("DS_SERVICO", 50);
            defs.SetWidth("DS_PRINCIPIO_ATIVO", 50);
            defs.SetWidth("DS_LST_PLANO", 50);

            ExportExcel("RelatorioMedicamentoReembolsavel", defs, sourceTable);
        }

        private string TransformarFlag(object y)
        {

            string retorno = "";

            if (y == null)
            {
                return retorno;
            }
            else if (y.ToString().Trim() == "")
            {
                return retorno;
            }
            else if (y.ToString().Trim() == "1")
            {
                retorno = "SIM";
            }
            else
            {
                retorno = "NÃO";
            }

            return retorno;
        }

        private string TransformarPlano(object y)
        {
            string retorno = "";

            if (y == null)
            {
                return retorno;
            }
            else if (y.ToString().Trim() == "")
            {
                return retorno;
            }
            else
            {
                String textoPlanos = "";
                String[] planos = y.ToString().Trim().Split('|');
                foreach (String plano in planos)
                {
                    if (plano.Trim() != "")
                    {
                        PPlanoVO vo = PlanoBO.Instance.GetPlano("0001", plano);

                        if (textoPlanos != "")
                        {
                            textoPlanos += "\n";
                        }

                        textoPlanos += vo.Nome;
                    }
                }

                retorno = textoPlanos;
            }

            return retorno;
        }

	}
}