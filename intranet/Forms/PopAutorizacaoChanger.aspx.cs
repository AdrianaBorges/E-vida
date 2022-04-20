using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Forms {
	public partial class PopAutorizacaoChanger : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int cdProtocolo;
				string tipo = Request["TIPO"];
				if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
					this.ShowError("A requisição está inválida!");
					return;
				}
				if (string.IsNullOrEmpty(tipo)) {
					this.ShowError("Tipo de requisição inválido!");
					return;
				}
				if (tipo.Equals("SOL_DOC")) {
					ShowSolDoc(cdProtocolo);
				} else if (tipo.Equals("CANCEL")) {
					ShowCancel(cdProtocolo);
				} else if (tipo.Equals("NEGAR")) {
					ShowNegar(cdProtocolo);
				} else {
					this.ShowError("Tipo de requisição desconhecido!");
					return;
				}
				IdAutorizacao = cdProtocolo;
				Tipo = tipo;
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO; }
		}

		private int IdAutorizacao {
			get { return (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private string Tipo {
			get { return (string)ViewState["TIPO"]; }
			set { ViewState["TIPO"] = value; }
		}

		private void ShowSolDoc(int cdProtocolo) {
			tbSolDoc.Visible = true;
			litProtocoloSolDoc.Text = cdProtocolo.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);
		}

		private void ShowCancel(int cdProtocolo) {
			tbCancel.Visible = true;
			litProtocoloCancel.Text = cdProtocolo.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);

		}

		private void ShowNegar(int cdProtocolo) {
			tbNegar.Visible = true;
			litProtocoloNegar.Text = cdProtocolo.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);

			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(cdProtocolo);
			if (vo.Origem != OrigemAutorizacao.CRED) {
				trNegBenef.Visible = true;
				lblBeneficiario.Text = vo.Usuario.Nomusr;
                List<FormNegativaVO> lstNegs = FormNegativaBO.Instance.ListByBeneficiario(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);
				if (lstNegs != null)
					lstNegs = lstNegs.FindAll(x => x.Status == FormNegativaStatus.APROVADO.ToString());
				gdvNegativa.DataSource = lstNegs;
				gdvNegativa.DataBind();

				if (lstNegs == null || lstNegs.Count == 0) {
					this.ShowError("Não existem negativas aprovadas no sistema associada ao beneficiário da autorização!");
					btnNegar.Visible = false;
					return;
				}
			} else {
				trNegCred.Visible = true;
				lblCredenciado.Text = vo.RedeAtendimento.Nome;
				btnNegar.Visible = true;
			}
		}

		private bool EnviarSolDoc() {
			if (string.IsNullOrEmpty(txtConteudoSolDoc.Text)) {
				this.ShowError("Informe o conteúdo para e-mail!");
				return false;
			}
			AutorizacaoBO.Instance.SolicitarDocumento(IdAutorizacao, txtConteudoSolDoc.Text, UsuarioLogado.Id);
			return true;
		}

		private bool Cancelar() {
			if (string.IsNullOrEmpty(txtCancel.Text)) {
				this.ShowError("Informe o motivo de cancelamento!");
				return false;
			}
			AutorizacaoBO.Instance.Cancelar(IdAutorizacao, txtCancel.Text, UsuarioLogado.Id);
			return true;
		}

		private bool Negar() {
			int? idNeg = null;
			byte[] anexo = null;
			if (trNegBenef.Visible) {
				if (string.IsNullOrEmpty(hidNeg.Value)) {
					this.ShowError("Selecione a negativa!");
					return false;
				}
				idNeg = Int32.Parse(hidNeg.Value);

				if (ParametroUtil.EmailEnabled) {
					ReportNegativa rpt = new ReportNegativa(ReportDir, UsuarioLogado);
					FormNegativaVO vo = FormNegativaBO.Instance.GetById(idNeg.Value);
					anexo = rpt.GerarRelatorio(vo);
				}
			}
			AutorizacaoBO.Instance.Negar(IdAutorizacao, idNeg, UsuarioLogado.Id, anexo);
			return true;
		}

		protected void btnEnviar_Click(object sender, EventArgs e) {
			string tipo = Tipo;
			try {
				bool ok = false;
				if (tipo.Equals("SOL_DOC")) {
					ok = EnviarSolDoc();
				} else if (tipo.Equals("CANCEL")) {
					ok = Cancelar();
				} else if (tipo.Equals("NEGAR")) {
					ok = Negar();
				}
				if (ok)
					this.RegisterScript("aqui", "setSolicitacao(" + IdAutorizacao + ")");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar", ex);
			}
		}
	}
}