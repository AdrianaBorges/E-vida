using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public class ExclusaoEnumTradutor {

		public static string TraduzStatus(StatusExclusao idStatus) {
			switch (idStatus) {
				case StatusExclusao.PENDENTE: return "PENDENTE";
				case StatusExclusao.APROVADO: return "APROVADO";
				case StatusExclusao.NEGADO: return "NEGADO";
				case StatusExclusao.AGUARDANDO_DOCUMENTACAO: return "AGUARDANDO DOCUMENTAÇÃO";
			}
			return " - INDEFINIDO -";
		}
	}
	public enum StatusExclusao {
		PENDENTE = 0,
		APROVADO = 2,
		NEGADO = 3,
		AGUARDANDO_DOCUMENTACAO = 4
	}
	[Serializable]
	public class ExclusaoVO {
		public const string FORMATO_PROTOCOLO = "000000000";

		public int CodSolicitacao { get; set; }
		public string Codint { get; set; }
        public string Codemp { get; set; }
		public String Matric { get; set; }
		public string Local { get; set; }
		public DateTime DataCriacao { get; set; }
		public StatusExclusao Status { get; set; }
		public DateTime? DataAlteracao { get; set; }
		public int? CodUsuarioAlteracao { get; set; }
		public string MotivoCancelamento { get; set; }
		public string NomeBeneficiarios { get; set; }
		public string Observacao { get; set; }
        public string Protocolo { get; set; } // exclusão

    }

    [Serializable]
    public class ExclusaoBeneficiario
    {
        public const string FORMATO_PROTOCOLO = "000000000";

        public int CodSolicitacao { get; set; }
        public string Email { get; set; }
        public string Carteirinha { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public string Assunto { get; set; }
        public string Texto { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Local { get; set; }
        public DateTime DataCriacao { get; set; }
        public string MotivoCancelamento { get; set; }
        public string Protocolo { get; set; }
        public string NomeBeneficiarios { get; set; }

    }

    [Serializable]
	public class ExclusaoBenefVO {
		public int CodSolicitacao { get; set; }
        public string Cdusuario { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
		public bool IsTitular { get; set; }
		public bool IsDepFamilia { get; set; }
		public string CodPlano { get; set; }
        public string Protocolo { get; set; } // protocolo de exclusão


        public string ToLog() {
            return "ExclusaoBenefVO { CodSolicitacao: " +
                   CodSolicitacao +
                   ", Codint: " +
                   Codint +
                   ", Codemp: " +
                   Codemp +
                   ", Matric: " +
                   Matric +
                   ", Tipreg: " +
                   Tipreg +
                   ", Tipreg: " +
                   "}";
		}
	}
}
