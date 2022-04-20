using eVida.Web.Controls;
using eVida.Web.Security;
using eVidaBeneficiarios.Classes.CanalGestante;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace eVidaBeneficiarios.CanalGestante {
	public partial class Default : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<PProdutoSaudeVO> lstPlanos = PLocatorDataBO.Instance.ListarProdutoSaude();
				var datasource = from x in lstPlanos.Where(x => x.Tipo.Equals("3"))
								 select new {
									 x.Codigo,
									 x.Descri,									 
									 DisplayField = String.Format("({0}) {1}", x.Codigo, x.Descri)
								 };
				dpdPlano.DataSource = datasource;
				dpdPlano.DataBind();
				dpdPlano.Items.Insert(0, new ListItem("SELECIONE", ""));

				string nextUrl = Session[typeof(eVidaBeneficiarios.CanalGestante.Default).ToString()] as string;
				if (nextUrl == null)
					lblLoginExpirado.Visible = false;
				else {
					Session["NEXT_URL"] = nextUrl;
					lblLoginExpirado.Visible = true;
				}
			}
		}

		protected void btnLogin_Click(object sender, EventArgs e) {
			try {
				string cartao = txtCartao.Text;
				string strDataNascimento = txtNascimento.Text;				
				DateTime dataNascimento;

				lblLoginExpirado.Visible = false;

				if (string.IsNullOrEmpty(cartao)) {
					this.ShowError("O número do cartão é obrigatório!");
					return;
				}

				if (string.IsNullOrEmpty(strDataNascimento)) {
					this.ShowError("A data de nascimento é obrigatória!");
					return;
				} else if (!DateTime.TryParse(strDataNascimento, out dataNascimento)){
					this.ShowError("Data de nascimento inválida!");
					return;
				}

				if (string.IsNullOrEmpty(dpdPlano.SelectedValue)) {
					this.ShowError("Informe o seu plano!");
					return;
				}
				string plano = dpdPlano.SelectedValue;

                #region[BENEFICIÁRIO NO ISA]

                /*HcBeneficiarioVO voh = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiarioByCartao(cartao);
                if (voh == null)
                {
                    this.ShowError("Beneficiário não encontrado!");
                    return;
                }
                if (voh.DtNascimento == null)
                {
                    this.ShowError("Dados do beneficiário incompleto. Contate o setor de cadastro!");
                    return;
                }
                if (voh.DtNascimento.Value != dataNascimento)
                {
                    this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
                    return;
                }

                HcBeneficiarioPlanoVO benefPlanoVO = eVidaGeneralLib.BO.BeneficiarioBO.Instance.GetBeneficiarioPlano(voh.CdBeneficiario);
                if (benefPlanoVO == null)
                {
                    this.ShowError("Não foi possível validar o plano do beneficiário!");
                    return;
                }
                if (!benefPlanoVO.CdPlanoVinculado.Equals(plano))
                {
                    this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
                    return;
                }*/

                HcBeneficiarioVO voh = new HcBeneficiarioVO();
                HcBeneficiarioPlanoVO benefPlanoVO = new HcBeneficiarioPlanoVO();

                #endregion

                #region[BENEFICIÁRIO NO PROTHEUS]

                PUsuarioVO vop = PUsuarioBO.Instance.GetUsuarioByCartao(cartao);
				if (vop == null) {
					this.ShowError("Beneficiário não encontrado!");
					return;
				}
				if (vop.Datnas == null) {
					this.ShowError("Dados do beneficiário incompleto. Contate o setor de cadastro!");
					return;
				}
				if (vop.Datnas != dataNascimento.ToString("yyyyMMdd", CultureInfo.InvariantCulture)) {
					this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
					return;
				}

                PFamiliaProdutoVO familiaProdutoVO = PFamiliaBO.Instance.GetFamiliaProduto(vop.Codint, vop.Codemp, vop.Matric, vop.Tipreg);
                if (familiaProdutoVO == null)
                {
					this.ShowError("Não foi possível validar o plano do beneficiário!");
					return;
				}
                if (!familiaProdutoVO.Codpla.Equals(plano))
                {
					this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
					return;
                }

                #endregion

                UsuarioCanalGestanteVO uVO = new UsuarioCanalGestanteVO();
				uVO.Beneficiario = voh;
				uVO.BeneficiarioPlano = benefPlanoVO;
                uVO.Usuario = vop;
                uVO.FamiliaProduto = familiaProdutoVO;

				PageHelper.SaveAuthentication(uVO, this.Session, this.Response, false);
				string nextUrl = Session["NEXT_URL"] as string;
				if (nextUrl == null) {
					nextUrl = "~/CanalGestante/CanalGestante.aspx";
				}
				Response.Redirect(nextUrl, false);
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar login!", ex);
			}
		}
	}
}