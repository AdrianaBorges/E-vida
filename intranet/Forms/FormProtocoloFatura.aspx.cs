using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;

namespace eVidaIntranet.Forms {
	public partial class FormProtocoloFatura : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				if (string.IsNullOrEmpty(Request["ID"])) {
					this.ShowError("Para edição deve ser selecionado um protocolo!");
					return;
				}
				int cdProtocolo;
				if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
					this.ShowError("Protoco inválido!");
					return;
				}

				/*foreach (int i in Enum.GetValues(typeof(StatusProtocoloFatura))) {
					StatusProtocoloFatura sts = (StatusProtocoloFatura)i;
					if (sts == StatusProtocoloFatura.CANCELADO) continue;
					dpdSituacao.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzStatus((StatusProtocoloFatura)i), i.ToString()));
				}*/

                foreach (int i in Enum.GetValues(typeof(FaseProtocoloFatura)))
                {
                    FaseProtocoloFatura sts = (FaseProtocoloFatura)i;
                    dpdSituacao.Items.Add(new ListItem(ProtocoloFaturaEnumTradutor.TraduzFase((FaseProtocoloFatura)i), i.ToString()));
                }

				List<MotivoPendenciaVO> lstPendencias = MotivoPendenciaBO.Instance.ListarMotivos(TipoMotivoPendencia.PROTOCOLO_FATURA);
				dpdPendencia.DataSource = lstPendencias;
				dpdPendencia.DataBind();
				dpdPendencia.Items.Insert(0, new ListItem("SELECIONE", ""));

				Bind(cdProtocolo);
			}
		}

		protected override Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.PROTOCOLO_FATURA; }
		}

		public int Id {
			get { return (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int cdProtocolo) {
			ProtocoloFaturaVO vo = ProtocoloFaturaBO.Instance.GetById(cdProtocolo);
			if (vo == null) {
				this.ShowError("Procolo inexistente!");
				return;
			}
			Id = vo.Id;

			hidProtocolo.Value = vo.Id.ToString();

			litProtocolo.Text = vo.NrProtocolo;
			txtDataEntrada.Text = vo.DataEntrada.ToString("dd/MM/yyyy");
			txtDataEmissao.Text = FormatUtil.FormatDataForm(vo.DataEmissao);
			txtVencimento.Text = FormatUtil.FormatDataForm(vo.DataVencimento);

			txtValorApresentado.Text = FormatUtil.FormatDecimalForm(vo.ValorApresentado);

			hidCodCredenciado.Value = vo.RedeAtendimento.Codigo;
			txtCpfCnpj.Text = FormatUtil.FormatCpfCnpj(vo.RedeAtendimento.Tippe, vo.RedeAtendimento.Cpfcgc);
			txtRazaoSocial.Text = vo.RedeAtendimento.Nome;

            /*string natureza = "";
            List<PEspecialidadeVO> lista_especialidade = PLocatorDataBO.Instance.ListarEspecialidade(vo.RedeAtendimento.Codigo);
            if (lista_especialidade.Count > 0)
            {
                foreach(PEspecialidadeVO especialidade in lista_especialidade){
                    natureza += (natureza != "" ? ", " + especialidade.Descri : especialidade.Descri);
                }
            }
            else {
                natureza = "-";
            }*/

            string natureza = "";
            PRedeAtendimentoVO credenciado = PRedeAtendimentoBO.Instance.GetById(vo.RedeAtendimento.Codigo);
            if(credenciado != null){
                PClasseRedeAtendimentoVO classerede = PLocatorDataBO.Instance.GetClasseRedeAtendimento(credenciado.Tippre);
                if(classerede != null){
                    natureza = classerede.Descri;
                }
            }
            
            litNatureza.Text = natureza;

            KeyValuePair<string, string> regiaoAtend = PRedeAtendimentoBO.Instance.GetRegiaoRedeAtendimento(vo.RedeAtendimento.Codigo);
            litUnidade.Text = regiaoAtend.Value;

			txtDocFiscal.Text = vo.DocumentoFiscal;
			txtValorGlosa.Text = FormatUtil.FormatDecimalForm(vo.ValorGlosa);
			txtValorProcessado.Text = FormatUtil.FormatDecimalForm(vo.ValorProcessado);

			txtDataFinalizacao.Text = FormatUtil.FormatDataForm(vo.DataFinalizacao);
			txtDataExpedicao.Text = FormatUtil.FormatDataForm(vo.DataExpedicao);

			dpdPendencia.SelectedValue = vo.CdPendencia != null ? vo.CdPendencia.Value.ToString() : "";

			//UsuarioVO usuarioVO = UsuarioBO.Instance.GetUsuarioById(vo.CdUsuarioCriacao);
			lblAnalista.Text = string.Empty;
			if (vo.CdUsuarioResponsavel != null) {
				UsuarioVO analistaVO = UsuarioBO.Instance.GetUsuarioById(vo.CdUsuarioResponsavel.Value);
				lblAnalista.Text = analistaVO.Login + " (" + analistaVO.Nome + ")";
			}

			if (vo.Situacao == StatusProtocoloFatura.CANCELADO) 
            {
				/*string strIdSts = ((int)StatusProtocoloFatura.CANCELADO).ToString();
				if (dpdSituacao.Items.FindByValue(strIdSts) == null) {
					dpdSituacao.Items.Insert(0, new ListItem("CANCELADO", strIdSts));
				}
				dpdSituacao.Enabled = false;*/
				btnSalvar.Visible = false;
				lblMotivo.Visible = true;
				//lblMotivo.Text = vo.MotivoCancelamento;
			}
			//dpdSituacao.SelectedValue = ((int)vo.Situacao).ToString();
            dpdSituacao.SelectedValue = ((int)vo.Fase).ToString();

			if (vo.Situacao != StatusProtocoloFatura.PROTOCOLADO) {
				btnCapa.Visible = true;
			} else {
				btnCapa.Visible = false;
			}
			if (vo.CdUsuarioResponsavel != null && vo.CdUsuarioResponsavel.Value == UsuarioLogado.Id) {
				btnSalvar.Visible = true;
				btnAssumir.Visible = false;
			} else {
				btnSalvar.Visible = false;
				btnAssumir.Visible = true;
			}

            BindFinanceiro(cdProtocolo);

		}

        private void BindFinanceiro(int cdProtocolo)
        {
            DataTable dt = PLocatorDataBO.Instance.BuscarPegFinanceiro(cdProtocolo);
            if (dt.Rows.Count == 0)
            {
                lblFinanceiro.Visible = true;
                return;
            }

            String valor = "&nbsp;";
            String descont = "&nbsp;";
            String valliq = "&nbsp;";
            String baixa = "&nbsp;";
            String irrf = "&nbsp;";
            String cofins = "&nbsp;";
            String pis = "&nbsp;";
            String csll = "&nbsp;";
            String iss = "&nbsp;";

            if (dt.Rows[0]["E2_VALOR"].ToString().Trim() != "" && dt.Rows[0]["E2_VALOR"].ToString().Trim() != "0") valor = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_VALOR"].ToString().Trim()));
            if (dt.Rows[0]["E2_DESCONT"].ToString().Trim() != "" && dt.Rows[0]["E2_DESCONT"].ToString().Trim() != "0") descont = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_DESCONT"].ToString().Trim()));
            if (dt.Rows[0]["E2_VALLIQ"].ToString().Trim() != "" && dt.Rows[0]["E2_VALLIQ"].ToString().Trim() != "0") valliq = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_VALLIQ"].ToString().Trim()));
            if (dt.Rows[0]["E2_BAIXA"].ToString().Trim() != "") baixa = DateTime.ParseExact(dt.Rows[0]["E2_BAIXA"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
            if (dt.Rows[0]["E2_IRRF"].ToString().Trim() != "" && dt.Rows[0]["E2_IRRF"].ToString().Trim() != "0") irrf = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_IRRF"].ToString().Trim()));
            if (dt.Rows[0]["E2_COFINS"].ToString().Trim() != "" && dt.Rows[0]["E2_COFINS"].ToString().Trim() != "0") cofins = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_COFINS"].ToString().Trim()));
            if (dt.Rows[0]["E2_PIS"].ToString().Trim() != "" && dt.Rows[0]["E2_PIS"].ToString().Trim() != "0") pis = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_PIS"].ToString().Trim()));
            if (dt.Rows[0]["E2_CSLL"].ToString().Trim() != "" && dt.Rows[0]["E2_CSLL"].ToString().Trim() != "0") csll = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_CSLL"].ToString().Trim()));
            if (dt.Rows[0]["E2_ISS"].ToString().Trim() != "" && dt.Rows[0]["E2_ISS"].ToString().Trim() != "0") iss = FormatUtil.FormatDecimalForm(Decimal.Parse(dt.Rows[0]["E2_ISS"].ToString().Trim()));


            litValor.Text = valor;
            litDescont.Text = descont;
            litValliq.Text = valliq;
            litBaixa.Text = baixa;
            litIrrf.Text = irrf;
            litCofins.Text = cofins;
            litPis.Text = pis;
            litCsll.Text = csll;
            litIss.Text = iss;

        }

		private void MostrarCredenciado() {
			txtCpfCnpj.Text = string.Empty;
			txtRazaoSocial.Text = string.Empty;

			if (!string.IsNullOrEmpty(hidCodCredenciado.Value)) {
                PRedeAtendimentoVO vo = PRedeAtendimentoBO.Instance.GetById(hidCodCredenciado.Value);
				txtCpfCnpj.Text = FormatUtil.FormatCpfCnpj(vo.Tippe, vo.Cpfcgc);
				txtRazaoSocial.Text = vo.Nome;
			}
		}

		private bool IsValidDate(DateTime? dt, bool canBeFuture) {
			if (dt == null) return true;
			if (dt.Value.Year < 2012) return false;
			if (dt.Value > DateTime.Now && !canBeFuture) {
				return false;
			}
			return true;
		}

		private void Salvar() {
			int codCredenciado;
			DateTime dtEntrada;
			string docFiscal;
			decimal? vlApresentado = null;
			DateTime? dtEmissao = null;
			DateTime dtVencimento;
			decimal? vlProcessado = null;
			decimal? vlGlosa = null;
			int? cdPendencia = null;
			DateTime? dtFinalizacao = null;
			DateTime? dtExpedicao = null;

			if (string.IsNullOrEmpty(hidCodCredenciado.Value)) {
				this.ShowError("Selecione o credenciado!");
				return;
			}
			if (!Int32.TryParse(hidCodCredenciado.Value, out codCredenciado)) {
				this.ShowError("Credenciado inválido!");
				return;
			}
			if (string.IsNullOrEmpty(txtDataEntrada.Text)) {
				this.ShowError("Informe a data de entrada!");
				return;
			}
			if (!DateTime.TryParse(txtDataEntrada.Text, out dtEntrada)) {
				this.ShowError("Data de entrada inválida!");
				return;
			} else {
				if (!IsValidDate(dtEntrada, false)) {
					this.ShowError("A data de entrada não pode ser antes de 2012 e não pode ser futura!");
					return;
				}
			}
			if (!string.IsNullOrEmpty(txtValorApresentado.Text)) {
				decimal d;
				if (!Decimal.TryParse(txtValorApresentado.Text, out d)) {
					this.ShowError("Valor apresentado inválido!");
					return;
				}
				vlApresentado = d;
			}

			if (!string.IsNullOrEmpty(txtDataEmissao.Text)) {
				DateTime dt;
				if (!DateTime.TryParse(txtDataEmissao.Text, out dt)) {
					this.ShowError("Data de emissão inválida!");
					return;
				}
				dtEmissao = dt;
				if (!IsValidDate(dtEmissao, false)) {
					this.ShowError("A data de emissão não pode ser antes de 2012 e não pode ser futura!");
					return;
				}
			}
			if (!string.IsNullOrEmpty(txtVencimento.Text)) {
				DateTime dt;
				if (!DateTime.TryParse(txtVencimento.Text, out dt)) {
					this.ShowError("Data de vencimento inválida!");
					return;
				}
				dtVencimento = dt;
				if (!IsValidDate(dtVencimento, true)) {
					this.ShowError("A data de vencimento não pode ser antes de 2012!");
					return;
				}
			} else {
				this.ShowError("A data de vencimento é obrigatória!");
				return;
			}
			if (!string.IsNullOrEmpty(txtValorProcessado.Text)) {
				decimal d;
				if (!Decimal.TryParse(txtValorProcessado.Text, out d)) {
					this.ShowError("Valor processado inválido!");
					return;
				}
				vlProcessado = d;
			}
			if (!string.IsNullOrEmpty(txtValorGlosa.Text)) {
				decimal d;
				if (!Decimal.TryParse(txtValorGlosa.Text, out d)) {
					this.ShowError("Valor de Glosa inválido!");
					return;
				}
				vlGlosa = d;
			}
			if (!string.IsNullOrEmpty(txtDataFinalizacao.Text)) {
				DateTime dt;
				if (!DateTime.TryParse(txtDataFinalizacao.Text, out dt)) {
					this.ShowError("Data de finalizacao inválida!");
					return;
				}
				dtFinalizacao = dt;
				if (!IsValidDate(dtFinalizacao, true)) {
					this.ShowError("A data de finalização não pode ser antes de 2012!");
					return;
				}
			}
			if (!string.IsNullOrEmpty(txtDataExpedicao.Text)) {
				DateTime dt;
				if (!DateTime.TryParse(txtDataExpedicao.Text, out dt)) {
					this.ShowError("Data de expedicao inválida!");
					return;
				}
				dtExpedicao = dt;
				if (!IsValidDate(dtExpedicao, true)) {
					this.ShowError("A data de expedição não pode ser antes de 2012!");
					return;
				}
			}
			if (!string.IsNullOrEmpty(dpdPendencia.SelectedValue)) {
				cdPendencia = Int32.Parse(dpdPendencia.SelectedValue);
			}

			docFiscal = txtDocFiscal.Text;

			ProtocoloFaturaVO vo = new ProtocoloFaturaVO();
			vo.Id = Id;
			vo.RedeAtendimento = new PRedeAtendimentoVO();
            vo.RedeAtendimento.Codigo = codCredenciado.ToString();
			vo.DataEntrada = dtEntrada;
			vo.DataEmissao = dtEmissao;
			vo.DocumentoFiscal = docFiscal;
			vo.ValorApresentado = vlApresentado;
			vo.CdUsuarioResponsavel = UsuarioLogado.Id;
			vo.DataVencimento = dtVencimento;
			vo.Situacao = (StatusProtocoloFatura)Int32.Parse(dpdSituacao.SelectedValue);
			vo.ValorProcessado = vlProcessado;
			vo.ValorGlosa = vlGlosa;
			vo.CdPendencia = cdPendencia;
			vo.DataFinalizacao = dtFinalizacao;
			vo.DataExpedicao = dtExpedicao;			
			
			ProtocoloFaturaBO.Instance.Salvar(vo);

			this.ShowInfo("Protocolo editado com sucesso!");
			Bind(vo.Id);
		}

		private void AssumirResponsabilidade() {
			ProtocoloFaturaBO.Instance.Assumir(Id, UsuarioLogado.Id);
			this.ShowInfo("Responsabilidade assumida. Agora é possível salvar as alterações!");
			lblAnalista.Text = UsuarioLogado.Usuario.Login + " (" + UsuarioLogado.Usuario.Nome + ")";
			btnSalvar.Visible = true;
			btnAssumir.Visible = false;
		}

		protected void btnBuscarCredenciado_Click(object sender, ImageClickEventArgs e) {
			try {
				MostrarCredenciado();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar credenciado!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao gerar protocolo!", ex);
			}
		}

		protected void btnAssumir_Click(object sender, EventArgs e) {
			try {
				AssumirResponsabilidade();
			} catch (Exception ex) {
				this.ShowError("Erro ao assumir responsabilidade!", ex);
			}
		}
	}
}