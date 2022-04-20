using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	internal class DepartamentoDAO {
		internal static List<PrDepartamentoVO> ListarDepartamentos(Database db) {
			string sql = "SELECT * " +
				" FROM VW_PR_SQB010 " +
				" ORDER BY QB_DESCRIC ";

			List<PrDepartamentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
			return lst;
		}

		private static PrDepartamentoVO FromDataRow(DataRow dr) {
			PrDepartamentoVO vo = new PrDepartamentoVO();
			vo.Codigo = dr.Field<string>("QB_DEPTO");
			vo.Nome = dr.Field<string>("QB_DESCRIC");

			vo.RecNo = Convert.ToInt32(dr["R_E_C_N_O_"]);

			vo.MatriculaResponsavel = Convert.ToInt64(dr["QB_ZMAT"]);
			return vo;
		}
	}
}
