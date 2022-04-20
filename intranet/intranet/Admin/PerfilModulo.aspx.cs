using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;

namespace eVidaIntranet.Admin {
	public partial class PerfilModulo : PageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<KeyValuePair<int,string>> lst = AdministracaoBO.Instance.ListarTodosPerfis();
				lst.Insert(0,new KeyValuePair<int, string>(0, "SELECIONE"));
				dpdPerfil.DataSource = lst;
				dpdPerfil.DataBind();

				List<ModuloVO> lstModulo = AdministracaoBO.Instance.ListarTodosModulos();
				ViewState["MODULOS"] = lstModulo;

				List<CategoriaModuloVO> lstCategoria = AdministracaoBO.Instance.ListarTodasCategorias();
				rptCategoria.DataSource = lstCategoria;
				rptCategoria.DataBind();
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_PERFIL_MODULO; }
		}

		protected void dpdPerfil_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				int idPerfil = Convert.ToInt32(dpdPerfil.SelectedValue);
				List<Modulo> lst = null;
				if (idPerfil != 0)
					lst = AdministracaoBO.Instance.ListarModulosPerfil(idPerfil);
				if (lst == null)
					lst = new List<Modulo>();

				foreach (RepeaterItem item in rptCategoria.Items) {
					if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem) {
						CheckBoxList chkModulo = item.FindControl("chkModulo") as CheckBoxList;
						foreach (ListItem chk in chkModulo.Items) {
							Modulo m = (Modulo)Int32.Parse(chk.Value);
							chk.Selected = lst.Contains(m);
						}
					}
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar módulos do perfil: ", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				int idPerfil = Convert.ToInt32(dpdPerfil.SelectedValue);
				if (idPerfil == 0) {
					this.ShowError("Selecione o perfil!");
					return;
				}
				List<Modulo> lst = new List<Modulo>();

				foreach (RepeaterItem item in rptCategoria.Items) {
					if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem) {
						CheckBoxList chkModulo = item.FindControl("chkModulo") as CheckBoxList;
						foreach (ListItem chk in chkModulo.Items) {
							if (chk.Selected) {
								Modulo m = (Modulo)Int32.Parse(chk.Value);
								lst.Add(m);
							}
						}
					}
				}
				AdministracaoBO.Instance.SalvarModulosPerfil(idPerfil, lst);
				this.ShowInfo("Alterações realizadas com sucesso!");
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar módulos do perfil: ", ex);
			}
		}

		protected void rptCategoria_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			RepeaterItem item = e.Item;
			if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem) {
				CheckBoxList chkModulo = item.FindControl("chkModulo") as CheckBoxList;
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