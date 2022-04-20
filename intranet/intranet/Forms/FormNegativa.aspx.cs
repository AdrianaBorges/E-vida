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
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms {
	public partial class FormNegativa : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				DataTable dtAbrangencia = PLocatorDataBO.Instance.ListarItensLista(PConstantes.LISTA_ABRANGENCIA);
				rblAreaGeografica.DataSource = dtAbrangencia;
				rblAreaGeografica.DataBind();
				rblAreaGeografica.Enabled = false;

				dpdTipoConselho.DataSource = PLocatorDataBO.Instance.ListarConselhoProfissional();
				dpdTipoConselho.DataBind();
				dpdTipoConselho.Items.Insert(0, new ListItem("SELECIONE", ""));

				dpdUfConselho.DataSource = PLocatorDataBO.Instance.ListarUf();
				dpdUfConselho.DataBind();
				dpdUfConselho.Items.Insert(0, new ListItem("SELECIONE", ""));

				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador da requisição inválido!");						
					} else {
						FormNegativaVO vo = FormNegativaBO.Instance.GetById(id);
						Bind(vo);
						this.btnSalvar.Text = "Salvar edição do formulário";
					}
				} else {
					lblUsuario.Text = UsuarioLogado.Usuario.Nome;
					lblCargo.Text = UsuarioLogado.Usuario.Cargo;
					rblAcomodacao.SelectedValue = FormNegativaVO.ACOMODACAO_APARTAMENTO;

					if (string.IsNullOrEmpty(lblCargo.Text)) {
						this.ShowError("O seu cargo não está cadastrado no sistema! Por favor entre em contato com os administradores!");
						this.btnBuscarBeneficiario.Visible = false;
						this.btnAdicionarItem.Visible = false;
						this.btnSalvar.Enabled = false;
						this.conteudo.Style.Add("display", "none");
					}
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.NEGATIVA; }
		}

		protected int? Id {
			get { return ViewState["ID"] != null ? Convert.ToInt32(ViewState["ID"]) : new int?(); }
			set { ViewState["ID"] = value; }
		}

		private void Bind(FormNegativaVO vo) {
			Id = vo.CodSolicitacao;

            this.hidCodBeneficiario.Value = vo.Codint.Trim() + "|" + vo.Codemp.Trim() + "|" + vo.Matric.Trim() + "|" + vo.Tipreg.Trim();
			this.litProtocolo.Text = vo.CodSolicitacao.ToString("000000000");
			this.litProtocoloAns.Text = vo.ProtocoloAns;

			chkCoberturaContratual.Checked = (vo.InfoDispositivoLegal & 1) == 1;
			chkIndicacao.Checked = (vo.InfoDispositivoLegal & 2) == 2;

			BuscarBeneficiario(vo);
			btnBuscarBeneficiario.Visible = false;

			rblAcomodacao.SelectedValue = vo.PadraoAcomodacao;
			rblTipoRede.SelectedValue = vo.TipoRede;

			DataTable dt = Items2Table(vo.Itens);
			BindRows(dt);

			BindAssistencialJus(vo.JustAssistencial);
			BindContratualJus(vo.JustContratual);

			txtDataFormulario.Text = vo.DataFormulario.ToString("dd/MM/yyyy");
			txtPrevContratual.Text = vo.PrevisaoContratual;
			txtSolicitacao.Text = vo.DescricaoSolicitacao;
			hidCredenciado.Value = vo.Prestador.Codigo;
			
			OnChangeTipoRede();
			if (vo.Prestador != null) {
				if (vo.Prestador.Cpfcgc.Trim() != "") {
					string strCpfCnpj = vo.Prestador.Cpfcgc;
					txtCpfCnpjPrestador.Text = strCpfCnpj;					
				} else {
					txtCpfCnpjPrestador.Text = "";
				}

				txtNomePrestador.Text = vo.Prestador.Nome;
				if (vo.Prestador.Codigo != "") {
					hidCredenciado.Value = vo.Prestador.Codigo;
				}
			}

			txtContrato.Text = vo.NrContrato;

			txtDataSolicitacao.Text = FormatUtil.FormatDataForm(vo.DataSolicitacao);
			if (vo.IdMotivoGlosa != null) {
				hidMotivo.Value = txtCodigoMotivo.Text = vo.IdMotivoGlosa.ToString();
				BindMotivo(vo.IdMotivoGlosa.Value);
			} else {
				txtCodigoMotivo.Text = hidMotivo.Value = string.Empty;
			}

			hidProfissional.Value = string.Empty;

			if (vo.Profissional != null) {
				PProfissionalSaudeVO prof = PLocatorDataBO.Instance.GetProfissional(vo.Profissional.Numcr, vo.Profissional.Estado, vo.Profissional.Codsig);
				txtNomeProfissional.Text = vo.Profissional.Nome;
                txtNroConselho.Text = vo.Profissional.Numcr;
                dpdTipoConselho.SelectedValue = vo.Profissional.Codsig.Trim();
                dpdUfConselho.SelectedValue = vo.Profissional.Estado;
				if (prof != null) {
					txtNomeProfissional.Enabled = false;
					txtNroConselho.Enabled = false;
					dpdTipoConselho.Enabled = false;
					dpdUfConselho.Enabled = false;
                    hidProfissional.Value = vo.Profissional.Numcr + ";" + vo.Profissional.Codsig + ";" + vo.Profissional.Estado;
				}
			}

			UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuario);
			lblUsuario.Text = usuario.Nome;
			lblCargo.Text = usuario.Cargo;

			btnPrint.Visible = btnPdf.Visible = false;
			btnAprovar.Visible = this.HasPermission(eVidaGeneralLib.VO.Modulo.APROVAR_NEGATIVA);
			if (vo.Status == FormNegativaStatus.CANCELADO.ToString()) {
				this.ShowInfo("O formulário foi cancelado. Não é possível salvar as alterações!");
				btnSalvar.Visible = false;
				btnAprovar.Visible = false;
			} else if (vo.Status == FormNegativaStatus.APROVADO.ToString()) {
				this.ShowInfo("O formulário foi aprovado. Não é possível salvar as alterações!");
				btnSalvar.Visible = false;
				btnPrint.Visible = btnPdf.Visible = true;
				btnAprovar.Visible = false;
			}

			btnPdf.OnClientClick = "return openPdf(" + vo.CodSolicitacao + ");";
			btnPrint.OnClientClick = "return openView(" + vo.CodSolicitacao + ");";
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

			if (infoVO.Plano != null) {
				lblNroRegistroProduto.Text = infoVO.Plano.Susep;
				ltNomePlano.Text = infoVO.Plano.Descri;
				rblAreaGeografica.SelectedValue = infoVO.Plano.Abrang;
			}
			txtContrato.Text = infoVO.NrContrato;

		}

		private bool ValidateRequireds() {
			List<ItemError> lstMsg = new List<ItemError>();
			DateTime dtValue;
			if (!chkCoberturaContratual.Checked && !chkIndicacao.Checked) {
				AddErrorMessage(lstMsg, chkCoberturaContratual, "Selecione pelo menos um motivo do dispositivo legal!");
			}
			if (string.IsNullOrEmpty(hidCodBeneficiario.Value)) {
				AddErrorMessage(lstMsg, btnBuscarBeneficiario, "Selecione o beneficiário!");
			}
			if (string.IsNullOrEmpty(rblAcomodacao.SelectedValue)) {
				AddErrorMessage(lstMsg, rblAcomodacao, "Informe o padrão de acomodação!");
			}

			if (string.IsNullOrEmpty(rblTipoRede.SelectedValue)) {
				AddErrorMessage(lstMsg, rblAcomodacao, "Informe o tipo da rede nos dados de solicitação!");
			}
			if (!DateTime.TryParse(txtDataFormulario.Text, out dtValue)) {
				AddErrorMessage(lstMsg, txtDataFormulario, "Informe a data do formulário!");
			}

			if (string.IsNullOrEmpty(hidProfissional.Value)) {
				if (string.IsNullOrEmpty(txtNroConselho.Text)) {
					this.AddErrorMessage(lstMsg, txtNroConselho, "Informe o número de conselho do profissional!");
				}
				if (string.IsNullOrEmpty(dpdTipoConselho.SelectedValue)) {
					this.AddErrorMessage(lstMsg, dpdTipoConselho, "Informe o tipo de conselho!");
				}
				if (string.IsNullOrEmpty(dpdUfConselho.SelectedValue)) {
					this.AddErrorMessage(lstMsg, dpdUfConselho, "Informe a UF do conselho!");
				}

				if (string.IsNullOrEmpty(txtNomeProfissional.Text)) {
					this.AddErrorMessage(lstMsg, txtNomeProfissional, "Informe o nome do profissional!");
				}
			}

			if ("CRED".Equals(rblTipoRede.SelectedValue)) {
				if (string.IsNullOrEmpty(hidCredenciado.Value)) {
					this.AddErrorMessage(lstMsg, btnLocPrestador, "Informe o prestador da REDE CREDENCIADA!");
				}
			} else {
				if (string.IsNullOrEmpty(txtNomePrestador.Text)) {
					AddErrorMessage(lstMsg, txtNomePrestador, "Informe a razão social ou o nome do prestador!");
				}
				long l;
				if (!Int64.TryParse(FormatUtil.UnformatCnpj(txtCpfCnpjPrestador.Text), out l)) {
					AddErrorMessage(lstMsg, txtCpfCnpjPrestador, "O CPF/CNPJ do Prestador deve ser numérico!");
				}
			}
			if (string.IsNullOrEmpty(txtContrato.Text)) {
				AddErrorMessage(lstMsg, txtContrato, "Informe o contrato do beneficiário!");
			}

			if (!DateTime.TryParse(txtDataSolicitacao.Text, out dtValue)) {
				AddErrorMessage(lstMsg, txtDataSolicitacao, "Informe a data de solicitação!");
			}

            if (dtlSolicitacoes.Items.Count == 0)
            {
                AddErrorMessage(lstMsg, dtlSolicitacoes, "Informe pelo menos um procedimento ou item assistencial!");
            }                 
            
            if (string.IsNullOrEmpty(hidMotivo.Value)) {
				AddErrorMessage(lstMsg, txtCodigoMotivo, "Informe o motivo de negativa!");
			}

			if (lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
			}
			return lstMsg.Count == 0;
		}
        
		#region Justificativas

		private void BindContratualJus(List<FormNegativaJustificativaVO> lst) {
			if (lst == null)
				return;
			int[] needParam = new int[] { 2, 5, 7 };
			foreach (FormNegativaJustificativaVO item in lst) {
				int i = item.IdJustificativa;
				CheckBox chk = conteudo.FindControl("chkJC" + i) as CheckBox;
				chk.Checked = true;
				if (Array.IndexOf(needParam, i) >= 0) {
					TextBox txtData = conteudo.FindControl("txtDataJC" + i) as TextBox;
					txtData.Text = item.Parametros;
				}
			}
		}

		private List<FormNegativaJustificativaVO> RetrieveContratualJus() {
			List<FormNegativaJustificativaVO> lst = new List<FormNegativaJustificativaVO>();
			DateTime dtValue;
			List<ItemError> lstMsg = new List<ItemError>();

			int[] needParam = new int[] { 2, 5, 7 };
			for (int i = 1; i <= 7; ++i) {
				CheckBox chk = conteudo.FindControl("chkJC" + i) as CheckBox;
				
				if (chk.Checked) {
					string param = null;
					if (Array.IndexOf(needParam, i) >= 0) {
						TextBox txtData = conteudo.FindControl("txtDataJC" + i) as TextBox;
						if (!DateTime.TryParse(txtData.Text, out dtValue)) {
							AddErrorMessage(lstMsg, txtData, "Informe uma data correta para a Justificativa Contratual " + i + "!");
						} else {
							param = dtValue.ToString("dd/MM/yyyy");
						}
					}
					lst.Add(new FormNegativaJustificativaVO() { IdJustificativa = i, Parametros = param });
				}
			}
			if (lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
				return null;
			}
			return lst;
		}

		private void BindAssistencialJus(List<FormNegativaJustificativaVO> lst) {
			if (lst == null)
				return;
			foreach (FormNegativaJustificativaVO item in lst) {
				int i = item.IdJustificativa;
				CheckBox chk = conteudo.FindControl("chkJA" + i) as CheckBox;
				chk.Checked = true;

				if (i == 99) {
					txtJA99.Text = item.Parametros;
				}
			}
		}

		private List<FormNegativaJustificativaVO> RetrieveAssistencialJus() {
			List<FormNegativaJustificativaVO> lst = new List<FormNegativaJustificativaVO>();
			
			List<ItemError> lstMsg = new List<ItemError>();
			
			for (int i = 1; i <= 13; ++i) {
				CheckBox chk = conteudo.FindControl("chkJA" + i) as CheckBox;

				if (chk.Checked) {
					lst.Add(new FormNegativaJustificativaVO() { IdJustificativa = i });
				}
			}
			if (chkJA99.Checked) {
				if (string.IsNullOrEmpty(txtJA99.Text)) {
					AddErrorMessage(lstMsg, txtJA99, "Informe o texto da justificativa assistencial Outros!");
				}
				lst.Add(new FormNegativaJustificativaVO() { IdJustificativa = 99, Parametros = txtJA99.Text });
			}
			if (lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
				return null;
			}
			return lst;
		}

		#endregion

		private void Salvar() {
			if (!ValidateRequireds())
				return;

			DataTable dtItems = RetrieveItemsFromDataList();
			if (!ValidateRows(dtItems))
				return;

			List<FormNegativaJustificativaVO> lstJusContratual = RetrieveContratualJus();
			if (lstJusContratual == null)
				return;

			List<FormNegativaJustificativaVO> lstJusAssistencial = RetrieveAssistencialJus();
			if (lstJusAssistencial == null)
				return;

			if (lstJusAssistencial.Count == 0 && lstJusContratual.Count == 0) {
				this.ShowError("Informe pelo menos uma Justificativa!");
				return;
			}

			FormNegativaVO vo = FillVO();
			List<FormNegativaItemVO> lstItems = Table2Items(dtItems);

			vo.Itens = lstItems;
			vo.JustAssistencial = lstJusAssistencial;
			vo.JustContratual = lstJusContratual;

			if (!ValidateUtil.IsValidLength(vo.DescricaoSolicitacao, 1000)) {
				this.ShowError("A descrição de solicitação deve ter no máximo 1000 caracteres!");
				return;
			}
			if (!ValidateUtil.IsValidLength(vo.PrevisaoContratual, 2000)) {
				this.ShowError("A previsão contratual deve ter no máximo 2000 caracteres!");
				return;
			}
			if (!ValidateUtil.IsValidLength(vo.Prestador.Nome, 255)) {
				this.ShowError("O nome do prestador deve ter no máximo 255 caracteres!");
				return;
			}
			if (lstItems != null) {
				foreach (FormNegativaItemVO item in lstItems) {
					if (!ValidateUtil.IsValidLength(item.Observacao, 2000)) {
						this.ShowError("A observação do item '" + item.Codpsa + "' deve ter no máximo 2000 caracteres");
						return;
					}
				}
			}
			if (lstJusContratual != null) {
				foreach (FormNegativaJustificativaVO just in lstJusContratual) {
					if (!ValidateUtil.IsValidLength(just.Parametros, 3500)) {
						this.ShowError("Os parametros para a justificativa contratual deve ter no máximo 3500 caracteres!");
						return;
					}
				}
			}
				
			if (lstJusAssistencial != null) {
				foreach (FormNegativaJustificativaVO just in lstJusAssistencial) {
					if (!ValidateUtil.IsValidLength(just.Parametros, 3500)) {
						this.ShowError("Os parametros para a justificativa assistencial deve ter no máximo 3500 caracteres!");
						return;
					}
				}
			}

			FormNegativaBO.Instance.Salvar(vo);
			litProtocolo.Text = vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO);
			this.ShowInfo("Formulário salvo com sucesso!");

			bool erroEmail = false;
			try {
				FormNegativaBO.Instance.EnviarEmailSolicitacao(vo, false);
			}
			catch (Exception ex) {
				erroEmail = true;
				Log.Error("Erro ao enviar o email", ex);
			}

			if (erroEmail) {
				this.ShowInfo("Porém houve um erro ao tentar enviar o e-mail!");
			}

			try {
				vo = FormNegativaBO.Instance.GetById(vo.CodSolicitacao);
				Bind(vo);
				this.SetFocus(btnPrint);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao carregar formulário!", ex);
			}
		}

		private FormNegativaVO FillVO() {
			FormNegativaVO vo = new FormNegativaVO();

			if (!string.IsNullOrEmpty(litProtocolo.Text))
				vo.CodSolicitacao = Int32.Parse(litProtocolo.Text);

            string[] dados_codigo = hidCodBeneficiario.Value.Split('|');
            vo.Codint = dados_codigo[0];
            vo.Codemp = dados_codigo[1];
            vo.Matric = dados_codigo[2];
            vo.Tipreg = dados_codigo[3];
			vo.DataFormulario = DateTime.Parse(txtDataFormulario.Text);
			vo.DescricaoSolicitacao = txtSolicitacao.Text;
			vo.IdUsuario = UsuarioLogado.Id;

			int infoLegal = 0;
			if (chkCoberturaContratual.Checked)
				infoLegal += 1;
			if (chkIndicacao.Checked)
				infoLegal += 2;

			vo.InfoDispositivoLegal = infoLegal;

			vo.PadraoAcomodacao = rblAcomodacao.SelectedValue;
			vo.PrevisaoContratual = txtPrevContratual.Text;
			vo.TipoRede = rblTipoRede.SelectedValue;

			vo.Prestador = new PRedeAtendimentoVO();
			vo.Prestador.Nome = txtNomePrestador.Text;
			if (!string.IsNullOrEmpty(txtCpfCnpjPrestador.Text))
				vo.Prestador.Cpfcgc = FormatUtil.UnformatCnpj(txtCpfCnpjPrestador.Text);
			if (!string.IsNullOrEmpty(hidCredenciado.Value))
				vo.Prestador.Codigo = hidCredenciado.Value;

			if (!string.IsNullOrEmpty(hidProfissional.Value)) {
				vo.Profissional = BindFromHidProfissional();
			} else {
                PProfissionalSaudeVO profissional = new PProfissionalSaudeVO()
                {
                    Nome = txtNomeProfissional.Text,
                    Codsig = dpdTipoConselho.SelectedValue,
                    Estado = dpdUfConselho.SelectedValue,
                    Numcr = txtNroConselho.Text
                };
                vo.Profissional = profissional;
			}

			vo.NrContrato = txtContrato.Text;
			vo.DataSolicitacao = Convert.ToDateTime(txtDataSolicitacao.Text);

			vo.IdMotivoGlosa = Convert.ToInt32(hidMotivo.Value);

			return vo;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar o formulário!", ex);
			}
		}

		protected void btnRemover_Click(object sender, ImageClickEventArgs e) {
			DataListItem row = (DataListItem)(sender as ImageButton).NamingContainer;
			RemoveRow(row.ItemIndex);
		}

		protected void btnBuscarBeneficiario_Click(object sender, ImageClickEventArgs e) {
			try {
				FormNegativaVO vo = new FormNegativaVO();

                string[] dados_codigo = hidCodBeneficiario.Value.Split('|');
                vo.Codint = dados_codigo[0];
                vo.Codemp = dados_codigo[1];
                vo.Matric = dados_codigo[2];
                vo.Tipreg = dados_codigo[3];
				vo.DataCriacao = DateTime.Now;
				BuscarBeneficiario(vo);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao apresentar dados do beneficiário!", ex);
			}
		}

		#region Gestao Itens

        protected void btnAdicionarItem_Click(object sender, EventArgs e)
        {
            AddNewRow();
        }

        private void AddNewRow()
        {
            DataTable dt = RetrieveItemsFromDataList();
            if (ValidateRows(dt))
            {
                dt.Rows.Add(dt.NewRow());
            }
            BindRows(dt);
        }

        private DataTable RetrieveItemsFromDataList()
        {
            DataTable dt = CreateTable();
            foreach (DataListItem item in dtlSolicitacoes.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
                    TextBox txtCodigo = item.FindControl("txtCodigo") as TextBox;
                    TextBox txtDescricao = item.FindControl("txtDescricao") as TextBox;
                    TextBox txtQuantidade = item.FindControl("txtQuantidade") as TextBox;
                    Label lblCounter = item.FindControl("lblCounter") as Label;
                    HiddenField hidCodServico = item.FindControl("hidCodServico") as HiddenField;

                    DataRow dr = dt.NewRow();
                    dr["cdTabela"] = "";
                    dr["cdMascara"] = "";

                    if (!string.IsNullOrEmpty(hidCodServico.Value))
                    {
                        string[] dados_servico = hidCodServico.Value.Split('|');
                        dr["cdTabela"] = dados_servico[0];
                        dr["cdMascara"] = dados_servico[1];
                    }
                    dr["dsServico"] = txtDescricao.Text;
                    dr["obs"] = txtObservacao.Text;

                    int qtd;
                    if (Int32.TryParse(txtQuantidade.Text, out qtd))
                        dr["qtd"] = qtd;

                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("cdTabela", typeof(string));
            dt.Columns.Add("cdMascara", typeof(string));
            dt.Columns.Add("dsServico", typeof(string));
            dt.Columns.Add("qtd", typeof(int));
            dt.Columns.Add("obs", typeof(string));
            return dt;
        }

        private bool ValidateRows(DataTable dt)
        {
            string pos = "";
            int posError = 0;
            List<string> lstMsg = new List<string>();
            List<string> lstMascaras = new List<string>();
            int outInt;
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                pos = (i + 1).ToString();
                DataRow dr = dt.Rows[i];
                if (!string.IsNullOrEmpty(Convert.ToString(dr["cdMascara"])))
                {
                    pos = Convert.ToString(dr["cdMascara"]);

                    if (lstMascaras.Contains(pos.ToLower()))
                    {
                        lstMsg.Add("O serviço " + pos + " já existe na listagem dos procedimentos!");
                        posError = posError == 0 ? i : posError;
                    }
                    lstMascaras.Add(pos.ToLower());
                }
                else
                {
                    lstMsg.Add("Informe o código de serviço na posição " + pos + "!");
                    posError = posError == 0 ? i : posError;
                }

                if (string.IsNullOrEmpty(Convert.ToString(dr["dsServico"])))
                {
                    lstMsg.Add("Informe a descrição do serviço " + pos + "!");
                    posError = posError == 0 ? i : posError;
                }

                if (string.IsNullOrEmpty(Convert.ToString(dr["qtd"])))
                {
                    lstMsg.Add("Informe a quantidade do serviço " + pos + "!");
                    posError = posError == 0 ? i : posError;
                }
                else if (!Int32.TryParse(Convert.ToString(dr["qtd"]), out outInt))
                {
                    lstMsg.Add("A quantidade do serviço " + pos + " deve ser numérica!");
                    posError = posError == 0 ? i : posError;
                }
            }

            if (lstMsg.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string m in lstMsg)
                {
                    sb.AppendLine(m);
                }
                if (sb.Length > 0)
                {
                    this.ShowError(sb.ToString());
                }

                if (posError != 0)
                {
                    int rowIndex = 0;
                    foreach (DataListItem row in dtlSolicitacoes.Items)
                    {
                        if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                        {
                            rowIndex++;
                            if (rowIndex == posError)
                            {
                                base.SetFocus(row.FindControl("txtCodigo") as WebControl);
                            }
                        }
                    }
                }
            }

            return lstMsg.Count == 0;
        }

        private void BindRows(DataTable dt)
        {
            dtlSolicitacoes.DataSource = dt;
            dtlSolicitacoes.DataBind();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function configAllCounters() { ");
            foreach (DataListItem item in dtlSolicitacoes.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
                    Label lblCounter = item.FindControl("lblCounter") as Label;
                    sb.AppendLine("configureCounter('" + txtObservacao.ClientID + "','" + lblCounter.ClientID + "'," + txtObservacao.MaxLength + " );");
                }
            }
            sb.AppendLine("}");
            sb.AppendLine("setTimeout(function() { configAllCounters(); }, 1000)");
            base.RegisterScript("configCounter", sb.ToString());
        }

		protected void btnBuscarCodigo_Click(object sender, ImageClickEventArgs e) {
			try {
				DataListItem row = (DataListItem)(sender as ImageButton).NamingContainer;
                HiddenField hidCodServico = (HiddenField)row.FindControl("hidCodServico");
				TextBox txtCodigo = row.FindControl("txtCodigo") as TextBox;
				TextBox txtDescricao = row.FindControl("txtDescricao") as TextBox;
				txtDescricao.Text = string.Empty;
				string cdMascara = txtCodigo.Text;
                hidCodServico.Value = string.Empty;
				DataTable dt = PLocatorDataBO.Instance.BuscarServicos(cdMascara, null, false);
				if (dt.Rows.Count == 0) {
					this.ShowError("O serviço informado não foi encontrado!");
				} else {
					DataRow dr = dt.Rows[0];					
					txtDescricao.Text = Convert.ToString(dr["BR8_DESCRI"]);
                    hidCodServico.Value = Convert.ToString(dr["CD_SERVICO"]); ;
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar o serviço", ex);
			}
		}

		protected void btnBuscarDescricao_Click(object sender, ImageClickEventArgs e) {
			try {
				DataListItem row = (DataListItem)(sender as ImageButton).NamingContainer;

                HiddenField hidCodServico = (HiddenField)row.FindControl("hidCodServico");
				TextBox txtCodigo = row.FindControl("txtCodigo") as TextBox;
				TextBox txtDescricao = row.FindControl("txtDescricao") as TextBox;

				txtDescricao.Text = string.Empty;

				string cdServico = txtCodigo.Text;
				txtCodigo.Text = string.Empty;

                string[] dados_servico = hidCodServico.Value.Split('|');
                string codpad = dados_servico[0];
                string codpsa = dados_servico[1];
				
				PTabelaPadraoVO srv = PLocatorDataBO.Instance.GetTabelaPadrao(codpad, codpsa);
				if (srv == null) {
					this.ShowError("O serviço informado não foi encontrado!");
				} else {
					txtCodigo.Text = srv.Codpsa;
					txtDescricao.Text = srv.Descri;
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar o serviço", ex);
			}
		}

		private List<FormNegativaItemVO> Table2Items(DataTable dtItems) {
			List<FormNegativaItemVO> lst = new List<FormNegativaItemVO>();
			foreach (DataRow drv in dtItems.Rows) {
				FormNegativaItemVO item = new FormNegativaItemVO();
                item.Codpad = Convert.ToString(drv["cdTabela"]);
				item.Codpsa = Convert.ToString(drv["cdMascara"]);
				item.Descri = Convert.ToString(drv["dsServico"]);
				item.Observacao = Convert.ToString(drv["obs"]);
				item.Quantidade = Convert.ToInt32(drv["qtd"]);

				lst.Add(item);
			}
			return lst;
		}

		private DataTable Items2Table(List<FormNegativaItemVO> lst) {
			DataTable dt = CreateTable();
			if (lst != null) {
				foreach (FormNegativaItemVO item in lst) {
					DataRow drv = dt.NewRow();
                    drv["cdTabela"] = item.Codpad;
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
			TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
			TextBox txtCodigo = item.FindControl("txtCodigo") as TextBox;
			TextBox txtDescricao = item.FindControl("txtDescricao") as TextBox;
			TextBox txtQuantidade = item.FindControl("txtQuantidade") as TextBox;
			Label lblCounter = item.FindControl("lblCounter") as Label;
            HiddenField hidCodServico = item.FindControl("hidCodServico") as HiddenField;

            hidCodServico.Value = Convert.ToString(drv["cdTabela"]) + "|" + Convert.ToString(drv["cdMascara"]);
			txtObservacao.Text = Convert.ToString(drv["obs"]);
			txtCodigo.Text = Convert.ToString(drv["cdMascara"]);
			txtDescricao.Text = Convert.ToString(drv["dsServico"]);
			txtQuantidade.Text = Convert.ToString(drv["qtd"]);

			ImageButton img1 = item.FindControl("btnBuscarCodigo") as ImageButton;
			ImageButton img2 = item.FindControl("btnBuscarDescricao") as ImageButton;
            img2.OnClientClick = "return openPopServico('" + hidCodServico.ClientID + "',this);";
		}

		private void RemoveRow(int index) {
			DataTable dt = this.RetrieveItemsFromDataList();
			dt.Rows.RemoveAt(index);

			BindRows(dt);
		}

        protected void dtlSolicitacoes_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataListItem item = e.Item;
                DataRowView drv = item.DataItem as DataRowView;

                BindItem(item, drv);
            }
        }

		#endregion

		protected void btnAprovar_Click(object sender, EventArgs e) {
			if (Id == null) {
				this.ShowError("A solicitação deve existir para ser aprovada!");
				return;
			}
			try {
				int cdProtocolo = Id.Value;
				FormNegativaBO.Instance.Aprovar(cdProtocolo, UsuarioLogado.Id, false);
				this.ShowInfo("Formulário aprovado com sucesso!");
				Bind(FormNegativaBO.Instance.GetById(cdProtocolo));
			} catch (Exception ex) {
				this.ShowError("Erro ao aprovar", ex);
			}
		}

		#region Profissional

		private void BindProfissional(string nroProf, string cdConselho, string ufConselho) {
			if (!string.IsNullOrEmpty(nroProf)) {
                PProfissionalSaudeVO vo = PLocatorDataBO.Instance.GetProfissional(nroProf, ufConselho, cdConselho);
				if (vo != null) {
					txtNroConselho.Text = nroProf;
					txtNroConselho.Enabled = false;
					txtNomeProfissional.Text = vo.Nome;
					txtNomeProfissional.Enabled = false;
					dpdTipoConselho.SelectedValue = cdConselho;
					dpdTipoConselho.Enabled = false;
					dpdUfConselho.SelectedValue = ufConselho;
					dpdUfConselho.Enabled = false;
				} else {
					this.ShowError("O profissional não foi encontrado! Preencher o nome manualmente!");
					txtNomeProfissional.Enabled = true;
				}
			} else {
				txtNroConselho.Text = "";
				txtNomeProfissional.Text = "";
				dpdTipoConselho.SelectedIndex = 0;
				dpdUfConselho.SelectedIndex = 0;
				txtNroConselho.Enabled = true;
				txtNomeProfissional.Enabled = true;
				dpdTipoConselho.Enabled = true;
				dpdUfConselho.Enabled = true;
			}
		}

        private PProfissionalSaudeVO BindFromHidProfissional()
        {

            string[] values = hidProfissional.Value.Trim().Split(';');
			string NrConselho = values[0];
			string CdConselho = values[1];
            string CdUf = values[2];

            PProfissionalSaudeVO Profissional = PLocatorDataBO.Instance.GetProfissional(NrConselho, CdUf, CdConselho);
			if (Profissional == null) {
				this.ShowError("Profissional não encontrado!");
				return null;
			}

			txtNomeProfissional.Text = Profissional.Nome;
            txtNroConselho.Text = Profissional.Numcr;
			dpdTipoConselho.SelectedValue = Profissional.Codsig;
			dpdUfConselho.SelectedValue = Profissional.Estado;

			return Profissional;
		}

		protected void btnLocProf_Click(object sender, ImageClickEventArgs e) {
			string value = hidProfissional.Value;
			if (NOT_FOUND_LOCATOR.Equals(value)) {
				hidProfissional.Value = string.Empty;
				txtNroConselho.Enabled = true;
				dpdTipoConselho.Enabled = true;
				dpdUfConselho.Enabled = true;
				txtNomeProfissional.Enabled = true;
				this.SetFocus(txtNroConselho);
			} else {
				BindFromHidProfissional();
			}
		}

		protected void btnClrProf_Click(object sender, ImageClickEventArgs e) {
			hidProfissional.Value = string.Empty;
			BindProfissional(null, null, null);
		}

		private void OnChangeConselho() {
			try {
				int idProf;
				string tipoConselho = dpdTipoConselho.SelectedValue;
				string ufConselho = dpdUfConselho.SelectedValue;
				if (!string.IsNullOrEmpty(txtNroConselho.Text) && !string.IsNullOrEmpty(tipoConselho) && !string.IsNullOrEmpty(dpdUfConselho.SelectedValue)) {
					if (Int32.TryParse(txtNroConselho.Text, out idProf)) {
						BindProfissional(idProf.ToString(), tipoConselho, ufConselho);
					} else {
						this.ShowError("O Nº do Conselho deve ser numérico!");
						return;
					}
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao carregar profissional!", ex);
			}
		}

		protected void dpdTipoConselho_SelectedIndexChanged(object sender, EventArgs e) {
			OnChangeConselho();
		}

		protected void txtNroConselho_TextChanged(object sender, EventArgs e) {
			OnChangeConselho();
		}

		protected void dpdUfConselho_SelectedIndexChanged(object sender, EventArgs e) {
			OnChangeConselho();
		}

		#endregion

		#region Prestador/Rede

		protected void rblTipoRede_SelectedIndexChanged(object sender, EventArgs e) {
			OnChangeTipoRede();
		}

		private void OnChangeTipoRede() {
			txtNomePrestador.Text = string.Empty;
			txtCpfCnpjPrestador.Text = string.Empty;
			hidCredenciado.Value = string.Empty;

			if (rblTipoRede.SelectedValue.Equals("CRED")) {
				btnLocPrestador.Visible = true;
				txtCpfCnpjPrestador.Enabled = false;
				txtNomePrestador.Enabled = false;
			} else {
				btnLocPrestador.Visible = false;
				txtNomePrestador.Enabled = true;
				txtCpfCnpjPrestador.Enabled = true;
			}
		}

		protected void btnLocPrestador_Click(object sender, ImageClickEventArgs e) {
			string value = hidCredenciado.Value;
			int idCred = 0;
			if (!Int32.TryParse(value, out idCred)) {
				this.ShowError("Credenciado inválido!");
				return;
			}
            BindCred(hidCredenciado.Value);
		}

		private void BindCred(string idCred) {
			if (idCred != "") {
                PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idCred);
                txtCpfCnpjPrestador.Text = vo.Tippe == PConstantes.PESSOA_JURIDICA ? (vo.Cpfcgc != null ? FormatUtil.FormatCnpj(vo.Cpfcgc) : "") : (vo.Cpfcgc != null ? FormatUtil.FormatCpf(vo.Cpfcgc) : "");
                txtNomePrestador.Text = vo.Nome;
			}
		}

		#endregion

		#region MotivoGlosa

		private void BindMotivo(int id) {
			lblDescricaoMotivo.Text = string.Empty;
			hidMotivo.Value = string.Empty;
			txtCodigoMotivo.Text = string.Empty;

			if (id == 0) {
				return;
			}
			MotivoGlosaVO vo = FormNegativaBO.Instance.GetMotivoGlosaById(id);
			if (vo == null) {
				this.ShowError("Motivo não encontrado ou inválido! Verifique o código novamente");
				return;
			}
			txtCodigoMotivo.Text = vo.Id.ToString();
			lblDescricaoMotivo.Text = vo.Grupo + " - " + vo.Descricao;
			hidMotivo.Value = vo.Id.ToString();
		}

		protected void txtCodigoMotivo_TextChanged(object sender, EventArgs e) {
			string strCodigo = txtCodigoMotivo.Text.Trim();
			if (string.IsNullOrEmpty(strCodigo)) {
				lblDescricaoMotivo.Text = string.Empty;
			} else {
				int id;
				if (!Int32.TryParse(strCodigo, out id)) {
					this.ShowError("O código do motivo deve ser numérico!");
					BindMotivo(0);
					return;
				}
				BindMotivo(id);
			}
		}

		protected void btnLocMotivo_Click(object sender, ImageClickEventArgs e) {
			string value = hidMotivo.Value;
			int id = 0;
			if (!Int32.TryParse(value, out id)) {				
				this.ShowError("Motivo inválido!");
				BindMotivo(0);
				return;
			}
			BindMotivo(id);
		}

		#endregion
	}
}