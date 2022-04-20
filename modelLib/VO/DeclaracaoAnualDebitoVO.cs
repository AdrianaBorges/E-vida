using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public enum StatusDeclaracaoAnualDebito {
		SOLICITADO = 0,
		GERADO = 1,
		ENVIADO = 2,
		ERRO = 3
	}

	public class DeclaracaoAnualDebitoEnumTradutor {
		public static string TraduzStatus(StatusDeclaracaoAnualDebito status) {
			switch (status) {
				case StatusDeclaracaoAnualDebito.SOLICITADO: return "SOLICITADO";
				case StatusDeclaracaoAnualDebito.GERADO: return "GERADO";
				case StatusDeclaracaoAnualDebito.ENVIADO: return "ENVIADO";
				case StatusDeclaracaoAnualDebito.ERRO: return "ERRO";
				default: return "INDEFINIDO";
			}
		}
	}

	[Serializable]
	public class DeclaracaoAnualDebitoVO {
		public int CodBeneficiario { get; set; }
		public int AnoRef { get; set; }
		public DateTime DataSolicitacao { get; set; }
		public int? CodUsuarioSolicitacao { get; set; }
		public StatusDeclaracaoAnualDebito Situacao { get; set; }
		public DateTime DataSituacao { get; set; }
		public string Erro { get; set; }
		public DateTime? DataEnvio { get; set; }
	}

	[Serializable]
	public class DeclaracaoAnualDebitoInfoVO {
		public int CdBeneficiario { get; set; }
		public int AnoRef { get; set; }

		public string NomeBeneficiario { get; set; }
		public long? Cpf { get; set; }
		public string NomePlano { get; set; }
	}
}
