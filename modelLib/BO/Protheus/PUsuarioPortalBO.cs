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
    public class PUsuarioPortalBO
    {
        private EVidaLog log = new EVidaLog(typeof(PUsuarioPortalBO));

        private static PUsuarioPortalBO instance = new PUsuarioPortalBO();

        public static PUsuarioPortalBO Instance { get { return instance; } }

        public PUsuarioPortalVO LogarBeneficiario(string login, string senha)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                PUsuarioPortalVO uVO = PUsuarioPortalDAO.LogarBeneficiario(login, senha, db);
                if (uVO == null)
                    return null;

                return uVO;
            }
        }

        public PUsuarioPortalVO LogarCredenciado(string login, string senha)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                PUsuarioPortalVO uVO = PUsuarioPortalDAO.LogarCredenciado(login, senha, db);
                if (uVO == null)
                    return null;

                return uVO;
            }
        }

        public void SalvarDadosContato(PUsuarioPortalVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando dados. Usuário Portal: " + vo.Logusr);

                PUsuarioPortalDAO.SalvarDadosContato(vo, db);

                connection.Commit();
            }
        }
    }
}
