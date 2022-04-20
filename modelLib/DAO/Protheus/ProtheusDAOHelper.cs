using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
	internal class ProtheusDAOHelper {

		public static long? GetLong(string value) {
			if (value != null) value = value.Trim();
			if (string.IsNullOrEmpty(value)) return new long?();
			return Int64.Parse(value);
		}
		public static long? GetLong(DataRow dr, string col) {
			string value = GetTrimString(dr, col);
			return GetLong(value);
		}


		public static DateTime? GetDate(string value) {
			if (value != null) value = value.Trim();
			if (string.IsNullOrEmpty(value)) return new DateTime?();
			return DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture);
		}

		public static DateTime? GetDateNative(DataRow dr, string col) {
			if (dr.IsNull(col)) return null;
			return Convert.ToDateTime(dr[col]);
		}

		public static DateTime? GetDatePr(DataRow dr, string col) {
			string value = GetTrimString(dr, col);
			return GetDate(value);
		}

		public static int? GetInt(DataRow dr, string col) {
			string value = GetTrimString(dr, col);
			if (string.IsNullOrEmpty(value)) return null;
			return Int32.Parse(value);
		}


		public static string GetTrimString(DataRow dr, string col) {
			if (dr.IsNull(col)) return null;
			string value = Convert.ToString(dr[col]).Trim();
			return value;
		}
	}
}
