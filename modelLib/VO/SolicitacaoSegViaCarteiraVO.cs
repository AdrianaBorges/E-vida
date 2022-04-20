using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO
{

    public enum MotivoSegVia
    {
        QUEBRA = 'Q',
        PERDA = 'P',
        ROUBO_FURTO = 'R'
    }

    public enum StatusSegVia
    {
        PENDENTE = 'P',
        FINALIZADO = 'F',
        CANCELADO = 'C'
    }

    public enum TipoArquivoSolicitacaoSegViaCarteira
    {
        BOLETIM_OCORRENCIA
    }

    public class SolicitacaoSegViaCarteiraEnumTradutor
    {
        public static string TraduzMotivo(MotivoSegVia motivo)
        {
            string dsMotivo;
            switch (motivo)
            {
                case MotivoSegVia.QUEBRA:
                    dsMotivo = "Quebra de Cartão";
                    break;
                case MotivoSegVia.PERDA:
                    dsMotivo = "Perda";
                    break;
                case MotivoSegVia.ROUBO_FURTO:
                    dsMotivo = "Roubo/Furto";
                    break;
                default:
                    dsMotivo = "INDEFINIDO";
                    break;
            }
            return dsMotivo;
        }

        public static string TraduzStatus(StatusSegVia status)
        {
            switch (status)
            {
                case StatusSegVia.PENDENTE: return "PENDENTE";
                case StatusSegVia.FINALIZADO: return "FINALIZADO";
                case StatusSegVia.CANCELADO: return "CANCELADO";
                default: return "INDEFINIDO";
            }
        }
    }

    public class SolicitacaoSegViaCarteiraVO
    {
        public int CdSolicitacao { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public DateTime Criacao { get; set; }
        public string Local { get; set; }
        public char Status { get; set; }
        public int? UsuarioAlteracao { get; set; }
        public DateTime? Alteracao { get; set; }
        public string MotivoCancelamento { get; set; }
        public string ProtocoloAns { get; set; }

        public string ToLog()
        {
            return "SolicitacaoSegViaCarteiraVO { CdSolicitacao: " + CdSolicitacao + ", Codint: " + Codint + ", Codemp: " + Codemp + ", Matric: " + Matric + ", Criacao: " + Criacao
                + ", Local: " + Local + ", Status: " + Status + ", UsuarioAlteracao: " + UsuarioAlteracao + ", Alteracao: " + Alteracao + "}";
        }
    }

    public class SolicitacaoSegViaCarteiraBenefVO
    {
        public int CdSolicitacao { get; set; }
        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
        public char Motivo { get; set; }

        public string ToLog()
        {
            return "SolicitacaoSegViaCarteiraBenefVO { CdSolicitacao: " + CdSolicitacao + ", Codint: " + Codint + ", Codemp: " + Codemp + ", Matric: " + Matric + ", Tipreg: " + Tipreg + ", Motivo: " + Motivo + "}";
        }
    }

    public class SolicitacaoSegViaCarteiraArquivoVO
    {
        public int IdSegViaCarteira { get; set; }
        public int IdArquivo { get; set; }
        public TipoArquivoSolicitacaoSegViaCarteira TipoArquivo { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataEnvio { get; set; }
    }

}
