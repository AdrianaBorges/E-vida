using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using eVida.Web.Report;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Forms {
	public partial class PopAutorizacaoAprov : PopUpPageBase {
		protected override void PageLoad(object sender, EventArgs e) {
			int cdProtocolo;
			if (!Int32.TryParse(Request["ID"], out cdProtocolo)) {
				this.ShowError("A requisição está inválida!");
				return;
			}

			hidRnd.Value = cdProtocolo.ToString();

			if (!IsPostBack) {
				List<AutorizacaoTissVO> lst = new List<AutorizacaoTissVO>();
				lst.Add(new AutorizacaoTissVO());
				BindList(lst);
			}
		}

		protected override eVidaGeneralLib.VO.Modulo Modulo {
			get { return eVidaGeneralLib.VO.Modulo.AUTORIZACAO; }
		}

		private void BindList(List<AutorizacaoTissVO> lst) {
			ltvAutorizacao.DataSource = lst;
			ltvAutorizacao.DataBind();

			string js = "var upls = new Array(); ";
			Random r = new Random();
			foreach (ListViewItem item in ltvAutorizacao.Items) {
				if (item.ItemType == ListViewItemType.DataItem) {
					AutorizacaoTissVO vo = lst[item.DataItemIndex];

					string inputFile = "fileUpload_" + item.DataItemIndex;
					ImageButton btnRemover = (ImageButton)item.FindControl("btnRemover");
					TextBox txtNroAutorizacao = (TextBox)item.FindControl("txtNroAutorizacao");
					Label lblFile = (Label)item.FindControl("file");
					HtmlInputHidden fileName = (HtmlInputHidden)item.FindControl("fileName");
					HtmlInputHidden ofileName = (HtmlInputHidden)item.FindControl("ofileName");

					lblFile.Text = vo.NomeArquivo;
					fileName.Value = vo.NomeArquivo;
					if (vo.NrAutorizacaoTiss != 0)
						txtNroAutorizacao.Text = vo.NrAutorizacaoTiss.ToString();
					if (vo.CodAutorizacao == 0)
						vo.CodAutorizacao = r.Next(9999);

					js += "upls.push({ prefix: '" + vo.CodAutorizacao + "', inputFile: '" + inputFile + "', lblFile: '" + lblFile.ClientID + "', lblFileName: '" + fileName.ClientID + "'}); ";
				}
			}
			this.RegisterScript("cfg", js);
		}

		private List<AutorizacaoTissVO> GetList(bool validate, out bool valid) {
			bool ok = true;
			List<AutorizacaoTissVO> lst = new List<AutorizacaoTissVO>();
			foreach (ListViewItem item in ltvAutorizacao.Items) {
				if (item.ItemType == ListViewItemType.DataItem) {
					ImageButton btnRemover = (ImageButton)item.FindControl("btnRemover");
					TextBox txtNroAutorizacao = (TextBox)item.FindControl("txtNroAutorizacao");
					Label lblFile = (Label)item.FindControl("file");
					HtmlInputHidden fileName = (HtmlInputHidden)item.FindControl("fileName");
					HtmlInputHidden ofileName = (HtmlInputHidden)item.FindControl("ofileName");

					int nroAutorizacaoTiss = 0;
					string arquivo = (!fileName.Value.Equals(ofileName.Value)) ? fileName.Value : "";
					lblFile.Text = arquivo;

					if (validate) {
						if (string.IsNullOrEmpty(txtNroAutorizacao.Text)) {
							this.ShowError("Informe o número de autorização TISS");
							if (ok) this.SetFocus(txtNroAutorizacao);
							ok = false;
						} else if (!Int32.TryParse(txtNroAutorizacao.Text, out nroAutorizacaoTiss)) {
							this.ShowError("O número de autorização TISS deve ser numérico!");
							if (ok) this.SetFocus(txtNroAutorizacao);
							ok = false;
						}

						if (string.IsNullOrEmpty(arquivo)) {
							this.ShowError("Informe o arquivo gerado no ISA a ser anexado!");
							if (ok) this.SetFocus(txtNroAutorizacao);
							ok = false;
						}
					} else {
						if (!Int32.TryParse(txtNroAutorizacao.Text, out nroAutorizacaoTiss)) {
							ok = false;
						}
					}

					AutorizacaoTissVO vo = new AutorizacaoTissVO();
					vo.NomeArquivo = arquivo;
					vo.NrAutorizacaoTiss = nroAutorizacaoTiss;
					lst.Add(vo);
				}
			}
			valid = ok;
			return lst;
		}

		private void IncluirNovaLinha() {
			bool ok = true;
			List<AutorizacaoTissVO> lst = GetList(true, out ok);
			if (!ok) return;

			AutorizacaoTissVO vo = new AutorizacaoTissVO();
			vo.NrAutorizacaoTiss = lst[lst.Count-1].NrAutorizacaoTiss;
			lst.Add(vo);
			BindList(lst);
		}

		private void RemoverLinha(int idx) {
			bool ok = true;
			List<AutorizacaoTissVO> lst = GetList(false, out ok);

			lst.RemoveAt(idx);
			BindList(lst);
		}

		protected void btnSalvar_Click(object sender, EventArgs e) {
			int id = Int32.Parse(Request["ID"]);

			bool ok = true;
			List<AutorizacaoTissVO> lst = GetList(true, out ok);
			if (!ok)
				return;

			TextBox txtObs = (TextBox)ltvAutorizacao.FindControl("txtObs");

			AutorizacaoBO.Instance.Aprovar(id, lst, txtObs.Text, UsuarioLogado.Id);

			this.RegisterScript("aqui", "setSolicitacao(" + id + ")");
		}

		protected void btnIncluir_Click(object sender, EventArgs e) {
			try {
				IncluirNovaLinha();
			}
			catch (Exception ex) {
				this.ShowError("Erro ao incluir nova linha!", ex);
			}
		}

		protected void btnRemover_Click(object sender, ImageClickEventArgs e) {
			try {
				int idx = Convert.ToInt32(((ImageButton)sender).CommandArgument);
				RemoverLinha(idx);
			}
			catch (Exception ex) {
				this.ShowError("Erro ao remover linha!", ex);
			}
		}

	}
}