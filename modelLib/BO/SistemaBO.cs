using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class SistemaBO {
		private static SistemaBO instance = new SistemaBO();

		public static SistemaBO Instance { get { return instance; } }

		public void KillDbSession(String killArgument) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				SistemaDAO.KillDbSession(killArgument, db);
				connection.Commit();
			}
		}
	}
}
