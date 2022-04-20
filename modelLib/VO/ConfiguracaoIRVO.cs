using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class ConfiguracaoIrVO {
		public bool EnableIrBeneficiario { get; set; }
		public bool EnableIrCredenciado { get; set; }
        public string EnderecoEVIDA { get; set; }
		public int DayIrBeneficiario { get; set; }
		public List<int> Anos { get; set; }
	}
}
