using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Exceptions {
	public class ParseFileException : EvidaException {
		public ParseFileException(int row, int col) : base(BuildMessage(row, col)) {
			Row = row;
			Col = col;
		}
		public ParseFileException(int row, int col, string message)
			: base(BuildMessage(row, col) + " - " + message) {
			Row = row;
			Col = col;
		}
		public ParseFileException(int row, int col, string message, Exception inner)
			: base(BuildMessage(row, col) + " - " + message, inner) {
			Row = row;
			Col = col;
		}

		public int Row { get; private set; }
		public int Col { get; private set; }

		private static string BuildMessage(int row, int col) {
			return "Erro ao ler arquivo linha: " + row + " coluna: " + col;
		}
	}
}
