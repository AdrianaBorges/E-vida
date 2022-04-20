using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Forms {
	public partial class PopImprimirProtocoloFatura : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");
				return;
			}
			Bind(cdProtocolo);
		}

		protected override Modulo Modulo {
			get { return Modulo.PROTOCOLO_FATURA; }
		}

		private void Bind(int idProtocolo) {
			ProtocoloFaturaVO vo = ProtocoloFaturaBO.Instance.GetById(idProtocolo);

			litProtocolo.Text = vo.NrProtocolo; //vo.Id.ToString(ProtocoloFaturaVO.FORMATO_PROTOCOLO) + "/" + vo.AnoEntrada;

			litDataEntrada.Text = vo.DataEntrada.ToString("dd/MM/yyyy");

            /*string natureza = "";
            List<PEspecialidadeVO> lista_especialidade = PLocatorDataBO.Instance.ListarEspecialidade(vo.RedeAtendimento.Codigo);
            if (lista_especialidade.Count > 0)
            {
                foreach (PEspecialidadeVO especialidade in lista_especialidade)
                {
                    natureza += (natureza != "" ? ", " + especialidade.Descri : especialidade.Descri);
                }
            }
            else
            {
                natureza = "-";
            }*/

            string natureza = "";
            PRedeAtendimentoVO credenciado = PRedeAtendimentoBO.Instance.GetById(vo.RedeAtendimento.Codigo);
            if (credenciado != null)
            {
                PClasseRedeAtendimentoVO classerede = PLocatorDataBO.Instance.GetClasseRedeAtendimento(credenciado.Tippre);
                if (classerede != null)
                {
                    natureza = classerede.Descri;
                }
            }

            litNatureza.Text = natureza;

			UsuarioVO usuarioVO = UsuarioBO.Instance.GetUsuarioById(vo.CdUsuarioCriacao);
			litUsuarioProtocolo.Text = usuarioVO.Login;

			litCpfCnpj.Text = FormatUtil.FormatCpfCnpj(vo.RedeAtendimento.Tippe, vo.RedeAtendimento.Cpfcgc);
			litRazaoSocial.Text = vo.RedeAtendimento.Nome;
		}

	}
}