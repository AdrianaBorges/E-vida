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
	public partial class ViewReciprocidade : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {

			try {
				if (!IsPostBack) {
					int id = 0;
					if (!Int32.TryParse(Request["ID"], out id)) {
						this.ShowError("Solicitação inválida!");
						return;
					}

					ReciprocidadeVO vo = Buscar(id);
					if (vo == null) {
						this.ShowError("Solicitação não encontrada!");
						return;
					}
					DataView dtBeneficiarios = BuildTableBeneficiarios(vo);
					DataTable dtAssistencia = BuildTableAssistencia(vo);

					POperadoraSaudeVO empresa = null;
                    if (!string.IsNullOrEmpty(vo.CodintReciprocidade)) {
                        if (!string.IsNullOrEmpty(vo.CodintReciprocidade.Trim()))
                        {
                            empresa = ReciprocidadeBO.Instance.GetOperadoraById(vo.CodintReciprocidade.Trim());
                        }
                    }
					
					dlBeneficiarios.DataSource = dtBeneficiarios;
					dlBeneficiarios.DataBind();

					repAssistencias.DataSource = dtAssistencia;
					repAssistencias.DataBind();

					lblData.Text = FormatUtil.FormatDataExtenso(vo.DataCriacao);

                    PFamiliaVO famVO = PFamiliaBO.Instance.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric);
                    PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);
                    PVidaVO vida = PVidaBO.Instance.GetVida(titular.Matvid);

					lblTitular.Text = titular.Nomusr;
					lblCartao.Text = titular.GetCarteira();
                    lblNascimento.Text = DateUtil.FormatDateYMDToDMY(titular.Datnas);
                    lblCpf.Text = titular.Cpfusr != null ? FormatUtil.FormatCpf(titular.Cpfusr) : "";
					lblEstadoCivil.Text = PLocatorDataBO.Instance.GetItemLista(PConstantes.LISTA_ESTADO_CIVIL_P, titular.Estciv);
					lblNomeMae.Text = titular.Mae;
					lblCns.Text = vida.Nrcrna;

					if (empresa != null) {
						lblEmpresa.Text = empresa.Nomint.Trim();
                        lblTelefone.Text = !string.IsNullOrEmpty(empresa.Telef1.Trim()) ? empresa.Telef1.Trim() : "";
                        lblFax.Text = !string.IsNullOrEmpty(empresa.Fax1.Trim()) ? empresa.Fax1.Trim() : "";
					}

					lblMunicipio.Text = vo.Endereco.Cidade;
					lblUf.Text = vo.Endereco.Uf;

					lblDataInicio.Text = vo.Inicio.ToString("dd/MM/yyyy");
					lblDataFim.Text = vo.Fim.ToString("dd/MM/yyyy");

					lblObs.Text = vo.Observacao;

					ViewState["STATUS"] = vo.Status;
					
					UsuarioVO usuarioAprov = null;
					if (vo.Status == StatusReciprocidade.APROVADO) {
						usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAprovacao);
					} else if (vo.Status == StatusReciprocidade.ENVIADO) {
						usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioEnvio);
					} else {
						usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAlteracao);
					}
					if (usuarioAprov != null) {
						litCargo.Text = usuarioAprov.Cargo;
						litUsuario.Text = usuarioAprov.Nome;
						imgAssinatura.ImageUrl = "../download.evida?TIPO=" + FileUtil.FileDir.ASSINATURA + "&ID=" + FormatUtil.ToBase64String(usuarioAprov.Login);
					} else {
						if (vo.Status != StatusReciprocidade.PENDENTE) {
							this.ShowError("Não foi possível identificar o usuário de aprovação/envio/alteração!");
						}
					}

				}
			}
			catch (Exception ex) {
				this.ShowError("Erro ao abrir a tela!", ex);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.RECIPROCIDADE_VIEW; }
		}

		private static DataTable BuscarBeneficiarios(ReciprocidadeVO vo) {
			return ReciprocidadeBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
		}

		private static ReciprocidadeVO Buscar(int id) {
			ReciprocidadeVO vo = ReciprocidadeBO.Instance.GetById(id);
			return vo;
		}

		protected void dlBeneficiarios_ItemDataBound(object sender, DataListItemEventArgs e) {
			DataListItem row = e.Item;
			if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem) {
				Label lblNome = row.FindControl("lblNome") as Label;
				Label lblNascimento = row.FindControl("lblNascimento") as Label;
				Label lblParentesco = row.FindControl("lblParentesco") as Label;
				Label lblCpf = row.FindControl("lblCpf") as Label;
				Label lblNomeMae = row.FindControl("lblNomeMae") as Label;
				Label lblCns = row.FindControl("lblCns") as Label;

				DataRowView data = row.DataItem as DataRowView;

				lblNome.Text = Convert.ToString(data["BA1_NOMUSR"]);
                lblNascimento.Text = data["BA1_DATNAS"] != DBNull.Value ? DateUtil.FormatDateYMDToDMY(Convert.ToString(data["BA1_DATNAS"])) : "";
				lblCpf.Text = data["BA1_CPFUSR"] != DBNull.Value ? FormatUtil.FormatCpf(Convert.ToString(data["BA1_CPFUSR"])) : "";
				lblNomeMae.Text = Convert.ToString(data["BA1_MAE"]);
                lblParentesco.Text = Convert.ToString(data["BRP_DESCRI"]);
                lblCns.Text = Convert.ToString(data["BTS_NRCRNA"]);
			}
		}

		private static DataView BuildTableBeneficiarios(ReciprocidadeVO vo) {

			DataTable dtBeneficiarios = BuscarBeneficiarios(vo);
			DataView dv = new DataView(dtBeneficiarios);
			dv.RowFilter = "BA1_TIPUSU <> '" + PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO + "'";

			return dv;
		}

		private static DataTable BuildTableAssistencia(ReciprocidadeVO vo) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");

			if (vo.Assistencia != null) {
				foreach (int idAssistencia in vo.Assistencia) {

					DataRow drN = dt.NewRow();
					drN["Nome"] = ReciprocidadeEnumTradutor.TraduzAssistencia(idAssistencia);
					dt.Rows.Add(drN);
				}
			}
			return dt;
		}

	}
}