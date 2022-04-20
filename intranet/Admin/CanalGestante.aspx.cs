using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaIntranet.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eVidaIntranet.Admin {
	public partial class CanalGestante : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {

				dpdAno.Items.Add(new ListItem("SELECIONE", ""));
				for (int i = 2013; i <= 2030; i++) {
					dpdAno.Items.Add(new ListItem(i.ToString(), i.ToString()));
				}

			}
		}

		protected override Modulo Modulo {
			get { return Modulo.ADMINISTRACAO_CANAL_GESTANTE; }
		}

		#region Objects

		[Serializable]
		public class ProfissionalScreen {
			public string Key { get { return Config.Numcr + ";" + Config.Codsig + ";" + Config.Estado; } }
			public string Nome { get; set; }
			public CanalGestanteConfigProfVO Config { get; set; }
		}

		[Serializable]
		public class CredenciadoScreen {
			public string Key { get { return Config.CodCredenciado.ToString(); } }
			public string Nome { get; set; }
			public CanalGestanteConfigCredVO Config { get; set; }
		}

		public List<ProfissionalScreen> Profissionais {
			get {
				if (ViewState["ProfissionalScreen"] == null) {
					Profissionais = new List<ProfissionalScreen>();
				}
				return ViewState["ProfissionalScreen"] as List<ProfissionalScreen>;
			}
			set {
				ViewState["ProfissionalScreen"] = value;
			}
		}

		public List<CredenciadoScreen> Credenciados {
			get {
				if (ViewState["CredenciadoScreen"] == null) {
					Credenciados = new List<CredenciadoScreen>();
				}
				return ViewState["CredenciadoScreen"] as List<CredenciadoScreen>;
			}
			set {
				ViewState["CredenciadoScreen"] = value;
			}
		}

		#endregion

		private void Buscar() {
			pnlDados.Visible = false;

			if (string.IsNullOrEmpty(dpdAno.SelectedValue)) {
				this.ShowError("Informe o ano!");
				return;
			}

			int ano = Int32.Parse(dpdAno.SelectedValue);

			CanalGestanteConfigVO configVO = CanalGestanteBO.Instance.GetConfig(ano);
			Bind(configVO);

			pnlDados.Visible = true;
		}

		private void Bind(CanalGestanteConfigVO configVO) {
			List<CanalGestanteConfigCredVO> lstCred = null;
			List<CanalGestanteConfigProfVO> lstProf = null;

			txtPartoNormal.Text = configVO != null ? configVO.PartoNormal.ToString("##0.##") : "";

			if (configVO != null) {
				lstCred = CanalGestanteBO.Instance.GetConfigCred(configVO.Ano);
				lstProf = CanalGestanteBO.Instance.GetConfigProf(configVO.Ano);
			}

			BindCredenciados(lstCred);
			BindProfissionais(lstProf);
		}

		private void Salvar() {
			decimal partoNormal;
			if (string.IsNullOrEmpty(dpdAno.SelectedValue)) {
				this.ShowError("Informe um ano!");
				return;
			}
			if (!Decimal.TryParse(txtPartoNormal.Text, out partoNormal)) {
				this.ShowError("Informe um valor de parto normal válido para a operadora!");
				return;
			}
			if (partoNormal < 0 || partoNormal > 100) {
				this.ShowError("O percentual de parto normal deve ser de 0 a 100.");
				return;
			}

			bool ok = false;
			LoadCredenciadoFromScreen(out ok, true);
			if (!ok) return;

			LoadProfissionalFromScreen(out ok, true);
			if (!ok) return;

			CanalGestanteConfigVO vo = new CanalGestanteConfigVO();
			vo.Ano = Int32.Parse(dpdAno.SelectedValue);
			vo.CodUsuarioAlteracao = UsuarioLogado.Id;
			vo.DataAlteracao = DateTime.Now;
			vo.PartoNormal = Decimal.Round(partoNormal, 2);

			CanalGestanteBO.Instance.Salvar(vo, Credenciados.Select(x => x.Config), Profissionais.Select(x => x.Config));

			this.ShowInfo("Configuração para o ano " + vo.Ano + " salva com sucesso!");
			Buscar();
		}

		#region Profissionais

		private void BindProfissionais(List<CanalGestanteConfigProfVO> lstProf) {
			Profissionais = null;
			if (lstProf != null) {
				List<ProfissionalScreen> lst = new List<ProfissionalScreen>();
				foreach (CanalGestanteConfigProfVO configProf in lstProf) {
					PProfissionalSaudeVO profVO = PLocatorDataBO.Instance.GetProfissional(configProf.Codigo);
					ProfissionalScreen screenVO = new ProfissionalScreen();
					screenVO.Nome = profVO.Nome;
					screenVO.Config = configProf;
					lst.Add(screenVO);
				}
				Profissionais = lst;
			}
			BindProfissionais();

		}

		private void BindProfissionais() {
			Profissionais.Sort((x, y) => x.Nome.CompareTo(y.Nome));
			ltvMedicos.DataSource = Profissionais;
			ltvMedicos.DataBind();
		}

		private List<ProfissionalScreen> LoadProfissionalFromScreen(out bool ok, bool showErrors = true) {
			List<ProfissionalScreen> lst = Profissionais;
			int pos = 0;
			ok = true;
			foreach (ListViewItem item in ltvMedicos.Items) {
				TextBox txtPartoNormal = (TextBox)item.FindControl("txtPartoNormal");
				decimal parto = 0;
				if (!Decimal.TryParse(txtPartoNormal.Text, out parto)) {
					if (showErrors)
						this.ShowError("O valor do parto normal para o médico " + lst[pos].Nome + " está inválido! Valor inválido será zerado!");
					ok = false;
				} else {
					if (parto < 0 || parto > 100) {
						if (showErrors)
							this.ShowError("O percentual do parto normal para o médico " + lst[pos].Nome + " deve ser de 0 a 100!");
						ok = false;

					}
				}
				lst[pos].Config.PartoNormal = Decimal.Round(parto, 2);
				pos++;
			}
			return lst;
		}

		private bool CheckProfExistente(PProfissionalSaudeVO prof) {
			foreach (ProfissionalScreen screen in Profissionais) {
				if (screen.Config.Codigo == prof.Codigo) {
					return true;
				}
			}
			return false;
		}

        private void AddMedico(string NrConselho, string CdConselho, string CdUf)
        {
            PProfissionalSaudeVO Profissional = PLocatorDataBO.Instance.GetProfissional(NrConselho, CdUf, CdConselho);
			if (Profissional == null) {
				this.ShowError("Profissional não encontrado!");
				return;
			}

			if (CheckProfExistente(Profissional)) {
				this.ShowError("Este médico já pertence à listagem!");
				return;
			}

			ProfissionalScreen prof = new ProfissionalScreen();
			prof.Nome = Profissional.Nome;
			prof.Config = new CanalGestanteConfigProfVO();
			prof.Config.Codigo = Profissional.Codigo;
			prof.Config.Numcr = Profissional.Numcr;
            prof.Config.Codsig = Profissional.Codsig;
            prof.Config.Estado = Profissional.Estado;

			bool ok = true;
			LoadProfissionalFromScreen(out ok);
			Profissionais.Add(prof);
			BindProfissionais();
		}

		private void ExcluirMedico(int idx) {
			bool ok = true;
			LoadProfissionalFromScreen(out ok, false);
			Profissionais.RemoveAt(idx);
			BindProfissionais();
		}

		#endregion

		#region Credenciados

		private void BindCredenciados(List<CanalGestanteConfigCredVO> lstCred) {
			Credenciados = null;
			if (lstCred != null) {
				List<CredenciadoScreen> lst = new List<CredenciadoScreen>();
				foreach (CanalGestanteConfigCredVO configCred in lstCred) {
					PRedeAtendimentoVO credVO = PRedeAtendimentoBO.Instance.GetById(configCred.CodCredenciado);
                    if (credVO != null) {
                        CredenciadoScreen screenVO = new CredenciadoScreen();
                        screenVO.Nome = credVO.Nome;
                        screenVO.Config = configCred;
                        lst.Add(screenVO);
                    }
				}
				Credenciados = lst;
			}
			BindCredenciados();

		}

		private void BindCredenciados() {
			Credenciados.Sort((x, y) => !x.Config.Uf.Equals(y.Config.Uf) ? x.Config.Uf.CompareTo(y.Config.Uf) : x.Nome.CompareTo(y.Nome));
			ltvEstabelecimentos.DataSource = Credenciados;
			ltvEstabelecimentos.DataBind();
		}

		private List<CredenciadoScreen> LoadCredenciadoFromScreen(out bool ok, bool showErrors = true) {
			List<CredenciadoScreen> lst = Credenciados;
			int pos = 0;
			ok = true;
			foreach (ListViewItem item in ltvEstabelecimentos.Items) {
				TextBox txtPartoNormal = (TextBox)item.FindControl("txtPartoNormal");
				decimal parto = 0;
				if (!Decimal.TryParse(txtPartoNormal.Text, out parto)) {
					if (showErrors)
						this.ShowError("O valor do parto normal para o estabelecimento " + lst[pos].Nome + " está inválido! Valor inválido será zerado!");
					ok = false;
				} else {
					if (parto < 0 || parto > 100) {
						if (showErrors)
							this.ShowError("O percentual do parto normal para o estabelecimento " + lst[pos].Nome + " deve ser de 0 a 100!");
						ok = false;

					}
				}
				
				lst[pos].Config.PartoNormal = Decimal.Round(parto, 2);
				pos++;
			}
			return lst;
		}

		private bool CheckCredExistente(string idCredenciado) {
			foreach (CredenciadoScreen screen in Credenciados) {
				if (screen.Config.CodCredenciado == idCredenciado) {
					return true;
				}
			}
			return false;
		}

		private void AddCredenciado(string idCredenciado) {

			PRedeAtendimentoVO credVO = PRedeAtendimentoBO.Instance.GetById(idCredenciado);
			if (credVO == null) {
				this.ShowError("Credenciado não encontrado!");
				return;
			}

			if (CheckCredExistente(idCredenciado)) {
				this.ShowError("Este credenciado já pertence à listagem!");
				return;
			}

            string uf = string.Empty;
            uf = credVO.Est;
            if (string.IsNullOrEmpty(uf))
            {
                this.ShowError("O endereço do credenciado está incorreto ou incompleto (não possui UF) no Protheus. Por favor ajuste-o antes de incluir no canal Gestante.");
                return;
            }

			CredenciadoScreen screenVO = new CredenciadoScreen();
			screenVO.Nome = credVO.Nome;
			screenVO.Config = new CanalGestanteConfigCredVO();
			screenVO.Config.CodCredenciado = credVO.Codigo;
			screenVO.Config.Uf = uf;

			bool ok = true;
			LoadCredenciadoFromScreen(out ok);

			Credenciados.Add(screenVO);
			BindCredenciados();
		}

		private void ExcluirCredenciado(int idx) {
			bool ok = true;
			LoadCredenciadoFromScreen(out ok, false);
			Credenciados.RemoveAt(idx);
			BindCredenciados();
		}

		#endregion

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				Buscar();
			} catch (Exception ex) {
				this.ShowError("Erro ao buscar dados do ano!", ex);
			}
		}

		protected void bntExcluirEstabelecimento_Click(object sender, ImageClickEventArgs e) {
			try {
				ListViewItem row = (ListViewItem)(sender as ImageButton).NamingContainer;
				ExcluirCredenciado(row.DataItemIndex);
			} catch (Exception ex) {
				this.ShowError("Erro ao excluir estabelecimento da lista!", ex);
			}
		}

		protected void bntExcluirMedico_Click(object sender, ImageClickEventArgs e) {
			try {
				ListViewItem row = (ListViewItem)(sender as ImageButton).NamingContainer;
				ExcluirMedico(row.DataItemIndex);
			} catch (Exception ex) {
				this.ShowError("Erro ao excluir médico da lista!", ex);
			}
		}

		protected void dpdAno_SelectedIndexChanged(object sender, EventArgs e) {
			pnlDados.Visible = false;
		}

		protected void btnAddMedico_Click(object sender, EventArgs e) {
			try {
				string hidProfissional = hidCodProfissional.Value;
				if (string.IsNullOrEmpty(hidProfissional)) {
					this.ShowError("Código invalido!");
					return;
				}

				string[] values = hidProfissional.Split(';');
				string NrConselho = values[0];
				string CdConselho = values[1];
                string CdUf = values[2];

                AddMedico(NrConselho, CdConselho, CdUf);
			} catch (Exception ex) {
				this.ShowError("Erro ao incluir medico na lista!", ex);
			}
		}

		protected void ltvMedicos_ItemDataBound(object sender, ListViewItemEventArgs e) {
			ListViewItem item = e.Item;
			if (item.ItemType == ListViewItemType.DataItem) {
				TextBox txtPartoNormal = (TextBox)item.FindControl("txtPartoNormal");
				ProfissionalScreen screen = (ProfissionalScreen) item.DataItem;
				//txtPartoNormal.Text = screen.Config.PartoNormal.ToString("###");
			}
		}

		protected void btnAddEstabelecimento_Click(object sender, EventArgs e) {
			try {
				string codCredenciado = hidCodCredenciado.Value;
				if (string.IsNullOrEmpty(codCredenciado)) {
					this.ShowError("Código invalido!");
					return;
				}

				AddCredenciado(codCredenciado);
			} catch (Exception ex) {
				this.ShowError("Erro ao incluir credenciado na lista!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar configuração!", ex);
			}
		}

	}
}