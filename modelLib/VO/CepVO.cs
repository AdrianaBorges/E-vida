using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class CepVO {
		public int Cep { get; set; }
		public string Uf { get; set; }
		public int IdLocalidade { get; set; }
		public string Cidade { get; set; }
		public string Bairro { get; set; }
		public string TipoLogradouro { get; set; }
		public string Rua { get; set; }
	}

	[Serializable]
	public class EnderecoVO : CepVO {
		public string NumeroEndereco { get; set; }
		public string Complemento { get; set; }
	}
}
