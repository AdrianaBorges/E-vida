using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.Util;

namespace eVidaIntranet.Admin {
	public partial class PopResumoPermUsuario : PopUpPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				string login = Request["LOGIN"];
				UsuarioVO vo = UsuarioBO.Instance.GetUsuarioByLogin(login.FromBase64String());

				List<Modulo> lstModulos = UsuarioBO.Instance.ListarModulosUsuario(vo.Id);
				List<ModuloVO> lstAllModulos = AdministracaoBO.Instance.ListarTodosModulos();
				ViewState["MODULOS"] = lstAllModulos.FindAll(x => lstModulos.Contains(x.Value));

				List<CategoriaModuloVO> lstCategoria = AdministracaoBO.Instance.ListarTodasCategorias();
				rptCategoria.DataSource = lstCategoria;
				rptCategoria.DataBind();

			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_USUARIO; }
		}

		protected void rptCategoria_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			RepeaterItem item = e.Item;
			if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem) {
				DataList chkModulo = item.FindControl("chkModulo") as DataList;
				CategoriaModuloVO vo = (CategoriaModuloVO)item.DataItem;

				int idCategoria = vo.Id;

				List<ModuloVO> lstAll = ViewState["MODULOS"] as List<ModuloVO>;
				List<ModuloVO> lst = lstAll.FindAll(x => x.IdCategoria == idCategoria);
				lst.Sort((x, y) => x.Nome.CompareTo(y.Nome));

				chkModulo.DataSource = lst;
				chkModulo.DataBind();
			}
		}
	}
}