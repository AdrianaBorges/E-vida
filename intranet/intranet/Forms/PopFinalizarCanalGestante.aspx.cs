using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class PopFinalizarCanalGestante : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				int cdProtocolo;
				if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
					this.ShowError("A requisição está inválida!");

					this.btnSalvar.Visible = false;
					return;
				}
				Bind(cdProtocolo);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.CANAL_GESTANTE; }
		}

		private void Bind(int cdProtocolo) {
			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(cdProtocolo);
			litProtocolo.Text = vo.Id + " / " + vo.DataSolicitacao.Year;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(txtDescricao.Text)) {
				this.ShowError("Informe o procedimento realizado!");
				return;
			}
			int id = Int32.Parse(Request["ID"]);
			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(id);
			if (vo.Status == StatusCanalGestante.FINALIZADO) {
				this.ShowError("A solicitação já está finalizada");
				return;
			}

			vo.Resposta = txtDescricao.Text;
			vo.CodUsuarioFinalizacao = UsuarioLogado.Id;

			CanalGestanteBO.Instance.Finalizar(vo);
			this.RegisterScript("aqui", "setCallback(" + id + ")");
		}
	}
}