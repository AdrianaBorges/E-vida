using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.Util;

namespace eVidaIntranet.Forms {
	public partial class ViewExclusao : PageBase {

		[Serializable]
		private class ExclusaoBenefTelaVO : ExclusaoBenefVO {
			public string Nome { get; set; }
			public string Parentesco { get; set; }
			public string Plano { get; set; }
			public String Nascimento { get; set; }
		}

		protected override void PageLoad(object sender, EventArgs e) {

			try {
				if (!IsPostBack) {
					int id = 0;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Solicitação inválida!");
						return;
					}

					ExclusaoVO vo = FormExclusaoBO.Instance.GetById(id);
					Bind(vo);
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao abrir a tela", ex);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.EXCLUSAO; }
		}

		private void Bind(ExclusaoVO vo) {

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			txtLocal.Text = vo.Local;
			txtEmailTitular.Text = titular.Email;
			txtNomeTitular.Text = titular.Nomusr;
			txtMatricula.Text = titular.Matant;

			lblData.Text = FormatUtil.FormatDataHoraExtenso(vo.DataCriacao);

			DataTable dtBenef = FormExclusaoBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
			List<ExclusaoBenefTelaVO> lst = new List<ExclusaoBenefTelaVO>();
			foreach (DataRow dr in dtBenef.Rows) {
				lst.Add(new ExclusaoBenefTelaVO()
				{
					Codint = Convert.ToString(dr["BA1_CODINT"]),
                    Codemp = Convert.ToString(dr["BA1_CODEMP"]),
                    Matric = Convert.ToString(dr["BA1_MATRIC"]),
                    Tipreg = Convert.ToString(dr["BA1_TIPREG"]),
                    Cdusuario = Convert.ToString(dr["BA1_CODINT"]).Trim() + "|" + Convert.ToString(dr["BA1_CODEMP"]).Trim() + "|" + Convert.ToString(dr["BA1_MATRIC"]).Trim() + "|" + Convert.ToString(dr["BA1_TIPREG"]).Trim(),
                    CodPlano = Convert.ToString(dr["BI3_CODIGO"]),
					IsDepFamilia = Convert.ToInt32(dr["in_dep_familia"]) == 1,
					IsTitular = Convert.ToInt32(dr["in_titular"]) == 1,
                    Nome = Convert.ToString(dr["BA1_NOMUSR"]),
					Plano = Convert.ToString(dr["BI3_DESCRI"]),
                    Parentesco = Convert.ToString(dr["BRP_DESCRI"]),
                    Nascimento = Convert.ToString(dr["BA1_DATNAS"])
				});
			}
			gdvDependentes.DataSource = lst;
			gdvDependentes.DataBind();

		}

		private string GetParentesco(ExclusaoBenefTelaVO vo) {
			if (vo.IsTitular) {
				return "TITULAR";
			} else {
				if (vo.IsDepFamilia) {
					return "DEPENDENTE FAMÍLIA";
				} else {
					return vo.Parentesco;
				}
			}
		}

		protected void gdvDependentes_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				ExclusaoBenefTelaVO vo = (ExclusaoBenefTelaVO)row.DataItem;

				row.Cells[2].Text = GetParentesco(vo);

				row.Cells[0].Text = (row.DataItemIndex + 1).ToString();

			}
		}

	}
}