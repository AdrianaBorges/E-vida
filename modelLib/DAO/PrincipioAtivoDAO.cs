using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class PrincipioAtivoDAO {
		private static int GetNextId(EvidaDatabase db) {
			string sql = "SELECT MAX(cd_principio_ativo)+1 from ev_principio_ativo ";
			return Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql));
		}

		internal static PrincipioAtivoVO FromDataRow(DataRow dr) {
			PrincipioAtivoVO vo = new PrincipioAtivoVO();
			vo.DataAlteracao = BaseDAO.GetNullableDate(dr, "dt_alteracao");
			vo.DataCriacao = dr.Field<DateTime>("dt_criacao");
			vo.Descricao = dr.Field<string>("ds_principio_ativo");
			vo.IdUsuarioCriacao = BaseDAO.GetNullableInt(dr, "id_usuario_criacao").Value;
			vo.IdUsuarioAlteracao = BaseDAO.GetNullableInt(dr, "id_usuario_alteracao");
			vo.Id = BaseDAO.GetNullableInt(dr, "cd_principio_ativo").Value;
			return vo;
		}

		internal static PrincipioAtivoVO GetById(int idPrincipio, EvidaDatabase db) {
			string sql = "SELECT * FROM ev_principio_ativo WHERE cd_principio_ativo = :id";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, idPrincipio));
			return BaseDAO.ExecuteDataRow(db, sql, FromDataRow, lstParams);
		}

		internal static DataTable Pesquisar(int? codigo, string texto, EvidaDatabase db) {
			string sql = "SELECT pa.*, (select count(1) from EV_MEDICAMENTO_REEMB mr where mr.id_principio_ativo = pa.cd_principio_ativo) qtd FROM ev_principio_ativo pa ";
			sql += " WHERE rownum <= 301 ";

			List<Parametro> lstParams = new List<Parametro>();
			if (codigo != null) {
				sql += " AND cd_principio_ativo = :id ";
				lstParams.Add(new Parametro(":id", DbType.Int32, codigo));
			}
			if (!string.IsNullOrEmpty(texto)) {
                sql += " AND upper(trim(ds_principio_ativo)) LIKE upper(trim(:texto)) ";
				lstParams.Add(new Parametro(":texto", DbType.String, "%" + texto.ToUpper() + "%"));
			}

			sql += " ORDER BY ds_principio_ativo ASC ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static List<PrincipioAtivoVO> GetByDescricao(string descricao, EvidaDatabase db) {
			string sql = "SELECT * FROM ev_principio_ativo WHERE upper(trim(ds_principio_ativo)) = :texto";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":texto", DbType.String, descricao.Trim().ToUpper()));

			return BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
		}

		internal static void Salvar(PrincipioAtivoVO vo, int idUsuario, EvidaDatabase db) {
			string sqlInsert = "INSERT INTO ev_principio_ativo (cd_principio_ativo, ds_principio_ativo, dt_criacao, id_usuario_criacao) ";
			sqlInsert += " VALUES (:id, :nome, LOCALTIMESTAMP, :usuario) ";

			string sqlUpdate = "UPDATE ev_principio_ativo SET ds_principio_ativo = :nome, DT_ALTERACAO = LOCALTIMESTAMP , id_usuario_alteracao = :usuario " +
				" WHERE cd_principio_ativo = :id";

			string sql = null;
			if (vo.Id == 0) {
				vo.Id = GetNextId(db);
				sql = sqlInsert;
			} else {
				sql = sqlUpdate;
			}

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":nome", DbType.String, vo.Descricao.Trim().ToUpper()));
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static void Excluir(int id, EvidaDatabase db) {
			string sql = "DELETE FROM ev_principio_ativo WHERE cd_principio_ativo = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, id));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}
	}
}
