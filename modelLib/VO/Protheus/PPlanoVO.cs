using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus {
	[Serializable]
	public class PPlanoVO {
		public string CodInt { get; set; }
		public string Codigo { get; set; }
		public string Nome { get; set; }
		public string NomeReduzido { get; set; }
		public string CodIsa { get; set; }
		public string CdSusep { get; set; }
		public string Abrangencia { get; set; }
	}
}
