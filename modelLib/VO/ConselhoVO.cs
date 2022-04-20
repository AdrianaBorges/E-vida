using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class ConselhoVO {
		public string Codigo { get; set; }
		public string Nome { get; set; }
	}

	[Serializable]
	public class ArquivoConselhoVO {
		public string CodConselho { get; set; }
		public int IdArquivo { get; set; }
		public string NomeArquivo { get; set; }
		public string Descricao { get; set; }
	}
}
