using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	public class ReportAutorizacaoProvisoria : ReportBase<AutorizacaoProvisoriaVO, ReportAutorizacaoProvisoriaBinder> {

		public ReportAutorizacaoProvisoria(String reportDir, IUsuarioLogado usuario) : base(reportDir, usuario) { }

		internal override AutorizacaoProvisoriaVO GerarDados(HttpRequest request) {
			int id;
			if ("true".Equals(request["GEN"], StringComparison.InvariantCultureIgnoreCase)) {
				return new AutorizacaoProvisoriaVO() {
					CodSolicitacao = Int32.MinValue
				};
			}
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				AutorizacaoProvisoriaVO vo = Buscar(id);
				return vo;
			}
		}
		
		private static AutorizacaoProvisoriaVO Buscar(int id) {
			AutorizacaoProvisoriaVO vo = AutorizacaoProvisoriaBO.Instance.GetById(id);
			return vo;
		}


		protected override ReportAutorizacaoProvisoriaBinder CreateBinder(AutorizacaoProvisoriaVO vo) {
			return new ReportAutorizacaoProvisoriaBinder(vo);
		}
	}
}