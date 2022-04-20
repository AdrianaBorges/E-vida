using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	public class ReportExclusao : ReportBase<ExclusaoVO, ReportExclusaoBinder> {
		
		public ReportExclusao(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}
		internal override ExclusaoVO GerarDados(HttpRequest request) {
			int id;
			if ("true".Equals(request["GEN"], StringComparison.InvariantCultureIgnoreCase)) {
				return new ExclusaoVO() {
					CodSolicitacao = Int32.MinValue
				};
			}
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				ExclusaoVO vo = Buscar(id);
				return vo;
			}
		}

		private ExclusaoVO Buscar(int id) {
			ExclusaoVO vo = FormExclusaoBO.Instance.GetById(id);
			return vo;
		}

		protected override ReportExclusaoBinder CreateBinder(ExclusaoVO vo) {
			return new ReportExclusaoBinder(vo);
		}
	}
}