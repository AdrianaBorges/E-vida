using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaCredenciados.Classes;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;

namespace eVidaCredenciados.controls {
	public partial class Menu : UserControlBase {

		protected void Page_Load(object sender, EventArgs e) {
			if (!IsPostBack) {
				if (this.UsuarioLogado == null)
					this.Visible = false;
				else {
					mnFrmAutorizacao.Visible = false;


                    /*string param = ParametroUtil.CredenciadoAutorizacao;
                    if (!string.IsNullOrEmpty(param)) {
                        IEnumerable<int> ids = ParseIds(param);
                        if (ids != null && ids.Contains(this.UsuarioLogado.Credenciado.CdCredenciado)) {
                            mnFrmAutorizacao.Visible = true;
                        }
                    } else {
                        mnFrmAutorizacao.Visible = true;
                    }*/

                    /*
                    // Permite o acesso ao Módulo de Autorização a todos os credenciados ativos no Protheus
                    PRedeAtendimentoVO credenciado = PRedeAtendimentoBO.Instance.GetById(this.UsuarioLogado.RedeAtendimento.Codigo);
                    if (credenciado != null)
                    {
                        if (String.IsNullOrEmpty(credenciado.Datblo.Trim()) || (!String.IsNullOrEmpty(credenciado.Datblo.Trim()) && Int32.Parse(credenciado.Datblo) > Int32.Parse(DateTime.Today.ToString("yyyyMMdd"))))
                        {
                            mnFrmAutorizacao.Visible = true;
                        }
                    }*/

					menuIr.Visible = menuIr.Enabled = this.UsuarioLogado.ConfiguracaoIr.EnableIrCredenciado;
				}
			}
		}

		protected void btnSair_Click(object sender, EventArgs e) {
			this.Session.Abandon();
			Response.Redirect("~/Login.aspx");
		}

		private IEnumerable<int> ParseIds(string param) {
			if (!string.IsNullOrEmpty(param)) {
				string[] ids = param.Split(new char[] { ';' });
				try {
					return ids.Select(x => Int32.Parse(x));
				} catch (Exception ex) {
					Log.Error("Erro ao parsar IDs Credenciados (" + param + "). ", ex);
				}
			}
			return null;
		}
	}
}