using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class RotinaBO {
		EVidaLog log = new EVidaLog(typeof(RotinaBO));

		private static RotinaBO instance = new RotinaBO();

		public static RotinaBO Instance { get { return instance; } }

		public List<RotinaVO> ListarRotinas() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return RotinaDAO.ListarRotinas(db);
			}
		}

		public RotinaVO GetById(int idRotina) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return RotinaDAO.GetById(idRotina, db);
			}
		}

		public DataTable PesquisarHistorico(int idRotina) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return RotinaDAO.PesquisarHistorico(idRotina, db);
			}
		}

		public bool HasPendente(int idRotina) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return RotinaDAO.HasPendente(idRotina, db);
			}
		}
		public void SolicitarExecucao(int idRotina, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Solicitando execução da rotina: " + idRotina);

				RotinaDAO.SolicitarExecucao(idRotina, idUsuario, db);

				connection.Commit();
			}
		}


		public List<ExecucaoRotinaVO> ListarExecPendente() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return RotinaDAO.ListarExecPendente(db);
			}
		}

		public void Executar(List<ExecucaoRotinaVO> lstPendente, List<ExecucaoRotinaVO> lstOk, List<ExecucaoRotinaVO> lstErro) {
			IEnumerable<int> lstIdRotina = lstPendente.Select(x => x.IdRotina);
			List<RotinaVO> lstRotina = RotinaBO.Instance.ListarRotinas();
			lstRotina.RemoveAll(x => !lstIdRotina.Contains(x.Id));

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				if (log.IsDebugEnabled)
					log.Debug("Execução rotinas");

				foreach (ExecucaoRotinaVO vo in lstPendente) {
					if (!RotinaDAO.IsPendente(vo, db)) {
						continue;
					}

					connection.CreateTransaction();
					RotinaVO rotina = lstRotina.First(x => x.Id == vo.IdRotina);
					RotinaDAO.RegistrarInicio(vo, db);
					connection.Commit();

					Exception erro = null;

					connection.CreateTransaction();
					log.Debug("Iniciando rotina");
					if (rotina.Tipo == TipoRotinaEnum.SQL)
						erro = RotinaDAO.Executar(rotina, db);
					else if (rotina.Tipo == TipoRotinaEnum.WINDOWS) {
						erro = Execute(rotina);
					}
					log.Debug("Rotina executada");

					if (erro == null) {
						lstOk.Add(vo);
						RotinaDAO.RegistrarOk(vo, db);
					} else {
						vo.Erro = erro.ToString();
						vo.ErroSQL = erro.Message;
						lstErro.Add(vo);
						RotinaDAO.RegistrarErro(vo, db);
					}
					connection.Commit();
				}
			}
		}

		private Exception Execute(RotinaVO rotina) {
			try {
				log.Debug("COMANDO: " + rotina.Comando);
				string command = rotina.Comando;
				string dir = null;
				string[] cmds;
				if (command.Contains("$$SPLIT$$")) {
					cmds = command.Split(new string[] { "$$SPLIT$$" }, StringSplitOptions.RemoveEmptyEntries);
					command = cmds[1];
					dir = cmds[0];

					if (!System.IO.Path.IsPathRooted(command)) {
						command = System.IO.Path.Combine(dir, command);
					}
				}
				log.Debug("dir: " + dir + " cmd:" + command);
				ProcessStartInfo process = new ProcessStartInfo(command);
				if (!string.IsNullOrEmpty(dir))
					process.WorkingDirectory = dir;
				//process.Arguments = command;
				process.UseShellExecute = false;
				process.RedirectStandardError = true;
				process.RedirectStandardOutput = true;

				Process cmd = Process.Start(process);

				cmd.WaitForExit(30 * 60 * 1000); // 30 minutos

				string stdoutx = cmd.StandardOutput.ReadToEnd();
				string stderrx = cmd.StandardError.ReadToEnd();
				if (!string.IsNullOrEmpty(stderrx)) {
					return new Exception(stderrx);
				}
				return null;
			} catch (Exception ex) {
				return ex;
			}
		}
	}
}
