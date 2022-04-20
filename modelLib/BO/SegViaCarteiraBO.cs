using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO
{
    public class SegViaCarteiraBO
    {
        EVidaLog log = new EVidaLog(typeof(SegViaCarteiraBO));

        private static SegViaCarteiraBO instance = new SegViaCarteiraBO();

        public static SegViaCarteiraBO Instance { get { return instance; } }

        public DataTable PesquisarSegViaCarteira(int? cdProtocolo, string cdFuncionario, string protocoloAns, DateTime? dtInicial, DateTime? dtFinal, StatusSegVia? status)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return SolicitacaoSegViaDAO.PesquisarSegViaCarteira(cdProtocolo, cdFuncionario, protocoloAns, dtInicial, dtFinal, status, db);
            }
        }

        public DataTable ListarDadosSegViaCarteira(PUsuarioVO beneficiario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return SolicitacaoSegViaDAO.ListarDadosSegViaCarteira(beneficiario, db);
            }
        }

        public void Salvar(SolicitacaoSegViaCarteiraVO vo, List<SolicitacaoSegViaCarteiraBenefVO> lst, string email, List<ArquivoTelaVO> lstArquivos)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando solicitação " + vo.Codint.Trim() + " - " + vo.Codemp.Trim() + " - " + vo.Matric.Trim() + " - " + email.Trim());

                PUsuarioVO titular = PUsuarioDAO.GetTitular(vo.Codint.Trim(), vo.Codemp.Trim(), vo.Matric.Trim(), db);
                if (titular == null)
                    throw new InvalidOperationException("Funcionário inexistente");

                if (string.IsNullOrEmpty(titular.Email.Trim()) || !titular.Email.Trim().Equals(email, StringComparison.InvariantCultureIgnoreCase))
                {
                    PUsuarioDAO.AlterarEmailTitular(titular.Codint.Trim(), titular.Codemp.Trim(), titular.Matric.Trim(), email.Trim(), db);
                }

                SolicitacaoSegViaDAO.Salvar(vo, lst, db);
                vo = SolicitacaoSegViaDAO.GetById(vo.CdSolicitacao, db);

                if (lstArquivos != null)
                    SalvarArquivos(vo.CdSolicitacao, lstArquivos, db);

                connection.Commit();
            }
        }

        public SolicitacaoSegViaCarteiraVO GetById(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                SolicitacaoSegViaCarteiraVO vo = SolicitacaoSegViaDAO.GetById(id, db);
                return vo;
            }
        }

        public DataTable BuscarBeneficiarios(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return SolicitacaoSegViaDAO.BuscarBeneficiarios(id, db);
            }
        }

        public DataTable BuscarSolicitacoes(PUsuarioVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return SolicitacaoSegViaDAO.BuscarSolicitacoes(vo.Codint, vo.Codemp, vo.Matric, db);
            }
        }

        public DataTable BuscarBeneficiariosPendentes(PUsuarioVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return SolicitacaoSegViaDAO.BuscarBeneficiariosPendentes(vo.Codint, vo.Codemp, vo.Matric, db);
            }
        }

        public void Finalizar(int cdProtocolo, UsuarioVO usuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Finalizando solicitação seg via " + cdProtocolo);

                SolicitacaoSegViaDAO.Finalizar(cdProtocolo, usuario.Id, db);

                connection.Commit();
            }
        }

        public void Cancelar(int cdProtocolo, string motivo, UsuarioVO usuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Cancelando solicitação seg via " + cdProtocolo);

                SolicitacaoSegViaDAO.Cancelar(cdProtocolo, motivo, usuario.Id, db);

                SolicitacaoSegViaCarteiraVO vo = SolicitacaoSegViaDAO.GetById(cdProtocolo, db);
                PUsuarioVO funcVO = PUsuarioDAO.GetTitular(vo.Codint, vo.Codemp, vo.Matric, db);
                EmailUtil.SegundaVia.SendCancelamentoSegViaCarteiraFuncionario(vo, funcVO);

                connection.Commit();
            }
        }

        public void EnviarEmailCriacao(SolicitacaoSegViaCarteiraVO vo, PUsuarioVO funcVO, byte[] anexo)
        {
            EmailUtil.SegundaVia.SendEmailSegVia(vo, funcVO, anexo);
        }

        #region[ARQUIVOS]

        private void SalvarArquivos(int idSegViaCarteira, List<ArquivoTelaVO> lstArquivos, EvidaDatabase db)
        {

            List<SolicitacaoSegViaCarteiraArquivoVO> lstNewFiles = PrepareFiles(idSegViaCarteira, lstArquivos);

            SolicitacaoSegViaDAO.CriarArquivos(idSegViaCarteira, lstNewFiles, db);

            MoveFiles(idSegViaCarteira, lstArquivos);
        }

        private List<SolicitacaoSegViaCarteiraArquivoVO> PrepareFiles(int idSegViaCarteira, List<ArquivoTelaVO> lstArquivos)
        {
            List<SolicitacaoSegViaCarteiraArquivoVO> lstNewFiles = new List<SolicitacaoSegViaCarteiraArquivoVO>();
            foreach (ArquivoTelaVO arq in lstArquivos)
            {
                SolicitacaoSegViaCarteiraArquivoVO fileVO = new SolicitacaoSegViaCarteiraArquivoVO();
                fileVO.IdSegViaCarteira = idSegViaCarteira;
                fileVO.NomeArquivo = arq.NomeTela;
                fileVO.TipoArquivo = (TipoArquivoSolicitacaoSegViaCarteira)Convert.ToInt32(arq.Parameters["TP_ARQUIVO"]);
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

        private void MoveFiles(int idSegViaCarteira, List<ArquivoTelaVO> lstArquivos)
        {
            String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.BOLETIM_OCORRENCIA);
            foreach (ArquivoTelaVO arq in lstArquivos)
            {
                FileUtil.MoverArquivo(idSegViaCarteira.ToString(), null, arq.NomeFisico, dirDestino, arq.NomeTela);
            }
        }

        private void DeleteFiles(IEnumerable<SolicitacaoSegViaCarteiraArquivoVO> lstDel)
        {
            if (lstDel != null && lstDel.Count() > 0)
            {
                String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.BOLETIM_OCORRENCIA);
                foreach (SolicitacaoSegViaCarteiraArquivoVO arq in lstDel)
                {
                    FileUtil.RemoverArquivo(arq.IdSegViaCarteira.ToString(), dirDestino, arq.NomeArquivo);
                }
            }
        }

        public List<SolicitacaoSegViaCarteiraArquivoVO> ListarArquivos(int idSegViaCarteira)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                List<SolicitacaoSegViaCarteiraArquivoVO> lst = SolicitacaoSegViaDAO.ListarArquivos(idSegViaCarteira, db);
                return lst;
            }
        }

        public void RemoverArquivo(SolicitacaoSegViaCarteiraArquivoVO arq)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                if (log.IsDebugEnabled)
                    log.Debug("Removendo arquivo " + arq.IdSegViaCarteira + " - " + arq.NomeArquivo);

                SolicitacaoSegViaDAO.ExcluirArquivo(arq, db);

                List<SolicitacaoSegViaCarteiraArquivoVO> lst = new List<SolicitacaoSegViaCarteiraArquivoVO>();
                lst.Add(arq);
                DeleteFiles(lst);

                connection.Commit();
            }
        }

        #endregion
    }
}
