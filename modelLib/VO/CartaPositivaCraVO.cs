using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public enum CartaPositivaCraTipo {
		CARENCIA = 1,
		ISENTO = 2
	}

	public enum CartaPositivaCraStatus {
		PENDENTE = 1,
		APROVADO = 2,
		CANCELADO = 3
	}


	public class CartaPositivaCraEnumTradutor {
		public static string TraduzStatus(CartaPositivaCraStatus status) {
			switch (status) {
				case CartaPositivaCraStatus.PENDENTE: return "PENDENTE";
				case CartaPositivaCraStatus.APROVADO: return "APROVADO";
				case CartaPositivaCraStatus.CANCELADO: return "CANCELADO";
				default: return "-";
			}
		}
		public static string TraduzTipo(CartaPositivaCraTipo tipo) {
			switch (tipo) {
				case CartaPositivaCraTipo.CARENCIA: return "EM CARÊNCIA";
				case CartaPositivaCraTipo.ISENTO: return "ISENTO DE CARÊNCIA";
				default: return "-";
			}
		}
	}

	[Serializable]
	public class CartaPositivaCraVO {
		public int Id { get; set; }
		public CartaPositivaCraTipo Tipo { get; set; }
		public DateTime DataSolicitacao { get; set; }

		public string ProtocoloCra { get; set; }
		public Protheus.PUsuarioVO Beneficiario { get; set; }
		public Protheus.PRedeAtendimentoVO Credenciado { get; set; }
		public string Contato { get; set; }

		public string CdPlano { get; set; }

		public int IdUsuarioCriacao { get; set; }
		public int IdUsuarioAlteracao { get; set; }
		public int? IdUsuarioAprovacao { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime DataAlteracao { get; set; }
		public DateTime? DataAprovacao { get; set; }

		public CartaPositivaCraStatus Status { get; set; }
		public string MotivoCancelamento { get; set; }
	}
}
