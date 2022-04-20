using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.DAO.SCL;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.VO.SCL;
using eVidaGeneralLib.Exceptions;

namespace eVidaGeneralLib.BO.Protheus
{
    public class PUsuarioBO
    {
        private EVidaLog log = new EVidaLog(typeof(PUsuarioBO));

        private static PUsuarioBO instance = new PUsuarioBO();

        public static PUsuarioBO Instance { get { return instance; } }

        public void SalvarDadosPessoais(PUsuarioVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando dados. Funcionário: " + vo.Codint + ", " + vo.Codemp + ", " + vo.Matric);

                HistoricoDadoPessoalDAO.SalvarHistorico(vo, db);
                PUsuarioDAO.SalvarDadosPessoais(vo, db);

                connection.Commit();
            }
        }

        public PUsuarioVO GetUsuario(string codint, string codemp, string matric, string tipreg)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PUsuarioDAO.GetRowById(codint, codemp, matric, tipreg, db);
            }
        }

        public PUsuarioVO GetUsuarioByCartao(string numCartao)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PUsuarioDAO.GetRowByCartao(numCartao, db);
            }
        }

        public DataTable BuscarUsuarios(string codint, string codemp, string matric)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PUsuarioDAO.GetUsuarios(codint, codemp, matric, db);
            }
        }

        public List<PUsuarioVO> ListarUsuarios(string codint, string codemp, string matric)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PUsuarioDAO.ListarUsuarios(codint, codemp, matric, db);
            }
        }

        public List<PUsuarioVO> ListarDependentes(string codint, string codemp, string matric)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PUsuarioDAO.ListarDependentes(codint, codemp, matric, db);
            }
        }

        public PUsuarioVO LogarBeneficiario(string login, string senha)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                SclUsuarioVO uVO = SclUsuarioDAO.Logar(login, senha, db);
                if (uVO == null)
                    return null;

                string codAlternativo = SclUsuarioDAO.GetCodAlternativoBeneficiario(uVO.Login, db);

                if (string.IsNullOrEmpty(codAlternativo))
                    throw new EvidaException("Número da Carteira do beneficiário não encontrado no DOMINIO");

                PUsuarioVO usuarioVO = PUsuarioDAO.GetRowByCartao(codAlternativo, db);

                if (usuarioVO == null)
                    throw new EvidaException("Não há beneficiário não associado ao usuário!");

                return usuarioVO;
            }
        }

        public PUsuarioVO GetTitular(string codint, string codemp, string matric)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PUsuarioDAO.GetTitular(codint, codemp, matric, db);
            }
        }

    }
}
