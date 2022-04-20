using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum TipoArquivoSapEnum {
		PPRS,
		FAMILIA,
		EVIDA_VIDA
	}

	public class ArquivoSapEnumTradutor {
		public static string TraduzTipoArquivo(TipoArquivoSapEnum tipo) {
			switch (tipo) {
				case TipoArquivoSapEnum.PPRS: return "PPRS";
				case TipoArquivoSapEnum.FAMILIA: return "FAMILIA";
				case TipoArquivoSapEnum.EVIDA_VIDA: return "+VIDA";
				default: return "-";
			}
		}
	}

	public class ArquivoSapVerbaVO {
		public TipoArquivoSapEnum TipoArq { get; set; }
		public int Verba { get; set; }
		public string GrupoLancamento { get; set; }
	}

	public class ArquivoSapVO {
		public const string ST_IMPORTADO = "IM";
		public const string ST_CANCELADO = "CA";
		public const string ST_QUITADO = "QD";
		
		public long IdArquivo { get; set; }
		public DateTime DataFolha { get; set; }
		public int Seq { get; set; }
		public string Nome { get; set; }
		public DateTime DataImportacao { get; set; }
		public DateTime DataRecebimento { get; set; }
		public string Status { get; set; }
		public TipoArquivoSapEnum TipoArquivo { get; set; }

		public List<ArquivoSapItemVO> Items { get; set; }
	}
	
	public class ArquivoSapItemVO {
		public long IdArquivo { get; set; }
		public int SeqItem { get; set; }
		public int CdEmpresa { get; set; }
		public long CdFuncionario { get; set; }
		public DateTime DataReferencia { get; set; }
		public int Verba { get; set; }
		public double Valor { get; set; }
	}
}
