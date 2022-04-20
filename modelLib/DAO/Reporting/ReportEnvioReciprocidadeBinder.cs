using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
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
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.Reporting {
	public class ReportEnvioReciprocidadeBinder : IReportBinder {
		private ReciprocidadeVO vo;

		public ReportEnvioReciprocidadeBinder(ReciprocidadeVO vo) {
			this.vo = vo;
		}

		public ReportBinderParams GetData() {
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.UseExternalImages = true;
			if (vo.CodSolicitacao == Int32.MinValue) {
				return GetBlank();
			}

			UsuarioVO usuarioAprov = null;
			if (vo.Status == StatusReciprocidade.APROVADO) {
				usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAprovacao);
			} else if (vo.Status == StatusReciprocidade.ENVIADO) {
				usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioEnvio);
			} else {
				usuarioAprov = UsuarioBO.Instance.GetUsuarioById(vo.CodUsuarioAlteracao);
			}

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(vo.Codint, vo.Codemp, vo.Matric);

            PVidaVO vida = null;
            if(!string.IsNullOrEmpty(titular.Matvid.Trim())){
                vida = PVidaBO.Instance.GetVida(titular.Matvid);
            }

			repParams.Params.Add("NumSolicitacao", vo.CodSolicitacao.ToString(ReciprocidadeVO.FORMATO_PROTOCOLO));
			repParams.Params.Add("NomeTitular", titular.Nomusr);
			repParams.Params.Add("Cartao", titular.GetCarteira());
			repParams.Params.Add("Nascimento", DateTime.ParseExact(titular.Datnas, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
            repParams.Params.Add("CPF", titular.Cpfusr != null ? FormatUtil.FormatCpf(titular.Cpfusr) : "");
			repParams.Params.Add("CNS", vida != null ? vida.Nrcrna.ToReportString() : "");
			repParams.Params.Add("EstadoCivil", PLocatorDataBO.Instance.GetItemLista(PConstantes.LISTA_ESTADO_CIVIL, titular.Estciv));
			repParams.Params.Add("NomeMae", titular.Mae);

			if (!string.IsNullOrEmpty(vo.CodintReciprocidade.Trim())) {
                POperadoraSaudeVO empresa = ReciprocidadeBO.Instance.GetOperadoraById(vo.CodintReciprocidade);
				repParams.Params.Add("NomeEmpresa", empresa.Nomint.Trim());
                repParams.Params.Add("TelEmpresa", !string.IsNullOrEmpty(empresa.Telef1.Trim()) ? empresa.Telef1.Trim() : "-");
                repParams.Params.Add("FaxEmpresa", !string.IsNullOrEmpty(empresa.Fax1.Trim()) ? empresa.Fax1.Trim() : "-");
			} else {
				repParams.Params.Add("NomeEmpresa", "-");
				repParams.Params.Add("TelEmpresa", "-");
				repParams.Params.Add("FaxEmpresa", "-");
			}
			repParams.Params.Add("Data", vo.DataCriacao.ToString("dd/MM/yyyy"));

			repParams.Params.Add("Cidade", vo.Endereco.Cidade);
			repParams.Params.Add("UF", vo.Endereco.Uf);

			repParams.Params.Add("DataInicio", vo.Inicio.ToString("dd/MM/yyyy"));
			repParams.Params.Add("DataFim", vo.Fim.ToString("dd/MM/yyyy"));

			repParams.Params.Add("Observacao", vo.Observacao);

			repParams.Params.Add("UsuarioAprov", usuarioAprov.Nome.ToUpper());
			repParams.Params.Add("CargoAprov", usuarioAprov.Cargo.ToReportString());

			string assinatura = UsuarioBO.Instance.GetAssinatura(usuarioAprov.Id);
			if (!string.IsNullOrEmpty(assinatura)) {
				Uri pathAsUri = new Uri(assinatura);
				repParams.Params.Add("ImgAssinaturaPath", pathAsUri.AbsolutePath);
			} else {
				repParams.Params.Add("ImgAssinaturaPath", "-");
			}

			repParams.DataSources.Add("dsBeneficiario", BuildTableBeneficiarios(vo));
			repParams.DataSources.Add("dsAssistencia", BuildTableAssistencia(vo));
			return repParams;
		}

		public string GerarNome() {
			if (vo.CodSolicitacao == Int32.MinValue)
				return "SOL_RECIPROCIDADE_BRANCO";
			return "SOL_RECIPROCIDADE_" + vo.CodSolicitacao;
		}

		public string DefaultRpt() {
			return "rptEnvioReciprocidade";
		}

		private ReportBinderParams GetBlank() {
			ReportBinderParams repParams = new ReportBinderParams();
			repParams.Params.Add("NumSolicitacao", "   ");

			repParams.Params.Add("NomeTitular", " ");
			repParams.Params.Add("Cartao", " ");
			repParams.Params.Add("Nascimento", " ");
			repParams.Params.Add("CPF", " ");
			repParams.Params.Add("CNS", " ");
			repParams.Params.Add("EstadoCivil", " ");
			repParams.Params.Add("NomeMae", " ");

			repParams.Params.Add("NomeEmpresa", " ");
			repParams.Params.Add("TelEmpresa", " ");
			repParams.Params.Add("FaxEmpresa", " ");

			repParams.Params.Add("Data", " ");

			repParams.Params.Add("Cidade", " ");
			repParams.Params.Add("UF", " ");

			repParams.Params.Add("DataInicio", " ");
			repParams.Params.Add("DataFim", " ");

			repParams.Params.Add("Observacao", " ");

			repParams.Params.Add("UsuarioAprov", " ");
			repParams.Params.Add("CargoAprov", " ");
			repParams.Params.Add("ImgAssinaturaPath", "-");

			DataTable dt = CreateTableBenefStruct();
			for (int i = 0; i < 3; i++) {
				DataRow drN = dt.NewRow();
				dt.Rows.Add(drN);
			}

			repParams.DataSources.Add("dsBeneficiario", dt);

			ReciprocidadeVO vo = new ReciprocidadeVO();
			vo.Assistencia = new List<int>();
			for (int i = 1; i <= 4; i++) {
				vo.Assistencia.Add(i);
			}
			repParams.DataSources.Add("dsAssistencia", BuildTableAssistencia(vo));

			return repParams;
		}

		private static DataTable BuscarBeneficiarios(ReciprocidadeVO vo) {
			return ReciprocidadeBO.Instance.BuscarBeneficiarios(vo.CodSolicitacao);
		}

		private static DataTable CreateTableBenefStruct() {
			DataTable dt = new DataTable();
			dt.Columns.Add("Nome");
			dt.Columns.Add("Parentesco");
			dt.Columns.Add("NomeMae");
			dt.Columns.Add("CPF");
			dt.Columns.Add("CNS");
			dt.Columns.Add("Nascimento");
			return dt;
		}

		private static DataTable BuildTableBeneficiarios(ReciprocidadeVO vo) {

			DataTable dtBeneficiarios = BuscarBeneficiarios(vo);
			DataView dv = new DataView(dtBeneficiarios);
			dv.RowFilter = "BA1_TIPUSU <> '" + PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO + "'";
			DataTable dt = CreateTableBenefStruct();

			foreach (DataRowView dr in dv) {
				DataRow drN = dt.NewRow();
				drN["Nome"] = Convert.ToString(dr["BA1_NOMUSR"]);
                drN["Parentesco"] = Convert.ToString(dr["BRP_DESCRI"]);
				drN["NomeMae"] = Convert.ToString(dr["BA1_MAE"]);
                drN["Nascimento"] = !string.IsNullOrEmpty(dr["BA1_DATNAS"].ToString().Trim()) ? DateTime.ParseExact(dr["BA1_DATNAS"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "-";
                drN["CPF"] = dr["BA1_CPFUSR"] != DBNull.Value ? FormatUtil.FormatCpf(Convert.ToString(dr["BA1_CPFUSR"])) : "";
				drN["CNS"] = Convert.ToString(dr["BTS_NRCRNA"]);
				dt.Rows.Add(drN);
			}
			return dt;
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
