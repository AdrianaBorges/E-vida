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

namespace eVidaGeneralLib.DAO {
	internal class LocatorDAO {

		internal static List<LotacaoVO> ListarLotacoes(EvidaDatabase db) {
			string sql = "SELECT cd_lotacao, ds_lotacao, sg_lotacao FROM isa_hc.Hc_v_Lotacao ORDER BY ds_lotacao ";

			Func<DataRow, LotacaoVO> convert = delegate(DataRow dr)
			{
				return new LotacaoVO()
				{
					CdLotacao = Convert.ToString(dr["cd_lotacao"]),
					DsLotacao = Convert.ToString(dr["ds_lotacao"]),
					SgLotacao = Convert.ToString(dr["sg_lotacao"])
				};
			};

			List<LotacaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);

			return lst;
		}

		internal static List<KeyValuePair<int, string>> ListarRegionais(EvidaDatabase db) {
			string sql = "SELECT cd_regional, ds_regional FROM isa_hc.hc_regional ORDER BY ds_regional ";

			List<KeyValuePair<int, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<int>("cd_regional", "ds_regional"));
			
			return lst;
		}

		internal static List<KeyValuePair<string, string>> ListarParentescos(EvidaDatabase db) {
			string sql = "SELECT cd_parentesco, ds_parentesco, cd_parentesco_ans " +
				" FROM isa_hc.hc_grau_parentesco ORDER BY ds_parentesco ";

			List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql,
				BaseDAO.Converter<string>("cd_parentesco", "ds_parentesco"));

			return lst;
		}

		internal static DataTable ListarNomesTitulares(EvidaDatabase db) {
			string sql = "SELECT DISTINCT A.NM_TITULAR FROM ISA_HC.HC_V_BENEFICIARIO A ORDER BY A.NM_TITULAR ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

			return dt;
		}

		internal static DataTable ListarCategorias(EvidaDatabase db) {
			string sql = "select cd_categoria, ds_categoria from isa_hc.hc_categoria order by ds_categoria ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

			return dt;
		}

		internal static List<HcEmpresaVO> ListarEmpresas(EvidaDatabase db) {
			string sql = "select cd_empresa, ds_empresa, st_empresa, tp_empresa, ds_sigla from isa_hc.hc_empresa order by ds_empresa ";

			Func<DataRow, HcEmpresaVO> convert = delegate(DataRow dr) {
				return new HcEmpresaVO() {
					Id = Convert.ToInt32(dr["cd_empresa"]),
					Nome = Convert.ToString(dr["ds_empresa"]),
					Status = Convert.ToString(dr["st_empresa"]),
					Tipo = Convert.ToString(dr["tp_empresa"]),
					Sigla = Convert.ToString(dr["ds_sigla"])
				};
			};
			List<HcEmpresaVO> dt = BaseDAO.ExecuteDataSet(db, sql, convert);

			return dt;
		}

		internal static DataTable ListarNomesBeneficiarios(EvidaDatabase db) {
			string sql = "SELECT DISTINCT A.NM_BENEFICIARIO FROM ISA_HC.HC_V_BENEFICIARIO A ORDER BY A.NM_BENEFICIARIO ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

			return dt;
		}

        internal static DataTable BuscarServicos(string cdMascara, string nome, bool diffCodTabela, EvidaDatabase db)
        {
            string sql = "SELECT " +
                (diffCodTabela ? "H.CD_TABELA, H.CD_SERVICO, " : " DISTINCT ") +
                "	H.DS_SERVICO, H.CD_MASCARA " +
                " FROM ISA_HC.HC_SERVICO H " +
                " WHERE 1 = 1 AND upper(trim(CD_TABELA)) NOT LIKE upper(trim('CBHPM%')) AND CD_TABELA <> 'AMB92' ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(cdMascara))
            {
                sql += " AND H.CD_MASCARA = :cdMascara";
                lstParams.Add(new Parametro() { Name = ":cdMascara", Tipo = DbType.String, Value = cdMascara });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(H.DS_SERVICO)) LIKE upper(trim(:nome))";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }

            sql += " ORDER BY " + (diffCodTabela ? "decode(h.cd_tabela,'TUSS','_',h.cd_tabela)," : "") + " H.DS_SERVICO ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

		internal static HcServicoVO GetServico(int cdServico, EvidaDatabase db) {
			string sql = "SELECT * FROM ISA_HC.HC_SERVICO H WHERE CD_SERVICO = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = cdServico });
			List<HcServicoVO> lstServicos = BaseDAO.ExecuteDataSet(db, sql, HcServicoVO.FromDataRow, lstParams);
			if (lstServicos != null && lstServicos.Count > 0)
				return lstServicos[0];
			return null;
		}

		internal static DataTable BuscarDoencas(string cdDoenca, string nome, Database db) {
			string sql = "SELECT UPPER(H.DS_DOENCA) DS_DOENCA, H.CD_DOENCA FROM ISA_HC.HC_DOENCA H WHERE 1 = 1 ";

			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(cdDoenca)) {
				sql += " AND H.CD_DOENCA = :cdDoenca";
				lstParams.Add(new Parametro() { Name = ":cdDoenca", Tipo = DbType.String, Value = cdDoenca });
			}
			if (!string.IsNullOrEmpty(nome)) {
                sql += " AND upper(trim((H.DS_DOENCA)) LIKE upper(trim(:nome))";
				lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
			}

			sql += " ORDER BY UPPER(H.DS_DOENCA) ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable ListarCredenciados(EvidaDatabase db) {
			string sql = "SELECT DISTINCT C.CD_CREDENCIADO, C.NM_RAZAO_SOCIAL FROM ISA_HC.HC_V_CREDENCIADO C ORDER BY C.NM_RAZAO_SOCIAL ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

			return dt;
		}

		internal static List<HcPlanoVO> ListarPlanos(EvidaDatabase db) {
			string sql = "SELECT CD_PLANO, DS_PLANO, TP_PLANO, CD_PLANO_ANS, TP_ABRANGENCIA, VL_GRAU_IMPORTANCIA, DS_SIGLA, CD_LOCAL_ATEND_PADRAO " +
				"	FROM ISA_HC.HC_PLANO P ";
			return BaseDAO.ExecuteDataSet(db, sql, HcPlanoVO.FromDataRow);
		}

        internal static List<KeyValuePair<string, string>> ListarConselhoProfissional(EvidaDatabase db)
        {
            string sql = "SELECT CD_CONSELHO, DS_CONSELHO " +
                " FROM isa_hc.HC_ANS_CONSELHO_PROFISSIONAL ORDER BY DS_CONSELHO ";

            List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql,
                BaseDAO.Converter<string>("CD_CONSELHO", "DS_CONSELHO"));

            return lst;
        }

		internal static HcAnsProfissionalVO GetProfissional(int seqProfissional, EvidaDatabase db) {
			string sql = "SELECT * FROM ISA_HC.HC_ANS_PROFISSIONAL P WHERE NR_SEQ_ANS_PROFISSIONAL = :seq";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":seq", DbType.Int32, seqProfissional));
			List<HcAnsProfissionalVO> lst = BaseDAO.ExecuteDataSet(db, sql, HcAnsProfissionalVO.FromDataRow, lstParams);
			if (lst == null || lst.Count == 0)
				return null;
			return lst[0];
		}

        internal static List<HcAnsProfissionalVO> BuscarProfissionais(string nroConselho, string nome, string uf, string codConselho, EvidaDatabase db)
        {
            string sql = "SELECT * FROM ISA_HC.HC_ANS_PROFISSIONAL P " +
                " WHERE 1 = 1 ";

            bool useUf = false;
            bool useNro = false;
            bool useCod = false;

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(nroConselho))
            {
                useNro = true;
                lstParams.Add(new Parametro() { Name = ":nroConselho", Tipo = DbType.String, Value = nroConselho });
            }
            if (!string.IsNullOrEmpty(codConselho))
            {
                useCod = true;
                lstParams.Add(new Parametro() { Name = ":codConselho", Tipo = DbType.String, Value = codConselho });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(NM_PROFISSIONAL)) LIKE upper(trim(:nome))";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }
            if (!string.IsNullOrEmpty(uf))
            {
                useUf = true;
                lstParams.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });
            }
            if (useUf || useNro || useCod)
            {
                int qtdParams = 0;
                string inSQL = string.Empty;
                string inSQL2 = string.Empty;
                if (useNro)
                {
                    inSQL = "NR_CONSELHO_{" + qtdParams + "}";
                    inSQL2 = ":nroConselho";
                    qtdParams++;
                }
                if (useCod)
                {
                    if (!string.IsNullOrEmpty(inSQL))
                    {
                        inSQL += ",";
                        inSQL2 += ",";
                    }
                    inSQL += "CD_CONSELHO_{" + qtdParams + "}";
                    inSQL2 += ":codConselho";
                    qtdParams++;
                }
                if (useUf)
                {
                    if (!string.IsNullOrEmpty(inSQL))
                    {
                        inSQL += ",";
                        inSQL2 += ",";
                    }
                    inSQL += "CD_UF_{" + qtdParams + "}";
                    inSQL2 += ":uf";
                    qtdParams++;
                }
                inSQL = "(" + inSQL + ") IN ((" + inSQL2 + ")) ";

                inSQL2 = string.Empty;
                for (int i = 1; i <= 5; i++)
                {
                    if (!string.IsNullOrEmpty(inSQL2)) inSQL2 += " OR ";
                    inSQL2 += " ( " + string.Format(inSQL, ArrayList.Repeat(i, qtdParams).ToArray()) + ") ";
                }
                sql += " AND (" + inSQL2 + " )";
            }
            else
            {
                return null;
            }
            sql += " ORDER BY NM_PROFISSIONAL ";
            return BaseDAO.ExecuteDataSet(db, sql, HcAnsProfissionalVO.FromDataRow, lstParams);
        }

		internal static DataTable ListarItensLista(string lista, EvidaDatabase db) {
			string sql = "SELECT il.cd_item_lista, il.ds_item_lista, CD_ORDEM_EXIBICAO " +
				" FROM ISA_SCL.SCL_LISTA L, ISA_SCL.SCL_SISTEMA S, ISA_SCL.SCL_ITEM_LISTA IL " +
				" WHERE l.cd_sistema = s.cd_sistema and il.cd_sistema = l.cd_sistema and il.cd_lista = l.cd_lista " +
				"	AND l.cd_lista = :lista AND S.CD_SISTEMA = 'HC3' " +
				" order by il.cd_ordem_exibicao ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":lista", Tipo = DbType.String, Value = lista });
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarBeneficiarios(string cdAlternativo, string nome, EvidaDatabase db) {
			string sql = "SELECT B.CD_BENEFICIARIO, B.CD_ALTERNATIVO, B.NM_BENEFICIARIO FROM ISA_HC.HC_V_BENEFICIARIO B WHERE 1 = 1 ";

			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(cdAlternativo)) {
				sql += " AND B.CD_ALTERNATIVO = :cdAlternativo";
				lstParams.Add(new Parametro() { Name = ":cdAlternativo", Tipo = DbType.String, Value = cdAlternativo });
			}
			if (!string.IsNullOrEmpty(nome)) {
                sql += " AND upper(trim(B.NM_BENEFICIARIO)) LIKE upper(trim(:nome))";
				lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
			}

			sql += " ORDER BY B.NM_BENEFICIARIO  ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static VO.CepVO BuscarCep(int cep, EvidaDatabase db) {
			string sql = "SELECT * FROM CEP WHERE nu_cep = :cep";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro() { Name = ":cep", Tipo = DbType.Int32, Value = cep });

			List<VO.CepVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromCepDataRow, lstP);

			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static VO.CepVO FromCepDataRow(DataRow dr) {
			VO.CepVO vo = new VO.CepVO();
			vo.Cep = Convert.ToInt32(dr["NU_CEP"]);
			vo.IdLocalidade = Convert.ToInt32(dr["ID_LOCALIDADE"]);
			vo.Bairro = Convert.ToString(dr["NO_BAIRRO"]);
			vo.Cidade = Convert.ToString(dr["NO_LOCALIDADE"]);
			vo.Rua = Convert.ToString(dr["NO_LOGRADOURO"]);
			vo.TipoLogradouro = Convert.ToString(dr["TP_LOGRADOURO"]);
			vo.Uf = Convert.ToString(dr["SG_UF"]);

			if (vo.TipoLogradouro != null && !vo.Rua.StartsWith(vo.TipoLogradouro + " ")
				&& !vo.Rua.Contains(" - " + vo.TipoLogradouro + " ") && !vo.TipoLogradouro.Equals("ND"))
				vo.Rua = vo.TipoLogradouro + " " + vo.Rua;

			return vo;
		}

		internal static DataTable BuscarMunicipios(string uf, EvidaDatabase db) {
			string sql = "SELECT DISTINCT NO_LOCALIDADE FROM CEP WHERE SG_UF = :uf order by NO_LOCALIDADE";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });

			return BaseDAO.ExecuteDataSet(db, sql, lstP);
		}

		internal static DataTable BuscarMunicipiosISA(string uf, EvidaDatabase db) {
			string sql = "SELECT DISTINCT CD_MUNICIPIO, DS_MUNICIPIO FROM ISA_HC.HC_MUNICIPIO WHERE CD_ESTADO = :uf order by DS_MUNICIPIO";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });

			return BaseDAO.ExecuteDataSet(db, sql, lstP);
		}

		internal static VO.CepVO BuscarCepISA(int cep, EvidaDatabase db) {
			string sql = "SELECT NR_CEP AS NU_CEP, C.CD_MUNICIPIO AS ID_LOCALIDADE, " +
				"	DS_LOGRADOURO AS NO_LOGRADOURO, DS_BAIRRO AS NO_BAIRRO, " +
				"	null AS TP_LOGRADOURO, M.CD_ESTADO AS SG_UF, M.DS_MUNICIPIO AS NO_LOCALIDADE " +
				"	FROM ISA_HC.HC_CEP C, ISA_HC.HC_MUNICIPIO M "+
				"	WHERE NR_CEP = :cep AND C.CD_MUNICIPIO = M.CD_MUNICIPIO ";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro() { Name = ":cep", Tipo = DbType.Int32, Value = cep });

			List<VO.CepVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromCepDataRow, lstP);

			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static List<VO.HC.HcBancoVO> ListarBancoISA(EvidaDatabase db) {
			string sql = "SELECT * " +
                "	FROM HC_BANCO B order by CD_BANCO ";

			Func<DataRow, VO.HC.HcBancoVO> convert = delegate(DataRow dr) {
				return new VO.HC.HcBancoVO() {
					Id = Convert.ToInt32(dr["cd_banco"]),
					Nome = Convert.ToString(dr["ds_banco"]),
					AceitaDebitoConta = "S".Equals(Convert.ToString(dr["fl_aceita_debito_conta"]), StringComparison.InvariantCultureIgnoreCase)
				};
			};

			List<VO.HC.HcBancoVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
			return lst;
		}

		internal static VO.HC.HcBancoVO GetBancoISA(int cdBanco, EvidaDatabase db) {
			string sql = "SELECT * " +
				"	FROM HC_BANCO B WHERE cd_banco = :idBanco";

			Func<DataRow, VO.HC.HcBancoVO> convert = delegate(DataRow dr) {
				return new VO.HC.HcBancoVO() {
					Id = Convert.ToInt32(dr["cd_banco"]),
					Nome = Convert.ToString(dr["ds_banco"]),
					AceitaDebitoConta = "S".Equals(Convert.ToString(dr["fl_aceita_debito_conta"]), StringComparison.InvariantCultureIgnoreCase)
				};
			};

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":idBanco", DbType.Int32, cdBanco));
			List<VO.HC.HcBancoVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static List<VO.HC.HcAgenciaBancariaVO> ListarAgenciaBancoISA(EvidaDatabase db) {
			string sql = "SELECT * " +
				"	FROM HC_AGENCIA_BANCARIA B ";

			Func<DataRow, VO.HC.HcAgenciaBancariaVO> convert = delegate(DataRow dr) {
				return new VO.HC.HcAgenciaBancariaVO() {
					IdBanco = Convert.ToInt32(dr["cd_banco"]),
					CdAgencia = Convert.ToString(dr["cd_agencia"]),
					Nome = Convert.ToString(dr["ds_agencia"]),
					DvAgencia = Convert.ToString(dr["nr_dv_agencia"])
				};
			};

			List<VO.HC.HcAgenciaBancariaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
			return lst;
		}

		internal static HcAgenciaBancariaVO GetAgenciaBancoISA(int idBanco, string cdAgencia, EvidaDatabase db) {
			string sql = "SELECT * " +
				"	FROM HC_AGENCIA_BANCARIA B where CD_BANCO = :idBanco AND CD_AGENCIA = :cdAgencia ";

			Func<DataRow, VO.HC.HcAgenciaBancariaVO> convert = delegate(DataRow dr) {
				return new VO.HC.HcAgenciaBancariaVO() {
					IdBanco = Convert.ToInt32(dr["cd_banco"]),
					CdAgencia = Convert.ToString(dr["cd_agencia"]),
					Nome = Convert.ToString(dr["ds_agencia"]),
					DvAgencia = Convert.ToString(dr["nr_dv_agencia"])
				};
			};

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":idBanco", DbType.Int32, idBanco));
			lstParams.Add(new Parametro(":cdAgencia", DbType.String, cdAgencia));

			List<VO.HC.HcAgenciaBancariaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

        internal static HcAgenciaBancariaVO GetAgenciaDvBancoISA(int idBanco, string cdAgencia, string dvAgencia, EvidaDatabase db)
        {
            string sql = "SELECT * " +
                "	FROM ISA_HC.HC_AGENCIA_BANCARIA B where CD_BANCO = :idBanco AND CD_AGENCIA = :cdAgencia AND NR_DV_AGENCIA = :dvAgencia ";

            Func<DataRow, VO.HC.HcAgenciaBancariaVO> convert = delegate(DataRow dr)
            {
                return new VO.HC.HcAgenciaBancariaVO()
                {
                    IdBanco = Convert.ToInt32(dr["cd_banco"]),
                    CdAgencia = Convert.ToString(dr["cd_agencia"]),
                    Nome = Convert.ToString(dr["ds_agencia"]),
                    DvAgencia = Convert.ToString(dr["nr_dv_agencia"])
                };
            };

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":idBanco", DbType.Int32, idBanco));
            lstParams.Add(new Parametro(":cdAgencia", DbType.String, cdAgencia));
            lstParams.Add(new Parametro(":dvAgencia", DbType.String, dvAgencia));

            List<VO.HC.HcAgenciaBancariaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static HcAgenciaBancariaVO GetAgenciaBancoFuncionarioISA(int idBanco, string cdAgencia, EvidaDatabase db)
        {
            // Tenta identificar uma agência com dígito NULL, com 1 dígito ou com 2 dígitos
            StringBuilder sql = new StringBuilder();
            sql.Append(" select * from isa_hc.hc_agencia_bancaria a ");
            sql.Append(" where exists (select * from isa_hc.hc_funcionario f where upper(trim(f.cd_agencia)) = upper(trim(a.cd_agencia)) and a.nr_dv_agencia is null and trim(f.cd_agencia) = :cdAgencia) ");
            sql.Append(" and a.cd_banco = :idBanco ");
            sql.Append(" union ");
            sql.Append(" select * from isa_hc.hc_agencia_bancaria a where exists (select * from isa_hc.hc_funcionario f where upper(substr(trim(f.cd_agencia), 1, length(trim(f.cd_agencia)) - 1)) = upper(trim(a.cd_agencia)) and upper(substr(trim(f.cd_agencia), length(trim(f.cd_agencia)), 1)) = upper(trim(a.nr_dv_agencia)) and trim(f.cd_agencia) = :cdAgencia) ");
            sql.Append(" and a.cd_banco = :idBanco ");
            sql.Append(" union ");
            sql.Append(" select * from isa_hc.hc_agencia_bancaria a where exists (select * from isa_hc.hc_funcionario f where upper(substr(trim(f.cd_agencia), 1, length(trim(f.cd_agencia)) - 2)) = upper(trim(a.cd_agencia)) and upper(substr(trim(f.cd_agencia), length(trim(f.cd_agencia)) - 1, 2)) = upper(trim(a.nr_dv_agencia)) and trim(f.cd_agencia) = :cdAgencia) ");
            sql.Append(" and a.cd_banco = :idBanco ");

            Func<DataRow, VO.HC.HcAgenciaBancariaVO> convert = delegate(DataRow dr)
            {
                return new VO.HC.HcAgenciaBancariaVO()
                {
                    IdBanco = Convert.ToInt32(dr["cd_banco"]),
                    CdAgencia = Convert.ToString(dr["cd_agencia"]),
                    Nome = Convert.ToString(dr["ds_agencia"]),
                    DvAgencia = Convert.ToString(dr["nr_dv_agencia"])
                };
            };

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":idBanco", DbType.Int32, idBanco));
            lstParams.Add(new Parametro(":cdAgencia", DbType.String, cdAgencia));

            List<VO.HC.HcAgenciaBancariaVO> lst = BaseDAO.ExecuteDataSet(db, sql.ToString(), convert, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

		internal static int? GetCdMunicipioFromAdesao(string cidade, string uf, EvidaDatabase db) {
			string sql = "select CD_MUNICIPIO from ISA_HC.HC_MUNICIPIO M " +
				" WHERE m.ds_municipio = UPPER(:cidade) AND M.CD_ESTADO = :uf " +
				" UNION " +
				" SELECT m.cd_municipio FROM CEP cep, ISA_HC.HC_MUNICIPIO M " +
				"	WHERE NO_LOCALIDADE = :cidade and cep.id_municipio = m.cd_municipio_ibge " +
				"		and cep.sg_uf = :uf AND cep.id_localidade <> 0";


			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":cidade", DbType.String, cidade));
			lstParams.Add(new Parametro(":uf", DbType.String, uf));

			object ret = BaseDAO.ExecuteScalar(db, sql, lstParams);
			if (ret == DBNull.Value)
				return null;
			return Convert.ToInt32(ret);
		}

		internal static List<HcNaturezaVO> ListarNatureza(EvidaDatabase db) {
			string sql = "SELECT * " +
				"	FROM ISA_HC.HC_NATUREZA_CREDENCIADO B ORDER BY DS_NATUREZA";

			Func<DataRow, VO.HC.HcNaturezaVO> convert = delegate(DataRow dr) {
				return new VO.HC.HcNaturezaVO() {
					CdNatureza = Convert.ToInt32(dr["cd_natureza"]),
					DsNatureza = Convert.ToString(dr["ds_natureza"]),
					CdAtendimentoWeb = Convert.ToString(dr["cd_atendimento_web"])
				};
			};

			List<VO.HC.HcNaturezaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
			return lst;
		}

		internal static List<KeyValuePair<int, string>> ListarMotivosDesligamento(EvidaDatabase db) {
			string sql = "SELECT * " +
				"	FROM ISA_HC.HC_MOTIVO_DESLIGAMENTO B ORDER BY ds_motivo_desligamento";


			List<KeyValuePair<int, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<int>("cd_motivo_desligamento", "ds_motivo_desligamento"));
			return lst;
		}

		internal static List<HcUnidadeOrganizacionalVO> ListarUnidadesOrganizacionais(int empresa, EvidaDatabase db) {
			string sql = "SELECT * FROM  isa_int.INT_UNIDADES_ORGANIZACIONAIS ";

			sql += " WHERE ";
			if (empresa == Constantes.EMPRESA_CEA)
                sql += " upper(trim(DS_UNIORG)) LIKE upper(trim('CEA%')) ";
			else if (empresa == Constantes.EMPRESA_ELETRONOTE)
                sql += " upper(trim(DS_UNIORG)) NOT LIKE upper(trim('CEA%')) ";
			else
				return null;

			sql += " order by ds_uniorg";

			Func<DataRow, VO.HC.HcUnidadeOrganizacionalVO> convert = delegate(DataRow dr) {
				return new VO.HC.HcUnidadeOrganizacionalVO() {
					CdUniOrg = Convert.ToString(dr["CD_UNIORG"]),
					DsUniOrg = Convert.ToString(dr["DS_UNIORG"]),
					SgUniOrg = Convert.ToString(dr["SG_UNIORG"]),
					CdUniOrgPai = Convert.ToString(dr["CD_UNIORG_PAI"]),
					Nivel = Convert.ToInt32(dr["VL_NIVEL"])
				};
			};

			List<HcUnidadeOrganizacionalVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
			return lst;
		}
	}
}
