using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcServicoVO {
		public int CdServico { get; set; }
		public string DsServico { get; set; }
		public string CdTabela { get; set; }
		public string CdMascara { get; set; }
		public int? CdServicoPai { get; set; }


		public static HcServicoVO FromDataRow(DataRow dr) {
			HcServicoVO vo = new HcServicoVO();
			vo.CdServico = Convert.ToInt32(dr["CD_SERVICO"]);
			vo.CdMascara = Convert.ToString(dr["CD_MASCARA"]);
			vo.CdTabela = Convert.ToString(dr["CD_TABELA"]);
			vo.DsServico = Convert.ToString(dr["DS_SERVICO"]);
			vo.CdServicoPai = dr.IsNull("CD_SERVICO_PAI") ? new int?() : Convert.ToInt32(dr["CD_SERVICO_PAI"]);
			return vo;
		}
	}
}
