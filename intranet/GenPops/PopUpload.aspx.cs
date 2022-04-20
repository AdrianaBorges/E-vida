using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVida.Web.Upload;
using eVidaGeneralLib.Util;
using eVidaIntranet.Classes;

namespace eVidaIntranet.GenPops {
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
				hidRnd.Value = UploadConfigManager.GetPrefix(UploadConfig, Sistema.INTRANET, this.UsuarioLogado.Id.ToString());
				if (GetUploadFilter().Equals("*")) {
					this.RegisterScript("FILETYPE", "var fileTypes = /(\\.|.*)$/i");
				} else {
					this.RegisterScript("FILETYPE", "var fileTypes = /(\\.|\\/)(" + GetUploadFilter().Replace(",", "|") + ")$/i");
				}
				lnkClassic.NavigateUrl = "./PopUploadClassico.aspx" + Request.Url.Query;
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.INICIAL; }
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