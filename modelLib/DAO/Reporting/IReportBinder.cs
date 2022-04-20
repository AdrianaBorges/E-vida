using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public interface IReportBinder {
		ReportBinderParams GetData();
		string GerarNome();
		string DefaultRpt();
	}
}
