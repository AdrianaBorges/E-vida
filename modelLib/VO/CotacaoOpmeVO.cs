using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum StatusCotacaoOpme {
		ENVIADA,
		APROVADA,
		CANCELADA
	}

	public static class CotacaoOpmeTradutorHelper {

		public static string TraduzStatus(StatusCotacaoOpme status) {
			return status.ToString();
		}
	}
	[Serializable]
	public class CotacaoOpmeVO {
	}
}
