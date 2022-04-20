using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class AutorizacaoDAO {
		private static EVidaLog log = new EVidaLog(typeof(AutorizacaoDAO));

		private static StatusAutorizacao[] STATUS_EM_ANDAMENTO = new StatusAutorizacao[] { StatusAutorizacao.COTANDO_OPME, StatusAutorizacao.REVALIDACAO,
			StatusAutorizacao.EM_ANALISE, StatusAutorizacao.ENVIADA, StatusAutorizacao.SOLICITANDO_DOC, StatusAutorizacao.ENVIADO_AUDITORIA,
			StatusAutorizacao.ANALISADO_AUDITORIA};

        private const string SQL_SELECT = "SELECT a.*, " +
                "	benef.BA1_MATANT benef_CD_MATANT, benef.BA1_NOMUSR benef_NM_NOMUSR, plano.BI3_DESCRI plano_DESCRI, " +
                "	titular.BA1_CODINT titular_CODINT, titular.BA1_CODEMP titular_CODEMP, titular.BA1_MATRIC titular_MATRIC, titular.BA1_TIPREG titular_TIPREG, titular.BA1_NOMUSR titular_NOMUSR, titular.BA1_CODINT || titular.BA1_CODEMP || titular.BA1_MATRIC || titular.BA1_TIPREG || titular.BA1_DIGITO titular_MATANT, " +
                "	gestor.nm_usuario gestor_NM_USUARIO, responsavel.nm_usuario responsavel_NM_USUARIO " +
                "	FROM EV_AUTORIZACAO a, VW_PR_USUARIO benef, " +
                "		VW_PR_USUARIO_ATUAL titular, VW_PR_PRODUTO_SAUDE plano, EV_USUARIO gestor, EV_USUARIO responsavel " +
                "	WHERE trim(a.BA1_CODINT_TITULAR) = trim(titular.BA1_CODINT) " +
                "   AND trim(a.BA1_CODEMP_TITULAR) = trim(titular.BA1_CODEMP) " +
                "   AND trim(a.BA1_MATRIC_TITULAR) = trim(titular.BA1_MATRIC) " +
                "   AND trim(a.BA1_TIPREG_TITULAR) = trim(titular.BA1_TIPREG) " +
                "	AND trim(a.BI3_CODIGO) = trim(plano.BI3_CODIGO) " +
                "	AND trim(a.BA1_CODINT) = trim(benef.BA1_CODINT) " +
                "	AND trim(a.BA1_CODEMP) = trim(benef.BA1_CODEMP) " +
                "	AND trim(a.BA1_MATRIC) = trim(benef.BA1_MATRIC) " +
                "	AND trim(a.BA1_TIPREG) = trim(benef.BA1_TIPREG) " +
                "	AND a.cd_usuario_alteracao = gestor.id_usuario (+) " +
                "	AND a.cd_usuario_responsavel = responsavel.id_usuario (+) ";

		private static int NextId(EvidaDatabase evdb) {			
			string sql = "SELECT SQ_EV_AUTORIZACAO.nextval FROM DUAL";

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, sql);

			return (int)idSolicitacao;
		}

		internal static AutorizacaoVO GetById(int id, EvidaDatabase db) {
			string sql = SQL_SELECT +
				"		AND a.cd_autorizacao = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			List<AutorizacaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

        internal static DataTable BuscarAutorizacaoByBeneficiario(string codintTitular, string codempTitular, string matricTitular, string tipregTitular, EvidaDatabase db)
        {
            string sql = SQL_SELECT +
                "	AND trim(a.BA1_CODINT_TITULAR) = trim(:codintTitular) " +
                "	AND trim(a.BA1_CODEMP_TITULAR) = trim(:codempTitular) " +
                "	AND trim(a.BA1_MATRIC_TITULAR) = trim(:matricTitular) " +
                "	AND trim(a.BA1_TIPREG_TITULAR) = trim(:tipregTitular) ";	

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":codintTitular", DbType.String, codintTitular));
            lstParam.Add(new Parametro(":codempTitular", DbType.String, codempTitular));
            lstParam.Add(new Parametro(":matricTitular", DbType.String, matricTitular));
            lstParam.Add(new Parametro(":tipregTitular", DbType.String, tipregTitular));

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);
			return dt;
		}

		internal static DataTable BuscarAutorizacaoByCredenciado(string codCredenciado, EvidaDatabase db) {
			VO.Filter.FilterAutorizacaoVO filtro = new VO.Filter.FilterAutorizacaoVO();
			filtro.Credenciado = new VO.Protheus.PRedeAtendimentoVO() {
				Codigo = codCredenciado
			};
			return Pesquisar(filtro, db);
		}

		internal static DataTable BuscarEmAndamento(EvidaDatabase db) {
			string sql = SQL_SELECT;

			sql += " AND a.st_autorizacao IN (";

			for (int i = 0; i < STATUS_EM_ANDAMENTO.Length; i++) {
				if (i != 0) sql += ",";
				sql += ((int)STATUS_EM_ANDAMENTO[i]).ToString();
			}
			sql += ") ";

			sql += " ORDER BY DT_SOLICITACAO";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql);
			return dt;
		}

		internal static DataTable Pesquisar(VO.Filter.FilterAutorizacaoVO filtro, EvidaDatabase db) {
			string sql = SQL_SELECT;

			List<Parametro> lstParam = new List<Parametro>();

			if (filtro.Id != null) {
				sql += " AND a.cd_autorizacao = :id ";
				lstParam.Add(new Parametro(":id", DbType.Int32, filtro.Id));
			}
			if (filtro.NroAutorizacaoTiss != null) {
				sql += " AND EXISTS (SELECT 1 FROM EV_AUTORIZACAO_TISS at WHERE at.CD_AUTORIZACAO = A.CD_AUTORIZACAO AND at.nr_autorizacao_tiss = :tiss) ";
				lstParam.Add(new Parametro(":tiss", DbType.Int32, filtro.NroAutorizacaoTiss));
			}
			if (filtro.Origem != null) {
				sql += " AND a.TP_ORIGEM = :origem ";
				lstParam.Add(new Parametro(":origem", DbType.String, filtro.Origem.ToString()));
			}
			if (filtro.Status != null) {
				sql += " AND a.st_autorizacao = :status ";
				lstParam.Add(new Parametro(":status", DbType.Int32, (int)filtro.Status));
			} 
            else if (filtro.EmAndamento != null && filtro.EmAndamento.Value) {
				sql += " AND a.st_autorizacao IN (";
				
				for (int i = 0; i < STATUS_EM_ANDAMENTO.Length; i++) {
					string paramName = ":status" + i;
					if (i != 0) sql += ",";
					sql += paramName;
					lstParam.Add(new Parametro(paramName, DbType.Int32, (int)STATUS_EM_ANDAMENTO[i]));
				}
				sql += ") ";
			}

			if (filtro.Beneficiario != null) {
				if (!string.IsNullOrEmpty(filtro.Beneficiario.Nomusr)) {
					sql += " AND upper(trim(benef.BA1_NOMUSR)) LIKE upper(trim(:nmBenef)) ";
					lstParam.Add(new Parametro(":nmBenef", DbType.String, "%" + filtro.Beneficiario.Nomusr.ToUpper() + "%"));
				}
                if (!string.IsNullOrEmpty(filtro.Beneficiario.Matant))
                {
					sql += " AND trim(benef.BA1_MATANT) = trim(:cartao) ";
					lstParam.Add(new Parametro(":cartao", DbType.String, filtro.Beneficiario.Matant));
				}
                if (!string.IsNullOrEmpty(filtro.Beneficiario.Matemp))
                {
                    sql += " AND trim(benef.BA1_MATEMP) = trim(:matricula) ";
					lstParam.Add(new Parametro(":matricula", DbType.String, filtro.Beneficiario.Matemp));
				}
			}
			if (filtro.Credenciado != null) {
                if (!string.IsNullOrEmpty(filtro.Credenciado.Nome))
                {
                    sql += " AND upper(trim(a.BAU_NOME)) LIKE upper(trim(:nomeCred)) ";
					lstParam.Add(new Parametro(":nomeCred", DbType.String, "%" + filtro.Credenciado.Nome.ToUpper() + "%"));
				}
                if (!string.IsNullOrEmpty(filtro.Credenciado.Cpfcgc))
                {
                    sql += " AND trim(a.BAU_CPFCGC) = trim(:cpfCnpj) ";
					lstParam.Add(new Parametro(":cpfCnpj", DbType.String, filtro.Credenciado.Cpfcgc));
				}
                if (!string.IsNullOrEmpty(filtro.Credenciado.Codigo))
                {
                    sql += " AND trim(a.BAU_CODIGO) = trim(:cdCred) ";
					lstParam.Add(new Parametro(":cdCred", DbType.String, filtro.Credenciado.Codigo));
				}
			}
			if (filtro.Profissional != null) {
                if (!string.IsNullOrEmpty(filtro.Profissional.Numcr))
                {
                    sql += " AND trim(a.BB0_NUMCR) = trim(:cdProfissional) ";
                    lstParam.Add(new Parametro(":cdProfissional", DbType.String, filtro.Profissional.Numcr));
				}
                if (!string.IsNullOrEmpty(filtro.Profissional.Codsig))
                {
                    sql += " AND trim(a.BB0_CODSIG) = trim(:tipoConselho) ";
                    lstParam.Add(new Parametro(":tipoConselho", DbType.String, filtro.Profissional.Codsig));
				}
                if (!string.IsNullOrEmpty(filtro.Profissional.Estado))
                {
                    sql += " AND trim(a.BB0_ESTADO) = trim(:ufConselho_ ";
                    lstParam.Add(new Parametro(":ufConselho", DbType.String, filtro.Profissional.Estado));
				}
                if (!string.IsNullOrEmpty(filtro.Profissional.Nome))
                {
                    sql += " AND upper(trim(a.BB0_NOME)) LIKE upper(trim(:nomeProf)) ";
					lstParam.Add(new Parametro(":nomeProf", DbType.String, "%" + filtro.Profissional.Nome.ToUpper() + "%"));
				}
			}

			if (filtro.Carater != null) {
				sql += " AND a.tp_urgencia = :carater ";
				lstParam.Add(new Parametro(":carater", DbType.String, filtro.Carater.ToString()));
			}
            if (!string.IsNullOrEmpty(filtro.CodDoenca))
            {
                sql += " AND trim(a.BA9_CODDOE) = trim(:codDoenca) ";
				lstParam.Add(new Parametro(":codDoenca", DbType.String, filtro.CodDoenca.ToUpper()));
			}
            if (!string.IsNullOrEmpty(filtro.NomeDoenca))
            {
                sql += " AND a.BA9_CODDOE IN (SELECT BA9_CODDOE FROM VW_PR_DOENCA d where upper(trim(d.BA9_DOENCA)) LIKE upper(trim(:nmDoenca)))";
				lstParam.Add(new Parametro(":nmDoenca", DbType.String, "%" + filtro.NomeDoenca.ToUpper() + "%"));
			}

			if (filtro.Internacao != null) {
				sql += " AND a.FL_INTERNACAO = :internacao";
				lstParam.Add(new Parametro(":internacao", DbType.StringFixedLength, filtro.Internacao.Value ? 'S' : 'N'));
			}

			if (filtro.DataInternacao != null) {
				sql += " AND a.DT_INTERNACAO = :dtInternacao";
				lstParam.Add(new Parametro(":dtInternacao", DbType.Date, filtro.DataInternacao.Value));
			}

            if (!string.IsNullOrEmpty(filtro.Hospital))
            {
                sql += " AND upper(trim(a.BAU_NOME_HOSPITAL)) LIKE upper(trim(:hospital))";
				lstParam.Add(new Parametro(":hospital", DbType.String, "%" + filtro.Hospital.ToUpper() + "%"));
			}

            if (!string.IsNullOrEmpty(filtro.Indicacao))
            {
                sql += " AND upper(trim(a.DS_INDICACAO_CLINICA)) LIKE upper(trim(:indicacao))";
				lstParam.Add(new Parametro(":indicacao", DbType.String, "%" + filtro.Indicacao.ToUpper() + "%"));
			}

            if (filtro.Tfd != null)
            {
                sql += " AND a.FL_TFD = :tfd";
                lstParam.Add(new Parametro(":tfd", DbType.StringFixedLength, filtro.Tfd.Value ? 'S' : 'N'));
            }

			if (!string.IsNullOrEmpty(filtro.CodServico) || filtro.Opme != null) {
				sql += " AND EXISTS (SELECT 1 FROM EV_AUTORIZACAO_PROCEDIMENTO AP WHERE AP.CD_AUTORIZACAO = A.CD_AUTORIZACAO ";
				if (filtro.CodServico != null) {

                    string[] dados_servico = filtro.CodServico.Split('|');
                    string codpad = dados_servico[0];
                    string codpsa = dados_servico[1];

                    sql += " AND upper(trim(ap.BR8_CODPAD)) = upper(trim(:codpad)) AND upper(trim(ap.BR8_CODPSA)) = upper(trim(:codpsa)) ";
                    lstParam.Add(new Parametro(":codpad", DbType.String, codpad));
                    lstParam.Add(new Parametro(":codpsa", DbType.String, codpsa));
				}
				if (filtro.Opme != null) {
					sql += " AND ap.fl_opme = :opme ";
					lstParam.Add(new Parametro(":opme", DbType.StringFixedLength, filtro.Opme.Value ? 'S' : 'N'));
				}
				sql += ")";
			}

			if (filtro.CodUsuarioCriacao != null && filtro.CodUsuarioCriacao.Count > 0) {
				sql += " AND a.cd_usuario_criacao IN (0";
				for (int i = 0; i < filtro.CodUsuarioCriacao.Count; i++) {
					string field = ":criador" + i;
					sql += ", " + field;
					lstParam.Add(new Parametro(field, DbType.Int32, filtro.CodUsuarioCriacao[i]));
				}
				sql += ")";
			}

			if (filtro.Tipo != null) {
				sql += " AND a.tp_autorizacao = :tipo";
				lstParam.Add(new Parametro(":tipo", DbType.String, filtro.Tipo.ToString()));
			}

			if (filtro.CodUsuarioResponsavel != null) {
				sql += " AND a.cd_usuario_responsavel = :gestorResp";
				lstParam.Add(new Parametro(":gestorResp", DbType.Int32, filtro.CodUsuarioResponsavel));
			}

			if (!string.IsNullOrEmpty(filtro.ProtocoloAns)) {
				sql += " AND NR_PROTOCOLO_ANS = :protocoloAns ";
				lstParam.Add(new Parametro(":protocoloAns", DbType.String, filtro.ProtocoloAns));

			}
			
			sql += " ORDER BY NVL(DT_SOL_REVALIDACAO, DT_SOLICITACAO)";
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);
			return dt;
		}

		internal static List<AutorizacaoArquivoVO> ListArquivos(int idAutorizacao, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_AUTORIZACAO_ARQUIVO WHERE CD_AUTORIZACAO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idAutorizacao));

			List<AutorizacaoArquivoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParam);
			return lst;
		}

		internal static List<AutorizacaoProcedimentoVO> ListProcedimentos(int idAutorizacao, EvidaDatabase db) {
            string sql = " SELECT ap.*, s.BR8_CODPAD, s.BR8_CODPSA, s.BR8_ANASIN, s.BR8_DESCRI, s.BR8_BENUTL, s.BR8_CODROL " +
                         " from EV_AUTORIZACAO_PROCEDIMENTO ap, VW_PR_TABELA_PADRAO s " +
                         " WHERE upper(trim(s.BR8_CODPSA)) = upper(trim(ap.BR8_CODPSA)) " +
                         " AND upper(trim(s.BR8_CODPAD)) = upper(trim(ap.BR8_CODPAD)) " +
                         " AND CD_AUTORIZACAO = :id ";	

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idAutorizacao));

			List<AutorizacaoProcedimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowProc, lstParam);
			return lst;
		}

		internal static List<AutorizacaoStatusVO> ListStatus1(int idAutorizacao, EvidaDatabase db) {
			string sql = "SELECT aus.* FROM EV_AUTORIZACAO_STATUS aus " +
				"	WHERE aus.CD_AUTORIZACAO = :id ORDER BY DT_STATUS";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idAutorizacao));

			List<AutorizacaoStatusVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowStatus, lstParam);
			return lst;
		}

		internal static List<AutorizacaoSolDocVO> ListSolDoc(int idAutorizacao, EvidaDatabase db) {
			string sql = "SELECT aus.*, u.nm_usuario FROM EV_AUTORIZACAO_SOL_DOC aus, EV_USUARIO u " +
				"	WHERE aus.CD_AUTORIZACAO = :id AND u.id_usuario = aus.cd_usuario ORDER BY DT_SOLICITACAO";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idAutorizacao));

			List<AutorizacaoSolDocVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowSolDoc, lstParam);
			return lst;
		}

		internal static List<AutorizacaoTissVO> ListTiss(int idAutorizacao, EvidaDatabase db) {
			string sql = "SELECT aus.* FROM EV_AUTORIZACAO_TISS aus " +
				"	WHERE aus.CD_AUTORIZACAO = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idAutorizacao));

			List<AutorizacaoTissVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowTiss, lstParam);
			return lst;
		}

		internal static List<UsuarioVO> ListarUsuariosGestao(EvidaDatabase db) {
			string sql = "SELECT u.* FROM EV_USUARIO u " +
				"	WHERE EXISTS (SELECT 1 FROM EV_AUTORIZACAO A WHERE A.CD_USUARIO_ALTERACAO = u.ID_USUARIO)";

			List<UsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, UsuarioDAO.FromDataRow);
			return lst;
		}

		internal static DataTable ListarHistorico(int idAutorizacao, EvidaDatabase db) {
			string sql = "SELECT ah.*, u.CD_USUARIO AS NM_USUARIO_ALTERACAO" +
				"	FROM EV_AUTORIZACAO_HISTORICO ah, EV_USUARIO U " +
				"	WHERE U.id_usuario (+)= ah.cd_usuario_alteracao and CD_AUTORIZACAO = :id " +
				"	ORDER BY ah.DT_ALTERACAO DESC";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idAutorizacao));

			return BaseDAO.ExecuteDataSet(db, sql, lstParam);
		}

		internal static AutorizacaoVO FromDataRow(DataRow dr) {
			AutorizacaoVO vo = new AutorizacaoVO();
			vo.Id = Convert.ToInt32(dr["CD_AUTORIZACAO"]);

			vo.DataSolicitacao = dr.Field<DateTime>("DT_SOLICITACAO");
			vo.DataAutorizacao = dr.Field<DateTime?>("DT_AUTORIZACAO");

            vo.Usuario = new VO.Protheus.PUsuarioVO()
            {
                Matant = dr.Field<string>("benef_CD_MATANT"),
                Codint = dr.Field<string>("BA1_CODINT"),
                Codemp = dr.Field<string>("BA1_CODEMP"),
                Matric = dr.Field<string>("BA1_MATRIC"),
                Tipreg = dr.Field<string>("BA1_TIPREG"),
                Nomusr = dr.Field<string>("benef_NM_NOMUSR")
            };

			vo.ProdutoSaude = new VO.Protheus.PProdutoSaudeVO()
			{
				Codigo = dr.Field<string>("BI3_CODIGO"),
                Descri = dr.Field<string>("plano_DESCRI")
			};

			vo.Origem = AutorizacaoTradutorHelper.TraduzOrigem(dr.Field<string>("TP_ORIGEM"));

			vo.RedeAtendimento = new VO.Protheus.PRedeAtendimentoVO()
			{
                Codigo = dr.Field<string>("BAU_CODIGO"),
                Cpfcgc = dr.Field<string>("BAU_CPFCGC"),
				Nome = dr.Field<string>("BAU_NOME")
			};

            vo.Profissional = new VO.Protheus.PProfissionalSaudeVO()
            {
                Nome = dr.Field<string>("BB0_NOME"),
                Codsig = dr.Field<string>("BB0_CODSIG"),
                Numcr = dr.Field<string>("BB0_NUMCR"),
                Estado = dr.Field<string>("BB0_ESTADO")
            };

			vo.Carater = dr.IsNull("TP_URGENCIA") ? new CaraterAutorizacao?() :
				AutorizacaoTradutorHelper.TraduzCarater(dr.Field<string>("TP_URGENCIA"));

			vo.CodDoenca = dr.Field<string>("BA9_CODDOE");
			vo.IndicacaoClinica = dr.Field<string>("DS_INDICACAO_CLINICA");
			vo.Internacao = dr.Field<string>("FL_INTERNACAO").Equals("S");
			vo.DataInternacao = dr.Field<DateTime?>("DT_INTERNACAO");

            vo.Tfd = dr.IsNull("FL_TFD") ? new bool?() : dr.Field<string>("FL_TFD").Equals("S");
            vo.DataInicioTfd = dr.IsNull("DT_INICIO_TFD") ? new DateTime?() : dr.Field<DateTime?>("DT_INICIO_TFD");
            vo.DataTerminoTfd = dr.IsNull("DT_TERMINO_TFD") ? new DateTime?() : dr.Field<DateTime?>("DT_TERMINO_TFD");

			vo.Obs = dr.Field<string>("DS_OBSERVACAO");

			vo.Hospital = new VO.Protheus.PRedeAtendimentoVO()
			{
                Codigo = dr.Field<string>("BAU_CODIGO_HOSPITAL"),
				Nome = dr.Field<string>("BAU_NOME_HOSPITAL")
			};

			vo.Status = (StatusAutorizacao) Convert.ToInt32(dr["ST_AUTORIZACAO"]);
			vo.DataStatus = dr.Field<DateTime>("DT_STATUS");
			vo.MotivoCancelamento = dr.Field<string>("DS_MOTIVO_CANCELAMENTO");

			vo.CodNegativa = dr.IsNull("CD_NEGATIVA") ? new int?() : Convert.ToInt32(dr["CD_NEGATIVA"]);
			vo.CodUsuarioAlteracao = dr.IsNull("CD_USUARIO_ALTERACAO") ? new int?() : Convert.ToInt32(dr["CD_USUARIO_ALTERACAO"]);
			vo.CodUsuarioCriacao = dr.IsNull("CD_USUARIO_CRIACAO") ? new int?() : Convert.ToInt32(dr["CD_USUARIO_CRIACAO"]);
			vo.CodUsuarioResponsavel = dr.IsNull("CD_USUARIO_RESPONSAVEL") ? new int?() : Convert.ToInt32(dr["CD_USUARIO_RESPONSAVEL"]);

            vo.UsuarioTitular = new VO.Protheus.PUsuarioVO()
            {
                Matant = dr.Field<string>("titular_MATANT"),
                Codint = dr.Field<string>("titular_CODINT"),
                Codemp = dr.Field<string>("titular_CODEMP"),
                Matric = dr.Field<string>("titular_MATRIC"),
                Tipreg = dr.Field<string>("titular_TIPREG"),
                Nomusr = dr.Field<string>("titular_NOMUSR")
            };

			vo.HorasPrazo = Convert.ToDouble(dr["nr_horas_prazo"]);
			vo.Opme = dr.Field<string>("FL_OPME").Equals("S");

			vo.Tipo = dr.IsNull("TP_AUTORIZACAO") ? TipoAutorizacao.MEDICA : AutorizacaoTradutorHelper.TraduzTipo(dr.Field<string>("TP_AUTORIZACAO"));

			vo.DataAlteracao = Convert.ToDateTime(dr["dt_alteracao"]);
			vo.OrigemAlteracao = AutorizacaoTradutorHelper.TraduzOrigem(dr.Field<string>("TP_ORIGEM_ALTERACAO"));

			vo.ComentarioAuditor = dr.Field<string>("DS_COMENTARIO_AUDITOR");
			vo.ObsAprovacao = dr.Field<string>("DS_OBS_APROVACAO");

			vo.DataSolRevalidacao = BaseDAO.GetNullableDate(dr, "DT_SOL_REVALIDACAO");
			vo.DataAprovRevalidacao = BaseDAO.GetNullableDate(dr, "DT_APROV_REVALIDACAO");

			vo.ProtocoloAns = dr.Field<string>("NR_PROTOCOLO_ANS");

			return vo;
		}

		private static AutorizacaoArquivoVO FromDataRowArquivo(DataRow dr) {
			AutorizacaoArquivoVO vo = new AutorizacaoArquivoVO();
			vo.CodAutorizacao = Convert.ToInt32(dr["cd_autorizacao"]);
			vo.NomeArquivo = dr.Field<string>("nm_arquivo");
			vo.DataEnvio = dr.Field<DateTime>("dt_envio");
			return vo;
		}

		private static AutorizacaoProcedimentoVO FromDataRowProc(DataRow dr) {
			AutorizacaoProcedimentoVO vo = new AutorizacaoProcedimentoVO();
			vo.CodAutorizacao = Convert.ToInt32(dr["cd_autorizacao"]);
			vo.Servico = new VO.Protheus.PTabelaPadraoVO()
			{
                Codpad = dr.Field<string>("BR8_CODPAD"),
                Codpsa = dr.Field<string>("BR8_CODPSA"),
                Anasin = dr.Field<string>("BR8_ANASIN"),
                Descri = dr.Field<string>("BR8_DESCRI"),
                Benutl = dr.Field<string>("BR8_BENUTL"),
                Codrol = dr.Field<string>("BR8_CODROL")
			};
			vo.Opme = dr.Field<string>("fl_opme").Equals("S");
			vo.Observacao = dr.Field<string>("ds_observacao");
			vo.Quantidade = Convert.ToInt32(dr["qt_item"]);
			return vo;
		}

		private static AutorizacaoStatusVO FromDataRowStatus(DataRow dr) {
			AutorizacaoStatusVO vo = new AutorizacaoStatusVO();
			vo.CodAutorizacao = Convert.ToInt32(dr["cd_autorizacao"]);
			vo.Data = dr.Field<DateTime>("dt_status");
			vo.Status = (StatusAutorizacao)Convert.ToInt32(dr["ST_AUTORIZACAO"]);
			vo.CodUsuario = dr.IsNull("cd_usuario") ? new int?() : Convert.ToInt32(dr["cd_usuario"]);
			return vo;
		}

		private static AutorizacaoSolDocVO FromDataRowSolDoc(DataRow dr) {
			AutorizacaoSolDocVO vo = new AutorizacaoSolDocVO();
			vo.CodAutorizacao = Convert.ToInt32(dr["cd_autorizacao"]);
			vo.Data = dr.Field<DateTime>("dt_solicitacao");
			vo.MensagemSolDoc = dr.Field<string>("ds_solicitacao");
			vo.CodUsuario = Convert.ToInt32(dr["cd_usuario"]);
			vo.NomUsuario = dr.Field<string>("nm_usuario");
			return vo;
		}

		private static AutorizacaoTissVO FromDataRowTiss(DataRow dr) {
			AutorizacaoTissVO vo = new AutorizacaoTissVO();
			vo.CodAutorizacao = Convert.ToInt32(dr["cd_autorizacao"]);
			vo.NomeArquivo = dr.Field<string>("nm_arquivo");
			vo.NrAutorizacaoTiss = Convert.ToInt32(dr["nr_autorizacao_tiss"]);
			return vo;
		}

		private static void Salvar(AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, EvidaDatabase evdb) {
			DbCommand dbCommand = null;
			Database db = evdb.Database;
			bool insert = false;
			if (vo.Id == 0) {
				dbCommand = CreateInsert(vo, evdb);
				insert = true;
			} else {
				dbCommand = CreateUpdate(vo, db);
			}

			vo.Opme = false;
			if (lstProcedimentos != null && lstProcedimentos.Count > 0)
				vo.Opme = lstProcedimentos.FindIndex(x => x.Opme) >= 0;

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);

            if (vo.Usuario.Codint == null)
                db.AddInParameter(dbCommand, ":codint", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Usuario.Codint.Trim());

            if (vo.Usuario.Codemp == null)
                db.AddInParameter(dbCommand, ":codemp", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Usuario.Codemp.Trim());

            if (vo.Usuario.Matric == null)
                db.AddInParameter(dbCommand, ":matric", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Usuario.Matric.Trim());

            if (vo.Usuario.Tipreg == null)
                db.AddInParameter(dbCommand, ":tipreg", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Usuario.Tipreg.Trim());

            if (vo.UsuarioTitular.Codint == null)
                db.AddInParameter(dbCommand, ":codintTitular", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":codintTitular", DbType.String, vo.UsuarioTitular.Codint.Trim());

            if (vo.UsuarioTitular.Codemp == null)
                db.AddInParameter(dbCommand, ":codempTitular", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":codempTitular", DbType.String, vo.UsuarioTitular.Codemp.Trim());

            if (vo.UsuarioTitular.Matric == null)
                db.AddInParameter(dbCommand, ":matricTitular", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":matricTitular", DbType.String, vo.UsuarioTitular.Matric.Trim());

            if (vo.UsuarioTitular.Tipreg == null)
                db.AddInParameter(dbCommand, ":tipregTitular", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":tipregTitular", DbType.String, vo.UsuarioTitular.Tipreg.Trim());

            if (vo.ProdutoSaude.Codigo == null)
                db.AddInParameter(dbCommand, ":cdPlano", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":cdPlano", DbType.String, vo.ProdutoSaude.Codigo.Trim());

            if (vo.RedeAtendimento.Codigo == null)
                db.AddInParameter(dbCommand, ":cdCredenciado", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":cdCredenciado", DbType.String, vo.RedeAtendimento.Codigo.Trim());

            if (vo.RedeAtendimento.Cpfcgc == null)
                db.AddInParameter(dbCommand, ":cnpj", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":cnpj", DbType.String, vo.RedeAtendimento.Cpfcgc.Trim());

            if (vo.RedeAtendimento.Nome == null)
                db.AddInParameter(dbCommand, ":razaoSocial", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":razaoSocial", DbType.String, vo.RedeAtendimento.Nome.Trim());

            if (vo.Profissional.Numcr == null)
                db.AddInParameter(dbCommand, ":cdProfissional", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":cdProfissional", DbType.String, vo.Profissional.Numcr.Trim());

            if (vo.Profissional.Codsig == null)
                db.AddInParameter(dbCommand, ":codsig", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":codsig", DbType.String, vo.Profissional.Codsig.Trim());

            if (vo.Profissional.Estado == null)
                db.AddInParameter(dbCommand, ":estado", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":estado", DbType.String, vo.Profissional.Estado.Trim());

            if (vo.Profissional.Nome == null)
                db.AddInParameter(dbCommand, ":nomeProfissional", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":nomeProfissional", DbType.String, vo.Profissional.Nome.Trim());
            
            db.AddInParameter(dbCommand, ":tipo", DbType.String, vo.Tipo.ToString());

            if (vo.Carater == null)
                db.AddInParameter(dbCommand, ":carater", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":carater", DbType.String, vo.Carater.ToString());

            if (vo.CodDoenca == null)
                db.AddInParameter(dbCommand, ":cdDoenca", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":cdDoenca", DbType.String, vo.CodDoenca.Trim());

            if (vo.IndicacaoClinica == null)
                db.AddInParameter(dbCommand, ":indicacaoClinica", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":indicacaoClinica", DbType.String, vo.IndicacaoClinica.Trim());

			db.AddInParameter(dbCommand, ":flInternacao", DbType.StringFixedLength, vo.Internacao ? 'S' : 'N');
			db.AddInParameter(dbCommand, ":dtInternacao", DbType.Date, vo.DataInternacao);

            if (vo.Hospital.Codigo == null)
                db.AddInParameter(dbCommand, ":bau_codigo_hospital", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":bau_codigo_hospital", DbType.String, vo.Hospital.Codigo.Trim());

            if (vo.Hospital.Nome == null)
                db.AddInParameter(dbCommand, ":bau_nome_hospital", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":bau_nome_hospital", DbType.String, vo.Hospital.Nome.Trim().Upper());
          

            db.AddInParameter(dbCommand, ":flTfd", DbType.StringFixedLength, Convert.ToBoolean(vo.Tfd) ? 'S' : 'N');
            db.AddInParameter(dbCommand, ":dtInicioTfd", DbType.Date, vo.DataInicioTfd);
            db.AddInParameter(dbCommand, ":dtTerminoTfd", DbType.Date, vo.DataTerminoTfd);

            if (vo.Obs == null)
                db.AddInParameter(dbCommand, ":obs", DbType.String, DBNull.Value);
            else
                db.AddInParameter(dbCommand, ":obs", DbType.String, vo.Obs.Trim());

			db.AddInParameter(dbCommand, ":opme", DbType.StringFixedLength, vo.Opme ? 'S' : 'N');

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			if (insert)
				InsertStatusChange(vo.Origem, vo.Id, vo.Status, vo.CodUsuarioAlteracao, 0, evdb);

			SalvarProcedimentos(vo, lstProcedimentos, evdb);

			InserirHistorico(vo.Id, evdb);
		}

		private static DbCommand CreateInsert(AutorizacaoVO vo, EvidaDatabase evdb) {
            string sql = "INSERT INTO EV_AUTORIZACAO (CD_AUTORIZACAO, DT_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BA1_CODINT_TITULAR, BA1_CODEMP_TITULAR, BA1_MATRIC_TITULAR, BA1_TIPREG_TITULAR, " +
                "	BI3_CODIGO, TP_ORIGEM, BAU_CODIGO, BAU_CPFCGC, BAU_NOME, " +
                "	BB0_NUMCR, BB0_CODSIG, BB0_ESTADO, BB0_NOME, TP_URGENCIA, TP_AUTORIZACAO, " +
                "	BA9_CODDOE, DS_INDICACAO_CLINICA, FL_INTERNACAO, DT_INTERNACAO, BAU_CODIGO_HOSPITAL, BAU_NOME_HOSPITAL, FL_TFD, DT_INICIO_TFD, DT_TERMINO_TFD, DS_OBSERVACAO, ST_AUTORIZACAO, " +
                "	DT_STATUS, NR_HORAS_PRAZO, FL_OPME, CD_USUARIO_CRIACAO, DT_ALTERACAO, TP_ORIGEM_ALTERACAO, NR_PROTOCOLO_ANS) " +
                " VALUES (:id, LOCALTIMESTAMP, :codint, :codemp, :matric, :tipreg, :codintTitular, :codempTitular, :matricTitular, :tipregTitular, " +
                "	:cdPlano, :origem, :cdCredenciado, :cnpj, :razaoSocial, " +
                "	:cdProfissional, :codsig, :estado, :nomeProfissional, :carater, :tipo, " +
                "	:cdDoenca, :indicacaoClinica, :flInternacao, :dtInternacao, :bau_codigo_hospital, :bau_nome_hospital, :flTfd, :dtInicioTfd, :dtTerminoTfd, :obs, :status, " +
                "	LOCALTIMESTAMP, 0, :opme, :idUsuarioCriacao, LOCALTIMESTAMP, :origem, PKG_PROTOCOLO_ANS.F_NEXT_VALUE())";

			vo.Id = NextId(evdb);

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":origem", DbType.String, vo.Origem.ToString());
			db.AddInParameter(dbCommand, ":status", DbType.Int32, StatusAutorizacao.ENVIADA);
			db.AddInParameter(dbCommand, ":idUsuarioCriacao", DbType.Int32, vo.CodUsuarioCriacao);

			return dbCommand;
		}

		private static DbCommand CreateUpdate(AutorizacaoVO vo, Database db) {
            string sql = "UPDATE EV_AUTORIZACAO SET BA1_CODINT = :codint, BA1_CODEMP = :codemp, BA1_MATRIC = :matric, BA1_TIPREG = :tipreg, " +
                "	BA1_CODINT_TITULAR = :codintTitular, BA1_CODEMP_TITULAR = :codempTitular, BA1_MATRIC_TITULAR = :matricTitular, BA1_TIPREG_TITULAR = :tipregTitular, BI3_CODIGO = :cdPlano, " +
                "	BAU_CODIGO = :cdCredenciado, BAU_CPFCGC = :cnpj, BAU_NOME = :razaoSocial, " +
                "	BB0_NUMCR = :cdProfissional, BB0_CODSIG = :codsig, BB0_ESTADO = :estado, BB0_NOME = :nomeProfissional, " +
                "	TP_URGENCIA = :carater, TP_AUTORIZACAO = :tipo, DT_ALTERACAO = LOCALTIMESTAMP, TP_ORIGEM_ALTERACAO = :origemAlt, " +
                "	BA9_CODDOE = :cdDoenca, DS_INDICACAO_CLINICA = :indicacaoClinica, FL_INTERNACAO = :flInternacao, DT_INTERNACAO = :dtInternacao, " +
                "	BAU_CODIGO_HOSPITAL = :bau_codigo_hospital, BAU_NOME_HOSPITAL = :bau_nome_hospital, FL_TFD = :flTfd, DT_INICIO_TFD = :dtInicioTfd, DT_TERMINO_TFD = :dtTerminoTfd, DS_OBSERVACAO = :obs, FL_OPME = :opme " +
                (vo.CodUsuarioAlteracao != null ? ", CD_USUARIO_ALTERACAO = :usuarioAlt " : "") +
                        "	WHERE CD_AUTORIZACAO = :id";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":origemAlt", DbType.String, vo.OrigemAlteracao.ToString());
			if (vo.CodUsuarioAlteracao != null)
				db.AddInParameter(dbCommand, ":usuarioAlt", DbType.Int32, vo.CodUsuarioAlteracao);

			return dbCommand;
		}

		private static void ChangeStatus(OrigemAutorizacao origemAlteracao, int id, StatusAutorizacao newStatus, int? idUsuario, EvidaDatabase db) {
			string sql = "SELECT ST_AUTORIZACAO, DT_STATUS FROM EV_AUTORIZACAO WHERE CD_AUTORIZACAO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);
			bool changed = false;
			double horas = 0;

			if (dt == null || dt.Rows.Count == 0) {
				throw new Exception("Apenas update por realizar esta operação!");
			} 
			DataRow dr = dt.Rows[0];
			int oldStatus = Convert.ToInt32(dr["ST_AUTORIZACAO"]);
			DateTime oldData = Convert.ToDateTime(dr["dt_status"]);

			StatusAutorizacao status = (StatusAutorizacao)oldStatus;
			changed = status != newStatus;

			//log.Debug("changed: "  + changed);
			if (changed) {
				if (status != StatusAutorizacao.SOLICITANDO_DOC)
					horas = DateUtil.CalcularHorasDiasUteis(oldData, DateTime.Now);
				InsertStatusChange(origemAlteracao, id, newStatus, idUsuario, horas, db);
			}
		}

		private static void InsertStatusChange(OrigemAutorizacao origemAlteracao, int id, StatusAutorizacao status, int? codUsuario, double horasAdd, EvidaDatabase evdb) {
			string sql = "INSERT INTO EV_AUTORIZACAO_STATUS (CD_AUTORIZACAO, DT_STATUS, ST_AUTORIZACAO, CD_USUARIO) " +
				"	VALUES (:id, LOCALTIMESTAMP, :status, :usuario) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)status);
			db.AddInParameter(dbCommand, ":usuario", DbType.Int32, codUsuario);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			sql = "UPDATE EV_AUTORIZACAO SET DT_STATUS = LOCALTIMESTAMP, ST_AUTORIZACAO = :status, " +
				" NR_HORAS_PRAZO = (NR_HORAS_PRAZO+:horas), DT_ALTERACAO = LOCALTIMESTAMP, " +
				" TP_ORIGEM_ALTERACAO = :origem " +
				(codUsuario != null ? ", CD_USUARIO_ALTERACAO = :usuario " : "") +
				(codUsuario != null ? ", CD_USUARIO_RESPONSAVEL = NVL(CD_USUARIO_RESPONSAVEL, :usuario) " : "") +
				" WHERE CD_AUTORIZACAO = :id";
			dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)status);
			db.AddInParameter(dbCommand, ":horas", DbType.Double, horasAdd);
			db.AddInParameter(dbCommand, ":origem", DbType.String, origemAlteracao.ToString());
			if (codUsuario != null)
				db.AddInParameter(dbCommand, ":usuario", DbType.Int32, codUsuario);	

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static void InserirHistorico(int id, EvidaDatabase db) {
            string sql = "INSERT INTO EV_AUTORIZACAO_HISTORICO ( " +
                            " CD_AUTORIZACAO, DT_ALTERACAO, TP_ORIGEM_ALTERACAO, CD_USUARIO_ALTERACAO, DT_SOLICITACAO, DT_AUTORIZACAO, " +
                            " BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BA1_CODINT_TITULAR, BA1_CODEMP_TITULAR, BA1_MATRIC_TITULAR, BA1_TIPREG_TITULAR, BI3_CODIGO, TP_ORIGEM, BAU_CODIGO, BAU_CPFCGC, BAU_NOME, " +
                            " BB0_NUMCR, BB0_CODSIG, BB0_ESTADO, BB0_NOME, TP_AUTORIZACAO, BA9_CODDOE, DS_INDICACAO_CLINICA, FL_INTERNACAO, " +
                            " DT_INTERNACAO, BAU_NOME_HOSPITAL, FL_TFD, DT_INICIO_TFD, DT_TERMINO_TFD, DS_OBSERVACAO, ST_AUTORIZACAO, DT_STATUS, DS_MOTIVO_CANCELAMENTO, CD_NEGATIVA, " +
                            " DS_SOLICITACAO_DOC, NR_HORAS_PRAZO, FL_OPME, BAU_CODIGO_HOSPITAL, CD_USUARIO_CRIACAO, TP_URGENCIA, " +
                            " DS_COMENTARIO_AUDITOR, DS_OBS_APROVACAO, CD_USUARIO_RESPONSAVEL, DT_SOL_REVALIDACAO, DT_APROV_REVALIDACAO, NR_PROTOCOLO_ANS) " +
                            " SELECT CD_AUTORIZACAO, DT_ALTERACAO, TP_ORIGEM_ALTERACAO, A.CD_USUARIO_ALTERACAO, DT_SOLICITACAO, A.DT_AUTORIZACAO, " +
                            "	BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BA1_CODINT_TITULAR, BA1_CODEMP_TITULAR, BA1_MATRIC_TITULAR, BA1_TIPREG_TITULAR, BI3_CODIGO, TP_ORIGEM, BAU_CODIGO, BAU_CPFCGC, BAU_NOME, " +
                            "	BB0_NUMCR, BB0_CODSIG, BB0_ESTADO, BB0_NOME, TP_AUTORIZACAO, BA9_CODDOE, DS_INDICACAO_CLINICA, FL_INTERNACAO, " +
                            "	DT_INTERNACAO, BAU_NOME_HOSPITAL, FL_TFD, DT_INICIO_TFD, DT_TERMINO_TFD, DS_OBSERVACAO, ST_AUTORIZACAO, DT_STATUS, DS_MOTIVO_CANCELAMENTO, CD_NEGATIVA, " +
                            "	DS_SOLICITACAO_DOC, NR_HORAS_PRAZO, FL_OPME, BAU_CODIGO_HOSPITAL, CD_USUARIO_CRIACAO, TP_URGENCIA, " +
                            "	DS_COMENTARIO_AUDITOR, DS_OBS_APROVACAO, CD_USUARIO_RESPONSAVEL, DT_SOL_REVALIDACAO, DT_APROV_REVALIDACAO, NR_PROTOCOLO_ANS " +
                            " FROM EV_AUTORIZACAO A " +
                            " WHERE A.CD_AUTORIZACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, id));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		internal static List<AutorizacaoArquivoVO> SalvarArquivos(AutorizacaoVO vo, List<AutorizacaoArquivoVO> lstArquivos, EvidaDatabase evdb) {
			Database db = evdb.Database; 

			DbCommand dbInsert = CreateInsertArquivo(vo.Id, evdb.Database);
			DbCommand dbDelete = CreateDeleteArquivo(vo.Id, evdb.Database);

			List<AutorizacaoArquivoVO> lstOldArquivos = ListArquivos(vo.Id, evdb);

			List<AutorizacaoArquivoVO> lstNew = new List<AutorizacaoArquivoVO>();
			List<AutorizacaoArquivoVO> lstDel = new List<AutorizacaoArquivoVO>();

			if (lstOldArquivos == null) {
				lstNew = lstArquivos;
			} else {
				foreach (AutorizacaoArquivoVO item in lstArquivos) {
					int idxOld = lstOldArquivos.FindIndex(x => x.NomeArquivo.Equals(item.NomeArquivo, StringComparison.InvariantCultureIgnoreCase));
					if (idxOld < 0)
						lstNew.Add(item);
				}
				foreach (AutorizacaoArquivoVO item in lstOldArquivos) {
					int idxNew = lstArquivos.FindIndex(x => x.NomeArquivo.Equals(item.NomeArquivo, StringComparison.InvariantCultureIgnoreCase));
					if (idxNew < 0)
						lstDel.Add(item);
				}
			}
			foreach (AutorizacaoArquivoVO item in lstNew) {
				db.SetParameterValue(dbInsert, ":nome", item.NomeArquivo);
				BaseDAO.ExecuteNonQuery(dbInsert, evdb);
			}
			foreach (AutorizacaoArquivoVO item in lstDel) {
				db.SetParameterValue(dbDelete, ":nome", item.NomeArquivo); 
				BaseDAO.ExecuteNonQuery(dbDelete, evdb);
			}
			return lstDel;
		}

		private static DbCommand CreateInsertArquivo(int idAutorizacao, Database db) {
			string sql = "INSERT INTO EV_AUTORIZACAO_ARQUIVO (CD_AUTORIZACAO, NM_ARQUIVO, DT_ENVIO) " +
				" VALUES (:id, :nome, LOCALTIMESTAMP)";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":nome", DbType.String);
			return dbCommand;
		}

		private static DbCommand CreateDeleteArquivo(int idAutorizacao, Database db) {
			string sql = "DELETE FROM EV_AUTORIZACAO_ARQUIVO WHERE CD_AUTORIZACAO = :id AND NM_ARQUIVO = :nome";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":nome", DbType.String);

			return dbCommand;
		}

		private static void SalvarProcedimentos(AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, EvidaDatabase evdb) {
			DbCommand dbInsert = CreateInsertProc(vo.Id, evdb.Database);
			DbCommand dbUpdate = CreateUpdateProc(vo.Id, evdb.Database);
			DbCommand dbDelete = CreateDeleteProc(vo.Id, evdb.Database);

			List<AutorizacaoProcedimentoVO> lstOldProcs = ListProcedimentos(vo.Id, evdb);

			if (lstProcedimentos == null)
				if (lstOldProcs != null)
					lstProcedimentos = lstOldProcs;
				else
					lstProcedimentos = new List<AutorizacaoProcedimentoVO>();

			List<AutorizacaoProcedimentoVO> lstNew = new List<AutorizacaoProcedimentoVO>();
			List<AutorizacaoProcedimentoVO> lstUpd = new List<AutorizacaoProcedimentoVO>();
			List<AutorizacaoProcedimentoVO> lstDel = new List<AutorizacaoProcedimentoVO>();
			if (lstOldProcs == null) {
				lstNew = lstProcedimentos;
			} else {
				foreach (AutorizacaoProcedimentoVO item in lstProcedimentos) {
                    int idxOld = lstOldProcs.FindIndex(x => x.Servico.Codpad == item.Servico.Codpad && x.Servico.Codpsa == item.Servico.Codpsa);
					if (idxOld < 0)
						lstNew.Add(item);
					else
						lstUpd.Add(item);
				}

				foreach (AutorizacaoProcedimentoVO item in lstOldProcs) {
                    int idxNew = lstProcedimentos.FindIndex(x => x.Servico.Codpad == item.Servico.Codpad && x.Servico.Codpsa == item.Servico.Codpsa);
					if (idxNew < 0)
						lstDel.Add(item);
				}
			}

			foreach (AutorizacaoProcedimentoVO item in lstNew) {
				InsUpdProc(item, dbInsert, evdb);
			}
			foreach (AutorizacaoProcedimentoVO item in lstUpd) {
				InsUpdProc(item, dbUpdate, evdb);
			}
			foreach (AutorizacaoProcedimentoVO item in lstDel) {
				DelProc(item, dbDelete, evdb);
			}
		}

		private static void InsUpdProc(AutorizacaoProcedimentoVO vo, DbCommand dbCommand, EvidaDatabase evdb) {
			Database db = evdb.Database;
			db.SetParameterValue(dbCommand, ":opme", vo.Opme ? 'S' : 'N');
			db.SetParameterValue(dbCommand, ":codpad", vo.Servico.Codpad);
            db.SetParameterValue(dbCommand, ":codpsa", vo.Servico.Codpsa);
			db.SetParameterValue(dbCommand, ":qtd", vo.Quantidade);
			db.SetParameterValue(dbCommand, ":obs", vo.Observacao);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);			
		}

		private static void DelProc(AutorizacaoProcedimentoVO vo, DbCommand dbCommand, EvidaDatabase evdb) {
			Database db = evdb.Database;
            db.SetParameterValue(dbCommand, ":codpad", vo.Servico.Codpad);
            db.SetParameterValue(dbCommand, ":codpsa", vo.Servico.Codpsa);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);			
		}

		private static DbCommand CreateInsertProc(int idAutorizacao, Database db) {
            string sql = "INSERT INTO EV_AUTORIZACAO_PROCEDIMENTO (CD_AUTORIZACAO, BR8_CODPAD, BR8_CODPSA, " +
				"	FL_OPME, QT_ITEM, DS_OBSERVACAO) " +
				" VALUES (:id, :codpad, :codpsa, :opme, :qtd, :obs)";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":opme", DbType.StringFixedLength);
			db.AddInParameter(dbCommand, ":codpad", DbType.String);
            db.AddInParameter(dbCommand, ":codpsa", DbType.String);
			db.AddInParameter(dbCommand, ":qtd", DbType.Int32);
			db.AddInParameter(dbCommand, ":obs", DbType.String);

			return dbCommand;
		}

		private static DbCommand CreateUpdateProc(int idAutorizacao, Database db) {
			string sql = "UPDATE EV_AUTORIZACAO_PROCEDIMENTO SET FL_OPME = :opme, " +
				"	QT_ITEM = :qtd, DS_OBSERVACAO = :obs " +
                "	WHERE CD_AUTORIZACAO = :id AND BR8_CODPAD = :codpad AND BR8_CODPSA = :codpsa ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":opme", DbType.StringFixedLength);
            db.AddInParameter(dbCommand, ":codpad", DbType.String);
            db.AddInParameter(dbCommand, ":codpsa", DbType.String);
			db.AddInParameter(dbCommand, ":qtd", DbType.Int32);
			db.AddInParameter(dbCommand, ":obs", DbType.String);

			return dbCommand;
		}

		private static DbCommand CreateDeleteProc(int idAutorizacao, Database db) {
            string sql = "DELETE FROM EV_AUTORIZACAO_PROCEDIMENTO WHERE CD_AUTORIZACAO = :id AND BR8_CODPAD = :codpad AND BR8_CODPSA = :codpsa ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
            db.AddInParameter(dbCommand, ":codpad", DbType.String);
            db.AddInParameter(dbCommand, ":codpsa", DbType.String);
			return dbCommand;
		}

		internal static void Criar(AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, EvidaDatabase db) {
			vo.Id = 0;
			Salvar(vo, lstProcedimentos, db);
		}

		internal static void Alterar(AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, EvidaDatabase db) {
			Salvar(vo, lstProcedimentos, db);
		}

		internal static void EnviarDocumentos(OrigemAutorizacao origemAlteracao, int idAutorizacao, int? idUsuario, List<AutorizacaoArquivoVO> lstNewFiles, EvidaDatabase evdb) {
			ChangeStatus(origemAlteracao, idAutorizacao, StatusAutorizacao.EM_ANALISE, idUsuario, evdb);

			Database db = evdb.Database;
			DbCommand dbInsert = CreateInsertArquivo(idAutorizacao, db);

			foreach (AutorizacaoArquivoVO item in lstNewFiles) {
				db.SetParameterValue(dbInsert, ":nome", item.NomeArquivo);
				BaseDAO.ExecuteNonQuery(dbInsert, evdb);
			}
			InserirHistorico(idAutorizacao, evdb);
		}

		internal static void Cancelar(int idAutorizacao, string motivo, int? idUsuario, EvidaDatabase evdb) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.CANCELADA, idUsuario, evdb);
			
			string sql = "UPDATE EV_AUTORIZACAO SET DS_MOTIVO_CANCELAMENTO = :motivo WHERE CD_AUTORIZACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":motivo", DbType.String, motivo);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			InserirHistorico(idAutorizacao, evdb);
		}
		
		internal static void IniciarAnalise(int idAutorizacao, int idUsuario, EvidaDatabase db) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.EM_ANALISE, idUsuario, db);
			InserirHistorico(idAutorizacao, db);
		}

		internal static void IniciarCotacao(int idAutorizacao, int idUsuario, EvidaDatabase db) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.COTANDO_OPME, idUsuario, db);
			InserirHistorico(idAutorizacao, db);
		}

		internal static void SolicitarDocumento(int idAutorizacao, string strConteudo, int idUsuario, EvidaDatabase evdb) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.SOLICITANDO_DOC, idUsuario, evdb);
			
			string sql = "INSERT INTO EV_AUTORIZACAO_SOL_DOC (CD_AUTORIZACAO, DT_SOLICITACAO, DS_SOLICITACAO, CD_USUARIO) " +
				" VALUES (:id, LOCALTIMESTAMP, :motivo, :idUsuario) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
			db.AddInParameter(dbCommand, ":motivo", DbType.String, strConteudo);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			InserirHistorico(idAutorizacao, evdb);
		}

		internal static void SolicitarPericia(int idAutorizacao, int idUsuario, EvidaDatabase db) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.ENVIADO_AUDITORIA, idUsuario, db);
		}

		internal static void EnviarComentarios(int idAutorizacao, string comentario, int idUsuario, EvidaDatabase evdb) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.ANALISADO_AUDITORIA, idUsuario, evdb);
			string sql = "UPDATE EV_AUTORIZACAO SET DS_COMENTARIO_AUDITOR = :comentario WHERE CD_AUTORIZACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":comentario", DbType.String, comentario);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			InserirHistorico(idAutorizacao, evdb);
		}

		internal static void Negar(int idAutorizacao, int? idNeg, int idUsuario, EvidaDatabase evdb) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.NEGADA, idUsuario, evdb);

			if (idNeg != null) {
				string sql = "UPDATE EV_AUTORIZACAO SET CD_NEGATIVA = :negativa WHERE CD_AUTORIZACAO = :id";

				Database db = evdb.Database;
				DbCommand dbCommand = db.GetSqlStringCommand(sql);
				db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
				db.AddInParameter(dbCommand, ":negativa", DbType.Int32, idNeg);

				BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			}
			InserirHistorico(idAutorizacao, evdb);
		}

		internal static void Aprovar(int idAutorizacao, List<AutorizacaoTissVO> lstArquivo, string obs, int idUsuario, EvidaDatabase evdb) {
			ChangeStatus(OrigemAutorizacao.GESTOR, idAutorizacao, StatusAutorizacao.APROVADA, idUsuario, evdb);

			string sql = "UPDATE EV_AUTORIZACAO SET DS_OBS_APROVACAO = :obs, " +
				" DT_AUTORIZACAO = NVL(DT_AUTORIZACAO,LOCALTIMESTAMP), DT_APROV_REVALIDACAO = CASE WHEN DT_AUTORIZACAO IS NULL THEN NULL ELSE LOCALTIMESTAMP END " +
				" WHERE CD_AUTORIZACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":obs", DbType.String, obs);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			sql = "INSERT INTO EV_AUTORIZACAO_TISS (CD_AUTORIZACAO, NR_AUTORIZACAO_TISS, NM_ARQUIVO) " +
				"	VALUES (:id, :nroTiss, :arquivo) ";

			dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, idAutorizacao);
			db.AddInParameter(dbCommand, ":nroTiss", DbType.Int32);
			db.AddInParameter(dbCommand, ":arquivo", DbType.String);

			foreach (AutorizacaoTissVO vo in lstArquivo) {
				db.SetParameterValue(dbCommand, ":nroTiss", vo.NrAutorizacaoTiss);
				db.SetParameterValue(dbCommand, ":arquivo", vo.NomeArquivo);
				BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			}
			InserirHistorico(idAutorizacao, evdb);
		}

		internal static void Revalidar(AutorizacaoVO vo, EvidaDatabase evdb) {
			// resetar as horas, colocar no status certo, limpar arquivos tiss
			string sql = "UPDATE EV_AUTORIZACAO SET DT_SOL_REVALIDACAO = LOCALTIMESTAMP, DT_APROV_REVALIDACAO = NULL, NR_HORAS_PRAZO = 0, " +
				" DT_INTERNACAO = :dtInternacao " +
				" WHERE CD_AUTORIZACAO = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":dtInternacao", DbType.Date, vo.DataInternacao);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			sql = "DELETE FROM EV_AUTORIZACAO_TISS WHERE CD_AUTORIZACAO = :id";
			dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

			vo.Status = StatusAutorizacao.REVALIDACAO;
			InsertStatusChange(vo.Origem, vo.Id, vo.Status, vo.CodUsuarioAlteracao, 0, evdb);
			InserirHistorico(vo.Id, evdb);
		}

	}
}
