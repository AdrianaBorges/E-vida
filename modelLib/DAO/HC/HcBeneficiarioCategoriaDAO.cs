using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO.HC {
	internal class HcBeneficiarioCategoriaDAO {
		internal static HcBeneficiarioCategoriaVO GetLastBeneficiarioData(long cdBeneficiario, DateTime? dtRef, EvidaDatabase db) {
			string sql = "select cd_beneficiario, bc.cd_categoria, dt_inicio_vigencia, dt_termino_vigencia, " +
				"	ds_categoria " +
				" from isa_hc.hc_beneficiario_categoria bc, isa_hc.hc_categoria cat " +
				"	WHERE cd_beneficiario = :cdBeneficiario AND bc.cd_categoria = cat.cd_categoria " +
				"		and dt_inicio_vigencia = (select max(dt_inicio_vigencia) from isa_hc.hc_beneficiario_categoria bc2 where bc.cd_beneficiario = bc2.cd_beneficiario AND bc2.dt_inicio_vigencia <= NVL(:dtRef,bc2.dt_inicio_vigencia) )";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int64, Value = cdBeneficiario });
			lstParams.Add(new Parametro() { Name = ":dtRef", Tipo = DbType.Date, Value = dtRef });

			List<HcBeneficiarioCategoriaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		
		}

		private static HcBeneficiarioCategoriaVO FromDataRow(DataRow dr) {
			HcBeneficiarioCategoriaVO vo = new HcBeneficiarioCategoriaVO();
			vo.CdBeneficiario = Convert.ToInt32(dr["cd_beneficiario"]);
			vo.CdCategoria = Convert.ToInt32(dr["cd_categoria"]);
			vo.InicioVigencia = dr.Field<DateTime>("dt_inicio_vigencia");
			vo.FimVigencia = dr.Field<DateTime?>("dt_termino_vigencia");
			vo.DsCategoria = Convert.ToString(dr["ds_categoria"]);
			return vo;
		}

		internal static void CriarBeneficiarioCategoria(HcBeneficiarioCategoriaVO benefCategoriaVO, EvidaDatabase evdb) {
			HcBeneficiarioCategoriaVO lastCategoria = GetLastBeneficiarioData(benefCategoriaVO.CdBeneficiario, null, evdb);

			if (lastCategoria != null) {
				if (lastCategoria.CdCategoria != benefCategoriaVO.CdCategoria || lastCategoria.InicioVigencia < benefCategoriaVO.InicioVigencia) {
					DesativarBeneficiarioCategoria(benefCategoriaVO.CdBeneficiario, benefCategoriaVO.InicioVigencia, evdb);
				}
			}
			string sql = "INSERT INTO ISA_HC.HC_BENEFICIARIO_CATEGORIA (CD_BENEFICIARIO, CD_CATEGORIA, DT_INICIO_VIGENCIA, " +
				"USER_CREATE, DATE_CREATE) VALUES " +
				" (:cdBenef, :cdCategoria, :dtInicio, :userInt, :dateInt) ";
			Database db = evdb.Database;

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBenef", DbType.Int32, benefCategoriaVO.CdBeneficiario);

			db.AddInParameter(dbCommand, ":cdCategoria", DbType.Int32, benefCategoriaVO.CdCategoria);
			db.AddInParameter(dbCommand, ":dtInicio", DbType.Date, benefCategoriaVO.InicioVigencia);

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO");
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static void DesativarBeneficiarioCategoria(int cdBeneficiario, DateTime inicio, EvidaDatabase evdb) {

			string sql = "UPDATE ISA_HC.HC_BENEFICIARIO_CATEGORIA SET DT_TERMINO_VIGENCIA = :dtInicio-1,  " +
				" USER_UPDATE = :userInt, DATE_UPDATE = :dateInt " + 
				" WHERE CD_BENEFICIARIO = :cdBenef AND (DT_TERMINO_VIGENCIA IS NULL OR DT_TERMINO_VIGENCIA >= :dtInicio) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBenef", DbType.Int32, cdBeneficiario);
			db.AddInParameter(dbCommand, ":dtInicio", DbType.Date, inicio);

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO");
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
