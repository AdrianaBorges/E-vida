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
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;

namespace eVidaIntranet.Forms {
	public partial class FormNegativaReanalise : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				DataTable dtAbrangencia = PLocatorDataBO.Instance.ListarItensLista(PConstantes.LISTA_ABRANGENCIA);
				dtlAreaGeografica.DataSource = dtAbrangencia;
				dtlAreaGeografica.DataBind();
				
				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador da requisição inválido!");						
					} else {
						FormNegativaVO vo = FormNegativaBO.Instance.GetById(id);
						Bind(vo);
					}
				} else {
					this.ShowError("A reanálise só pode ser feita para um formulário existente!");
					btnSalvar.Visible = false;
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.NEGATIVA; }
		}

		protected int Id {
			get { return Convert.ToInt32(ViewState["ID"]); }
			set { ViewState["ID"] = value; }
		}

		private void Bind(FormNegativaVO vo) {
			Id = vo.CodSolicitacao;
			this.litProtocolo.Text = vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO);
			this.litProtocoloAns.Text = "- PREENCHIDO APÓS SALVAR - ";

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

			txtSolicitacao.Text = vo.DescricaoSolicitacao;


			txtDataSolicitacao.Text = FormatUtil.FormatDataForm(vo.DataSolicitacao);
			if (vo.IdMotivoGlosa != null) {
				MotivoGlosaVO motivo = FormNegativaBO.Instance.GetMotivoGlosaById(vo.IdMotivoGlosa.Value);
				lblMotivo.Text = motivo.Id + " - " + motivo.Grupo + " - " + motivo.Descricao;
			}

			if (vo.Profissional != null && !string.IsNullOrEmpty(vo.Profissional.Numcr)) {
				txtNomeProfissional.Text = vo.Profissional.Nome;
                txtNroConselho.Text = vo.Profissional.Numcr ;
				txtConselho.Text = vo.Profissional.Codsig + " - " + vo.Profissional.Estado;
			}
			lblIdProtocolo.Text = vo.ProtocoloAns;

			btnPdf.Visible = false;
			if (vo.Reanalise != null) {
				BindReanalise(vo.Reanalise);
			} else {
				lblUsuario.Text = UsuarioLogado.Usuario.Nome;
				lblCargo.Text = UsuarioLogado.Usuario.Cargo;
				if (string.IsNullOrEmpty(lblCargo.Text)) {
					this.ShowError("Seu usuário não possui cargo cadastrado! Entrar em contato com o suporte!");
					btnSalvar.Visible = false;
				}
			}

			btnPdf.OnClientClick = "return openPdf(" + vo.CodSolicitacao + ");";
		}

		private void BindReanalise(FormNegativaReanaliseVO vo) {
			UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuario);
			lblUsuario.Text = usuario.Nome;
			lblCargo.Text = usuario.Cargo;
			txtDataReanalise.Text = FormatUtil.FormatDataForm(vo.DataFormulario);

			txtObsPosicionamento.Text = vo.ObservacaoParecer;
			txtJustReanalise.Text = vo.JustificativaNegativa;
			rblPosicionamento.SelectedValue = vo.Parecer == 1 ? "A" : "N";
			litProtocoloAns.Text = vo.ProtocoloAns;

			txtDevolucao.Text = vo.ObservacaoDevolucao;

			btnAprovar.Visible = this.HasPermission(eVidaGeneralLib.VO.Modulo.APROVAR_NEGATIVA);
			btnDevolver.Visible = this.HasPermission(eVidaGeneralLib.VO.Modulo.APROVAR_NEGATIVA);

			tbDevolver.Visible = true;
			if (vo.Status == FormNegativaReanaliseStatus.FINALIZADO) {
				UsuarioVO aprovador = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuarioUpdate);
				lblAprovador.Text = aprovador.Nome;
				lblCargoAprovador.Text = aprovador.Cargo;

				this.ShowInfo("A reanálise foi finalizada. Não é possível salvar as alterações!");
				btnSalvar.Visible = false;
				btnAprovar.Visible = false;
				btnPdf.Visible = true;
				tbDevolver.Visible = false;
			} else if (vo.Status == FormNegativaReanaliseStatus.DEVOLVIDO) {
				btnAprovar.Visible = false;
				btnDevolver.Visible = false;
			}
			this.btnSalvar.Text = "Salvar edição da reanálise";

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

		private bool ValidateRequireds() {
			List<ItemError> lstMsg = new List<ItemError>();
			DateTime dtValue;
			if (string.IsNullOrEmpty(rblPosicionamento.SelectedValue)) {
				AddErrorMessage(lstMsg, rblPosicionamento, "Selecione o posicionamento da reanálise!");
			}
			if (string.IsNullOrEmpty(txtJustReanalise.Text)) {
				AddErrorMessage(lstMsg, txtJustReanalise, "Informe a Justificativa da negativa de cobertura!");
			}
			if (string.IsNullOrEmpty(txtDataReanalise.Text)) {
				AddErrorMessage(lstMsg, txtDataReanalise, "Informe a data de reanálise!");
			} else if (!DateTime.TryParse(txtDataReanalise.Text, out dtValue)) {
				AddErrorMessage(lstMsg, txtDataReanalise, "A data de reanálise está inválida!");
			}
			if (lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
			}
			return lstMsg.Count == 0;
		}
		
		private void Salvar() {
			if (!ValidateRequireds())
				return;

			FormNegativaReanaliseVO vo = FillVO();

			FormNegativaBO.Instance.Salvar(vo);
			this.ShowInfo("Reanálise salva com sucesso!");

			bool erroEmail = false;
			FormNegativaVO negVO = null;
			try {
				negVO = FormNegativaBO.Instance.GetById(vo.Id);
				FormNegativaBO.Instance.EnviarEmailSolicitacao(negVO, true);
			}
			catch (Exception ex) {
				erroEmail = true;
				Log.Error("Erro ao enviar o email", ex);
			}

			if (erroEmail) {
				this.ShowInfo("Porém houve um erro ao tentar enviar o e-mail!");
			}

			try {
				Bind(negVO);
				this.SetFocus(btnPdf);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar formulário!", ex);
			}
		}

		private FormNegativaReanaliseVO FillVO() {
			FormNegativaReanaliseVO vo = new FormNegativaReanaliseVO();

			if (!string.IsNullOrEmpty(litProtocolo.Text))
				vo.Id = Int32.Parse(litProtocolo.Text);

			vo.DataFormulario = DateTime.Parse(txtDataReanalise.Text);
			vo.IdUsuario = UsuarioLogado.Id;
			vo.JustificativaNegativa = txtJustReanalise.Text;
			vo.ObservacaoParecer = txtObsPosicionamento.Text;
			vo.Parecer = rblPosicionamento.SelectedValue.Equals("A") ? 1 : 0;
			return vo;
		}

		private void Devolver() {
			string motivo = txtDevolucao.Text;
			if (string.IsNullOrEmpty(motivo)) {
				this.ShowError("Informe o motivo da devolução!");
				return;
			}

			FormNegativaVO vo = FormNegativaBO.Instance.DevolverReanalise(Id, UsuarioLogado.Id, motivo);
			this.ShowInfo("Reanálise devolvida ao elaborador!");
			this.Bind(vo);
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar o formulário!", ex);
			}
		}
		
		protected void btnAprovar_Click(object sender, EventArgs e) {
			try {
				int cdProtocolo = Id;
				FormNegativaBO.Instance.Aprovar(cdProtocolo, UsuarioLogado.Id, true);
				this.ShowInfo("Reanálise aprovada com sucesso!");
				Bind(FormNegativaBO.Instance.GetById(cdProtocolo));
			} catch (Exception ex) {
				this.ShowError("Erro ao aprovar", ex);
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

		protected void btnDevolver_Click(object sender, EventArgs e) {
			try {
				Devolver();
			} catch (Exception ex) {
				this.ShowError("Erro ao devolver reanálise!", ex);
			}
		}

	}
}