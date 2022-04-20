using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus {
	[Serializable]
	public class PBeneficiarioVO {
		public string CodInt { get; set; }
		public string CodEmp { get; set; }
		public string Matricula { get; set; }

		public string MatriculaVida { get; set; }
		public DateTime DtNascimento { get; set; }
		public DateTime DtValidadeCarteira { get; set; }
		public string Nome { get; set; }
		public string CdAlternativo { get; set; }
		public long? Cpf { get; set; }

		public string TpBeneficiario { get; set; }
		public string CdGrauParentesco { get; set; }
		public string Email { get; set; }
	}
}
