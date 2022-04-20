using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcPlanoVO {
		public string CdPlano { get; set; }
		public string DsPlano { get; set; }
		public string TpPlano { get; set; }
		public string VlGrauImportancia { get; set; }
		public string CdPlanoAns { get; set; }
		public string TpAbrangencia { get; set; }
		public string DsSigla { get; set; }
		public string LocalAtendPadrao { get; set; }

		public static HcPlanoVO FromDataRow(DataRow dr) {
			HcPlanoVO vo = new HcPlanoVO();
			vo.CdPlano = Convert.ToString(dr["CD_PLANO"]);
			vo.DsPlano = Convert.ToString(dr["DS_PLANO"]);
			vo.TpPlano = Convert.ToString(dr["TP_PLANO"]);
			vo.VlGrauImportancia = Convert.ToString(dr["VL_GRAU_IMPORTANCIA"]);
			vo.CdPlanoAns = Convert.ToString(dr["CD_PLANO_ANS"]);
			vo.TpAbrangencia = Convert.ToString(dr["TP_ABRANGENCIA"]);
			vo.DsSigla = Convert.ToString(dr["DS_SIGLA"]);
			vo.LocalAtendPadrao = Convert.ToString(dr["CD_LOCAL_ATEND_PADRAO"]);
			return vo;
		}
	}
}
