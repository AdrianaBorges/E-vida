using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	public class HcDependenteVO {

		public int CdEmpresa { get; set; }
		public long CdFuncionario { get; set; }
		public DateTime DtAdmissao { get; set; }
		public int CdDependente { get; set; }
		public string NmDependente { get; set; }
		public string CdGrauParentesco { get; set; }
		public DateTime? DtNascimento { get; set; }
		public string TpSexo { get; set; }
		public string TpEstadoCivil { get; set; }
		public string TpDependente { get; set; }
		public string FlDeficienteFisico { get; set; }
		public string NmPai { get; set; }
		public string NmMae { get; set; }
		public long? NrCpf { get; set; }
		public string NrRg { get; set; }
		public string DsOrgExpRg { get; set; }
		public string CdUfOrgExpRg { get; set; }
		public DateTime? DataEmissaoRg { get; set; }

		public bool IsNew { get; set; }
	}
}
