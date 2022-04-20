using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {

	public class UsuarioAudit {
		public int IdUsuario { get; set; }
		public string Ip { get; set; }
		public UsuarioVO Usuario { get; set; }
	}

	public enum TipoUsuario {
		BENEFICIARIO = 'B',
		FUNCIONARIO = 'F'
	}

	public class UsuarioVO {
		public int Id { get; set; }
		public string Login { get; set; }
		public string Nome { get; set; }
		public string Email { get; set; }
		public string Cargo { get; set; }
		public long? Matricula { get; set; }
		public DateTime? UltimoLogin { get; set; }
		public string Regional { get; set; }
	}
}
