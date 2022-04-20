using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class ArquivoTelaVO {
		public string Id { get; set; }
		public string NomeFisico { get; set; }
		public string NomeTela { get; set; }
		public bool IsNew { get; set; }
		public string Descricao { get; set; }
		public Dictionary<string,string> Parameters { get; set; }

		public object InternalVO { get; set; }
	}
}
