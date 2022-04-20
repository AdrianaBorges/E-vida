using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.Internal {
	public partial class Negado : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			string strModulo = Request["MODULO"];
			if (string.IsNullOrEmpty(strModulo)) {
				ltModulo.Text = "NÃO IDENTIFICADO";
			} else {
				Modulo m;
				if (!Enum.TryParse(strModulo, out m)) {
					ltModulo.Text = "INVÁLIDO";
				} else {
					ModuloVO vo = AdministracaoBO.Instance.GetModulo(m);
					if (vo == null)
						ltModulo.Text = "NÃO ENCONTRADO";
					else
						ltModulo.Text = vo.Nome;
				}
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
		}
	}
}