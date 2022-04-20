using System;
using System.Web.Security;
using System.Globalization;
using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO;

namespace eVidaBeneficiarios.controls {
	public partial class Header : UserControlBase {
		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				UsuarioBeneficiarioVO uVO = UsuarioLogado;
				if (uVO != null) {
					divHeaderInfo2.Visible = false;

                    PUsuarioVO u = uVO.Usuario;
                    ltNome.Text = u.Nomusr + " (" + u.Matant + ")";
                    string strPlano = "SEM PLANO ATUAL";
                    string strVigenciaPlano = "SEM PLANO ATUAL";
                    string strCategoria = "SEM CATEGORIA";
                    string strVigenciaCategoria = "SEM CATEGORIA";

                    if (uVO.ProdutoSaude != null)
                    {
                        strPlano = uVO.ProdutoSaude.Descri + " (" + uVO.ProdutoSaude.Codigo + ")";
                    }

                    if (uVO.FamiliaProduto != null)
                    {
                        strVigenciaPlano = DateTime.ParseExact(uVO.FamiliaProduto.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString() + " - ";
                        if (!string.IsNullOrEmpty(uVO.FamiliaProduto.Datblo.Trim()))
                            strVigenciaPlano += DateTime.ParseExact(uVO.FamiliaProduto.Datblo, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                    }

                    if (uVO.FamiliaContrato != null)
                    {
                        strCategoria = uVO.FamiliaContrato.Descri;
                        strVigenciaCategoria = DateTime.ParseExact(uVO.FamiliaContrato.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString() + " - ";
                        if (!string.IsNullOrEmpty(uVO.FamiliaContrato.Datblo.Trim()))
                            strVigenciaCategoria += DateTime.ParseExact(uVO.FamiliaContrato.Datblo, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                    }

                    PGrupoEmpresaVO empresa = PLocatorDataBO.Instance.ListarEmpresas().Find(x => x.Codigo.Trim().ToUpper() == u.Codemp.Trim().ToUpper());
                    if (empresa != null)
                        ltEmpresa.Text = empresa.Descri + " (" + empresa.Nreduz + ")";

                    ltPlano.Text = strPlano;
                    ltVigenciaPlano.Text = strVigenciaPlano;
                    ltCategoria.Text = strCategoria;
                    ltVigenciaCategoria.Text = strVigenciaCategoria;


				}
				else {
					lnkInicial.Enabled = false;
					divHeaderInfo.Visible = false;
				}
			}
		}

		protected void btnSair_Click(object sender, EventArgs e) {
			PageHelper.DoLogout(Session);
			Response.Redirect("~/Login.aspx");
		}
	}
}