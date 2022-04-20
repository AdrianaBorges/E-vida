using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using static System.String;

namespace eVidaGeneralLib.DAO.Protheus
{
    internal class PFamiliaDAO
    {
        private static string FIELDS = " ba3_codint, ba3_codemp, ba3_matric, ba3_matemp, ba3_yregio, ba3_undorg, ba3_conemp, ba3_vercon, ba3_subcon, ba3_versub, ba3_codpla, ba3_versao, ba3_datbas, ba3_demiti, " +
                " ba3_datdem, ba3_motdem, ba3_matant, ba3_yautpr, ba3_datblo, ba3_motblo, ba3_consid, ba3_end, ba3_numero, ba3_comple, ba3_bairro, " +
                " ba3_codmun, ba3_mun, ba3_estado, ba3_cep, ba3_bcocli, ba3_agecli, ba3_ctacli, ba3_forpag, ba3_diaret, ba3_rotina, ba3_codcli ";

        internal static PFamiliaVO GetByMatricula(PDados.Empresa empresa, string matemp, EvidaDatabase db)
        {
            string strEmpresa = Convert.ToString(Convert.ToInt32(empresa)).PadLeft(4, '0');

            string sql = "select " + FIELDS +
                " from VW_PR_FAMILIA " +
                " where upper(trim(ba3_mateemp)) = upper(trim(:matemp)) " +
                " and upper(trim(ba3_codmp)) = upper(trim(:codemp)) " +
                " and trim(ba3_datbas) in (select max(trim(ba3_datbas)) from VW_PR_FAMILIA where upper(trim(ba3_matemp)) = upper(trim(:matemp)) and upper(trim(ba3_codemp)) = upper(trim(:codemp)) ) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = strEmpresa.Trim() });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static PFamiliaVO GetByMatriculaTitular(PDados.Empresa empresa, string matemp, PPessoaVO titular, EvidaDatabase db)
        {
            string strEmpresa = Convert.ToString(Convert.ToInt32(empresa)).PadLeft(4, '0');

            string sql = "select * from (select " + FIELDS +
                " from VW_PR_USUARIO_FULL " +
                " where upper(trim(ba3_matemp)) = upper(trim(:matemp)) " +
                " and upper(trim(ba3_codemp)) = upper(trim(:codemp)) " +
                " and upper(trim(ba1_cpfusr)) = upper(trim(:cpf)) " +
                " and ((upper(trim(ba1_nomusr)) like upper(trim(:nome)) and upper(trim(ba1_drgusr)) = upper(trim(:rg))) or upper(trim(bts_nrcrna)) = upper(trim(:cns))) " +
                " and ba1_tipusu = 'T' " +
                " order by ba1_datcar desc) where rownum = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = strEmpresa.Trim() });
            lstParams.Add(new Parametro() { Name = ":cpf", Tipo = DbType.String, Value = titular.Cpf.Trim() });
            lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = titular.Nome.Trim() });
            lstParams.Add(new Parametro() { Name = ":rg", Tipo = DbType.String, Value = titular.Rg.Trim() });
            lstParams.Add(new Parametro() { Name = ":cns", Tipo = DbType.String, Value = titular.Cns.Trim() });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static PFamiliaVO GetAtualByMatricula(PDados.Empresa empresa, string matemp, EvidaDatabase db)
        {
            string strEmpresa = Convert.ToString(Convert.ToInt32(empresa)).PadLeft(4, '0');

            string sql = "select " + FIELDS +
                " from VW_PR_FAMILIA_ATUAL " +
                " where upper(trim(ba3_matemp)) = upper(trim(:matemp)) " +
                " and upper(trim(ba3_codemp)) = upper(trim(:codemp)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = strEmpresa.Trim() });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static PFamiliaVO GetAtualByMatriculaTitular(PDados.Empresa empresa, string matemp, PPessoaVO titular, EvidaDatabase db)
        {
            string strEmpresa = Convert.ToString(Convert.ToInt32(empresa)).PadLeft(4, '0');

            string sql = "select " + FIELDS +
                " from VW_PR_USUARIO_FULL " +
                " where upper(trim(ba3_matemp)) = upper(trim(:matemp)) " +
                " and upper(trim(ba3_codemp)) = upper(trim(:codemp)) " +
                " and upper(trim(ba1_cpfusr)) = upper(trim(:cpf)) " +
                " and ((upper(trim(ba1_nomusr)) like upper(trim(:nome)) and upper(trim(ba1_drgusr)) = upper(trim(:rg))) or upper(trim(bts_nrcrna)) = upper(trim(:cns))) " +
                " and ba1_tipusu = 'T' " +
                " and (ba3_datblo = '        ' or ba3_datblo > to_char(sysdate, 'yyyymmdd')) " +
                " and ba1_datcar <= to_char(sysdate, 'yyyymmdd') and (ba1_datblo = '        ' or ba1_datblo > to_char(sysdate, 'yyyymmdd')) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = strEmpresa.Trim() });
            lstParams.Add(new Parametro() { Name = ":cpf", Tipo = DbType.String, Value = titular.Cpf.Trim() });
            lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = titular.Nome.Trim() });
            lstParams.Add(new Parametro() { Name = ":rg", Tipo = DbType.String, Value = titular.Rg.Trim() });
            lstParams.Add(new Parametro() { Name = ":cns", Tipo = DbType.String, Value = titular.Cns.Trim() });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static PFamiliaVO GetByMatricula(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_FAMILIA " +
                " where upper(trim(ba3_codint)) = upper(trim(:codint)) " +
                " and upper(trim(ba3_codemp)) = upper(trim(:codemp)) " +
                " and upper(trim(ba3_matric)) = upper(trim(:matric)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static PFamiliaVO GetByContrato(PDados.Empresa empresa, string matemp, string conemp, string subcon, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_FAMILIA " +
                " where upper(trim(ba3_matemp)) = upper(trim(:matemp)) " +
                " and upper(trim(ba3_codemp)) = upper(trim(:codemp)) " +
                " and upper(trim(ba3_conemp)) = upper(trim(:conemp)) " +
                " and upper(trim(ba3_subcon)) = upper(trim(:subcon)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = Convert.ToString(Convert.ToInt32(empresa)).Trim().PadLeft(4, '0') });
            lstParams.Add(new Parametro() { Name = ":conemp", Tipo = DbType.String, Value = conemp.Trim().PadLeft(12, '0') });
            lstParams.Add(new Parametro() { Name = ":subcon", Tipo = DbType.String, Value = subcon.Trim().PadLeft(9, '0') });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static PFamiliaVO GetByContratoTitular(PDados.Empresa empresa, string matemp, PPessoaVO titular, string conemp, string subcon, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_USUARIO_FULL " +
                " where upper(trim(ba3_matemp)) = upper(trim(:matemp)) " +
                " and upper(trim(ba3_codemp)) = upper(trim(:codemp)) " +
                " and upper(trim(ba3_conemp)) = upper(trim(:conemp)) " +
                " and upper(trim(ba3_subcon)) = upper(trim(:subcon)) " +
                " and upper(trim(ba1_cpfusr)) = upper(trim(:cpf)) " +
                " and ((upper(trim(ba1_nomusr)) like upper(trim(:nome)) and upper(trim(ba1_drgusr)) = upper(trim(:rg))) or upper(trim(bts_nrcrna)) = upper(trim(:cns))) " +
                " and ba1_tipusu = 'T' ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matemp", Tipo = DbType.String, Value = matemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = Convert.ToString(Convert.ToInt32(empresa)).Trim().PadLeft(4, '0') });
            lstParams.Add(new Parametro() { Name = ":conemp", Tipo = DbType.String, Value = conemp.Trim().PadLeft(12, '0') });
            lstParams.Add(new Parametro() { Name = ":subcon", Tipo = DbType.String, Value = subcon.Trim().PadLeft(9, '0') });
            lstParams.Add(new Parametro() { Name = ":cpf", Tipo = DbType.String, Value = titular.Cpf.Trim() });
            lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = titular.Nome.Trim() });
            lstParams.Add(new Parametro() { Name = ":rg", Tipo = DbType.String, Value = titular.Rg.Trim() });
            lstParams.Add(new Parametro() { Name = ":cns", Tipo = DbType.String, Value = titular.Cns.Trim() });

            List<PFamiliaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        private static PFamiliaVO FromDataRow(DataRow dr)
        {
            PFamiliaVO vo = new PFamiliaVO();

            vo.Codint = dr["ba3_codint"] != DBNull.Value ? dr.Field<string>("ba3_codint") : Empty;
            vo.Codemp = dr["ba3_codemp"] != DBNull.Value ? dr.Field<string>("ba3_codemp") : Empty;
            vo.Matric = dr["ba3_matric"] != DBNull.Value ? dr.Field<string>("ba3_matric") : Empty;
            vo.Matemp = dr["ba3_matemp"] != DBNull.Value ? dr.Field<string>("ba3_matemp") : Empty;
            vo.Yregio = dr["ba3_yregio"] != DBNull.Value ? dr.Field<string>("ba3_yregio") : Empty;
            vo.Undorg = dr["ba3_undorg"] != DBNull.Value ? dr.Field<string>("ba3_undorg") : Empty;
            vo.Conemp = dr["ba3_conemp"] != DBNull.Value ? dr.Field<string>("ba3_conemp") : Empty;
            vo.Vercon = dr["ba3_vercon"] != DBNull.Value ? dr.Field<string>("ba3_vercon") : Empty;
            vo.Subcon = dr["ba3_subcon"] != DBNull.Value ? dr.Field<string>("ba3_subcon") : Empty;
            vo.Versub = dr["ba3_versub"] != DBNull.Value ? dr.Field<string>("ba3_versub") : Empty;
            vo.Codpla = dr["ba3_codpla"] != DBNull.Value ? dr.Field<string>("ba3_codpla") : Empty;
            vo.Versao = dr["ba3_versao"] != DBNull.Value ? dr.Field<string>("ba3_versao") : Empty;
            vo.Datbas = dr["ba3_datbas"] != DBNull.Value ? dr.Field<string>("ba3_datbas") : Empty;
            vo.Demiti = dr["ba3_demiti"] != DBNull.Value ? dr.Field<string>("ba3_demiti") : Empty;
            vo.Datdem = dr["ba3_datdem"] != DBNull.Value ? dr.Field<string>("ba3_datdem") : Empty;
            vo.Motdem = dr["ba3_motdem"] != DBNull.Value ? dr.Field<string>("ba3_motdem") : Empty;
            vo.Matant = dr["ba3_matant"] != DBNull.Value ? dr.Field<string>("ba3_matant") : Empty;
            vo.Yautpr = dr["ba3_yautpr"] != DBNull.Value ? dr.Field<string>("ba3_yautpr") : Empty;
            vo.Datblo = dr["ba3_datblo"] != DBNull.Value ? dr.Field<string>("ba3_datblo") : Empty;
            vo.Motblo = dr["ba3_motblo"] != DBNull.Value ? dr.Field<string>("ba3_motblo") : Empty;
            vo.Consid = dr["ba3_consid"] != DBNull.Value ? dr.Field<string>("ba3_consid") : Empty;
            vo.End = dr["ba3_end"] != DBNull.Value ? dr.Field<string>("ba3_end") : Empty;
            vo.Numero = dr["ba3_numero"] != DBNull.Value ? dr.Field<string>("ba3_numero") : Empty;
            vo.Comple = dr["ba3_comple"] != DBNull.Value ? dr.Field<string>("ba3_comple") : Empty;
            vo.Bairro = dr["ba3_bairro"] != DBNull.Value ? dr.Field<string>("ba3_bairro") : Empty;
            vo.Codmun = dr["ba3_codmun"] != DBNull.Value ? dr.Field<string>("ba3_codmun") : Empty;
            vo.Mun = dr["ba3_mun"] != DBNull.Value ? dr.Field<string>("ba3_mun") : Empty;
            vo.Estado = dr["ba3_estado"] != DBNull.Value ? dr.Field<string>("ba3_estado") : Empty;
            vo.Cep = dr["ba3_cep"] != DBNull.Value ? dr.Field<string>("ba3_cep") : Empty;
            vo.Bcocli = dr["ba3_bcocli"] != DBNull.Value ? dr.Field<string>("ba3_bcocli") : Empty;
            vo.Agecli = dr["ba3_agecli"] != DBNull.Value ? dr.Field<string>("ba3_agecli") : Empty;
            vo.Ctacli = dr["ba3_ctacli"] != DBNull.Value ? dr.Field<string>("ba3_ctacli") : Empty;
            vo.Forpag = dr["ba3_forpag"] != DBNull.Value ? dr.Field<string>("ba3_forpag") : Empty;
            vo.Diaret = dr["ba3_diaret"] != DBNull.Value ? Convert.ToInt32(dr["ba3_diaret"]) : 0;
            vo.Rotina = dr["ba3_rotina"] != DBNull.Value ? dr.Field<string>("ba3_rotina") : Empty;
            vo.Codcli = dr["ba3_codcli"] != DBNull.Value ? dr.Field<string>("ba3_codcli") : Empty;
            
            return vo;
        }

        private static string NextId(string codemp, EvidaDatabase evdb)
        {
            // Pega o próximo número da sequência do empregado
            string sql = "select NVL(to_number(max(trim(ba3_matric))), 0) + 1 from BA3010 where upper(trim(ba3_codemp)) = upper(trim(:codemp)) ";

            Database db = evdb.Database;
            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim().PadLeft(4, '0') });

            int proximo = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
            string matric = proximo.ToString().PadLeft(6, '0');

            return matric;
        }

        private static Int64 NextRecno(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from BA3010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }

        private static Int64 NextRecnoBC3(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from BC3010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }	

        internal static string CriarFamilia(PFamiliaVO familia, EvidaDatabase db)
        {
            string sql = "insert into BA3010 (" +
                         "ba3_codint, ba3_codemp, ba3_matric, ba3_matemp, ba3_matant, ba3_yregio, ba3_datbas, ba3_codpla, ba3_versao, ba3_conemp, ba3_vercon, ba3_subcon, ba3_versub, ba3_undorg, ba3_end,  ba3_numero, ba3_comple, ba3_bairro, ba3_mun, ba3_estado, ba3_cep, ba3_agecli, ba3_bcocli, ba3_ctacli, ba3_forpag, ba3_diaret, ba3_rotina, ba3_tippag, ba3_portad, ba3_agedep, ba3_ctacor, ba3_codcli, ba3_cobniv, ba3_numcon, ba3_tipous, ba3_loja, R_E_C_N_O_) " +
               " values (    :codint,    :codemp,    :matric,    :matemp,    :matant,    :yregio,    :datbas,    :codpla,    :versao,    :conemp,    :vercon,    :subcon,    :versub,    :undorg,    :end,     :numero,    :comple,    :bairro,    :mun,    :estado,    :cep,       :agecli, :bcocli,    :ctacli,    :forpag,    :diaret,    :rotina,    :tippag,    :portad,    :agedep,    :ctacor,    :codcli,    :cobniv,    :numcon,    :tipous,    :loja,     :recno) ";

            Int64 recno = NextRecno(db);
            familia.Matric = NextId(familia.Codemp, db);

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":codint", DbType.String, "0001"));
            lstParam.Add(familia.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, familia.Codemp.Trim().Length >= 4 ? familia.Codemp.Trim().Substring(0, 4) : familia.Codemp.Trim().PadRight(4, ' ')));
            lstParam.Add(familia.Matric == null ? new Parametro(":matric", DbType.String, "".PadRight(6, ' ')) : new Parametro(":matric", DbType.String, familia.Matric.Trim().Length >= 6 ? familia.Matric.Trim().Substring(0, 6) : familia.Matric.Trim().PadRight(6, ' ')));
            lstParam.Add(familia.Matemp == null ? new Parametro(":matemp", DbType.String, "".PadRight(30, ' ')) : new Parametro(":matemp", DbType.String, familia.Matemp.Trim().Length >= 30 ? familia.Matemp.Trim().Substring(0, 30) : familia.Matemp.Trim().PadRight(30, ' ')));
            lstParam.Add(familia.Matant == null ? new Parametro(":matant", DbType.String, "".PadRight(20, ' ')) : new Parametro(":matant", DbType.String, familia.Matant.Trim().Length >= 20 ? familia.Matant.Trim().Substring(0, 20) : familia.Matant.Trim().PadRight(20, ' ')));
            //lstParam.Add(familia.Yregio == null ? new Parametro(":yregio", DbType.String, "".PadRight(3, ' ')) : new Parametro(":yregio", DbType.String, familia.Yregio.Trim().Length >= 3 ? familia.Yregio.Trim().Substring(0, 3) : familia.Yregio.Trim().PadRight(3, ' ')));
            lstParam.Add(new Parametro(":yregio", DbType.String, "011"));
            lstParam.Add(familia.Datbas == null ? new Parametro(":datbas", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datbas", DbType.String, familia.Datbas.Trim().Length >= 8 ? familia.Datbas.Trim().Substring(0, 8) : familia.Datbas.Trim().PadRight(8, ' ')));
            lstParam.Add(familia.Codpla == null ? new Parametro(":codpla", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codpla", DbType.String, familia.Codpla.Trim().Length >= 4 ? familia.Codpla.Trim().Substring(0, 4) : familia.Codpla.Trim().PadRight(4, ' ')));
            lstParam.Add(familia.Versao == Empty ? new Parametro(":versao", DbType.String, "".PadRight(3, ' ')) : new Parametro(":versao", DbType.String, familia.Versao.Trim().Length >= 3 ? familia.Versao.Trim().Substring(0, 3) : familia.Versao.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Conemp == null ? new Parametro(":conemp", DbType.String, "".PadRight(12, ' ')) : new Parametro(":conemp", DbType.String, familia.Conemp.Trim().Length >= 12 ? familia.Conemp.Trim().Substring(0, 12) : familia.Conemp.Trim().PadRight(12, ' ')));
            lstParam.Add(familia.Vercon == null ? new Parametro(":vercon", DbType.String, "".PadRight(3, ' ')) : new Parametro(":vercon", DbType.String, familia.Vercon.Trim().Length >= 3 ? familia.Vercon.Trim().Substring(0, 3) : familia.Vercon.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Subcon == null ? new Parametro(":subcon", DbType.String, "".PadRight(9, ' ')) : new Parametro(":subcon", DbType.String, familia.Subcon.Trim().Length >= 9 ? familia.Subcon.Trim().Substring(0, 9) : familia.Subcon.Trim().PadRight(9, ' ')));
            lstParam.Add(familia.Versub == null ? new Parametro(":versub", DbType.String, "".PadRight(3, ' ')) : new Parametro(":versub", DbType.String, familia.Versub.Trim().Length >= 3 ? familia.Versub.Trim().Substring(0, 3) : familia.Versub.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Undorg == null ? new Parametro(":undorg", DbType.String, "".PadRight(8, ' ')) : new Parametro(":undorg", DbType.String, familia.Undorg.Trim().Length >= 8 ? familia.Undorg.Trim().Substring(0, 8) : familia.Undorg.Trim().PadRight(8, ' ')));
            lstParam.Add(familia.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, familia.End.Trim().Length >= 40 ? familia.End.Trim().Substring(0, 40) : familia.End.Trim().PadRight(40, ' ')));
            lstParam.Add(familia.Numero == null ? new Parametro(":numero", DbType.String, "".PadRight(6, ' ')) : new Parametro(":numero", DbType.String, familia.Numero.Trim().Length >= 6 ? familia.Numero.Trim().Substring(0, 6) : familia.Numero.Trim().PadRight(6, ' ')));
            lstParam.Add(familia.Comple == null ? new Parametro(":comple", DbType.String, "".PadRight(20, ' ')) : new Parametro(":comple", DbType.String, familia.Comple.Trim().Length >= 20 ? familia.Comple.Trim().Substring(0, 20) : familia.Comple.Trim().PadRight(20, ' ')));
            lstParam.Add(familia.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, familia.Bairro.Trim().Length >= 40 ? familia.Bairro.Trim().Substring(0, 40) : familia.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(familia.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(30, ' ')) : new Parametro(":mun", DbType.String, familia.Mun.Trim().Length >= 30 ? familia.Mun.Trim().Substring(0, 30) : familia.Mun.Trim().PadRight(30, ' ')));
            lstParam.Add(familia.Estado == null ? new Parametro(":estado", DbType.String, "".PadRight(2, ' ')) : new Parametro(":estado", DbType.String, familia.Estado.Trim().Length >= 2 ? familia.Estado.Trim().Substring(0, 2) : familia.Estado.Trim().PadRight(2, ' ')));
            lstParam.Add(familia.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, familia.Cep.Trim().Length >= 8 ? familia.Cep.Trim().Substring(0, 8) : familia.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(familia.Agecli == null ? new Parametro(":agecli", DbType.String, "".PadRight(5, ' ')) : new Parametro(":agecli", DbType.String, familia.Agecli.Trim().Length >= 5 ? familia.Agecli.Trim().Substring(0, 5) : familia.Agecli.Trim().PadRight(5, ' ')));
            lstParam.Add(familia.Bcocli == null ? new Parametro(":bcocli", DbType.String, "".PadRight(3, ' ')) : new Parametro(":bcocli", DbType.String, familia.Bcocli.Trim().Length >= 3 ? familia.Bcocli.Trim().Substring(0, 3) : familia.Bcocli.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Ctacli == null ? new Parametro(":ctacli", DbType.String, "".PadRight(10, ' ')) : new Parametro(":ctacli", DbType.String, familia.Ctacli.Trim().Length >= 10 ? familia.Ctacli.Trim().Substring(0, 10) : familia.Ctacli.Trim().PadRight(10, ' ')));
            lstParam.Add(familia.Forpag == Empty ? new Parametro(":forpag", DbType.String, "".PadRight(3, ' ')) : new Parametro(":forpag", DbType.String, familia.Forpag.Trim().Length >= 3 ? familia.Forpag.Trim().Substring(0, 3) : familia.Forpag.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Diaret == Int32.MinValue ? new Parametro(":diaret", DbType.Int32, 0) : new Parametro(":diaret", DbType.Int32, familia.Diaret));
            lstParam.Add(familia.Rotina == Empty ? new Parametro(":rotina", DbType.String, "".PadRight(20, ' ')) : new Parametro(":rotina", DbType.String, familia.Rotina.Trim().Length >= 20 ? familia.Rotina.Trim().Substring(0, 20) : familia.Rotina.Trim().PadRight(20, ' ')));
            lstParam.Add(familia.Tippag == Empty ? new Parametro(":tippag", DbType.String, "".PadRight(2, ' ')) : new Parametro(":tippag", DbType.String, familia.Tippag.Trim().Length >= 2 ? familia.Tippag.Trim().Substring(0, 2) : familia.Tippag.Trim().PadRight(2, ' ')));

            
            // Atribuindo conta bancária para o produto E-VIDA-MASTER
            if (familia.Codpla.Equals("0014"))
            {
                lstParam.Add(new Parametro(":portad", DbType.String, "001"));
                lstParam.Add(new Parametro(":agedep", DbType.String, "0452"));
                lstParam.Add(new Parametro(":ctacor", DbType.String, "129500"));

            }
            else
            {
                lstParam.Add(item: familia.Portad == Empty
                    ? new Parametro(":portad", DbType.String, "".PadRight(3, ' '))
                    : new Parametro(":portad", DbType.String, 
                        familia.Portad.Trim().Length >= 3
                            ? familia.Portad.Trim().Substring(0,3)
                            : familia.Portad.Trim().PadRight(3,' ')));

                lstParam.Add(familia.Agedep == Empty
                    ? new Parametro(":agedep", DbType.String, "".PadRight(5, ' '))
                    : new Parametro(":agedep", DbType.String,
                        familia.Agedep.Trim().Length >= 5
                            ? familia.Agedep.Trim().Substring(0, 5)
                            : familia.Agedep.Trim().PadRight(5, ' ')));

                lstParam.Add(familia.Ctacor == Empty
                    ? new Parametro(":ctacor", DbType.String, "".PadRight(10, ' '))
                    : new Parametro(":ctacor", DbType.String,
                        familia.Ctacor.Trim().Length >= 10
                            ? familia.Ctacor.Trim().Substring(0, 10)
                            : familia.Ctacor.Trim().PadRight(10, ' ')));

            }


            lstParam.Add(familia.Codcli == Empty
                ? new Parametro(":codcli", DbType.String, "".PadRight(6, ' '))
                : new Parametro(":codcli", DbType.String,
                    familia.Codcli.Trim().Length >= 6
                        ? familia.Codcli.Trim().Substring(0, 6)
                        : familia.Codcli.Trim().PadRight(6, ' ')));

            lstParam.Add(new Parametro(":cobniv", DbType.String, "1"));

            lstParam.Add(familia.Numcon == null
                ? new Parametro(":numcon", DbType.String, "".PadRight(5, ' '))
                : new Parametro(":numcon", DbType.String,
                    familia.Numcon.Trim().Length >= 5
                        ? familia.Numcon.Trim().Substring(0, 5)
                        : familia.Numcon.Trim().PadRight(5, ' ')));

            lstParam.Add(new Parametro(":tipous", DbType.String, "2"));
            lstParam.Add(new Parametro(":loja", DbType.String, "01"));

            lstParam.Add(new Parametro(":recno", DbType.Int64, recno));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
            
            return familia.Matric;
        }

        internal static void AlterarFamilia(PFamiliaVO familia, EvidaDatabase db)
        {
            string sql = "update BA3010 set ba3_matemp = :matemp, ba3_codpla = :codpla, ba3_conemp = :conemp, ba3_vercon = :vercon, ba3_subcon = :subcon, ba3_versub = :versub, ba3_undorg = :undorg, ba3_end = :end, ba3_numero = :numero,  " +
                " ba3_comple = :comple, ba3_bairro = :bairro, ba3_mun = :mun, ba3_estado = :estado, ba3_cep = :cep, ba3_agecli = :agecli, ba3_bcocli = :bcocli, ba3_ctacli = :ctacli, " +
                "ba3_numcon = :numcon, " +
                "ba3_motblo = '   ', ba3_datblo = '        ', ba3_consid = ' ' " +
                " where upper(trim(ba3_codint)) = upper(trim(:codint)) and upper(trim(ba3_codemp)) = upper(trim(:codemp)) and upper(trim(ba3_matric)) = upper(trim(:matric)) ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(familia.Codint == null ? new Parametro(":codint", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codint", DbType.String, familia.Codint.Trim()));
            lstParam.Add(familia.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, familia.Codemp.Trim()));
            lstParam.Add(familia.Matric == null ? new Parametro(":matric", DbType.String, "".PadRight(6, ' ')) : new Parametro(":matric", DbType.String, familia.Matric.Trim()));
            lstParam.Add(familia.Matemp == null ? new Parametro(":matemp", DbType.String, "".PadRight(30, ' ')) : new Parametro(":matemp", DbType.String, familia.Matemp.Trim().Length >= 30 ? familia.Matemp.Trim().Substring(0, 30) : familia.Matemp.Trim().PadRight(30, ' ')));
            lstParam.Add(familia.Codpla == null ? new Parametro(":codpla", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codpla", DbType.String, familia.Codpla.Trim().Length >= 4 ? familia.Codpla.Trim().Substring(0, 4) : familia.Codpla.Trim().PadRight(4, ' ')));
            lstParam.Add(familia.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, familia.Codemp.Trim().Length >= 4 ? familia.Codemp.Trim().Substring(0, 4) : familia.Codemp.Trim().PadRight(4, ' ')));
            lstParam.Add(familia.Conemp == null ? new Parametro(":conemp", DbType.String, "".PadRight(12, ' ')) : new Parametro(":conemp", DbType.String, familia.Conemp.Trim().Length >= 12 ? familia.Conemp.Trim().Substring(0, 12) : familia.Conemp.Trim().PadRight(12, ' ')));
            lstParam.Add(familia.Vercon == null ? new Parametro(":vercon", DbType.String, "".PadRight(3, ' ')) : new Parametro(":vercon", DbType.String, familia.Vercon.Trim().Length >= 3 ? familia.Vercon.Trim().Substring(0, 3) : familia.Vercon.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Subcon == null ? new Parametro(":subcon", DbType.String, "".PadRight(9, ' ')) : new Parametro(":subcon", DbType.String, familia.Subcon.Trim().Length >= 9 ? familia.Subcon.Trim().Substring(0, 9) : familia.Subcon.Trim().PadRight(9, ' ')));
            lstParam.Add(familia.Versub == null ? new Parametro(":versub", DbType.String, "".PadRight(3, ' ')) : new Parametro(":versub", DbType.String, familia.Versub.Trim().Length >= 3 ? familia.Versub.Trim().Substring(0, 3) : familia.Versub.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Undorg == null ? new Parametro(":undorg", DbType.String, "".PadRight(8, ' ')) : new Parametro(":undorg", DbType.String, familia.Undorg.Trim().Length >= 8 ? familia.Undorg.Trim().Substring(0, 8) : familia.Undorg.Trim().PadRight(8, ' ')));
            lstParam.Add(familia.End == null ? new Parametro(":end", DbType.String, "".PadRight(40, ' ')) : new Parametro(":end", DbType.String, familia.End.Trim().Length >= 40 ? familia.End.Trim().Substring(0, 40) : familia.End.Trim().PadRight(40, ' ')));
            lstParam.Add(familia.Numero == null ? new Parametro(":numero", DbType.String, "".PadRight(6, ' ')) : new Parametro(":numero", DbType.String, familia.Numero.Trim().Length >= 6 ? familia.Numero.Trim().Substring(0, 6) : familia.Numero.Trim().PadRight(6, ' ')));
            lstParam.Add(familia.Comple == null ? new Parametro(":comple", DbType.String, "".PadRight(20, ' ')) : new Parametro(":comple", DbType.String, familia.Comple.Trim().Length >= 20 ? familia.Comple.Trim().Substring(0, 20) : familia.Comple.Trim().PadRight(20, ' ')));
            lstParam.Add(familia.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, familia.Bairro.Trim().Length >= 40 ? familia.Bairro.Trim().Substring(0, 40) : familia.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(familia.Mun == null ? new Parametro(":mun", DbType.String, "".PadRight(30, ' ')) : new Parametro(":mun", DbType.String, familia.Mun.Trim().Length >= 30 ? familia.Mun.Trim().Substring(0, 30) : familia.Mun.Trim().PadRight(30, ' ')));
            lstParam.Add(familia.Estado == null ? new Parametro(":estado", DbType.String, "".PadRight(2, ' ')) : new Parametro(":estado", DbType.String, familia.Estado.Trim().Length >= 2 ? familia.Estado.Trim().Substring(0, 2) : familia.Estado.Trim().PadRight(2, ' ')));
            lstParam.Add(familia.Cep == null ? new Parametro(":cep", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cep", DbType.String, familia.Cep.Trim().Length >= 8 ? familia.Cep.Trim().Substring(0, 8) : familia.Cep.Trim().PadRight(8, ' ')));
            lstParam.Add(familia.Agecli == null ? new Parametro(":agecli", DbType.String, "".PadRight(5, ' ')) : new Parametro(":agecli", DbType.String, familia.Agecli.Trim().Length >= 5 ? familia.Agecli.Trim().Substring(0, 5) : familia.Agecli.Trim().PadRight(5, ' ')));
            lstParam.Add(familia.Bcocli == null ? new Parametro(":bcocli", DbType.String, "".PadRight(3, ' ')) : new Parametro(":bcocli", DbType.String, familia.Bcocli.Trim().Length >= 3 ? familia.Bcocli.Trim().Substring(0, 3) : familia.Bcocli.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Ctacli == null ? new Parametro(":ctacli", DbType.String, "".PadRight(10, ' ')) : new Parametro(":ctacli", DbType.String, familia.Ctacli.Trim().Length >= 10 ? familia.Ctacli.Trim().Substring(0, 10) : familia.Ctacli.Trim().PadRight(10, ' ')));

            lstParam.Add(familia.Numcon == null
                ? new Parametro(":numcon", DbType.String, "".PadRight(5, ' '))
                : new Parametro(":numcon", DbType.String,
                    familia.Numcon.Trim().Length >= 5
                        ? familia.Numcon.Trim().Substring(0, 5)
                        : familia.Numcon.Trim().PadRight(5, ' ')));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void BloquearFamilia(PFamiliaVO familia, string cdMotivoDesligFamilia, string descMotivoDesligFamilia, EvidaDatabase evdb)
        {
            string sql = "update BA3010 set ba3_datblo = :hoje, ba3_motblo = :motblo, ba3_consid = 'F' " +
                "	where upper(trim(ba3_codint)) = upper(trim(:codint)) and upper(trim(ba3_codemp)) = upper(trim(:codemp)) and upper(trim(ba3_matric)) = upper(trim(:matric)) ";

            string hoje = DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string agora = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":hoje", DbType.String, hoje));
            lstParams.Add(cdMotivoDesligFamilia == null ? new Parametro(":motblo", DbType.String, "".PadRight(3, ' ')) : new Parametro(":motblo", DbType.String, cdMotivoDesligFamilia.Trim().Length >= 3 ? cdMotivoDesligFamilia.Trim().Substring(0, 3) : cdMotivoDesligFamilia.Trim().PadRight(3, '0')));
            lstParams.Add(new Parametro(":codint", DbType.String, familia.Codint.Trim()));
            lstParams.Add(new Parametro(":codemp", DbType.String, familia.Codemp.Trim()));
            lstParams.Add(new Parametro(":matric", DbType.String, familia.Matric.Trim()));

            BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);

            sql = " insert into BC3010 (bc3_matric, bc3_tipo, bc3_data, bc3_motblo, bc3_obs, bc3_usuope, bc3_nivblq, bc3_blofat, bc3_datped, bc3_datlan, bc3_horlan, R_E_C_N_O_) " +
                  " values (:matric, :tipo, :data, :motblo, :obs, :usuope, :nivblq, :blofat, :datped, :datlan, :horlan, :recno) ";

            Int64 recno = NextRecnoBC3(evdb);

            List<Parametro> lstParams2 = new List<Parametro>();
            lstParams2.Add(new Parametro(":matric", DbType.String, familia.Codint.Trim() + familia.Codemp.Trim() + familia.Matric.Trim()));
            lstParams2.Add(new Parametro(":tipo", DbType.String, "0"));
            lstParams2.Add(new Parametro(":data", DbType.String, hoje));
            lstParams2.Add(cdMotivoDesligFamilia == null ? new Parametro(":motblo", DbType.String, "".PadRight(3, ' ')) : new Parametro(":motblo", DbType.String, cdMotivoDesligFamilia.Trim().Length >= 3 ? cdMotivoDesligFamilia.Trim().Substring(0, 3) : cdMotivoDesligFamilia.Trim().PadRight(3, '0')));
            lstParams2.Add(new Parametro(":obs", DbType.String, descMotivoDesligFamilia.Trim()));
            lstParams2.Add(new Parametro(":usuope", DbType.String, "Administrador"));
            lstParams2.Add(new Parametro(":nivblq", DbType.String, "S"));
            lstParams2.Add(new Parametro(":blofat", DbType.String, "1"));
            lstParams2.Add(new Parametro(":datped", DbType.String, hoje));
            lstParams2.Add(new Parametro(":datlan", DbType.String, hoje));
            lstParams2.Add(new Parametro(":horlan", DbType.String, agora));
            lstParams2.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParams2, evdb);

        }

        internal static void DesbloquearFamilia(PFamiliaVO familia, string cdMotivoDesligFamilia, string descMotivoDesligFamilia, DateTime inicioVigencia, EvidaDatabase evdb)
        {
            string sql = " insert into BC3010 (bc3_matric, bc3_tipo, bc3_data, bc3_motblo, bc3_obs, bc3_usuope, bc3_nivblq, bc3_blofat, bc3_datlan, bc3_horlan, R_E_C_N_O_) " +
                  " values (:matric, :tipo, :data, :motblo, :obs, :usuope, :nivblq, :blofat, :datlan, :horlan, :recno) ";

            string hoje = DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string agora = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            Int64 recno = NextRecnoBC3(evdb);

            List<Parametro> lstParams2 = new List<Parametro>();
            lstParams2.Add(new Parametro(":matric", DbType.String, familia.Codint.Trim() + familia.Codemp.Trim() + familia.Matric.Trim()));
            lstParams2.Add(new Parametro(":tipo", DbType.String, "1"));
            lstParams2.Add(new Parametro(":data", DbType.String, inicioVigencia.ToString("yyyyMMdd", CultureInfo.InvariantCulture)));
            lstParams2.Add(cdMotivoDesligFamilia == null ? new Parametro(":motblo", DbType.String, "".PadRight(3, ' ')) : new Parametro(":motblo", DbType.String, cdMotivoDesligFamilia.Trim().Length >= 3 ? cdMotivoDesligFamilia.Trim().Substring(0, 3) : cdMotivoDesligFamilia.Trim().PadRight(3, '0')));
            lstParams2.Add(new Parametro(":obs", DbType.String, descMotivoDesligFamilia.Trim()));
            lstParams2.Add(new Parametro(":usuope", DbType.String, "Administrador"));
            lstParams2.Add(new Parametro(":nivblq", DbType.String, "S"));
            lstParams2.Add(new Parametro(":blofat", DbType.String, "1"));
            lstParams2.Add(new Parametro(":datlan", DbType.String, hoje));
            lstParams2.Add(new Parametro(":horlan", DbType.String, agora));
            lstParams2.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParams2, evdb);

        }

        internal static void AtualizarDadosBancarios(PFamiliaVO familia, EvidaDatabase db)
        {
            string sql = "update BA3010 set ba3_agecli = :agecli, ba3_bcocli = :bcocli, ba3_ctacli = :ctacli " +
                " where upper(trim(ba3_codint)) = upper(trim(:codint)) and upper(trim(ba3_codemp)) = upper(trim(:codemp)) and upper(trim(ba3_matric)) = upper(trim(:matric)) ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(familia.Codint == null ? new Parametro(":codint", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codint", DbType.String, familia.Codint.Trim()));
            lstParam.Add(familia.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, familia.Codemp.Trim()));
            lstParam.Add(familia.Matric == null ? new Parametro(":matric", DbType.String, "".PadRight(6, ' ')) : new Parametro(":matric", DbType.String, familia.Matric.Trim()));
            lstParam.Add(familia.Agecli == null ? new Parametro(":agecli", DbType.String, "".PadRight(5, ' ')) : new Parametro(":agecli", DbType.String, familia.Agecli.Trim().Length >= 5 ? familia.Agecli.Trim().Substring(0, 5) : familia.Agecli.Trim().PadRight(5, ' ')));
            lstParam.Add(familia.Bcocli == null ? new Parametro(":bcocli", DbType.String, "".PadRight(3, ' ')) : new Parametro(":bcocli", DbType.String, familia.Bcocli.Trim().Length >= 3 ? familia.Bcocli.Trim().Substring(0, 3) : familia.Bcocli.Trim().PadRight(3, ' ')));
            lstParam.Add(familia.Ctacli == null ? new Parametro(":ctacli", DbType.String, "".PadRight(10, ' ')) : new Parametro(":ctacli", DbType.String, familia.Ctacli.Trim().Length >= 10 ? familia.Ctacli.Trim().Substring(0, 10) : familia.Ctacli.Trim().PadRight(10, ' ')));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void SalvarDadosContato(PFamiliaVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE BA3010 SET BA3_END = :endereco, BA3_NUMERO = :numero, " +
                "	BA3_COMPLE = :complemento, BA3_BAIRRO = :bairro, BA3_CEP =:cep, BA3_CODMUN = :cidade, BA3_MUN = :municipio, BA3_ESTADO = :estado " +
                "	WHERE upper(trim(BA3_CODINT)) = upper(trim(:codint)) AND upper(trim(BA3_CODEMP)) = upper(trim(:codemp)) AND upper(trim(BA3_MATRIC)) = upper(trim(:matric)) ";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":endereco", DbType.String, vo.End.Trim().Length >= 40 ? vo.End.Trim().Substring(0, 40).ToUpper() : vo.End.Trim().PadRight(40, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Numero.Trim().Length >= 6 ? vo.Numero.Trim().Substring(0, 6).ToUpper() : vo.Numero.Trim().PadRight(6, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Comple.Trim().Length >= 20 ? vo.Comple.Trim().Substring(0, 20).ToUpper() : vo.Comple.Trim().PadRight(20, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Bairro.Trim().Length >= 40 ? vo.Bairro.Trim().Substring(0, 40).ToUpper() : vo.Bairro.Trim().PadRight(40, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":cep", DbType.String, vo.Cep.Trim().Length >= 8 ? vo.Cep.Trim().Substring(0, 8).ToUpper() : vo.Cep.Trim().PadRight(8, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Codmun.Trim().Length >= 7 ? vo.Codmun.Trim().Substring(0, 7).ToUpper() : vo.Codmun.Trim().PadRight(7, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":municipio", DbType.String, vo.Mun.Trim().Length >= 30 ? vo.Mun.Trim().Substring(0, 30).ToUpper() : vo.Mun.Trim().PadRight(30, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":estado", DbType.String, vo.Estado.Trim().Length >= 2 ? vo.Estado.Trim().Substring(0, 2).ToUpper() : vo.Estado.Trim().PadRight(2, ' ').ToUpper());

            db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

    }
}
