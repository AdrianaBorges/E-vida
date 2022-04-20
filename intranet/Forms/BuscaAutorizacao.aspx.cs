using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Filter;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using SkyReport.ExcelExporter;
using eVidaGeneralLib.VO.SCL;

namespace eVidaIntranet.Forms {
	public partial class BuscaAutorizacao : RelatorioExcelPageBase {
		private Dictionary<int, UsuarioVO> cacheUsuario = null;

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdSituacao.Items.Add(new ListItem("TODOS", ""));
					dpdSituacao.Items.Add(new ListItem("EM ANDAMENTO", "-1"));
					foreach (StatusAutorizacao i in Enum.GetValues(typeof(StatusAutorizacao))) {
						dpdSituacao.Items.Add(new ListItem(AutorizacaoTradutorHelper.TraduzStatus(i), i.ToString()));
					}
					dpdSituacao.SelectedValue = "-1";

					dpdOrigem.Items.Add(new ListItem("TODAS", ""));
					foreach (OrigemAutorizacao i in Enum.GetValues(typeof(OrigemAutorizacao))) {
						dpdOrigem.Items.Add(new ListItem(AutorizacaoTradutorHelper.TraduzOrigem(i), i.ToString()));
					}

					dpdCarater.Items.Add(new ListItem("TODOS", ""));
					foreach (CaraterAutorizacao i in Enum.GetValues(typeof(CaraterAutorizacao))) {
						dpdCarater.Items.Add(new ListItem(AutorizacaoTradutorHelper.TraduzCarater(i), i.ToString()));
					}
					dpdPrazo.Items.Add(new ListItem("TODOS", ""));
					foreach (PrazoAutorizacao i in Enum.GetValues(typeof(PrazoAutorizacao))) {
						dpdPrazo.Items.Add(new ListItem(AutorizacaoTradutorHelper.TraduzPrazo(i), i.ToString()));
					}
					dpdTipo.Items.Add(new ListItem("TODOS", ""));
					foreach (TipoAutorizacao i in Enum.GetValues(typeof(TipoAutorizacao))) {
						dpdTipo.Items.Add(new ListItem(AutorizacaoTradutorHelper.TraduzTipo(i), i.ToString()));
					}

					List<KeyValuePair<string,string>> lstConselho = PLocatorDataBO.Instance.ListarConselhoProfissional();
					dpdConselho.DataSource = lstConselho;
					dpdConselho.DataBind();
					dpdConselho.Items.Insert(0,new ListItem("TODOS", ""));

					dpdUfConselho.DataSource = PLocatorDataBO.Instance.ListarUf();
					dpdUfConselho.DataBind();
					dpdUfConselho.Items.Insert(0, new ListItem("TODOS", ""));

					List<UsuarioVO> lstUsuarios = AutorizacaoBO.Instance.ListarUsuariosGestao();
					dpdGestorAjuste.Items.Insert(0, new ListItem("TODOS", ""));
					if (lstUsuarios != null) {
						lstUsuarios.Sort((x, y) => x.Nome.CompareTo(y.Nome));
						cacheUsuario = new Dictionary<int, UsuarioVO>();
						foreach (UsuarioVO usuario in lstUsuarios) {
							cacheUsuario.Add(usuario.Id, usuario);
							dpdGestorAjuste.Items.Add(new ListItem(usuario.Nome + " (" + usuario.Login + ")", usuario.Id.ToString()));
						}
					}

					if (!HasPermissaoGestao()) {
						dpdOrigem.SelectedValue = OrigemAutorizacao.GESTOR.ToString();
						dpdOrigem.Enabled = false;
					} else {
						if (!HasPermissaoOdonto()) {
							dpdTipo.SelectedValue = TipoAutorizacao.MEDICA.ToString();
							dpdTipo.Enabled = false;
						}
						if (!HasPermissaoMedica()) {
							dpdTipo.SelectedValue = TipoAutorizacao.ODONTO.ToString();
							dpdTipo.Enabled = false;
						}
					}

