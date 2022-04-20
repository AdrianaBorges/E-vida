using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Forms {
	public partial class FormAutorizacao : FormPageBase {
		[Serializable]
		public class ArquivoTela {
			public bool New { get; set; }
			public string NomeFisico { get; set; }
			public string NomeSalvar { get; set; }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

				dpdTipoConselho.DataSource = PLocatorDataBO.Instance.ListarConselhoProfissional();
				dpdTipoConselho.DataBind();
				dpdTipoConselho.Items.Insert(0, new ListItem("SELECIONE", ""));

				dpdUfConselho.DataSource = PLocatorDataBO.Instance.ListarUf();
				dpdUfConselho.DataBind();
				dpdUfConselho.Items.Insert(0, new ListItem("SELECIONE", ""));

				btnSalvar.Visible = false;

				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id = 0;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador de requisição inexistente!");
						return;
					}

					Bind(id);
				} else {
					CanEdit = true;
					btnSalvar.Visible = true;
					divProcedimentos.Visible = HasPermissaoGestao();

					//divAtendimento.Visible = true;
					//conteudo.Visible = false;
					divAtendimento.Visible = false;
					conteudo.Visible = true;
				}

			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO; }
		}

		private bool HasPermissaoGestao() {
			return HasPermission(Modulo.GERIR_AUTORIZACAO_MEDICA) || HasPermission(Modulo.GERIR_AUTORIZACAO_ODONTO);
		}

		public List<ArquivoTela> Arquivos {
			get {
				if (ViewState["ARQUIVOS"] == null) {
					Arquivos = new List<ArquivoTela>();
				}
				return ViewState["ARQUIVOS"] as List<ArquivoTela>;
			}
			set {
				ViewState["ARQUIVOS"] = value;
			}
		}

		public bool CanEdit {
			get {
				return Convert.ToBoolean(ViewState["CAN_EDIT"]);
			}
			set {
				ViewState["CAN_EDIT"] = value.ToString();
			}
		}

		public bool CanAddFile {
			get {
				return Convert.ToBoolean(ViewState["CanAddFile"]);
			}
			set {
				ViewState["CanAddFile"] = value.ToString();
			}
		}

		private void Bind(int id) {
			litProtocolo.Value = id.ToString();
			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(id);
			if (vo == null) {
				this.ShowError("Identificador de requisição inválido!");
				return;
			}
			lblProtocoloANS.Text = vo.ProtocoloAns;
			btnHistorico.Visible = true;

			CanEdit = 
				(vo.Status != StatusAutorizacao.APROVADA && vo.Status != StatusAutorizacao.CANCELADA && vo.Status != StatusAutorizacao.NEGADA
					&& vo.Status != StatusAutorizacao.ENVIADA && HasPermissaoGestao()) 
				|| (vo.Status == StatusAutorizacao.ENVIADA && vo.Origem == OrigemAutorizacao.GESTOR);
			CanAddFile = CanEdit || (vo.Status == StatusAutorizacao.SOLICITANDO_DOC && vo.Origem == OrigemAutorizacao.GESTOR);

			txtProtocolo.Text = vo.Id.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);
			
			lnkNeg.Visible = false;
			lblMotivo.Visible = false;
			if (vo.Status == StatusAutorizacao.APROVADA) {
				List<AutorizacaoTissVO> lstTiss = AutorizacaoBO.Instance.ListTiss(vo.Id);
				rptAutorizacaoTiss.DataSource = lstTiss;
				rptAutorizacaoTiss.DataBind();

				if (rptAutorizacaoTiss.Items.Count > 0) {
					rptAutorizacaoTiss.Visible = true;
					foreach (RepeaterItem item in rptAutorizacaoTiss.Items) {
						LinkButton lnkAutorizacaoTiss = (LinkButton)item.FindControl("lnkAutorizacaoTiss");
						AutorizacaoTissVO tissVO = lstTiss[item.ItemIndex];
						lnkAutorizacaoTiss.Text = tissVO.NrAutorizacaoTiss.ToString();
						lnkAutorizacaoTiss.Visible = true;
						lnkAutorizacaoTiss.OnClientClick = "return openDownload('" + tissVO.NomeArquivo + "', false)";
					}
				} else {
					rptAutorizacaoTiss.Visible = false;
				}

				if (!string.IsNullOrEmpty(vo.ObsAprovacao)) {
                    lblMotivo.Text = vo.ObsAprovacao;
					lblMotivo.Visible = true;
				}
			} else if (vo.Status == StatusAutorizacao.NEGADA) {
				if (vo.CodNegativa != null) {
					lnkNeg.Visible = true;
					lnkNeg.OnClientClick = "return openNegPdf(" + vo.CodNegativa.Value + ");";
				}
			} else if (vo.Status == StatusAutorizacao.CANCELADA) {
				lblMotivo.Visible = true;
				lblMotivo.Text = " - " + vo.MotivoCancelamento;
			}

            txtCartao.Text = vo.UsuarioTitular.Matant;
			txtDataSolicitacao.Text = vo.DataSolicitacao.ToString("dd/MM/yyyy HH:mm");
			txtStatus.Text = AutorizacaoTradutorHelper.TraduzStatus(vo.Status);
			txtDataAutorizacao.Text = vo.DataAutorizacao != null ? vo.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm") : "";
			txtOrigem.Text = AutorizacaoTradutorHelper.TraduzOrigem(vo.Origem);
            BindTitular(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);

            dpdDependente.SelectedValue = vo.Usuario.Codint.Trim() + "|" + vo.Usuario.Codemp.Trim() + "|" + vo.Usuario.Matric.Trim() + "|" + vo.Usuario.Tipreg.Trim();
			dpdDependente_SelectedIndexChanged(null, null);

			if (vo.RedeAtendimento != null) {

                if (vo.RedeAtendimento.Cpfcgc != null)
                {
                    txtCpfCnpj.Text = vo.RedeAtendimento.Cpfcgc.Trim();
                    /*
                    if (strCpfCnpj.Length > 11)
                        txtCpfCnpj.Text = FormatUtil.FormatCnpj(strCpfCnpj);
                    else
                        txtCpfCnpj.Text = FormatUtil.FormatCpf(strCpfCnpj);
                     * */
                }
                else 
                {
                    txtCpfCnpj.Text = "";
                }

                if (vo.RedeAtendimento.Nome != null)
                {
                    txtRazaoSocial.Text = vo.RedeAtendimento.Nome.Trim();
                }

                if (vo.RedeAtendimento.Codigo != null)
                {
                    txtCpfCnpj.Enabled = false;
                    txtRazaoSocial.Enabled = false;
                    hidCredenciado.Value = vo.RedeAtendimento.Codigo.Trim();
                }
                
			}

			if (vo.Profissional != null) {
                PProfissionalSaudeVO prof = PLocatorDataBO.Instance.GetProfissional(vo.Profissional.Numcr.Trim(), vo.Profissional.Estado.Trim(), vo.Profissional.Codsig.Trim());
                txtNomeProfissional.Text = vo.Profissional.Nome;
                txtNroConselho.Text = vo.Profissional.Numcr;
                dpdTipoConselho.SelectedValue = vo.Profissional.Codsig.Trim();
                dpdUfConselho.SelectedValue = vo.Profissional.Estado.Trim();
				if (prof != null) {
					txtNomeProfissional.Enabled = false;
					txtNroConselho.Enabled = false;
					dpdTipoConselho.Enabled = false;
					dpdUfConselho.Enabled = false;
                    hidProfissional.Value = vo.Profissional.Numcr.Trim() + ";" + vo.Profissional.Codsig.Trim() + ";" + vo.Profissional.Estado.Trim();
				}
			}

			if (vo.Carater != null)
				dpdCarater.SelectedValue = vo.Carater.ToString();

			dpdTipo.SelectedValue = vo.Tipo.ToString();

			dpdInternacao.SelectedValue = vo.Internacao ? "S" : "N";

            hidDoenca.Value = vo.CodDoenca;
            txtDoenca.Text = vo.CodDoenca;

			if (vo.DataInternacao != null)
				txtDataInternacao.Text = vo.DataInternacao.Value.ToShortDateString();

			if (vo.Hospital != null) {
                if (vo.Hospital.Codigo != null)
                {
                    hidHospital.Value = vo.Hospital.Codigo.Trim();
                }
                if (vo.Hospital.Nome != null)
                {
                    txtHospital.Text = vo.Hospital.Nome.Trim();
                }
                lblHospitalMan.Visible = true;
                if (vo.Hospital.Codigo != null)
                {
                    lblHospitalMan.Visible = string.IsNullOrEmpty(vo.Hospital.Codigo.Trim());  
                }
			}
			
			txtIndicacao.Text = vo.IndicacaoClinica;

            dpdTfd.SelectedValue = Convert.ToBoolean(vo.Tfd) ? "S" : "N";

            if (vo.DataInicioTfd != null)
                txtDataInicioTfd.Text = vo.DataInicioTfd.Value.ToShortDateString();

            if (vo.DataTerminoTfd != null)
                txtDataTerminoTfd.Text = vo.DataTerminoTfd.Value.ToShortDateString();

			List<AutorizacaoProcedimentoVO> lstProcedimentos = AutorizacaoBO.Instance.ListProcedimentos(vo.Id);
			DataTable dtProcs = Items2Table(lstProcedimentos);
			BindRows(dtProcs);

			List<ArquivoTela> lstArqs = Arquivos;
			List<AutorizacaoArquivoVO> lstArquivos = AutorizacaoBO.Instance.ListArquivos(vo.Id);
			lstArqs = lstArquivos.Select(x => new ArquivoTela()
			{
				New = false,
				NomeSalvar = x.NomeArquivo
			}).ToList();
			Arquivos = lstArqs;
			ltvArquivos.DataSource = lstArqs;
			ltvArquivos.DataBind();

			List<AutorizacaoSolDocVO> lstSolDoc = AutorizacaoBO.Instance.ListSolicitacoesDoc(vo.Id);

			if (lstSolDoc != null && lstSolDoc.Count > 0) {
				lstSolDoc.Sort((x, y) => y.Data.CompareTo(x.Data));
				string lastMessage = lstSolDoc[0].MensagemSolDoc;
				trMensagemSolDoc.Visible = true;
				txtMensagemSolDoc.Text = lastMessage;
				if (vo.Status == StatusAutorizacao.SOLICITANDO_DOC) {
					lblMotivo.Text = " - " + lastMessage;
					lblMotivo.Visible = true;
				}

				gdvSolDoc.DataSource = lstSolDoc;
				gdvSolDoc.DataBind();
			} else {
				trMensagemSolDoc.Visible = false;
			}

            txtObservacao.Text = vo.Obs;
            txtComentario.Text = vo.ComentarioAuditor;

			txtDataSolRevalidacao.Text = vo.DataSolRevalidacao != null ? vo.DataSolRevalidacao.Value.ToString("dd/MM/yyyy HH:mm") : "";
			txtDataAprovRevalidacao.Text = vo.DataAprovRevalidacao != null ? vo.DataAprovRevalidacao.Value.ToString("dd/MM/yyyy HH:mm") : "";

			divProcedimentos.Visible = HasPermissaoGestao();

			btnAnalisar.Visible = (vo.Status == StatusAutorizacao.ENVIADA || vo.Status == StatusAutorizacao.REVALIDACAO) && HasPermissaoGestao();
			btnCotar.Visible = (vo.Status == StatusAutorizacao.ENVIADA || vo.Status == StatusAutorizacao.REVALIDACAO || vo.Status == StatusAutorizacao.EM_ANALISE) && HasPermissaoGestao();
			btnEnviarDoc.Visible = CanAddFile && !CanEdit;
			btnIncluirArquivo.Visible = CanAddFile;

			txtCartao.Enabled = CanEdit;
			btnSalvar.Visible = CanEdit;
			dpdDependente.Enabled = CanEdit;
			btnLocCred.Visible = CanEdit;
			btnLocDoenca.Visible = CanEdit;
			btnLocProf.Visible = CanEdit;
			btnClrCred.Visible = CanEdit;
			btnClrDoenca.Visible = CanEdit;
			btnClrProf.Visible = CanEdit;
			btnAdicionarItem.Visible = CanEdit;
			btnClrHospital.Visible = CanEdit;
			btnLocHospital.Visible = CanEdit;

			divAtendimento.Visible = false;
			conteudo.Visible = true;
			tbComentarioAuditor.Visible = !string.IsNullOrEmpty(vo.ComentarioAuditor) || vo.Status == StatusAutorizacao.ENVIADO_AUDITORIA;
			btnEnviarComentario.Visible = HasPermissaoGestao() && vo.Status == StatusAutorizacao.ENVIADO_AUDITORIA;
		}

		private void BindTitular(string codint, string codemp, string matric, string tipreg) {
			if (string.IsNullOrEmpty(txtCartao.Text)) {
				this.ShowError("Informe o cartão do titular!");
				return;
			}
			PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
			List<PUsuarioVO> lstBeneficiarios = null;
			if (titular == null) {
				this.ShowError("Titular não encontrado!");
				txtNomeTitular.Text = string.Empty;
			} else {
				lstBeneficiarios = PUsuarioBO.Instance.ListarUsuarios(titular.Codint, titular.Codemp, titular.Matric);

				if (lstBeneficiarios == null) {
					this.ShowError("O funcionário não possui beneficiários vinculados!");
					return;
				}

				// nova autorizacao, filtrar apenas beneficiarios ativos
				/*if (string.IsNullOrEmpty(litProtocolo.Value)) {
					lstBeneficiarios.RemoveAll(x => !String.IsNullOrEmpty(x.Datblo.Trim()) && x.Tipusu != "T");
				} else {
                    lstBeneficiarios.RemoveAll(x => !String.IsNullOrEmpty(x.Datblo.Trim()) && x.Tipusu != "T"
                        && (x.Codint != codint || x.Codemp != codemp || x.Matric != matric || x.Tipreg != tipreg));
				}*/

                if (string.IsNullOrEmpty(litProtocolo.Value))
                {
                    lstBeneficiarios.RemoveAll(x => (x.Datblo != "        " && Int32.Parse(x.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))) && x.Tipusu != "T");
                }
                else
                {
                    lstBeneficiarios.RemoveAll(x => (x.Datblo != "        " && Int32.Parse(x.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))) && x.Tipusu != "T"
                        && (x.Codint != codint || x.Codemp != codemp || x.Matric != matric || x.Tipreg != tipreg));
                }

				dpdDependente.DataSource = lstBeneficiarios;
				dpdDependente.DataBind();
				dpdDependente.Items.Insert(0, new ListItem("SELECIONE", ""));

				txtNomeTitular.Text = titular.Nomusr;
			}
		}

        private PProfissionalSaudeVO BindFromHidProfissional()
        {
            string[] values = hidProfissional.Value.Split(';');

            string numcr = values[0];
            string codsig = values[1];
            string estado = values[2];

            PProfissionalSaudeVO Profissional = PLocatorDataBO.Instance.GetProfissional(numcr, estado, codsig);
			if (Profissional == null) {
				this.ShowError("Profissional não encontrado!");
				return null;
			}
            Profissional.Numcr = numcr;
            Profissional.Codsig = codsig;
			Profissional.Estado = estado;

			txtNomeProfissional.Text = Profissional.Nome;
            txtNroConselho.Text = numcr;
            dpdTipoConselho.SelectedValue = codsig;
            dpdUfConselho.SelectedValue = estado;

			return Profissional;
		}

		private void MostrarDependente() {

            string[] dados_beneficiario = dpdDependente.SelectedValue.Split('|');
            string codint = dados_beneficiario[0];
            string codemp = dados_beneficiario[1];
            string matric = dados_beneficiario[2];
            string tipreg = dados_beneficiario[3];

			PUsuarioVO vo = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
			PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(codint, codemp, matric, tipreg);
			PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla);
			PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
			txtNumCartao.Text = vo.Matant;
			txtPlano.Text = plano.Descri;
			txtNomeTitular.Text = titular.Nomusr;
			txtCartao.Text = titular.Matant;

			txtMunicipioUf.Text = vo.Munici + " / " + vo.Estado;
		}

		private void ChangeTitular() {
			dpdDependente.Items.Clear();
			txtNumCartao.Text = string.Empty;
			txtNomeTitular.Text = string.Empty;
			txtPlano.Text = string.Empty;
			txtMunicipioUf.Text = string.Empty;

			BindTitular("", "", "", "");
		}

		private bool ValidateRequired() {
			List<ItemError> lst = new List<ItemError>();

			if (string.IsNullOrEmpty(txtCartao.Text)) {
				this.AddErrorMessage(lst, txtCartao, "Informe o cartão do titular!");
			}
			if (string.IsNullOrEmpty(dpdDependente.SelectedValue)) {
				this.AddErrorMessage(lst, dpdDependente, "Selecione o beneficiário!");
			}
			
			/*if (string.IsNullOrEmpty(hidCredenciado.Value)) {
				if (string.IsNullOrEmpty(txtCpfCnpj.Text)) {
					this.AddErrorMessage(lst, txtCpfCnpj, "Informe o cpf/cnpj do credenciado!");
				}
				if (string.IsNullOrEmpty(txtRazaoSocial.Text)) {
					this.AddErrorMessage(lst, txtRazaoSocial, "Informe o nome/razão social do credenciado!");
				}
			}*/			

			if (string.IsNullOrEmpty(hidProfissional.Value)) {
				if (string.IsNullOrEmpty(txtNroConselho.Text)) {
					this.AddErrorMessage(lst, txtNroConselho, "Informe o número de conselho do profissional!");
				}
				if (string.IsNullOrEmpty(dpdTipoConselho.SelectedValue)) {
					this.AddErrorMessage(lst, dpdTipoConselho, "Informe o tipo de conselho!");
				}
				if (string.IsNullOrEmpty(dpdUfConselho.SelectedValue)) {
					this.AddErrorMessage(lst, dpdUfConselho, "Informe a UF do conselho!");
				}

				if (string.IsNullOrEmpty(txtNomeProfissional.Text)) {
					this.AddErrorMessage(lst, txtNomeProfissional, "Informe o nome do profissional!");
				}
			}

			if (string.IsNullOrEmpty(dpdTipo.SelectedValue)) {
				this.AddErrorMessage(lst, dpdTipo, "Informe o tipo de autorização!");
			}

			if (string.IsNullOrEmpty(dpdInternacao.SelectedValue)) {
				this.AddErrorMessage(lst, dpdInternacao, "Informe se há ou não internação!");
			} else {
				if (dpdInternacao.SelectedValue.Equals("S")) {
					if (string.IsNullOrEmpty(txtDataInternacao.Text)) {
						this.AddErrorMessage(lst, txtDataInternacao, "Informe a data de internação!");
					}
				}
			}

            if (string.IsNullOrEmpty(dpdTfd.SelectedValue))
            {
                this.AddErrorMessage(lst, dpdTfd, "Informe se é ou não TFD!");
            }
            else
            {
                if (dpdTfd.SelectedValue.Equals("S"))
                {
                    if (string.IsNullOrEmpty(txtDataInicioTfd.Text))
                    {
                        this.AddErrorMessage(lst, txtDataInicioTfd, "Informe a data de início do TFD!");
                    }
                    if (string.IsNullOrEmpty(txtDataTerminoTfd.Text))
                    {
                        this.AddErrorMessage(lst, txtDataTerminoTfd, "Informe a data de término do TFD!");
                    }
                }
            }  

			if (string.IsNullOrEmpty(txtHospital.Text)) {
				this.AddErrorMessage(lst, txtHospital, "Informe o hospital/clínica!");
			}
			if (HasPermissaoGestao()) {
				DataTable dtItems = RetrieveItemsFromDataList();
				if (dtItems.Rows.Count == 0) {
					this.AddErrorMessage(lst, btnAdicionarItem, "É necessário incluir pelo menos um procedimento!");
				}
			}
			if (Arquivos.Count == 0) {
				this.AddErrorMessage(lst, btnIncluirArquivo, "É necessário incluir pelo menos um pedido médico!");
			}

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}

			return true;
		}

		private bool ValidateFields() {
			if (!ValidateRequired())
				return false;

			List<ItemError> lst = new List<ItemError>();

			if (string.IsNullOrEmpty(hidCredenciado.Value) && !string.IsNullOrEmpty(txtCpfCnpj.Text)) {
				long cnpj = 0;
				string str = FormatUtil.UnformatCnpj(txtCpfCnpj.Text);
				if (!Int64.TryParse(str, out cnpj)) {
					this.AddErrorMessage(lst, txtCpfCnpj, "O CPF/CNPJ informado está em um formato inválido!");
				}
			}

			if (string.IsNullOrEmpty(hidProfissional.Value)) {
				int nroConselho = 0;
				if (!Int32.TryParse(txtNroConselho.Text, out nroConselho)) {
					this.AddErrorMessage(lst, txtNroConselho, "O nº do conselho deve ser numérico!");
				}
			}

			if (!string.IsNullOrEmpty(txtDataInternacao.Text)) {
				DateTime dt;
				if (!DateTime.TryParse(txtDataInternacao.Text, out dt)) {
					this.AddErrorMessage(lst, txtDataInternacao, "A data de internação está em formato incorreto!");
				}
			}

            if (!string.IsNullOrEmpty(txtDataInicioTfd.Text) && !string.IsNullOrEmpty(txtDataTerminoTfd.Text))
            {
                DateTime dtInicioTfd;
                DateTime dtTerminoTfd;

                if (!DateTime.TryParse(txtDataInicioTfd.Text, out dtInicioTfd))
                {
                    this.AddErrorMessage(lst, txtDataInicioTfd, "A data de início do TFD está em formato incorreto!");
                }
                else if (!DateTime.TryParse(txtDataTerminoTfd.Text, out dtTerminoTfd))
                {
                    this.AddErrorMessage(lst, txtDataTerminoTfd, "A data de término do TFD está em formato incorreto!");
                }
                else if (dtTerminoTfd < dtInicioTfd)
                {
                    this.AddErrorMessage(lst, txtDataTerminoTfd, "A data de término do TFD não pode ser menor que a data de início do TFD!");
                }
            }

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}
			return true;
		}

		private void Salvar() {
			if (!ValidateFields()) {
				return;
			}
			
			DataTable dtItems = null;

			if (HasPermissaoGestao()) {
				dtItems = RetrieveItemsFromDataList();
				if (!ValidateRows(dtItems))
					return;
			}

			AutorizacaoVO vo = new AutorizacaoVO();

			if (!string.IsNullOrEmpty(litProtocolo.Value)) {
				vo.Id = Int32.Parse(litProtocolo.Value);
				vo = AutorizacaoBO.Instance.GetById(vo.Id);
			}

			string[] dados_beneficiario = dpdDependente.SelectedValue.Split('|');
            string codint = dados_beneficiario[0];
            string codemp = dados_beneficiario[1];
            string matric = dados_beneficiario[2];
            string tipreg = dados_beneficiario[3];

			PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
			vo.Usuario = benef;

			if (!string.IsNullOrEmpty(dpdCarater.SelectedValue))
				vo.Carater = dpdCarater.SelectedValue.Equals("ELETIVA") ? CaraterAutorizacao.ELETIVA : CaraterAutorizacao.URGENCIA;

			vo.CodDoenca = hidDoenca.Value;

			vo.Tipo = AutorizacaoTradutorHelper.TraduzTipo(dpdTipo.SelectedValue);

			if (!string.IsNullOrEmpty(hidCredenciado.Value)) {
				vo.RedeAtendimento = PRedeAtendimentoBO.Instance.GetById(hidCredenciado.Value);
			} else {
				vo.RedeAtendimento = new PRedeAtendimentoVO();
				if (!string.IsNullOrEmpty(txtRazaoSocial.Text))
					vo.RedeAtendimento.Nome = txtRazaoSocial.Text;
				if (!string.IsNullOrEmpty(txtCpfCnpj.Text))
                    vo.RedeAtendimento.Cpfcgc = FormatUtil.UnformatCnpj(txtCpfCnpj.Text);
			}

			if (!string.IsNullOrEmpty(txtDataInternacao.Text))
				vo.DataInternacao = DateTime.Parse(txtDataInternacao.Text);

			vo.IndicacaoClinica = txtIndicacao.Text;
			vo.Internacao = dpdInternacao.SelectedValue.Equals("S");
			vo.Hospital = new PRedeAtendimentoVO();
			if (!string.IsNullOrEmpty(hidHospital.Value))
				vo.Hospital.Codigo = hidHospital.Value;
			vo.Hospital.Nome = txtHospital.Text;

            vo.Tfd = dpdTfd.SelectedValue.Equals("S");

            if (Convert.ToBoolean(vo.Tfd))
            {
                if (!string.IsNullOrEmpty(txtDataInicioTfd.Text))
                    vo.DataInicioTfd = DateTime.Parse(txtDataInicioTfd.Text);

                if (!string.IsNullOrEmpty(txtDataTerminoTfd.Text))
                    vo.DataTerminoTfd = DateTime.Parse(txtDataTerminoTfd.Text);
            }

			vo.Obs = txtObservacao.Text;

            vo.UsuarioTitular = PUsuarioBO.Instance.GetTitular(benef.Codint, benef.Codemp, benef.Matric);
            PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);
			PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla);
			vo.ProdutoSaude = plano;

			if (!string.IsNullOrEmpty(hidProfissional.Value)) {
				vo.Profissional = BindFromHidProfissional();
			} else {
				vo.Profissional = new PProfissionalSaudeVO();
                vo.Profissional.Numcr = txtNroConselho.Text;
                vo.Profissional.Codsig = dpdTipoConselho.SelectedValue;
                vo.Profissional.Estado = dpdUfConselho.SelectedValue;
                vo.Profissional.Nome = txtNomeProfissional.Text;
			}

			vo.Obs = txtObservacao.Text;
			List<AutorizacaoProcedimentoVO> lstProcs = null;
			if (HasPermissaoGestao()) {
				lstProcs = Table2Items(dtItems);
			}

			List<AutorizacaoArquivoVO> lstArqs = Arquivos.Select(x => new AutorizacaoArquivoVO()
			{
				NomeArquivo = x.NomeSalvar,
				CodAutorizacao = x.New ? 0 : vo.Id
			}).ToList();

			if (tbComentarioAuditor.Visible) {
				vo.ComentarioAuditor = txtComentario.Text;
			}

			bool isNew = vo.Id == 0;
			AutorizacaoBO.Instance.Salvar(vo, lstProcs, lstArqs, UsuarioLogado.Usuario);

			this.btnSalvar.Visible = false;
			if (isNew)
				this.ShowInfo("Solicitação de Autorização cadastrada com sucesso! Acompanhe através do portal ou e-mail!");
			else
				this.ShowInfo("Alterações realizadas com sucesso!");

			try {
				this.RegisterScript("OK", "isMsgSalvaOK = true;");
				//Bind(vo.Id);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao recarregar formulário!", ex);
			}
		}
				
		#region Gestão Arquivos

		private void AddArquivo(string fisico, string original) {
			List<ArquivoTela> lstAtual = Arquivos;
			ArquivoTela vo = new ArquivoTela()
			{
				New = true,
				NomeSalvar = original,
				NomeFisico = fisico
			};
			bool contains = lstAtual.FindIndex(x => x.NomeSalvar.Equals(original, StringComparison.InvariantCultureIgnoreCase)) >= 0;
			if (!contains) {
				lstAtual.Add(vo);
			} else {
				this.ShowError("Este arquivo já existe na listagem! Por favor, exclua o antigo ou renomeie o arquivo novo!");
			}
			ltvArquivos.DataSource = lstAtual;
			ltvArquivos.DataBind();
		}

		private void RemoverArquivo(string nome) {
			List<ArquivoTela> lstAtual = Arquivos;
			int idx = lstAtual.FindIndex(x => x.NomeSalvar.Equals(nome, StringComparison.InvariantCultureIgnoreCase));
			if (idx >= 0)
				lstAtual.RemoveAt(idx);
			else {
				this.ShowError("Erro ao tentar remover arquivo inexistente na lista!");
			}

			ltvArquivos.DataSource = lstAtual;
			ltvArquivos.DataBind();
		}

		#endregion

		private void BindCred(string idCred) {
			if (!String.IsNullOrEmpty(idCred.Trim())) {
                PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idCred);
                txtCpfCnpj.Text = vo.Tippe == PConstantes.PESSOA_JURIDICA ? (vo.Cpfcgc != null ? FormatUtil.FormatCnpj(vo.Cpfcgc) : "") : (vo.Cpfcgc != null ? FormatUtil.FormatCpf(vo.Cpfcgc) : "");
				txtRazaoSocial.Text = vo.Nome;
				txtCpfCnpj.Enabled = false;
				txtRazaoSocial.Enabled = false;
			} else {
				txtCpfCnpj.Text = "";
				txtRazaoSocial.Text = "";
				txtCpfCnpj.Enabled = true;
				txtRazaoSocial.Enabled = true;
			}
		}

		private void BindHospital(string idHospital) {
			txtHospital.Enabled = false;
            if (!String.IsNullOrEmpty(idHospital.Trim()))
            {
                PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idHospital);
				txtHospital.Text = vo.Nome;
				lblHospitalMan.Visible = false;
			} else {
				txtHospital.Text = "";
				lblHospitalMan.Visible = false;
			}
		}

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
					this.ShowError("O profissional não foi encontrado!");
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

		#region Gestao Itens

        private void AddNewRow()
        {
            DataTable dt = RetrieveItemsFromDataList();
            if (ValidateRows(dt))
            {
                DataRow dr = dt.NewRow();
                dr["cdTabela"] = "";
                dr["cdMascara"] = "";
                dt.Rows.Add(dr);
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
                    TextBox txtMascara = (TextBox)item.FindControl("txtMascara");
                    Label txtDescricao = item.FindControl("txtDescricao") as Label;
                    TextBox txtQuantidade = item.FindControl("txtQuantidade") as TextBox;
                    DropDownList dpdOpme = item.FindControl("dpdOpme") as DropDownList;
                    Label lblCounter = item.FindControl("lblCounter") as Label;
                    ImageButton btnRemover = item.FindControl("btnRemover") as ImageButton;
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
                    dr["opme"] = dpdOpme.SelectedValue;

                    int qtd;
                    if (Int32.TryParse(txtQuantidade.Text, out qtd))
                        dr["qtd"] = qtd;

                    dt.Rows.Add(dr);

                    if (!CanEdit)
                    {
                        btnRemover.Visible = false;
                    }
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
            dt.Columns.Add("opme", typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0], dt.Columns[1] };
            return dt;
        }

        private bool ValidateRows(DataTable dt)
        {
            string pos = "";
            int posError = 0;
            List<string> lstMsg = new List<string>();

            int outInt;
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                pos = (i + 1).ToString();
                DataRow dr = dt.Rows[i];

                if (string.IsNullOrEmpty(Convert.ToString(dr["cdMascara"])))
                {
                    lstMsg.Add("Informe o serviço na posição " + pos + "!");
                    posError = posError == 0 ? i : posError;
                }
                else
                {
                    pos = Convert.ToString(dr["cdMascara"]);
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

                if ("S".Equals(Convert.ToString(dr["opme"])))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(dr["obs"])))
                    {
                        //Para procedimentos que necessitem de OPME são necessárias informações adicionais. Favor inserir no campo INFORMAÇÕES ADICIONAIS do procedimento (Código).
                        lstMsg.Add("Para procedimentos OPME são necessárias informações adicionais. Favor inserir no campo INFORMAÇÕES ADICIONAIS do procedimento " + pos + "!");
                        posError = posError == 0 ? i : posError;
                    }
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
                                base.SetFocus(row.FindControl("txtObservacao") as WebControl);
                            }
                        }
                    }
                }
            }

            return lstMsg.Count == 0;
        }
        
        private void BindRows(DataTable dt) {
			dtlSolicitacoes.DataSource = dt;
			dtlSolicitacoes.DataBind();

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("function configAllCounters() { ");
			foreach (DataListItem item in dtlSolicitacoes.Items) {
				if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item) {
					TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
					Label lblCounter = item.FindControl("lblCounter") as Label;
					sb.AppendLine("configureCounter('" + txtObservacao.ClientID + "','" + lblCounter.ClientID + "'," + txtObservacao.MaxLength + " );");
				}
			}
			sb.AppendLine("}");
			base.RegisterScript("configCounter", sb.ToString());
		}

        protected void txtMascara_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataListItem row = (DataListItem)(sender as TextBox).NamingContainer;
                HiddenField hidCodServico = (HiddenField)row.FindControl("hidCodServico");
                TextBox txtMascara = (TextBox)row.FindControl("txtMascara");
                Label txtDescricao = (Label)row.FindControl("txtDescricao");
                string cdMascara = txtMascara.Text;
                hidCodServico.Value = string.Empty;
                string cd = PLocatorDataBO.Instance.GetCodServicoByMascara(cdMascara);
                if (String.IsNullOrEmpty(cd))
                {
                    this.ShowError("Serviço não encontrado!");
                    txtDescricao.Text = string.Empty;
                }
                else
                {
                    hidCodServico.Value = cd;
                    BindServico(row);
                }
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao buscar o serviço", ex);
            }
        }

        private void BindServico(DataListItem row)
        {
            HiddenField hidCodServico = (HiddenField)row.FindControl("hidCodServico");
            TextBox txtMascara = (TextBox)row.FindControl("txtMascara");
            Label txtDescricao = (Label)row.FindControl("txtDescricao");

            txtMascara.Text = txtDescricao.Text = string.Empty;
            if (!string.IsNullOrEmpty(hidCodServico.Value))
            {
                foreach (DataListItem item in dtlSolicitacoes.Items)
                {
                    if (item == row) continue;
                    HiddenField hidCodServicoIn = (HiddenField)item.FindControl("hidCodServico");
                    if (hidCodServico.Value.Equals(hidCodServicoIn.Value))
                    {
                        this.ShowError("Já existe um procedimento na lista para este serviço!");
                        hidCodServico.Value = string.Empty;
                        return;
                    }
                }

                string[] dados_servico = hidCodServico.Value.Split('|');
                string codpad = dados_servico[0];
                string codpsa = dados_servico[1];

                PTabelaPadraoVO vo = PLocatorDataBO.Instance.GetTabelaPadrao(codpad, codpsa);
                if (vo != null)
                {
                    txtMascara.Text = vo.Codpsa;
                    txtDescricao.Text = vo.Descri;
                }
                else
                {
                    this.ShowError("Serviço não encontrado!");
                }
            }
            else
            {
                txtMascara.Text = txtDescricao.Text = string.Empty;
            }
        }

        protected void btnBuscarDescricao_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataListItem row = (DataListItem)(sender as ImageButton).NamingContainer;
                BindServico(row);
            }
            catch (Exception ex)
            {
                this.ShowError("Erro ao escolher serviço!", ex);
            }
        }
        
        private List<AutorizacaoProcedimentoVO> Table2Items(DataTable dtItems) {
			List<AutorizacaoProcedimentoVO> lst = new List<AutorizacaoProcedimentoVO>();
			foreach (DataRow drv in dtItems.Rows) {
				AutorizacaoProcedimentoVO item = new AutorizacaoProcedimentoVO();
				item.Servico = new PTabelaPadraoVO();
                item.Servico.Codpad = Convert.ToString(drv["cdTabela"]);
                item.Servico.Codpsa = Convert.ToString(drv["cdMascara"]);
                item.Servico.Descri = Convert.ToString(drv["dsServico"]);
				item.Observacao = Convert.ToString(drv["obs"]);
				item.Quantidade = Convert.ToInt32(drv["qtd"]);
				item.Opme = Convert.ToString(drv["opme"]).Equals("S");

				lst.Add(item);
			}
			return lst;
		}

		private DataTable Items2Table(List<AutorizacaoProcedimentoVO> lst) {
			DataTable dt = CreateTable();
			if (lst != null) {
				foreach (AutorizacaoProcedimentoVO item in lst) {
					DataRow drv = dt.NewRow();
                    drv["cdTabela"] = item.Servico.Codpad;
					drv["cdMascara"] = item.Servico.Codpsa;
					drv["dsServico"] = item.Servico.Descri;
					drv["obs"] = item.Observacao;
					drv["qtd"] = item.Quantidade;
					drv["opme"] = item.Opme ? "S" : "N";
					dt.Rows.Add(drv);
				}
			}
			return dt;
		}

		private void BindItem(DataListItem item, DataRowView drv) {
			TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
			TextBox txtMascara = (TextBox)item.FindControl("txtMascara");
			Label txtDescricao = item.FindControl("txtDescricao") as Label;
			TextBox txtQuantidade = item.FindControl("txtQuantidade") as TextBox;
			DropDownList dpdOpme = item.FindControl("dpdOpme") as DropDownList;
			Label lblCounter = item.FindControl("lblCounter") as Label;
			ImageButton btnRemover = item.FindControl("btnRemover") as ImageButton;
			HiddenField hidCodServico = item.FindControl("hidCodServico") as HiddenField;

            hidCodServico.Value = Convert.ToString(drv["cdTabela"]) + "|" + Convert.ToString(drv["cdMascara"]);
			txtObservacao.Text = Convert.ToString(drv["obs"]);
			txtMascara.Text = Convert.ToString(drv["cdMascara"]);
			txtDescricao.Text = Convert.ToString(drv["dsServico"]);
			txtQuantidade.Text = Convert.ToString(drv["qtd"]);
			dpdOpme.SelectedValue = Convert.ToString(drv["opme"]);

			ImageButton img2 = item.FindControl("btnBuscarDescricao") as ImageButton;

			if (CanEdit) {
				img2.OnClientClick = "return openPopServico('" + hidCodServico.ClientID + "',this);";
			} else {
				txtMascara.Enabled = false;
				btnRemover.Visible = false;
				img2.Visible = false;
			}
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

		private void Analisar() {
			int id = Int32.Parse(litProtocolo.Value);
			AutorizacaoVO vo = AutorizacaoBO.Instance.IniciarAnalise(id, UsuarioLogado.Id);
			this.ShowInfo("Solicitação em análise!");
			Bind(id);
		}

		private void Cotar() {
			int id = Int32.Parse(litProtocolo.Value);
			AutorizacaoVO vo = AutorizacaoBO.Instance.GetById(id);
			if (vo == null) {
				this.ShowError("Autorização não encontrada");
				return;
			}

			if (!vo.Opme) {
				this.ShowError("Esta solicitação não possui procedimentos OPME salvos! Não é possível iniciar a cotação!");
				return;
			}

			vo = AutorizacaoBO.Instance.IniciarCotacao(id, UsuarioLogado.Id);
			this.ShowInfo("Status alterado com sucesso!");
			Bind(id);
		}

		private void EnviarComentarios() {
			int id = Int32.Parse(litProtocolo.Value);
			string comentario = txtComentario.Text;
			AutorizacaoBO.Instance.EnviarComentarios(id, comentario, UsuarioLogado.Id);
			this.ShowInfo("Comentários enviados com sucesso!");
			Bind(id);
		}

		private void EnviarDocumentos() {
			AutorizacaoVO vo = new AutorizacaoVO();
			vo.Id = Int32.Parse(litProtocolo.Value);
			vo = AutorizacaoBO.Instance.GetById(vo.Id);

			List<AutorizacaoArquivoVO> lstArqs = new List<AutorizacaoArquivoVO>();
			foreach (ArquivoTela arq in Arquivos) {
				if (arq.New) {
					lstArqs.Add(new AutorizacaoArquivoVO()
					{
						NomeArquivo = arq.NomeSalvar
					});
				}
			}
			if (lstArqs.Count == 0) {
				this.ShowError("É necessário incluir um novo documento para poder reencaminhar o formulário!");
				return;
			}
			AutorizacaoBO.Instance.EnviarDocumentos(OrigemAutorizacao.GESTOR, vo, UsuarioLogado.Id, lstArqs);
			this.ShowInfo("Documentos enviados com sucesso! Aguarde enquanto o gestor analisa a solicitação!");

			Bind(vo.Id);
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar formulário! ", ex);
			}
		}

		protected void dpdDependente_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				if (string.IsNullOrEmpty(dpdDependente.SelectedValue)) {
					this.ShowError("Selecione o dependente!");
					return;
				}
				MostrarDependente();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao selecionar beneficiário! ", ex);
			}
		}

		protected void btnAdicionarItem_Click(object sender, EventArgs e) {
			AddNewRow();
		}

		protected void btnRemover_Click(object sender, ImageClickEventArgs e) {
			DataListItem row = (DataListItem)(sender as ImageButton).NamingContainer;
			RemoveRow(row.ItemIndex);
		}

		protected void btnIncluirArquivo_Click(object sender, EventArgs e) {
			try {
				string fisico = hidArqFisico.Value;
				string original = hidArqOrigem.Value;
				AddArquivo(fisico, original);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao incluir arquivo na lista!", ex);
			}
		}

		protected void btnRemoverArquivo_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				string nome = btn.CommandArgument;
				RemoverArquivo(nome);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao remover item da lista", ex);
			}
		}

		#region Locators

		protected void btnLocCred_Click(object sender, ImageClickEventArgs e) {
			string value = hidCredenciado.Value;
			if (NOT_FOUND_LOCATOR.Equals(value)) {
				hidCredenciado.Value = string.Empty;
				txtCpfCnpj.Enabled = true;
				txtRazaoSocial.Enabled = true;
				this.SetFocus(txtCpfCnpj);
			} else {
				int idCred = 0;
				if (!Int32.TryParse(value, out idCred)) {
					this.ShowError("Credenciado inválido!");
					return;
				}
                BindCred(value.Trim());
			}
		}

		protected void btnClrCred_Click(object sender, ImageClickEventArgs e) {
			hidCredenciado.Value = string.Empty;
			BindCred("");
		}

		protected void txtCpfCnpj_TextChanged(object sender, EventArgs e) {
			long cpfCnpj = 0;
			if (string.IsNullOrEmpty(txtCpfCnpj.Text)) {
				btnClrCred_Click(sender, null);
			} else {
				if (!Int64.TryParse(txtCpfCnpj.Text, out cpfCnpj)) {
					this.ShowError("O CPF/CNPJ deve ser numérico!");
					return;
				}
				DataTable dt = PRedeAtendimentoBO.Instance.Pesquisar(null, cpfCnpj.ToString(), false);
				if (dt == null || dt.Rows.Count == 0) {
					if (!txtRazaoSocial.Enabled)
						this.ShowError("Credenciado não encontrado! Verifique o cpf/cnpj ou utilize o sistema de busca!");
					return;
				}

				string cdCredenciado = Convert.ToString(dt.Rows[0]["BAU_CODIGO"]);
				BindCred(cdCredenciado);
			}
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
			}
			catch (Exception ex) {
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

		protected void btnLocHospital_Click(object sender, ImageClickEventArgs e) {
			string value = hidHospital.Value;
			if (NOT_FOUND_LOCATOR.Equals(value)) {
				hidHospital.Value = string.Empty;
				txtHospital.Enabled = true;
				this.SetFocus(txtHospital);
			} else {
				int idCred = 0;
				if (!Int32.TryParse(value, out idCred)) {
					this.ShowError("Hospital inválido!");
					return;
				}
                BindHospital(value.Trim());
			}
		}

		protected void btnClrHospital_Click(object sender, ImageClickEventArgs e) {
			hidHospital.Value = string.Empty;
			BindHospital("");
		}
		
		protected void btnClrDoenca_Click(object sender, ImageClickEventArgs e) {
			txtDoenca.Text = string.Empty;
		}

		protected void txtCartao_TextChanged(object sender, EventArgs e) {
			try {
				ChangeTitular();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar titular!", ex);
			}
		}

		#endregion

		protected void btnAnalisar_Click(object sender, EventArgs e) {
			try {
				Analisar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao iniciar análise!", ex);
			}
		}

		protected void btnCotar_Click(object sender, EventArgs e) {
			try {
				Cotar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao iniciar cotação!", ex);
			}
		}

		protected void btnEnviarDoc_Click(object sender, EventArgs e) {
			try {
				EnviarDocumentos();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao reenviar formulário!", ex);
			}
		}

		protected void rblAtendimento_SelectedIndexChanged(object sender, EventArgs e) {
			string value = rblAtendimento.SelectedValue;
			if (!"1".Equals(value)) {
				pnlMsgReembolso.Visible = true;
			} else {
				conteudo.Visible = true;
				divAtendimento.Visible = false;
			}
		}

		protected void btnEnviarComentario_Click(object sender, EventArgs e) {
			try {
				EnviarComentarios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao enviar análise!", ex);
			}
		}

	}
}