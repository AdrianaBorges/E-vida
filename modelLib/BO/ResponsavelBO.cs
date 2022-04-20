using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.BO {
	public class ResponsavelBO {

		EVidaLog log = new EVidaLog(typeof(ResponsavelBO));

		private static ResponsavelBO instance = new ResponsavelBO();

		public static ResponsavelBO Instance { get { return instance; } }

		public DataTable BuscarBeneficiarios(long matricula, string nome, string cdAlternativo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ResponsavelDAO.BuscarBeneficiarios(nome, matricula, cdAlternativo, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarResponsaveis(int cdBeneficiario) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ResponsavelDAO.BuscarResponsaveis(cdBeneficiario, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarPossiveisResponsaveis(long matricula, string nome) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return ResponsavelDAO.BuscarPossiveisResponsaveis(matricula, nome, db);
				}
				finally {
					connection.Close();
				}
			}
		}

		public void SalvarResponsaveis(int cdBeneficiario, List<VO.ResponsavelVO> lst, int idUsuario) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando responsáveis. Beneficiario: " + cdBeneficiario);

					ResponsavelDAO.SalvarResponsaveis(cdBeneficiario, lst, idUsuario, transaction, db);

					transaction.Commit();
				}
				catch {
					transaction.Rollback();
					throw;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarDependentes(string codint, string codemp, string matric, TipoResponsavel tipo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
                    return ResponsavelDAO.BuscarDependentes(codint, codemp, matric, tipo, db);
				}
				finally {
					connection.Close();
				}
			}
		}
	}
}
