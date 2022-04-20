using eVidaGeneralLib.BO.ControleEmail;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class ControleEmailBO {
		EVidaLog log = new EVidaLog(typeof(ControleEmailBO));

		private static ControleEmailBO instance = new ControleEmailBO();

		public static ControleEmailBO Instance { get { return instance; } }

		public List<ControleEmailVO> ListarSolPendente(int maxRecords) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					List<ControleEmailVO> lst = ControleEmailDAO.ListarSolPendente(maxRecords, null, db);
					return lst;
				} finally {
					connection.Close();
				}
			}
		}

		public void EnviarEmail(ControleEmailVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Enviando Email " + vo.Id);


				ControleEmailDAO.Finalizar(vo, db);
				ControleEmailAction action = ControleEmailAction.GetAction(vo.Tipo, db);
				action.Finalizar(vo);

				EmailUtil.SendControleEmail(vo);

				connection.Commit();
			}
		}

		public void RegistrarErro(ControleEmailVO vo, Exception ex) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				log.Error("Registrando ERRO " + vo.Id, ex);

				bool lastError = ControleEmailDAO.MarcarErro(vo, ex, db);
				ControleEmailAction action = ControleEmailAction.GetAction(vo.Tipo, db);
				action.MarcarErro(vo, ex);

				connection.Commit();
			}
		}
	}
}
