using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PFaixaEtariaBeneficiarioVO
    {
        public string Codigo { get; set; }
        public string Codfor { get; set; }
        public string Codfai { get; set; }
        public string Sexo { get; set; }
        public Double Idaini { get; set; }
        public Double Idafin { get; set; }
        public Double Valfai { get; set; }
        public string Faifam { get; set; }
        public Double Qtdmin { get; set; }
        public Double Qtdmax { get; set; }
        public Double Rejapl { get; set; }
        public string Automa { get; set; }
        public string Perrej { get; set; }
        public string Anomes { get; set; }
        public Double Vlrant { get; set; }

        public static PFaixaEtariaBeneficiarioVO FromDataRow(DataRow dr)
        {
            PFaixaEtariaBeneficiarioVO vo = new PFaixaEtariaBeneficiarioVO();

            vo.Codigo = dr["BTN_CODIGO"] != DBNull.Value ? dr.Field<string>("BTN_CODIGO") : String.Empty;
            vo.Codfor = dr["BTN_CODFOR"] != DBNull.Value ? dr.Field<string>("BTN_CODFOR") : String.Empty;
            vo.Codfai = dr["BTN_CODFAI"] != DBNull.Value ? dr.Field<string>("BTN_CODFAI") : String.Empty;
            vo.Sexo = dr["BTN_SEXO"] != DBNull.Value ? dr.Field<string>("BTN_SEXO") : String.Empty;
            vo.Idaini = dr["BTN_IDAINI"] != DBNull.Value ? Convert.ToDouble(dr["BTN_IDAINI"]) : 0.0;
            vo.Idafin = dr["BTN_IDAFIN"] != DBNull.Value ? Convert.ToDouble(dr["BTN_IDAFIN"]) : 0.0;
            vo.Valfai = dr["BTN_VALFAI"] != DBNull.Value ? Convert.ToDouble(dr["BTN_VALFAI"]) : 0.0;
            vo.Faifam = dr["BTN_FAIFAM"] != DBNull.Value ? dr.Field<string>("BTN_FAIFAM") : String.Empty;
            vo.Qtdmin = dr["BTN_QTDMIN"] != DBNull.Value ? Convert.ToDouble(dr["BTN_QTDMIN"]) : 0.0;
            vo.Qtdmax = dr["BTN_QTDMAX"] != DBNull.Value ? Convert.ToDouble(dr["BTN_QTDMAX"]) : 0.0;
            vo.Rejapl = dr["BTN_REJAPL"] != DBNull.Value ? Convert.ToDouble(dr["BTN_REJAPL"]) : 0.0;
            vo.Automa = dr["BTN_AUTOMA"] != DBNull.Value ? dr.Field<string>("BTN_AUTOMA") : String.Empty;
            vo.Perrej = dr["BTN_PERREJ"] != DBNull.Value ? dr.Field<string>("BTN_PERREJ") : String.Empty;
            vo.Anomes = dr["BTN_ANOMES"] != DBNull.Value ? dr.Field<string>("BTN_ANOMES") : String.Empty;
            vo.Vlrant = dr["BTN_VLRANT"] != DBNull.Value ? Convert.ToDouble(dr["BTN_VLRANT"]) : 0.0;

            return vo;
        }

    }
}
