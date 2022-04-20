using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using eVidaGeneralLib.DAO;

namespace eVidaGeneralLib.Util {
	internal class ReportQueryUtil {
		private static EVidaLog log = new EVidaLog(typeof(ReportQueryUtil));

		public string FileName { get; private set; }
		private ReportQuery query;
		private StringBuilder sbFixed = new StringBuilder();
		private StringBuilder sbWhereParams = new StringBuilder();
		private StringBuilder sbEndQuery = new StringBuilder();
		private List<Parametro> lstParams = new List<Parametro>();

		public ReportQueryUtil(string fileName) {
			this.FileName = fileName;

			LoadQuery();
		}

		private void LoadQuery() {
			XmlSerializer ser = new XmlSerializer(typeof(ReportQuery));
			string path = ParametroUtil.ReportQueryFolder;
			if (string.IsNullOrEmpty(path))
				path = Path.GetFullPath(@".\");

			path = Path.Combine(path, FileName);
			log.Debug("Path for report: " + path);
			log.Debug("FullPath for report: " + Path.GetFullPath(path));

			using (StreamReader sr = new StreamReader(path)) {
				object o = ser.Deserialize(sr);
				query = (ReportQuery)o;
			}

			sbFixed.Append("SELECT ").AppendLine(query.Fields);
			sbFixed.Append("FROM ").AppendLine(query.From);
			if (!string.IsNullOrEmpty(query.FixedFilter)) {
				sbFixed.Append("WHERE ").AppendLine(query.FixedFilter);
			} else {
				sbFixed.AppendLine(" WHERE 1 = 1");
			}

			if (!string.IsNullOrEmpty(query.GroupBy))
				sbEndQuery.Append(" GROUP BY ").AppendLine(query.GroupBy);

			if (!string.IsNullOrEmpty(query.OrderBy))
				sbEndQuery.Append(" ORDER BY ").AppendLine(query.OrderBy);
		}

		public List<Parametro> AddParameter<T>(List<T> lst, DbType type, string paramName) {
			ReportFilterQuery filter = query.Filters.Find(x => x.Name.Equals(paramName));
			string inExp = "";
			List<Parametro> lstOut = new List<Parametro>();
			for (int i = 0; i < lst.Count; ++i) {
				string name = ":" + paramName + i;
				if (i > 0) inExp += ", ";
				inExp += name;
				lstOut.Add(new Parametro(name, type, lst[i]));
			}
			inExp = filter.Expression.Replace(":" + paramName, inExp);
			sbWhereParams.Append(" AND ").AppendLine(inExp);

			lstParams.AddRange(lstOut);
			return lstOut;
		}

		public Parametro AddParameter(object value, DbType type, string paramName) {
			ReportFilterQuery filter = query.Filters.Find(x => x.Name.Equals(paramName));
			string name = ":" + paramName;
			sbWhereParams.Append(" AND ").AppendLine(filter.Expression);

			Parametro p = new Parametro(name, type, value);
			lstParams.Add(p);
			return p;
		}

		public Parametro[] AddParameter(object value1, object value2, DbType type, string paramName) {
			ReportFilterQuery filter = query.Filters.Find(x => x.Name.Equals(paramName));
			string name = ":" + paramName;
			sbWhereParams.Append(" AND ").AppendLine(filter.Expression);
			Parametro[] p = new Parametro[] {
				new Parametro(name + "1", type, value1),
				new Parametro(name + "2", type, value2)
			}; 
			lstParams.AddRange(p);
			return p;
		}

		public void Clear() {
			sbWhereParams.Clear();
			lstParams.Clear();
		}

		public string BuildFinalQuery() {
			StringBuilder sb = new StringBuilder();
			sb.Append(sbFixed);
			sb.Append(sbWhereParams);
			sb.Append(sbEndQuery);
			return sb.ToString();
		}

		public List<Parametro> GetParameters() {
			return lstParams;
		}
	}


	[Serializable]
	public class ReportQuery {

		public string Fields { get; set; }
		public string From { get; set; }
		public string OrderBy { get; set; }
		public string GroupBy { get; set; }

		[XmlIgnore]
		public string FixedFilter { get; set; }

		[XmlElement("FixedFilter")]
		public System.Xml.XmlCDataSection FixedFilterCDATA {
			get {
				return new System.Xml.XmlDocument().CreateCDataSection(FixedFilter);
			}
			set {
				FixedFilter = value.Value;
			}
		}

		public List<ReportFilterQuery> Filters { get; set; }

	}

	[Serializable]
	public class ReportFilterQuery {
		public string Name { get; set; }
		[XmlIgnore]
		public string Expression { get; set; }

		[XmlElement("Expression")]
		public System.Xml.XmlNode ExpressionCDATA {
			get {
				return new System.Xml.XmlDocument().CreateCDataSection(Expression);
			}
			set {
				Expression = value.Value;
			}
		}
	}
}
