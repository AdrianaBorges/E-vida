using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportProducaoFaturamentoBinder : IReportBinder {
		[Serializable]
		public class ParamsVO {
			public DataTable Dados { get; set; }
			public DateTime Inicio { get; set; }
			public DateTime Fim { get; set; }
			public string Tipo { get; set; }
		}

		private ParamsVO vo;
		public ReportProducaoFaturamentoBinder(ParamsVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

			DataTable dt = vo.Dados;

			dt.Columns["qtd_item"].ColumnName = "Guias";
			dt.Columns["tp_sistema_atend"].ColumnName = "Tipo";
			dt.Columns["user_update"].ColumnName = "Usuario";
			dt.Columns["nm_usuario"].ColumnName = "Nome";
			dt.Columns["tp_origem"].ColumnName = "Origem";

			if (!vo.Tipo.Equals("PROTOCOLO"))
				dt.Columns["date_update"].ColumnName = "Data";

			repParams.Params.Add("dtInicio", vo.Inicio.ToString("dd/MM/yyyy"));
			repParams.Params.Add("dtFim", vo.Fim.ToString("dd/MM/yyyy"));

			repParams.DataSources.Add("dsFaturamento", dt);
			return repParams;
		}

		public string GerarNome() {
			string nome = null;

			if (vo.Tipo.Equals("ALL")) {
				nome = "PRODUCAO";
			} else if (vo.Tipo.Equals("DIA")) {
				nome = "PROD_USUARIO_DIA";
			} else {
				nome = "PROTOCOLOS";
			}

			nome += "_" + vo.Inicio.ToString("ddMMyyyy") + "_" + vo.Fim.ToString("ddMMyyyy");
			return nome;
		}

		public string DefaultRpt() {
			return vo.Tipo.Equals("DIA") ? "rptProducaoUsuarioDia" : "rptProducaoFaturamento";
		}
	}
}
