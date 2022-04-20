using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using eVidaGeneralLib.VO;

namespace eVidaGeneralLib.Util {
	public static class FormatUtil {
		public static int? TryParseNullableInt(string source) {
			int i;
			if (!Int32.TryParse(source, out i))
				return null;
			return i;
		}

		public static String ToReportString(this String source) {
			if (string.IsNullOrEmpty(source))
				return "-";
			return source;
		}
		public static String Upper(this String source) {
			if (source == null)
				return null;
			return source.ToUpper();
		}
		public static String ToBase64String(this String source) {
			return Convert.ToBase64String(Encoding.Unicode.GetBytes(source));
		}

		public static String FromBase64String(this String source) {
			return Encoding.Unicode.GetString(Convert.FromBase64String(source));
		}

		public static string ListToString<T>(IEnumerable<T> lst) {
			if (lst == null) return null;
			string str = String.Join("|", lst);
			str = "|" + str + "|";
			return str;
		}
		public static List<string> StringToList(string str) {
			List<string> lst = new List<string>();
			if (!string.IsNullOrEmpty(str)) {
				foreach (string area in str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)) {
					lst.Add(area);
				}
			}
			return lst;
		}

		public static string FormatCep(long cep) {
			if (cep == 0)
				return "";
			return String.Format(@"{0:00\.000\-000}", cep);
		}
		public static string UnformatCpf(string cpf) {
			return FilterString(cpf, @"\d+");
		}
		public static string UnformatCep(string cep) {
			return FilterString(cep, @"\d+");
		}
		public static string UnformatCnpj(string cnpj) {
			return FilterString(cnpj, @"\d+");
		}
		public static string FilterString(string original, string pattern) {
			string str = "";
			foreach (Match match in Regex.Matches(original, pattern, RegexOptions.IgnoreCase)) {
				str += match.Value;
			}
			return str;
		}

		public static bool IsValidEmail(string strIn) {
			if (String.IsNullOrEmpty(strIn))
				return false;

			// Use IdnMapping class to convert Unicode domain names. 
			strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
									  RegexOptions.None);

			if (String.IsNullOrEmpty(strIn))
				return false;

			// Return true if strIn is in valid e-mail format. 
			bool isValid = Regex.IsMatch(strIn,
					  @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
					  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
					  RegexOptions.IgnoreCase);

			if (!isValid)
				return false;

			return true;
		}

		private static string DomainMapper(Match match) {
			// IdnMapping class with default property values.
			System.Globalization.IdnMapping idn = new System.Globalization.IdnMapping();

			string domainName = match.Groups[2].Value;
			try {
				domainName = idn.GetAscii(domainName);
			}
			catch (ArgumentException) {
				return null;
			}
			return match.Groups[1].Value + domainName;
		}

		public static string FormatCpf(long? cpf) {
			if (cpf == null)
				return "";
			if (cpf == 0)
				return "";
			return String.Format(@"{0:000\.000\.000\-00}", cpf.Value); ;
		}

		public static string FormatCpf(string cpf) {
			if (cpf == null || cpf.Trim().Length == 0)
				return "";
			return String.Format(@"{0:000\.000\.000\-00}", long.Parse(cpf)); ;
		}

		public static string FormatCnpj(string cnpj) {
			if (cnpj == null || cnpj == "")
				return "";
			return String.Format(@"{0:00\.000\.000\/0000\-00}", cnpj);
		}

		public static string FormatCpfCnpj(string tipoPessoa, string cpfCnpj) {
			if (tipoPessoa.Equals(Constantes.PESSOA_FISICA))
				return FormatCpf(cpfCnpj);
			return FormatCnpj(cpfCnpj);
		}
		/*public static string FormatCnpj(string cnpj) {
			return String.Format(@"{0:00\.000\.000\/0000\-00}", cnpj);
		}*/

		public static string TryFormatCpfCnpj(object cpfCnpj) {
			if (cpfCnpj == DBNull.Value) return "";
			string str = Convert.ToString(cpfCnpj);
			if (str.Length > 11)
				return FormatCnpj(str);
			return FormatCpf(long.Parse(str));
		}
		public static string FormatTel(string tel) {
			if (string.IsNullOrEmpty(tel))
				return "";
			if (tel.Length > 10) {
				return string.Format(@"{0:\(00\) 00000\-0000}", long.Parse(tel));
			}
			return String.Format(@"{0:\(00\) 0000\-0000}", long.Parse(tel)); ;
		}

		public static string FormatDataExtenso(DateTime dateTime) {
			return dateTime.ToString("dd \\de MMMM \\de yyyy");
		}

        public static string FormatDataHoraExtenso(DateTime dateTime)
        {
            return dateTime.ToString("dd \\de MMMM \\de yyyy HH:mm");
        }

		public static string FormatDecimalForm(decimal? value) {
			return value != null ? value.Value.ToString("#.00") : string.Empty;
		}

		public static string FormatDataForm(DateTime? value) {
			return value != null ? value.Value.ToString("dd/MM/yyyy") : "";
		}

		internal static string EnsureCapacity(string str, int capacity) {
			if (string.IsNullOrEmpty(str)) return str;
			if (str.Length > capacity) return str.Substring(0, capacity);
			return str;
		}

		public static string OnlyNumbers(string str) {
			if (string.IsNullOrEmpty(str)) return string.Empty;
			return FilterString(str, @"\d+");
		}

	}
}
