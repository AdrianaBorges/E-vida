using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Reporting;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ReportingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eVida.Web.Report {

	public class ReportDeclaracaoAnualDebito : ReportBase<DeclaracaoAnualDebitoInfoVO, ReportDeclaracaoAnualDebitoBinder> {
		
		public ReportDeclaracaoAnualDebito(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override DeclaracaoAnualDebitoInfoVO GerarDados(HttpRequest request) {
			DeclaracaoAnualDebitoInfoVO info = new DeclaracaoAnualDebitoInfoVO();
			info.CdBeneficiario = Int32.Parse(request["CD_BENEFICIARIO"]);
			info.AnoRef = Int32.Parse(request["ANO"]);

			GetBinder(info);

			return info;
		}

		public byte[] GerarNovoRelatorio(int cdBeneficiario, int ano) {
			DeclaracaoAnualDebitoInfoVO info = new DeclaracaoAnualDebitoInfoVO();
			info.AnoRef = ano;
			info.CdBeneficiario = cdBeneficiario;

			DeclaracaoAnualDebitoBO.Instance.FillInfo(info);

			return GerarRelatorio(info);
		}


		protected override ReportDeclaracaoAnualDebitoBinder CreateBinder(DeclaracaoAnualDebitoInfoVO vo) {
			return new ReportDeclaracaoAnualDebitoBinder(vo);
		}
	}
}
