using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PFamiliaProdutoVO
    {
        public const string CARENCIA_ISENTO = "I";
        public const string CARENCIA_NORMAL = "N";

        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
        public string Codpla { get; set; }
        public string Tipo { get; set; }
        public string Ycaren { get; set; }
        public string Datcar { get; set; }
        public string Datblo { get; set; }
        public string Motblo { get; set; }
        public string Consid { get; set; }

    }
}
