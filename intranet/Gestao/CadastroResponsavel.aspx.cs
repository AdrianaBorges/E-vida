using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;

namespace eVidaIntranet.Gestao {
	public partial class CadastroResponsavel : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int cdBeneficiario;

				if (string.IsNullOrEmpty(Request["ID"]) || !Int32.TryParse(Request["ID"], out cdBeneficiario)) {
					ShowError("Identificador de requisição inválido!");
					btnSalvar.Visible = false;
					btnCancelar.Visible = false;
					btnAdicionar.Visible = false;
					return;
				}
				Bind(cdBeneficiario);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_RESPONSAVEL; }
		}

		private void Bind(int cdBeneficiario) {
			ViewState["ID"] = cdBeneficiario;

			HcBeneficiarioVO benefVO = BeneficiarioBO.Instance.GetBeneficiario(cdBeneficiario);
			if (benefVO == null) {
				ShowError("Beneficiário inválido!");
				return;
			}

			lblAlternativo.Text = benefVO.CdAlternativo;
			lblMatricula.Text = benefVO.CdFuncionario.ToString();
			lblNome.Text = benefVO.NmBeneficiario;

			HcBeneficiarioPlanoVO benefPlanoVO = BeneficiarioBO.Instance.GetBeneficiarioPlano(cdBeneficiario);
			if (benefPlanoVO == null) {
				ShowError("Não foi possível encontrar o plano do beneficiário!");
				return;
			}
			HcPlanoVO planoVO = LocatorDataBO.Instance.GetPlano(benefPlanoVO.CdPlanoVinculado);
			lblPlano.Text = planoVO.DsPlano;
			lblVigencia.Text = benefPlanoVO.InicioVigencia.ToString("dd/MM/yyyy") + " - " +
				(benefPlanoVO.FimVigencia != null ? benefPlanoVO.FimVigencia.Value.ToString("dd/MM/yyyy") : "");
			ViewState["benefPlanoVO"] = benefPlanoVO;

			CarregarResponsaveis();
		}

		private void CarregarResponsaveis() {
			int cdBeneficiario = Convert.ToInt32(ViewState["ID"]);

			DataTable dt = ResponsavelBO.Instance.BuscarResponsaveis(cdBeneficiario);
			dlResponsavel.DataSource = dt;
			foreach (DataRow dr in dt.Rows) {
				dr["nm_beneficiario_financeiro"] = dr["cd_funcionario_financeiro"] +
					" - " + dr["nm_beneficiario_financeiro"];

				dr["nm_beneficiario_plano"] = dr["cd_funcionario_plano"] +
					" - " + dr["nm_beneficiario_plano"];
			}

			BindRows(dt);
		}

		private void Salvar() {
			int idUsuario = this.UsuarioLogado.Id;

			List<ItemError> lstMsg = new List<ItemError>();
			DataTable dt = RetrieveItemsFromDataList(lstMsg);

			if (lstMsg.Count != 0)
				return;

			List<ResponsavelVO> lst = Table2Items(dt);

			lstMsg = CheckConcorrencia(lst);
			if (lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
				return;
			}

			int cdBeneficiario = Convert.ToInt32(ViewState["ID"]);
			ResponsavelBO.Instance.SalvarResponsaveis(cdBeneficiario, lst, idUsuario);
			ShowInfo("Responsáveis salvos com sucesso!");
			CarregarResponsaveis();
		}

		private List<ItemError> CheckConcorrencia(List<ResponsavelVO> lst) {
			List<ItemError> lstMsg = new List<ItemError>();
			for (int i = 0; i < lst.Count-1; ++i) {
				ResponsavelVO vo = lst[i];
				DateTime dtInicio = vo.InicioVigencia;
				DateTime dtFim = vo.FimVigencia;

				for (int j = i+1; j < lst.Count; ++j) {
					ResponsavelVO vo2 = lst[j];
					DateTime dtI = vo2.InicioVigencia;
					DateTime dtF = vo2.FimVigencia;

					if (DateUtil.CheckConcorrencia(dtInicio, dtFim, dtI, dtF)) {
						AddErrorMessage(lstMsg, null, "O cadastro do perído " +
							dtInicio.ToString("dd/MM/yyyy") + " a " + dtFim.ToString("dd/MM/yyyy") +
							" está em conflito com o cadastro do período " +
							dtI.ToString("dd/MM/yyyy") + " a " + dtF.ToString("dd/MM/yyyy"));							
					}
				}
			}
			return lstMsg;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				ShowError("Erro ao salvar! " + ex.Message);
				Log.Error("Erro ao salvar!", ex);
			}
		}

		protected void btnCancelar_Click(object sender, EventArgs e) {
			CarregarResponsaveis();
		}

		protected void dlResponsavel_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
				RepeaterItem item = e.Item;
				DataRowView drv = item.DataItem as DataRowView;

				BindItem(item, drv);
			} else if (e.Item.ItemType == ListItemType.Footer) {
				e.Item.Visible = dlResponsavel.Items.Count == 0;
			}
		}

		protected void btnAdicionar_Click(object sender, EventArgs e) {
			AddNewRow();
		}

		protected void btnRemover_Click(object sender, ImageClickEventArgs e) {
			RepeaterItem row = (RepeaterItem)(sender as ImageButton).NamingContainer;
			RemoveRow(row.ItemIndex);
		}

		#region Gestao Itens
		
		private void BindRows(DataTable dt) {
			dlResponsavel.DataSource = dt;
			dlResponsavel.DataBind();

		}

		private DataTable CreateTable() {
			DataTable dt = new DataTable();
			dt.Columns.Add("dt_inicio_vigencia", typeof(DateTime));
			dt.Columns.Add("dt_fim_vigencia", typeof(DateTime));
			dt.Columns.Add("cd_beneficiario_financeiro", typeof(Int32));
			dt.Columns.Add("cd_beneficiario_plano", typeof(Int32));
			dt.Columns.Add("ds_observacao", typeof(string));
			dt.Columns.Add("nm_beneficiario_financeiro", typeof(string));
			dt.Columns.Add("nm_beneficiario_plano", typeof(string));
			return dt;
		}

		private DataTable RetrieveItemsFromDataList(List<ItemError> lstMsg) {
			bool showError = lstMsg != null;
			if (!showError)
				lstMsg = new List<ItemError>();

			DataTable dt = CreateTable();
			foreach (RepeaterItem item in dlResponsavel.Items) {
				if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item) {
					TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
					TextBox txtInicio = item.FindControl("txtInicio") as TextBox;
					TextBox txtFim = item.FindControl("txtFim") as TextBox;

					HiddenField hidRespFinanceiro = item.FindControl("hidRespFinanceiro") as HiddenField;
					HiddenField hidNmRespFinanceiro = item.FindControl("hidNmRespFinanceiro") as HiddenField;
					ImageButton btnRespFinanceiro = item.FindControl("btnRespFinanceiro") as ImageButton;
					Label lblRespFinanceiro = item.FindControl("lblRespFinanceiro") as Label;

					HiddenField hidRespFamilia = item.FindControl("hidRespFamilia") as HiddenField;
					HiddenField hidNmRespFamilia = item.FindControl("hidNmRespFamilia") as HiddenField;
					ImageButton btnRespFamilia = item.FindControl("btnRespFamilia") as ImageButton;
					Label lblRespFamilia = item.FindControl("lblRespFamilia") as Label;

					DateTime? dtInicio = null;
					DateTime? dtFim = null;
					int? cdBenefFin = null;
					int? cdBenefPln = null;
					string nmRespFin = null;
					string nmRespPln = null;

					DataRow dr = dt.NewRow();

					DateTime date;
					if (!DateTime.TryParse(txtInicio.Text, out date)) {
						AddErrorMessage(lstMsg, txtInicio, "Informe a data de início corretamente!");
					} else {
						dtInicio = date;
					}
					if (!DateTime.TryParse(txtFim.Text, out date)) {
						AddErrorMessage(lstMsg, txtInicio, "Informe a data de fim corretamente!");
					} else {
						dtFim = date;
					}
					if (string.IsNullOrEmpty(hidRespFamilia.Value)) {
						AddErrorMessage(lstMsg, btnRespFamilia, "Informe o responsável família!");
					} else {
						cdBenefPln = Int32.Parse(hidRespFamilia.Value);
					}
					if (string.IsNullOrEmpty(hidRespFinanceiro.Value)) {
						AddErrorMessage(lstMsg, btnRespFinanceiro, "Informe o responsável financeiro!");
					} else {
						cdBenefFin = Int32.Parse(hidRespFinanceiro.Value);
					}
					if (dtInicio != null && dtFim != null && dtInicio.Value.CompareTo(dtFim.Value) >= 0) {
						AddErrorMessage(lstMsg, txtInicio, "A data inicial deve ser menor que a data final!");
					}

					if (dtInicio != null) {
						HcBeneficiarioPlanoVO benefPlanoVO = ViewState["benefPlanoVO"] as HcBeneficiarioPlanoVO;
						if (dtInicio < benefPlanoVO.InicioVigencia)
							AddErrorMessage(lstMsg, txtInicio, "A data inicial não pode ser anterior à data de início de vigência do plano!");
					}

					nmRespFin = hidNmRespFinanceiro.Value;
					nmRespPln = hidNmRespFamilia.Value;
					lblRespFamilia.Text = nmRespPln;
					lblRespFinanceiro.Text = nmRespFin;

					if (dtInicio != null)  dr["dt_inicio_vigencia"] = dtInicio;
					if (dtFim != null) dr["dt_fim_vigencia"] = dtFim;
					if (cdBenefFin != null) dr["cd_beneficiario_financeiro"] = cdBenefFin;
					if (cdBenefPln != null) dr["cd_beneficiario_plano"] = cdBenefPln;
					dr["ds_observacao"] = txtObservacao.Text;
					dr["nm_beneficiario_financeiro"] = nmRespFin;
					dr["nm_beneficiario_plano"] = nmRespPln;

					dt.Rows.Add(dr);
				}
			}

			if (showError && lstMsg.Count > 0) {
				ShowErrorList(lstMsg);
			}
			return dt;
		}

		private void AddNewRow() {
			List<ItemError> lst = new List<ItemError>();
			DataTable dt = RetrieveItemsFromDataList(lst);
			if (lst.Count == 0) {
				dt.Rows.Add(dt.NewRow());
			}
			BindRows(dt);
		}

		private List<ResponsavelVO> Table2Items(DataTable dtItems) {
			List<ResponsavelVO> lst = new List<ResponsavelVO>();
			foreach (DataRow drv in dtItems.Rows) {
				ResponsavelVO item = new ResponsavelVO();
				item.CdBeneficiarioFinanceiro = Convert.ToInt32(drv["cd_beneficiario_financeiro"]);
				item.CdBeneficiarioPlano = Convert.ToInt32(drv["cd_beneficiario_plano"]);
				item.Observacao = Convert.ToString(drv["ds_observacao"]);
				item.InicioVigencia = Convert.ToDateTime(drv["dt_inicio_vigencia"]);
				item.FimVigencia = Convert.ToDateTime(drv["dt_fim_vigencia"]);

				lst.Add(item);
			}
			return lst;
		}

		private void BindItem(RepeaterItem item, DataRowView drv) {
			TextBox txtObservacao = item.FindControl("txtObservacao") as TextBox;
			TextBox txtInicio = item.FindControl("txtInicio") as TextBox;
			TextBox txtFim = item.FindControl("txtFim") as TextBox;

			HiddenField hidRespFinanceiro = item.FindControl("hidRespFinanceiro") as HiddenField;
			HiddenField hidNmRespFinanceiro = item.FindControl("hidNmRespFinanceiro") as HiddenField;
			ImageButton btnRespFinanceiro = item.FindControl("btnRespFinanceiro") as ImageButton;
			Label lblRespFinanceiro = item.FindControl("lblRespFinanceiro") as Label;

			HiddenField hidRespFamilia = item.FindControl("hidRespFamilia") as HiddenField;
			HiddenField hidNmRespFamilia = item.FindControl("hidNmRespFamilia") as HiddenField;
			ImageButton btnRespFamilia = item.FindControl("btnRespFamilia") as ImageButton;
			Label lblRespFamilia = item.FindControl("lblRespFamilia") as Label;

			txtObservacao.Text = Convert.ToString(drv["ds_observacao"]);
			if (drv["dt_inicio_vigencia"] != DBNull.Value)
				txtInicio.Text = Convert.ToDateTime(drv["dt_inicio_vigencia"]).ToString("dd/MM/yyyy");
			if (drv["dt_fim_vigencia"] != DBNull.Value)
				txtFim.Text = Convert.ToDateTime(drv["dt_fim_vigencia"]).ToString("dd/MM/yyyy");

			hidRespFinanceiro.Value = Convert.ToString(drv["cd_beneficiario_financeiro"]);
			hidNmRespFinanceiro.Value = Convert.ToString(drv["nm_beneficiario_financeiro"]);
			lblRespFinanceiro.Text = Convert.ToString(drv["nm_beneficiario_financeiro"]);

			hidRespFamilia.Value = Convert.ToString(drv["cd_beneficiario_plano"]);
			hidNmRespFamilia.Value = Convert.ToString(drv["nm_beneficiario_plano"]);
			lblRespFamilia.Text = Convert.ToString(drv["nm_beneficiario_plano"]);

			btnRespFinanceiro.OnClientClick = "return openPopResponsavel(" + 
				"'" + item.ClientID + "', " +
				item.ItemIndex + ", " +
				"'" + hidRespFinanceiro.ID + "', " +
				"'" + hidNmRespFinanceiro.ID + "', " +
				"'" + lblRespFinanceiro.ID + "');";
			btnRespFamilia.OnClientClick = "return openPopResponsavel(" +
				"'" + item.ClientID + "', " +
				item.ItemIndex + ", " +
				"'" + hidRespFamilia.ID + "', " +
				"'" + hidNmRespFamilia.ID + "', " +
				"'" + lblRespFamilia.ID + "');"; ;


		}

		private void RemoveRow(int index) {
			
			DataTable dt = this.RetrieveItemsFromDataList(null);
			dt.Rows.RemoveAt(index);

			BindRows(dt);
		}

		#endregion

	}
}