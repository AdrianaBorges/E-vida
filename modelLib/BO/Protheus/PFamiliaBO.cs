using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.DAO.Protheus;

namespace eVidaGeneralLib.BO.Protheus
{
    public class PFamiliaBO
    {
        private EVidaLog log = new EVidaLog(typeof(PFamiliaBO));

        private static PFamiliaBO instance = new PFamiliaBO();

        public static PFamiliaBO Instance { get { return instance; } }

        public PFamiliaVO GetByMatricula(PDados.Empresa empresa, string matricula)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetByMatricula(empresa, matricula, db);
            }
        }

        public PFamiliaVO GetByMatriculaTitular(PDados.Empresa empresa, string matricula, PPessoaVO titular)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetByMatriculaTitular(empresa, matricula, titular, db);
            }
        }

        public PFamiliaVO GetAtualByMatricula(PDados.Empresa empresa, string matricula)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetAtualByMatricula(empresa, matricula, db);
            }
        }

        public PFamiliaVO GetAtualByMatriculaTitular(PDados.Empresa empresa, string matricula, PPessoaVO titular)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetAtualByMatriculaTitular(empresa, matricula, titular, db);
            }
        }

        public PFamiliaVO GetByMatricula(string codint, string codemp, string matric)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetByMatricula(codint, codemp, matric, db);
            }
        }

        public PFamiliaVO GetByContrato(PDados.Empresa empresa, string matricula, string conemp, string subcon)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetByContrato(empresa, matricula, conemp, subcon, db);
            }
        }

        public PFamiliaVO GetByContratoTitular(PDados.Empresa empresa, string matricula, PPessoaVO titular, string conemp, string subcon)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaDAO.GetByContratoTitular(empresa, matricula, titular, conemp, subcon, db);
            }
        }

        public PFamiliaProdutoVO GetFamiliaProduto(string codint, string codemp, string matric, string tipreg)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                PFamiliaProdutoVO benefPlano = PFamiliaProdutoDAO.GetLastFamiliaData(codint, codemp, matric, tipreg, null, db);

                // Solicitado pelo Sr. Eurico, do Cadastro
                // Se o plano da família for o 0001 e o plano do usuário for o 0004, a consulta considera o plano do usuário. Para os demais casos, a consulta considera o plano da família.
                PUsuarioVO usuario = PUsuarioDAO.GetRowById(codint, codemp, matric, tipreg, db);
                if(usuario != null)
                {
                    if (benefPlano.Codpla != null && usuario.Codpla != null)
                    {
                        if (benefPlano.Codpla == "0001" && usuario.Codpla == "0004")
                        {
                            benefPlano.Codpla = "0004";
                        }                          
                    }
                }

                return benefPlano;
            }
        }

        public PFamiliaContratoVO GetFamiliaContrato(string codint, string codemp, string matric, string tipreg)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PFamiliaContratoDAO.GetLastFamiliaData(codint, codemp, matric, tipreg, null, db);
            }
        }

        public void AtualizarDadosBancarios(PFamiliaVO familia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                PFamiliaDAO.AtualizarDadosBancarios(familia, db);
                connection.Commit();
            }
        }

        public void SalvarDadosContato(PFamiliaVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando dados. Família: " + vo.Codint + ", " + vo.Codemp + ", " + vo.Matric);

                PFamiliaDAO.SalvarDadosContato(vo, db);

                connection.Commit();
            }
        }

    }
}
