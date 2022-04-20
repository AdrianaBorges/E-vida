using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public enum StatusCanalGestante {
		GERANDO = 0,
		FINALIZADO = 1,
		PENDENTE = 2
	}

	public class CanalGestanteEnumTradutor {
		public static string TraduzStatus(StatusCanalGestante status) {
			switch (status) {
				case StatusCanalGestante.GERANDO: return "GERANDO";
				case StatusCanalGestante.FINALIZADO: return "FINALIZADO";
				case StatusCanalGestante.PENDENTE: return "PENDENTE DE ESCLARECIMENTOS";
				default: return "INDEFINIDO";
			}
		}

		public static string TraduzControle(int controle) {
			switch (controle) {
				case CanalGestanteVO.CONTROLE_OK: return "OK";
				case CanalGestanteVO.CONTROLE_ALERTA: return "ATENÇÃO";
				case CanalGestanteVO.CONTROLE_ATRASADO: return "CRÍTICO";
				default: return "INDEFINIDO";
			}
		}

		public static int CalcularControle(DateTime dtSolicitacao, DateTime? dtFinalizacao) {
			if (dtFinalizacao == null)
				dtFinalizacao = DateTime.Now.Date;

			dtSolicitacao = dtSolicitacao.Date;
			if (dtFinalizacao >= dtSolicitacao.AddDays(CanalGestanteVO.DIAS_CRITICO)) {
				return CanalGestanteVO.CONTROLE_ATRASADO;
			} else if (dtFinalizacao >= dtSolicitacao.AddDays(CanalGestanteVO.DIAS_ALERTA)) {
				return CanalGestanteVO.CONTROLE_ALERTA;
			}
			return ProtocoloFaturaVO.CONTROLE_OK;
		}
	}

	[Serializable]
	public class CanalGestanteVO {
		public const string FORMATO_PROTOCOLO_FILE = "000000000000000";

		public const string TIPO_CONTATO_EMAIL = "EMAIL";
		public const string TIPO_CONTATO_PRESENCIAL = "PRESENCIALMENTE";
		public const string TIPO_CONTATO_CARTA = "CARTA";

		public const int CONTROLE_OK = 0;
		public const int CONTROLE_ALERTA = 1;
		public const int CONTROLE_ATRASADO = 2;

		public const int DIAS_ALERTA = 13;
		public const int DIAS_CRITICO = 16;

		public int Id { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
		public DateTime DataSolicitacao { get; set; }
		public StatusCanalGestante Status { get; set; }
		public string Pendencia { get; set; }
		public string TipoContato { get; set; }
		public DateTime? DataFinalizacao { get; set; }
		public string Resposta { get; set; }
		public int? CodUsuarioFinalizacao { get; set; }

		public string CdCredenciado { get; set; }
		public string NrSeqAnsProfissional { get; set; }
	}

	[Serializable]
	public class CanalGestanteBenefVO {
		public const string CARTA_INFO = "CARTA_INFO";
		public const string CARTAO_GES = "CARTAO_GES";
		public const string PARTOGRAMA = "PARTOGRAMA";

        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
		public string CodAlternativo { get; set; }
		public DateTime DataNascimento { get; set; }
		public string PlanoVinculado { get; set; }
		public string Email { get; set; }
		public string Telefone { get; set; }
		public DateTime? DataDownloadInfo { get; set; }
		public DateTime? DataDownloadCartao { get; set; }
		public DateTime? DataDownloadPartograma { get; set; }

	}

	[Serializable]
	public class CanalGestanteConfigVO {
		public int Ano { get; set; }
		public decimal PartoNormal { get; set; }
		public int CodUsuarioAlteracao { get; set; }
		public DateTime DataAlteracao { get; set; }
	}

	[Serializable]
	public class CanalGestanteConfigCredVO {
		public int Ano { get; set; }
		public string CodCredenciado { get; set; }
		public string Uf { get; set; }
		public decimal PartoNormal { get; set; }
	}

	[Serializable]
	public class CanalGestanteConfigProfVO {
		public int Ano { get; set; }
		public string Codigo { get; set; }
        public string Codsig { get; set; }
        public string Estado { get; set; }
        public string Numcr { get; set; }
		public decimal PartoNormal { get; set; }
	}
}
