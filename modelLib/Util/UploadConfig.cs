using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util {
	public enum Sistema {
		BENEFICIARIO,
		CREDENCIADO,
		INTRANET
	}
	public sealed class UploadFilePrefix {
		public static readonly UploadFilePrefix GENERIC = new UploadFilePrefix("GEN");
		public static readonly UploadFilePrefix AUTORIZACAO = new UploadFilePrefix("AUT");
		public static readonly UploadFilePrefix ASSINATURA = new UploadFilePrefix("ASS");
		public static readonly UploadFilePrefix REUNIAO = new UploadFilePrefix("REU");
		public static readonly UploadFilePrefix CONSELHO = new UploadFilePrefix("CON");
		public static readonly UploadFilePrefix INDISPONIBILIDADE_REDE = new UploadFilePrefix("INR");
		public static readonly UploadFilePrefix SOLICITACAO_VIAGEM = new UploadFilePrefix("VGM");
		public static readonly UploadFilePrefix MEDICAMENTO_REEMBOLSAVEL = new UploadFilePrefix("MED");
        public static readonly UploadFilePrefix BOLETIM_OCORRENCIA = new UploadFilePrefix("BOL");

		private UploadFilePrefix(string val) {
			Value = val;
		}

		public string Value { get; private set; }

		internal static UploadFilePrefix FromString(string tipo) {
			if (AUTORIZACAO.Value.Equals(tipo)) {
				return AUTORIZACAO;
			}
			if (ASSINATURA.Value.Equals(tipo)) {
				return ASSINATURA;
			}
			if (REUNIAO.Value.Equals(tipo)) {
				return REUNIAO;
			}
			if (CONSELHO.Value.Equals(tipo)) {
				return CONSELHO;
			}
			if (INDISPONIBILIDADE_REDE.Value.Equals(tipo)) {
				return INDISPONIBILIDADE_REDE;
			}
			if (SOLICITACAO_VIAGEM.Value.Equals(tipo)) {
				return SOLICITACAO_VIAGEM;
			}
			if (MEDICAMENTO_REEMBOLSAVEL.Value.Equals(tipo)) {
				return MEDICAMENTO_REEMBOLSAVEL;
			}
            if (BOLETIM_OCORRENCIA.Value.Equals(tipo))
            {
                return BOLETIM_OCORRENCIA;
            }
			return GENERIC;
		}
	}
	public class UploadConfigManager {
		public static UploadConfig GetConfig(string tipo) {
			UploadFilePrefix prefix = UploadFilePrefix.FromString(tipo);
			return GetConfig(prefix);
		}

		public static UploadConfig GetConfig(UploadFilePrefix type) {
			if (type == UploadFilePrefix.AUTORIZACAO ) {
				return new UploadConfig(UploadFilePrefix.AUTORIZACAO)
				{
					FileTypes = "PDF, JPG, JPEG, BMP, PNG, TIFF, DOC, XLS, PPT, DOCX, XLSX, PPTX".Split(',').Select(x => x.Trim()).ToArray(),
					MaxSize = 20 * 1024
				};
			} else if (type == UploadFilePrefix.ASSINATURA) {
				return new UploadConfig(UploadFilePrefix.ASSINATURA)
				{
					FileTypes = "JPG, JPEG, PNG".Split(',').Select(x => x.Trim()).ToArray(),
					MaxSize = 1024
				};
			} else if (type == UploadFilePrefix.INDISPONIBILIDADE_REDE) {
				return new UploadConfig(UploadFilePrefix.INDISPONIBILIDADE_REDE)
				{
					FileTypes = new string[] { "*" },
					MaxSize = 30 * 1024
				};
			} else if (type == UploadFilePrefix.SOLICITACAO_VIAGEM) {
				return new UploadConfig(UploadFilePrefix.SOLICITACAO_VIAGEM) {
					FileTypes = "PDF, JPG, PNG, DOC, DOCX".Split(',').Select(x => x.Trim()).ToArray(),
					MaxSize = 10 * 1024
				};
			} else if (type == UploadFilePrefix.MEDICAMENTO_REEMBOLSAVEL) {
				return new UploadConfig(UploadFilePrefix.MEDICAMENTO_REEMBOLSAVEL) {
					FileTypes = "PDF, JPG, PNG, DOC, DOCX".Split(',').Select(x => x.Trim()).ToArray(),
					MaxSize = 10 * 1024
				};
			}
            else if (type == UploadFilePrefix.BOLETIM_OCORRENCIA)
            {
                return new UploadConfig(UploadFilePrefix.BOLETIM_OCORRENCIA)
                {
                    FileTypes = "PDF, JPG, PNG, DOC, DOCX".Split(',').Select(x => x.Trim()).ToArray(),
                    MaxSize = 10 * 1024
                };
            }
			return new UploadConfig(UploadFilePrefix.GENERIC)
			{
				FileTypes = new string[] { "*" },
				MaxSize = 10 * 1024
			};
		}

		public static string GetPrefix(UploadConfig config, Sistema sistema, string id) {
			string sisPrefix = "";
			switch (sistema) {
				case Sistema.BENEFICIARIO: sisPrefix = "B"; break;
				case Sistema.CREDENCIADO: sisPrefix = "C"; break;
				case Sistema.INTRANET: sisPrefix = "U"; break;
				default: throw new ArgumentOutOfRangeException("sistema", sistema.ToString());
			}
			return config.Prefix + sisPrefix + id;
		}

        public static string GetPrefix(UploadFilePrefix prefix, Sistema sistema, string id)
        {
			UploadConfig config = GetConfig(prefix);
			return GetPrefix(config, sistema, id);
		}
	}
	[Serializable]
	public class UploadConfig {
		public UploadConfig(UploadFilePrefix prefix) {
			FilePrefix = prefix;
		}
		private UploadFilePrefix FilePrefix { get; set; }
		public string[] FileTypes { get; set; }
		public int MaxSize { get; set; } // KB

		public string Prefix {
			get { return FilePrefix.Value; }
		}
	}

}
