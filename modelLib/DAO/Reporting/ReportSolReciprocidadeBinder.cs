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
	public class ReportSolReciprocidadeBinder : IReportBinder {
		private ReciprocidadeVO vo;

		public ReportSolReciprocidadeBinder(ReciprocidadeVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			if (vo.CodSolicitacao == Int32.MinValue) {
				return GetBlank();
			}

			ReportBinderParams repParams = new ReportBinderParams();
            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(ReciprocidadeVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NomeTitular", titular.Nomusr);
			repParams.Params.Add("Cartao", titular.GetCarteira());
			repParams.Params.Add("Email", titular.Email);

			repParams.Params.Add("Local", vo.Local);
			repParams.Params.Add("DataCriacao", vo.DataCriacao.ToString("dd/MM/yyyy"));

			repParams.Params.Add("CEP", FormatUtil.FormatCep(vo.Endereco.Cep));
			repParams.Params.Add("Rua", vo.Endereco.Rua);
			repParams.Params.Add("NumEndereco", vo.Endereco.NumeroEndereco);
			repParams.Params.Add("Complemento", vo.Endereco.Complemento);
			repParams.Params.Add("Bairro", vo.Endereco.Bairro);
			repParams.Params.Add("Cidade", vo.Endereco.Cidade);
			repParams.Params.Add("UF", vo.Endereco.Uf);

			repParams.Params.Add("DataInicio", vo.Inicio.ToString("dd/MM/yyyy"));
			repParams.Params.Add("DataFim", vo.Fim.ToString("dd/MM/yyyy"));

			repParams.Params.Add("ProtocoloAns", vo.ProtocoloAns);

			repParams.DataSources.Add("dsBeneficiario", BuildTableBeneficiarios(vo));

			return repParams;
		}

		private ReportBinderParams GetBlank() {
			ReportBinderParams repParams = new ReportBinderParams();

			repParams.Params.Add("NumSolicitacao", "   ");

			repParams.Params.Add("NomeTitular", "   ");
			repParams.Params.Add("Cartao", "   ");
			repParams.Params.Add("Email", "   ");

			repParams.Params.Add("Local", "   ");
			repParams.Params.Add("DataCriacao", "   ");

			repParams.Params.Add("CEP", "   ");
			repParams.Params.Add("Rua", "   ");
			repParams.Params.Add("NumEndereco", "   ");
			repParams.Params.Add("Complemento", "   ");
			repParams.Params.Add("Bairro", "   ");
			repParams.Params.Add("Cidade", "   ");
			repParams.Params.Add("UF", "   ");

			repParams.Params.Add("DataInicio", "   ");
			repParams.Params.Add("DataFim", "   ");

			repParams.Params.Add("ProtocoloAns", "   ");

			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("NomeMae");
			dt.Columns.Add("CPF");
			dt.Columns.Add("Nascimento");
			for (int i = 0; i < 3; i++) {
				DataRow drN = dt.NewRow();
				dt.Rows.Add(drN);
			}

			repParams.DataSources.Add("dsBeneficiario", dt);
			return repParams;
		}

		public string GerarNome() {
			if (vo.CodSolicitacao == Int32.MinValue)
				return "SOL_REQ_RECIPROCIDADE_BRANCO";
			return "SOL_REQ_RECIPROCIDADE_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptSolReciprocidade";
		}

		private static DataTable BuscarBeneficiarios(ReciprocidadeVO vo) {
			return ReciprocidadeBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
		}

		private static DataTable BuildTableBeneficiarios(ReciprocidadeVO vo) {

			DataTable dtBeneficiarios = BuscarBeneficiarios(vo);
			DataView dv = new DataView(dtBeneficiarios);
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("Plano");
			dt.Columns.Add("CPF");
			dt.Columns.Add("Nascimento");

			foreach (DataRowView dr in dv) {
				DataRow drN = dt.NewRow();
				drN["Nome"] = Convert.ToString(dr["BA1_NOMUSR"]);
				drN["Parentesco"] = Convert.ToString(dr["BRP_DESCRI"]);
				drN["Plano"] = Convert.ToString(dr["BI3_DESCRI"]);
                drN["Nascimento"] = !string.IsNullOrEmpty(dr["BA1_DATNAS"].ToString().Trim()) ? DateTime.ParseExact(dr["BA1_DATNAS"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "-";
				drN["CPF"] = dr["BA1_CPFUSR"] != DBNull.Value ? FormatUtil.FormatCpf(Convert.ToString(dr["BA1_CPFUSR"])) : "";
				dt.Rows.Add(drN);
			}
			return dt;
		}

	}
}
