using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus {
	public class PServicoVO {
		//select cd_mascara, cd_tabela, ds_servico, cd_rol_ans
		public string Mascara { get; set; }
		public string Tabela { get; set; }
		public string Descricao { get; set; }
		public string CdRolAns { get; set; }
	}
}
