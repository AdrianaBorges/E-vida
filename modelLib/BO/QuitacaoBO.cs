using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class QuitacaoBO {
		EVidaLog log = new EVidaLog(typeof(QuitacaoBO));

		private static QuitacaoBO instance = new QuitacaoBO();

		public static QuitacaoBO Instance { get { return instance; } }

		public bool PossuiSeqAnterior(TipoArquivoSapEnum tipoArquivo, DateTime dataFolha) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = ArquivoSapDAO.PesquisarArquivos(tipoArquivo, dataFolha.Year, dataFolha.Month, null, db);
				foreach (DataRow dr in dt.Rows) {
					if (Convert.ToString(dr["CD_STATUS"]).Equals(ArquivoSapVO.ST_IMPORTADO) ||
						Convert.ToString(dr["CD_STATUS"]).Equals(ArquivoSapVO.ST_QUITADO))
						return true;
				}
				return false;
			}
		}

		public DataTable Pesquisar(TipoArquivoSapEnum? tipoArquivo, int? ano, int? mes, string status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = ArquivoSapDAO.PesquisarArquivos(tipoArquivo, ano, mes, status, db);
				return dt;
			}
		}

		public void Importar(ArquivoSapVO arquivo, UsuarioVO usuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Importando arquivo. Nome: " + arquivo.Nome + " Data: " + arquivo.DataFolha);

				ArquivoSapDAO.Importar(arquivo, usuario, db);

				connection.Commit();
			}
		}

		public ArquivoSapVO GetById(long id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				ArquivoSapVO vo = ArquivoSapDAO.GetById(id, db);
				return vo;
			}
		}

		public DataTable RelatorioInconsistencia(long id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ArquivoSapDAO.RelatorioInconsistencia(id, db);
			}
		}

		public bool ExisteParcelaPosterior(TipoArquivoSapEnum tipoArquivo, DateTime dataFolha) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ArquivoSapDAO.ExisteParcelaPosterior(tipoArquivo, dataFolha, db);
			}
		}

		public void Quitar(long id, UsuarioVO usuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Quitando arquivo. ID: " + id + " Usuário: " + usuario.Id);

				ArquivoSapDAO.Quitar(id, usuario, db);

				connection.Commit();
			}
		}
		public void Cancelar(long id, UsuarioVO usuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Cancelando arquivo. ID: " + id + " Usuário: " + usuario.Id);

				ArquivoSapDAO.Cancelar(id, usuario, db);

				connection.Commit();
			}
		}

		public List<ArquivoSapVerbaVO> ListarVerbas(TipoArquivoSapEnum tipoArquivo) {
			List<ArquivoSapVerbaVO> lst = CacheHelper.GetFromCache<List<ArquivoSapVerbaVO>>("VERBA_ARQUIVO_SAP");
			if (lst == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					lst = ArquivoSapDAO.ListarVerbas(db);
					CacheHelper.AddOnCache("VERBA_ARQUIVO_SAP", lst, 30);
				}
			}
			if (lst != null) {
				return lst.FindAll(x => x.TipoArq == tipoArquivo);
			}
			return new List<ArquivoSapVerbaVO>();
		}
	}
}
