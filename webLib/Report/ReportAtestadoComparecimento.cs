using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	public class ReportAtestadoComparecimento : ReportBase<AtestadoComparecimentoVO, ReportAtestadoComparecimentoBinder> {
		public ReportAtestadoComparecimento(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override AtestadoComparecimentoVO GerarDados(HttpRequest request) {
			int id;
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				AtestadoComparecimentoVO vo = Buscar(id);
				return vo;
			}
		}

		private AtestadoComparecimentoVO Buscar(int id) {
			AtestadoComparecimentoVO vo = AtestadoComparecimentoBO.Instance.GetById(id);
			return vo;
		}

		protected override ReportAtestadoComparecimentoBinder CreateBinder(AtestadoComparecimentoVO vo) {
			return new ReportAtestadoComparecimentoBinder(vo);
		}
	}
}
