using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eVidaBeneficiarios.Classes.CanalGestante {
	public abstract class PageBase : AllPageBase<UsuarioCanalGestanteVO> {

		protected void Page_Load(object sender, EventArgs e) {
			Log.Debug("Acessando " + Request.RawUrl);
			try {
				if (!IsLoginPage()) {
					if (PageHelper.CheckLogin(this)) {
						PageLoad(sender, e);
					} else {
						Session[typeof(eVidaBeneficiarios.CanalGestante.Default).ToString()] = this.Request.RawUrl;
						Response.Redirect("~/CanalGestante/Default.aspx", false);
					}
				} else {
					PageLoad(sender, e);
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar página!", ex);
			}

		}

		protected abstract void PageLoad(object sender, EventArgs e);


		public override bool IsLoginPage() {
			return this.Request.RawUrl.IndexOf("CanalGestante/Default.aspx", StringComparison.InvariantCultureIgnoreCase) >= 0
				|| this.Request.RawUrl.EndsWith("CanalGestante", StringComparison.InvariantCultureIgnoreCase)
				|| this.Request.RawUrl.EndsWith("CanalGestante/", StringComparison.InvariantCultureIgnoreCase);
		}

        public override UsuarioCanalGestanteVO GetUsuario(string codigo_beneficiario)
        {
            string[] dados_beneficiario = codigo_beneficiario.Split('|');
            string username = dados_beneficiario[0];
            string codint = dados_beneficiario[1];
            string codemp = dados_beneficiario[2];
            string matric = dados_beneficiario[3];
            string tipreg = dados_beneficiario[4];

            #region[BENEFICIÁRIO NO ISA]

            int cdBeneficiario = 0;

			if (!Int32.TryParse(username, out cdBeneficiario)) return null;

			HcBeneficiarioVO beneficiario = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiario(cdBeneficiario);

			if (beneficiario == null)
				return null;

            HcBeneficiarioPlanoVO benefPlanoVO = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiarioPlano(cdBeneficiario);
			UsuarioCanalGestanteVO uVO = new UsuarioCanalGestanteVO()
			{
				Beneficiario = beneficiario,
				BeneficiarioPlano = benefPlanoVO
			};

            #endregion

            #region[BENEFICIÁRIO NO PROTHEUS]

            PUsuarioVO vop = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
            if (vop == null)
                return null;

            uVO.Usuario = vop;

            PFamiliaProdutoVO familiaProdutoVO = PFamiliaBO.Instance.GetFamiliaProduto(vop.Codint, vop.Codemp, vop.Matric, vop.Tipreg);
            if (familiaProdutoVO != null)
            {
                uVO.FamiliaProduto = familiaProdutoVO;
            }

            #endregion

            return uVO;
		}

	}
}