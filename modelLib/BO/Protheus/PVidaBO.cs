using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.DAO.Protheus;

namespace eVidaGeneralLib.BO.Protheus
{
    public class PVidaBO
    {
        private EVidaLog log = new EVidaLog(typeof(PVidaBO));

        private static PVidaBO instance = new PVidaBO();

        public static PVidaBO Instance { get { return instance; } }

        public List<PVidaVO> ListarVidas(string codint, string codemp, string matric)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PVidaDAO.ListarVidas(codint, codemp, matric, db);
            }
        }

        public PVidaVO GetVida(string matvid)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PVidaDAO.GetRowById(matvid, db);
            }
        }

        public void SalvarDadosContato(PVidaVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                if (log.IsDebugEnabled)
                    log.Debug("Salvando dados. Vida: " + vo.Matvid);

                PVidaDAO.SalvarDadosContato(vo, db);

                connection.Commit();
            }
        }

    }
}
