using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Filter;

namespace eVidaMailJob.Reciprocidade
{
    class MailReciprocidadeSend
    {
        const string ALERTA_TOKEN = "ALERTAS";
        const string CRITICA_TOKEN = "CRITICAS";

        class ParametrosControle
        {
            public ParametrosControle()
            {
                Alertas = new SortedSet<int>();
                Criticas = new SortedSet<int>();
            }
            public SortedSet<int> Alertas { get; private set; }
            public SortedSet<int> Criticas { get; private set; }
        }

        class MapReciprocidade
        {
            public UsuarioVO Usuario { get; set; }
            public List<ReciprocidadeVO> Alertas { get; set; }
            public List<ReciprocidadeVO> Criticas { get; set; }
        }

        private EVidaLog log = new EVidaLog(typeof(MailReciprocidadeSend));
        private long idProcesso;
        private ParametrosControle parametros;

        public void Run()
        {
            log.Info("Run");
            idProcesso = 0;
            parametros = null;

            try
            {
                idProcesso = ProcessoBO.Instance.RegistrarProcesso(ControleProcessoEnum.RECIPROCIDADE_ALERTA);
                log.Debug("Processo: " + idProcesso);
                AtualizarSituacao();
                int qtd = SendMail();
                log.Debug("QTD: " + qtd);
                ProcessoBO.Instance.SucessoProcesso(idProcesso, ControleProcessoEnum.RECIPROCIDADE_ALERTA, qtd, ToString(parametros));
            }
            catch (Exception ex)
            {
                log.Error("Erro ao processar", ex);
                if (idProcesso != 0)
                    ProcessoBO.Instance.ErroProcesso(idProcesso, ControleProcessoEnum.RECIPROCIDADE_ALERTA, ex);
            }
            log.Info("End");
        }

        private void AtualizarSituacao()
        {

            DataTable dtPendente = ReciprocidadeBO.Instance.BuscarEmAndamento();

            if (dtPendente == null)
                return;

            foreach (DataRow dr in dtPendente.Rows)
            {
                ReciprocidadeVO recVO = (ReciprocidadeVO)dr["OBJ"];

                if (recVO.Status == StatusReciprocidade.PENDENTE)
                {

                    if (TranscorreramDias(recVO.DataCriacao, 5))
                    {
                        ReciprocidadeBO.Instance.AtualizarSituacao(recVO.CodSolicitacao, SituacaoReciprocidade.CRITICA);
                    }
                    else if (TranscorreramDias(recVO.DataCriacao, 3))
                    {
                        ReciprocidadeBO.Instance.AtualizarSituacao(recVO.CodSolicitacao, SituacaoReciprocidade.ALERTA);
                    }
                    else
                    {
                        ReciprocidadeBO.Instance.AtualizarSituacao(recVO.CodSolicitacao, SituacaoReciprocidade.NORMAL);
                    }

                }
                else if (recVO.Status == StatusReciprocidade.ENVIADO)
                {
                    if (TranscorreramDias((DateTime)recVO.DataEnvio, 5))
                    {
                        ReciprocidadeBO.Instance.AtualizarSituacao(recVO.CodSolicitacao, SituacaoReciprocidade.CRITICA);
                    }
                    else if (TranscorreramDias((DateTime)recVO.DataEnvio, 3))
                    {
                        ReciprocidadeBO.Instance.AtualizarSituacao(recVO.CodSolicitacao, SituacaoReciprocidade.ALERTA);
                    }
                    else
                    {
                        ReciprocidadeBO.Instance.AtualizarSituacao(recVO.CodSolicitacao, SituacaoReciprocidade.NORMAL);
                    }

                }
            }

        }

        private bool TranscorreramDias(DateTime dataInicial, int numDias)
        {
            DateTime dataLimite = DateUtil.SomarDiasUteis(dataInicial, numDias);

            if (DateTime.Now.Date >= dataLimite.Date)
                return true;

            return false;
        }

