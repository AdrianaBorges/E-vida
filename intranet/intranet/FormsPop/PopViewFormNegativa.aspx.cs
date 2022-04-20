using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.Util;

namespace eVidaIntranet.Forms {
	public partial class PopViewFormNegativa : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				DataTable dtAbrangencia = PLocatorDataBO.Instance.ListarItensLista(PConstantes.LISTA_ABRANGENCIA);
				dtlAreaGeografica.DataSource = dtAbrangencia;
				dtlAreaGeografica.DataBind();

				int id;
				if (!Int32.TryParse(Request["ID"], out id)) {
					this.ShowError("Identificador da requisição inválido!");
				} else {
					FormNegativaVO vo = FormNegativaBO.Instance.GetById(id);
					if (vo == null) {
						this.ShowError("Identificador inexistente!");
						return;
					}
					Bind(vo);
				}
				
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.NEGATIVA; }
		}

		private void Bind(FormNegativaVO vo) {
			this.litProtocolo.Text = vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO);
			this.litProtocoloAns.Text = vo.ProtocoloAns;

			lblCoberturaContratual.Text = (vo.InfoDispositivoLegal & 1) == 1 ? "X" : "";
			lblIndicacao.Text = (vo.InfoDispositivoLegal & 2) == 2 ? "X" : "";

			BuscarBeneficiario(vo);
			lblContrato.Text = vo.NrContrato;

			chkApartamento.Text = vo.PadraoAcomodacao.Equals("APT") ? "X" : "";
			chkEnfermaria.Text = vo.PadraoAcomodacao.Equals("ENF") ? "X" : "";

			chkRedeLivre.Text = vo.TipoRede.Equals("LIVR") ? "X" : "";
			chkRedeCred.Text = vo.TipoRede.Equals("CRED") ? "X" : "";

			if (vo.Prestador != null) {
				lblCpfCnpjPrestador.Text = FormatUtil.TryFormatCpfCnpj(vo.Prestador.Cpfcgc);
				txtNomePrestador.Text = vo.Prestador.Nome;
			}

			DataTable dt = Items2Table(vo.Itens);
			BindRows(dt);

			BindAssistencialJus(vo.JustAssistencial);
			BindContratualJus(vo.JustContratual);

			txtDataFormulario.Text = FormatUtil.FormatDataExtenso(vo.DataFormulario);
			txtPrevContratual.Text = vo.PrevisaoContratual;
			txtSolicitacao.Text = vo.DescricaoSolicitacao;

			UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuario);
			lblUsuario.Text = usuario.Nome;
			lblCargo.Text = usuario.Cargo;

			txtDataSolicitacao.Text = FormatUtil.FormatDataForm(vo.DataSolicitacao);
			if (vo.IdMotivoGlosa != null) {
				MotivoGlosaVO motivo = FormNegativaBO.Instance.GetMotivoGlosaById(vo.IdMotivoGlosa.Value);
				lblMotivo.Text = motivo.Id + " - " + motivo.Grupo + " - " + motivo.Descricao;
			}

			if (vo.Profissional != null && !string.IsNullOrEmpty(vo.Profissional.Numcr)) {
				txtNomeProfissional.Text = vo.Profissional.Nome;
                txtNroConselho.Text = vo.Profissional.Numcr;
				txtConselho.Text = vo.Profissional.Codsig + " - " + vo.Profissional.Estado;
			}

			if (vo.Status == FormNegativaStatus.APROVADO.ToString()) {
				UsuarioVO aprovador = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuarioUpdate);
				lblAprovador.Text = aprovador.Nome;
				lblCargoAprovador.Text = aprovador.Cargo;
			}
		}

		private void BuscarBeneficiario(FormNegativaVO vo) {
			FormNegativaInfoAdicionalVO infoVO = FormNegativaBO.Instance.GetInfoAdicional(vo);

			if (infoVO == null) {
				this.ShowError("Beneficiário inexistente!");
				return;
			}

			lblBeneficiario.Text = infoVO.NomeBeneficiario;
			lblCartao.Text = infoVO.Cartao;
			lblCpf.Text = infoVO.Cpf;
			lblNascimento.Text = infoVO.DataNascimento.ToString("dd/MM/yyyy");

			lblDataAdesao.Text = infoVO.DataAdesao.ToString("dd/MM/yyyy");

			lblNroRegistroProduto.Text = infoVO.Plano.Susep;
			lblNomePlano.Text = infoVO.Plano.Descri;

			foreach (DataListItem item in dtlAreaGeografica.Items) {
				HiddenField hid = item.FindControl("hid") as HiddenField;
				if (hid.Value.Equals(infoVO.Plano.Abrang)) {
					Label lbl = item.FindControl("lblCheck") as Label;
					lbl.Text = "X";
				}
			}
		}

		private void BindContratualJus(List<FormNegativaJustificativaVO> lst) {
			if (lst != null) {
				int[] needParam = new int[] { 2, 5, 7 };
				foreach (FormNegativaJustificativaVO item in lst) {
					int i = item.IdJustificativa;
					Label chk = conteudo.FindControl("chkJC" + i) as Label;
					chk.Text = "( X )";
					if (Array.IndexOf(needParam, i) >= 0) {
						Label txtData = conteudo.FindControl("txtDataJC" + i) as Label;
						txtData.Text = item.Parametros;
						txtData.Font.Bold = true;
					}
				}
			}
			for (int i = 1; i <= 7; i++) {
				Label chk = conteudo.FindControl("chkJC" + i) as Label;
				if (chk.Text.Length == 0)
					chk.Text = "(&nbsp;&nbsp;&nbsp;&nbsp;)";
				Label txt = conteudo.FindControl("txtDataJC" + i) as Label;
				if (txt != null && txt.Text.Length == 0) {
					txt.Text = "__/__/____";
				}
			}
		}

		private void BindAssistencialJus(List<FormNegativaJustificativaVO> lst) {
			if (lst != null) {
				foreach (FormNegativaJustificativaVO item in lst) {
					int i = item.IdJustificativa;
					Label chk = conteudo.FindControl("chkJA" + i) as Label;
					chk.Text = "( X )";

					if (i == 99) {
						txtJA99.Text = item.Parametros;
					}
				}
			}
			for (int i = 1; i <= 13; i++) {
				Label chk = conteudo.FindControl("chkJA" + i) as Label;
				if (chk.Text.Length == 0)
					chk.Text = "(&nbsp;&nbsp;&nbsp;&nbsp;)";
			}
			if (chkJA99.Text.Length == 0) {
				chkJA99.Text = "(&nbsp;&nbsp;&nbsp;&nbsp;)";
				txtJA99.Text = "______________________________________";
			}
		}

		private DataTable CreateTable() {
			DataTable dt = new DataTable();
			dt.Columns.Add("cdMascara", typeof(string));
			dt.Columns.Add("dsServico", typeof(string));
			dt.Columns.Add("qtd", typeof(int));
			dt.Columns.Add("obs", typeof(string));
			return dt;
		}
		
		private void BindRows(DataTable dt) {
			dtlSolicitacoes.DataSource = dt;
			dtlSolicitacoes.DataBind();
		}

		private DataTable Items2Table(List<FormNegativaItemVO> lst) {
			DataTable dt = CreateTable();
			if (lst != null) {
				foreach (FormNegativaItemVO item in lst) {
					DataRow drv = dt.NewRow();
					drv["cdMascara"] = item.Codpsa;
					drv["dsServico"] = item.Descri;
					drv["obs"] = item.Observacao;
					drv["qtd"] = item.Quantidade;
					dt.Rows.Add(drv);
				}
			}
			return dt;
		}

		private void BindItem(DataListItem item, DataRowView drv) {
			Label txtObservacao = item.FindControl("txtObservacao") as Label;
			Label txtCodigo = item.FindControl("txtCodigo") as Label;
			Label txtDescricao = item.FindControl("txtDescricao") as Label;
			Label txtQuantidade = item.FindControl("txtQuantidade") as Label;

			txtObservacao.Text = Convert.ToString(drv["obs"]);
			txtCodigo.Text = Convert.ToString(drv["cdMascara"]);
			txtDescricao.Text = Convert.ToString(drv["dsServico"]);
			txtQuantidade.Text = Convert.ToString(drv["qtd"]);

		}

		protected void dtlSolicitacoes_ItemDataBound(object sender, DataListItemEventArgs e) {
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
				DataListItem item = e.Item;
				DataRowView drv = item.DataItem as DataRowView;

				BindItem(item, drv);
			}
		}
	}
}