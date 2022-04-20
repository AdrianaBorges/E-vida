using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Reporting {
	public class ReportAtestadoComparecimentoBinder : IReportBinder {
		private AtestadoComparecimentoVO vo;

		public ReportAtestadoComparecimentoBinder(AtestadoComparecimentoVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

			PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(AtestadoComparecimentoVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NomeTitular", vo.Nome);
			repParams.Params.Add("Cartao", titular.Matant);
			repParams.Params.Add("Lotacao", vo.Lotacao);
			repParams.Params.Add("DataAtendimento", vo.DataAtendimento.ToShortDateString());
			repParams.Params.Add("DataCriacao", vo.DataCriacao.ToString("dd \\de MMMM \\de yyyy"));
			repParams.Params.Add("HoraInicio", vo.HoraInicio);
			repParams.Params.Add("HoraFim", vo.HoraFim);
			repParams.Params.Add("IndPericia", vo.TipoPericia.ToString());

            if (vo.Codint == null || vo.Codemp == null || vo.Matric == null || vo.Tipreg == null)
				repParams.Params.Add("Beneficiario", "TITULAR");
			else {
                PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
				repParams.Params.Add("Beneficiario", benef.Nomusr);
			}
			return repParams;
		}

		public string GerarNome() {
			return "ATESTADO_COMPARECIMENTO_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptAtestadoComparecimento";
		}
	}
}
