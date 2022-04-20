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
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios {
	public partial class RelatorioAutorizacao : RelatorioExcelPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				DataTable dt = RelatorioBO.Instance.ListarUserUpdateAutorizacao();
				dpdUsuario.DataSource = dt;
				dpdUsuario.DataBind();
				dpdUsuario.Items.Insert(0, new ListItem("TODOS", ""));
			}
			this.Form.DefaultButton = btnBuscar.UniqueID;
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RELATORIO_AUTORIZACAO; }
		}

		private bool TryGetLong(string value, out long? outLong) {
			outLong = null;
			if (string.IsNullOrEmpty(value))
				return true;
			long oL;
			if (!Int64.TryParse(value, out oL)) {
				return false;
			}
			outLong = oL;
			return true;
		}

		private void ExportarExcel() {
			DataTable sourceTable = this.GetRelatorioTable();

			ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

			defs.SetWidth("nr_cnpj_cpf", 16);
			defs.SetWidth("nm_titular", 30);
			defs.SetWidth("nm_beneficiario", 30);
			defs.SetWidth("tp_autorizacao", 25);
			defs.SetWidth("tp_sistema_atend", 15);
			defs.SetWidth("ds_servico", 60);
			defs.SetWidth("nm_razao_social", 40);
			defs.SetWidth("nm_medico_solicitante", 25);

			ExportExcel("RelatorioAutorizacoes", defs, sourceTable);
		}

		private void Buscar() {
			DateTime dtInicio;
			DateTime dtFim;
			string sistema = null;
			string status = null;
			long? cdMatricula = null;
			string titular = null;
			string dependente = null;
			int? nroAutorizacaoIsa = null;
			int? nroAutorizacaoWeb = null;
			string tipo = null;
			string cdMascara = null;
			string dsServico = null;
			long? cpf = null;
			string nmCredenciado = null;
			string userUpdate = null;

			if (!DateTime.TryParse(txtInicio.Text, out dtInicio)) {
				this.ShowError("Informe uma data inicial correta!");
				return;
			}
			if (!DateTime.TryParse(txtFim.Text, out dtFim)) {
				this.ShowError("Informe uma data final correta!");
				return;
			}
			if (!string.IsNullOrEmpty(dpdSistema.SelectedValue))
				sistema = dpdSistema.SelectedValue;

			if (!string.IsNullOrEmpty(dpdStatus.SelectedValue))
				status = dpdStatus.SelectedValue;

			if (!TryGetLong(txtMatricula.Text, out cdMatricula)) {
				this.ShowError("A matrícula deve ser numérica!");
				return;
			}
			if (!TryGetInt(txtAutorizacaoIsa.Text, out nroAutorizacaoIsa)) {
				this.ShowError("O número de autorização ISA deve ser numérico!");
				return;
			}
			if (!TryGetInt(txtAutorizacaoWeb.Text, out nroAutorizacaoWeb)) {
				this.ShowError("O número de autorização WEB deve ser numérico!");
				return;
			}
			if (!TryGetLong(txtCpf.Text, out cpf)) {
				this.ShowError("O CPF/CNPJ deve ser numérico!");
				return;
			}
			titular = dpdTitular.SelectedValue;
			dependente = dpdDependente.SelectedValue;
			tipo = dpdTipo.SelectedValue;
			cdMascara = txtCodServico.Text;
			dsServico = dpdDesServico.Text;
			nmCredenciado = dpdCredenciado.Text;
			userUpdate = dpdUsuario.SelectedValue;

			DataTable dtAcessos = RelatorioBO.Instance.BuscarAutorizacoes(dtInicio, dtFim, cdMatricula, titular, dependente,
				nroAutorizacaoIsa, nroAutorizacaoWeb, tipo, status, sistema, cdMascara, dsServico, cpf, nmCredenciado, userUpdate);

			btnExportar.Visible = dtAcessos.Rows.Count > 0;
			lblCount.Visible = true;
			lblCount.Text = "Foram encontrados " + dtAcessos.Rows.Count + " registros.";

			this.ShowPagingGrid(gdvRelatorio, dtAcessos, null);

		}

		private bool TryGetInt(string value, out int? outInt) {
			outInt = null;
			if (string.IsNullOrEmpty(value))
				return true;
			int oL;
			if (!Int32.TryParse(value, out oL)) {
				return false;
			}
			outInt = oL;
			return true;
		}

		#region Eventos

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try { Buscar(); }
			catch (Exception ex) {
				this.ShowError("Erro ao consultar o relatorio: " + ex.Message);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				ExportarExcel();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar!");
				Log.Error("Erro ao exportar para excel!", ex);
			}
		}
		
		protected void imgBuscaTitular_Click(object sender, ImageClickEventArgs e) {
			try {
				BuscaTitular(txtFTitular.Text);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar titulares.");
				Log.Error("Erro ao buscar titulares", ex);
			}
		}

		protected void imgBuscaDependente_Click(object sender, ImageClickEventArgs e) {
			try {
				BuscaDependente(txtFDependente.Text);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar dependentes.");
				Log.Error("Erro ao buscar dependentes", ex);
			}
		}

		protected void imgBuscaDesServico_Click(object sender, ImageClickEventArgs e) {
			try {
				BuscaDesServico(txtFDesServico.Text);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar serviços.");
				Log.Error("Erro ao buscar serviços", ex);
			}
		}

		protected void imgBuscaCredenciado_Click(object sender, ImageClickEventArgs e) {
			try {
				BuscaCredenciado(txtFCredenciado.Text);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar credenciados.");
				Log.Error("Erro ao buscar credenciados", ex);
			}
		}
		
		#endregion

		#region Buscas LOV

		private void BuscaTitular(string nome) {
			DataTable dt = Session["NOMES_TITULARES"] as DataTable;
			if (dt == null) {
				dt = PLocatorDataBO.Instance.ListarNomesTitulares();
				Session["NOMES_TITULARES"] = dt;
			}
			DoLov(nome, "BA1_NOMUSR", dt, dpdTitular, "titular");
		}

		private void BuscaDependente(string nome) {
			DataTable dt = Session["NOMES_BENEFICIARIOS"] as DataTable;
			if (dt == null) {
				dt = PLocatorDataBO.Instance.ListarNomesBeneficiarios();
				Session["NOMES_BENEFICIARIOS"] = dt;
			}
			DoLov(nome, "BA1_NOMUSR", dt, dpdDependente, "dependente");
		}

		private void BuscaDesServico(string nome) {
			DataTable dt = null;
			if (nome.Length >= 3) dt = PLocatorDataBO.Instance.BuscarServicos(null, nome, false);

			DoLov(nome, null, dt, dpdDesServico, "serviço");
		}

		private void BuscaCredenciado(string nome) {
			DataTable dt = Session["CREDENCIADOS"] as DataTable;
			if (dt == null) {
                dt = PLocatorDataBO.Instance.ListarRedesAtendimento();
				Session["CREDENCIADOS"] = dt;
			}
			DoLov(nome, "BAU_NOME", dt, dpdCredenciado, "credenciado");
		}

		private void DoLov(string value, string column, DataTable dt, DropDownList dpd, string nomeItem) {

			if (value.Length >= 3) {
				DataView dv = new DataView(dt);
				
				if (column != null)
					dv.RowFilter = column + @" LIKE '%" + value.ToUpper() + "%'";

				if (dv.Count == 0) {
					this.ShowInfo("Não foi encontrado nenhum " + nomeItem + " com este filtro!");
				} else if (dv.Count > 300) {
					this.ShowInfo("A pesquisa encontrou muitos registros, apenas os 300 primeiros serão exibidos. Por favor, informe mais detalhes!");
					DataTable dt2 = dv.ToTable().AsEnumerable().Take(300).CopyToDataTable();
					dv = dt2.DefaultView;
				}

				dpd.DataSource = dv;
				dpd.DataBind();
			} else {
				if (value.Length != 0) {
					this.ShowError("Por favor, informe pelo menos 3 caracteres para filtro!");
				}

				dpd.Items.Clear();
				dpd.Items.Add(new ListItem("TODOS", ""));
			}
		}

		#endregion
	}
}