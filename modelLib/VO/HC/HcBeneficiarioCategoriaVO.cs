using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcBeneficiarioCategoriaVO {
		public int CdBeneficiario { get; set; }
		public DateTime InicioVigencia { get; set; }
		public DateTime? FimVigencia { get; set; }
		public int CdCategoria { get; set; }
		public string DsCategoria { get; set; }
	}
}
