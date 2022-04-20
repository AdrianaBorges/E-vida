using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ReportingServices;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	public class ReportSolReciprocidade : ReportBase<ReciprocidadeVO, ReportSolReciprocidadeBinder> {

		public ReportSolReciprocidade(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}
		
		internal override ReciprocidadeVO GerarDados(HttpRequest request) {
			int id;
			if ("true".Equals(request["GEN"], StringComparison.InvariantCultureIgnoreCase)) {
				return new ReciprocidadeVO() {
					CodSolicitacao = Int32.MinValue
				};
			}
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				ReciprocidadeVO vo = Buscar(id);
				return vo;
			}
		}

		private static ReciprocidadeVO Buscar(int id) {
			ReciprocidadeVO vo = ReciprocidadeBO.Instance.GetById(id);
			return vo;
		}

		protected override ReportSolReciprocidadeBinder CreateBinder(ReciprocidadeVO vo) {
			return new ReportSolReciprocidadeBinder(vo);
		}
	}
}