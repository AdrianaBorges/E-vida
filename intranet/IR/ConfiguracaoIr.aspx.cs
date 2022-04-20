using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.IR {
	public partial class ConfiguracaoIr : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				for (int i = 1; i <= 29; ++i) {
					dpdDiaIrBeneficiario.Items.Add(new ListItem(i.ToString(), i.ToString()));
				}

				Bind();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.IR_CONFIGURACAO; }
		}

		private void Bind() {
			ConfiguracaoIrVO vo = ConfiguracaoIrBO.Instance.GetConfiguracao();
			dpdIrBeneficiario.SelectedValue = vo.EnableIrBeneficiario ? "S" : "N";
			dpdIrCredenciados.SelectedValue = vo.EnableIrCredenciado ? "S" : "N";
            txtEnderecoEVIDA.Text = vo.EnderecoEVIDA;

			gdvAnos.DataSource = vo.Anos;
			gdvAnos.DataBind();

			dpdDiaIrBeneficiario.SelectedValue = vo.DayIrBeneficiario.ToString();
		}

		private void Salvar() {

            string enderecoEVIDA = txtEnderecoEVIDA.Text;

            if (string.IsNullOrEmpty(enderecoEVIDA))
            {
                this.ShowError("O campo Endereço da E-VIDA não pode estar em branco!");
                return;
            }

			ConfiguracaoIrVO vo = new ConfiguracaoIrVO();
			vo.EnableIrCredenciado = dpdIrCredenciados.SelectedValue.Equals("S");
			vo.EnableIrBeneficiario = dpdIrBeneficiario.SelectedValue.Equals("S");
			vo.DayIrBeneficiario = Int32.Parse(dpdDiaIrBeneficiario.SelectedValue);
            vo.EnderecoEVIDA = txtEnderecoEVIDA.Text;

			ConfiguracaoIrBO.Instance.Salvar(vo);
			this.ShowInfo("Configuração salva com sucesso!");
			Bind();
		}

		private void IncluirAno() {
			int ano = 0;

			if (!Int32.TryParse(txtAno.Text, out ano)) {
				this.ShowError("O ano deve ser numérico e superior a 2012");
				return;
			} else if (ano <= 2012) {
				this.ShowError("O ano deve ser numérico e superior a 2012");
				return;
			}

			ConfiguracaoIrBO.Instance.IncluirAno(ano);

			this.ShowInfo("Ano incluído com sucesso!");
			Bind();
		}

		private void RemoverAno(int ano) {
			ConfiguracaoIrBO.Instance.RemoverAno(ano);

			this.ShowInfo("Ano removido com sucesso!");
			Bind();
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar configuracao.", ex);
			}
		}

		protected void btnIncluirAno_Click(object sender, EventArgs e) {
			try {
				IncluirAno();
			} catch (Exception ex) {
				this.ShowError("Erro ao incluir ano!", ex);
			}
		}

		protected void gdvAnos_RowCommand(object sender, GridViewCommandEventArgs e) {
			try {
				if (e.CommandArgument != null && e.CommandArgument.ToString().Length > 0) {
					int ano = Convert.ToInt32(e.CommandArgument);

					if (e.CommandName == "CmdExcluir") {
						RemoverAno(ano);
					} else {
						this.ShowError("Comando não reconhecido: " + e.CommandName);
					}
				}
			} catch (Exception ex) {
				this.ShowError("Erro ao executar a ação! " + e.CommandName + " - " + e.CommandArgument, ex);
			}
		}

	}
}