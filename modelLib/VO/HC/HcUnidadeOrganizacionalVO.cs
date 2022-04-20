using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcUnidadeOrganizacionalVO {
		public string CdUniOrg { get; set; }
		public string DsUniOrg { get; set; }
		public string SgUniOrg { get; set; }
		public string CdUniOrgPai { get; set; }
		public int Nivel { get; set; }
	}
}
