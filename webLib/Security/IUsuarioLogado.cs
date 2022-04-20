using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVida.Web.Security {
	public interface IUsuarioLogado {
		string Username { get; }

		bool HasPermission(eVidaGeneralLib.VO.Modulo modulo);
	}
}
