using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class FuncionarioBO {
		private EVidaLog log = new EVidaLog(typeof(FuncionarioBO));

		private static FuncionarioBO instance = new FuncionarioBO();

		public static FuncionarioBO Instance { get { return instance; } }

		/*public void SalvarDadosPessoais(HcFuncionarioVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Salvando dados. Funcionario: " + vo.CdFuncionario);

				HistoricoDadoPessoalDAO.SalvarHistorico(vo, db);
				HcFuncionarioDAO.SalvarDadosPessoais(vo, db);

				connection.Commit();
			}
		}*/

		public HcFuncionarioVO GetByMatricula(int cdEmpresa, long matricula) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcFuncionarioDAO.GetByMatricula(cdEmpresa, matricula, db);
			}
		}
	}
}
