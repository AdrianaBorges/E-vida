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
    internal class PFamiliaProdutoDAO
    {
        internal static PFamiliaProdutoVO GetByFamiliaData(string codint, string codemp, string matric, string tipreg, string data, Database db)
        {
            string sql = "select ba1_codint, ba1_codemp, ba1_matric, ba1_tipreg, ba3_codpla, bi3_tipo, ba1_ycaren, ba1_datcar, ba1_datblo, ba1_motblo, ba1_consid " +
                "	from VW_PR_FAMILIA_PRODUTO fp WHERE trim(ba1_codint) = trim(:codint) and trim(ba1_codemp) = trim(:codemp) and trim(ba1_matric) = trim(:matric) and trim(ba1_tipreg) = trim(:tipreg) " +
                "		and trim(:data) BETWEEN trim(ba1_datcar) AND DECODE(ba1_datblo , '        ' , TO_CHAR(SYSDATE+1, 'YYYYMMDD'), ba1_datblo)";      

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });
            lstParams.Add(new Parametro() { Name = ":tipreg", Tipo = DbType.String, Value = tipreg });
            lstParams.Add(new Parametro() { Name = ":data", Tipo = DbType.String, Value = data });

            List<PFamiliaProdutoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PFamiliaProdutoVO GetLastFamiliaData(string codint, string codemp, string matric, string tipreg, string dtRef, EvidaDatabase db)
        {
            string sql = "select ba1_codint, ba1_codemp, ba1_matric, ba1_tipreg, ba3_codpla, bi3_tipo, ba1_ycaren, ba1_datcar, ba1_datblo, ba1_motblo, ba1_consid " +
                         " from VW_PR_FAMILIA_PRODUTO fp WHERE trim(ba1_codint) = trim(:codint) and trim(ba1_codemp) = trim(:codemp) and trim(ba1_matric) = trim(:matric) and trim(ba1_tipreg) = trim(:tipreg) " +
                         " and trim(ba1_datcar) = (select max(fp2.ba1_datcar) from VW_PR_FAMILIA_PRODUTO fp2 where trim(fp2.ba1_codint) = trim(:codint) AND trim(fp2.ba1_codemp) = trim(:codemp) AND trim(fp2.ba1_matric) = trim(:matric) AND trim(fp2.ba1_tipreg) = trim(:tipreg) AND trim(fp2.ba1_datcar) <= NVL(:dtRef, trim(fp2.ba1_datcar)) )";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });
            lstParams.Add(new Parametro() { Name = ":tipreg", Tipo = DbType.String, Value = tipreg });
            lstParams.Add(new Parametro() { Name = ":dtRef", Tipo = DbType.String, Value = dtRef });

            List<PFamiliaProdutoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        private static PFamiliaProdutoVO FromDataRow(DataRow dr)
        {
            PFamiliaProdutoVO vo = new PFamiliaProdutoVO();

            vo.Codint = dr["ba1_codint"] != DBNull.Value ? dr.Field<string>("ba1_codint") : String.Empty;
            vo.Codemp = dr["ba1_codemp"] != DBNull.Value ? dr.Field<string>("ba1_codemp") : String.Empty;
            vo.Matric = dr["ba1_matric"] != DBNull.Value ? dr.Field<string>("ba1_matric") : String.Empty;
            vo.Tipreg = dr["ba1_tipreg"] != DBNull.Value ? dr.Field<string>("ba1_tipreg") : String.Empty;
            vo.Codpla = dr["ba3_codpla"] != DBNull.Value ? dr.Field<string>("ba3_codpla") : String.Empty;
            vo.Tipo = dr["bi3_tipo"] != DBNull.Value ? dr.Field<string>("bi3_tipo") : String.Empty;
            vo.Ycaren = dr["ba1_ycaren"] != DBNull.Value ? dr.Field<string>("ba1_ycaren") : String.Empty;
            vo.Datcar = dr["ba1_datcar"] != DBNull.Value ? dr.Field<string>("ba1_datcar") : String.Empty;
            vo.Datblo = dr["ba1_datblo"] != DBNull.Value ? dr.Field<string>("ba1_datblo") : String.Empty;
            vo.Motblo = dr["ba1_motblo"] != DBNull.Value ? dr.Field<string>("ba1_motblo") : String.Empty;
            vo.Consid = dr["ba1_consid"] != DBNull.Value ? dr.Field<string>("ba1_consid") : String.Empty;

            return vo;
        }

    }
}