        private int SendMail()
        {
            DataTable dtPendente = ReciprocidadeBO.Instance.BuscarEmAndamento();
            if (dtPendente == null)
                return 0;

            List<ReciprocidadeVO> lstReciprocidadeAlerta = new List<ReciprocidadeVO>();
            List<ReciprocidadeVO> lstReciprocidadeCritica = new List<ReciprocidadeVO>();

            foreach (DataRow dr in dtPendente.Rows)
            {
                ReciprocidadeVO recVO = (ReciprocidadeVO)dr["OBJ"];

                if (recVO.Situacao == SituacaoReciprocidade.ALERTA)
                    lstReciprocidadeAlerta.Add(recVO);
                else if (recVO.Situacao == SituacaoReciprocidade.CRITICA)
                    lstReciprocidadeCritica.Add(recVO);
            }

            int count = lstReciprocidadeAlerta.Count + lstReciprocidadeCritica.Count;
            if (count > 0)
            {
                if (!CheckParametro(lstReciprocidadeAlerta, lstReciprocidadeCritica))
                {
                    log.Info("Parâmetros iguais!");
                    return 0;
                }

                // Obtém o dicionário
                Dictionary<int, MapReciprocidade> dicionario = BuildMaps(lstReciprocidadeAlerta, lstReciprocidadeCritica);

                // Para cada usuario responsável no dicionário
                foreach (int idUsuario in dicionario.Keys)
                {
                    // Obtém o mapa deste usuário responsável
                    MapReciprocidade mapa = dicionario[idUsuario];

                    // Obtém o usuário completo deste mapa
                    UsuarioVO usuario = mapa.Usuario;

                    // Obtém a lista de reciprocidades em situação de alerta deste mapa
                    List<ReciprocidadeVO> lstAlertas = mapa.Alertas;

                    // Obtém a lista de reciprocidades em situação crítica deste mapa
                    List<ReciprocidadeVO> lstCriticas = mapa.Criticas;

                    try
                    {
                        ReciprocidadeBO.Instance.EnviarEmailAlerta(usuario, lstAlertas, lstCriticas);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Erro ao enviar alertas ao usuário " + usuario.Login, ex);
                    }

                }

            }
            return count;
        }

