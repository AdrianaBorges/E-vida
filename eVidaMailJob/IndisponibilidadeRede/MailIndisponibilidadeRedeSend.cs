using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Filter;

namespace eVidaMailJob.IndisponibilidadeRede
{
    class MailIndisponibilidadeRedeSend
    {
        const string ALERTA_TOKEN = "ALERTAS";
        const string FORA_TOKEN = "FORA";

        class ParametrosControle
        {
            public ParametrosControle()
            {
                Alertas = new SortedSet<int>();
                Fora = new SortedSet<int>();
            }
            public SortedSet<int> Alertas { get; private set; }
            public SortedSet<int> Fora { get; private set; }
        }

        private EVidaLog log = new EVidaLog(typeof(MailIndisponibilidadeRedeSend));
        private long idProcesso;
        private ParametrosControle parametros;

        public void Run()
        {
            log.Info("Run");
            idProcesso = 0;
            parametros = null;

            try
            {
                idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.INDISPONIBILIDADE_REDE_ALERTA);
                log.Debug("Processo: " + idProcesso);
                int qtd = SendMail();
                log.Debug("QTD: " + qtd);
                ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.INDISPONIBILIDADE_REDE_ALERTA, qtd, ToString(parametros));
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar", ex);
                if (idProcesso != 0)
                    ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.INDISPONIBILIDADE_REDE_ALERTA, ex);
            }
            log.Info("End");
        }

        private int SendMail()
        {
            int enviados = 0;

            DataTable dtPendente = IndisponibilidadeRedeBO.Instance.ListarProtocolosPendentes();
            if (dtPendente == null)
                return 0;

            foreach (DataRow dr in dtPendente.Rows)
            {
                List<IndisponibilidadeRedeVO> lstProtocoloAlerta = new List<IndisponibilidadeRedeVO>();
                List<IndisponibilidadeRedeVO> lstProtocoloFora = new List<IndisponibilidadeRedeVO>();

                EncaminhamentoIndisponibilidadeRede setor = (EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(dr[0]);
                String[] lista_protocolo = Convert.ToString(dr[1]).Split(';');

                foreach(string protocolo in lista_protocolo){

                    IndisponibilidadeRedeVO vo = IndisponibilidadeRedeBO.Instance.GetById(Int32.Parse(protocolo.Trim()));

                    Double horasPendencia = 0;
                    DateTime? inicioPendencia = null;
                    DateTime dataVencimento;

                    dataVencimento = DateUtil.SomarDiasUteis(vo.DataCriacao, vo.DiasPrazo);

                    DataTable dtPendencias = IndisponibilidadeRedeBO.Instance.ListarPendencias(vo.Id);
                    foreach (DataRow linha in dtPendencias.Rows)
                    {
                        DateTime dataEvento = Convert.ToDateTime(linha["DATA"]);
                        string tipoEvento = Convert.ToString(linha["EVENTO"]);

                        if (tipoEvento == "PENDENTE")
                        {
                            if (inicioPendencia == null)
                            {
                                if (dataEvento < dataVencimento)
                                {
                                    inicioPendencia = dataEvento;
                                }

                            }
                        }
                        else if (tipoEvento == "RESOLVIDO")
                        {
                            if (inicioPendencia != null)
                            {
                                if (dataEvento >= dataVencimento)
                                {
                                    horasPendencia += DateUtil.CalcularHorasDiasUteis((DateTime)inicioPendencia, dataVencimento);
                                }
                                else
                                {
                                    horasPendencia += DateUtil.CalcularHorasDiasUteis((DateTime)inicioPendencia, dataEvento);
                                }
                            }

                            inicioPendencia = null;
                        }
                    }

                    if (inicioPendencia != null)
                    {
                        horasPendencia += DateUtil.CalcularHorasDiasUteis((DateTime)inicioPendencia, dataVencimento);
                    }

                    dataVencimento = DateUtil.SomarDiasUteis(dataVencimento, (Int32)Math.Floor(horasPendencia / 24));

                    if(dataVencimento < DateTime.Now.Date){

                        if (DateUtil.SomarDiasUteis(dataVencimento, 2) >= DateTime.Now.Date)
                        {
                            // Faltam menos de dois dias para o prazo
                            lstProtocoloAlerta.Add(vo);
                        }
                        else
                        {
                            // Já passou do prazo
                            lstProtocoloFora.Add(vo);
                        }
                    }

                }

                int count = lstProtocoloFora.Count + lstProtocoloAlerta.Count;
                if (count > 0)
                {
                    if (NaoEnviouHoje())
                    {
                        IndisponibilidadeRedeBO.Instance.EnviarEmailAlerta(setor, lstProtocoloAlerta, lstProtocoloFora);
                    }
                    else
                    {
                        log.Info("Alertas já foram enviados hoje!");
                        return 0;
                    }
                    
                }

                enviados += count;
                
            }

            return enviados;
        }

        private bool NaoEnviouHoje() 
        { 
            ControleProcessoVO controle = ProcessoBO.Instance.GetAnteriorSucesso(ControleProcessoEnum.INDISPONIBILIDADE_REDE_ALERTA);
            if (controle != null)
            {
                if (controle.Inicio.Date == DateTime.Now.Date) {
                    return false;
                }
            }
            return true;
        }

        private bool CheckParametro(List<IndisponibilidadeRedeVO> lstProtocoloAlerta, List<IndisponibilidadeRedeVO> lstProtocoloFora)
        {
            parametros = new ParametrosControle();

            if (lstProtocoloAlerta.Count > 0)
                parametros.Alertas.UnionWith(lstProtocoloAlerta.Select(x => x.Id));
            if (lstProtocoloFora.Count > 0)
                parametros.Fora.UnionWith(lstProtocoloFora.Select(x => x.Id));

            ControleProcessoVO controle = ProcessoBO.Instance.GetAnteriorSucesso(ControleProcessoEnum.INDISPONIBILIDADE_REDE_ALERTA);
            if (controle != null)
            {
                ParametrosControle oldParametros = Parse(controle.Adicional);
                if (SameParametros(parametros, oldParametros))
                    return false;
            }
            return true;
        }

        private bool SameParametros(ParametrosControle p1, ParametrosControle p2)
        {
            if (p1 == null) return false;
            if (p2 == null) return false;

            if (p1.Alertas.Count != p2.Alertas.Count) return false;
            if (p1.Fora.Count != p2.Fora.Count) return false;

            foreach (int id in p1.Alertas)
            {
                if (!p2.Alertas.Contains(id))
                    return false;
            }
            foreach (int id in p1.Fora)
            {
                if (!p2.Fora.Contains(id))
                    return false;
            }
            return true;
        }

        private ParametrosControle Parse(string infoAdicional)
        {
            if (string.IsNullOrEmpty(infoAdicional))
                return null;

            int idx = infoAdicional.IndexOf("$$");
            if (idx < 0)
                return null;
            string[] infoSplit = infoAdicional.Split(new string[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);
            if (infoSplit == null || infoSplit.Length < 3)
                return null;


            string strAlertas = infoSplit[0];
            string strFora = infoSplit[1];

            if (!strAlertas.StartsWith(ALERTA_TOKEN)) return null;
            if (!strFora.StartsWith(FORA_TOKEN)) return null;

            string[] strIdAlertas = ParseToken(strAlertas, ALERTA_TOKEN);
            string[] strIdFora = ParseToken(strFora, FORA_TOKEN);

            ParametrosControle parametro = new ParametrosControle();
            foreach (string strId in strIdAlertas)
            {
                parametro.Alertas.Add(Int32.Parse(strId));
            }
            foreach (string strId in strIdFora)
            {
                parametro.Fora.Add(Int32.Parse(strId));
            }
            return parametro;
        }

        private string[] ParseToken(string fullToken, string tokenName)
        {
            string str = fullToken.Substring(tokenName.Length + 2, fullToken.Length - (tokenName.Length + 2) - 1);
            string[] strIds = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return strIds;
        }

        private string ToString(ParametrosControle parametros)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ALERTA_TOKEN).Append("=[");
            if (parametros != null && parametros.Alertas != null && parametros.Alertas.Count > 0)
                sb.Append(parametros.Alertas.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            sb.Append("]");
            sb.Append("$$");
            sb.Append(FORA_TOKEN).Append("=[");
            if (parametros != null && parametros.Fora != null && parametros.Fora.Count > 0)
                sb.Append(parametros.Fora.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            sb.Append("]");
            return sb.ToString();
        }

    }
}
