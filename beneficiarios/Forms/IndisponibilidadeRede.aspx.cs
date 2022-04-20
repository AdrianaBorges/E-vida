using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Forms {
	public partial class IndisponibilidadeRede : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
                PUsuarioVO titular = UsuarioLogado.UsuarioTitular;

				dpdUf.DataSource = LocatorDataBO.Instance.ListarUf().Select(x => x.Sigla);
				dpdUf.DataBind();
				dpdUf.Items.Insert(0, new ListItem("SELECIONE", ""));

				txtEmail.Text = titular.Email;
				btnSalvar.Visible = false;

				List<EspecialidadeVO> lstEspecialidade = IndisponibilidadeRedeBO.Instance.ListarEspecialidades();
				dpdEspecialidade.DataSource = lstEspecialidade;
				dpdEspecialidade.DataBind();
				dpdEspecialidade.Items.Insert(0, new ListItem("SELECIONE", ""));

				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id = 0;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador de requisição inexistente!");
						return;
					}

					IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(id);
					if (vo == null) {
						this.ShowError("Identificador de requisição inválido!");
						return;
					}
					if (!CheckBeneficiario(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric)) {
						this.ShowError("Identificador não pertence à matrícula!");
						return;
					}
					Bind(vo);

				} else {
					CarregarBeneficiarios();
					btnEnviar.Visible = true;
					btnSalvar.Visible = false;
				}
			}

		}

		public int? Id {
			get { return ViewState["ID"] == null ? new int?() : Convert.ToInt32(ViewState["ID"]); }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int id) {
			IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(id);
			Bind(vo);
		}

		private void Bind(IndisponibilidadeRedeVO vo) {
			Id = vo.Id;
			litProtocolo.Value = vo.Id.ToString();

			lblProtocolo.Text = vo.Id.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO);
			lblProtocoloAns.Text = vo.ProtocoloAns;

			lblDataSolicitacao.Text = vo.DataCriacao.ToShortDateString();
			lblSituacao.Text = IndisponibilidadeRedeEnumTradutor.TraduzStatus(vo.Situacao);
			if (vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO)
				lblSituacao.Text += " - " + vo.MotivoEncerramento;

			lblPrazo.Text = vo.DiasPrazo + " dias";// - " + .ToShortDateString();

			lblAtraso.Visible = false;
			DateTime dtLimite = DateTime.Now.Date;
			if (vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO) {
				dtLimite = vo.DataSituacao.Date;
			}
			if (vo.DataCriacao.AddDays(vo.DiasPrazo) < dtLimite) {
				lblAtraso.Visible = true;
			}

			CarregarBeneficiarios();

            string cd_usuario = vo.Usuario.Codint.Trim() + "|" + vo.Usuario.Codemp.Trim() + "|" + vo.Usuario.Matric.Trim() + "|" + vo.Usuario.Tipreg.Trim();
            dpdBeneficiario.SelectedValue = cd_usuario;

			txtEmail.Text = vo.EmailContato;
			txtTelContato.Text = vo.TelefoneContato;

			dpdEspecialidade.SelectedValue = vo.IdEspecialidade.ToString();
			CarregarPrioridades();
			dpdPrioridade.SelectedValue = vo.Prioridade.ToString();

			if (vo.CpfCnpjCred!= null) {
				string strCpfCnpj = vo.CpfCnpjCred.ToString();
				if (strCpfCnpj.Length > 11)
					txtCpfCnpj.Text = FormatUtil.FormatCnpj(strCpfCnpj);
				else
					txtCpfCnpj.Text = FormatUtil.FormatCpf(strCpfCnpj);
			} else {
				txtCpfCnpj.Text = "";
			}
			txtRazaoSocial.Text = vo.RazaoSocialCred;
			
			if (vo.ValorSolicitacao != null) {
				txtValor.Text = FormatUtil.FormatDecimalForm(vo.ValorSolicitacao);
			}

			dpdUf.SelectedValue = vo.Uf;
			dpdUf_SelectedIndexChanged(dpdUf, EventArgs.Empty);
			dpdMunicipio.SelectedValue = vo.IdLocalidade != null ? vo.IdLocalidade.ToString() : string.Empty;

			dpdBeneficiario.Enabled = false;
			txtTelContato.Enabled = false;
			txtEmail.Enabled = false;
			dpdEspecialidade.Enabled = false;
			txtCpfCnpj.Enabled = false;
			txtRazaoSocial.Enabled = false;
			txtValor.Enabled = false;
			btnSalvar.Visible = false;
			btnIncluirArquivo.Visible = false;

			btnEnviar.Visible = false;
			dpdEspecialidade.Enabled = false;
			dpdPrioridade.Enabled = false;
			dvMensagemAlteracao.Visible = !btnIncluirArquivo.Visible;

			dpdUf.Enabled = dpdMunicipio.Enabled = false;

			CarregarArquivos();
			BindObs();
			txtObs.Enabled = true;

			btnIncluirArquivo.Visible = false;

			if (vo.Situacao == StatusIndisponibilidadeRede.ENCERRADO) {
				txtObs.Visible = false;
				btnSalvarObs.Visible = false;
			}
		}

		private void BindObs() {
			IEnumerable<IndisponibilidadeRedeObsVO> lst = IndisponibilidadeRedeBO.Instance.ListarObs(Id.Value);
			if (lst != null)
				lst = lst.Where(x => x.TipoObs == IndisponibilidadeRedeObsVO.TIPO_EXTERNO);
			gdvObs.DataSource = lst;
			gdvObs.DataBind();
			btnSalvarObs.Visible = true;
			gdvObs.Visible = true; 
			txtObs.Text = string.Empty;
		}

		private void CarregarBeneficiarios() {

            List<PUsuarioVO> lstUsuarios = UsuarioLogado.Usuarios;
            // nova autorizacao, filtrar apenas usuários ativos
            if (Id == null)
            {
                lstUsuarios = new List<PUsuarioVO>(lstUsuarios);
                lstUsuarios.RemoveAll(x => (x.Datblo != "        " && Int32.Parse(x.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))) && x.Tipusu != "T");
            }
            dpdBeneficiario.DataSource = lstUsuarios;
            dpdBeneficiario.DataBind();
            dpdBeneficiario.Items.Insert(0, new ListItem("SELECIONE", ""));

		}

		private void CarregarPrioridades() {
			dpdPrioridade.Items.Clear();
			if (!string.IsNullOrEmpty(dpdEspecialidade.SelectedValue)) {
				EspecialidadeVO vo = IndisponibilidadeRedeBO.Instance.GetEspecialidadeById(Int32.Parse(dpdEspecialidade.SelectedValue));

				dpdPrioridade.Items.Add(new ListItem("", "SELECIONE"));
				List<PrioridadeIndisponibilidadeRede> lstPrioridades = IndisponibilidadeRedeBO.Instance.GetPrioridadesEspecialidade(vo);
				foreach (PrioridadeIndisponibilidadeRede prioridade in lstPrioridades) {
					dpdPrioridade.Items.Add(new ListItem(IndisponibilidadeRedeEnumTradutor.TraduzPrioridade(prioridade), prioridade.ToString()));	
				}				
			}
		}

		private bool ValidateRequired() {
			List<ItemError> lst = new List<ItemError>();

			if (string.IsNullOrEmpty(dpdBeneficiario.SelectedValue)) {
				this.AddErrorMessage(lst, dpdBeneficiario, "Informe o beneficiário!");
			}
			if (string.IsNullOrEmpty(txtTelContato.Text)) {
				this.AddErrorMessage(lst, txtTelContato, "Informe o telefone de contato!");
			}
			if (string.IsNullOrEmpty(txtEmail.Text)) {
				this.AddErrorMessage(lst, txtEmail, "Informe o e-mail!");
			}
			if (string.IsNullOrEmpty(dpdEspecialidade.SelectedValue)) {
				this.AddErrorMessage(lst, dpdEspecialidade, "Informe a especialidade!");
			}
			if (string.IsNullOrEmpty(dpdPrioridade.SelectedValue)) {
				this.AddErrorMessage(lst, dpdPrioridade, "Informe a prioridade!");
			}
			if (string.IsNullOrEmpty(dpdUf.SelectedValue)) {
				this.AddErrorMessage(lst, dpdUf, "Informe a UF para atendimento!");
			}
			if (string.IsNullOrEmpty(dpdMunicipio.SelectedValue)) {
				this.AddErrorMessage(lst, dpdMunicipio, "Informe o Município para atendimento!");
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

			if (!string.IsNullOrEmpty(txtCpfCnpj.Text)) {
				long cnpj = 0;
				string str = FormatUtil.UnformatCnpj(txtCpfCnpj.Text);
				if (str.Length != 11 && str.Length != 14) {
					this.AddErrorMessage(lst, txtCpfCnpj, "O CPF/CNPJ dev ter 11 ou 14 dígitos!");
				} else if (!Int64.TryParse(str, out cnpj)) {
					this.AddErrorMessage(lst, txtCpfCnpj, "O CPF/CNPJ informado está em um formato inválido!");
				}
			}
			
			if (!string.IsNullOrEmpty(txtValor.Text)) {
				decimal d;
				if (!Decimal.TryParse(txtValor.Text, out d)) {
					this.AddErrorMessage(lst, txtValor, "O valor deve ser numérico!");
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

			IndisponibilidadeRedeVO vo = new IndisponibilidadeRedeVO();

            string[] dados_usuario = dpdBeneficiario.SelectedValue.Split('|');
            string codint = dados_usuario[0];
            string codemp = dados_usuario[1];
            string matric = dados_usuario[2];
            string tipreg = dados_usuario[3];

            vo.Usuario = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);

			vo.EmailContato = txtEmail.Text;
			vo.TelefoneContato = txtTelContato.Text;
			vo.Prioridade = IndisponibilidadeRedeEnumTradutor.TraduzPrioridade(dpdPrioridade.SelectedValue);
			vo.IdEspecialidade = Int32.Parse(dpdEspecialidade.SelectedValue);

			if (!string.IsNullOrEmpty(txtRazaoSocial.Text))
				vo.RazaoSocialCred = txtRazaoSocial.Text;
			if (!string.IsNullOrEmpty(txtCpfCnpj.Text))
				vo.CpfCnpjCred = Int64.Parse(FormatUtil.UnformatCnpj(txtCpfCnpj.Text));

			if (!string.IsNullOrEmpty(txtValor.Text)) {
				vo.ValorSolicitacao = Decimal.Parse(txtValor.Text);
			}

			vo.Uf = dpdUf.SelectedValue;
			vo.IdLocalidade = Convert.ToInt32(dpdMunicipio.SelectedValue);

			if (Id == null) {
				IndisponibilidadeRedeBO.Instance.CriarSolicitacao(vo, txtObs.Text, IndisponibilidadeRedeObsVO.ORIGEM_BENEF, Arquivos);
				this.ShowInfo("Solicitação criada com sucesso! Protocolo: <span style=\"font-size:x-large\"><b>" + vo.Id.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO) + "</b></span><br>" +
					"Data de abertura: <span style=\"font-size:x-large\"><b>" + vo.DataCriacao.ToString("dd/MM/yyyy") + "</b></span> <br>" +
					"Prazo estimado: <span style=\"font-size:x-large\"><b>" + vo.DiasPrazo + "</b> dias</span> ");
			} else {
				vo.Id = Id.Value;
				IndisponibilidadeRedeBO.Instance.SalvarSolicitacao(vo, null);
				this.ShowInfo("Alterações realizadas com sucesso!");
			}
			Bind(vo.Id);
		}

		private void SalvarObs() {
			if (Id == null) {
				this.ShowError("Observação deve ser inserida em um protocolo já existente.");
				return;
			}
			if (string.IsNullOrEmpty(txtObs.Text)) {
				this.ShowError("Informe a observação a ser incluída!");
				return;
			}
			IndisponibilidadeRedeObsVO vo = new IndisponibilidadeRedeObsVO();
			vo.IdIndisponibilidade = Id.Value;
			vo.CodUsuario = null;
			vo.Observacao = txtObs.Text;
			vo.Origem = IndisponibilidadeRedeObsVO.ORIGEM_BENEF;
			vo.TipoObs = IndisponibilidadeRedeObsVO.TIPO_EXTERNO;
			IndisponibilidadeRedeBO.Instance.IncluirObs(vo);
			this.ShowInfo("Observação Incluída com sucesso!");			
			BindObs();
		}

		#region Arquivos

		public List<ArquivoTelaVO> Arquivos {
			get {
				if (ViewState["ARQUIVOS"] == null) {
					Arquivos = new List<ArquivoTelaVO>();
				}
				return ViewState["ARQUIVOS"] as List<ArquivoTelaVO>;
			}
			set {
				ViewState["ARQUIVOS"] = value;
			}
		}

		private void CarregarArquivos() {
			dvArquivos.Visible = true;
			List<ArquivoTelaVO> lstArqs = null;
			if (Id != null) {
				List<IndisponibilidadeRedeArquivoVO> lstArquivos = IndisponibilidadeRedeBO.Instance.ListarArquivos(Id.Value);
				if (lstArquivos == null)
					lstArquivos = new List<IndisponibilidadeRedeArquivoVO>();

				lstArqs = lstArquivos.Where(x => x.TipoArquivo == TipoArquivoIndisponibilidadeRede.BENEFICIARIO).Select(x =>
					new ArquivoTelaVO() {
						Id = x.IdArquivo.ToString(),
						NomeTela = x.NomeArquivo,
						IsNew = false
					}).ToList();
				Arquivos = lstArqs;
			} else {
				lstArqs = Arquivos;
			}

			ltvArquivo.DataSource = lstArqs;
			ltvArquivo.DataBind();

			updArquivos.Update();
			btnIncluirArquivo.Visible = true;
		}

		private IndisponibilidadeRedeArquivoVO ArquivoTela2VO(ArquivoTelaVO telaVO) {
			IndisponibilidadeRedeArquivoVO vo = new IndisponibilidadeRedeArquivoVO();
			if (!string.IsNullOrEmpty(telaVO.Id)) {
				vo.IdArquivo = Int32.Parse(telaVO.Id);
			}
			vo.NomeArquivo = telaVO.NomeTela;
			return vo;
		}

		private void AddArquivo(string fisico, string original) {
			List<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO vo = new ArquivoTelaVO() {
				NomeFisico = fisico,
				NomeTela = original,
				IsNew = true,
				Parameters = new Dictionary<string,string>()
			};
			vo.Parameters.Add("TP_ARQUIVO", ((int)TipoArquivoIndisponibilidadeRede.BENEFICIARIO).ToString());
			bool contains = lstAtual.FindIndex(x => x.NomeTela.Equals(original, StringComparison.InvariantCultureIgnoreCase)) >= 0;
			if (!contains) {
				lstAtual.Add(vo);
			} else {
				this.ShowError("Este arquivo já existe na listagem! Por favor, exclua o antigo ou renomeie o arquivo novo!");
				return;
			}
			
			ltvArquivo.DataSource = lstAtual;
			ltvArquivo.DataBind();

			if (Id != null) {
				SalvarAddArquivo();
			} else {
				this.ShowInfo("Arquivo adicionado em tela! Arquivos só serão salvos quando formulário enviado!");
			}
		}

		private void RemoverArquivo(ListViewDataItem row) {
			int idx = row.DataItemIndex;

			List<ArquivoTelaVO> lstAtual = Arquivos;
			ArquivoTelaVO telaVO = lstAtual[idx];
			
			if (!telaVO.IsNew) {
				IndisponibilidadeRedeArquivoVO vo = ArquivoTela2VO(telaVO);
				IndisponibilidadeRedeBO.Instance.RemoverArquivo(vo);
			} else if (Id == null) {
				lstAtual.RemoveAt(idx);
				Arquivos = lstAtual;
			}
			CarregarArquivos();
		}

		private void SalvarAddArquivo() {
			ArquivoTelaVO telaVO = Arquivos.Last(x => x.IsNew);
			IndisponibilidadeRedeBO.Instance.SalvarArquivo(Id.Value, telaVO);

			CarregarArquivos();
			this.ShowInfo("Arquivo salvo com sucesso no sistema!");
		}

		protected void bntRemoverArquivo_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewDataItem row = (ListViewDataItem)btn.NamingContainer;
				RemoverArquivo(row);
			} catch (Exception ex) {
				this.ShowError("Erro ao remover item da lista", ex);
			}
		}

		protected void btnIncluirArquivo_Click(object sender, EventArgs e) {
			AddArquivo(hidArqFisico.Value, hidArqOrigem.Value);
		}

		#endregion
		
		protected void dpdEspecialidade_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				CarregarPrioridades();
			} catch (Exception ex) {
				this.ShowError("Erro ao carregar prioridade!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar!", ex);
			}
		}
		
		protected void dpdBeneficiario_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				if (string.IsNullOrEmpty(dpdBeneficiario.SelectedValue))
					return;
                DataTable dt = IndisponibilidadeRedeBO.Instance.BuscarIndisponibilidadeRede(UsuarioLogado.Codint, UsuarioLogado.Codemp, UsuarioLogado.Matric);
				if (dt != null && dt.Rows.Count > 0) {
					foreach (DataRow dr in dt.Rows) {

                        string cd_usuario = dr["BA1_CODINT"].ToString().Trim() + "|" + dr["BA1_CODEMP"].ToString().Trim() + "|" + dr["BA1_MATRIC"].ToString().Trim() + "|" + dr["BA1_TIPREG"].ToString().Trim();

                        if (dpdBeneficiario.SelectedValue == cd_usuario)
                        {
							StatusIndisponibilidadeRede status = (StatusIndisponibilidadeRede)Convert.ToInt32(dr["id_situacao"]);
							if (status != StatusIndisponibilidadeRede.ENCERRADO) {
								this.RegisterScript("benef", "showExistente('" + dr["cd_indisponibilidade"] + "');");
								return;
							}
						}
					}
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao verificar beneficiário!", ex);
			}
		}

		protected void btnSalvarObs_Click(object sender, EventArgs e) {
			try {
				SalvarObs();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar a observação.", ex);
			}
		}

		protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				dpdMunicipio.Items.Clear();
				string uf = dpdUf.SelectedValue;
				if (!string.IsNullOrEmpty(uf)) {
					DataTable dtMunicipios = PLocatorDataBO.Instance.BuscarMunicipiosProtheus(uf);
					dpdMunicipio.Items.AddRange(dtMunicipios.AsEnumerable().Select(x =>
                        new ListItem(Convert.ToString(x["BID_DESCRI"]), Convert.ToString(x["BID_CODMUN"]))).ToArray());					
				}
				dpdMunicipio.Items.Insert(0, new ListItem("SELECIONE", ""));
			} catch (Exception ex) {
				this.ShowError("Erro ao selecionar UF.", ex);
			}
		}

	}
}