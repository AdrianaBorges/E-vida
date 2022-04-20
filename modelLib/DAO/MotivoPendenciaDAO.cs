using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class MotivoPendenciaDAO {
		private static EVidaLog log = new EVidaLog(typeof(MotivoPendenciaDAO));

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT NVL(MAX(CD_MOTIVO_PENDENCIA),0)+1 FROM EV_MOTIVO_PENDENCIA ";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		internal static List<MotivoPendenciaVO> ListarMotivos(TipoMotivoPendencia? tipo, EvidaDatabase db) {
			string sql = "SELECT cd_motivo_pendencia, ds_motivo_pendencia, tp_motivo_pendencia FROM ev_motivo_pendencia ";

			List<Parametro> lstParams = new List<Parametro>();
			if (tipo != null) {
				sql += " WHERE tp_motivo_pendencia = :tipo ";
				lstParams.Add(new Parametro(":tipo", DbType.Int32, (int)tipo.Value));
			}

			sql += " ORDER BY ds_motivo_pendencia";

			List<MotivoPendenciaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			return lst;
		}

		internal static MotivoPendenciaVO GetById(int id, EvidaDatabase db) {
			string sql = "SELECT cd_motivo_pendencia, ds_motivo_pendencia, tp_motivo_pendencia FROM ev_motivo_pendencia where cd_motivo_pendencia = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, id));

			List<MotivoPendenciaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static MotivoPendenciaVO FromDataRow(DataRow dr) {
			MotivoPendenciaVO vo = new MotivoPendenciaVO();
			vo.Id = Convert.ToInt32(dr["cd_motivo_pendencia"]);
			vo.Nome = Convert.ToString(dr["ds_motivo_pendencia"]);
			vo.Tipo = (TipoMotivoPendencia)Convert.ToInt32(dr["tp_motivo_pendencia"]);
			return vo;
		}

		internal static void Salvar(MotivoPendenciaVO vo, EvidaDatabase evdb) {		
			DbCommand dbCommand = null;
			if (vo.Id == 0) {
				dbCommand = CreateInsert(vo, evdb);
			} else {
				dbCommand = CreateUpdate(vo, evdb);
			}

			Database db = evdb.Database;
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":tipo", DbType.Int32, (int)vo.Tipo);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Nome);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static DbCommand CreateInsert(MotivoPendenciaVO vo, EvidaDatabase db) {
			string sql = "INSERT INTO EV_MOTIVO_PENDENCIA (cd_motivo_pendencia, ds_motivo_pendencia, tp_motivo_pendencia) " +
				" VALUES (:id, :nome, :tipo)";

			vo.Id = NextId(db);

			DbCommand dbCommand = db.Database.GetSqlStringCommand(sql);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(MotivoPendenciaVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_MOTIVO_PENDENCIA SET ds_motivo_pendencia = :nome, tp_motivo_pendencia = :tipo " +
				"	WHERE cd_motivo_pendencia = :id";

			DbCommand dbCommand = db.Database.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void Excluir(int id, EvidaDatabase db) {
			string sql = "DELETE FROM EV_MOTIVO_PENDENCIA " +
				"	WHERE cd_motivo_pendencia = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, id));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

	}
}
