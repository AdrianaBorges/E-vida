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
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {
	public class Report2aViaCarteira : ReportBase<SolicitacaoSegViaCarteiraVO, Report2aViaCarteiraBinder> {
		public Report2aViaCarteira(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}
		
		private static SolicitacaoSegViaCarteiraVO Buscar(int id) {
			SolicitacaoSegViaCarteiraVO vo = SegViaCarteiraBO.Instance.GetById(id);
			return vo;
		}

		protected override Report2aViaCarteiraBinder CreateBinder(SolicitacaoSegViaCarteiraVO vo) {
			return new Report2aViaCarteiraBinder(vo);
		}

		internal override SolicitacaoSegViaCarteiraVO GerarDados(HttpRequest request) {
			int id;
			if ("true".Equals(request["GEN"], StringComparison.InvariantCultureIgnoreCase)) {
				return new SolicitacaoSegViaCarteiraVO() {
					CdSolicitacao = Int32.MinValue,
					ProtocoloAns = "-"
				};
			}
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				SolicitacaoSegViaCarteiraVO vo = Buscar(id);
				
				//Se a requisição foi pelo beneficiários, então verificar a matrícula
                if (!CheckUsuarioBeneficiario(vo.Codint.Trim(), vo.Codemp.Trim(), vo.Matric.Trim()))
                {
					return null;
				}
				
				return vo;
			}
		}


	}
}