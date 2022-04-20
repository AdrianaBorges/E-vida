using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	public class PBeneficiarioDAO {
		static string FIELDS = @"cd_int, cd_emp, cd_matricula, cd_mat_vida, nm_beneficiario, nr_cpf, dt_nascimento, cd_alternativo, tp_beneficiario, cd_grau_parentesco, 
			nm_mae, nm_pai, dt_validade_carteira, ds_email, cd_cco, ds_obs_aut, tp_carencia";

		internal static PBeneficiarioVO GetRowByCartao(string numCartao, EvidaDatabase db) {
			string sql = "select " + FIELDS +
                " from VW_PR_BENEFICIARIO WHERE trim(cd_alternativo) = trim(:numCartao) OR REPLACE(cd_alternativo, '-', '') = REPLACE(:numCartao, '-', '')";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":numCartao", Tipo = DbType.String, Value = numCartao });

			List<PBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static PBeneficiarioVO GetRow(string codInt, string codEmp, string codMatricula, EvidaDatabase db) {
			string sql = "select " + FIELDS +
                " from VW_PR_BENEFICIARIO WHERE trim(cd_int) = trim(:codInt) AND trim(cd_emp) = trim(:codEmp) AND trim(cd_matricula) = trim(:codMatricula)";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":codInt", DbType.String, codInt));
			lstParams.Add(new Parametro(":codEmp", DbType.String, codEmp));
			lstParams.Add(new Parametro(":codMatricula", DbType.String, codMatricula));

			List<PBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static PBeneficiarioVO FromDataRow(DataRow dr) {
			PBeneficiarioVO vo = new PBeneficiarioVO();
			vo.CodEmp = dr.Field<string>("cd_int");
			vo.CodInt = dr.Field<string>("cd_emp");
			vo.Matricula = dr.Field<string>("cd_matricula");

			vo.CdAlternativo = dr.Field<string>("cd_alternativo");
			vo.CdGrauParentesco = dr.Field<string>("cd_grau_parentesco");
			vo.Cpf = ProtheusDAOHelper.GetLong(dr, "nr_cpf");
			vo.DtNascimento = ProtheusDAOHelper.GetDateNative(dr, "dt_nascimento").Value;
			vo.DtValidadeCarteira = ProtheusDAOHelper.GetDateNative(dr, "dt_validade_carteira").Value;
			vo.Email = ProtheusDAOHelper.GetTrimString(dr, "ds_email");
			vo.MatriculaVida = ProtheusDAOHelper.GetTrimString(dr, "cd_mat_vida");
			vo.Nome = ProtheusDAOHelper.GetTrimString(dr, "nm_beneficiario");
			vo.TpBeneficiario = dr.Field<string>("tp_beneficiario");
			return vo;
		}
	}
}
