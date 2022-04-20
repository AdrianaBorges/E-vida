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
    internal class PVidaDAO
    {
        private static string FIELDS = " bts_matvid, bts_nomusr, bts_nomcar, bts_estciv, bts_sexo, bts_datnas, bts_datobi, bts_mae, bts_pai, bts_deffis, bts_tippes, bts_cpfusr, " +
                " bts_drgusr, bts_orgem, bts_rgest, bts_pispas, bts_nrcrna, bts_endere, bts_nr_end, bts_comend, bts_bairro, bts_codmun, bts_munici, bts_estado, " +
                " bts_cepusr, bts_ddd, bts_telefo, bts_telres, bts_telcom, bts_email ";

        internal static List<PVidaVO> ListarVidas(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                "	from VW_PR_VIDA " +
                "	where upper(trim(bts_matvid)) in (select distinct upper(trim(ba1_matvid)) from VW_PR_USUARIO where upper(trim(ba1_codint)) = upper(trim(:codint)) and upper(trim(ba1_codemp)) = upper(trim(:codemp)) and upper(trim(ba1_matric)) = upper(trim(:matric))) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });

            List<PVidaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            return lst;
        }

        internal static PVidaVO GetRowById(string matvid, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_VIDA WHERE upper(trim(BTS_MATVID)) = upper(trim(:matvid)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matvid", Tipo = DbType.String, Value = matvid.Trim() });

            List<PVidaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        private static PVidaVO FromDataRow(DataRow dr)
        {
            PVidaVO vo = new PVidaVO();

            vo.Matvid = dr["bts_matvid"] != DBNull.Value ? dr.Field<string>("bts_matvid") : String.Empty;
            vo.Nomusr = dr["bts_nomusr"] != DBNull.Value ? dr.Field<string>("bts_nomusr") : String.Empty;
            vo.Nomcar = dr["bts_nomcar"] != DBNull.Value ? dr.Field<string>("bts_nomcar") : String.Empty;
            vo.Estciv = dr["bts_estciv"] != DBNull.Value ? dr.Field<string>("bts_estciv") : String.Empty;
            vo.Sexo = dr["bts_sexo"] != DBNull.Value ? dr.Field<string>("bts_sexo") : String.Empty;
            vo.Datnas = dr["bts_datnas"] != DBNull.Value ? dr.Field<string>("bts_datnas") : String.Empty;
            vo.Datobi = dr["bts_datobi"] != DBNull.Value ? dr.Field<string>("bts_datobi") : String.Empty;
            vo.Mae = dr["bts_mae"] != DBNull.Value ? dr.Field<string>("bts_mae") : String.Empty;
            vo.Pai = dr["bts_pai"] != DBNull.Value ? dr.Field<string>("bts_pai") : String.Empty;
            vo.Deffis = dr["bts_deffis"] != DBNull.Value ? dr.Field<string>("bts_deffis"): String.Empty;
            vo.Tippes = dr["bts_tippes"] != DBNull.Value ? dr.Field<string>("bts_tippes") : String.Empty;
            vo.Cpfusr = dr["bts_cpfusr"] != DBNull.Value ? dr.Field<string>("bts_cpfusr") : String.Empty;
            vo.Drgusr = dr["bts_drgusr"] != DBNull.Value ? dr.Field<string>("bts_drgusr") : String.Empty;
            vo.Orgem = dr["bts_orgem"] != DBNull.Value ? dr.Field<string>("bts_orgem") : String.Empty;
            vo.Rgest = dr["bts_rgest"] != DBNull.Value ? dr.Field<string>("bts_rgest") : String.Empty;
            vo.Pispas = dr["bts_pispas"] != DBNull.Value ? dr.Field<string>("bts_pispas") : String.Empty;
            vo.Nrcrna = dr["bts_nrcrna"] != DBNull.Value ? dr.Field<string>("bts_nrcrna") : String.Empty;
            vo.Endere = dr["bts_endere"] != DBNull.Value ? dr.Field<string>("bts_endere") : String.Empty;
            vo.Nrend = dr["bts_nr_end"] != DBNull.Value ? dr.Field<string>("bts_nr_end") : String.Empty;
            vo.Comend = dr["bts_comend"] != DBNull.Value ? dr.Field<string>("bts_comend") : String.Empty;
            vo.Bairro = dr["bts_bairro"] != DBNull.Value ? dr.Field<string>("bts_bairro") : String.Empty;
            vo.Codmun = dr["bts_codmun"] != DBNull.Value ? dr.Field<string>("bts_codmun") : String.Empty;
            vo.Munici = dr["bts_munici"] != DBNull.Value ? dr.Field<string>("bts_munici") : String.Empty;
            vo.Estado = dr["bts_estado"] != DBNull.Value ? dr.Field<string>("bts_estado") : String.Empty;
            vo.Cepusr = dr["bts_cepusr"] != DBNull.Value ? dr.Field<string>("bts_cepusr") : String.Empty;
            vo.Ddd = dr["bts_ddd"] != DBNull.Value ? dr.Field<string>("bts_ddd") : String.Empty;
            vo.Telefo = dr["bts_telefo"] != DBNull.Value ? dr.Field<string>("bts_telefo") : String.Empty;
            vo.Telres = dr["bts_telres"] != DBNull.Value ? dr.Field<string>("bts_telres") : String.Empty;
            vo.Telcom = dr["bts_telcom"] != DBNull.Value ? dr.Field<string>("bts_telcom") : String.Empty;
            vo.Email = dr["bts_email"] != DBNull.Value ? dr.Field<string>("bts_email") : String.Empty;

            return vo;
        }

        internal static PVidaVO GetByCns(string cns, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                "	from VW_PR_VIDA " +
                "	WHERE upper(trim(bts_nrcrna)) like upper(trim(:cns)) " +
                "	and upper(trim(bts_matvid)) in (select distinct upper(trim(ba1_matvid)) from VW_PR_USUARIO) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":cns", Tipo = DbType.String, Value = cns.Trim() });

            return BaseDAO.ExecuteDataRow(db, sql, FromDataRow, lstParams);
        }

        private static string NextId(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de vida
            string sql = "select to_number(max(trim(bts_matvid))) + 1 from BTS010";
            int matvid = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql));
            return matvid.ToString().PadLeft(8, '0');
        }

        private static Int64 NextRecno(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from BTS010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }

        internal static string CriarVida(PVidaVO vida, EvidaDatabase db)
        {
            string sql = "insert into BTS010 (bts_matvid, bts_nomusr, bts_nomcar, bts_sexo, bts_datnas, bts_cpfusr, bts_drgusr, bts_rgest, bts_pai, bts_mae, bts_tippes, " +
                " bts_orgem, bts_estciv, bts_email, bts_nrcrna, bts_endere, bts_nr_end, bts_comend, bts_bairro, bts_codmun, bts_munici, bts_ddd, bts_telres, bts_telefo, bts_telcom, bts_estado, bts_cepusr, bts_univer, bts_deffis, bts_tutela, bts_invali, R_E_C_N_O_) " +
                " values (:matvid, :nomusr, :nomcar, :sexo, :datnas, :cpfusr, :drgusr, :rgest, :pai, :mae, :tippes, :orgem, :estciv, :email, :nrcrna, :endere, :nrend, :comend, :bairro, :codmun, :munici, :ddd, :telres, :telefo, :telcom, :estado, :cepusr, :univer, :deffis, :tutela, :invali, :recno)";

            Int64 recno = NextRecno(db);
            vida.Matvid = NextId(db);

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(vida.Matvid == null ? new Parametro(":matvid", DbType.String, "".PadRight(8, ' ')) : new Parametro(":matvid", DbType.String, vida.Matvid.Trim().Length >= 8 ? vida.Matvid.Trim().Substring(0, 8) : vida.Matvid.Trim().PadRight(8, ' ')));
            lstParam.Add(vida.Nomusr == null ? new Parametro(":nomusr", DbType.String, "".PadRight(70, ' ')) : new Parametro(":nomusr", DbType.String, vida.Nomusr.Trim().Length >= 70 ? vida.Nomusr.Trim().Substring(0, 70) : vida.Nomusr.Trim().PadRight(70, ' ')));
            lstParam.Add(vida.Nomcar == null ? new Parametro(":nomcar", DbType.String, "".PadRight(30, ' ')) : new Parametro(":nomcar", DbType.String, vida.Nomcar.Trim().Length >= 30 ? vida.Nomcar.Trim().Substring(0, 30) : vida.Nomcar.Trim().PadRight(30, ' ')));
            lstParam.Add(vida.Sexo == null ? new Parametro(":sexo", DbType.String, "".PadRight(1, ' ')) : new Parametro(":sexo", DbType.String, vida.Sexo.Trim().Length >= 1 ? vida.Sexo.Trim().Substring(0, 1) : vida.Sexo.Trim().PadRight(1, ' ')));
            lstParam.Add(vida.Datnas == null ? new Parametro(":datnas", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datnas", DbType.String, vida.Datnas.Trim().Length >= 8 ? vida.Datnas.Trim().Substring(0, 8) : vida.Datnas.Trim().PadRight(8, ' ')));
            lstParam.Add(vida.Cpfusr == null ? new Parametro(":cpfusr", DbType.String, "".PadRight(14, ' ')) : new Parametro(":cpfusr", DbType.String, vida.Cpfusr.Trim().Length >= 14 ? vida.Cpfusr.Trim().Substring(0, 14) : vida.Cpfusr.Trim().PadRight(14, ' ')));
            lstParam.Add(vida.Drgusr == null ? new Parametro(":drgusr", DbType.String, "".PadRight(20, ' ')) : new Parametro(":drgusr", DbType.String, vida.Drgusr.Trim().Length >= 20 ? vida.Drgusr.Trim().Substring(0, 20) : vida.Drgusr.Trim().PadRight(20, ' ')));
            lstParam.Add(vida.Rgest == null ? new Parametro(":rgest", DbType.String, "".PadRight(2, ' ')) : new Parametro(":rgest", DbType.String, vida.Rgest.Trim().Length >= 2 ? vida.Rgest.Trim().Substring(0, 2) : vida.Rgest.Trim().PadRight(2, ' ')));
            lstParam.Add(vida.Pai == null ? new Parametro(":pai", DbType.String, "".PadRight(30, ' ')) : new Parametro(":pai", DbType.String, vida.Pai.Trim().Length >= 30 ? vida.Pai.Trim().Substring(0, 30) : vida.Pai.Trim().PadRight(30, ' ')));
            lstParam.Add(vida.Mae == null ? new Parametro(":mae", DbType.String, "".PadRight(120, ' ')) : new Parametro(":mae", DbType.String, vida.Mae.Trim().Length >= 120 ? vida.Mae.Trim().Substring(0, 120) : vida.Mae.Trim().PadRight(120, ' ')));
            lstParam.Add(new Parametro(":tippes", DbType.String, "F"));
            lstParam.Add(vida.Orgem == null ? new Parametro(":orgem", DbType.String, "".PadRight(10, ' ')) : new Parametro(":orgem", DbType.String, vida.Orgem.Trim().Length >= 10 ? vida.Orgem.Trim().Substring(0, 10) : vida.Orgem.Trim().PadRight(10, ' ')));
            lstParam.Add(vida.Estciv == null ? new Parametro(":estciv", DbType.String, "".PadRight(1, ' ')) : new Parametro(":estciv", DbType.String, vida.Estciv.Trim().Length >= 1 ? vida.Estciv.Trim().Substring(0, 1) : vida.Estciv.Trim().PadRight(1, ' ')));
            lstParam.Add(vida.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, vida.Email.Trim().Length >= 100 ? vida.Email.Trim().Substring(0, 100) : vida.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(vida.Nrcrna == null ? new Parametro(":nrcrna", DbType.String, "".PadRight(15, ' ')) : new Parametro(":nrcrna", DbType.String, vida.Nrcrna.Trim().Length >= 15 ? vida.Nrcrna.Trim().Substring(0, 15) : vida.Nrcrna.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Endere == null ? new Parametro(":endere", DbType.String, "".PadRight(40, ' ')) : new Parametro(":endere", DbType.String, vida.Endere.Trim().Length >= 40 ? vida.Endere.Trim().Substring(0, 40) : vida.Endere.Trim().PadRight(40, ' ')));
            lstParam.Add(vida.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, vida.Nrend.Trim().Length >= 6 ? vida.Nrend.Trim().Substring(0, 6) : vida.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(vida.Comend == null ? new Parametro(":comend", DbType.String, "".PadRight(20, ' ')) : new Parametro(":comend", DbType.String, vida.Comend.Trim().Length >= 20 ? vida.Comend.Trim().Substring(0, 20) : vida.Comend.Trim().PadRight(20, ' ')));
            lstParam.Add(vida.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, vida.Bairro.Trim().Length >= 40 ? vida.Bairro.Trim().Substring(0, 40) : vida.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(vida.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(7, ' ')) : new Parametro(":codmun", DbType.String, vida.Codmun.Trim().Length >= 7 ? vida.Codmun.Trim().Substring(0, 7) : vida.Codmun.Trim().PadRight(7, ' ')));
            lstParam.Add(vida.Munici == null ? new Parametro(":munici", DbType.String, "".PadRight(30, ' ')) : new Parametro(":munici", DbType.String, vida.Munici.Trim().Length >= 30 ? vida.Munici.Trim().Substring(0, 30) : vida.Munici.Trim().PadRight(30, ' ')));
            lstParam.Add(vida.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, vida.Ddd.Trim().Length >= 3 ? vida.Ddd.Trim().Substring(0, 3) : vida.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(vida.Telres == null ? new Parametro(":telres", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telres", DbType.String, vida.Telres.Trim().Length >= 15 ? vida.Telres.Trim().Substring(0, 15) : vida.Telres.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Telefo == null ? new Parametro(":telefo", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telefo", DbType.String, vida.Telefo.Trim().Length >= 15 ? vida.Telefo.Trim().Substring(0, 15) : vida.Telefo.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Telcom == null ? new Parametro(":telcom", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telcom", DbType.String, vida.Telcom.Trim().Length >= 15 ? vida.Telcom.Trim().Substring(0, 15) : vida.Telcom.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Estado == null ? new Parametro(":estado", DbType.String, "".PadRight(2, ' ')) : new Parametro(":estado", DbType.String, vida.Estado.Trim().Length >= 2 ? vida.Estado.Trim().Substring(0, 2) : vida.Estado.Trim().PadRight(2, ' ')));
            lstParam.Add(vida.Cepusr == null ? new Parametro(":cepusr", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cepusr", DbType.String, vida.Cepusr.Trim().Length >= 8 ? vida.Cepusr.Trim().Substring(0, 8) : vida.Cepusr.Trim().PadRight(8, ' ')));
            lstParam.Add(new Parametro(":univer", DbType.String, "0"));
            lstParam.Add(new Parametro(":deffis", DbType.String, "0"));
            lstParam.Add(new Parametro(":tutela", DbType.String, "0"));
            lstParam.Add(new Parametro(":invali", DbType.String, "0"));
            
            lstParam.Add(new Parametro(":recno", DbType.Int64, recno));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
              
            return vida.Matvid;
        }

        internal static void AlterarVida(PVidaVO vida, EvidaDatabase db)
        {
            string sql = "update BTS010 set bts_nomusr = :nomusr, bts_nomcar = :nomcar, bts_sexo = :sexo, bts_datnas = :datnas, bts_cpfusr = :cpfusr, bts_drgusr = :drgusr, " +
                " bts_rgest = :rgest, bts_pai = :pai, bts_mae = :mae, bts_orgem = :orgem, bts_estciv = :estciv, bts_email = :email, bts_nrcrna = :nrcrna, bts_endere = :endere, " +
                " bts_nr_end = :nrend, bts_comend = :comend, bts_bairro = :bairro, bts_codmun = :codmun, bts_munici = :munici, bts_ddd = :ddd, bts_telres = :telres, bts_telefo = :telefo, bts_telcom = :telcom, bts_estado = :estado, bts_cepusr = :cepusr " +
                " where upper(trim(bts_matvid)) = upper(trim(:matvid)) ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(vida.Matvid == null ? new Parametro(":matvid", DbType.String, "".PadRight(8, ' ')) : new Parametro(":matvid", DbType.String, vida.Matvid.Trim()));
            lstParam.Add(vida.Nomusr == null ? new Parametro(":nomusr", DbType.String, "".PadRight(70, ' ')) : new Parametro(":nomusr", DbType.String, vida.Nomusr.Trim().Length >= 70 ? vida.Nomusr.Trim().Substring(0, 70) : vida.Nomusr.Trim().PadRight(70, ' ')));
            lstParam.Add(vida.Nomcar == null ? new Parametro(":nomcar", DbType.String, "".PadRight(30, ' ')) : new Parametro(":nomcar", DbType.String, vida.Nomcar.Trim().Length >= 30 ? vida.Nomcar.Trim().Substring(0, 30) : vida.Nomcar.Trim().PadRight(30, ' ')));
            lstParam.Add(vida.Sexo == null ? new Parametro(":sexo", DbType.String, "".PadRight(1, ' ')) : new Parametro(":sexo", DbType.String, vida.Sexo.Trim().Length >= 1 ? vida.Sexo.Trim().Substring(0, 1) : vida.Sexo.Trim().PadRight(1, ' ')));
            lstParam.Add(vida.Datnas == null ? new Parametro(":datnas", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datnas", DbType.String, vida.Datnas.Trim().Length >= 8 ? vida.Datnas.Trim().Substring(0, 8) : vida.Datnas.Trim().PadRight(8, ' ')));
            lstParam.Add(vida.Cpfusr == null ? new Parametro(":cpfusr", DbType.String, "".PadRight(14, ' ')) : new Parametro(":cpfusr", DbType.String, vida.Cpfusr.Trim().Length >= 14 ? vida.Cpfusr.Trim().Substring(0, 14) : vida.Cpfusr.Trim().PadRight(14, ' ')));
            lstParam.Add(vida.Drgusr == null ? new Parametro(":drgusr", DbType.String, "".PadRight(20, ' ')) : new Parametro(":drgusr", DbType.String, vida.Drgusr.Trim().Length >= 20 ? vida.Drgusr.Trim().Substring(0, 20) : vida.Drgusr.Trim().PadRight(20, ' ')));
            lstParam.Add(vida.Rgest == null ? new Parametro(":rgest", DbType.String, "".PadRight(2, ' ')) : new Parametro(":rgest", DbType.String, vida.Rgest.Trim().Length >= 2 ? vida.Rgest.Trim().Substring(0, 2) : vida.Rgest.Trim().PadRight(2, ' ')));
            lstParam.Add(vida.Pai == null ? new Parametro(":pai", DbType.String, "".PadRight(30, ' ')) : new Parametro(":pai", DbType.String, vida.Pai.Trim().Length >= 30 ? vida.Pai.Trim().Substring(0, 30) : vida.Pai.Trim().PadRight(30, ' ')));
            lstParam.Add(vida.Mae == null ? new Parametro(":mae", DbType.String, "".PadRight(120, ' ')) : new Parametro(":mae", DbType.String, vida.Mae.Trim().Length >= 120 ? vida.Mae.Trim().Substring(0, 120) : vida.Mae.Trim().PadRight(120, ' ')));
            lstParam.Add(vida.Orgem == null ? new Parametro(":orgem", DbType.String, "".PadRight(10, ' ')) : new Parametro(":orgem", DbType.String, vida.Orgem.Trim().Length >= 10 ? vida.Orgem.Trim().Substring(0, 10) : vida.Orgem.Trim().PadRight(10, ' ')));
            lstParam.Add(vida.Estciv == null ? new Parametro(":estciv", DbType.String, "".PadRight(1, ' ')) : new Parametro(":estciv", DbType.String, vida.Estciv.Trim().Length >= 1 ? vida.Estciv.Trim().Substring(0, 1) : vida.Estciv.Trim().PadRight(1, ' ')));
            lstParam.Add(vida.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, vida.Email.Trim().Length >= 100 ? vida.Email.Trim().Substring(0, 100) : vida.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(vida.Nrcrna == null ? new Parametro(":nrcrna", DbType.String, "".PadRight(15, ' ')) : new Parametro(":nrcrna", DbType.String, vida.Nrcrna.Trim().Length >= 15 ? vida.Nrcrna.Trim().Substring(0, 15) : vida.Nrcrna.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Endere == null ? new Parametro(":endere", DbType.String, "".PadRight(40, ' ')) : new Parametro(":endere", DbType.String, vida.Endere.Trim().Length >= 40 ? vida.Endere.Trim().Substring(0, 40) : vida.Endere.Trim().PadRight(40, ' ')));
            lstParam.Add(vida.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, vida.Nrend.Trim().Length >= 6 ? vida.Nrend.Trim().Substring(0, 6) : vida.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(vida.Comend == null ? new Parametro(":comend", DbType.String, "".PadRight(20, ' ')) : new Parametro(":comend", DbType.String, vida.Comend.Trim().Length >= 20 ? vida.Comend.Trim().Substring(0, 20) : vida.Comend.Trim().PadRight(20, ' ')));
            lstParam.Add(vida.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, vida.Bairro.Trim().Length >= 40 ? vida.Bairro.Trim().Substring(0, 40) : vida.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(vida.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(7, ' ')) : new Parametro(":codmun", DbType.String, vida.Codmun.Trim().Length >= 7 ? vida.Codmun.Trim().Substring(0, 7) : vida.Codmun.Trim().PadRight(7, ' ')));
            lstParam.Add(vida.Munici == null ? new Parametro(":munici", DbType.String, "".PadRight(30, ' ')) : new Parametro(":munici", DbType.String, vida.Munici.Trim().Length >= 30 ? vida.Munici.Trim().Substring(0, 30) : vida.Munici.Trim().PadRight(30, ' ')));
            lstParam.Add(vida.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, vida.Ddd.Trim().Length >= 3 ? vida.Ddd.Trim().Substring(0, 3) : vida.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(vida.Telres == null ? new Parametro(":telres", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telres", DbType.String, vida.Telres.Trim().Length >= 15 ? vida.Telres.Trim().Substring(0, 15) : vida.Telres.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Telefo == null ? new Parametro(":telefo", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telefo", DbType.String, vida.Telefo.Trim().Length >= 15 ? vida.Telefo.Trim().Substring(0, 15) : vida.Telefo.Trim().PadRight(15, ' ')));
            lstParam.Add(vida.Telcom == null ? new Parametro(":telcom", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telcom", DbType.String, vida.Telcom.Trim().Length >= 15 ? vida.Telcom.Trim().Substring(0, 15) : vida.Telcom.Trim().PadRight(15, ' ')));            
            lstParam.Add(vida.Estado == null ? new Parametro(":estado", DbType.String, "".PadRight(2, ' ')) : new Parametro(":estado", DbType.String, vida.Estado.Trim().Length >= 2 ? vida.Estado.Trim().Substring(0, 2) : vida.Estado.Trim().PadRight(2, ' ')));
            lstParam.Add(vida.Cepusr == null ? new Parametro(":cepusr", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cepusr", DbType.String, vida.Cepusr.Trim().Length >= 8 ? vida.Cepusr.Trim().Substring(0, 8) : vida.Cepusr.Trim().PadRight(8, ' ')));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void SalvarDadosContato(PVidaVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE BTS010 SET BTS_ENDERE = :endereco, BTS_NR_END = :numero, " +
                "	BTS_COMEND = :complemento, BTS_BAIRRO = :bairro, BTS_CEPUSR =:cep, BTS_CODMUN = :cidade, BTS_MUNICI = :municipio, BTS_ESTADO = :estado, " +
                "	BTS_EMAIL = :email, BTS_DDD = :ddd, BTS_TELRES = :telResidencial, " +
                "	BTS_TELCOM = :telComercial, BTS_TELEFO = :telCelular " +
                "	WHERE upper(trim(BTS_MATVID)) = upper(trim(:matvid)) ";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":endereco", DbType.String, vo.Endere.Trim().Length >= 40 ? vo.Endere.Trim().Substring(0, 40).ToUpper() : vo.Endere.Trim().PadRight(40, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Nrend.Trim().Length >= 6 ? vo.Nrend.Trim().Substring(0, 6).ToUpper() : vo.Nrend.Trim().PadRight(6, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Comend.Trim().Length >= 20 ? vo.Comend.Trim().Substring(0, 20).ToUpper() : vo.Comend.Trim().PadRight(20, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Bairro.Trim().Length >= 40 ? vo.Bairro.Trim().Substring(0, 40).ToUpper() : vo.Bairro.Trim().PadRight(40, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":cep", DbType.String, vo.Cepusr.Trim().Length >= 8 ? vo.Cepusr.Trim().Substring(0, 8).ToUpper() : vo.Cepusr.Trim().PadRight(8, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Codmun.Trim().Length >= 7 ? vo.Codmun.Trim().Substring(0, 7).ToUpper() : vo.Codmun.Trim().PadRight(7, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":municipio", DbType.String, vo.Munici.Trim().Length >= 30 ? vo.Munici.Trim().Substring(0, 30).ToUpper() : vo.Munici.Trim().PadRight(30, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":estado", DbType.String, vo.Estado.Trim().Length >= 2 ? vo.Estado.Trim().Substring(0, 2).ToUpper() : vo.Estado.Trim().PadRight(2, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email.Trim().Length >= 100 ? vo.Email.Trim().Substring(0, 100).ToUpper() : vo.Email.Trim().PadRight(100, ' ').ToUpper());

            db.AddInParameter(dbCommand, ":ddd", DbType.String, vo.Ddd.Trim().Length >= 3 ? vo.Ddd.Trim().Substring(0, 3) : vo.Ddd.Trim().PadRight(3, ' '));
            db.AddInParameter(dbCommand, ":telResidencial", DbType.String, vo.Telres.Trim().Length >= 15 ? vo.Telres.Trim().Substring(0, 15) : vo.Telres.Trim().PadRight(15, ' '));
            db.AddInParameter(dbCommand, ":telComercial", DbType.String, vo.Telcom.Trim().Length >= 15 ? vo.Telcom.Trim().Substring(0, 15) : vo.Telcom.Trim().PadRight(15, ' '));
            db.AddInParameter(dbCommand, ":telCelular", DbType.String, vo.Telefo.Trim().Length >= 15 ? vo.Telefo.Trim().Substring(0, 15) : vo.Telefo.Trim().PadRight(15, ' '));

            db.AddInParameter(dbCommand, ":matvid", DbType.String, vo.Matvid.Trim());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

    }
}
