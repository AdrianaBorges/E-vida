using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaIntranet.Forms {
	public partial class FormAtestadoComparecimento : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				btnSalvar.Visible = true;
				ltData.Text = String.Format("{0:dd \\de MMMM \\de yyyy}", DateTime.Now);

				if (!string.IsNullOrEmpty(Request["ID"])) {
					int id = Int32.Parse(Request["ID"]);
					Bind(id);
				} else {
					txtData.Text = DateTime.Now.ToShortDateString();
					txtHoraInicial.Text = String.Format("{0:00}", DateTime.Now.Hour);
					txtMinInicial.Text = String.Format("{0:00}", DateTime.Now.Minute);
				}
			}

		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.ATESTADO_COMPARECIMENTO; }
		}

		private void Bind(PUsuarioVO titular) {
			if (titular != null) {
				PFamiliaVO funcVO = PFamiliaBO.Instance.GetByMatricula(titular.Codint, titular.Codemp, titular.Matric);
				txtNomeTitular.Text = titular.Nomusr;
				LotacaoVO lotacao = PLocatorDataBO.Instance.GetLotacao(funcVO.Undorg);
				if (lotacao != null)
					txtLotacao.Text = lotacao.SgLotacao;

				FillBeneficiarios(titular);
			} else {
				txtNomeTitular.Text = "";
				txtLotacao.Text = "";
				FillBeneficiarios(null);
			}
			
		}

		private void Bind(int id) {
			AtestadoComparecimentoVO vo = AtestadoComparecimentoBO.Instance.GetById(id);
			Bind(vo);
		}

		private void Bind(AtestadoComparecimentoVO vo) {
			ViewState["ID"] = vo.CodSolicitacao.ToString();
			litProtocolo.Value = vo.CodSolicitacao.ToString();

			txtNomeTitular.Text = vo.Nome;
			txtLotacao.Text = vo.Lotacao;
			txtData.Text = vo.DataAtendimento.ToShortDateString();

			PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
			if (titular == null) {
                this.ShowError("Titular não encontrado - Codint: " + titular.Codint + " - Codemp: " + titular.Codemp + " - Matric: " + titular.Matric + " - Tipreg: " + titular.Tipreg);
				return;
			}

			txtCartao.Text = titular.Matant;
			FillBeneficiarios(titular);
            if (!String.IsNullOrEmpty(vo.Codint) && !String.IsNullOrEmpty(vo.Codemp) && !String.IsNullOrEmpty(vo.Matric) && !String.IsNullOrEmpty(vo.Tipreg))
            {
                string cd_usuario = vo.Codint.Trim() + "|" + vo.Codemp.Trim() + "|" + vo.Matric.Trim() + "|" + vo.Tipreg.Trim();
                dpdBeneficiario.SelectedValue = cd_usuario;
            }
            else
            {
                dpdBeneficiario.SelectedValue = String.Empty;
            }
				

			string[] horaMinuto = vo.HoraInicio.Split(':');
			txtHoraInicial.Text = horaMinuto[0];
			txtMinInicial.Text = horaMinuto[1];
			horaMinuto = vo.HoraFim.Split(':');
			txtHoraFinal.Text = horaMinuto[0];
			txtMinFinal.Text = horaMinuto[1];

			foreach (ListItem item in chkTipoAtendimento.Items) {
				int val = Int32.Parse(item.Value);
				item.Selected = (val & vo.TipoPericia) == val;
			}
			
			ltData.Text = FormatUtil.FormatDataExtenso(vo.DataCriacao);
			
			btnFinalizar.Visible = vo.IdStatus == (int)StatusAtestadoComparecimento.PENDENTE;
			btnPdf.Visible = btnImprimir.Visible = true;
			btnSalvar.Visible = vo.IdStatus == (int)StatusAtestadoComparecimento.PENDENTE;
		}

		private void FillBeneficiarios(PUsuarioVO titular) {
			List<PUsuarioVO> lstBenef = null;
            
            dpdBeneficiario.Items.Clear();

			if (titular != null) {

                string cd_usuario = titular.Codint.Trim() + "|" + titular.Codemp.Trim() + "|" + titular.Matric.Trim() + "|" + titular.Tipreg.Trim();
                
                lstBenef = PUsuarioBO.Instance.ListarDependentes(titular.Codint, titular.Codemp, titular.Matric);
                if (lstBenef != null) {
                    lstBenef.Sort((x, y) => (x.Nomusr.CompareTo(y.Nomusr)));
                }
                
                dpdBeneficiario.DataSource = lstBenef;
                dpdBeneficiario.DataBind();
                dpdBeneficiario.Items.Insert(0, new ListItem("TITULAR", cd_usuario));
			}
            
		}

		private int GetSelectedPericia() {
			int value = 0;
			foreach (ListItem item in chkTipoAtendimento.Items) {
				if (item.Selected)
					value += Int32.Parse(item.Value);
			}
			return value;
		}

		private bool ValidateRequired() {
			List<ItemError> lst = new List<ItemError>();
			if (string.IsNullOrEmpty(txtCartao.Text)) {
				this.AddErrorMessage(lst, txtCartao, "Informe o cartão do titular!");
			} 
			if (string.IsNullOrEmpty(txtNomeTitular.Text)) {
				this.AddErrorMessage(lst, txtNomeTitular, "Informe o nome do funcionário!");
			}
            // Tiago Souza pediu pra retirar esta validação em 26/03/0218
			//if (string.IsNullOrEmpty(txtLotacao.Text)) {
			//	this.AddErrorMessage(lst, txtLotacao, "Informe a lotação do funcionário!");
			//}
			if (string.IsNullOrEmpty(txtData.Text)) {
				this.AddErrorMessage(lst, txtLotacao, "Informe a data de comparecimento!");
			}
			if (string.IsNullOrEmpty(txtHoraInicial.Text) || string.IsNullOrEmpty(txtMinInicial.Text)) {
				this.AddErrorMessage(lst, txtHoraInicial, "Informe a hora de início!");
			}
			if (string.IsNullOrEmpty(txtHoraFinal.Text) || string.IsNullOrEmpty(txtMinFinal.Text)) {
				this.AddErrorMessage(lst, txtHoraFinal, "Informe a hora de fim!");
			}
			
			int tpPericia = GetSelectedPericia();

			if (tpPericia == 0) {
				this.AddErrorMessage(lst, chkTipoAtendimento, "Informe pelo menos uma perícia!");
			}

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}

			return true;
		}

		private bool ValidateFields() {
			if (!ValidateRequired())
				return false;

			List<ItemError> lst = new List<ItemError>();

			DateTime dtInicio = DateTime.MinValue;
			DateTime dtFim = DateTime.MinValue;

			DateTime data = DateTime.MinValue;
			int iValue;

			if (!DateTime.TryParse(txtData.Text, out data)) {
				this.AddErrorMessage(lst, txtData, "Informe uma data válida!");
			} else {
				dtInicio = data;
				dtFim = data;
			}
			
			if (!Int32.TryParse(txtHoraInicial.Text, out iValue)) {
				this.AddErrorMessage(lst, txtHoraInicial, "A hora inicial deve ser numérica!");
			} else {
				if (iValue < 0 || iValue > 24) {
					this.AddErrorMessage(lst, txtHoraInicial, "A hora inicial deve estar entre 0 e 24!");
				} else {
					if (dtInicio != DateTime.MinValue)
						dtInicio = dtInicio.AddHours(iValue);
				}
			} 
			if (!Int32.TryParse(txtMinInicial.Text, out iValue)) {
				this.AddErrorMessage(lst, txtHoraInicial, "O minuto inicial deve ser numérico!");
			} else {
				if (iValue < 0 || iValue > 59) {
					this.AddErrorMessage(lst, txtHoraFinal, "O minuto inicial deve estar entre 0 e 59!");
				} else {
					if (dtInicio != DateTime.MinValue)
						dtInicio = dtInicio.AddMinutes(iValue);
				}
			}

			if (!Int32.TryParse(txtHoraFinal.Text, out iValue)) {
				this.AddErrorMessage(lst, txtHoraFinal, "A hora final deve ser numérica!");
			} else {
				if (iValue < 0 || iValue > 24) {
					this.AddErrorMessage(lst, txtHoraFinal, "A hora final deve estar entre 0 e 24!");
				} else {
					if (dtFim != DateTime.MinValue)
						dtFim = dtFim.AddHours(iValue);
				}
			}

			if (!Int32.TryParse(txtMinFinal.Text, out iValue)) {
				this.AddErrorMessage(lst, txtMinFinal, "O minuto final deve ser numérico!");
			} else {
				if (iValue < 0 || iValue > 59) {
					this.AddErrorMessage(lst, txtMinFinal, "O minuto final deve estar entre 0 e 59!");
				} else {
					if (dtFim != DateTime.MinValue)
						dtFim = dtFim.AddMinutes(iValue);
				}
			}

			if (dtInicio != DateTime.MinValue && dtFim != DateTime.MinValue) {
				if (dtInicio >= dtFim) {
					this.AddErrorMessage(lst, txtHoraInicial, "A hora inicial deve ser menor que a hora final!");
				}
			}

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}
			return true;
		}

		private void Salvar(bool finalizar) {
			if (!ValidateFields()) {
				return;
			}

			AtestadoComparecimentoVO vo = new AtestadoComparecimentoVO();
			if (!string.IsNullOrEmpty((string)ViewState["ID"])) {
				vo.CodSolicitacao = Convert.ToInt32(ViewState["ID"]);
			}
			vo.DataAtendimento = DateTime.Parse(txtData.Text);
			vo.HoraInicio = txtHoraInicial.Text + ":" + txtMinInicial.Text;
			vo.HoraFim = txtHoraFinal.Text + ":" + txtMinFinal.Text;
			vo.Lotacao = txtLotacao.Text.ToUpper();

			PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
            vo.Codint = titular.Codint;
            vo.Codemp = titular.Codemp;
            vo.Matric = titular.Matric;
            vo.Tipreg = titular.Tipreg;
			vo.Nome = txtNomeTitular.Text.ToUpper();
			vo.TipoPericia = GetSelectedPericia();

            if (!String.IsNullOrEmpty(dpdBeneficiario.SelectedValue))
            {
                string[] dados_beneficiario = dpdBeneficiario.SelectedValue.Split('|');
                vo.Codint = dados_beneficiario[0];
                vo.Codemp = dados_beneficiario[1];
                vo.Matric = dados_beneficiario[2];
                vo.Tipreg = dados_beneficiario[3];
            }

			vo.IdUsuarioCriacao = UsuarioLogado.Id;
			vo.IdUsuarioFinalizacao = UsuarioLogado.Id;
			vo.IdStatus = (int)StatusAtestadoComparecimento.PENDENTE;
			AtestadoComparecimentoBO.Instance.Salvar(vo, finalizar);

			if (finalizar)
				this.ShowInfo("Formulário finalizado com sucesso!");
			else
				this.ShowInfo("Formulário salvo com sucesso!");
			
			Bind(vo.CodSolicitacao);
		}

		private void Finalizar() {
			Salvar(true);
		}

		protected void txtCartao_TextChanged(object sender, EventArgs e) {
			try {
				PUsuarioVO titular = PUsuarioBO.Instance.GetUsuarioByCartao(txtCartao.Text);
				Bind(titular);
				if (titular == null) {
					this.ShowError("Titular não encontrado!");
					return;
				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar o titular!", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar(false);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar formulário!", ex);
			}
		}

		protected void btnFinalizar_Click(object sender, EventArgs e) {
			try {
				Finalizar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao finalizar formulário! ", ex);
			}
		}

	}
}