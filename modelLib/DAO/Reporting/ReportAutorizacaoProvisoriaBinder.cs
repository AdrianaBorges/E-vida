using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace eVidaGeneralLib.Reporting {
	public class ReportAutorizacaoProvisoriaBinder :IReportBinder {
		private AutorizacaoProvisoriaVO vo;
		int[] PLANOS_ESPECIAIS = new int[] { PConstantes.PLANO_EVIDA_FAMILIA, PConstantes.PLANO_EVIDA_MELHOR_IDADE, PConstantes.PLANO_MAIS_VIDA_CEA };

		public ReportAutorizacaoProvisoriaBinder(AutorizacaoProvisoriaVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			if (vo.CodSolicitacao == Int32.MinValue) {
				return GetBlank();
			}
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.UseExternalImages = true;

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(AutorizacaoProvisoriaVO.FORMATO_PROTOCOLO) + " / " + vo.DataCriacao.Year);

            PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(benef.Codint, benef.Codemp, benef.Matric);
            PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(vo.Plano.Codigo);

            //UsuarioVO usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAprovacao.Value);
			UsuarioVO usuarioCriador = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioCriacao);

			repParams.Params.Add("NomeBeneficiario", benef.Nomusr);
			repParams.Params.Add("NomeTitular", titular.Nomusr);
			repParams.Params.Add("Matricula", titular.Matemp);
            repParams.Params.Add("Nascimento", !string.IsNullOrEmpty(benef.Datnas.Trim()) ? DateTime.ParseExact(benef.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "-");
			repParams.Params.Add("CodAlternativo", benef.Matant);
			repParams.Params.Add("Acomodacao", "APARTAMENTO");
			repParams.Params.Add("Validade", vo.FimVigencia.ToString("dd/MM/yyyy"));

			repParams.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(vo.DataCriacao));
			repParams.Params.Add("Local", vo.Local.Cidade);

			bool showPlantaoSocial = !PLANOS_ESPECIAIS.Contains(Int32.Parse(plano.Codigo)) && !vo.IsReciprocidade;
			repParams.Params.Add("PlantaoSocial", !showPlantaoSocial ? "-" : vo.Local.Telefone);
			repParams.Params.Add("Plano", plano.Codigo);

			repParams.DataSources.Add("dsProcedimentos", BuildTableAssistencia(vo));

			if (vo.Coberturas != null && vo.Coberturas.Count > 0) {
				int idxUrgencia = vo.Coberturas.IndexOf("UrE");
				if (idxUrgencia >= 0) {
					vo.Coberturas[idxUrgencia] = "Urgência e Emergência";
				}
				repParams.Params.Add("Cobertura", vo.Coberturas.Aggregate((x, y) => x + ", " + y));
			} else {
				repParams.Params.Add("Cobertura", "-");
			}
			repParams.Params.Add("Observacao", string.IsNullOrEmpty(vo.Observacao) ? " - " : vo.Observacao);
			repParams.Params.Add("Abrangencia", vo.Abrangencia.ToString());

			repParams.Params.Add("UsuarioAprov", usuarioCriador.Nome.ToUpper());
			repParams.Params.Add("CargoAprov", usuarioCriador.Cargo.ToReportString());
			string assinatura = UsuarioBO.Instance.GetAssinatura(usuarioCriador.Id);
			if (!string.IsNullOrEmpty(assinatura)) {
				Uri pathAsUri = new Uri(assinatura);
				repParams.Params.Add("ImgAssinaturaPath", pathAsUri.AbsolutePath);
			} else {
				repParams.Params.Add("ImgAssinaturaPath", "-");
			}

			return repParams;
		}

		private ReportBinderParams GetBlank() {
			ReportBinderParams repParams = new ReportBinderParams();
			
			repParams.Params.Add("NumSolicitacao", "   ");

			repParams.Params.Add("NomeBeneficiario", " ");
			repParams.Params.Add("NomeTitular", " ");
			repParams.Params.Add("Matricula", " ");
			repParams.Params.Add("Nascimento", " ");
			repParams.Params.Add("CodAlternativo", " ");
			repParams.Params.Add("Acomodacao", "APARTAMENTO");
			repParams.Params.Add("Validade", " ");

			repParams.Params.Add("DataCriacao", " ");
			repParams.Params.Add("Local", " ");
			repParams.Params.Add("PlantaoSocial", " ");
			repParams.Params.Add("Plano", " ");

			AutorizacaoProvisoriaVO vo = new AutorizacaoProvisoriaVO();
			vo.Procedimentos = new List<int>();
			for (int i = 1; i <= 7; i++) {
				vo.Procedimentos.Add(i);
			}

			repParams.DataSources.Add("dsProcedimentos", BuildTableAssistencia(vo));

			repParams.Params.Add("Cobertura", " ");

			repParams.Params.Add("Observacao", " ");
			repParams.Params.Add("Abrangencia", " ");

			repParams.Params.Add("UsuarioAprov", " ");
			repParams.Params.Add("CargoAprov", " ");
			repParams.Params.Add("ImgAssinaturaPath", "-");
			return repParams;
		}

		private static DataTable BuildTableAssistencia(AutorizacaoProvisoriaVO vo) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			if (vo.Procedimentos != null) {
				foreach (int idAssistencia in vo.Procedimentos) {
					DataRow drN = dt.NewRow();
					drN["Nome"] = AutorizacaoProvisoriaEnumTradutor.TraduzProcedimentos(idAssistencia);
					dt.Rows.Add(drN);
				}
			}
			return dt;
		}

		public string GerarNome() {
			if (vo.CodSolicitacao == Int32.MinValue)
				return "SOL_PROVISORIA_BRANCO";
			return "SOL_PROVISORIA_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptAutorizacaoProvisoria";
		}
	}
}
