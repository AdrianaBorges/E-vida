using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportExclusaoBinder : IReportBinder {
		private ExclusaoVO vo;

		public ReportExclusaoBinder(ExclusaoVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			if (vo.CodSolicitacao == Int32.MinValue) {
				return GetBlank();
			}
			ReportBinderParams repParams = new ReportBinderParams();

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(ExclusaoVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NomeTitular", titular.Nomusr);
			repParams.Params.Add("Cartao", titular.Matant);
			repParams.Params.Add("Email", titular.Email);
			repParams.Params.Add("Local", vo.Local);
			repParams.Params.Add("DataCriacao", FormatUtil.FormatDataHoraExtenso(vo.DataCriacao));
			repParams.Params.Add("Obs", string.IsNullOrEmpty(vo.Observacao) ? "-" : vo.Observacao);

			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("Plano");

			DataTable dtBenef = FormExclusaoBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
			foreach (DataRow dr in dtBenef.Rows) {
				DataRow drN = dt.NewRow();
				drN["Nome"] = Convert.ToString(dr["BA1_NOMUSR"]);
				drN["Parentesco"] = GetParentesco(dr);
				drN["Plano"] = Convert.ToString(dr["BI3_DESCRI"]);

				dt.Rows.Add(drN);
			}

			repParams.DataSources.Add("DataSet1", dt);
			return repParams;
		}

		public string GerarNome() {
			if (vo.CodSolicitacao == Int32.MinValue)
				return "Exclusao_BRANCO";
			return "Exclusao_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptExclusao";
		}

		private string GetParentesco(DataRow dr) {

			bool isTitular = Convert.ToInt32(dr["in_titular"]) == 1;
			bool isDepFamilia = Convert.ToInt32(dr["in_dep_familia"]) == 1;
			if (isTitular) {
				return "TITULAR";
			} else {
				if (isDepFamilia) {
					return "DEPENDENTE FAMÍLIA";
				} else {
					return Convert.ToString(dr["BRP_DESCRI"]);
				}
			}
		}

		private ReportBinderParams GetBlank() {
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.Params.Add("NumSolicitacao", "   ");

			repParams.Params.Add("NomeBeneficiario", " ");
			repParams.Params.Add("NomeTitular", " ");
			repParams.Params.Add("Cartao", " ");
			repParams.Params.Add("Email", " ");
			repParams.Params.Add("DataCriacao", " ");
			repParams.Params.Add("Local", " ");

			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("Plano");
			dt.Columns.Add("Idade");
			for (int i = 0; i < 3; ++i) {
				DataRow drN = dt.NewRow();
				dt.Rows.Add(drN);
			}

			repParams.DataSources.Add("DataSet1", dt);
			return repParams;
		}
	}
}
