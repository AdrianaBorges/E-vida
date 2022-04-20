using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.Util.Email {

	internal static class EmailTypeWrapper {

		public static string GenerateEmailBody(this EmailProvider.EmailType tipo, Dictionary<string, string> parms = null) {
			return EmailProvider.GenerateEmailBody(tipo, parms);
		}
		public static string GenerateEmailTitle(this EmailProvider.EmailType tipo) {
			return EmailProvider.GenerateEmailTitle(tipo);
		}
		public static string GetSender(this EmailProvider.EmailType tipo) {
			return EmailProvider.GetSender(tipo);
		}
	}
	internal class Configurator {

		private static List<EmailConfigVO> LoadConfig() {
			List<EmailConfigVO> lstCache = CacheHelper.GetFromCache<List<EmailConfigVO>>("EMAIL_CONFIG");
			if (lstCache == null) {
				lock (typeof(EmailConfigVO)) {
					lstCache = CacheHelper.GetFromCache<List<EmailConfigVO>>("EMAIL_CONFIG");
					if (lstCache == null) {
						DataTable dt = null;
						using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
							EvidaConnectionHolder connection = db.CreateConnection();
							dt = EmailConfigDAO.GetConfigs(db);
						}
						lstCache = new List<EmailConfigVO>();
						lstCache.AddRange(from row in dt.AsEnumerable()
										  select new EmailConfigVO() {
											  Id = Convert.ToInt32(row.Field<decimal>("id_config")),
											  Nome = row.Field<string>("nm_config"),
											  Email = row.Field<string>("ds_email_envio"),
											  Assunto = row.Field<string>("ds_assunto"),
											  Arquivo = row.Field<string>("ds_arquivo")
										  });
						CacheHelper.AddOnCache("EMAIL_CONFIG", lstCache);
					}
				}
			}
			return lstCache;
		}

		internal static EmailConfigVO GetConfig(int tipo) {
			List<EmailConfigVO> lstCache = LoadConfig();
			EmailConfigVO vo = lstCache.Find(x => x.Id == tipo);
			return vo;
		}

		internal static List<EmailConfigVO> ListAllConfig() {
			return LoadConfig();
		}
	}
}
