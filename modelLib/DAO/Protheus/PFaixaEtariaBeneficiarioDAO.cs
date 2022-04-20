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
    internal class PFaixaEtariaBeneficiarioDAO
    {
        private static string FIELDS = " btn_codigo, btn_codfor, btn_codfai, btn_sexo, btn_idaini, btn_idafin, btn_valfai, btn_faifam, btn_qtdmin, " +
        " btn_qtdmax, btn_rejapl, btn_automa, btn_perrej, btn_anomes, btn_vlrant ";

        internal static List<PFaixaEtariaBeneficiarioVO> ListarFaixas(string codigo, string numcon, string subcon, string codpro, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                "	from VW_PR_FAIXA_ETARIA_BENEF " +
                "	where btn_codigo = :codigo  " +
                "	and btn_numcon = :numcon " +
                "	and btn_subcon = :subcon " +
                "   and btn_codpro = :codpro ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo.Trim() });
            lstParams.Add(new Parametro() { Name = ":numcon", Tipo = DbType.String, Value = numcon.Trim() });
            lstParams.Add(new Parametro() { Name = ":subcon", Tipo = DbType.String, Value = subcon.Trim() });
            lstParams.Add(new Parametro() { Name = ":codpro", Tipo = DbType.String, Value = codpro.Trim() });

            List<PFaixaEtariaBeneficiarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, PFaixaEtariaBeneficiarioVO.FromDataRow, lstParams);
            return lst;
        }

    }
}
