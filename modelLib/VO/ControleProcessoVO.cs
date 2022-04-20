using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO {
	public enum StatusControleProcesso {
		EXECUTANDO,
		SUCESSO,
		ERRO,
		ABORTADO
	}
	public enum ControleProcessoEnum {
		AUTORIZACAO_ALERTA,
		ROTINA_BANCO,
		DEBITO_ANUAL,
		CONTROLE_EMAIL,
		BILHETAGEM,
        PROTOCOLO_FATURA_ALERTA,
        RECIPROCIDADE_ALERTA,
        RECIPROCIDADE_RENOVACAO_PERGUNTA,
        RECIPROCIDADE_RENOVACAO_RESPOSTA,
        GERACAO_ENVIO_ARQUIVO_OPTUM,
        INDISPONIBILIDADE_REDE_ALERTA
	}

	public class ControleProcessoVO {
		public long Id { get; set; }
		public ControleProcessoEnum Processo { get; set; }
		public StatusControleProcesso Status { get; set; }
		public DateTime Inicio { get; set; }
		public DateTime? Fim { get; set; }
		public int Quantidade { get; set; }
		public string Adicional { get; set; }
	}
}
