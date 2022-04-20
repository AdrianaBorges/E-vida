using eVidaGeneralLib.DAO.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class SistemaDAO {
		public static void KillDbSession(String killArgument, EvidaDatabase db) {
			string sql = "alter system kill session '" + killArgument + "'";
			List<Parametro> lstParam = new List<Parametro>();
			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}
	}
}
