using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.DAO.HC
{
    internal class ExtratoIrBeneficiarioDAO
    {

        public static List<int> ListarAnosDisponiveis(int cdBeneficiario, Database db)
        {
            string sql = "select DISTINCT EXTRACT(YEAR FROM DT_ANO_MES_REF) AS ANO" +
                "	from isa_hc.hc_extrato_ir_beneficiario " +
                "	WHERE CD_BENEFICIARIO = :cdBeneficiario AND dt_ano_mes_ref < trunc(sysdate,'year')";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":cdBeneficiario", Tipo = DbType.Int32, Value = cdBeneficiario });

            Func<DataRow, int> convert = delegate(DataRow dr)
            {
                return Convert.ToInt32(dr["ANO"]);
            };

            List<int> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
            return lst;
        }

        internal static DataTable RelatorioMensalidade(int cdEmpresa, long matricula, int ano, Database db)
        {
            string xml = "ExtratoMensalidadeIr.xml";

            ReportQueryUtil query = new ReportQueryUtil(xml);

            DateTime dtInicio = new DateTime(ano, 1, 1);
            DateTime dtFim = new DateTime(ano, 12, 31);

            query.AddParameter(cdEmpresa, DbType.Int32, "empresa");
            query.AddParameter(matricula, DbType.Int64, "matricula");
            query.AddParameter(dtInicio, dtFim, DbType.Date, "data");

            string sql = query.BuildFinalQuery();

            List<Parametro> lstParams = query.GetParameters();
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable RelatorioMensalidadeTable(string codint, string codemp, string matric, int ano, Database db)
        {
            StringBuilder v_sql = new StringBuilder();
            v_sql.Append(" select nm_beneficiario, nr_cpf, vl_despesa_copart, vl_despesa_mens, to_char(dt_ano_mes_ref, 'MM/RRRR') mes_ano ");
            v_sql.Append(" from ev_ir_mensalidade ");
            v_sql.Append(" where ba1_codint = :codint and ba1_codemp = :codemp and ba1_matric = :matric ");
            v_sql.Append(" and dt_ano_mes_ref between :data1 AND :data2 ");
            v_sql.Append(" order by ba1_tipusu desc ");

            DateTime dtInicio = new DateTime(ano, 1, 1);
            DateTime dtFim = new DateTime(ano, 12, 31);

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro(":codint", DbType.String, codint));
            lstP.Add(new Parametro(":codemp", DbType.String, codemp));
            lstP.Add(new Parametro(":matric", DbType.String, matric));
            lstP.Add(new Parametro(":data1", DbType.Date, dtInicio));
            lstP.Add(new Parametro(":data2", DbType.Date, dtFim));

            DataTable dt = BaseDAO.ExecuteDataSet(db, v_sql.ToString(), lstP);

            return dt;
        }

        internal static DataTable FuncionariosMensalidade(int ano, Database db)
        {
            string xml = "ExtratoMensalidadeIr.xml";

            ReportQueryUtil query = new ReportQueryUtil(xml);

            DateTime dtInicio = new DateTime(ano, 1, 1);
            DateTime dtFim = new DateTime(ano, 12, 31);

            query.AddParameter(dtInicio, dtFim, DbType.Date, "data");

            string sql = query.BuildFinalQuery();

            sql = "SELECT DISTINCT CD_EMPRESA, CD_FUNCIONARIO FROM (" + sql + ") WHERE CD_CATEGORIA IN (5, 12, 33)";

            List<Parametro> lstParams = query.GetParameters();
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable RelatorioReembolsoIr(int cdEmpresa, long matricula, int ano, Database db)
        {
            string xml = "ExtratoReembolsoIr.xml";

            ReportQueryUtil query = new ReportQueryUtil(xml);

            DateTime dtInicio = new DateTime(ano, 1, 1);
            DateTime dtFim = new DateTime(ano, 12, 31);

            query.AddParameter(cdEmpresa, DbType.Int32, "empresa");
            query.AddParameter(matricula, DbType.Int64, "matricula");
            query.AddParameter(dtInicio, dtFim, DbType.Date, "data");

            string sql = query.BuildFinalQuery();

            List<Parametro> lstParams = query.GetParameters();
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable RelatorioReembolsoIrTable(string carteira, int ano, Database db)
        {
            StringBuilder v_sql = new StringBuilder();
            v_sql.Append(" select nm_beneficiario, nr_cpf, nm_razao_social, nr_cnpj_cpf, to_char(dt_ano_mes_ref, 'MM/RRRR') mes_ano, to_char(dt_atendimento_item, 'MM/RRRR') dt_atendimento_item, vl_apresentado, vl_reembolso, vl_diff ");
            v_sql.Append(" from ev_ir_reembolso ");
            v_sql.Append(" where upper(trim(matric_titular)) = upper(trim(replace(:carteira,'.',''))) ");
            v_sql.Append(" and dt_ano_mes_ref between :data1 AND :data2 ");

            DateTime dtInicio = new DateTime(ano, 1, 1);
            DateTime dtFim = new DateTime(ano, 12, 31);

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro(":carteira", DbType.String, carteira));
            lstP.Add(new Parametro(":data1", DbType.Date, dtInicio));
            lstP.Add(new Parametro(":data2", DbType.Date, dtFim));

            DataTable dt = BaseDAO.ExecuteDataSet(db, v_sql.ToString(), lstP);

            return dt;
        }

        internal static DataTable FuncionariosReembolso(int ano, Database db)
        {
            string xml = "ExtratoReembolsoIr.xml";

            ReportQueryUtil query = new ReportQueryUtil(xml);

            DateTime dtInicio = new DateTime(ano, 1, 1);
            DateTime dtFim = new DateTime(ano, 12, 31);

            query.AddParameter(dtInicio, dtFim, DbType.Date, "data");

            string sql = query.BuildFinalQuery();

            sql = "SELECT DISTINCT CD_EMPRESA, CD_FUNCIONARIO FROM (" + sql + ") WHERE CD_CATEGORIA IN (5, 12, 33)";

            List<Parametro> lstParams = query.GetParameters();
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

    }
}
