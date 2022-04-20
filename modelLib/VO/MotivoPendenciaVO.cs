using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public enum TipoMotivoPendencia {
		PROTOCOLO_FATURA = 1
	}

	public class MotivoPendenciaEnumTradutor {
		public static string TraduzTipo(TipoMotivoPendencia tipo) {
			return "PROTOCOLO DE FATURA";
		}
	}

	[Serializable]
	public class MotivoPendenciaVO {
		public int Id { get; set; }
		public string Nome { get; set; }
		public TipoMotivoPendencia Tipo { get; set; }
	}
}
