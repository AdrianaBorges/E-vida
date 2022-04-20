using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Filter {
	public class FilterAutorizacaoVO {
		public int? Id { get; set; }
		public int? NroAutorizacaoTiss { get; set; }
		public StatusAutorizacao? Status { get; set; }
		public bool? EmAndamento { get; set; }
		public OrigemAutorizacao? Origem { get; set; }

        public Protheus.PUsuarioVO Beneficiario { get; set; }
		public Protheus.PRedeAtendimentoVO Credenciado { get; set; }
		public Protheus.PProfissionalSaudeVO Profissional { get; set; }

		public CaraterAutorizacao? Carater { get; set; }
		public string CodDoenca { get; set; }
		public string NomeDoenca { get; set; }
		public bool? Internacao { get; set; }
		public DateTime? DataInternacao { get; set; }
		public string Hospital { get; set; }
		public string Indicacao { get; set; }
        public bool? Tfd { get; set; }
		public string CodServico { get; set; }
		public bool? Opme { get; set; }

		public List<int> CodUsuarioCriacao { get; set; }

		public TipoAutorizacao? Tipo { get; set; }

		public int? CodUsuarioResponsavel { get; set; }

		public string ProtocoloAns { get; set; }
	}
}
