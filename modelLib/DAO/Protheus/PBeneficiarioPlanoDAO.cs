using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	internal class PBeneficiarioPlanoDAO {

		static string FIELDS = @"cd_int, cd_emp, cd_plano, cd_matricula";

		internal static PBeneficiarioPlanoVO GetPlanoBeneficiario(string codInt, string codEmp, string matricula, EvidaDatabase db) {
			string sql = "select " + FIELDS +
                " from VW_PR_BENEF_PLANO WHERE trim(cd_int) = trim(:codInt) AND trim(cd_emp) = trim(:codEmp) AND trim(cd_matricula) = trim(:matricula) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":codInt", DbType.String, codInt));
			lstParams.Add(new Parametro(":codEmp", DbType.String, codEmp));
			lstParams.Add(new Parametro(":matricula", DbType.String, matricula));

			List<PBeneficiarioPlanoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;

		}

		private static PBeneficiarioPlanoVO FromDataRow(DataRow dr) {
			PBeneficiarioPlanoVO vo = new PBeneficiarioPlanoVO();
			vo.CodEmp = dr.Field<string>("cd_emp");
			vo.CodInt = dr.Field<string>("cd_int");
			vo.CodPlano = dr.Field<string>("cd_plano");
			vo.Matricula = dr.Field<string>("cd_matricula");
			return vo;
		}
	}
}
