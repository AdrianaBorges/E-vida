using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcDemonstrativoAnaliseContaVO {
		public long CpfCnpj { get; set; }
		public DateTime Referencia { get; set; }
		public string DocumentoFiscal { get; set; }
	}
}
