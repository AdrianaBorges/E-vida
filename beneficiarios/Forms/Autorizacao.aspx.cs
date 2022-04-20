using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Upload;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaBeneficiarios.Forms {
	public partial class Autorizacao : FormPageBase {
		[Serializable]
		public class ArquivoTela {
			public bool New { get; set; }
			public string NomeFisico { get; set; }
			public string NomeSalvar { get; set; }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				
				UploadConfig config = UploadConfigManager.GetConfig(UploadFilePrefix.AUTORIZACAO);
				ltFormatoUpload.Text = config.FileTypes.Aggregate((x, y) => x + ", " + y);
				ltTamUpload.Text = (config.MaxSize/1024).ToString();

                txtNomeTitular.Text = UsuarioLogado.UsuarioTitular.Nomusr;
				
				List<PUsuarioVO> lstBenefs = UsuarioLogado.Usuarios;
				// nova autorizacao, filtrar apenas beneficiarios ativos
				if (string.IsNullOrEmpty(Request["ID"])) {
					lstBenefs = new List<PUsuarioVO>(lstBenefs);
                    lstBenefs.RemoveAll(x => (x.Datblo != "        " && Int32.Parse(x.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))) && x.Tipusu != "T");
				}
				dpdDependente.DataSource = lstBenefs;
				dpdDependente.DataBind();
				dpdDependente.Items.Insert(0, new ListItem("SELECIONE", ""));				

				dpdTipoConselho.DataSource = PLocatorDataBO.Instance.ListarConselhoProfissional();
				dpdTipoConselho.DataBind();

				dpdUfConselho.DataSource = PLocatorDataBO.Instance.ListarUf();
				dpdUfConselho.DataBind();

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
					CanAddFile = true;
					btnSalvar.Visible = true;
					btnEnviarDoc.Visible = false;

					//divAtendimento.Visible = true;
					//conteudo.Visible = false;
					divAtendimento.Visible = false;
					conteudo.Visible = true;
				}

			}

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
            if (vo == null || !CheckBeneficiario(vo.Usuario.Codint.Trim(), vo.Usuario.Codemp.Trim(), vo.Usuario.Matric.Trim()))
            {
				this.ShowError("Identificador de requisição inválido!");
				return;
			}
			txtProtocoloANS.Text = vo.ProtocoloAns;

			CanEdit = vo.Status == StatusAutorizacao.ENVIADA;
			CanAddFile = vo.Status == StatusAutorizacao.SOLICITANDO_DOC;

			txtProtocolo.Text = vo.Id.ToString(AutorizacaoVO.FORMATO_PROTOCOLO);
			txtDataSolicitacao.Text = vo.DataSolicitacao.ToString("dd/MM/yyyy HH:mm");
			txtStatus.Text = AutorizacaoTradutorHelper.TraduzStatus(vo.Status);
			txtDataAutorizacao.Text = vo.DataAutorizacao != null ? vo.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm") : "";

			txtDataSolRevalidacao.Text = vo.DataSolRevalidacao != null ? vo.DataSolRevalidacao.Value.ToString("dd/MM/yyyy HH:mm") : "";
			txtDataAprovRevalidacao.Text = vo.DataAprovRevalidacao != null ? vo.DataAprovRevalidacao.Value.ToString("dd/MM/yyyy HH:mm") : "";

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

				if (!string.IsNullOrEmpty(vo.ObsAprovacao.Trim())) {
					lblMotivo.Text = vo.ObsAprovacao;
					lblMotivo.Visible = true;
				}
			} else if (vo.Status == StatusAutorizacao.NEGADA && vo.CodNegativa != null) {
				lnkNeg.Visible = true;
				lnkNeg.OnClientClick = "return openNegPdf(" + vo.CodNegativa.Value + ");";
			} else if (vo.Status == StatusAutorizacao.CANCELADA) {
				lblMotivo.Visible = true;
				lblMotivo.Text = " - " + vo.MotivoCancelamento;
			}

            dpdDependente.SelectedValue = vo.Usuario.Codint.Trim() + "|" + vo.Usuario.Codemp.Trim() + "|" + vo.Usuario.Matric.Trim() + "|" + vo.Usuario.Tipreg.Trim();
			dpdDependente_SelectedIndexChanged(null, null);

			txtCpfCnpj.Enabled = false;
			txtRazaoSocial.Enabled = false;
			if (vo.RedeAtendimento != null) {
                if (!string.IsNullOrEmpty(vo.RedeAtendimento.Cpfcgc.Trim()))
                {
					string strCpfCnpj = vo.RedeAtendimento.Cpfcgc.Trim();
					if (strCpfCnpj.Length > 11)
						txtCpfCnpj.Text = FormatUtil.FormatCnpj(strCpfCnpj);
					else
						txtCpfCnpj.Text = FormatUtil.FormatCpf(strCpfCnpj);
				} else {
					txtCpfCnpj.Text = "";
				}

                txtRazaoSocial.Text = vo.RedeAtendimento.Nome;
                if (!string.IsNullOrEmpty(vo.RedeAtendimento.Codigo.Trim()))
                {
                    hidCredenciado.Value = vo.RedeAtendimento.Codigo;
				}
			}

			txtNomeProfissional.Enabled = false;
			txtNroConselho.Enabled = false;
			dpdTipoConselho.Enabled = false;

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

            hidDoenca.Value = vo.CodDoenca.Trim();
            txtDoenca.Text = vo.CodDoenca;

			if (vo.DataInternacao != null)
				txtDataInternacao.Text = vo.DataInternacao.Value.ToShortDateString();

			if (vo.Hospital != null) {
                hidHospital.Value = vo.Hospital.Codigo.Trim();
				txtHospital.Text = vo.Hospital.Nome;
			}
			txtIndicacao.Text = vo.IndicacaoClinica;

            dpdTfd.SelectedValue = Convert.ToBoolean(vo.Tfd) ? "S" : "N";

            if (vo.DataInicioTfd != null)
                txtDataInicioTfd.Text = vo.DataInicioTfd.Value.ToShortDateString();

            if (vo.DataTerminoTfd != null)
                txtDataTerminoTfd.Text = vo.DataTerminoTfd.Value.ToShortDateString();

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
			
			if (CanEdit) {
				btnSalvar.Visible = true;
				btnEnviarDoc.Visible = false;
			} else {
				dpdDependente.Enabled = false;
				btnLocCred.Visible = false;
				btnLocDoenca.Visible = false;
				btnLocProf.Visible = false;
				btnClrCred.Visible = false;
				btnClrDoenca.Visible = false;
				btnClrProf.Visible = false;

				if (!CanAddFile) {
					btnIncluirArquivo.Visible = false;
					btnEnviarDoc.Visible = false;
				}
			}
			divAtendimento.Visible = false;
			conteudo.Visible = true;
		}

        private PProfissionalSaudeVO BindFromHidProfissional()
        {
			string[] values = hidProfissional.Value.Split(';');
            string nrConselho = values[0];
            string cdConselho = values[1];
            string cdUf = values[2];

            PProfissionalSaudeVO Profissional = PLocatorDataBO.Instance.GetProfissional(nrConselho, cdUf, cdConselho);
			if (Profissional == null) {
				this.ShowError("Profissional não encontrado!");
				return null;
			}
            Profissional.Numcr = nrConselho;
            Profissional.Codsig = cdConselho;
            Profissional.Estado = cdUf;

			txtNomeProfissional.Text = Profissional.Nome;
            txtNroConselho.Text = nrConselho;
            dpdTipoConselho.SelectedValue = cdConselho;
            dpdUfConselho.SelectedValue = cdUf;

			return Profissional;
		}

		private void MostrarDependente() {
			string[] dados_beneficiario = dpdDependente.SelectedValue.Split('|');
            string codint = dados_beneficiario[0];
            string codemp = dados_beneficiario[1];
            string matric = dados_beneficiario[2];
            string tipreg = dados_beneficiario[3];

            PUsuarioVO vo = UsuarioLogado.Usuarios.Find(x => x.Codint.Trim() == codint.Trim() && x.Codemp.Trim() == codemp.Trim() && x.Matric.Trim() == matric.Trim() && x.Tipreg.Trim() == tipreg.Trim());
            PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(codint, codemp, matric, tipreg);
			PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla);
			txtNumCartao.Text = vo.Matant;
			txtPlano.Text = plano.Descri;
		}
						
		private bool ValidateRequired() {
			List<ItemError> lst = new List<ItemError>();
			if (string.IsNullOrEmpty(dpdDependente.SelectedValue)) {
				this.AddErrorMessage(lst, dpdDependente, "Selecione o beneficiário!");
			}
			/*
			if (string.IsNullOrEmpty(hidCredenciado.Value)) {
				if (string.IsNullOrEmpty(txtCpfCnpj.Text)) {
					this.AddErrorMessage(lst, txtCpfCnpj, "Informe o cpf/cnpj do credenciado!");
				}
				if (string.IsNullOrEmpty(txtRazaoSocial.Text)) {
					this.AddErrorMessage(lst, txtRazaoSocial, "Informe o nome/razão social do credenciado!");
				}
			}
			*/
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
            vo.UsuarioTitular = UsuarioLogado.UsuarioTitular;

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
			List<AutorizacaoArquivoVO> lstArqs = Arquivos.Select(x => new AutorizacaoArquivoVO()
			{
				NomeArquivo = x.NomeSalvar,
				CodAutorizacao = x.New ? 0 : vo.Id
			}).ToList();


            AutorizacaoBO.Instance.Salvar(vo, lstArqs, UsuarioLogado.UsuarioTitular);

			this.btnSalvar.Visible = false;

			this.ShowInfo("Solicitação de Autorização cadastrada com sucesso! Acompanhe através do portal ou e-mail!");
			try {
				this.RegisterScript("OK", "isMsgSalvaOK = true;");
				//Bind(vo.Id);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao recarregar formulário!");
				Log.Error("Erro ao recarregar formulário.", ex);
			}
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
			AutorizacaoBO.Instance.EnviarDocumentos(OrigemAutorizacao.BENEF, vo, null, lstArqs);
			this.ShowInfo("Documentos enviados com sucesso! Aguarde enquanto o gestor analisa a solicitação!");

			Bind(vo.Id);
		}

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

		private void BindCred(string idCred) {
			txtCpfCnpj.Enabled = false;
			txtRazaoSocial.Enabled = false;
			if (!string.IsNullOrEmpty(idCred)) {
				PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idCred);
                txtCpfCnpj.Text = vo.Tippe == PConstantes.PESSOA_JURIDICA ? (vo.Cpfcgc != null ? FormatUtil.FormatCnpj(vo.Cpfcgc) : "") : (vo.Cpfcgc != null ? FormatUtil.FormatCpf(vo.Cpfcgc) : "");
				txtRazaoSocial.Text = vo.Nome;
			} else {
				txtCpfCnpj.Text = "";
				txtRazaoSocial.Text = "";
			}
		}

		private void BindHospital(string idHospital) {
			txtHospital.Enabled = false;
			if (!string.IsNullOrEmpty(idHospital)) {
                PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(idHospital);
				txtHospital.Text = vo.Nome;
			} else {
				txtHospital.Text = "";
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

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar formulário! " + ex.Message);
				Log.Error("Erro ao salvar formulário.", ex);
			}
		}
		
		protected void dpdDependente_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				MostrarDependente();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao selecionar beneficiário! " + ex.Message);
				Log.Error("Erro ao selecionar beneficiário", ex);
			}
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

		protected void btnClrDoenca_Click(object sender, ImageClickEventArgs e) {
			txtDoenca.Text = string.Empty;
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

		protected void btClrHospital_Click(object sender, ImageClickEventArgs e) {
			hidHospital.Value = string.Empty;
			BindHospital("");
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

	}
}