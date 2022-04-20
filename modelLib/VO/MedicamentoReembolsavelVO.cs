using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public class PrincipioAtivoVO {
		public int Id { get; set; }
		public string Descricao { get; set; }
		public int IdUsuarioCriacao { get; set; }
		public DateTime DataCriacao { get; set; }
		public int? IdUsuarioAlteracao { get; set; }
		public DateTime? DataAlteracao{ get; set; }
	}

	public class MedicamentoReembolsavelVO {
		public string Mascara { get; set; }
		public string Descricao { get; set; }
		public int IdPrincipioAtivo { get; set; }
		public List<string> Planos { get; set; }
		public bool Reembolsavel { get; set; }
		public bool UsoContinuo { get; set; }
		public string Obs { get; set; }
		public int IdUsuarioCriacao { get; set; }
		public DateTime DataCriacao { get; set; }
		public int IdUusarioAlteracao { get; set; }
		public DateTime DataAlteracao { get; set; }

		/*Planos com Cobertura (checklist para indicar os planos que têm cobertura do medicamento)
Reembolsável (S/N)
Uso Contínuo (S/N)
Observações (campo texto livre multi-linha)
Anexos (lista de arquivos)
Cadastrado Por (usuario da intranet e data, controlados pelo sistema)
Alterado Por (usuario da intranet e data, controlados pelo sistema)
Principio Ativo (campo texto livre)
*/
	}

	public class MedicamentoReembolsavelArqVO {
		public int IdArquivo { get; set; }
		public string Mascara { get; set; }		
		public string NomeArquivo { get; set; }
		public DateTime DataEnvio { get; set; }
		public int IdUsuarioEnvio { get; set; }
	}
}
