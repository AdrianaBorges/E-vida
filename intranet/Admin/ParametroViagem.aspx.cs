using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Admin
{
    public partial class ParametroViagem : PageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind();
            }
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_VIAGEM_PARAMETRO; }
        }

        public int? IdDiaria { get { return GetViewStateInt("ID_DIARIA"); } set { ViewState["ID_DIARIA"] = value; } }
        public int? IdKm { get { return GetViewStateInt("ID_KM"); } set { ViewState["ID_KM"] = value; } }

        private int? GetViewStateInt(string key)
        {
            if (ViewState[key] == null) return null;
            return Convert.ToInt32(ViewState[key]);
        }

        private void Bind()
        {
            LimparAll();
            List<ParametroVariavelVO> lstParamDiaria = ParametroVariavelBO.Instance.GetParametroAll(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_DIARIA);
            if (lstParamDiaria != null)
            {
                gdvDiaria.DataSource = from l in lstParamDiaria orderby l.Inicio descending select l;
                gdvDiaria.DataBind();
            }

            List<ParametroVariavelVO> lstParamKm = ParametroVariavelBO.Instance.GetParametroAll(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_KM);
            if (lstParamKm != null)
            {
                gdvKm.DataSource = from l in lstParamKm orderby l.Inicio descending select l;
                gdvKm.DataBind();
            }
        }

        private void LimparAll()
        {
            LimparDiaria();
            LimparKm();
        }

        private void LimparKm()
        {
            IdKm = null;
            lblKSeq.Text = string.Empty;
            txtKInicio.Text = string.Empty;
            txtKFim.Text = string.Empty;
            txtKValor.Text = string.Empty;
            btnKSalvar.Text = "Incluir";
        }

        private void LimparDiaria()
        {
            IdDiaria = null;
            lblDSeq.Text = string.Empty;
            txtDInicio.Text = string.Empty;
            txtDFim.Text = string.Empty;
            txtDValor.Text = string.Empty;
            btnDSalvar.Text = "Incluir";
        }

        private void EditarDiaria(GridViewRow row)
        {
            LimparAll();
            int idLinha = Convert.ToInt32(gdvDiaria.DataKeys[row.RowIndex].Value);

            ParametroVariavelVO vo = ParametroVariavelBO.Instance.GetParametro(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_DIARIA, idLinha);
            lblDSeq.Text = vo.IdLinha.ToString();
            txtDInicio.Text = vo.Inicio.ToShortDateString();
            txtDFim.Text = vo.Fim.ToShortDateString();
            txtDValor.Text = vo.Value;
            IdDiaria = vo.IdLinha;
            btnDSalvar.Text = "Alterar";

        }
        private void EditarKm(GridViewRow row)
        {
            LimparAll();
            int idLinha = Convert.ToInt32(gdvKm.DataKeys[row.RowIndex].Value);

            ParametroVariavelVO vo = ParametroVariavelBO.Instance.GetParametro(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_KM, idLinha);
            lblKSeq.Text = vo.IdLinha.ToString();
            txtKInicio.Text = vo.Inicio.ToShortDateString();
            txtKFim.Text = vo.Fim.ToShortDateString();
            txtKValor.Text = vo.Value;
            IdKm = vo.IdLinha;
            btnKSalvar.Text = "Alterar";
        }

        private void SalvarKm()
        {
            Salvar(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_KM, IdKm, txtKInicio, txtKFim, txtKValor);
        }

        private void SalvarDiaria()
        {
            Salvar(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_DIARIA, IdDiaria, txtDInicio, txtDFim, txtDValor);
        }

        private void Salvar(ParametroUtil.ParametroVariavelType parametro, int? id, TextBox txtInicio, TextBox txtFim, TextBox txtValor)
        {
            DateTime inicio;
            DateTime fim;
            decimal valor;
            if (string.IsNullOrEmpty(txtInicio.Text))
            {
                this.ShowError("Informe o início da vigência!");
                return;
            }
            if (string.IsNullOrEmpty(txtFim.Text))
            {
                this.ShowError("Informe o fim da vigência!");
                return;
            }
            if (!DateTime.TryParse(txtInicio.Text, out inicio))
            {
                this.ShowError("O início está em formato inválido!");
                return;
            }
            if (!DateTime.TryParse(txtFim.Text, out fim))
            {
                this.ShowError("O fim está em formato inválido!");
                return;
            }
            if (string.IsNullOrEmpty(txtValor.Text))
            {
                this.ShowError("Informe o valor da diária!");
                return;
            }
            if (!Decimal.TryParse(txtValor.Text, out valor))
            {
                this.ShowError("O valor da diária deve ser numérico!");
                return;
            }

            ParametroVariavelVO vo = new ParametroVariavelVO();
            if (id != null) vo.IdLinha = id.Value;
            vo.ParamId = (int)parametro;
            vo.Fim = fim;
            vo.Inicio = inicio;
            vo.Value = valor.ToString();

            ParametroVariavelVO outroVO = ParametroVariavelBO.Instance.CheckConcorrencia(vo);
            if (outroVO != null)
            {
                this.ShowError("Já existe outro registro com a vigência em " + outroVO.Inicio.ToShortDateString() + " - " + outroVO.Fim.ToShortDateString());
                return;
            }

            ParametroVariavelBO.Instance.Salvar(vo, UsuarioLogado.Id);

            this.ShowInfo("Parâmetro salvo com sucesso!");
            Bind();
        }

        protected void btnDLimpar_Click(object sender, EventArgs e)
        {
            try
            {
                LimparDiaria();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao limpar formulário de Diária!", ex);
            }
        }

        protected void btnDSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarDiaria();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar valor de diária!", ex);
            }
        }

        protected void btnKLimpar_Click(object sender, EventArgs e)
        {
            try
            {
                LimparKm();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao limpar formulário de KM!", ex);
            }
        }

        protected void btnKSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarKm();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao salvar valor de KM!", ex);
            }
        }

        protected void btnDEditar_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                EditarDiaria(row);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao selecionar registro para edição!", ex);
            }

        }

        protected void btnKEditar_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                EditarKm(row);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao selecionar registro para edição!", ex);
            }
        }

    }
}