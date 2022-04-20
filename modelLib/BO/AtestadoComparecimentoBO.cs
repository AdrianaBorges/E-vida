using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class AtestadoComparecimentoBO {
		EVidaLog log = new EVidaLog(typeof(AtestadoComparecimentoBO));

		private static AtestadoComparecimentoBO instance = new AtestadoComparecimentoBO();

		public static AtestadoComparecimentoBO Instance { get { return instance; } }

		public void Salvar(AtestadoComparecimentoVO vo, bool finalizar) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
                    log.Debug("Salvando solicitação - Codint: " + vo.Codint + " - Codemp: " + vo.Codemp + " - Matric: " + vo.Matric + " - Tipreg: " + vo.Tipreg + " - Nome: " + vo.Nome);

				AtestadoComparecimentoDAO.Salvar(vo, db);

				if (finalizar) {
					log.Debug("Finalizando solicitação: " + vo.CodSolicitacao);
					AtestadoComparecimentoDAO.Finalizar(vo.CodSolicitacao, vo.IdUsuarioFinalizacao.Value, db);
				}

				connection.Commit();
			}
		}

		public AtestadoComparecimentoVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				AtestadoComparecimentoVO vo = AtestadoComparecimentoDAO.GetById(id, db);
				return vo;
			}
		}

		public DataTable Pesquisar(string matricula, int? cdProtocolo, StatusAtestadoComparecimento? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AtestadoComparecimentoDAO.Pesquisar(matricula, cdProtocolo, status, db);
			}
		}

	}
}
