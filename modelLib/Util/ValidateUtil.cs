using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace eVidaGeneralLib.Util {
	public class ValidateUtil {
		private static string[] INVALID_CPF = {"11111111111", "22222222222", "33333333333", "44444444444", 
                                              "55555555555", "66666666666", "77777777777", "88888888888", "99999999999"};
		static EVidaLog log = new EVidaLog(typeof(ValidateUtil));
		public static bool IsValidName(string name) {
			if (string.IsNullOrEmpty(name)) return true;
			name = name.TrimEnd();
			//. Nome com apenas uma palavra;
			//. Nomes que contenham um ou mais números: 1, 2, 3, 4, 5, 6, 7, 8, 9 e 0; 
			//. Nomes que contenham um ou mais caracteres especiais

			string regex = @"^(\p{L}+\s\p{L}+)+(\p{L}+\s{0,1})*$";
			bool match = Regex.IsMatch(name, regex, RegexOptions.IgnoreCase);
			if (!match) return false;

			// Primeiro nome com apenas uma letra, exceto quando o  primeiro nome for: D, I, O, U, Y (com ou sem acento)
			string primeiro = name.Substring(0, name.IndexOf(' '));
			if (primeiro.Length == 1 && ("DIOUYÍÓÚÝ".IndexOf(primeiro) < 0))
				return false;

			// Último nome com apenas uma letra, exceto quando o último nome for: I, O, U, Y (com ou sem acento);
			string ultimo = name.Substring(name.LastIndexOf(' ') + 1);
			if (ultimo.Length == 1 && ("IOUYÍÓÚÝ".IndexOf(ultimo) < 0))
				return false;

			return true;
		}

		public static bool IsValidCPF(string cpf) {
			cpf = cpf.Trim();
			if (cpf.Length != 11 || Array.BinarySearch(INVALID_CPF, cpf) >= 0)
				return false;
			
			for (int x = 0; x <= 1; x++) {
				int n1 = 0;
				for (int i = 0; i < 9 + x; i++) {
					n1 += Int32.Parse(cpf.Substring(i, 1)) * (10 + x - i);
				}
				int n2 = 11 - (n1 - ((int)(n1 / 11) * 11));
				if (n2 == 10 || n2 == 11) n2 = 0;

				if (n2 != Int32.Parse(cpf.Substring(9 + x, 1)))
					return false;
			}

			return true;
		}

		public static bool IsValidDomain(string email) {
			string domain = email.Substring(email.IndexOf("@") + 1);
			string byPass = ParametroUtil.EmailByPass;
			string[] dominios = null;

			if (!string.IsNullOrEmpty(byPass)) {
				dominios = byPass.ToLower().Split(';');
			}

			IPHostEntry host = null;
			try {
				if (dominios != null && Array.IndexOf(dominios, domain.ToLower()) >= 0) {
					log.Debug("Email com validacao bypass: " + domain);
					return true;
				}
				
				host = Dns.GetHostEntry(domain);
			}
			catch (SocketException ex) {
				log.Error("Erro ao validar do dominio " + domain, ex);
				return true;
			}
			return host != null && host.AddressList.Length > 0;
		}

		public static bool IsValidLength(string txt, int maxLength) {
			if (string.IsNullOrEmpty(txt)) return true;
			if (txt.Length <= maxLength) return true;
			return false;
		}

		/// <summary>
		/// Se A <> NULL, RETORNA A, CASO CONTRARIO RETORNA B
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		internal static string Nvl(string a, string b) {
			if (!string.IsNullOrEmpty(a) && !a.Equals("\0")) return a;
			return b;
		}

		/// <summary>
		/// Se A <> NULL, RETORNA A, CASO CONTRARIO RETORNA B
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		internal static int? Nvl(int? a, int? b) {
			if (a != null) return a;
			return b;
		}

		internal static DateTime? Nvl(DateTime? a, DateTime? b) {
			if (a != null) return a;
			return b;
		}

		internal static string[] TrySplitTelefone(string telefoneFormatado) {
			if (telefoneFormatado.Contains(")") && telefoneFormatado.StartsWith("(")) {
				string ddd;
				string telefone;

				string[] tel = telefoneFormatado.Split(new char[] {'(',')'}, StringSplitOptions.RemoveEmptyEntries);
				ddd = tel[0];
				telefone = ClearTelefone(tel[1]);
				if (tel.Length > 2)
					throw new ArgumentException("Telefone " + telefoneFormatado + " não pode ser avaliado.");
				return new string[] {ddd, telefone};
			}
			return new string[] { "", telefoneFormatado };
		}

		internal static string ClearTelefone(string tel) {
			if (string.IsNullOrEmpty(tel)) return "";
			string str = "";
			foreach (char c in tel) {
				if (Char.IsDigit(c)) str += c;
			}
			return str;
		}

        public static int DigitoModulo11(long intNumero)
        {
            int[] intPesos = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5, 6, 7, 8, 9 };
            string strText = intNumero.ToString();

            if (strText.Length > 16)
                throw new Exception("Número não suportado pela função!");

            int intSoma = 0;
            int intIdx = 0;

            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
            }

            int intResto = (intSoma * 10) % 11;
            int intDigito = intResto;
            if (intDigito >= 10)
                intDigito = 0;

            return intDigito;
        }	

	}
}
