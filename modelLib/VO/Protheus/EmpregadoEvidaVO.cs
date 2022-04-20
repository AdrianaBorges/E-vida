using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus {
	public class EmpregadoEvidaVO {
		public long Matricula { get; set; }
		public string Nome { get; set; }
		public DateTime DataNascimento { get; set; }
		public string Cpf { get; set; }
		public string Rg { get; set; }
		public string RgOrg { get; set; }
		public string RgUf { get; set; }

		public string Email { get; set; }

		public string CodFuncao { get; set; }
		public string Funcao { get; set; }

		public string CodCentroCusto { get; set; }
		public string CentroCusto { get; set; }

		public string CodDepartamento { get; set; }
		public string Departamento { get; set; }

		public int RecNo { get; set; }
	}
}
