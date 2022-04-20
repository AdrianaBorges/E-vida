using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum TipoControleEmail {
		DECLARACAO_DEBITO_ANUAL = 1,
		PROTOCOLO_FATURA = 2
	}

	public enum StatusControleEmail {
		PENDENTE = 0,
		ENVIADO = 1,
		RETENTAR = 2,
		ERRO = 3
	}

	[Serializable]
	public class ControleEmailVO {
		public int Id { get; set; }
		public TipoControleEmail Tipo { get; set; }
		public KeyValuePair<string, string> Sender {get; set;}
		public List<KeyValuePair<string,string>> Destinatarios { get; set; }
		public List<string> Referencia { get; set; }
		public string Conteudo { get; set; }
		public List<string> Anexos { get; set; }
		public string Titulo { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime DataAgendamento { get; set; }
		public int QtdTentativa { get; set; }
		public DateTime? DataEnvio { get; set; }
		public string Erro { get; set; }
		public StatusControleEmail Situacao { get; set; }
	}
}
