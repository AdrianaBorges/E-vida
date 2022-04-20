using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.VO;

namespace eVidaIntranet {
	public partial class Inicial : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {

		}
		protected override Modulo Modulo {
			get { return Modulo.INICIAL; }
		}

	}
}