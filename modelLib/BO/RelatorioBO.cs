using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class RelatorioBO {
		private static RelatorioBO instance = new RelatorioBO();

		public static RelatorioBO Instance { get { return instance; } }

		public DataTable BuscarProvisao(bool agrupado, DateTime dtInicio, DateTime dtFim) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.RelatorioProvisao(agrupado, dtInicio, dtFim, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarPagamento(bool agrupado, DateTime dtInicio, DateTime dtFim, string tipoPessoaCred, List<int> lstRegional) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.RelatorioPagamento(agrupado, dtInicio, dtFim, tipoPessoaCred, lstRegional, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarContabilizacao(string tipoSistema, string status, List<int> lstEmpresa, List<int> lstPlanos, List<int> lstCategoria, DateTime dtInicio, DateTime dtFim) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.RelatorioContabilizacao(tipoSistema, status, lstEmpresa, lstPlanos, lstCategoria, dtInicio, dtFim, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable ListarUserUpdateAutorizacao() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.ListarUserUpdateAutorizacao(db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarAutorizacoes(DateTime dtInicio, DateTime dtFim, long? cdMatricula, string titular, string dependente,
			int? nroAutorizacaoIsa, int? nroAutorizacaoWeb, string tipo, string status, string sistemaAtendimento, string cdMascara, string dsServico,
			long? cpf, string nmCredenciado, string userUpdate) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.RelatorioAutorizacoes(dtInicio, dtFim, cdMatricula, titular, dependente,
						nroAutorizacaoIsa, nroAutorizacaoWeb, tipo, status, sistemaAtendimento, cdMascara, dsServico,
						cpf, nmCredenciado, userUpdate, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarUserUpdateAtendimento(List<string> ids) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarUserUpdateAtendimento(ids, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarFaturamentoUsuario(bool byItem, DateTime dtInicio, DateTime dtFim, List<string> lstUsuarios) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.RelatorioFaturamentoUsuario(byItem, dtInicio, dtFim, lstUsuarios, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarMensalidade(DateTime dataReferencia, List<string> lstPlanos, List<int> lstCategoria) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarMensalidade(dataReferencia, lstPlanos, lstCategoria, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarCoparticipacao(DateTime dataInicio, DateTime dataFim, List<string> lstSituacao, string cartaoTitular, bool parcela) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = RelatorioDAO.RelatorioCoparticipacao(dataInicio, dataFim, lstSituacao, cartaoTitular, parcela, db);
				return dt;
			}
		
		}

		DataTable Pivot(DataTable dt, DataColumn pivotColumn, DataColumn pivotValue, out List<string> pivotColumnValues) {
			// find primary key columns 
			//(i.e. everything but pivot column and pivot value)
			DataTable temp = dt.Copy();
			temp.Columns.Remove(pivotColumn.ColumnName);
			temp.Columns.Remove(pivotValue.ColumnName);
			string[] pkColumnNames = temp.Columns.Cast<DataColumn>()
				.Select(c => c.ColumnName)
				.ToArray();

			// prep results table
			DataTable result = temp.DefaultView.ToTable(true, pkColumnNames).Copy();
			result.PrimaryKey = result.Columns.Cast<DataColumn>().ToArray();

			List<string> pValues = new List<string>();

			dt.AsEnumerable()
				.Select(r => r[pivotColumn.ColumnName].ToString())
				.Distinct().ToList()
				.ForEach(c =>
				{
					result.Columns.Add(c, pivotValue.DataType);
					pValues.Add(c);
				});

			pivotColumnValues = pValues;
			
			// load it
			foreach (DataRow row in dt.Rows) {
				// find row to update
				DataRow aggRow = result.Rows.Find(
					pkColumnNames
						.Select(c => row[c])
						.ToArray());

				string columnName = row.Field<string>(pivotColumn.ColumnName);

				// the aggregate used here is LATEST 
				// adjust the next line if you want (SUM, MAX, etc...)
				decimal? value = aggRow.Field<decimal?>(columnName);
				decimal? value2 = row.Field<decimal?>(pivotValue.ColumnName);

				if (value == null) value = value2;
				else if (value2 != null) value += value2;
				
				aggRow[columnName] = value;
			}

			return result;
		}

		public DataTable BuscarBoletosVencimento(DateTime dataReferencia, out List<string> lstCategorias) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarBoletosVencimento(dataReferencia, db);
					if (dt != null && dt.Rows.Count > 0)
						dt = Pivot(dt, dt.Columns["cd_grupo_lancto"], dt.Columns["vl_beneficiario"], out lstCategorias);
					else {
						lstCategorias = new List<string>();
					}
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarBoletosPendentes() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarBoletosPendentes(db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

        public DataTable BuscarBeneficiariosPorLocal(string tipoRelatorio, List<string> lstRegional, List<string> lstPlano, List<string> lstUf, List<string> lstGrauParentesco, bool? deficiente, bool? estudante)
        {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarBeneficiariosPorLocal(tipoRelatorio, lstPlano, lstUf, lstRegional, lstGrauParentesco, deficiente, estudante, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

        public DataTable BuscarBeneficiariosEmLista(List<string> lstRegional, List<string> lstPlano, List<string> lstUf, List<string> lstGrauParentesco, bool? deficiente, bool? estudante)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    DataTable dt = RelatorioDAO.BuscarBeneficiariosEmLista(lstPlano, lstUf, lstRegional, lstGrauParentesco, deficiente, estudante, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public DataTable BuscarCredenciadosEmLista(List<string> lstUf, string status)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    DataTable dt = RelatorioDAO.BuscarCredenciadosEmLista(lstUf, status, db);
                    return dt;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

		public DataTable BuscarCustoInternacao(DateTime? dtRef, string cartao, string nomeBenef,
			int? nroAutorizacao, List<int> lstPlano) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarCustoInternacao(dtRef, cartao, nomeBenef, nroAutorizacao, lstPlano, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarServicoPrestador(string tipo, DateTime? dtInicial, DateTime? dtFinal, List<string> lstMascara, List<int> lstRegional) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarServicoPrestador(tipo, dtInicial, dtFinal, lstMascara, lstRegional, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarFranquiasGeradas(DateTime? dtRef, string cartao, string nomeBenef, int? nroAutorizacao) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarFranquiasGeradas(dtRef, cartao, nomeBenef, nroAutorizacao, db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarParcelamento() {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					DataTable dt = RelatorioDAO.BuscarParcelamento(db);
					return dt;
				}
				finally {
					connection.Close();
				}
			}
		}

		public DataTable BuscarTravamentoISA() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = RelatorioDAO.BuscarTravamentoISA(db);
				return dt;
			}
		}

		public DataTable BuscarBeneficiariosCco(string cco) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = RelatorioDAO.BuscarBeneficiariosCco(cco, db);
				return dt;
			}
		}

		public DataTable DebitosCongelados(DateTime dtRef, string cdPlano) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = RelatorioDAO.DebitosCongelados(dtRef, cdPlano, db);
				return dt;
			}
		}

		public DataTable CreditosBeneficiario(DateTime dataInicio, DateTime dataFim, string cartao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = RelatorioDAO.CreditosBeneficiario(dataInicio, dataFim, cartao, db);
				return dt;
			}
		}
	}
}
