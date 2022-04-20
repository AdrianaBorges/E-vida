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
    internal class PFaixaEtariaFormasFamiliaDAO
    {
        private static Int64 NextRecno(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from BBU010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }

        internal static void CriarFaixaEtariaFormasFamilia(PFaixaEtariaFormasFamiliaVO faixa, EvidaDatabase db)
        {
            string sql = "insert into BBU010 (bbu_codope, bbu_codemp, bbu_matric, bbu_codfor, bbu_codfai, bbu_sexo, bbu_idaini, bbu_idafin, bbu_valfai, bbu_faifam, bbu_qtdmin, " +
                " bbu_qtdmax, bbu_rejapl, bbu_automa, bbu_perrej, bbu_anomes, bbu_vlrant, R_E_C_N_O_) " +
                " values (:codope, :codemp, :matric, :codfor, :codfai, :sexo, :idaini, :idafin, :valfai, :faifam, :qtdmin, :qtdmax, :rejapl, :automa, :perrej, :anomes, :vlrant, :recno)";

            Int64 recno = NextRecno(db);

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(faixa.Codope == null ? new Parametro(":codope", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codope", DbType.String, faixa.Codope.Trim().Length >= 4 ? faixa.Codope.Trim().Substring(0, 4) : faixa.Codope.Trim().PadRight(4, ' ')));
            lstParam.Add(faixa.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, faixa.Codemp.Trim().Length >= 4 ? faixa.Codemp.Trim().Substring(0, 4) : faixa.Codemp.Trim().PadRight(4, ' ')));
            lstParam.Add(faixa.Matric == null ? new Parametro(":matric", DbType.String, "".PadRight(6, ' ')) : new Parametro(":matric", DbType.String, faixa.Matric.Trim().Length >= 6 ? faixa.Matric.Trim().Substring(0, 6) : faixa.Matric.Trim().PadRight(6, ' ')));
            lstParam.Add(faixa.Codfor == null ? new Parametro(":codfor", DbType.String, "".PadRight(3, ' ')) : new Parametro(":codfor", DbType.String, faixa.Codfor.Trim().Length >= 3 ? faixa.Codfor.Trim().Substring(0, 3) : faixa.Codfor.Trim().PadRight(3, ' ')));
            lstParam.Add(faixa.Codfai == null ? new Parametro(":codfai", DbType.String, "".PadRight(3, ' ')) : new Parametro(":codfai", DbType.String, faixa.Codfai.Trim().Length >= 3 ? faixa.Codfai.Trim().Substring(0, 3) : faixa.Codfai.Trim().PadRight(3, ' ')));
            lstParam.Add(faixa.Sexo == null ? new Parametro(":sexo", DbType.String, "".PadRight(1, ' ')) : new Parametro(":sexo", DbType.String, faixa.Sexo.Trim().Length >= 1 ? faixa.Sexo.Trim().Substring(0, 1) : faixa.Sexo.Trim().PadRight(1, ' ')));
            lstParam.Add(new Parametro(":idaini", DbType.Double, faixa.Idaini));
            lstParam.Add(new Parametro(":idafin", DbType.Double, faixa.Idafin));
            lstParam.Add(new Parametro(":valfai", DbType.Double, faixa.Valfai));
            lstParam.Add(faixa.Faifam == null ? new Parametro(":faifam", DbType.String, "".PadRight(1, ' ')) : new Parametro(":faifam", DbType.String, faixa.Faifam.Trim().Length >= 1 ? faixa.Faifam.Trim().Substring(0, 1) : faixa.Faifam.Trim().PadRight(1, ' ')));
            lstParam.Add(new Parametro(":qtdmin", DbType.Double, faixa.Qtdmin));
            lstParam.Add(new Parametro(":qtdmax", DbType.Double, faixa.Qtdmax));
            lstParam.Add(new Parametro(":rejapl", DbType.Double, faixa.Rejapl));
            lstParam.Add(faixa.Automa == null ? new Parametro(":automa", DbType.String, "".PadRight(1, ' ')) : new Parametro(":automa", DbType.String, faixa.Automa.Trim().Length >= 1 ? faixa.Automa.Trim().Substring(0, 1) : faixa.Automa.Trim().PadRight(1, ' ')));
            lstParam.Add(faixa.Perrej == null ? new Parametro(":perrej", DbType.String, "".PadRight(8, ' ')) : new Parametro(":perrej", DbType.String, faixa.Perrej.Trim().Length >= 8 ? faixa.Perrej.Trim().Substring(0, 8) : faixa.Perrej.Trim().PadRight(8, ' ')));
            lstParam.Add(faixa.Anomes == null ? new Parametro(":anomes", DbType.String, "".PadRight(6, ' ')) : new Parametro(":anomes", DbType.String, faixa.Anomes.Trim().Length >= 6 ? faixa.Anomes.Trim().Substring(0, 6) : faixa.Anomes.Trim().PadRight(6, ' ')));
            lstParam.Add(new Parametro(":vlrant", DbType.Double, faixa.Vlrant));
            lstParam.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }
    }
}
