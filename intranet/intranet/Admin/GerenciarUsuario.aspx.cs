using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;

namespace eVidaIntranet.Admin {

	public partial class GerenciarUsuario : PageBase {

        protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ADMINISTRACAO_USUARIO; }
		}

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<KeyValuePair<int, string>> lst = AdministracaoBO.Instance.ListarTodosPerfis();
				lst.Insert(0, new KeyValuePair<int, string>(0, "TODOS"));
				lst.Insert(1, new KeyValuePair<int, string>(-1, "APENAS COM PERFIL"));
				lst.Insert(2, new KeyValuePair<int, string>(-2, "APENAS SEM PERFIL"));
				dpdPerfil.DataSource = lst;
				dpdPerfil.DataBind();

				List<KeyValuePair<string, string>> lstRegionais = PLocatorDataBO.Instance.ListarRegioes();
				dpdRegional.DataSource = lstRegionais.Select(x => new {
					Key = x.Key,
					Value = x.Value + " (" + x.Key + ")"
				});
				dpdRegional.DataBind();
				dpdRegional.Items.Insert(0, new ListItem("TODOS", ""));
			}
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				int? idPerfil = new int?();
				string idRegional = null;
				if (!dpdPerfil.SelectedValue.Equals("0")) {
					idPerfil = Convert.ToInt32(dpdPerfil.SelectedValue);
				}
				if (!string.IsNullOrEmpty(dpdRegional.SelectedValue)) {
					idRegional = dpdRegional.SelectedValue;
				}
				DataTable dt = UsuarioBO.Instance.PesquisarUsuarios(txtLogin.Text, txtNome.Text, idPerfil, idRegional);

				gdvUsuarios.DataSource = dt;
				gdvUsuarios.DataBind();
			}
			catch (Exception ex) {
				Log.Error("Erro ao consultar usuários!", ex);
				this.ShowError("Erro ao consultar usuários!" + ex.Message);
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {

            if (string.IsNullOrEmpty(txtLogin.Text.Trim()))
            {
                this.ShowError("Digite o Login.");
                return;
            }
            Response.Redirect("./EditarUsuario.aspx?ID=" + txtLogin.Text.Trim());
		}

	}
}