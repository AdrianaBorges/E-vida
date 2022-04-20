using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcFuncionarioVO {
		public int CdEmpresa { get; set; }
		public long CdFuncionario { get; set; }
		public DateTime Admissao { get; set; }
		public string Nome { get; set; }
		public string Rg { get; set; }
		public long Cpf { get; set; }
		public DateTime Nascimento { get; set; }
		public string Sexo { get; set; }
		public string Email { get; set; }
		public string NomePai { get; set; }
		public string NomeMae { get; set; }
		public string TipoEstadoCivil { get; set; }
		public EnderecoVO Endereco { get; set; }
		public EnderecoVO EnderecoCob { get; set; }
		public int? CdLocal { get; set; }
		public string CdLotacao { get; set; }
		public int? CdEmpresaResponsavel { get; set; }

		public bool IsDeficienteFisico { get; set; }
		public string OrgaoExpedidorRg { get; set; }
		public string UfOrgaoExpedidorRg { get; set; }
		public DateTime? DataEmissaoRg { get; set; }

		public int? CdBanco { get; set; }
		public string CdAgencia { get; set; }
		public string ContaBancaria { get; set; }
		public string DvContaBancaria { get; set; }

		public string DddTelResidencial { get; set; }
		public string TelResidencial { get; set; }
		public string DddTelCelular { get; set; }
		public string TelCelular { get; set; }
		public string DddTelComercial { get; set; }
		public string TelComercial { get; set; }

		public string GetLogId() {
			return CdEmpresa + "/" + CdFuncionario;
		}
	}

}
