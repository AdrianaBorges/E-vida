using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO.Protheus
{
    internal class PFamiliaContratoDAO
    {
        internal static PFamiliaContratoVO GetLastFamiliaData(string codint, string codemp, string matric, string tipreg, string dtRef, EvidaDatabase db)
        {
            string sql = "select ba1_codint, ba1_codemp, ba1_matric, ba1_tipreg, ba1_conemp, ba1_vercon, ba1_subcon, ba1_versub, bqc_descri, ba1_datcar, ba1_datblo, ba1_motblo, ba1_consid  " +
                "	from VW_PR_FAMILIA_CONTRATO fc WHERE trim(ba1_codint) = trim(:codint) and trim(ba1_codemp) = trim(:codemp) and trim(ba1_matric) = trim(:matric) and trim(ba1_tipreg) = trim(:tipreg) " +
                "		and trim(ba1_datcar) = (select max(trim(fc2.ba1_datcar)) from VW_PR_FAMILIA_CONTRATO fc2 where trim(fc2.ba1_codint) = trim(:codint) AND trim(fc2.ba1_codemp) = trim(:codemp) AND trim(fc2.ba1_matric) = trim(:matric) AND trim(fc2.ba1_tipreg) = trim(:tipreg) AND trim(fc2.ba1_datcar) <= NVL(:dtRef, trim(fc2.ba1_datcar)) )";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });
            lstParams.Add(new Parametro() { Name = ":tipreg", Tipo = DbType.String, Value = tipreg });
            lstParams.Add(new Parametro() { Name = ":dtRef", Tipo = DbType.String, Value = dtRef });

            List<PFamiliaContratoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        private static PFamiliaContratoVO FromDataRow(DataRow dr)
        {
            PFamiliaContratoVO vo = new PFamiliaContratoVO();

            vo.Codint = dr["ba1_codint"] != DBNull.Value ? dr.Field<string>("ba1_codint") : String.Empty;
            vo.Codemp = dr["ba1_codemp"] != DBNull.Value ? dr.Field<string>("ba1_codemp") : String.Empty;
            vo.Matric = dr["ba1_matric"] != DBNull.Value ? dr.Field<string>("ba1_matric") : String.Empty;
            vo.Tipreg = dr["ba1_tipreg"] != DBNull.Value ? dr.Field<string>("ba1_tipreg") : String.Empty;
            vo.Conemp = dr["ba1_conemp"] != DBNull.Value ? dr.Field<string>("ba1_conemp") : String.Empty;
            vo.Vercon = dr["ba1_vercon"] != DBNull.Value ? dr.Field<string>("ba1_vercon") : String.Empty;
            vo.Subcon = dr["ba1_subcon"] != DBNull.Value ? dr.Field<string>("ba1_subcon") : String.Empty;
            vo.Versub = dr["ba1_versub"] != DBNull.Value ? dr.Field<string>("ba1_versub") : String.Empty;
            vo.Descri = dr["bqc_descri"] != DBNull.Value ? dr.Field<string>("bqc_descri") : String.Empty;
            vo.Datcar = dr["ba1_datcar"] != DBNull.Value ? dr.Field<string>("ba1_datcar") : String.Empty;
            vo.Datblo = dr["ba1_datblo"] != DBNull.Value ? dr.Field<string>("ba1_datblo") : String.Empty;
            vo.Motblo = dr["ba1_motblo"] != DBNull.Value ? dr.Field<string>("ba1_motblo") : String.Empty;
            vo.Consid = dr["ba1_consid"] != DBNull.Value ? dr.Field<string>("ba1_consid") : String.Empty;

            return vo;
        }
    }
}
