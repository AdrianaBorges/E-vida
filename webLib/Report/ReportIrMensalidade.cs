using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report {

	public class ReportIrMensalidade : ReportBase<ReportIrMensalidadeBinder.ParamsVO, ReportIrMensalidadeBinder> {

		public ReportIrMensalidade(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override ReportIrMensalidadeBinder.ParamsVO GerarDados(HttpRequest request) {
			int ano;
			if (request["ANO"] == null || !Int32.TryParse(request["ANO"], out ano))
				return null;
			else {
				string cartaoTitular;
				UsuarioBeneficiarioVO uVO = Usuario as UsuarioBeneficiarioVO;
				//Se a requisição nao foi pelo beneficiários, então deve ter o parametro da matrícula
				if (uVO == null) {
					cartaoTitular = request["CARTAO_TITULAR"];
					if (string.IsNullOrEmpty(cartaoTitular))
						return null;
				} else {
					cartaoTitular = uVO.Usuario.Matant;
				}
				return new ReportIrMensalidadeBinder.ParamsVO() 
				{
					AnoRef = ano,
					CartaoTitular = cartaoTitular
				};
			}
		}

		protected override ReportIrMensalidadeBinder CreateBinder(ReportIrMensalidadeBinder.ParamsVO vo) {
			return new ReportIrMensalidadeBinder(vo);
		}
	}
}