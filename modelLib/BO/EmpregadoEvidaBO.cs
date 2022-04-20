using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class EmpregadoEvidaBO {
		private EVidaLog log = new EVidaLog(typeof(EmpregadoEvidaBO));

		private static EmpregadoEvidaBO instance = new EmpregadoEvidaBO();

		public static EmpregadoEvidaBO Instance { get { return instance; } }

		public EmpregadoEvidaVO GetByMatricula(long matricula) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return EmpregadoEvidaDAO.GetByMatricula(matricula, db);
			}
		}

        public EmpregadoEvidaVO GetCoordenador(long matricula, string funcao)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return EmpregadoEvidaDAO.GetCoordenador(matricula, funcao, db);
            }
        }

        public EmpregadoEvidaVO GetDiretor(long matricula, string funcao)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return EmpregadoEvidaDAO.GetDiretor(matricula, funcao, db);
            }
        }

		public List<PrCentroCustoVO> ListarCentroCusto() {
			lock (typeof(PrCentroCustoVO)) {
				List<PrCentroCustoVO> lst = CacheHelper.GetFromCache<List<PrCentroCustoVO>>("PR_CENTRO_CUSTO");
				if (lst == null) {
					lst = ListarCentroCustoDb();
					CacheHelper.AddOnCache("PR_CENTRO_CUSTO", lst, 5);
				}
				return lst;
			}
		}

		private List<PrCentroCustoVO> ListarCentroCustoDb() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CentroCustoDAO.ListarCentroCusto(db);
				} finally {
					connection.Close();
				}
			}
		}
	}
}