					if (Request["BACK"] != null) {
						LoadFilter(true);
					}
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar a página", ex);
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO; }
		}

		private bool HasPermissaoGestao() {
			return HasPermissaoMedica() || HasPermissaoOdonto();
		}

		private bool HasPermissaoMedica() {
			return HasPermission(Modulo.GERIR_AUTORIZACAO_MEDICA);
		}

		private bool HasPermissaoOdonto() {
			return HasPermission(Modulo.GERIR_AUTORIZACAO_ODONTO);
		}

		private FilterAutorizacaoVO LastFilter {
			get { return (FilterAutorizacaoVO)Session["FILTRO_AUTORIZACAO"]; }
			set { Session["FILTRO_AUTORIZACAO"] = value; }
		}

		private string ToString(int? i) {
			if (i == null) return string.Empty;
			return i.ToString();
		}

		private string ToString(bool? i) {
			if (i == null) return string.Empty;
			if (i.Value) return "S";
			return "N";
		}

		private void LoadFilter(bool search) {
			FilterAutorizacaoVO filtro = LastFilter;
			if (filtro != null) {

				txtProtocolo.Text = ToString(filtro.Id);
				txtNroTiss.Text = ToString(filtro.NroAutorizacaoTiss);
				dpdSituacao.SelectedValue = filtro.EmAndamento != null && filtro.EmAndamento.Value ? "-1" : 
					filtro.Status != null ? filtro.Status.Value.ToString() : "";
				dpdOrigem.SelectedValue = filtro.Origem != null ? filtro.Origem.Value.ToString() : "";

                txtMatricula.Text = filtro.Beneficiario != null && !string.IsNullOrEmpty(filtro.Beneficiario.Matemp.Trim()) ? filtro.Beneficiario.Matemp : "";
				txtNumCartao.Text = filtro.Beneficiario != null ? filtro.Beneficiario.Matant : "";
				txtNomeBenef.Text = filtro.Beneficiario != null ? filtro.Beneficiario.Nomusr : "";

				txtCpfCnpj.Text = filtro.Credenciado != null ? filtro.Credenciado.Cpfcgc : "";
				txtNomeCred.Text = filtro.Credenciado != null ? filtro.Credenciado.Nome : "";

				if (filtro.Profissional != null) {
                    if (filtro.Profissional != null)
                    {
                        txtNroConselho.Text = filtro.Profissional.Numcr;
                        dpdConselho.SelectedValue = filtro.Profissional.Codsig;
                        dpdUfConselho.SelectedValue = filtro.Profissional.Estado;
					}
					txtNomeProfissional.Text = filtro.Profissional.Nome;
				}

				dpdCarater.SelectedValue = filtro.Carater != null ? filtro.Carater.Value.ToString() : "";
				txtCodDoenca.Text = filtro.CodDoenca;
				txtNomeDoenca.Text = filtro.NomeDoenca;
				dpdInternacao.SelectedValue = ToString(filtro.Internacao);
				txtHospital.Text = filtro.Hospital;
				txtDataInternacao.Text = filtro.DataInternacao != null ? filtro.DataInternacao.Value.ToShortDateString() : "";
				txtIndicacao.Text = filtro.Indicacao;
                dpdTfd.SelectedValue = ToString(filtro.Tfd);

				if (filtro.CodServico != null) {
					hidCodServico.Value = filtro.CodServico.ToString();

                    string[] dados_servico = hidCodServico.Value.Split('|');
                    string codpad = dados_servico[0];
                    string codpsa = dados_servico[1];

					PTabelaPadraoVO servico = PLocatorDataBO.Instance.GetTabelaPadrao(codpad, codpsa);
					lblServico.Text = servico.Descri;
				}

				dpdOpme.SelectedValue = ToString(filtro.Opme);
				dpdTipo.SelectedValue = filtro.Tipo != null ? filtro.Tipo.Value.ToString() : "";
				dpdGestorAjuste.SelectedValue = ToString(filtro.CodUsuarioResponsavel);

				if (search)
					Buscar(filtro);
			}
		}

		private void LimparFiltros(bool clearSession) {
			if (clearSession) {
				LastFilter = new FilterAutorizacaoVO();
				LastFilter.EmAndamento = true;
			}
			LoadFilter(false);
			LastFilter = null;
		}

		private void Buscar(FilterAutorizacaoVO filtro) {
			DataTable dt = AutorizacaoBO.Instance.Pesquisar(filtro);

			if (!string.IsNullOrEmpty(dpdPrazo.SelectedValue))
				dt = FiltrarPrazo(dt, (PrazoAutorizacao)Enum.Parse(typeof(PrazoAutorizacao), dpdPrazo.SelectedValue, true));

			this.ShowPagingGrid(gdvRelatorio, dt, null);

			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
			btnExportar.Visible = dt.Rows.Count > 0;
		}

		private void Aprovar(GridViewRow row, int index) {
			int cdAutorizacao = Convert.ToInt32(gdvRelatorio.DataKeys[index]["cd_autorizacao"]);

			AutorizacaoVO autVO = AtualizarLinha(row, index);
			if (autVO.Status == StatusAutorizacao.APROVADA) {
				//BindStatusRow(row, autVO);
				row.Cells[10].Text = autVO.DataAutorizacao.ToString();
			} else {
				List<string> lstErrors = AutorizacaoBO.Instance.CheckAntesAprovar(cdAutorizacao);
				if (lstErrors.Count == 0) {
					this.RegisterScript("APROVAR", "openAprovar($('#" + row.FindControl("btnAprovar").ClientID + "')," + cdAutorizacao + ", " + index + ");");
				} else {
					IEnumerable<ItemError> errors = lstErrors.Select(x => new ItemError()
					{
						Control = null,
						Message = x
					});
					this.ShowErrorList(errors);
					return;
				}
			}
		}

		private AutorizacaoVO AtualizarLinha(GridViewRow row, int index) {
			int cdAutorizacao = Convert.ToInt32(gdvRelatorio.DataKeys[index]["cd_autorizacao"]);
			AutorizacaoVO autVO = AutorizacaoBO.Instance.GetById(cdAutorizacao);
			PrazoAutorizacaoVO prazo = AutorizacaoBO.Instance.CalcularPrazo(autVO);
			BindStatusRow(row, autVO, prazo);
			return autVO;
		}

		private void SolicitarPericia(GridViewRow row, int index) {
			int cdAutorizacao = Convert.ToInt32(gdvRelatorio.DataKeys[index]["cd_autorizacao"]);
			AutorizacaoVO autVO = AutorizacaoBO.Instance.GetById(cdAutorizacao);
			if (AutorizacaoTradutorHelper.IsStatusFim(autVO.Status)) {
				this.ShowError("A autorização está no status '" +
					AutorizacaoTradutorHelper.TraduzStatus(autVO.Status) +
					"' e ter perícia solicitada!");
			} else {
				AutorizacaoBO.Instance.SolicitarPericia(autVO, UsuarioLogado.Id);
				this.ShowInfo("Perícia solicitada!");
				AtualizarLinha(row, index);
			}
		}
		
		private void BindStatusRow(GridViewRow row, AutorizacaoVO autVO, PrazoAutorizacaoVO prazo) {

			LinkButton btnRevalidar = row.FindControl("btnRevalidar") as LinkButton;
			LinkButton btnCancelar = row.FindControl("btnCancelar") as LinkButton;
			LinkButton btnAprovar = row.FindControl("btnAprovar") as LinkButton;
			LinkButton btnNegar = row.FindControl("btnNegar") as LinkButton;
			LinkButton btnSolDoc = row.FindControl("btnSolDoc") as LinkButton;
			LinkButton btnPericia = row.FindControl("btnPericia") as LinkButton;
			ImageButton btnEditar = row.FindControl("btnEditar") as ImageButton;
			ImageButton btnPdf = row.FindControl("btnPdf") as ImageButton;
			Image imgControle = row.FindControl("imgControle") as Image;
			TableCell cellStatus = row.Cells[9];
			TableCell cellTiss = row.Cells[4];

			StatusAutorizacao status = autVO.Status;
			String motivo = autVO.MotivoCancelamento;

			cellStatus.Text = AutorizacaoTradutorHelper.TraduzStatus(status);
			if (status == StatusAutorizacao.CANCELADA)
				cellStatus.Text += " - " + motivo;

			string urlImg = "";
			switch (prazo.Prazo) {
				case PrazoAutorizacao.DENTRO_PRAZO:
					urlImg = "~/img/progress_ok.png";
					break;
				case PrazoAutorizacao.ALERTA:
					urlImg = "~/img/progress_alert.png";
					break;
				case PrazoAutorizacao.FORA_PRAZO:
					urlImg = "~/img/progress_fail.png";
					break;
				default:
					break;
			}
			imgControle.ImageUrl = urlImg;
			imgControle.ToolTip = DateUtil.FormatHours(prazo.Horas);

			btnSolDoc.Visible = btnCancelar.Visible = btnAprovar.Visible = btnNegar.Visible = btnPericia.Visible
				= status != StatusAutorizacao.ENVIADA && status != StatusAutorizacao.APROVADA && status != StatusAutorizacao.CANCELADA && status != StatusAutorizacao.NEGADA;
			
			if (status == StatusAutorizacao.APROVADA) {
				List<AutorizacaoTissVO> lstTiss = AutorizacaoBO.Instance.ListTiss(autVO.Id);
				if (lstTiss != null && lstTiss.Count > 0) {
					btnPdf.Visible = true;
					btnPdf.OnClientClick = "return openPdf(" + autVO.Id + ", '" + lstTiss[0].NomeArquivo + "');";
					cellTiss.Text = lstTiss.Select(x => x.NrAutorizacaoTiss.ToString()).Aggregate((x, y) => x + ", " + y);
				}
			}

			btnAprovar.Visible &= HasPermissaoGestao();
			btnSolDoc.Visible &= HasPermissaoGestao();
			btnNegar.Visible &= HasPermissaoGestao();
			btnPericia.Visible &= HasPermissaoGestao();

			if (AutorizacaoBO.Instance.PodeRevalidar(autVO)) {
				btnRevalidar.Visible = true;
			} else {
				btnRevalidar.Visible = false;
			}

			if (btnAprovar.Visible || ((status == StatusAutorizacao.ENVIADA || status == StatusAutorizacao.SOLICITANDO_DOC) && autVO.Origem == OrigemAutorizacao.GESTOR)
				|| (status == StatusAutorizacao.ENVIADA && HasPermissaoGestao())) {
				btnEditar.ImageUrl = ResolveClientUrl("~/img/ico_editar.gif");
			} else {
				btnEditar.ImageUrl = ResolveClientUrl("~/img/lupa.gif");
			}

			if (btnAprovar.Visible || (status == StatusAutorizacao.ENVIADA && autVO.Origem == OrigemAutorizacao.GESTOR)) {
				btnCancelar.Visible = true;
			} else {
				btnCancelar.Visible = false;
			}

		}

		protected void gdvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView vo = (DataRowView)e.Row.DataItem;
				GridViewRow row = e.Row;

				AutorizacaoVO autVO = (AutorizacaoVO)vo["OBJ"]; //AutorizacaoBO.Instance.ConvertPesquisaRow(vo.Row);
				PrazoAutorizacaoVO prazo = (PrazoAutorizacaoVO)vo["PRAZO"]; 
				BindStatusRow(row, autVO, prazo);

				if (autVO.Origem == OrigemAutorizacao.GESTOR) {
					string nomeGestor = FirstNameGestor(autVO.CodUsuarioCriacao);					
					row.Cells[6].Text = "GESTOR - " + nomeGestor;
				}

				Label lblUltimoAjuste = (Label)row.FindControl("lblUltimoAjuste");
				string alteracao = GetUltimaAlteracao(autVO);
				lblUltimoAjuste.Text = alteracao;

                if (autVO.Tfd != null)
                {
                    if (Convert.ToBoolean(autVO.Tfd) == true)
                    {
                        row.Cells[14].Text = "SIM";
                    }
                    else
                    {
                        row.Cells[14].Text = "NÃO";
                    }
                }
                else
                {
                    row.Cells[14].Text = "";
                }

			}
		}

		private string GetUltimaAlteracao(AutorizacaoVO autVO) {
			string alteracao = autVO.DataAlteracao.ToString("dd/MM/yyyy HH:mm") + " - " + autVO.OrigemAlteracao;
			if (autVO.OrigemAlteracao == OrigemAutorizacao.GESTOR && autVO.CodUsuarioAlteracao != null) {
				string nomeGestor = FirstNameGestor(autVO.CodUsuarioAlteracao);
				if (!string.IsNullOrEmpty(nomeGestor))
					alteracao += " - " + nomeGestor;
			}
			return alteracao;
		}

		private string FirstNameGestor(int? idUsuario) {
			if (idUsuario != null) {
				if (cacheUsuario == null)
					cacheUsuario = new Dictionary<int, UsuarioVO>();

				UsuarioVO gestorCriacao = null;
				if (cacheUsuario.ContainsKey(idUsuario.Value))
					gestorCriacao = cacheUsuario[idUsuario.Value];
				else {
					gestorCriacao = UsuarioBO.Instance.GetUsuarioById(idUsuario.Value);
					cacheUsuario.Add(idUsuario.Value, gestorCriacao);
				}
				string nomeGestor = gestorCriacao.Nome;
				nomeGestor = nomeGestor.Split(' ')[0];
				return nomeGestor;
			}
			return null;
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			
			defs.Add(new ExcelColumnDefinition()
			{
				HeaderText = "CONTROLE",
				ColumnName = "PRAZO",
				StyleName = ExcelStyleDefinition.TEXT,
				Transformer = x => AutorizacaoTradutorHelper.TraduzPrazo(((PrazoAutorizacaoVO)x).Prazo)
			});
			//defs.Add(new ExcelColumnDefinition() {
				//HeaderText = "DIAS EXTRAS",
				//ColumnName = "PRAZO",
				//StyleName = new ExcelStyleDefinition() {					
				//	Alignment = ExcelStyleDefinition.INTEGER.Alignment,
				//	NumberFormat = "#.00",
				//	WrapText = false					
				//},
				//Transformer = x => ((PrazoAutorizacaoVO)x).Horas/24
			//});
			defs["ST_AUTORIZACAO"].ForEach(x => x.Transformer = y => AutorizacaoTradutorHelper.TraduzStatus((StatusAutorizacao)Convert.ToInt32(y)));
			defs.Add(new ExcelColumnDefinition()
			{
				HeaderText = "ÚLTIMA ALTERAÇÃO",
				ColumnName = "OBJ",
				StyleName = ExcelStyleDefinition.TEXT,
				Transformer = x => GetUltimaAlteracao((AutorizacaoVO)x),
				Width = 27
			});
            defs.SetWidth("benef_NM_NOMUSR", 40);
			ExportExcel("RelatorioAutorizacao", defs, sourceTable);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				FilterAutorizacaoVO filtro = new FilterAutorizacaoVO();

				int iValue;
				long lValue;
				DateTime dtValue;

				if (!string.IsNullOrEmpty(txtProtocolo.Text)) {
					if (Int32.TryParse(txtProtocolo.Text, out iValue)) {
						filtro.Id = iValue;
					} else {
						this.ShowError("O protocolo deve ser numérico!");
						return;
					}
				}

				if (!string.IsNullOrEmpty(txtNroTiss.Text)) {
					if (Int32.TryParse(txtNroTiss.Text, out iValue)) {
						filtro.NroAutorizacaoTiss = iValue;
					} else {
						this.ShowError("O número TISS deve ser numérico!");
						return;
					}
				}

				if (!string.IsNullOrEmpty(dpdSituacao.SelectedValue)) {
					StatusAutorizacao status;
					if (dpdSituacao.SelectedValue.Equals("-1")) {
						filtro.EmAndamento = true;
					} else {
						if (Enum.TryParse(dpdSituacao.SelectedValue, true, out status)) {
							filtro.Status = status;
						}
					}
				}

				if (!string.IsNullOrEmpty(dpdOrigem.SelectedValue)) {
					OrigemAutorizacao origem;
					if (Enum.TryParse(dpdOrigem.SelectedValue, true, out origem)) {
						filtro.Origem = origem;
					}
				}

				if (!string.IsNullOrEmpty(txtMatricula.Text)) {
					if (Int64.TryParse(txtMatricula.Text, out lValue)) {
						if (filtro.Beneficiario == null)
							filtro.Beneficiario = new PUsuarioVO();
                        filtro.Beneficiario.Matemp = txtMatricula.Text;
					} else {
						this.ShowError("A matrícula deve ser numérica!");
						return;
					}
				}

				if (!string.IsNullOrEmpty(txtNumCartao.Text)) {
					if (filtro.Beneficiario == null)
                        filtro.Beneficiario = new PUsuarioVO();
					filtro.Beneficiario.Matant = txtNumCartao.Text;
				}

				if (!string.IsNullOrEmpty(txtNomeBenef.Text)) {
					if (filtro.Beneficiario == null)
                        filtro.Beneficiario = new PUsuarioVO();
					filtro.Beneficiario.Nomusr = txtNomeBenef.Text;
				}

				if (!string.IsNullOrEmpty(txtCpfCnpj.Text)) {
					if (Int64.TryParse(txtCpfCnpj.Text, out lValue)) {
						if (filtro.Credenciado == null)
							filtro.Credenciado = new PRedeAtendimentoVO();
                        filtro.Credenciado.Cpfcgc = txtCpfCnpj.Text;
					} else {
						this.ShowError("O cpf/cnpj deve ser numérico!");
						return;
					}
				}

				if (!string.IsNullOrEmpty(txtNomeCred.Text)) {
					if (filtro.Credenciado == null)
                        filtro.Credenciado = new PRedeAtendimentoVO();
					filtro.Credenciado.Nome = txtNomeCred.Text;
				}

				if (!string.IsNullOrEmpty(txtNroConselho.Text)) {
					if (!Int32.TryParse(txtNroConselho.Text, out iValue)) {
						if (filtro.Profissional == null) {
							filtro.Profissional = new PProfissionalSaudeVO();
                            filtro.Profissional.Numcr = txtNroConselho.Text;
						}
					}
				}

				if (!string.IsNullOrEmpty(dpdConselho.SelectedValue)) {
					if (filtro.Profissional == null) {
                        filtro.Profissional = new PProfissionalSaudeVO();
                        filtro.Profissional.Codsig = dpdConselho.SelectedValue;
					}
				}

				if (!string.IsNullOrEmpty(dpdUfConselho.SelectedValue)) {
					if (filtro.Profissional == null) {
                        filtro.Profissional = new PProfissionalSaudeVO();
                        filtro.Profissional.Estado = dpdUfConselho.SelectedValue;
					}
				}

				if (!string.IsNullOrEmpty(txtNomeProfissional.Text)) {
					if (filtro.Profissional == null)
                        filtro.Profissional = new PProfissionalSaudeVO();
					filtro.Profissional.Nome = txtNomeProfissional.Text;
				}

				if (!string.IsNullOrEmpty(dpdCarater.SelectedValue)) {
					CaraterAutorizacao carater;
					if (Enum.TryParse(dpdCarater.SelectedValue, true, out carater))
						filtro.Carater = carater;
				}
				filtro.CodDoenca = txtCodDoenca.Text;
				filtro.NomeDoenca = txtNomeDoenca.Text;

				if (!string.IsNullOrEmpty(dpdInternacao.SelectedValue)) {
					filtro.Internacao = dpdInternacao.SelectedValue.Equals("S");
				}
				filtro.Hospital = txtHospital.Text;
				if (!string.IsNullOrEmpty(txtDataInternacao.Text)) {
					if (DateTime.TryParse(txtDataInternacao.Text, out dtValue))
						filtro.DataInternacao = dtValue;
				}
				filtro.Indicacao = txtIndicacao.Text;

                if (!string.IsNullOrEmpty(dpdTfd.SelectedValue))
                {
                    filtro.Tfd = dpdTfd.SelectedValue.Equals("S");
                }

				if (!string.IsNullOrEmpty(hidCodServico.Value)) {
					filtro.CodServico = hidCodServico.Value;
				}
				if (!string.IsNullOrEmpty(dpdOpme.SelectedValue)) {
					filtro.Opme = dpdOpme.SelectedValue.Equals("S");
				}

				if (!HasPermissaoGestao()) {
					filtro.Origem = OrigemAutorizacao.GESTOR;
					if (UsuarioLogado.Usuario.Regional != null)
						filtro.CodUsuarioCriacao = UsuarioBO.Instance.ListarUsuarios().
							Where(x => UsuarioLogado.Usuario.Regional == x.Regional).
							Select(x => x.Id).ToList();
					else {
						filtro.CodUsuarioCriacao = new List<int>();
						filtro.CodUsuarioCriacao.Add(UsuarioLogado.Usuario.Id);
					}
				} 
				if (!string.IsNullOrEmpty(dpdTipo.SelectedValue)) {
					TipoAutorizacao tipo;
					if (Enum.TryParse(dpdTipo.SelectedValue, true, out tipo))
						filtro.Tipo = tipo;
				}

				if (!string.IsNullOrEmpty(dpdGestorAjuste.SelectedValue)) {
					filtro.CodUsuarioResponsavel = Convert.ToInt32(dpdGestorAjuste.SelectedValue);
				}

				if (!string.IsNullOrEmpty(txtProtocoloANS.Text)) {
					filtro.ProtocoloAns = txtProtocoloANS.Text;
				}

				LastFilter = filtro;
				Buscar(filtro);				
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar os dados!", ex);
			}
		}

		private DataTable FiltrarPrazo(DataTable dt, PrazoAutorizacao filtroPrazo) {
			DataTable dtFim = dt.Clone();
			foreach (DataRow dr in dt.Rows) {
				AutorizacaoVO autVO = (AutorizacaoVO)dr["OBJ"]; // AutorizacaoBO.Instance.ConvertPesquisaRow(dr);
				PrazoAutorizacaoVO prazo = (PrazoAutorizacaoVO)dr["PRAZO"]; //AutorizacaoBO.Instance.CalcularPrazo(autVO);
				if (prazo.Prazo == filtroPrazo) {
					DataRow drFim = dtFim.NewRow();
					drFim.ItemArray = dr.ItemArray;
					dtFim.Rows.Add(drFim);
				}
			}
			return dtFim;
		}

		protected void btnLocServico_Click(object sender, ImageClickEventArgs e) {
			try {
				if (!string.IsNullOrEmpty(hidCodServico.Value)) {

                    string[] dados_servico = hidCodServico.Value.Split('|');
                    string codpad = dados_servico[0];
                    string codpsa = dados_servico[1];

					PTabelaPadraoVO vo = PLocatorDataBO.Instance.GetTabelaPadrao(codpad, codpsa);
					lblServico.Text = vo.Codpsa + " - " + vo.Descri;
				} else {
					lblServico.Text = "";
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao escolher serviço como filtro!", ex);
			}
		}

		protected void btnClrServico_Click(object sender, ImageClickEventArgs e) {
			hidCodServico.Value = string.Empty;
			lblServico.Text = "";
		}

		protected void gdvRelatorio_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (this.IsPagingCommand(sender, e)) return;

				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					// Retrieve the row index stored in the CommandArgument property.
					int index = Convert.ToInt32(e.CommandArgument);

					// Retrieve the row that contains the button 
					// from the Rows collection.
					GridViewRow row = gdvRelatorio.Rows[index];

					if (e.CommandName == "CmdAprovar") {
						Aprovar(row, index);
					} else if (e.CommandName == "CmdCancelar") {
						AtualizarLinha(row, index);
					} else if (e.CommandName.Equals("CmdNegar")) {
						AtualizarLinha(row, index);
					} else if (e.CommandName.Equals("CmdDocAdicional")) {
						AtualizarLinha(row, index);
					} else if (e.CommandName.Equals("CmdPericia")) {
						SolicitarPericia(row, index);
					} else if (e.CommandName.Equals("CmdRevalidar")) {
						AtualizarLinha(row, index);
					} else {
						this.ShowError("Comando não reconhecido: " + e.CommandName);
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}

		protected void btnLimpar_Click(object sender, EventArgs e) {
			try {
				LimparFiltros(true);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao limpar filtros", ex);
			}
		}

	}
}