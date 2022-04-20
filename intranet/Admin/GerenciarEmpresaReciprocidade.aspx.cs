using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Admin {
	public partial class GerenciarEmpresaReciprocidade : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
                carregarEmpresas();
			}
		}

        private void carregarEmpresas() {
            List<EmpresaReciprocidadeVO> lstEmpresa = ReciprocidadeBO.Instance.ListarEmpresas();
            gdvEmpresas.DataSource = lstEmpresa;
            gdvEmpresas.DataBind();
        }

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_EMP_RECIPROCIDADE; }
		}

		private void Excluir(GridViewRow row, int index) {
			int cdEmpresa = Int32.Parse(gdvEmpresas.DataKeys[index][0].ToString());
			
			/*DataTable dt = ReciprocidadeBO.Instance.RelatorioReciprocidade(null, null, null, null, cdEmpresa, null, null, null);
			if (dt.Rows.Count > 0) {
				this.ShowError("Esta empresa possui solicitações de reciprocidades vinculadas!");
				return;
			}
			ReciprocidadeBO.Instance.ExcluirEmpresa(cdEmpresa);*/
            carregarEmpresas();
		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				EmpresaReciprocidadeVO vo = (EmpresaReciprocidadeVO)e.Row.DataItem;
				GridViewRow row = e.Row;

				string str = " - ";
				if (vo.AreaAtuacao != null) {
					foreach (string area in vo.AreaAtuacao) {
						str += area + " - ";
					}
				}
				row.Cells[2].Text = str.Substring(3);
			}
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (e.CommandName == "Excluir") {
					if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
						// Retrieve the row index stored in the CommandArgument property.
						int index = Convert.ToInt32(e.CommandArgument);

						// Retrieve the row that contains the button 
						// from the Rows collection.
						GridViewRow row = gdvEmpresas.Rows[index];

						Excluir(row, index);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}
	}
}