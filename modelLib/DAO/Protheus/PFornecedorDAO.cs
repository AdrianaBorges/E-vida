using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.DAO.Protheus
{
    internal class PFornecedorDAO
    {
        private static string FIELDS = " a2_cod, a2_loja, a2_nome, a2_nreduz, a2_end, a2_nr_end, a2_complem, a2_bairro, a2_codmun, a2_mun, a2_est, " +
                " a2_cep, a2_tipo, a2_tpessoa, a2_cgc, a2_ddd, a2_tel, a2_telcom, a2_telres, a2_banco, a2_agencia, a2_numcon, a2_pais, a2_codpais, a2_email ";

        private static string NextId(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral
            string sql = "select to_number(max(trim(a2_cod))) + 1 from SA2010 where evida.is_number(trim(a2_cod)) = 1";
            int cod = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql));
            return cod.ToString().PadLeft(6, '0');
        }

        private static Int64 NextRecno(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from SA2010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }

        internal static PFornecedorVO GetFornecedor(string cgc, EvidaDatabase db)
        {
            string sql = "select * from (select " + FIELDS +
                "	from VW_PR_FORNECEDOR " +
                "	where upper(trim(a2_cgc)) like upper(trim(:cgc)) " +
                "   ) where rownum = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":cgc", Tipo = DbType.String, Value = cgc.Trim() });

            List<PFornecedorVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        private static PFornecedorVO FromDataRow(DataRow dr)
        {
            PFornecedorVO vo = new PFornecedorVO();

            vo.Cod = dr["a2_cod"] != DBNull.Value ? dr.Field<string>("a2_cod") : String.Empty;
            vo.Loja = dr["a2_loja"] != DBNull.Value ? dr.Field<string>("a2_loja") : String.Empty;
            vo.Nome = dr["a2_nome"] != DBNull.Value ? dr.Field<string>("a2_nome") : String.Empty;
            vo.Nreduz = dr["a2_nreduz"] != DBNull.Value ? dr.Field<string>("a2_nreduz") : String.Empty;
            vo.End = dr["a2_end"] != DBNull.Value ? dr.Field<string>("a2_end") : String.Empty;
            vo.Nrend = dr["a2_nr_end"] != DBNull.Value ? dr.Field<string>("a2_nr_end") : String.Empty;
            vo.Complem = dr["a2_complem"] != DBNull.Value ? dr.Field<string>("a2_complem") : String.Empty;
            vo.Bairro = dr["a2_bairro"] != DBNull.Value ? dr.Field<string>("a2_bairro") : String.Empty;
            vo.Codmun = dr["a2_codmun"] != DBNull.Value ? dr.Field<string>("a2_codmun") : String.Empty;
            vo.Mun = dr["a2_mun"] != DBNull.Value ? dr.Field<string>("a2_mun") : String.Empty;
            vo.Est = dr["a2_est"] != DBNull.Value ? dr.Field<string>("a2_est") : String.Empty;
            vo.Cep = dr["a2_cep"] != DBNull.Value ? dr.Field<string>("a2_cep") : String.Empty;
            vo.Tipo = dr["a2_tipo"] != DBNull.Value ? dr.Field<string>("a2_tipo") : String.Empty;
            vo.Tpessoa = dr["a2_tpessoa"] != DBNull.Value ? dr.Field<string>("a2_tpessoa") : String.Empty;
            vo.Cgc = dr["a2_cgc"] != DBNull.Value ? dr.Field<string>("a2_cgc") : String.Empty;
            vo.Ddd = dr["a2_ddd"] != DBNull.Value ? dr.Field<string>("a2_ddd") : String.Empty;
            vo.Tel = dr["a2_tel"] != DBNull.Value ? dr.Field<string>("a2_tel") : String.Empty;
            vo.Telcom = dr["a2_telcom"] != DBNull.Value ? dr.Field<string>("a2_telcom") : String.Empty;
            vo.Telres = dr["a2_telres"] != DBNull.Value ? dr.Field<string>("a2_telres") : String.Empty;
            vo.Banco = dr["a2_banco"] != DBNull.Value ? dr.Field<string>("a2_banco") : String.Empty;
            vo.Agencia = dr["a2_agencia"] != DBNull.Value ? dr.Field<string>("a2_agencia") : String.Empty;
            vo.Numcon = dr["a2_numcon"] != DBNull.Value ? dr.Field<string>("a2_numcon") : String.Empty;
            vo.Pais = dr["a2_pais"] != DBNull.Value ? dr.Field<string>("a2_pais") : String.Empty;
            vo.Codpais = dr["a2_codpais"] != DBNull.Value ? dr.Field<string>("a2_codpais") : String.Empty;
            vo.Email = dr["a2_email"] != DBNull.Value ? dr.Field<string>("a2_email") : String.Empty;

            return vo;
        }

        internal static void CriarFornecedor(PFornecedorVO fornecedor, EvidaDatabase db)
        {
            string sql = "insert into SA2010 (a2_cod, a2_loja, a2_nome, a2_nreduz, a2_end, a2_nr_end, a2_complem, a2_bairro, a2_codmun, a2_mun, a2_est, " +
                " a2_cep, a2_tipo, a2_tpessoa, a2_cgc, a2_ddd, a2_tel, a2_telcom, a2_telres, a2_banco, a2_agencia, a2_numcon, a2_pais, a2_codpais, a2_email, R_E_C_N_O_) " +
                " values (:cod, :loja, :nome, :nreduz, :end, :nrend, :complem, :bairro, :codmun, :mun, :est, :cep, :tipo, :tpessoa, :cgc, :ddd, :tel, :telcom, :telres, " +
                " :banco, :agencia, :numcon, :pais, :codpais, :email, :recno)";

            Int64 recno = NextRecno(db);
            fornecedor.Cod = NextId(db);

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(fornecedor.Cod == null ? new Parametro(":cod", DbType.String, "".PadRight(6, ' ')) : new Parametro(":cod", DbType.String, fornecedor.Cod.Trim().Length >= 6 ? fornecedor.Cod.Trim().Substring(0, 6) : fornecedor.Cod.Trim().PadRight(6, ' ')));
            lstParam.Add(new Parametro(":loja", DbType.String, "01"));
            lstParam.Add(fornecedor.Nome == null ? new Parametro(":nome", DbType.String, "".PadRight(40, ' ')) : new Parametro(":nome", DbType.String, fornecedor.Nome.Trim().Length >= 40 ? fornecedor.Nome.Trim().Substring(0, 40) : fornecedor.Nome.Trim().PadRight(40, ' ')));
            lstParam.Add(fornecedor.Nreduz == null ? new Parametro(":nreduz", DbType.String, "".PadRight(20, ' ')) : new Parametro(":nreduz", DbType.String, fornecedor.Nreduz.Trim().Length >= 20 ? fornecedor.Nreduz.Trim().Substring(0, 20) : fornecedor.Nreduz.Trim().PadRight(20, ' ')));
            lstParam.Add(fornecedor.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, fornecedor.End.Trim().Length >= 40 ? fornecedor.End.Trim().Substring(0, 40) : fornecedor.End.Trim().PadRight(40, ' ')));
            lstParam.Add(fornecedor.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, fornecedor.Nrend.Trim().Length >= 6 ? fornecedor.Nrend.Trim().Substring(0, 6) : fornecedor.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(fornecedor.Complem == null ? new Parametro(":complem", DbType.String, "".PadRight(50, ' ')) : new Parametro(":complem", DbType.String, fornecedor.Complem.Trim().Length >= 50 ? fornecedor.Complem.Trim().Substring(0, 50) : fornecedor.Complem.Trim().PadRight(50, ' ')));
            lstParam.Add(fornecedor.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, fornecedor.Bairro.Trim().Length >= 40 ? fornecedor.Bairro.Trim().Substring(0, 40) : fornecedor.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(fornecedor.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(5, ' ')) : new Parametro(":codmun", DbType.String, fornecedor.Codmun.Trim().Length >= 5 ? fornecedor.Codmun.Trim().Substring(0, 5) : fornecedor.Codmun.Trim().PadRight(5, ' ')));
            lstParam.Add(fornecedor.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(50, ' ')) : new Parametro(":mun", DbType.String, fornecedor.Mun.Trim().Length >= 50 ? fornecedor.Mun.Trim().Substring(0, 50) : fornecedor.Mun.Trim().PadRight(50, ' ')));
            lstParam.Add(fornecedor.Est == null ? new Parametro(":est", DbType.String, "".PadRight(2, ' ')) : new Parametro(":est", DbType.String, fornecedor.Est.Trim().Length >= 2 ? fornecedor.Est.Trim().Substring(0, 2) : fornecedor.Est.Trim().PadRight(2, ' ')));
            lstParam.Add(fornecedor.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, fornecedor.Cep.Trim().Length >= 8 ? fornecedor.Cep.Trim().Substring(0, 8) : fornecedor.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(new Parametro(":tipo", DbType.String, "F"));
            lstParam.Add(new Parametro(":tpessoa", DbType.String, "PF"));
            lstParam.Add(fornecedor.Cgc == null ? new Parametro(":cgc", DbType.String, "".PadRight(14, ' ')) : new Parametro(":cgc", DbType.String, fornecedor.Cgc.Trim().Length >= 14 ? fornecedor.Cgc.Trim().Substring(0, 14) : fornecedor.Cgc.Trim().PadRight(14, ' ')));
            lstParam.Add(fornecedor.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, fornecedor.Ddd.Trim().Length >= 3 ? fornecedor.Ddd.Trim().Substring(0, 3) : fornecedor.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(fornecedor.Tel == null ? new Parametro(":tel", DbType.String, "".PadRight(15, ' ')) : new Parametro(":tel", DbType.String, fornecedor.Tel.Trim().Length >= 15 ? fornecedor.Tel.Trim().Substring(0, 15) : fornecedor.Tel.Trim().PadRight(15, ' ')));
            lstParam.Add(fornecedor.Telcom == null ? new Parametro(":telcom", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telcom", DbType.String, fornecedor.Telcom.Trim().Length >= 15 ? fornecedor.Telcom.Trim().Substring(0, 15) : fornecedor.Telcom.Trim().PadRight(15, ' ')));
            lstParam.Add(fornecedor.Telres == null ? new Parametro(":telres", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telres", DbType.String, fornecedor.Telres.Trim().Length >= 15 ? fornecedor.Telres.Trim().Substring(0, 15) : fornecedor.Telres.Trim().PadRight(15, ' ')));
            lstParam.Add(fornecedor.Banco == null ? new Parametro(":banco", DbType.String, "".PadRight(3, ' ')) : new Parametro(":banco", DbType.String, fornecedor.Banco.Trim().Length >= 3 ? fornecedor.Banco.Trim().Substring(0, 3) : fornecedor.Banco.Trim().PadRight(3, ' ')));
            lstParam.Add(fornecedor.Agencia == null ? new Parametro(":agencia", DbType.String, "".PadRight(5, ' ')) : new Parametro(":agencia", DbType.String, fornecedor.Agencia.Trim().Length >= 5 ? fornecedor.Agencia.Trim().Substring(0, 5) : fornecedor.Agencia.Trim().PadRight(5, ' ')));
            lstParam.Add(fornecedor.Numcon == null ? new Parametro(":numcon", DbType.String, "".PadRight(10, ' ')) : new Parametro(":numcon", DbType.String, fornecedor.Numcon.Trim().Length >= 10 ? fornecedor.Numcon.Trim().Substring(0, 10) : fornecedor.Numcon.Trim().PadRight(10, ' ')));
            lstParam.Add(new Parametro(":pais", DbType.String, "105"));
            lstParam.Add(new Parametro(":codpais", DbType.String, "01058"));
            lstParam.Add(fornecedor.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, fornecedor.Email.Trim().Length >= 100 ? fornecedor.Email.Trim().Substring(0, 100) : fornecedor.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void AlterarFornecedor(PFornecedorVO fornecedor, EvidaDatabase db)
        {
            string sql = "update SA2010 set a2_nome = :nome, a2_nreduz = :nreduz, a2_end = :end, a2_nr_end = :nrend, a2_complem = :complem, " +
                " a2_bairro = :bairro, a2_codmun = :codmun, a2_mun = :mun, a2_est = :est, a2_cep = :cep, a2_ddd = :ddd, " +
                " a2_tel = :tel, a2_telcom = :telcom, a2_telres = :telres, a2_banco = :banco, a2_agencia = :agencia, a2_numcon = :numcon, a2_email = :email " +
                " where upper(trim(a2_cgc)) = upper(trim(:cgc)) ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(fornecedor.Nome == null ? new Parametro(":nome", DbType.String, "".PadRight(40, ' ')) : new Parametro(":nome", DbType.String, fornecedor.Nome.Trim().Length >= 40 ? fornecedor.Nome.Trim().Substring(0, 40) : fornecedor.Nome.Trim().PadRight(40, ' ')));
            lstParam.Add(fornecedor.Nreduz == null ? new Parametro(":nreduz", DbType.String, "".PadRight(20, ' ')) : new Parametro(":nreduz", DbType.String, fornecedor.Nreduz.Trim().Length >= 20 ? fornecedor.Nreduz.Trim().Substring(0, 20) : fornecedor.Nreduz.Trim().PadRight(20, ' ')));
            lstParam.Add(fornecedor.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, fornecedor.End.Trim().Length >= 40 ? fornecedor.End.Trim().Substring(0, 40) : fornecedor.End.Trim().PadRight(40, ' ')));
            lstParam.Add(fornecedor.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, fornecedor.Nrend.Trim().Length >= 6 ? fornecedor.Nrend.Trim().Substring(0, 6) : fornecedor.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(fornecedor.Complem == null ? new Parametro(":complem", DbType.String, "".PadRight(50, ' ')) : new Parametro(":complem", DbType.String, fornecedor.Complem.Trim().Length >= 50 ? fornecedor.Complem.Trim().Substring(0, 50) : fornecedor.Complem.Trim().PadRight(50, ' ')));
            lstParam.Add(fornecedor.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, fornecedor.Bairro.Trim().Length >= 40 ? fornecedor.Bairro.Trim().Substring(0, 40) : fornecedor.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(fornecedor.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(5, ' ')) : new Parametro(":codmun", DbType.String, fornecedor.Codmun.Trim().Length >= 5 ? fornecedor.Codmun.Trim().Substring(0, 5) : fornecedor.Codmun.Trim().PadRight(5, ' ')));
            lstParam.Add(fornecedor.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(50, ' ')) : new Parametro(":mun", DbType.String, fornecedor.Mun.Trim().Length >= 50 ? fornecedor.Mun.Trim().Substring(0, 50) : fornecedor.Mun.Trim().PadRight(50, ' ')));
            lstParam.Add(fornecedor.Est == null ? new Parametro(":est", DbType.String, "".PadRight(2, ' ')) : new Parametro(":est", DbType.String, fornecedor.Est.Trim().Length >= 2 ? fornecedor.Est.Trim().Substring(0, 2) : fornecedor.Est.Trim().PadRight(2, ' ')));
            lstParam.Add(fornecedor.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, fornecedor.Cep.Trim().Length >= 8 ? fornecedor.Cep.Trim().Substring(0, 8) : fornecedor.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(fornecedor.Cgc == null ? new Parametro(":cgc", DbType.String, "".PadRight(14, ' ')) : new Parametro(":cgc", DbType.String, fornecedor.Cgc.Trim().Length >= 14 ? fornecedor.Cgc.Trim().Substring(0, 14) : fornecedor.Cgc.Trim().PadRight(14, ' ')));
            lstParam.Add(fornecedor.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, fornecedor.Ddd.Trim().Length >= 3 ? fornecedor.Ddd.Trim().Substring(0, 3) : fornecedor.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(fornecedor.Tel == null ? new Parametro(":tel", DbType.String, "".PadRight(15, ' ')) : new Parametro(":tel", DbType.String, fornecedor.Tel.Trim().Length >= 15 ? fornecedor.Tel.Trim().Substring(0, 15) : fornecedor.Tel.Trim().PadRight(15, ' ')));
            lstParam.Add(fornecedor.Telcom == null ? new Parametro(":telcom", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telcom", DbType.String, fornecedor.Telcom.Trim().Length >= 15 ? fornecedor.Telcom.Trim().Substring(0, 15) : fornecedor.Telcom.Trim().PadRight(15, ' ')));
            lstParam.Add(fornecedor.Telres == null ? new Parametro(":telres", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telres", DbType.String, fornecedor.Telres.Trim().Length >= 15 ? fornecedor.Telres.Trim().Substring(0, 15) : fornecedor.Telres.Trim().PadRight(15, ' ')));
            lstParam.Add(fornecedor.Banco == null ? new Parametro(":banco", DbType.String, "".PadRight(3, ' ')) : new Parametro(":banco", DbType.String, fornecedor.Banco.Trim().Length >= 3 ? fornecedor.Banco.Trim().Substring(0, 3) : fornecedor.Banco.Trim().PadRight(3, ' ')));
            lstParam.Add(fornecedor.Agencia == null ? new Parametro(":agencia", DbType.String, "".PadRight(5, ' ')) : new Parametro(":agencia", DbType.String, fornecedor.Agencia.Trim().Length >= 5 ? fornecedor.Agencia.Trim().Substring(0, 5) : fornecedor.Agencia.Trim().PadRight(5, ' ')));
            lstParam.Add(fornecedor.Numcon == null ? new Parametro(":numcon", DbType.String, "".PadRight(10, ' ')) : new Parametro(":numcon", DbType.String, fornecedor.Numcon.Trim().Length >= 10 ? fornecedor.Numcon.Trim().Substring(0, 10) : fornecedor.Numcon.Trim().PadRight(10, ' ')));
            lstParam.Add(fornecedor.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, fornecedor.Email.Trim().Length >= 100 ? fornecedor.Email.Trim().Substring(0, 100) : fornecedor.Email.Trim().PadRight(100, ' ')));
            
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }
    }
}
