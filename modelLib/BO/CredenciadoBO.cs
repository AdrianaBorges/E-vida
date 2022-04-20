using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO.SCL;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.SCL;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class CredenciadoBO {
		EVidaLog log = new EVidaLog(typeof(CredenciadoBO));

		private static CredenciadoBO instance = new CredenciadoBO();

		public static CredenciadoBO Instance { get { return instance; } }

		public HcVCredenciadoVO LogarCredenciado(string login, string senha) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();

				SclUsuarioVO uVO = SclUsuarioDAO.Logar(login, senha, db);
				if (uVO == null)
					return null;
				long cpfCnpj;
				if (!Int64.TryParse(uVO.Login, out cpfCnpj)) {
					return new HcVCredenciadoVO();
				}

				HcVCredenciadoVO vo = HcVCredenciadoDAO.GetByDoc(cpfCnpj, db);
				if (vo == null)
					return new HcVCredenciadoVO();
				return vo;
			}
		}

		public HcVCredenciadoVO GetByDoc(long cpfCnpj) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVCredenciadoDAO.GetByDoc(cpfCnpj, db);
			}
		}

		public HcVCredenciadoVO GetById(int idCredenciado) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVCredenciadoDAO.GetById(idCredenciado, db);
			}
		}

		public DataTable Pesquisar(string razaoSocial, long? cpfCnpj, bool apenasHospital) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVCredenciadoDAO.Pesquisar(razaoSocial, cpfCnpj, apenasHospital, db);
			}
		}

		public KeyValuePair<int, string> GetRegional(int cdCredenciado, DateTime? dtVigencia = null) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();

				int cdRegional = HcVCredenciadoDAO.GetRegionalCredenciado(cdCredenciado, dtVigencia, db);
				string nmRegional = LocatorDataBO.Instance.GetRegional(cdRegional, db);
				return new KeyValuePair<int, string>(cdRegional, nmRegional);
			}
		}

		public List<HcCredenciadoFoneVO> ListarFones(int cdCredenciado) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVCredenciadoDAO.ListarFones(cdCredenciado, db);
			}
		}

		public List<HcCredenciadoEnderecoVO> ListarEnderecos(int cdCredenciado) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVCredenciadoDAO.ListarEnderecos(cdCredenciado, db);
			}
		}
	}
}
