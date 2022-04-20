using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PSubcontratoVO
    {
        public string Numcon { get; set; }
        public string Vercon { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Subcon { get; set; }
        public string Versub { get; set; }
        public string Descri { get; set; }
        public string Nreduz { get; set; }
        public string Tippag { get; set; }
        public string Portad { get; set; }
        public string Agedep { get; set; }
        public string Ctacor { get; set; }
        public string Tipblo { get; set; }
        public string Codblo { get; set; }
        public string Datblo { get; set; }

        public static PSubcontratoVO FromDataRow(DataRow dr)
        {
            PSubcontratoVO vo = new PSubcontratoVO();

            vo.Numcon = dr["BQC_NUMCON"] != DBNull.Value ? dr.Field<string>("BQC_NUMCON") : String.Empty;
            vo.Vercon = dr["BQC_VERCON"] != DBNull.Value ? dr.Field<string>("BQC_VERCON") : String.Empty;
            vo.Codint = dr["BQC_CODINT"] != DBNull.Value ? dr.Field<string>("BQC_CODINT") : String.Empty;
            vo.Codemp = dr["BQC_CODEMP"] != DBNull.Value ? dr.Field<string>("BQC_CODEMP") : String.Empty;
            vo.Subcon = dr["BQC_SUBCON"] != DBNull.Value ? dr.Field<string>("BQC_SUBCON") : String.Empty;
            vo.Versub = dr["BQC_VERSUB"] != DBNull.Value ? dr.Field<string>("BQC_VERSUB") : String.Empty;
            vo.Descri = dr["BQC_DESCRI"] != DBNull.Value ? dr.Field<string>("BQC_DESCRI") : String.Empty;
            vo.Nreduz = dr["BQC_NREDUZ"] != DBNull.Value ? dr.Field<string>("BQC_NREDUZ") : String.Empty;
            vo.Tippag = dr["BQC_TIPPAG"] != DBNull.Value ? dr.Field<string>("BQC_TIPPAG") : String.Empty;
            vo.Portad = dr["BQC_PORTAD"] != DBNull.Value ? dr.Field<string>("BQC_PORTAD") : String.Empty;
            vo.Agedep = dr["BQC_AGEDEP"] != DBNull.Value ? dr.Field<string>("BQC_AGEDEP") : String.Empty;
            vo.Ctacor = dr["BQC_CTACOR"] != DBNull.Value ? dr.Field<string>("BQC_CTACOR") : String.Empty;
            vo.Tipblo = dr["BQC_TIPBLO"] != DBNull.Value ? dr.Field<string>("BQC_TIPBLO") : String.Empty;
            vo.Codblo = dr["BQC_CODBLO"] != DBNull.Value ? dr.Field<string>("BQC_CODBLO") : String.Empty;
            vo.Datblo = dr["BQC_DATBLO"] != DBNull.Value ? dr.Field<string>("BQC_DATBLO") : String.Empty;

            return vo;
        }
    }
}
