using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO.Protheus {
	public class BeneficiarioBO {

		private static BeneficiarioBO instance = new BeneficiarioBO();

		public static BeneficiarioBO Instance { get { return instance; } }

		public PBeneficiarioVO GetBeneficiarioByCartao(string numCartao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PBeneficiarioDAO.GetRowByCartao(numCartao, db);
			}
		}

		public PBeneficiarioPlanoVO GetBeneficiarioPlano(PBeneficiarioVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PBeneficiarioPlanoDAO.GetPlanoBeneficiario(vo.CodInt, vo.CodEmp, vo.Matricula, db);
			}
		}
	}
}
