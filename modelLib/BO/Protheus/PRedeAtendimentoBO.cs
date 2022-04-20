using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO.SCL;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.VO.SCL;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO.Protheus
{
    public class PRedeAtendimentoBO
    {
        EVidaLog log = new EVidaLog(typeof(PRedeAtendimentoBO));

        private static PRedeAtendimentoBO instance = new PRedeAtendimentoBO();

        public static PRedeAtendimentoBO Instance { get { return instance; } }

        public PRedeAtendimentoVO LogarRedeAtendimento(string login, string senha)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();

                SclUsuarioVO uVO = SclUsuarioDAO.Logar(login, senha, db);
                if (uVO == null)
                    return null;
                long cpfCnpj;
                if (!Int64.TryParse(uVO.Login, out cpfCnpj))
                {
                    return new PRedeAtendimentoVO();
                }

                PRedeAtendimentoVO vo = PRedeAtendimentoDAO.GetByDoc(uVO.Login, db);
                if (vo == null)
                    return new PRedeAtendimentoVO();
                return vo;
            }
        }

        public PRedeAtendimentoVO GetByDoc(string cpfCnpj)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PRedeAtendimentoDAO.GetByDoc(cpfCnpj, db);
            }
        }

        public PRedeAtendimentoVO GetByName(string razaoSocial)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PRedeAtendimentoDAO.GetByName(razaoSocial, db);
            }
        }

        public PRedeAtendimentoVO GetById(string codigo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PRedeAtendimentoDAO.GetById(codigo, db);
            }
        }

        public KeyValuePair<string, string> GetRegiaoRedeAtendimento(string codigo, DateTime? dtVigencia = null)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();

                string codreg = PRedeAtendimentoDAO.GetRegiaoRedeAtendimento(codigo, dtVigencia, db);
                string descri = PLocatorDataBO.Instance.GetRegiao(codreg, db);
                return new KeyValuePair<string, string>(codreg, descri);
            }
        }

        public DataTable Pesquisar(string razaoSocial, string cpfCnpj, bool apenasHospital)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PRedeAtendimentoDAO.Pesquisar(razaoSocial, cpfCnpj, apenasHospital, db);
            }
        }
    }
}
