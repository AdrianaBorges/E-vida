using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Reporting;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eVida.Web.Report {
	public class ReportCataPositivaCra : ReportBase<CartaPositivaCraVO, ReportCartaPositivaCraBinder> {

		public ReportCataPositivaCra(String reportDir, IUsuarioLogado usuario) : base(reportDir, usuario) { }

		internal override CartaPositivaCraVO GerarDados(HttpRequest request) {
			int id;
			
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				CartaPositivaCraVO vo = Buscar(id);
				return vo;
			}
		}

		private static CartaPositivaCraVO Buscar(int id) {
			CartaPositivaCraVO vo = CartaPositivaCraBO.Instance.GetById(id);
			return vo;
		}


		protected override ReportCartaPositivaCraBinder CreateBinder(CartaPositivaCraVO vo) {
			return new ReportCartaPositivaCraBinder(vo);
		}
	}
}
