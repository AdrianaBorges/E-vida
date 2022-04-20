using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcBeneficiarioPlanoVO {
		public const string CARENCIA_ISENTO = "IT";
		public const string CARENCIA_NORMAL = "NOR";
		public const string FUNDO_RESERVA_ISENTO = "IT";
		public const string FUNDO_RESERVA_NORMAL = "NOR";

		public const string SIST_ATEND_CREDENCIAMENTO = "CRED";
		public const string SIST_ATEND_REEMBOLSO = "REEMB";
		public const string SIST_ATEND_CREDREEMB = "CREDRE";
		
		public int CdBeneficiario { get; set; }
		public string TpPlano { get; set; }
		public DateTime InicioVigencia { get; set; }
		public DateTime? FimVigencia { get; set; }
		public string CdPlanoVinculado { get; set; }
		public string CdPlanoEmpresa { get; set; }
		public string TpCarencia { get; set; }
		public string TpFundoReserva { get; set; }
		public string FlTemSubsidio { get; set; }
		public string TpSistemaAtend { get; set; }
		public string Observacao { get; set; }
		public int? CdMotivoDesligamento { get; set; }
	}
}
