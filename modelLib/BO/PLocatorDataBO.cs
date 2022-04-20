using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.BO
{
    public class PLocatorDataBO
    {
        EVidaLog log = new EVidaLog(typeof(PLocatorDataBO));
        private static PLocatorDataBO instance = new PLocatorDataBO();

        public static PLocatorDataBO Instance { get { return instance; } }

        private void AddOnCache(string name, object value)
        {
            CacheHelper.AddOnCache(name, value);
        }

        private object GetFromCache(string name)
        {
            return CacheHelper.GetFromCache<object>(name);
        }

        public DataTable ListarNomesTitulares()
        {
            DataTable dt = GetFromCache("NOMES_TITULARES") as DataTable;

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarNomesTitulares(db);
                    AddOnCache("NOMES_TITULARES", dt);
                }
            }
            return dt;
        }

        public HcPlanoVO GetPlano(string idPlano)
        {
            List<HcPlanoVO> lst = ListarPlanos();
            return lst.Find(x => x.CdPlano.Equals(idPlano));
        }

        internal HcPlanoVO GetPlano(string idPlano, EvidaDatabase db)
        {
            List<HcPlanoVO> lst = ListarPlanos(db);
            return lst.Find(x => x.CdPlano.Equals(idPlano));
        }

        public List<HcPlanoVO> ListarPlanos()
        {
            return ListarPlanos(null);
        }

        internal List<HcPlanoVO> ListarPlanos(EvidaDatabase db = null)
        {
            List<HcPlanoVO> dt = GetFromCache("PLANOS") as List<HcPlanoVO>;

            if (dt == null)
            {
                if (db == null)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarPlanos(db);
                        AddOnCache("PLANOS", dt);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarPlanos(db);
                    AddOnCache("PLANOS", dt);
                }
            }
            return new List<HcPlanoVO>(dt);
        }

        public PProdutoSaudeVO GetProdutoSaude(string codigo)
        {
            List<PProdutoSaudeVO> lst = ListarProdutoSaude();
            return lst.Find(x => x.Codigo.Equals(codigo.PadLeft(4, '0')));
        }

        internal PProdutoSaudeVO GetProdutoSaude(string codigo, EvidaDatabase db)
        {
            List<PProdutoSaudeVO> lst = ListarProdutoSaude(db);
            return lst.Find(x => x.Codigo.Equals(codigo));
        }

        public List<PProdutoSaudeVO> ListarProdutoSaude()
        {
            return ListarProdutoSaude(null);
        }

        internal List<PProdutoSaudeVO> ListarProdutoSaude(EvidaDatabase db = null)
        {
            List<PProdutoSaudeVO> dt = GetFromCache("PRODUTOSAUDE") as List<PProdutoSaudeVO>;

            if (dt == null)
            {
                if (db == null)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarProdutoSaude(db);
                        AddOnCache("PRODUTOSAUDE", dt);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarProdutoSaude(db);
                    AddOnCache("PRODUTOSAUDE", dt);
                }
            }
            return new List<PProdutoSaudeVO>(dt);
        }

        public List<PProfissionalSaudeVO> BuscarProfissionais(string nroConselho, string nome, string uf, string codConselho)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                List<PProfissionalSaudeVO> dt = PLocatorDAO.BuscarProfissionais(nroConselho, nome, uf, codConselho, db);
                return dt;
            }
        }

        public PProfissionalSaudeVO GetProfissional(string nroConselho, string uf, string codConselho)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                List<PProfissionalSaudeVO> dt = PLocatorDAO.BuscarProfissionais(nroConselho, null, uf, codConselho, db);
                if (dt != null && dt.Count > 0)
                    return dt[0];
                return null;
            }
        }

        public PProfissionalSaudeVO GetProfissional(string seqProfissional)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetProfissional(seqProfissional, db);
            }
        }

        internal DataTable ListarCategorias(int empresa, EvidaDatabase db)
        {
            DataTable dt = GetFromCache("CATEGORIA") as DataTable;

            if (dt == null)
            {
                bool createConnection = db == null;
                if (createConnection)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarCategorias(empresa, db);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarCategorias(empresa, db);
                }
                AddOnCache("CATEGORIA", dt);
            }
            return dt;
        }

        internal DataTable ListarCategorias(EvidaDatabase db)
        {
            DataTable dt = GetFromCache("CATEGORIA") as DataTable;

            if (dt == null)
            {
                bool createConnection = db == null;
                if (createConnection)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarCategorias(db);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarCategorias(db);
                }
                AddOnCache("CATEGORIA", dt);
            }
            return dt;
        }

        public DataTable ListarCategorias(int empresa)
        {
            return ListarCategorias(empresa, null);
        }

        public DataTable ListarCategorias()
        {
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

        public List<PGrupoEmpresaVO> ListarEmpresas()
        {
            List<PGrupoEmpresaVO> dt = (List<PGrupoEmpresaVO>)GetFromCache("EMPRESA");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarEmpresas(db);
                    AddOnCache("EMPRESA", dt);
                }
            }
            return dt;
        }

        public PGrupoEmpresaVO GetEmpresa(string cdEmpresa)
        {
            List<PGrupoEmpresaVO> dt = ListarEmpresas();
            return dt.Find(x => x.Codigo == cdEmpresa);
        }

        public DataTable ListarItensLista(string lista)
        {
            DataTable dt = GetFromCache("ITENS_LISTA_" + lista) as DataTable;

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarItensLista(lista, db);
                    AddOnCache("ITENS_LISTA_" + lista, dt);
                }
            }
            return dt;
        }

        public string GetItemLista(string lista, string cdItem)
        {
            DataTable dtLista = ListarItensLista(lista);
            if (dtLista != null)
            {
                DataRow[] drs = dtLista.Select("cd_item_lista = '" + cdItem + "'");
                if (drs.Length > 0)
                {
                    return Convert.ToString(drs[0]["ds_item_lista"]);
                }
            }
            return null;
        }

        public List<KeyValuePair<string, string>> ListaParentescos()
        {
            List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("PARENTESCO");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarParentescos(db);
                    AddOnCache("PARENTESCO", dt);
                }
            }
            return dt;
        }

        public string GetParentesco(string cdParentesco)
        {
            List<KeyValuePair<string, string>> dt = ListaParentescos();
            if (dt != null)
            {
                return dt.Find(x => x.Key == cdParentesco).Value;
            }
            return null;
        }

        public List<KeyValuePair<string, string>> ListarConselhoProfissional()
        {
            List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("CONSELHO_PROF");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarConselhoProfissional(db);
                    AddOnCache("CONSELHO_PROF", dt);
                }
            }
            return dt;
        }

        public IEnumerable<PConstantes.Uf> ListarUf()
        {
            return PConstantes.Uf.Values;
        }

        public List<KeyValuePair<int, string>> ListarRegionais()
        {
            return ListarRegionais(null);
        }

        internal List<KeyValuePair<int, string>> ListarRegionais(EvidaDatabase db)
        {
            List<KeyValuePair<int, string>> dt = (List<KeyValuePair<int, string>>)GetFromCache("REGIONAL");

            if (dt == null)
            {
                if (db == null)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarRegionais(db);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarRegionais(db);
                }

                AddOnCache("REGIONAL", dt);
            }
            return dt;
        }

        internal string GetRegional(int cdLocal, EvidaDatabase db)
        {
            List<KeyValuePair<int, string>> dt = ListarRegionais(db);
            if (dt != null)
            {
                return dt.Find(x => x.Key == cdLocal).Value;
            }
            return null;
        }

        public string GetRegional(int cdLocal)
        {
            return GetRegional(cdLocal, null);
        }

        public List<KeyValuePair<string, string>> ListarRegioes()
        {
            return ListarRegioes(null);
        }

        internal List<KeyValuePair<string, string>> ListarRegioes(EvidaDatabase db)
        {
            List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("REGIAO");

            if (dt == null)
            {
                if (db == null)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarRegioes(db);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarRegioes(db);
                }

                AddOnCache("REGIAO", dt);
            }
            return dt;
        }

        internal string GetRegiao(string cdLocal, EvidaDatabase db)
        {
            List<KeyValuePair<string, string>> dt = ListarRegioes(db);
            if (dt != null)
            {
                return dt.Find(x => x.Key == cdLocal).Value;
            }
            return null;
        }

        public string GetRegiao(string cdLocal)
        {
            return GetRegiao(cdLocal, null);
        }
        
        private List<LotacaoVO> ListarLotacoes()
        {
            List<LotacaoVO> dt = (List<LotacaoVO>)GetFromCache("LOTACAO");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarLotacoes(db);
                    AddOnCache("LOTACAO", dt);
                }
            }
            return dt;
        }

        public LotacaoVO GetLotacao(string cdLotacao)
        {
            List<LotacaoVO> dt = ListarLotacoes();
            if (dt != null)
            {
                return dt.Find(x => x.CdLotacao.Equals(cdLotacao));
            }
            return null;
        }

        public DataTable ListarNomesBeneficiarios()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = PLocatorDAO.ListarNomesBeneficiarios(db);
                return dt;
            }
        }

        public HcServicoVO GetServico(int cdServico)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetServico(cdServico, db);
            }
        }

        public string GetCodServicoByMascara(string cdMascara)
        {
            DataTable dt = BuscarServicos(cdMascara, null, true);
            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow dr = dt.Rows[0];
            string cdServico = Convert.ToString(dr["CD_SERVICO"]);
            return cdServico;
        }

        public DataTable BuscarServicos(string cdMascara, string nome, bool diffCodTabela)
        {
            DataTable dt;
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                dt = PLocatorDAO.BuscarServicos(cdMascara, nome, diffCodTabela, db);
            }

            return dt;
        }

        public PTabelaPadraoVO GetTabelaPadrao(string codpad, string codpsa)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetTabelaPadrao(codpad, codpsa, db);
            }
        }
        
        public DataTable BuscarDoencas(string cdDoenca, string nome)
        {
            DataTable dt;
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    dt = PLocatorDAO.BuscarDoencas(cdDoenca, nome, db);
                }
                finally
                {
                    connection.Close();
                }
            }

            return dt;
        }

        public DataTable ListarCredenciados()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = PLocatorDAO.ListarCredenciados(db);
                return dt;
            }
        }

        public DataTable ListarRedesAtendimento()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = PLocatorDAO.ListarRedesAtendimento(db);
                return dt;
            }
        }

        public DataTable BuscarBeneficiarios(string cdAlternativo, string nome)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.BuscarBeneficiarios(cdAlternativo, nome, db);
            }
        }

        public DataTable BuscarUsuarios(string cdAlternativo, string nome)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.BuscarUsuarios(cdAlternativo, nome, db);
            }
        }

        public DataTable BuscarMunicipios(string uf)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.BuscarMunicipios(uf, db);
            }
        }

        public DataTable BuscarMunicipiosISA(string uf)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.BuscarMunicipiosISA(uf, db);
            }
        }

        public DataTable BuscarMunicipiosProtheus(string uf)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.BuscarMunicipiosProtheus(uf, db);
            }
        }

        public IEnumerable<CepVO> BuscarMunicipiosISACep(string uf)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = PLocatorDAO.BuscarMunicipiosISA(uf, db);
                if (dt != null)
                {
                    IEnumerable<CepVO> lstCep = from r in dt.AsEnumerable()
                                                select new CepVO()
                                                {
                                                    Cidade = (string)r["ds_municipio"],
                                                    IdLocalidade = Convert.ToInt32(r["cd_municipio"])
                                                };
                    return lstCep;
                }
                return null;
            }
        }

        public IEnumerable<PCepVO> BuscarMunicipiosPROTHEUSCep(string uf)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = PLocatorDAO.BuscarMunicipiosProtheus(uf, db);
                if (dt != null)
                {
                    IEnumerable<PCepVO> lstCep = from r in dt.AsEnumerable()
                                                select new PCepVO()
                                                {
                                                    Mun = (string)r["BID_DESCRI"],
                                                    Codmun = (string)r["BID_CODMUN"]
                                                };
                    return lstCep;
                }
                return null;
            }
        }

        public PCepVO BuscarCepProtheus(string cep)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.BuscarCepProtheus(cep, db);
            }
        }

        public List<HcNaturezaVO> ListarNatureza()
        {
            List<HcNaturezaVO> dt = (List<HcNaturezaVO>)GetFromCache("NATUREZA");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarNatureza(db);
                    AddOnCache("NATUREZA", dt);
                }
            }
            return dt;
        }

        public List<PEspecialidadeVO> ListarEspecialidade()
        {
            List<PEspecialidadeVO> dt = (List<PEspecialidadeVO>)GetFromCache("ESPECIALIDADE");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarEspecialidade(db);
                    AddOnCache("ESPECIALIDADE", dt);
                }
            }
            return dt;
        }

        public List<PEspecialidadeVO> ListarEspecialidade(string codigo)
        {
            List<PEspecialidadeVO> dt = null;
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                dt = PLocatorDAO.ListarEspecialidade(codigo, db);
            }

            return dt;
        }

        public List<PClasseRedeAtendimentoVO> ListarClasseRedeAtendimento()
        {
            List<PClasseRedeAtendimentoVO> dt = (List<PClasseRedeAtendimentoVO>)GetFromCache("CLASSEREDEATENDIMENTO");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarClasseRedeAtendimento(db);
                    AddOnCache("CLASSEREDEATENDIMENTO", dt);
                }
            }
            return dt;
        }

        public HcNaturezaVO GetNatureza(int idNatureza)
        {
            List<HcNaturezaVO> lst = ListarNatureza();
            if (lst != null)
            {
                return lst.First(x => x.CdNatureza == idNatureza);
            }
            return null;
        }

        public List<KeyValuePair<string, string>> ListarMotivosDesligamentoFamilia()
        {
            List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("MOTIVO_DESLIGAMENTO_FAMILIA");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarMotivosDesligamentoFamilia(db);
                    AddOnCache("MOTIVO_DESLIGAMENTO_FAMILIA", dt);
                }
            }
            return dt;
        }

        public KeyValuePair<string, string>? GetMotivoDesligamentoFamilia(string idMotivo)
        {
            List<KeyValuePair<string, string>> lst = ListarMotivosDesligamentoFamilia();
            if (lst != null)
            {
                int idx = lst.FindIndex(x => x.Key == idMotivo);
                if (idx >= 0)
                    return lst[idx];
            }
            return null;
        }

        public List<KeyValuePair<string, string>> ListarMotivosDesligamentoUsuario()
        {
            List<KeyValuePair<string, string>> dt = (List<KeyValuePair<string, string>>)GetFromCache("MOTIVO_DESLIGAMENTO_USUARIO");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarMotivosDesligamentoUsuario(db);
                    AddOnCache("MOTIVO_DESLIGAMENTO_USUARIO", dt);
                }
            }
            return dt;
        }

        public KeyValuePair<string, string>? GetMotivoDesligamentoUsuario(string idMotivo)
        {
            List<KeyValuePair<string, string>> lst = ListarMotivosDesligamentoUsuario();
            if (lst != null)
            {
                int idx = lst.FindIndex(x => x.Key == idMotivo);
                if (idx >= 0)
                    return lst[idx];
            }
            return null;
        }

        public string ObterDescricaoMotivoDesligamentoUsuario(string codigo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.ObterDescricaoMotivoDesligamentoUsuario(codigo, db);
            }
        }

        public string ObterDescricaoMotivoDesligamentoFamilia(string codigo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.ObterDescricaoMotivoDesligamentoFamilia(codigo, db);
            }
        }

        public List<HcBancoVO> ListarBancoISA()
        {
            List<HcBancoVO> dt = (List<HcBancoVO>)GetFromCache("BANCO");

            if (dt == null)
            {
                using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarBancoISA(db);
                    AddOnCache("BANCO", dt);
                }
            }
            return dt;
        }

        public List<HcAgenciaBancariaVO> ListarAgenciaISA()
        {
            return ListarAgenciaISA(null);
        }

        internal List<HcAgenciaBancariaVO> ListarAgenciaISA(EvidaDatabase db)
        {
            List<HcAgenciaBancariaVO> dt = (List<HcAgenciaBancariaVO>)GetFromCache("AGENCIA_BANCARIA");

            if (dt == null)
            {
                if (db == null)
                {
                    using (db = EvidaDatabase.CreateDatabase(null))
                    {
                        EvidaConnectionHolder connection = db.CreateConnection();
                        dt = PLocatorDAO.ListarAgenciaBancoISA(db);
                    }
                }
                else
                {
                    dt = PLocatorDAO.ListarAgenciaBancoISA(db);
                }
                AddOnCache("AGENCIA_BANCARIA", dt);
            }
            return dt;
        }

        public List<HcUnidadeOrganizacionalVO> ListarUnidadesOrganizacionais(int cdEmpresa)
        {
            return ListarUnidadesOrganizacionais(cdEmpresa, null);
        }

        internal List<HcUnidadeOrganizacionalVO> ListarUnidadesOrganizacionais(int cdEmpresa, EvidaDatabase db)
        {
            List<HcUnidadeOrganizacionalVO> dt = null;
            if (db == null)
            {
                using (db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarUnidadesOrganizacionais(cdEmpresa, db);
                }
            }
            else
            {
                dt = PLocatorDAO.ListarUnidadesOrganizacionais(cdEmpresa, db);
            }
            return dt;
        }

        public List<PCadastroOrganizacionalVO> ListarCadastrosOrganizacionais(int cdEmpresa)
        {
            return ListarCadastrosOrganizacionais(cdEmpresa, null);
        }

        internal List<PCadastroOrganizacionalVO> ListarCadastrosOrganizacionais(int cdEmpresa, EvidaDatabase db)
        {
            List<PCadastroOrganizacionalVO> dt = null;
            if (db == null)
            {
                using (db = EvidaDatabase.CreateDatabase(null))
                {
                    EvidaConnectionHolder connection = db.CreateConnection();
                    dt = PLocatorDAO.ListarCadastrosOrganizacionais(cdEmpresa, db);
                }
            }
            else
            {
                dt = PLocatorDAO.ListarCadastrosOrganizacionais(cdEmpresa, db);
            }
            return dt;
        }

        internal HcBancoVO GetBanco(int idBanco)
        {
            List<HcBancoVO> lst = ListarBancoISA();
            if (lst != null)
            {
                return lst.FirstOrDefault(x => x.Id == idBanco);
            }
            return null;
        }

        public HcAgenciaBancariaVO GetAgencia(int idBanco, string cdAgencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetAgenciaBancoISA(idBanco, cdAgencia, db);
            }
        }

        public HcAgenciaBancariaVO GetAgenciaDv(int idBanco, string cdAgencia, string dvAgencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetAgenciaDvBancoISA(idBanco, cdAgencia, dvAgencia, db);
            }
        }

        public HcAgenciaBancariaVO GetAgenciaFuncionario(int idBanco, string cdAgencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetAgenciaBancoFuncionarioISA(idBanco, cdAgencia, db);
            }
        }

        public DataTable BuscarPeg(String codRda, DateTime? dataEntrada, String docFiscal, decimal? valorApresentado, DateTime? dataEmissao, DateTime? dataVencimento)
        {
            DataTable dt;
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    dt = PLocatorDAO.BuscarPeg(codRda, dataEntrada, docFiscal, valorApresentado, dataEmissao, dataVencimento, db);
                }
                finally
                {
                    connection.Close();
                }
            }

            return dt;
        }

        public DataTable BuscarPegFinanceiro(int codigo)
        {
            DataTable dt;
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                try
                {
                    dt = PLocatorDAO.BuscarPegFinanceiro(codigo, db);
                }
                finally
                {
                    connection.Close();
                }
            }

            return dt;
        }

        public PPegVO GetPeg(string codpeg, string codrda)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetPeg(codpeg, codrda, db);
            }
        }

        public PClasseRedeAtendimentoVO GetClasseRedeAtendimento(string codigo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetClasseRedeAtendimento(codigo, db);
            }
        }

        public PEmpresaModalidadeCobrancaVO GetEmpresaModalidadeCobranca(string codigo, string numcon, string vercon, string subcon, string versub, string codpro, string versao)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetEmpresaModalidadeCobranca(codigo, numcon, vercon, subcon, versub, codpro, versao, db);
            }
        }

        public POperadoraSaudeVO GetOperadoraSaude(string codint)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetOperadoraSaude(codint, db);
            }
        }

        public PFormaPagamentoVO GetFormaPagamento(string codigo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetFormaPagamento(codigo, db);
            }
        }

        public PModalidadeCobrancaVO GetModalidadeCobranca(string codigo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetModalidadeCobranca(codigo, db);
            }
        }

        public PSubcontratoVO GetSubcontrato(string numcon, string vercon, string subcon, string versub)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.GetSubcontrato(numcon, vercon, subcon, versub, db);
            }
        }

        public string ObterReducaoEndereco(string endereco, string numero, string complemento, string bairro)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PLocatorDAO.ObterReducaoEndereco(endereco, numero, complemento, bairro, db);
            }
        }
    }
}
