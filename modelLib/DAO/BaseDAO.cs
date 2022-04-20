using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace eVidaGeneralLib.DAO
{
    class ParametroVarRow {
		private Dictionary<string, object> values;

		public ParametroVarRow(List<Parametro> lstParams) {
			values = new Dictionary<string, object>();
			foreach (Parametro param in lstParams) {
				if (param is ParametroVar)
					values.Add(param.Name, param.Value);
			}
		}

		public ICollection<string> Params { get { return values.Keys; } }

		public object this[string param] {
			get { return values[NormalizeParam(param)]; }
			set { values[NormalizeParam(param)] = value; }
		}

		private string NormalizeParam(string param) {
			if (param.StartsWith(":")) return param;
			return ":" + param;
		}

		public void Set(string param, object value) {
			this[param] = value;
		}

		internal string ToLog() {
			StringBuilder sb = new StringBuilder("Param");
			foreach (string key in values.Keys) {
				sb.AppendFormat(" {0} = {1} ||", key, values[key]);
			}
			return sb.ToString();
		}
	}

	class ParametroVar : Parametro {
		public ParametroVar() { }
		public ParametroVar(string nome, DbType tipo) : base(nome, tipo, null) {
		}
	}

	class Parametro {
		public Parametro() { }
		public Parametro(string nome, DbType tipo, object value) {
			this.Name = nome;
			this.Tipo = tipo;
			this.Value = value;
		}
		public string Name { get; set; }
		public DbType Tipo { get; set; }
		public object Value { get; set; }
	}

	class BaseDAO {

		static EVidaLog log = new EVidaLog(typeof(BaseDAO));
		
		private static DbCommand BuildQuery(string sql, List<Parametro> parameters, Database db) {
			if (log.IsDebugEnabled) {
				log.Debug("BuildQuery: " + sql);
				if (parameters != null)
					log.Debug("Parametros: " + parameters.Count);
			}
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			if (parameters != null) {
				foreach (Parametro p in parameters) {
					if (log.IsDebugEnabled)
						log.Debug("Parametro " + p.Name + " - " + p.Tipo + " - " + p.Value);
					db.AddInParameter(dbCommand, p.Name, p.Tipo, p.Value);	
				}
			}
			return dbCommand;
		}

		internal static object ExecuteScalar(Database db, string sql, List<Parametro> parameters = null, DbTransaction transaction = null) {
			DateTime start = DateTime.Now;
			try {
				DbCommand dbCommand = BuildQuery(sql, parameters, db);

				object o;
				if (transaction == null)
					o = db.ExecuteScalar(dbCommand);
				else
					o = db.ExecuteScalar(dbCommand, transaction);

				log.Debug("Tempo de exec: " + (DateTime.Now.Subtract(start).TotalMilliseconds));

				log.Debug("Result: " + o);
				return o;
			}
			catch (Exception ex) {
				throw new Exception("Erro ao executar query", ex);
			}
		}

		internal static object ExecuteScalar(EvidaDatabase db, string sql, List<Parametro> parameters = null) {
			return ExecuteScalar(db.Database, sql, parameters, db.CurrentTransaction);
		}

        internal static decimal ExecuteScalar(DbCommand dbCommand, EvidaDatabase evdb)
        {
            throw new NotImplementedException();
        }

		internal static List<T> ExecuteDataSet<T>(int maxRows, Database db, string sql, Func<DataRow, T> func, List<Parametro> parameters = null, DbTransaction transaction = null) {
			DateTime start = DateTime.Now;
			try {
				DataTable dt = ExecuteDataSet(db, sql, parameters, transaction);

				if (dt.Rows.Count == 0) {
					return null;
				}
				start = DateTime.Now;
				List<T> lst = new List<T>();
				int count = 0;
				foreach (DataRow dr in dt.Rows) {
					T vo = func(dr);
					lst.Add(vo);
					count++;
					if (maxRows != -1 && count >= maxRows) break;
				}

				log.Debug("Tempo de transform: " + (DateTime.Now.Subtract(start).TotalMilliseconds));
				return lst;
			} catch (Exception ex) {
				throw new Exception("Erro ao executar query", ex);
			}
		}

		internal static List<T> ExecuteDataSet<T>(Database db, string sql, Func<DataRow, T> func, List<Parametro> parameters = null, DbTransaction transaction = null) {
			return ExecuteDataSet(-1, db, sql, func, parameters, transaction);
		}
		
        internal static List<T> ExecuteDataSet<T>(EvidaDatabase db, string sql, Func<DataRow, T> func, List<Parametro> parameters = null) {
			return ExecuteDataSet(-1, db.Database, sql, func, parameters, db.CurrentTransaction);
		}
		
        internal static T ExecuteDataRow<T>(EvidaDatabase db, string sql, Func<DataRow, T> func, List<Parametro> parameters = null) {
			List<T> lst = ExecuteDataSet(1, db.Database, sql, func, parameters, db.CurrentTransaction);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return default(T);
		}
		
        internal static DataTable ExecuteDataSet(EvidaDatabase db, string sql, List<Parametro> parameters = null) {
			return ExecuteDataSet(db.Database, sql, parameters, db.CurrentTransaction);
		}

		internal static DataTable ExecuteDataSet(Database db, string sql, List<Parametro> parameters = null, DbTransaction transaction = null) {
			DateTime start = DateTime.Now;
			try {				
				DbCommand dbCommand = BuildQuery(sql, parameters, db);

				DataSet dsResult = null;

				if (transaction == null)
					dsResult = db.ExecuteDataSet(dbCommand);
				else
					dsResult = db.ExecuteDataSet(dbCommand, transaction);

				log.Debug("Tempo de exec: " + (DateTime.Now.Subtract(start).TotalMilliseconds));

				DataTable dt =  dsResult.Tables[0];
				log.Debug(dt.Rows.Count + " results");
				return dt;
			}
			catch (Exception ex) {
				throw new Exception("Erro ao executar query", ex);
			}
		}

		internal static Func<DataRow, KeyValuePair<T, string>> Converter<T>(string colId, string colDescription) {
			Func<DataRow, KeyValuePair<T, string>> convert = delegate(DataRow dr)
			{
				T key;
				object o = dr.Field<object>(colId);
				key = (T)Convert.ChangeType(o, typeof(T));
				
				return new KeyValuePair<T, string>(key, Convert.ToString(dr[colDescription]));
			};
			return convert;
		}

		public static T? GetNullableEnum<T>(DataRow dr, string column) where T : struct {
			int? iValue = GetNullableInt(dr, column);
			if (iValue == null)
				return null;
			return (T)Enum.Parse(typeof(T), iValue.ToString(), true);
		}

		public static int? GetNullableInt(DataRow dr, string column) {
			if (dr.IsNull(column))
				return new int?();
			return Convert.ToInt32(dr[column]);
		}

		public static long? GetNullableLong(DataRow dr, string column) {
			if (dr.IsNull(column))
				return new long?();
			return Convert.ToInt64(dr[column]);
		}

		public static DateTime? GetNullableDate(DataRow dr, string column) {
			if (dr.IsNull(column))
				return new DateTime?();
			return Convert.ToDateTime(dr[column]);
		}

		internal static decimal? GetNullableDecimal(DataRow dr, string column) {
			if (dr.IsNull(column))
				return new decimal?();
			return Convert.ToDecimal(dr[column]);
		}

		internal static bool? GetNullableBoolean(DataRow dr, string column) {
			if (dr.IsNull(column))
				return new bool?();
			return Convert.ToBoolean(dr[column]);
		}

		internal static bool? GetNullableBoolean(DataRow dr, string column, string trueValue) {
			if (dr.IsNull(column))
				return new bool?();
			return Convert.ToString(dr[column]).Equals(trueValue);
		}

		public static T ToT<T>(object value) where T : IConvertible {
			if (value == null && typeof(T) == typeof(Nullable))
				return default(T);
			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Boolean:
					return (T)(object)Convert.ToBoolean(value);
				case TypeCode.Byte:
					return (T)(object)Convert.ToByte(value);
				case TypeCode.Char:
					return (T)(object)Convert.ToChar(value);
				case TypeCode.DateTime:
					return (T)(object)Convert.ToDateTime(value);
				case TypeCode.Decimal:
					return (T)(object)Convert.ToDecimal(value);
				case TypeCode.Double:
					return (T)(object)Convert.ToDouble(value);
				case TypeCode.Int16:
					return (T)(object)Convert.ToInt16(value);
				case TypeCode.Int32:
					return (T)(object)Convert.ToInt32(value);
				case TypeCode.Int64:
					return (T)(object)Convert.ToInt64(value);
				case TypeCode.Object:
					return (T)value;
				case TypeCode.SByte:
					return (T)(object)Convert.ToSByte(value);
				case TypeCode.Single:
					return (T)(object)Convert.ToSingle(value);
				case TypeCode.String:
					return (T)(object)Convert.ToString(value);
				case TypeCode.UInt16:
					return (T)(object)Convert.ToUInt16(value);
				case TypeCode.UInt32:
					return (T)(object)Convert.ToUInt32(value);
				case TypeCode.UInt64:
					return (T)(object)Convert.ToUInt64(value);
				case TypeCode.DBNull:
				case TypeCode.Empty:
					return default(T);
				default:
					throw new ApplicationException("Unsupported generics type");
			}
		}
		
		internal static int ExecuteNonQueryMultiRows(string sql, List<Parametro> lstParam, List<ParametroVarRow> values, EvidaDatabase db) {
			if (db.CurrentTransaction == null) {
				throw new InvalidOperationException("Transaction Required");
			} else {
				log.Debug("Transaction: " + db.CurrentTransaction.GetHashCode());
			}
			if (values.Count == 0) return 0;

			DbCommand dbCommand = BuildQuery(sql, lstParam, db.Database);
			int count = 0;
			foreach (ParametroVarRow row in values) {
				foreach (string parameter in row.Params) {
					object value = row[parameter];					
					db.Database.SetParameterValue(dbCommand, parameter, value);
				}
				if (log.IsDebugEnabled) {
					log.Debug(row.ToLog());
				}
				count += db.Database.ExecuteNonQuery(dbCommand, db.CurrentTransaction);
			}
			log.Debug("Qtd changed: " + count);
			return count;
		}

		internal static int ExecuteNonQuery(DbCommand dbCommand, EvidaDatabase db) {
			return ExecuteNonQuery(dbCommand, db.CurrentTransaction, db.Database);
		}

		internal static int ExecuteNonQuery(DbCommand dbCommand, DbTransaction transaction, Database db) {
			if (transaction == null) {
				throw new InvalidOperationException("Transaction Required");
			}
			if (log.IsDebugEnabled) {
				log.Debug(dbCommand.CommandText);
				foreach (DbParameter param in dbCommand.Parameters) {
					log.Debug("Param " + param.ParameterName + " = " + db.GetParameterValue(dbCommand, param.ParameterName));
				}
				log.Debug("Transaction: " + transaction.GetHashCode());
			}
			int count = db.ExecuteNonQuery(dbCommand, transaction);
			log.Debug("Qtd changed: " + count);
			return count;
		}

		internal static int ExecuteNonQuery(string sql, List<Parametro> parameters, EvidaDatabase db) {
			DbCommand cmd = BuildQuery(sql, parameters, db.Database);
			return ExecuteNonQuery(cmd, db);
		}
    }
}
