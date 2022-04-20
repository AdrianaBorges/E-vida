using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	internal class CentroCustoDAO {
		internal static List<PrCentroCustoVO> ListarCentroCusto(Database db) {
			string sql = "SELECT * " +
				" FROM VW_PR_CTT010 " +
				" ORDER BY CTT_DESC01 ";

			List<PrCentroCustoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
			return lst;
		}

		private static PrCentroCustoVO FromDataRow(DataRow dr) {
			PrCentroCustoVO vo = new PrCentroCustoVO();
			vo.Codigo = dr.Field<string>("CTT_CUSTO");
			vo.Nome = dr.Field<string>("CTT_DESC01");

			vo.RecNo = Convert.ToInt32(dr["R_E_C_N_O_"]);

			vo.MatriculaResponsavel = Convert.ToInt64(dr["CCT_ZMAT"]);
			return vo;
		}
	}
}
