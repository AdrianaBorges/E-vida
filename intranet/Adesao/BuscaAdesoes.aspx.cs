using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;

namespace eVidaIntranet.Adesao
{
    public partial class BuscaAdesoes : RelatorioExcelPageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<ListItem> lstEmp = new List<ListItem>();
                foreach (PDados.Empresa emp in Enum.GetValues(typeof(PDados.Empresa)))
                {
                    lstEmp.Add(new ListItem(PDados.EnumTradutor.TraduzEmpresa(emp), emp.ToString()));
                }
                dpdEmpresa.Items.Add(new ListItem("SELECIONE", ""));
                dpdEmpresaResumo.Items.Add(new ListItem("TODAS", ""));

                foreach (ListItem item in lstEmp.OrderBy(item => item.Text))
                {
                    dpdEmpresaResumo.Items.Add(item);
                    dpdEmpresa.Items.Add(item);
                }

                #region[VERIFICAÇÃO DA PATROCINADORA]

                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_ELETRONORTE))
                {
                    dpdEmpresaResumo.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.ELETRONORTE)).Selected = true;
                    dpdEmpresaResumo.Enabled = false;
                    dpdEmpresa.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.ELETRONORTE)).Selected = true;
                    dpdEmpresa.Enabled = false;
                }

                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_CEA))
                {
                    dpdEmpresaResumo.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.CEA)).Selected = true;
                    dpdEmpresaResumo.Enabled = false;
                    dpdEmpresa.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.CEA)).Selected = true;
                    dpdEmpresa.Enabled = false;
                }

                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_EVIDA))
                {
                    dpdEmpresaResumo.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.EVIDA)).Selected = true;
                    dpdEmpresaResumo.Enabled = false;
                    dpdEmpresa.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.EVIDA)).Selected = true;
                    dpdEmpresa.Enabled = false;
                }

                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASD))
                {
                    dpdEmpresaResumo.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.AMAZONASD)).Selected = true;
                    dpdEmpresaResumo.Enabled = false;
                    dpdEmpresa.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.AMAZONASD)).Selected = true;
                    dpdEmpresa.Enabled = false;
                }

                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASGT))
                {
                    dpdEmpresaResumo.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.AMAZONASGT)).Selected = true;
                    dpdEmpresaResumo.Enabled = false;
                    dpdEmpresa.Items.FindByText(PDados.EnumTradutor.TraduzEmpresa(PDados.Empresa.AMAZONASGT)).Selected = true;
                    dpdEmpresa.Enabled = false;
                }

                #endregion

                dpdStatus.Items.Add(new ListItem("TODOS", ""));
                dpdStatus.Items.Add(new ListItem("PENDENTE", PDados.SituacaoDeclaracao.PENDENTE.ToString()));
                dpdStatus.Items.Add(new ListItem("RECEBIDA", PDados.SituacaoDeclaracao.RECEBIDA.ToString()));
                dpdStatus.Items.Add(new ListItem("VALIDADA", PDados.SituacaoDeclaracao.VALIDADA.ToString()));
                dpdStatus.Items.Add(new ListItem("INVALIDA", PDados.SituacaoDeclaracao.INVALIDA.ToString()));

                // Se o usuário não for da patrocinadora
                if (!UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_ELETRONORTE) &&
                    !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_CEA) &&
                    !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_EVIDA) &&
                    !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASD) &&
                    !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASGT)) 
                { 
                    // Ele pode visualizar o filtro de adesões integradas
                    dpdStatus.Items.Add(new ListItem("INTEGRADA", PDados.SituacaoDeclaracao.INTEGRADA.ToString()));

                }

                BuscarResumo();
            }
        }

        /*private bool IsFullSearch
        {
            get { return ViewState["FULL_SEARCH"] == null ? false : (bool)ViewState["FULL_SEARCH"]; }
            set { ViewState["FULL_SEARCH"] = value; }
        }*/

        protected override Modulo Modulo
        {
            get { return Modulo.ADESAO; }
        }

        protected void dpdEmpresaResumo_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuscarResumo();
        }

        private void BuscarResumo()
        {
            try
            {
                DataTable dtAcessos = PAdesaoBO.Instance.BuscarResumo();

                this.Session["rptResumo"] = dtAcessos;
                DataView view = dtAcessos.DefaultView;
                view.Sort = "ID_EMPRESA ASC, PRODUTO ASC";

                FilterResumo(dtAcessos);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar resumo", ex);
            }
        }

        private void FilterResumo(DataTable dtAcessos)
        {
            PDados.Empresa? empresa = null;
            if (!string.IsNullOrEmpty(dpdEmpresaResumo.SelectedValue))
            {
                empresa = (PDados.Empresa)Enum.Parse(typeof(PDados.Empresa), dpdEmpresaResumo.SelectedValue);
            }
            if (empresa != null)
            {
                dtAcessos.DefaultView.RowFilter = "ID_EMPRESA = '" + (empresa.Value) + "'";
            }
            gdvResumo.DataSource = dtAcessos;
            gdvResumo.DataBind();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar adesões!", ex);
            }
        }

        private void Buscar()
        {
            BuscarResumo();

            PDados.Empresa? empresa = null;
            int? numProposta = null;
            long? matricula = null;
            PDados.SituacaoDeclaracao? status = null;

            if (!string.IsNullOrEmpty(dpdEmpresa.SelectedValue))
            {
                empresa = (PDados.Empresa)Enum.Parse(typeof(PDados.Empresa), dpdEmpresa.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txtNumProposta.Text))
            {
                int iValue;
                if (!Int32.TryParse(txtNumProposta.Text, out iValue))
                {
                    this.ShowError("O número da proposta deve ser numérico!");
                    return;
                }
                numProposta = iValue;
            }
            if (!string.IsNullOrEmpty(txtMatricula.Text))
            {
                long lValue;
                if (!Int64.TryParse(txtMatricula.Text, out lValue))
                {
                    this.ShowError("A matrícula deve ser numérica!");
                    return;
                }
                matricula = lValue;
            }
            if (!string.IsNullOrEmpty(dpdStatus.SelectedValue))
            {
                status = (PDados.SituacaoDeclaracao)Enum.Parse(typeof(PDados.SituacaoDeclaracao), dpdStatus.SelectedValue);
            }

            if (empresa == null && numProposta == null && matricula == null && status == null)
            {
                this.ShowError("Informe pelo menos um filtro!");
                return;
            }
            //IsFullSearch = numProposta == null && matricula == null;

            DataTable dt = PAdesaoBO.Instance.Pesquisar(empresa, numProposta, matricula, status);
            gdvRelatorio.DataSource = dt;
            gdvRelatorio.DataBind();

            //lblObsReceber.Visible = IsFullSearch;

            if (dt == null || dt.Rows.Count == 0)
            {
                this.ShowError("Não foi encontrada nenhuma proposta com o filtro!");
            }
        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (row.RowType == DataControlRowType.DataRow)
            {

                DataRowView drV = (DataRowView)row.DataItem;
                HyperLink lnk = (HyperLink)row.Cells[9].Controls[0];
                Literal litValidada2 = (Literal)row.FindControl("litValidada2");
                Button btnValidar = (Button)row.FindControl("btnValidar");
                Button btnReceber = (Button)row.FindControl("btnReceber");
                LinkButton btnIntegrar = (LinkButton)row.FindControl("btnIntegrar");

                int status = Convert.ToInt32(drV["ID_STATUS"]);

                //lnk.NavigateUrl = ParametroUtil.UrlSiteAdesao + "/View/View.aspx?id=" + ((Guid)drV["ID_DECLARACAO"]).ToString("N") + "&TP=" + drV["ID_PRODUTO"];
                lnk.NavigateUrl = ParametroUtil.UrlSiteAdesao + "/View/View.aspx?id=" + drV["NU_DECLARACAO"] + "&TP=" + drV["ID_PRODUTO"];

                litValidada2.Text = (status == (int)PDados.SituacaoDeclaracao.INVALIDA ? "<b>INVÁLIDA</b> -" : "") + Convert.ToString(drV["DS_VALIDACAO"]);

                /*if (IsFullSearch)
                {
                    btnValidar.Visible = false;
                    btnReceber.Visible = false;
                    btnIntegrar.Visible = false;
                }
                else
                {*/
                    btnReceber.Visible = drV["DT_RECEBIDA"] == DBNull.Value;
                    btnValidar.Visible = drV["DT_RECEBIDA"] != DBNull.Value && drV["DT_VALIDADA"] == DBNull.Value;
                    btnIntegrar.Visible = (status == (int)PDados.SituacaoDeclaracao.VALIDADA) && drV["DT_VALIDADA"] != DBNull.Value && drV["DT_INTEGRACAO"] == DBNull.Value;
                //}

                PDados.Empresa empresa = (PDados.Empresa)Enum.Parse(typeof(PDados.Empresa), Convert.ToString(drV["id_empresa"]));

                // Se o usuário pertence a uma empresa patrocinadora
                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_ELETRONORTE) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_CEA) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_EVIDA) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASD) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASGT))
                {
                    // Ele só poderá visualizar as adesões da sua empresa
                    if (empresa == PDados.Empresa.ELETRONORTE && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_ELETRONORTE)) row.Visible = false;
                    else if (empresa == PDados.Empresa.CEA && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_CEA)) row.Visible = false;
                    else if (empresa == PDados.Empresa.EVIDA && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_EVIDA)) row.Visible = false;
                    else if (empresa == PDados.Empresa.AMAZONASD && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASD)) row.Visible = false;
                    else if (empresa == PDados.Empresa.AMAZONASGT && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASGT)) row.Visible = false;

                    // Ele não visualiza a coluna Data de Integração
                    gdvRelatorio.Columns[8].Visible = false;
                }
                else 
                {
                    if (!UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR))
                        btnValidar.Visible = false;                    
                }
                
                if (!UsuarioLogado.HasPermission(Modulo.ADESAO_INTEGRAR))
                    btnIntegrar.Visible = false;

                string produto = Convert.ToString(drV["ID_PRODUTO"]);
                if (!PAdesaoBO.Instance.IsValidForIntegracao(empresa, produto))
                {
                    btnIntegrar.Visible = false;
                }
            }
        }

        protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void btnReceber_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int id = Convert.ToInt32(btn.CommandArgument);
                PAdesaoBO.Instance.MarcarRecebida(id);
                this.ShowInfo("Adesão " + id + " marcada como recebida!");
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao marcar formulário como recebido!", ex);
            }
        }

        protected void btnValidar_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int id = Convert.ToInt32(btn.CommandArgument);
                PDeclaracaoVO vo = PAdesaoBO.Instance.GetById(id);

                this.ShowInfo("Adesão " + id + " marcada como validada/invalidada!");
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao marcar formulário como validado!", ex);
            }
        }

        protected void gdvResumo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drV = (DataRowView)row.DataItem;

                PDados.Empresa empresa = (PDados.Empresa)Enum.Parse(typeof(PDados.Empresa), Convert.ToString(drV["id_empresa"]));

                // Se o usuário pertence a uma empresa patrocinadora
                if (UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_ELETRONORTE) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_CEA) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_EVIDA) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASD) || UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASGT))
                {
                    // Ele só poderá visualizar o resumo da sua empresa
                    if (empresa == PDados.Empresa.ELETRONORTE && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_ELETRONORTE)) row.Visible = false;
                    else if (empresa == PDados.Empresa.CEA && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_CEA)) row.Visible = false;
                    else if (empresa == PDados.Empresa.EVIDA && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_EVIDA)) row.Visible = false;
                    else if (empresa == PDados.Empresa.AMAZONASD && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASD)) row.Visible = false;
                    else if (empresa == PDados.Empresa.AMAZONASGT && !UsuarioLogado.HasPermission(Modulo.ADESAO_VALIDAR_AMAZONASGT)) row.Visible = false;
                }
            }
        }

    }
}