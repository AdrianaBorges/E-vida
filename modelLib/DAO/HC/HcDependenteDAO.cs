using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.HC {
	internal class HcDependenteDAO {

		internal static List<HcDependenteVO> ListarDependentes(HcFuncionarioVO funcionario, EvidaDatabase db) {
			string sql = "select * " +
				"	from isa_hc.hc_dependente " +
				"	WHERE cd_funcionario = :cdFuncionario AND cd_empresa = :cdEmpresa";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdFuncionario", Tipo = DbType.Int64, Value = funcionario.CdFuncionario });
			lstParams.Add(new Parametro() { Name = ":cdEmpresa", Tipo = DbType.Int32, Value = funcionario.CdEmpresa });

			List<HcDependenteVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}
		private static HcDependenteVO FromDataRow(DataRow dr) {
			HcDependenteVO vo = new HcDependenteVO();
			vo.CdEmpresa = (int)dr.Field<decimal>("CD_EMPRESA");
			vo.CdFuncionario = (long)dr.Field<decimal>("CD_FUNCIONARIO");
			vo.NmDependente = dr.Field<string>("NM_DEPENDENTE");
			vo.DtAdmissao = dr.Field<DateTime>("DT_ADMISSAO");
			vo.DtNascimento = BaseDAO.GetNullableDate(dr, "DT_NASCIMENTO");
			vo.NrCpf = dr["NR_CPF"] != DBNull.Value ? Convert.ToInt64(dr["NR_CPF"]) : new long?();
			vo.NrRg = dr.Field<string>("NR_RG");
			vo.DsOrgExpRg = dr.Field<string>("DS_ORG_EXP_RG");
			vo.CdUfOrgExpRg = dr.Field<string>("CD_UF_ORG_EXP_RG");
			vo.DataEmissaoRg = BaseDAO.GetNullableDate(dr, "DT_EMISSAO_NR_RG");
			vo.TpEstadoCivil = Convert.ToString(dr["tp_estado_civil"]);
			vo.CdGrauParentesco = Convert.ToString(dr["cd_grau_parentesco"]);
			vo.NmMae = Convert.ToString(dr["nm_mae"]);
			vo.NmPai = Convert.ToString(dr["nm_pai"]);
			vo.CdDependente = Convert.ToInt32(dr["cd_dependente"]);
			vo.FlDeficienteFisico = Convert.ToString(dr["fl_deficiente_fisico"]);
			vo.TpDependente = Convert.ToString(dr["tp_dependente"]);

			if (vo.NrCpf != null && vo.NrCpf.Value == 0)
				vo.NrCpf = null;

			return vo;
		}

		internal static void CriarDependente(HcDependenteVO dep, EvidaDatabase db) {
			dep.DsOrgExpRg = SCL.SclItemListaDAO.CheckItemLista(Constantes.LISTA_ORGAO_EXPEDIDOR, dep.DsOrgExpRg, db);


			string sql = "INSERT INTO ISA_HC.HC_DEPENDENTE (CD_EMPRESA, CD_FUNCIONARIO, DT_ADMISSAO, CD_DEPENDENTE, NM_DEPENDENTE, CD_GRAU_PARENTESCO, " +
				" TP_SEXO, TP_DEPENDENTE, FL_PRINCIPAL, FL_DEFICIENTE_FISICO, FL_DEP_IRRF, FL_USA_CONTA_FUNC, FL_USA_ENDERECO_FUNC, FL_ESTUDANTE, FL_DEP_SAL_FAMILIA, FL_ENTR_TERMO_TERM_DEP, " +
				" DT_NASCIMENTO, TP_ESTADO_CIVIL, NM_PAI, NM_MAE, NR_CPF, NR_RG, DS_ORG_EXP_RG, CD_UF_ORG_EXP_RG, DT_EMISSAO_NR_RG, " +
				" USER_CREATE, DATE_CREATE ) " +
				" VALUES " +
				" (:empresa, :matricula, :dtAdmissao, :id, :nome, :parentesco, " +
				" :sexo, :tpDependente, :flPrincipal, :flDeficiente, :flIrrf, :flUsaContaTit, :flUsaEndTit, :flEstudante, :flDepSalFamilia, :flTermoDep, " +
				" :nascimento, :estadoCivil, :pai, :mae, :cpf, :rg, :orgaoRg, :ufRg, :dtEmissaoRg, " +
				" :userInt, :dateInt) ";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":empresa", DbType.Int32, dep.CdEmpresa));
			lstParam.Add(new Parametro(":matricula", DbType.Int64, dep.CdFuncionario));
			lstParam.Add(new Parametro(":dtAdmissao", DbType.Date, dep.DtAdmissao));
			lstParam.Add(new Parametro(":id", DbType.Int32, dep.CdDependente));
			lstParam.Add(new Parametro(":nome", DbType.String, dep.NmDependente));

			lstParam.Add(new Parametro(":parentesco", DbType.String, dep.CdGrauParentesco));
			lstParam.Add(new Parametro(":sexo", DbType.String, dep.TpSexo));

			lstParam.Add(new Parametro(":tpDependente", DbType.String, dep.TpDependente));
			lstParam.Add(new Parametro(":flPrincipal", DbType.String, "N"));
			lstParam.Add(new Parametro(":flDeficiente", DbType.String, "N"));
			lstParam.Add(new Parametro(":flIrrf", DbType.String, "N"));
			lstParam.Add(new Parametro(":flUsaContaTit", DbType.String, "S"));
			lstParam.Add(new Parametro(":flUsaEndTit", DbType.String, "S"));
			lstParam.Add(new Parametro(":flEstudante", DbType.String, "N"));
			lstParam.Add(new Parametro(":flDepSalFamilia", DbType.String, "N"));
			lstParam.Add(new Parametro(":flTermoDep", DbType.String, "N"));

			lstParam.Add(new Parametro(":pai", DbType.String, dep.NmPai));
			lstParam.Add(new Parametro(":mae", DbType.String, dep.NmMae));

			lstParam.Add(new Parametro(":nascimento", DbType.Date, dep.DtNascimento));
			lstParam.Add(new Parametro(":estadoCivil", DbType.String, dep.TpEstadoCivil));

			if (dep.NrCpf != null && dep.NrCpf.Value == 0)
				dep.NrCpf = null;
			lstParam.Add(new Parametro(":cpf", DbType.Int64, dep.NrCpf));
			lstParam.Add(new Parametro(":rg", DbType.String, dep.NrRg));
			lstParam.Add(new Parametro(":orgaoRg", DbType.String, dep.DsOrgExpRg));
			lstParam.Add(new Parametro(":ufRg", DbType.String, dep.CdUfOrgExpRg));
			lstParam.Add(new Parametro(":dtEmissaoRg", DbType.Date, dep.DataEmissaoRg));

			lstParam.Add(new Parametro(":userInt", DbType.String, "INTRANET - INTEGRAÇÃO"));
			lstParam.Add(new Parametro(":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void AtualizarDependente(HcDependenteVO dep, EvidaDatabase db) {
			dep.DsOrgExpRg = SCL.SclItemListaDAO.CheckItemLista(Constantes.LISTA_ORGAO_EXPEDIDOR, dep.DsOrgExpRg, db);

			string sql = "UPDATE ISA_HC.HC_DEPENDENTE SET CD_GRAU_PARENTESCO = :parentesco, TP_SEXO = :sexo, DT_NASCIMENTO = :nascimento, " +
				" FL_PRINCIPAL= NVL(FL_PRINCIPAL, :flPrincipal), FL_DEFICIENTE_FISICO = NVL(FL_DEFICIENTE_FISICO, :flDeficiente), " +
				" FL_DEP_IRRF = NVL(FL_DEP_IRRF, :flIrrf), FL_USA_CONTA_FUNC = NVL(FL_USA_CONTA_FUNC,:flUsaContaTit), " +
				" FL_USA_ENDERECO_FUNC = NVL(FL_USA_ENDERECO_FUNC, :flUsaEndTit), FL_ESTUDANTE = NVL(FL_ESTUDANTE, :flEstudante), " +
				" FL_DEP_SAL_FAMILIA = NVL(FL_DEP_SAL_FAMILIA, :flDepSalFamilia), FL_ENTR_TERMO_TERM_DEP = NVL(FL_ENTR_TERMO_TERM_DEP, :flTermoDep), " +
				" TP_ESTADO_CIVIL = :estadoCivil, NM_PAI = :pai, NM_MAE = :mae, NR_CPF = :cpf, " +
				" NR_RG = :rg, DS_ORG_EXP_RG = :orgaoRg, CD_UF_ORG_EXP_RG = :ufRg, DT_EMISSAO_NR_RG = :dtEmissaoRg, " +
				" USER_UPDATE = :userInt, DATE_UPDATE = :dateInt " +
				" WHERE CD_EMPRESA = :empresa AND CD_FUNCIONARIO = :matricula AND CD_DEPENDENTE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":empresa", DbType.Int32, dep.CdEmpresa));
			lstParam.Add(new Parametro(":matricula", DbType.Int64, dep.CdFuncionario));
			lstParam.Add(new Parametro(":id", DbType.Int32, dep.CdDependente));

			lstParam.Add(new Parametro(":parentesco", DbType.String, dep.CdGrauParentesco));
			lstParam.Add(new Parametro(":sexo", DbType.String, dep.TpSexo));

			lstParam.Add(new Parametro(":flPrincipal", DbType.String, "N"));
			lstParam.Add(new Parametro(":flDeficiente", DbType.String, "N"));
			lstParam.Add(new Parametro(":flIrrf", DbType.String, "N"));
			lstParam.Add(new Parametro(":flUsaContaTit", DbType.String, "S"));
			lstParam.Add(new Parametro(":flUsaEndTit", DbType.String, "S"));
			lstParam.Add(new Parametro(":flEstudante", DbType.String, "N"));
			lstParam.Add(new Parametro(":flDepSalFamilia", DbType.String, "N"));
			lstParam.Add(new Parametro(":flTermoDep", DbType.String, "N"));

			lstParam.Add(new Parametro(":pai", DbType.String, dep.NmPai));
			lstParam.Add(new Parametro(":mae", DbType.String, dep.NmMae));

			lstParam.Add(new Parametro(":nascimento", DbType.Date, dep.DtNascimento));
			lstParam.Add(new Parametro(":estadoCivil", DbType.String, dep.TpEstadoCivil));

			lstParam.Add(new Parametro(":cpf", DbType.Int64, dep.NrCpf));
			lstParam.Add(new Parametro(":rg", DbType.String, dep.NrRg));
			lstParam.Add(new Parametro(":orgaoRg", DbType.String, dep.DsOrgExpRg));
			lstParam.Add(new Parametro(":ufRg", DbType.String, dep.CdUfOrgExpRg));
			lstParam.Add(new Parametro(":dtEmissaoRg", DbType.Date, dep.DataEmissaoRg));

			lstParam.Add(new Parametro(":userInt", DbType.String, "INTRANET - INTEGRAÇÃO"));
			lstParam.Add(new Parametro(":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

	}
}
