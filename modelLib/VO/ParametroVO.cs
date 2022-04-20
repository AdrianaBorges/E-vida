using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	[Serializable]
	public class ParametroVO {
		public int Id { get; set; }
		public string Value { get; set; }
	}
	[Serializable]
	public class ParametroVariavelVO {
		public ParametroUtil.ParametroVariavelType Id {
			get { return (ParametroUtil.ParametroVariavelType)this.ParamId; }
		}
		public int IdLinha { get; set; }
		public int ParamId { get; set; }
		public DateTime Inicio { get; set; }
		public DateTime Fim { get; set; }
		public int IdUsuarioCriacao { get; set; }
		public int? IdUsuarioAlteracao { get; set; }
		public DateTime Criacao { get; set; }
		public DateTime? Alteracao { get; set; }
		public string Value { get; set; }
	}
}
