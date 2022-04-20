using eVida.Web.Controls;
using eVida.Web.Report;
using eVida.Web.Report.Externo;
using eVida.Web.Security;
using eVidaBeneficiarios.Classes.Externo;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Reporting.Externo;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Externo {
	public partial class ConsultaPlano : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			lblLoginExpirado.Visible = false;
			if (!IsPostBack) {
				dvLogin.Visible = true;
				ViewState.Clear();

				if (Request["EXP"] != null) {
					lblLoginExpirado.Visible = true;
				} else if (Request["LOG"] != null) {
					PBeneficiarioVO vo = Session["LOGIN"] as PBeneficiarioVO;
					if (vo == null) {
						Response.Redirect(GetInitialPage(), true);
						return;
					}
					dvLogin.Visible = false;
					BindAfterLogin(vo);
				}
			}
		}

		public override bool IsLoginPage() {
			return Request["LOG"] == null;
		}

		protected override string GetInitialPage() {
			return "~/Externo/ConsultaPlano.aspx?EXP=1";
		}

		protected PBeneficiarioVO Beneficiario {
			get { return (PBeneficiarioVO)ViewState["BENEF"]; }
			set { ViewState["BENEF"] = value; }
		}
		protected PPlanoVO Plano {
			get { return (PPlanoVO)ViewState["PLANO"]; }
			set { ViewState["PLANO"] = value; }
		}

		protected void btnLogin_Click(object sender, EventArgs e) {
			try {
				string cartao = txtCartao.Text;
				string strDataNascimento = txtNascimento.Text;				
				DateTime dataNascimento;

				lblLoginExpirado.Visible = false;

				if (string.IsNullOrEmpty(cartao)) {
					this.ShowError("O número do cartão é obrigatório!");
					return;
				}

				if (string.IsNullOrEmpty(strDataNascimento)) {
					this.ShowError("A data de nascimento é obrigatória!");
					return;
				} else if (!DateTime.TryParse(strDataNascimento, out dataNascimento)){
					this.ShowError("Data de nascimento inválida!");
					return;
				}

				PBeneficiarioVO vo = BeneficiarioBO.Instance.GetBeneficiarioByCartao(cartao);
				if (vo == null) {
					this.ShowError("Beneficiário não encontrado!");
					return;
				}
				if (vo.DtNascimento == DateTime.MinValue) {
					this.ShowError("Dados do beneficiário incompleto. Contate o setor de cadastro!");
					return;
				}
				if (vo.DtNascimento != dataNascimento) {
					this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
					return;
				}

				PBeneficiarioPlanoVO benefPlanoVO = BeneficiarioBO.Instance.GetBeneficiarioPlano(vo);
				if (benefPlanoVO == null) {
					this.ShowError("Não foi possível validar o plano do beneficiário!");
					return;
				}

				UsuarioNoLoginRequiredVO uVO = new UsuarioNoLoginRequiredVO();

				PageHelper.SaveAuthentication(uVO, this.Session, this.Response, false);
				Session["LOGIN"] = vo;
				Response.Redirect("~/Externo/ConsultaPlano.aspx?LOG=1", true);
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar login!", ex);
			}
		}

		private void BindAfterLogin(PBeneficiarioVO vo) {
			dvLogin.Visible = false;
			dvResult.Visible = true;

			PBeneficiarioPlanoVO benefPlanoVO = BeneficiarioBO.Instance.GetBeneficiarioPlano(vo);
			PPlanoVO plano = PlanoBO.Instance.GetPlano(benefPlanoVO.CodInt, benefPlanoVO.CodPlano);

			Beneficiario = vo;
			Plano = plano;		

			header.UpdateBeneficiario(vo, plano);
		}

		private DataTable Result {
			get { return (DataTable)Session["RESULT_" + this.GetType().Name]; }
			set { Session["RESULT_" + this.GetType().Name] = value; }
		}

		private void BuscarServicos() {
			string codigo;
			string descricao;

			codigo = txtCodigoTuss.Text;
			descricao = txtNomeTuss.Text;

			if (string.IsNullOrEmpty(codigo) && string.IsNullOrEmpty(descricao)) {
				this.ShowInfo("Informe pelo menos um campo para filtro!");
				return;
			}

			List<string> lstTabela = new List<string>();
			lstTabela.Add("00");
			lstTabela.Add("22");
			DataTable dt = ServicoBO.Instance.Pesquisar(codigo, descricao, Plano.Codigo, true, lstTabela);
			bool hasRows = dt != null && dt.Rows.Count > 0;
			dt.DefaultView.Sort = "ds_servico ASC";
			gdvResultado.PageIndex = 0;
			gdvResultado.DataSource = dt;
			gdvResultado.DataBind();
			Result = dt;

			if (!hasRows) {
				lblContador.Text = string.Empty;
				btnExportar.Visible = false;
				this.ShowInfo("Não foram encontrados registros para este critério de busca!");
				return;
			}
			btnExportar.Visible = true;
			lblContador.Text = "Foram encontrados " + (dt.Rows.Count) + " registros!";
			lblContador.Visible = true;
		}

		private void ExportarPDF() {
			PBeneficiarioVO benef = Beneficiario;
			PPlanoVO plano = Plano;

			if (benef == null || plano == null) {
				Response.Redirect(GetInitialPage(), true);
				return;
			}

			DataTable dt = Result;
			if (dt == null) {
				this.ShowError("O resultado da busca expirou. Realize a consulta novamente.");
				return;
			}


			ReportConsultaPlanoExtBinder.ParamsVO dados = new ReportConsultaPlanoExtBinder.ParamsVO();
			dados.Dados = dt;
			dados.Beneficiario = benef;
			dados.Plano = plano;
			ReportConsultaPlanoExt.SaveDados(Request, Session, dados);

			string tipo = ReportHandler.EnumRelatorio.CONSULTA_PLANO_EXT.ToString();
			base.RegisterScript("GRAFICO", String.Format("openReport('{0}','{1}');", tipo, DateTime.Now.Ticks.ToString()));
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				BuscarServicos();
			} catch(Exception ex) {
				this.ShowError("Erro ao realizar busca!", ex);
			}
		}

		protected void gdvResultado_PageIndexChanging(object sender, GridViewPageEventArgs e) {
			try {
				DataTable dt = Result;
				if (dt == null) {
					BuscarServicos();
					dt = Result;
				} else {
					gdvResultado.PageIndex = e.NewPageIndex;
					gdvResultado.DataSource = dt;
					gdvResultado.DataBind();
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar paginação!", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarPDF();
			} catch (Exception ex) {
				this.ShowError("Erro ao gerar PDF!", ex);
			}
		}
	}
}