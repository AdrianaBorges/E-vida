using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.Exceptions;
using System.Globalization;

namespace eVidaGeneralLib.Reporting {
	public class ReportCartaPositivaCraBinder : IReportBinder {
		private CartaPositivaCraVO vo;

		public ReportCartaPositivaCraBinder(CartaPositivaCraVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.UseExternalImages = true;

			repParams.Params.Add("NumSolicitacao", vo.Id.ToString());
			repParams.Params.Add("TipoCarta", ((int)vo.Tipo).ToString());
			repParams.Params.Add("ProtocoloCra", vo.ProtocoloCra);
			repParams.Params.Add("DataCriacao", vo.DataCriacao.ToShortDateString());

			repParams.Params.Add("NomeBeneficiario", vo.Beneficiario.Nomusr);
			repParams.Params.Add("CodAlternativo", vo.Beneficiario.Matant);
            repParams.Params.Add("Nascimento", !string.IsNullOrEmpty(vo.Beneficiario.Datnas.Trim()) ? DateTime.ParseExact(vo.Beneficiario.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString() : " - ");
			repParams.Params.Add("Filiacao", vo.Beneficiario.Mae);
			repParams.Params.Add("TipoBeneficiario", vo.Beneficiario.Tipusu);
            repParams.Params.Add("Cpf", vo.Beneficiario.Cpfusr != null ? FormatUtil.FormatCpf(vo.Beneficiario.Cpfusr).ToReportString() : "");

			PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(vo.CdPlano);
            PFamiliaProdutoVO benefPlano = PFamiliaBO.Instance.GetFamiliaProduto(vo.Beneficiario.Codint, vo.Beneficiario.Codemp, vo.Beneficiario.Matric, vo.Beneficiario.Tipreg);

			repParams.Params.Add("Plano", plano.Codigo + " - " + plano.Descri);
            repParams.Params.Add("Vigencia", DateTime.ParseExact(benefPlano.Datcar, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString() + " - " + (!string.IsNullOrEmpty(benefPlano.Datblo.Trim()) ? DateTime.ParseExact(benefPlano.Datblo, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString() : " "));

			repParams.Params.Add("CredCpfCnpj", FormatUtil.FormatCpfCnpj(vo.Credenciado.Tippe, vo.Credenciado.Cpfcgc));
			repParams.Params.Add("CredNome", vo.Credenciado.Nome);
			repParams.Params.Add("Contato", vo.Contato);

			if (vo.IdUsuarioAprovacao == null) {
				throw new EvidaException("O formulário não está aprovado ainda!");
			}

			UsuarioVO usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.IdUsuarioAprovacao.Value);

			repParams.Params.Add("UsuarioAprov", usuarioAprov.Nome.ToUpper());
			repParams.Params.Add("CargoAprov", usuarioAprov.Cargo.ToReportString());

			string assinatura = UsuarioBO.Instance.GetAssinatura(usuarioAprov.Id);
			if (!string.IsNullOrEmpty(assinatura)) {
				Uri pathAsUri = new Uri(assinatura);
				repParams.Params.Add("ImgAssinaturaPath", pathAsUri.AbsolutePath);
			} else {
				repParams.Params.Add("ImgAssinaturaPath", "-");
			}

			return repParams;
		}

		public string GerarNome() {
			return "CARTA_POSITIVA_CRA_" + vo.Id;
		}

		public string DefaultRpt() {
			return "rptCartaPositivaCra";
		}
	}
}
