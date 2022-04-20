using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Util {

	internal class EvidaDatabase : IDisposable {
		public Database Database { get; private set; }
		public EvidaConnectionHolder CurrentConnection { get; private set; }
		public UsuarioAudit UsuarioAuditoria { get; private set; }

		private EvidaDatabase(UsuarioAudit usuario) {
			UsuarioAuditoria = usuario;
		}

		public DbTransaction CurrentTransaction {
			get {
				return CurrentConnection.Transaction;
			}
		}

		public static EvidaDatabase CreateDatabase(UsuarioAudit usuario) {
			EvidaDatabase db = new EvidaDatabase(usuario);
			db.Database = DatabaseFactory.CreateDatabase();
			return db;
		}

		public EvidaConnectionHolder CreateConnection() {
			CurrentConnection = new EvidaConnectionHolder(Database);
			return CurrentConnection;
		}

		public void Dispose() {
			if (CurrentConnection != null)
				CurrentConnection.Dispose();
		}
	}
}
