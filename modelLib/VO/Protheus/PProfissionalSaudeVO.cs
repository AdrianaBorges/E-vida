using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PProfissionalSaudeVO
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Cgc { get; set; }
        public string Codsig { get; set; }
        public string Numcr { get; set; }
        public string Estado { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Codblo { get; set; }
        public string Datblo { get; set; }

        public static PProfissionalSaudeVO FromDataRow(DataRow dr)
        {
            PProfissionalSaudeVO vo = new PProfissionalSaudeVO();

            vo.Codigo = Convert.ToString(dr["BB0_CODIGO"]);
            vo.Nome = Convert.ToString(dr["BB0_NOME"]);
            vo.Cgc = Convert.ToString(dr["BB0_CGC"]);
            vo.Codsig = Convert.ToString(dr["BB0_CODSIG"]);
            vo.Numcr = Convert.ToString(dr["BB0_NUMCR"]);
            vo.Estado = Convert.ToString(dr["BB0_ESTADO"]);
            vo.Tel = Convert.ToString(dr["BB0_TEL"]);
            vo.Email = Convert.ToString(dr["BB0_EMAIL"]);
            vo.Codblo = Convert.ToString(dr["BB0_CODBLO"]);
            vo.Datblo = Convert.ToString(dr["BB0_DATBLO"]);

            return vo;
        }

    }

}
