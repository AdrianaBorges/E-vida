using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaBilhetagemJob {
	class Program {
		private static EVidaLog log = new EVidaLog(typeof(Program));

		static void Main(string[] args) {
			log.Info("Start");
			try {
				Bilhetagem.Executor a = new Bilhetagem.Executor();
				a.Run();
			} catch (Exception ex) {
				log.Error("Erro ao processar!", ex);
			}
			log.Info("End");
		}
	}
}
