using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace eVidaBeneficiarios.WS {
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WsLogin" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select WsLogin.svc or WsLogin.svc.cs at the Solution Explorer and start debugging.
	
	public class WsLogin : IWsLogin {
		

		public WsLoginRetorno Login(string usuario, string senha) {
			WsLoginRetorno retorno = new WsLoginRetorno();
			HcBeneficiarioVO titularVO = BeneficiarioBO.Instance.LogarBeneficiario(usuario, senha);
			if (titularVO == null) {
				retorno.Status = false;
				retorno.MotivoRestricao = "Usuario/senha inválidos!";
				return retorno;
			}

			List<HcBeneficiarioVO> lstBenefs = BeneficiarioBO.Instance.ListarBeneficiarios(titularVO.CdEmpresa, titularVO.CdFuncionario);
			if (lstBenefs == null) {
				retorno.Status = false;
				retorno.MotivoRestricao = "Beneficiários não encontrados!";
				return retorno;
			}

			HcBeneficiarioVO titular = lstBenefs.First(x => x.TpBeneficiario.Equals(Constantes.TIPO_BENEFICIARIO_FUNCIONARIO));

			retorno.UsuarioLogin = BeneficiarioToWsUsuario(titular);
			retorno.Familia = new List<WsUsuario>();
			foreach (HcBeneficiarioVO benef in lstBenefs) {
				if (benef != titular) {
					retorno.Familia.Add(BeneficiarioToWsUsuario(benef));
				}
			}
			return retorno;
		}

		private WsUsuario BeneficiarioToWsUsuario(HcBeneficiarioVO benef) {
			WsUsuario usuario = new WsUsuario();
			usuario.Id = benef.CdBeneficiario.ToString();
			usuario.Matricula = benef.CdAlternativo;
			return usuario;
		}
	}
}
