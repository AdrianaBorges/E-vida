using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaRotinaJob.Rotina {
	class ExecutorRotina {
		const string OK_TOKEN = "OK";
		const string ERRO_TOKEN = "ERRO";

		private EVidaLog log = new EVidaLog(typeof(ExecutorRotina));
		private long idProcesso;
		List<ExecucaoRotinaVO> lstExecOk;
		List<ExecucaoRotinaVO> lstExecErro;

		public void Run() {
			log.Info("Run");
			idProcesso = 0;

			try {
				if (!HasRotinas()) {
					log.Info("SEM ROTINAS");
					log.Info("End");
					return;
				}
				idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.ROTINA_BANCO);
				log.Debug("Processo: " + idProcesso);
				int qtd = ExecutarRotinas();
				log.Debug("QTD: " + qtd);
				ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.ROTINA_BANCO, qtd, BuildParamString());
			} catch (Exception ex) {
				log.Error("Erro ao processar", ex);
				if (idProcesso != 0)
					ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.ROTINA_BANCO, ex);
			}
			log.Info("End");
		}

		private bool HasRotinas() {
			List<ExecucaoRotinaVO> lstPendente = RotinaBO.Instance.ListarExecPendente();
			if (lstPendente == null || lstPendente.Count == 0)
				return false;
			return lstPendente.Count > 0;
		}

		private int ExecutarRotinas() {
			List<ExecucaoRotinaVO> lstPendente = RotinaBO.Instance.ListarExecPendente();
			if (lstPendente == null || lstPendente.Count == 0)
				return 0;

			int count = lstPendente.Count;
			if (count > 0) {
				lstExecOk = new List<ExecucaoRotinaVO>();
				lstExecErro = new List<ExecucaoRotinaVO>();
				RotinaBO.Instance.Executar(lstPendente, lstExecOk, lstExecErro);				
			}
			return count;
		}
		private string BuildParamString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(OK_TOKEN).Append("=[");
			if (lstExecOk != null && lstExecOk.Count > 0)
				sb.Append(lstExecOk.Select(x => x.Id.ToString()).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			sb.Append("$$");
			sb.Append(ERRO_TOKEN).Append("=[");
			if (lstExecErro != null && lstExecErro.Count > 0)
				sb.Append(lstExecErro.Select(x => x.Id.ToString()).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			return sb.ToString();
		}

	}
}
