using eVida.Web.Report;
using eVidaBeneficiarios.Classes.CanalGestante;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
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

namespace eVidaBeneficiarios.CanalGestante {
	public partial class CanalGestante : PageBase {

		protected override void PageLoad(object sender, EventArgs e) {
			/*ScriptManager.GetCurrent(this).RegisterPostBackControl(btnCartaoInfo);
			ScriptManager.GetCurrent(this).RegisterPostBackControl(btnCartaoGest);
			ScriptManager.GetCurrent(this).RegisterPostBackControl(btnPartograma);*/
			if (!IsPostBack) {
				BindFixos();

				PUsuarioVO benefVO = UsuarioLogado.Usuario;
				Bind(benefVO.Codint, benefVO.Codemp, benefVO.Matric, benefVO.Tipreg);
			}
		}

		private void BindFixos() {
			int ano = CanalGestanteBO.Instance.CalcularAno(DateTime.Now.Date);

			List<string> lst = CanalGestanteBO.Instance.ListarUfConfig(ano);
			dpdUf.DataSource = lst;
			dpdUf.DataBind();
			dpdUf.Items.Insert(0, new ListItem("SELECIONE", ""));

			BindUf(string.Empty);

		}

		private void BindUf(string uf) {
			dpdEstabelecimento.Items.Clear();
			dpdMedico.Items.Clear();

			if (!string.IsNullOrEmpty(uf)) {
				BindCredMedico(uf);
			} else {
				dpdEstabelecimento.Items.Add(new ListItem("SELECIONE A UF", ""));
				dpdMedico.Items.Add(new ListItem("SELECIONE A UF", ""));
				dpdEstabelecimento.Enabled = false;
				dpdMedico.Enabled = false;
			}
		}

		private void BindCredMedico(string uf) {
			int ano = CanalGestanteBO.Instance.CalcularAno(DateTime.Now.Date);

			List<CanalGestanteConfigCredVO> lstConfigCred = CanalGestanteBO.Instance.GetConfigCred(ano);
			List<CanalGestanteConfigProfVO> lstConfigProf = CanalGestanteBO.Instance.GetConfigProf(ano);

			List<PRedeAtendimentoVO> lstCredenciados = CanalGestanteBO.Instance.ListarInfoCredenciadoConfig(ano);
			List<PProfissionalSaudeVO> lstProfissional = CanalGestanteBO.Instance.ListarInfoProfissionalConfig(ano);

			if (lstConfigCred != null) {
				if (!string.IsNullOrEmpty(uf)) {
					lstConfigCred.RemoveAll(x => !x.Uf.Equals(uf, StringComparison.InvariantCultureIgnoreCase));
				}
				if (lstCredenciados != null) {
					lstCredenciados.RemoveAll(x => !lstConfigCred.Exists(y => y.CodCredenciado == x.Codigo));
					lstCredenciados.Sort((x, y) => x.Nome.CompareTo(y.Nome));
				}
			} else {
				lstCredenciados = new List<PRedeAtendimentoVO>();
			}

			if (lstConfigProf != null) {
				if (!string.IsNullOrEmpty(uf)) {
					lstConfigProf.RemoveAll(x => !x.Estado.Equals(uf, StringComparison.InvariantCultureIgnoreCase));
				}
				if (lstProfissional != null) {
					lstProfissional.RemoveAll(x => !lstConfigProf.Exists(y => y.Codigo == x.Codigo));
					lstProfissional.Sort((x, y) => x.Nome.CompareTo(y.Nome));
				}
			} else {
				lstProfissional = new List<PProfissionalSaudeVO>();
			}
			
			dpdEstabelecimento.DataSource = lstCredenciados;
			dpdEstabelecimento.DataBind();
			if (dpdEstabelecimento.Items.Count > 0) {
				dpdEstabelecimento.Items.Insert(0, new ListItem("SELECIONE", ""));
				dpdEstabelecimento.Enabled = true;
			} else {
				dpdEstabelecimento.Items.Insert(0, new ListItem("NÃO EXISTEM ESTABELECIMENTOS PARA A UF!"));
				dpdEstabelecimento.Enabled = false;
			}

			dpdMedico.DataSource = lstProfissional.Select(x => new PProfissionalSaudeVO() {
				Codigo = x.Codigo,
				Nome = x.Nome + " (" + x.Codsig  + " - " + x.Numcr +  ") "
			});
			dpdMedico.DataBind();
			if (dpdMedico.Items.Count > 0) {
				dpdMedico.Items.Insert(0, new ListItem("SELECIONE", ""));
				dpdMedico.Enabled = true;
			} else {
				dpdMedico.Items.Insert(0, new ListItem("NÃO EXISTEM MÉDICOS PARA A UF!"));
				dpdMedico.Enabled = false;
			}
		}

		private void Bind(string codint, string codemp, string matric, string tipreg) {
			CanalGestanteBenefVO infoBenefVO = CanalGestanteBO.Instance.GetInfoBenef(codint, codemp, matric, tipreg);

			if (infoBenefVO != null) {
				txtEmail.Text = infoBenefVO.Email;
				txtTelefone.Text = infoBenefVO.Telefone;
			}
            BindSolicitacoes(codint, codemp, matric, tipreg);
		}

