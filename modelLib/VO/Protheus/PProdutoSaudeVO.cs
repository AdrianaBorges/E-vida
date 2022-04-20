using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PProdutoSaudeVO
    {
        public string Versao { get; set; }
        public string Codigo { get; set; }
        public string Descri { get; set; }
        public string Nreduz { get; set; }
        public string Susep { get; set; }
        public string Tipo { get; set; }
        public string Codant { get; set; }
        public string Abrang { get; set; }
        public string Yautpr { get; set; }

        public static PProdutoSaudeVO FromDataRow(DataRow dr)
        {
            PProdutoSaudeVO vo = new PProdutoSaudeVO();

            vo.Versao = dr["bi3_versao"] != DBNull.Value ? dr.Field<string>("bi3_versao") : String.Empty;
            vo.Codigo = dr["bi3_codigo"] != DBNull.Value ? dr.Field<string>("bi3_codigo") : String.Empty;
            vo.Descri = dr["bi3_descri"] != DBNull.Value ? dr.Field<string>("bi3_descri") : String.Empty;
            vo.Nreduz = dr["bi3_nreduz"] != DBNull.Value ? dr.Field<string>("bi3_nreduz") : String.Empty;
            vo.Susep = dr["bi3_susep"] != DBNull.Value ? dr.Field<string>("bi3_susep") : String.Empty;
            vo.Tipo = dr["bi3_tipo"] != DBNull.Value ? dr.Field<string>("bi3_tipo") : String.Empty;
            vo.Codant = dr["bi3_codant"] != DBNull.Value ? dr.Field<string>("bi3_codant") : String.Empty;
            vo.Abrang = dr["bi3_abrang"] != DBNull.Value ? dr.Field<string>("bi3_abrang") : String.Empty;
            vo.Yautpr = dr["bi3_yautpr"] != DBNull.Value ? dr.Field<string>("bi3_yautpr") : String.Empty;
            
            return vo;
        }
    }
}
