using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Adesao;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO
{
    public class OptumBO
    {
        EVidaLog log = new EVidaLog(typeof(OptumBO));

        private static OptumBO instance = new OptumBO();

        public static OptumBO Instance { get { return instance; } }

        public DataTable BuscarSolGeraisInterV4(DateTime data)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = OptumDAO.BuscarSolGeraisInterV4(data, db);
                return dt;
            }
        }

        public DataTable BuscarCadastroAtivos()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = OptumDAO.BuscarCadastroAtivos(db);
                return dt;
            }
        }

        public DataTable BuscarCadastroExclusoes(DateTime data)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = OptumDAO.BuscarCadastroExclusoes(data, db);
                return dt;
            }
        }

        public DataTable BuscarCadastroMovimentacoes(DateTime data)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = OptumDAO.BuscarCadastroMovimentacoes(data, db);
                return dt;
            }
        }

        public DataTable BuscarInternacoes(DateTime data)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = OptumDAO.BuscarInternacoes(data, db);
                return dt;
            }
        }

        public DataTable BuscarMedicamentos()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = OptumDAO.BuscarMedicamentos(db);
                return dt;
            }
        }

        public void EnviarEmailEnvolvidos() {

            EmailUtil.Optum.SendInformativoTranferencia();
        }

    }
}
