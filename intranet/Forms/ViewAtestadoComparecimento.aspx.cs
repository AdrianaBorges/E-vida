using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms {
	public partial class ViewAtestadoComparecimento : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			try {
				if (!IsPostBack) {
					if (!string.IsNullOrEmpty(Request["ID"])) {
						int id = Int32.Parse(Request["ID"]);
						ViewState["ID"] = id;
						Bind(id);
					} else {
						this.ShowError("Parâmetros inválidos!");
					}
				}
			}
			catch (Exception ex) {
				this.ShowError(ex);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ATESTADO_COMPARECIMENTO; }
		}

		private void Bind(int id) {
			AtestadoComparecimentoVO vo = AtestadoComparecimentoBO.Instance.GetById(id);
			Bind(vo);
		}

		private void Bind(AtestadoComparecimentoVO vo) {
			lblProtocolo.Text = vo.CodSolicitacao.ToString(AtestadoComparecimentoVO.FORMATO_PROTOCOLO);

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
			txtNomeTitular.Text = vo.Nome;
			txtCartao.Text = titular.Matant;
			txtLotacao.Text = vo.Lotacao;
			txtData.Text = vo.DataAtendimento.ToShortDateString();

			txtHoraInicial.Text = vo.HoraInicio;
			txtHoraFinal.Text = vo.HoraFim;

			foreach (ListItem item in chkTipoAtendimento.Items) {
				int val = Int32.Parse(item.Value);
				item.Selected = (val & vo.TipoPericia) == val;
			}

            if (!String.IsNullOrEmpty(vo.Codint) && !String.IsNullOrEmpty(vo.Codemp) && !String.IsNullOrEmpty(vo.Matric) && !String.IsNullOrEmpty(vo.Tipreg))
            {
				PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
				txtBeneficiario.Text = benef.Nomusr;
			} else {
				txtBeneficiario.Text = "TITULAR";
			}

			ltData.Text = String.Format("{0:dd \\de MMMM \\de yyyy}", vo.DataCriacao);
		}

	}
}