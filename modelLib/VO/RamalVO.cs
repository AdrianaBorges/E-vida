using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class RamalVO {
		public int NrRamal { get; set; }
		public string Alias { get; set; }
		public string Tipo { get; set; }
		public int? IdSetor { get; set; }
		public List<int> Usuarios { get; set; }
	}
}
