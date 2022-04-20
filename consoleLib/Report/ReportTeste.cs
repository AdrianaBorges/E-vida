using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using SkyReport.ReportingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVida.Console.Report {
	public class ReportTeste : ReportBase<ReportTeste.ParamsVO> {

		public class ParamsVO {
			public long CdFuncionario { get; set; }
			public int AnoRef { get; set; }
		}

		public ReportTeste(string reportDir)
			: base(reportDir) {
		}

		protected override string GerarNome(ParamsVO vo) {
			return "TESTE" + vo.CdFuncionario + "_" + vo.AnoRef;
		}

		protected override string DefaultRpt(ParamsVO vo) {
			return "rptTeste";
		}

		protected override void FillReport(ParamsVO vo, ReportParams repParam, RelatorioHelper helper) {
			helper.EnableExternalImages = true;

			repParam.Params.Add("NumSolicitacao", "123123");
			repParam.Params.Add("Local", "Teste");			
			repParam.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(DateTime.Now));
			repParam.Params.Add("UsuarioAprov", "LUCAS DANIEL OTTONI");
			repParam.Params.Add("CargoAprov", "TESTE");
			string assinatura = UsuarioBO.Instance.GetAssinatura(1);
			if (!string.IsNullOrEmpty(assinatura)) {
				Uri pathAsUri = new Uri(assinatura);
				repParam.Params.Add("ImgAssinaturaPath", pathAsUri.AbsolutePath);
			} else {
				repParam.Params.Add("ImgAssinaturaPath", "-");
			}

			repParam.Params.Add("Conteudo", "<b>OK</b>Teste do template de pdf<br>Segunda linha<br>Terceira Linha<br>");			
		}
	}
	
}
