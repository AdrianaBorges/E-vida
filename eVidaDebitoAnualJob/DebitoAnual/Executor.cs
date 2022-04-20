using consoleLib.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaDebitoAnualJob.DebitoAnual {
	internal class Executor {
		const string OK_TOKEN = "OK";
		const string ERRO_TOKEN = "ERRO";

		private EVidaLog log = new EVidaLog(typeof(Executor));
		private long idProcesso;
		List<DeclaracaoAnualDebitoVO> lstExecOk;
		List<DeclaracaoAnualDebitoVO> lstExecErro;

		public void Run() {
			log.Info("Run");
			idProcesso = 0;

			try {
				List<DeclaracaoAnualDebitoVO> lstSolicitacoes = DeclaracaoAnualDebitoBO.Instance.ListarSolPendente(1000);
				if (lstSolicitacoes == null)
					lstSolicitacoes = new List<DeclaracaoAnualDebitoVO>();
				if (lstSolicitacoes.Count == 0) {
					log.Info("SEM SOLICITACOES PENDENTES");
					log.Info("End");
					return;
				}
				idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.DEBITO_ANUAL);
				log.Debug("Processo: " + idProcesso);
				int qtd = Executar(lstSolicitacoes);
				log.Debug("QTD: " + qtd);
				ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.DEBITO_ANUAL, qtd, BuildParamString());
			} catch (Exception ex) {
				log.Error("Erro ao processar", ex);
				if (idProcesso != 0)
					ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.DEBITO_ANUAL, ex);
			}
			log.Info("End");
		}
		
		
		public bool GerarPDF(DeclaracaoAnualDebitoVO vo) {
			try {	        
				ReportDeclaracaoAnualDebito rpt = new ReportDeclaracaoAnualDebito(ParametroUtil.ReportRdlcFolder);
				byte[] bytes = rpt.GerarNovoRelatorio(vo.CodBeneficiario, vo.AnoRef);

				DeclaracaoAnualDebitoBO.Instance.RegistarGerado(vo, bytes);

				return true;
			} catch (Exception ex) {
				DeclaracaoAnualDebitoBO.Instance.RegistrarErro(vo, ex);
				return false;
			}
			
		}


		private int Executar(List<DeclaracaoAnualDebitoVO> lstSolicitacoes) {
			if (lstSolicitacoes.Count == 0)
				return 0;

			int count = lstSolicitacoes.Count;
			lstExecOk = new List<DeclaracaoAnualDebitoVO>();
			lstExecErro = new List<DeclaracaoAnualDebitoVO>();
			
			foreach (DeclaracaoAnualDebitoVO vo in lstSolicitacoes) {
				bool ok = GerarPDF(vo);
				if (ok)
					lstExecOk.Add(vo);
				else
					lstExecErro.Add(vo);
			}

			return count;
		}
		private string BuildParamString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(OK_TOKEN).Append("=[");
			if (lstExecOk != null && lstExecOk.Count > 0)
				sb.Append(lstExecOk.Count);
				//sb.Append(lstExecOk.Select(x => x.AnoRef + "|" + x.CodBeneficiario).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			sb.Append("$$");
			sb.Append(ERRO_TOKEN).Append("=[");
			if (lstExecErro != null && lstExecErro.Count > 0)
				sb.Append(lstExecErro.Select(x => x.AnoRef + "|" + x.CodBeneficiario).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			return sb.ToString();
		}

	}
}
