using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace eVidaGeneralLib.DAO
{
    internal class IndisponibilidadeRedeDAO {

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT SQ_EV_INDISPONIBILIDADE_REDE.nextval FROM DUAL";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

        internal static DataTable BuscarIndisponibilidadeRede(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "SELECT CD_INDISPONIBILIDADE, IR.BA1_CODINT, IR.BA1_CODEMP, IR.BA1_MATRIC, IR.BA1_TIPREG, DT_SOLICITACAO, IR.ID_ESPECIALIDADE, NR_PRIORIDADE, NR_DIAS_PRAZO, ID_SITUACAO, " +
                      "	NR_PROTOCOLO_ANS, IR.SG_UF, IR.CD_MUNICIPIO, " +
                      "	BENEF.BA1_NOMUSR, ESP.NM_ESPECIALIDADE, DT_ATENDIMENTO, CD_USUARIO_ATENDENTE  " +
                      " FROM EV_INDISPONIBILIDADE_REDE IR, VW_PR_USUARIO BENEF, EV_ESPECIALIDADE ESP " +
                      " WHERE trim(BENEF.BA1_CODINT) = trim(IR.BA1_CODINT) " +
                      "	AND trim(BENEF.BA1_CODEMP) = trim(IR.BA1_CODEMP) " +
                      "	AND trim(BENEF.BA1_MATRIC) = trim(IR.BA1_MATRIC) " +
                      "	AND trim(BENEF.BA1_TIPREG) = trim(IR.BA1_TIPREG) " +
                      "	AND trim(BENEF.BA1_CODINT) = trim(:codint) " +
                      "	AND trim(BENEF.BA1_CODEMP) = trim(:codemp) " +
                      "	AND trim(BENEF.BA1_MATRIC) = trim(:matric) " +
                      "	AND ESP.ID_ESPECIALIDADE = IR.ID_ESPECIALIDADE " +
                      " ORDER BY CD_INDISPONIBILIDADE DESC ";

			List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

			return dt;
		}

        internal static DataTable Pesquisar(long? matricula, int? id, string protocoloAns, List<StatusIndisponibilidadeRede> filtroStatus, List<EncaminhamentoIndisponibilidadeRede> filtroSetor, string uf, int? idMunicipio, int? pendencia, string procedencia, bool? acompanhante, EvidaDatabase evdb)
        {
            string sql = "SELECT IR.CD_INDISPONIBILIDADE, IR.BA1_CODINT, IR.BA1_CODEMP, IR.BA1_MATRIC, IR.BA1_TIPREG, BENEF.BA1_MATEMP, DT_SOLICITACAO, IR.ID_ESPECIALIDADE, NR_PRIORIDADE, IR.NR_DIAS_PRAZO, IR.ID_SITUACAO, " +
                    "	NR_PROTOCOLO_ANS, IR.SG_UF, IR.CD_MUNICIPIO, CD_USUARIO_ATUANTE, " +
                    "	BENEF.BA1_NOMUSR, ESP.NM_ESPECIALIDADE, ID_SETOR_ENCAMINHAMENTO, " +
                    "	DT_SOLICITACAO + IR.NR_DIAS_PRAZO VENCIMENTO, M.BID_DESCRI, IR.TP_PENDENCIA, IR.DT_PENDENCIA, " +
                    "	CASE ID_SITUACAO WHEN 3 THEN 0 ELSE trunc(sysdate)-trunc(DT_SOLICITACAO + NR_DIAS_PRAZO) END DIAS_ATRASO, H.NR_HORAS, IR.DT_SITUACAO, IR.DT_ATENDIMENTO, IR.CD_USUARIO_ATENDENTE, IR.DS_PROCEDENCIA, IR.FL_ACOMPANHANTE " +
                    " FROM EV_INDISPONIBILIDADE_REDE IR, VW_PR_USUARIO BENEF, EV_ESPECIALIDADE ESP, VW_PR_MUNICIPIO M, VW_INDISP_REDE_HORAS_TOTAL H " +
                    " WHERE trim(BENEF.BA1_CODINT) = trim(IR.BA1_CODINT) " +
                    "   AND trim(BENEF.BA1_CODEMP) = trim(IR.BA1_CODEMP) " +
                    "	AND trim(BENEF.BA1_MATRIC) = trim(IR.BA1_MATRIC) " +
                    "	AND trim(BENEF.BA1_TIPREG) = trim(IR.BA1_TIPREG) " +
                    "	AND ESP.ID_ESPECIALIDADE = IR.ID_ESPECIALIDADE " +
                    "	AND IR.CD_MUNICIPIO = M.BID_CODMUN (+) " +
                    "	AND IR.CD_INDISPONIBILIDADE = H.CD_INDISPONIBILIDADE(+) ";

			List<Parametro> lstParams = new List<Parametro>();
			if (matricula != null) {
                sql += " AND trim(benef.BA1_MATEMP) = trim(:matricula) ";
				lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.Int64, Value = matricula.Value });
			}
			if (id != null) {
                sql += " AND IR.CD_INDISPONIBILIDADE = :id ";
				lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id.Value });
			}
			if (!string.IsNullOrEmpty(protocoloAns)) {
				sql += " AND NR_PROTOCOLO_ANS = :protocoloAns ";
				lstParams.Add(new Parametro() { Name = ":protocoloAns", Tipo = DbType.String, Value = protocoloAns });

			}
			if (filtroStatus != null && filtroStatus.Count > 0) {
                sql += " AND IR.ID_SITUACAO IN (-1 ";
				int idx = 1;
				foreach (StatusIndisponibilidadeRede status in filtroStatus) {
					string param = ":status_" + idx;
					sql += ", " + param;
					lstParams.Add(new Parametro() { Name = param, Tipo = DbType.Int32, Value = (int)status });
					idx++;
				}
				sql += ")";
			}

			if (filtroSetor != null) {
				//if (!filtroSetor.Contains(EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO)) {
					sql += " AND ID_SETOR_ENCAMINHAMENTO IN (-1";
					int idx = 1;
					foreach (EncaminhamentoIndisponibilidadeRede setor in filtroSetor) {
						string param = ":setor_" + idx;
						sql += ", " + param;
						lstParams.Add(new Parametro() { Name = param, Tipo = DbType.Int32, Value = (int)setor });
						idx++;
					}
					sql += ")";
				//}
			}

			if (!string.IsNullOrEmpty(uf)) {
				sql += " AND IR.SG_UF = :uf ";
				lstParams.Add(new Parametro(":uf", DbType.String, uf));
			}

			if (idMunicipio != null) {
				sql += " AND IR.CD_MUNICIPIO = :idMunicipio ";
				lstParams.Add(new Parametro(":idMunicipio", DbType.Int32, idMunicipio.Value));
			}
			if (pendencia != null) {
				if (pendencia.Value == -1) { // SEM PENDENCIA
					sql += " AND IR.TP_PENDENCIA IS NULL ";
				} else {
					sql += " AND IR.TP_PENDENCIA = :pendencia";
					lstParams.Add(new Parametro(":pendencia", DbType.Int32, pendencia.Value));
				}
				
			}
            if (!string.IsNullOrEmpty(procedencia))
            {
                sql += " AND IR.DS_PROCEDENCIA = :procedencia";
                lstParams.Add(new Parametro(":procedencia", DbType.String, procedencia));
            }

            if (acompanhante != null)
            {
                sql += " AND IR.FL_ACOMPANHANTE = :flAcompanhante";
                lstParams.Add(new Parametro(":flAcompanhante", DbType.StringFixedLength, (bool)acompanhante ? 'S' : 'N'));
            }

            sql += " ORDER BY IR.NR_DIAS_PRAZO ASC ";
			DataTable dt = BaseDAO.ExecuteDataSet(evdb, sql, lstParams);

			return dt;
		}

		internal static IndisponibilidadeRedeVO GetById(int id, EvidaDatabase evdb) {
			string sql = "SELECT * FROM EV_INDISPONIBILIDADE_REDE A" +
				"		WHERE a.cd_indisponibilidade = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));

			List<IndisponibilidadeRedeVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static IndisponibilidadeRedeVO FromDataRow(DataRow dr) {
			IndisponibilidadeRedeVO vo = new IndisponibilidadeRedeVO();
			vo.Id = Convert.ToInt32(dr["CD_INDISPONIBILIDADE"]);
			vo.DataCriacao = Convert.ToDateTime(dr["DT_SOLICITACAO"]);

			vo.Usuario = new PUsuarioVO();
            vo.Usuario.Codint = dr.Field<string>("BA1_CODINT");
            vo.Usuario.Codemp = dr.Field<string>("BA1_CODEMP");
            vo.Usuario.Matric = dr.Field<string>("BA1_MATRIC");
            vo.Usuario.Tipreg = dr.Field<string>("BA1_TIPREG");
			vo.IdEspecialidade = Convert.ToInt32(dr["ID_ESPECIALIDADE"]);
			vo.Prioridade = (PrioridadeIndisponibilidadeRede) Convert.ToInt32(dr["NR_PRIORIDADE"]);

			vo.TelefoneContato = dr.Field<string>("DS_TELEFONE");
			vo.EmailContato = dr.Field<string>("DS_EMAIL");

			vo.DiasPrazo = Convert.ToInt32(dr["NR_DIAS_PRAZO"]);

			vo.TelefonePrestador = dr.Field<string>("DS_TELEFONE_PRESTADOR");
			vo.EnderecoPrestador = dr.Field<string>("DS_ENDERECO_PRESTADOR");
			vo.ValorSolicitacao = BaseDAO.GetNullableDecimal(dr, "VL_SOLICITACAO");
			vo.CodigoServicoFinanceiro = dr.Field<string>("CD_SERVICO_FINANCEIRO");
			
			vo.AvalAutorizacao = dr.IsNull("ID_AVAL_AUTORIZACAO") ? new AvalIndisponibilidadeRede?() : (AvalIndisponibilidadeRede)Convert.ToInt32(dr["ID_AVAL_AUTORIZACAO"]);
			vo.AvalDiretoria = dr.IsNull("ID_AVAL_DIRETORIA") ? new AvalIndisponibilidadeRede?() : (AvalIndisponibilidadeRede)Convert.ToInt32(dr["ID_AVAL_DIRETORIA"]);
			vo.AvalFaturamento = dr.IsNull("ID_AVAL_FATURAMENTO") ? new AvalIndisponibilidadeRede?() : (AvalIndisponibilidadeRede)Convert.ToInt32(dr["ID_AVAL_FATURAMENTO"]);
			
			vo.Agencia = dr.Field<string>("DS_AGENCIA");
			vo.Banco = dr.Field<string>("DS_BANCO");
			vo.ContaCorrente = dr.Field<string>("DS_CONTA_CORRENTE");
			vo.Favorecido = dr.Field<string>("DS_FAVORECIDO");

			vo.SituacaoFinanceiro = dr.IsNull("ID_SITUACAO_FINANCEIRO") ? new SituacaoFinanceiroIndisponibilidadeRede?() : (SituacaoFinanceiroIndisponibilidadeRede)Convert.ToInt32(dr["ID_SITUACAO_FINANCEIRO"]);
			vo.ValorFinanceiro = BaseDAO.GetNullableDecimal(dr, "VL_FINANCEIRO");

			vo.Situacao = (StatusIndisponibilidadeRede)Convert.ToInt32(dr["ID_SITUACAO"]);
			vo.DataSituacao = Convert.ToDateTime(dr["DT_SITUACAO"]);
			
			vo.ComplementoDiretoria = dr.Field<string>("DS_COMPLEMENTO_DIRETORIA");
			vo.ComplementoAutorizacao = dr.Field<string>("DS_COMPLEMENTO_AUTORIZACAO");

            vo.Acompanhante = dr.IsNull("FL_ACOMPANHANTE") ? new bool?() : dr.Field<string>("FL_ACOMPANHANTE").Equals("S");

			vo.ObservacaoFinanceiro = dr.Field<string>("DS_OBS_FINANCEIRO");
			vo.ObservacaoFinanceiroBaixa = dr.Field<string>("DS_OBS_FINANCEIRO_BAIXA");

            vo.ObservacaoCadastro = dr.Field<string>("DS_OBS_CADASTRO");

			vo.SetorEncaminhamento = (EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(dr["ID_SETOR_ENCAMINHAMENTO"]);
			vo.CodUsuarioAtuante = BaseDAO.GetNullableInt(dr, "CD_USUARIO_ATUANTE");

			vo.MotivoEncerramento = dr.Field<string>("DS_MOTIVO_ENCERRAMENTO");

            vo.Procedencia = dr.Field<string>("DS_PROCEDENCIA");

			vo.CodUsuarioFaturamento = dr.IsNull("CD_USUARIO_FATURAMENTO") ? new int?() : Convert.ToInt32(dr["CD_USUARIO_FATURAMENTO"]);
			vo.ProtocoloFaturamento = dr.IsNull("NR_PROTOCOLO_FATURAMENTO") ? new decimal?() : Convert.ToDecimal(dr["NR_PROTOCOLO_FATURAMENTO"]);
            vo.ObservacaoFaturamento = dr.Field<string>("DS_OBS_FATURAMENTO");

			vo.CpfCnpjCred = dr.IsNull("NR_CNPJ_CPF") ? new long?() : Convert.ToInt64(dr["NR_CNPJ_CPF"]);
			vo.RazaoSocialCred = dr.Field<string>("NM_RAZAO_SOCIAL");

			vo.ProtocoloAns = dr.Field<string>("NR_PROTOCOLO_ANS");

			vo.Uf = dr.Field<string>("SG_UF");
			vo.IdLocalidade = BaseDAO.GetNullableInt(dr, "CD_MUNICIPIO");

			vo.DataPendencia = BaseDAO.GetNullableDate(dr, "DT_PENDENCIA");
			vo.Pendencia = BaseDAO.GetNullableEnum<TipoPendenciaIndisponibilidadeRede>(dr, "TP_PENDENCIA");
			vo.TratativaGuiaMedico = dr.Field<string>("DS_GUIA_MEDICO");
			vo.TratativaReciprocidade = dr.Field<string>("DS_RECIPROCIDADE");

			vo.DataExecucao = BaseDAO.GetNullableDate(dr, "DT_EXEC_COBRANCA");
			vo.ValorExecucao = BaseDAO.GetNullableDecimal(dr, "VL_EXEC_COBRANCA");

            vo.DataAtendimento = BaseDAO.GetNullableDate(dr, "DT_ATENDIMENTO");
            vo.CodUsuarioAtendente = BaseDAO.GetNullableInt(dr, "CD_USUARIO_ATENDENTE");

			return vo;
		}

		private static EspecialidadeVO FromDataRowEspecialidade(DataRow dr) {
			EspecialidadeVO vo = new EspecialidadeVO();
			vo.Id = Convert.ToInt32(dr["ID_ESPECIALIDADE"]);
			vo.Nome = Convert.ToString(dr["NM_ESPECIALIDADE"]);
			vo.PrazoConsulta = BaseDAO.GetNullableInt(dr, "NR_PRAZO_CONSULTA");
			vo.PrazoExame = BaseDAO.GetNullableInt(dr, "NR_PRAZO_EXAME");
			vo.PrazoAltaComplexidade = BaseDAO.GetNullableInt(dr, "NR_PRAZO_ALTA_COMPLEXIDADE");
			vo.PrazoAtendimentoHosp = BaseDAO.GetNullableInt(dr, "NR_PRAZO_REGIME_HOSPITALAR");
			vo.PrazoUrgenciaEmergencia = BaseDAO.GetNullableInt(dr, "NR_PRAZO_URGENCIA");
			vo.PrazoOutros = BaseDAO.GetNullableInt(dr, "NR_PRAZO_OUTROS");
			return vo;
		}

		private static IndisponibilidadeRedeOrcamentoVO FromDataRowOrcamento(DataRow dr) {
			IndisponibilidadeRedeOrcamentoVO vo = new IndisponibilidadeRedeOrcamentoVO();
			vo.CpfCnpj = Convert.ToString(dr["nr_cpf_cnpj"]);
			vo.IdOrcamento = Convert.ToInt32(dr["id_orcamento"]);
			vo.NomePrestador = Convert.ToString(dr["ds_prestador"]);
			vo.Telefone = Convert.ToString(dr["ds_telefone"]);
			vo.Email = Convert.ToString(dr["ds_email"]);
			vo.Valor = BaseDAO.GetNullableDecimal(dr, "vl_orcamento");
			vo.IdIndisponibilidade = Convert.ToInt32(dr["CD_INDISPONIBILIDADE"]);
			return vo;
		}

		internal static List<EspecialidadeVO> ListarEspecialidades(EvidaDatabase db) {
			string sql = "SELECT * FROM EV_ESPECIALIDADE A ORDER BY NM_ESPECIALIDADE";

			List<EspecialidadeVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowEspecialidade);
			return lst;
		}

		internal static void Criar(IndisponibilidadeRedeVO vo, EvidaDatabase evdb) {
            string sql = "INSERT INTO EV_INDISPONIBILIDADE_REDE (CD_INDISPONIBILIDADE, DT_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, ID_ESPECIALIDADE, " +
				"	NR_PRIORIDADE, DS_TELEFONE, DS_EMAIL, NR_DIAS_PRAZO, " +
				"	ID_SITUACAO, ID_SETOR_ENCAMINHAMENTO, DT_SITUACAO, " +
				"	NR_CNPJ_CPF, NM_RAZAO_SOCIAL, VL_SOLICITACAO, DS_ENDERECO_PRESTADOR, DS_TELEFONE_PRESTADOR, " +
                "	NR_PROTOCOLO_ANS, SG_UF, CD_MUNICIPIO, DT_ATENDIMENTO, CD_USUARIO_ATENDENTE) " +
                " VALUES (:id, LOCALTIMESTAMP, :codint, :codemp, :matric, :tipreg, :idEspecialidade, " +
				"	:prioridade, :telContato, :email, :diasPrazo, " +
				"	:status, :idSetor, LOCALTIMESTAMP, " +
				"	:cnpj, :razaoSocial, :vlSolicitacao, :endPrestador, :telPrestador, " +
                "	PKG_PROTOCOLO_ANS.F_NEXT_VALUE(), :uf, :municipio, :dtAtendimento, :cdUsuarioAtendente) ";

			vo.Id = NextId(evdb);
			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
            lstParams.Add(new Parametro(":codint", DbType.String, vo.Usuario.Codint.Trim()));
            lstParams.Add(new Parametro(":codemp", DbType.String, vo.Usuario.Codemp.Trim()));
            lstParams.Add(new Parametro(":matric", DbType.String, vo.Usuario.Matric.Trim()));
            lstParams.Add(new Parametro(":tipreg", DbType.String, vo.Usuario.Tipreg.Trim()));
			lstParams.Add(new Parametro(":idEspecialidade", DbType.Int32, vo.IdEspecialidade));
			lstParams.Add(new Parametro(":prioridade", DbType.Int32, (int)vo.Prioridade));
			lstParams.Add(new Parametro(":telContato", DbType.String, vo.TelefoneContato));
			lstParams.Add(new Parametro(":email", DbType.String, vo.EmailContato.Upper()));
			lstParams.Add(new Parametro(":diasPrazo", DbType.Int32, vo.DiasPrazo));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusIndisponibilidadeRede.ABERTO));

            if (vo.Uf == "SP")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_ARARAQUARA));

            if (vo.Uf == "PA")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_BELEM));


            if (vo.Uf == "RR")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_BOAVISTA));


            if (vo.Uf == "MT")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_CUIABA));

            if (vo.Uf == "AP")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_MACAPA));

            if (vo.Uf == "AM")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_MANAUS));

            if (vo.Uf == "TO")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_PALMAS));

            if (vo.Uf == "RO")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_PORTOVELHO));

            if (vo.Uf == "AC")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_RIOBRANCO));

            if (vo.Uf == "MA")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.PAT_SAOLUIS));

            if (vo.Uf == "DF")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "AC")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "AL")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "BA")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "CE")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int) EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "ES")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "GO")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "MG")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "PB")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "PR")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "PE")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "PI")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "RJ")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "RN")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "RS")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "SC")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));

            if (vo.Uf == "SE")
                lstParams.Add(new Parametro(":idSetor", DbType.Int32,
                    (int)EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));


            lstParams.Add(new Parametro(":cnpj", DbType.Int64, vo.CpfCnpjCred));
			lstParams.Add(new Parametro(":razaoSocial", DbType.String, vo.RazaoSocialCred.Upper()));

			lstParams.Add(new Parametro(":vlSolicitacao", DbType.Decimal, vo.ValorSolicitacao));

			lstParams.Add(new Parametro(":endPrestador", DbType.String, vo.EnderecoPrestador));
			lstParams.Add(new Parametro(":telPrestador", DbType.String, vo.TelefonePrestador));

			lstParams.Add(new Parametro(":uf", DbType.String, vo.Uf));


            lstParams.Add(new Parametro(":municipio", DbType.Int32, vo.IdLocalidade));


            if (vo.DataAtendimento != DateTime.MinValue)
            {
                lstParams.Add(new Parametro(":dtAtendimento", DbType.DateTime, vo.DataAtendimento));
                lstParams.Add(new Parametro(":cdUsuarioAtendente", DbType.Int32, vo.CodUsuarioAtendente));
            }
            else
            {
                lstParams.Add(new Parametro(":dtAtendimento", DbType.DateTime, DBNull.Value));
                lstParams.Add(new Parametro(":cdUsuarioAtendente", DbType.Int32, DBNull.Value));
            }

			BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);
		}


        internal static void Alterar(IndisponibilidadeRedeVO vo, EvidaDatabase evdb) {
            string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET BA1_CODINT = :codint, BA1_CODEMP = :codemp, BA1_MATRIC = :matric, BA1_TIPREG = :tipreg, ID_ESPECIALIDADE = :idEspecialidade, " +
				"	NR_PRIORIDADE = :prioridade, DS_TELEFONE = :telContato, DS_EMAIL = :email, NR_DIAS_PRAZO = :diasPrazo, " +
				"	NR_CNPJ_CPF = :cnpj, NM_RAZAO_SOCIAL = :razaoSocial, VL_SOLICITACAO = :vlSolicitacao, " +
				"	DS_ENDERECO_PRESTADOR = :endPrestador, DS_TELEFONE_PRESTADOR = :telPrestador, " +
                "	SG_UF = :uf, CD_MUNICIPIO = :municipio, DS_GUIA_MEDICO = :guiaMedico, DS_RECIPROCIDADE = :reciprocidade, DT_ATENDIMENTO = :dtAtendimento, CD_USUARIO_ATENDENTE = :cdUsuarioAtendente " +
				"	WHERE CD_INDISPONIBILIDADE = :id";


			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro(":id", DbType.Int32, vo.Id));
            lstParams.Add(new Parametro(":codint", DbType.String, vo.Usuario.Codint.Trim()));
            lstParams.Add(new Parametro(":codemp", DbType.String, vo.Usuario.Codemp.Trim()));
            lstParams.Add(new Parametro(":matric", DbType.String, vo.Usuario.Matric.Trim()));
            lstParams.Add(new Parametro(":tipreg", DbType.String, vo.Usuario.Tipreg.Trim()));
            lstParams.Add(new Parametro(":idEspecialidade", DbType.Int32, vo.IdEspecialidade));
			lstParams.Add(new Parametro(":prioridade", DbType.Int32, (int)vo.Prioridade));
			lstParams.Add(new Parametro(":telContato", DbType.String, vo.TelefoneContato));
			lstParams.Add(new Parametro(":email", DbType.String, vo.EmailContato.Upper()));
			lstParams.Add(new Parametro(":diasPrazo", DbType.Int32, vo.DiasPrazo));

			lstParams.Add(new Parametro(":cnpj", DbType.Int64, vo.CpfCnpjCred));
			lstParams.Add(new Parametro(":razaoSocial", DbType.String, vo.RazaoSocialCred.Upper()));

			lstParams.Add(new Parametro(":vlSolicitacao", DbType.Decimal, vo.ValorSolicitacao));

			lstParams.Add(new Parametro(":endPrestador", DbType.String, vo.EnderecoPrestador));
			lstParams.Add(new Parametro(":telPrestador", DbType.String, vo.TelefonePrestador));

			lstParams.Add(new Parametro(":uf", DbType.String, vo.Uf));
			lstParams.Add(new Parametro(":municipio", DbType.Int32, vo.IdLocalidade));

			lstParams.Add(new Parametro(":guiaMedico", DbType.String, vo.TratativaGuiaMedico));
			lstParams.Add(new Parametro(":reciprocidade", DbType.String, vo.TratativaReciprocidade));

            if (vo.DataAtendimento != DateTime.MinValue)
            {
                lstParams.Add(new Parametro(":dtAtendimento", DbType.DateTime, vo.DataAtendimento));
                lstParams.Add(new Parametro(":cdUsuarioAtendente", DbType.Int32, vo.CodUsuarioAtendente));
            }
            else
            {
                lstParams.Add(new Parametro(":dtAtendimento", DbType.DateTime, DBNull.Value));
                lstParams.Add(new Parametro(":cdUsuarioAtendente", DbType.Int32, DBNull.Value));
            }

			BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);
		}

		internal static void SalvarCredenciamento(IndisponibilidadeRedeVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET DS_BANCO = :banco, DS_AGENCIA = :agencia, DS_CONTA_CORRENTE = :contaCorrente, " +
				"	DS_FAVORECIDO = :favorecido, VL_FINANCEIRO = :vlFinanceiro, " +
				"	CD_SERVICO_FINANCEIRO = :cdServicoFinanceiro, ID_AVAL_FATURAMENTO = :avalFaturamento " +
				"	WHERE CD_INDISPONIBILIDADE = :id";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":banco", DbType.String, vo.Banco));
			lstParam.Add(new Parametro(":agencia", DbType.String, vo.Agencia));
			lstParam.Add(new Parametro(":contaCorrente", DbType.String, vo.ContaCorrente));
			lstParam.Add(new Parametro(":favorecido", DbType.String, vo.Favorecido));
			lstParam.Add(new Parametro(":vlFinanceiro", DbType.Decimal, vo.ValorFinanceiro));
			lstParam.Add(new Parametro(":cdServicoFinanceiro", DbType.String, vo.CodigoServicoFinanceiro));
			lstParam.Add(new Parametro(":avalFaturamento", DbType.Int32, vo.AvalFaturamento != null ? (int)vo.AvalFaturamento.Value : new int?()));
			
			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
		}

		internal static void SalvarFinanceiro(IndisponibilidadeRedeVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET DS_OBS_FINANCEIRO = :obsFinanceiro, DS_OBS_FINANCEIRO_BAIXA = :obsFinanceiro2 " +
				"	WHERE CD_INDISPONIBILIDADE = :id";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":obsFinanceiro", DbType.AnsiString, vo.ObservacaoFinanceiro));
			lstParam.Add(new Parametro(":obsFinanceiro2", DbType.AnsiString, vo.ObservacaoFinanceiroBaixa));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
		}

		internal static void ExecutarCobranca(int idIndisponibilidade, DateTime dtExecucao, decimal vlExecucao, EvidaDatabase db) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET DT_EXEC_COBRANCA = :dtExecucao, VL_EXEC_COBRANCA = :vlExecucao, " +
				"	ID_SITUACAO_FINANCEIRO = :situacaoFinanceiro " +
				"	WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idIndisponibilidade));
			lstParam.Add(new Parametro(":dtExecucao", DbType.Date, dtExecucao));
			lstParam.Add(new Parametro(":vlExecucao", DbType.Decimal, vlExecucao));
			lstParam.Add(new Parametro(":situacaoFinanceiro", DbType.Int32, (int)SituacaoFinanceiroIndisponibilidadeRede.EXECUCAO_COBRANCA_REALIZADA));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void SalvarDiretoria(IndisponibilidadeRedeVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET ID_AVAL_DIRETORIA = :aval, DS_COMPLEMENTO_DIRETORIA = :complemento " +
				"	WHERE CD_INDISPONIBILIDADE = :id";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":complemento", DbType.AnsiString, vo.ComplementoDiretoria));
			lstParam.Add(new Parametro(":aval", DbType.Int32, vo.AvalDiretoria != null ? (int)vo.AvalDiretoria.Value : new int?()));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
		}

		internal static void SalvarFaturamento(IndisponibilidadeRedeVO vo, EvidaDatabase db) {
            string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET NR_PROTOCOLO_FATURAMENTO = :protocolo, DS_OBS_FATURAMENTO = :obsFaturamento " +
				"	WHERE CD_INDISPONIBILIDADE = :id";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":protocolo", DbType.Decimal, vo.ProtocoloFaturamento));
            lstParam.Add(new Parametro(":obsFaturamento", DbType.AnsiString, vo.ObservacaoFaturamento));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void SalvarAutorizacao(IndisponibilidadeRedeVO vo, EvidaDatabase evdb) {
            string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET ID_AVAL_AUTORIZACAO = :aval, DS_COMPLEMENTO_AUTORIZACAO = :complemento, FL_ACOMPANHANTE = :acompanhante " +
				"	WHERE CD_INDISPONIBILIDADE = :id";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":complemento", DbType.AnsiString, vo.ComplementoAutorizacao));
			lstParam.Add(new Parametro(":aval", DbType.Int32, vo.AvalAutorizacao != null ? (int)vo.AvalAutorizacao.Value : new int?()));
            lstParam.Add(new Parametro(":acompanhante", DbType.StringFixedLength, Convert.ToBoolean(vo.Acompanhante) ? 'S' : 'N'));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
		}

        internal static void SalvarCadastro(IndisponibilidadeRedeVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET DS_OBS_CADASTRO = :obsCadastro " +
                "	WHERE CD_INDISPONIBILIDADE = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
            lstParam.Add(new Parametro(":obsCadastro", DbType.AnsiString, vo.ObservacaoCadastro));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }				
        
        internal static List<IndisponibilidadeRedeOrcamentoVO> ListarOrcamento(int idIndisponibilidade, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_INDISP_REDE_ORCAMENTO WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idIndisponibilidade));

			return BaseDAO.ExecuteDataSet(db, sql, FromDataRowOrcamento, lstParam);
		}

		internal static void SalvarOrcamentos(int id, List<IndisponibilidadeRedeOrcamentoVO> lstOrcamento, EvidaDatabase db) {
			string sql = "DELETE FROM EV_INDISP_REDE_ORCAMENTO WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			BaseDAO.ExecuteNonQuery(sql, lstParam, db);

			sql = "INSERT INTO EV_INDISP_REDE_ORCAMENTO (CD_INDISPONIBILIDADE, ID_ORCAMENTO, NR_CPF_CNPJ, DS_PRESTADOR, DS_TELEFONE, DS_EMAIL, VL_ORCAMENTO) " +
				"	VALUES (:idIndisp, :id, :cpfCnpj, :nome, :telefone, :email, :valor) ";


			List<Parametro> lstParamVar = new List<Parametro>();			
			lstParamVar.Add(new Parametro(":idIndisp", DbType.Int32, id));
			lstParamVar.Add(new ParametroVar(":id", DbType.Int32));
			lstParamVar.Add(new ParametroVar(":cpfCnpj", DbType.String));
			lstParamVar.Add(new ParametroVar(":nome", DbType.String));
			lstParamVar.Add(new ParametroVar(":telefone", DbType.String));
			lstParamVar.Add(new ParametroVar(":email", DbType.String));
			lstParamVar.Add(new ParametroVar(":valor", DbType.Decimal));

			List<ParametroVarRow> lstRows = new List<ParametroVarRow>();
			int idRow = 1;
			foreach (IndisponibilidadeRedeOrcamentoVO orc in lstOrcamento) {
				ParametroVarRow row = new ParametroVarRow(lstParamVar);
				row["id"] = idRow;
				row["cpfCnpj"] = orc.CpfCnpj;
				row["nome"] = orc.NomePrestador;
				row["telefone"] = orc.Telefone;
				row["email"] = orc.Email;
				row["valor"] = orc.Valor;

				lstRows.Add(row);
				idRow++;
			}
			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParamVar, lstRows, db);
		}

		#region Obs

		public static List<IndisponibilidadeRedeObsVO> ListarObs(int idIndisponibilidade, EvidaDatabase db) {
			string sql = "SELECT * FROM ev_indisp_rede_obs A" +
				"		WHERE a.cd_indisponibilidade = :id ORDER BY dt_registro";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idIndisponibilidade));

			List<IndisponibilidadeRedeObsVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowObs, lstParam);
			return lst;
		}

		private static IndisponibilidadeRedeObsVO FromDataRowObs(DataRow dr) {
			IndisponibilidadeRedeObsVO vo = new IndisponibilidadeRedeObsVO();
			vo.CodUsuario = dr.IsNull("CD_USUARIO") ? new int?() : Convert.ToInt32(dr["CD_USUARIO"]);
			vo.IdIndisponibilidade = Convert.ToInt32(dr["CD_INDISPONIBILIDADE"]);
			vo.DataRegistro = Convert.ToDateTime(dr["DT_REGISTRO"]);
			vo.Origem = dr.Field<string>("TP_ORIGEM");
			vo.Observacao = dr.Field<string>("DS_OBSERVACAO");
			vo.TipoObs = Convert.ToInt32(dr["TP_OBS"]);
			vo.Pendencia = BaseDAO.GetNullableEnum<TipoPendenciaIndisponibilidadeRede>(dr, "TP_PENDENCIA");
			return vo;
		}

		public static void IncluirObs(IndisponibilidadeRedeObsVO vo, EvidaDatabase evdb) {
			string sql = "INSERT INTO ev_indisp_rede_obs (CD_INDISPONIBILIDADE, DT_REGISTRO, TP_ORIGEM, CD_USUARIO, DS_OBSERVACAO, TP_PENDENCIA, TP_OBS) " +
				"	VALUES (:id, LOCALTIMESTAMP, :origem, :idUsuario, :obs, :pendencia, :tipo) ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.IdIndisponibilidade));
			lstParam.Add(new Parametro(":origem", DbType.String, vo.Origem));
			lstParam.Add(new Parametro(":idUsuario", DbType.Int32, vo.CodUsuario));
			lstParam.Add(new Parametro(":obs", DbType.AnsiString, vo.Observacao.Upper()));
			lstParam.Add(new Parametro(":tipo", DbType.Int32, vo.TipoObs));
			lstParam.Add(new Parametro(":pendencia", DbType.Int32, vo.Pendencia != null ? ((int)vo.Pendencia) : new int?()));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
		}

		#endregion

		#region Arquivo

		private static IndisponibilidadeRedeArquivoVO FromDataRowArquivo(DataRow dr) {
			IndisponibilidadeRedeArquivoVO vo = new IndisponibilidadeRedeArquivoVO();
			vo.IdArquivo = Convert.ToInt32(dr["CD_ARQUIVO"]);
			vo.IdIndisponibilidade = Convert.ToInt32(dr["CD_INDISPONIBILIDADE"]);
			vo.DataEnvio = Convert.ToDateTime(dr["dt_envio"]);
			vo.NomeArquivo = Convert.ToString(dr["nm_arquivo"]);
			vo.TipoArquivo = (TipoArquivoIndisponibilidadeRede)Convert.ToInt32(dr["tp_arquivo"]);
			return vo;
		}

		internal static void CriarArquivos(int idIndisponibilidade, List<IndisponibilidadeRedeArquivoVO> lstNewFiles, EvidaDatabase evdb) {
			Database db = evdb.Database;
			DbCommand dbCommand = CreateInsertArquivo(db);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, idIndisponibilidade);
			db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32);
			db.AddInParameter(dbCommand, ":tipo", DbType.Int32);
			db.AddInParameter(dbCommand, ":nome", DbType.String);

			int idNextArquivo = GetNextArquivoId(idIndisponibilidade, evdb);

			foreach (IndisponibilidadeRedeArquivoVO arq in lstNewFiles) {
				arq.IdArquivo = idNextArquivo++;

				db.SetParameterValue(dbCommand, ":idArquivo", arq.IdArquivo);
				db.SetParameterValue(dbCommand, ":tipo", (int)arq.TipoArquivo);
				db.SetParameterValue(dbCommand, ":nome", arq.NomeArquivo);

				BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			}

		}

		internal static List<IndisponibilidadeRedeArquivoVO> ListarArquivos(int idIndisponibilidade, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_INDISP_REDE_ARQUIVO A" +
				"		WHERE a.cd_indisponibilidade = :id ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idIndisponibilidade));

			List<IndisponibilidadeRedeArquivoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParam);
			return lst;
		}

		private static int GetNextArquivoId(int idIndisponibilidade, EvidaDatabase evdb) {
			string sql = "SELECT NVL(MAX(CD_ARQUIVO),0)+1 FROM EV_INDISP_REDE_ARQUIVO WHERE CD_INDISPONIBILIDADE = :idIndisp";
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":idIndisp", DbType.Int32, idIndisponibilidade));
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, sql, lstParam);

			return (int)idSolicitacao;
		}

		private static DbCommand CreateInsertArquivo(Database db) {
			string sql = "INSERT INTO EV_INDISP_REDE_ARQUIVO (CD_INDISPONIBILIDADE, CD_ARQUIVO, TP_ARQUIVO, NM_ARQUIVO, DT_ENVIO) " +
				"	VALUES (:id, :idArquivo, :tipo, :nome, LOCALTIMESTAMP) ";

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			return dbCommand;
		}

		internal static void ExcluirArquivo(IndisponibilidadeRedeArquivoVO vo, EvidaDatabase evdb) {
			string sql = "DELETE FROM EV_INDISP_REDE_ARQUIVO " +
				"	WHERE CD_INDISPONIBILIDADE = :idIndisp AND CD_ARQUIVO = :idArquivo";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":idIndisp", DbType.Int32, vo.IdIndisponibilidade));
			lstParam.Add(new Parametro(":idArquivo", DbType.Int32, vo.IdArquivo));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
		}

		#endregion

		#region Alteracao Status

		private static IndisponibilidadeRedeHistoricoVO FromDataRowHistorico(DataRow dr) {
			IndisponibilidadeRedeHistoricoVO vo = new IndisponibilidadeRedeHistoricoVO();
			vo.CodUsuario = dr.IsNull("CD_USUARIO") ? new int?() : Convert.ToInt32(dr["CD_USUARIO"]);
			vo.IdIndisponibilidade = Convert.ToInt32(dr["CD_INDISPONIBILIDADE"]);
			vo.DataHistorico = Convert.ToDateTime(dr["DT_STATUS"]);
			vo.StatusOrigem = dr.IsNull("ID_SITUACAO_ORIGEM") ? new StatusIndisponibilidadeRede?() : (StatusIndisponibilidadeRede) Convert.ToInt32(dr["ID_SITUACAO_ORIGEM"]);
			vo.StatusDestino = (StatusIndisponibilidadeRede)Convert.ToInt32(dr["ID_SITUACAO_DESTINO"]);
			vo.Setor = (EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(dr["ID_SETOR_ENCAMINHAMENTO"]);
			return vo;
		}

		internal static List<IndisponibilidadeRedeHistoricoVO> ListarHistorico(int idIndisponibilidade, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_INDISP_REDE_STATUS A" +
				"		WHERE a.cd_indisponibilidade = :id ORDER BY DT_STATUS";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idIndisponibilidade));

			List<IndisponibilidadeRedeHistoricoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowHistorico, lstParam);
			return lst;
		}

		public static void InsertStatusChange(int id, StatusIndisponibilidadeRede? statusOrigem, StatusIndisponibilidadeRede statusDestino, EncaminhamentoIndisponibilidadeRede setor,
			int? codUsuario, EvidaDatabase db) {
			string sql = "INSERT INTO EV_INDISP_REDE_STATUS (CD_INDISPONIBILIDADE, DT_STATUS, ID_SITUACAO_ORIGEM, ID_SITUACAO_DESTINO, ID_SETOR_ENCAMINHAMENTO, CD_USUARIO) " +
			"	VALUES (:id, LOCALTIMESTAMP, :statusOrigem, :statusDestino, :setor, :usuario) ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":statusOrigem", DbType.Int32, statusOrigem != null ? (int)statusOrigem : new int?()));
			lstParam.Add(new Parametro(":statusDestino", DbType.Int32, (int)statusDestino));
			lstParam.Add(new Parametro(":setor", DbType.Int32, (int)setor));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, codUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}
		
		internal static void AssumirSolicitacao(IndisponibilidadeRedeVO vo, UsuarioVO usuarioVO, EvidaDatabase db) {
			int? usuarioFaturamento = vo.CodUsuarioFaturamento;
			if (vo.SetorEncaminhamento == EncaminhamentoIndisponibilidadeRede.FATURAMENTO && usuarioFaturamento == null)
				usuarioFaturamento = usuarioVO.Id;

			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET CD_USUARIO_ATUANTE = :usuario, ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, " +
				"	CD_USUARIO_FATURAMENTO = :usuarioFat " +
				"	WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, usuarioVO.Id));
			lstParam.Add(new Parametro(":usuarioFat", DbType.Int32, usuarioFaturamento));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)StatusIndisponibilidadeRede.EM_ATENDIMENTO));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void EncaminharSolicitacao(IndisponibilidadeRedeVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET CD_USUARIO_ATUANTE = :usuario, ID_SITUACAO = :status, ID_SETOR_ENCAMINHAMENTO = :setor, DT_SITUACAO = LOCALTIMESTAMP, " +
				"	ID_SITUACAO_FINANCEIRO = :situacaoFinanceiro " +
				"	WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, null));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)StatusIndisponibilidadeRede.PENDENTE));
			lstParam.Add(new Parametro(":situacaoFinanceiro", DbType.Int32, vo.SituacaoFinanceiro != null ? (int)vo.SituacaoFinanceiro : new int?()));
			lstParam.Add(new Parametro(":setor", DbType.Int32, (int)vo.SetorEncaminhamento));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void Encerrar(IndisponibilidadeRedeVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET CD_USUARIO_ATUANTE = :usuario, ID_SITUACAO = :status, ID_SETOR_ENCAMINHAMENTO = :setor, " +
                "	DS_MOTIVO_ENCERRAMENTO = :motivo, DS_PROCEDENCIA = :procedencia, DT_SITUACAO = LOCALTIMESTAMP, DT_ATENDIMENTO = :dataatendimento, CD_USUARIO_ATENDENTE = :cdUsuarioAtendente " +
				"	WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, null));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)StatusIndisponibilidadeRede.ENCERRADO));
			lstParam.Add(new Parametro(":setor", DbType.Int32, EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO));// correto, pois é encerramento
			lstParam.Add(new Parametro(":motivo", DbType.String, vo.MotivoEncerramento));
            lstParam.Add(new Parametro(":procedencia", DbType.String, vo.Procedencia));
            lstParam.Add(new Parametro(":dataatendimento", DbType.Date, vo.DataAtendimento));
            lstParam.Add(new Parametro(":cdUsuarioAtendente", DbType.Int32, vo.CodUsuarioAtendente));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void AlterarPendencia(int id, TipoPendenciaIndisponibilidadeRede pendencia, EvidaDatabase db) {
			string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET TP_PENDENCIA = :pendencia, DT_PENDENCIA = LOCALTIMESTAMP " +
				"	WHERE CD_INDISPONIBILIDADE = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
            lstParam.Add(new Parametro(":pendencia", DbType.Int32, (pendencia == TipoPendenciaIndisponibilidadeRede.RECEBIDO || pendencia == TipoPendenciaIndisponibilidadeRede.RESOLVIDO) ? new int?() : (int)pendencia));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		#endregion

        internal static void ApagarEncerramento(IndisponibilidadeRedeVO vo, EvidaDatabase db)
        {
            string sql = "UPDATE EV_INDISPONIBILIDADE_REDE SET DS_MOTIVO_ENCERRAMENTO = :motivo, DS_PROCEDENCIA = :procedencia " +
                "	WHERE CD_INDISPONIBILIDADE = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
            lstParam.Add(new Parametro(":motivo", DbType.String, null));
            lstParam.Add(new Parametro(":procedencia", DbType.String, null));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static DataTable ListarEventos(int id, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_INDISP_REDE_EVENTO WHERE CD_INDISPONIBILIDADE = :id order by data";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, id));

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static List<Int32> ListarUltimosProtocolos(EvidaDatabase db)
        {
            string sql = "SELECT CD_INDISPONIBILIDADE FROM EV_INDISPONIBILIDADE_REDE WHERE ID_SITUACAO <> 3 OR SYSDATE < DT_SITUACAO + 90";

            return BaseDAO.ExecuteDataSet(db, sql, delegate(DataRow dr)
            {
                return Int32.Parse(dr["CD_INDISPONIBILIDADE"].ToString());
            });

        }

        public static void IncluirHorasSetor(IndisponibilidadeRedeHorasSetorVO vo, EvidaDatabase evdb)
        {
            string sql = "INSERT INTO ev_indisp_rede_horas_setor (CD_INDISPONIBILIDADE, ID_SETOR, NR_HORAS) " +
                "	VALUES (:id, :setor, :horas) ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, vo.IdIndisponibilidade));
            lstParam.Add(new Parametro(":setor", DbType.Int32, vo.Setor));
            lstParam.Add(new Parametro(":horas", DbType.Double, vo.Horas));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void ExcluirHorasSetor(int id, EvidaDatabase evdb)
        {
            string sql = "DELETE FROM EV_INDISP_REDE_HORAS_SETOR WHERE CD_INDISPONIBILIDADE = :id";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, id));
            BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);
        }

        private static IndisponibilidadeRedeHorasSetorVO FromDataRowHorasSetor(DataRow dr)
        {
            IndisponibilidadeRedeHorasSetorVO vo = new IndisponibilidadeRedeHorasSetorVO();
            vo.IdIndisponibilidade = Convert.ToInt32(dr["CD_INDISPONIBILIDADE"]);
            vo.Setor = (EncaminhamentoIndisponibilidadeRede)Convert.ToInt32(dr["ID_SETOR"]);
            vo.Horas = Convert.ToDouble(dr["NR_HORAS"]);
            return vo;
        }

        internal static List<IndisponibilidadeRedeHorasSetorVO> ListarHorasSetor(int id, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_INDISP_REDE_HORAS_SETOR" +
                "		WHERE CD_INDISPONIBILIDADE = :id ORDER BY ID_SETOR";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, id));

            List<IndisponibilidadeRedeHorasSetorVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowHorasSetor, lstParam);
            return lst;
        }

        internal static DataTable ListarProtocolosPendentes(EvidaDatabase db)
        {
            StringBuilder v_sql = new StringBuilder();
            v_sql.Append(" select id_setor_encaminhamento, rtrim(xmlagg(xmlelement(e, cd_indisponibilidade, ';').extract('//text()')),';')  ");
            v_sql.Append(" from ev_indisponibilidade_rede ");
            v_sql.Append(" where id_situacao <> :status ");
            v_sql.Append(" and dt_atendimento is null ");
            v_sql.Append(" and BA1_MATRIC is not null ");
            v_sql.Append(" group by id_setor_encaminhamento ");
            v_sql.Append(" order by id_setor_encaminhamento ");

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusIndisponibilidadeRede.ENCERRADO));

            DataTable dt = BaseDAO.ExecuteDataSet(db, v_sql.ToString(), lstParams);

            return dt;
        }

        internal static Double ObterHorasUsadas(int id, EvidaDatabase db)
        {
            string sql = "select NR_HORAS from VW_INDISP_REDE_HORAS_TOTAL where cd_indisponibilidade = :id";
            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, id));
            return Convert.ToDouble(BaseDAO.ExecuteScalar(db, sql, lstParam));
        }

        internal static DataTable ListarPendencias(int id, EvidaDatabase db)
        {
            StringBuilder v_sql = new StringBuilder();
            v_sql.Append(" select DATA, EVENTO ");
            v_sql.Append(" from VW_INDISP_REDE_EVENTO ");
            v_sql.Append(" where CD_INDISPONIBILIDADE = :id ");
            v_sql.Append(" and (evento = 'PENDENTE' or evento = 'RESOLVIDO') ");
            v_sql.Append(" order by DATA ");

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, id));

            DataTable dt = BaseDAO.ExecuteDataSet(db, v_sql.ToString(), lstParams);

            return dt;
        }
	}
}
