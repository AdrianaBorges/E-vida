using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class AutorizacaoIsaBO {
		EVidaLog log = new EVidaLog(typeof(AutorizacaoIsaBO));

		private static AutorizacaoIsaBO instance = new AutorizacaoIsaBO();

		public static AutorizacaoIsaBO Instance { get { return instance; } }

		public HcAutorizacaoVO GetById(int nroAutorizacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcAutorizacaoDAO.GetRowById(nroAutorizacao, db);
			}
		}

		public void AjustarDatas(int nroAutorizacao, DateTime dtInicio, DateTime dtFim, VO.UsuarioVO usuarioVO) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				HcAutorizacaoDAO.AjustarDatas(nroAutorizacao, dtInicio, dtFim, usuarioVO, db);

				connection.Commit();
			}
		}
	}
}
