using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.VO.Adesao
{
    public class PIntegracaoAdesaoVO
    {
        public PDeclaracaoVO Declaracao { get; set; }
        public PFamiliaVO Familia { get; set; }
        public PUsuarioVO UsuarioTitular { get; set; }
        public PVidaVO VidaTitular { get; set; }
        public PFornecedorVO FornecedorTitular { get; set; }
        public PClienteVO Cliente { get; set; }
        public List<PFaixaEtariaFormasFamiliaVO> FaixasEtarias { get; set; }
        public bool FlagNovaFamilia { get; set; }
        public bool FlagNovoUsuarioTitular { get; set; }
        public bool FlagNovaVidaTitular { get; set; }
        public bool FlagNovoFornecedorTitular { get; set; }
        public bool FlagNovoCliente { get; set; }


        // Mantidos apenas para compatibilidade
        public string CdCategoria { get; set; }
        public DateTime InicioVigencia { get; set; }
        public string CdPlanoVinculado { get; set; }
        public string TpCarencia { get; set; }
        public string CdMotivoDesligamentoFamilia { get; set; }
        public string CdMotivoDesligamentoUsuario { get; set; }
    }
}
