using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util {
	public static class CacheHelper {
		private static MemoryCache cache = MemoryCache.Default;
		/*
		private abstract class CacheItem {
			public DateTime CacheTime { get; set; }
			public abstract object Value { get; }
		}

		private class CacheItem<T> : CacheItem {
			public T Item { get; set; }
			public override object Value {
				get {
					return Item;
				}
			}
		}
		
		private static Dictionary<string, CacheItem> cache = new Dictionary<string, CacheItem>();

		private static bool IsExpired(this CacheItem item) {
			return item.CacheTime < DateTime.Now;
		}

		private static DateTime CalculateCacheTime(int minutes) {
			if (minutes == -1) {
				return DateTime.MaxValue;
			} else {
				return DateTime.Now.AddMinutes(minutes);
			}
		}
		*/
		public static T GetFromCache<T>(string key) {
			key = key.ToUpper();

			lock (key) {
				object o = cache.Get(key);
				if (o == null)
					return default(T);
				return (T)o;
			}
		}

		public static void AddOnCache<T>(string key, T o, int minutes = 10) {
			if (o == null) return;
			key = key.ToUpper();
			lock (key) {
				CacheItemPolicy policy = new CacheItemPolicy();
				policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(minutes);

				CacheItem item = cache.AddOrGetExisting(new CacheItem(key, o), policy);
				if (item != null)
					item.Value = o;
			}
		}

		public static void RemoveFromCache(string key) {
			key = key.ToUpper();
			lock (key) {
				cache.Remove(key);
			}
		}

		public static Dictionary<string, object> GetAllCache() {
			Dictionary<string, object> values = new Dictionary<string, object>();
			
			foreach (KeyValuePair<string,object> item in cache) {
				values.Add(item.Key, item.Value);
			}
			
			return values;
		}

		internal static void Clear() {
			cache.Trim(100);
		}
	}
}
