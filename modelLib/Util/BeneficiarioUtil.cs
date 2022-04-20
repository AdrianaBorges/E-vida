using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util
{
    public static class BeneficiarioUtil
    {
        public static bool Bloqueado(string datBloqueio)
        {
            return datBloqueio != "        " && int.Parse(datBloqueio) <= int.Parse(DateTime.Today.ToString("yyyyMMdd"));

        }
    }
}
