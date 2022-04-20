using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util {
	internal static class RowConverterUtil {
		public static DateTime? ConvertToDateTime(this DataRow dr, string colName) {
			if (dr[colName] == DBNull.Value)
				return null;
			return Convert.ToDateTime(dr[colName]);
		}

		public static int? ConvertToInt(this DataRow dr, string colName) {
			if (dr[colName] == DBNull.Value)
				return null;
			return Convert.ToInt32(dr[colName]);
		}

		public static long? ConvertToLong(this DataRow dr, string colName) {
			if (dr[colName] == DBNull.Value)
				return null;
			return Convert.ToInt64(dr[colName]);
		}
	}
}
