using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.Util {
	public class ParametroUtil {

		static EVidaLog log = new EVidaLog(typeof(ParametroUtil));

		public enum ParametroType {
			REPORT_QUERY_FOLDER = 1,
			REPORT_RDLC_FOLDER = 2,
			URL_GALENUS = 4,
			FILE_REPOSITORY = 5,
			EMAIL_AUTORIZA = 6,
			URL_BENEFICIARIOS = 7,
			EMAIL_BYPASS = 8,
			URL_INTRANET = 9,
			EMAIL_PERICIA_MEDICA = 10,
			EMAIL_PERICIA_ODONTO = 11,
			EMAIL_CADASTRO = 12,
			EMAIL_FINANCEIRO = 13,
			EMAIL_CREDENCIAMENTO = 14,
			EMAIL_DIRETORIA = 15,
			EMAIL_FATURAMENTO = 16,
			EMAIL_REEMBOLSO = 17,
			EMAIL_NOTA_FISCAL = 18,
			URL_ADESAO = 30,
			BILHETAGEM_DIR_IN = 40,
			BILHETAGEM_DIR_BAD = 41,
			BILHETAGEM_DIR_OUT = 42,
			IR_FOLDER_CREDENCIADO = 300,
			IR_FOLDER_BENEFICIARIO = 301,
			CREDENCIADO_AUTORIZACAO = 501,
			VIAGEM_VALOR_DIARIA = 10001,
			VIAGEM_VALOR_KM = 10002,
			CONFIGURACAO_IR_HABILITACAO_BENEF = 20001,
			CONFIGURACAO_IR_HABILITACAO_CRED = 20002,
			CONFIGURACAO_IR_ANOS = 20003,
            CONFIGURACAO_IR_DIA_BENEF = 20004,
            CONFIGURACAO_IR_ENDERECO_EVIDA = 20005
		}

		public enum ParametroVariavelType {
			VIAGEM_VALOR_DIARIA = 10001,
			VIAGEM_VALOR_KM = 10002
		}

		public static string EmailByPass {
			get { return GetParameter(ParametroType.EMAIL_BYPASS); }
		}

		public static bool EmailEnabled {
			get {
				string value = ConfigurationManager.AppSettings["EmailEnabled"];
				if (value != null && value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
					return true;
				return false;
			}
		}

		public static string EmailAutoriza {
			get { return GetParameter(ParametroType.EMAIL_AUTORIZA, "autoriza@e-vida.org.br"); }
		}
		public static string EmailPericiaMedica {
			get { return GetParameter(ParametroType.EMAIL_PERICIA_MEDICA, "auditoriamedica@e-vida.org.br"); }
		}
		public static string EmailPericiaOdonto {
			get { return GetParameter(ParametroType.EMAIL_PERICIA_ODONTO, "odontologia@e-vida.org.br"); }
		}
		public static string EmailCadastro {
			get { return GetParameter(ParametroType.EMAIL_CADASTRO, "cadastro@e-vida.org.br"); }
		}
		public static string EmailCredenciamento {
			get { return GetParameter(ParametroType.EMAIL_CREDENCIAMENTO, "credenciamento@e-vida.org.br"); }
		}
		public static string EmailFinanceiro {
			get { return GetParameter(ParametroType.EMAIL_FINANCEIRO, "financeiro@e-vida.org.br"); }
		}
		public static string EmailDiretoria {
			get { return GetParameter(ParametroType.EMAIL_DIRETORIA, "diretoria@e-vida.org.br"); }
		}
		public static string EmailFaturamento {
			get { return GetParameter(ParametroType.EMAIL_FATURAMENTO, "faturamento@e-vida.org.br"); }
		}
		public static string EmailReembolso {
			get { return GetParameter(ParametroType.EMAIL_REEMBOLSO, "reembolso@e-vida.org.br"); }
		}
		public static string EmailNotaFiscal {
			get { return GetParameter(ParametroType.EMAIL_NOTA_FISCAL, "notafiscal@e-vida.org.br"); }
		}

		public static string UrlSiteIntranet {
			get { return GetParameter(ParametroType.URL_INTRANET, "http://intranet.e-vida.org.br"); }
		}

		public static string UrlSiteBeneficiarios {
			get { return GetParameter(ParametroType.URL_BENEFICIARIOS, "http://beneficiarios.e-vida.org.br"); }
		}

		public static string UrlSiteAdesao {
			//get { return GetParameter(ParametroType.URL_ADESAO, "http://simulador.e-vida.org.br/adesao"); }
            get { return GetParameter(ParametroType.URL_ADESAO, "http://adesao-des.e-vida.org.br"); }
		}

		public static string UrlGalenus {
			get { return GetParameter(ParametroType.URL_GALENUS); }
		}

		public static string FileRepository {
			get { return GetParameter(ParametroType.FILE_REPOSITORY); }
		}

		public static string ReportQueryFolder {
			get { return GetParameter(ParametroType.REPORT_QUERY_FOLDER); }
		}

		public static string ReportRdlcFolder {
			get { return GetParameter(ParametroType.REPORT_RDLC_FOLDER); }
		}

		public static string IrFolderCredenciado {
			get { return GetParameter(ParametroType.IR_FOLDER_CREDENCIADO); }
		}

		public static string IrFolderBeneficiario {
			get { return GetParameter(ParametroType.IR_FOLDER_BENEFICIARIO); }
		}
		public static string CredenciadoAutorizacao {
			get { return GetParameter(ParametroType.CREDENCIADO_AUTORIZACAO); }
		}

		public static string GetParameter(ParametroType parametro) {
			string value = null;
			try {
				LoadConfig();
				List<ParametroVO> lstCache = ListAll();
				ParametroVO p = lstCache.Find(x => x.Id == (int)parametro);
				value = p.Value;
			} catch (Exception ex) {
				log.Error("Falha ao obter parametro " + parametro, ex);
			}
			if (string.IsNullOrEmpty(value))
				value = ConfigurationManager.AppSettings[parametro.ToString()];
			
			return value;
		}

		public static List<ParametroVO> ListAll() {
			LoadConfig();
			return CacheHelper.GetFromCache<List<ParametroVO>>("PARAMETRO");
		}

		#region Cache

		public static void ClearCache() {
			CacheHelper.Clear();
		}

		private static string GetParameter(ParametroType parametro, string defaultValue) {
			string value = GetParameter(parametro);
			
			if (string.IsNullOrEmpty(value))
				value = defaultValue;

			return value;
		}

		private static void LoadConfig() {
			lock ("PARAMETRO") {
				List<ParametroVO> lst = CacheHelper.GetFromCache<List<ParametroVO>>("PARAMETRO");
				if (lst == null) {
					using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
						EvidaConnectionHolder connection = db.CreateConnection();
						lst = ParametroDAO.GetConfigs(db);
						CacheHelper.AddOnCache("PARAMETRO", lst);
					}
				}
			}
			return;
		}
		#endregion

	}
}
