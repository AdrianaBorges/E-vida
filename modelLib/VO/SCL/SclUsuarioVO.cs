using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.SCL {
	public class SclUsuarioVO {
		public string Login { get; set; }
		public string Nome { get; set; }
		public long? Matricula { get; set; }
		public bool Ativo { get; set; }
		public DateTime DataCadastro { get; set; }
		public DateTime? DataExpiracaoSenha { get; set; }

		public string UserUpdate { get; set; }
		public string DateUpdate { get; set; }

		public List<SclUsuarioDominioVO> Dominios { get; set; }
		public List<SclPerfilUsuarioVO> Perfis { get; set; }
	}

	public enum SclUsuarioDominio {
		EMPRESA,
		GALENUS_ADM,
		GAL_CNPJ_PRESTADOR,
		GAL_NRCARTEIRA_BENEF,
		GAL_OPERADORA,
		HCTISS_ADM,
		SEG_POR_USUARIO,
		UNIORG,
		UNIORG_FULL
	}

	public class SclUsuarioDominioVO {

		public string CdUsuario { get; set; }
		public SclUsuarioDominio IdDominio { get; set; }
		public string CdAutorizacaoI { get; set; }
	}

	public class SclPerfilUsuarioVO {
		public string CdUsuario { get; set; }
		public int CdGrupo { get; set; }
		public string CdSistema { get; set; }
		public string NmGrupo { get; set; }
	}
}
