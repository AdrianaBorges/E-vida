using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaBeneficiarios.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaBeneficiarios.Forms {
	public partial class DadosPessoais : FormPageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				dpdUf.DataSource = PConstantes.Uf.Values;
				dpdUf.DataBind();
				dpdUf.Items.Insert(0, new ListItem("-", ""));

                /*
                dpdBanco.DataSource = LocatorDataBO.Instance.ListarBancoISA();
                dpdBanco.DataValueField = "Id";
                dpdBanco.DataTextField = "Descricao";
                dpdBanco.DataBind();
                dpdBanco.Items.Insert(0, new ListItem("Selecione", ""));
                */
				Bind();
			}
		}

		private void Bind() {

            PUsuarioVO vo = PUsuarioBO.Instance.GetUsuario(UsuarioLogado.UsuarioTitular.Codint, UsuarioLogado.UsuarioTitular.Codemp, UsuarioLogado.UsuarioTitular.Matric, UsuarioLogado.UsuarioTitular.Tipreg);
            PFamiliaVO fam = PFamiliaBO.Instance.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric);

			lblEmpresa.Text = PLocatorDataBO.Instance.GetEmpresa(vo.Codemp).Descri.Trim();
            txtEmail.Text = vo.Email.Trim();
            txtMatricula.Text = vo.Matemp.Trim();
            txtNome.Text = vo.Nomusr.Trim();

			if (!string.IsNullOrEmpty(vo.Endere.Trim())) {
                txtComplemento.Text = vo.Comend.Trim();
                txtNumero.Text = vo.Nrend.Trim();
                txtCep.Text = FormatUtil.FormatCep(Int64.Parse(vo.Cepusr.Trim()));
                txtBairro.Text = vo.Bairro.Trim();
                txtEndereco.Text = vo.Endere.Trim();
                dpdUf.SelectedValue = vo.Estado.Trim();
                PopularMunicipios();
                dpdMunicipio.SelectedValue = vo.Codmun.Trim();
            }


            #region[TELEFONES]

            string ddd = "";
            if (vo.Ddd != null)
            {
                ddd = vo.Ddd.Trim();
            }

            if (vo.Telres != null)
            {
                txtTelResidencial.Text = FormatUtil.OnlyNumbers(vo.Telres.Trim());
            }
            
            if (!string.IsNullOrEmpty(txtTelResidencial.Text.Trim()))
            {
                txtDddResidencial.Text = ddd;
            }

            if (vo.Telcom != null)
            {
                txtTelComercial.Text = FormatUtil.OnlyNumbers(vo.Telcom.Trim());
            }
            
            if (!string.IsNullOrEmpty(txtTelComercial.Text.Trim()))
            {
                txtDddComercial.Text = ddd;
            }

            if (vo.Telefo != null)
            {
                txtTelCelular.Text = FormatUtil.OnlyNumbers(vo.Telefo.Trim());
            }
            
            if (!string.IsNullOrEmpty(txtTelCelular.Text.Trim()))
            {
                txtDddCelular.Text = ddd;
            }

            #endregion

            #region[DADOS BANCÁRIOS]

            /*
            dpdBanco.SelectedValue = Int32.Parse(fam.Bcocli.Trim()).ToString();

            // O sistema só consegue identificar o dígito verificador se ele encontrar a agência no ISA_HC, pois ela possui o dígito verificador separado
            HcAgenciaBancariaVO agenciaBancaria = PLocatorDataBO.Instance.GetAgenciaFuncionario(Int32.Parse(fam.Bcocli.Trim()), fam.Agecli.Trim());
            if (agenciaBancaria != null)
            {
                txtAgencia.Text = agenciaBancaria.CdAgencia.Trim();
                txtDvAgencia.Text = agenciaBancaria.DvAgencia.Trim();
            }
            else
            {
                string cd_agencia = "";
                string dv_agencia = "";

                if (fam.Agecli.Trim() != "" && fam.Agecli.Trim().Length >= 2)
                {
                    cd_agencia = fam.Agecli.Trim().Substring(0, fam.Agecli.Trim().Length - 1);
                    dv_agencia = fam.Agecli.Trim().Substring(fam.Agecli.Trim().Length - 1, 1);
                }

                txtAgencia.Text = cd_agencia;
                txtDvAgencia.Text = dv_agencia;   

            }

            string cd_conta = "";
            string dv_conta = "";

            if (fam.Ctacli.Trim() != "" && fam.Ctacli.Trim().Length >= 2)
            {
                cd_conta = fam.Ctacli.Trim().Substring(0, fam.Ctacli.Trim().Length - 1);
                dv_conta = fam.Ctacli.Trim().Substring(fam.Ctacli.Trim().Length - 1, 1);
            }

            txtConta.Text = cd_conta;
            txtDvConta.Text = dv_conta;
            */

            #endregion

        }

		private void PopularMunicipios() {
			DataTable dt = PLocatorDataBO.Instance.BuscarMunicipiosProtheus(dpdUf.SelectedValue);
			dpdMunicipio.DataSource = dt;
			dpdMunicipio.DataBind();
			dpdMunicipio.Items.Insert(0, new ListItem("SELECIONE", ""));
		}

		private bool ValidateRequired() {

			List<ItemError> lst = new List<ItemError>(); 
			if (string.IsNullOrEmpty(txtEmail.Text.Trim())) {
				this.AddErrorMessage(lst, txtEmail, "Informe o e-mail!");
			}
			if (string.IsNullOrEmpty(txtCep.Text.Trim())) {
				this.AddErrorMessage(lst, txtCep, "Informe o CEP!");
			}
            if (string.IsNullOrEmpty(txtEndereco.Text.Trim()))
            {
				this.AddErrorMessage(lst, txtEndereco, "Informe o endereço!");
			}
            if (string.IsNullOrEmpty(txtNumero.Text.Trim()))
            {
				this.AddErrorMessage(lst, txtNumero, "Informe o número do endereço!");
			}
            if (string.IsNullOrEmpty(txtBairro.Text.Trim()))
            {
				this.AddErrorMessage(lst, txtBairro, "Informe o bairro!");
			}
			if (string.IsNullOrEmpty(dpdUf.SelectedValue)) {
				this.AddErrorMessage(lst, dpdUf, "Informe a UF do endereço!");
			}
			if (string.IsNullOrEmpty(dpdMunicipio.SelectedValue)) {
				this.AddErrorMessage(lst, dpdMunicipio, "Informe o município!");
			}

            bool hasCelular = !string.IsNullOrEmpty(txtDddCelular.Text.Trim()) && !string.IsNullOrEmpty(txtTelCelular.Text.Trim());
            bool hasComercial = !string.IsNullOrEmpty(txtDddComercial.Text.Trim()) && !string.IsNullOrEmpty(txtTelComercial.Text.Trim());
            bool hasResidencial = !string.IsNullOrEmpty(txtDddResidencial.Text.Trim()) && !string.IsNullOrEmpty(txtTelResidencial.Text.Trim());
			if (!hasCelular && !hasComercial && !hasResidencial) {
				this.AddErrorMessage(lst, txtDddCelular, "Informe pelo menos um telefone de contato!");
			}

            /*
            if (string.IsNullOrEmpty(dpdBanco.SelectedValue) || string.IsNullOrEmpty(txtAgencia.Text) || string.IsNullOrEmpty(txtConta.Text))
            {
                AddErrorMessage(lst, dpdBanco, "Informe os dados bancários completos!");
            }
            */

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
			int iValue;
			if (!FormatUtil.IsValidEmail(txtEmail.Text.Trim())) {
				this.AddErrorMessage(lst, txtEmail, "Informe um e-mail válido!");
			}
            if (txtEmail.Text.Trim().Length > 60)
            {
                this.AddErrorMessage(lst, txtEmail, "O e-mail deve conter até 60 caracteres!");
            }			

			if (!Int32.TryParse(FormatUtil.UnformatCep(txtCep.Text.Trim()), out iValue)) {
				this.AddErrorMessage(lst, txtCep, "O CEP está num formato inválido!");
			}

			PCepVO vo = PLocatorDataBO.Instance.BuscarCepProtheus(iValue.ToString());
            if (vo == null) {
                this.AddErrorMessage(lst, txtCep, "CEP não encontrado!");
            }

            if (!string.IsNullOrEmpty(txtDddCelular.Text.Trim()))
            {
                if (!Int32.TryParse(txtDddCelular.Text.Trim(), out iValue))
                {
					this.AddErrorMessage(lst, txtDddCelular, "O DDD do Celular deve ser numérico!");
				}
			}
            if (!string.IsNullOrEmpty(txtDddComercial.Text.Trim()))
            {
                if (!Int32.TryParse(txtDddComercial.Text.Trim(), out iValue))
                {
					this.AddErrorMessage(lst, txtDddComercial, "O DDD do Telefone Comercial deve ser numérico!");
				}
			}
            if (!string.IsNullOrEmpty(txtDddResidencial.Text.Trim()))
            {
                if (!Int32.TryParse(txtDddResidencial.Text.Trim(), out iValue))
                {
					this.AddErrorMessage(lst, txtDddResidencial, "O DDD do Telefone Residencial deve ser numérico!");
				}
			}
            if (!string.IsNullOrEmpty(txtTelCelular.Text.Trim()))
            {
                if (!Int32.TryParse(txtTelCelular.Text.Trim(), out iValue))
                {
					this.AddErrorMessage(lst, txtTelCelular, "O Celular deve ser numérico!");
				}
			}
            if (!string.IsNullOrEmpty(txtTelComercial.Text.Trim()))
            {
                if (!Int32.TryParse(txtTelComercial.Text.Trim(), out iValue))
                {
					this.AddErrorMessage(lst, txtTelComercial, "O Telefone Comercial deve ser numérico!");
				}
			}
            if (!string.IsNullOrEmpty(txtTelResidencial.Text.Trim()))
            {
                if (!Int32.TryParse(txtTelResidencial.Text.Trim(), out iValue))
                {
					this.AddErrorMessage(lst, txtTelResidencial, "O Telefone Residencial deve ser numérico!");
				}
			}

            /*
            // Validação da agência bancária no ISA
            HcAgenciaBancariaVO agenciaBancaria = PLocatorDataBO.Instance.GetAgenciaDv(Int32.Parse(dpdBanco.SelectedValue), txtAgencia.Text.Trim().ToUpper(), txtDvAgencia.Text.Trim().ToUpper());
            if (agenciaBancaria == null)
            {
                this.AddErrorMessage(lst, txtAgencia, "Agência Bancária não localizada em nossa base de dados. Favor enviar um e-mail para cadastro@e-vida.org.br informando Banco, Agência e Conta Corrente para que seja realizada a atualização dos seus dados.");
            }
            */

			if (lst.Count > 0) {
				this.ShowErrorList(lst);
				return false;
			}

			return true;
		}

		private void Salvar() {
			if (!ValidateFields())
				return;

            #region[USUÁRIO]

            eVidaGeneralLib.VO.Protheus.PUsuarioVO vo = new eVidaGeneralLib.VO.Protheus.PUsuarioVO();
			vo.Codint = UsuarioLogado.UsuarioTitular.Codint.Trim();
            vo.Codemp = UsuarioLogado.UsuarioTitular.Codemp.Trim();
            vo.Matric = UsuarioLogado.UsuarioTitular.Matric.Trim();
            vo.Tipreg = UsuarioLogado.UsuarioTitular.Tipreg.Trim();

            vo.Cepusr = FormatUtil.UnformatCep(txtCep.Text);
            vo.Codmun = dpdMunicipio.SelectedValue;
            vo.Bairro = txtBairro.Text.Trim();
            vo.Endere = txtEndereco.Text.Trim();
            vo.Munici = dpdMunicipio.SelectedItem.Text;
            vo.Estado = dpdUf.SelectedValue;
            vo.Nrend = txtNumero.Text.Trim();
            vo.Comend = txtComplemento.Text.Trim();

			vo.Email = txtEmail.Text.Trim();

            if(!string.IsNullOrEmpty(txtDddCelular.Text.Trim()))
            {
                vo.Ddd = txtDddCelular.Text.Trim();
            }
            else if (!string.IsNullOrEmpty(txtDddComercial.Text.Trim()))
            {
                vo.Ddd = txtDddComercial.Text.Trim();
            }
            else if (!string.IsNullOrEmpty(txtDddResidencial.Text.Trim()))
            {
                vo.Ddd = txtDddResidencial.Text.Trim();
            }

            vo.Telefo = txtTelCelular.Text.Trim();
            vo.Telcom = txtTelComercial.Text.Trim();
            vo.Telres = txtTelResidencial.Text.Trim();

            PUsuarioBO.Instance.SalvarDadosPessoais(vo);

            #endregion

            #region[VIDA]

            eVidaGeneralLib.VO.Protheus.PVidaVO vida = new eVidaGeneralLib.VO.Protheus.PVidaVO();
            vida.Matvid = UsuarioLogado.UsuarioTitular.Matvid.Trim();

            vida.Cepusr = FormatUtil.UnformatCep(txtCep.Text);
            vida.Codmun = dpdMunicipio.SelectedValue;
            vida.Bairro = txtBairro.Text.Trim();
            vida.Endere = txtEndereco.Text.Trim();
            vida.Munici = dpdMunicipio.SelectedItem.Text;
            vida.Estado = dpdUf.SelectedValue;
            vida.Nrend = txtNumero.Text.Trim();
            vida.Comend = txtComplemento.Text.Trim();

            vida.Email = txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(txtDddCelular.Text.Trim()))
            {
                vida.Ddd = txtDddCelular.Text.Trim();
            }
            else if (!string.IsNullOrEmpty(txtDddComercial.Text.Trim()))
            {
                vida.Ddd = txtDddComercial.Text.Trim();
            }
            else if (!string.IsNullOrEmpty(txtDddResidencial.Text.Trim()))
            {
                vida.Ddd = txtDddResidencial.Text.Trim();
            }

            vida.Telefo = txtTelCelular.Text.Trim();
            vida.Telcom = txtTelComercial.Text.Trim();
            vida.Telres = txtTelResidencial.Text.Trim();

            PVidaBO.Instance.SalvarDadosContato(vida);

            #endregion

            #region[FAMÍLIA]

            PFamiliaVO familia = PFamiliaBO.Instance.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric);

            /*
            familia.Bcocli = Int32.Parse(dpdBanco.SelectedValue).ToString().PadLeft(3, '0');
            familia.Agecli = txtAgencia.Text.Trim().ToUpper() + txtDvAgencia.Text.Trim().ToUpper();
            familia.Ctacli = txtConta.Text.Trim().ToUpper() + txtDvConta.Text.Trim().ToUpper();

            // Apenas o usuário titular pode alterar os dados bancários
            if(UsuarioLogado.Usuario.Tipusu == "T"){
                PFamiliaBO.Instance.AtualizarDadosBancarios(familia);
            }
            */
            
            familia.Cep = FormatUtil.UnformatCep(txtCep.Text);
            familia.Codmun = dpdMunicipio.SelectedValue;
            familia.Bairro = txtBairro.Text.Trim();
            familia.End = txtEndereco.Text.Trim();
            familia.Mun = dpdMunicipio.SelectedItem.Text;
            familia.Estado = dpdUf.SelectedValue;
            familia.Numero = txtNumero.Text.Trim();
            familia.Comple = txtComplemento.Text.Trim();

            PFamiliaBO.Instance.SalvarDadosContato(familia);

            #endregion

            #region[CLIENTE]

            eVidaGeneralLib.VO.Protheus.PClienteVO cliente = new eVidaGeneralLib.VO.Protheus.PClienteVO();
            cliente.Cod = familia.Codcli;

            cliente.Cep = FormatUtil.UnformatCep(txtCep.Text);
            cliente.Codmun = dpdMunicipio.SelectedValue;
            cliente.Bairro = txtBairro.Text.Trim();
            cliente.End = txtEndereco.Text.Trim();
            cliente.Mun = dpdMunicipio.SelectedItem.Text;
            cliente.Est = dpdUf.SelectedValue;
            cliente.Nrend = txtNumero.Text.Trim();
            cliente.Complem = txtComplemento.Text.Trim();
            cliente.Email = txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(txtDddCelular.Text.Trim()))
            {
                cliente.Ddd = txtDddCelular.Text.Trim();
                cliente.Tel = txtTelCelular.Text.Trim();
            }
            else if (!string.IsNullOrEmpty(txtDddComercial.Text.Trim()))
            {
                cliente.Ddd = txtDddComercial.Text.Trim();
                cliente.Tel = txtTelComercial.Text.Trim();
            }
            else if (!string.IsNullOrEmpty(txtDddResidencial.Text.Trim()))
            {
                cliente.Ddd = txtDddResidencial.Text.Trim();
                cliente.Tel = txtTelResidencial.Text.Trim();
            }

            PClienteBO.Instance.SalvarDadosContato(cliente);

            #endregion

            #region[BENEFICIÁRIO LOGADO]

            PUsuarioVO logado = PUsuarioBO.Instance.GetUsuario(UsuarioLogado.Codint, UsuarioLogado.Codemp, UsuarioLogado.Matric, UsuarioLogado.Tipreg);

            eVidaGeneralLib.VO.Protheus.PUsuarioPortalVO portal = new eVidaGeneralLib.VO.Protheus.PUsuarioPortalVO();
            portal.Logusr = logado.Cpfusr.Trim();
            portal.Email = txtEmail.Text.Trim();

            PUsuarioPortalBO.Instance.SalvarDadosContato(portal);

            #endregion

            this.ShowInfo("Dados salvos com sucesso!");

			Bind();
		}

		protected void txtCep_TextChanged(object sender, EventArgs e) {
			try {
				int cep;
				Int32.TryParse(FormatUtil.UnformatCep(txtCep.Text), out cep);
				PCepVO vo = PLocatorDataBO.Instance.BuscarCepProtheus(cep.ToString());
				if (vo == null) {
					this.ShowError("CEP não encontrado!");
					return;
				}
				Bind(vo);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar cep! " + ex.Message);
				Log.Error("Erro ao buscar cep.", ex);
			}
		}

        private void Bind(PCepVO vo)
        {
            txtBairro.Text = vo.Bairro;
            txtEndereco.Text = vo.End;
            dpdUf.SelectedValue = vo.Est;
            PopularMunicipios();
            dpdMunicipio.SelectedValue = vo.Codmun;

            DesabilitarEndereco();
        }

		protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				PopularMunicipios();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao buscar municípios! " + ex.Message);
				Log.Error("Erro ao buscar municípios.", ex);
			}
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				Salvar();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao salvar dados! " + ex.Message);
				Log.Error("Erro ao salvar dados.", ex);			
			}
		}

        /*
        protected void txtConta_TextChanged(object sender, EventArgs e)
        {
            this.ShowInfo("Atenção! A conta informada neste campo deve ser do tipo CONTA CORRENTE. Caso informe os dados de CONTA POUPANÇA, a E-VIDA não poderá efetuar créditos de Reembolso.");
        }

        protected void txtDvConta_TextChanged(object sender, EventArgs e)
        {
            this.ShowInfo("Atenção! A conta informada neste campo deve ser do tipo CONTA CORRENTE. Caso informe os dados de CONTA POUPANÇA, a E-VIDA não poderá efetuar créditos de Reembolso.");
        }	
        */

        private void DesabilitarEndereco() 
        {
            txtBairro.Enabled = true;
            txtEndereco.Enabled = true;
            dpdUf.Enabled = true;
            dpdMunicipio.Enabled = true;

            if (!string.IsNullOrEmpty(txtBairro.Text.Trim()))
            {
                txtBairro.Enabled = false;
            }
            if (!string.IsNullOrEmpty(txtEndereco.Text.Trim()))
            {
                txtEndereco.Enabled = false;
            }
            if(!string.IsNullOrEmpty(dpdUf.SelectedValue))
            {
                dpdUf.Enabled = false;
            }
            if (!string.IsNullOrEmpty(dpdMunicipio.SelectedValue))
            {
                dpdMunicipio.Enabled = false;
            }
        }

	}
}