using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO.Protheus {
	public class PlanoBO {
		private static PlanoBO instance = new PlanoBO();

		public static PlanoBO Instance { get { return instance; } }

		public List<PPlanoVO> ListarPlanos() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PPlanoDAO.ListarPlanos(db);
			}
		}

		public PPlanoVO GetPlano(string cdInt, string codigo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PPlanoDAO.Get(cdInt, codigo, db);
			}
		}
	}
}
