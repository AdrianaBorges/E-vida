using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO.Protheus {
	public class ServicoBO {
		private static ServicoBO instance = new ServicoBO();

		public static ServicoBO Instance { get { return instance; } }

		public PServicoVO GetById(string cdTabela, string mascara) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PServicoDAO.GetById(cdTabela, mascara, db);
			}
		}

		public DataTable Pesquisar(string cdTuss, string dsTuss, string cdPlano, bool hasInPlano, List<string> lstTabela) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PServicoDAO.PesquisarServico(cdTuss, dsTuss, cdPlano, hasInPlano, lstTabela, db);
			}
		}

		public PServicoVO GetByMascara(string mascara) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return PServicoDAO.GetByMascara(mascara, db);
			}
		}

	}
}
