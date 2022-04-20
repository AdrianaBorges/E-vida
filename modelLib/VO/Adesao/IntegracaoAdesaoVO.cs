using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Adesao {
	public class IntegracaoAdesaoVO {
		public DeclaracaoVO Declaracao { get; set; }
		public HC.HcFuncionarioVO Funcionario { get; set; }
		public HC.HcBeneficiarioVO Titular { get; set; }
		public HC.HcBeneficiarioPlanoVO BeneficiarioPlanoCorrespondente { get; set; }
		public HC.HcBeneficiarioCategoriaVO CategoriaIntegracao { get; set; }
		public HC.HcBeneficiarioPlanoVO PlanoIntegracao { get; set; }
	}
}
