using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data.Common;

namespace eVidaGeneralLib.DAO.Util
{
    internal class EvidaConnectionHolder : IDisposable {
		private DbConnection Connection { get; set; }
		public DbTransaction Transaction { get; private set; }

		public EvidaConnectionHolder(Database db) {
			Connection = db.CreateConnection();
			Connection.Open();
		}

		public DbTransaction CreateTransaction() {
			Transaction = Connection.BeginTransaction();
			return Transaction;
		}

		public void Commit() {
			Transaction.Commit();
			Transaction = null;
		}

		public void Rollback() {
			Transaction.Rollback();
			Transaction = null;
		}

		public void Dispose() {
			if (Transaction != null) {
				Transaction.Rollback();
				Transaction.Dispose();
			}
			if (Connection != null)
				Connection.Dispose();
		}
	}
}
