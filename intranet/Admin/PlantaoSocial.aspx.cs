using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Admin {
	public partial class PlantaoSocial : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdUf.DataSource = PConstantes.Uf.Values;
				dpdUf.DataBind();
				dpdUf.Items.Insert(0, new ListItem("SELECIONE", ""));

				BindAll();
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.ADMINISTRACAO_PLANTAO_SOCIAL; }
		}

		private int? EditId {
			get { return (int?)ViewState["ED_ID"]; }
			set { ViewState["ED_ID"] = value; }
		}

		private void BindAll() {
			List<PlantaoSocialLocalVO> lstLocais = AutorizacaoProvisoriaBO.Instance.ListarPlantaoSocialLocal();

			ltvPlantaoSocial.DataSource = lstLocais;
			ltvPlantaoSocial.DataBind();

			updLocais.Update();
		}
		
		private void BindUf(string uf) {
			IEnumerable<PCepVO> lstMunicipio = PLocatorDataBO.Instance.BuscarMunicipiosPROTHEUSCep(uf);

			dpdMunicipio.DataSource = lstMunicipio;
			dpdMunicipio.DataBind();
			dpdMunicipio.Items.Insert(0, new ListItem("SELECIONE", ""));
		}

		private void ClearForm() {
			dpdMunicipio.Enabled = true;
			dpdMunicipio.Items.Clear();
			dpdUf.Enabled = true;
			dpdUf.SelectedValue = string.Empty;
			txtTelefone.Text = string.Empty;
			EditId = null;
			btnNovo.Visible = false;
		}

		private void SalvarPlantao() {
			string uf = dpdUf.SelectedValue;
			string telefone = txtTelefone.Text;

			if (string.IsNullOrEmpty(uf)) {
				this.ShowError("Selecione a UF!");
				return;
			}
			if (string.IsNullOrEmpty(dpdMunicipio.SelectedValue)) {
				this.ShowError("Selecione o município!");
				return;
			}
			if (string.IsNullOrEmpty(telefone)) {
				this.ShowError("Informe o telefone a ser salvo!");
				return;
			}

			PlantaoSocialLocalVO vo = new PlantaoSocialLocalVO();
			vo.Uf = uf;
			vo.CodMunicipio = dpdMunicipio.SelectedValue;
			vo.Cidade = dpdMunicipio.SelectedItem.Text;
			vo.Telefone = telefone;

			if (EditId != null) {
				vo.Id = EditId.Value;
			} else {
				if (AutorizacaoProvisoriaBO.Instance.ExistePlantao(vo.Uf, vo.CodMunicipio)) {
					this.ShowError("Já existe um plantão cadastrado para este local! Caso queira alterar o telefone selecione o plantão para edição.");
					return;
				}
			}

			AutorizacaoProvisoriaBO.Instance.Salvar(vo);
			this.ShowInfo("Telefone salvo com sucesso!");

			ClearForm();
			BindAll();
		}
		
		private void RemoverPlantaoSocial(ListViewItem row) {
			int id = (int)ltvPlantaoSocial.DataKeys[row.DataItemIndex]["Id"];

			PlantaoSocialLocalVO vo = new PlantaoSocialLocalVO();
			vo.Id = id;

			if (AutorizacaoProvisoriaBO.Instance.IsLocalUtilizado(vo)) {
				this.ShowError("Este plantão social já é utilizada em um formulário!");
				return;
			}
			AutorizacaoProvisoriaBO.Instance.Excluir(vo);
			this.ShowInfo("Dados do plantão social removidos com sucesso!");

			BindAll();
		}

		private void Editar(ListViewItem row) {
			int id = (int)ltvPlantaoSocial.DataKeys[row.DataItemIndex]["Id"];

			PlantaoSocialLocalVO vo = AutorizacaoProvisoriaBO.Instance.GetPlantaoById(id);
			dpdUf.SelectedValue = vo.Uf;
			BindUf(vo.Uf);
			dpdMunicipio.SelectedValue = vo.CodMunicipio;
			txtTelefone.Text = vo.Telefone;
			EditId = id;
			btnNovo.Visible = true;

			dpdMunicipio.Enabled = false;
			dpdUf.Enabled = false;

			updUf.Update();
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarPlantao();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar telefone.", ex);
			}
		}

		protected void bntExcluir_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem ufRow = (ListViewItem)btn.NamingContainer;

				RemoverPlantaoSocial(ufRow);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao remover plantão social!", ex);
			}
		}

		protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				if (!string.IsNullOrEmpty(dpdUf.SelectedValue))
					BindUf(dpdUf.SelectedValue);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao obter dados da UF", ex);
			}
		}

		protected void btnEditar_Click(object sender, ImageClickEventArgs e) {
			try {
				ImageButton btn = (ImageButton)sender;
				ListViewItem ufRow = (ListViewItem)btn.NamingContainer;

				Editar(ufRow);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao montar formulário para edição de plantão social!", ex);
			}
		}

		protected void btnNovo_Click(object sender, EventArgs e) {
			ClearForm();
		}

	}
}