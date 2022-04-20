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
	internal class BilhetagemDAO {
		static EVidaLog log = new EVidaLog(typeof(BilhetagemDAO));

		private static long NextId(DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT SQ_BILHETAGEM.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (long)idSolicitacao;
		}
		
		private static BilhetagemVO FromDataRow(DataRow dr) {
			BilhetagemVO vo = new BilhetagemVO();
			vo.Id = Convert.ToInt64(dr["id_registro"]);
			vo.Conexao = dr.Field<string>("ds_conexao");
			vo.Conta = dr.Field<string>("ds_conta");
			vo.DataBilhetagem = Convert.ToDateTime(dr["dt_bilhetagem"]);
			vo.DataImportacao = Convert.ToDateTime(dr["dt_importacao"]);
			vo.Destino = Convert.ToString(dr["ds_tel_destino"]);
			vo.DestinoRaw = dr.Field<string>("ds_destino");
			vo.Direcao = dr.Field<string>("tp_direcao");
			vo.DuracaoRaw = dr.Field<string>("ds_duracao");
			vo.Duracao = Convert.ToInt32(dr["nr_duracao"]);
			vo.Estado = dr.Field<string>("ds_estado");
			vo.Juntor = dr.Field<string>("ds_juntor");
			vo.Origem = Convert.ToString(dr["ds_tel_origem"]);
			vo.OrigemRaw = dr.Field<string>("ds_origem");

			vo.Arquivo = dr.Field<string>("ds_arquivo");
			return vo;
		}
		
		/*
		internal static void IncluirBilhetagem(BilhetagemVO vo, DbTransaction transaction, Database db) {

			long id = NextId(transaction, db);
			string sql = "INSERT INTO EV_BILHETAGEM(id_registro, DT_BILHETAGEM, TP_DIRECAO, DS_DURACAO, DS_ORIGEM, DS_DESTINO, DS_JUNTOR, " +
				"	DS_CONTA, DS_ESTADO, DS_CONEXAO, DT_IMPORTACAO, DS_TEL_ORIGEM, DS_TEL_DESTINO, NR_DURACAO, DS_ARQUIVO, DS_LINHA) " +
				" VALUES (:id, :data, :direcao, :duracaoRaw, :origemRaw, :destinoRaw, :juntor, " +
				"	:conta, :estado, :conexao, LOCALTIMESTAMP, :origem, :destino, :duracao, :arquivo, :linha)";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int64, id);
			db.AddInParameter(dbCommand, ":data", DbType.DateTime, vo.DataBilhetagem);
			db.AddInParameter(dbCommand, ":direcao", DbType.String, vo.Direcao);
			db.AddInParameter(dbCommand, ":duracaoRaw", DbType.String, vo.DuracaoRaw);
			db.AddInParameter(dbCommand, ":origemRaw", DbType.String, vo.OrigemRaw);
			db.AddInParameter(dbCommand, ":destinoRaw", DbType.String, vo.DestinoRaw);
			db.AddInParameter(dbCommand, ":juntor", DbType.String, vo.Juntor);

			db.AddInParameter(dbCommand, ":conta", DbType.String, vo.Conta);
			db.AddInParameter(dbCommand, ":estado", DbType.String, vo.Estado);
			db.AddInParameter(dbCommand, ":conexao", DbType.String, vo.Conexao);

			db.AddInParameter(dbCommand, ":origem", DbType.String, vo.Origem);
			db.AddInParameter(dbCommand, ":destino", DbType.String, vo.Destino);
			db.AddInParameter(dbCommand, ":duracao", DbType.Int32, vo.Duracao);
			db.AddInParameter(dbCommand, ":arquivo", DbType.String, vo.Arquivo);

			db.AddInParameter(dbCommand, ":linha", DbType.String, vo.Linha);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}


		internal static BilhetagemVO GetLast(DbTransaction transaction, Database db) {
			string sql = "SELECT * FROM EV_BILHETAGEM B WHERE B.ID_REGISTRO = (" +
				" SELECT MAX(ID_REGISTRO) FROM EV_BILHETAGEM ) ";

			List<BilhetagemVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}*/

		internal static void ClearTemporarios(DbTransaction transaction, Database db) {

			string sql = "TRUNCATE TABLE EV_BILHETAGEM_TMP";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static void IncluirTemporarios(List<BilhetagemVO> lstBilhetes, DbTransaction transaction, Database db) {

			string sql = "INSERT INTO EV_BILHETAGEM_TMP(NR_BILHETE, DT_BILHETAGEM, TP_DIRECAO, DS_DURACAO, DS_ORIGEM, DS_DESTINO, DS_JUNTOR, " +
				"	DS_CONTA, DS_ESTADO, DS_CONEXAO, DT_IMPORTACAO, DS_TEL_ORIGEM, DS_TEL_DESTINO, NR_DURACAO, DS_ARQUIVO, DS_LINHA) " +
				" VALUES (:nrBilhete, :data, :direcao, :duracaoRaw, :origemRaw, :destinoRaw, :juntor, " +
				"	:conta, :estado, :conexao, LOCALTIMESTAMP, :origem, :destino, :duracao, :arquivo, :linha)";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":nrBilhete", DbType.Int64);
			db.AddInParameter(dbCommand, ":data", DbType.DateTime);
			db.AddInParameter(dbCommand, ":direcao", DbType.String);
			db.AddInParameter(dbCommand, ":duracaoRaw", DbType.String);
			db.AddInParameter(dbCommand, ":origemRaw", DbType.String);
			db.AddInParameter(dbCommand, ":destinoRaw", DbType.String);
			db.AddInParameter(dbCommand, ":juntor", DbType.String);

			db.AddInParameter(dbCommand, ":conta", DbType.String);
			db.AddInParameter(dbCommand, ":estado", DbType.String);
			db.AddInParameter(dbCommand, ":conexao", DbType.String);

			db.AddInParameter(dbCommand, ":origem", DbType.String);
			db.AddInParameter(dbCommand, ":destino", DbType.String);
			db.AddInParameter(dbCommand, ":duracao", DbType.Int32);
			db.AddInParameter(dbCommand, ":arquivo", DbType.String);
			db.AddInParameter(dbCommand, ":linha", DbType.String);

			int count = 0;
			DateTime start = DateTime.Now;
			foreach (BilhetagemVO vo in lstBilhetes) {
				db.SetParameterValue(dbCommand, ":nrBilhete", vo.NumeroBilhete);
				db.SetParameterValue(dbCommand, ":data", vo.DataBilhetagem);
				db.SetParameterValue(dbCommand, ":direcao", vo.Direcao);
				db.SetParameterValue(dbCommand, ":duracaoRaw", vo.DuracaoRaw);
				db.SetParameterValue(dbCommand, ":origemRaw", vo.OrigemRaw);
				db.SetParameterValue(dbCommand, ":destinoRaw", vo.DestinoRaw);
				db.SetParameterValue(dbCommand, ":juntor", vo.Juntor);

				db.SetParameterValue(dbCommand, ":conta", vo.Conta);
				db.SetParameterValue(dbCommand, ":estado", vo.Estado);
				db.SetParameterValue(dbCommand, ":conexao", vo.Conexao);

				db.SetParameterValue(dbCommand, ":origem", vo.Origem);
				db.SetParameterValue(dbCommand, ":destino", vo.Destino);
				db.SetParameterValue(dbCommand, ":duracao", vo.Duracao);
				db.SetParameterValue(dbCommand, ":arquivo", vo.Arquivo);
				db.SetParameterValue(dbCommand, ":linha", vo.Linha);
				BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
				count++;

				if (count % 1000 == 0) {
					double milis = DateTime.Now.Subtract(start).TotalMilliseconds;
					Console.WriteLine("WRITING DATABASE: " + count + " - " + milis);
					log.Info("WRITING DATABASE: " + count + " - " + milis);
					start = DateTime.Now;
				}
			}
		}

		internal static int ProcessarDiferenca(DbTransaction transaction, Database db) {
			const string sqlProcCanc = "BEGIN PC_PROCESSAR_BILHETAGEM(:QTD); END;";
			DbCommand cmd = db.GetSqlStringCommand(sqlProcCanc);
			db.AddOutParameter(cmd, ":qtd", DbType.Int64, 32);
			BaseDAO.ExecuteNonQuery(cmd, transaction, db);

			object o = db.GetParameterValue(cmd, ":qtd");
			return Convert.ToInt32(o);
		}

		internal static DataTable Pesquisar(DateTime? dtInicial, DateTime? dtFinal, string direcao, List<int> lstSetores, List<int> lstRamais, List<string> lstEstados, Database db) {
			string sql = "SELECT DT_BILHETAGEM, NR_DURACAO, TP_DIRECAO, DS_ESTADO, DS_TEL_ORIGEM, DS_TEL_DESTINO, DS_DURACAO, " +
				"	RORIGEM_NR_RAMAL, RORIGEM_ID_SETOR, RORIGEM_DS_USUARIO, '' AS RORIGEM_DS_RAMAL, " +
				"	RDESTINO_NR_RAMAL, RDESTINO_ID_SETOR, RDESTINO_DS_USUARIO, '' AS RDESTINO_DS_RAMAL " +
				" FROM VW_BILHETAGEM B " +
				" WHERE 1 = 1 "	;

			List<Parametro> lstParams = new List<Parametro>();
			if (dtInicial != null) {
				sql += " AND DT_BILHETAGEM >= :inicio ";
				lstParams.Add(new Parametro(":inicio", DbType.Date, dtInicial.Value));
			}
			if (dtFinal != null) {
				sql += " AND DT_BILHETAGEM <= :final ";
				lstParams.Add(new Parametro(":final", DbType.Date, dtFinal.Value));
			}

			if (!string.IsNullOrEmpty(direcao)) {
				sql += " AND TP_DIRECAO = :direcao ";
				lstParams.Add(new Parametro(":direcao", DbType.String, direcao));
			} else {
				sql += " AND TP_DIRECAO IN ('R', 'O') ";
			}

			if (lstEstados != null && lstEstados.Count > 0) {
				string sqlEstados = "";
				sqlEstados = "('0'";
				foreach (string estado in lstEstados) {
					sqlEstados += " ,'" + estado + "' ";
				}
				sqlEstados += ")";
				sql += " AND DS_ESTADO IN " + sqlEstados;
			}

			string sqlRamais = "";
			if (lstRamais != null && lstRamais.Count > 0) {
				sqlRamais = "IN (0";
				foreach (int ramal in lstRamais) {
					sqlRamais += " ," + ramal;
				}
				sqlRamais += ")";
			}

			if (!string.IsNullOrEmpty(sqlRamais)) {
				sql += " AND (RORIGEM_NR_RAMAL " + sqlRamais + " OR " +
					" RDESTINO_NR_RAMAL " + sqlRamais + ") ";
			}


			string sqlSetores = "";
			if (lstSetores != null && lstSetores.Count > 0) {
				sqlSetores = "IN (0";
				foreach (int setor in lstSetores) {
					sqlSetores += " ," + setor;
				}
				sqlSetores += ")";
			}

			if (!string.IsNullOrEmpty(sqlSetores)) {
				sql += " AND (RORIGEM_ID_SETOR " + sqlSetores +
					" OR RDESTINO_ID_SETOR " + sqlSetores +
					" OR EXISTS (SELECT 1 " +
					"	FROM EV_REL_RAMAL_USUARIO RRU, EV_REL_SETOR_USUARIO RSU " +
					"	WHERE RSU.ID_USUARIO = RRU.ID_USUARIO " +
					"		AND (RRU.NR_RAMAL = RORIGEM_NR_RAMAL OR RRU.NR_RAMAL = RDESTINO_NR_RAMAL) "	+
					"		AND RSU.ID_SETOR " + sqlSetores + 
					"	)" +
					") ";
			}

			sql += " ORDER BY DT_BILHETAGEM ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);
			return dt;
		}
	}
}
