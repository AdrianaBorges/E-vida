using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PContatoOperadoraVO
    {
        public string Codint { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Telcon { get; set; }
        public string Ramal { get; set; }
        public string Setor { get; set; }
        public string Contis { get; set; }
        public string Tipcom { get; set; }
        public string Rg { get; set; }
        public string Especi { get; set; }
        public string Site { get; set; }
        public string Filial { get; set; }

        public static PContatoOperadoraVO FromDataRow(DataRow dr)
        {
            PContatoOperadoraVO vo = new PContatoOperadoraVO();

            vo.Codint = dr["BIM_CODINT"] != DBNull.Value ? dr.Field<string>("BIM_CODINT") : String.Empty;
            vo.Codigo = dr["BIM_CODIGO"] != DBNull.Value ? dr.Field<string>("BIM_CODIGO") : String.Empty;
            vo.Nome = dr["BIM_NOME"] != DBNull.Value ? dr.Field<string>("BIM_NOME") : String.Empty;
            vo.Email = dr["BIM_EMAIL"] != DBNull.Value ? dr.Field<string>("BIM_EMAIL") : String.Empty;
            vo.Fax = dr["BIM_FAX"] != DBNull.Value ? dr.Field<string>("BIM_FAX") : String.Empty;
            vo.Telcon = dr["BIM_TELCON"] != DBNull.Value ? dr.Field<string>("BIM_TELCON") : String.Empty;
            vo.Ramal = dr["BIM_RAMAL"] != DBNull.Value ? dr.Field<string>("BIM_RAMAL") : String.Empty;
            vo.Setor = dr["BIM_SETOR"] != DBNull.Value ? dr.Field<string>("BIM_SETOR") : String.Empty;
            vo.Contis = dr["BIM_CONTIS"] != DBNull.Value ? dr.Field<string>("BIM_CONTIS") : String.Empty;
            vo.Tipcom = dr["BIM_TIPCOM"] != DBNull.Value ? dr.Field<string>("BIM_TIPCOM") : String.Empty;
            vo.Rg = dr["BIM_RG"] != DBNull.Value ? dr.Field<string>("BIM_RG") : String.Empty;
            vo.Especi = dr["BIM_ESPECI"] != DBNull.Value ? dr.Field<string>("BIM_ESPECI") : String.Empty;
            vo.Site = dr["BIM_SITE"] != DBNull.Value ? dr.Field<string>("BIM_SITE") : String.Empty;
            vo.Filial = dr["BIM_FILIAL"] != DBNull.Value ? dr.Field<string>("BIM_FILIAL") : String.Empty;

            return vo;
        }

    }
}
