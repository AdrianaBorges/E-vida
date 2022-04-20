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
	internal class HcBeneficiarioPlanoDAO {
		internal static HcBeneficiarioPlanoVO GetByBeneficiarioData(long cdBeneficiario, DateTime data, Database db) {
			string sql = "select cd_beneficiario, tp_plano, dt_inicio_vigencia, dt_termino_vigencia, cd_plano_vinculado, " +
				" cd_plano_empresa, tp_carencia, ds_observacao " +
				" from isa_hc.hc_beneficiario_plano WHERE cd_beneficiario = :cdBeneficiario " +
				" AND trunc(:data) BETWEEN dt_inicio_vigencia AND NVL(dt_termino_vigencia,SYSDATE+1)";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int64, Value = cdBeneficiario });
			lstParams.Add(new Parametro() { Name = ":data", Tipo = DbType.Date, Value = data });

			List<HcBeneficiarioPlanoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static HcBeneficiarioPlanoVO GetLastBeneficiarioData(long cdBeneficiario, DateTime? dtRef, EvidaDatabase db) {
			string sql = "select cd_beneficiario, tp_plano, dt_inicio_vigencia, dt_termino_vigencia, cd_plano_vinculado,  cd_plano_empresa, tp_carencia, ds_observacao  " +
				"	from isa_hc.hc_beneficiario_plano bp WHERE cd_beneficiario = :cdBeneficiario " +
				"		and dt_inicio_vigencia = (select max(dt_inicio_vigencia) from isa_hc.hc_beneficiario_plano bp2 where bp.cd_beneficiario = bp2.cd_beneficiario AND bp2.dt_inicio_vigencia <= NVL(:dtRef,bp2.dt_inicio_vigencia) )";


			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int64, Value = cdBeneficiario });
			lstParams.Add(new Parametro() { Name = ":dtRef", Tipo = DbType.Date, Value = dtRef });

			List<HcBeneficiarioPlanoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		
		}

		private static HcBeneficiarioPlanoVO FromDataRow(DataRow dr) {
			HcBeneficiarioPlanoVO vo = new HcBeneficiarioPlanoVO();
			vo.CdBeneficiario = Convert.ToInt32(dr["cd_beneficiario"]);
			vo.TpPlano = Convert.ToString(dr["tp_plano"]);
			vo.InicioVigencia = dr.Field<DateTime>("dt_inicio_vigencia");
			vo.FimVigencia = dr.Field<DateTime?>("dt_termino_vigencia");
				//dr["dt_termino_vigencia"] != DBNull.Value ? Convert.ToDateTime(dr["dt_termino_vigencia"]) : new DateTime?();
			vo.CdPlanoVinculado = Convert.ToString(dr["cd_plano_vinculado"]);
			vo.CdPlanoEmpresa = dr.Field<string>("cd_plano_empresa");
			vo.TpCarencia = dr.Field<string>("tp_carencia");
			vo.Observacao = dr.Field<string>("ds_observacao");
			return vo;
		}
		
		internal static void CriarBeneficiarioPlano(HcBeneficiarioPlanoVO benefPlanoVO, EvidaDatabase evdb) {
			HcBeneficiarioPlanoVO lastPlano = GetLastBeneficiarioData(benefPlanoVO.CdBeneficiario, null, evdb);
			if (lastPlano != null) {
				if (!lastPlano.CdPlanoVinculado.Equals(benefPlanoVO.CdPlanoVinculado) || !lastPlano.TpPlano.Equals(benefPlanoVO.TpPlano)) {
					DesativarBeneficiarioPlano(benefPlanoVO.CdBeneficiario, benefPlanoVO.InicioVigencia, benefPlanoVO.CdMotivoDesligamento.Value, evdb);
				}
			}
			string sql = "INSERT INTO ISA_HC.HC_BENEFICIARIO_PLANO (CD_BENEFICIARIO, TP_PLANO, CD_PLANO_VINCULADO, CD_PLANO_EMPRESA, DT_INICIO_VIGENCIA, " +
				" TP_CARENCIA, TP_FUNDO_RESERVA, FL_TEM_SUBSIDIO, TP_SISTEMA_ATEND_LIBERADO,FL_MENSALIDADE_LIMITADA, DS_OBSERVACAO, USER_CREATE, DATE_CREATE) VALUES " +
				" (:cdBenef, :tpPlano, :cdPlano, :cdPlanoEmpresa, :dtInicio, :tpCarencia, :tpFundoReserva, :flSubsidio, :tpSistemaAtend, :flMensalidadeLimitada, :obs, :userInt, :dateInt) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBenef", DbType.Int32, benefPlanoVO.CdBeneficiario);

			db.AddInParameter(dbCommand, ":tpPlano", DbType.String, benefPlanoVO.TpPlano);
			db.AddInParameter(dbCommand, ":cdPlano", DbType.String, benefPlanoVO.CdPlanoVinculado);
			db.AddInParameter(dbCommand, ":cdPlanoEmpresa", DbType.String, benefPlanoVO.CdPlanoEmpresa);
			db.AddInParameter(dbCommand, ":dtInicio", DbType.Date, benefPlanoVO.InicioVigencia);

			db.AddInParameter(dbCommand, ":tpCarencia", DbType.String, benefPlanoVO.TpCarencia);
			db.AddInParameter(dbCommand, ":tpFundoReserva", DbType.String, benefPlanoVO.TpFundoReserva);
			db.AddInParameter(dbCommand, ":flSubsidio", DbType.String, benefPlanoVO.FlTemSubsidio);
			db.AddInParameter(dbCommand, ":tpSistemaAtend", DbType.String, benefPlanoVO.TpSistemaAtend);
			db.AddInParameter(dbCommand, ":flMensalidadeLimitada", DbType.String, "N");
			db.AddInParameter(dbCommand, ":obs", DbType.String, benefPlanoVO.Observacao);

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO");
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static void DesativarBeneficiarioPlano(int cdBeneficiario, DateTime inicio, int cdMotivoDesligamento, EvidaDatabase evdb) {
			string sql = "UPDATE ISA_HC.HC_BENEFICIARIO_PLANO SET DT_TERMINO_VIGENCIA = :dtInicio-1, CD_MOTIVO_DESLIGAMENTO = :motivoDesligamento, " +
				" USER_UPDATE = :userInt, DATE_UPDATE = :dateInt " +
				" WHERE CD_BENEFICIARIO = :cdBenef AND (DT_TERMINO_VIGENCIA IS NULL OR DT_TERMINO_VIGENCIA >= :dtInicio) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBenef", DbType.Int32, cdBeneficiario);
			db.AddInParameter(dbCommand, ":dtInicio", DbType.Date, inicio);
			db.AddInParameter(dbCommand, ":motivoDesligamento", DbType.Int32, cdMotivoDesligamento);

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO");
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
