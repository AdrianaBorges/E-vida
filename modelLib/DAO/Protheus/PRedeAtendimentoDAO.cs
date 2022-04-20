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

namespace eVidaGeneralLib.DAO.Protheus
{
    internal class PRedeAtendimentoDAO
    {
        private static string FIELDS = " BAU_CODIGO, BAU_TIPPE, BAU_CPFCGC, BAU_NOME, BAU_NREDUZ, BAU_NFANTA, BAU_TIPPRE, BAU_CNES, BAU_INSCR, BAU_INSCRM, BAU_LOCAL, BAU_DESLOC, " +
            " BAU_CEP, BAU_TIPLOG, BAU_END, BAU_NUMERO, BAU_COMPL, BAU_BAIRRO, BAU_MUN, BAU_EST, BAU_REGMUN, BAU_DDD, BAU_TEL, BAU_EMAIL, BAU_INSS, BAU_DIAPOL, BAU_DTINCL, BAU_DATBLO, BAU_CODBLO ";

        internal static PRedeAtendimentoVO GetById(string codigo, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_REDE_ATENDIMENTO WHERE trim(BAU_CODIGO) = trim(:codigo) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo.PadLeft(6, '0') });

            List<PRedeAtendimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowRedeAtend, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PRedeAtendimentoVO GetByDoc(string cpfCnpj, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_REDE_ATENDIMENTO v WHERE trim(BAU_CPFCGC) = trim(:cpfCnpj) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":cpfCnpj", Tipo = DbType.String, Value = cpfCnpj });

            List<PRedeAtendimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowRedeAtend, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PRedeAtendimentoVO GetByName(string razaoSocial, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_REDE_ATENDIMENTO v WHERE upper(trim(BAU_NOME)) LIKE upper(trim(:razaoSocial)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":razaoSocial", Tipo = DbType.String, Value = "%" + razaoSocial.ToUpper() + "%" });

            List<PRedeAtendimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowRedeAtend, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PRedeAtendimentoVO FromDataRowRedeAtend(DataRow dr)
        {
            PRedeAtendimentoVO vo = new PRedeAtendimentoVO();

            vo.Codigo = dr["bau_codigo"] != DBNull.Value ? dr.Field<string>("bau_codigo") : String.Empty;
            vo.Tippe = dr["bau_tippe"] != DBNull.Value ? dr.Field<string>("bau_tippe") : String.Empty;
            vo.Cpfcgc = dr["bau_cpfcgc"] != DBNull.Value ? dr.Field<string>("bau_cpfcgc") : String.Empty;
            vo.Nome = dr["bau_nome"] != DBNull.Value ? dr.Field<string>("bau_nome") : String.Empty;
            vo.Nreduz = dr["bau_nreduz"] != DBNull.Value ? dr.Field<string>("bau_nreduz") : String.Empty;
            vo.Nfanta = dr["bau_nfanta"] != DBNull.Value ? dr.Field<string>("bau_nfanta") : String.Empty;
            vo.Tippre = dr["bau_tippre"] != DBNull.Value ? dr.Field<string>("bau_tippre") : String.Empty;
            vo.Cnes = dr["bau_cnes"] != DBNull.Value ? dr.Field<string>("bau_cnes") : String.Empty;
            vo.Inscr = dr["bau_inscr"] != DBNull.Value ? dr.Field<string>("bau_inscr") : String.Empty;
            vo.Inscrm = dr["bau_inscrm"] != DBNull.Value ? dr.Field<string>("bau_inscrm") : String.Empty;
            vo.Local = dr["bau_local"] != DBNull.Value ? dr.Field<string>("bau_local") : String.Empty;
            vo.Desloc = dr["bau_desloc"] != DBNull.Value ? dr.Field<string>("bau_desloc") : String.Empty;
            vo.Cep = dr["bau_cep"] != DBNull.Value ? dr.Field<string>("bau_cep") : String.Empty;
            vo.Tiplog = dr["bau_tiplog"] != DBNull.Value ? dr.Field<string>("bau_tiplog") : String.Empty;
            vo.End = dr["bau_end"] != DBNull.Value ? dr.Field<string>("bau_end") : String.Empty;
            vo.Numero = dr["bau_numero"] != DBNull.Value ? dr.Field<string>("bau_numero") : String.Empty;
            vo.Compl = dr["bau_compl"] != DBNull.Value ? dr.Field<string>("bau_compl") : String.Empty;
            vo.Bairro = dr["bau_bairro"] != DBNull.Value ? dr.Field<string>("bau_bairro") : String.Empty;
            vo.Mun = dr["bau_mun"] != DBNull.Value ? dr.Field<string>("bau_mun") : String.Empty;
            vo.Est = dr["bau_est"] != DBNull.Value ? dr.Field<string>("bau_est") : String.Empty;
            vo.Regmun = dr["bau_regmun"] != DBNull.Value ? dr.Field<string>("bau_regmun") : String.Empty;
            vo.Ddd = dr["bau_ddd"] != DBNull.Value ? dr.Field<string>("bau_ddd") : String.Empty;
            vo.Tel = dr["bau_tel"] != DBNull.Value ? dr.Field<string>("bau_tel") : String.Empty;
            vo.Email = dr["bau_email"] != DBNull.Value ? dr.Field<string>("bau_email") : String.Empty;
            vo.Inss = dr["bau_inss"] != DBNull.Value ? dr.Field<string>("bau_inss") : String.Empty;
            vo.Diapol = dr["bau_diapol"] != DBNull.Value ? dr.Field<string>("bau_diapol") : String.Empty;
            vo.Dtincl = dr["bau_dtincl"] != DBNull.Value ? dr.Field<string>("bau_dtincl") : String.Empty;
            vo.Datblo = dr["bau_datblo"] != DBNull.Value ? dr.Field<string>("bau_datblo") : String.Empty;
            vo.Codblo = dr["bau_codblo"] != DBNull.Value ? dr.Field<string>("bau_codblo") : String.Empty;

            return vo;
        }

        internal static string GetRegiaoRedeAtendimento(string codigo, DateTime? dtVigencia, EvidaDatabase db)
        {
            string sql = "select BAU_REGMUN " +
                " from VW_PR_REDE_ATENDIMENTO where trim(BAU_DTINCL) < to_char(:vigencia, 'YYYYMMDD') and (BAU_DATBLO is null or trim(BAU_DATBLO) > to_char(:vigencia, 'YYYYMMDD')) " +
                "	and trim(BAU_CODIGO) = trim(:codigo) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":vigencia", Tipo = DbType.Date, Value = dtVigencia == null ? DateTime.Now.Date : dtVigencia.Value });
            lstParams.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo });
            object o = BaseDAO.ExecuteScalar(db, sql, lstParams);
            if (o == DBNull.Value)
                return "";
            return Convert.ToString(o);
        }

        internal static DataTable Pesquisar(string razaoSocial, string cpfCnpj, bool hospital, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_REDE_ATENDIMENTO WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(razaoSocial))
            {
                sql += " AND upper(trim(BAU_NOME)) LIKE upper(trim(:razaoSocial)) ";
                lstParams.Add(new Parametro() { Name = ":razaoSocial", Tipo = DbType.String, Value = "%" + razaoSocial.ToUpper() + "%" });
            }

            if (!string.IsNullOrEmpty(cpfCnpj))
            {
                sql += " AND trim(BAU_CPFCGC) = trim(:cpfCnpj) ";
                lstParams.Add(new Parametro() { Name = ":cpfCnpj", Tipo = DbType.String, Value = cpfCnpj });
            }

            if (hospital)
            {
                //sql += " AND tp_sistema_atend IN ('CRED', 'AMB')";
            }
            return BaseDAO.ExecuteDataSet(db, sql, lstParams);
        }

    }
}
