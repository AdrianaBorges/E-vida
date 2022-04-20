using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace eVidaGeneralLib.Util {
	public class EVidaLog {
		private ILog logger;

		private static bool initialized = false;
		private static Object locker = new Object();

		public EVidaLog(Type t) {
			if (!initialized) {
				lock (locker) {
					if (!initialized) {
						Initialize();
						initialized = true;
					}
				}
			}
			logger = LogManager.GetLogger(t);
		}

		public static void Initialize() {
			if (System.IO.File.Exists("log4net.config")) {
				log4net.Config.XmlConfigurator.Configure();
			} else {
				log4net.Config.XmlConfigurator.Configure();
			}
			
		}

		public static Object Context {
			set { log4net.GlobalContext.Properties["USER"] = value; log4net.GlobalContext.Properties["NDC"] = value; }
		}

		public bool IsDebugEnabled { get { return logger.IsDebugEnabled; } }

		public void Debug(string msg) {
			if (logger.IsDebugEnabled) {
				logger.Debug(msg);
			}
		}

		public void Info(string msg) {
			logger.Info(msg);
		}
		public void Error(string msg) {
			logger.Error(msg);
		}

		public void Error(string msg, Exception ex) {
			logger.Error(msg, ex);
		}
	}

}
