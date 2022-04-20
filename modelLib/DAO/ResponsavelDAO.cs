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
	internal class ResponsavelDAO {
		internal static DataTable BuscarBeneficiarios(string nome, long matricula, string cdAlternativo, Database db) {
			string sql = "select b.cd_beneficiario, b.cd_alternativo, b.tp_beneficiario, b.cd_funcionario, b.nm_beneficiario " +
				" from isa_hc.hc_v_beneficiario b, isa_hc.hc_beneficiario_plano bp " +
				" where b.cd_beneficiario = bp.cd_beneficiario and (bp.dt_termino_vigencia > sysdate or bp.dt_termino_vigencia IS NULL) " +
				"	and bp.cd_plano_vinculado = " + Constantes.PLANO_EVIDA_FAMILIA;

			List<Parametro> lstParams = new List<Parametro>();
			if (matricula != 0) {
				sql += " AND b.CD_FUNCIONARIO = :matricula";
				lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.Int64, Value = matricula });
			}
			if (!string.IsNullOrEmpty(nome)) {
                sql += " AND upper(trim(b.nm_beneficiario)) LIKE upper(trim(:nome)) ";
				lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
			}
			if (!string.IsNullOrEmpty(cdAlternativo)) {
				sql += " AND b.cd_alternativo = :cdAlternativo";
				lstParams.Add(new Parametro() { Name = ":cdAlternativo", Tipo = DbType.String, Value = cdAlternativo });
			}
			sql += " ORDER BY b.nm_beneficiario ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarResponsaveis(int cdBeneficiario, Database db) {

			string sql = " select r.cd_beneficiario, r.dt_inicio_vigencia, r.dt_fim_vigencia, r.ds_observacao, " +
				"	r.cd_beneficiario_financeiro, b1.nm_beneficiario nm_beneficiario_financeiro, b1.cd_funcionario cd_funcionario_financeiro, " +
				"	r.cd_beneficiario_plano, b2.nm_beneficiario nm_beneficiario_plano, b2.cd_funcionario cd_funcionario_plano " +
				" from ev_responsavel r, isa_hc.hc_v_beneficiario b1, isa_hc.hc_v_beneficiario b2 " +
				" WHERE r.cd_beneficiario_financeiro = b1.cd_beneficiario " +
				"	and r.cd_beneficiario_plano = b2.cd_beneficiario " +
				"	and r.cd_beneficiario = :beneficiario ORDER BY r.dt_inicio_vigencia";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro(":beneficiario", DbType.Int32, cdBeneficiario));

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

		internal static DataTable BuscarPossiveisResponsaveis(long matricula, string nome, Database db) {

			string sql = " select b.cd_beneficiario, b.cd_funcionario, b.nm_beneficiario " +
				" FROM isa_hc.hc_v_beneficiario b" +
				" WHERE b.tp_beneficiario = '" + Constantes.TIPO_BENEFICIARIO_FUNCIONARIO + "' " +
				"	and b.cd_empresa in (60,61) " +
				"	and b.cd_situacao_benef = 'A' ";

			List<Parametro> lstP = new List<Parametro>();
			if (matricula != 0) {
				sql += " AND b.cd_funcionario = :matricula ";
				lstP.Add(new Parametro(":matricula", DbType.Int64, matricula));
			}
			if (!string.IsNullOrEmpty(nome)) {
                sql += " AND upper(trim(b.nm_beneficiario)) LIKE upper(trim(:nome)) ";
				lstP.Add(new Parametro(":nome", DbType.String, "%" + nome.ToUpper() + "%"));
			}
			sql += " ORDER BY b.nm_beneficiario ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

		internal static void SalvarResponsaveis(int cdBeneficiario, List<ResponsavelVO> lst, int idUsuario, DbTransaction transaction, Database db) {
			DataTable dtOld = BuscarResponsaveis(cdBeneficiario, db);

			DbCommand insCommand = CreateInsertCommand(cdBeneficiario, idUsuario, db);
			DbCommand updCommand = CreateUpdateCommand(cdBeneficiario, idUsuario, db);
			DbCommand delCommand = CreateDeleteCommand(cdBeneficiario, db);

			foreach (DataRow dr in dtOld.Rows) {
				DateTime dtInicio = Convert.ToDateTime(dr["dt_inicio_vigencia"]);
				
				ResponsavelVO vo = lst.Find(x => x.InicioVigencia.CompareTo(dtInicio) == 0);
				if (vo == null) {
					db.AddInParameter(delCommand, ":inicio", DbType.DateTime, dtInicio);
					db.ExecuteNonQuery(delCommand, transaction);
				}
			}

			foreach (ResponsavelVO vo in lst) {
				DataRow[] drs = dtOld.Select("dt_inicio_vigencia = #" + vo.InicioVigencia.ToString("yyyy-MM-dd") + "#");
				DbCommand dbCommand = null;
				if (drs.Length == 0) {
					dbCommand = insCommand;
				} else {
					DataRow dr = drs[0];
					if (Convert.ToDateTime(dr["dt_fim_vigencia"]).CompareTo(vo.FimVigencia) != 0 ||
						Convert.ToInt32(dr["CD_BENEFICIARIO_FINANCEIRO"]) != vo.CdBeneficiarioFinanceiro ||
						Convert.ToInt32(dr["CD_BENEFICIARIO_PLANO"]) != vo.CdBeneficiarioPlano ||
						!vo.Observacao.Equals(Convert.ToString(dr["DS_OBSERVACAO"]), StringComparison.InvariantCultureIgnoreCase)) {
						dbCommand = updCommand;
					}
						
				}
				if (dbCommand != null)
					ExecuteInsUpd(vo, dbCommand, transaction, db);
			}
		}

		private static void ExecuteInsUpd(ResponsavelVO vo, DbCommand dbCommand, DbTransaction transaction, Database db) {
			db.SetParameterValue(dbCommand, ":inicio", vo.InicioVigencia);
			db.SetParameterValue(dbCommand, ":fim", vo.FimVigencia);
			db.SetParameterValue(dbCommand, ":cdBenefFinanceiro", vo.CdBeneficiarioFinanceiro);
			db.SetParameterValue(dbCommand, ":cdBenefPlano", vo.CdBeneficiarioPlano);
			db.SetParameterValue(dbCommand, ":observacao", vo.Observacao);

			db.ExecuteNonQuery(dbCommand, transaction);
		}

		private static DbCommand CreateInsertCommand(int cdBeneficiario, int idUsuario, Database db) {
			string sql = "INSERT INTO EV_RESPONSAVEL (CD_BENEFICIARIO, DT_INICIO_VIGENCIA, DT_FIM_VIGENCIA, " +
				" CD_BENEFICIARIO_FINANCEIRO, CD_BENEFICIARIO_PLANO, DS_OBSERVACAO, " +
				" ID_USUARIO_CRIACAO, DT_CRIACAO, ID_USUARIO_ALTERACAO, DT_ALTERACAO) " +
				" VALUES (:cdBeneficiario, :inicio, :fim, :cdBenefFinanceiro, :cdBenefPlano, :observacao, " +
				" :usuario, LOCALTIMESTAMP, null, null)";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, cdBeneficiario);
			db.AddInParameter(dbCommand, ":inicio", DbType.Date);
			db.AddInParameter(dbCommand, ":fim", DbType.Date);
			db.AddInParameter(dbCommand, ":cdBenefFinanceiro", DbType.Int32);
			db.AddInParameter(dbCommand, ":cdBenefPlano", DbType.Int32);
			db.AddInParameter(dbCommand, ":observacao", DbType.String);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, idUsuario);

			return dbCommand;
		}

		private static DbCommand CreateUpdateCommand(int cdBeneficiario, int idUsuario, Database db) {
			string sql = "UPDATE EV_RESPONSAVEL SET DT_FIM_VIGENCIA = :fim, " +
				" CD_BENEFICIARIO_FINANCEIRO = :cdBenefFinanceiro, CD_BENEFICIARIO_PLANO = :cdBenefPlano, " +
				" DS_OBSERVACAO = :observacao, ID_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP " +
				" WHERE CD_BENEFICIARIO = :cdBeneficiario AND DT_INICIO_VIGENCIA = :inicio ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, cdBeneficiario);
			db.AddInParameter(dbCommand, ":inicio", DbType.Date);
			db.AddInParameter(dbCommand, ":fim", DbType.Date);
			db.AddInParameter(dbCommand, ":cdBenefFinanceiro", DbType.Int32);
			db.AddInParameter(dbCommand, ":cdBenefPlano", DbType.Int32);
			db.AddInParameter(dbCommand, ":observacao", DbType.String);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, idUsuario);

			return dbCommand;
		}

		private static DbCommand CreateDeleteCommand(int cdBeneficiario, Database db) {
			string sql = "DELETE FROM EV_RESPONSAVEL " +
				" WHERE CD_BENEFICIARIO = :cdBeneficiario AND DT_INICIO_VIGENCIA = :inicio ";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":cdBeneficiario", DbType.Int32, cdBeneficiario);
			db.AddInParameter(dbCommand, ":inicio", DbType.Date);

			return dbCommand;
		}

        internal static DataTable BuscarDependentes(string codint, string codemp, string matric, TipoResponsavel tipo, Database db)
        {

            string sql = "select plano.BI3_CODIGO, plano.BI3_TIPO, plano.BI3_DESCRI, par.BRP_DESCRI, b.* " +
            " from VW_PR_USUARIO_ATUAL b, VW_PR_FAMILIA_PRODUTO bplano, VW_PR_PRODUTO_SAUDE plano, " +
            " VW_PR_USUARIO_ATUAL benefResp, VW_PR_GRAU_PARENTESCO par " +
            " where trim(b.BA1_CODINT) = trim(bplano.BA1_CODINT) " +
            " and trim(b.BA1_CODEMP) = trim(bplano.BA1_CODEMP) " +
            " and trim(b.BA1_MATRIC) = trim(bplano.BA1_MATRIC) " +
            " and trim(b.BA1_TIPREG) = trim(bplano.BA1_TIPREG) " +
            " and (trim(bplano.BA1_DATCAR) <= to_char(SYSDATE, 'YYYYMMDD') and (trim(bplano.BA1_DATBLO) > to_char(SYSDATE, 'YYYYMMDD') or bplano.BA1_DATBLO  = '        ')) " +
            " and trim(plano.BI3_CODIGO) = trim(bplano.BA3_CODPLA) " +
            " and trim(plano.BI3_TIPO) = trim(bplano.BI3_TIPO) " +
            " and trim(b.BA1_CODINT) = trim(benefResp.BA1_CODINT) " +
            " and trim(b.BA1_CODEMP) = trim(benefResp.BA1_CODEMP) " +
            " and trim(b.BA1_MATRIC) = trim(benefResp.BA1_MATRIC) " +
            " and trim(benefResp.BA1_TIPUSU) = '" + PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO + "' " +
            " and trim(benefResp.BA1_CODINT) = trim(:codint) and trim(benefResp.BA1_CODEMP) = trim(:codemp) and trim(benefResp.BA1_MATRIC) = trim(:matric) " +
            " and trim(b.BA1_GRAUPA) = trim(par.BRP_CODIGO) " +
            " and trim(b.BA1_TIPUSU) <> 'T' ";
            
            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;

		}
	}
}