		private void BindSolicitacoes(string codint, string codemp, string matric, string tipreg) {
			IEnumerable<CanalGestanteVO> lstProtocolo = CanalGestanteBO.Instance.ListarProtocolosBenef(codint, codemp, matric, tipreg);
			if (lstProtocolo != null) {
				lstProtocolo = lstProtocolo.Where(x => x.Status != StatusCanalGestante.GERANDO);
				lstProtocolo = lstProtocolo.OrderByDescending(x => x.Id);
			}
			gdvSolAnterior.DataSource = lstProtocolo;
			gdvSolAnterior.DataBind();
		}

		private void CartaInformacao() {
			string appPath = Request.PhysicalApplicationPath;
			string path = System.IO.Path.Combine(appPath, "CanalGestante", "CartaInformacao.pdf");
			if (System.IO.File.Exists(path)) {
				CanalGestanteBenefVO vo = SalvarInfo(false);
				if (vo != null) {
					
					/*
					string contentType = MimeMapping.GetMimeMapping("CartaInformacao.pdf");
					Response.BufferOutput = true;
					Response.Clear();
					Response.AddHeader("content-disposition", "attachment;filename=\"" + "CartaInformacao.pdf" + "\"");
					Response.ContentType = contentType;
					Response.WriteFile(path);*/
					this.RegisterScript("carta", "openLocalPdf('" + CanalGestanteBenefVO.CARTA_INFO + "');");
				}
			} else {
				this.ShowError("Arquivo não encontrado!");
				return;
			}
			
		}

		private void CartaoGestante() {
			string appPath = Request.PhysicalApplicationPath;
			string path = System.IO.Path.Combine(appPath, "CanalGestante", "CartaoGestante.pdf");
			if (System.IO.File.Exists(path)) {
				CanalGestanteBenefVO vo = SalvarInfo(false);
				if (vo != null) {
					this.RegisterScript("cartao", "openLocalPdf('" + CanalGestanteBenefVO.CARTAO_GES + "');");
				}
			} else {
				this.ShowError("Arquivo não encontrado!");
				return;
			}
		}

		private void Partograma() {
			string appPath = Request.PhysicalApplicationPath;
			string path = System.IO.Path.Combine(appPath, "CanalGestante", "Partograma.pdf");
			if (System.IO.File.Exists(path)) {
				CanalGestanteBenefVO vo = SalvarInfo(false);
				if (vo != null) {
					this.RegisterScript("cartao", "openLocalPdf('" + CanalGestanteBenefVO.PARTOGRAMA + "');");
				}
			} else {
				this.ShowError("Arquivo não encontrado!");
				return;
			}

		}

		private void GerarProtocolo() {
			CanalGestanteBenefVO benefVO = SalvarInfo(false);
			if (benefVO == null) return;

			if (string.IsNullOrEmpty(dpdMedico.SelectedValue) && string.IsNullOrEmpty(dpdEstabelecimento.SelectedValue)) {
				this.ShowError("Selecione pelo menos um estabelecimento e/ou médico!");
				return;
			}

			string cdCredenciado = null;
			string nrSeqProfissional = null;
			
			if (!string.IsNullOrEmpty(dpdEstabelecimento.SelectedValue))
				cdCredenciado = dpdEstabelecimento.SelectedValue;
			if (!string.IsNullOrEmpty(dpdMedico.SelectedValue))
				nrSeqProfissional = dpdMedico.SelectedValue;

            CanalGestanteVO vo = CanalGestanteBO.Instance.GerarProtocolo(benefVO.Codint.Trim(), benefVO.Codemp.Trim(), benefVO.Matric.Trim(), benefVO.Tipreg.Trim(), cdCredenciado.Trim(), nrSeqProfissional.Trim());
			ReportSolicitacaoCanalGestante rpt = new ReportSolicitacaoCanalGestante(ReportDir, UsuarioLogado);
			byte[] relatorio = rpt.GerarRelatorio(vo);
			CanalGestanteBO.Instance.SalvarProtocolo(vo, relatorio);
			try {
				string nome;
				string path = CanalGestanteBO.Instance.GetFilePath(vo, out nome);
				CanalGestanteBO.Instance.EnviarEmail(vo, path);
			} catch (Exception ex) {
				this.ShowError("Erro ao enviar protocolo por email!", ex);
			}
			this.RegisterScript("protocolo", "openProtocolo('" + vo.Id + "');");
		}

		private void RegerarProtocolo(int id) {
			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(id);
			if (vo == null) {
				this.ShowError("Protocolo não encontrado!");
				return;
			}
			string nome;
			string path = CanalGestanteBO.Instance.GetFilePath(vo, out nome);
			if (!System.IO.File.Exists(path)) {
				this.ShowError("Arquivo não encontrado no servidor. Entre em contato com o suporte técnico!");
				return;
			}

			this.RegisterScript("protocolo", "openLocalPdf('PROTOCOLO', " + id + ");");
		}

