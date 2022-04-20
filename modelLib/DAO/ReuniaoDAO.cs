using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.DAO {
	internal class ReuniaoDAO {
		static EVidaLog log = new EVidaLog(typeof(ReuniaoDAO));

		private static int GetNextId(DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT NVL(MAX(CD_REUNIAO),0) FROM EV_REUNIAO ");

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (int)idSolicitacao + 1;
		}

		internal static ReuniaoVO GetById(int id, Database db, DbTransaction transaction = null) {
			string sql = "SELECT A.* " +
				" FROM EV_REUNIAO A " +
				" WHERE A.cd_reuniao = :id";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

			List<ReuniaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams, transaction);
			if (lst == null || lst.Count == 0)
				return null;
			return lst[0];
		}

		internal static List<ReuniaoVO> ListReuniaoAno(List<string> lstConselho, int ano, Database db) {
			string sql = "SELECT A.* " +
				" FROM EV_REUNIAO A " +
				" WHERE A.dt_reuniao BETWEEN :inicio AND :fim";

			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro() { Name = ":inicio", Tipo = DbType.Date, Value = new DateTime(ano, 01, 01) });
			lstParams.Add(new Parametro() { Name = ":fim", Tipo = DbType.Date, Value = new DateTime(ano, 12, 31) });

			if (lstConselho != null && lstConselho.Count > 0) {
				sql += " AND a.cd_conselho IN (''";
				for (int i = 0; i < lstConselho.Count; i++){
					sql += ", :cdConselho_" + i;
					lstParams.Add(new Parametro() { Name = ":cdConselho_" + i, Tipo = DbType.String, Value = lstConselho[i] });
				}
				sql += ")";				
			}

			List<ReuniaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}

		private static ReuniaoVO FromDataRow(DataRow dr) {
			ReuniaoVO vo = new ReuniaoVO();
			vo.Id = Convert.ToInt32(dr["cd_reuniao"]);
			vo.CodConselho = Convert.ToString(dr["cd_conselho"]);
			vo.Data = Convert.ToDateTime(dr["dt_reuniao"]);
			vo.Descricao = Convert.ToString(dr["ds_reuniao"]);
			vo.Email = Convert.ToString(dr["ds_email"]);
			vo.AssuntoEmail = Convert.ToString(dr["ds_assunto_email"]);
			vo.Titulo = Convert.ToString(dr["nm_reuniao"]);

			return vo;
		}

		private static ArquivoReuniaoVO FromDataRowArquivo(DataRow dr) {
			ArquivoReuniaoVO vo = new ArquivoReuniaoVO();
			vo.IdArquivo = Convert.ToInt32(dr["cd_arquivo"]);
			vo.CodReuniao = Convert.ToInt32(dr["cd_reuniao"]);
			vo.NomeArquivo = Convert.ToString(dr["nm_arquivo"]);
			vo.Descricao = Convert.ToString(dr["ds_arquivo"]);
			return vo;
		}

		internal static DataTable Pesquisar(string cdConselho, string titulo, string descricao, DateTime? inicio, DateTime? fim, Database db) {
			string sql = "SELECT cd_reuniao, r.cd_conselho, DT_CRIACAO, dt_reuniao, nm_reuniao, ds_reuniao, nm_conselho " +
				" FROM EV_REUNIAO r, EV_CONSELHO c " +
				" WHERE r.cd_conselho = c.cd_conselho ";

			List<Parametro> lstParams = new List<Parametro>();
			if (inicio != null && fim != null) {
				sql += " AND r.dt_reuniao BETWEEN :inicio AND :fim ";
				lstParams.Add(new Parametro(":inicio", DbType.Date, inicio.Value));
				lstParams.Add(new Parametro(":fim", DbType.Date, fim.Value));
			}
			if (!string.IsNullOrEmpty(cdConselho)) {
				sql += " AND r.cd_conselho = :cdConselho ";
				lstParams.Add(new Parametro() { Name = ":cdConselho", Tipo = DbType.String, Value = cdConselho });
			}
			if (!string.IsNullOrEmpty(titulo)) {
                sql += " AND upper(trim(r.nm_reuniao)) like upper(trim(:nome))";
				lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + titulo.ToUpper() + "%" });
			}
			if (!string.IsNullOrEmpty(descricao)) {
                sql += " AND upper(trim(r.ds_reuniao)) like upper(trim(:descricao)) ";
				lstParams.Add(new Parametro() { Name = ":descricao", Tipo = DbType.String, Value = "%" + descricao.ToUpper() + "%" });
			}
			sql += " ORDER BY dt_reuniao ASC, r.cd_conselho ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static void Salvar(ReuniaoVO vo, UsuarioVO criador, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (vo.Id == 0) {
				dbCommand = CreateInsert(vo, criador, db);
				vo.Id = GetNextId(transaction, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":cdConselho", DbType.String, vo.CodConselho);
			db.AddInParameter(dbCommand, ":data", DbType.Date, vo.Data);
			db.AddInParameter(dbCommand, ":descricao", DbType.String, vo.Descricao);
			db.AddInParameter(dbCommand, ":titulo", DbType.String, vo.Titulo);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static DbCommand CreateInsert(ReuniaoVO vo, UsuarioVO criador, Database db) {
			string sql = "INSERT INTO EV_REUNIAO (CD_REUNIAO, CD_CONSELHO, NM_REUNIAO, DS_REUNIAO, DT_REUNIAO, CD_USUARIO_CRIACAO, DT_CRIACAO) " +
				"	VALUES (:id, :cdConselho, :titulo, :descricao, :data, :usuario, LOCALTIMESTAMP) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, criador.Id);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(ReuniaoVO vo, Database db) {
			string sql = "UPDATE EV_REUNIAO SET CD_CONSELHO = :cdConselho, NM_REUNIAO = :titulo, DS_REUNIAO = :descricao, DT_REUNIAO = :data " +
				"	WHERE CD_REUNIAO = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void RegistrarEmail(int id, string assunto, string texto, DbTransaction transaction, Database db) {
			string sql = "UPDATE EV_REUNIAO SET DS_EMAIL = :texto, DS_ASSUNTO_EMAIL = :assunto " +
				"	WHERE CD_REUNIAO = :cdReuniao";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdReuniao", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":assunto", DbType.String, assunto);
			db.AddInParameter(dbCommand, ":texto", DbType.String, texto);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static List<ArquivoReuniaoVO> ListarArquivosByReuniao(int id, Database db) {
			string sql = "SELECT A.* " +
				" FROM EV_REUNIAO_ARQUIVO A " +
				" WHERE A.cd_reuniao = :id order by a.CD_ARQUIVO";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

			List<ArquivoReuniaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParams);
			return lst;
		}

		internal static void SalvarArquivo(ArquivoReuniaoVO vo, DbTransaction transaction, Database db) {
			DbCommand dbCommand = null;
			if (vo.IdArquivo == 0) {
				dbCommand = CreateInsert(vo, db);
				vo.IdArquivo = GetNextArquivoId(vo.CodReuniao, transaction, db);
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32, vo.IdArquivo);
			db.AddInParameter(dbCommand, ":cdReuniao", DbType.Int32, vo.CodReuniao);
			db.AddInParameter(dbCommand, ":descricao", DbType.String, vo.Descricao);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		private static int GetNextArquivoId(int cdReuniao, DbTransaction transaction, Database db) {
			DbCommand dbCommand = db.GetSqlStringCommand("SELECT NVL(MAX(CD_ARQUIVO),0) FROM EV_REUNIAO_ARQUIVO WHERE CD_REUNIAO = :cdReuniao");
			db.AddInParameter(dbCommand, ":cdReuniao", DbType.Int32, cdReuniao);

			decimal idSolicitacao = (decimal)db.ExecuteScalar(dbCommand, transaction);

			return (int)idSolicitacao + 1;
		}

		private static DbCommand CreateInsert(ArquivoReuniaoVO vo, Database db) {
			string sql = "INSERT INTO EV_REUNIAO_ARQUIVO (CD_REUNIAO, CD_ARQUIVO, NM_ARQUIVO, DS_ARQUIVO) " +
				"	VALUES (:cdReuniao, :idArquivo, :nome, :descricao) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.NomeArquivo);
			return dbCommand;
		}

		private static DbCommand CreateUpdate(ArquivoReuniaoVO vo, Database db) {
			string sql = "UPDATE EV_REUNIAO_ARQUIVO SET DS_ARQUIVO = :descricao " +
				"	WHERE CD_REUNIAO = :cdReuniao AND CD_ARQUIVO = :idArquivo";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void ExcluirArquivo(ArquivoReuniaoVO vo, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_REUNIAO_ARQUIVO " +
				"	WHERE CD_REUNIAO = :cdReuniao AND CD_ARQUIVO = :idArquivo";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdReuniao", DbType.Int32, vo.CodReuniao);
			db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32, vo.IdArquivo);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static void Excluir(ReuniaoVO vo, DbTransaction transaction, Database db) {
			string sql = "DELETE FROM EV_REUNIAO " +
				"	WHERE CD_REUNIAO = :cdReuniao";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdReuniao", DbType.Int32, vo.Id);
			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}
	}
}
