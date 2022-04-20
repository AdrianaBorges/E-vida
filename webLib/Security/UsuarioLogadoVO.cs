using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.VO.SCL;

namespace eVida.Web.Security {
	[Serializable]
	public abstract class UsuarioLogadoVO : IUsuarioLogado {
		public abstract string Username { get; }
		public virtual bool HasPermission(Modulo modulo) {
			return true;
		}
	}

	[Serializable]
	public class UsuarioIntranetVO : UsuarioLogadoVO {
		public UsuarioVO Usuario { get; set; }
		public List<Modulo> Permissoes { get; set; }
		public SclUsuarioVO UsuarioScl { get; set; }

		public int Id { get { return Usuario.Id; } }

		public override string Username {
			get { return Usuario.Login; }
		}

		public override bool HasPermission(Modulo modulo) {
			if (modulo == Modulo.LOGIN || modulo == Modulo.INICIAL)
				return true;

			return Permissoes.Contains(modulo);
		}
	}

    [Serializable]
    public class UsuarioBeneficiarioVO : UsuarioLogadoVO
    {
        #region[BENEFICIÁRIO NO ISA]

        public HcBeneficiarioVO Beneficiario { get; set; }
        public HcBeneficiarioVO Titular { get; set; }
        public HcPlanoVO Plano { get; set; }
        public HcBeneficiarioPlanoVO BeneficiarioPlano { get; set; }
        public List<HcBeneficiarioVO> Beneficiarios { get; set; }
        public HcBeneficiarioCategoriaVO BeneficiarioCategoria { get; set; }

        public long CdBeneficiario
        {
            get
            {
                return Beneficiario.CdBeneficiario;
            }
        }

        /*public override string Username
        {
            get { return CdBeneficiario.ToString(); }
        }*/

        public long CdFuncionario
        {
            get { return Beneficiario.CdFuncionario; }
        }

        public int CdEmpresa
        {
            get { return Beneficiario.CdEmpresa; }
        }

        #endregion

        #region[BENEFICIÁRIO NO PROTHEUS]

        public PUsuarioVO Usuario { get; set; }
        public PUsuarioVO UsuarioTitular { get; set; }
        public PProdutoSaudeVO ProdutoSaude { get; set; }
        public PFamiliaProdutoVO FamiliaProduto { get; set; }
        public List<PUsuarioVO> Usuarios { get; set; }
        public PFamiliaContratoVO FamiliaContrato { get; set; }
        public ConfiguracaoIrVO ConfiguracaoIr { get; set; }

        public string CdUsuario
        {
            get
            {
                return Usuario.Codint.Trim() + "|" + Usuario.Codemp.Trim() + "|" + Usuario.Matric.Trim() + "|" + Usuario.Tipreg.Trim();
            }
        }

        public override string Username
        {
            get { return Usuario.Nomusr; }
        }

        public string Codint
        {
            get { return Usuario.Codint; }
        }

        public string Codemp
        {
            get { return Usuario.Codemp; }
        }

        public string Matric
        {
            get { return Usuario.Matric; }
        }

        public string Tipreg
        {
            get { return Usuario.Tipreg; }
        }

        public string Matemp
        {
            get { return Usuario.Matemp; }
        }

        #endregion
    }

	[Serializable]
	public class UsuarioCredenciadoVO : UsuarioLogadoVO
    {

        #region[CREDENCIADO NO ISA]

        public HcVCredenciadoVO Credenciado { get; set; }
        public ConfiguracaoIrVO ConfiguracaoIr { get; set; }

        #endregion

        #region[CREDENCIADO NO PROTHEUS]

        public PRedeAtendimentoVO RedeAtendimento { get; set; }
		public override string Username {
			get { return RedeAtendimento.Cpfcgc; }
        }

        #endregion
    }

	[Serializable]
	public class UsuarioCanalGestanteVO : UsuarioLogadoVO
    {
        #region[BENEFICIÁRIO NO ISA]

        public HcBeneficiarioVO Beneficiario { get; set; }
        public HcBeneficiarioPlanoVO BeneficiarioPlano { get; set; }
        /*public override string Username
        {
            get { return Beneficiario.CdBeneficiario.ToString(); }
        }*/

        #endregion

        #region[BENEFICIÁRIO NO PROTHEUS]

        public PUsuarioVO Usuario { get; set; }
		public PFamiliaProdutoVO FamiliaProduto { get; set; }
		public override string Username {
            get { return Usuario.Nomusr; }
        }

        #endregion
    }

	public class UsuarioNoLoginRequiredVO : UsuarioLogadoVO {

		public override string Username {
			get { return ""; }
		}
	}
}
