using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.DAO.Protheus;

namespace eVidaGeneralLib.BO.Protheus
{
    public class PClienteBO
    {
        private EVidaLog log = new EVidaLog(typeof(PVidaBO));

        private static PClienteBO instance = new PClienteBO();

        public static PClienteBO Instance { get { return instance; } }

        public void SalvarDadosContato(PClienteVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando dados. Cliente: " + vo.Cod);

                PClienteDAO.SalvarDadosContato(vo, db);

                connection.Commit();
            }
        }

        public DataTable Pesquisar(string cpf, string nome, string preenchido)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PClienteDAO.Pesquisar(cpf, nome, preenchido, db);
            }
        }

        public PClienteVO GetById(string id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PClienteDAO.GetById(id, db);
            }
        }

        public void AlterarEnderecoCobranca(string cod, string endcob)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Anterando Endereço. Cliente: " + cod + ", Endereço: " + endcob);

                PClienteDAO.AlterarEnderecoCobranca(cod, endcob, db);

                connection.Commit();
            }
        }
    }
}
