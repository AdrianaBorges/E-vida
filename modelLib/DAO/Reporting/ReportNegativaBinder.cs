using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportNegativaBinder : IReportBinder {
		private FormNegativaVO vo;
		private bool reanalise;
		public ReportNegativaBinder(FormNegativaVO form, bool reanalise) {
			this.vo = form;
			this.reanalise = reanalise;
		}

		public string GerarNome() {
			return (reanalise ? "REANALISE_" : "") + "NEGATIVA_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptNegativa" + (reanalise ? "Reanalise" : "");
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

			FormNegativaInfoAdicionalVO infoVO = FormNegativaBO.Instance.GetInfoAdicional(vo);

			UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(reanalise ? vo.Reanalise.IdUsuario : vo.IdUsuario);
			UsuarioVO usuarioAprov = UsuarioBO.Instance.GetUsuarioById(reanalise ? vo.Reanalise.IdUsuarioUpdate : vo.IdUsuarioUpdate);

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(FormNegativaVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NomeBeneficiario", infoVO.NomeBeneficiario);
			repParams.Params.Add("Cartao", infoVO.Cartao.ToReportString());
			repParams.Params.Add("DataNascimento", infoVO.DataNascimento.ToString("dd/MM/yyyy"));
			repParams.Params.Add("Cpf", infoVO.Cpf.ToReportString());
			repParams.Params.Add("DataAdesao", infoVO.DataAdesao.ToString("dd/MM/yyyy"));
            repParams.Params.Add("NroProdutoAns", (infoVO.Plano == null || string.IsNullOrEmpty(infoVO.Plano.Susep)) ? "-" : infoVO.Plano.Susep);
            repParams.Params.Add("NomePlano", (infoVO.Plano == null || string.IsNullOrEmpty(infoVO.Plano.Descri)) ? "-" : infoVO.Plano.Descri);
			repParams.Params.Add("PadraoAcomodacao", vo.PadraoAcomodacao);
			repParams.Params.Add("TipoRede", vo.TipoRede);
			repParams.Params.Add("NomePrestador", vo.Prestador.Nome.ToReportString());
			repParams.Params.Add("DescricaoSolicitacao", vo.DescricaoSolicitacao.ToReportString());
			repParams.Params.Add("PrevContratual", vo.PrevisaoContratual);
			repParams.Params.Add("Usuario", usuario.Nome);
			repParams.Params.Add("Cargo", usuario.Cargo);
			repParams.Params.Add("DataCriacao", vo.DataCriacao.ToShortDateString());
			repParams.Params.Add("Ano", vo.DataCriacao.Year.ToString());
			repParams.Params.Add("DataFormulario", vo.DataFormulario.ToString("dd \\de MMMM \\de yyyy"));

			repParams.Params.Add("IndCoberturaContratual", ((vo.InfoDispositivoLegal & 1) == 1) ? "S" : "N");
			repParams.Params.Add("IndIndicacaoTecnica", ((vo.InfoDispositivoLegal & 2) == 2) ? "S" : "N");

			repParams.Params.Add("UsuarioAprov", usuarioAprov.Nome.ToUpper());
			repParams.Params.Add("CargoAprov", usuarioAprov.Cargo.ToReportString());

			repParams.Params.Add("ProtocoloAns", reanalise ? vo.Reanalise.ProtocoloAns : vo.ProtocoloAns);
			if (reanalise) {
				repParams.Params.Add("ProtocoloAnsOrigem", vo.ProtocoloAns);
				repParams.Params.Add("JustificativaNegativa", vo.Reanalise.JustificativaNegativa);
				repParams.Params.Add("ParecerReanalise", vo.Reanalise.Parecer == 1 ? "A" : "N");
				repParams.Params.Add("ParecerReanaliseObs", vo.Reanalise.ObservacaoParecer);
				repParams.Params["DataCriacao"] = vo.Reanalise.DataCriacao.ToShortDateString();
				repParams.Params["DataFormulario"] = vo.Reanalise.DataFormulario.ToShortDateString();
			}

			repParams.Params.Add("CpfCnpjPrestador", FormatUtil.TryFormatCpfCnpj(vo.Prestador.Cpfcgc).ToReportString());
			repParams.Params.Add("DataSolicitacao", FormatUtil.FormatDataForm(vo.DataSolicitacao).ToReportString());
			repParams.Params.Add("NroConselho", vo.Profissional.Numcr.ToReportString());
			repParams.Params.Add("Conselho", vo.Profissional.Codsig + " - " + vo.Profissional.Estado);
			repParams.Params.Add("NomeProfissional", vo.Profissional.Nome.ToReportString());

			if (vo.IdMotivoGlosa != null) {
				MotivoGlosaVO motivoVO = FormNegativaBO.Instance.GetMotivoGlosaById(vo.IdMotivoGlosa.Value);
				repParams.Params.Add("MotivoNegativa", motivoVO.Id + " - " + motivoVO.Grupo + " - " + motivoVO.Descricao);
			} else {
				repParams.Params.Add("MotivoNegativa", " ");
			}

			string assinatura = UsuarioBO.Instance.GetAssinatura(usuarioAprov.Id);
			if (!string.IsNullOrEmpty(assinatura)) {
				Uri pathAsUri = new Uri(assinatura);
				repParams.Params.Add("ImgAssinaturaPath", pathAsUri.AbsolutePath);
			} else {
				repParams.Params.Add("ImgAssinaturaPath", "-");
			}
			DataTable dt = BuildItens(vo);
			repParams.DataSources.Add("dsItens", dt);

			if (!reanalise) {
				dt = BuildJustificativa(vo.JustContratual);
				repParams.DataSources.Add("dsJustContratual", dt);
				dt = BuildJustificativa(vo.JustAssistencial);
				repParams.DataSources.Add("dsJustAssistencial", dt);
			}
			dt = BuildRegiaoAbrangencia(infoVO.Plano == null ? "" : infoVO.Plano.Abrang);
			repParams.DataSources.Add("dsRegiaoAbrangencia", dt);

			return repParams;
		}

		private DataTable BuildRegiaoAbrangencia(string tpAbrangencia) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome1");
			dt.Columns.Add("Selecionado1");
			dt.Columns.Add("Nome2");
			dt.Columns.Add("Selecionado2");

			DataTable dtLista = LocatorDataBO.Instance.ListarItensLista(PConstantes.LISTA_ABRANGENCIA);
			DataRow drNew = null;
			int pos = 0;
			foreach (DataRow dr in dtLista.Rows) {
				if (drNew == null)
					drNew = dt.NewRow();

				pos++;
				drNew["Nome" + pos] = Convert.ToString(dr["DS_ITEM_LISTA"]);
				drNew["Selecionado" + pos] = Convert.ToString(dr["CD_ITEM_LISTA"]).Equals(tpAbrangencia) ? "X" : "";
				if (pos == 2) {
					dt.Rows.Add(drNew);
					drNew = null;
					pos = 0;
				}
			}

			return dt;
		}

		private static DataTable BuildItens(FormNegativaVO vo) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Mascara");
			dt.Columns.Add("Descricao");
			dt.Columns.Add("Qtd");
			dt.Columns.Add("Obs");

			if (vo.Itens != null) {
				foreach (FormNegativaItemVO item in vo.Itens) {
					DataRow drN = dt.NewRow();
					drN["MASCARA"] = item.Codpsa;
					drN["Descricao"] = item.Descri;
					drN["Qtd"] = item.Quantidade.ToString();
					drN["Obs"] = item.Observacao;

					dt.Rows.Add(drN);
				}
			}
			return dt;
		}

		private static DataTable BuildJustificativa(List<FormNegativaJustificativaVO> lst) {
			DataTable dt = new DataTable();
			dt.Columns.Add("ID");
			dt.Columns.Add("Parameter");

			if (lst != null) {
				foreach (FormNegativaJustificativaVO item in lst) {
					DataRow drN = dt.NewRow();
					drN["ID"] = item.IdJustificativa;
					drN["Parameter"] = item.Parametros;

					dt.Rows.Add(drN);
				}
			}
			return dt;
		}


	}
}
