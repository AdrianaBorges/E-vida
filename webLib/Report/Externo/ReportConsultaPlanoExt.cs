using eVida.Web.Security;
using eVidaGeneralLib.Reporting.Externo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace eVida.Web.Report.Externo {
	public class ReportConsultaPlanoExt : ReportBase<ReportConsultaPlanoExtBinder.ParamsVO, ReportConsultaPlanoExtBinder> {

		public ReportConsultaPlanoExt(string reportDir)
			: base(reportDir, new UsuarioNoLoginRequiredVO()) {
		}

		public static void SaveDados(HttpRequest request, HttpSessionState session, ReportConsultaPlanoExtBinder.ParamsVO dados) {
			session["REL_CONSULTA_PLANO_EXT"] = dados;
		}

		internal override ReportConsultaPlanoExtBinder.ParamsVO GerarDados(HttpRequest request, HttpSessionState session) {
			ReportConsultaPlanoExtBinder.ParamsVO rd = session["REL_CONSULTA_PLANO_EXT"] as ReportConsultaPlanoExtBinder.ParamsVO;
			DataTable dt = null;
			if (rd != null) {
				dt = rd.Dados;
			}
			if (dt == null) {
				return null;
			}
			return rd;
		}

		internal override ReportConsultaPlanoExtBinder.ParamsVO GerarDados(HttpRequest request) {
			throw new NotImplementedException();
		}
		
		protected override ReportConsultaPlanoExtBinder CreateBinder(ReportConsultaPlanoExtBinder.ParamsVO vo) {
			return new ReportConsultaPlanoExtBinder(vo);
		}
	}
}
