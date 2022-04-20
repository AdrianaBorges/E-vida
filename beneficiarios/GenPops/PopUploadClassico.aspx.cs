using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Upload;
using eVidaGeneralLib.Util;
using eVidaBeneficiarios.Classes;

namespace eVidaBeneficiarios.GenPops {
	public partial class PopUploadClassico : PopUpPageBase {

		protected void Page_PreRender(object sender, EventArgs e) {
			ScriptManager manager = (ScriptManager)Master.FindControl("ScriptManager1");
			manager.RegisterPostBackControl(btnEnviar);
		}

		protected override void PageLoad(object sender, EventArgs e) {			
			//if (!IsPostBack) {
				string tipo = Request["TIPO"];
				if (string.IsNullOrEmpty(tipo)) {
					this.ShowError("Tipo de upload não definido!");
					hidRnd.Visible = false;
					return;
				}
				LoadConfig();
                hidRnd.Value = UploadConfigManager.GetPrefix(UploadConfig, Sistema.BENEFICIARIO, this.UsuarioLogado.UsuarioTitular.Codint.Trim() + this.UsuarioLogado.UsuarioTitular.Codemp.Trim() + this.UsuarioLogado.UsuarioTitular.Matric.Trim() + this.UsuarioLogado.UsuarioTitular.Tipreg.Trim());
			//}
		}

		public UploadConfig UploadConfig { get; set; }

		private UploadConfig LoadConfig() {
			if (UploadConfig == null) {
				string tipo = Request["TIPO"];
				if (string.IsNullOrEmpty(tipo)) return null;
				UploadConfig = UploadConfigManager.GetConfig(tipo);
			}
			return UploadConfig;
		}

		protected string GetUploadFilter() {
			if (UploadConfig != null)
				return UploadConfig.FileTypes.Aggregate((x, y) => x + "," + y);
			return null;
		}
		protected int GetUploadSize() {
			if (UploadConfig != null)
				return UploadConfig.MaxSize;
			return 0;
		}

		protected void btnEnviar_Click(object sender, EventArgs e) {
			if (upFile.HasFile) {
				HttpPostedFile file = upFile.PostedFile;
				if (!IsValidFile(file)) {
					return;
				}
				string originalName = UploadHandler.CanonizeName(Path.GetFileName(file.FileName));
				string shortName = UploadHandler.SaveFile(upFile.PostedFile, this.hidRnd.Value);
				this.RegisterScript("fileOk", "afterUpload('" + shortName + "','" + originalName + "')");
			} else {
				this.ShowError("Selecione o arquivo a enviar!");
			}
		}

		private bool IsValidFile(HttpPostedFile file) {
			LoadConfig();

			string[] types = UploadConfig.FileTypes;
			if (types != null && types.Length > 0) {
				bool invalid = types.FirstOrDefault(x => file.FileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)) == null;
				if (invalid) {
					this.ShowError("Tipo de arquivo não suportado para envio!");
					return false;
				}
			}
			int maxLength = UploadConfig.MaxSize;
			if (maxLength > 0) {
				int fileLength = file.ContentLength / 1024;
				if (fileLength > maxLength) {
					this.ShowError("Arquivo muito grande!");
					return false;
				}
			}
			return true;
		}
	}
}