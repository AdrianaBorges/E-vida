using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.VO {
	public enum TipoArquivoIndisponibilidadeRede {
		BENEFICIARIO,
		CREDENCIAMENTO,
		COMPROVANTE
	}

	public enum PrioridadeIndisponibilidadeRede { 
		CONSULTA,
		EXAMES,
		ALTA_COMPLEXIDADE,
		REGIME_HOSPITALAR,
		URGENCIA_EMERGENCIA,
		OUTROS
	}

	public enum StatusIndisponibilidadeRede {
		ABERTO,
		EM_ATENDIMENTO,
		PENDENTE,
		ENCERRADO
	}

	public enum AvalIndisponibilidadeRede {
		DIRETORIA_DEFERIDO,
		DIRETORIA_INDEFERIDO,
		DIRETORIA_COMPLEMENTO,

		FATURAMENTO_FATURAMENTO,
		FATURAMENTO_REEMBOLSO,

		AUTORIZACAO_AUTORIZADO,
		AUTORIZACAO_NAO_AUTORIZADO,
		AUTORIZACAO_COMPLEMENTO,
	}

	public enum SituacaoFinanceiroIndisponibilidadeRede {
		PAGAMENTO_SOLICITADO,
		PAGAMENTO_REALIZADO,
		EXECUTAR_COBRANCA,
		EXECUCAO_COBRANCA_REALIZADA
	}

    public enum EncaminhamentoIndisponibilidadeRede
    {
        CREDENCIAMENTO,
        DIRETORIA,
        FINANCEIRO,
        FATURAMENTO,
        AUTORIZACAO,
        REGIONAL,
        CADASTRO,
        BENEFICIARIO,
        SEFAT_CM,
        SEFAT_RB,
        PAT_ARARAQUARA,
        PAT_BELEM,
        PAT_BOAVISTA,
        PAT_CUIABA,
        PAT_IMPERATRIZ,
        PAT_MACAPA,
        PAT_MANAUS,
        PAT_MARABA,
        PAT_PALMAS,
        PAT_PORTOVELHO,
        PAT_RIOBRANCO,
        PAT_SAOLUIS,
        PAT_TUCURUI,
        PAT_SAOPAULO
    }

    public enum TipoPendenciaIndisponibilidadeRede
    {
        PEDIDO_MEDICO,
        TERMO_COMPROMISSO,
        NF_RECIBO_REEMBOLSO,
        NF_RECIBO_FINANCEIRO,
        RECEBIDO,
        RESOLVIDO
    }

	public class IndisponibilidadeRedeEnumTradutor {
		public static AvalIndisponibilidadeRede[] AVAL_AUTORIZACAO = new AvalIndisponibilidadeRede[] { AvalIndisponibilidadeRede.AUTORIZACAO_AUTORIZADO, AvalIndisponibilidadeRede.AUTORIZACAO_NAO_AUTORIZADO, AvalIndisponibilidadeRede.AUTORIZACAO_COMPLEMENTO };
		public static AvalIndisponibilidadeRede[] AVAL_FATURAMENTO = new AvalIndisponibilidadeRede[] { AvalIndisponibilidadeRede.FATURAMENTO_FATURAMENTO, AvalIndisponibilidadeRede.FATURAMENTO_REEMBOLSO };
		public static AvalIndisponibilidadeRede[] AVAL_DIRETORIA = new AvalIndisponibilidadeRede[] { AvalIndisponibilidadeRede.DIRETORIA_DEFERIDO, AvalIndisponibilidadeRede.DIRETORIA_INDEFERIDO, AvalIndisponibilidadeRede.DIRETORIA_COMPLEMENTO };

		public static string TraduzPrioridade(PrioridadeIndisponibilidadeRede prioridade) {
			switch (prioridade) {
				case PrioridadeIndisponibilidadeRede.CONSULTA: return "CONSULTA";
				case PrioridadeIndisponibilidadeRede.EXAMES: return "EXAMES";
				case PrioridadeIndisponibilidadeRede.ALTA_COMPLEXIDADE: return "PROCEDIMENTO DE ALTA COMPLEXIDADE";
				case PrioridadeIndisponibilidadeRede.REGIME_HOSPITALAR: return "ATENDIMENTO EM REGIME HOSPITALAR";
				case PrioridadeIndisponibilidadeRede.URGENCIA_EMERGENCIA: return "URGENCIA E EMERGENCIA";
				case PrioridadeIndisponibilidadeRede.OUTROS: return "OUTROS";
				default: return "INDEFINIDO";
			}
		}

		public static PrioridadeIndisponibilidadeRede TraduzPrioridade(string prioridade) {
			Array arr = Enum.GetValues(typeof(PrioridadeIndisponibilidadeRede));
			foreach (object o in arr) {
				if (((PrioridadeIndisponibilidadeRede)o).ToString().Equals(prioridade, StringComparison.InvariantCultureIgnoreCase))
					return (PrioridadeIndisponibilidadeRede)o;
			}
			return PrioridadeIndisponibilidadeRede.CONSULTA;
		}

		public static string TraduzStatus(StatusIndisponibilidadeRede status) {
			switch (status) {
				case StatusIndisponibilidadeRede.ABERTO: return "ABERTO";
				case StatusIndisponibilidadeRede.EM_ATENDIMENTO: return "EM ATENDIMENTO";
				case StatusIndisponibilidadeRede.PENDENTE: return "PENDENTE";
				case StatusIndisponibilidadeRede.ENCERRADO: return "ENCERRADO";
				default: return "INDEFINIDO";
			}

		}

		public static string TraduzStatusFinanceiro(SituacaoFinanceiroIndisponibilidadeRede status) {
			switch (status) {
				case SituacaoFinanceiroIndisponibilidadeRede.PAGAMENTO_REALIZADO: return "PAGAMENTO REALIZADO";
				case SituacaoFinanceiroIndisponibilidadeRede.PAGAMENTO_SOLICITADO: return "SOLICITADO PAGAMENTO";
				case SituacaoFinanceiroIndisponibilidadeRede.EXECUTAR_COBRANCA: return "EXECUÇÃO PENDENTE";
				case SituacaoFinanceiroIndisponibilidadeRede.EXECUCAO_COBRANCA_REALIZADA: return "EXECUÇÃO REALIZADA";
				default: return "INDEFINIDO";
			}

		}

		public static string TraduzAval(AvalIndisponibilidadeRede aval) {
			switch (aval) {
				case AvalIndisponibilidadeRede.DIRETORIA_DEFERIDO: return "DEFERIDO";
				case AvalIndisponibilidadeRede.DIRETORIA_INDEFERIDO: return "INDEFERIDO";
				case AvalIndisponibilidadeRede.DIRETORIA_COMPLEMENTO: return "COMPLEMENTO";
				case AvalIndisponibilidadeRede.FATURAMENTO_FATURAMENTO: return "FATURAMENTO";
				case AvalIndisponibilidadeRede.FATURAMENTO_REEMBOLSO: return "REEMBOLSO";
				case AvalIndisponibilidadeRede.AUTORIZACAO_AUTORIZADO: return "AUTORIZADO";
				case AvalIndisponibilidadeRede.AUTORIZACAO_NAO_AUTORIZADO: return "NÃO AUTORIZADO";
				case AvalIndisponibilidadeRede.AUTORIZACAO_COMPLEMENTO: return "COMPLEMENTO";
				default: return "INDEFINIDO";
			}
		}

		public static AvalIndisponibilidadeRede TraduzAval(string aval) {
			Array arr = Enum.GetValues(typeof(AvalIndisponibilidadeRede));
			foreach (object o in arr) {
				if (((AvalIndisponibilidadeRede)o).ToString().Equals(aval, StringComparison.InvariantCultureIgnoreCase))
					return (AvalIndisponibilidadeRede)o;
			}
			throw new ArgumentOutOfRangeException("aval", aval);
		}

		public static string TraduzSetor(EncaminhamentoIndisponibilidadeRede setorDestino) {
			return setorDestino.ToString();
		}

		public static EncaminhamentoIndisponibilidadeRede TraduzSetor(string setor) {
			Array arr = Enum.GetValues(typeof(EncaminhamentoIndisponibilidadeRede));
			foreach (object o in arr) {
				if (((EncaminhamentoIndisponibilidadeRede)o).ToString().Equals(setor, StringComparison.InvariantCultureIgnoreCase))
					return (EncaminhamentoIndisponibilidadeRede)o;
			}
			throw new ArgumentOutOfRangeException("setor", setor);
		}

        public static string TraduzTipoPendencia(TipoPendenciaIndisponibilidadeRede pendencia)
        {
            switch (pendencia)
            {
                case TipoPendenciaIndisponibilidadeRede.PEDIDO_MEDICO: return "PEDIDO MÉDICO";
                case TipoPendenciaIndisponibilidadeRede.TERMO_COMPROMISSO: return "TERMO DE COMPROMISSO";
                case TipoPendenciaIndisponibilidadeRede.NF_RECIBO_REEMBOLSO: return "NF/RECIBO - REEMBOLSO";
                case TipoPendenciaIndisponibilidadeRede.NF_RECIBO_FINANCEIRO: return "NF/RECIBO - FINANCEIRO";
                case TipoPendenciaIndisponibilidadeRede.RECEBIDO: return "RECEBIDO";
                case TipoPendenciaIndisponibilidadeRede.RESOLVIDO: return "RESOLVIDO";
            }
            return "INDEFINIDO";
        }

        public static string TraduzEncaminhamento(EncaminhamentoIndisponibilidadeRede setor)
        {
            switch (setor)
            {
                case EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO: return "CREDENCIAMENTO";
                case EncaminhamentoIndisponibilidadeRede.DIRETORIA: return "ALÇADA SUPERIOR";
                case EncaminhamentoIndisponibilidadeRede.FINANCEIRO: return "FINANCEIRO";
                case EncaminhamentoIndisponibilidadeRede.FATURAMENTO: return "FATURAMENTO";
                case EncaminhamentoIndisponibilidadeRede.AUTORIZACAO: return "AUTORIZAÇÃO";
                case EncaminhamentoIndisponibilidadeRede.REGIONAL: return "REGIONAL";
                case EncaminhamentoIndisponibilidadeRede.CADASTRO: return "CADASTRO";
                case EncaminhamentoIndisponibilidadeRede.BENEFICIARIO: return "BENEFICIÁRIO";
                case EncaminhamentoIndisponibilidadeRede.SEFAT_CM: return "SEFAT-CM";
                case EncaminhamentoIndisponibilidadeRede.SEFAT_RB: return "SEFAT-RB";
                case EncaminhamentoIndisponibilidadeRede.PAT_ARARAQUARA: return "PAT ARARAQUARA";
                case EncaminhamentoIndisponibilidadeRede.PAT_BELEM: return "PAT BELÉM";
                case EncaminhamentoIndisponibilidadeRede.PAT_BOAVISTA: return "PAT BOA VISTA";
                case EncaminhamentoIndisponibilidadeRede.PAT_CUIABA: return "PAT CUIABÁ";
                case EncaminhamentoIndisponibilidadeRede.PAT_IMPERATRIZ: return "PAT IMPERATRIZ";
                case EncaminhamentoIndisponibilidadeRede.PAT_MACAPA: return "PAT MACAPÁ";
                case EncaminhamentoIndisponibilidadeRede.PAT_MANAUS: return "PAT MANAUS";
                case EncaminhamentoIndisponibilidadeRede.PAT_MARABA: return "PAT MARABÁ";
                case EncaminhamentoIndisponibilidadeRede.PAT_PALMAS: return "PAT PALMAS";
                case EncaminhamentoIndisponibilidadeRede.PAT_PORTOVELHO: return "PAT PORTO VELHO";
                case EncaminhamentoIndisponibilidadeRede.PAT_RIOBRANCO: return "PAT RIO BRANCO";
                case EncaminhamentoIndisponibilidadeRede.PAT_SAOLUIS: return "PAT SÃO LUÍS";
                case EncaminhamentoIndisponibilidadeRede.PAT_TUCURUI: return "PAT TUCURUÍ";
                default: return "INDEFINIDO";
            }

            
        }

	}

	[Serializable]
	public class IndisponibilidadeRedeVO {
		public const string FORMATO_PROTOCOLO = "000000000";

		public int Id { get; set; }
        public PUsuarioVO Usuario { get; set; }
		public DateTime DataCriacao { get; set; }
		public int IdEspecialidade { get; set; }
		public PrioridadeIndisponibilidadeRede Prioridade { get; set; }
		public string TelefoneContato { get; set; }
		public string EmailContato { get; set; }
		public string Uf { get; set; }
		public int? IdLocalidade { get; set; }

		public int DiasPrazo { get; set; }
		public decimal? ValorSolicitacao { get; set; }
		public string TelefonePrestador { get; set; }
		public string EnderecoPrestador { get; set; }

		public AvalIndisponibilidadeRede? AvalDiretoria { get; set; }
		public AvalIndisponibilidadeRede? AvalFaturamento { get; set; }
		public AvalIndisponibilidadeRede? AvalAutorizacao { get; set; }

		public string Banco { get; set; }
		public string Agencia { get; set; }
		public string ContaCorrente { get; set; }
		public string Favorecido { get; set; }
		public decimal? ValorFinanceiro { get; set; }
		public string CodigoServicoFinanceiro { get; set; }

		public StatusIndisponibilidadeRede Situacao { get; set; }
		public DateTime DataSituacao { get; set; }

		public string ComplementoDiretoria { get; set; }
		public string ComplementoAutorizacao { get; set; }

        public bool? Acompanhante { get; set; }

		public SituacaoFinanceiroIndisponibilidadeRede? SituacaoFinanceiro { get; set; }
		public string ObservacaoFinanceiro { get; set; }
		public string ObservacaoFinanceiroBaixa { get; set; }

        public string ObservacaoCadastro { get; set; }

		public int? CodUsuarioFaturamento { get; set; }
		public decimal? ProtocoloFaturamento { get; set; }
        public string ObservacaoFaturamento { get; set; }

		public EncaminhamentoIndisponibilidadeRede SetorEncaminhamento { get; set; }
		public int? CodUsuarioAtuante { get; set; }

		public string MotivoEncerramento { get; set; }

        public string Procedencia { get; set; }

		public long? CpfCnpjCred { get; set; }
		public string RazaoSocialCred { get; set; }

		public string ProtocoloAns { get; set; }

		public string TratativaGuiaMedico { get; set; }
		public string TratativaReciprocidade { get; set; }
		public TipoPendenciaIndisponibilidadeRede? Pendencia { get; set; }
		public DateTime? DataPendencia { get; set; }

		public DateTime? DataExecucao { get; set; }
		public decimal? ValorExecucao { get; set; }

        public DateTime? DataAtendimento { get; set; }
        public int? CodUsuarioAtendente { get; set; }
	}

	[Serializable]
	public class IndisponibilidadeRedeObsVO {
		public const string ORIGEM_BENEF = "BENEF";
		public const string ORIGEM_INTRANET = "INTRANET";
		public const int TIPO_EXTERNO = 1;
		public const int TIPO_INTERNO = 2;

		public int IdIndisponibilidade { get; set; }
		public DateTime DataRegistro { get; set; }
		public string Origem { get; set; }
		public string Observacao { get; set; }
		public int? CodUsuario { get; set; }
		public int TipoObs { get; set; }
		public TipoPendenciaIndisponibilidadeRede? Pendencia { get; set; }
	}

	public class IndisponibilidadeRedeHistoricoVO {
		public int IdIndisponibilidade { get; set; }
		public DateTime DataHistorico { get; set; }
		public StatusIndisponibilidadeRede? StatusOrigem { get; set; }
		public StatusIndisponibilidadeRede StatusDestino { get; set; }
		public EncaminhamentoIndisponibilidadeRede Setor { get; set; }
		public int? CodUsuario { get; set; }
	}

	public class IndisponibilidadeRedeArquivoVO {
		public int IdIndisponibilidade  { get; set; }
		public int IdArquivo { get; set; }
		public TipoArquivoIndisponibilidadeRede TipoArquivo { get; set; }
		public string NomeArquivo { get; set; }
		public DateTime DataEnvio { get; set; }
	}

	[Serializable]
	public class EspecialidadeVO {
		public int Id { get; set; }
		public string Nome { get; set; }
		public int? PrazoConsulta { get; set; }
		public int? PrazoExame { get; set; }
		public int? PrazoAltaComplexidade { get; set; }
		public int? PrazoAtendimentoHosp { get; set; }
		public int? PrazoUrgenciaEmergencia { get; set; }
		public int? PrazoOutros { get; set; }
	}

	[Serializable]
	public class IndisponibilidadeRedeOrcamentoVO {
		public int IdIndisponibilidade { get; set; }
		public int IdOrcamento { get; set; }
		public string CpfCnpj { get; set; }
		public string NomePrestador { get; set; }
		public string Telefone { get; set; }
		public string Email { get; set; }
		public decimal? Valor { get; set; }
	}

    [Serializable]
    public class IndisponibilidadeRedeHorasSetorVO
    {
        public int IdIndisponibilidade { get; set; }
        public EncaminhamentoIndisponibilidadeRede Setor { get; set; }
        public double Horas { get; set; }
    }	
}

