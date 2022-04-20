using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class CanalGestanteBO {
		EVidaLog log = new EVidaLog(typeof(CanalGestanteBO));

		private static CanalGestanteBO instance = new CanalGestanteBO();

		public static CanalGestanteBO Instance { get { return instance; } }

		#region Config

		public int CalcularAno(DateTime dtSolicitacao) {
			dtSolicitacao = dtSolicitacao.Date;
			int ano = dtSolicitacao.Year;
			DateTime dtLimite = new DateTime(ano, 03, 31);
			if (dtSolicitacao > dtLimite) {
				ano--;
			} else {
				ano -= 2;
			}
			return ano;
		}

		public CanalGestanteConfigVO GetConfig(DateTime dtSolicitacao) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					int ano = CalcularAno(dtSolicitacao);
					return CanalGestanteDAO.GetConfig(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		public CanalGestanteConfigVO GetConfig(int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.GetConfig(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<CanalGestanteConfigCredVO> GetConfigCred(int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.GetConfigCred(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<CanalGestanteConfigProfVO> GetConfigProf(int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.GetConfigProf(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		public void Salvar(CanalGestanteConfigVO vo, IEnumerable<CanalGestanteConfigCredVO> lstCred, IEnumerable<CanalGestanteConfigProfVO> lstProf) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando config canal gestante " + vo.Ano + " - " + vo.CodUsuarioAlteracao);

					CanalGestanteConfigVO oldVO = CanalGestanteDAO.GetConfig(vo.Ano, db);
					bool novo = oldVO == null;

					CanalGestanteDAO.SalvarConfig(vo, novo, transaction, db);

					CanalGestanteDAO.SalvarConfig(vo.Ano, lstCred, novo, transaction, db);
					CanalGestanteDAO.SalvarConfig(vo.Ano, lstProf, novo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public List<VO.Protheus.PRedeAtendimentoVO> ListarInfoCredenciadoConfig(int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.ListarInfoCredenciadoConfig(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<VO.Protheus.PProfissionalSaudeVO> ListarInfoProfissionalConfig(int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.ListarInfoProfissionalConfig(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		public List<string> ListarUfConfig(int ano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.ListarUfConfig(ano, db);
				} finally {
					connection.Close();
				}
			}
		}

		#endregion

		#region Beneficiario

		public CanalGestanteBenefVO GetInfoBenef(string codint, string codemp, string matric, string tipreg) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
                return CanalGestanteDAO.GetInfoBenef(codint, codemp, matric, tipreg, db);
			}
		}

		public void Salvar(CanalGestanteBenefVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
                    log.Debug("Salvando benef canal gestante - Codint: " + vo.Codint + " - Codemp: " + vo.Codemp + " - Matric: " + vo.Matric + " - Tipreg: " + vo.Tipreg + " - Email: " + vo.Email + " - Telefone: " + vo.Telefone);

				CanalGestanteBenefVO oldVO = CanalGestanteDAO.GetInfoBenef(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);
				bool novo = oldVO == null;

				CanalGestanteDAO.SalvarBenef(vo, novo, db);

				connection.Commit();
			}
		}

		public void MarcarDownload(string codint, string codemp, string matric, string tipreg, string tipo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
                    log.Debug("MarcarDownload canal gestante - Tipo: " + tipo + " - Codint: " + codint + " - Codemp: " + codemp + " - Matric: " + matric + " - Tipreg: " + tipreg);

				CanalGestanteBenefVO oldVO = CanalGestanteDAO.GetInfoBenef(codint, codemp, matric, tipreg, db);
				bool novo = oldVO == null;
				if (novo) {
					throw new EvidaException("Dados do beneficiário incompletos!");
				}
                CanalGestanteDAO.MarcarDownload(codint, codemp, matric, tipreg, tipo, db);

				connection.Commit();
			}
		}

		#endregion

		#region Protocolo

		public DataTable Pesquisar(int? idProtocolo, string cartao, StatusCanalGestante? status, int? controle) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.Pesquisar(idProtocolo, cartao, status, controle, db);
				} finally {
					connection.Close();
				}
			}
		}

        public List<CanalGestanteVO> ListarProtocolosBenef(string codint, string codemp, string matric, string tipreg)
        {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					return CanalGestanteDAO.ListarProtocolosBenef(codint, codemp, matric, tipreg, db);
				} finally {
					connection.Close();
				}
			}
		}

		public CanalGestanteVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return CanalGestanteDAO.GetById(id, db);
			}
		}

		public CanalGestanteVO GerarProtocolo(string codint, string codemp, string matric, string tipreg, string cdCredenciado, string nrSeqProfissional) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
                    log.Debug("Gerando protocolo canal gestante - Codint: " + codint + " - Codemp: " + codemp + " - Tipreg: " + tipreg + " - cdCredenciado: " + cdCredenciado + " - nrSeqProfissional: " + nrSeqProfissional);

				int id = CanalGestanteDAO.GerarProtocolo(codint, codemp, matric, tipreg, cdCredenciado, nrSeqProfissional, db);

				CanalGestanteVO vo = CanalGestanteDAO.GetById(id, db);

				connection.Commit();
				return vo;
			}
		}

		public string GetFilePath(CanalGestanteVO vo, out string nome) {
			string dir = FileUtil.GetRepositoryDir(FileUtil.FileDir.PROTOCOLO_CANAL_GESTANTE);
			dir = System.IO.Path.GetFullPath(dir);
			string dir2 = System.IO.Path.Combine(dir, vo.DataSolicitacao.Year.ToString("0000"));
			nome = vo.Id.ToString(CanalGestanteVO.FORMATO_PROTOCOLO_FILE) + ".pdf";
			string path = System.IO.Path.Combine(dir2, nome);
			return path;
		}

		public void SalvarProtocolo(CanalGestanteVO vo, byte[] relatorio) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Salvando arquivo protocolo canal gestante " + vo.Id);

					string nome;
					string path = GetFilePath(vo, out nome);
					System.IO.FileInfo fi = new System.IO.FileInfo(path);
					fi.Directory.Create();
					System.IO.File.WriteAllBytes(path, relatorio);

					CanalGestanteDAO.AtualizarGerado(vo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void SolicitarEsclarecimento(int id, string tipoContato, string mensagem) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("SolicitarEsclarecimento canal gestante " + id);

				CanalGestanteDAO.SolicitarEsclarecimento(id, tipoContato, mensagem, db);

				CanalGestanteVO vo = CanalGestanteDAO.GetById(id, db);
				CanalGestanteBenefVO canalBenefVO = CanalGestanteDAO.GetInfoBenef(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);
                VO.Protheus.PUsuarioVO benefVO = DAO.Protheus.PUsuarioDAO.GetRowById(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);

				EmailUtil.CanalGestante.SendCanalGestanteSolEsclarecimentoBenef(vo, canalBenefVO, benefVO);
				connection.Commit();
			}
		}

		public void EnviarEmail(CanalGestanteVO vo, string path) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();

                CanalGestanteBenefVO canalBenefVO = CanalGestanteDAO.GetInfoBenef(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);
                VO.Protheus.PUsuarioVO benefVO = DAO.Protheus.PUsuarioDAO.GetRowById(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg, db);

				EmailUtil.CanalGestante.SendCanalGestanteBeneficiario(vo, canalBenefVO, benefVO, path);
			}
		}

		public void Finalizar(CanalGestanteVO vo) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					if (log.IsDebugEnabled)
						log.Debug("Finalizar canal gestante " + vo.Id);

					CanalGestanteDAO.Finalizar(vo, transaction, db);

					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

        #endregion
	}
}

