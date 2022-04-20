using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PEmpresaModalidadeCobrancaVO
    {
        public string Filial { get; set; }
        public string Codigo { get; set; }
        public string Numcon { get; set; }
        public string Vercon { get; set; }
        public string Subcon { get; set; }
        public string Versub { get; set; }
        public string Codpro { get; set; }
        public string Versao { get; set; }
        public string Codfor { get; set; }
        public string Rgimp { get; set; }
        public string Arqimp { get; set; }
        public string Automa { get; set; }

        public static PEmpresaModalidadeCobrancaVO FromDataRow(DataRow dr)
        {
            PEmpresaModalidadeCobrancaVO vo = new PEmpresaModalidadeCobrancaVO();

            vo.Filial = dr["BT9_FILIAL"] != DBNull.Value ? dr.Field<string>("BT9_FILIAL") : String.Empty;
            vo.Codigo = dr["BT9_CODIGO"] != DBNull.Value ? dr.Field<string>("BT9_CODIGO") : String.Empty;
            vo.Numcon = dr["BT9_NUMCON"] != DBNull.Value ? dr.Field<string>("BT9_NUMCON") : String.Empty;
            vo.Vercon = dr["BT9_VERCON"] != DBNull.Value ? dr.Field<string>("BT9_VERCON") : String.Empty;
            vo.Subcon = dr["BT9_SUBCON"] != DBNull.Value ? dr.Field<string>("BT9_SUBCON") : String.Empty;
            vo.Versub = dr["BT9_VERSUB"] != DBNull.Value ? dr.Field<string>("BT9_VERSUB") : String.Empty;
            vo.Codpro = dr["BT9_CODPRO"] != DBNull.Value ? dr.Field<string>("BT9_CODPRO") : String.Empty;
            vo.Versao = dr["BT9_VERSAO"] != DBNull.Value ? dr.Field<string>("BT9_VERSAO") : String.Empty;
            vo.Codfor = dr["BT9_CODFOR"] != DBNull.Value ? dr.Field<string>("BT9_CODFOR") : String.Empty;
            vo.Rgimp = dr["BT9_RGIMP"] != DBNull.Value ? dr.Field<string>("BT9_RGIMP") : String.Empty;
            vo.Arqimp = dr["BT9_ARQIMP"] != DBNull.Value ? dr.Field<string>("BT9_ARQIMP") : String.Empty;
            vo.Automa = dr["BT9_AUTOMA"] != DBNull.Value ? dr.Field<string>("BT9_AUTOMA") : String.Empty;

            return vo;
        }
    }
}
