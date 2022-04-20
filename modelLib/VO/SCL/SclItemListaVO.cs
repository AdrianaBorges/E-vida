using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.SCL {
	[Serializable]
	public class SclItemListaVO {
		public string CdSistema { get; set; }
		public string CdLista { get; set; }
		public string Id { get; set; }
		public string Descricao { get; set; }
		public int OrdemExibicao { get; set; }
		public bool Unico { get; set; }
		public bool Ativo { get; set; }
	}
}
