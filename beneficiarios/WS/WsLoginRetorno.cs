using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace eVidaBeneficiarios.WS {
	[DataContract(Namespace="evida.org.br")]
	public class WsLoginRetorno {
		[DataMember]
		public bool Status { get; set;}
		[DataMember(Name="Motivo_restricao")]
		public string MotivoRestricao { get; set;}
		[DataMember(Name="usuario_login")]
		public WsUsuario UsuarioLogin { get; set; }
		[DataMember(Name="familia")]
		public List<WsUsuario> Familia { get; set; }
	}

	[DataContract(Namespace = "evida.org.br", Name="usuario")]
	public class WsUsuario {
		[DataMember(Name="Id_sistema_interno")]
		public string Id { get; set; }
		[DataMember(Name="matricula")]
		public string Matricula { get; set; }
	}
}