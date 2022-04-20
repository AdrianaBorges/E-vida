using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum TipoResponsavel {
		FINANCEIRO,
		FAMILIA
	}
	public class ResponsavelVO {
		public int CdBeneficiario { get; set; }
		public DateTime InicioVigencia { get; set; }
		public DateTime FimVigencia { get; set; }
		public int CdBeneficiarioFinanceiro { get; set; }
		public int CdBeneficiarioPlano { get; set; }
		public string Observacao { get; set; }
	}
}
