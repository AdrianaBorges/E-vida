using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public enum StatusAutorizacao {
		ENVIADA = 0,
		EM_ANALISE = 1,
		COTANDO_OPME = 2,
		SOLICITANDO_DOC = 3,
		APROVADA = 4,
		CANCELADA = 5,
		NEGADA = 6,
		ENVIADO_AUDITORIA = 7,
		ANALISADO_AUDITORIA =  8,
		REVALIDACAO = 9
	}

	public enum OrigemAutorizacao {
		CRED,
		BENEF,
		GESTOR
	}

	public enum CaraterAutorizacao {
		ELETIVA,
		URGENCIA
	}

	public enum PrazoAutorizacao {
		DENTRO_PRAZO,
		ALERTA,
		FORA_PRAZO
	}

	public enum TipoAutorizacao {
		MEDICA,
		ODONTO
	}

	public class AutorizacaoTradutorHelper {
		public const string CANCELADO_BENEFICIARIO = "EXCLUIDO PELO BENEFICIARIO";
		public const string CANCELADO_CREDENCIADO = "EXCLUIDO PELO CREDENCIADO";

		public static string TraduzStatus(StatusAutorizacao status) {
			switch (status) {
				case StatusAutorizacao.ENVIADA: return "ENVIADA";
				case StatusAutorizacao.EM_ANALISE: return "EM ANÁLISE";
				case StatusAutorizacao.COTANDO_OPME: return "COTANDO OPME";
				case StatusAutorizacao.SOLICITANDO_DOC: return "SOLICITANDO DOCUMENTO E/OU INFORMAÇÃO ADICIONAL";
				case StatusAutorizacao.APROVADA:
				case StatusAutorizacao.CANCELADA:
				case StatusAutorizacao.NEGADA:
					return status.ToString();
				case StatusAutorizacao.ENVIADO_AUDITORIA: return "ENVIADO PARA AUDITORIA";
				case StatusAutorizacao.ANALISADO_AUDITORIA: return "ANALISADO PELA AUDITORIA";
				case StatusAutorizacao.REVALIDACAO: return "EM REVALIDAÇÃO";
				default:
					break;
			}
			return status.ToString();
		}

		public static string TraduzOrigem(OrigemAutorizacao origem) {
			return origem.ToString();
		}

		public static string TraduzCarater(CaraterAutorizacao carater) {
			return carater.ToString();
		}

		public static string TraduzTipo(TipoAutorizacao tipo) {
			return tipo.ToString();
		}

		public static string TraduzPrazo(PrazoAutorizacao prazo) {
			switch (prazo) {
				case PrazoAutorizacao.DENTRO_PRAZO: return "Dentro do prazo";
				case PrazoAutorizacao.ALERTA: return "Alerta";
				case PrazoAutorizacao.FORA_PRAZO: return "Fora do prazo";
				default:
					break;
			}
			return prazo.ToString();
		}

		public static OrigemAutorizacao TraduzOrigem(string value) {
			if (OrigemAutorizacao.CRED.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
				return OrigemAutorizacao.CRED;
			if (OrigemAutorizacao.BENEF.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
				return OrigemAutorizacao.BENEF;
			return OrigemAutorizacao.GESTOR;
		}

		public static CaraterAutorizacao TraduzCarater(string value) {
			if (CaraterAutorizacao.ELETIVA.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
				return CaraterAutorizacao.ELETIVA;
			if (CaraterAutorizacao.URGENCIA.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
				return CaraterAutorizacao.URGENCIA;
			throw new ArgumentOutOfRangeException("value", value);
		}

		public static TipoAutorizacao TraduzTipo(string value) {
			if (TipoAutorizacao.MEDICA.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
				return TipoAutorizacao.MEDICA;
			if (TipoAutorizacao.ODONTO.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
				return TipoAutorizacao.ODONTO;
			throw new ArgumentOutOfRangeException("value", value);
		}

		public static bool NecessitaCalculoPrazo(StatusAutorizacao status) {
			return status == StatusAutorizacao.ENVIADA || status == StatusAutorizacao.COTANDO_OPME || status == StatusAutorizacao.REVALIDACAO
				|| status == StatusAutorizacao.EM_ANALISE || status == StatusAutorizacao.ENVIADO_AUDITORIA;
		}

		public static bool IsStatusFim(StatusAutorizacao status) {
			return status == StatusAutorizacao.APROVADA || status == StatusAutorizacao.CANCELADA
				|| status == StatusAutorizacao.NEGADA;
		}
	}

	[Serializable]
	public class PrazoAutorizacaoVO : IComparable {
		public PrazoAutorizacao Prazo { get; set; }
		public double Horas { get; set; }
		public double Diff { get; set; }

		public int CompareTo(object obj) {
			PrazoAutorizacaoVO other = (PrazoAutorizacaoVO)obj;
			return this.Horas.CompareTo(other.Horas);
		}
	}
	
	[Serializable]
	public class AutorizacaoVO : IComparable {
		public const string FORMATO_PROTOCOLO = "000000000";
		
		public int Id { get; set; }
		public DateTime DataSolicitacao { get; set; }
		public DateTime? DataAutorizacao { get; set; }
        public Protheus.PUsuarioVO Usuario { get; set; }
        public Protheus.PUsuarioVO UsuarioTitular { get; set; }
        public Protheus.PProdutoSaudeVO ProdutoSaude { get; set; }

        public OrigemAutorizacao Origem { get; set; }
        public Protheus.PRedeAtendimentoVO RedeAtendimento { get; set; }
        public Protheus.PProfissionalSaudeVO Profissional { get; set; }

        public CaraterAutorizacao? Carater { get; set; }
		public string CodDoenca { get; set; }
		public string IndicacaoClinica { get; set; }
		public bool Internacao { get; set; }
		public DateTime? DataInternacao { get; set; }
        public Protheus.PRedeAtendimentoVO Hospital { get; set; }
        public bool? Tfd { get; set; }
        public DateTime? DataInicioTfd { get; set; }
        public DateTime? DataTerminoTfd { get; set; }
		public string Obs { get; set; }
		public StatusAutorizacao Status { get; set; }
		public DateTime DataStatus { get; set; }
		public string MotivoCancelamento { get; set; }
		public int? CodNegativa { get; set; }
		public int? CodUsuarioAlteracao { get; set; }
		public int? CodUsuarioCriacao { get; set; }
		public int? CodUsuarioResponsavel { get; set; }
		public bool Opme { get; set; }
		public double HorasPrazo { get; set; }
		public TipoAutorizacao Tipo { get; set; }
		public DateTime DataAlteracao { get; set; }
		public OrigemAutorizacao OrigemAlteracao { get; set; }
		
		public string ComentarioAuditor { get; set; }
		public string ObsAprovacao { get; set; }

		public DateTime? DataSolRevalidacao;
		public DateTime? DataAprovRevalidacao;

		public string ProtocoloAns { get; set; }

		public int CompareTo(object obj) {
			return this.Id.CompareTo(((AutorizacaoVO)obj).Id);
		}
	}

	[Serializable]
	public class AutorizacaoStatusVO {
		public int CodAutorizacao { get; set; }
		public DateTime Data { get; set; }
		public StatusAutorizacao Status { get; set; }
		public int? CodUsuario { get; set; }
	}

	[Serializable]
	public class AutorizacaoSolDocVO {
		public int CodAutorizacao { get; set; }
		public DateTime Data { get; set; }
		public string MensagemSolDoc { get; set; }
		public int CodUsuario { get; set; }
		public string NomUsuario { get; set; }
	}

	[Serializable]
	public class AutorizacaoArquivoVO {
		public int CodAutorizacao { get; set; }
		public string NomeArquivo { get; set; }
		public DateTime DataEnvio { get; set; }
	}

	[Serializable]
	public class AutorizacaoProcedimentoVO {
		public int CodAutorizacao { get; set; }
		public Protheus.PTabelaPadraoVO Servico { get; set; }
		public int Quantidade { get; set; }
		public string Observacao { get; set; }
		public bool Opme { get; set; }
	}

	[Serializable]
	public class AutorizacaoTissVO {
		public int CodAutorizacao { get; set; }
		public string NomeArquivo { get; set; }
		public int NrAutorizacaoTiss { get; set; }
	}
}
