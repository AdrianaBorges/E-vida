using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class EmailConfigDAO {
		internal static DataTable GetConfigs(EvidaDatabase db) {
			string sql = "SELECT id_config, nm_config, ds_email_envio, ds_assunto, ds_arquivo " +
				" FROM ev_config_email config " +
				"	ORDER BY id_config ";

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

			return dt;
		}
	}
}
