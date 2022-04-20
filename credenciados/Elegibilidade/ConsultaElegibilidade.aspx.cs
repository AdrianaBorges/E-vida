using eVidaCredenciados.Classes.Elegibilidade;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace eVidaCredenciados.Elegibilidade {
	public partial class ConsultaElegibilidade : PageBase {

        string[] comRestricao = new string[] { "000000001", "000000002", "000000003", "000000007", "000000008", "000000009" };

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<PProdutoSaudeVO> lstPlanos = PLocatorDataBO.Instance.ListarProdutoSaude();
                var datasource = from x in lstPlanos.Where(x => x.Codigo != "0012")             // O plano E-VIDA PPRS GENITORES (0012) não deve aparecer na lista.
								 select new {
									 x.Codigo,
									 x.Descri,
                                     DisplayField = String.Format("{0} ({1})", x.Descri, x.Codant)
								 };
				dpdPlano.DataSource = datasource;
				dpdPlano.DataBind();
				dpdPlano.Items.Insert(0, new ListItem("SELECIONE", ""));

				tbResult.Visible = false;
			}
		}

		public override bool IsLoginPage() {
			return true;
		}

		protected void btnLogin_Click(object sender, EventArgs e) {
			try {
				string cartao = txtCartao.Text;
				string strDataNascimento = txtNascimento.Text;
				DateTime dataNascimento;

				lblLoginExpirado.Visible = false;
				tbResult.Visible = false;

				if (string.IsNullOrEmpty(cartao)) {
					this.ShowError("O número da carteira é obrigatório!");
					return;
				}

				if (string.IsNullOrEmpty(strDataNascimento)) {
					this.ShowError("A data de nascimento é obrigatória!");
					return;
				} else if (!DateTime.TryParse(strDataNascimento, out dataNascimento)) {
					this.ShowError("Data de nascimento inválida!");
					return;
				}

				if (string.IsNullOrEmpty(dpdPlano.SelectedValue)) {
					this.ShowError("Informe o plano do beneficiário!");
					return;
				}
				string plano = dpdPlano.SelectedValue;

                PUsuarioVO vo = PUsuarioBO.Instance.GetUsuarioByCartao(cartao);
				if (vo == null) {
					this.ShowError("Beneficiário não encontrado!");
					return;
				}
                if (vo.Datnas == null || vo.Datnas == String.Empty || vo.Datnas == "        ")
                {
					this.ShowError("Dados do beneficiário incompletos. Contate o setor de cadastro!");
					return;
				}
				if (vo.Datnas != dataNascimento.ToString("yyyyMMdd")) {
					this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
					return;
				}

                PFamiliaProdutoVO benefPlanoVO = PFamiliaBO.Instance.GetFamiliaProduto(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
				if (benefPlanoVO == null) {
					this.ShowError("Não foi possível validar o plano do beneficiário!");
					return;
				}
				if (!benefPlanoVO.Codpla.Equals(plano)) {
					this.ShowError("Dados inválidos. Verifique os dados informados e tente novamente!");
					return;
				}

				Bind(vo, benefPlanoVO);

			} catch (Exception ex) {
				this.ShowError("Erro ao realizar consulta!", ex);
			}
		}

        private void Bind(PUsuarioVO vo, PFamiliaProdutoVO benefPlanoVO)
        {
			tbResult.Visible = true;

            bool ativo = benefPlanoVO.Datblo == null || benefPlanoVO.Datblo == String.Empty || benefPlanoVO.Datblo == "        " || DateTime.ParseExact(benefPlanoVO.Datblo, "yyyyMMdd", CultureInfo.InvariantCulture) > DateTime.Now.Date;
            lblSituacao.Text = ativo ? "ATIVO" : "BLOQUEADO";
            
            // Se for CEA e se for um dos subcontratos com restrição
            if (vo.Codemp == "0003" && comRestricao.Contains(vo.Subcon) && ativo)
            {
                lblSituacao.Text = "(COM RESTRIÇÃO)";
                lblAtendimento.Text = "";
            }
            
			lblSituacao.ForeColor = ativo ? System.Drawing.Color.Blue : System.Drawing.Color.Red;

			litNomeBeneficiario.Text = vo.Nomusr;

            #region[NOME NA CARTEIRA]

            // O nome do beneficiário deve ser obtido de bts_nomcar. SGA nº 12728059 em 01/04/2019
            PVidaVO vida = PVidaBO.Instance.GetVida(vo.Matvid);
            if(vida != null)
            {
                if(vida.Nomcar.Trim() != String.Empty)
                {
                    litNomeBeneficiario.Text = vida.Nomcar.Trim();
                }
            }

            #endregion

            litDataNascimento.Text = DateTime.ParseExact(vo.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
			litCartao.Text = txtCartao.Text;    // O Tiago Souza pediu em 27/07/2018 pra mostrar a carteira de acordo com a pesquisa.

			PProdutoSaudeVO planoVO = PLocatorDataBO.Instance.GetProdutoSaude(benefPlanoVO.Codpla);
            litPlano.Text = planoVO.Descri + " (" + planoVO.Codant + ")";
            litCobertura.Text = DateTime.ParseExact(benefPlanoVO.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();

			bool carencia = HasCarencia(vo, benefPlanoVO);
			if(carencia) {
				lblCarencia.Text = "BENEFICIÁRIO EM CUMPRIMENTO DE PERÍODO DE CARÊNCIA";
				lblCarencia.ForeColor = System.Drawing.Color.Red;
			} else {
				lblCarencia.Text = "BENEFICIÁRIO NÃO ESTÁ EM CARÊNCIA";
				lblCarencia.ForeColor = System.Drawing.Color.Blue;

                // Se for CEA e se for um dos subcontratos com restrição
                if (vo.Codemp == "0003" && comRestricao.Contains(vo.Subcon) && ativo) {
                    lblCarencia.Text = "(COM RESTRIÇÃO VIDE OBSERVAÇÃO)";
                }

			}

			litObservacao.Text = BuildObs(carencia, vo, benefPlanoVO);
		}

        private bool HasCarencia(PUsuarioVO vo, PFamiliaProdutoVO benefPlanoVO)
        {
			/*
			 * MSG PARA CAMPO CARÊNCIA
			 * TP_SEXO = ‘M’
			 *		SE TP_CARENCIA = ‘NOR’ E (SYSDATE > DT_INICIO_VIGENCIA + 180) OU TP_CARENCIA = ‘IT’
			 *			MSG = ‘BENEFICIÁRIO NÃO ESTÁ EM CARÊNCIA’
			 *		SENÃO
			 *			MSG = ‘BENEFICIÁRIO EM CUMPRIMENTO DE PERÍODO DE CARÊNCIA’ 
			 * 
			 * TP_SEXO = ‘F’
			 *		SE TP_CARENCIA = ‘NOR’ E (SYSDATE > DT_INICIO_VIGENCIA + 300) OU TP_CARENCIA = ‘IT’
			 *			MSG = ‘BENEFICIÁRIO NÃO ESTÁ EM CARÊNCIA’
			 *		SENÃO
			 *			MSG = ‘BENEFICIÁRIO EM CUMPRIMENTO DE PERÍODO DE CARÊNCIA’
			 * */

			string sexo = vo.Sexo;
			string tipoCarencia = vo.Ycaren;
			int qtdDiasCarencia = 0;

			if (sexo.Equals(PConstantes.SEXO_MASCULINO)) qtdDiasCarencia = 180;
			else qtdDiasCarencia = 300;

			bool emCarencia = true;

			if (tipoCarencia.Equals(PFamiliaProdutoVO.CARENCIA_NORMAL)) {
                if (DateTime.ParseExact(vo.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).AddDays(qtdDiasCarencia) <= DateTime.Now.Date)
                {
					emCarencia = false;
				} else {
					emCarencia = true;
				}
            }
            else if (tipoCarencia.Equals(PFamiliaProdutoVO.CARENCIA_ISENTO))
            {
				emCarencia = false;
			}

			return emCarencia;
		}

        private string BuildObs(bool isCarencia, PUsuarioVO vo, PFamiliaProdutoVO benefPlanoVO)
        {
			/*
			 *	SE MSG = ‘BENEFICIÁRIO NÃO ESTÁ EM CARÊNCIA’
			 *		SE CD_PLANO_VINCULADO IN (‘20’, ‘1’, ‘2’, ‘4’)
			 *			MSG_OBS = (NULL)
			 *		SE CD_PLANO VINCULADO IN (‘3’, ‘5’, ‘21’, ‘22’, ‘23’)
			 *			MSG_OBS = ‘COBERTURA APENAS DO ROL DE PROCEDIMENTOS DA ANS’
			 *			
			 *	SE MSG = ‘BENEFICIÁRIO EM CUMPRIMENTO DE PERÍODO DE CARÊNCIA’ 
			 *		SE CD_PLANO_VINCULADO IN (‘20’, ‘1’, ‘2’, ‘4’)
			 *			SE TP_SEXO = ‘M’
			 *				MSG_OBS = ‘TÉRMINO CARÊNCIA 
			 *							URGÊNCIA/EMERGÊNCIA: %DATA1%
			 *							DEMAIS PROCEDIMENTOS: %DATA2%’
			 *			SE TP_SEXO = ‘F’
			 *				MSG_OBS = ‘TÉRMINO CARÊNCIA
			 *							URGÊNCIA/EMERGÊNCIA: %DATA1%
			 *							DEMAIS PROCEDIMENTOS: %DATA2%’
			 *							PARTO: %DATA3%’
			 *		SE CD_PLANO_VINCULADO IN (‘3’, ‘5’, ‘21’, ‘22’, ‘23’)
			 *			SE TP_SEXO = ‘M’
			 *				MSG_OBS = ‘COBERTURA APENAS DO ROL DE PROCEDIMENTOS DA ANS
			 *							TÉRMINO CARÊNCIA
			 *							URGÊNCIA/EMERGÊNCIA: %DATA1%
			 *							DEMAIS PROCEDIMENTOS: %DATA2%’
			 *			SE TP_SEXO = ‘F’
			 *				MSG_OBS = ‘COBERTURA APENAS DO ROL DE PROCEDIMENTOS DA ANS
			 *							TÉRMINO CARÊNCIA
			 *							 URGÊNCIA/EMERGÊNCIA: %DATA1%
			 *							 DEMAIS PROCEDIMENTOS: %DATA2%’
			 *							 PARTO: %DATA3%’
			 */

            // Se for CEA e se for um dos subcontratos com restrição
            if (vo.Codemp == "0003" && comRestricao.Contains(vo.Subcon)){
                return "BENEFICIARIO COM PLANO SUSPENSO, SEM DIREITO A ATENDIMENTO";
            }

            string[] planosApenasRol = new string[] { "0003", "0005", "0007", "0008", "0009", "0010", "0011" };
            string[] planoNormal = new string[] { "0001", "0002", "0004", "0006" };
			string header = "";
			string container = "";
			string cdPlano = benefPlanoVO.Codpla;

			if (planoNormal.Contains(cdPlano)) {
				header = "-";
			} else {
				header = "COBERTURA APENAS DO ROL DE PROCEDIMENTOS DA ANS";
			}

			if (!isCarencia) {
				return header;
			}
			
			string sexo = vo.Sexo;
            string data1 = DateTime.ParseExact(vo.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).AddDays(1).ToShortDateString();
            string data2 = DateTime.ParseExact(vo.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).AddDays(180).ToShortDateString();
            string data3 = DateTime.ParseExact(vo.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).AddDays(300).ToShortDateString();
			container = header + "<BR/>TÉRMINO CARÊNCIA <BR/>URGÊNCIA/EMERGÊNCIA: " + data1 + " <BR/>DEMAIS PROCEDIMENTOS: " + data2;
			if (sexo.Equals(PConstantes.SEXO_FEMININO)) {
				container += "<BR/>PARTO: " + data3;
			}
			return container;
		}
	}
}