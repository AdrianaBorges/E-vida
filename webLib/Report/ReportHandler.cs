using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eVida.Web;
using eVida.Web.Security;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;
using System.Configuration;
using eVidaGeneralLib.Util;
using eVida.Web.Report.CustomHandlers;

namespace eVida.Web.Report {
	public class ReportHandler : SkyReport.ReportingServices.ReportHandler {
		public enum EnumRelatorio {
			EXCLUSAO,
			SEGUNDA_VIA,
			MENSALIDADE_IR,
			REEMBOLSO_IR,
			NEGATIVA,
			SOL_RECIPROCIDADE,
			ENVIO_RECIPROCIDADE,
			FATURAMENTO_USUARIO,
			UNIVERSITARIO,
			ATESTADO_COMPARECIMENTO,
			AUTORIZACAO_PROVISORIA,
			PROTOCOLO_FATURA_CAPA,
			GERENCIAL_BILHETAGEM,
			VIAGEM,
			CARTA_POSITIVA_CRA,
			CONSULTA_PLANO_EXT
		}
		public static IEnumerable<T> GetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static string GetJsVarName(EnumRelatorio val) {
			return "RELATORIO_" + val;
		}

		protected override IReportBase GetReportClass(string report, HttpContext context) {
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			HttpSessionState session = context.Session;


			string reportDir = ParametroUtil.ReportRdlcFolder;
			if (string.IsNullOrEmpty(reportDir))
				reportDir = request.MapPath("~/rpts");

			IUsuarioLogado usuario = (IUsuarioLogado)session["USUARIO"];

			if (report.Equals("JAVASCRIPT", StringComparison.InvariantCultureIgnoreCase)) {
				return Javascript(reportDir, usuario);
			} else if (report.Equals("LIST_REPORTS", StringComparison.InvariantCultureIgnoreCase)) {
				return ListReports(reportDir, usuario);
			}
			EnumRelatorio tipoReport;
			if (!Enum.TryParse(report, out tipoReport)) {
				throw new InvalidOperationException("Tipo de relatório inválido");
			}

			switch (tipoReport) {
				// Comum
				case EnumRelatorio.EXCLUSAO: return new ReportExclusao(reportDir, usuario);
				case EnumRelatorio.SEGUNDA_VIA: return new Report2aViaCarteira(reportDir, usuario);
				case EnumRelatorio.UNIVERSITARIO: return new ReportUniversitario(reportDir, usuario);

				// Beneficiario
				case EnumRelatorio.MENSALIDADE_IR: return new ReportIrMensalidade(reportDir, usuario);
				case EnumRelatorio.REEMBOLSO_IR: return new ReportIrReembolso(reportDir, usuario);

				// Intranet
				case EnumRelatorio.NEGATIVA: return new ReportNegativa(reportDir, usuario);
				case EnumRelatorio.SOL_RECIPROCIDADE: return new ReportSolReciprocidade(reportDir, usuario);
				case EnumRelatorio.ENVIO_RECIPROCIDADE: return new ReportEnvioReciprocidade(reportDir, usuario);
				case EnumRelatorio.FATURAMENTO_USUARIO: return new ReportProducaoFaturamento(reportDir, usuario);
				case EnumRelatorio.ATESTADO_COMPARECIMENTO: return new ReportAtestadoComparecimento(reportDir, usuario);
				case EnumRelatorio.AUTORIZACAO_PROVISORIA: return new ReportAutorizacaoProvisoria(reportDir, usuario);
				case EnumRelatorio.PROTOCOLO_FATURA_CAPA: return new ReportProtocoloFaturaCapa(reportDir, usuario);
				case EnumRelatorio.GERENCIAL_BILHETAGEM: return new ReportBilhetagem(reportDir, usuario);
				case EnumRelatorio.VIAGEM: return new ReportViagem(reportDir, usuario);
				case EnumRelatorio.CARTA_POSITIVA_CRA: return new ReportCataPositivaCra(reportDir, usuario);

				// EXTERNO
				case EnumRelatorio.CONSULTA_PLANO_EXT: return new Externo.ReportConsultaPlanoExt(reportDir);

				default: throw new InvalidOperationException("Classe de relatório não suportado");
			}
		}

		private IReportBase Javascript(string reportDir, IUsuarioLogado usuario) {
			return new JsReport(reportDir, usuario);
		}


		private IReportBase ListReports(string reportDir, IUsuarioLogado usuario) {
			return new ListReports(reportDir, usuario);
		}
	}
}
