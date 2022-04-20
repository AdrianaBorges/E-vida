using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	public class HcBeneficiarioVO {

		public int CdBeneficiario { get; set; }
		public string CdAlternativo { get; set; }
		public int CdEmpresa { get; set; }
		public string TpBeneficiario { get; set; }
		public long CdFuncionario { get; set; }
		public string NmBeneficiario { get; set; }
		public DateTime? DtDemissao { get; set; }
		public DateTime? DtValidadeCarteira { get; set; }
		public string TpEstadoCivil { get; set; }
		public string TpSexo { get; set; }
		public DateTime? DtNascimento { get; set; }
		public string CdGrauParentesco { get; set; }
		public string CdSituacaoBenef { get; set; }
		public string NmMae { get; set; }
		public long? NrCpf { get; set; }
		public int? CdDependente { get; set; }
		public DateTime DtAdmissao { get; set; }
		public string NmTitular { get; set; }
		public int CdBeneficiarioTitular { get; set; }
		public int? CdLocal { get; set; }
		public string CdLotacao { get; set; }
		public string Email { get; set; }
		public string NrCns { get; set; }

		public EnderecoVO Endereco { get; set; }
	}
}
