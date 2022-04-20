using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO
{
    public class ViagemBO
    {
        EVidaLog log = new EVidaLog(typeof(ViagemBO));

        private static ViagemBO instance = new ViagemBO();

        public static ViagemBO Instance { get { return instance; } }

        public DataTable Pesquisar(string matricula, int? cdProtocolo, StatusSolicitacaoViagem? status, int? cdUsuarioCriacao)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return ViagemDAO.Pesquisar(matricula, cdProtocolo, status, cdUsuarioCriacao, db);
            }
        }

        public SolicitacaoViagemVO GetById(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
                if (vo != null)
                {
                    if (!vo.IsExterno)
                    {
                        VO.Protheus.EmpregadoEvidaVO funcVO = DAO.Protheus.EmpregadoEvidaDAO.GetByMatricula(vo.Empregado.Matricula, db);
                        vo.Empregado = funcVO;
                    }
                }
                return vo;
            }
        }

        public void CancelarSolicitacao(int id, string motivo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ViagemDAO.CancelarSolicitacao(id, motivo, db);

                connection.Commit();
            }
        }

        public void CriarSolicitacao(SolicitacaoViagemVO vo, ArquivoTelaVO arqCurso)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                vo.DataCriacao = DateTime.Now;

                ViagemDAO.Criar(vo, db);
                if (arqCurso != null)
                {
                    SalvarArquivo(vo.Id, TipoArquivoViagem.CURSO, arqCurso, db);
                }
                CheckViagemExterno(vo, true, db);

                connection.Commit();
            }
        }

        public void SalvarSolicitacao(SolicitacaoViagemVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                SolicitacaoViagemVO oldVO = ViagemDAO.GetById(vo.Id, db);
                ViagemDAO.SalvarSolicitacao(vo, db);
                bool sendEmail = oldVO.Situacao != StatusSolicitacaoViagem.SOLICITACAO_PENDENTE;
                CheckViagemExterno(vo, sendEmail, db);

                connection.Commit();
            }
        }

        private bool SkipCoordenador(bool isExterno, EmpregadoEvidaVO empregado, out string msg, EvidaDatabase db)
        {
            if (isExterno)
            {
                msg = "EXTERNA";
                return true;
            }
            EmpregadoEvidaVO funcVO = EmpregadoEvidaDAO.GetByMatricula(empregado.Matricula, db);
            string codFuncao = funcVO.CodFuncao;
            if (IsFuncaoSkipCoordenador(codFuncao))
            {
                msg = "FUNÇÃO: " + codFuncao;
                return true;
            }
            msg = string.Empty;
            return false;
        }

        private bool IsFuncaoSkipCoordenador(string funcao)
        {
            string[] FUNCAO_SKIP = new string[] { "00004", "00013", "00014", "00015", "00018", "00025" };
            foreach (string fn in FUNCAO_SKIP)
            {
                if (fn.Equals(funcao)) return true;
            }
            return false;
        }

        private bool SkipDiretor(EmpregadoEvidaVO empregado, out string msg, EvidaDatabase db)
        {
            EmpregadoEvidaVO funcVO = EmpregadoEvidaDAO.GetByMatricula(empregado.Matricula, db);
            string codFuncao = funcVO.CodFuncao;
            if (IsFuncaoSkipDiretor(codFuncao))
            {
                msg = "FUNÇÃO: " + codFuncao;
                return true;
            }
            msg = string.Empty;
            return false;
        }

        private bool IsFuncaoSkipDiretor(string funcao)
        {
            string[] FUNCAO_SKIP = new string[] { "00025" };
            foreach (string fn in FUNCAO_SKIP)
            {
                if (fn.Equals(funcao)) return true;
            }
            return false;
        }

        private void CheckViagemExterno(SolicitacaoViagemVO vo, bool sendEmail, EvidaDatabase db)
        {
            vo = ViagemDAO.GetById(vo.Id, db);

            // Aprovação automática do coordenador
            string msgJust = string.Empty;
            if (SkipCoordenador(vo.IsExterno, vo.Empregado, out msgJust, db))
            {
                string justificativa = "APROVAÇÃO AUTOMÁTICA";
                justificativa += " - " + msgJust;
                ViagemDAO.SalvarSolicitacaoAprovCoordenador(vo.Id, true, justificativa, 0, db);

                // Aprovação automática do diretor
                msgJust = string.Empty;
                if (SkipDiretor(vo.Empregado, out msgJust, db))
                {
                    justificativa = "APROVAÇÃO AUTOMÁTICA";
                    justificativa += " - " + msgJust;
                    ViagemDAO.SalvarSolicitacaoAprovDiretoria(vo.Id, true, justificativa, 0, db);

                    if (sendEmail)
                    {
                        vo.Situacao = StatusSolicitacaoViagem.SOLICITACAO_APROVADO_DIRETORIA;
                        EnviarEmailRealizarCompra(vo.Id, db);
                    }

                }
                else
                {
                    if (sendEmail)
                    {
                        vo.Situacao = StatusSolicitacaoViagem.SOLICITACAO_APROVADO_COORDENADOR;
                        EnviarEmailSolicitacaoDiretorExterno(vo, db);
                    }
                }

            }
            else
            {
                if (sendEmail)
                {
                    EnviarEmailSolicitacaoCoordenador(vo, db);
                }
            }
        }

        #region Emails

        private void EnviarEmailSolicitacaoCoordenador(SolicitacaoViagemVO vo, EvidaDatabase db)
        {
            if (vo.IsExterno)
            {
                log.Error("Não era pra solicitar envio de coordenador para viagem externa: " + vo.Id);
                return;
            }

            EmpregadoEvidaVO coordenador = EmpregadoEvidaDAO.GetCoordenador(vo.Empregado.Matricula, vo.Empregado.CodFuncao, db);
            if (coordenador == null)
            {
                throw new Exception("Não foi possível encontrar o coordenador do funcionário " + vo.Empregado.Matricula);
            }
            if (string.IsNullOrEmpty(coordenador.Email))
            {
                throw new Exception("O coordenador " + coordenador.Nome + " não possui email cadastrado!");
            }
            EmailUtil.Viagem.SendViagemCriacao(vo, coordenador);

        }

        private void EnviarEmailSolicitacaoDiretorInterno(SolicitacaoViagemVO vo, EvidaDatabase db)
        {
            if (vo.IsExterno)
            {
                log.Error("Não era pra solicitar envio de diretor interno para viagem externa: " + vo.Id);
                return;
            }

            EmpregadoEvidaVO diretor = EmpregadoEvidaDAO.GetDiretor(vo.Empregado.Matricula, vo.Empregado.CodFuncao, db);
            if (diretor == null)
            {
                throw new Exception("Não foi possível encontrar o diretor do funcionário " + vo.Empregado.Matricula);
            }
            if (string.IsNullOrEmpty(diretor.Email))
            {
                throw new Exception("O diretor " + diretor.Nome + " não possui email cadastrado!");
            }
            EmailUtil.Viagem.SendViagemCriacao(vo, diretor);

        }

        private void EnviarEmailSolicitacaoDiretorExterno(SolicitacaoViagemVO vo, EvidaDatabase db)
        {
            if (!vo.IsExterno)
            {
                log.Error("Não era pra solicitar envio de diretor externo para viagem interna: " + vo.Id);
                return;
            }

            List<EmpregadoEvidaVO> lstDiretores = EmpregadoEvidaDAO.ListarDiretores(db);
            if (lstDiretores == null || lstDiretores.Count == 0)
            {
                throw new Exception("Não foi possível encontrar os diretores da E-VIDA no Protheus.");
            }
            EmailUtil.Viagem.SendViagemCriacaoExterno(vo, lstDiretores);
        }

        private void EnviarEmailSolAprovCancelCoordenador(SolicitacaoViagemVO vo, bool aprovado, string mensagem, EvidaDatabase db)
        {
            UsuarioVO usuario = UsuarioDAO.GetUsuarioById(vo.CodUsuarioSolicitante, db);

            if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
            {
                EmailUtil.Viagem.SendViagemSolAprovNegarCoordenador(vo, usuario, aprovado, mensagem);
            }
        }

        private void EnviarEmailSolAprovCancelDiretoria(int id, bool aprovado, string mensagem, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
            UsuarioVO usuario = UsuarioDAO.GetUsuarioById(vo.CodUsuarioSolicitante, db);

            if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
            {
                EmailUtil.Viagem.SendViagemSolAprovNegarDiretoria(vo, usuario, aprovado, mensagem);
            }
        }

        private void EnviarEmailPcAprovCancelFinanceiro(int id, bool aprovado, string mensagem, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
            UsuarioVO usuario = UsuarioDAO.GetUsuarioById(vo.CodUsuarioSolicitante, db);

            if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
            {
                EmailUtil.Viagem.SendViagemPcAprovNegarFinanceiro(vo, usuario, aprovado, mensagem);
            }
        }

        private void EnviarEmailPcPendenteDiretoria(int id, byte[] relatorio, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
            List<EmpregadoEvidaVO> lstDiretores = new List<EmpregadoEvidaVO>();
            if (!vo.IsExterno)
            {
                EmpregadoEvidaVO diretor = EmpregadoEvidaDAO.GetDiretor(vo.Empregado.Matricula, vo.Empregado.CodFuncao, db);
                lstDiretores.Add(diretor);
            }
            else
            {
                lstDiretores = EmpregadoEvidaDAO.ListarDiretores(db);
                if (lstDiretores == null || lstDiretores.Count == 0)
                {
                    throw new Exception("Não foi possível encontrar os diretores da E-VIDA no Protheus.");
                }
            }

            if (lstDiretores.Count != 0)
            {
                EmailUtil.Viagem.SendViagemPcPendenteDiretoria(vo, lstDiretores, relatorio);
            }
        }

        private void EnviarEmailRealizarCompra(int id, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
            List<UsuarioVO> lstSecretaria = UsuarioBO.Instance.GetUsuariosByModulo(Modulo.VIAGEM_SECRETARIA);
            if (lstSecretaria != null && lstSecretaria.Count > 0)
            {
                EmailUtil.Viagem.SendViagemRealizarCompra(vo, lstSecretaria);
            }
        }

        private void EnviarEmailCompraEfetuada(int idViagem, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(idViagem, db);
            EmailUtil.Viagem.SendViagemCompraEfetuada(vo);
        }

        private void EnviarEmailFinanceiroPago(int idViagem, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(idViagem, db);
            UsuarioVO usuario = UsuarioDAO.GetUsuarioById(vo.CodUsuarioSolicitante, db);
            EmailUtil.Viagem.SendViagemFinanceiroPago(vo, usuario);
        }

        #endregion

        public void AprovarSolicitacaoCoordenador(int id, bool aprovado, string justificativa, int idUsuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ViagemDAO.SalvarSolicitacaoAprovCoordenador(id, aprovado, justificativa, idUsuario, db);

                SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
                EnviarEmailSolAprovCancelCoordenador(vo, aprovado, justificativa, db);
                if (aprovado)
                    EnviarEmailSolicitacaoDiretorInterno(vo, db);
                connection.Commit();
            }
        }

        public void AprovarSolicitacaoDiretoria(int id, bool aprovado, string justificativa, int idUsuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ViagemDAO.SalvarSolicitacaoAprovDiretoria(id, aprovado, justificativa, idUsuario, db);
                EnviarEmailSolAprovCancelDiretoria(id, aprovado, justificativa, db);
                if (aprovado)
                {
                    EnviarEmailRealizarCompra(id, db);
                }
                connection.Commit();
            }
        }

        public void AprovarPrestacaoContaFinanceiro(int id, bool aprovado, string justificativa, int idUsuario, byte[] relatorio)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (aprovado)
                {
                    List<SolicitacaoViagemDespesaDetalhadaVO> lstDespDet = ViagemDAO.ListarDespesasDetalhadas(id, db);
                    if (lstDespDet != null && lstDespDet.Count > 0)
                    {
                        if (lstDespDet.Where(x => x.DataConferido == null).Count() > 0)
                        {
                            throw new EvidaException("Não é possível aprovar prestação de conta, pois existem despesas que ainda não foram conferidas!");
                        }
                    }
                }

                ViagemDAO.SalvarPrestContaAprovFinanceiro(id, aprovado, justificativa, idUsuario, db);
                EnviarEmailPcAprovCancelFinanceiro(id, aprovado, justificativa, db);
                if (aprovado)
                {
                    EnviarEmailPcPendenteDiretoria(id, relatorio, db);
                }
                connection.Commit();
            }
        }

        public void AprovarPrestacaoContaDiretoria(int id, bool aprovado, string justificativa, int idUsuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                ViagemDAO.SalvarPrestContaAprovDiretoria(id, aprovado, justificativa, idUsuario, db);
                EnviarEmailPcAprovCancelDiretoria(id, aprovado, justificativa, db);

                connection.Commit();
            }
        }

        private void EnviarEmailPcAprovCancelDiretoria(int id, bool aprovado, string mensagem, EvidaDatabase db)
        {
            SolicitacaoViagemVO vo = ViagemDAO.GetById(id, db);
            UsuarioVO usuario = UsuarioDAO.GetUsuarioById(vo.CodUsuarioSolicitante, db);

            if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
            {
                EmailUtil.Viagem.SendViagemPcAprovNegarDiretoria(vo, usuario, aprovado, mensagem);
            }
        }

        public void SalvarInfoCompra(SolicitacaoViagemInfoCompraVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                SolicitacaoViagemVO oldVO = ViagemDAO.GetById(vo.IdViagem, db);
                bool changeStatus = (oldVO.Situacao != StatusSolicitacaoViagem.COMPRA_EFETUADA);
                decimal qtdDiarias = ViagemHelper.CalcularQtdTotalDiarias(oldVO.Compra.Passagens);
                ViagemDAO.SalvarInfoCompra(vo, qtdDiarias, changeStatus, db);

                if (changeStatus)
                    EnviarEmailCompraEfetuada(vo.IdViagem, db);

                connection.Commit();
            }
        }

        public List<SolicitacaoViagemItinerarioVO> IncluirItinerario(SolicitacaoViagemItinerarioVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ViagemDAO.IncluirItinerario(vo, db);
                List<SolicitacaoViagemItinerarioVO> lst = ViagemDAO.ListarItinerarios(vo.IdViagem, vo.TipoRegistro, db);

                if (vo.TipoRegistro == TipoItinerarioSolicitacaoViagem.PASSAGEM)
                {
                    decimal qtdDiarias = ViagemHelper.CalcularQtdTotalDiarias(lst);
                    ViagemDAO.SalvarInfoCompra(vo.IdViagem, qtdDiarias, db);
                }

                connection.Commit();
                return lst;
            }
        }

        public List<SolicitacaoViagemItinerarioVO> RemoverItinerario(int idViagem, TipoItinerarioSolicitacaoViagem tipo, int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                SolicitacaoViagemItinerarioVO itinerario = ViagemDAO.GetItinerarioById(idViagem, tipo, id, db);
                if (itinerario.IdArquivo != null)
                {
                    throw new EvidaException("O itinerário não pode ser removido pois possui comprovante anexado!");
                }

                ViagemDAO.RemoverItinerario(idViagem, tipo, id, db);
                List<SolicitacaoViagemItinerarioVO> lst = ViagemDAO.ListarItinerarios(idViagem, tipo, db);
                if (tipo == TipoItinerarioSolicitacaoViagem.PASSAGEM)
                {
                    List<ViagemValorVariavelVO> lstDiarias = ViagemHelper.CalcularDiarias(lst);
                    decimal qtdDiarias = ViagemHelper.CalcularQtdTotalDiarias(lstDiarias);
                    ViagemDAO.SalvarInfoCompra(idViagem, qtdDiarias, db);
                }
                connection.Commit();
                return lst;
            }
        }

        public void SalvarFinanceiroPagamento(SolicitacaoViagemInfoCompraVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                SolicitacaoViagemVO oldVO = ViagemDAO.GetById(vo.IdViagem, db);
                if (oldVO.Situacao == StatusSolicitacaoViagem.COMPRA_EFETUADA)
                    ViagemDAO.SalvarFinanceiroPagamento(vo, db);

                EnviarEmailFinanceiroPago(vo.IdViagem, db);

                connection.Commit();
            }
        }

        public void ConfirmarPagamentoRecebido(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ViagemDAO.ConfirmarPagamentoRecebido(id, db);
                connection.Commit();
            }
        }

        public void SalvarPrestacaoConta(SolicitacaoViagemInfoPrestacaoContaVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ViagemDAO.SalvarPrestacaoConta(vo, db);
                connection.Commit();
            }
        }

        public void SalvarComprovantePagamento(int id, ArquivoTelaVO arq, decimal valorPago)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Salvando comprovante " + id + " - " + arq.NomeTela + " - " + arq.NomeFisico);

                ViagemDAO.SalvarFinanceiroPagamento(id, valorPago, db);
                List<ArquivoTelaVO> lstNew = new List<ArquivoTelaVO>();
                lstNew.Add(arq);

                RemoverAntigos(id, TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA, null, db);

                SalvarArquivos(id, TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA, lstNew, db);
                connection.Commit();
            }
        }

        public void SalvarComprovanteReembolso(int id, ArquivoTelaVO arq)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Salvando comprovante " + id + " - " + arq.NomeTela + " - " + arq.NomeFisico);

                List<ArquivoTelaVO> lstNew = new List<ArquivoTelaVO>();
                lstNew.Add(arq);

                RemoverAntigos(id, TipoArquivoViagem.COMPROVANTE_REEMBOLSO, null, db);

                SalvarArquivos(id, TipoArquivoViagem.COMPROVANTE_REEMBOLSO, lstNew, db);
                connection.Commit();
            }
        }

        public void SalvarRelatorioViagem(int id, ArquivoTelaVO arq)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Salvando relatorio " + id + " - " + arq.NomeTela + " - " + arq.NomeFisico);

                List<ArquivoTelaVO> lstNew = new List<ArquivoTelaVO>();
                lstNew.Add(arq);

                RemoverAntigos(id, TipoArquivoViagem.RELATORIO_VIAGEM, null, db);

                SalvarArquivos(id, TipoArquivoViagem.RELATORIO_VIAGEM, lstNew, db);
                connection.Commit();
            }
        }

        private void RemoverAntigos(int idViagem, TipoArquivoViagem tipo, int? idArquivo, EvidaDatabase db)
        {
            IEnumerable<SolicitacaoViagemArquivoVO> lstOld = ViagemDAO.ListarArquivos(idViagem, db);
            if (lstOld != null)
            {
                lstOld = lstOld.Where(x => x.TipoArquivo == tipo && (idArquivo == null || idArquivo.Value == x.IdArquivo));
                foreach (SolicitacaoViagemArquivoVO arqOld in lstOld)
                {
                    ViagemDAO.ExcluirArquivo(arqOld, db);
                }
                DeleteFiles(idViagem, tipo, lstOld);
            }
        }

        public void RemoverComprovanteReembolso(int idViagem)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Removendo comprovante de reemblso " + idViagem);

                RemoverAntigos(idViagem, TipoArquivoViagem.COMPROVANTE_REEMBOLSO, null, db);
                connection.Commit();
            }
        }

        public void RemoverComprovantePagamento(int idViagem)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Removendo comprovante " + idViagem);

                RemoverAntigos(idViagem, TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA, null, db);
                connection.Commit();
            }
        }

        #region Despesa Detalhada

        public List<SolicitacaoViagemDespesaDetalhadaVO> IncluirDespesaDetalhada(SolicitacaoViagemDespesaDetalhadaVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Incluindo despesa detalhada " + vo.IdViagem + " - " + vo.Data + " - " + vo.Descricao);
                ViagemDAO.IncluirDespesaDetalhada(vo, db);
                List<SolicitacaoViagemDespesaDetalhadaVO> lst = ViagemDAO.ListarDespesasDetalhadas(vo.IdViagem, db);
                connection.Commit();
                return lst;
            }
        }

        public List<SolicitacaoViagemDespesaDetalhadaVO> RemoverDespesaDetalhada(int idViagem, int idDespesa)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Removendo despesa detalhada " + idViagem + " - " + idDespesa);
                ViagemDAO.RemoverDespesaDetalhada(idViagem, idDespesa, db);

                List<SolicitacaoViagemDespesaDetalhadaVO> lst = ViagemDAO.ListarDespesasDetalhadas(idViagem, db);
                connection.Commit();
                return lst;
            }
        }

        public List<SolicitacaoViagemDespesaDetalhadaVO> MarcarDespDetConferido(int idViagem, int idDespesa, bool ok)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Marcar despesa detalhada " + idViagem + " - " + idDespesa + " - " + ok);
                ViagemDAO.MarcarDespesaDetalhadaConferido(idViagem, idDespesa, ok, db);

                List<SolicitacaoViagemDespesaDetalhadaVO> lst = ViagemDAO.ListarDespesasDetalhadas(idViagem, db);
                connection.Commit();
                return lst;
            }
        }

        #endregion

        #region Arquivos

        public List<SolicitacaoViagemArquivoVO> ListarArquivos(int idViagem)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                List<SolicitacaoViagemArquivoVO> lst = ViagemDAO.ListarArquivos(idViagem, db);
                return lst;
            }
        }

        private List<SolicitacaoViagemArquivoVO> SalvarArquivos(int idViagem, TipoArquivoViagem tpArquivo, List<ArquivoTelaVO> lstArquivos, EvidaDatabase db)
        {
            List<SolicitacaoViagemArquivoVO> lstNewFiles = PrepareFiles(idViagem, tpArquivo, lstArquivos);
            ViagemDAO.CriarArquivos(idViagem, tpArquivo, lstNewFiles, db);
            SetIdNewFiles(lstArquivos, lstNewFiles);
            MoveFiles(idViagem, tpArquivo, lstArquivos);
            return lstNewFiles;
        }

        private List<SolicitacaoViagemArquivoVO> PrepareFiles(int idViagem, TipoArquivoViagem tpArquivo, List<ArquivoTelaVO> lstArquivos)
        {
            List<SolicitacaoViagemArquivoVO> lstNewFiles = new List<SolicitacaoViagemArquivoVO>();
            foreach (ArquivoTelaVO arq in lstArquivos)
            {
                SolicitacaoViagemArquivoVO fileVO = new SolicitacaoViagemArquivoVO();
                if (!string.IsNullOrEmpty(arq.Id))
                {
                    fileVO.IdArquivo = Int32.Parse(arq.Id);
                }
                fileVO.IdViagem = idViagem;
                fileVO.NomeArquivo = arq.NomeTela;
                fileVO.TipoArquivo = tpArquivo;
                if (arq.IsNew)
                {
                    fileVO.DataEnvio = DateTime.Now;

                    string diskFile = arq.NomeFisico;
                    if (!FileUtil.HasTempFile(diskFile))
                    {
                        throw new Exception("Arquivo enviado [" + arq.NomeTela + "] não existe em disco (" + diskFile + ")!");
                    }
                    lstNewFiles.Add(fileVO);
                }
            }
            return lstNewFiles;
        }

        private void SetIdNewFiles(List<ArquivoTelaVO> lstArquivos, List<SolicitacaoViagemArquivoVO> lstNewFiles)
        {
            int i = 0;
            foreach (ArquivoTelaVO arq in lstArquivos)
            {
                if (arq.IsNew)
                {
                    SolicitacaoViagemArquivoVO vo = lstNewFiles[i];
                    arq.Id = vo.IdArquivo.ToString();
                    ++i;
                }
            }
        }

        public string GetPrefixTipoArquivo(TipoArquivoViagem tipoArq)
        {
            switch (tipoArq)
            {
                case TipoArquivoViagem.COMPROVANTE_PAGAMENTO_DIARIA: return "CPAG";
                case TipoArquivoViagem.COMPROVANTE_PAGTO_HOTEL: return "CPHOT";
                case TipoArquivoViagem.COMPROVANTE_PAGTO_TRASLADO: return "CPTRA";
                case TipoArquivoViagem.COMPROVANTE_DESPESA: return "CDESP";
                case TipoArquivoViagem.RELATORIO_VIAGEM: return "REL";
                case TipoArquivoViagem.CURSO: return "CURSO";
                case TipoArquivoViagem.COMPROVANTE_REEMBOLSO: return "CREEMB";
                default: throw new InvalidOperationException("Tipo de arquivo sem prefixo: " + tipoArq);
            }
        }

        public string GetFileDiskId(SolicitacaoViagemArquivoVO arq)
        {
            return GetFileDiskId(arq.TipoArquivo, arq.IdArquivo);
        }

        private string GetFileDiskId(TipoArquivoViagem tipoArq, ArquivoTelaVO arq)
        {
            return GetFileDiskId(tipoArq, Int32.Parse(arq.Id));
        }

        private string GetFileDiskId(TipoArquivoViagem tipoArq, int idArq)
        {
            return GetPrefixTipoArquivo(tipoArq) + "_" + idArq;
        }

        private void MoveFiles(int idViagem, TipoArquivoViagem tipoArq, List<ArquivoTelaVO> lstArquivos)
        {
            String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.SOLICITACAO_VIAGEM, idViagem.ToString());
            foreach (ArquivoTelaVO arq in lstArquivos)
            {
                FileUtil.MoverArquivo(GetFileDiskId(tipoArq, arq), null, arq.NomeFisico, dirDestino, arq.NomeTela);
            }
        }

        private void DeleteFiles(int idViagem, TipoArquivoViagem tipo, IEnumerable<SolicitacaoViagemArquivoVO> lstDel)
        {
            if (lstDel != null && lstDel.Count() > 0)
            {
                String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.SOLICITACAO_VIAGEM, idViagem.ToString());
                foreach (SolicitacaoViagemArquivoVO arq in lstDel)
                {
                    FileUtil.RemoverArquivo(GetFileDiskId(arq), dirDestino, arq.NomeArquivo);
                }
            }
        }

        public void SalvarArquivoDespesa(int idViagem, ArquivoTelaVO arq)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                SalvarArquivo(idViagem, TipoArquivoViagem.COMPROVANTE_DESPESA, arq, db);
                connection.Commit();
            }
        }

        public void SalvarArquivoCurso(int idViagem, ArquivoTelaVO arq)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                if (log.IsDebugEnabled)
                    log.Debug("Salvando arquivo curso " + idViagem + " - " + arq.NomeTela + " - " + arq.NomeFisico);

                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                List<ArquivoTelaVO> lstNew = new List<ArquivoTelaVO>();
                lstNew.Add(arq);

                RemoverAntigos(idViagem, TipoArquivoViagem.CURSO, null, db);

                SalvarArquivo(idViagem, TipoArquivoViagem.CURSO, arq, db);
                connection.Commit();
            }
        }

        private List<SolicitacaoViagemArquivoVO> SalvarArquivo(int idViagem, TipoArquivoViagem tipoArq, ArquivoTelaVO arq, EvidaDatabase db)
        {
            if (log.IsDebugEnabled)
                log.Debug("Salvando arquivo " + idViagem + " - " + tipoArq + " - " + arq.NomeTela + " - " + arq.NomeFisico);

            List<ArquivoTelaVO> lst = new List<ArquivoTelaVO>();
            lst.Add(arq);

            return SalvarArquivos(idViagem, tipoArq, lst, db);
        }

        public void RemoverArquivo(SolicitacaoViagemArquivoVO arq)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                RemoverArquivo(arq, true, db);
                connection.Commit();
            }
        }

        private void RemoverArquivo(SolicitacaoViagemArquivoVO arq, bool delete, EvidaDatabase db)
        {
            if (log.IsDebugEnabled)
                log.Debug("Removendo arquivo " + arq.IdViagem + " - " + arq.NomeArquivo);

            if (delete)
                ViagemDAO.ExcluirArquivo(arq, db);

            List<SolicitacaoViagemArquivoVO> lst = new List<SolicitacaoViagemArquivoVO>();
            lst.Add(arq);
            DeleteFiles(arq.IdViagem, arq.TipoArquivo, lst);
        }

        public void IncluirComprovanteItinerario(int idViagem, TipoItinerarioSolicitacaoViagem tipo, int id, ArquivoTelaVO vo)
        {
            TipoArquivoViagem tipoArq = tipo == TipoItinerarioSolicitacaoViagem.HOTEL ? TipoArquivoViagem.COMPROVANTE_PAGTO_HOTEL : TipoArquivoViagem.COMPROVANTE_PAGTO_TRASLADO;

            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                SolicitacaoViagemItinerarioVO itinerarioVO = ViagemDAO.GetItinerarioById(idViagem, tipo, id, db);

                if (itinerarioVO.IdArquivo != null)
                {
                    List<SolicitacaoViagemArquivoVO> lstArquivos = ViagemDAO.ListarArquivos(idViagem, tipoArq, db);
                    SolicitacaoViagemArquivoVO arquivoOld = lstArquivos.Find(x => x.IdArquivo == itinerarioVO.IdArquivo.Value);
                    if (arquivoOld != null)
                    {
                        RemoverArquivo(arquivoOld, false, db);
                        vo.Id = arquivoOld.IdArquivo.ToString();
                    }
                }

                List<SolicitacaoViagemArquivoVO> lstArqs = SalvarArquivo(idViagem, tipoArq, vo, db);
                ViagemDAO.VincularArquivoItinerario(itinerarioVO, lstArqs[0], db);

                connection.Commit();
            }
        }

        #endregion

    }
}
