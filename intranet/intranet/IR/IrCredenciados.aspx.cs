using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.IR {
	public partial class IrCredenciados : PageBase {

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.IR_CREDENCIADO; }
        }

		public class GridRowHelper {
			GridViewRow row;
			public GridRowHelper(GridViewRow r) {
				this.row = r;
			}

			public HyperLink LinkRendimentos { get { return (HyperLink) row.Cells[3].Controls[0]; } }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string downloadFile = Request["DW_FILE"];
				string strId = Request["ID"];
				if (!string.IsNullOrEmpty(downloadFile) && !string.IsNullOrEmpty(strId)) {
					int tipo = Int32.Parse(downloadFile);
					int ano = DateTime.Now.Year - 1;
					if (!string.IsNullOrEmpty(Request["ANO"])) {
						ano = Int32.Parse(Request["ANO"]);
					}
                    WriteFileIr(strId, ano);
				} else {
					dpdAnoRef.Items.Add(new ListItem((DateTime.Now.Year - 1).ToString(), (DateTime.Now.Year - 1).ToString()));
					dpdAnoRef.SelectedValue = (DateTime.Now.Year - 1).ToString();
				}
			}
		}

        private void WriteFileIr(string idCredenciado, int ano)
        {
            PRedeAtendimentoVO cred = PRedeAtendimentoBO.Instance.GetById(idCredenciado);

            string fullName = GetFile(cred, ano);

            Response.Clear();
            if (System.IO.File.Exists(fullName))
            {
                string name = System.IO.Path.GetFileName(fullName);
                Response.AddHeader("content-disposition", "attachment;filename=\"" + name + "\"");
                Response.WriteFile(fullName);
            }
            else
            {
                Response.Write("Arquivo inválido!");
                Response.End();
            }
        }

        private string GetFile(PRedeAtendimentoVO credVO, int ano)
        {
            string dir = GetIrDir(ano);
            IEnumerable<string> files = System.IO.Directory.EnumerateFiles(dir, GetPrefixFileName(credVO, ano) + ".*");
            if (files.Count() == 0) return null;
            foreach (string file in files)
            {
                if (file.EndsWith("pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    return file;
                }
            }
            return files.First();
        }

        private string GetIrDir(int ano)
        {

            string dir = ParametroUtil.IrFolderCredenciado;
            dir = System.IO.Path.GetFullPath(dir);
            dir = System.IO.Path.Combine(dir, ano.ToString());
            return dir;
        }

        private string GetPrefixFileName(PRedeAtendimentoVO credVO, int ano)
        {
            string file = string.Empty;
            if (credVO.Tippe.Equals(PConstantes.PESSOA_JURIDICA))
            {
                string cnpj = FormatUtil.UnformatCnpj(FormatUtil.FormatCnpj(credVO.Cpfcgc));
                file = cnpj;
            }
            else
            {
                string cpf = FormatUtil.UnformatCpf(FormatUtil.FormatCpf(credVO.Cpfcgc));
                file = cpf;
            }
            return file;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar credenciados", ex);
            }
        }

        private void Buscar()
        {
            long cpfCnpj = 0;
            string razaoSocial = txtRazaoSocial.Text.Trim();

            if (!string.IsNullOrEmpty(razaoSocial) && (razaoSocial.Length < 3))
            {
                this.ShowError("Informe pelo menos 3 caracteres para busca pelo nome!");
                return;
            }

            if (!string.IsNullOrEmpty(txtCnpj.Text.Trim()) && !Int64.TryParse(txtCnpj.Text.Trim(), out cpfCnpj))
            {
                this.ShowError("O documento deve ser numérico!");
                return;
            }
            if (string.IsNullOrEmpty(razaoSocial.Trim()) && cpfCnpj == 0)
            {
                this.ShowError("Por favor informe pelo menos um filtro!");
                return;
            }

            DataTable dt = PRedeAtendimentoBO.Instance.Pesquisar(razaoSocial, txtCnpj.Text.Trim(), false);

            gdvResultado.DataSource = dt;
            gdvResultado.DataBind();

            if (dt.Rows.Count == 0)
            {
                lblCount.Visible = false;
                this.ShowInfo("Não foram encontrados credenciados com estes filtros!");
                return;
            }
            else
            {
                lblCount.Visible = true;
                lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
            }
        }

		protected void gdvResultado_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				GridRowHelper helper = new GridRowHelper(row);
				DataRowView dr = (DataRowView)row.DataItem;

                PRedeAtendimentoVO cred = PRedeAtendimentoBO.Instance.GetById(dr["BAU_CODIGO"].ToString());
				int ano = Int32.Parse(dpdAnoRef.SelectedValue);

				string fullName = GetFile(cred, ano);

				helper.LinkRendimentos.Visible = !string.IsNullOrEmpty(fullName);
				helper.LinkRendimentos.NavigateUrl = "IrCredenciados.aspx?id=" + cred.Codigo.Trim() + "&DW_FILE=1078&ANO=" + ano;

				if (!helper.LinkRendimentos.Visible) {
					((TableCell)helper.LinkRendimentos.Parent).Text = "Não disponível";
				}
			}
		}

	}
}