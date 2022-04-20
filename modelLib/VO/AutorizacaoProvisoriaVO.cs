using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.VO {
	public class AutorizacaoProvisoriaEnumTradutor {
		/*
		 * 	CONSULTAS;
	EXAMES COMPLEMENTARES;
	ATENDIMENTO HOSPITALAR;
	PROCEDIMENTOS ODONTOLOGICOS;
	INTERNAÇÕES E CIRURGIAS;
	APENAS PROCEDIMENTOS COBERTOS PELO ROL DA ANS;
	NÃO VALIDO ODONTOLOGIA E FARMACIA
*/
		public static string TraduzProcedimentos(int idProcedimento) {
			switch (idProcedimento) {
				case 1: return "CONSULTAS";
				case 2: return "EXAMES COMPLEMENTARES";
				case 3: return "ATENDIMENTO HOSPITALAR";
				case 4: return "PROCEDIMENTOS ODONTOLOGICOS";
				case 5: return "INTERNAÇÕES E CIRURGIAS";
				case 6: return "APENAS PROCEDIMENTOS COBERTOS PELO ROL DA ANS";
				case 7: return "NÃO VÁLIDO ODONTOLOGIA E FARMÁCIA";
			}
			return " - INDEFINIDO -";
		}

		public static string TraduzSituacao(StatusAutorizacaoProvisoria status) {
			switch (status) {
				case StatusAutorizacaoProvisoria.APROVADO: return "GERADO";
				case StatusAutorizacaoProvisoria.PENDENTE: return "EM EDIÇÃO";
				case StatusAutorizacaoProvisoria.NEGADO: return "NEGADO/CANCELADO";
			}

			return " - INDEFINIDO -";
		}
	}

	[Serializable]
	public class PlantaoSocialLocalVO {
		public int Id { get; set; }
		public string Uf { get; set; }
		public string CodMunicipio { get; set; }
		public string Cidade { get; set; }
		public string Telefone { get; set; }
	}

	public enum StatusAutorizacaoProvisoria {
		PENDENTE = 0,
		APROVADO = 1,
		NEGADO = 2
	}

	public enum AbrangenciaAutorizacaoProvisoria {
		NACIONAL = 'N',
		REGIONAL = 'R'
	}

	[Serializable]
	public class AutorizacaoProvisoriaVO {
		public const string FORMATO_PROTOCOLO = "000000000";

		public int CodSolicitacao { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
        public PProdutoSaudeVO Plano { get; set; }
		public DateTime FimVigencia { get; set; }

		public PlantaoSocialLocalVO Local { get; set; }

		public List<int> Procedimentos { get; set; }

		public bool IsReciprocidade { get; set; }
		public List<string> Coberturas { get; set; }

		public StatusAutorizacaoProvisoria Status { get; set; }
		public AbrangenciaAutorizacaoProvisoria Abrangencia { get; set; }
		
		public DateTime DataCriacao { get; set; }
		public int CodUsuarioCriacao { get; set; }
		public DateTime? DataAlteracao { get; set; }
		public int? CodUsuarioAlteracao { get; set; }
		public DateTime? DataAprovacao { get; set; }
		public int? CodUsuarioAprovacao { get; set; }

		public string MotivoCancelamento { get; set; }
		public string Observacao { get; set; }
	}
}
