using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum FormNegativaStatus {
		SOB_ANALISE,
		APROVADO,
		CANCELADO
	}

	public enum FormNegativaReanaliseStatus {
		SOB_ANALISE,
		FINALIZADO,
		DEVOLVIDO
	}

	public class FormNegativaVO {
		public const string FORMATO_PROTOCOLO = "000000000";
		public const string ACOMODACAO_ENFERMARIA = "ENF";
		public const string ACOMODACAO_APARTAMENTO = "APT";

		public int CodSolicitacao { get; set; }
		public int InfoDispositivoLegal { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
		public string PadraoAcomodacao { get; set; }
		public string TipoRede { get; set; }
		public string DescricaoSolicitacao { get; set; }
		public string PrevisaoContratual { get; set; }
		public DateTime DataFormulario { get; set; }
		public int IdUsuario { get; set; }
		public int IdUsuarioUpdate { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime DataAlteracao { get; set; }
		public string Status { get; set; }
		public string MotivoCancelamento { get; set; }
		public PRedeAtendimentoVO Prestador { get; set; }
		public string ProtocoloAns { get; set; }
		public PProfissionalSaudeVO Profissional { get; set; }

		public int? IdMotivoGlosa { get; set; }
		public string NrContrato { get; set; }
		public DateTime? DataSolicitacao { get; set; }

		public List<FormNegativaItemVO> Itens { get; set; }
		public List<FormNegativaJustificativaVO> JustContratual { get; set; }
		public List<FormNegativaJustificativaVO> JustAssistencial { get; set; }

		public FormNegativaReanaliseVO Reanalise { get; set; }
	}

	public class FormNegativaItemVO {
        public string Codpad { get; set; }
		public string Codpsa { get; set; }
		public string Descri { get; set; }
		public int Quantidade { get; set; }
		public string Observacao { get; set; }
	}

	public class FormNegativaJustificativaVO {
		public int IdJustificativa { get; set; }
		public string Parametros { get; set; }
	}

	public class FormNegativaInfoAdicionalVO {
		public string NomeBeneficiario { get; set; }
		public string Cartao { get; set; }
		public string Cpf { get; set; }
		public DateTime DataNascimento { get; set; }
		public DateTime DataAdesao { get; set; }
		public Protheus.PProdutoSaudeVO Plano { get; set; }
		public string NrContrato { get; set; }
	}

	public class MotivoGlosaVO {
		public int Id { get; set; }
		public string Grupo { get; set; }
		public string Descricao { get; set; }
	}

	public class FormNegativaReanaliseVO {
		public int Id { get; set; }
		public string ProtocoloAns { get; set; }
		public DateTime DataFormulario { get; set; }
		public string JustificativaNegativa { get; set; }
		public int Parecer { get; set; }
		public string ObservacaoParecer { get; set; }
		public int IdUsuario { get; set; }
		public int IdUsuarioUpdate { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime DataAlteracao { get; set; }
		public FormNegativaReanaliseStatus Status { get; set; }
		public string ObservacaoDevolucao { get; set; }
	}
}
