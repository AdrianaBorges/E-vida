using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class DemonstrativoAnaliseContaBO {
		EVidaLog log = new EVidaLog(typeof(DemonstrativoAnaliseContaBO));

		private static DemonstrativoAnaliseContaBO instance = new DemonstrativoAnaliseContaBO();

		public static DemonstrativoAnaliseContaBO Instance { get { return instance; } }

		public List<HcDemonstrativoAnaliseContaVO> ListarSolicitacoes(long cpfCnpj, string docFiscal) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcDemonstrativoAnaliseContaDAO.ListarSolicitacoes(cpfCnpj, docFiscal, db);
			}
		}

		public void GerarSolicitacao(HcDemonstrativoAnaliseContaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Salvando solicitação " + vo.CpfCnpj + " - " + vo.DocumentoFiscal + " - " + vo.Referencia);

				HcDemonstrativoAnaliseContaDAO.GerarSolicitacao(vo, db);

				connection.Commit();
			}
		}
	}
}