		private void EnviarPorEmail(int id) {
			CanalGestanteVO vo = CanalGestanteBO.Instance.GetById(id);
			if (vo == null) {
				this.ShowError("Protocolo não encontrado!");
				return;
			}
			string nome;
			string path = CanalGestanteBO.Instance.GetFilePath(vo, out nome);
			if (!System.IO.File.Exists(path)) {
				this.ShowError("Arquivo não encontrado no servidor. Entre em contato com o suporte técnico!");
				return;
			}

			CanalGestanteBO.Instance.EnviarEmail(vo, path);
			this.ShowError("Email enviado com sucesso!");
		}

		private CanalGestanteBenefVO SalvarInfo(bool refresh = true) {
			if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtTelefone.Text)) {
				this.ShowError("Informe e-mail e telefone para contato!");
				return null;
			}

			CanalGestanteBenefVO infoBenefVO = new CanalGestanteBenefVO();
			PUsuarioVO benefVO = UsuarioLogado.Usuario;
			PFamiliaProdutoVO benefPlano = UsuarioLogado.FamiliaProduto;

			infoBenefVO.CodAlternativo = benefVO.Matant;
			infoBenefVO.Codint = benefVO.Codint;
            infoBenefVO.Codemp = benefVO.Codemp;
            infoBenefVO.Matric = benefVO.Matric;
            infoBenefVO.Tipreg = benefVO.Tipreg;
			infoBenefVO.DataNascimento = DateTime.ParseExact(benefVO.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture);
			infoBenefVO.Email = txtEmail.Text;
			infoBenefVO.PlanoVinculado = benefPlano.Codpla;
			infoBenefVO.Telefone = txtTelefone.Text;

			CanalGestanteBO.Instance.Salvar(infoBenefVO);
			if (refresh) {
				this.ShowInfo("Informações atualizadas com sucesso!");

				Bind(benefVO.Codint, benefVO.Codemp, benefVO.Matric, benefVO.Tipreg);
			}
			return infoBenefVO;
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			try {
				SalvarInfo();
			} catch (Exception ex) {
				this.ShowError("Erro ao salvar.", ex);
			}
		}

		protected void btnCartaoInfo_Click(object sender, EventArgs e) {
			try {
				CartaInformacao();
			} catch (Exception ex) {
				this.ShowError("Erro ao solicitar carta de informação!", ex);
			}
		}

		protected void btnCartaoGest_Click(object sender, EventArgs e) {
			try {
				CartaoGestante();
			} catch (Exception ex) {
				this.ShowError("Erro ao solicitar cartão gestante!", ex);
			}
		}

		protected void btnPartograma_Click(object sender, EventArgs e) {
			try {
				Partograma();
			} catch (Exception ex) {
				this.ShowError("Erro ao solicitar partograma!", ex);
			}
		}

		protected void btnRefresh_Click(object sender, EventArgs e) {
            BindSolicitacoes(UsuarioLogado.Usuario.Codint, UsuarioLogado.Usuario.Codemp, UsuarioLogado.Usuario.Matric, UsuarioLogado.Usuario.Tipreg);
		}

		protected void btnSolicitar_Click(object sender, EventArgs e) {
			try {
				GerarProtocolo();
			} catch (Exception ex) {
				this.ShowError("Erro ao gerar solicitação!", ex);
			}
		}

		protected void gdvSolAnterior_RowDataBound(object sender, GridViewRowEventArgs e) {
			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.DataRow) {
				Literal ltProtocolo = (Literal)row.FindControl("ltProtocolo");
				Literal ltStatus = (Literal)row.FindControl("ltStatus");

				CanalGestanteVO vo = (CanalGestanteVO)row.DataItem;
				ltProtocolo.Text = vo.Id.ToString() + "/" + vo.DataSolicitacao.Year.ToString();
				ltStatus.Text = CanalGestanteEnumTradutor.TraduzStatus(vo.Status);
			}
		}

		protected void btnRegerar_Click(object sender, ImageClickEventArgs e) {
			try {
				int id;
				ImageButton btn = (ImageButton)sender;
				if (!Int32.TryParse(btn.CommandArgument, out id)) {
					this.ShowError("ID inválido!");
					return;
				}
				RegerarProtocolo(id);
			} catch (Exception ex) {
				this.ShowError("Erro ao obter arquivo gerado!", ex);
			}
		}

		protected void btnEmail_Click(object sender, ImageClickEventArgs e) {
			try {
				int id;
				ImageButton btn = (ImageButton)sender;
				if (!Int32.TryParse(btn.CommandArgument, out id)) {
					this.ShowError("ID inválido!");
					return;
				}
				EnviarPorEmail(id);
			} catch (Exception ex) {
				this.ShowError("Erro ao obter arquivo gerado!", ex);
			}
		}

		protected void dpdUf_SelectedIndexChanged(object sender, EventArgs e) {
			try {
				BindUf(dpdUf.SelectedValue);
			} catch (Exception ex) {
				this.ShowError("Erro ao pesquisar pela UF.", ex);
			}
		}

	}
}