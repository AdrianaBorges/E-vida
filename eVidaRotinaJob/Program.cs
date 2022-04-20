using eVidaGeneralLib.Util;
using eVidaGeneralLib.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaRotinaJob {
	class Program {
		private static EVidaLog log = new EVidaLog(typeof(Program));
        static void Main(string[] args)
        {
            log.Info("Start");
            try
            {
                IndisponibilidadeRedeBO.Instance.AtualizarPrazos();

                //Rotina.ExecutorRotina a = new Rotina.ExecutorRotina();
                //a.Run();
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar!", ex);
            }
            log.Info("End");
        }
	}
}
