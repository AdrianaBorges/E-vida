using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public enum StatusDeclaracaoUniversitario {
		PENDENTE = 1,
		APROVADO = 2,
		RECUSADO = 3
	}

	public class DeclaracaoUniversitarioEnumTradutor {

		public static string TraduzStatus(StatusDeclaracaoUniversitario status) {
			switch (status) {
				case StatusDeclaracaoUniversitario.PENDENTE: return "PENDENTE";
				case StatusDeclaracaoUniversitario.APROVADO: return "APROVADO";
				case StatusDeclaracaoUniversitario.RECUSADO: return "RECUSADO";
			}
			return "-";
		}
	}

	public class DeclaracaoUniversitarioVO {
		public const string FORMATO_PROTOCOLO = "0000000000";

		public int CodSolicitacao { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
		public string Matric { get; set; }
		public string Tipreg { get; set; }
		public string CodPlanoBeneficiario { get; set; }
		public string NomeArquivo { get; set; }
		public DateTime DataCriacao { get; set; }
		public StatusDeclaracaoUniversitario Status { get; set; }
		public int? CodUsuarioAlteracao { get; set; }
		public DateTime? DataAlteracao { get; set; }
		public string MotivoCancelamento { get; set; }

		public string ToLog() {
            return "DeclaracaoUniversitarioVO { CdSolicitacao: " + CodSolicitacao + ", Codint: " + Codint + ", Codemp: " + Codemp + ", Matric: " + Matric + ", Tipreg: " + Tipreg + ", DataCriacao: " + DataCriacao
				+ ", NomeArquivo: " + NomeArquivo + ", Status: " + Status + ", CodUsuarioAlteracao: " + CodUsuarioAlteracao + ", DataAlteracao: " + DataAlteracao + "}";
		}
	}

}
