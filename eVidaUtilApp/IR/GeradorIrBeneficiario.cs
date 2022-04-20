using eVida.Console.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaUtilApp.IR {
	internal class GeradorIrBeneficiario {

		private EVidaLog log = new EVidaLog(typeof(GeradorIrBeneficiario));
		public void Run(string[] args) {
			if (args == null || args.Length == 0) {
				WriteErrorLog("Parâmetros IR inválidos - NO PARAMETERS");
				return;
			}
			if (args.Length < 2) {
				WriteErrorLog("Parâmetros IR inválidos - LESS THAN 2");
				return;
			}

			log.Info("Run");
			try {
				bool mensalidade = false;
				bool reembolso = false;
				if (args[0].Equals("MENS")) {
					mensalidade = true;
				} else if (args[0].Equals("REEM")) {
					reembolso = true;
				} else if (args[0].Equals("*")) {
					mensalidade = reembolso = true;
				}
				string dir = args[1];
				bool dirOk = false;
				try {
					dirOk = System.IO.Directory.Exists(dir);
				} catch (Exception ex) {
					WriteErrorLog("Diretório inválido: " + ex.Message);
					return;
				}
				if (!dirOk) {
					WriteErrorLog("Diretório destino inexistente: " + dir);
					return;
				}
				GerarArquivo(dir, mensalidade, reembolso);
			} catch (Exception ex) {
				WriteErrorLog("Erro ao processar");
				log.Error("Erro ao processar", ex);				
			}
			log.Info("End");
		}

		private void GerarArquivo(string dir, bool mensalidade, bool reembolso) {
			int ano = DateTime.Now.Year - 1;
			if (mensalidade)
				GerarArquivoMensalidade(dir, ano);
			if (reembolso)
				GerarArquivoReembolso(dir, ano);
		}

		private void GerarArquivoMensalidade(string dir, int ano) {
			ReportIrMensalidade rpt = new ReportIrMensalidade(ParametroUtil.ReportRdlcFolder);

			DataTable dt = ExtratoIrBeneficiarioBO.Instance.FuncionariosMensalidade(ano);
			if (dt == null) return;

			int count = 0;

			DateTime start = DateTime.Now;

			WriteInfoLog("TOTAL MENSALIDADE: " + dt.Rows.Count);
			foreach (DataRow dr in dt.Rows) {
				if (count % 100 == 0) {
					WriteInfoLog("MENSALIDADE: " + count);
				}
				count++;
				ReportIrMensalidade.ParamsVO vo = new ReportIrMensalidade.ParamsVO();
				vo.CdEmpresa = Convert.ToInt32(dr["cd_empresa"]);
				vo.CdFuncionario = Convert.ToInt32(dr["cd_funcionario"]);
				vo.AnoRef = ano;

				byte[] bytes = rpt.GerarRelatorio(vo);
				string filePath = System.IO.Path.Combine(dir, "MENSALIDADE_" + vo.CdFuncionario + ".pdf");
				WritePdf(bytes, filePath);
			}
			WriteInfoLog("MENSALIDADE OK - " + count);
			WriteInfoLog("TEMPO MENSALIDADE - " + (DateTime.Now.Subtract(start).TotalMilliseconds));

		}
		private void GerarArquivoReembolso(string dir, int ano) {
			ReportIrReembolso rpt = new ReportIrReembolso(ParametroUtil.ReportRdlcFolder);

			DataTable dt = ExtratoIrBeneficiarioBO.Instance.FuncionariosReembolso(ano);
			if (dt == null) return;

			int count = 0;
			DateTime start = DateTime.Now;
			WriteInfoLog("TOTAL REEMBOLSO: " + dt.Rows.Count);
			foreach (DataRow dr in dt.Rows) {
				if (count % 100 == 0) {
					WriteDebugLog("REEMBOLSO: " + count);
				}
				count++;
				ReportIrReembolso.ParamsVO vo = new ReportIrReembolso.ParamsVO();
				vo.CdEmpresa = Convert.ToInt32(dr["cd_empresa"]);
				vo.CdFuncionario = Convert.ToInt32(dr["cd_funcionario"]);
				vo.AnoRef = ano;

				byte[] bytes = rpt.GerarRelatorio(vo);
				string filePath = System.IO.Path.Combine(dir, "REEMBOLSO_" + vo.CdFuncionario + ".pdf");
				WritePdf(bytes, filePath);

			}
			WriteInfoLog("REEMBOLSO OK - " + count);
			WriteInfoLog("TEMPO REEMBOLSO - " + (DateTime.Now.Subtract(start).TotalMilliseconds));
		}

		private void WritePdf(byte[] bytes, string path) {
			System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
			bw.Write(bytes);
			bw.Close();
		}

		private void WriteDebugLog(string msg, bool console = false) {
			log.Debug(msg);
			if (console)
				Console.WriteLine(DateTime.Now + " " + msg);
		}
		private void WriteErrorLog(string msg) {
			log.Error(msg);
			Console.WriteLine(DateTime.Now + " " + msg);
		}
		private void WriteInfoLog(string msg) {
			log.Info(msg);
			Console.WriteLine(DateTime.Now + " " + msg);
		}
	}
}
