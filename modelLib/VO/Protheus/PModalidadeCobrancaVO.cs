using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PModalidadeCobrancaVO
    {
        public string Filial { get; set; }
        public string Propri { get; set; }
        public string Codigo { get; set; }
        public string Descri { get; set; }
        public string Data { get; set; }
        public string Memo { get; set; }
        public string Uso { get; set; }
        public string Tipo { get; set; }
        public string Faifam { get; set; }

        public static PModalidadeCobrancaVO FromDataRow(DataRow dr)
        {
            PModalidadeCobrancaVO vo = new PModalidadeCobrancaVO();

            vo.Filial = dr["BJ1_FILIAL"] != DBNull.Value ? dr.Field<string>("BJ1_FILIAL") : String.Empty;
            vo.Propri = dr["BJ1_PROPRI"] != DBNull.Value ? dr.Field<string>("BJ1_PROPRI") : String.Empty;
            vo.Codigo = dr["BJ1_CODIGO"] != DBNull.Value ? dr.Field<string>("BJ1_CODIGO") : String.Empty;
            vo.Descri = dr["BJ1_DESCRI"] != DBNull.Value ? dr.Field<string>("BJ1_DESCRI") : String.Empty;
            vo.Data = dr["BJ1_DATA"] != DBNull.Value ? dr.Field<string>("BJ1_DATA") : String.Empty;
            vo.Memo = dr["BJ1_MEMO"] != DBNull.Value ? dr.Field<string>("BJ1_MEMO") : String.Empty;
            vo.Uso = dr["BJ1_USO"] != DBNull.Value ? dr.Field<string>("BJ1_USO") : String.Empty;
            vo.Tipo = dr["BJ1_TIPO"] != DBNull.Value ? dr.Field<string>("BJ1_TIPO") : String.Empty;
            vo.Faifam = dr["BJ1_FAIFAM"] != DBNull.Value ? dr.Field<string>("BJ1_FAIFAM") : String.Empty;

            return vo;
        }
    }
}
