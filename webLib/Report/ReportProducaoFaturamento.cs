using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using eVida.Web.Security;
using eVidaGeneralLib.Util;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {

	public class ReportProducaoFaturamento : ReportBase<ReportProducaoFaturamentoBinder.ParamsVO, ReportProducaoFaturamentoBinder> {

		public ReportProducaoFaturamento(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		public static void SaveDados(HttpRequest request, HttpSessionState session, ReportProducaoFaturamentoBinder.ParamsVO dados) {
			session["REL_PRODUCAO_FATURAMENTO"] = dados;
		}

		internal override ReportProducaoFaturamentoBinder.ParamsVO GerarDados(HttpRequest request, HttpSessionState session) {
			DateTime dtInicio = DateTime.Parse("01/08/2013");
			DateTime dtFim = DateTime.Parse("31/08/2013");
			int hashCode;

			string strInicio = request["dtInicio"];
			string strFim = request["dtFim"];
			string strHash = request["hc"];
			string strTipo = request["tr"];
			if (string.IsNullOrEmpty(strInicio) || string.IsNullOrEmpty(strFim)
				|| string.IsNullOrEmpty(strHash) || string.IsNullOrEmpty(strTipo)) {
				return null;
			} else {
				strInicio = strInicio.FromBase64String();
				strFim = strFim.FromBase64String();

				if (!DateTime.TryParse(strInicio, out dtInicio) || !DateTime.TryParse(strFim, out dtFim)
					|| !Int32.TryParse(strHash, out hashCode)) {
					return null;
				}
				if (!strTipo.Equals("ALL") && !strTipo.Equals("DIA") && !strTipo.Equals("PROTOCOLO")) {
					return null;
				}

				ReportProducaoFaturamentoBinder.ParamsVO rd = session["REL_PRODUCAO_FATURAMENTO"] as ReportProducaoFaturamentoBinder.ParamsVO;
				DataTable dt = null;

				if (rd != null) {
					dt = rd.Dados;
				}
				/*
				if (strTipo.Equals("PROTOCOLO")) {
					dt = session["RELATORIO_PROTOCOLO"] as DataTable;
				}
				*/
				if (dt == null || dt.GetHashCode() != hashCode) {
					return null;
				}
				dt = dt.Copy();

				return new ReportProducaoFaturamentoBinder.ParamsVO()
				{
					Dados = dt,
					Inicio = dtInicio,
					Fim = dtFim,
					Tipo = strTipo
				};
			}
		}

		internal override ReportProducaoFaturamentoBinder.ParamsVO GerarDados(HttpRequest request) {
			throw new NotImplementedException();
		}
		
		protected override ReportProducaoFaturamentoBinder CreateBinder(ReportProducaoFaturamentoBinder.ParamsVO vo) {
			return new ReportProducaoFaturamentoBinder(vo);
		}
	}
}
