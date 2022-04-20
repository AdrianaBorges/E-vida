using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class LocatorDataBO {
		EVidaLog log = new EVidaLog(typeof(LocatorDataBO));
		private static LocatorDataBO instance = new LocatorDataBO();

		public static LocatorDataBO Instance { get { return instance; } }

		private void AddOnCache(string name, object value) {
			CacheHelper.AddOnCache(name, value);
		}

		private object GetFromCache(string name) {
			return CacheHelper.GetFromCache<object>(name);
		}

		public DataTable ListarNomesTitulares() {
			DataTable dt = GetFromCache("NOMES_TITULARES") as DataTable;

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarNomesTitulares(db);
					AddOnCache("NOMES_TITULARES", dt);
				}
			}
			return dt;
		}

		public HcPlanoVO GetPlano(string idPlano) {
			List<HcPlanoVO> lst = ListarPlanos();
			return lst.Find(x => x.CdPlano.Equals(idPlano));
		}

		internal HcPlanoVO GetPlano(string idPlano, EvidaDatabase db) {
			List<HcPlanoVO> lst = ListarPlanos(db);
			return lst.Find(x => x.CdPlano.Equals(idPlano));
		}

		public List<HcPlanoVO> ListarPlanos() {
			return ListarPlanos(null);
		}

		internal List<HcPlanoVO> ListarPlanos(EvidaDatabase db = null) {
			List<HcPlanoVO> dt = GetFromCache("PLANOS") as List<HcPlanoVO>;

			if (dt == null) {
				if (db == null) {
					using (db = EvidaDatabase.CreateDatabase(null)) {
						EvidaConnectionHolder connection = db.CreateConnection();
						dt = LocatorDAO.ListarPlanos(db);
						AddOnCache("PLANOS", dt);
					}
				} else {
					dt = LocatorDAO.ListarPlanos(db);
					AddOnCache("PLANOS", dt);
				}
			}
			return new List<HcPlanoVO>(dt);
		}

		public List<HcAnsProfissionalVO> BuscarProfissionais(string nroConselho, string nome, string uf, string codConselho) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				List<HcAnsProfissionalVO> dt = LocatorDAO.BuscarProfissionais(nroConselho, nome, uf, codConselho, db);
				return dt;
			}
		}

        public HcAnsProfissionalVO GetProfissional(ProfissionalConselhoVO conselho)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                List<HcAnsProfissionalVO> dt = LocatorDAO.BuscarProfissionais(conselho.NrConselho, null, conselho.CdUf, conselho.CdConselho, db);
                if (dt != null && dt.Count > 0)
                    return dt[0];
                return null;
            }
        }

		public HcAnsProfissionalVO GetProfissional(int seqProfissional) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.GetProfissional(seqProfissional, db);
			}
		}

		internal DataTable ListarCategorias(EvidaDatabase db) {
			DataTable dt = GetFromCache("CATEGORIA") as DataTable;

			if (dt == null) {
				bool createConnection = db == null;
				if (createConnection) {
					using (db = EvidaDatabase.CreateDatabase(null)) {
						EvidaConnectionHolder connection = db.CreateConnection();
						dt = LocatorDAO.ListarCategorias(db);
					}
				} else {
					dt = LocatorDAO.ListarCategorias(db);
				}
				AddOnCache("CATEGORIA", dt);
			}
			return dt;
		}

		public DataTable ListarCategorias() {
			return ListarCategorias(null);
		}

		/*
		internal List<KeyValuePair<int,string>> ListarCategorias2(Database db) {
			DataTable dt = ListarCategorias(db);
			if (dt != null) {
				IEnumerable<KeyValuePair<int,string>> enumKeyValue = dt.AsEnumerable().Select(x =>
					new KeyValuePair<int, string>(Convert.ToInt32(x["CD_CATEGORIA"]), Convert.ToString(x["DS_CATEGORIA"])));
				return enumKeyValue.ToList();
			}
			return null;
		}*/

		public List<HcEmpresaVO> ListarEmpresas() {
			List<HcEmpresaVO> dt = (List<HcEmpresaVO>)GetFromCache("EMPRESA");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarEmpresas(db);
					AddOnCache("EMPRESA", dt);
				}
			}
			return dt;
		}

		public HcEmpresaVO GetEmpresa(int cdEmpresa) {
			List<HcEmpresaVO> dt = ListarEmpresas();
			return dt.Find(x => x.Id == cdEmpresa);
		}

		public DataTable ListarItensLista(string lista) {
			DataTable dt = GetFromCache("ITENS_LISTA_" + lista) as DataTable;

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarItensLista(lista, db);
					AddOnCache("ITENS_LISTA_" + lista, dt);
				}
			}
			return dt;
		}

		public string GetItemLista(string lista, string cdItem) {
			DataTable dtLista = ListarItensLista(lista);
			if (dtLista != null) {
				DataRow[] drs = dtLista.Select("cd_item_lista = '" + cdItem + "'");
				if (drs.Length > 0) {
					return Convert.ToString(drs[0]["ds_item_lista"]);
				}
			}
			return null;
		}

		public List<KeyValuePair<string, string>> ListaParentescos() {
			List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("PARENTESCO");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarParentescos(db);
					AddOnCache("PARENTESCO", dt);
				}
			}
			return dt;
		}

		public string GetParentesco(string cdParentesco) {
			List<KeyValuePair<string, string>> dt = ListaParentescos();
			if (dt != null) {
				return dt.Find(x => x.Key == cdParentesco).Value;
			}
			return null;
		}

		public List<KeyValuePair<string, string>> ListarConselhoProfissional() {
			List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("CONSELHO_PROF");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarConselhoProfissional(db);
					AddOnCache("CONSELHO_PROF", dt);
				}				
			}
			return dt;
		}

		public IEnumerable<Constantes.Uf> ListarUf() {
			return Constantes.Uf.Values;
		}

		public List<KeyValuePair<int,string>> ListarRegionais() {
			return ListarRegionais(null);
		}

		internal List<KeyValuePair<int, string>> ListarRegionais(EvidaDatabase db) {
			List<KeyValuePair<int, string>> dt = (List<KeyValuePair<int,string>>)GetFromCache("REGIONAL");

			if (dt == null) {
				if (db == null) {
					using (db = EvidaDatabase.CreateDatabase(null)) {
						EvidaConnectionHolder connection = db.CreateConnection();
						dt = LocatorDAO.ListarRegionais(db);
					}
				} else {
					dt = LocatorDAO.ListarRegionais(db);
				}

				AddOnCache("REGIONAL", dt);
			}
			return dt;
		}

		internal string GetRegional(int cdLocal, EvidaDatabase db) {
			List<KeyValuePair<int, string>> dt = ListarRegionais(db);
			if (dt != null) {
				return dt.Find(x => x.Key == cdLocal).Value;
			}
			return null;
		}

		public string GetRegional(int cdLocal) {
			return GetRegional(cdLocal, null);
		}

		private List<LotacaoVO> ListarLotacoes() {
			List<LotacaoVO> dt = (List<LotacaoVO>)GetFromCache("LOTACAO");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarLotacoes(db);
					AddOnCache("LOTACAO", dt);
				}
			}
			return dt;
		}

		public LotacaoVO GetLotacao(string cdLotacao) {
			List<LotacaoVO> dt = ListarLotacoes();
			if (dt != null) {
				return dt.Find(x => x.CdLotacao.Equals(cdLotacao));
			}
			return null;
		}

		public DataTable ListarNomesBeneficiarios() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = LocatorDAO.ListarNomesBeneficiarios(db);
				return dt;
			}
		}

		public HcServicoVO GetServico(int cdServico) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.GetServico(cdServico, db);
			}
		}

		public int? GetCodServicoByMascara(string cdMascara) {
			DataTable dt = BuscarServicos(cdMascara, null, true);
			if (dt == null || dt.Rows.Count == 0)
				return null;

			DataRow dr = dt.Rows[0];
			int cdServico = Convert.ToInt32(dr["CD_SERVICO"]);
			return cdServico;
		}

		public DataTable BuscarServicos(string cdMascara, string nome, bool diffCodTabela) {
			DataTable dt;
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				dt = LocatorDAO.BuscarServicos(cdMascara, nome, diffCodTabela, db);
			}
			
			return dt;
		}

		public DataTable BuscarDoencas(string cdDoenca, string nome) {
			DataTable dt;
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				try {
					dt = LocatorDAO.BuscarDoencas(cdDoenca, nome, db);
				}
				finally {
					connection.Close();
				}
			}

			return dt;
		}

		public DataTable ListarCredenciados() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = LocatorDAO.ListarCredenciados(db);
				return dt;
			}
		}

		public DataTable BuscarBeneficiarios(string cdAlternativo, string nome) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.BuscarBeneficiarios(cdAlternativo, nome, db);
			}
		}

		public CepVO BuscarCep(int cep) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.BuscarCep(cep, db);
			}
		}

		public DataTable BuscarMunicipios(string uf) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.BuscarMunicipios(uf, db);
			}
		}

		public DataTable BuscarMunicipiosISA(string uf) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.BuscarMunicipiosISA(uf, db);
			}
		}

		public IEnumerable<CepVO> BuscarMunicipiosISACep(string uf) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = LocatorDAO.BuscarMunicipiosISA(uf, db);
				if (dt != null) {
					IEnumerable<CepVO> lstCep = from r in dt.AsEnumerable()
												select new CepVO() {
													Cidade = (string)r["ds_municipio"],
													IdLocalidade = Convert.ToInt32(r["cd_municipio"])
												};
					return lstCep;
				}
				return null;
			}
		}

		public CepVO BuscarCepISA(int cep) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return LocatorDAO.BuscarCepISA(cep, db);
			}
		}

		public List<HcNaturezaVO> ListarNatureza() {
			List<HcNaturezaVO> dt = (List<HcNaturezaVO>)GetFromCache("NATUREZA");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarNatureza(db);
					AddOnCache("NATUREZA", dt);
				}
			}
			return dt;
		}

		public HcNaturezaVO GetNatureza(int idNatureza) {
			List<HcNaturezaVO> lst = ListarNatureza();
			if (lst != null) {
				return lst.First(x => x.CdNatureza == idNatureza);
			}
			return null;
		}

		public List<KeyValuePair<int, string>> ListarMotivosDesligamento() {
			List<KeyValuePair<int, string>> dt = (List<KeyValuePair<int, string>>)GetFromCache("MOTIVO_DESLIGAMENTO");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarMotivosDesligamento(db);
					AddOnCache("MOTIVO_DESLIGAMENTO", dt);
				}
			}
			return dt;
		}

		public KeyValuePair<int, string>? GetMotivoDesligamento(int idMotivo) {
			List<KeyValuePair<int, string>> lst = ListarMotivosDesligamento();
			if (lst != null) {
				int idx = lst.FindIndex(x => x.Key == idMotivo);
				if (idx >= 0)
					return lst[idx];
			}
			return null;
		}

		public List<HcBancoVO> ListarBancoISA() {
			List<HcBancoVO> dt = (List<HcBancoVO>)GetFromCache("BANCO");

			if (dt == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarBancoISA(db);
					AddOnCache("BANCO", dt);
				}
			}
			return dt;
		}

		public List<HcAgenciaBancariaVO> ListarAgenciaISA() {
			return ListarAgenciaISA(null);
		}

		internal List<HcAgenciaBancariaVO> ListarAgenciaISA(EvidaDatabase db) {
			List<HcAgenciaBancariaVO> dt = (List<HcAgenciaBancariaVO>)GetFromCache("AGENCIA_BANCARIA");

			if (dt == null) {
				if (db == null) {
					using (db = EvidaDatabase.CreateDatabase(null)) {
						EvidaConnectionHolder connection = db.CreateConnection();
						dt = LocatorDAO.ListarAgenciaBancoISA(db);
					}
				} else {
					dt = LocatorDAO.ListarAgenciaBancoISA(db);
				}
				AddOnCache("AGENCIA_BANCARIA", dt);
			}
			return dt;
		}

		public List<HcUnidadeOrganizacionalVO> ListarUnidadesOrganizacionais(int cdEmpresa) {
			return ListarUnidadesOrganizacionais(cdEmpresa, null);
		}

		internal List<HcUnidadeOrganizacionalVO> ListarUnidadesOrganizacionais(int cdEmpresa, EvidaDatabase db) {
			List<HcUnidadeOrganizacionalVO> dt = null;
			if (db == null) {
				using (db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();
					dt = LocatorDAO.ListarUnidadesOrganizacionais(cdEmpresa, db);
				}
			} else {
				dt = LocatorDAO.ListarUnidadesOrganizacionais(cdEmpresa, db);
			}
			return dt;
		}

		internal HcBancoVO GetBanco(int idBanco) {
			List<HcBancoVO> lst = ListarBancoISA();
			if (lst != null) {
				return lst.FirstOrDefault(x => x.Id == idBanco);
			}
			return null;
		}

        public HcAgenciaBancariaVO GetAgencia(int idBanco, string cdAgencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return LocatorDAO.GetAgenciaBancoISA(idBanco, cdAgencia, db);
            }
        }

        public HcAgenciaBancariaVO GetAgenciaDv(int idBanco, string cdAgencia, string dvAgencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return LocatorDAO.GetAgenciaDvBancoISA(idBanco, cdAgencia, dvAgencia, db);
            }
        }

        public HcAgenciaBancariaVO GetAgenciaFuncionario(int idBanco, string cdAgencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return LocatorDAO.GetAgenciaBancoFuncionarioISA(idBanco, cdAgencia, db);
            }
        }	
	}
}
