using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcEmpresaVO {
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Status { get; set; }
		public string Tipo { get; set; }
		public string Sigla { get; set; }
	}
}
