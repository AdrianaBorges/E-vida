using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Gestao {
	public partial class Quitacao : RelatorioExcelPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			string id = Request["ID"];

			if (!IsPostBack) {
				string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
				foreach (string name in monthNames) {
					if (!string.IsNullOrEmpty(name))
						dpdMes.Items.Add(new ListItem(name.ToUpper(), (dpdMes.Items.Count + 1).ToString()));
				}
				dpdMes.SelectedValue = DateTime.Now.Month.ToString();

				for (int i = DateTime.Now.Year - 1; i <= DateTime.Now.Year+1; ++i) {
					dpdAno.Items.Add(new ListItem(i + "", i + ""));
				}
				dpdAno.SelectedValue = DateTime.Now.Year.ToString();

				dpdTipoArquivo.Items.Add(new ListItem("SELECIONE", ""));
				foreach (TipoArquivoSapEnum tipoArq in Enum.GetValues(typeof(TipoArquivoSapEnum))) {
					dpdTipoArquivo.Items.Add(new ListItem(ArquivoSapEnumTradutor.TraduzTipoArquivo(tipoArq), tipoArq.ToString()));
				}

				if (!string.IsNullOrEmpty(id)) {
					long cod;
					if (!Int64.TryParse(id, out cod)) {
						this.ShowError("Identificador de requisição inválido!");
						return;
					}
					Bind(cod);
				} else {
					this.hidRnd.Value = "";
				}
			}
		}


		protected override Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.GESTAO_QUITACAO; }
		}

		private void Bind(long id) {
			try {
				this.ViewState["ID"] = id;

				this.hidRnd.Visible = false;
				this.fileName.Value = "";
				this.file.Text = "";
				dpdMes.Enabled = false;
				dpdAno.Enabled = false;
				dpdTipoArquivo.Enabled = false;
				txtRecebimento.Enabled = false;
				this.divUpload.Visible = false;	

				btnCancelar.Visible = true;
				btnExportar.Visible = true;
				btnImportar.Visible = false;
				btnQuitar.Visible = true;
				
				ArquivoSapVO vo = QuitacaoBO.Instance.GetById(id);

				this.file.Text = vo.Nome;
				this.dpdAno.SelectedValue = vo.DataFolha.Year.ToString();
				this.dpdMes.SelectedValue = vo.DataFolha.Month.ToString();
				this.txtRecebimento.Text = vo.DataRecebimento.ToString("dd/MM/yyyy");
				this.dpdTipoArquivo.SelectedValue = vo.TipoArquivo.ToString();

				string status = vo.Status;
				if (status.Equals(ArquivoSapVO.ST_CANCELADO)) {
					btnQuitar.Visible = false;
					btnCancelar.Visible = false;
				} else if (status.Equals(ArquivoSapVO.ST_QUITADO)) {
					btnQuitar.Visible = false;
				}

				BindGrid(id);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao apresentar os dados!", ex);
			}
		}

		private void BindGrid(long id) {
			DataTable dt = QuitacaoBO.Instance.RelatorioInconsistencia(id);

			btnExportar.Visible = dt.Rows.Count > 0;
			lblCount.Visible = true;
			lblCount.Text = "Foram encontrados " + dt.Rows.Count + " registros.";
			
			this.ShowPagingGrid(gdvRelatorio, dt, "cd_funcionario");
		}

		private void Importar() {
			DateTime dtFolha;
			DateTime dtRecebimento;
			TipoArquivoSapEnum tipoArquivo;

			this.file.Text = fileName.Value;
			if (string.IsNullOrEmpty(fileName.Value)) {
				this.ShowError("Informe o arquivo CSV a ser importado.");
				return;
			}

			if (string.IsNullOrEmpty(dpdAno.SelectedValue) || string.IsNullOrEmpty(dpdMes.SelectedValue)) {
				this.ShowError("Informe a data de Folha!");
				return;
			}

			dtFolha = new DateTime(Int32.Parse(dpdAno.SelectedValue), Int32.Parse(dpdMes.SelectedValue), 1);

			if (!DateTime.TryParse(txtRecebimento.Text, out dtRecebimento)) {
				this.ShowError("Informe a data de recebimento!");
				return;
			}

			if (!Enum.TryParse(dpdTipoArquivo.SelectedValue, out tipoArquivo)) {
				this.ShowError("Informe o tipo do arquivo!");
				return;
			}

			if (QuitacaoBO.Instance.PossuiSeqAnterior(tipoArquivo, dtFolha)) {
				this.ShowError("Existe um arquivo para esta data folha que não está cancelado! Por favor, realize o cancelamento do mesmo antes de importar um novo arquivo.");
				return;
			}

			ArquivoSapVO vo = ParsarArquivo(fileName.Value, dtFolha);
			if (vo == null) {
				this.ShowError("Existem erros que impossibilitaram a importação do arquivo!");
				return;
			}

			List<ArquivoSapVerbaVO> verbas = QuitacaoBO.Instance.ListarVerbas(tipoArquivo);

			if (!CheckVerbas(vo, verbas)) {
				this.ShowError("O arquivo enviado não possui nenhuma verba cadastrada correspondente ao tipo de arquivo: " + tipoArquivo + ". Por favor verifique novamente o conteúdo do arquivo!");
				return;
			}

			vo.Nome = ofileName.Value;
			vo.DataFolha = dtFolha;
			vo.DataRecebimento = dtRecebimento;
			vo.TipoArquivo = tipoArquivo;

			QuitacaoBO.Instance.Importar(vo, this.UsuarioLogado.Usuario);

			this.ShowInfo("Arquivo importado com sucesso!");
			Bind(vo.IdArquivo);
		}

		/**
		 * Se ao menos uma linha conter a verba, então true
		 * */
		private bool CheckVerbas(ArquivoSapVO vo, List<ArquivoSapVerbaVO> verbas) {
			foreach (ArquivoSapItemVO item in vo.Items) {
				if (verbas.Exists(x => x.Verba == item.Verba)) return true;
			}
			return false;
		}

		private void Cancelar() {
			long id = (long)ViewState["ID"];
			ArquivoSapVO vo = QuitacaoBO.Instance.GetById(id);
			if (QuitacaoBO.Instance.ExisteParcelaPosterior(vo.TipoArquivo, vo.DataFolha)) {
				this.ShowError("Não é possível realizar o cancelamento deste arquivo pois a folha de pagamento posterior já foi fechada!");
				return;
			}

			QuitacaoBO.Instance.Cancelar(id, UsuarioLogado.Usuario);

			this.ShowInfo("Arquivo cancelado com sucesso!");

			Bind(id);
		}

		private void Quitar() {
			long id = (long)ViewState["ID"];

			QuitacaoBO.Instance.Quitar(id, UsuarioLogado.Usuario);

			this.ShowInfo("Arquivo quitado com sucesso!");

			Bind(id);
		}
		#region Parsar Arquivo
		private ArquivoSapVO ParsarArquivo(string nomeArquivo, DateTime dtFolha) {
			ArquivoSapVO vo = new ArquivoSapVO();

			vo.Items = new List<ArquivoSapItemVO>();
			using (StreamReader sr = new StreamReader(Path.Combine(Path.GetTempPath(), nomeArquivo))) {
				string linha = null;
				int lineNumber = 0;
				DateTime dtFolhaEsperado = dtFolha.AddMonths(-1);
				while ((linha = sr.ReadLine()) != null) {
					linha = linha.Trim();
					if (linha.ToUpper().StartsWith("MAT")) continue;
					lineNumber++;

					ArquivoSapItemVO item = ParsarLinha(lineNumber, linha);
					if (item == null)
						return null;

					if (item.DataReferencia != dtFolhaEsperado) {
						this.ShowError("A linha " + lineNumber + " possui uma data de referencia inconsistente (Esperado: " + dtFolhaEsperado + ", Encontrado: " + item.DataReferencia + ")!");
						return null;
					}

					item.SeqItem = lineNumber;
					vo.Items.Add(item);
				}
			}
			return vo;
		}

		private ArquivoSapItemVO ParsarLinha(int lineNumber, string linha) {
			string[] values = linha.Split(';');
			long matricula;
			int empresa;
			DateTime dataRef;
			int verba;
			double valor;

			if (values.Length < 5) {
				this.ShowError("Erro na linha " + lineNumber + " - " + linha + ". O arquivo não contém o mínimo de 5 colunas (MATRICULA, EMPRESA, ANOMES, VERBA, VALOR).");
				return null;
			}

			if (!Int64.TryParse(values[0].Trim(), out matricula)) {
				this.ShowError(GenLineErrorMessage("MATRICULA", lineNumber, values[0], linha));
				return null;
			}

			if (!Int32.TryParse(values[1].Trim(), out empresa)) {
				this.ShowError(GenLineErrorMessage("EMPRESA", lineNumber, values[1], linha));
				return null;
			}

			string anoMes = values[2].Trim();
			if (anoMes.Length != 6) {
				this.ShowError(GenLineErrorMessage("MÊS (formato AAAAMM)", lineNumber, values[2], linha));
				return null;
			} else {
				string ano = anoMes.Substring(0, 4);
				string mes = anoMes.Substring(4);
				if (!DateTime.TryParse("01/" + mes + "/" + ano, out dataRef)) {
					this.ShowError(GenLineErrorMessage("MÊS (formato AAAAMM) ", lineNumber, values[2], linha));
					return null;
				}

				
			}

			if (!Int32.TryParse(values[3].Trim(), out verba)) {
				this.ShowError(GenLineErrorMessage("VERBA", lineNumber, values[3], linha));
				return null;
			}
			if (!Double.TryParse(values[4].Trim(), out valor)) {
				this.ShowError(GenLineErrorMessage("VALOR", lineNumber, values[4], linha));
				return null;
			}

			ArquivoSapItemVO item = new ArquivoSapItemVO();
			item.CdEmpresa = empresa;
			item.CdFuncionario = matricula;
			item.DataReferencia = dataRef;
			item.Valor = valor;
			item.Verba = verba;
			return item;

		}

		private string GenLineErrorMessage(string field, int lineNumber, string value, string line) {
			return "Erro no campo " + field + " da linha " + lineNumber + " Valor encontrado: " + value + " - Linha: " + line;
		}
		#endregion

		#region Eventos
		protected void btnImportar_Click(object sender, EventArgs e) {
			try {
				Importar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao importar o arquivo! ", ex);
			}
		}

		protected void btnCancelar_Click(object sender, EventArgs e) {
			try {
				Cancelar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao cancelar o arquivo! ", ex);
			}
		}

		protected void btnExportar_Click(object sender, EventArgs e) {
			try {
				DataTable sourceTable = this.GetRelatorioTable();

				ExcelColumnDefinitionCollection defs = GetDefinitionsFromGrid(gdvRelatorio);

				ExportExcel("RelInconsistenciaQuitacao", defs, sourceTable);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao exportar a consulta! ", ex);
			}
		}

		protected void btnQuitar_Click(object sender, EventArgs e) {
			try {
				Quitar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao quitar o arquivo! ", ex);
			}
		}
		#endregion
	}
}