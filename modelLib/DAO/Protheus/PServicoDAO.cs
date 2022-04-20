using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	internal class PServicoDAO {

		internal static PServicoVO FromDataRow(DataRow dr) {
			PServicoVO vo = new PServicoVO();
			vo.CdRolAns = dr.Field<string>("cd_rol_ans");
			vo.Descricao = dr.Field<string>("ds_servico");
			vo.Mascara = dr.Field<string>("cd_mascara");
			vo.Tabela = dr.Field<string>("cd_tabela");
			return vo;
		}

		public static PServicoVO GetById(string cdTabela, string cdMascara, EvidaDatabase db) {
			string sql = @"select cd_mascara, cd_tabela, ds_servico, cd_rol_ans ";
			sql += @" from VW_PR_SERVICO servico ";

			sql += " WHERE cd_tabela = :tabela AND cd_mascara = :mascara";
			
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":tabela", DbType.String, cdTabela));
			lstParams.Add(new Parametro(":mascara", DbType.String, cdMascara));
			return BaseDAO.ExecuteDataRow(db, sql, FromDataRow, lstParams);
		}

		internal static PServicoVO GetByMascara(string mascara, EvidaDatabase db) {
			string sql = @"select cd_mascara, cd_tabela, ds_servico, cd_rol_ans ";
			sql += @" from VW_PR_SERVICO servico ";

			sql += " WHERE cd_mascara = :mascara";
			
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":mascara", DbType.String, mascara));
			return BaseDAO.ExecuteDataRow(db, sql, FromDataRow, lstParams);
		}

		public static DataTable PesquisarServico(string codTuss, string dsTuss, string plano, bool? hasInPlano, List<string> lstTabela, EvidaDatabase db) {
			string sql = @"select cd_mascara, cd_tabela, ds_servico, cd_rol_ans ";

			string sqlExistPlano = @"(SELECT 1 FROM VW_PR_REL_PLANO_GRUPO plano_grupo, VW_PR_PLANO plano, VW_PR_REL_GRUPO_SERVICO grupo_servico 
						WHERE plano.cd_plano = :cdPlano
							AND plano_grupo.cd_int_plano = plano.cd_int || plano.cd_plano
							AND grupo_servico.cd_int = plano.cd_int and grupo_servico.cd_grupo = plano_grupo.cd_grupo
							AND grupo_servico.cd_mascara = servico.cd_mascara AND grupo_servico.cd_tabela = servico.cd_tabela)";

			if (!string.IsNullOrEmpty(plano) && hasInPlano != null && hasInPlano.Value) {
				sql += ", " + sqlExistPlano.Replace("SELECT 1", "SELECT COUNT(1) ") + " AS HAS_PLANO ";
			}

			sql += @" from VW_PR_SERVICO servico 
				WHERE ck_ativo = '1'";
					
			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(codTuss)) {
				lstParams.Add(new Parametro(":codTuss", DbType.String, codTuss));
				sql += " AND cd_mascara = :codTuss";
			}
			if (!string.IsNullOrEmpty(dsTuss)) {
				lstParams.Add(new Parametro(":nome", DbType.String, "%" + dsTuss.ToUpper() + "%"));
                sql += " AND upper(trim(ds_servico)) LIKE upper(trim(:nome)) ";
			}

			if (lstTabela != null) {
				sql += " and cd_tabela in ('' ";
				foreach (string cdTabela in lstTabela) {
					sql += ", '" + cdTabela + "'";
				}
				sql += ")";
			}

			if (!string.IsNullOrEmpty(plano) && (hasInPlano == null || !hasInPlano.Value)) {
				sql += @" AND EXISTS " + sqlExistPlano;
			}
			if (!string.IsNullOrEmpty(plano)) {
				lstParams.Add(new Parametro(":cdPlano", DbType.String, plano));
			}
			
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);
			dt.PrimaryKey = new DataColumn[] { dt.Columns[0], dt.Columns[1] };
			return dt;
		}

	}
}
