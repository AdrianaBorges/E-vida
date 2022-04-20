using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Forms {
	public partial class FormAutorizacaoProvisoria : FormPageBase {

		int[] PLANOS_ESPECIAIS = new int[] { PConstantes.PLANO_EVIDA_FAMILIA, PConstantes.PLANO_EVIDA_MELHOR_IDADE, PConstantes.PLANO_MAIS_VIDA_CEA };
		int[] PROCEDIMENTOS_PARA_ESPECIAIS = new int[] { 1, 2, 3, 4, 5 };
		string OBS_PARA_ESPECIAIS = "Cobertura  do Rol de Procedimentos e Eventos da ANS.";
		string OBS_FIXA_2 = "Necessária autorização prévia: Cirurgia, Intern., TC, RM, Cintilografia, Polissonografia, Terapias, Fisio buco-maxilo-facial, Trat.Dermat., Odonto, Monit.Epilepsia, DIU, Quimio, Sedação Profunda, Painel Hibrid. Mol., RPG, Hidroterapia e  Home Care.";

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdLocal.Enabled = false;

				for (int i = 1; i <= 7; i++) {
					chkAssistencias.Items.Add(new ListItem(AutorizacaoProvisoriaEnumTradutor.TraduzProcedimentos(i), i.ToString()));
				}

				if (Request["ID"] != null) {
					int id = Convert.ToInt32(Request["ID"]);
					Bind(id);
				}

			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO_PROVISORIA; }
		}

		private List<PlantaoSocialLocalVO> Locais {
			get { 
				List<PlantaoSocialLocalVO> lst = (List<PlantaoSocialLocalVO>)Session["PLANTAO_LOCAL"];
				if (lst == null) {
					lst = AutorizacaoProvisoriaBO.Instance.ListarPlantaoSocialLocal();
					Locais = lst;
				}
				return lst;
			}
			set { Session["PLANTAO_LOCAL"] = value; }
		}

		public int Id {
			get { return ViewState["ID"] == null ? 0 : (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private string LocalToDropId(PlantaoSocialLocalVO local) {
			return local.Id.ToString();
		}

		private List<int> GetProcedimentos() {
			List<int> lstPrcs = new List<int>();
			foreach (ListItem item in chkAssistencias.Items) {
				if (item.Selected)
					lstPrcs.Add(Convert.ToInt32(item.Value));
			}
			return lstPrcs;
		}

		private List<string> GetCoberturas() {
			List<string> lstPrcs = new List<string>();
			foreach (ListItem item in chkCobertura.Items) {
				if (item.Selected)
					lstPrcs.Add(item.Value);
			}
			return lstPrcs;
		}

		private void ClearLocais() {
			dpdLocal.Items.Clear();
		}

		private void BindLocais() {
			List<PlantaoSocialLocalVO> lstLocal = Locais;
			if (lstLocal != null) {
				lstLocal.Sort((x, y) => x.Cidade.CompareTo(y.Cidade));

				foreach (PlantaoSocialLocalVO local in lstLocal) {
					dpdLocal.Items.Add(new ListItem(local.Cidade + " - " + local.Uf, LocalToDropId(local)));
				}
			}
			dpdLocal.Items.Insert(0, new ListItem("SELECIONE", ""));
		}

		private void Bind(int id) {
			Id = id;
			btnPdf.Visible = false;
			AutorizacaoProvisoriaVO vo = AutorizacaoProvisoriaBO.Instance.GetById(id);
			if (vo == null) {
				this.ShowError("Solicitação não encontrada!");
				return;
			}
			hidProtocolo.Value = vo.CodSolicitacao.ToString();

			txtProtocolo.Text = vo.CodSolicitacao.ToString(AutorizacaoProvisoriaVO.FORMATO_PROTOCOLO);
			txtDataSolicitacao.Text = vo.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss");
			lblSituacao.Text = AutorizacaoProvisoriaEnumTradutor.TraduzSituacao(vo.Status);
			if (!string.IsNullOrEmpty(vo.MotivoCancelamento)) {
				lblMotivo.Text = " - " + vo.MotivoCancelamento;
			}

			hidCodint.Value = vo.Codint.ToString();
            hidCodemp.Value = vo.Codemp.ToString();
            hidMatric.Value = vo.Matric.ToString();
            hidTipreg.Value = vo.Tipreg.ToString();
			
            MostrarDependente();
			txtValidade.Text = vo.FimVigencia.ToString("dd/MM/yyyy");

			dpdLocal.SelectedValue = LocalToDropId(vo.Local);
			lblPlantaoSocial.Text = vo.Local.Telefone;

			foreach (ListItem item in chkAssistencias.Items) {
				item.Selected = vo.Procedimentos.Contains(Convert.ToInt32(item.Value));
			}

			dpdReciprocidade.SelectedValue = vo.IsReciprocidade ? "S" : "N";
			ChangeReciprocidade();
			foreach (ListItem item in chkCobertura.Items) {
				item.Selected = vo.Coberturas.Contains(item.Value);
			}
			rbAbrangencia.SelectedValue = ((char)vo.Abrangencia).ToString();
			txtObs.Text = vo.Observacao;
			
			if (vo.Status == StatusAutorizacaoProvisoria.NEGADO) {
				this.ShowInfo("O formulário foi negado/cancelado. Não é possível salvar as alterações!");
				btnSalvar.Visible = false;
			} else if (vo.Status == StatusAutorizacaoProvisoria.APROVADO) {
				this.ShowInfo("O formulário foi gerado. Não é possível salvar as alterações!");
				btnSalvar.Visible = false;
				btnPdf.Visible = true;
			}
			btnGerar.Visible = btnSalvar.Visible;
		}
		
		private void ClearDepData() {
			//txtNumCartao.Text = string.Empty;
			txtNomeDependente.Text = string.Empty;
			txtNomeTitular.Text = string.Empty;
			txtPlano.Text = string.Empty;
			txtMatricula.Text = string.Empty;
			dpdLocal.Enabled = false;
			lblPlantaoSocial.Text = string.Empty;
			ClearLocais();
		}

		private PlantaoSocialLocalVO GetLocalSelecionado() {
			string value = dpdLocal.SelectedValue;
			if (string.IsNullOrEmpty(value)) {
				return null;
			}
			int codLocal = Convert.ToInt32(value);
			PlantaoSocialLocalVO vo = Locais.Find(x => x.Id == codLocal);
			return vo;
		}

		private void ChangeCartao() {
			hidCodint.Value = string.Empty;
            hidCodemp.Value = string.Empty;
            hidMatric.Value = string.Empty;
            hidTipreg.Value = string.Empty;

			if (!string.IsNullOrEmpty(txtNumCartao.Text)) {
				PUsuarioVO vo = PUsuarioBO.Instance.GetUsuarioByCartao(txtNumCartao.Text);
				if (vo == null) {
					this.ShowError("Beneficiário não encontrado! Verifique novamente o número do cartão ou utilize o sistema de busca!");
				} else {
                    if (vo.Datblo != "        " && Int32.Parse(vo.Datblo) <= Int32.Parse(DateTime.Today.ToString("yyyyMMdd")))
                    {
						this.ShowError("O beneficiário está bloqueado não pode ter autorização provisória solicitada!");
						return;
					}
					hidCodint.Value = vo.Codint.ToString();
                    hidCodemp.Value = vo.Codemp.ToString();
                    hidMatric.Value = vo.Matric.ToString();
                    hidTipreg.Value = vo.Tipreg.ToString();
				}
			}
			MostrarDependente();
		}

		private void CheckPlantaoSocial() {
			lblPlantaoSocial.Visible = false;

			if (string.IsNullOrEmpty(hidMatric.Value))
				return;

			string codint = hidCodint.Value;
            string codemp = hidCodemp.Value;
            string matric = hidMatric.Value;
            string tipreg = hidTipreg.Value;

            PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
            PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(benef.Codint, benef.Codemp, benef.Matric, benef.Tipreg);

			bool isPlanoOK = !PLANOS_ESPECIAIS.Contains(Int32.Parse(benefPlano.Codpla));
			bool isCoirma = "S".Equals(dpdReciprocidade.SelectedValue);

			lblPlantaoSocial.Visible = isPlanoOK && !isCoirma;
		}

		private void MostrarDependente() {
			ClearDepData();
            if (!string.IsNullOrEmpty(hidMatric.Value))
            {
                string codint = hidCodint.Value;
                string codemp = hidCodemp.Value;
                string matric = hidMatric.Value;
                string tipreg = hidTipreg.Value;

                PUsuarioVO vo = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
                PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);                
                PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla);
                PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(codint, codemp, matric);

                txtNumCartao.Text = vo.Matant;
				txtPlano.Text = plano.Descri;
				txtNomeTitular.Text = titular.Nomusr;
				txtMatricula.Text = vo.Matemp.ToString();
				txtNomeDependente.Text = vo.Nomusr;

				int idPlano = Int32.Parse(plano.Codigo);
				dpdLocal.Enabled = true;
				BindLocais();

				if (txtObs.Text.StartsWith(OBS_PARA_ESPECIAIS)) {
					txtObs.Text = txtObs.Text.Substring(OBS_PARA_ESPECIAIS.Length).Trim();
				}
				if (txtObs.Text.StartsWith(OBS_FIXA_2)) {
					txtObs.Text = txtObs.Text.Substring(OBS_FIXA_2.Length).Trim();
				}
				if (PLANOS_ESPECIAIS.Contains(idPlano)) {
					chkAssistencias.Enabled = false;
					foreach (ListItem item in chkAssistencias.Items) {
						if (PROCEDIMENTOS_PARA_ESPECIAIS.Contains(Int32.Parse(item.Value))) {
							item.Selected = true;
						} else {
							item.Selected = false;
						}
					}

					string obsFixa = OBS_PARA_ESPECIAIS + "\n" + OBS_FIXA_2;
					if (!txtObs.Text.StartsWith(obsFixa)) {
						txtObs.Text = obsFixa + "\n" + txtObs.Text;
					}
				} else {
					chkAssistencias.Enabled = true;
					string obsFixa = OBS_FIXA_2;
					if (!txtObs.Text.StartsWith(obsFixa)) {
						txtObs.Text = obsFixa + "\n" + txtObs.Text;
					}
				}
			}
			CheckPlantaoSocial();
		}
		
		private void Salvar() {
			if (string.IsNullOrEmpty(hidMatric.Value)) {
				this.ShowError("Informe o dependente!");
				return;
			}
			if (string.IsNullOrEmpty(txtValidade.Text)) {
				this.ShowError("Informe a validade");
				return;
			}
			if (string.IsNullOrEmpty(dpdLocal.SelectedValue)) {
				this.ShowError("Informe o local para plantão social!");
				return;
			}

			DateTime dtValidade;
			if (!DateTime.TryParse(txtValidade.Text, out dtValidade)) {
				this.ShowError("Data de validade inválida!");
				return;
			}

			if (string.IsNullOrEmpty(dpdReciprocidade.SelectedValue)) {
				this.ShowError("Informe se a autorização será para coirmãs!");
				return;
			}

			if (string.IsNullOrEmpty(rbAbrangencia.SelectedValue)) {
				this.ShowError("Informe a abrangência!");
				return;
			}

			List<string> lstCobertura = GetCoberturas();
			if (lstCobertura.Count == 0) {
				this.ShowError("Informe ao menos uma cobertura!");
				return;
			}

			List<int> lstProcedimentos = GetProcedimentos();
			if (lstProcedimentos.Count == 0) {
				this.ShowError("Informe ao menos um procedimento!");
				return;
			}

			AutorizacaoProvisoriaVO vo = new AutorizacaoProvisoriaVO();
			vo.CodSolicitacao = Id;

            string codint = hidCodint.Value;
            string codemp = hidCodemp.Value;
            string matric = hidMatric.Value;
            string tipreg = hidTipreg.Value;

            PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(codint, codemp, matric, tipreg);
			vo.Codint = benef.Codint;
            vo.Codemp = benef.Codemp;
            vo.Matric = benef.Matric;
            vo.Tipreg = benef.Tipreg;
			
			vo.CodUsuarioCriacao = UsuarioLogado.Id;
			vo.FimVigencia = dtValidade;
			vo.Local = GetLocalSelecionado();

            PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(benef.Codint, benef.Codemp, benef.Matric, benef.Tipreg);
			PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(benefPlano.Codpla);
			vo.Plano = plano;

			vo.IsReciprocidade = dpdReciprocidade.SelectedValue.Equals("S");
			vo.Coberturas = lstCobertura;
			vo.Abrangencia = (AbrangenciaAutorizacaoProvisoria)rbAbrangencia.SelectedValue[0];

			vo.Procedimentos = lstProcedimentos;
			vo.Observacao = txtObs.Text;

			AutorizacaoProvisoriaBO.Instance.Salvar(vo);

			this.ShowInfo("Solicitação salva com sucesso!");
			Bind(vo.CodSolicitacao);
		}

		private void ChangeReciprocidade() {
			string selecao = dpdReciprocidade.SelectedValue;
			foreach (ListItem item in chkCobertura.Items) {
				item.Selected = false;
			}
			chkCobertura.Enabled = false;
			rbAbrangencia.Enabled = false;
			if (!string.IsNullOrEmpty(selecao)) {
				if (selecao.Equals("S")) {
					chkCobertura.Enabled = true;
					rbAbrangencia.Enabled = true;
				} else {
					string[] naoReciprocidade = new string[] { "AMB", "OBS", "ODO", "UrE"};
					foreach (ListItem item in chkCobertura.Items) {
						if (naoReciprocidade.Contains(item.Value))
							item.Selected = true;
					}
					rbAbrangencia.SelectedValue = "N";
				}
			}
			CheckPlantaoSocial();
		}

		protected void txtNumCartao_TextChanged(object sender, EventArgs e) {
			try {
				ChangeCartao();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar beneficiário!", ex);
			}
		}

		protected void dpdLocal_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				PlantaoSocialLocalVO vo = GetLocalSelecionado();
				if (vo == null)
					lblPlantaoSocial.Text = string.Empty;
				else
					lblPlantaoSocial.Text = vo.Telefone;
			}
			catch (Exception ex) {
				this.ShowError("Erro ao selecionar local.", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar!", ex);
			}
		}

		protected void btnBuscarBeneficiario_Click(object sender, ImageClickEventArgs e) {
			try {
				MostrarDependente();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar beneficiario!", ex);
			}
		}

		protected void dpdReciprocidade_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				ChangeReciprocidade();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao selecionar opção de reciprocidade!", ex);
			}
		}

		protected void btnGerar_Click(object sender, EventArgs e) {
			try {
				AutorizacaoProvisoriaVO vo = AutorizacaoProvisoriaBO.Instance.GetById(Id);
				ReportAutorizacaoProvisoria rpt = new ReportAutorizacaoProvisoria(ReportDir, UsuarioLogado);
                vo.CodUsuarioAprovacao = UsuarioLogado.Id;
				byte[] anexo = rpt.GerarRelatorio(vo);
				AutorizacaoProvisoriaBO.Instance.Aprovar(vo, UsuarioLogado.Id, anexo);
				Bind(Id);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao gerar autorização!", ex);
			}
		}
		
	}
}