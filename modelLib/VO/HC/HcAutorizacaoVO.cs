using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	
	[Serializable]
	public class HcAutorizacaoVO {
		public int NrAutorizacao { get; set; }
		public long CdBeneficiario { get; set; }
		public DateTime DtRegistro { get; set; }
		public string TpAutorizacao { get; set; }
		public string StAutorizacao { get; set; }
		public DateTime? DtInicioAutorizacao { get; set; }
		public DateTime? DtTerminoAutorizacao { get; set; }
		public string Observacao { get; set; }
		public string UserUpdate { get; set; }
		public string DateUpdate { get; set; }
		public int? NrDiasAutorizados { get; set; }
	}
}
