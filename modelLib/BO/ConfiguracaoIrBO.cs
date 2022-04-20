using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class ConfiguracaoIrBO {
		EVidaLog log = new EVidaLog(typeof(ConfiguracaoIrBO));

		private static ConfiguracaoIrBO instance = new ConfiguracaoIrBO();

		public static ConfiguracaoIrBO Instance { get { return instance; } }

		public void Salvar(ConfiguracaoIrVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				ConfiguracaoIrDAO.SaveConfig(vo, db);

				connection.Commit();
			}
		}

		public ConfiguracaoIrVO GetConfiguracao() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ConfiguracaoIrDAO.GetConfig(db);
			}
		}

		public void IncluirAno(int ano) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				ConfiguracaoIrVO config = ConfiguracaoIrDAO.GetConfig(db);
				if (config.Anos.Contains(ano)) {
					return;
				} else {
					config.Anos.Add(ano);
				}

				ConfiguracaoIrDAO.SaveYearsConfig(config.Anos, db);
				connection.Commit();
			}
		}

		public void RemoverAno(int ano) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				ConfiguracaoIrVO config = ConfiguracaoIrDAO.GetConfig(db);
				connection.CreateTransaction();

				if (config.Anos.Contains(ano)) {
					config.Anos.Remove(ano);
				} else {
					return;
				}

				ConfiguracaoIrDAO.SaveYearsConfig(config.Anos, db);
				connection.Commit();
			}
		}
	}
}
