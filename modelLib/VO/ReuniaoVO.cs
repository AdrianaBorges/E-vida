using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class ReuniaoVO {
		public int Id { get; set; }
		public string CodConselho { get; set; }
		public string Titulo { get; set; }
		public string Descricao { get; set; }
		public DateTime Data { get; set; }
		public string Email { get; set; }
		public string AssuntoEmail { get; set; }
	}

	[Serializable]
	public class ArquivoReuniaoVO {
		public int CodReuniao { get; set; }
		public int IdArquivo { get; set; }
		public string NomeArquivo { get; set; }
		public string Descricao { get; set; }
	}
}
