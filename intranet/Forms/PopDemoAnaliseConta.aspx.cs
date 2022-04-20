using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class PopDemoAnaliseConta : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				this.btnSolicitar.Visible = false;
				int cdProtocolo;
				if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
					this.ShowError("A requisição está inválida!");

					this.btnSolicitar.Visible = false;
					return;
				}

				string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
				foreach (string name in monthNames) {
					if (!string.IsNullOrEmpty(name))
						dpdMes.Items.Add(new ListItem(name.ToUpper(), (dpdMes.Items.Count + 1).ToString()));
				}
				dpdMes.SelectedValue = DateTime.Now.Month.ToString();

				for (int i = DateTime.Now.Year - 5; i <= DateTime.Now.Year + 1; ++i) {
					dpdAno.Items.Add(new ListItem(i + "", i + ""));
				}
				dpdAno.SelectedValue = DateTime.Now.Year.ToString();

				Bind(cdProtocolo);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.PROTOCOLO_FATURA; }
		}

		private int CdProtocolo {
			get { return (int)ViewState["ID"]; }
			set { ViewState["ID"] = value; }
		}

		private void Bind(int cdProtocolo) {
			CdProtocolo = cdProtocolo;

			ProtocoloFaturaVO vo = ProtocoloFaturaBO.Instance.GetById(cdProtocolo);
			if (vo == null) {
				this.ShowError("Protocolo de Fatura não encontrado!");
				return;
			}

			if (string.IsNullOrEmpty(vo.DocumentoFiscal)) {
				this.ShowError("Para demonstrativo de análise de conta é necessário que o protocolo tenha preenchido o Documento Fiscal!");
				return;
			}

			List<HcDemonstrativoAnaliseContaVO> lst = DemonstrativoAnaliseContaBO.Instance.ListarSolicitacoes(Int64.Parse(vo.RedeAtendimento.Cpfcgc), vo.DocumentoFiscal);

			if (lst != null) {
				lst.Sort((x, y) => y.Referencia.CompareTo(x.Referencia));
			}

			gdvSolicitacoes.DataSource = lst;
			gdvSolicitacoes.DataBind();

			this.btnSolicitar.Visible = true;
		}

		private void Salvar() {
			ProtocoloFaturaVO vo = ProtocoloFaturaBO.Instance.GetById(CdProtocolo);

			HcDemonstrativoAnaliseContaVO demoVO = new HcDemonstrativoAnaliseContaVO();
            demoVO.CpfCnpj = Int64.Parse(vo.RedeAtendimento.Cpfcgc);
			demoVO.DocumentoFiscal = vo.DocumentoFiscal;
			demoVO.Referencia = new DateTime(Int32.Parse(dpdAno.SelectedValue), Int32.Parse(dpdMes.SelectedValue), 1);

			DemonstrativoAnaliseContaBO.Instance.GerarSolicitacao(demoVO);

			this.ShowInfo("Solicitacao enviada com sucesso!");
			Bind(CdProtocolo);
		}

		protected void btnSolicitar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar solicitacao!", ex);
			}
		}

	}
}