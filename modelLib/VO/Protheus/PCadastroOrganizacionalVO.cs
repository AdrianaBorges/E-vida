using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PCadastroOrganizacionalVO
    {
        public string Codseq { get; set; }
        public string Codorg { get; set; }
        public string Descri { get; set; }
        public string Sigla { get; set; }
        public string Filial { get; set; }

    }
}
