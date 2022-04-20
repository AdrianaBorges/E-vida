using eVida.Console.Report;
using eVidaGeneralLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaUtilApp.PdfTeste {
	class Teste {

		public void Run(string[] args) {
			ReportTeste teste = new ReportTeste(ParametroUtil.ReportRdlcFolder);
			ReportTeste.ParamsVO parms = new ReportTeste.ParamsVO();
			parms.CdFuncionario = 123123;
			byte[] bytes = teste.GerarRelatorio(parms);
			string filePath = System.IO.Path.Combine(@"C:\temp\", "teste.pdf");
			WritePdf(bytes, filePath);
		}
		private void WritePdf(byte[] bytes, string path) {
			System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
			bw.Write(bytes);
			bw.Close();
		}
	}
}