        private Dictionary<int, MapReciprocidade> BuildMaps(List<ReciprocidadeVO> lstAlertas, List<ReciprocidadeVO> lstCriticas)
        {
            // Dicionário
            Dictionary<int, MapReciprocidade> dicionario = new Dictionary<int, MapReciprocidade>();

            // Lista de todos os usuários
            List<UsuarioVO> lstUsuarios = UsuarioBO.Instance.ListarUsuarios();

            #region[COORDENADOR]

            // Lista de coordenadores que deverão receber os emails
            List<string> lstEmailCoordenador = new List<string>();
            List<int> lstIdCoordenador = new List<int>();

            lstEmailCoordenador.Add("marcio.souza@e-vida.org.br");
            lstEmailCoordenador.Add("wagner.rezende@e-vida.org.br");
            lstEmailCoordenador.Add("thiago.santana@e-vida.org.br");
            lstEmailCoordenador.Add("evando.junior@e-vida.org.br");

            // Para cada email de coordenador
            foreach (string emailCoordenador in lstEmailCoordenador)
            {

                // Cria um novo mapa
                MapReciprocidade mapaCoordenador = new MapReciprocidade();

                // Obtém os dados completos desse usuário coordenador
                UsuarioVO usuarioCoordenador = lstUsuarios.Find(x => x.Email.Trim() == emailCoordenador);

                if (usuarioCoordenador == null) continue;

                // Adiciona o Id do coordenador à lista, pra permitir uma busca pelo Id
                lstIdCoordenador.Add(usuarioCoordenador.Id);

                // Atribui o usuário coordenador ao mapa
                mapaCoordenador.Usuario = usuarioCoordenador;

                // Cria a lista de reciprocidades em situação de alerta no mapa
                mapaCoordenador.Alertas = new List<ReciprocidadeVO>();

                // Cria a lista de reciprocidades em situação crítica no mapa
                mapaCoordenador.Criticas = new List<ReciprocidadeVO>();

                // Adiciona o mapa do coordenador ao dicionário
                dicionario.Add(usuarioCoordenador.Id, mapaCoordenador);

            }

            #endregion

            #region[ALERTAS]

            // Para cada reciprocidade em situação de alerta
            foreach (ReciprocidadeVO reciprocidade in lstAlertas)
            {
                // Se a reciprocidade tiver um responsável (quem fez o envio) e se esse responsável não for um dos coordenadores
                if (reciprocidade.CodUsuarioEnvio != 0 && !lstIdCoordenador.Contains(reciprocidade.CodUsuarioEnvio))
                {
                    // Mapa
                    MapReciprocidade mapa = null;

                    // Usuário responsável
                    int idUsuario = reciprocidade.CodUsuarioEnvio;

                    // Se o dicionário já contém esse usuário responsável
                    if (dicionario.ContainsKey(idUsuario))
                    {
                        // Obtém o mapa deste usuário
                        mapa = dicionario[idUsuario];
                    }
                    else
                    {
                        // Cria um novo mapa
                        mapa = new MapReciprocidade();

                        // Obtém os dados completos deste usuário responsável
                        UsuarioVO usuario = lstUsuarios.Find(x => x.Id == idUsuario);

                        // Atribui o usuário ao mapa
                        mapa.Usuario = usuario;

                        // Cria a lista de reciprocidades em situação de alerta no mapa
                        mapa.Alertas = new List<ReciprocidadeVO>();

                        // Cria a lista de reciprocidades em situação crítica no mapa
                        mapa.Criticas = new List<ReciprocidadeVO>();

                        // Adiciona o mapa ao dicionário
                        dicionario.Add(idUsuario, mapa);
                    }

                    // Adiciona esta reciprocidade à lista de reciprocidades em situação de alerta do mapa
                    mapa.Alertas.Add(reciprocidade);
                }

                // Os coordenadores recebem todas as reciprocidades em situação de alerta
                foreach (int idCoordenador in lstIdCoordenador)
                {
                    dicionario[idCoordenador].Alertas.Add(reciprocidade);
                }

            }

            #endregion

            #region[CRÍTICAS]

            // Para cada reciprocidade em situação crítica
            foreach (ReciprocidadeVO reciprocidade in lstCriticas)
            {
                // Se a reciprocidade tiver um responsável (quem fez o envio) e se esse responsável não for o coordenador
                if (reciprocidade.CodUsuarioEnvio != 0 && !lstIdCoordenador.Contains(reciprocidade.CodUsuarioEnvio))
                {
                    // Mapa
                    MapReciprocidade mapa = null;

                    // Usuário responsável
                    int idUsuario = reciprocidade.CodUsuarioEnvio;

                    // Se o dicionário já contém esse usuário responsável
                    if (dicionario.ContainsKey(idUsuario))
                    {
                        // Obtém o mapa deste usuário
                        mapa = dicionario[idUsuario];
                    }
                    else
                    {
                        // Cria um novo mapa
                        mapa = new MapReciprocidade();

                        // Obtém os dados completos deste usuário responsável
                        UsuarioVO usuario = lstUsuarios.Find(x => x.Id == idUsuario);

                        // Atribui o usuário ao mapa
                        mapa.Usuario = usuario;

                        // Cria a lista de reciprocidades em situação de alerta no mapa
                        mapa.Alertas = new List<ReciprocidadeVO>();

                        // Cria a lista de reciprocidades em situação crítica no mapa
                        mapa.Criticas = new List<ReciprocidadeVO>();

                        // Adiciona o mapa ao dicionário
                        dicionario.Add(idUsuario, mapa);
                    }

                    // Adiciona esta reciprocidade à lista de reciprocidades em situação crítica do mapa
                    mapa.Criticas.Add(reciprocidade);
                }

                // Os coordenadores recebem todas as reciprocidades em situação crítica
                foreach (int idCoordenador in lstIdCoordenador)
                {
                    dicionario[idCoordenador].Criticas.Add(reciprocidade);
                }
            }

            #endregion

            return dicionario;
        }

