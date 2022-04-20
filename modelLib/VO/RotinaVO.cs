using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class RotinaVO {
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public string Comando { get; set; }
		public Modulo Modulo { get; set; }
		public TipoRotinaEnum Tipo { get; set; }
	}

	public enum TipoRotinaEnum {
		SQL,
		WINDOWS
	}

	[Serializable]
	public class ExecucaoRotinaVO {
		public const string ST_PENDENTE = "PENDENTE";
		public const string ST_EXECUTANDO = "EXECUTANDO";
		public const string ST_SUCESSO = "SUCESSO";
		public const string ST_FALHA = "FALHA";

		public int Id { get; set; }
		public int IdRotina { get; set; }
		public DateTime DataCriacao { get; set; }
		public int IdUsuarioCriacao { get; set; }
		public DateTime? Inicio { get; set; }
		public DateTime? Fim { get; set; }
		public string Status { get; set; }
		public string Erro { get; set; }
		public string ErroSQL { get; set; }
	}
}
