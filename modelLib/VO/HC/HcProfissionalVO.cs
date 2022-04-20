using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.HC {
	[Serializable]
	public class HcAnsProfissionalVO {
		public int NrSeqAnsProfissional { get; set; }
		public long NrCpf { get; set; }
		public string NmProfissional { get; set; }
		public string Email { get; set; }
		public List<ProfissionalConselhoVO> Conselhos { get; set; }

		public HcAnsProfissionalVO() {
			Conselhos = new List<ProfissionalConselhoVO>();
		}

		public static HcAnsProfissionalVO FromDataRow(DataRow dr) {
			HcAnsProfissionalVO vo = new HcAnsProfissionalVO();

			vo.NrSeqAnsProfissional = Convert.ToInt32(dr["NR_SEQ_ANS_PROFISSIONAL"]);
			vo.NrCpf = Convert.ToInt64(dr["NR_CPF"]);
			vo.NmProfissional = Convert.ToString(dr["NM_PROFISSIONAL"]);
			vo.Email = Convert.ToString(dr["DS_EMAIL"]);
			vo.Conselhos = new List<ProfissionalConselhoVO>();

			for (int i = 1; i <= 5; i++) {
				if (dr.IsNull("CD_CONSELHO_" + i)) break;
				ProfissionalConselhoVO conselho = new ProfissionalConselhoVO();
				conselho.Position = i;
				conselho.CdConselho = Convert.ToString(dr["CD_CONSELHO_" + i]);
				conselho.CdUf = Convert.ToString(dr["CD_UF_" + i]);
				conselho.NrConselho = Convert.ToString(dr["NR_CONSELHO_" + i]);
				vo.Conselhos.Add(conselho);
			}

			return vo;
		}

		public ProfissionalConselhoVO ConselhoPrincipal {
			get {
				if (Conselhos != null && Conselhos.Count > 0)
					return Conselhos[0];
				return null;
			}
		}
	}

	[Serializable]
	public class ProfissionalConselhoVO {
		public int Position { get; set; }
		public string CdConselho { get; set; }
		public string CdUf { get; set; }
		public string NrConselho { get; set; }
	}
}
