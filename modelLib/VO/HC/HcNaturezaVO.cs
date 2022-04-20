using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcNaturezaVO {
		public int CdNatureza { get; set; }
		public string DsNatureza { get; set; }
		public string CdAtendimentoWeb { get; set; }
		public string CdGrupoIss { get; set; }
	}
}
