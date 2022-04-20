using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Exceptions {
	[Serializable]
	public class EvidaException : Exception {
		public EvidaException() { }
		public EvidaException(string message) : base(message) { }
		public EvidaException(string message, Exception inner) : base(message, inner) { }
		protected EvidaException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
