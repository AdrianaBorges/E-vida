using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Gestao {
	public partial class AjusteAutorizacao : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_ALT_AUTORIZACAO_ISA; }
		}

		private void Clear() {
			ViewState["NR_AUTORIZACAO"] = null;
			lblInicio.Text = string.Empty;
			lblTermino.Text = string.Empty;
			lblDias.Text = string.Empty;
			lblNroAutorizacao.Text = string.Empty;

			txtInicio.Text = string.Empty;
			txtFim.Text = string.Empty;
			lblDias2.Text = string.Empty;
		}

		private void FillAutorizacao(int nroAutorizacao) {
			HcAutorizacaoVO vo = AutorizacaoIsaBO.Instance.GetById(nroAutorizacao);
			if (vo == null) {
				this.ShowError("Autorização não encontrada!");
				return;
			}
			lblNroAutorizacao.Text = vo.NrAutorizacao.ToString();

			if (vo.DtInicioAutorizacao.HasValue) {
				lblInicio.Text = vo.DtInicioAutorizacao.Value.ToShortDateString();
				txtInicio.Text = lblInicio.Text;
			} else {
				lblInicio.Text = "-";
				txtInicio.Text = string.Empty;
			}
			if (vo.DtTerminoAutorizacao.HasValue) {
				lblTermino.Text = vo.DtTerminoAutorizacao.Value.ToShortDateString();
				txtFim.Text = lblTermino.Text;
			} else {
				lblTermino.Text = "-";
				txtFim.Text = string.Empty;
			}

			lblDias.Text = vo.NrDiasAutorizados.HasValue ? vo.NrDiasAutorizados.Value.ToString() : "-";

			CalcularDiasAutorizados();
			ViewState["NR_AUTORIZACAO"] = nroAutorizacao;
		}

		private void CalcularDiasAutorizados() {
			lblDias2.Text = "-";

			DateTime dtInicio;
			DateTime dtTermino;

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtTermino)) {
				return;
			}
			if (dtTermino < dtInicio) {
				this.ShowError("A data início deve ser menor que a data fim!");
				return;
			}
			lblDias2.Text = DateUtil.DayDiff(dtTermino, dtInicio, 1).ToString();
			
		}

		private void Salvar(int nroAutorizacao, DateTime dtInicio, DateTime dtFim) {
			AutorizacaoIsaBO.Instance.AjustarDatas(nroAutorizacao, dtInicio, dtFim, UsuarioLogado.Usuario);
			this.ShowInfo("Datas alteradas com sucesso!");
			FillAutorizacao(nroAutorizacao);
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				if (ViewState["NR_AUTORIZACAO"] == null) {
					this.ShowError("Selecione uma autorização para alteração!");
					return;
				}
				int nroAutorizacao = (int)ViewState["NR_AUTORIZACAO"];
				DateTime dtInicio;
				DateTime dtFim;

				if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
					this.ShowError("Informe a data de início!");
					return;
				}

				if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
					this.ShowError("Informe a data de término!");
					return;
				}

				Salvar(nroAutorizacao, dtInicio, dtFim);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar os dados!", ex);
			}
		}

		protected void btnPesquisar_Click(object sender, EventArgs e) {
			try {
				int nroAutorizacao;

				Clear();

				if (!Int32.TryParse(txtNroAutorizacao.Text, out nroAutorizacao)) {
					this.ShowError("A autorização deve ser numérica!");
					return;
				}

				FillAutorizacao(nroAutorizacao);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar dados", ex);
			}
		}

		protected void data_TextChanged(object sender, EventArgs e) {
			TextBox txt = (TextBox)sender;
			DateTime dt;

			if (!DateTime.TryParse(txt.Text, out dt)) {
				this.ShowError("Informe uma data válida!");
			}
			CalcularDiasAutorizados();
		}
	}
}