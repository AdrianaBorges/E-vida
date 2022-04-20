using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.HC {
	internal class HcDemonstrativoAnaliseContaDAO {
		public static List<HcDemonstrativoAnaliseContaVO> ListarSolicitacoes(long cpfCnpj, string docFiscal, EvidaDatabase db) {
			string sql = "select * " +
				"	from isa_hc.gal_demonst_analise_conta_job  " +
				"	WHERE nr_cnpj_cpf = :cnpj AND nr_docto = :docFiscal";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cnpj", Tipo = DbType.Int64, Value = cpfCnpj });
			lstParams.Add(new Parametro() { Name = ":docFiscal", Tipo = DbType.String, Value = docFiscal });

			List<HcDemonstrativoAnaliseContaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			return lst;
		}
		//  insert into gal_demonst_analise_conta_job (nr_cnpj_cpf, dt_ano_mes_ref, nr_docto)
		// Values (11006293000211, to_date('01/01/2015', 'dd/mm/yyyy'), '65110');
		private static HcDemonstrativoAnaliseContaVO FromDataRow(DataRow dr) {
			HcDemonstrativoAnaliseContaVO vo = new HcDemonstrativoAnaliseContaVO();
			vo.CpfCnpj = Convert.ToInt64(dr["nr_cnpj_cpf"]);
			vo.DocumentoFiscal = Convert.ToString(dr["nr_docto"]);
			vo.Referencia = Convert.ToDateTime(dr["dt_ano_mes_ref"]);
			return vo;
		}

		public static void GerarSolicitacao(HcDemonstrativoAnaliseContaVO vo, EvidaDatabase db) {
			string sql = "INSERT INTO ISA_HC.gal_demonst_analise_conta_job (nr_cnpj_cpf, dt_ano_mes_ref, nr_docto) " +
				"	VALUES (:cnpj, :referencia, :docFiscal) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":cnpj", DbType.Int64, vo.CpfCnpj));
			lstParams.Add(new Parametro(":docFiscal", DbType.String, vo.DocumentoFiscal));
			lstParams.Add(new Parametro(":referencia", DbType.Date, vo.Referencia));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}
	}
}
