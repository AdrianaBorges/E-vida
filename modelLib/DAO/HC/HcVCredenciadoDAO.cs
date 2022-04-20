using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO.HC {
	internal class HcVCredenciadoDAO {
		private static string FIELDS = "v.cd_credenciado, v.nm_razao_social, v.nm_fantasia, v.tp_pessoa, v.nr_cnpj_cpf, v.nr_pis, " +
			"	v.nr_inscricao_inss, v.nr_inscricao_estadual, v.nr_inscricao_municipal, v.ds_email, v.tp_sistema_atend, v.st_credenciado, " +
			"	v.tp_classe_credenciado, v.cd_natureza_credenciado, v.tx_administrativa, v.nr_contrato, v.dt_inicio_contrato, v.dt_termino_contrato," +
			"	v.nr_reg_conselho_reg, v.tp_valor_pagar, v.tp_pagamento, v.ds_obs, v.tp_cobranca_filme, v.cns_cnes, v.nr_ans, v.cd_externo, " +
			"	v.cd_holding, v.fl_guiamedico, v.cd_nivel_hospitalar ";

		internal static HcVCredenciadoVO GetById(int cdCredenciado, EvidaDatabase db) {
			string sql = "select " + FIELDS + ", c.nr_formula, c.tp_dias " +
				" from isa_hc.hc_v_credenciado v, isa_hc.hc_credenciado c WHERE c.cd_credenciado = :cdCredenciado AND v.cd_credenciado = c.cd_credenciado ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdCredenciado", Tipo = DbType.Int32, Value = cdCredenciado });

			List<HcVCredenciadoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowVCred, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static HcVCredenciadoVO GetByDoc(long cpfCnpj, EvidaDatabase db) {
			string sql = "select " + FIELDS + ", null as nr_formula, null as tp_dias " +
				" from isa_hc.hc_v_credenciado v WHERE NR_CNPJ_CPF = :cpfCnpj";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cpfCnpj", Tipo = DbType.Int64, Value = cpfCnpj });

			List<HcVCredenciadoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowVCred, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static HcVCredenciadoVO FromDataRowVCred(DataRow dr) {
			HcVCredenciadoVO vo = new HcVCredenciadoVO();
			vo.CdCredenciado = Convert.ToInt32(dr["cd_credenciado"]);
			vo.RazaoSocial = dr.Field<string>("nm_razao_social");
			vo.NomeFantasia = dr.Field<string>("nm_fantasia");
			vo.TipoPessoa = dr.Field<string>("tp_pessoa");
			vo.CpfCnpj = (long)dr.Field<decimal>("nr_cnpj_cpf");
			vo.Email = dr.Field<string>("ds_email");
			vo.TipoSistemaAtendimento = dr.Field<string>("tp_sistema_atend");
			vo.Situacao = dr.Field<string>("st_credenciado");
			vo.TipoValorPagar = dr.Field<string>("tp_valor_pagar");
			vo.TipoPagamento = dr.Field<string>("tp_pagamento");
			vo.TipoCobrancaFilme = dr.Field<string>("tp_cobranca_filme");
			vo.CdNatureza = BaseDAO.GetNullableInt(dr, "cd_natureza_credenciado");
			vo.NrFormula = BaseDAO.GetNullableInt(dr, "nr_formula");
			return vo;
		}

		private static HcCredenciadoFoneVO FromDataRowFone(DataRow dr) {
			HcCredenciadoFoneVO vo = new HcCredenciadoFoneVO();
			vo.CdCredenciado = Convert.ToInt32(dr["cd_credenciado"]);
			vo.Ddd = BaseDAO.GetNullableInt(dr, "nr_ddd").Value;
			vo.Telefone = dr.Field<string>("nr_telefone");
			vo.Ramal = Convert.ToString(dr["nr_ramal"]);
			vo.TpTelefone = dr.Field<string>("tp_telefone");
			return vo;
		}

		private static HcCredenciadoEnderecoVO FromDataRowEndereco(DataRow dr) {
			HcCredenciadoEnderecoVO vo = new HcCredenciadoEnderecoVO();
			vo.CdCredenciado = Convert.ToInt32(dr["cd_credenciado"]);
			vo.TpEndereco = dr.Field<string>("tp_endereco");
			vo.Cep = BaseDAO.GetNullableInt(dr, "nr_cep");
			vo.DsBairro = dr.Field<string>("ds_bairro");
			vo.CdMunicipio = BaseDAO.GetNullableInt(dr, "cd_municipio");
			vo.DsComplemento = dr.Field<string>("ds_complemento");
			vo.DsEndereco = dr.Field<string>("ds_endereco");
			vo.Uf = dr.Field<string>("cd_uf");

			return vo;
		}

		internal static DataTable Pesquisar(string razaoSocial, long? cpfCnpj, bool hospital, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				" from isa_hc.hc_v_credenciado v WHERE 1 = 1 ";

			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(razaoSocial)) {
                sql += " AND upper(trim(nm_razao_social)) LIKE upper(trim(:razaoSocial)) ";
				lstParams.Add(new Parametro() { Name = ":razaoSocial", Tipo = DbType.String, Value = "%" + razaoSocial.ToUpper() + "%" });
			}

			if (cpfCnpj != null) {
				sql += " AND nr_cnpj_cpf = :cpfCnpj ";
				lstParams.Add(new Parametro() { Name = ":cpfCnpj", Tipo = DbType.Int64, Value = cpfCnpj.Value });
			}

			if (hospital) {
				sql += " AND tp_sistema_atend IN ('CRED', 'AMB')";
			}
			return BaseDAO.ExecuteDataSet(db, sql, lstParams);
		}

		internal static int GetRegionalCredenciado(int idCredenciado, DateTime? dtVigencia, EvidaDatabase db) {
			string sql = "select cd_regional " +
				" FROM ISA_HC.hc_credenciado_regional cr WHERE cr.dt_inicio_vigencia < :vigencia and (dt_termino_vigencia is null or dt_termino_vigencia > :vigencia) "+
				"	and CR.CD_cREDENCIADO = :idCredenciado ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":vigencia", Tipo = DbType.Date, Value = dtVigencia == null ? DateTime.Now.Date : dtVigencia.Value });
			lstParams.Add(new Parametro() { Name = ":idCredenciado", Tipo = DbType.Int32, Value = idCredenciado });
			object o = BaseDAO.ExecuteScalar(db, sql, lstParams);
			if (o == DBNull.Value)
				return 0;
			return Convert.ToInt32(o);
		}

		internal static List<HcCredenciadoFoneVO> ListarFones(int idCredenciado, EvidaDatabase db) {
			string sql = "select * FROM isa_hc.hc_credenciado_fone f WHERE cd_credenciado = :idCredenciado";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idCredenciado", Tipo = DbType.Int32, Value = idCredenciado });
			List<HcCredenciadoFoneVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowFone, lstParams);
			return lst;
		}

		internal static List<HcCredenciadoEnderecoVO> ListarEnderecos(int idCredenciado, EvidaDatabase db) {
			string sql = "select * FROM isa_hc.hc_credenciado_endereco f WHERE cd_credenciado = :idCredenciado";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":idCredenciado", Tipo = DbType.Int32, Value = idCredenciado });
			List<HcCredenciadoEnderecoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowEndereco, lstParams);
			return lst;
		}
	}
}
