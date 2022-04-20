using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcBancoVO {
		public int Id { get; set; }
		public string Nome { get; set; }
        public string Descricao { get { return "(" + Id.ToString("0000") + ") " + Nome.Trim().ToUpper(); } }
		public bool AceitaDebitoConta { get; set; }
	}
	[Serializable]
	public class HcAgenciaBancariaVO {
		public int IdBanco { get; set; }
		public string CdAgencia { get; set; }
		public string Nome { get; set; }
		public string DvAgencia { get; set; }
	}
}
