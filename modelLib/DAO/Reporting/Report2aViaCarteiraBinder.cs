using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System.Data;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;

namespace eVidaGeneralLib.Reporting {
	public class Report2aViaCarteiraBinder : IReportBinder {
		private SolicitacaoSegViaCarteiraVO vo;

		public Report2aViaCarteiraBinder(SolicitacaoSegViaCarteiraVO vo) {
			this.vo = vo;
		}

		public string GerarNome() {
			if (vo.CdSolicitacao == Int32.MinValue)
				return "SEGUNDA_VIA_BRANCO";
			return "SEGUNDA_VIA_" + vo.CdSolicitacao;
		}

		public string DefaultRpt() {
			return "rpt2aViaCarteira";
		}

		public ReportBinderParams GetData() {
			if (vo.CdSolicitacao == Int32.MinValue) {
				return GetBlank();
			}
			ReportBinderParams repParams = new ReportBinderParams();

			PUsuarioVO titularVO = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			repParams.Params.Add("NumSolicitacao", vo.CdSolicitacao.ToString("0000000000"));
			repParams.Params.Add("NomeTitular", titularVO.Nomusr);
			repParams.Params.Add("Cartao", titularVO.Matant);
			repParams.Params.Add("Email", titularVO.Email);
			repParams.Params.Add("Local", vo.Local);
			repParams.Params.Add("DataCriacao", vo.Criacao.ToString("dd \\de MMMM \\de yyyy"));
			repParams.Params.Add("ProtocoloAns", vo.ProtocoloAns);

			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("Plano");
			dt.Columns.Add("Motivo");

			DataTable dtBeneficiarios = BuscarBeneficiarios(vo);
			foreach (DataRow dr in dtBeneficiarios.Rows) {
				DataRow drN = dt.NewRow();
				drN["Nome"] = Convert.ToString(dr["NM_BENEFICIARIO"]);
                drN["Parentesco"] = Convert.ToString(dr["BRP_DESCRI"]);

                drN["Plano"] = Convert.ToString(dr["BI3_DESCRI"]);

				MotivoSegVia motivo = (MotivoSegVia)Convert.ToChar(dr["TP_MOTIVO"]);
				string dsMotivo = SolicitacaoSegViaCarteiraEnumTradutor.TraduzMotivo(motivo);
				drN["Motivo"] = dsMotivo;
				dt.Rows.Add(drN);
			}

			repParams.DataSources.Add("DataSet1", dt);
			return repParams;
		}

		private ReportBinderParams GetBlank() {
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.Params.Add("NumSolicitacao", "   ");

			repParams.Params.Add("NomeBeneficiario", " ");
			repParams.Params.Add("NomeTitular", " ");
			repParams.Params.Add("Matricula", " ");
			repParams.Params.Add("Email", " ");
			repParams.Params.Add("DataCriacao", " ");
			repParams.Params.Add("Local", " ");
			repParams.Params.Add("ProtocoloAns", " ");

			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("Plano");
			dt.Columns.Add("Motivo");
			for (int i = 0; i < 3; ++i) {
				DataRow drN = dt.NewRow();
				dt.Rows.Add(drN);
			}

			repParams.DataSources.Add("DataSet1", dt);
			return repParams;
		}

		private static DataTable BuscarBeneficiarios(SolicitacaoSegViaCarteiraVO vo) {
			return SegViaCarteiraBO.Instance.BuscarBeneficiarios(vo.CdSolicitacao);
		}

	}
}
