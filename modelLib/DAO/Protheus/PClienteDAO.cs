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
    internal class PClienteDAO
    {
        private static string FIELDS = " a1_cod, a1_pessoa, a1_cgc, a1_nome, a1_nreduz, a1_cep, a1_email, a1_end, a1_nr_end, a1_complem, a1_bairro, " +
                " a1_est, a1_cod_mun, a1_mun, a1_ddd, a1_tel, a1_pais, a1_msblql, a1_codpais, a1_endcob ";

        internal static List<PClienteVO> ListarClientes(string cgc, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                "	from VW_PR_CLIENTE " +
                "	where upper(trim(a1_cgc)) like upper(trim(:cgc))  ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":cgc", Tipo = DbType.String, Value = cgc.Trim() });

            List<PClienteVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            return lst;
        }

        internal static PClienteVO GetCliente(string cgc, EvidaDatabase db)
        {
            string sql = "select * from (select " + FIELDS +
                "	from VW_PR_CLIENTE " +
                "	where upper(trim(a1_cgc)) like upper(trim(:cgc)) " +
                "   order by a1_cod desc) where rownum = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":cgc", Tipo = DbType.String, Value = cgc.Trim() });

            List<PClienteVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        private static PClienteVO FromDataRow(DataRow dr)
        {
            PClienteVO vo = new PClienteVO();

            vo.Cod = dr["a1_cod"] != DBNull.Value ? dr.Field<string>("a1_cod") : String.Empty;
            vo.Pessoa = dr["a1_pessoa"] != DBNull.Value ? dr.Field<string>("a1_pessoa") : String.Empty;
            vo.Cgc = dr["a1_cgc"] != DBNull.Value ? dr.Field<string>("a1_cgc") : String.Empty;
            vo.Nome = dr["a1_nome"] != DBNull.Value ? dr.Field<string>("a1_nome") : String.Empty;
            vo.Nreduz = dr["a1_nreduz"] != DBNull.Value ? dr.Field<string>("a1_nreduz") : String.Empty;
            vo.Cep = dr["a1_cep"] != DBNull.Value ? dr.Field<string>("a1_cep") : String.Empty;
            vo.Email = dr["a1_email"] != DBNull.Value ? dr.Field<string>("a1_email") : String.Empty;
            vo.End = dr["a1_end"] != DBNull.Value ? dr.Field<string>("a1_end") : String.Empty;
            vo.Nrend = dr["a1_nr_end"] != DBNull.Value ? dr.Field<string>("a1_nr_end") : String.Empty;
            vo.Complem = dr["a1_complem"] != DBNull.Value ? dr.Field<string>("a1_complem") : String.Empty;
            vo.Bairro = dr["a1_bairro"] != DBNull.Value ? dr.Field<string>("a1_bairro") : String.Empty;
            vo.Est = dr["a1_est"] != DBNull.Value ? dr.Field<string>("a1_est") : String.Empty;
            vo.Codmun = dr["a1_cod_mun"] != DBNull.Value ? dr.Field<string>("a1_cod_mun") : String.Empty;
            vo.Mun = dr["a1_mun"] != DBNull.Value ? dr.Field<string>("a1_mun") : String.Empty;
            vo.Ddd = dr["a1_ddd"] != DBNull.Value ? dr.Field<string>("a1_ddd") : String.Empty;
            vo.Tel = dr["a1_tel"] != DBNull.Value ? dr.Field<string>("a1_tel") : String.Empty;
            vo.Pais = dr["a1_pais"] != DBNull.Value ? dr.Field<string>("a1_pais") : String.Empty;
            vo.Msblql = dr["a1_msblql"] != DBNull.Value ? dr.Field<string>("a1_msblql") : String.Empty;
            vo.Codpais = dr["a1_codpais"] != DBNull.Value ? dr.Field<string>("a1_codpais") : String.Empty;
            vo.Endcob = dr["a1_endcob"] != DBNull.Value ? dr.Field<string>("a1_endcob") : String.Empty;

            return vo;
        }

        private static string NextId(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de cliente
            string sql = "select to_number(max(trim(a1_cod))) + 1 from SA1010";
            int cod = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql));
            return cod.ToString().PadLeft(6, '0');
        }

        private static Int64 NextRecno(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from SA1010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }

        internal static string CriarCliente(PClienteVO cliente, EvidaDatabase db)
        {
            string sql = "insert into SA1010 (a1_cod, a1_loja, a1_pessoa, a1_cgc, a1_nome, a1_nreduz, a1_cep, a1_email, a1_end, a1_nr_end, " +
                " a1_complem, a1_bairro, a1_est, a1_cod_mun, a1_mun, a1_ddd, a1_tel, a1_pais, a1_msblql, a1_codpais, a1_tipo, R_E_C_N_O_) " +
                " values (:cod, :loja, :pessoa, :cgc, :nome, :nreduz, :cep, :email, :end, :nrend, :complem, :bairro, :est, :codmun, :mun, :ddd, :tel, :pais, :msblql, :codpais, :tipo, :recno)";

            Int64 recno = NextRecno(db);
            cliente.Cod = NextId(db);

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(cliente.Cod == null ? new Parametro(":cod", DbType.String, "".PadRight(6, ' ')) : new Parametro(":cod", DbType.String, cliente.Cod.Trim().Length >= 6 ? cliente.Cod.Trim().Substring(0, 6) : cliente.Cod.Trim().PadRight(6, ' ')));
            lstParam.Add(new Parametro(":loja", DbType.String, "01"));
            lstParam.Add(new Parametro(":pessoa", DbType.String, "F"));
            lstParam.Add(cliente.Cgc == null ? new Parametro(":cgc", DbType.String, "".PadRight(14, ' ')) : new Parametro(":cgc", DbType.String, cliente.Cgc.Trim().Length >= 14 ? cliente.Cgc.Trim().Substring(0, 14) : cliente.Cgc.Trim().PadRight(14, ' ')));
            lstParam.Add(cliente.Nome == null ? new Parametro(":nome", DbType.String, "".PadRight(40, ' ')) : new Parametro(":nome", DbType.String, cliente.Nome.Trim().Length >= 40 ? cliente.Nome.Trim().Substring(0, 40) : cliente.Nome.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Nreduz == null ? new Parametro(":nreduz", DbType.String, "".PadRight(20, ' ')) : new Parametro(":nreduz", DbType.String, cliente.Nreduz.Trim().Length >= 20 ? cliente.Nreduz.Trim().Substring(0, 20) : cliente.Nreduz.Trim().PadRight(20, ' ')));
            lstParam.Add(cliente.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, cliente.Cep.Trim().Length >= 8 ? cliente.Cep.Trim().Substring(0, 8) : cliente.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(cliente.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, cliente.Email.Trim().Length >= 100 ? cliente.Email.Trim().Substring(0, 100) : cliente.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(cliente.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, cliente.End.Trim().Length >= 40 ? cliente.End.Trim().Substring(0, 40) : cliente.End.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, cliente.Nrend.Trim().Length >= 6 ? cliente.Nrend.Trim().Substring(0, 6) : cliente.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(cliente.Complem == null ? new Parametro(":complem", DbType.String, "".PadRight(50, ' ')) : new Parametro(":complem", DbType.String, cliente.Complem.Trim().Length >= 50 ? cliente.Complem.Trim().Substring(0, 50) : cliente.Complem.Trim().PadRight(50, ' ')));
            lstParam.Add(cliente.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, cliente.Bairro.Trim().Length >= 40 ? cliente.Bairro.Trim().Substring(0, 40) : cliente.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Est == null ? new Parametro(":est", DbType.String, "".PadRight(2, ' ')) : new Parametro(":est", DbType.String, cliente.Est.Trim().Length >= 2 ? cliente.Est.Trim().Substring(0, 2) : cliente.Est.Trim().PadRight(2, ' ')));
            lstParam.Add(cliente.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(5, ' ')) : new Parametro(":codmun", DbType.String, cliente.Codmun.Trim().Length >= 5 ? cliente.Codmun.Trim().Substring(0, 5) : cliente.Codmun.Trim().PadRight(5, ' ')));
            lstParam.Add(cliente.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(60, ' ')) : new Parametro(":mun", DbType.String, cliente.Mun.Trim().Length >= 60 ? cliente.Mun.Trim().Substring(0, 60) : cliente.Mun.Trim().PadRight(60, ' ')));
            lstParam.Add(cliente.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, cliente.Ddd.Trim().Length >= 3 ? cliente.Ddd.Trim().Substring(0, 3) : cliente.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(cliente.Tel == null ? new Parametro(":tel", DbType.String, "".PadRight(15, ' ')) : new Parametro(":tel", DbType.String, cliente.Tel.Trim().Length >= 15 ? cliente.Tel.Trim().Substring(0, 15) : cliente.Tel.Trim().PadRight(15, ' ')));
            lstParam.Add(new Parametro(":pais", DbType.String, "105"));
            lstParam.Add(new Parametro(":msblql", DbType.String, "2"));
            lstParam.Add(new Parametro(":codpais", DbType.String, "01058"));
            lstParam.Add(new Parametro(":tipo", DbType.String, "F"));
            lstParam.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);

            return cliente.Cod;
        }

        internal static void AlterarCliente(PClienteVO cliente, EvidaDatabase db)
        {
            string sql = "update SA1010 set a1_nome = :nome, a1_nreduz = :nreduz, a1_cep = :cep, " +
                " a1_email = :email, a1_end = :end, a1_nr_end = :nrend, a1_complem = :complem, a1_bairro = :bairro, a1_est = :est, a1_cod_mun = :codmun, " +
                " a1_mun = :mun, a1_ddd = :ddd, a1_tel = :tel " +
                " where upper(trim(a1_cgc)) = upper(trim(:cgc)) ";

            List<Parametro> lstParam = new List<Parametro>();

            lstParam.Add(cliente.Cgc == null ? new Parametro(":cgc", DbType.String, "".PadRight(14, ' ')) : new Parametro(":cgc", DbType.String, cliente.Cgc.Trim().Length >= 14 ? cliente.Cgc.Trim().Substring(0, 14) : cliente.Cgc.Trim().PadRight(14, ' ')));
            lstParam.Add(cliente.Nome == null ? new Parametro(":nome", DbType.String, "".PadRight(40, ' ')) : new Parametro(":nome", DbType.String, cliente.Nome.Trim().Length >= 40 ? cliente.Nome.Trim().Substring(0, 40) : cliente.Nome.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Nreduz == null ? new Parametro(":nreduz", DbType.String, "".PadRight(20, ' ')) : new Parametro(":nreduz", DbType.String, cliente.Nreduz.Trim().Length >= 20 ? cliente.Nreduz.Trim().Substring(0, 20) : cliente.Nreduz.Trim().PadRight(20, ' ')));
            lstParam.Add(cliente.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, cliente.Cep.Trim().Length >= 8 ? cliente.Cep.Trim().Substring(0, 8) : cliente.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(cliente.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, cliente.Email.Trim().Length >= 100 ? cliente.Email.Trim().Substring(0, 100) : cliente.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(cliente.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, cliente.End.Trim().Length >= 40 ? cliente.End.Trim().Substring(0, 40) : cliente.End.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, cliente.Nrend.Trim().Length >= 6 ? cliente.Nrend.Trim().Substring(0, 6) : cliente.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(cliente.Complem == null ? new Parametro(":complem", DbType.String, "".PadRight(50, ' ')) : new Parametro(":complem", DbType.String, cliente.Complem.Trim().Length >= 50 ? cliente.Complem.Trim().Substring(0, 50) : cliente.Complem.Trim().PadRight(50, ' ')));
            lstParam.Add(cliente.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, cliente.Bairro.Trim().Length >= 40 ? cliente.Bairro.Trim().Substring(0, 40) : cliente.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Est == null ? new Parametro(":est", DbType.String, "".PadRight(2, ' ')) : new Parametro(":est", DbType.String, cliente.Est.Trim().Length >= 2 ? cliente.Est.Trim().Substring(0, 2) : cliente.Est.Trim().PadRight(2, ' ')));
            lstParam.Add(cliente.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(5, ' ')) : new Parametro(":codmun", DbType.String, cliente.Codmun.Trim().Length >= 5 ? cliente.Codmun.Trim().Substring(0, 5) : cliente.Codmun.Trim().PadRight(5, ' ')));
            lstParam.Add(cliente.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(60, ' ')) : new Parametro(":mun", DbType.String, cliente.Mun.Trim().Length >= 60 ? cliente.Mun.Trim().Substring(0, 60) : cliente.Mun.Trim().PadRight(60, ' ')));
            lstParam.Add(cliente.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, cliente.Ddd.Trim().Length >= 3 ? cliente.Ddd.Trim().Substring(0, 3) : cliente.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(cliente.Tel == null ? new Parametro(":tel", DbType.String, "".PadRight(15, ' ')) : new Parametro(":tel", DbType.String, cliente.Tel.Trim().Length >= 15 ? cliente.Tel.Trim().Substring(0, 15) : cliente.Tel.Trim().PadRight(15, ' ')));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void SalvarDadosContato(PClienteVO cliente, EvidaDatabase db)
        {
            string sql = "update SA1010 set a1_cep = :cep, a1_email = :email, a1_end = :end, a1_nr_end = :nrend, a1_complem = :complem, " +
                " a1_bairro = :bairro, a1_est = :est, a1_cod_mun = :codmun, a1_mun = :mun, a1_ddd = :ddd, a1_tel = :tel " +
                " where upper(trim(a1_cod)) = upper(trim(:cod)) ";

            List<Parametro> lstParam = new List<Parametro>();

            lstParam.Add(new Parametro(":cod", DbType.String, cliente.Cod.Trim().PadRight(6, '0')));
            lstParam.Add(cliente.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, cliente.Cep.Trim().Length >= 8 ? cliente.Cep.Trim().Substring(0, 8) : cliente.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(cliente.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, cliente.Email.Trim().Length >= 100 ? cliente.Email.Trim().Substring(0, 100) : cliente.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(cliente.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, cliente.End.Trim().Length >= 40 ? cliente.End.Trim().Substring(0, 40) : cliente.End.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, cliente.Nrend.Trim().Length >= 6 ? cliente.Nrend.Trim().Substring(0, 6) : cliente.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(cliente.Complem == null ? new Parametro(":complem", DbType.String, "".PadRight(50, ' ')) : new Parametro(":complem", DbType.String, cliente.Complem.Trim().Length >= 50 ? cliente.Complem.Trim().Substring(0, 50) : cliente.Complem.Trim().PadRight(50, ' ')));
            lstParam.Add(cliente.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, cliente.Bairro.Trim().Length >= 40 ? cliente.Bairro.Trim().Substring(0, 40) : cliente.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(cliente.Est == null ? new Parametro(":est", DbType.String, "".PadRight(2, ' ')) : new Parametro(":est", DbType.String, cliente.Est.Trim().Length >= 2 ? cliente.Est.Trim().Substring(0, 2) : cliente.Est.Trim().PadRight(2, ' ')));
            lstParam.Add(cliente.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(5, ' ')) : new Parametro(":codmun", DbType.String, cliente.Codmun.Trim().Length >= 5 ? cliente.Codmun.Trim().Substring(0, 5) : cliente.Codmun.Trim().PadRight(5, ' ')));
            lstParam.Add(cliente.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(60, ' ')) : new Parametro(":mun", DbType.String, cliente.Mun.Trim().Length >= 60 ? cliente.Mun.Trim().Substring(0, 60) : cliente.Mun.Trim().PadRight(60, ' ')));
            lstParam.Add(cliente.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, cliente.Ddd.Trim().Length >= 3 ? cliente.Ddd.Trim().Substring(0, 3) : cliente.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(cliente.Tel == null ? new Parametro(":tel", DbType.String, "".PadRight(15, ' ')) : new Parametro(":tel", DbType.String, cliente.Tel.Trim().Length >= 15 ? cliente.Tel.Trim().Substring(0, 15) : cliente.Tel.Trim().PadRight(15, ' ')));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static DataTable Pesquisar(string cpf, string nome, string preenchido, EvidaDatabase evdb)
        {
            string sql = "SELECT A1_FILIAL, A1_COD, A1_LOJA, A1_CGC, A1_NOME, A1_CEP, A1_END, A1_NR_END, A1_COMPLEM, A1_BAIRRO, A1_ENDCOB " +
                    "	FROM VW_PR_CLIENTE " +
                    "	WHERE A1_PESSOA = 'F' ";

            List<Parametro> lstParams = new List<Parametro>();

            if (!string.IsNullOrEmpty(cpf))
            {
                sql += " AND upper(trim(A1_CGC)) = upper(trim(:cpf)) ";
                lstParams.Add(new Parametro() { Name = ":cpf", Tipo = DbType.String, Value = cpf });
            }

            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(A1_NOME)) LIKE upper(trim(:nome)) ";
                lstParams.Add(new Parametro(":nome", DbType.String, "%" + nome.Trim() + "%"));
            }

            if (!string.IsNullOrEmpty(preenchido.Trim()))
            {
                if (preenchido.ToUpper() == "S")
                {
                    sql += " AND trim(A1_ENDCOB) IS NOT NULL ";
                }
                else if (preenchido.ToUpper() == "N")
                {
                    sql += " AND trim(A1_ENDCOB) IS NULL ";
                }
            }

            sql += " ORDER BY A1_NOME ASC ";
            DataTable dt = BaseDAO.ExecuteDataSet(evdb, sql, lstParams);

            return dt;
        }

        internal static PClienteVO GetById(string id, EvidaDatabase db)
        {
            string sql = "select * from (select " + FIELDS +
                "	from VW_PR_CLIENTE " +
                "	where upper(trim(a1_cod)) like upper(trim(:id)) " +
                "   order by a1_cod desc) where rownum = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.String, Value = id.Trim() });

            List<PClienteVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static void AlterarEnderecoCobranca(string cod, string endcob, EvidaDatabase db)
        {
            string sql = "update SA1010 set a1_endcob = :endcob " +
                " where upper(trim(a1_cod)) = upper(trim(:cod)) ";

            List<Parametro> lstParam = new List<Parametro>();

            lstParam.Add(new Parametro(":cod", DbType.String, cod.PadLeft(6, '0')));
            lstParam.Add(endcob == null ? new Parametro(":endcob", DbType.String, "".PadRight(60, ' ')) : new Parametro(":endcob", DbType.String, endcob.Trim().Length >= 60 ? endcob.Trim().Substring(0, 60) : endcob.Trim().PadRight(60, ' ')));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }
    }
}
