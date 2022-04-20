using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class MedicamentoReembolsavelDAO {

		internal static MedicamentoReembolsavelVO FromDataRow(DataRow dr) {
			MedicamentoReembolsavelVO vo = new MedicamentoReembolsavelVO();
			vo.DataAlteracao = dr.Field<DateTime>("dt_alteracao");
			vo.DataCriacao = dr.Field<DateTime>("dt_criacao");
			vo.Descricao = dr.Field<string>("ds_servico");
			vo.IdUsuarioCriacao = BaseDAO.GetNullableInt(dr, "id_usuario_criacao").Value;
			vo.IdUusarioAlteracao = BaseDAO.GetNullableInt(dr, "id_usuario_alteracao").Value;
			vo.Mascara = dr.Field<string>("cd_mascara");
			vo.Obs = dr.Field<string>("ds_obs");
			vo.Planos = FormatUtil.StringToList(dr.Field<string>("ds_lst_plano"));
			vo.IdPrincipioAtivo = BaseDAO.GetNullableInt(dr, "id_principio_ativo").Value;
			vo.Reembolsavel = dr.IsNull("ck_reembolsavel") ? false : Convert.ToInt32(dr["ck_reembolsavel"]) == 1;
			vo.UsoContinuo = dr.IsNull("ck_uso_continuo") ? false : Convert.ToInt32(dr["ck_uso_continuo"]) == 1;
			return vo;
		}

		internal static MedicamentoReembolsavelArqVO FromDataRowArq(DataRow dr) {
			MedicamentoReembolsavelArqVO vo = new MedicamentoReembolsavelArqVO();
			vo.IdArquivo = BaseDAO.GetNullableInt(dr, "id_arquivo").Value;
			vo.NomeArquivo = dr.Field<string>("nm_arquivo");
			vo.Mascara = dr.Field<string>("cd_mascara");
			vo.DataEnvio = dr.Field<DateTime>("dt_envio");
			vo.IdUsuarioEnvio = BaseDAO.GetNullableInt(dr, "id_usuario_envio").Value;
			return vo;
		}

		internal static MedicamentoReembolsavelVO GetById(string mascara, EvidaDatabase evdb) {
			string sql = "SELECT * FROM EV_MEDICAMENTO_REEMB MR" +
				"		WHERE MR.CD_MASCARA = :mascara ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, mascara));

			return BaseDAO.ExecuteDataRow(evdb, sql, FromDataRow, lstParam);
		}

        internal static DataTable Pesquisar(string codigo, string texto, string reembolsavel, string continuo, string plano, bool apenasComDados, EvidaDatabase db)
        {
            /*string sql = "SELECT * FROM (";
            sql += " SELECT DISTINCT S.cd_mascara, S.ds_servico, S.cd_rol_ans, mr.cd_mascara mascara_interna, p.DS_PRINCIPIO_ATIVO, MR.DS_LST_PLANO, MR.CK_REEMBOLSAVEL, MR.CK_USO_CONTINUO " +
                " FROM EV_MEDICAMENTO_REEMB MR, VW_PR_SERVICO S, EV_PRINCIPIO_ATIVO P " +
                " WHERE S.CD_MASCARA = MR.CD_MASCARA(+) " +
                "	AND p.cd_principio_ativo (+)= mr.id_principio_ativo ";
            sql += ") WHERE rownum <= 301 ";*/

            string sql = "SELECT DISTINCT cd_mascara, ds_servico, cd_rol_ans, cd_mascara mascara_interna, DS_PRINCIPIO_ATIVO, DS_LST_PLANO, CK_REEMBOLSAVEL, CK_USO_CONTINUO FROM (";
            sql += " SELECT S.cd_mascara, S.ds_servico, S.cd_rol_ans, mr.cd_mascara mascara_interna, p.DS_PRINCIPIO_ATIVO, MR.DS_LST_PLANO, MR.CK_REEMBOLSAVEL, MR.CK_USO_CONTINUO " +
                " FROM EV_MEDICAMENTO_REEMB MR, VW_PR_SERVICO S, EV_PRINCIPIO_ATIVO P " +
                " WHERE S.CD_MASCARA = MR.CD_MASCARA(+) " +
                "	AND p.cd_principio_ativo (+)= mr.id_principio_ativo ";
            sql += ") WHERE rownum <= 301 ";	

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(codigo))
            {
                sql += " AND cd_mascara = :mascara ";
                lstParams.Add(new Parametro(":mascara", DbType.String, codigo));
            }

            if (!string.IsNullOrEmpty(texto))
            {
                sql += " AND (upper(trim(DS_SERVICO)) LIKE upper(trim(:texto)) ";
                sql += "	OR upper(trim(DS_PRINCIPIO_ATIVO)) LIKE upper(trim(:texto)))";
                lstParams.Add(new Parametro(":texto", DbType.String, "%" + texto.ToUpper() + "%"));
            }

            if (!string.IsNullOrEmpty(reembolsavel))
            {
                sql += " AND CK_REEMBOLSAVEL = :reembolsavel";
                lstParams.Add(new Parametro(":reembolsavel", DbType.Int32, reembolsavel == "S" ? 1 : 0));
            }

            if (!string.IsNullOrEmpty(continuo))
            {
                sql += " AND CK_USO_CONTINUO = :continuo";
                lstParams.Add(new Parametro(":continuo", DbType.Int32, continuo == "S" ? 1 : 0));
            }

            if (!string.IsNullOrEmpty(plano))
            {
                sql += " AND upper(trim(DS_LST_PLANO)) LIKE upper(trim(:plano)) ";
                lstParams.Add(new Parametro(":plano", DbType.String, "%" + plano.ToUpper() + "%"));
            }

            if (apenasComDados)
            {
                sql += " AND mascara_interna IS NOT NULL ";
            }

            sql += " ORDER BY DS_SERVICO ASC ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

		internal static List<MedicamentoReembolsavelVO> ListByPrincipioAtivo(int idPrincipio, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_MEDICAMENTO_REEMB MR" +
				"		WHERE MR.id_principio_ativo = :idPrincipio ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":idPrincipio", DbType.Int32, idPrincipio));

			return BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);
		}

		internal static void Salvar(MedicamentoReembolsavelVO vo, int idUsuario, EvidaDatabase db) {
			MedicamentoReembolsavelVO odlVO = GetById(vo.Mascara, db);
			int usuario = vo.IdUusarioAlteracao;

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, vo.Mascara));
			lstParam.Add(new Parametro(":nome", DbType.String, vo.Descricao)); 
			lstParam.Add(new Parametro(":principio", DbType.Int32, vo.IdPrincipioAtivo));
			lstParam.Add(new Parametro(":obs", DbType.String, vo.Obs));
			lstParam.Add(new Parametro(":planos", DbType.String, FormatUtil.ListToString(vo.Planos)));
			lstParam.Add(new Parametro(":reembolsavel", DbType.Boolean, vo.Reembolsavel));
			lstParam.Add(new Parametro(":usoCont", DbType.Boolean, vo.UsoContinuo));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			string sql = null;
			if (odlVO != null) {
				// existe
				sql = "UPDATE EV_MEDICAMENTO_REEMB SET DS_SERVICO = :nome, DT_ALTERACAO = LOCALTIMESTAMP, id_usuario_alteracao = :usuario, ";
				sql += " id_principio_ativo = :principio, ds_obs = :obs, ds_lst_plano = :planos, " +
					" ck_reembolsavel = :reembolsavel, ck_uso_continuo = :usoCont ";
				sql += " WHERE cd_mascara = :mascara";
			} else {
				sql = " INSERT INTO EV_MEDICAMENTO_REEMB (cd_mascara, ds_servico, id_principio_ativo, ds_obs, ds_lst_plano, " +
					" ck_reembolsavel, ck_uso_continuo, " +
					" id_usuario_criacao, dt_criacao, id_usuario_alteracao, DT_ALTERACAO) ";
				sql += " VALUES (:mascara, :nome, :principio, :obs, :planos, :reembolsavel, :usoCont," +
					" :usuario, LOCALTIMESTAMP, :usuario, LOCALTIMESTAMP)";
			}
			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void Replicar(MedicamentoReembolsavelVO vo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_MEDICAMENTO_REEMB SET DT_ALTERACAO = LOCALTIMESTAMP, id_usuario_alteracao = :usuario, ";
			sql += " ds_obs = :obs, ds_lst_plano = :planos, " +
				" ck_reembolsavel = :reembolsavel, ck_uso_continuo = :usoCont ";
			sql += " WHERE id_principio_ativo = :principio AND cd_mascara <> :mascara";


			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, vo.Mascara));
			lstParam.Add(new Parametro(":principio", DbType.Int32, vo.IdPrincipioAtivo));
			lstParam.Add(new Parametro(":obs", DbType.String, vo.Obs));
			lstParam.Add(new Parametro(":planos", DbType.String, FormatUtil.ListToString(vo.Planos)));
			lstParam.Add(new Parametro(":reembolsavel", DbType.Boolean, vo.Reembolsavel));
			lstParam.Add(new Parametro(":usoCont", DbType.Boolean, vo.UsoContinuo));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static List<MedicamentoReembolsavelArqVO> ListarArquivos(string mascara, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_MEDICAMENTO_REEMB_ARQ MRA " +
				"		WHERE MRA.CD_MASCARA = :mascara";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, mascara));

			return BaseDAO.ExecuteDataSet(db, sql, FromDataRowArq, lstParam);
		}

		internal static void ExcluirArquivo(MedicamentoReembolsavelArqVO arq, EvidaDatabase db) {
			string sql = "DELETE FROM EV_MEDICAMENTO_REEMB_ARQ MRA " +
				"		WHERE MRA.CD_MASCARA = :mascara AND MRA.CD_TABELA = :tabela AND MRA.ID_ARQUIVO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, arq.Mascara));
			lstParam.Add(new Parametro(":id", DbType.Int32, arq.IdArquivo));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		private static int GetNextArquivoId(string cdMascara, EvidaDatabase db) {
			string sql = "SELECT NVL(MAX(ID_ARQUIVO),0)+1 FROM EV_MEDICAMENTO_REEMB_ARQ MRA " +
				"		WHERE MRA.CD_MASCARA = :mascara ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, cdMascara));

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql, lstParam);
			return (int)idSolicitacao;
		}

		internal static void CriarArquivos(string cdMascara, int usuario, List<MedicamentoReembolsavelArqVO> lstNewFiles, EvidaDatabase db) {
			int idNextArquivo = GetNextArquivoId(cdMascara, db);
			
			string sql = "INSERT INTO EV_MEDICAMENTO_REEMB_ARQ (cd_mascara, id_arquivo, nm_arquivo, id_usuario_envio, dt_envio) " +
				"VALUES (:mascara, :id, :nome, :usuario, LOCALTIMESTAMP)";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":mascara", DbType.String, cdMascara));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, usuario));
			lstParam.Add(new Parametro(":id", DbType.Int32, 0));
			lstParam.Add(new Parametro(":nome", DbType.String, ""));
			
			foreach (MedicamentoReembolsavelArqVO arq in lstNewFiles) {
				if (arq.IdArquivo == 0) {
					arq.IdArquivo = idNextArquivo++;

					lstParam.First(x => x.Name.Equals(":id")).Value = arq.IdArquivo;
					lstParam.First(x => x.Name.Equals(":nome")).Value = arq.NomeArquivo;

					BaseDAO.ExecuteNonQuery(sql, lstParam, db);
				}
			}

		}

	}
}
