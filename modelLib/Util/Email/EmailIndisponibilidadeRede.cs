using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.Util.Email {
	public class EmailIndisponibilidadeRede : EmailProvider {

		#region Indisponibilidade Rede
		internal static void SendIndisponibilidadeRedeEncaminhar(IndisponibilidadeRedeVO vo) {
			MailAddress email;

			switch (vo.SetorEncaminhamento) {
				case EncaminhamentoIndisponibilidadeRede.AUTORIZACAO: email = GetMailToAutoriza(); break;
				case EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO: email = GetMailToCredenciamento(); break;
				case EncaminhamentoIndisponibilidadeRede.DIRETORIA: email = GetMailToDiretoria(); break;
				case EncaminhamentoIndisponibilidadeRede.FATURAMENTO:
					if (vo.AvalFaturamento.Value == AvalIndisponibilidadeRede.FATURAMENTO_FATURAMENTO)
						email = GetMailToFaturamento();
					else email = GetMailToReembolso();
					break;
				case EncaminhamentoIndisponibilidadeRede.FINANCEIRO: email = GetMailToFinanceiro(); break;
				case EncaminhamentoIndisponibilidadeRede.REGIONAL: return; // Quando for regional não encaminha email
				default: email = GetMailToCredenciamento(); break;
			}


			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO));
			parms.Add("$$SETOR_ENCAMINHAMENTO$$", IndisponibilidadeRedeEnumTradutor.TraduzEncaminhamento(vo.SetorEncaminhamento));

			string body = GenerateEmailBody(EmailType.ENVIO_INDISPONIBILIDADE_REDE_ENCAMINHAR, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(email);
			GenericSendEmail(EmailType.ENVIO_INDISPONIBILIDADE_REDE_ENCAMINHAR, col, body);
		}

        internal static void SendIndisponibilidadeRedeCriada(IndisponibilidadeRedeVO vo, PUsuarioVO usuario, string emailStr)
        {
            MailAddress email = new MailAddress(emailStr, usuario.Nomusr);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO));
			string body = GenerateEmailBody(EmailType.ENVIO_INDISPONIBILIDADE_REDE_CRIADA, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(email);
			GenericSendEmail(EmailType.ENVIO_INDISPONIBILIDADE_REDE_CRIADA, col, body);
		}

		internal static void SendIndisponibilidadeRedeEncerrada(IndisponibilidadeRedeVO vo, PUsuarioVO usuario, string emailStr, string motivo) {
			MailAddress email = new MailAddress(emailStr, usuario.Nomusr);

			Dictionary<string, string> parms = new Dictionary<string, string>();
			parms.Add("$$CD_SOLICITACAO$$", vo.Id.ToString(IndisponibilidadeRedeVO.FORMATO_PROTOCOLO));
			parms.Add("$$MOTIVO_ENCERRAMENTO$$", motivo);
			string body = GenerateEmailBody(EmailType.ENVIO_INDISPONIBILIDADE_REDE_ENCERRADA, parms);

			MailAddressCollection col = new MailAddressCollection();
			col.Add(email);
			GenericSendEmail(EmailType.ENVIO_INDISPONIBILIDADE_REDE_ENCERRADA, col, body);
		}

        internal static void SendIndisponibilidadeRedeAlerta(EncaminhamentoIndisponibilidadeRede setor, IEnumerable<IndisponibilidadeRedeVO> lstProtocoloAlerta, IEnumerable<IndisponibilidadeRedeVO> lstProtocoloFora)
        {
            MailAddress email;

            switch (setor)
            {
                case EncaminhamentoIndisponibilidadeRede.AUTORIZACAO: email = GetMailToAutoriza(); break;
                case EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO: email = GetMailToCredenciamento(); break;
                case EncaminhamentoIndisponibilidadeRede.DIRETORIA: email = GetMailToDiretoria(); break;
                case EncaminhamentoIndisponibilidadeRede.FATURAMENTO: email = GetMailToFaturamento(); break;
                case EncaminhamentoIndisponibilidadeRede.FINANCEIRO: email = GetMailToFinanceiro(); break;
                case EncaminhamentoIndisponibilidadeRede.CADASTRO: email = GetMailToCadastro(); break;
                case EncaminhamentoIndisponibilidadeRede.REGIONAL: return; // Quando for regional não encaminha email
                case EncaminhamentoIndisponibilidadeRede.BENEFICIARIO: return; // Quando for beneficiário não encaminha email
                default: email = GetMailToCredenciamento(); break;
            }

            string strAlerta = string.Empty;
            string strFora = string.Empty;

            strAlerta = AggregateIds(lstProtocoloAlerta);
            strFora = AggregateIds(lstProtocoloFora);

            if (string.IsNullOrEmpty(strAlerta) && string.IsNullOrEmpty(strFora))
                return;

            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("$$PROTOCOLO_SETOR$$", setor.ToString());
            parms.Add("$$PROTOCOLO_ALERTA$$", strAlerta);
            parms.Add("$$PROTOCOLO_FORA$$", strFora);
            string body = GenerateEmailBody(EmailType.ENVIO_INDISPONIBILIDADE_REDE_ALERTA, parms);

            MailAddressCollection col = new MailAddressCollection();
            col.Add(email);

            GenericSendEmail(EmailType.ENVIO_INDISPONIBILIDADE_REDE_ALERTA, col, body);
        }

        private static string AggregateIds(IEnumerable<IndisponibilidadeRedeVO> lst)
        {
            string str = string.Empty;

            if (lst.Count() > 0)
            {
                str = lst.Select(x => x.Id.ToString()).Aggregate((x, y) => x + "," + y);
            }
            return str;
        }

		#endregion

	}
}
