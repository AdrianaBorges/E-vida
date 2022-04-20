using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class RelatorioDAO {
		static EVidaLog log = new EVidaLog(typeof(RelatorioDAO));

		internal static DataTable RelatorioProvisao(bool agrupado, DateTime dtInicio, DateTime dtFim, Database db) {
			string xmlAgrupado = "Provisao_AGRUPADO.xml";
			string xmlDetalhado = "Provisao_DETALHADO.xml";

			ReportQueryUtil query = new ReportQueryUtil(agrupado ? xmlAgrupado : xmlDetalhado);

			List<Parametro> lstParams = new List<Parametro>();

			lstParams.AddRange(query.AddParameter(dtInicio, dtFim, DbType.Date, "data"));

			string sql = query.BuildFinalQuery();

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable RelatorioPagamento(bool agrupado, DateTime dtInicio, DateTime dtFim, string tipoPessoaCred, List<int> lstRegional, Database db) {
			string xmlAgrupado = "Pagamento_AGRUPADO.xml";
			string xmlDetalhado = "Pagamento_DETALHADO.xml";

			ReportQueryUtil query = new ReportQueryUtil(agrupado ? xmlAgrupado : xmlDetalhado);

			query.AddParameter(dtInicio, dtFim, DbType.Date, "data");

			if (!string.IsNullOrEmpty(tipoPessoaCred)) {
				query.AddParameter(tipoPessoaCred, DbType.String, "tipoPessoa");
			}
			if (lstRegional != null && lstRegional.Count > 0) {
				query.AddParameter(lstRegional, DbType.Int32, "regional");
			}


			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable RelatorioContabilizacao(string tipoSistemaAtendimento, string statusAtendimento, List<int> lstEmpresa, List<int> lstPlanos, List<int> lstCategoria, DateTime dtInicio, DateTime dtFim, Database db) {
			ReportQueryUtil query = new ReportQueryUtil("Contabilizacao.xml");

			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(statusAtendimento))
				lstParams.Add(query.AddParameter(statusAtendimento, DbType.String, "status"));
			if (!string.IsNullOrEmpty(tipoSistemaAtendimento))
				lstParams.Add(query.AddParameter(tipoSistemaAtendimento, DbType.String, "sistema"));
			
			lstParams.AddRange(query.AddParameter(dtInicio, dtFim, DbType.Date, "data"));


			if (lstEmpresa != null && lstEmpresa.Count > 0) {
				lstParams.AddRange(query.AddParameter(lstEmpresa, DbType.Int32, "empresa"));

			}
			if (lstPlanos != null && lstPlanos.Count > 0) {
				lstParams.AddRange(query.AddParameter(lstPlanos, DbType.Int32, "plano"));
			}

			if (lstCategoria != null && lstCategoria.Count > 0) {
				lstParams.AddRange(query.AddParameter(lstCategoria, DbType.Int32, "categoria"));
			}

			string sql = query.BuildFinalQuery();

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable ListarUserUpdateAutorizacao(Database db) {
			string sql = "SELECT DISTINCT A.USER_UPDATE FROM ISA_HC.HC_AUTORIZACAO A ORDER BY A.USER_UPDATE ";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);

			return dt;
		}

        // Para migrar esta consulta abaixo, é necessário identificar onde estão as autorizações no Protheus
		internal static DataTable RelatorioAutorizacoes(DateTime dtInicio, DateTime dtFim, long? cdMatricula, string titular, string dependente,
			int? nroAutorizacaoIsa, int? nroAutorizacaoWeb, string tipo, string status, string sistemaAtendimento, string cdMascara, string dsServico,
			long? cpf, string nmCredenciado, string userUpdate, Database db) {

                string sql = "SELECT b.BA1_MATEMP, t.BA1_NOMUSR as nm_titular, b.BA1_NOMUSR as nm_beneficiario, a.nr_autorizacao, a.nr_autorizacao_web, a.dt_registro, trunc(A.dt_autorizacao) dt_autorizacao, " +
                " DECODE (a.tp_autorizacao, 'ODONTO', 'ATENDIMENTO ODONTOLÓGICO', 'INTERN', 'INTERNAÇÃO HOSPITALAR', 'PROC', 'PROCEDIMENTOS ESPECIALIZADOS', 'CART', 'CARTA COMPROMISSO', 'PSICO', 'PSICOLOGIA') tp_autorizacao, " +
                " DECODE (a.st_autorizacao, 'A', 'APROVADA', 'C', 'CANCELADA', 'E', 'EM PERICIA', 'P', 'PENDENTE', 'R', 'REPROVADA', 'U', 'UTILIZADA') st_autorizacao, " +
                " DECODE (a.tp_sistema_atend, 'CRED', 'CREDENCIAMENTO', 'REEMB', 'REEMBOLSO') tp_sistema_atend, " +
                " a.nr_dias_autorizados, a.ds_motivo_cancelamento, A.ds_compl_reprovacao, a.dt_inicio_autorizacao, a.dt_termino_autorizacao, " +
                " a.ds_observacao, h.cd_mascara, UPPER(h.ds_servico) ds_servico, g.qt_solicitada, g.qt_autorizada, c.BAU_NOME as nm_razao_social, " +
                " evida.f_mascara_cnpj (trim(c.BAU_CPFCGC), c.tp_pessoa) nr_cnpj_cpf, " +
                " e.BID_DESCRI, c.BAU_EST, a.nm_medico_solicitante, a.BB0_NUMCR, a.BB0_CODSIG, a.BB0_ESTADO, a.user_update, g.dt_validade " +
                "	FROM   isa_hc.hc_autorizacao a, VW_PR_USUARIO b, VW_PR_USUARIO t, VW_PR_REDE_ATENDIMENTO c, " +
                "		VW_PR_MUNICIPIO e, isa_hc.hc_item_autorizacao g, isa_hc.hc_servico h " +
                "	WHERE	trim(a.BA1_CODINT) = trim(b.BA1_CODINT) AND trim(a.BA1_CODEMP) = trim(b.BA1_CODEMP) AND trim(a.BA1_MATRIC) = trim(b.BA1_MATRIC) AND trim(a.BA1_TIPREG) = trim(b.BA1_TIPREG) " +
                "		AND trim(b.BA1_CODINT) = trim(t.BA1_CODINT) AND trim(b.BA1_CODEMP) = trim(t.BA1_CODEMP) AND trim(b.BA1_MATRIC) = trim(t.BA1_MATRIC) AND trim(t.BA1_TIPUSU) = 'T' " +
                "		AND trim(a.BAU_CODIGO) = trim(c.BAU_CODIGO) " +
                "		AND trim(c.BAU_MUN) = trim(e.BID_CODMUN) " +
                "		AND a.nr_autorizacao = g.nr_autorizacao " +
                "		and g.cd_servico = h.cd_servico " +
                "		AND a.dt_registro BETWEEN :dtInicial AND :dtFinal ";

            string sqlOrderBy = " ORDER BY A.dt_registro";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":dtInicial", Tipo = DbType.Date, Value = dtInicio });
            lstParams.Add(new Parametro() { Name = ":dtFinal", Tipo = DbType.Date, Value = dtFim });

            if (cdMatricula != null && cdMatricula.HasValue)
            {
                sql += " AND trim(b.BA1_MATEMP) = trim(:cdMatricula) ";
                lstParams.Add(new Parametro(":cdMatricula", DbType.String, cdMatricula.Value));
            }
            if (!string.IsNullOrEmpty(titular))
            {
                sql += " AND trim(t.BA1_NOMUSR) = trim(:titular) ";
                lstParams.Add(new Parametro(":titular", DbType.String, titular));
            }
            if (!string.IsNullOrEmpty(dependente))
            {
                sql += " AND trim(b.BA1_NOMUSR) = trim(:dependente) ";
                lstParams.Add(new Parametro(":dependente", DbType.String, dependente));
            }
            if (nroAutorizacaoIsa != null && nroAutorizacaoIsa.HasValue)
            {
                sql += " AND A.NR_AUTORIZACAO = :nroAutorizacaoIsa ";
                lstParams.Add(new Parametro(":nroAutorizacaoIsa", DbType.Int32, nroAutorizacaoIsa.Value));
            }
            if (nroAutorizacaoWeb != null && nroAutorizacaoWeb.HasValue)
            {
                sql += " AND A.NR_AUTORIZACAO_WEB = :nroAutorizacaoWeb ";
                lstParams.Add(new Parametro(":nroAutorizacaoWeb", DbType.Int32, nroAutorizacaoWeb.Value));
            }
            if (!string.IsNullOrEmpty(tipo))
            {
                sql += " AND A.TP_AUTORIZACAO = :tipo ";
                lstParams.Add(new Parametro(":tipo", DbType.String, tipo));
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND A.ST_AUTORIZACAO = :status ";
                lstParams.Add(new Parametro(":status", DbType.String, status));
            }
            if (!string.IsNullOrEmpty(sistemaAtendimento))
            {
                sql += " AND A.TP_SISTEMA_ATEND = :sistemaAtendimento ";
                lstParams.Add(new Parametro(":sistemaAtendimento", DbType.String, sistemaAtendimento));
            }
            if (!string.IsNullOrEmpty(cdMascara))
            {
                sql += " AND H.CD_MASCARA = :cdMascara ";
                lstParams.Add(new Parametro(":cdMascara", DbType.String, cdMascara));
            }
            if (!string.IsNullOrEmpty(dsServico))
            {
                sql += " AND H.DS_SERVICO = :dsServico ";
                lstParams.Add(new Parametro(":dsServico", DbType.String, dsServico));
            }
            if (cpf != null && cpf.HasValue)
            {
                sql += " AND trim(c.BAU_CPFCGC) = trim(:cpf) ";
                lstParams.Add(new Parametro(":cpf", DbType.String, cpf.Value));
            }
            if (!string.IsNullOrEmpty(nmCredenciado))
            {
                sql += " AND trim(C.BAU_NOME) = trim(:nmCredenciado) ";
                lstParams.Add(new Parametro(":nmCredenciado", DbType.String, nmCredenciado));
            }
            if (!string.IsNullOrEmpty(userUpdate))
            {
                sql += " AND A.USER_UPDATE = :userUpdate ";
                lstParams.Add(new Parametro(":userUpdate", DbType.String, userUpdate));
            }
            sql += sqlOrderBy;	

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}
		
		internal static DataTable RelatorioFaturamentoUsuario(bool byItem, DateTime dtInicio, DateTime dtFim, List<string> lstUsuarios, Database db) {
			ReportQueryUtil query = new ReportQueryUtil("FaturamentoUsuario_" + (byItem ? "ITEM" : "GUIA") + ".xml");

			query.AddParameter(dtInicio, dtFim, DbType.Date, "data");
			if (lstUsuarios != null && lstUsuarios.Count > 0)
				query.AddParameter(lstUsuarios, DbType.String, "user");

			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable RelatorioFaturamentoUsuario2(bool byItem, DateTime dtInicio, DateTime dtFim, List<string> lstUsuarios, Database db) {
			string sql = "SELECT   " +
				(byItem ? " a.nr_protocolo, a.nr_atendimento, " : "0 nr_protocolo, 0 nr_atendimento, ") + " count(1) as qtd_item, " +
				"		DECODE(a.tp_sistema_atend, 'CRED', 'FATURA', 'REEMB', 'REEMBOLSO') tp_sistema_atend, " +
				"		a.user_update, UPPER(c.nm_usuario) nm_usuario, to_date(a.date_update, 'dd/MM/yyyy hh24:mi') date_update, " +
				"		DECODE (a.tp_origem, 'DIG', 'DIGITADO', 'WEB', 'REEMBOLSO', 'XML', 'FATURAMENTO ELETRONICO') tp_origem " +
				"	FROM   isa_hc.hc_atendimento a, isa_scl.scl_usuario c " +
				(byItem ? ", isa_hc.hc_item_atendimento ia " : "") +
				"	WHERE	a.user_update = c.cd_usuario " +
				(byItem ? " AND ia.cd_organizacao = a.cd_organizacao and ia.nr_atendimento = a.nr_atendimento " : "") +
				"			AND TO_DATE(SUBSTR (a.date_update, 1, 10)) BETWEEN :dataInicial AND :dataFinal ";

			string sqlGrpOrd = " GROUP BY   a.tp_sistema_atend, a.user_update, a.tp_origem, c.nm_usuario, a.date_update " +
				(byItem ? ", a.nr_protocolo, a.nr_atendimento " : "") +  
				" ORDER BY   c.nm_usuario, to_date(a.date_update, 'dd/MM/yyyy hh24:mi') ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":dataInicial", Tipo = DbType.Date, Value = dtInicio });
			lstParams.Add(new Parametro() { Name = ":dataFinal", Tipo = DbType.Date, Value = dtFim });

			if (lstUsuarios != null && lstUsuarios.Count > 0) {
				sql += " AND A.USER_UPDATE IN (";
				for (int i = 0; i < lstUsuarios.Count; i++) {
					if (i > 0)
						sql += ", ";
					sql += ":user" + i;
					lstParams.Add(new Parametro(":user" + i, DbType.String, lstUsuarios[i]));
				}
				sql += ")";
			}

			sql += sqlGrpOrd;

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}
		
		internal static DataTable BuscarUserUpdateAtendimento(List<string> ids, Database db) {
			string sql = "SELECT U.CD_USUARIO, U.NM_USUARIO FROM SCL_USUARIO U " +
				"	WHERE 1 = 1";

			List<Parametro> lstParams = new List<Parametro>();

			if (ids.Count == 1) {
                sql += "		AND upper(trim(U.cd_USUARIO)) LIKE upper(trim(:id)) ";
				lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.String, Value = "%" + ids[0].ToUpper() + "%" });
			} else {
				sql += "		AND U.cd_USUARIO IN (' '";
				int count = 1;
				foreach (string id in ids) {
					sql += ", :id" + count;
					lstParams.Add(new Parametro() { Name = ":id" + count, Tipo = DbType.String, Value = id.ToUpper()});
					count++;
				}
				sql += ")";
			}

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarMensalidade(DateTime dataReferencia, List<string> lstGrupos, List<int> lstCategoria, Database db) {
			ReportQueryUtil query = new ReportQueryUtil("Mensalidade.xml");

			query.AddParameter(dataReferencia, DbType.Date, "dataRef");
			query.AddParameter(lstGrupos, DbType.String, "grupo");
			query.AddParameter(lstCategoria, DbType.String, "categoria");

			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable RelatorioCoparticipacao(DateTime dataInicio, DateTime dataFim, List<string> lstSituacao, string cartaoTitular, bool parcela, EvidaDatabase db) {
			string xml = parcela ? "CoParticipacaoParcela.xml" : "Coparticipacao.xml";

			ReportQueryUtil query = new ReportQueryUtil(xml);

			query.AddParameter(dataInicio, dataFim, DbType.Date, "dataRef");
			query.AddParameter(lstSituacao, DbType.String, "situacao");

			if (!string.IsNullOrEmpty(cartaoTitular)) {
				query.AddParameter(cartaoTitular, DbType.String, "cartaoTitular");
			}

			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarBoletosVencimento(DateTime dataReferencia, Database db) {
			string xml = "BoletosVencimento.xml";

			ReportQueryUtil query = new ReportQueryUtil(xml);

			query.AddParameter(dataReferencia, DbType.Date, "dataRef");

			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarBoletosPendentes(Database db) {
			string xml = "BoletosPendentes.xml";

			ReportQueryUtil query = new ReportQueryUtil(xml);
			
			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

        internal static DataTable BuscarBeneficiariosPorLocal(string tipoRelatorio, List<string> lstPlanos, List<string> lstUf, List<string> lstRegional, List<string> lstGrauParentesco, bool? deficiente, bool? estudante, Database db)
        {
			ReportQueryUtil query = new ReportQueryUtil(tipoRelatorio.Equals("A") ? "BeneficiariosPorLocal.xml" : "BeneficiariosPorLocal_SINTETICO.xml");

			List<Parametro> lstParams = new List<Parametro>();

			if (lstPlanos != null && lstPlanos.Count > 0) {
				lstParams.AddRange(query.AddParameter(lstPlanos, DbType.String, "plano"));
			}
			if (lstUf != null && lstUf.Count > 0) {
				lstParams.AddRange(query.AddParameter(lstUf, DbType.String, "uf"));
			}
			/*if (lstRegional != null && lstRegional.Count > 0) {
                lstParams.AddRange(query.AddParameter(lstRegional, DbType.String, "regional"));
			}*/
			if (lstGrauParentesco != null && lstGrauParentesco.Count > 0) {
                lstParams.AddRange(query.AddParameter(lstGrauParentesco, DbType.String, "parentesco"));
			}
			if (deficiente != null) {
				lstParams.Add(query.AddParameter(deficiente.Value ? 'S' : 'N', DbType.AnsiStringFixedLength, "deficiente"));
			}
			if (estudante != null) {
				lstParams.Add(query.AddParameter(estudante.Value ? 'S' : 'N', DbType.AnsiStringFixedLength, "estudante"));
			}
			string sql = query.BuildFinalQuery();

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

        internal static DataTable BuscarBeneficiariosEmLista(List<string> lstPlanos, List<string> lstUf, List<string> lstRegional, List<string> lstGrauParentesco, bool? deficiente, bool? estudante, Database db)
        {
            ReportQueryUtil query = new ReportQueryUtil("BeneficiariosEmLista.xml");

            List<Parametro> lstParams = new List<Parametro>();

            if (lstPlanos != null && lstPlanos.Count > 0)
            {
                lstParams.AddRange(query.AddParameter(lstPlanos, DbType.String, "plano"));
            }
            if (lstUf != null && lstUf.Count > 0)
            {
                lstParams.AddRange(query.AddParameter(lstUf, DbType.String, "uf"));
            }
            /*if (lstRegional != null && lstRegional.Count > 0)
            {
                lstParams.AddRange(query.AddParameter(lstRegional, DbType.String, "regional"));
            }*/
            if (lstGrauParentesco != null && lstGrauParentesco.Count > 0)
            {
                lstParams.AddRange(query.AddParameter(lstGrauParentesco, DbType.String, "parentesco"));
            }
            if (deficiente != null)
            {
                lstParams.Add(query.AddParameter(deficiente.Value ? 'S' : 'N', DbType.AnsiStringFixedLength, "deficiente"));
            }
            if (estudante != null)
            {
                lstParams.Add(query.AddParameter(estudante.Value ? 'S' : 'N', DbType.AnsiStringFixedLength, "estudante"));
            }
            string sql = query.BuildFinalQuery();

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarCredenciadosEmLista(List<string> lstUf, string status, Database db)
        {
            ReportQueryUtil query = new ReportQueryUtil("CredenciadosEmLista.xml");

            List<Parametro> lstParams = new List<Parametro>();

            if (lstUf != null && lstUf.Count > 0)
            {
                lstParams.AddRange(query.AddParameter(lstUf, DbType.String, "uf"));
            }
            /*if (cdNatureza != null)
            {
                lstParams.Add(query.AddParameter(cdNatureza, DbType.Int32, "cd_natureza"));
            }
            if (sistema != null)
            {
                lstParams.Add(query.AddParameter(sistema, DbType.String, "sistema"));
            }*/
            if (status != null)
            {
                lstParams.Add(query.AddParameter(status, DbType.String, "status"));
            }
            string sql = query.BuildFinalQuery();

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

		internal static DataTable BuscarServicoPrestador(string tipo, DateTime? dtInicial, DateTime? dtFinal, List<string> lstMascara, List<int> lstRegional, Database db) {
			string reportFile = tipo.Equals("S") ? "ServicoPrestador_SINTETICO.xml" : "ServicoPrestador_ANALITICO.xml";
			ReportQueryUtil query = new ReportQueryUtil(reportFile);


			if (dtInicial != null && dtFinal != null) {
				query.AddParameter(dtInicial, dtFinal, DbType.Date, "periodo");
			}

			if (lstRegional != null && lstRegional.Count > 0) {
				query.AddParameter(lstRegional, DbType.Int32, "regional");
			}

			if (lstMascara != null && lstMascara.Count > 0) {
				query.AddParameter(lstMascara, DbType.String, "mascara");
			}
			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}
		
        internal static DataTable BuscarCustoInternacao(DateTime? dtRef, string cartao, string nomeBenef, 
			int? nroAutorizacao, List<int> lstPlano, Database db) {
			ReportQueryUtil query = new ReportQueryUtil("CustoInternacao.xml");

			if (dtRef != null)
				query.AddParameter(dtRef, DbType.Date, "dataRef");

			if (!string.IsNullOrEmpty(cartao)) {
				query.AddParameter(cartao, DbType.String, "cartao");
			}

			if (!string.IsNullOrEmpty(nomeBenef)) {
				query.AddParameter("%" + nomeBenef.ToUpper() + "%", DbType.String, "nomeBenef");
			}

			if (nroAutorizacao != null) {
				query.AddParameter(nroAutorizacao, DbType.Int32, "nroAutorizacao");
			}

			if (lstPlano != null && lstPlano.Count > 0) {
				query.AddParameter(lstPlano, DbType.String, "plano");
			}

			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarFranquiasGeradas(DateTime? dtRef, string cartao, string nomeBenef, int? nroAutorizacao, Database db) {
			ReportQueryUtil query = new ReportQueryUtil("FranquiasGeradas.xml");

			if (dtRef != null)
				query.AddParameter(dtRef, DbType.Date, "dataRef");

			if (!string.IsNullOrEmpty(cartao)) {
				query.AddParameter(cartao, DbType.String, "cartao");
			}

			if (!string.IsNullOrEmpty(nomeBenef)) {
				query.AddParameter("%" + nomeBenef.ToUpper() + "%", DbType.String, "nomeBenef");
			}

			if (nroAutorizacao != null) {
				query.AddParameter(nroAutorizacao, DbType.Int32, "nroAutorizacao");
			}

			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarParcelamento(Database db) {
			ReportQueryUtil query = new ReportQueryUtil("Parcelamentos.xml");
			
			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarTravamentoISA(EvidaDatabase evdb) {
			ReportQueryUtil query = new ReportQueryUtil("TravamentosISA.xml");

			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(evdb, sql, lstParams);

			return dt;
		}

		internal static DataTable BuscarBeneficiariosCco(string cco, EvidaDatabase db) {
			ReportQueryUtil query = new ReportQueryUtil("BeneficiariosPorCco.xml");

			query.AddParameter(cco, DbType.String, "cco");

			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable DebitosCongelados(DateTime dataRef, string cdPlano, EvidaDatabase db) {
			ReportQueryUtil query = new ReportQueryUtil("DebitosCongelados.xml");

			query.AddParameter(dataRef, DbType.Date, "filtro");
			if (!string.IsNullOrEmpty(cdPlano))
				query.AddParameter(cdPlano, DbType.String, "plano");

			string sql = query.BuildFinalQuery();
			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

		internal static DataTable CreditosBeneficiario(DateTime dataInicio, DateTime dataFim, string cartao, EvidaDatabase db) {
			ReportQueryUtil query = new ReportQueryUtil("CreditoBeneficiario.xml");

			query.AddParameter(dataInicio, dataFim, DbType.Date, "dataDoc");

			if (!string.IsNullOrEmpty(cartao)) {
				query.AddParameter(cartao, DbType.String, "cartaoTitular");
			}

			string sql = query.BuildFinalQuery();

			List<Parametro> lstParams = query.GetParameters();
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}
	}
}
