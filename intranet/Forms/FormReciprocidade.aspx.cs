using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms {
	public partial class FormReciprocidade : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				try {
					dpdEmpresa.Items.Clear();
					dpdEmpresa.Items.Insert(0, new ListItem("SELECIONE", ""));

					int id;
					if (string.IsNullOrEmpty(Request["ID"]) ||
						!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Identificador de requisição inválido!");
						return;
					}

					btnSalvar.Visible = this.HasPermission(Modulo.RECIPROCIDADE_GESTAO);
					ReciprocidadeVO vo = ReciprocidadeBO.Instance.GetById(id);
					Bind(vo);
				}
				catch (Exception ex) {
					this.ShowError("Erro ao carregar os dados da página! ", ex);
				}
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RECIPROCIDADE_VIEW; }
		}

		private void Bind(ReciprocidadeVO vo) {
            
            PFamiliaVO famVO = PFamiliaBO.Instance.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric);
            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
            PVidaVO vida = PVidaBO.Instance.GetVida(titular.Matvid);

            lblCpf.Text = titular.Cpfusr != null ? FormatUtil.FormatCpf(titular.Cpfusr) : "";
			lblCns.Text = vida.Nrcrna;

			lblEstadoCivil.Text = PLocatorDataBO.Instance.GetItemLista(PConstantes.LISTA_ESTADO_CIVIL_P, titular.Estciv);
			lblCartao.Text = titular.GetCarteira();
            lblNascimento.Text = DateUtil.FormatDateYMDToDMY(titular.Datnas);
			lblNomeMae.Text = titular.Mae;
			lblTitular.Text = titular.Nomusr;

			lblMunicipio.Text = vo.Endereco.Cidade;
			lblUf.Text = vo.Endereco.Uf;
			
			lblDataInicio.Text = vo.Inicio.ToString("dd/MM/yyyy");
			lblDataFim.Text = vo.Fim.ToString("dd/MM/yyyy");
			lblData.Text = vo.DataCriacao.ToString("dd/MM/yyyy");

			DataTable dtBeneficiarios = ReciprocidadeBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
			DataView dv = new DataView(dtBeneficiarios);
			dv.RowFilter = "BA1_TIPUSU <> '" + PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO + "'"; 
			dlBeneficiarios.DataSource = dv;
			dlBeneficiarios.DataBind();

			IEnumerable<POperadoraSaudeVO> lstEmpresas = ReciprocidadeBO.Instance.ListarOperadoras();
			//if (lstEmpresas != null)
			//	lstEmpresas = lstEmpresas.Where(x => x.AreaAtuacao != null && x.AreaAtuacao.Contains(vo.Endereco.Uf));

			UsuarioVO usuarioAprov = null;
			if (vo.Status == StatusReciprocidade.APROVADO) {
				usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAprovacao);
			} else if (vo.Status == StatusReciprocidade.ENVIADO) {
				usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioEnvio);
			} else {
				usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAlteracao);
			}

			if (usuarioAprov == null)
				usuarioAprov = UsuarioLogado.Usuario;

			litCargo.Text = usuarioAprov.Cargo;
			litUsuario.Text = usuarioAprov.Nome;
			imgAssinatura.ImageUrl = "../download.evida?TIPO=" + FileUtil.FileDir.ASSINATURA + "&ID=" + FormatUtil.ToBase64String(usuarioAprov.Login);

            bool hasLimiteOrtodontico = ReciprocidadeBO.Instance.HasLimiteOrtodontico(vo.CodSolicitacao);
			if (hasLimiteOrtodontico) {
				txtObs.Text = "BENEFICIÁRIO SEM DIREITO A MANUTENÇÃO ORTODONTICA";
			}

			if (lstEmpresas == null || lstEmpresas.Count() == 0) {
				//this.ShowError("Não existem empresas cadastradas com área de atuação em " + vo.Endereco.Uf);
                this.ShowError("Não existem empresas cadastradas.");
				return;
			}

			dpdEmpresa.DataSource = lstEmpresas;
			dpdEmpresa.DataValueField = "Codint";
			dpdEmpresa.DataTextField = "Nomint";
			dpdEmpresa.DataBind();

			dpdEmpresa.Items.Insert(0, new ListItem("SELECIONE", ""));
		}

		private void Bind(EmpresaReciprocidadeVO vo) {
			if (vo == null)
				vo = new EmpresaReciprocidadeVO();
			lblTelefone.Text = (vo.Telefone != null && vo.Telefone.Count > 0) ? vo.Telefone[0] : "";
			lblFax.Text = (vo.Fax != null && vo.Fax.Count > 0) ? vo.Fax[0] : "";
		}

		private List<int> GetAssistencias() {
			List<int> lst = new List<int>();
			foreach (ListItem item in chkAssistencias.Items) {
				if (item.Selected)
					lst.Add(Int32.Parse(item.Value));
			}
			return lst;
		}

		private bool ValidateRequireds() {
			List<ItemError> lstMsg = new List<ItemError>();
			if (string.IsNullOrEmpty(dpdEmpresa.SelectedValue)) {
				AddErrorMessage(lstMsg, dpdEmpresa, "Selecione a empresa credenciada!");
			}

			List<int> lstAssistencia = GetAssistencias();
			if (lstAssistencia.Count == 0) {
				AddErrorMessage(lstMsg, chkAssistencias, "Selecione pelo menos uma assistência autorizada!");
			}

			if (lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
			}
			return lstMsg.Count == 0;
		}

		private void Salvar() {
			if (!ValidateRequireds()) {
				return;
			}

			ReciprocidadeVO vo = new ReciprocidadeVO();
			vo = ReciprocidadeBO.Instance.GetById(Int32.Parse(Request["ID"]));

			vo.CodintReciprocidade = dpdEmpresa.SelectedValue.Trim();
			vo.Assistencia = GetAssistencias();
			vo.Observacao = txtObs.Text;

			byte[] anexo = null;
			if (ParametroUtil.EmailEnabled) {
				ReportEnvioReciprocidade rpt = new ReportEnvioReciprocidade(ReportDir, UsuarioLogado);
				vo.Status = StatusReciprocidade.ENVIADO;
				vo.CodUsuarioEnvio = UsuarioLogado.Id;
				anexo = rpt.GerarRelatorio(vo);
			}
			
			ReciprocidadeBO.Instance.Enviar(vo, UsuarioLogado.Id, anexo);

			this.ShowInfo("Formulário enviado com sucesso!");
			btnSalvar.Visible = false;
		}

		protected void dlBeneficiarios_ItemDataBound(object sender, DataListItemEventArgs e) {
			DataListItem row = e.Item;
			if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem) {
				Label lblNome = row.FindControl("lblNome") as Label;
				Label lblNascimento = row.FindControl("lblNascimento") as Label;
				Label lblParentesco = row.FindControl("lblParentesco") as Label;
				Label lblCpf = row.FindControl("lblCpf") as Label;
				Label lblNomeMae = row.FindControl("lblNomeMae") as Label;
				Label lblCns = row.FindControl("lblCns") as Label;
				
				DataRowView data = row.DataItem as DataRowView;

                lblNome.Text = Convert.ToString(data["BA1_NOMUSR"]);
                lblNascimento.Text = data["BA1_DATNAS"] != DBNull.Value ? DateUtil.FormatDateYMDToDMY(Convert.ToString(data["BA1_DATNAS"])) : "";
                lblCpf.Text = data["BA1_CPFUSR"] != DBNull.Value ? FormatUtil.FormatCpf(Convert.ToString(data["BA1_CPFUSR"]).Trim()) : "";
                lblNomeMae.Text = Convert.ToString(data["BA1_MAE"]);
                lblParentesco.Text = Convert.ToString(data["BRP_DESCRI"]);
                lblCns.Text = Convert.ToString(data["BTS_NRCRNA"]);
			}
		}

		protected void dpdEmpresa_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				EmpresaReciprocidadeVO vo = null;
				if (!string.IsNullOrEmpty(dpdEmpresa.SelectedValue)) {
					int idEmpresa = Int32.Parse(dpdEmpresa.SelectedValue);
					vo = ReciprocidadeBO.Instance.GetEmpresaById(idEmpresa);
				}
				Bind(vo);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao mostrar dados da empresa!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar o formulário!", ex);
			}
		}
	}
}