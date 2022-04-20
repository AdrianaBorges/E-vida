using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcVCredenciadoVO {
		public int CdCredenciado { get; set; }
		public string RazaoSocial { get; set; }
		public string NomeFantasia { get; set; }
		public string TipoPessoa { get; set; }
		public long CpfCnpj { get; set; }
		public string Email { get; set; }
		public string TipoSistemaAtendimento { get; set; }
		public string Situacao { get; set; }
		public string TipoValorPagar { get; set; }
		public string TipoPagamento { get; set; }
		public string TipoCobrancaFilme { get; set; }
		public int? CdNatureza { get; set; }


		public int? NrFormula { get; set; }
		public string TipoDias { get; set; }
	}

	public class HcCredenciadoFoneVO {
		public const string RESIDENCIAL = "RES";
		public const string COMERCIAL = "COM";
		public const string FAX = "FAX";
		public const string CELULAR = "CEL";
		public const string OUTROS = "OUT";

		public int CdCredenciado { get; set; }
		public string TpTelefone { get; set; }
		public int Ddd { get; set; }
		public string Telefone { get; set; }
		public string Ramal { get; set; }
	}

	[Serializable]
	public class HcCredenciadoEnderecoVO {
		public int CdCredenciado { get; set; }
		public string TpEndereco { get; set; }
		public string DsEndereco { get; set; }
		public string DsComplemento { get; set; }
		public string DsBairro { get; set; }
		public int? Cep { get; set; }
		public int? CdMunicipio { get; set; }
		public string Uf { get; set; }
	}
}
