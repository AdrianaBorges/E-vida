using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace eVida.Console.Util {
	public class JSUtil {

		public static string Serialize(object o) {
			JavaScriptSerializer jsSerializer = BuildSerializer();
			return jsSerializer.Serialize(o);
		}

		public static T Deserialize<T>(string str) {
			JavaScriptSerializer jsSerializer = BuildSerializer();
			return jsSerializer.Deserialize<T>(str);
		}

		public static JavaScriptSerializer BuildSerializer() {
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new[] { new DateTimeConverter() });
			return serializer;
		}
	}

	internal class DateTimeConverter : JavaScriptConverter {
		private string STRING_FORMAT = "yyyy-MM-dd HH:mm:ss";
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer) {
			if (dictionary.ContainsKey("value")) {
				return DateTime.ParseExact(dictionary["value"].ToString(), STRING_FORMAT, System.Globalization.CultureInfo.CurrentCulture);
			}
			return new JavaScriptSerializer().ConvertToType(dictionary, type);
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer) {
			if (!(obj is DateTime)) return null;
			DateTime dt = (DateTime)obj;
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("value", dt.ToString(STRING_FORMAT));
			return dic;
		}

		public override IEnumerable<Type> SupportedTypes {
			get { return new[] { typeof(DateTime) }; }
		}
	}

}
