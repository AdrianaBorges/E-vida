using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.DAO {
	internal class ParametroDAO {
		internal static List<ParametroVO> GetConfigs(EvidaDatabase db) {
			string sql = "SELECT id_parametro, vl_parametro " +
				" FROM ev_parametro p " +
				" ORDER BY id_parametro ";

			List<ParametroVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
			return lst;
		}

		private static int GetNextSeq(ParametroVariavelVO vo, EvidaDatabase db) {
			string sql = "SELECT NVL(MAX(ID_SEQ),0) PROX " +
				" FROM ev_param_variavel p " +
				" where id_parametro = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro("id", DbType.Int32, vo.ParamId));
			object o = BaseDAO.ExecuteScalar(db, sql, lstParams);
			return Convert.ToInt32(o) + 1;
		}

		private static ParametroVO FromDataRow(DataRow row) {
			ParametroVO vo = new ParametroVO();
			vo.Id = Convert.ToInt32(row["id_parametro"]);
			vo.Value = Convert.ToString(row["vl_parametro"]);
			return vo;
		}

		private static ParametroVariavelVO FromDataRowVariavel(DataRow row) {
			ParametroVariavelVO vo = new ParametroVariavelVO();
			vo.IdLinha = Convert.ToInt32(row["id_seq"]);
			vo.ParamId = Convert.ToInt32(row["id_parametro"]);
			vo.Value = Convert.ToString(row["vl_parametro"]);
			vo.Inicio = Convert.ToDateTime(row["DT_INICIO_VIGENCIA"]);
			vo.Fim = Convert.ToDateTime(row["DT_FIM_VIGENCIA"]);
			vo.Alteracao = BaseDAO.GetNullableDate(row, "DT_ALTERACAO");
			vo.Criacao = BaseDAO.GetNullableDate(row, "DT_CRIACAO").Value;
			vo.IdUsuarioAlteracao = BaseDAO.GetNullableInt(row, "ID_USUARIO_ALTERACAO");
			vo.IdUsuarioCriacao = BaseDAO.GetNullableInt(row, "ID_USUARIO_CRIACAO").Value;
			return vo;
		}

		internal static void Update(ParametroUtil.ParametroType parametroType, string valor, EvidaDatabase db) {
			string sql = "UPDATE ev_parametro SET vl_parametro = :valor WHERE id_parametro = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, (int)parametroType));
			lstParam.Add(new Parametro(":valor", DbType.String, valor));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static List<ParametroVariavelVO> GetParametroRange(ParametroUtil.ParametroVariavelType idParametro, DateTime inicio, DateTime fim, EvidaDatabase db) {
			string sql = "SELECT id_parametro, id_seq, DT_INICIO_VIGENCIA, DT_FIM_VIGENCIA, vl_parametro, ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO " +
				" FROM ev_param_variavel p " +
				" WHERE id_parametro = :id AND ((dt_inicio_vigencia <= :fim and dt_fim_vigencia >= :fim) OR "+
				"		(dt_inicio_vigencia <= :inicio and dt_fim_vigencia >= :inicio) OR " +
				"		(dt_inicio_vigencia >= :inicio and dt_fim_vigencia <= :fim)) " +
				" ORDER BY id_parametro, DT_INICIO_VIGENCIA ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, (int)idParametro));
			lstParam.Add(new Parametro(":inicio", DbType.Date, inicio.Date));
			lstParam.Add(new Parametro(":fim", DbType.Date, fim.Date));
			List<ParametroVariavelVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowVariavel, lstParam);
			return lst;
		}

		internal static ParametroVariavelVO GetParametro(ParametroUtil.ParametroVariavelType idParametro, int idSeq, EvidaDatabase db) {
			string sql = "SELECT id_parametro, id_seq, DT_INICIO_VIGENCIA, DT_FIM_VIGENCIA, vl_parametro, ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO " +
				" FROM ev_param_variavel p " +
				" WHERE id_parametro = :id AND id_seq = :seq " +
				" ORDER BY id_parametro, DT_INICIO_VIGENCIA ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, (int)idParametro));
			lstParam.Add(new Parametro(":seq", DbType.Int32, idSeq));
			return BaseDAO.ExecuteDataRow(db, sql, FromDataRowVariavel, lstParam);
		}

		internal static ParametroVariavelVO GetParametro(ParametroUtil.ParametroVariavelType idParametro, DateTime dtRef, EvidaDatabase db) {
			string sql = "SELECT id_parametro, id_seq, DT_INICIO_VIGENCIA, DT_FIM_VIGENCIA, vl_parametro, ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO " +
				" FROM ev_param_variavel p " +
				" WHERE id_parametro = :id AND :data between DT_INICIO_VIGENCIA AND DT_FIM_VIGENCIA " +
				" ORDER BY id_parametro, DT_INICIO_VIGENCIA ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, (int)idParametro));
			lstParam.Add(new Parametro(":data", DbType.Date, dtRef.Date));
			return BaseDAO.ExecuteDataRow(db, sql, FromDataRowVariavel, lstParam);
		}

		internal static List<ParametroVariavelVO> GetParametroAll(ParametroUtil.ParametroVariavelType idParametro, EvidaDatabase db) {
			string sql = "SELECT id_parametro, id_seq, DT_INICIO_VIGENCIA, DT_FIM_VIGENCIA, vl_parametro, ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO " +
				" FROM ev_param_variavel p " +
				" WHERE id_parametro = :id " +
				" ORDER BY id_parametro, DT_INICIO_VIGENCIA ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, (int)idParametro));
			List<ParametroVariavelVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowVariavel, lstParam);
			return lst;
		}

		internal static void Salvar(ParametroVariavelVO vo, int idUsuario, EvidaDatabase db) {
			string sqlIns = "INSERT INTO ev_param_variavel (id_parametro, id_seq, DT_INICIO_VIGENCIA, DT_FIM_VIGENCIA, vl_parametro, ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO) " +
				" VALUES (:id, :idLinha, :inicio, :fim, :valor, :idUsuario, LOCALTIMESTAMP, NULL, NULL)";
			string sqlUpd = "UPDATE ev_param_variavel SET DT_INICIO_VIGENCIA = :inicio, DT_FIM_VIGENCIA = :fim, vl_parametro =:valor, " +
				"	ID_USUARIO_ALTERACAO = :idUsuario, DT_ALTERACAO = LOCALTIMESTAMP " +
				" WHERE ID_SEQ = :idLinha and id_parametro = :id";

			string sql = null;
			if (vo.IdLinha == 0) {
				sql = sqlIns;
				vo.IdLinha = GetNextSeq(vo, db);
			} else {
				sql = sqlUpd;
			}

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":idLinha", DbType.Int32, vo.IdLinha));
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.ParamId));
			lstParam.Add(new Parametro(":valor", DbType.String, vo.Value));
			lstParam.Add(new Parametro(":inicio", DbType.Date, vo.Inicio));
			lstParam.Add(new Parametro(":fim", DbType.Date, vo.Fim));
			lstParam.Add(new Parametro(":idUsuario", DbType.Int32, vo.IdUsuarioCriacao));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);

		}

	}
}
