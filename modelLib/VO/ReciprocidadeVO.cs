using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public class ReciprocidadeEnumTradutor {

		public static string TraduzAssistencia(int idAssistencia) {
			switch (idAssistencia) {
				case 1: return "CONSULTAS";
				case 2: return "MEDICO-HOSPITALAR";
				case 3: return "EXAMES COMPLEMENTARES";
				case 4: return "INTERNAÇÕES E CIRURGIAS";
			}
			return " - INDEFINIDO -";
		}

        public static string TraduzSituacao(SituacaoReciprocidade situacao)
        {
            switch (situacao)
            {
                case SituacaoReciprocidade.NORMAL: return "NORMAL";
                case SituacaoReciprocidade.ALERTA: return "ALERTA";
                case SituacaoReciprocidade.CRITICA: return "CRÍTICA";
                default:
                    break;
            }
            return situacao.ToString();
        }   

	}
	public enum StatusReciprocidade
	{
		PENDENTE = 0,
		ENVIADO = 1,
		APROVADO = 2,
		NEGADO = 3
	}

    public enum SituacaoReciprocidade
    {
        NORMAL = 0,
        ALERTA = 1,
        CRITICA = 2
    }   

	public class ReciprocidadeVO {
		public const string FORMATO_PROTOCOLO = "000000000";

		public int CodSolicitacao { get; set; }
		public string Codint { get; set; }
        public string Codemp { get; set; }
		public string Matric { get; set; }
		public DateTime Inicio { get; set; }
		public DateTime Fim { get; set; }
		public EnderecoVO Endereco { get; set; }
		public string Local { get; set; }
		public string CodintReciprocidade { get; set; }
		public List<int> Assistencia { get; set; }
		public string Observacao { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime? DataEnvio { get; set; }
		public StatusReciprocidade Status { get; set; }
		public DateTime DataAlteracao { get; set; }
		public int CodUsuarioAlteracao { get; set; }
		public string MotivoCancelamento { get; set; }
		public int CodUsuarioEnvio { get; set; }
		public int CodUsuarioAprovacao { get; set; }
		public DateTime? DataAprovacao { get; set; }
		public string ObservacaoAprovacao { get; set; }
		public string ArquivoAprovacao { get; set; }
		public string ProtocoloAns { get; set; }
        public SituacaoReciprocidade Situacao { get; set; }
	}


	public class ReciprocidadeBenefVO {
		public int CodSolicitacao { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
		public bool IsTitular { get; set; }
		public string TipoPlano { get; set; }
		public string InicioPlano { get; set; }

		public string ToLog() {
            return "ReciprocidadeBenefVO { CodSolicitacao: " + CodSolicitacao + ", Codint: " + Codint + ", Codemp: " + Codemp + ", Matric: " + Matric + ", Tipreg: " + Tipreg + "}";
		}
	}
}
