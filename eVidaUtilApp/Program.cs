using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaUtilApp {
	class Program {

		private static EVidaLog log = new EVidaLog(typeof(Program));

		static void Main(string[] args) {
			log.Info("Start");
			try {
				if (args.Length == 0) {
					Console.WriteLine("Parâmetros inválidos");
					log.Info("SEM PARAMETROS");
					log.Info("End");
					return;
				}
				string[] newArgs = new string[args.Length - 1];
				if (args.Length > 1)
					Array.Copy(args, 1, newArgs, 0, newArgs.Length);

				if (args[0].Equals("IRB")) {
					IR.GeradorIrBeneficiario a = new IR.GeradorIrBeneficiario();
					a.Run(newArgs);
				} else if (args[0].Equals("TESTE")) {
					PdfTeste.Teste a = new PdfTeste.Teste();
					a.Run(newArgs);
				}
			} catch (Exception ex) {
				log.Error("Erro ao processar!", ex);
			}
			log.Info("End");
		}
	}
}
