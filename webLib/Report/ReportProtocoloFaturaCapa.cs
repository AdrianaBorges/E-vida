using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Reporting;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eVida.Web.Report {
	public class ReportProtocoloFaturaCapa : ReportBase<ProtocoloFaturaVO, ReportProtocoloFaturaCapaBinder> {

		public ReportProtocoloFaturaCapa(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override ProtocoloFaturaVO GerarDados(HttpRequest request) {
			int id;
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				ProtocoloFaturaVO vo = Buscar(id);

				UsuarioIntranetVO uVO = Usuario as UsuarioIntranetVO;
				return vo;
			}
		}

		private ProtocoloFaturaVO Buscar(int id) {
			ProtocoloFaturaVO vo = ProtocoloFaturaBO.Instance.GetById(id);
			return vo;
		}

		protected override ReportProtocoloFaturaCapaBinder CreateBinder(ProtocoloFaturaVO vo) {
			return new ReportProtocoloFaturaCapaBinder(vo);
		}
	}
}

