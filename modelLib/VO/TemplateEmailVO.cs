using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum TipoTemplateEmail {
		PROTOCOLO_FATURA = 1
	}

	public class TemplateEmailEnumTradutor {
		
		public static string TraduzTipo(TipoTemplateEmail tipo) {
			switch (tipo) {
				case TipoTemplateEmail.PROTOCOLO_FATURA: return "PROTOCOLO DE FATURA";
				default: return "INDEFINIDO";
			}
		}
	}

	[Serializable]
	public class TemplateEmailVO {
		public int Id { get; set; }
		public TipoTemplateEmail Tipo { get; set; }
		public string Nome { get; set; }
		public string Texto { get; set; }
		public int CdUsuarioCriacao { get; set; }
		public DateTime DataCriacao { get; set; }
		public int? CdUsuarioAlteracao { get; set; }
		public DateTime? DataAlteracao { get; set; }
	}
}
