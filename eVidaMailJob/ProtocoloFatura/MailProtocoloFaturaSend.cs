using eVida.Console.Util;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaMailJob.ProtocoloFatura {
	class MailProtocoloFaturaSend {
		const string ALERTA_TOKEN = "ALERTAS";

		class ParametrosControle {
			private SortedSet<int> _alertas;
			public ParametrosControle() {
				_alertas = new SortedSet<int>();
			}
			public IEnumerable<int> Alertas {
				get {
					return _alertas.ToList().AsReadOnly();
				}
				set {
					_alertas.Clear();
					if (value != null && value.Count() > 0)
						value.ToList().ForEach(x => _alertas.Add(x));
				}
			}
			public DateTime DateRef { get; set; }
			public int DateCount { get; set; }

		}

		class MapProtocolo {
			public UsuarioVO Usuario { get; set; }
			public List<ProtocoloFaturaVO> Protocolos { get; set; }
		}

		private EVidaLog log = new EVidaLog(typeof(MailProtocoloFaturaSend));
		private long idProcesso;
		private ParametrosControle parametros;

		public void Run() {
			log.Info("Run");
			idProcesso = 0;
			parametros = null;

			try {
				idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.PROTOCOLO_FATURA_ALERTA);
				log.Debug("Processo: " + idProcesso);
				int qtd = SendMail();
				log.Debug("QTD: " + qtd);
				ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.PROTOCOLO_FATURA_ALERTA, qtd, ToString(parametros));
			} catch (Exception ex) {
				log.Error("Erro ao processar", ex);
				if (idProcesso != 0)
					ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.PROTOCOLO_FATURA_ALERTA, ex);
			}
			log.Info("End");
		}

		private int SendMail() {
			List<ProtocoloFaturaVO> lstPendentes = ProtocoloFaturaBO.Instance.FindAllActiveNotify();
			if (lstPendentes == null)
				return 0;

			int count = lstPendentes.Count;
			if (count > 0) {
				if (!CheckParametro(lstPendentes)) {
					log.Info("Parâmetros iguais!");
					return 0;
				}
				Dictionary<int, MapProtocolo> mapProtocolo = BuildMaps(lstPendentes);
				foreach (int idUsuario in mapProtocolo.Keys) {
					MapProtocolo item = mapProtocolo[idUsuario];
					UsuarioVO usuario = item.Usuario;
					List<ProtocoloFaturaVO> lst = item.Protocolos;
					try {
						ProtocoloFaturaBO.Instance.EnviarEmailAlerta(usuario, lst);	
					} catch (Exception ex) {
						log.Error("Erro ao enviar alertas ao usuário " + usuario.Login, ex);
					}
					
				}
				
			}
			return count;
		}

		private Dictionary<int, MapProtocolo> BuildMaps(List<ProtocoloFaturaVO> lstPendentes) {
			List<UsuarioVO> lstUsuarios = UsuarioBO.Instance.ListarUsuarios();
			Dictionary<int, MapProtocolo> mapProtocolo = new Dictionary<int, MapProtocolo>();
			foreach (ProtocoloFaturaVO protocolo in lstPendentes) {
				if (protocolo.CdUsuarioResponsavel != null) {
					MapProtocolo item = null;
					int idUsuario = protocolo.CdUsuarioResponsavel.Value;
					if (mapProtocolo.ContainsKey(idUsuario)) {
						item = mapProtocolo[idUsuario];
					} else {
						UsuarioVO usuario = lstUsuarios.Find(x => x.Id == idUsuario);
						item = new MapProtocolo();
						item.Usuario = usuario;
						item.Protocolos = new List<ProtocoloFaturaVO>();
						mapProtocolo.Add(idUsuario,item);
					}
					item.Protocolos.Add(protocolo);
				}
			}
			return mapProtocolo;
		}

		private bool CheckParametro(List<ProtocoloFaturaVO> lstProtocoloAlerta) {
			parametros = new ParametrosControle();
			if (lstProtocoloAlerta.Count > 0)
				parametros.Alertas = lstProtocoloAlerta.Select(x => x.Id).ToList();
			parametros.DateRef = DateTime.Now;
			ControleProcessoVO controle = ProcessoBO.Instance.GetAnteriorSucesso(ControleProcessoEnum.PROTOCOLO_FATURA_ALERTA);
			if (controle != null) {
				ParametrosControle parametrosOld = Parse(controle.Adicional);
				if (parametrosOld != null) {
					if (parametrosOld.DateRef.Date.Equals(parametros.DateRef.Date)) {
						if (ToString(parametros.Alertas).Equals(ToString(parametrosOld.Alertas))) {

							if (parametrosOld.DateRef.AddHours(4) > parametros.DateRef) {
								parametros.DateRef = parametrosOld.DateRef;
								return false;
							}
							parametros.DateCount = parametrosOld.DateCount + 1;
						}
					}
				}
			}
			return parametros.DateCount < 2;
		}

		private ParametrosControle Parse(string infoAdicional) {
			//NOVOS=[123]$$ALERTAS=[]$$FORA=[145]
			if (string.IsNullOrEmpty(infoAdicional))
				return null;
			
			if (infoAdicional != null) {
				ParametrosControle retorno = JSUtil.Deserialize<ParametrosControle>(infoAdicional);
				return retorno;
			}
			return null;
		}

		private string[] ParseToken(string fullToken, string tokenName) {
			string str = fullToken.Substring(tokenName.Length + 2, fullToken.Length - (tokenName.Length + 2) - 1);
			string[] strIds = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			return strIds;
		}

		private string ToString(ParametrosControle parametros) {
			string str = JSUtil.Serialize(parametros);
			return str;
		}

		private string ToString(IEnumerable<int> lst) {
			return lst.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y);
		}
	}
}
