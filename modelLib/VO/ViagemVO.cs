using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public class SolicitacaoViagemEnumTradutor {
		public static TipoDespesaPrestContaViagem[] TIPO_DESPESA_TRANSPORTE = new TipoDespesaPrestContaViagem[] 
			{ TipoDespesaPrestContaViagem.AVIAO, TipoDespesaPrestContaViagem.TAXI, TipoDespesaPrestContaViagem.COMBUSTIVEL,
			TipoDespesaPrestContaViagem.ESTACIONAMENTO, TipoDespesaPrestContaViagem.ONIBUS_INTERURBANO, TipoDespesaPrestContaViagem.ONUBUS_COLETIVO_URBANO,
			TipoDespesaPrestContaViagem.METRO, TipoDespesaPrestContaViagem.PEDAGIO,TipoDespesaPrestContaViagem.TAXA_EMBARQUE };

		public static TipoDespesaPrestContaViagem[] TIPO_DESPESA_HOSPEDAGEM = new TipoDespesaPrestContaViagem[] { TipoDespesaPrestContaViagem.HOTEL, TipoDespesaPrestContaViagem.OUTROS };

		public static TipoDespesaPrestContaViagem[] TIPO_DESPESA_ALIMENTACAO = new TipoDespesaPrestContaViagem[] { TipoDespesaPrestContaViagem.REFEICAO, TipoDespesaPrestContaViagem.OUTROS };

		public static TipoDespesaPrestContaViagem[] TIPO_DESPESA_DIVERSOS = new TipoDespesaPrestContaViagem[] { TipoDespesaPrestContaViagem.TAXA_ADMINISTRATIVA,
			TipoDespesaPrestContaViagem.FOTOCOPIA, TipoDespesaPrestContaViagem.INSCRICAO_CURSO, TipoDespesaPrestContaViagem.OUTROS };

		public static TipoDespesaPrestContaViagem[] GetListaTipoDespesa(GrupoDespesaPrestContaViagem grupo) {
			switch (grupo) {
				case GrupoDespesaPrestContaViagem.ALIMENTACAO: return TIPO_DESPESA_ALIMENTACAO;
				case GrupoDespesaPrestContaViagem.DIVERSOS: return TIPO_DESPESA_DIVERSOS;
				case GrupoDespesaPrestContaViagem.HOSPEDAGEM: return TIPO_DESPESA_HOSPEDAGEM;
				case GrupoDespesaPrestContaViagem.TRANSPORTE: return TIPO_DESPESA_TRANSPORTE;
			}
			throw new InvalidOperationException("grupo não tratado: " + grupo);
		}

		public static string TraduzStatus(StatusSolicitacaoViagem status) {
			switch (status) {
				case StatusSolicitacaoViagem.SOLICITACAO_PENDENTE: return "PENDENTE";
				case StatusSolicitacaoViagem.SOLICITACAO_APROVADO_COORDENADOR: return "SOLICITAÇÃO LIBERADA PARA DIRETORIA";
				case StatusSolicitacaoViagem.SOLICITACAO_APROVADO_DIRETORIA: return "SOLICITAÇÃO APROVADA / COMPRA PENDENTE";
				case StatusSolicitacaoViagem.SOLICITACAO_REPROVADO_COORDENADOR: return "SOLICITAÇÃO REPROVADA PELO COORDENADOR";
				case StatusSolicitacaoViagem.SOLICITACAO_REPROVADO_DIRETORIA: return "SOLICITAÇÃO REPROVADA PELA DIRETORIA";
				case StatusSolicitacaoViagem.COMPRA_EFETUADA: return "COMPRA DE PASSAGEM EFETUADA";
				case StatusSolicitacaoViagem.PAGAMENTO_ADIANTAMENTO_EFETUADO: return "PAGAMENTO DE ADIANTAMENTO EFETUADO";
				case StatusSolicitacaoViagem.PAGAMENTO_ADIANTAMENTO_CONFERIDO: return "PAGAMENTO DE ADIANTAMENTO/DIÁRIAS CONFIRMADO";
				case StatusSolicitacaoViagem.PRESTACAO_CONTA_PENDENTE: return "PRESTAÇÃO DE CONTA PENDENTE DE ANÁLISE";
				case StatusSolicitacaoViagem.PRESTACAO_CONTA_CONFERIDA: return "PRESTAÇÃO DE CONTA CONFERIDA PELO FINANCEIRO";
				case StatusSolicitacaoViagem.PRESTACAO_CONTA_APROVADA: return "PRESTAÇÃO DE CONTA APROVADA";
				case StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_DIRETORIA: return "PRESTAÇÃO DE CONTA REPROVADA PELA DIRETORIA";
				case StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_FINANCEIRO: return "PRESTAÇÃO DE CONTA REPROVADA PELO FINANCEIRO";
				case StatusSolicitacaoViagem.CANCELADO: return "CANCELADO";
				default:
					return "-INDEFINIDO-";
			}
		}
		public static string TraduzTipoDespPrestConta(TipoDespesaPrestContaViagem tipo) {
			switch (tipo) {
				case TipoDespesaPrestContaViagem.AVIAO: return "AVIÃO";
				case TipoDespesaPrestContaViagem.TAXI: return "TAXI";
				case TipoDespesaPrestContaViagem.COMBUSTIVEL: return "COMBUSTÍVEL";
				case TipoDespesaPrestContaViagem.ESTACIONAMENTO: return "ESTACIONAMENTO";
				case TipoDespesaPrestContaViagem.ONIBUS_INTERURBANO: return "ÔNIBUS INTERURBANO";
				case TipoDespesaPrestContaViagem.ONUBUS_COLETIVO_URBANO: return "ÔNIBUS COLETIVO URBANO";
				case TipoDespesaPrestContaViagem.METRO: return "METRÔ";
				case TipoDespesaPrestContaViagem.PEDAGIO: return "PEDÁGIO";
				case TipoDespesaPrestContaViagem.TAXA_EMBARQUE: return "TAXA DE EMBARQUE";
				case TipoDespesaPrestContaViagem.HOTEL: return "HOTEL";
				case TipoDespesaPrestContaViagem.REFEICAO: return "REFEIÇÃO";
				case TipoDespesaPrestContaViagem.TAXA_ADMINISTRATIVA: return "TAXAS ADMINISTRATIVAS";
				case TipoDespesaPrestContaViagem.FOTOCOPIA: return "FOTOCÓPIA";
				case TipoDespesaPrestContaViagem.INSCRICAO_CURSO: return "INSCRIÇÃO EM CURSO";
				case TipoDespesaPrestContaViagem.OUTROS: return "OUTROS";
				default:
					return "-INDEFINIDO-";
			}
		}
		public static string TraduzGrupoDespPrestConta(GrupoDespesaPrestContaViagem grupo) {
			switch (grupo) {
				case GrupoDespesaPrestContaViagem.TRANSPORTE: return "TRANSPORTE";
				case GrupoDespesaPrestContaViagem.HOSPEDAGEM: return "HOSPEDAGEM";
				case GrupoDespesaPrestContaViagem.ALIMENTACAO: return "ALIMENTAÇÃO";
				case GrupoDespesaPrestContaViagem.DIVERSOS: return "DIVERSOS";
				default:
					return "-INDEFINIDO-";
			}
		}
		public static string TraduzMeioTransporte(MeioTransporteViagem meio) {
			switch (meio) {
				case MeioTransporteViagem.AEREO: return "AÉREO";
				case MeioTransporteViagem.ONIBUS: return "ÔNIBUS INTERURBANO";
				case MeioTransporteViagem.CARRO_PROPRIO: return "CARRO PRÓPRIO";
				case MeioTransporteViagem.OUTROS: return "OUTROS";
				default:
					return "-INDEFINIDO";
			}
		}

		public static bool IsStatusPrestacaoContas(StatusSolicitacaoViagem status) {
			return status == StatusSolicitacaoViagem.PRESTACAO_CONTA_APROVADA ||
				status == StatusSolicitacaoViagem.PRESTACAO_CONTA_CONFERIDA ||
				status == StatusSolicitacaoViagem.PRESTACAO_CONTA_PENDENTE ||
				status == StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_DIRETORIA ||
				status == StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_FINANCEIRO;
		}
	}

	public enum TipoItinerarioSolicitacaoViagem {
		ITINERARIO,
		PASSAGEM,
		HOTEL
	}

	public enum TipoArquivoViagem {
		COMPROVANTE_PAGAMENTO_DIARIA,
		COMPROVANTE_DESPESA,
		RELATORIO_VIAGEM,
		COMPROVANTE_PAGTO_TRASLADO,
		COMPROVANTE_PAGTO_HOTEL,
		CURSO,
		COMPROVANTE_REEMBOLSO
	}
	public enum StatusSolicitacaoViagem {
		SOLICITACAO_PENDENTE = 1,
		SOLICITACAO_APROVADO_COORDENADOR = 2,
		SOLICITACAO_REPROVADO_COORDENADOR = 3,
		SOLICITACAO_APROVADO_DIRETORIA = 4,
		SOLICITACAO_REPROVADO_DIRETORIA = 5,
		COMPRA_EFETUADA = 10,
		PAGAMENTO_ADIANTAMENTO_EFETUADO = 11,
		PAGAMENTO_ADIANTAMENTO_CONFERIDO = 12,
		PRESTACAO_CONTA_PENDENTE = 20,
		PRESTACAO_CONTA_CONFERIDA = 21,
		PRESTACAO_CONTA_REPROVADO_FINANCEIRO = 22,
		PRESTACAO_CONTA_APROVADA = 23,
		PRESTACAO_CONTA_REPROVADO_DIRETORIA = 24,
		CANCELADO = 25
	}

	public enum TipoViagem {
		CURSO = 1,
		VISITAS = 2
	}

	public enum GrupoDespesaPrestContaViagem {
		TRANSPORTE = 1,
		HOSPEDAGEM = 2,
		ALIMENTACAO = 3,
		DIVERSOS = 4
	}

	public enum TipoDespesaPrestContaViagem {
		AVIAO = 1,
		TAXI = 2,
		COMBUSTIVEL = 3,
		ESTACIONAMENTO = 4,
		ONIBUS_INTERURBANO = 5,
		ONUBUS_COLETIVO_URBANO = 6,
		METRO = 7,
		PEDAGIO = 8,
		TAXA_EMBARQUE = 9,

		HOTEL = 101,

		REFEICAO = 301,

		TAXA_ADMINISTRATIVA = 401,
		FOTOCOPIA = 402,
		INSCRICAO_CURSO = 403,

		OUTROS = 999
	}

	public enum MeioTransporteViagem {
		AEREO = 1,
		ONIBUS = 2,
		CARRO_PROPRIO = 3,
		OUTROS = 99			
	}
	[Serializable]
	public class SolicitacaoViagemVO {
		public const string FORMATO_PROTOCOLO = "000000000";

		public int Id { get; set; }
		public bool IsExterno { get; set; }
		public TipoViagem? TipoViagem { get; set; }
		public Protheus.EmpregadoEvidaVO Empregado { get; set; }
		public DateTime DataCriacao { get; set; }

		public SolicitacaoViagemInfoSolicitacaoVO Solicitacao { get; set; }
		public SolicitacaoViagemInfoPrestacaoContaVO PrestacaoConta { get; set; }
		public SolicitacaoViagemInfoCompraVO Compra { get; set; }

		public SolicitacaoViagemAprovacaoVO AprovSolicitacaoDiretoria { get; set; }
		public SolicitacaoViagemAprovacaoVO AprovSolicitacaoCoordenador { get; set; }
		public SolicitacaoViagemAprovacaoVO AprovPrestacaoFinanceiro { get; set; }
		public SolicitacaoViagemAprovacaoVO AprovPrestacaoDiretoria { get; set; }
		
		public StatusSolicitacaoViagem Situacao { get; set; }
		public DateTime DataSituacao { get; set; }

		public int CodUsuarioSolicitante { get; set; }
	}
	[Serializable]
	public class SolicitacaoViagemAprovacaoVO {
		public bool Aprovado { get; set; }
		public string Justificativa { get; set; }
		public int IdUsuario { get; set; }
	}
	[Serializable]
	public class SolicitacaoViagemInfoSolicitacaoVO {
		public string Nome { get; set; }
		public long Cpf { get; set; }
		public string Rg { get; set; }
		public DateTime DataNascimento { get; set; }
		public string Cargo { get; set; }
		public string Telefone { get; set; }
		public string Ramal { get; set; }
		public bool UsaValorDiaria { get; set; }

		public string Objetivo { get; set; }
		public List<MeioTransporteViagem> MeioTransporte { get; set; }

		public HC.HcBancoVO Banco { get; set; }
		public string Agencia { get; set; }
		public string ContaCorrente { get; set; }

		public decimal? ValorAdiantamento { get; set; }
		public string JustificativaAdiantamento { get; set; }


		public List<SolicitacaoViagemItinerarioVO> Itinerarios { get; set; }
	}
	[Serializable]
	public class SolicitacaoViagemInfoPrestacaoContaVO {
		public int IdViagem { get; set; }
		public decimal? ValorPassagens { get; set; }
		public decimal? ValorHospedagem { get; set; }
		public string ResumoViagem { get; set; }
		
		public List<SolicitacaoViagemDespesaDetalhadaVO> DespesasDetalhadas { get; set; }
	}

	[Serializable]
	public class SolicitacaoViagemDespesaDetalhadaVO {
		public int IdViagem { get; set; }
		public int IdDespesa { get; set; }
		public DateTime Data { get; set; }
		public string Descricao { get; set; }
		public GrupoDespesaPrestContaViagem GrupoDespesa { get; set; }
		public TipoDespesaPrestContaViagem TipoDespesa { get; set; }
		public string DescricaoTipoDespesa { get; set; }
		public string Identificador { get; set; }
		public decimal Valor { get; set; }
		public DateTime? DataConferido { get; set; }
	}
	[Serializable]
	public class SolicitacaoViagemItinerarioVO {
		public int IdViagem { get; set; }
		public int IdItinerario { get; set; }
		public TipoItinerarioSolicitacaoViagem TipoRegistro { get; set; } // Itinerario, Passagem, Hotel
		public string Origem { get; set; }
		public string Destino { get; set; }
		public DateTime DataPartida { get; set; }
		public DateTime DataRetorno { get; set; }
		public decimal? Valor { get; set; }
		public string Complemento { get; set; }
		public TipoArquivoViagem? TipoArquivo { get; set; }
		public int? IdArquivo { get; set; }
	}
	[Serializable]
	public class SolicitacaoViagemInfoCompraVO {
		public int IdViagem { get; set; }
		public List<SolicitacaoViagemItinerarioVO> Passagens { get; set; }
		public List<SolicitacaoViagemItinerarioVO> Hoteis { get; set; }

		public decimal? ValorCurso { get; set; }
		public decimal? ValorPago { get; set; }
		public bool RecebimentoConfirmado { get; set; }
	}
	[Serializable]
	public class SolicitacaoViagemArquivoVO {
		public int IdViagem { get; set; }
		public int IdArquivo { get; set; }
		public TipoArquivoViagem TipoArquivo { get; set; }
		public string NomeArquivo { get; set; }
		public DateTime DataEnvio { get; set; }
	}
	[Serializable]
	public class ViagemValorVariavelVO {
		public ParametroVariavelVO Parametro { get; set; }
		public DateTime Inicio { get; set; }
		public DateTime Fim { get; set; }
		public decimal Quantidade { get; set; }

		public decimal ValorFinal {
			get {
				return Quantidade * Decimal.Parse(Parametro.Value);
			}
		}
	}

}
