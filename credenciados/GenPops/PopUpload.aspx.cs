using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Upload;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.Util;

namespace eVidaCredenciados.GenPops {
	public partial class PopUpload : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string tipo = Request["TIPO"];
				if (string.IsNullOrEmpty(tipo)) {
					this.ShowError("Tipo de upload não definido!");
					hidRnd.Visible = false;
					return;
				}	
				UploadConfig = UploadConfigManager.GetConfig(tipo);
                hidRnd.Value = UploadConfigManager.GetPrefix(UploadConfig, Sistema.CREDENCIADO, this.UsuarioLogado.RedeAtendimento.Codigo);
				this.RegisterScript("FILETYPE", "var fileTypes = /(\\.|\\/)(" + GetUploadFilter().Replace(",", "|") + ")$/i");
			}
		}

		public UploadConfig UploadConfig { get; set; }

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
	}
}