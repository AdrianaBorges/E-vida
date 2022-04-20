using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PFaixaEtariaFormasFamiliaVO
    {
        public string Codope { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
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
    }
}
