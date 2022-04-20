using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class MotivoPendenciaBO {
		EVidaLog log = new EVidaLog(typeof(MotivoPendenciaBO));

		private static MotivoPendenciaBO instance = new MotivoPendenciaBO();

		public static MotivoPendenciaBO Instance { get { return instance; } }

		
		public MotivoPendenciaVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return MotivoPendenciaDAO.GetById(id, db);
			}
		}

		public List<MotivoPendenciaVO> ListarAllMotivos() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return MotivoPendenciaDAO.ListarMotivos(null, db);
			}
		}

		public List<MotivoPendenciaVO> ListarMotivos(TipoMotivoPendencia tipo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return MotivoPendenciaDAO.ListarMotivos(tipo, db);
			}
		}

		public bool CheckAlteracao(MotivoPendenciaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				MotivoPendenciaVO oldVO = MotivoPendenciaDAO.GetById(vo.Id, db);
				if (oldVO == null)
					return true;

				if (oldVO.Tipo != vo.Tipo)
					return !IsMotivoUtilizado(vo, db);

				return true;
			}
		}
		public bool IsMotivoUtilizado(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				MotivoPendenciaVO oldVO = MotivoPendenciaDAO.GetById(id, db);
				return IsMotivoUtilizado(oldVO, db);
			}
		}

		public void Salvar(MotivoPendenciaVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				MotivoPendenciaDAO.Salvar(vo, db);

				connection.Commit();
			}
		}

		public void Excluir(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				MotivoPendenciaDAO.Excluir(id, db);

				connection.Commit();
			}
		}



		#region CheckUtilizacao

		private bool IsMotivoUtilizado(MotivoPendenciaVO vo, EvidaDatabase db) {
			if (vo.Tipo == TipoMotivoPendencia.PROTOCOLO_FATURA) {
				return IsMotivoProtocoloUtilizado(vo.Id, db);
			}
			throw new InvalidOperationException("Tipo de motivo " + vo.Tipo + " não suportado ainda!");
		}

		private bool IsMotivoProtocoloUtilizado(int id, EvidaDatabase db) {
			return ProtocoloFaturaDAO.HasPendenciaUtilizada(id, db);
		}
		#endregion
	}
}
