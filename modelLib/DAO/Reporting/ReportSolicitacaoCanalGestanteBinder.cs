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

namespace eVidaGeneralLib.Reporting {
	public class ReportSolicitacaoCanalGestanteBinder : IReportBinder {
		private CanalGestanteVO vo;

		public ReportSolicitacaoCanalGestanteBinder(CanalGestanteVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();

			CanalGestanteConfigVO configVO = CanalGestanteBO.Instance.GetConfig(vo.DataSolicitacao);

			if (configVO == null) {
				int ano = CanalGestanteBO.Instance.CalcularAno(vo.DataSolicitacao);
				throw new Exception("Configuração para o ano " + ano + " não encontrada!");
			}

			CanalGestanteBenefVO benefVO = CanalGestanteBO.Instance.GetInfoBenef(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
            PUsuarioVO hcBenefVO = PUsuarioBO.Instance.GetUsuario(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
            PFamiliaProdutoVO benefPlanoVO = PFamiliaBO.Instance.GetFamiliaProduto(vo.Codint, vo.Codemp, vo.Matric, vo.Tipreg);
            PProdutoSaudeVO planoVO = PLocatorDataBO.Instance.GetProdutoSaude(benefPlanoVO.Codpla);

			List<CanalGestanteConfigCredVO> lstCredenciados = CanalGestanteBO.Instance.GetConfigCred(configVO.Ano);
			List<CanalGestanteConfigProfVO> lstMedicos = CanalGestanteBO.Instance.GetConfigProf(configVO.Ano);
			List<PRedeAtendimentoVO> lstInfoCredenciados = CanalGestanteBO.Instance.ListarInfoCredenciadoConfig(configVO.Ano);
			List<PProfissionalSaudeVO> lstInfoMedicos = CanalGestanteBO.Instance.ListarInfoProfissionalConfig(configVO.Ano);

			FiltrarMedicos(vo, lstMedicos, lstInfoMedicos);
			FiltrarCredenciados(vo, lstCredenciados, lstInfoCredenciados);

			repParams.Params.Add("NumSolicitacao", vo.Id.ToString());
			repParams.Params.Add("NomeBeneficiario", hcBenefVO.Nomusr);
			repParams.Params.Add("NumCartao", benefVO.CodAlternativo);
			repParams.Params.Add("Idade", DateUtil.CalculaIdade(benefVO.DataNascimento).ToString());
			repParams.Params.Add("Email", benefVO.Email);
			repParams.Params.Add("Telefone", benefVO.Telefone);
			repParams.Params.Add("Plano", planoVO.Codigo + " - " + planoVO.Descri);
			repParams.Params.Add("DataSolicitacao", vo.DataSolicitacao.ToString("dd/MM/yyyy"));
			repParams.Params.Add("DataEmissao", vo.DataSolicitacao.ToString("dd/MM/yyyy"));
			repParams.Params.Add("Ano", vo.DataSolicitacao.Year.ToString());
			repParams.Params.Add("AnoCalculo", configVO.Ano.ToString());


			repParams.Params.Add("PartoNormal", configVO.PartoNormal.ToString("##0.00"));
			repParams.Params.Add("PartoCesarea", (CalcularPartoCesarea(configVO.PartoNormal)).ToString("##0.00"));

			DataTable dtCred = GerarTableCredenciados(lstCredenciados, lstInfoCredenciados);
			DataTable dtProf = GerarTableMedicos(lstMedicos, lstInfoMedicos);

			repParams.DataSources.Add("dsEstabelecimentos", dtCred);
			repParams.DataSources.Add("dsMedicos", dtProf);
			return repParams;
		}

		public string GerarNome() {
			return "CANAL_GESTANTE_" + vo.Id.ToString(CanalGestanteVO.FORMATO_PROTOCOLO_FILE);
		}

		public string DefaultRpt() {
			return "rptSolicitacaoCanalGestante";
		}

		private void FiltrarMedicos(CanalGestanteVO vo, List<CanalGestanteConfigProfVO> lstMedicos, List<PProfissionalSaudeVO> lstInfoMedicos) {
			if (vo.NrSeqAnsProfissional != null) {
				lstMedicos.RemoveAll(x => x.Codigo != vo.NrSeqAnsProfissional);
				lstInfoMedicos.RemoveAll(x => x.Codigo != vo.NrSeqAnsProfissional);     // Confirmar depois se este filtro está correto. Caso dê problema, retirar.
			} else {
				lstMedicos.Clear();
				lstInfoMedicos.Clear();
			}
		}

		private void FiltrarCredenciados(CanalGestanteVO vo, List<CanalGestanteConfigCredVO> lstCredenciados, List<PRedeAtendimentoVO> lstInfoCredenciados) {
			if (vo.CdCredenciado != null) {
				lstCredenciados.RemoveAll(x => x.CodCredenciado != vo.CdCredenciado);
				lstInfoCredenciados.RemoveAll(x => x.Codigo != vo.CdCredenciado);
			} else {
				lstCredenciados.Clear();
				lstInfoCredenciados.Clear();
			}
		}

		private decimal CalcularPartoCesarea(decimal partoNormal) {
			decimal pn = partoNormal;
			return 100 - pn;
		}

		private DataTable GerarTableCredenciados(List<CanalGestanteConfigCredVO> lstCredenciados, List<PRedeAtendimentoVO> lstInfoCredenciados) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("UF");
			dt.Columns.Add("PartoNormal");
			dt.Columns.Add("PartoCesarea");

			if (lstCredenciados != null) {
				foreach (CanalGestanteConfigCredVO cred in lstCredenciados) {
					DataRow dr = dt.NewRow();
					string nome = " ";
					PRedeAtendimentoVO infoVO = null;
					if (lstInfoCredenciados != null)
						infoVO = lstInfoCredenciados.FirstOrDefault(x => x.Codigo == cred.CodCredenciado);
					if (infoVO != null)
						nome = infoVO.Nome;

					dr["Nome"] = nome;
					dr["UF"] = cred.Uf;
					dr["PartoNormal"] = cred.PartoNormal.ToString("##0.00");
					dr["PartoCesarea"] = CalcularPartoCesarea(cred.PartoNormal).ToString("##0.00");

					dt.Rows.Add(dr);
				}
			}

			dt = dt.AsEnumerable().OrderBy(x => x["UF"]).ThenBy(x => x["Nome"]).AsDataView().ToTable();

			return dt;
		}

		private DataTable GerarTableMedicos(List<CanalGestanteConfigProfVO> lstMedicos, List<PProfissionalSaudeVO> lstInfoMedicos) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("CRMUF");
			dt.Columns.Add("PartoNormal");
			dt.Columns.Add("PartoCesarea");

			if (lstMedicos != null) {
				foreach (CanalGestanteConfigProfVO cred in lstMedicos) {
					DataRow dr = dt.NewRow();
					string nome = " ";
					PProfissionalSaudeVO infoVO = null;
					if (lstInfoMedicos != null)
                        infoVO = lstInfoMedicos.FirstOrDefault(x => x.Codigo == cred.Codigo); // Confirmar depois se este filtro está correto. Caso dê problema, retirar.
					if (infoVO != null)
						nome = infoVO.Nome;

					dr["Nome"] = nome;
					dr["CRMUF"] = cred.Codsig + " / " + cred.Estado;
					dr["PartoNormal"] = cred.PartoNormal.ToString("##0.00");
					dr["PartoCesarea"] = CalcularPartoCesarea(cred.PartoNormal).ToString("##0.00");

					dt.Rows.Add(dr);
				}
			}
			dt = dt.AsEnumerable().OrderBy(x => x["Nome"]).AsDataView().ToTable();
			return dt;
		}

	}
}
