using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVidaGeneralLib.VO.HC;
using SkyReport.ReportingServices;

namespace eVida.Console.Report {
	
	public abstract class ReportBase<T> : SkyReport.ReportingServices.ReportBase<T> {

		protected eVidaGeneralLib.Util.EVidaLog Log { get; private set; }

		public ReportBase(string reportDir) : base(reportDir) {
			Log = new eVidaGeneralLib.Util.EVidaLog(typeof(T));
		}
		
	}
}