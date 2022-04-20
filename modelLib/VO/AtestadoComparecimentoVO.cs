using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum StatusAtestadoComparecimento {
		PENDENTE = 0,
		EMITIDO = 1
	}
	[Serializable]
	public class AtestadoComparecimentoVO {
		public const string FORMATO_PROTOCOLO = "000000000";

		public int CodSolicitacao { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
		public string Nome { get; set; }
		public string Lotacao { get; set; }
		public int IdStatus { get; set; }
		public DateTime DataAtendimento { get; set; }
		public string HoraInicio { get; set; }
		public string HoraFim { get; set; }
		public int TipoPericia { get; set; }
		public int IdUsuarioCriacao { get; set; }
		public DateTime DataCriacao { get; set; }
		public int? IdUsuarioFinalizacao { get; set; }
		public DateTime? DataFinalizacao { get; set; }
	}
}
