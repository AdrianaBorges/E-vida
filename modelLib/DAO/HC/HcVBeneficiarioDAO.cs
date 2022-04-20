using System;
using System.Collections.Generic;
using System.Data;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO.HC {
	internal class HcVBeneficiarioDAO {
		private static string FIELDS = "cd_beneficiario, cd_alternativo, cd_empresa, tp_beneficiario, cd_funcionario, nm_beneficiario, " +
				" dt_obito_fun, dt_obito_dep, dt_demissao, dt_validade_carteira, dt_termino_dependencia, tp_estado_civil, " +
				" fl_deficiente_fisico, tp_sexo, dt_nascimento, cd_grau_parentesco, cd_situacao_benef, cd_banco, cd_agencia, nr_conta, tp_conta, " +
				" nm_mae, cd_parentesco_ans, nr_cpf, ds_endereco, nr_endereco, ds_complemento, ds_bairro, cd_municipio, ds_municipio, " +
				" ds_uf, nr_cep, nr_rg, ds_org_exp_rg, nr_pis, cd_dependente, dt_admissao, cd_empresa_responsavel, nm_titular, " +
				" cd_beneficiario_titular, dt_inicio_pea, dt_termino_pea, nr_carteira_pea, cd_local, cd_lotacao, cd_uf_org_exp_rg, ds_email, " +
				" ds_observacao, cd_lotacao_aux, nr_cns, nr_decl_nasc_vivo";

		internal static HcBeneficiarioVO GetRowById(long cdBeneficiario, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				" from isa_hc.hc_v_beneficiario WHERE cd_beneficiario = :cdBeneficiario";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int64, Value = cdBeneficiario });

			List<HcBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static HcBeneficiarioVO GetRowByCartao(string numCartao, EvidaDatabase db) {
			return GetRowByCartao(numCartao, false, db);
		}

		internal static HcBeneficiarioVO GetRowByCartao(string numCartao, bool useTrata, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				" from isa_hc.hc_v_beneficiario WHERE ";

			List<Parametro> lstParams = new List<Parametro>();
			if (!useTrata) {
				sql += " cd_alternativo = :numCartao ";
			} else {
				sql += " ISA_HC.FN_TRATA_CD_ALTERNATIVO(cd_alternativo) = ISA_HC.FN_TRATA_CD_ALTERNATIVO(:numCartao) ";
			}
			lstParams.Add(new Parametro(":numCartao", DbType.String, numCartao));
			List<HcBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static HcBeneficiarioVO GetTitular(int cdEmpresa, long cdFuncionario, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				" from isa_hc.hc_v_beneficiario WHERE cd_empresa = :cdEmpresa AND cd_funcionario = :cdFuncionario AND tp_beneficiario = '" + Constantes.TIPO_BENEFICIARIO_FUNCIONARIO + "'";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdEmpresa", Tipo = DbType.Int32, Value = cdEmpresa });
			lstParams.Add(new Parametro() { Name = ":cdFuncionario", Tipo = DbType.Int64, Value = cdFuncionario });

			List<HcBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static HcBeneficiarioVO FromDataRow(DataRow dr) {
			HcBeneficiarioVO vo = new HcBeneficiarioVO();
			vo.CdBeneficiario = Convert.ToInt32(dr["cd_beneficiario"]);
			vo.CdAlternativo = Convert.ToString(dr["cd_alternativo"]);
			vo.CdEmpresa = Convert.ToInt32(dr["cd_empresa"]);
			vo.TpBeneficiario = Convert.ToString(dr["tp_beneficiario"]);
            vo.CdFuncionario = Convert.ToInt64(dr["cd_funcionario"]);
			vo.NmBeneficiario = Convert.ToString(dr["nm_beneficiario"]);
			vo.DtDemissao = dr.ConvertToDateTime("dt_demissao");
			vo.DtValidadeCarteira =  dr.ConvertToDateTime("dt_validade_carteira");
			vo.TpEstadoCivil = Convert.ToString(dr["tp_estado_civil"]);
			vo.TpSexo = Convert.ToString(dr["tp_sexo"]);
			vo.DtNascimento = dr.ConvertToDateTime("dt_nascimento");
			vo.CdGrauParentesco = Convert.ToString(dr["cd_grau_parentesco"]);
			vo.CdSituacaoBenef = Convert.ToString(dr["cd_situacao_benef"]);
			vo.NmMae = Convert.ToString(dr["nm_mae"]);
			vo.NrCpf = dr.ConvertToLong("nr_cpf");
			vo.CdDependente = dr.ConvertToInt("cd_dependente");
			vo.DtAdmissao = dr.ConvertToDateTime("dt_admissao").Value;
			vo.NmTitular = Convert.ToString(dr["nm_titular"]);
			vo.CdBeneficiarioTitular = Convert.ToInt32(dr["cd_beneficiario_titular"]);
			vo.CdLotacao = Convert.ToString(dr["cd_lotacao"]);
			vo.CdLocal = dr.ConvertToInt("cd_local");
			vo.Email = Convert.ToString(dr["ds_email"]);
			vo.NrCns = Convert.ToString(dr["nr_cns"]);

			vo.Endereco = new VO.EnderecoVO();
			vo.Endereco.Bairro = Convert.ToString(dr["ds_bairro"]);
			vo.Endereco.Cep = Convert.ToInt32(dr["nr_cep"]);
			vo.Endereco.Cidade = Convert.ToString(dr["ds_municipio"]);
			vo.Endereco.Complemento = Convert.ToString(dr["ds_complemento"]);
			//vo.Endereco.IdLocalidade = dr.IsNull("cd_municipio") ? 0 : Convert.ToInt32(dr["cd_municipio"]);
			vo.Endereco.NumeroEndereco = Convert.ToString(dr["nr_endereco"]);
			vo.Endereco.Rua = Convert.ToString(dr["ds_endereco"]);
			vo.Endereco.Uf = Convert.ToString(dr["ds_uf"]);

			if (vo.NrCpf != null && vo.NrCpf.Value == 0)
				vo.NrCpf = null;

			return vo;
		}

		internal static List<HcBeneficiarioVO> ListarBeneficiarios(int cdEmpresa, long cdFuncionario, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				"	from isa_hc.hc_v_beneficiario " +
				"	WHERE cd_funcionario = :cdFuncionario AND cd_empresa = :cdEmpresa";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdFuncionario", Tipo = DbType.Int64, Value = cdFuncionario });
			lstParams.Add(new Parametro() { Name = ":cdEmpresa", Tipo = DbType.Int32, Value = cdEmpresa });

			List<HcBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}

		internal static DataTable GetBeneficiarios(int cdEmpresa, long cdFuncionario, EvidaDatabase db) {
			string sql = "select plano.cd_plano, plano.tp_plano, plano.ds_plano, par.ds_parentesco, bplano.dt_inicio_vigencia, b.* " +
				" from isa_hc.hc_v_beneficiario b,  isa_hc.hc_grau_parentesco par, " +
				"		isa_hc.hc_beneficiario_plano bplano, isa_hc.hc_plano plano " +
				" where cd_funcionario = :cdFuncionario and cd_empresa = :cdEmpresa " +
				"		and b.cd_grau_parentesco = par.cd_parentesco(+)	" +
				"		and b.cd_beneficiario = bplano.cd_beneficiario " +
				"		and (bplano.dt_inicio_vigencia <= sysdate and (bplano.dt_termino_vigencia > sysdate or bplano.dt_termino_vigencia is null)) " +
				"		and plano.cd_plano = bplano.cd_plano_vinculado and plano.tp_plano = bplano.tp_plano " +
				" ORDER BY nvl(b.cd_dependente, 0) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdFuncionario", Tipo = DbType.Int64, Value = cdFuncionario });
			lstParams.Add(new Parametro() { Name = ":cdEmpresa", Tipo = DbType.Int32, Value = cdEmpresa });

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		private static int GetNextSeqBeneficiario(EvidaDatabase db) {
			string sql = "SELECT ISA_HC.SEQ_HC_BENEFICIARIO.nextval FROM DUAL";

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);

			return (int)idSolicitacao;
		}

		internal static void CriarBeneficiario(HcBeneficiarioVO benef, EvidaDatabase db) {
			string sql = "INSERT INTO ISA_HC.HC_BENEFICIARIO (CD_BENEFICIARIO, TP_BENEFICIARIO, CD_EMPRESA, CD_FUNCIONARIO, DT_ADMISSAO, " +
				" CD_SITUACAO_BENEF, CD_ALTERNATIVO, CD_BENEFICIARIO_TITULAR, DS_OBSERVACAO, " +
				" CD_DEPENDENTE, NR_CNS, USER_CREATE, DATE_CREATE) " +
				" VALUES (:id, :tpBeneficiario, :empresa, :matricula, :dtAdmissao, " +
				" :situacao, :cdAlternativo, :cdBenefTitular, :obs, " +
				" :cdDependente, :cns, :userInt, :dateInt)";

			benef.CdBeneficiario = GetNextSeqBeneficiario(db);

			if (benef.TpBeneficiario.Equals(Constantes.TIPO_BENEFICIARIO_FUNCIONARIO)) {
				benef.CdBeneficiarioTitular = benef.CdBeneficiario;
			}

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, benef.CdBeneficiario));
			lstParam.Add(new Parametro(":tpBeneficiario", DbType.String, benef.TpBeneficiario));

			lstParam.Add(new Parametro(":empresa", DbType.Int32, benef.CdEmpresa));
			lstParam.Add(new Parametro(":matricula", DbType.Int64, benef.CdFuncionario));
			lstParam.Add(new Parametro(":dtAdmissao", DbType.Date, benef.DtAdmissao));

			lstParam.Add(new Parametro(":situacao", DbType.String, benef.CdSituacaoBenef));
			lstParam.Add(new Parametro(":cdAlternativo", DbType.String, benef.CdAlternativo));
			lstParam.Add(new Parametro(":cdBenefTitular", DbType.Int32, benef.CdBeneficiarioTitular));

			lstParam.Add(new Parametro(":cdDependente", DbType.Int32, benef.CdDependente));
			lstParam.Add(new Parametro(":cns", DbType.String, benef.NrCns));

			lstParam.Add(new Parametro(":userInt", DbType.String, "INTRANET - INTEGRAÇÃO"));
			lstParam.Add(new Parametro(":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

			lstParam.Add(new Parametro(":obs", DbType.String, "Cirurgia, Intern., TC, RM, Cintilografia, Polissonografia, Terapias, Fisio buco-maxilo-facial, Trat. Dermat., Odonto, Monit. Epilepsia, DIU, Quimio, Sedação Profunda, Painel Hibrid. Mol., RPG, Hidroterapia e Home Care"));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void CriarBenefRespFinanceiro(HcBeneficiarioVO benef, DateTime dtInicio, EvidaDatabase db) {
			if (ExisteRespFinanceiro(benef, db))
				return;

			string sql = "INSERT INTO ISA_HC.hc_benef_resp_financeiro (CD_BENEFICIARIO, DT_INICIO_VIGENCIA, cd_beneficiario_financeiro, DS_OBSERVACAO, " +
				" USER_CREATE, DATE_CREATE) " +
				" VALUES (:id, :dtInicio, :cdResp, :obs, " +
				" :userInt, :dateInt)";
			
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, benef.CdBeneficiario));
			lstParams.Add(new Parametro(":dtInicio", DbType.Date, dtInicio));

			lstParams.Add(new Parametro(":cdResp", DbType.Int32, benef.CdBeneficiarioTitular));
			lstParams.Add(new Parametro(":obs", DbType.String, "CADASTRO"));
			
			lstParams.Add(new Parametro(":userInt", DbType.String, "INTRANET - INTEGRAÇÃO"));
			lstParams.Add(new Parametro(":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		private static bool ExisteRespFinanceiro(HcBeneficiarioVO benef, EvidaDatabase db) {
			string sql = "select count(1) " +
				" from isa_hc.hc_benef_resp_financeiro b " +
				" where b.cd_beneficiario = :cdBeneficiario " +
				"		and (b.dt_inicio_vigencia <= sysdate and (b.dt_termino_vigencia > sysdate or b.dt_termino_vigencia is null)) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":cdBeneficiario", DbType.Int32, benef.CdBeneficiario));

			object o = BaseDAO.ExecuteScalar(db, sql, lstParams);
			if (o != DBNull.Value)
				return Convert.ToInt32(o) > 0;
			return false;
		}

		internal static void CriarBenefHistTit(HcBeneficiarioVO benef, DateTime dtInicio, EvidaDatabase db) {
			if (ExisteBenefHistTit(benef, db))
				return;

			string sql = "INSERT INTO ISA_HC.hc_beneficiario_hist_tit (CD_BENEFICIARIO, DT_INICIO_VIGENCIA, " +
				" CD_EMPRESA, CD_FUNCIONARIO, DT_ADMISSAO, CD_DEPENDENTE, DS_MOTIVO_ALTERACAO, FL_PRIMEIRO_REG, " +
				" USER_CREATE, DATE_CREATE) " +
				" VALUES (:id, :dtInicio, :empresa, :matricula, :dtAdmissao, :cdDependente, :motivo, :flPrimeiro," +
				" :userInt, :dateInt)";
			
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, benef.CdBeneficiario));
			lstParams.Add(new Parametro(":dtInicio", DbType.Date, dtInicio));

			lstParams.Add(new Parametro(":empresa", DbType.Int32, benef.CdEmpresa));
			lstParams.Add(new Parametro(":matricula", DbType.Int64, benef.CdFuncionario));
			lstParams.Add(new Parametro(":dtAdmissao", DbType.Date, benef.DtAdmissao));

			lstParams.Add(new Parametro(":cdDependente", DbType.Int32, benef.CdDependente));

			lstParams.Add(new Parametro(":motivo", DbType.String, "INCLUSAO"));
			lstParams.Add(new Parametro(":flPrimeiro", DbType.String, "S"));

			lstParams.Add(new Parametro(":userInt", DbType.String, "INTRANET - INTEGRAÇÃO"));
			lstParams.Add(new Parametro(":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		private static bool ExisteBenefHistTit(HcBeneficiarioVO benef, EvidaDatabase db) {
			string sql = "select count(1) " +
				" from isa_hc.hc_beneficiario_hist_tit b " +
				" where b.cd_beneficiario = :cdBeneficiario " +
				"		and (b.dt_inicio_vigencia <= sysdate and (b.dt_termino_vigencia > sysdate or b.dt_termino_vigencia is null)) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int32, Value = benef.CdBeneficiario });

			object o = BaseDAO.ExecuteScalar(db, sql, lstParams);
			if (o != DBNull.Value)
				return Convert.ToInt32(o) > 0;
			return false;
		}

		internal static HcBeneficiarioVO GetByCns(string cns, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				"	from isa_hc.hc_v_beneficiario " +
				"	WHERE nr_cns = :cns";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cns", Tipo = DbType.String, Value = cns });

			return BaseDAO.ExecuteDataRow(db, sql, FromDataRow, lstParams);
		}
	}
}
