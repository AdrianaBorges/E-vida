using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PUsuarioVO
    {
        public string Cdusuario { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
        public string Digito { get; set; }
        public string Matemp { get; set; }
        public string Matvid { get; set; }
        public string Conemp { get; set; }
        public string Vercon { get; set; }
        public string Subcon { get; set; }
        public string Versub { get; set; }
        public string Tipusu { get; set; }
        public string Graupa { get; set; }
        public string Datadm { get; set; }
        public string Datinc { get; set; }
        public string Datcar { get; set; }
        public string Dtvlcr { get; set; }
        public string Matant { get; set; }
        public string Yautpr { get; set; }
        public string Datblo { get; set; }
        public string Motblo { get; set; }
        public string Consid { get; set; }
        public string Nomusr { get; set; }
        public string Estciv { get; set; }
        public string Sexo { get; set; }
        public string Datnas { get; set; }
        public string Mae { get; set; }
        public string Pai { get; set; }
        public string Cpfusr { get; set; }
        public string Drgusr { get; set; }
        public string Orgem { get; set; }
        public string Pispas { get; set; }
        public string Endere { get; set; }
        public string Nrend { get; set; }
        public string Comend { get; set; }
        public string Bairro { get; set; }
        public string Codmun { get; set; }
        public string Munici { get; set; }
        public string Estado { get; set; }
        public string Cepusr { get; set; }
        public string Ddd { get; set; }
        public string Telefo { get; set; }
        public string Telres { get; set; }
        public string Telcom { get; set; }
        public string Email { get; set; }
        public string Ycaren { get; set; }
        public string Ycdleg { get; set; }
        public string Image { get; set; }
        public string Codpla { get; set; }
        public string Protocolo { get; set; }

        public string GetLogId()
        {
            return Cdusuario;
        }

        public string GetCarteira()
        {
            return Codint.Trim() + Codemp.Trim() + Matric.Trim() + Tipreg.Trim() + "." + Digito.Trim();
        }

    }
}
