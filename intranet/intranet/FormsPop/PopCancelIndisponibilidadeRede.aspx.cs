using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Forms {
	public partial class PopCancelIndisponibilidadeRede : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");

				this.btnSalvar.Visible = false;
				return;
			}
			litProtocolo.Text = cdProtocolo.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO);
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INDISPONIBILIDADE_REDE; }
		}

        protected void dpdProcedencia_change(object sender, EventArgs e)
        {
            txtDataAtendimento.Text = "";

            if (dpdProcedencia.SelectedValue == "P")
            {
                this.lblDataAtendimento.Visible = true;
                this.txtDataAtendimento.Visible = true;
            }
            else
            {
                this.lblDataAtendimento.Visible = false;
                this.txtDataAtendimento.Visible = false;
            }
        }

        protected void data_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            DateTime dt;

            if (!DateTime.TryParse(txt.Text, out dt))
            {
                this.ShowError("Informe uma data válida!");
            }
        }

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtCancelamento.Text)) {
				this.ShowError("Informe o motivo de cancelamento ou finalização!");
				return;
			}

            if (string.IsNullOrEmpty(dpdProcedencia.SelectedValue))
            {
                this.ShowError("Informe a procedência!");
                return;
            }

            DateTime dtAtendimento = DateTime.MinValue;
            if (dpdProcedencia.SelectedValue == "P")
            {
                if (txtDataAtendimento.Text.Trim() == "")
                {
                    this.ShowError("Informe a Data de Atendimento!");
                    return;
                }

                if (!DateTime.TryParse(txtDataAtendimento.Text, out dtAtendimento))
                {
                    this.ShowError("A Data de Atendimento informada não é válida!");
                    return;
                }
            }

    		int id = Int32.Parse(Request["ID"]);
			IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(id);

            if (string.IsNullOrEmpty(vo.EmailContato.Trim()))
            {
                this.ShowError("O e-mail de contato da solicitação não está preenchido!");
                return;
            }	

			if (vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO)
				this.RegisterScript("aqui", "setCancelamento(" + id + ")");

			vo.SetorEncaminhamento = EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO;
			vo.MotivoEncerramento = txtCancelamento.Text;
            vo.Procedencia = dpdProcedencia.SelectedValue;

            if (vo.Procedencia == "P")
            {
                vo.DataAtendimento = dtAtendimento;
                vo.CodUsuarioAtendente = UsuarioLogado.Id;
            }
            else 
            {
                vo.DataAtendimento = (DateTime?)null;
                vo.CodUsuarioAtendente = (int?)null;
            }

            vo.DataAtendimento = txtDataAtendimento.Text != "" ? DateTime.Parse(txtDataAtendimento.Text) : (DateTime?)null;
			IndisponibilidadeRedeBO.Instance.Encerrar(vo, UsuarioLogado.Id);
			this.RegisterScript("aqui", "setCancelamento(" + id + ")");
		}
	}
}