using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO.Adesao
{
    internal static class PDeclaracaoDAO
    {
        internal static DataTable BuscarResumo(EvidaDatabase evdb)
        {
            string sql = "SELECT D.ID_EMPRESA, D.ID_PRODUTO, COUNT(1) QTD_TOTAL, " +
                " SUM(CASE WHEN ID_STATUS = 0 THEN 1 ELSE 0 END) QTD_PENDENTE, " +
                " SUM(CASE WHEN ID_STATUS >= 1 THEN 1 ELSE 0 END) QTD_VERIFICADO, " +
                " SUM(CASE WHEN ID_STATUS = 2 THEN 1 ELSE 0 END) QTD_VALIDADO, " +
                " SUM(CASE WHEN ID_STATUS = 3 THEN 1 ELSE 0 END) QTD_INVALIDADO, " +
                " SUM(CASE WHEN ID_STATUS = 4 THEN 1 ELSE 0 END) QTD_INTEGRADO " +
                "	FROM DECLARACAO D " +
                "	GROUP BY D.ID_EMPRESA, D.ID_PRODUTO " +
                "	ORDER BY D.ID_EMPRESA, D.ID_PRODUTO ";

            DataTable dt = BaseDAO.ExecuteDataSet(evdb, sql);
            TraduzirProduto(dt);
            TraduzirEmpresa(dt);

            dt.DefaultView.Sort = "EMPRESA, PRODUTO";

            return dt;
        }

        private static void TraduzirProduto(DataTable dt)
        {
            if (dt.Columns.Contains("ID_PRODUTO") && !dt.Columns.Contains("PRODUTO"))
            {
                dt.Columns.Add("PRODUTO", typeof(string));
                foreach (DataRow dr in dt.Rows)
                {
                    PDados.Produto p = PDados.Produto.Find(Convert.ToString(dr["ID_PRODUTO"]));
                    dr["PRODUTO"] = p != null ? p.Descricao : Convert.ToString(dr["ID_PRODUTO"]);
                }
            }
        }

        private static void TraduzirEmpresa(DataTable dt)
        {
            if (dt.Columns.Contains("ID_EMPRESA") && !dt.Columns.Contains("EMPRESA"))
            {
                dt.Columns.Add("EMPRESA", typeof(string));
                foreach (DataRow dr in dt.Rows)
                {
                    PDados.Empresa p = (PDados.Empresa)Enum.Parse(typeof(PDados.Empresa), dr.Field<string>("id_empresa"));
                    dr["EMPRESA"] = PDados.EnumTradutor.TraduzEmpresa(p);
                }
            }
        }

        private static void TraduzirIdDeclaracao(DataTable dt)
        {
            if (dt.Columns.Contains("ID_BYTE") && !dt.Columns.Contains("ID_DECLARACAO"))
            {
                dt.Columns.Add("ID_DECLARACAO", typeof(Guid));
                foreach (DataRow dr in dt.Rows)
                {
                    byte[] idByte = (byte[])dr["ID_BYTE"];
                    dr["ID_DECLARACAO"] = new Guid(idByte);
                }
            }
        }

        internal static DataTable Pesquisar(PDados.Empresa? empresa, int? numero, long? matricula, PDados.SituacaoDeclaracao? status, EvidaDatabase evdb)
        {
            Database db = evdb.Database;

            string sql = "SELECT D.ID_EMPRESA, D.ID_PRODUTO, ID_DECLARACAO ID_BYTE, ID_STATUS, NU_DECLARACAO, " +
                " DS_NOME NOME_TITULAR, ID_MATRICULA, DT_CRIACAO, DT_RECEBIDA, DT_VALIDADA, DS_VALIDACAO, ID_STATUS, DT_INTEGRACAO " +
                "	FROM DECLARACAO D " +
                "	WHERE 1 = 1 ";

            List<Parametro> lstParams = new List<Parametro>();
            if (numero != null)
            {
                sql += " AND NU_DECLARACAO = :numero ";
                lstParams.Add(new Parametro(":numero", DbType.Int32, numero));
            }
            if (matricula != null)
            {
                sql += " AND ID_MATRICULA = :matricula ";
                lstParams.Add(new Parametro(":matricula", DbType.Int64, matricula));
            }
            if (empresa != null)
            {
                sql += " AND ID_EMPRESA = :empresa ";
                lstParams.Add(new Parametro(":empresa", DbType.String, empresa.ToString()));
            }
            if (status != null)
            {
                sql += " AND ID_STATUS = :status";
                lstParams.Add(new Parametro(":status", DbType.Int32, (int)status.Value));
            }

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            TraduzirProduto(dt);
            TraduzirIdDeclaracao(dt);

            return dt;
        }

        internal static void MarcarRecebida(int id, DbTransaction transaction, Database db)
        {
            String sql = "UPDATE DECLARACAO SET ID_STATUS = :status, DT_RECEBIDA = LOCALTIMESTAMP WHERE NU_DECLARACAO = :id";
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)PDados.SituacaoDeclaracao.RECEBIDA);

            BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
        }

        internal static PDeclaracaoVO GetById(int numProposta, EvidaDatabase evdb)
        {
            string sql = "SELECT id_declaracao, nu_declaracao, dt_criacao, id_produto, id_matricula, ds_nome, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, dt_nascimento, ds_cns, ds_cpf, ds_nome_mae, ds_nome_pai, ds_rg, BA1_SEXO, " +
            "	dt_admissao, ds_email, BA1_ESTCIV, ds_lotacao, ds_orgao_expedidor, ds_uf_orgao_expedidor, dt_emissao_rg, ds_tel_celular, ds_tel_residencial, ds_tel_comercial, " +
            "	ds_bairro, ds_cidade, ds_complemento, ds_numero, ds_rua, ds_uf, ds_cep, id_autorizacao, dt_inicio, " +
            "	d.id_banco, ds_agencia, ds_conta_corrente, ds_dv_agencia, ds_dv_conta_corrente, nu_dia_pagamento, ds_local, " +
            "	ds_nome_requerente, id_parentesco, vl_inss, vl_previnorte, vl_outras_fontes, b.no_banco, " +
            "	ds_nome_resp_financ, ds_cpf_resp_financ, dt_nascimento_resp_financ, ds_sexo_resp_financ, ds_tel_resp_financ, ds_email_resp_financ, " +
            "	tp_plano, id_empresa, dt_validada, ds_validacao, dt_inicio_integracao, dt_integracao, id_usuario_integracao, cd_categoria_integracao, " +
            "	BG1_CODBLO, BG3_CODBLO, cd_plano_integracao, tp_carencia_integracao " +
            " FROM DECLARACAO d, EVIDA.BANCO b WHERE nu_declaracao = :id AND d.id_banco = b.id_banco(+) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = numProposta });

            List<PDeclaracaoVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
            {
                PDeclaracaoVO vo = lst[0];

                vo.Dependentes = FindDependentesByNum(numProposta, evdb);
                return vo;
            }
            return null;
        }

        private static PDeclaracaoVO FromDataRow(DataRow dr)
        {
            PDeclaracaoVO vo = new PDeclaracaoVO();

            vo.Id = new Guid((byte[])(dr["id_declaracao"]));
            vo.Numero = Convert.ToInt32(dr["nu_declaracao"]);
            vo.Criacao = Convert.ToDateTime(dr["dt_criacao"]);
            vo.Produto = Convert.ToString(dr["id_produto"]).Trim();
            vo.Titular = new PPessoaVO();
            vo.Titular.Codint = (dr["BA1_CODINT"] != DBNull.Value) ? Convert.ToString(dr["BA1_CODINT"]).Trim() : "";
            vo.Titular.Codemp = (dr["BA1_CODEMP"] != DBNull.Value) ? Convert.ToString(dr["BA1_CODEMP"]).Trim() : "";
            vo.Titular.Matric = (dr["BA1_MATRIC"] != DBNull.Value) ? Convert.ToString(dr["BA1_MATRIC"]).Trim() : "";
            vo.Titular.Tipreg = (dr["BA1_TIPREG"] != DBNull.Value) ? Convert.ToString(dr["BA1_TIPREG"]).Trim() : "";
            vo.Titular.Matemp = (dr["id_matricula"] != DBNull.Value) ? Convert.ToString(dr["id_matricula"]).Trim() : "";
            vo.Titular.Nome = Convert.ToString(dr["ds_nome"]);
            vo.Titular.Nascimento = Convert.ToDateTime(dr["dt_nascimento"]);
            vo.Titular.Cns = Convert.ToString(dr["ds_cns"]);
            vo.Titular.Cpf = Convert.ToString(dr["ds_cpf"]);
            vo.Titular.NomeMae = Convert.ToString(dr["ds_nome_mae"]);
            vo.Titular.NomePai = Convert.ToString(dr["ds_nome_pai"]);
            vo.Titular.Rg = Convert.ToString(dr["ds_rg"]);
            vo.Titular.Sexo = (PDados.Sexo)Convert.ToChar(dr["BA1_SEXO"]);
            vo.Titular.Admissao = dr["dt_admissao"] != DBNull.Value ? Convert.ToDateTime(dr["dt_admissao"]) : new DateTime?();
            vo.Email = Convert.ToString(dr["ds_email"]);
            vo.Titular.Email = vo.Email;
            vo.Titular.EstadoCivil = PDados.DeclaracaoEstadoCivil.Find(Convert.ToString(dr["BA1_ESTCIV"]));
            vo.Titular.Lotacao = Convert.ToString(dr["ds_lotacao"]);
            vo.Titular.OrgaoExpedidor = Convert.ToString(dr["ds_orgao_expedidor"]);
            vo.Titular.UfOrgaoExpedidor = Convert.ToString(dr["ds_uf_orgao_expedidor"]);
            vo.Titular.DataEmissaoRg = dr["dt_emissao_rg"] != DBNull.Value ? Convert.ToDateTime(dr["dt_emissao_rg"]) : new DateTime?();
            vo.TelCelular = Convert.ToString(dr["ds_tel_celular"]);
            vo.TelResidencial = Convert.ToString(dr["ds_tel_residencial"]);
            vo.TelComercial = Convert.ToString(dr["ds_tel_comercial"]);

            vo.Endereco = new PAdesaoEnderecoVO
            {
                Bairro = Convert.ToString(dr["ds_bairro"]),
                Cidade = Convert.ToString(dr["ds_cidade"]),
                Complemento = Convert.ToString(dr["ds_complemento"]),
                NumeroEndereco = Convert.ToString(dr["ds_numero"]),
                Rua = Convert.ToString(dr["ds_rua"]),
                Uf = Convert.ToString(dr["ds_uf"]),
                Cep = Convert.ToString(dr["ds_cep"])
            };

            vo.DadosBancarios = new PDadosBancariosVO
            {
                IdBanco = Convert.ToString(dr["id_banco"]),
                NomeBanco = Convert.ToString(dr["no_banco"]),
                Agencia = Convert.ToString(dr["ds_agencia"]),
                Conta = Convert.ToString(dr["ds_conta_corrente"]),
                DVAgencia = Convert.ToString(dr["ds_dv_agencia"]),
                DVConta = Convert.ToString(dr["ds_dv_conta_corrente"])
            };

            vo.Local = Convert.ToString(dr["ds_local"]);

            //ds_nome_requerente, id_parentesco, vl_inss, vl_previnorte, vl_outras_fontes
            vo.NomeRequerente = Convert.ToString(dr["ds_nome_requerente"]);
            vo.Parentesco = dr["id_parentesco"] != DBNull.Value ? Convert.ToInt32(dr["id_parentesco"]) : new int?();
            vo.Inss = dr["vl_inss"] != DBNull.Value ? Convert.ToDecimal(dr["vl_inss"]) : new decimal?();
            vo.Previnorte = dr["vl_previnorte"] != DBNull.Value ? Convert.ToDecimal(dr["vl_previnorte"]) : new decimal?();
            vo.OutrasFontes = dr["vl_outras_fontes"] != DBNull.Value ? Convert.ToDecimal(dr["vl_outras_fontes"]) : new decimal?();

            vo.OpcaoAutorizacao = dr["id_autorizacao"] != DBNull.Value ? Convert.ToInt32(dr["id_autorizacao"]) : 0;
            vo.InicioPlano = dr["dt_inicio"] != DBNull.Value ? Convert.ToDateTime(dr["dt_inicio"]) : DateTime.MinValue;

            if (!string.IsNullOrEmpty(dr.Field<string>("tp_plano")))
                vo.Plano = PDados.Produto.Find(dr.Field<string>("tp_plano"));

            vo.Empresa = (PDados.Empresa)Enum.Parse(typeof(PDados.Empresa), dr.Field<string>("id_empresa"));
            vo.ObsValidacao = dr.Field<string>("ds_validacao");

            vo.InicioPlanoIntegracao = BaseDAO.GetNullableDate(dr, "dt_inicio_integracao");
            vo.IdUsuarioIntegracao = BaseDAO.GetNullableInt(dr, "id_usuario_integracao");
            vo.DataIntegracao = BaseDAO.GetNullableDate(dr, "dt_integracao");
            vo.CdCategoria = BaseDAO.GetNullableInt(dr, "cd_categoria_integracao");
            vo.PlanoIntegracao = Convert.ToString(dr["cd_plano_integracao"]);
            vo.CdMotivoDesligamentoFamilia = dr["BG3_CODBLO"] != DBNull.Value ? Convert.ToString(dr["BG3_CODBLO"]).Trim() : "";
            vo.CdMotivoDesligamentoUsuario = dr["BG1_CODBLO"] != DBNull.Value ? Convert.ToString(dr["BG1_CODBLO"]).Trim() : "";
            vo.CarenciaIntegracao = dr.Field<string>("TP_CARENCIA_INTEGRACAO");

            return vo;
        }

        internal static List<PDeclaracaoDependenteVO> FindDependentesByNum(int numProposta, EvidaDatabase evdb)
        {
            string sql = "SELECT id_declaracao, dt_criacao, ds_cns, ds_cpf, dt_nascimento, ds_nome, ds_nome_mae, ds_nome_pai, ds_rg, BA1_SEXO, id_parentesco, DS_ORGAO_EXPEDIDOR, DS_UF_ORGAO_EXPEDIDOR, " +
                " dt_emissao_rg, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG " +
            " FROM DECLARACAO_DEPENDENTE d WHERE ID_DECLARACAO IN (SELECT ID_DECLARACAO FROM DECLARACAO WHERE NU_DECLARACAO = :id) ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = numProposta });

            List<PDeclaracaoDependenteVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRowDep, lstP);
            return lst;
        }

        private static PDeclaracaoDependenteVO FromDataRowDep(DataRow dr)
        {
            //id_declaracao, dt_criacao, ds_cns, ds_cpf, dt_nascimento, ds_nome, ds_nome_mae, ds_nome_pai, ds_rg, ds_sexo,
            //id_parentesco, DS_ORGAO_EXPEDIDOR, DS_UF_ORGAO_EXPEDIDOR
            PDeclaracaoDependenteVO vo = new PDeclaracaoDependenteVO();

            vo.Nome = Convert.ToString(dr["ds_nome"]);
            vo.Nascimento = Convert.ToDateTime(dr["dt_nascimento"]);
            vo.Cns = Convert.ToString(dr["ds_cns"]);
            vo.Cpf = Convert.ToString(dr["ds_cpf"]);
            vo.NomeMae = Convert.ToString(dr["ds_nome_mae"]);
            vo.NomePai = Convert.ToString(dr["ds_nome_pai"]);
            vo.Rg = Convert.ToString(dr["ds_rg"]);
            vo.Sexo = (PDados.Sexo)Convert.ToChar(dr["BA1_SEXO"]);
            vo.Parentesco = PDados.DeclaracaoParentesco.Find(Convert.ToInt32(dr["id_parentesco"]));
            vo.OrgaoExpedidor = Convert.ToString(dr["DS_ORGAO_EXPEDIDOR"]);
            vo.UfOrgaoExpedidor = Convert.ToString(dr["DS_UF_ORGAO_EXPEDIDOR"]);
            vo.DataEmissaoRg = dr["dt_emissao_rg"] != DBNull.Value ? Convert.ToDateTime(dr["dt_emissao_rg"]) : new DateTime?();
            vo.Codint = (dr["BA1_CODINT"] != DBNull.Value) ? Convert.ToString(dr["BA1_CODINT"]).Trim() : "";
            vo.Codemp = (dr["BA1_CODEMP"] != DBNull.Value) ? Convert.ToString(dr["BA1_CODEMP"]).Trim() : "";
            vo.Matric = (dr["BA1_MATRIC"] != DBNull.Value) ? Convert.ToString(dr["BA1_MATRIC"]).Trim() : "";
            vo.Tipreg = (dr["BA1_TIPREG"] != DBNull.Value) ? Convert.ToString(dr["BA1_TIPREG"]).Trim() : "";

            if (!string.IsNullOrEmpty(vo.Cpf) && (vo.Cpf.Equals("0") || vo.Cpf.Equals("00000000000")))
            {
                vo.Cpf = string.Empty;
            }

            return vo;
        }

        internal static void MarcarValidada(int id, bool isValido, string motivo, EvidaDatabase evdb)
        {
            String sql = "UPDATE DECLARACAO SET ID_STATUS = :status, DS_VALIDACAO = :motivo, DT_VALIDADA = LOCALTIMESTAMP WHERE NU_DECLARACAO = :id";
            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":motivo", DbType.AnsiString, motivo);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(isValido ? PDados.SituacaoDeclaracao.VALIDADA : PDados.SituacaoDeclaracao.INVALIDA));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void MarcarIntegrada(int id, int idUsuario, string lotacao, PIntegracaoAdesaoVO integracaoVO, EvidaDatabase evdb)
        {
            String sql = "UPDATE DECLARACAO SET DT_INTEGRACAO = LOCALTIMESTAMP, ID_USUARIO_INTEGRACAO = :usuario, DT_INICIO_INTEGRACAO = :inicio, ID_STATUS = :status, " +
                " BG3_CODBLO = :cdMotivoDesligamentoFamilia, BG1_CODBLO = :cdMotivoDesligamentoUsuario, CD_PLANO_INTEGRACAO = :cdPlano, CD_LOTACAO_INTEGRACAO = :cdLotacao, TP_CARENCIA_INTEGRACAO = :tpCarencia, " +
                " BA3_CODEMP_INTEGRACAO = :codemp, BA3_CONEMP_INTEGRACAO = :conemp, BA3_SUBCON_INTEGRACAO =:subcon " +
                "	WHERE NU_DECLARACAO = :id";

            // Quebra a categoria
            String codemp = "";
            String conemp = "";
            String subcon = "";

            if(!String.IsNullOrEmpty(integracaoVO.CdCategoria)){

                String[] substrings = integracaoVO.CdCategoria.Split('-');
                codemp = substrings[0];
                conemp = substrings[1];
                subcon = substrings[2];
            }

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":usuario", DbType.Int32, idUsuario);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)PDados.SituacaoDeclaracao.INTEGRADA);
            db.AddInParameter(dbCommand, ":inicio", DbType.Date, integracaoVO.InicioVigencia);
            //db.AddInParameter(dbCommand, ":cdCategoria", DbType.Int32, integracaoVO.CdCategoria);  -- a categoria foi substituída por codem, conemp e subcon
            db.AddInParameter(dbCommand, ":codemp", DbType.String, codemp);
            db.AddInParameter(dbCommand, ":conemp", DbType.String, conemp);
            db.AddInParameter(dbCommand, ":subcon", DbType.String, subcon);
            db.AddInParameter(dbCommand, ":cdPlano", DbType.String, integracaoVO.CdPlanoVinculado);
            db.AddInParameter(dbCommand, ":cdMotivoDesligamentoFamilia", DbType.String, integracaoVO.CdMotivoDesligamentoFamilia);
            db.AddInParameter(dbCommand, ":cdMotivoDesligamentoUsuario", DbType.String, integracaoVO.CdMotivoDesligamentoUsuario);
            db.AddInParameter(dbCommand, ":cdLotacao", DbType.String, lotacao);
            db.AddInParameter(dbCommand, ":tpCarencia", DbType.String, integracaoVO.TpCarencia);

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

    }
}
