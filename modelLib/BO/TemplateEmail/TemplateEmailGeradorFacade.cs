using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO.TemplateEmail {
	internal class TemplateEmailGeradorFacade {
		public static string[] GetTagsByTipo(TipoTemplateEmail tipo) {
			if (tipo == TipoTemplateEmail.PROTOCOLO_FATURA) {
				return TemplateEmailGeradorProtocoloFatura.TAG_PROTOCOLO_FATURA;
			}
			throw new InvalidOperationException("Tipo de Template não definido");
		}
		public static TemplateEmailGerador GetGerador(TemplateEmailVO vo, EvidaDatabase db, SortedDictionary<string, string> parametros) {
			if (vo.Tipo == TipoTemplateEmail.PROTOCOLO_FATURA)
				return new TemplateEmailGeradorProtocoloFatura(vo, db, parametros);
			return null;
		}
	}
}
