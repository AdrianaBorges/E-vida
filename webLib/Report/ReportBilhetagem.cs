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
using eVida.Web.Controls;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {

	public class ReportBilhetagem : ReportBase<ReportBilhetagemBinder.ParamsVO, ReportBilhetagemBinder> {


		public ReportBilhetagem(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		public static void SaveDados(HttpRequest request, HttpSessionState session, ReportBilhetagemBinder.ParamsVO dados) {
			session["REL_BILHETAGEM"] = dados;
		}

		internal override ReportBilhetagemBinder.ParamsVO GerarDados(HttpRequest request, HttpSessionState session) {
			DateTime dtInicio = DateTime.Parse("01/08/2013");
			DateTime dtFim = DateTime.Parse("31/08/2013");
			int hashCode = 0;

			string strInicio = request["dtInicio"];
			string strFim = request["dtFim"];
			string strHash = request["hc"];
			string strDirecao = request["dir"];
			if (string.IsNullOrEmpty(strInicio) || string.IsNullOrEmpty(strFim)
				|| string.IsNullOrEmpty(strHash)) {
				return null;
			} else {
				strInicio = strInicio.FromBase64String();
				strFim = strFim.FromBase64String();

				if (!DateTime.TryParse(strInicio, out dtInicio) || !DateTime.TryParse(strFim, out dtFim)
					|| !Int32.TryParse(strHash, out hashCode)) {
					return null;
				}
				if (!strDirecao.Equals("") && !strDirecao.Equals("R") && !strDirecao.Equals("O")) {
					return null;
				}

				ReportBilhetagemBinder.ParamsVO rd = session["REL_BILHETAGEM"] as ReportBilhetagemBinder.ParamsVO;
				DataTable dt = null;
				if (rd != null) {
					dt = rd.Dados;
				}
				
				if (dt == null || dt.GetHashCode() != hashCode) {
					return null;
				}
				dt = dt.Copy();

				return new ReportBilhetagemBinder.ParamsVO()
				{
					Dados = dt,
					Inicio = dtInicio,
					Fim = dtFim,
					Direcao = strDirecao
				};
			}
		}

		internal override ReportBilhetagemBinder.ParamsVO GerarDados(HttpRequest request) {
			throw new NotImplementedException();
		}

		protected override ReportBilhetagemBinder CreateBinder(ReportBilhetagemBinder.ParamsVO vo) {
			return new ReportBilhetagemBinder(vo);
		}
	}
}
