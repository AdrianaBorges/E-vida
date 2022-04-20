using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public class BilhetagemVO {
		public long Id { get; set; }
		public DateTime DataBilhetagem { get; set; }
		public string Direcao { get; set; }
		public string DuracaoRaw { get; set; }
		public int Duracao { get; set; }
		public string Origem { get; set; }
		public string OrigemRaw { get; set; }
		public string Destino { get; set; }
		public string DestinoRaw { get; set; }
		public string Juntor { get; set; }
		public string Conta { get; set; }
		public string Estado { get; set; }
		public string Conexao { get; set; }
		public DateTime DataImportacao { get; set; }
		public long NumeroBilhete { get; set; }

		public string Arquivo { get; set; }
		public string Linha { get; set; }
	}
}
