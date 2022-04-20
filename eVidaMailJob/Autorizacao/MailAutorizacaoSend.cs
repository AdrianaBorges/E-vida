using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Filter;

namespace eVidaMailJob.Autorizacao {
	class MailAutorizacaoSend {
		const string NOVO_TOKEN = "NOVOS";
		const string ALERTA_TOKEN = "ALERTAS";
		const string FORA_TOKEN = "FORA";
		class ParametrosControle {
			public ParametrosControle() {
				Alertas = new SortedSet<int>();
				Fora = new SortedSet<int>();
				Novos = new SortedSet<int>();
			}
			public SortedSet<int> Alertas { get; private set; }
			public SortedSet<int> Fora { get; private set; }
			public SortedSet<int> Novos { get; private set; }
		}

		private EVidaLog log = new EVidaLog(typeof(MailAutorizacaoSend));
		private long idProcesso;
		private ParametrosControle parametros;
		public void Run() {
			log.Info("Run");
			idProcesso = 0;
			parametros = null;

			try {
				idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.AUTORIZACAO_ALERTA);
				log.Debug("Processo: " + idProcesso);
				int qtd = SendMail();
				log.Debug("QTD: " + qtd);
				ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.AUTORIZACAO_ALERTA, qtd, ToString(parametros));
			}
			catch (Exception ex) {
				log.Error("Erro ao processar", ex);
				if (idProcesso != 0)
					ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.AUTORIZACAO_ALERTA, ex);
			}
			log.Info("End");
		}

		private int SendMail() {
			DataTable dtPendente = AutorizacaoBO.Instance.BuscarEmAndamento();
			if (dtPendente == null)
				return 0;

			List<AutorizacaoVO> lstProtocoloNovo = new List<AutorizacaoVO>();
			List<AutorizacaoVO> lstProtocoloAlerta = new List<AutorizacaoVO>();
			List<AutorizacaoVO> lstProtocoloFora = new List<AutorizacaoVO>();
			
			foreach (DataRow dr in dtPendente.Rows) {
				AutorizacaoVO autVO = (AutorizacaoVO)dr["OBJ"];
				PrazoAutorizacaoVO prazo = (PrazoAutorizacaoVO)dr["PRAZO"];

				if (prazo.Prazo == PrazoAutorizacao.ALERTA)
					lstProtocoloAlerta.Add(autVO);
				else if (prazo.Prazo == PrazoAutorizacao.FORA_PRAZO)
					lstProtocoloFora.Add(autVO);

				if (autVO.Status == StatusAutorizacao.ENVIADA)
					lstProtocoloNovo.Add(autVO);
			}
			int count = lstProtocoloNovo.Count + lstProtocoloFora.Count + lstProtocoloAlerta.Count;
			if (count > 0) {
				if (CheckParametro(lstProtocoloNovo, lstProtocoloAlerta, lstProtocoloFora)) {
					AutorizacaoBO.Instance.EnviarEmailAlerta(lstProtocoloNovo, lstProtocoloAlerta, lstProtocoloFora);
				} else {
					log.Info("Parâmetros iguais!");
					return 0;
				}
			}
			return count;
		}

		private bool CheckParametro(List<AutorizacaoVO> lstProtocoloNovo, List<AutorizacaoVO> lstProtocoloAlerta, List<AutorizacaoVO> lstProtocoloFora) {
			parametros = new ParametrosControle(); 
			if (lstProtocoloNovo.Count > 0)
				parametros.Novos.UnionWith(lstProtocoloNovo.Select(x => x.Id));
			if (lstProtocoloAlerta.Count > 0)
				parametros.Alertas.UnionWith(lstProtocoloAlerta.Select(x => x.Id));
			if (lstProtocoloFora.Count > 0)
				parametros.Fora.UnionWith(lstProtocoloFora.Select(x => x.Id));

			ControleProcessoVO controle = ProcessoBO.Instance.GetAnteriorSucesso(ControleProcessoEnum.AUTORIZACAO_ALERTA);
			if (controle != null) {
				ParametrosControle oldParametros = Parse(controle.Adicional);
				if (SameParametros(parametros, oldParametros))
					return false;
			}
			return true;
		}

		private string[] ParseToken(string fullToken, string tokenName) {
			string str = fullToken.Substring(tokenName.Length + 2, fullToken.Length - (tokenName.Length + 2) - 1);
			string[] strIds = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			return strIds;
		}

		private ParametrosControle Parse(string infoAdicional) {
			//NOVOS=[123]$$ALERTAS=[]$$FORA=[145]
			if (string.IsNullOrEmpty(infoAdicional))
				return null;
			
			int idx = infoAdicional.IndexOf("$$");
			if (idx < 0)
				return null;
			string[] infoSplit = infoAdicional.Split(new string[] {"$$"}, StringSplitOptions.RemoveEmptyEntries);
			if (infoSplit == null || infoSplit.Length < 3)
				return null;

			string strNovos = infoSplit[0];			
			string strAlertas = infoSplit[1];
			string strFora = infoSplit[2];

			if (!strNovos.StartsWith(NOVO_TOKEN)) return null;
			if (!strAlertas.StartsWith(ALERTA_TOKEN)) return null;
			if (!strFora.StartsWith(FORA_TOKEN)) return null;

			string[] strIdNovos = ParseToken(strNovos, NOVO_TOKEN);
			string[] strIdAlertas = ParseToken(strAlertas, ALERTA_TOKEN);
			string[] strIdFora = ParseToken(strFora, FORA_TOKEN);

			ParametrosControle parametro = new ParametrosControle();
			foreach (string strId in strIdNovos) {
				parametro.Novos.Add(Int32.Parse(strId));
			}
			foreach (string strId in strIdAlertas) {
				parametro.Alertas.Add(Int32.Parse(strId));
			}
			foreach (string strId in strIdFora) {
				parametro.Fora.Add(Int32.Parse(strId));
			}
			return parametro;
		}

		private string ToString(ParametrosControle parametros) {
			StringBuilder sb = new StringBuilder();
			sb.Append(NOVO_TOKEN).Append("=[");
			if (parametros != null && parametros.Novos != null && parametros.Novos.Count > 0)
				sb.Append(parametros.Novos.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			sb.Append("$$");
			sb.Append(ALERTA_TOKEN).Append("=[");
			if (parametros != null && parametros.Alertas != null && parametros.Alertas.Count > 0)
				sb.Append(parametros.Alertas.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			sb.Append("$$");
			sb.Append(FORA_TOKEN).Append("=[");
			if (parametros != null && parametros.Fora != null && parametros.Fora.Count > 0)
				sb.Append(parametros.Fora.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
			sb.Append("]");
			return sb.ToString();
		}

		private bool SameParametros(ParametrosControle p1, ParametrosControle p2) {
			if (p1 == null) return false;
			if (p2 == null) return false;

			if (p1.Novos.Count != p2.Novos.Count) return false;
			if (p1.Alertas.Count != p2.Alertas.Count) return false;
			if (p1.Fora.Count != p2.Fora.Count) return false;

			foreach (int id in p1.Novos) {
				if (!p2.Novos.Contains(id))
					return false;
			}
			foreach (int id in p1.Alertas) {
				if (!p2.Alertas.Contains(id))
					return false;
			}
			foreach (int id in p1.Fora) {
				if (!p2.Fora.Contains(id))
					return false;
			}
			return true;
		}
	}
}
