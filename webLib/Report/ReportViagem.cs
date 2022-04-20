using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Reporting;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVida.Web.Report {
	public class ReportViagem : ReportBase<SolicitacaoViagemVO, ReportViagemBinder> {
		public ReportViagem(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}
		protected override ReportViagemBinder CreateBinder(SolicitacaoViagemVO vo) {
			return new ReportViagemBinder(vo);
		}

		internal override SolicitacaoViagemVO GerarDados(System.Web.HttpRequest request) {
			int id;
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				SolicitacaoViagemVO vo = Buscar(id);
				return vo;
			}
		}
		private SolicitacaoViagemVO Buscar(int id) {
			SolicitacaoViagemVO vo = ViagemBO.Instance.GetById(id);
			return vo;
		}

	}
}
