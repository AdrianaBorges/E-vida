using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO;
using eVidaIntranet.Classes;

namespace eVidaIntranet.Reuniao {
	public partial class Calendario : PageBase {
		private List<ReuniaoVO> _lstReuniao;

		protected override void PageLoad(object sender, EventArgs e) {
			if (!IsPostBack) {
				List<ConselhoVO> lstConselhos = ConselhoBO.Instance.ListarConselhos();
				chkLstOrgao.DataSource = lstConselhos;
				chkLstOrgao.DataBind();


				ConselhoVO conselho = ConselhoBO.Instance.GetConselhoByUsuario(UsuarioLogado.Id);
				if (conselho == null) {
					//this.ShowError("Você deve pertencer a um conselho ou ter permissão para gerenciar reunião para visualizar o Calendário!");
					//return;
				} else {
					foreach (ListItem item in chkLstOrgao.Items) {
						if (item.Value.Equals(conselho.Codigo)) {
							item.Selected = true;
						}
					}
					CurrentListaConselho = GetSelecaoConselho();
					BindAno(DateTime.Now.Year);
				}

				
			}
		}

		protected override Modulo Modulo {
			get { return Modulo.VISUALIZAR_REUNIAO; }
		}

		protected int CurrentAno {
			get { return ViewState["ANO"] == null ? DateTime.Now.Year : (int)ViewState["ANO"]; }
			set { ViewState["ANO"] = value; }
		}

		protected List<string> CurrentListaConselho {
			get { return (List<string>)ViewState["CONS"]; }
			set { ViewState["CONS"] = value; }
		}

		protected List<ReuniaoVO> Reunioes {
			get { return _lstReuniao; }
			set { _lstReuniao = value; }
		}

		protected DateTime? CurrentDate {
			get { return ViewState["DATE"] == null ? new DateTime?() : (DateTime)ViewState["DATE"]; }
			set { ViewState["DATE"] = value; }
		}

		private List<string> GetSelecaoConselho() {
			List<string> lstRet = new List<string>();
			foreach (ListItem item in chkLstOrgao.Items) {
				if (item.Selected)
					lstRet.Add(item.Value);
			}
			return lstRet;
		}

		private void BindAno(int ano) {
			CurrentAno = ano;
			CurrentDate = null;

			if (Reunioes == null)
				Reunioes = ReuniaoBO.Instance.ListReuniaoAno(CurrentListaConselho, ano);

			List<KeyValuePair<int, string>> meses = new List<KeyValuePair<int, string>>();
			string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

			foreach (string name in monthNames) {
				if (!string.IsNullOrEmpty(name))
					meses.Add(new KeyValuePair<int,string>(meses.Count+1, name));
			}
			dlCalendar.DataSource = meses;
			dlCalendar.DataBind();

			List<int> lstAnos = new List<int>();
			for (int i = ano - 5; i <= ano + 5; ++i)
				lstAnos.Add(i);
			rptAno.DataSource = lstAnos;
			rptAno.DataBind();

			gdvRelatorio.DataSource = null;
			gdvRelatorio.DataBind();
		}

		private void BindReuniao(DateTime date) {
			if (Reunioes == null)
				Reunioes = ReuniaoBO.Instance.ListReuniaoAno(CurrentListaConselho, CurrentAno);

			List<ReuniaoVO> lst = Reunioes.FindAll(x => x.Data == date);
			gdvRelatorio.DataSource = lst;
			gdvRelatorio.DataBind();
		}

		protected void cal_DayRender(object sender, DayRenderEventArgs e) {
			DateTime date = e.Day.Date;
			e.Day.IsSelectable = false;
			if (e.Day.IsOtherMonth) {
				e.Cell.ForeColor = System.Drawing.Color.White;
				return;
			}
			if (Reunioes != null) {
				ReuniaoVO r = Reunioes.FirstOrDefault(x => x.Data == date);
				if (r != null) {
					e.Day.IsSelectable = true;
					e.Cell.BackColor = System.Drawing.Color.Yellow;
					if (CurrentDate != null && CurrentDate == date) {
						e.Cell.ForeColor = System.Drawing.Color.Red;
					}
				}
			}
		}

		protected void dlCalendar_ItemDataBound(object sender, DataListItemEventArgs e) {
			DataListItem item = e.Item;
			if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem) {
				KeyValuePair<int, string> mes = (KeyValuePair<int, string>)item.DataItem;
				System.Web.UI.WebControls.Calendar cal = (System.Web.UI.WebControls.Calendar)item.FindControl("calendario");
				cal.VisibleDate = new DateTime(CurrentAno, mes.Key, 1);
			}
		}

		protected void calendario_SelectionChanged(object sender, EventArgs e) {
			System.Web.UI.WebControls.Calendar cal = (System.Web.UI.WebControls.Calendar)sender;

			DateTime date = cal.SelectedDate;
			CurrentDate = date;
			cal.SelectedDates.Clear();
			BindReuniao(date);
		}

		protected void btnAno_Click(object sender, EventArgs e) {
			LinkButton btn = (LinkButton)sender;
			int ano = Int32.Parse(btn.Text);
			BindAno(ano);
		}

		protected void btnBuscar_Click(object sender, EventArgs e) {
			try {
				List<string> lst = GetSelecaoConselho();
				if (lst.Count == 0) {
					this.ShowError("Selecione pelo menos um conselho para buscar as reuniões!");
					return;
				}
				CurrentListaConselho = GetSelecaoConselho();
				BindAno(CurrentAno);
			} catch (Exception ex) {
				this.ShowError("Erro ao realizar busca!", ex);
			}
		}

	}
}