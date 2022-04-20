using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class FormProtocoloFaturaNova : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.PROTOCOLO_FATURA; }
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

        /*protected void btnGerar_Click(object sender, EventArgs e) {
            try {
                Gerar();
            } catch (Exception ex) {
                this.ShowError("Erro ao gerar protocolo!", ex);
            }
        }

		private void Gerar() {
			int codCredenciado;
			DateTime dtEntrada;
			string docFiscal;
			decimal? vlApresentado = null;
			DateTime? dtEmissao = null;
			DateTime? dtVencimento = null;

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
			if (trVencimento.Visible) {
				DateTime dt;
				if (string.IsNullOrEmpty(txtDataVencimento.Text)) {
					this.ShowError("O credenciado não possui fórmula de cálculo, é obrigatório informar a data de vencimento!");
					return;
				}
				if (!DateTime.TryParse(txtDataVencimento.Text, out dt)) {
					this.ShowError("Data de vencimento inválida!");
					return;
				}
				dtVencimento = dt; 
				if (!IsValidDate(dtVencimento, true)) {
					this.ShowError("A data de vencimento não pode ser antes de 2012!");
					return;
				}
			}

			docFiscal = txtDocFiscal.Text;
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

			ProtocoloFaturaVO vo = new ProtocoloFaturaVO();
			vo.RedeAtendimento = new PRedeAtendimentoVO();
            vo.RedeAtendimento.Codigo = hidCodCredenciado.Value.Trim();
			vo.DataEntrada = dtEntrada;
			vo.DataEmissao = dtEmissao;
			vo.DocumentoFiscal = docFiscal;
			vo.ValorApresentado = vlApresentado;
			vo.DataVencimento = dtVencimento == null ? DateTime.MinValue : dtVencimento.Value;

			ProtocoloFaturaBO.Instance.Gerar(vo, UsuarioLogado.Id);
			this.ShowInfo("PROTOCOLO Nº <b>" + vo.Id.ToString(ProtocoloFaturaVO.FORMATO_PROTOCOLO) + "/" + vo.AnoEntrada + "</b><br/>" +
				" GERADO COM SUCESSO!");
				//"NATUREZA: " + vo.Natureza + "<br />" +
				//"DATA DE ENTRADA: " + vo.DataEntrada.ToString("dd/MM/yyyy") + "<br />" +
				//"USUÁRIO DO PROTOCOLO: " + UsuarioLogado.Username + "</B>");
			hidProtocolo.Value = vo.Id.ToString();
			btnImprimir.Visible = true;
			btnGerar.Visible = false;
			btnEditar.Visible = true;
		}*/

		protected void btnBuscarCredenciado_Click(object sender, ImageClickEventArgs e) {
			try {
				MostrarCredenciado();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar credenciado!", ex);
			}
		}

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar(true);
        }

        private void Buscar(bool msg) 
        {
            String codRda = "";
            DateTime? dataEntrada = null;
            decimal? valorApresentado = null;
            DateTime? dataEmissao = null;
            DateTime? dataVencimento = null;

            if (string.IsNullOrEmpty(txtCpfCnpj.Text)
                && string.IsNullOrEmpty(txtDataEntrada.Text)
                && string.IsNullOrEmpty(txtDocFiscal.Text)
                && string.IsNullOrEmpty(txtValorApresentado.Text)
                && string.IsNullOrEmpty(txtDataEmissao.Text)
                && string.IsNullOrEmpty(txtDataVencimento.Text))
            {
                this.ShowError("Por favor, informe pelo menos um campo de filtro!");
                return;
            }
			
			txtCpfCnpj.Text = FormatUtil.OnlyNumbers(txtCpfCnpj.Text);

            if (!string.IsNullOrEmpty(txtCpfCnpj.Text))
            {
                long l = 0;
                if (!Int64.TryParse(txtCpfCnpj.Text, out l))
                {
                    this.ShowError("O CPF/CNPJ deve ser numérico");
                    return;
                }

                PRedeAtendimentoVO credenciado = PRedeAtendimentoBO.Instance.GetByDoc(txtCpfCnpj.Text);
                if (credenciado != null) 
                {
                    codRda = credenciado.Codigo;
                }
            }

            if (!string.IsNullOrEmpty(txtDataEntrada.Text))
            {
                DateTime data;
                if (!DateTime.TryParse(txtDataEntrada.Text, out data))
                {
                    this.ShowError("Data de entrada inválida!");
                    return;
                }
                dataEntrada = data;
            }

            if (!string.IsNullOrEmpty(txtDocFiscal.Text))
            {
                Int64 d;
                if (!Int64.TryParse(txtDocFiscal.Text, out d))
                {
                    this.ShowError("Documento fiscal inválido!");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(txtValorApresentado.Text))
            {
                decimal d;
                if (!Decimal.TryParse(txtValorApresentado.Text.Replace('.', ','), out d))
                {
                    this.ShowError("Valor apresentado inválido!");
                    return;
                }
                valorApresentado = d;
            }

            if (!string.IsNullOrEmpty(txtDataEmissao.Text))
            {
                DateTime data;
                if (!DateTime.TryParse(txtDataEmissao.Text, out data))
                {
                    this.ShowError("Data de emissão inválida!");
                    return;
                }
                dataEmissao = data;
            }

            if (!string.IsNullOrEmpty(txtDataVencimento.Text))
            {
                DateTime data;
                if (!DateTime.TryParse(txtDataVencimento.Text, out data))
                {
                    this.ShowError("Data de vencimento inválida!");
                    return;
                }
                dataVencimento = data;
            }

            DataTable dt = PLocatorDataBO.Instance.BuscarPeg(codRda, dataEntrada, txtDocFiscal.Text, valorApresentado, dataEmissao, dataVencimento);
            if (dt.Rows.Count > 300)
            {
                dt = dt.AsEnumerable().Take(300).CopyToDataTable();
                if(msg){
                    this.ShowInfo("Foram retornados apenas os 300 primeiros resultados da pesquisa. Por favor informe mais detalhes!");
                }
                
            }
            gdvProtocolo.DataSource = dt;
            gdvProtocolo.DataBind();

            if (dt.Rows.Count == 0) {
                if (msg) {
                    this.ShowInfo("Não foram encontrados Protocolos de Entrega de Guias no Protheus com este filtro!");
                }
            }
        
        }

        protected void gdvProtocolo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView vo = (DataRowView)e.Row.DataItem;
                GridViewRow row = e.Row;

                String dataEntrada = Convert.ToString(vo["BCI_YREDOC"]).Trim();
                if(!String.IsNullOrEmpty(dataEntrada)){
                    if (dataEntrada.Length == 8)
                    {
                        row.Cells[4].Text = dataEntrada.Substring(6, 2) + "/" + dataEntrada.Substring(4, 2) + "/" + dataEntrada.Substring(0, 4);
                    }
                }

                String dataEmissao = Convert.ToString(vo["BCI_YEMISS"]).Trim();
                if (!String.IsNullOrEmpty(dataEmissao))
                {
                    if (dataEmissao.Length == 8)
                    {
                        row.Cells[7].Text = dataEmissao.Substring(6, 2) + "/" + dataEmissao.Substring(4, 2) + "/" + dataEmissao.Substring(0, 4);
                    }
                }

                String dataVencimento = Convert.ToString(vo["BCI_YVEDOC"]).Trim();
                if (!String.IsNullOrEmpty(dataVencimento))
                {
                    if (dataVencimento.Length == 8)
                    {
                        row.Cells[8].Text = dataVencimento.Substring(6, 2) + "/" + dataVencimento.Substring(4, 2) + "/" + dataVencimento.Substring(0, 4);
                    }
                }

            }
        }
        
        protected void gdvProtocolo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CmdSalvar")
            {
                // Retrieve the row index stored in the CommandArgument property.
                int index = Convert.ToInt32(e.CommandArgument);

                Salvar(index);

            }
        }

        private void Salvar(int index)
        {

            // Retrieve the row that contains the button 
            // from the Rows collection.
            GridViewRow row = gdvProtocolo.Rows[index];

            string codpeg = row.Cells[1].Text.Replace("&nbsp;", "").Trim();

            if (string.IsNullOrEmpty(codpeg))
            {
                this.ShowError("O protocolo não foi preenchido!");
                return;
            }

            if (ProtocoloFaturaBO.Instance.ExisteProtocoloFatura(codpeg))
            {
                this.ShowError("O PEG " + codpeg + " já foi salvo na intranet!");
                return;
            }

            string codrda = Convert.ToString(gdvProtocolo.DataKeys[row.RowIndex]["BCI_CODRDA"]);
            if (string.IsNullOrEmpty(codrda.Trim()))
            {
                this.ShowError("O protocolo não possui RDA!");
                return;
            }

            PPegVO peg = PLocatorDataBO.Instance.GetPeg(codpeg, codrda);
            if(peg == null)
            {
                this.ShowError("O protocolo não foi encontrado no Protheus!");
                return;
            }

            string nrProtocolo;
            string codCredenciado;
            DateTime dtEntrada;
            string docFiscal;
            decimal? vlApresentado = null;
            DateTime? dtEmissao = null;
            DateTime? dtVencimento = null;
            decimal? vlProcessado = null;
            decimal? vlGlosa = null;
            DateTime? dtFinalizacao = null;

            #region[PROTOCOLO]

            nrProtocolo = peg.Codpeg;

            #endregion

            #region[CREDENCIADO]

            codCredenciado = peg.Codrda;

            #endregion

            #region[DATA DE ENTRADA]

            if (string.IsNullOrEmpty(peg.Yredoc.Trim()))
            {
                this.ShowError("A data de entrada não está preenchida no Protheus!");
                return;
            }

            if(peg.Yredoc.Trim().Length < 8)
            {
                this.ShowError("Data de entrada inválida!");
                return;
            }

            if (!DateTime.TryParse(DateUtil.FormatDateYMDToDMY(peg.Yredoc.Trim()), out dtEntrada))
            {
                this.ShowError("Data de entrada inválida!");
                return;
            }
            else
            {
                if (!IsValidDate(dtEntrada, false))
                {
                    this.ShowError("A data de entrada não pode ser antes de 2012 e não pode ser futura!");
                    return;
                }
            }

            #endregion

            #region[DOCUMENTO FISCAL]

            docFiscal = peg.Ydoc.Trim();
            if (!string.IsNullOrEmpty(peg.Ydoc.Trim()))
            {
                decimal d;
                if (!Decimal.TryParse(peg.Ydoc.Trim(), out d))
                {
                    this.ShowError("Doc. Fiscal inválido!");
                    return;
                }
            }

            #endregion

            #region[VALOR APRESENTADO]

            vlApresentado = (decimal?)peg.Yvldoc;

            #endregion

            #region[DATA DE EMISSÃO]

            if (!string.IsNullOrEmpty(peg.Yemiss.Trim()))
            {
                if (peg.Yemiss.Trim().Length < 8)
                {
                    this.ShowError("Data de emissão inválida!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(DateUtil.FormatDateYMDToDMY(peg.Yemiss.Trim()), out dt))
                {
                    this.ShowError("Data de emissão inválida!");
                    return;
                }
                dtEmissao = dt;
                if (!IsValidDate(dtEmissao, false))
                {
                    this.ShowError("A data de emissão não pode ser antes de 2012 e não pode ser futura!");
                    return;
                }
            }

            #endregion

            #region[DATA DE VENCIMENTO]

            if (string.IsNullOrEmpty(peg.Yvedoc.Trim()))
            {
                this.ShowError("A data de vencimento não está preenchida no Protheus!");
                return;
            }

            if (peg.Yvedoc.Trim().Length < 8)
            {
                this.ShowError("Data de vencimento inválida!");
                return;
            }

            DateTime dtV;
            if (!DateTime.TryParse(DateUtil.FormatDateYMDToDMY(peg.Yvedoc.Trim()), out dtV))
            {
                this.ShowError("Data de vencimento inválida!");
                return;
            }
            dtVencimento = dtV;
            if (!IsValidDate(dtVencimento, true))
            {
                this.ShowError("A data de vencimento não pode ser antes de 2012!");
                return;
            }


            #endregion

            #region[VALOR PROCESSADO]

            vlProcessado = (decimal?)peg.Vlrgui;

            #endregion
           
            #region[VALOR GLOSA]

            vlGlosa = (decimal?)peg.Vlrglo;

            #endregion 

            #region[DATA DE FINALIZAÇÃO]

            if (!string.IsNullOrEmpty(peg.Dthrlb.Trim()))
            {
                if (peg.Dthrlb.Trim().Length < 8)
                {
                    this.ShowError("Data de finalização inválida!");
                    return;
                }

                DateTime dtF;
                if (!DateTime.TryParse(DateUtil.FormatDateYMDToDMY(peg.Dthrlb.Trim()), out dtF))
                {
                    this.ShowError("Data de finalização inválida!");
                    return;
                }
                dtFinalizacao = dtF;
                if (!IsValidDate(dtFinalizacao, true))
                {
                    this.ShowError("A data de finalização não pode ser antes de 2012!");
                    return;
                }
            }
            
            #endregion

            ProtocoloFaturaVO vo = new ProtocoloFaturaVO();
            vo.NrProtocolo = nrProtocolo;
            vo.RedeAtendimento = new PRedeAtendimentoVO();
            vo.RedeAtendimento.Codigo = codCredenciado;
            vo.DataEntrada = dtEntrada;
            vo.DataEmissao = dtEmissao;
            vo.DocumentoFiscal = docFiscal;
            vo.ValorApresentado = vlApresentado;
            vo.DataVencimento = dtVencimento == null ? DateTime.MinValue : dtVencimento.Value;
            vo.ValorProcessado = vlProcessado;
            vo.ValorGlosa = vlGlosa;
            vo.DataFinalizacao = dtFinalizacao;
            vo.Fase = (FaseProtocoloFatura)Int32.Parse(peg.Fase);

            ProtocoloFaturaBO.Instance.Gerar(vo, UsuarioLogado.Id);
            this.ShowInfo("PROTOCOLO Nº <b>" + vo.NrProtocolo + "</b><br/>" +
                " SALVO COM SUCESSO!");

            hidProtocolo.Value = vo.Id.ToString();
            btnImprimir.Visible = true;
            btnEditar.Visible = true;
            Buscar(false);
        }

	}
}