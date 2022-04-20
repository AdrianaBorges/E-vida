using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaDebitoAnualJob {
	class Program {
		private static EVidaLog log = new EVidaLog(typeof(Program));

		static void Main(string[] args) {
			log.Info("Start");
			try {
				DebitoAnual.Executor a = new DebitoAnual.Executor();
				a.Run();
			} catch (Exception ex) {
				log.Error("Erro ao processar!", ex);
			}
			log.Info("End");
		}
	}
}
