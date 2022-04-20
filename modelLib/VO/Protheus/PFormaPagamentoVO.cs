using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PFormaPagamentoVO
    {
        public string Filial { get; set; }
        public string Codigo { get; set; }
        public string Rotina { get; set; }

        public static PFormaPagamentoVO FromDataRow(DataRow dr)
        {
            PFormaPagamentoVO vo = new PFormaPagamentoVO();

            vo.Filial = dr["BJ0_FILIAL"] != DBNull.Value ? dr.Field<string>("BJ0_FILIAL") : String.Empty;
            vo.Codigo = dr["BJ0_CODIGO"] != DBNull.Value ? dr.Field<string>("BJ0_CODIGO") : String.Empty;
            vo.Rotina = dr["BJ0_ROTINA"] != DBNull.Value ? dr.Field<string>("BJ0_ROTINA") : String.Empty;

            return vo;
        }
    }
}
