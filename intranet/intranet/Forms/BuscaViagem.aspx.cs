using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms
{
    public partial class BuscaViagem : RelatorioExcelPageBase
    {
        private int CELL_SITUACAO = 5;

        protected override void PageLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //ScriptManager.GetCurrent(this).RegisterPostBackControl(btnBuscar);
                try
                {
                    dpdSituacao.Items.Add(new ListItem("TODOS", ""));
                    Array arr = Enum.GetValues(typeof(StatusSolicitacaoViagem));
                    Array.Sort(arr);
                    foreach (StatusSolicitacaoViagem i in arr)
                    {
                        dpdSituacao.Items.Add(new ListItem(SolicitacaoViagemEnumTradutor.TraduzStatus(i), ((int)i).ToString()));
                    }

                    if (!HasSpecialPermission())
                    {
                        if (UsuarioLogado.Usuario.Matricula == null)
                        {
                            this.ShowError("Seu usuário não possui matrícula associada! Contate o suporte!");
                            return;
                        }
                        txtMatricula.Text = UsuarioLogado.Usuario.Matricula.Value.ToString();
                        txtMatricula.Enabled = false;
                    }

                }
                catch (Exception ex)
                {
                    this.ShowError("Erro ao carregar a página", ex);
                }
            }
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.VIAGEM; }

        }

        private bool HasSpecialPermission()
        {
            Modulo[] modulos = new Modulo[] { Modulo.VIAGEM_COORDENADOR, Modulo.VIAGEM_DIRETORIA, Modulo.VIAGEM_FINANCEIRO, Modulo.VIAGEM_RESPONSAVEL, Modulo.VIAGEM_SECRETARIA };
            foreach (Modulo m in modulos)
            {
                if (UsuarioLogado.HasPermission(m)) return true;
            }
            return false;
        }

        private void Buscar()
        {
            string matricula = txtMatricula.Text;
            int? cdProtocolo = null;
            StatusSolicitacaoViagem? status = null;
            int iValue;

            if (!string.IsNullOrEmpty(txtProtocolo.Text))
                if (Int32.TryParse(txtProtocolo.Text, out iValue))
                {
                    cdProtocolo = iValue;
                }

            if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue))
            {
                if (Int32.TryParse(dpdSituacao.SelectedValue, out iValue))
                {
                    status = (StatusSolicitacaoViagem)iValue;
                }
            }

            DataTable dt = ViagemBO.Instance.Pesquisar(matricula, cdProtocolo, status, null);
            this.ShowPagingGrid(gdvRelatorio, dt, null);

            lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
            btnExportar.Visible = false;//dt.Rows.Count > 0;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar dados", ex);
            }

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {

        }

        private bool PodeCancelar(StatusSolicitacaoViagem status)
        {
            if (status == StatusSolicitacaoViagem.CANCELADO) return false;

            if (status == StatusSolicitacaoViagem.SOLICITACAO_PENDENTE) return true;

            if (SolicitacaoViagemEnumTradutor.IsStatusPrestacaoContas(status))
            {
                return false;
            }

            if (HasPermission(Modulo.VIAGEM_SECRETARIA) || HasPermission(Modulo.VIAGEM_FINANCEIRO))
            {
                return true;
            }
            return false;
        }

        protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                StatusSolicitacaoViagem status = (StatusSolicitacaoViagem)Convert.ToInt32(vo["id_situacao"]);

                row.Cells[CELL_SITUACAO].Text = SolicitacaoViagemEnumTradutor.TraduzStatus(status);

                LinkButton btnCancelar = (LinkButton)row.FindControl("btnCancelar");
                ImageButton btnPdf = (ImageButton)row.FindControl("btnPdf");

                btnCancelar.Visible = false;
                btnPdf.Visible = false;
                if (PodeCancelar(status))
                {
                    btnCancelar.Visible = true;
                }
                if (status == StatusSolicitacaoViagem.PRESTACAO_CONTA_CONFERIDA || status == StatusSolicitacaoViagem.PRESTACAO_CONTA_APROVADA)
                {
                    btnPdf.Visible = true;
                }
            }
        }

        protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (this.IsPagingCommand(sender, e)) return;
                Buscar();
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar dados", ex);
            }
        }

    }
}