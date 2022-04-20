using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO
{
    internal class OptumDAO
    {
        internal static DataTable BuscarSolGeraisInterV4(DateTime data, EvidaDatabase db)
        {
            //string sql = "select * from VW_OPTUM_SOL_GERAIS_INTERV4 where dt_atendimento_item >= :data ";
            string sql = "select * from VW_OPTUM_SOL_GERAIS_INTERV4 ";

            List<Parametro> lstParams = new List<Parametro>();
            //lstParams.Add(new Parametro(":data", DbType.Date, data));
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarCadastroAtivos(EvidaDatabase db)
        {
            string sql = "select * from VW_OPTUM_CADASTRO_ATIVOS ";

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

            return dt;
        }

        internal static DataTable BuscarCadastroExclusoes(DateTime data, EvidaDatabase db)
        {
            //string sql = "select * from VW_OPTUM_CADASTRO_EXCLUSOES where dt_termino_vigencia > :data ";
            string sql = "select * from VW_OPTUM_CADASTRO_EXCLUSOES ";

            List<Parametro> lstParams = new List<Parametro>();
            //lstParams.Add(new Parametro(":data", DbType.Date, data));
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarCadastroMovimentacoes(DateTime data, EvidaDatabase db)
        {
            //string sql = "select * from VW_OPTUM_CADASTRO_MOVIMENT where dt_termino_vigencia > :data OR dt_termino_vigencia IS NULL ";
            string sql = "select * from VW_OPTUM_CADASTRO_MOVIMENT ";

            List<Parametro> lstParams = new List<Parametro>();
            //lstParams.Add(new Parametro(":data", DbType.Date, data));
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarInternacoes(DateTime data, EvidaDatabase db)
        {
            //string sql = "select * from VW_OPTUM_INTERNACAO where dt_registro >= :data ";
            string sql = "select * from VW_OPTUM_INTERNACAO ";

            List<Parametro> lstParams = new List<Parametro>();
            //lstParams.Add(new Parametro(":data", DbType.Date, data));
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarMedicamentos(EvidaDatabase db)
        {
            string sql = "select * from VW_OPTUM_MEDICAMENTOS ";

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

            return dt;
        }

    }
}
