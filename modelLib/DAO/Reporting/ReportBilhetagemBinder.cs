using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	[Serializable]
	public class ReportBilhetagemBinder :IReportBinder {

		[Serializable]
		public class ParamsVO {
			public DataTable Dados { get; set; }
			public DateTime Inicio { get; set; }
			public DateTime Fim { get; set; }
			public string Direcao { get; set; }
		}

		private ParamsVO vo;
		public ReportBilhetagemBinder(ParamsVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

			DataTable dt = vo.Dados;

			DataTable dtReport = new DataTable();

			dtReport.Columns.Add(new DataColumn("DsRamal"));/*
			dtReport.Columns.Add(new DataColumn("atendidas"));
			dtReport.Columns.Add(new DataColumn("desviadas"));
			dtReport.Columns.Add(new DataColumn("ocupadas"));
			dtReport.Columns.Add(new DataColumn("sem resposta"));
			dtReport.Columns.Add(new DataColumn("transferida"));*/
			dtReport.Columns.Add(new DataColumn("Estado"));


			foreach (DataRow dr in dt.Rows) {
				int? idRamalOrigem = dr.IsNull("RORIGEM_NR_RAMAL") ? new int?() : Convert.ToInt32(dr["RORIGEM_NR_RAMAL"]);
				int? idRamalDestino = dr.IsNull("RDESTINO_NR_RAMAL") ? new int?() : Convert.ToInt32(dr["RDESTINO_NR_RAMAL"]);

				if (idRamalOrigem != null) {
					if (vo.Direcao.Equals("") || vo.Direcao.Equals("O")) {
						DataRow drReport = dtReport.NewRow();
						drReport["DsRamal"] = dr["RORIGEM_DS_RAMAL"];
						drReport["Estado"] = dr["ds_estado"];
						dtReport.Rows.Add(drReport);
					}
				}
				if (idRamalDestino != null) {
					if (vo.Direcao.Equals("") || vo.Direcao.Equals("R")) {
						DataRow drReport = dtReport.NewRow();
						drReport["DsRamal"] = dr["RDESTINO_DS_RAMAL"];
						drReport["Estado"] = dr["ds_estado"];
						dtReport.Rows.Add(drReport);
					}
				}

			}


			repParams.Params.Add("dtInicio", vo.Inicio.ToString("dd/MM/yyyy"));
			repParams.Params.Add("dtFim", vo.Fim.ToString("dd/MM/yyyy"));
			repParams.Params.Add("tipoChamadas", vo.Direcao.Equals("") ? "AMBAS" : vo.Direcao.Equals("O") ? "ORIGINADAS" : "RECEBIDAS");

			repParams.DataSources.Add("dsBilhetagem", dtReport);

			return repParams;
		}

		public string GerarNome() {
			string nome = null;

			if (vo.Direcao.Equals("")) {
				nome = "AMBOS";
			} else if (vo.Direcao.Equals("R")) {
				nome = "RECEBIDAS";
			} else if (vo.Direcao.Equals("O")) {
				nome = "ORIGINADAS";
			} else {
				nome = "I";
			}

			nome += "_" + vo.Inicio.ToString("ddMMyyyy") + "_" + vo.Fim.ToString("ddMMyyyy");
			return nome;
		}

		public string DefaultRpt() {
			return "rptBilhetagem";
		}
	}
}
