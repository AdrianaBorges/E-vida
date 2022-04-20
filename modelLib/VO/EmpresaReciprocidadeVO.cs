using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public class EmpresaReciprocidadeVO {
		public int Codigo { get; set; }
		public string Nome { get; set; }
		public List<string> AreaAtuacao { get; set; }
		public EnderecoVO Endereco { get; set; }
		public List<string> Email { get; set; }
		public List<string> Telefone { get; set; }		
		public List<string> Fax { get; set; }
		public string UrlGuia { get; set; }
		public string Contato { get; set; }
		public string AreaContato { get; set; }
		public string FuncaoContato { get; set; }
	}


}
