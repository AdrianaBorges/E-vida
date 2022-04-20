using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace eVidaBeneficiarios.Classes {
	public abstract class FormPageBase : PageBase {
		protected bool CheckBeneficiario(string codint, string codemp, string matric) {
            if (UsuarioLogado.Codint.Trim() != codint.Trim() || UsuarioLogado.Codemp.Trim() != codemp.Trim() || UsuarioLogado.Matric.Trim() != matric.Trim()) return false;
			return true;
		}
	}
}