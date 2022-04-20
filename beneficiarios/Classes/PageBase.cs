using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using eVidaGeneralLib.VO;
using System.Collections.Generic;
using System.Text;
using eVida.Web.Security;
using eVidaGeneralLib.VO.SCL;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVida.Web.Controls;

namespace eVidaBeneficiarios.Classes {
	public abstract class PageBase : AllPageBase<UsuarioBeneficiarioVO> {

		protected void Page_Load(object sender, EventArgs e) {
			Log.Debug("Acessando " + Request.RawUrl);
			try {
				if (!IsLoginPage()) {
					if (PageHelper.CheckLogin(this)) {
						PageLoad(sender, e);
					} else {
						Session[typeof(Login).ToString()] = this.Request.RawUrl;
						Response.Redirect("~/Login.aspx", false);
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
			return this.Request.RawUrl.IndexOf("Login.aspx", StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

        public override UsuarioBeneficiarioVO GetUsuario(string codigo_beneficiario)
        {
            string[] dados_beneficiario = codigo_beneficiario.Split('|');
            string username = dados_beneficiario[0];
            string codint = dados_beneficiario[1];
            string codemp = dados_beneficiario[2];
            string matric = dados_beneficiario[3];
            string tipreg = dados_beneficiario[4];

            #region[BENEFICIÁRIO NO ISA]

            /*long cdBeneficiario = 0;

            if (!Int64.TryParse(username, out cdBeneficiario)) return null;

            HcBeneficiarioVO voh = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiario(cdBeneficiario);
            if (voh == null)
                return null;

            List<HcBeneficiarioVO> lstBeneficiarios = eVidaGeneralLib.BO.BeneficiarioBO.Instance.ListarBeneficiarios(voh.CdEmpresa, voh.CdFuncionario);
            ConfiguracaoIrVO configIr = ConfiguracaoIrBO.Instance.GetConfiguracao();

            UsuarioBeneficiarioVO uVO = new UsuarioBeneficiarioVO()
            {
                Beneficiario = voh,
                Beneficiarios = lstBeneficiarios,
                ConfiguracaoIr = configIr
            };

            if (lstBeneficiarios != null)
            {
                HcBeneficiarioVO titularVO = lstBeneficiarios.Find(x => x.TpBeneficiario.Equals(Constantes.TIPO_BENEFICIARIO_FUNCIONARIO));
                uVO.Titular = titularVO;

                HcBeneficiarioPlanoVO benefPlanoVO = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiarioPlano(voh.CdBeneficiario);
                if (benefPlanoVO != null)
                {
                    uVO.BeneficiarioPlano = benefPlanoVO;
                    uVO.Plano = LocatorDataBO.Instance.GetPlano(benefPlanoVO.CdPlanoVinculado);
                }

                HcBeneficiarioCategoriaVO benefCatVO = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiarioCategoria(voh.CdBeneficiario);
                if (benefCatVO != null)
                {
                    uVO.BeneficiarioCategoria = benefCatVO;
                }
            }*/

            #endregion

            #region[BENEFICIÁRIO NO PROTHEUS]

            PUsuarioVO vop = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
            if (vop == null)
                return null;

            List<PUsuarioVO> lstUsuarios = PUsuarioBO.Instance.ListarUsuarios(vop.Codint, vop.Codemp, vop.Matric);
            ConfiguracaoIrVO configIr = ConfiguracaoIrBO.Instance.GetConfiguracao();

            UsuarioBeneficiarioVO uVO = new UsuarioBeneficiarioVO()
            {
                Usuario = vop,
                Usuarios = lstUsuarios,
                ConfiguracaoIr = configIr
            };

            if (lstUsuarios != null)
            {
                PUsuarioVO titularVO = lstUsuarios.Find(x => x.Tipusu.Equals(PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO));
                uVO.UsuarioTitular = titularVO;

                PFamiliaProdutoVO familiaProdutoVO = PFamiliaBO.Instance.GetFamiliaProduto(vop.Codint, vop.Codemp, vop.Matric, vop.Tipreg);
                if (familiaProdutoVO != null)
                {
                    uVO.FamiliaProduto = familiaProdutoVO;
                    uVO.ProdutoSaude = PLocatorDataBO.Instance.GetProdutoSaude(familiaProdutoVO.Codpla);
                }

                PFamiliaContratoVO familiaContratoVO = PFamiliaBO.Instance.GetFamiliaContrato(vop.Codint, vop.Codemp, vop.Matric, vop.Tipreg);
                if (familiaContratoVO != null)
                {
                    uVO.FamiliaContrato = familiaContratoVO;
                }
            }

            #endregion

            return uVO;
        }

		protected DataTable GetRelatorioTable() {
			RelatorioData data = this.Session["RELATORIO"] as RelatorioData;
			DataTable sourceTable = null;
			if (data == null) {
				sourceTable = this.Session["RELATORIO"] as DataTable;
			} else {
				if (data.CanBeUsed(this.Context))
					sourceTable = data.Table;
			}
			return sourceTable;
		}

		protected void SaveRelatorioData(DataTable sourceTable) {
			this.Session["RELATORIO"] = RelatorioData.Create(sourceTable, this.Context);
		}
	}

}