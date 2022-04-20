using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Reporting;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eVida.Web.Report {

	public class ReportSolicitacaoCanalGestante : ReportBase<CanalGestanteVO, ReportSolicitacaoCanalGestanteBinder> {

		public ReportSolicitacaoCanalGestante(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override CanalGestanteVO GerarDados(HttpRequest request) {
			int id;
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				CanalGestanteVO vo = Buscar(id);
				return vo;
			}
		}

		private CanalGestanteVO Buscar(int id) {
			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(id);
			return vo;
		}


		protected override ReportSolicitacaoCanalGestanteBinder CreateBinder(CanalGestanteVO vo) {
			return new ReportSolicitacaoCanalGestanteBinder(vo);
		}
	}
}
