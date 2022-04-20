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
	public class ReportUniversitario : ReportBase<DeclaracaoUniversitarioVO, ReportUniversitarioBinder> {

		public ReportUniversitario(string reportDir, IUsuarioLogado usuario)
			: base(reportDir, usuario) {
		}

		internal override DeclaracaoUniversitarioVO GerarDados(HttpRequest request) {
			int id; 
			if ("true".Equals(request["GEN"], StringComparison.InvariantCultureIgnoreCase)) {
				return new DeclaracaoUniversitarioVO() {
					CodSolicitacao = Int32.MinValue
				};
			}
			if (request["ID"] == null || !Int32.TryParse(request["ID"], out id))
				return null;
			else {
				DeclaracaoUniversitarioVO vo = Buscar(id);

				UsuarioBeneficiarioVO uVO = Usuario as UsuarioBeneficiarioVO;
				//Se a requisição foi pelo beneficiários, então verificar a matrícula
				if (uVO != null) {
                    if (vo.Codint.Trim() != uVO.Codint.Trim() || vo.Codemp.Trim() != uVO.Codemp.Trim() || vo.Matric.Trim() != uVO.Matric.Trim())
						return null;
				}
				return vo;
			}
		}

		private DeclaracaoUniversitarioVO Buscar(int id) {
			DeclaracaoUniversitarioVO vo = DeclaracaoUniversitarioBO.Instance.GetById(id);
			return vo;
		}
		
		protected override ReportUniversitarioBinder CreateBinder(DeclaracaoUniversitarioVO vo) {
			return new ReportUniversitarioBinder(vo);
		}
	}
}