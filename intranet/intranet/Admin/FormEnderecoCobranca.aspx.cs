using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Admin
{
    public partial class FormEnderecoCobranca : FormPageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    string id = Request.QueryString["ID"].ToString();
                    Bind(id);
                }
                else
                {

                }
            }
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_ENDERECO_COBRANCA; }
        }

        private string Id
        {
            get { return ViewState["ID"] != null ? (string)ViewState["ID"] : ""; }
            set { ViewState["ID"] = value; }
        }

        private void Bind(string id)
        {
            try
            {
                PClienteVO vo = PClienteBO.Instance.GetById(id);
                if (vo == null)
                {
                    this.ShowError("Beneficiário inexistente!");
                    return;
                }
                Id = id;

                this.litCpf.Text = vo.Cgc.Trim();
                this.litNome.Text = vo.Nome.Trim();
                this.litEndereco.Text = vo.End.Trim();
                this.litNumero.Text = vo.Nrend.Trim();
                this.litComplemento.Text = vo.Complem.Trim();
                this.litBairro.Text = vo.Bairro.Trim();
                this.txtEndCob.Text = vo.Endcob.Trim();

            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao carregar dados do beneficiário! ", ex);
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Salvar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar o template!", ex);
            }
        }

        private void Salvar()
        {

            if (string.IsNullOrEmpty(txtEndCob.Text.Trim()))
            {
                this.ShowError("Informe o Endereço de Cobrança!");
                return;
            }
            if (txtEndCob.Text.Trim().Length > 60)
            {
                this.ShowError("O Endereço de Cobrança só pode ter até 60 caracteres!");
                return;
            }

            PClienteBO.Instance.AlterarEnderecoCobranca(Id, txtEndCob.Text.Trim().ToUpper());

            this.ShowInfo("Endereço de Cobrança salvo com sucesso!");
            Bind(Id);

        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Bind(Id);
        }

        protected void btnGerar_Click(object sender, EventArgs e)
        {
            txtEndCob.Text = PLocatorDataBO.Instance.ObterReducaoEndereco(litEndereco.Text.Trim(), litNumero.Text.Trim(), litComplemento.Text.Trim(), litBairro.Text.Trim());
        }

    }
}