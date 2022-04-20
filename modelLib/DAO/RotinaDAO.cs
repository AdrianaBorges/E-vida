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
	internal class RotinaDAO {
		static EVidaLog log = new EVidaLog(typeof(RotinaDAO));
		
		private static int NextExecId(EvidaDatabase db) {
			string sql = "SELECT SQ_EV_EXEC_ROTINA.nextval FROM DUAL";

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);

			return (int)idSolicitacao;
		}

		internal static List<RotinaVO> ListarRotinas(EvidaDatabase db) {
			string sql = "SELECT L.* FROM EV_ROTINA L" +
				" ORDER BY L.NM_ROTINA ";
			List<RotinaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowRotina);
			return lst;
		}

		internal static RotinaVO GetById(int idRotina, EvidaDatabase db) {
			string sql = "SELECT A.* " +
				" FROM EV_ROTINA A " +
				" WHERE A.ID_ROTINA = :id";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = idRotina });

			List<RotinaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowRotina, lstParams);
			if (lst == null || lst.Count == 0)
				return null;
			return lst[0];
		}

		private static RotinaVO FromDataRowRotina(DataRow dr) {
			RotinaVO vo = new RotinaVO();
			vo.Id = Convert.ToInt32(dr["id_rotina"]);
			vo.Nome = Convert.ToString(dr["nm_rotina"]);
			vo.Descricao = Convert.ToString(dr["ds_rotina"]);
			vo.Comando = Convert.ToString(dr["ds_comando"]);
			vo.Modulo = (Modulo)Convert.ToInt32(dr["id_modulo"]);
			vo.Tipo = (TipoRotinaEnum)Enum.Parse(typeof(TipoRotinaEnum), Convert.ToString(dr["tp_rotina"]));
			return vo;
		}

		private static ExecucaoRotinaVO FromDataRowExec(DataRow dr) {
			ExecucaoRotinaVO vo = new ExecucaoRotinaVO();
			vo.Id = Convert.ToInt32(dr["id_exec"]);
			vo.IdRotina = Convert.ToInt32(dr["id_rotina"]);
			vo.DataCriacao = Convert.ToDateTime(dr["dt_criacao"]);
			vo.IdUsuarioCriacao = Convert.ToInt32(dr["id_usuario"]);
			
			vo.Inicio = BaseDAO.GetNullableDate(dr, "dt_inicio");
			vo.Fim = BaseDAO.GetNullableDate(dr, "dt_fim");
			vo.Status = Convert.ToString(dr["st_registro"]);
			return vo;
		}

		internal static DataTable PesquisarHistorico(int idRotina, EvidaDatabase db) {
			string sql = "SELECT id_exec, er.DT_CRIACAO, dt_inicio, dt_fim, er.id_usuario, st_registro, ds_erro, ds_erro_sql, " +
				" c.nm_usuario " +
				" FROM EV_EXEC_ROTINA er, EV_USUARIO c " +
				" WHERE er.id_usuario = c.id_usuario ";

			List<Parametro> lstParams = new List<Parametro>();

			sql += " AND er.id_rotina = :idRotina";
			lstParams.Add(new Parametro(":idRotina", DbType.Int32, idRotina));
			
			sql += " ORDER BY dt_criacao DESC ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void SolicitarExecucao(int idRotina, int idUsuario, EvidaDatabase db) {
			string sql = "INSERT INTO EV_EXEC_ROTINA (ID_EXEC, ID_ROTINA, DT_CRIACAO, ID_USUARIO, ST_REGISTRO) " +
				" VALUES (:id, :idRotina, LOCALTIMESTAMP, :idUsuario, :status)";

			int id = NextExecId(db);

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, id));
			lstParams.Add(new Parametro(":idRotina", DbType.Int32, idRotina));
			lstParams.Add(new Parametro(":idUsuario", DbType.Int32, idUsuario));
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_PENDENTE));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static bool HasPendente(int idRotina, EvidaDatabase db) {
			string sql = "SELECT COUNT(1) FROM EV_EXEC_ROTINA ER WHERE ER.ID_ROTINA = :idRotina AND ST_REGISTRO = :status";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":idRotina", DbType.Int32, idRotina));
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_PENDENTE));

			int qtd = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
			return qtd > 0;
		}

		internal static List<ExecucaoRotinaVO> ListarExecPendente(EvidaDatabase db) {
			string sql = "SELECT * FROM EV_EXEC_ROTINA ER WHERE ST_REGISTRO = :status";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_PENDENTE));

			List<ExecucaoRotinaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowExec, lstParams);
			return lst;
		}

		internal static Exception Executar(RotinaVO vo, EvidaDatabase db) {
			try {
				BaseDAO.ExecuteNonQuery(vo.Comando, null, db);
				return null;
			} catch (Exception ex) {
				return ex;
			}
		}

		internal static void RegistrarInicio(ExecucaoRotinaVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_EXEC_ROTINA SET DT_INICIO = LOCALTIMESTAMP, ST_REGISTRO = :status " +
				" WHERE ID_EXEC = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_EXECUTANDO));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static void RegistrarOk(ExecucaoRotinaVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_EXEC_ROTINA SET DT_FIM = LOCALTIMESTAMP, ST_REGISTRO = :status " +
				" WHERE ID_EXEC = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_SUCESSO));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static void RegistrarErro(ExecucaoRotinaVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_EXEC_ROTINA SET DT_FIM = LOCALTIMESTAMP, ST_REGISTRO = :status, " +
				"	DS_ERRO = :erro, DS_ERRO_SQL = :erroSql " +
				" WHERE ID_EXEC = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_FALHA));
			lstParams.Add(new Parametro(":erro", DbType.String, vo.Erro));
			lstParams.Add(new Parametro(":erroSql", DbType.String, vo.ErroSQL));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static bool IsPendente(ExecucaoRotinaVO vo, EvidaDatabase db) {
			string sql = "SELECT COUNT(1) FROM EV_EXEC_ROTINA ER WHERE ID_EXEC = :id AND ST_REGISTRO = :status";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParams.Add(new Parametro(":status", DbType.String, ExecucaoRotinaVO.ST_PENDENTE));

			int qtd = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
			return qtd > 0;
		}
	}
}

