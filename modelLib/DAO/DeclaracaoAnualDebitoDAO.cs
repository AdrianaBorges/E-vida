using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class DeclaracaoAnualDebitoDAO {

		internal static DeclaracaoAnualDebitoVO GetById(int cdBeneficiario, int ano, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_DECLARACAO_ANUAL_DEBITO WHERE CD_BENEFICIARIO = :cdBeneficiario AND NR_ANO_REF = :ano";
			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro() { Name = ":ano", Tipo = DbType.Int32, Value = ano });
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int32, Value = cdBeneficiario });

			List<DeclaracaoAnualDebitoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static DeclaracaoAnualDebitoVO FromDataRow(DataRow dr) {
			DeclaracaoAnualDebitoVO vo = new DeclaracaoAnualDebitoVO();
			vo.AnoRef = Convert.ToInt32(dr["NR_ANO_REF"]);
			vo.CodBeneficiario = Convert.ToInt32(dr["CD_BENEFICIARIO"]);
			vo.CodUsuarioSolicitacao = BaseDAO.GetNullableInt(dr, "CD_USUARIO_SOLICITACAO");
			vo.DataEnvio = BaseDAO.GetNullableDate(dr, "DT_ENVIO");
			vo.DataSituacao = BaseDAO.GetNullableDate(dr, "DT_STATUS").Value;
			vo.DataSolicitacao = BaseDAO.GetNullableDate(dr, "DT_SOLICITACAO").Value;
			vo.Erro = Convert.ToString(dr["DS_ERRO"]);
			vo.Situacao = (StatusDeclaracaoAnualDebito)Convert.ToInt32(dr["CD_STATUS"]);
			return vo;
		}

		internal static DataTable Pesquisar(int ano, string cdPlano, int? empresa, long? matricula, int? status, bool? apenasQuitados, EvidaDatabase db) {
            string sql = " SELECT B.cd_beneficiario, B.cd_alternativo, B.cd_empresa, B.cd_funcionario, B.nm_beneficiario, :ano ANO, " +
                "		DEB.QTD_QUITADO, DEB.QTD_PENDENTE, BP.CD_PLANO_VINCULADO, P.DS_PLANO, DAB.DT_SOLICITACAO, DAB.CD_STATUS, DAB.DS_ERRO, DAB.DT_STATUS, DAB.DT_ENVIO " +
                " FROM ISA_HC.HC_V_BENEFICIARIO B, ISA_HC.HC_BENEFICIARIO_PLANO BP, ISA_HC.HC_PLANO P, " +
                "		EV_DECLARACAO_ANUAL_DEBITO DAB, " +
                "		(SELECT CD_BENEFICIARIO, SUM(DECODE(ST_DEBITO_BENEFICIARIO, 'Q',1, 0)) QTD_QUITADO, SUM(DECODE(ST_DEBITO_BENEFICIARIO, 'Q',0, 'C',0, 1)) QTD_PENDENTE" +
                "			FROM ISA_HC.HC_DEBITO_BENEFICIARIO " +
                "			WHERE DT_ANO_MES_REF BETWEEN :inicio AND :fim " +
                "			GROUP BY CD_BENEFICIARIO ) DEB " +
                " WHERE P.CD_PLANO = BP.CD_PLANO_VINCULADO AND P.TP_PLANO = BP.TP_PLANO " +
                "		AND BP.CD_BENEFICIARIO = B.CD_BENEFICIARIO " +
                //"		AND SYSDATE BETWEEN BP.DT_INICIO_VIGENCIA AND NVL(BP.DT_TERMINO_VIGENCIA, SYSDATE) " +
                "		AND (to_char(BP.DT_INICIO_VIGENCIA, 'yyyy') <= :ano and (BP.DT_TERMINO_VIGENCIA is null or to_char(BP.DT_TERMINO_VIGENCIA, 'yyyy') >= :ano)) " +
                "		and B.CD_BENEFICIARIO = DEB.CD_BENEFICIARIO " +
                "		and DAB.CD_BENEFICIARIO(+) = B.cd_beneficiario " +
                "		AND DAB.NR_ANO_REF (+) = :ano";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":ano", Tipo = DbType.Int32, Value = ano });
			lstParams.Add(new Parametro() { Name = ":inicio", Tipo = DbType.Date, Value = new DateTime(ano, 1, 1) });
			lstParams.Add(new Parametro() { Name = ":fim", Tipo = DbType.Date, Value = new DateTime(ano, 12, 31) });

			if (!string.IsNullOrEmpty(cdPlano)) {
				sql += " AND p.cd_plano = :plano ";
				lstParams.Add(new Parametro() { Name = ":plano", Tipo = DbType.String, Value = cdPlano });
			} else {
				//sql += " AND p.cd_plano <> :plano ";
				//lstParams.Add(new Parametro() { Name = ":plano", Tipo = DbType.String, Value = Constantes.PLANO_MAIS_VIDA_CEA.ToString() });
			}

			if (matricula != null) {
				sql += " AND b.cd_funcionario = :matricula ";
				lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.Int64, Value = matricula });
			}
			if (empresa != null) {
				sql += " AND b.cd_empresa = :empresa";
				lstParams.Add(new Parametro() { Name = ":empresa", Tipo = DbType.Int32, Value = empresa });
			}
			if (status != null) {
				if (status.Value != -1) {
					sql += " AND dab.CD_STATUS = :status ";
					lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = status.Value });
				} else {
					sql += " AND dab.CD_BENEFICIARIO IS NULL ";
				}
			}
			if (apenasQuitados != null) {
				if (apenasQuitados.Value) {
					sql += " AND DEB.QTD_PENDENTE = 0 ";
				} else {
					sql += " AND DEB.QTD_PENDENTE > 0 ";
				}
			}

			sql += " ORDER BY B.NM_BENEFICIARIO ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;

		}

		internal static void Criar(DeclaracaoAnualDebitoVO vo, EvidaDatabase evdb) {
			string sql = "INSERT INTO EV_DECLARACAO_ANUAL_DEBITO (CD_BENEFICIARIO, NR_ANO_REF, DT_SOLICITACAO, CD_USUARIO_SOLICITACAO, CD_STATUS, DT_STATUS) " +
				" VALUES (:cdBeneficiario, :anoRef, LOCALTIMESTAMP, :usuario, :status, LOCALTIMESTAMP)";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, vo.CodBeneficiario);
			db.AddInParameter(dbCommand, ":anoRef", DbType.Int32, vo.AnoRef);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, vo.CodUsuarioSolicitacao);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusDeclaracaoAnualDebito.SOLICITADO);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static bool CheckDebitoAno(int cdBeneficiario, int ano, EvidaDatabase db) {
			string sql = "SELECT COUNT(1) FROM ISA_HC.HC_DEBITO_BENEFICIARIO WHERE DT_ANO_MES_REF BETWEEN :inicio AND :fim AND CD_BENEFICIARIO = :cdBeneficiario AND ST_DEBITO_BENEFICIARIO NOT IN ('Q','C')";
			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro() { Name = ":inicio", Tipo = DbType.Date, Value = new DateTime(ano, 1, 1) });
			lstParams.Add(new Parametro() { Name = ":fim", Tipo = DbType.Date, Value = new DateTime(ano, 12, 31) });
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int32, Value = cdBeneficiario });

			object o = BaseDAO.ExecuteScalar(db, sql, lstParams);
			return Convert.ToInt32(o) == 0;

		}

		internal static void Finalizar(DeclaracaoAnualDebitoVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_DECLARACAO_ANUAL_DEBITO SET DT_STATUS = LOCALTIMESTAMP, DT_ENVIO = LOCALTIMESTAMP, DS_ERRO = NULL, CD_STATUS = :status " +
				"	WHERE CD_BENEFICIARIO = :cdBeneficiario and NR_ANO_REF = :ano ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, vo.CodBeneficiario);
			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.AnoRef);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusDeclaracaoAnualDebito.ENVIADO);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static List<DeclaracaoAnualDebitoVO> ListById(List<int> lst, int ano, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_DECLARACAO_ANUAL_DEBITO WHERE NR_ANO_REF = :ano AND CD_BENEFICIARIO IN (";
			List<Parametro> lstParams = new List<Parametro>();

			int i = 0;
			foreach (int codBeneficiario in lst) {
				if (i > 0) {
					sql += ",";
				}
				sql += " :benef" + i;
				lstParams.Add(new Parametro() { Name = ":benef" + i, Tipo = DbType.Int32, Value = codBeneficiario });
				++i;
			}
			sql += " )";
			lstParams.Add(new Parametro() { Name = ":ano", Tipo = DbType.Int32, Value = ano });

			List<DeclaracaoAnualDebitoVO> lstReturn = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lstReturn;
		}

		internal static List<int> ListPendenciaAno(List<int> lst, int ano, EvidaDatabase db) {
			string sqlIn = "";
			List<Parametro> lstParams = new List<Parametro>();

			int i = 0;
			foreach (int codBeneficiario in lst) {
				if (i > 0) {
					sqlIn += ", ";
				}
				sqlIn += " :benef" + i;
				lstParams.Add(new Parametro() { Name = ":benef" + i, Tipo = DbType.Int32, Value = codBeneficiario });
				++i;
			}
			
			string sql = "SELECT CD_BENEFICIARIO, COUNT(1) qtd " +
				"	FROM ISA_HC.HC_DEBITO_BENEFICIARIO " +
				"	WHERE DT_ANO_MES_REF BETWEEN :inicio AND :fim AND ST_DEBITO_BENEFICIARIO NOT IN ('Q','C') " +
				"		AND CD_BENEFICIARIO IN (" + sqlIn + ") " +
				"	GROUP BY CD_BENEFICIARIO " +
				"	HAVING COUNT(1) > 0 ";

			lstParams.Add(new Parametro() { Name = ":inicio", Tipo = DbType.Date, Value = new DateTime(ano, 1, 1) });
			lstParams.Add(new Parametro() { Name = ":fim", Tipo = DbType.Date, Value = new DateTime(ano, 12, 31) });

			Func<DataRow, int> convert = delegate(DataRow dr) {
				return Convert.ToInt32(dr["CD_BENEFICIARIO"]);
			};


			List<int> lstRetorno = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
			return lstRetorno;
		}

		internal static void Ressolicitar(DeclaracaoAnualDebitoVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_DECLARACAO_ANUAL_DEBITO SET DT_SOLICITACAO = LOCALTIMESTAMP, CD_USUARIO_SOLICITACAO = :codUsuario, " +
				"	DT_STATUS = LOCALTIMESTAMP, DT_ENVIO = NULL, DS_ERRO = NULL, CD_STATUS = :status " +
				"	WHERE CD_BENEFICIARIO = :cdBeneficiario and NR_ANO_REF = :ano ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, vo.CodBeneficiario);
			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.AnoRef);
			db.AddInParameter(dbCommand, ":codUsuario", DbType.Int32, vo.CodUsuarioSolicitacao);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusDeclaracaoAnualDebito.SOLICITADO);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static List<DeclaracaoAnualDebitoVO> ListarSolPendente(int maxRecords, Database db) {
			string sql = "SELECT * FROM EV_DECLARACAO_ANUAL_DEBITO WHERE CD_STATUS = :status AND ROWNUM <= :maxRecords ORDER BY DT_SOLICITACAO ASC";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = (int)StatusDeclaracaoAnualDebito.SOLICITADO });
			lstParams.Add(new Parametro() { Name = ":maxRecords", Tipo = DbType.Int32, Value = (int)maxRecords });

			List<DeclaracaoAnualDebitoVO> lstReturn = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lstReturn;
		}

		internal static void RegistrarGerado(DeclaracaoAnualDebitoVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_DECLARACAO_ANUAL_DEBITO SET DT_STATUS = LOCALTIMESTAMP, DT_ENVIO = NULL, DS_ERRO = NULL, CD_STATUS = :status " +
				"	WHERE CD_BENEFICIARIO = :cdBeneficiario and NR_ANO_REF = :ano ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, vo.CodBeneficiario);
			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.AnoRef);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusDeclaracaoAnualDebito.GERADO);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void MarcarErro(DeclaracaoAnualDebitoVO vo, Exception ex, EvidaDatabase evdb) {
			string sql = "UPDATE EV_DECLARACAO_ANUAL_DEBITO SET CD_STATUS = :status, DT_ENVIO = NULL, DS_ERRO = :erro " +
				"	WHERE CD_BENEFICIARIO = :cdBeneficiario and NR_ANO_REF = :ano ";

			vo.Erro = ex.ToString();
			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, vo.CodBeneficiario);
			db.AddInParameter(dbCommand, ":ano", DbType.Int32, vo.AnoRef);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusDeclaracaoAnualDebito.ERRO);
			db.AddInParameter(dbCommand, ":erro", DbType.String, vo.Erro);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
