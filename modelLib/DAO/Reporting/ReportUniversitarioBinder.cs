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
using System.Globalization;

namespace eVidaGeneralLib.Reporting {
	public class ReportUniversitarioBinder : IReportBinder {
		private DeclaracaoUniversitarioVO vo;
		public ReportUniversitarioBinder(DeclaracaoUniversitarioVO vo) {

			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			if (vo.CodSolicitacao == Int32.MinValue) {
				return GetBlank();
			}
			ReportBinderParams repParams = new ReportBinderParams();
			PUsuarioVO titularVO = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(DeclaracaoUniversitarioVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NomeTitular", titularVO.Nomusr);
			repParams.Params.Add("Cartao", titularVO.Matant);
			repParams.Params.Add("Email", titularVO.Email);
			repParams.Params.Add("Local", " - ");
			repParams.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(vo.DataCriacao));

			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("Plano");
			dt.Columns.Add("Idade");

            PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);

			DataRow drN = dt.NewRow();
			drN["Nome"] = benef.Nomusr;
			drN["Parentesco"] = PLocatorDataBO.Instance.GetParentesco(benef.Graupa);
			drN["Plano"] = PLocatorDataBO.Instance.GetProdutoSaude(vo.CodPlanoBeneficiario).Descri;
			if (!string.IsNullOrEmpty(benef.Datnas.Trim()))
				drN["Idade"] = DateUtil.CalculaIdade(DateTime.ParseExact(benef.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture));

			dt.Rows.Add(drN);

			repParams.DataSources.Add("DataSet1", dt);
			return repParams;
		}

		public string GerarNome() {
			if (vo.CodSolicitacao == Int32.MinValue)
				return "Declaracao_Universitario_BRANCO";
			return "Declaracao_Universitario_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptUniversitario";
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
