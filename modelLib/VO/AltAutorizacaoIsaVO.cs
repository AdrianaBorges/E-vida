using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class AltAutorizacaoIsaVO {
		public int NrAutorizacao { get; set; }
		public DateTime Data { get; set; }
		public int IdUsuario { get; set; }
		public DateTime? InicioAnt { get; set; }
		public DateTime InicioNovo { get; set; }
		public DateTime? TerminoAnt { get; set; }
		public DateTime TerminoNovo { get; set; }

	}
}
