using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;

namespace eVidaMailJob {
	class Program {

		private static EVidaLog log = new EVidaLog(typeof(Program));

		static void Main(string[] args) {
			log.Info("Start");
			//MailAutorizacao();
			//MailProtocolo();
            //MailReciprocidade();
            //MailIndisponibilidadeRede();
			log.Info("End");
		}

		private static void MailAutorizacao() {
			try {
				Autorizacao.MailAutorizacaoSend a = new Autorizacao.MailAutorizacaoSend();
				a.Run();
			} catch (Exception ex) {
				log.Error("Erro ao processar!", ex);
			}
		}

		private static void MailProtocolo() {
			try {
				ProtocoloFatura.MailProtocoloFaturaSend a = new ProtocoloFatura.MailProtocoloFaturaSend();
				a.Run();
			} catch (Exception ex) {
				log.Error("Erro ao processar!", ex);
			}
		}

        private static void MailReciprocidade()
        {
            try
            {
                Reciprocidade.MailReciprocidadeSend a = new Reciprocidade.MailReciprocidadeSend();
                a.Run();
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar!", ex);
            }
        }

        private static void MailIndisponibilidadeRede()
        {
            try
            {
                IndisponibilidadeRede.MailIndisponibilidadeRedeSend a = new IndisponibilidadeRede.MailIndisponibilidadeRedeSend();
                a.Run();
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar!", ex);
            }
        }	
	}
}
