using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportBinderParams {
		public ReportBinderParams() {
			DataSources = new Dictionary<string, DataTable>();
			Params = new Dictionary<string, string>();
		}
		public bool UseExternalImages { get; set; }
		public Dictionary<string, DataTable> DataSources { get; set; }
		public Dictionary<string, string> Params { get; set; }
	}
}
