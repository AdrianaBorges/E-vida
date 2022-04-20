using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.DAO {
	internal class ControleProcessoDAO {
		private static long NextId(DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT SQ_EV_CONTROLE_PROCESSO.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (long)idSolicitacao;
		}

		internal static ControleProcessoVO GetAnteriorSucesso(ControleProcessoEnum processo, Database db) {
			string sql = "SELECT * FROM (SELECT * FROM EV_CONTROLE_PROCESSO CP " +
				" WHERE CP.CD_PROCESSO = :cod AND CP.ST_PROCESSO = :status" +
				"	AND DT_INICIO > TRUNC(SYSDATE) " +
				" ORDER BY CP.ID_PROCESSO DESC) WHERE ROWNUM = 1";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cod", Tipo = DbType.String, Value = processo.ToString() });
			lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.String, Value = StatusControleProcesso.SUCESSO.ToString() });

			List<ControleProcessoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static ControleProcessoVO FromDataRow(DataRow dr) {
			ControleProcessoVO vo = new ControleProcessoVO();
			vo.Id = Convert.ToInt64(dr["id_processo"]);
			vo.Processo = (ControleProcessoEnum)Enum.Parse(typeof(ControleProcessoEnum), Convert.ToString(dr["cd_processo"]));
			vo.Status = (StatusControleProcesso)Enum.Parse(typeof(StatusControleProcesso), Convert.ToString(dr["st_processo"]));
			vo.Inicio = Convert.ToDateTime(dr["dt_inicio"]);
			vo.Fim = !dr.IsNull("dt_fim") ? Convert.ToDateTime(dr["dt_fim"]) : new DateTime?();
			vo.Quantidade = Convert.ToInt32(dr["qt_registro"]);
			vo.Adicional = Convert.ToString(dr["ds_adicional"]);
			return vo;
		}

		internal static void AbortarAnterior(ControleProcessoEnum processo, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_CONTROLE_PROCESSO SET ST_PROCESSO = :status, DT_FIM = LOCALTIMESTAMP, DT_CHECK = LOCALTIMESTAMP " +
				" WHERE CD_PROCESSO = :cod AND ST_PROCESSO = :iniciando";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cod", DbType.String, processo.ToString());
			db.AddInParameter(dbCommand, ":status", DbType.String, StatusControleProcesso.ABORTADO.ToString());
			db.AddInParameter(dbCommand, ":iniciando", DbType.String, StatusControleProcesso.EXECUTANDO.ToString());

			db.ExecuteNonQuery(dbCommand, transaction);
		}

		internal static long RegistrarProcesso(ControleProcessoEnum processo, DbTransaction transaction, Database db) {

			long id = NextId(transaction, db);
			string sql = "INSERT INTO EV_CONTROLE_PROCESSO(ID_PROCESSO, CD_PROCESSO, DT_INICIO, DT_CHECK, QT_REGISTRO, ST_PROCESSO) " +
				" VALUES (:id, :cod, LOCALTIMESTAMP, LOCALTIMESTAMP, 0, :status)";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int64, id);
			db.AddInParameter(dbCommand, ":cod", DbType.String, processo.ToString());
			db.AddInParameter(dbCommand, ":status", DbType.String, StatusControleProcesso.EXECUTANDO.ToString());

			db.ExecuteNonQuery(dbCommand, transaction);
			return id;
		}

		internal static void FinalizarProcesso(long idProcesso, ControleProcessoEnum processo, int qtd, StatusControleProcesso status, string parametros, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_CONTROLE_PROCESSO SET ST_PROCESSO = :status, DT_FIM = LOCALTIMESTAMP, DT_CHECK = LOCALTIMESTAMP, " +
				" QT_REGISTRO = QT_REGISTRO + :qtd, DS_ADICIONAL = :params " +
				" WHERE CD_PROCESSO = :cod AND ID_PROCESSO = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int64, idProcesso);
			db.AddInParameter(dbCommand, ":qtd", DbType.Int32, qtd);
			db.AddInParameter(dbCommand, ":cod", DbType.String, processo.ToString());
			db.AddInParameter(dbCommand, ":status", DbType.String, status.ToString());
			db.AddInParameter(dbCommand, ":params", DbType.AnsiString, parametros);

			db.ExecuteNonQuery(dbCommand, transaction);
		}
	}
}
