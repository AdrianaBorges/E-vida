using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO
{
    [Serializable]
    public class PCepVO
    {
        public string Cep { get; set; }
        public string Est { get; set; }
        public string Codmun { get; set; }
        public string Mun { get; set; }
        public string Bairro { get; set; }
        public string Tiplog { get; set; }
        public string Desclog { get; set; }
        public string End { get; set; }
    }

    [Serializable]
    public class PEnderecoVO : PCepVO
    {
        public string Nrend { get; set; }
        public string Comend { get; set; }
    }
}