using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Adesao;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Adesao {
	internal static class DeclaracaoDAO {

		internal static DataTable BuscarResumo(EvidaDatabase evdb) {
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

		internal static DataTable Pesquisar(Dados.Empresa? empresa, int? numero, long? matricula, Dados.SituacaoDeclaracao? status, EvidaDatabase evdb) {
			Database db = evdb.Database;

			string sql = "SELECT D.ID_EMPRESA, D.ID_PRODUTO, ID_DECLARACAO ID_BYTE, ID_STATUS, NU_DECLARACAO, " +
				" DS_NOME NOME_TITULAR, ID_MATRICULA, DT_CRIACAO, DT_RECEBIDA, DT_VALIDADA, DS_VALIDACAO, ID_STATUS, DT_INTEGRACAO " +
				"	FROM DECLARACAO D " +
				"	WHERE 1 = 1 ";
			List<Parametro> lstParams = new List<Parametro>();
			if (numero != null) {
				sql += " AND NU_DECLARACAO = :numero ";
				lstParams.Add(new Parametro(":numero", DbType.Int32, numero));
			}
			if (matricula != null) {
				sql += " AND ID_MATRICULA = :matricula ";
				lstParams.Add(new Parametro(":matricula", DbType.Int64, matricula));
			}
			if (empresa != null) {
				sql += " AND ID_EMPRESA = :empresa ";
				lstParams.Add(new Parametro(":empresa", DbType.String, empresa.ToString()));
			}
			if (status != null) {
				sql += " AND ID_STATUS = :status";
				lstParams.Add(new Parametro(":status", DbType.Int32, (int)status.Value));
			}

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			TraduzirProduto(dt);
			TraduzirIdDeclaracao(dt);

			return dt;
		}

		private static void TraduzirProduto(DataTable dt) {
			if (dt.Columns.Contains("ID_PRODUTO") && !dt.Columns.Contains("PRODUTO")) {
				dt.Columns.Add("PRODUTO", typeof(string));
				foreach (DataRow dr in dt.Rows) {
					Dados.Produto p = Dados.Produto.Find(Convert.ToString(dr["ID_PRODUTO"]));
					dr["PRODUTO"] = p != null ? p.Descricao : Convert.ToString(dr["ID_PRODUTO"]);
				}
			}
		}

		private static void TraduzirEmpresa(DataTable dt) {
			if (dt.Columns.Contains("ID_EMPRESA") && !dt.Columns.Contains("EMPRESA")) {
				dt.Columns.Add("EMPRESA", typeof(string));
				foreach (DataRow dr in dt.Rows) {
					Dados.Empresa p = (Dados.Empresa)Enum.Parse(typeof(Dados.Empresa), dr.Field<string>("id_empresa"));
					dr["EMPRESA"] = Dados.EnumTradutor.TraduzEmpresa(p);
				}
			}
		}

		private static void TraduzirIdDeclaracao(DataTable dt) {
			if (dt.Columns.Contains("ID_BYTE") && !dt.Columns.Contains("ID_DECLARACAO")) {
				dt.Columns.Add("ID_DECLARACAO", typeof(Guid));
				foreach (DataRow dr in dt.Rows) {
					byte[] idByte = (byte[])dr["ID_BYTE"];
					dr["ID_DECLARACAO"] = new Guid(idByte);
				}
			}
		}

		internal static DeclaracaoVO GetById(int numProposta, EvidaDatabase evdb) {
			string sql = "SELECT id_declaracao, nu_declaracao, dt_criacao, id_produto, ds_nome, id_matricula, dt_nascimento, ds_cns, ds_cpf, ds_nome_mae, ds_nome_pai, ds_rg, ds_sexo, " +
			"	dt_admissao, ds_email, id_estado_civil, ds_lotacao, ds_orgao_expedidor, ds_uf_orgao_expedidor, dt_emissao_rg, ds_tel_celular, ds_tel_residencial, ds_tel_comercial, " +
			"	ds_bairro, ds_cidade, ds_complemento, ds_numero, ds_rua, ds_uf, ds_cep, id_autorizacao, dt_inicio, " +
			"	d.id_banco, ds_agencia, ds_conta_corrente, ds_dv_agencia, ds_dv_conta_corrente, nu_dia_pagamento, ds_local, " +
			"	ds_nome_requerente, id_parentesco, vl_inss, vl_previnorte, vl_outras_fontes, b.no_banco, " +
			"	ds_nome_resp_financ, ds_cpf_resp_financ, dt_nascimento_resp_financ, ds_sexo_resp_financ, ds_tel_resp_financ, ds_email_resp_financ, " +
			"	tp_plano, id_empresa, dt_validada, ds_validacao, dt_inicio_integracao, dt_integracao, id_usuario_integracao, cd_categoria_integracao, " +
			"	cd_motivo_desligamento, cd_plano_integracao, tp_carencia_integracao " +
			" FROM DECLARACAO d, BANCO b WHERE nu_declaracao = :id AND d.id_banco = b.id_banco(+) ";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = numProposta });

			List<DeclaracaoVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRow, lstP);

			if (lst != null && lst.Count > 0) {
				DeclaracaoVO vo = lst[0];

				vo.Dependentes = FindDependentesByNum(numProposta, evdb);
				return vo;
			}
			return null;
		}

		internal static List<DeclaracaoDependenteVO> FindDependentesByNum(int numProposta, EvidaDatabase evdb) {
			string sql = "SELECT id_declaracao, dt_criacao, ds_cns, ds_cpf, dt_nascimento, ds_nome, ds_nome_mae, ds_nome_pai, ds_rg, ds_sexo, id_parentesco, DS_ORGAO_EXPEDIDOR, DS_UF_ORGAO_EXPEDIDOR, " +
				" dt_emissao_rg, CD_DEPENDENTE " +
			" FROM DECLARACAO_DEPENDENTE d WHERE ID_DECLARACAO IN (SELECT ID_DECLARACAO FROM DECLARACAO WHERE NU_DECLARACAO = :id) ";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = numProposta });

			List<DeclaracaoDependenteVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRowDep, lstP);
			return lst;
		}

		private static DeclaracaoDependenteVO FromDataRowDep(DataRow dr) {
			//id_declaracao, dt_criacao, ds_cns, ds_cpf, dt_nascimento, ds_nome, ds_nome_mae, ds_nome_pai, ds_rg, ds_sexo,
			//id_parentesco, DS_ORGAO_EXPEDIDOR, DS_UF_ORGAO_EXPEDIDOR
			DeclaracaoDependenteVO vo = new DeclaracaoDependenteVO();

			vo.Nome = Convert.ToString(dr["ds_nome"]);
			vo.Nascimento = Convert.ToDateTime(dr["dt_nascimento"]);
			vo.Cns = Convert.ToString(dr["ds_cns"]);
			vo.Cpf = Convert.ToString(dr["ds_cpf"]);
			vo.NomeMae = Convert.ToString(dr["ds_nome_mae"]);
			vo.NomePai = Convert.ToString(dr["ds_nome_pai"]);
			vo.Rg = Convert.ToString(dr["ds_rg"]);
			vo.Sexo = (Dados.Sexo)Convert.ToChar(dr["ds_sexo"]);
			vo.Parentesco = Dados.DeclaracaoParentesco.Find(Convert.ToInt32(dr["id_parentesco"]));
			vo.OrgaoExpedidor = Convert.ToString(dr["DS_ORGAO_EXPEDIDOR"]);
			vo.UfOrgaoExpedidor = Convert.ToString(dr["DS_UF_ORGAO_EXPEDIDOR"]);
			vo.DataEmissaoRg = dr["dt_emissao_rg"] != DBNull.Value ? Convert.ToDateTime(dr["dt_emissao_rg"]) : new DateTime?();
			vo.CodDependente = !dr.IsNull("CD_DEPENDENTE") ? Convert.ToInt32(dr["CD_DEPENDENTE"]) : new int?();

			if (!string.IsNullOrEmpty(vo.Cpf) && (vo.Cpf.Equals("0") || vo.Cpf.Equals("00000000000"))) {
				vo.Cpf = string.Empty;
			}

			return vo;
		}

		private static DeclaracaoVO FromDataRow(DataRow dr) {
			DeclaracaoVO vo = new DeclaracaoVO();

			vo.Id = new Guid((byte[])(dr["id_declaracao"]));
			vo.Numero = Convert.ToInt32(dr["nu_declaracao"]);
			vo.Criacao = Convert.ToDateTime(dr["dt_criacao"]);
			vo.Produto = Convert.ToString(dr["id_produto"]).Trim();
			vo.Titular = new PessoaVO();
			vo.Titular.Nome = Convert.ToString(dr["ds_nome"]);
			vo.Titular.Matricula = Convert.ToInt64(dr["id_matricula"]);
			vo.Titular.Nascimento = Convert.ToDateTime(dr["dt_nascimento"]);
			vo.Titular.Cns = Convert.ToString(dr["ds_cns"]);
			vo.Titular.Cpf = Convert.ToString(dr["ds_cpf"]);
			vo.Titular.NomeMae = Convert.ToString(dr["ds_nome_mae"]);
			vo.Titular.NomePai = Convert.ToString(dr["ds_nome_pai"]);
			vo.Titular.Rg = Convert.ToString(dr["ds_rg"]);
			vo.Titular.Sexo = (Dados.Sexo)Convert.ToChar(dr["ds_sexo"]);
			vo.Titular.Admissao = dr["dt_admissao"] != DBNull.Value ? Convert.ToDateTime(dr["dt_admissao"]) : new DateTime?();
			vo.Email = Convert.ToString(dr["ds_email"]);
			vo.Titular.Email = vo.Email;
			vo.Titular.EstadoCivil = Dados.DeclaracaoEstadoCivil.Find(Convert.ToInt32(dr["id_estado_civil"]));
			vo.Titular.Lotacao = Convert.ToString(dr["ds_lotacao"]);
			vo.Titular.OrgaoExpedidor = Convert.ToString(dr["ds_orgao_expedidor"]);
			vo.Titular.UfOrgaoExpedidor = Convert.ToString(dr["ds_uf_orgao_expedidor"]);
			vo.Titular.DataEmissaoRg = dr["dt_emissao_rg"] != DBNull.Value ? Convert.ToDateTime(dr["dt_emissao_rg"]) : new DateTime?();
			vo.TelCelular = Convert.ToString(dr["ds_tel_celular"]);
			vo.TelResidencial = Convert.ToString(dr["ds_tel_residencial"]);
			vo.TelComercial = Convert.ToString(dr["ds_tel_comercial"]);

			vo.Endereco = new AdesaoEnderecoVO {
				Bairro = Convert.ToString(dr["ds_bairro"]),
				Cidade = Convert.ToString(dr["ds_cidade"]),
				Complemento = Convert.ToString(dr["ds_complemento"]),
				NumeroEndereco = Convert.ToString(dr["ds_numero"]),
				Rua = Convert.ToString(dr["ds_rua"]),
				Uf = Convert.ToString(dr["ds_uf"]),
				Cep = Convert.ToString(dr["ds_cep"])
			};

			vo.DadosBancarios = new DadosBancariosVO {
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

			//"	ds_nome_resp_financ, ds_cpf_resp_financ, dt_nascimento_resp_financ, ds_sexo_resp_financ, ds_tel_resp_financ, ds_email_resp_financ " +
			vo.ResponsavelFinanceiro = new ResponsavelFinanceiroVO {
				Nome = Convert.ToString(dr["ds_nome_resp_financ"]),
				Cpf = Convert.ToString(dr["ds_cpf_resp_financ"]),
				Nascimento = dr["dt_nascimento_resp_financ"] != DBNull.Value ? Convert.ToDateTime(dr["dt_nascimento_resp_financ"]) : DateTime.MinValue,
				Sexo = dr["ds_sexo_resp_financ"] != DBNull.Value ? (Dados.Sexo)Convert.ToChar(dr["ds_sexo_resp_financ"]) : Dados.Sexo.INDEFINIDO,
				Telefone = Convert.ToString(dr["ds_tel_resp_financ"]),
				Email = Convert.ToString(dr["ds_email_resp_financ"])
			};
			if (string.IsNullOrEmpty(vo.ResponsavelFinanceiro.Nome))
				vo.ResponsavelFinanceiro = null;

			vo.OpcaoAutorizacao = dr["id_autorizacao"] != DBNull.Value ? Convert.ToInt32(dr["id_autorizacao"]) : 0;
			vo.InicioPlano = dr["dt_inicio"] != DBNull.Value ? Convert.ToDateTime(dr["dt_inicio"]) : DateTime.MinValue;

			if (!string.IsNullOrEmpty(dr.Field<string>("tp_plano")))
				vo.Plano = Dados.Produto.Find(dr.Field<string>("tp_plano"));

			vo.Empresa = (Dados.Empresa)Enum.Parse(typeof(Dados.Empresa), dr.Field<string>("id_empresa"));
			vo.ObsValidacao = dr.Field<string>("ds_validacao");

			vo.InicioPlanoIntegracao = BaseDAO.GetNullableDate(dr, "dt_inicio_integracao");
			vo.IdUsuarioIntegracao = BaseDAO.GetNullableInt(dr, "id_usuario_integracao");
			vo.DataIntegracao = BaseDAO.GetNullableDate(dr, "dt_integracao");
			vo.CdCategoria = BaseDAO.GetNullableInt(dr, "cd_categoria_integracao");
			vo.PlanoIntegracao = Convert.ToString(dr["cd_plano_integracao"]);
			vo.CdMotivoDesligamento = BaseDAO.GetNullableInt(dr,"cd_motivo_desligamento");
			vo.CarenciaIntegracao = dr.Field<string>("TP_CARENCIA_INTEGRACAO");

			return vo;
		}

		internal static void MarcarRecebida(int id, DbTransaction transaction, Database db) {
			String sql = "UPDATE DECLARACAO SET ID_STATUS = :status, DT_RECEBIDA = LOCALTIMESTAMP WHERE NU_DECLARACAO = :id";
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)Dados.SituacaoDeclaracao.RECEBIDA);

			BaseDAO.ExecuteNonQuery(dbCommand, transaction, db);
		}

		internal static void MarcarValidada(int id, bool isValido, string motivo, EvidaDatabase evdb) {
			String sql = "UPDATE DECLARACAO SET ID_STATUS = :status, DS_VALIDACAO = :motivo, DT_VALIDADA = LOCALTIMESTAMP WHERE NU_DECLARACAO = :id";
			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":motivo", DbType.AnsiString, motivo);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(isValido ? Dados.SituacaoDeclaracao.VALIDADA : Dados.SituacaoDeclaracao.INVALIDA));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void MarcarIntegrada(int id, int idUsuario, string lotacao, VO.HC.HcBeneficiarioCategoriaVO categoriaVO, VO.HC.HcBeneficiarioPlanoVO planoVO, EvidaDatabase evdb) {
			String sql = "UPDATE DECLARACAO SET DT_INTEGRACAO = LOCALTIMESTAMP, ID_USUARIO_INTEGRACAO = :usuario, DT_INICIO_INTEGRACAO = :inicio, ID_STATUS = :status, " +
				" CD_CATEGORIA_INTEGRACAO = :cdCategoria, CD_MOTIVO_DESLIGAMENTO = :cdMotivoDesligamento, CD_PLANO_INTEGRACAO = :cdPlano, " +
				" CD_LOTACAO_INTEGRACAO = :cdLotacao, TP_CARENCIA_INTEGRACAO = :tpCarencia " +
				"	WHERE NU_DECLARACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, idUsuario);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)Dados.SituacaoDeclaracao.INTEGRADA);
			db.AddInParameter(dbCommand, ":inicio", DbType.Date, categoriaVO.InicioVigencia);
			db.AddInParameter(dbCommand, ":cdCategoria", DbType.Int32, categoriaVO.CdCategoria);
			db.AddInParameter(dbCommand, ":cdPlano", DbType.String, planoVO.CdPlanoVinculado);
			db.AddInParameter(dbCommand, ":cdMotivoDesligamento", DbType.Int32, planoVO.CdMotivoDesligamento);
			db.AddInParameter(dbCommand, ":cdLotacao", DbType.String, lotacao);
			db.AddInParameter(dbCommand, ":tpCarencia", DbType.String, planoVO.TpCarencia);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
