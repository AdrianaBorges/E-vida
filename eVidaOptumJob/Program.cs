using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;

namespace eVidaOptumJob
{
    class Program
    {
        private static EVidaLog log = new EVidaLog(typeof(Program));

        static void Main(string[] args)
        {
            log.Info("Start");
            Optum();
            log.Info("End");
        }

        private static void Optum()
        {
            try
            {
                Optum.Optum a = new Optum.Optum();
                a.Run();
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar!", ex);
            }
        }

    }
}
