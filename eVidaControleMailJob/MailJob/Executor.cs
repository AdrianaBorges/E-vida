using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaControleMailJob.MailJob {
	internal class Executor {
		const string OK_TOKEN = "OK";
		const string ERRO_TOKEN = "ERRO";

		private EVidaLog log = new EVidaLog(typeof(Executor));
		private long idProcesso;
		List<ControleEmailVO> lstExecOk;
		List<ControleEmailVO> lstExecErro;

		public void Run() {
			log.Info("Run");
			idProcesso = 0;

			try {
				List<ControleEmailVO> lstSolicitacoes = ControleEmailBO.Instance.ListarSolPendente(1000);
				if (lstSolicitacoes == null)
					lstSolicitacoes = new List<ControleEmailVO>();
				if (lstSolicitacoes.Count == 0) {
					log.Info("SEM SOLICITACOES PENDENTES");
					log.Info("End");
					return;
				}
				idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.CONTROLE_EMAIL);
				log.Debug("Processo: " + idProcesso);
				int qtd = Executar(lstSolicitacoes);
				log.Debug("QTD: " + qtd);
				ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.CONTROLE_EMAIL, qtd, BuildParamString());
			} catch (Exception ex) {
				log.Error("Erro ao processar", ex);
				if (idProcesso != 0)
					ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.CONTROLE_EMAIL, ex);
			}
			log.Info("End");
		}


		public bool EnviarEmail(ControleEmailVO vo) {
			try {
				ControleEmailBO.Instance.EnviarEmail(vo);

				return true;
			} catch (Exception ex) {
				ControleEmailBO.Instance.RegistrarErro(vo, ex);
				return false;
			}

		}


		private int Executar(List<ControleEmailVO> lstSolicitacoes) {
			if (lstSolicitacoes.Count == 0)
				return 0;

			int count = lstSolicitacoes.Count;
			lstExecOk = new List<ControleEmailVO>();
			lstExecErro = new List<ControleEmailVO>();

			foreach (ControleEmailVO vo in lstSolicitacoes) {
				bool ok = EnviarEmail(vo);
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
				sb.Append(lstExecErro.Select(x => x.Id.ToString()).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			return sb.ToString();
		}

	}
}
