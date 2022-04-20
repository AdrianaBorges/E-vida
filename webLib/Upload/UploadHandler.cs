using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;

namespace eVida.Web.Upload {
	
	public class UploadHandler : IHttpHandler {
		/// <summary>
		/// You will need to configure this handler in the Web.config file of your 
		/// web and register it with IIS before being able to use it. For more information
		/// see the following link: http://go.microsoft.com/?linkid=8101007
		/// </summary>
		#region IHttpHandler Members

		public bool IsReusable {
			// Return false in case your Managed Handler cannot be reused for another request.
			// Usually this would be false in case you have some state information preserved per request.
			get { return false; }
		}

		public void ProcessRequest(HttpContext context) {
			switch (context.Request.HttpMethod) {
				case "POST": UploadFile(context); return;
				default:
					context.Response.ClearHeaders();
					context.Response.StatusCode = 405;
					break;
			}
		}

		public static string CanonizeName(string fileName) {
			fileName = fileName.Where(x => !@":#?/\%".Contains(x)).Select(x => x.ToString()).Aggregate((x, y) => x + y);
			string retorno = "";
			foreach (char c in fileName) {
				int code = (int)c;
				if (code <= 255) retorno += c;
			}
			return retorno;
		}

		public static string SaveFile(HttpPostedFile file, HttpRequest request) {
			return SaveFile(file, request["prefix"]);
		}

		public static string SaveFile(HttpPostedFile file, string prefix) {
			string newShortName = Path.GetFileName(file.FileName);
			if (!string.IsNullOrEmpty(prefix))
				newShortName = prefix + "_" + newShortName;

			newShortName = CanonizeName(newShortName);

			string filename = Path.Combine(Path.GetTempPath(), newShortName);
			file.SaveAs(filename);
			return newShortName;
		}

		private void UploadFile(HttpContext context) {
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			if (request.Files.Count > 0) {
				/*var file = context.Request.Files[0];
				//string filename = Path.Combine(Server.MapPath("~/Temp"), file.FileName);

				string shortName = Path.GetFileName(file.FileName);
				string originalName = shortName;
				if (!string.IsNullOrEmpty(request["prefix"]))
					shortName = request["prefix"] + "_" + shortName;

				string filename = Path.Combine(Path.GetTempPath(), shortName);
				file.SaveAs(filename);
				*/


				HttpPostedFile file = request.Files[0];

				var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
				response.Clear();
				response.ContentType = "text/plain";

				string fileName = file.FileName;
				fileName = CanonizeName(fileName);
				try {
					string originalName = CanonizeName(Path.GetFileName(fileName));
					string shortName = SaveFile(file, request);

					var result = new
					{
						files = new[] {
							new { name = fileName, size = file.ContentLength, url = shortName, originalName = originalName } 
						}
					};

					response.Write(serializer.Serialize(result));
				}
				catch (Exception ex) {
					EVidaLog log = new EVidaLog(typeof(UploadHandler));
					log.Error("Erro no upload", ex);
					var result = new
					{
						files = new[] {
							new { name = fileName, size = file.ContentLength, error = ex.Message } 
						}
					};
					response.Write(serializer.Serialize(result));
				}
				response.End();

				/*
				 * "name": "picture1.jpg",
    "size": 902604,
    "url": "http:\/\/example.org\/files\/picture1.jpg",
    "thumbnailUrl": "http:\/\/example.org\/files\/thumbnail\/picture1.jpg",
    "deleteUrl": "http:\/\/example.org\/files\/picture1.jpg",
    "deleteType": "DELETE"*/
			}
		}
		
		#endregion
	}
}
