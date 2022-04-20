using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.Util;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace eVidaGeneralLib.BO
{
    public class ExtratoIrBeneficiarioBO
    {
        private static ExtratoIrBeneficiarioBO instance = new ExtratoIrBeneficiarioBO();

        public static ExtratoIrBeneficiarioBO Instance { get { return instance; } }

        public List<int> ListarAnosDisponiveis(int codBeneficiario)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();

                try
                {
                    return ExtratoIrBeneficiarioDAO.ListarAnosDisponiveis(codBeneficiario, db);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable RelatorioMensalidade(int cdEmpresa, long matricula, int ano)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();

                try
                {
                    DataTable dt = ExtratoIrBeneficiarioDAO.RelatorioMensalidade(cdEmpresa, matricula, ano, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable RelatorioMensalidadeTable(string codint, string codemp, string matric, int ano)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();

                try
                {
                    DataTable dt = ExtratoIrBeneficiarioDAO.RelatorioMensalidadeTable(codint, codemp, matric, ano, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public string GetIrDir(int ano)
        {
            string dir = ParametroUtil.IrFolderBeneficiario;
            dir = System.IO.Path.Combine(dir, ano.ToString());
            dir = System.IO.Path.GetFullPath(dir);
            return dir;
        }

        public string RelatorioMensalidadeFile(string cartaoTitular, int ano)
        {
            string irDir = GetIrDir(ano);
            irDir = System.IO.Path.Combine(irDir, "MENSALIDADE");
            string path = System.IO.Path.Combine(irDir, cartaoTitular + ".pdf");
            if (System.IO.File.Exists(path))
                return path;
            return null;
        }

        public DataTable FuncionariosMensalidade(int ano)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();

                try
                {
                    DataTable dt = ExtratoIrBeneficiarioDAO.FuncionariosMensalidade(ano, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable FuncionariosReembolso(int ano)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();

                try
                {
                    DataTable dt = ExtratoIrBeneficiarioDAO.FuncionariosReembolso(ano, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable TotalizarMensalidade(DataTable dt)
        {
            DataTable dtTotal = new DataTable();
            dtTotal.Columns.Add("nm_beneficiario", typeof(string));
            dtTotal.Columns.Add("vl_despesa_copart", typeof(double));
            dtTotal.Columns.Add("vl_despesa_mens", typeof(double));
            dtTotal.Columns.Add("nr_cpf", typeof(string));

            var sumarizado = from row in dt.AsEnumerable()
                             group row by new
                             {
                                 Nome = row.Field<string>("nm_beneficiario"),
                                 Cpf = row.Field<string>("nr_cpf")
                             } into gcs
                             select new
                             {
                                 Nome = gcs.Key.Nome,
                                 Cpf = gcs.Key.Cpf,
                                 ValorCopart = gcs.Sum(x => x.Field<decimal>("vl_despesa_copart")),
                                 ValorMensal = gcs.Sum(x => x.Field<decimal>("vl_despesa_mens"))
                             };

            foreach (var obj in sumarizado)
            {
                DataRow newRow = dtTotal.NewRow();
                newRow["nm_beneficiario"] = obj.Nome;
                newRow["vl_despesa_copart"] = obj.ValorCopart;
                newRow["vl_despesa_mens"] = obj.ValorMensal;
                newRow["nr_cpf"] = obj.Cpf;
                dtTotal.Rows.Add(newRow);
            }

            return dtTotal;
        }

        public DataTable RelatorioReembolsoIr(int cdEmpresa, long matricula, int ano)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    DataTable dt = ExtratoIrBeneficiarioDAO.RelatorioReembolsoIr(cdEmpresa, matricula, ano, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable RelatorioReembolsoIrTable(string carteira, int ano)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    DataTable dt = ExtratoIrBeneficiarioDAO.RelatorioReembolsoIrTable(carteira, ano, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
