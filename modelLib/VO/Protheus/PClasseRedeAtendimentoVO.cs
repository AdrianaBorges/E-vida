using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PClasseRedeAtendimentoVO
    {
        public string Filial { get; set; }
        public string Codigo { get; set; }
        public string Descri { get; set; }
        public string Codpt { get; set; }
        public string Forcon { get; set; }
        public string Qtdesp { get; set; }
        public string Tippe { get; set; }
        public string Obrcid { get; set; }
        public string Consft { get; set; }
    }
}
