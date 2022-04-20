using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Forms {
	public partial class PopAutorizacaoRevalidar : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int cdProtocolo;
				if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
					this.ShowError("A requisição está inválida!");

					this.btnSalvar.Visible = false;
					return;
				}
				Bind(cdProtocolo);
			}
		}

		public int Id {
			get { return (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int cdProtocolo) {
			Id = cdProtocolo;

			btnSalvar.Visible = false;
			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(cdProtocolo);
            if (vo.UsuarioTitular.Codint != UsuarioLogado.UsuarioTitular.Codint || vo.UsuarioTitular.Codemp != UsuarioLogado.UsuarioTitular.Codemp || vo.UsuarioTitular.Matric != UsuarioLogado.UsuarioTitular.Matric || vo.UsuarioTitular.Tipreg != UsuarioLogado.UsuarioTitular.Tipreg)
            {
				this.ShowError("Esta autorização não pertence a você!");
				
				return;
			}

			litProtocolo.Text = vo.Id.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);

			if (vo.Internacao) {
				tbDataProc.Visible = true;
			}
			btnSalvar.Visible = true;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			DateTime dataInternacao = DateTime.MinValue;

			if (tbDataProc.Visible) {
				if (string.IsNullOrEmpty(txtDataInternacao.Text)) {
					this.ShowError("Informe a próxima data provável de internação!");
					return;
				}
				if (!DateTime.TryParse(txtDataInternacao.Text, out dataInternacao)) {
					this.ShowError("A data de internação é inválida!");
					return;
				}
			}
			int cdProtocolo = Id;

			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(cdProtocolo);
			if (vo.Status != StatusAutorizacao.APROVADA) {
				this.ShowError("A solicitação apenas pode ser REVALIDADA se já estiver sido APROVADA!");
				return;
			} else {
				if (!AutorizacaoBO.Instance.PodeRevalidar(vo)) {
					this.ShowError("A autorização não pode ser revalidada!");
					return;
				}
				/*
				if (!vo.Internacao) {
					DateTime dataVigencia = vo.DataAutorizacao.Value.Date.AddDays(90);
					if (dataVigencia >= DateTime.Now.Date) {
						this.ShowError("A autorização possui vigência até " + dataVigencia.ToShortDateString() + ". A revalidação poderá apenas ser solicitada após esta data!");
						return;
					}
				}*/
			}

			if (dataInternacao != DateTime.MinValue) {
				vo.DataInternacao = dataInternacao;
			} else {
				vo.DataInternacao = null;
			}

			vo.Origem = OrigemAutorizacao.BENEF;
			AutorizacaoBO.Instance.Revalidar(vo);			

			this.RegisterScript("aqui", "setCallback(" + cdProtocolo + ")");
		}

	}
}