using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaBilhetagemJob.Bilhetagem {
	class Executor {

		const string FILE_TOKEN = "FILENAME";
		const string ROWS_TOKEN = "ROWCOUNT";
		const string OK_TOKEN = "OK";
		const string ERRO_TOKEN = "ERRO";

		private EVidaLog log = new EVidaLog(typeof(Executor));
		private long idProcesso;
		
		public void Run() {
			log.Info("Run");
			idProcesso = 0;
			int sucesso = 0;
			int erro = 0;
			int rows = 0;
			try {
				string fileName = GetNextFile();
				if (string.IsNullOrEmpty(fileName)) {
					Console.Error.WriteLine("SEM ARQUIVOS PARA PROCESSAR!");
					log.Info("SEM ARQUIVOS PENDENTES");
					log.Info("End");
					return;
				}
				while (!string.IsNullOrEmpty(fileName)) {
					idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.BILHETAGEM);
					log.Debug("Processo: " + idProcesso);
					Executar(fileName, out rows, out sucesso, out erro);
					log.Debug("OK: " + sucesso + " ERRO:" + erro);
					ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.BILHETAGEM, sucesso + erro, BuildParamString(fileName, rows, sucesso, erro));
					fileName = GetNextFile();
				}
			} catch (Exception ex) {
				log.Error("Erro ao processar", ex);
				Console.Error.WriteLine("ERRO! " + ex.Message);
				if (idProcesso != 0)
					ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.BILHETAGEM, ex);
			}
			log.Info("End");
		}

		private string GetNextFile() {
			string dirIn = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_IN);
			if (string.IsNullOrEmpty(dirIn))
				throw new Exception("Diretório IN não configurado");
			string dirBad = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_BAD);
			if (string.IsNullOrEmpty(dirBad))
				throw new Exception("Diretório BAD não configurado");
			string dirOut = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_OUT);
			if (string.IsNullOrEmpty(dirOut))
				throw new Exception("Diretório OUT não configurado");

			IEnumerable<string> files = System.IO.Directory.EnumerateFiles(dirIn, "*.csv");
			if (files.Count() == 0)
				return null;
			files = files.OrderBy(x => x);

			return files.First();
		}

		private void Executar(string fileName, out int rowCount, out int sucesso, out int erro) {
			string filePartNoExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
			string ext = System.IO.Path.GetExtension(fileName);

			string dirOut = ParametroUtil.GetParameter(ParametroUtil.ParametroType.BILHETAGEM_DIR_OUT);
			
			BilhetagemBO.Instance.ProcessarArquivo(fileName, out rowCount, out sucesso, out erro);
			string data = DateTime.Now.ToString("yyyyMMddHHmmss");
			System.IO.File.Move(fileName, System.IO.Path.Combine(dirOut, data + "_" + filePartNoExt + ext));
		}
		private string BuildParamString(string fileName, int rowCount, int sucesso, int erro) {
			StringBuilder sb = new StringBuilder();
			sb.Append(FILE_TOKEN).Append("=[");
			sb.Append(fileName);
			sb.Append("]");
			sb.Append("$$");
			
			sb.Append(ROWS_TOKEN).Append("=[");
			sb.Append(rowCount);
			sb.Append("]");
			sb.Append("$$");
			
			sb.Append(OK_TOKEN).Append("=[");
			sb.Append(sucesso);
			sb.Append("]");
			sb.Append("$$");
			
			sb.Append(ERRO_TOKEN).Append("=[");
			sb.Append(erro);
			sb.Append("]");
			return sb.ToString();
		}

	}
}
