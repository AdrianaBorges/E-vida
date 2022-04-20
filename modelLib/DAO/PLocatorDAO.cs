using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO
{

    internal class PLocatorDAO
    {
        internal static List<LotacaoVO> ListarLotacoes(EvidaDatabase db)
        {
            string sql = "SELECT bbz_codorg, bbz_descri, bbz_sigla FROM VW_PR_CADASTRO_ORGANIZACIONAL ORDER BY bbz_descri ";

            Func<DataRow, LotacaoVO> convert = delegate(DataRow dr)
            {
                return new LotacaoVO()
                {
                    CdLotacao = Convert.ToString(dr["bbz_codorg"]),
                    DsLotacao = Convert.ToString(dr["bbz_descri"]),
                    SgLotacao = Convert.ToString(dr["bbz_sigla"])
                };
            };

            List<LotacaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);

            return lst;
        }

        internal static List<KeyValuePair<int, string>> ListarRegionais(EvidaDatabase db)
        {
            string sql = "SELECT cd_regional, ds_regional FROM isa_hc.hc_regional ORDER BY ds_regional ";

            List<KeyValuePair<int, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<int>("cd_regional", "ds_regional"));

            return lst;
        }

        internal static List<KeyValuePair<string, string>> ListarRegioes(EvidaDatabase db)
        {
            string sql = "select BIB_CODREG, BIB_DESCRI from VW_PR_REGIAO order by BIB_DESCRI ";

            List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<string>("BIB_CODREG", "BIB_DESCRI"));

            return lst;
        }

        internal static List<KeyValuePair<string, string>> ListarParentescos(EvidaDatabase db)
        {
            string sql = "SELECT BRP_CODIGO, BRP_DESCRI, BRP_CODSIB " +
                " FROM VW_PR_GRAU_PARENTESCO ORDER BY BRP_DESCRI ";

            List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql,
                BaseDAO.Converter<string>("BRP_CODIGO", "BRP_DESCRI"));

            return lst;
        }

        internal static DataTable ListarNomesTitulares(EvidaDatabase db)
        {
            string sql = "select distinct trim(BA1_NOMUSR) as BA1_NOMUSR from VW_PR_USUARIO where trim(BA1_TIPUSU) = 'T' order by trim(BA1_NOMUSR) ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

            return dt;
        }

        internal static DataTable ListarCategorias(EvidaDatabase db)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select bqc_codemp || '-' || bqc_numcon || '-' || bqc_subcon as cd_categoria, bqc_descri as ds_categoria ");
            sql.Append(" from VW_PR_SUBCONTRATO ");
            sql.Append(" order by bqc_descri ");

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql.ToString());
            return dt;
        }

        internal static DataTable ListarCategorias(int empresa, EvidaDatabase db)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select bqc_codemp || '-' || bqc_numcon || '-' || bqc_subcon as cd_categoria, bqc_descri as ds_categoria ");
            sql.Append(" from VW_PR_SUBCONTRATO ");
            sql.Append(" where upper(trim(bqc_codemp)) = upper(trim(:empresa)) ");
            sql.Append(" order by bqc_descri ");

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":empresa", Tipo = DbType.String, Value = empresa.ToString().PadLeft(4, '0') });

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql.ToString(), lstParams);
            return dt;
        }

        internal static List<PGrupoEmpresaVO> ListarEmpresas(EvidaDatabase db)
        {
            string sql = "select BG9_CODIGO, BG9_DESCRI, BG9_NREDUZ, BG9_EMPANT from VW_PR_GRUPO_EMPRESA order by BG9_DESCRI ";

            Func<DataRow, PGrupoEmpresaVO> convert = delegate(DataRow dr)
            {
                return new PGrupoEmpresaVO()
                {
                    Codigo = Convert.ToString(dr["BG9_CODIGO"]),
                    Descri = Convert.ToString(dr["BG9_DESCRI"]),
                    Nreduz = Convert.ToString(dr["BG9_NREDUZ"]),
                    Empant = Convert.ToString(dr["BG9_EMPANT"])
                };
            };
            List<PGrupoEmpresaVO> dt = BaseDAO.ExecuteDataSet(db, sql, convert);

            return dt;
        }

        internal static DataTable ListarNomesBeneficiarios(EvidaDatabase db)
        {
            string sql = "select distinct trim(BA1_NOMUSR) as BA1_NOMUSR from VW_PR_USUARIO order by trim(BA1_NOMUSR) ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

            return dt;
        }

        internal static DataTable BuscarServicos(string cdMascara, string nome, bool diffCodTabela, EvidaDatabase db)
        {

            string sql = "SELECT " +
                (diffCodTabela ? " T.BR8_CODPAD, " : " DISTINCT ") +
                " T.BR8_DESCRI, T.BR8_CODPSA, trim(T.BR8_CODPAD) || '|' || trim(T.BR8_CODPSA) as CD_SERVICO " +
                " FROM VW_PR_TABELA_PADRAO T " +
                " WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(cdMascara))
            {
                sql += " AND trim(T.BR8_CODPSA) = trim(:cdMascara) ";
                lstParams.Add(new Parametro() { Name = ":cdMascara", Tipo = DbType.String, Value = cdMascara });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(T.BR8_DESCRI)) LIKE upper(trim(:nome))";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }

            sql += " ORDER BY " + (diffCodTabela ? "T.BR8_CODPAD, " : "") + " T.BR8_DESCRI ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static HcServicoVO GetServico(int cdServico, EvidaDatabase db)
        {
            string sql = "SELECT * FROM ISA_HC.HC_SERVICO H WHERE CD_SERVICO = :id ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = cdServico });
            List<HcServicoVO> lstServicos = BaseDAO.ExecuteDataSet(db, sql, HcServicoVO.FromDataRow, lstParams);
            if (lstServicos != null && lstServicos.Count > 0)
                return lstServicos[0];
            return null;
        }

        internal static PTabelaPadraoVO GetTabelaPadrao(string codpad, string codpsa, EvidaDatabase db)
        {
            string sql = "SELECT * from VW_PR_TABELA_PADRAO " +
            " WHERE upper(trim(BR8_CODPAD)) = upper(trim(:codpad)) " +
            " AND upper(trim(BR8_CODPSA)) = upper(trim(:codpsa)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codpad", Tipo = DbType.String, Value = codpad });
            lstParams.Add(new Parametro() { Name = ":codpsa", Tipo = DbType.String, Value = codpsa });
            List<PTabelaPadraoVO> lstServicos = BaseDAO.ExecuteDataSet(db, sql, FromDataRowTabelaPadrao, lstParams);
            if (lstServicos != null && lstServicos.Count > 0)
                return lstServicos[0];
            return null;
        }

        internal static PTabelaPadraoVO FromDataRowTabelaPadrao(DataRow dr)
        {
            PTabelaPadraoVO vo = new PTabelaPadraoVO();
            vo.Codpad = Convert.ToString(dr["BR8_CODPAD"]);
            vo.Codpsa = Convert.ToString(dr["BR8_CODPSA"]);
            vo.Anasin = Convert.ToString(dr["BR8_ANASIN"]);
            vo.Descri = Convert.ToString(dr["BR8_DESCRI"]);
            vo.Benutl = Convert.ToString(dr["BR8_BENUTL"]);
            vo.Codrol = Convert.ToString(dr["BR8_CODROL"]);

            return vo;
        }

        internal static DataTable BuscarDoencas(string cdDoenca, string nome, Database db)
        {
            string sql = "SELECT UPPER(H.BA9_DOENCA) BA9_DOENCA, H.BA9_CODDOE FROM VW_PR_DOENCA H WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(cdDoenca))
            {
                sql += " AND trim(H.BA9_CODDOE) = trim(:cdDoenca)";
                lstParams.Add(new Parametro() { Name = ":cdDoenca", Tipo = DbType.String, Value = cdDoenca });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(H.BA9_DOENCA)) LIKE upper(trim(:nome))";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }

            sql += " ORDER BY UPPER(H.BA9_DOENCA) ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable ListarCredenciados(EvidaDatabase db)
        {
            string sql = "SELECT DISTINCT C.CD_CREDENCIADO, C.NM_RAZAO_SOCIAL FROM ISA_HC.HC_V_CREDENCIADO C ORDER BY C.NM_RAZAO_SOCIAL ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

            return dt;
        }

        internal static DataTable ListarRedesAtendimento(EvidaDatabase db)
        {
            string sql = "SELECT DISTINCT BAU_CODIGO, C.BAU_NOME FROM VW_PR_REDE_ATENDIMENTO C ORDER BY C.BAU_NOME ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

            return dt;
        }

        internal static List<HcPlanoVO> ListarPlanos(EvidaDatabase db)
        {
            string sql = "SELECT CD_PLANO, DS_PLANO, TP_PLANO, CD_PLANO_ANS, TP_ABRANGENCIA, VL_GRAU_IMPORTANCIA, DS_SIGLA, CD_LOCAL_ATEND_PADRAO " +
                "	FROM ISA_HC.HC_PLANO P ";
            return BaseDAO.ExecuteDataSet(db, sql, HcPlanoVO.FromDataRow);
        }

        internal static List<PProdutoSaudeVO> ListarProdutoSaude(EvidaDatabase db)
        {
            string sql = "select bi3_versao, bi3_codigo, bi3_descri, bi3_nreduz, bi3_susep, bi3_tipo, bi3_codant, bi3_abrang, bi3_yautpr " +
                "	from VW_PR_PRODUTO_SAUDE ";
            return BaseDAO.ExecuteDataSet(db, sql, PProdutoSaudeVO.FromDataRow);
        }

        internal static List<KeyValuePair<string, string>> ListarConselhoProfissional(EvidaDatabase db)
        {
            string sql = "SELECT BAH_CODIGO, BAH_DESCRI " +
                " FROM VW_PR_CONSELHO_REGIONAL ORDER BY BAH_DESCRI ";

            List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql,
                BaseDAO.Converter<string>("BAH_CODIGO", "BAH_DESCRI"));

            return lst;
        }

        internal static PProfissionalSaudeVO GetProfissional(string seqProfissional, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_PROFISSIONAL_SAUDE P WHERE trim(BB0_CODIGO) = trim(:seq)";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":seq", DbType.String, seqProfissional));
            List<PProfissionalSaudeVO> lst = BaseDAO.ExecuteDataSet(db, sql, PProfissionalSaudeVO.FromDataRow, lstParams);
            if (lst == null || lst.Count == 0)
                return null;
            return lst[0];
        }

        internal static List<PProfissionalSaudeVO> BuscarProfissionais(string nroConselho, string nome, string uf, string codConselho, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_PROFISSIONAL_SAUDE P " +
                " WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();

            if (!string.IsNullOrEmpty(nroConselho))
            {
                sql += " AND trim(P.BB0_NUMCR) = trim(:nroConselho) ";
                lstParams.Add(new Parametro() { Name = ":nroConselho", Tipo = DbType.String, Value = nroConselho });
            }
            if (!string.IsNullOrEmpty(codConselho))
            {
                sql += " AND trim(P.BB0_CODSIG) = trim(:codConselho) ";
                lstParams.Add(new Parametro() { Name = ":codConselho", Tipo = DbType.String, Value = codConselho });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(P.BB0_NOME)) LIKE upper(trim(:nome))";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }
            if (!string.IsNullOrEmpty(uf))
            {
                sql += " AND trim(P.BB0_ESTADO) = trim(:uf)";
                lstParams.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });
            }
            sql += " ORDER BY BB0_NOME ";
            return BaseDAO.ExecuteDataSet(db, sql, FromDataRowProfissionalSaude, lstParams);
        }

        internal static PProfissionalSaudeVO FromDataRowProfissionalSaude(DataRow dr)
        {
            PProfissionalSaudeVO vo = new PProfissionalSaudeVO();

            vo.Codigo = Convert.ToString(dr["BB0_CODIGO"]);
            vo.Nome = Convert.ToString(dr["BB0_NOME"]);
            vo.Cgc = Convert.ToString(dr["BB0_CGC"]);
            vo.Codsig = Convert.ToString(dr["BB0_CODSIG"]);
            vo.Numcr = Convert.ToString(dr["BB0_NUMCR"]);
            vo.Estado = Convert.ToString(dr["BB0_ESTADO"]);
            vo.Tel = Convert.ToString(dr["BB0_TEL"]);
            vo.Email = Convert.ToString(dr["BB0_EMAIL"]);
            vo.Codblo = Convert.ToString(dr["BB0_CODBLO"]);
            vo.Datblo = Convert.ToString(dr["BB0_DATBLO"]);

            return vo;
        }

        internal static DataTable ListarItensLista(string lista, EvidaDatabase db)
        {
            string sql = "SELECT il.cd_item_lista, il.ds_item_lista, CD_ORDEM_EXIBICAO " +
                " FROM SCL_LISTA L, SCL_SISTEMA S, SCL_ITEM_LISTA IL " +
                " WHERE l.cd_sistema = s.cd_sistema and il.cd_sistema = l.cd_sistema and il.cd_lista = l.cd_lista " +
                "	AND l.cd_lista = :lista AND S.CD_SISTEMA = 'HC3' " +
                " order by il.cd_ordem_exibicao ";
            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":lista", Tipo = DbType.String, Value = lista });
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarBeneficiarios(string cdAlternativo, string nome, EvidaDatabase db)
        {
            string sql = "SELECT B.CD_BENEFICIARIO, B.CD_ALTERNATIVO, B.NM_BENEFICIARIO FROM ISA_HC.HC_V_BENEFICIARIO B WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(cdAlternativo))
            {
                sql += " AND B.CD_ALTERNATIVO = :cdAlternativo";
                lstParams.Add(new Parametro() { Name = ":cdAlternativo", Tipo = DbType.String, Value = cdAlternativo });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(B.NM_BENEFICIARIO)) LIKE upper(trim(:nome)) ";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }

            sql += " ORDER BY B.NM_BENEFICIARIO  ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarUsuarios(string cdAlternativo, string nome, EvidaDatabase db)
        {
            string sql = "SELECT B.BA1_CODINT, B.BA1_CODEMP, B.BA1_MATRIC, B.BA1_TIPREG, B.BA1_MATEMP, trim(B.BA1_CODINT) || '|' || trim(B.BA1_CODEMP) || '|' || trim(B.BA1_MATRIC) || '|' || trim(B.BA1_TIPREG) as cd_beneficiario, B.BA1_MATANT, B.BA1_NOMUSR FROM VW_PR_USUARIO_ATUAL B WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(cdAlternativo))
            {
                sql += " AND trim(B.BA1_MATANT) = trim(:cdAlternativo) ";
                lstParams.Add(new Parametro() { Name = ":cdAlternativo", Tipo = DbType.String, Value = cdAlternativo });
            }
            if (!string.IsNullOrEmpty(nome))
            {
                sql += " AND upper(trim(B.BA1_NOMUSR)) LIKE upper(trim(:nome)) ";
                lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
            }

            sql += " ORDER BY trim(B.BA1_NOMUSR) ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static VO.PCepVO FromCepDataRow(DataRow dr)
        {
            VO.PCepVO vo = new VO.PCepVO();
            vo.Cep = dr["BC9_CEP"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BC9_CEP"]);
            vo.Codmun = dr["BC9_CODMUN"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BC9_CODMUN"]);
            vo.Bairro = dr["BC9_BAIRRO"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BC9_BAIRRO"]);
            vo.Mun = dr["BID_DESCRI"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BID_DESCRI"]);
            vo.End = dr["BC9_END"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BC9_END"]);
            vo.Tiplog = dr["BC9_TIPLOG"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BC9_TIPLOG"]);
            vo.Desclog = dr["B18_DESCRI"] == DBNull.Value ? String.Empty : Convert.ToString(dr["B18_DESCRI"]);
            vo.Est = dr["BID_EST"] == DBNull.Value ? String.Empty : Convert.ToString(dr["BID_EST"]);

            // Teddy solicitou no SGA 15106957 em 28/02/2019 para retirar esta concatenação do tipo de logradouro.
            /*
            if (!string.IsNullOrEmpty(vo.Desclog.Trim()) && !vo.End.StartsWith(vo.Desclog.Trim()) && !vo.End.Contains(" - " + vo.Desclog.Trim() + " ") && !string.IsNullOrEmpty(vo.End.Trim()))
                vo.End = vo.Desclog.Trim() + " " + vo.End.Trim();
            */

            return vo;
        }

        internal static DataTable BuscarMunicipios(string uf, EvidaDatabase db)
        {
            // O Tiago pediu pra alterar em 26/03/2018, para passar a pegar do Protheus
            //string sql = "SELECT DISTINCT NO_LOCALIDADE FROM CEP WHERE SG_UF = :uf order by NO_LOCALIDADE";
            string sql = "select distinct trim(BID_DESCRI) NO_LOCALIDADE from vw_pr_municipio where trim(BID_EST) = trim(:uf) order by trim(BID_DESCRI)";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });

            return BaseDAO.ExecuteDataSet(db, sql, lstP);
        }

        internal static DataTable BuscarMunicipiosISA(string uf, EvidaDatabase db)
        {
            string sql = "SELECT DISTINCT CD_MUNICIPIO, DS_MUNICIPIO FROM ISA_HC.HC_MUNICIPIO WHERE CD_ESTADO = :uf order by DS_MUNICIPIO";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });

            return BaseDAO.ExecuteDataSet(db, sql, lstP);
        }

        internal static DataTable BuscarMunicipiosProtheus(string uf, EvidaDatabase db)
        {
            string sql = "select distinct BID_CODMUN, BID_DESCRI from vw_pr_municipio where trim(BID_EST) = trim(:uf) order by BID_DESCRI";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":uf", Tipo = DbType.String, Value = uf });

            return BaseDAO.ExecuteDataSet(db, sql, lstP);
        }

        internal static VO.PCepVO BuscarCepProtheus(string cep, EvidaDatabase db)
        {
            StringBuilder v_sql = new StringBuilder();

            v_sql.Append(" select upper(trim(c.BC9_CEP)) BC9_CEP, upper(trim(c.BC9_CODMUN)) BC9_CODMUN, upper(trim(c.BC9_END)) BC9_END, upper(trim(c.BC9_BAIRRO)) BC9_BAIRRO, ");
            v_sql.Append(" upper(trim(c.BC9_TIPLOG)) BC9_TIPLOG, upper(trim(t.B18_DESCRI)) B18_DESCRI, upper(trim(c.BC9_EST)) BID_EST, upper(trim(m.BID_DESCRI)) BID_DESCRI ");
            v_sql.Append(" from VW_PR_CEP c ");
            v_sql.Append(" left join VW_PR_MUNICIPIO m ");
            v_sql.Append(" on trim(c.BC9_CODMUN) = trim(m.BID_CODMUN) ");
            v_sql.Append(" left join VW_PR_TIPO_LOGRADOURO t ");
            v_sql.Append(" on trim(c.BC9_TIPLOG) = trim(t.B18_CODIGO) ");
            v_sql.Append(" where upper(trim(c.BC9_CEP)) = upper(trim(:cep)) ");

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":cep", Tipo = DbType.String, Value = cep.PadLeft(8, '0') });

            List<VO.PCepVO> lst = BaseDAO.ExecuteDataSet(db, v_sql.ToString(), FromCepDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static List<VO.HC.HcBancoVO> ListarBancoISA(EvidaDatabase db)
        {
            string sql = "SELECT * FROM HC_BANCO B order by CD_BANCO ";

            Func<DataRow, VO.HC.HcBancoVO> convert = delegate(DataRow dr)
            {
                return new VO.HC.HcBancoVO()
                {
                    Id = Convert.ToInt32(dr["cd_banco"]),
                    Nome = Convert.ToString(dr["ds_banco"]),
                    AceitaDebitoConta = "S".Equals(Convert.ToString(dr["fl_aceita_debito_conta"]), StringComparison.InvariantCultureIgnoreCase)
                };
            };

            List<VO.HC.HcBancoVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
            return lst;
        }

        internal static VO.HC.HcBancoVO GetBancoISA(int cdBanco, EvidaDatabase db)
        {
            string sql = "SELECT * " +
                "	FROM HC_BANCO B WHERE cd_banco = :idBanco";

            Func<DataRow, VO.HC.HcBancoVO> convert = delegate(DataRow dr)
            {
                return new VO.HC.HcBancoVO()
                {
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

        internal static List<VO.HC.HcAgenciaBancariaVO> ListarAgenciaBancoISA(EvidaDatabase db)
        {
            string sql = "SELECT * " +
                "	FROM HC_AGENCIA_BANCARIA B ";

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

            List<VO.HC.HcAgenciaBancariaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
            return lst;
        }

        internal static HcAgenciaBancariaVO GetAgenciaBancoISA(int idBanco, string cdAgencia, EvidaDatabase db)
        {
            string sql = "SELECT * " +
                "	FROM HC_AGENCIA_BANCARIA B where CD_BANCO = :idBanco AND CD_AGENCIA = :cdAgencia ";

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

            List<VO.HC.HcAgenciaBancariaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static HcAgenciaBancariaVO GetAgenciaDvBancoISA(int idBanco, string cdAgencia, string dvAgencia, EvidaDatabase db)
        {
            string sql = "SELECT * " +
                "	FROM HC_AGENCIA_BANCARIA B where CD_BANCO = :idBanco AND CD_AGENCIA = :cdAgencia AND NR_DV_AGENCIA = :dvAgencia ";

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
            sql.Append(" select * from hc_agencia_bancaria a ");
            sql.Append(" where exists (select * from VW_PR_FAMILIA f where upper(trim(f.BA3_AGECLI)) = upper(trim(a.cd_agencia)) and a.nr_dv_agencia is null and trim(f.BA3_AGECLI) = :cdAgencia) ");
            sql.Append(" and a.cd_banco = :idBanco ");
            sql.Append(" union ");
            sql.Append(" select * from hc_agencia_bancaria a where exists (select * from VW_PR_FAMILIA f where upper(substr(trim(f.BA3_AGECLI), 1, length(trim(f.BA3_AGECLI)) - 1)) = upper(trim(a.cd_agencia)) and upper(substr(trim(f.BA3_AGECLI), length(trim(f.BA3_AGECLI)), 1)) = upper(trim(a.nr_dv_agencia)) and trim(f.BA3_AGECLI) = :cdAgencia) ");
            sql.Append(" and a.cd_banco = :idBanco ");
            sql.Append(" union ");
            sql.Append(" select * from hc_agencia_bancaria a where exists (select * from VW_PR_FAMILIA f where upper(substr(trim(f.BA3_AGECLI), 1, length(trim(f.BA3_AGECLI)) - 2)) = upper(trim(a.cd_agencia)) and upper(substr(trim(f.BA3_AGECLI), length(trim(f.BA3_AGECLI)) - 1, 2)) = upper(trim(a.nr_dv_agencia)) and trim(f.BA3_AGECLI) = :cdAgencia) ");
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

        internal static int? GetCdMunicipioFromAdesao(string cidade, string uf, EvidaDatabase db)
        {
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

        internal static List<HcNaturezaVO> ListarNatureza(EvidaDatabase db)
        {
            string sql = "SELECT * " +
                "	FROM ISA_HC.HC_NATUREZA_CREDENCIADO B ORDER BY DS_NATUREZA";

            Func<DataRow, VO.HC.HcNaturezaVO> convert = delegate(DataRow dr)
            {
                return new VO.HC.HcNaturezaVO()
                {
                    CdNatureza = Convert.ToInt32(dr["cd_natureza"]),
                    DsNatureza = Convert.ToString(dr["ds_natureza"]),
                    CdAtendimentoWeb = Convert.ToString(dr["cd_atendimento_web"])
                };
            };

            List<VO.HC.HcNaturezaVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
            return lst;
        }

        internal static List<PEspecialidadeVO> ListarEspecialidade(EvidaDatabase db)
        {
            string sql = "SELECT * from VW_PR_ESPECIALIDADE order by BAQ_DESCRI";

            Func<DataRow, VO.Protheus.PEspecialidadeVO> convert = delegate(DataRow dr)
            {
                return new VO.Protheus.PEspecialidadeVO()
                {
                    Codint = Convert.ToString(dr["BAQ_CODINT"]),
                    Codesp = Convert.ToString(dr["BAQ_CODESP"]),
                    Descri = Convert.ToString(dr["BAQ_DESCRI"]),
                    Cbos = Convert.ToString(dr["BAQ_CBOS"]),
                    Descbo = Convert.ToString(dr["BAQ_DESCBO"])
                };
            };

            List<VO.Protheus.PEspecialidadeVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
            return lst;
        }

        internal static List<PEspecialidadeVO> ListarEspecialidade(string codigo, EvidaDatabase db)
        {
            string sql = "select * " +
                "	from VW_PR_ESPECIALIDADE_REDE r " +
                "   inner join VW_PR_ESPECIALIDADE e " +
                "   on trim(r.BBF_CDESP) = trim(e.BAQ_CODESP) " +
                "   where trim(r.BBF_CODIGO) = trim(:codigo) " +
                "   order by e.BAQ_DESCRI ";

            Func<DataRow, VO.Protheus.PEspecialidadeVO> convert = delegate(DataRow dr)
            {
                return new VO.Protheus.PEspecialidadeVO()
                {
                    Codint = Convert.ToString(dr["BAQ_CODINT"]),
                    Codesp = Convert.ToString(dr["BAQ_CODESP"]),
                    Descri = Convert.ToString(dr["BAQ_DESCRI"]),
                    Cbos = Convert.ToString(dr["BAQ_CBOS"]),
                    Descbo = Convert.ToString(dr["BAQ_DESCBO"])
                };
            };

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo });

            List<VO.Protheus.PEspecialidadeVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
            return lst;
        }

        internal static List<PClasseRedeAtendimentoVO> ListarClasseRedeAtendimento(EvidaDatabase db)
        {
            string sql = "SELECT * from VW_PR_CLASSE_REDE_ATENDIMENTO order by BAG_DESCRI";

            Func<DataRow, VO.Protheus.PClasseRedeAtendimentoVO> convert = delegate(DataRow dr)
            {
                return new VO.Protheus.PClasseRedeAtendimentoVO()
                {
                    Filial = Convert.ToString(dr["BAG_FILIAL"]),
                    Codigo = Convert.ToString(dr["BAG_CODIGO"]),
                    Descri = Convert.ToString(dr["BAG_DESCRI"]),
                    Codpt = Convert.ToString(dr["BAG_CODPT"]),
                    Forcon = Convert.ToString(dr["BAG_FORCON"]),
                    Qtdesp = Convert.ToString(dr["BAG_QTDESP"]),
                    Tippe = Convert.ToString(dr["BAG_TIPPE"]),
                    Obrcid = Convert.ToString(dr["BAG_OBRCID"]),
                    Consft = Convert.ToString(dr["BAG_CONSFT"]),
                };
            };

            List<VO.Protheus.PClasseRedeAtendimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
            return lst;
        }

        internal static List<KeyValuePair<string, string>> ListarMotivosDesligamentoFamilia(EvidaDatabase db)
        {
            string sql = "select bg3_codblo as cd_motivo_desligamento, bg3_desblo as ds_motivo_desligamento from VW_PR_BLOQUEIO_DA_FAMILIA order by bg3_desblo";

            List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<string>("cd_motivo_desligamento", "ds_motivo_desligamento"));
            return lst;
        }

        internal static List<KeyValuePair<string, string>> ListarMotivosDesligamentoUsuario(EvidaDatabase db)
        {
            string sql = "select bg1_codblo as cd_motivo_desligamento, bg1_desblo as ds_motivo_desligamento from VW_PR_BLOQUEIO_DO_USUARIO order by bg1_desblo";

            List<KeyValuePair<string, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<string>("cd_motivo_desligamento", "ds_motivo_desligamento"));
            return lst;
        }

        internal static string ObterDescricaoMotivoDesligamentoFamilia(string codigo, EvidaDatabase db)
        {
            string sql = "select BG3_DESBLO from vw_pr_bloqueio_da_familia where trim(BG3_CODBLO) = trim(:codigo)";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":codigo", DbType.String, codigo));

            object ret = BaseDAO.ExecuteScalar(db, sql, lstParams);
            if (ret == DBNull.Value)
                return "";
            return Convert.ToString(ret).Trim();
        }

        internal static string ObterDescricaoMotivoDesligamentoUsuario(string codigo, EvidaDatabase db)
        {
            string sql = "select BG1_DESBLO from vw_pr_bloqueio_do_usuario where trim(BG1_CODBLO) = trim(:codigo)";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":codigo", DbType.String, codigo));

            object ret = BaseDAO.ExecuteScalar(db, sql, lstParams);
            if (ret == DBNull.Value)
                return "";
            return Convert.ToString(ret).Trim();
        }

        internal static List<HcUnidadeOrganizacionalVO> ListarUnidadesOrganizacionais(int empresa, EvidaDatabase db)
        {
            string sql = "SELECT * FROM  isa_int.INT_UNIDADES_ORGANIZACIONAIS ";

            sql += " WHERE ";
            if (empresa == Constantes.EMPRESA_CEA)
                sql += " upper(trim(DS_UNIORG)) LIKE upper(trim('CEA%')) ";
            else if (empresa == Constantes.EMPRESA_ELETRONOTE)
                sql += " upper(trim(DS_UNIORG)) NOT LIKE upper(trim('CEA%')) ";
            else
                return null;

            sql += " order by ds_uniorg";

            Func<DataRow, VO.HC.HcUnidadeOrganizacionalVO> convert = delegate(DataRow dr)
            {
                return new VO.HC.HcUnidadeOrganizacionalVO()
                {
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

        internal static List<PCadastroOrganizacionalVO> ListarCadastrosOrganizacionais(int empresa, EvidaDatabase db)
        {
            string sql = " select * from VW_PR_CADASTRO_ORGANIZACIONAL ";

            sql += " where ";
            if (empresa == PConstantes.EMPRESA_CEA)
                sql += " upper(trim(bbz_descri)) like upper(trim('CEA%')) ";
            else if (empresa == PConstantes.EMPRESA_ELETRONORTE)
                sql += " upper(trim(bbz_descri)) not like upper(trim('CEA%')) ";
            else
                return null;

            sql += " order by bbz_descri ";

            Func<DataRow, PCadastroOrganizacionalVO> convert = delegate(DataRow dr)
            {
                return new PCadastroOrganizacionalVO()
                {
                    Codseq = Convert.ToString(dr["BBZ_CODSEQ"]),
                    Codorg = Convert.ToString(dr["BBZ_CODORG"]),
                    Descri = Convert.ToString(dr["BBZ_DESCRI"]),
                    Sigla = Convert.ToString(dr["BBZ_SIGLA"]),
                    Filial = Convert.ToString(dr["BBZ_FILIAL"])
                };
            };

            List<PCadastroOrganizacionalVO> lst = BaseDAO.ExecuteDataSet(db, sql, convert);
            return lst;
        }

        internal static DataTable BuscarPeg(String codRda, DateTime? dataEntrada, String docFiscal, decimal? valorApresentado, DateTime? dataEmissao, DateTime? dataVencimento, Database db)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select trim(bci_codpeg) bci_codpeg, trim(bci_codrda) bci_codrda, trim(bci_cgcrda) bci_cgcrda, trim(bci_nomrda) bci_nomrda, trim(bci_yredoc) bci_yredoc, trim(bci_ydoc) bci_ydoc, trim(bci_yvldoc) bci_yvldoc, trim(bci_yemiss) bci_yemiss, trim(bci_yvedoc) bci_yvedoc ");
            sql.Append(" from VW_PR_PEG p ");
            sql.Append(" where 1 = 1 ");
            sql.Append(" and not exists (select * from EV_PROTOCOLO_FATURA where nr_protocolo like '%' || p.bci_codpeg || '%' ) ");

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(codRda))
            {
                sql.Append(" AND upper(trim(bci_codrda)) = upper(trim(:codRda)) ");
                lstParams.Add(new Parametro() { Name = ":codRda", Tipo = DbType.String, Value = codRda });
            }
            if (dataEntrada != null)
            {
                sql.Append(" AND upper(trim(bci_yredoc)) = upper(trim(:dataEntrada)) ");
                lstParams.Add(new Parametro() { Name = ":dataEntrada", Tipo = DbType.String, Value = Convert.ToDateTime(dataEntrada).ToString("yyyyMMdd") });
            }
            if (!string.IsNullOrEmpty(docFiscal))
            {
                sql.Append(" AND upper(trim(bci_ydoc)) like upper(trim(:docFiscal)) ");
                lstParams.Add(new Parametro() { Name = ":docFiscal", Tipo = DbType.String, Value = '%' + docFiscal.Trim().ToUpper() + '%' });
            }
            if (valorApresentado != null)
            {
                sql.Append(" AND upper(trim(bci_yvldoc)) = upper(trim(:valorApresentado)) ");
                lstParams.Add(new Parametro() { Name = ":valorApresentado", Tipo = DbType.Double, Value = valorApresentado });
            }
            if (dataEmissao != null)
            {
                sql.Append(" AND upper(trim(bci_yemiss)) = upper(trim(:dataEmissao)) ");
                lstParams.Add(new Parametro() { Name = ":dataEmissao", Tipo = DbType.String, Value = Convert.ToDateTime(dataEmissao).ToString("yyyyMMdd") });
            }
            if (dataVencimento != null)
            {
                sql.Append(" AND upper(trim(bci_yvedoc)) = upper(trim(:dataVencimento)) ");
                lstParams.Add(new Parametro() { Name = ":dataVencimento", Tipo = DbType.String, Value = Convert.ToDateTime(dataVencimento).ToString("yyyyMMdd") });
            }
            sql.Append(" ORDER BY bci_codpeg desc ");
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql.ToString(), lstParams);

            return dt;
        }

        internal static DataTable BuscarPegFinanceiro(int codigo, Database db)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" select E2_VALOR, E2_ISS, E2_IRRF, E2_DESCONT, E2_COFINS, E2_PIS, E2_CSLL, E2_BAIXA, E2_VALLIQ ");
            sql.Append(" from EV_PROTOCOLO_FATURA ");
            sql.Append(" where CD_PROTOCOLO_FATURA = :codigo ");

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codigo", Tipo = DbType.Int32, Value = codigo });
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql.ToString(), lstParams);

            return dt;
        }

        internal static PPegVO GetPeg(string codpeg, string codrda, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_PEG " +
                "	WHERE trim(BCI_CODPEG) = trim(:codpeg) and trim(BCI_CODRDA) = trim(:codrda) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":codpeg", Tipo = DbType.String, Value = codpeg });
            lstP.Add(new Parametro() { Name = ":codrda", Tipo = DbType.String, Value = codrda });

            List<PPegVO> lst = BaseDAO.ExecuteDataSet(db, sql, PPegVO.FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PClasseRedeAtendimentoVO GetClasseRedeAtendimento(string codigo, EvidaDatabase db)
        {
            string sql = "SELECT * from VW_PR_CLASSE_REDE_ATENDIMENTO " +
            " WHERE upper(trim(BAG_CODIGO)) = upper(trim(:codigo)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo });
            List<PClasseRedeAtendimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowClasseRedeAtendimento, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PClasseRedeAtendimentoVO FromDataRowClasseRedeAtendimento(DataRow dr)
        {
            PClasseRedeAtendimentoVO vo = new PClasseRedeAtendimentoVO();
            vo.Filial = Convert.ToString(dr["BAG_FILIAL"]);
            vo.Codigo = Convert.ToString(dr["BAG_CODIGO"]);
            vo.Descri = Convert.ToString(dr["BAG_DESCRI"]);
            vo.Codpt = Convert.ToString(dr["BAG_CODPT"]);
            vo.Forcon = Convert.ToString(dr["BAG_FORCON"]);
            vo.Qtdesp = Convert.ToString(dr["BAG_QTDESP"]);
            vo.Tippe = Convert.ToString(dr["BAG_TIPPE"]);
            vo.Obrcid = Convert.ToString(dr["BAG_OBRCID"]);
            vo.Consft = Convert.ToString(dr["BAG_CONSFT"]);

            return vo;
        }

        internal static PEmpresaModalidadeCobrancaVO GetEmpresaModalidadeCobranca(string codigo, string numcon, string vercon, string subcon, string versub, string codpro, string versao, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_EMPR_MODALIDADE_COBRANCA " +
                "	WHERE trim(BT9_CODIGO) = trim(:codigo) " +
                "	AND trim(BT9_NUMCON) = trim(:numcon) " +
                "	AND trim(BT9_VERCON) = trim(:vercon) " +
                "	AND trim(BT9_SUBCON) = trim(:subcon) " +
                "	AND trim(BT9_VERSUB) = trim(:versub) " +
                "	AND trim(BT9_CODPRO) = trim(:codpro) " +
                "	AND trim(BT9_VERSAO) = trim(:versao) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo });
            lstP.Add(new Parametro() { Name = ":numcon", Tipo = DbType.String, Value = numcon });
            lstP.Add(new Parametro() { Name = ":vercon", Tipo = DbType.String, Value = vercon });
            lstP.Add(new Parametro() { Name = ":subcon", Tipo = DbType.String, Value = subcon });
            lstP.Add(new Parametro() { Name = ":versub", Tipo = DbType.String, Value = versub });
            lstP.Add(new Parametro() { Name = ":codpro", Tipo = DbType.String, Value = codpro });
            lstP.Add(new Parametro() { Name = ":versao", Tipo = DbType.String, Value = versao });

            List<PEmpresaModalidadeCobrancaVO> lst = BaseDAO.ExecuteDataSet(db, sql, PEmpresaModalidadeCobrancaVO.FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static POperadoraSaudeVO GetOperadoraSaude(string codint, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_OPERADORA_SAUDE " +
                "	WHERE trim(BA0_CODINT) = trim(:codint) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });

            List<POperadoraSaudeVO> lst = BaseDAO.ExecuteDataSet(db, sql, POperadoraSaudeVO.FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PFormaPagamentoVO GetFormaPagamento(string codigo, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_FORMA_PAGAMENTO " +
                "	WHERE trim(BJ0_CODIGO) = trim(:codigo) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo });

            List<PFormaPagamentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, PFormaPagamentoVO.FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PModalidadeCobrancaVO GetModalidadeCobranca(string codigo, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_MODALIDADE_COBRANCA " +
                "	WHERE trim(BJ1_CODIGO) = trim(:codigo) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":codigo", Tipo = DbType.String, Value = codigo });

            List<PModalidadeCobrancaVO> lst = BaseDAO.ExecuteDataSet(db, sql, PModalidadeCobrancaVO.FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PSubcontratoVO GetSubcontrato(string numcon, string vercon, string subcon, string versub, EvidaDatabase db)
        {
            string sql = "SELECT * from VW_PR_SUBCONTRATO " +
            " WHERE upper(trim(BQC_NUMCON)) = upper(trim(:numcon)) " +
            " AND upper(trim(BQC_VERCON)) = upper(trim(:vercon)) " +
            " AND upper(trim(BQC_SUBCON)) = upper(trim(:subcon)) " +
            " AND upper(trim(BQC_VERSUB)) = upper(trim(:versub)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":numcon", Tipo = DbType.String, Value = numcon });
            lstParams.Add(new Parametro() { Name = ":vercon", Tipo = DbType.String, Value = vercon });
            lstParams.Add(new Parametro() { Name = ":subcon", Tipo = DbType.String, Value = subcon });
            lstParams.Add(new Parametro() { Name = ":versub", Tipo = DbType.String, Value = versub });
            List<PSubcontratoVO> lst = BaseDAO.ExecuteDataSet(db, sql, PSubcontratoVO.FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static string ObterReducaoEndereco(string endereco, string numero, string complemento, string bairro, EvidaDatabase db)
        {
            string sql = "select evida.f_reduz_endereco(:endereco, :numero, :complemento, :bairro) from dual";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":endereco", DbType.String, endereco));
            lstParams.Add(new Parametro(":numero", DbType.String, numero));
            lstParams.Add(new Parametro(":complemento", DbType.String, complemento));
            lstParams.Add(new Parametro(":bairro", DbType.String, bairro));

            object ret = BaseDAO.ExecuteScalar(db, sql, lstParams);
            if (ret == DBNull.Value)
                return "";
            return Convert.ToString(ret).Trim();
        }

    }
}