        private bool CheckParametro(List<ReciprocidadeVO> lstReciprocidadeAlerta, List<ReciprocidadeVO> lstReciprocidadeCritica)
        {
            parametros = new ParametrosControle();
            if (lstReciprocidadeAlerta.Count > 0)
                parametros.Alertas.UnionWith(lstReciprocidadeAlerta.Select(x => x.CodSolicitacao));
            if (lstReciprocidadeCritica.Count > 0)
                parametros.Criticas.UnionWith(lstReciprocidadeCritica.Select(x => x.CodSolicitacao));

            ControleProcessoVO controle = ProcessoBO.Instance.GetAnteriorSucesso(ControleProcessoEnum.RECIPROCIDADE_ALERTA);
            if (controle != null)
            {
                ParametrosControle oldParametros = Parse(controle.Adicional);
                if (SameParametros(parametros, oldParametros))
                    return false;
            }
            return true;
        }

        private string[] ParseToken(string fullToken, string tokenName)
        {
            string str = fullToken.Substring(tokenName.Length + 2, fullToken.Length - (tokenName.Length + 2) - 1);
            string[] strIds = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return strIds;
        }

        private ParametrosControle Parse(string infoAdicional)
        {
            //ALERTAS=[123]$$CRITICAS=[145]
            if (string.IsNullOrEmpty(infoAdicional))
                return null;

            int idx = infoAdicional.IndexOf("$$");
            if (idx < 0)
                return null;
            string[] infoSplit = infoAdicional.Split(new string[] { "$$" }, StringSplitOptions.RemoveEmptyEntries);
            if (infoSplit == null || infoSplit.Length < 2)
                return null;

            string strAlertas = infoSplit[0];
            string strCriticas = infoSplit[1];

            if (!strAlertas.StartsWith(ALERTA_TOKEN)) return null;
            if (!strCriticas.StartsWith(CRITICA_TOKEN)) return null;

            string[] strIdAlertas = ParseToken(strAlertas, ALERTA_TOKEN);
            string[] strIdCriticas = ParseToken(strCriticas, CRITICA_TOKEN);

            ParametrosControle parametro = new ParametrosControle();
            foreach (string strId in strIdAlertas)
            {
                parametro.Alertas.Add(Int32.Parse(strId));
            }
            foreach (string strId in strIdCriticas)
            {
                parametro.Criticas.Add(Int32.Parse(strId));
            }
            return parametro;
        }

        private string ToString(ParametrosControle parametros)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ALERTA_TOKEN).Append("=[");
            if (parametros != null && parametros.Alertas != null && parametros.Alertas.Count > 0)
                sb.Append(parametros.Alertas.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            sb.Append("]");
            sb.Append("$$");
            sb.Append(CRITICA_TOKEN).Append("=[");
            if (parametros != null && parametros.Criticas != null && parametros.Criticas.Count > 0)
                sb.Append(parametros.Criticas.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y));
            sb.Append("]");
            return sb.ToString();
        }

        private bool SameParametros(ParametrosControle p1, ParametrosControle p2)
        {
            if (p1 == null) return false;
            if (p2 == null) return false;

            if (p1.Alertas.Count != p2.Alertas.Count) return false;
            if (p1.Criticas.Count != p2.Criticas.Count) return false;

            foreach (int id in p1.Alertas)
            {
                if (!p2.Alertas.Contains(id))
                    return false;
            }
            foreach (int id in p1.Criticas)
            {
                if (!p2.Criticas.Contains(id))
                    return false;
            }
            return true;
        }
    }
}
