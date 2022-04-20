using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	internal class PPlanoDAO {

		static string FIELDS = @"cd_int, cd_plano, ds_plano, ds_plano_reduzido, cd_plano_isa, cd_plano_susep, tp_abrangencia";

		internal static PPlanoVO GetByIsa(string cdPlanoIsa, EvidaDatabase db) {
			string sql = "select " + FIELDS +
                " from VW_PR_PLANO WHERE trim(cd_plano_isa) = trim(:cdPlanoIsa)";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":cdPlanoIsa", DbType.String, cdPlanoIsa));

			List<PPlanoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static List<PPlanoVO> ListarPlanos(EvidaDatabase db) {
			string sql = "select " + FIELDS +
				" from VW_PR_PLANO ORDER BY ds_plano";

			List<PPlanoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
			return lst;
		}

		internal static PPlanoVO Get(string cdInt, string cdPlano, EvidaDatabase db) {
			string sql = "select " + FIELDS +
                " from VW_PR_PLANO WHERE trim(cd_int) = trim(:cdInt) AND trim(cd_plano) = trim(:cdPlano) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":cdInt", DbType.String, cdInt));
			lstParams.Add(new Parametro(":cdPlano", DbType.String, cdPlano));

			List<PPlanoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static PPlanoVO FromDataRow(DataRow dr) {
			PPlanoVO vo = new PPlanoVO();
			vo.CodInt = dr.Field<string>("cd_int");
			vo.Abrangencia = dr.Field<string>("tp_abrangencia");
			vo.CdSusep = dr.Field<string>("cd_plano_susep");
			vo.Codigo = dr.Field<string>("cd_plano");
			vo.Nome = ProtheusDAOHelper.GetTrimString(dr, "ds_plano");
			vo.NomeReduzido = ProtheusDAOHelper.GetTrimString(dr, "ds_plano_reduzido");
			vo.CodIsa = dr.Field<string>("cd_plano_isa");
			return vo;
		}
	}
}
