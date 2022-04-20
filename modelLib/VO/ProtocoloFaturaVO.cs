using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum StatusProtocoloFatura {
		PROTOCOLADO = 1,
		RECEBIDO_ANALISTA = 2,
		EM_PROCESSAMENTO = 3,
		COM_PENDENCIAS = 4,
		FINALIZADO = 5,
		DEVOLVIDO = 6, 
		CANCELADO = 7
	}

    public enum FaseProtocoloFatura
    {
        INDEFINIDA = 0,
        EM_DIGITACAO = 1,
        EM_CONFERENCIA = 2,
        PRONTO = 3,
        FATURADO = 4
    }

	public class ProtocoloFaturaEnumTradutor {
		public static string TraduzStatus(StatusProtocoloFatura status) {
			switch (status) {
				case StatusProtocoloFatura.PROTOCOLADO: return "PROTOCOLADO";
				case StatusProtocoloFatura.RECEBIDO_ANALISTA: return "RECEBIDO PELO ANALISTA";
				case StatusProtocoloFatura.EM_PROCESSAMENTO: return "EM PROCESSAMENTO";
				case StatusProtocoloFatura.COM_PENDENCIAS: return "COM PENDENCIAS";
				case StatusProtocoloFatura.FINALIZADO: return "FINALIZADO E ENTREGUE AO FINANCEIRO";
				case StatusProtocoloFatura.CANCELADO: return "CANCELADO";
				case StatusProtocoloFatura.DEVOLVIDO: return "DEVOLVIDO TOTAL";
			}
			return "INDEFINIDO";
		}

		public static string TraduzControle(int controle) {
			switch (controle) {
				case ProtocoloFaturaVO.CONTROLE_OK: return "EM DIA";
				case ProtocoloFaturaVO.CONTROLE_ALERTA: return "PERTO DE 5 DIAS DO VENCIMENTO";
				case ProtocoloFaturaVO.CONTROLE_ATRASADO: return "VENCIDA";
				default: return "INDEFINIDO";
			}
		}

        public static string TraduzFase(FaseProtocoloFatura fase)
        {
            switch (fase)
            {
                case FaseProtocoloFatura.INDEFINIDA: return "INDEFINIDA";
                case FaseProtocoloFatura.EM_DIGITACAO: return "EM DIGITAÇÃO";
                case FaseProtocoloFatura.EM_CONFERENCIA: return "EM CONFERÊNCIA";
                case FaseProtocoloFatura.PRONTO: return "PRONTO";
                case FaseProtocoloFatura.FATURADO: return "FATURADO";
                default: return "INDEFINIDA";
            }
        }

		public static int CalcularControle(DateTime dtVencimento, DateTime? dtFinalizacao) {
			if (dtFinalizacao == null)
				dtFinalizacao = DateTime.Now.Date;
			if (dtVencimento >= dtFinalizacao.Value) {
				if (dtVencimento <= dtFinalizacao.Value.AddDays(5))
					return ProtocoloFaturaVO.CONTROLE_ALERTA;
				return ProtocoloFaturaVO.CONTROLE_OK;
			}
			return ProtocoloFaturaVO.CONTROLE_ATRASADO;
		}
	}

	[Serializable]
	public class ProtocoloFaturaVO {

		public const string FORMATO_PROTOCOLO = "000000000";

		public const int CONTROLE_OK = 0;
		public const int CONTROLE_ALERTA = 1;
		public const int CONTROLE_ATRASADO = 2;

		public const int DIAS_ALERTA = 5;

		public int Id { get; set; }
		public string NrProtocolo { get; set; }
        public Protheus.PRedeAtendimentoVO RedeAtendimento { get; set; }
		public DateTime DataEntrada { get; set; }
		public int AnoEntrada { get; set; }
		public string DocumentoFiscal { get; set; }
		public decimal? ValorApresentado { get; set; }
		public decimal? ValorProcessado { get; set; }
		public decimal? ValorGlosa { get; set; }
		public DateTime? DataEmissao { get; set; }
		public DateTime DataVencimento { get; set; }
		
		public int CdUsuarioCriacao { get; set; }
		public int? CdUsuarioResponsavel { get; set; }
		public StatusProtocoloFatura Situacao { get; set; }
        public FaseProtocoloFatura Fase { get; set; }
		public string MotivoCancelamento { get; set; }

		public int? CdPendencia { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime? DataFinalizacao { get; set; }
		public DateTime? DataExpedicao { get; set; }
		
        public int Controle {
			get {
				return ProtocoloFaturaEnumTradutor.CalcularControle(DataVencimento, DataFinalizacao);
			}
		}
	}
}
