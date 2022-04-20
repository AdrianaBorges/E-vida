using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace eVidaBeneficiarios.WS {
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWsLogin" in both code and config file together.
	[ServiceContract(Namespace = "evida.org.br")]
	public interface IWsLogin {
		[OperationContract]
		WsLoginRetorno Login(string User_login, string User_psw);
	}
}
