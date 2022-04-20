using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class FormNegativaDAO {

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT SQ_EV_FORM_NEGATIVA.nextval FROM DUAL";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		internal static void Salvar(FormNegativaVO vo, EvidaDatabase db) {
			if (vo.CodSolicitacao != 0) {
				Update(vo, db);
			} else {
				Insert(vo, db);
			}			
		}

		internal static void Salvar(FormNegativaReanaliseVO vo, EvidaDatabase db) {
			string sql = "UPDATE EV_FORM_NEGATIVA SET DT_REANALISE_SOL = :dtReanalise, IN_REANALISE_POSICIONAMENTO = :posicionamento, " +
				" DS_REANALISE_JUSTIFICATIVA = :justReanalise, DS_REANALISE_PARECER = :parecerObs, " +
				" ID_REANALISE_USUARIO = NVL(ID_REANALISE_USUARIO, :usuario), DT_REANALISE_CRIACAO = NVL(DT_REANALISE_CRIACAO, LOCALTIMESTAMP), " +
				" ID_REANALISE_USUARIO_UPDATE = :usuario, DT_REANALISE_UPDATE = LOCALTIMESTAMP, " +
				" CD_STATUS_REANALISE = :status, NR_REANALISE_PROTOCOLO_ANS = NVL(NR_REANALISE_PROTOCOLO_ANS, PKG_PROTOCOLO_ANS.F_NEXT_VALUE()) " +
				" WHERE CD_SOLICITACAO = :id";


			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.Id));
			lstParam.Add(new Parametro(":dtReanalise", DbType.Date, vo.DataFormulario));
			lstParam.Add(new Parametro(":posicionamento", DbType.Int32, vo.Parecer));
			lstParam.Add(new Parametro(":justReanalise", DbType.String, vo.JustificativaNegativa));
			lstParam.Add(new Parametro(":parecerObs", DbType.String, vo.ObservacaoParecer));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)vo.Status));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, vo.IdUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		private static void Insert(FormNegativaVO vo, EvidaDatabase evdb) {

            string sql = "INSERT INTO EV_FORM_NEGATIVA (CD_SOLICITACAO, IN_LEGAL, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, IN_ACOMODACAO, IN_TIPO_REDE, DS_SOLICITACAO, " +
                " DS_PREVISAO_CONTRATUAL, DT_FORMULARIO, BAU_NOME, BAU_CPFCGC, BAU_CODIGO, BB0_CODIGO, BB0_CODSIG, " +
                "	BB0_ESTADO, BB0_NOME, BB0_NUMCR, NR_CONTRATO, DT_SOLICITACAO, CD_MOTIVO_GLOSA, " +
                "	ID_USUARIO, DT_CREATE, CD_STATUS, ID_USUARIO_UPDATE, DT_UPDATE, NR_PROTOCOLO_ANS) " +
                " VALUES (:id, :inLegal, :codint, :codemp, :matric, :tipreg, :acomodacao, :tipoRede, :solicitacao, " +
                " :prevContratual, :dataForm, :nomePrestador, :cpfCnpj, :cdCred, :cdProf, :conselhoProf, " +
                "	:ufConselhoProf, :nomeProf, :nroConselhoProf, :contrato, :dataSolicitacao, :motivoGlosa, " +
                "	:usuario, LOCALTIMESTAMP, :status, :usuario, LOCALTIMESTAMP, PKG_PROTOCOLO_ANS.F_NEXT_VALUE())";    

			vo.CodSolicitacao = NextId(evdb);
			vo.DataCriacao = DateTime.Now;
			vo.IdUsuarioUpdate = vo.IdUsuario;
			vo.DataAlteracao = vo.DataCriacao;

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));
			lstParam.Add(new Parametro(":inLegal", DbType.Int32, vo.InfoDispositivoLegal));
            lstParam.Add(new Parametro(":codint", DbType.String, vo.Codint.Trim()));
            lstParam.Add(new Parametro(":codemp", DbType.String, vo.Codemp.Trim()));
            lstParam.Add(new Parametro(":matric", DbType.String, vo.Matric.Trim()));
            lstParam.Add(new Parametro(":tipreg", DbType.String, vo.Tipreg.Trim()));
			lstParam.Add(new Parametro(":acomodacao", DbType.String, vo.PadraoAcomodacao));
			lstParam.Add(new Parametro(":tipoRede", DbType.String, vo.TipoRede));
			lstParam.Add(new Parametro(":solicitacao", DbType.AnsiString, vo.DescricaoSolicitacao));
			lstParam.Add(new Parametro(":prevContratual", DbType.AnsiString, vo.PrevisaoContratual));
			lstParam.Add(new Parametro(":dataForm", DbType.Date, vo.DataFormulario));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, vo.IdUsuario));
			lstParam.Add(new Parametro(":status", DbType.String, FormNegativaStatus.SOB_ANALISE));

            lstParam.Add(new Parametro(":nomePrestador", DbType.String, vo.Prestador.Nome.Trim()));
            lstParam.Add(new Parametro(":cpfCnpj", DbType.String, vo.Prestador.Cpfcgc.Trim()));
            lstParam.Add(new Parametro(":cdCred", DbType.String, vo.Prestador.Codigo.Trim()));

            lstParam.Add(new Parametro(":cdProf", DbType.String, vo.Profissional.Codigo.Trim()));
            lstParam.Add(new Parametro(":nomeProf", DbType.String, vo.Profissional.Nome.Trim()));
            lstParam.Add(new Parametro(":conselhoProf", DbType.String, vo.Profissional.Codsig.Trim()));
            lstParam.Add(new Parametro(":ufConselhoProf", DbType.String, vo.Profissional.Estado.Trim()));
            lstParam.Add(new Parametro(":nroConselhoProf", DbType.String, vo.Profissional.Numcr.Trim()));

			lstParam.Add(new Parametro(":contrato", DbType.String, vo.NrContrato));
			lstParam.Add(new Parametro(":dataSolicitacao", DbType.Date, vo.DataSolicitacao));

			lstParam.Add(new Parametro(":motivoGlosa", DbType.Int32, vo.IdMotivoGlosa));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);

			InsertItems(vo, evdb);

			InsertJustificativas(vo, evdb);
		}

		private static void Update(FormNegativaVO vo, EvidaDatabase evdb) {

            string sql = "UPDATE EV_FORM_NEGATIVA SET IN_LEGAL = :inLegal, IN_ACOMODACAO = :acomodacao, IN_TIPO_REDE = :tipoRede, " +
                " DS_SOLICITACAO = :solicitacao, DS_PREVISAO_CONTRATUAL = :prevContratual, DT_FORMULARIO = :dataForm, " +
                "	BAU_NOME = :nomePrestador, BAU_CPFCGC = :cpfCnpj, BAU_CODIGO = :cdCred, " +
                "	BB0_CODIGO = :cdProf, BB0_CODSIG = :conselhoProf, BB0_ESTADO = :ufConselhoProf, " +
                "	BB0_NOME = :nomeProf, BB0_NUMCR = :nroConselhoProf, " +
                "	NR_CONTRATO = :contrato, DT_SOLICITACAO = :dataSolicitacao, " +
                "	CD_MOTIVO_GLOSA = :motivoGlosa, " +
                " ID_USUARIO_UPDATE = :usuario, DT_UPDATE = LOCALTIMESTAMP where CD_SOLICITACAO = :id";   

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));
			lstParam.Add(new Parametro(":inLegal", DbType.Int32, vo.InfoDispositivoLegal));
			lstParam.Add(new Parametro(":acomodacao", DbType.String, vo.PadraoAcomodacao));
			lstParam.Add(new Parametro(":tipoRede", DbType.String, vo.TipoRede));
			lstParam.Add(new Parametro(":solicitacao", DbType.AnsiString, vo.DescricaoSolicitacao));
			lstParam.Add(new Parametro(":prevContratual", DbType.AnsiString, vo.PrevisaoContratual));
			lstParam.Add(new Parametro(":dataForm", DbType.Date, vo.DataFormulario));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, vo.IdUsuario));

            lstParam.Add(new Parametro(":nomePrestador", DbType.String, vo.Prestador.Nome.Trim()));
            lstParam.Add(new Parametro(":cpfCnpj", DbType.String, vo.Prestador.Cpfcgc.Trim()));
            lstParam.Add(new Parametro(":cdCred", DbType.String, vo.Prestador.Codigo.Trim()));

            lstParam.Add(new Parametro(":cdProf", DbType.String, vo.Profissional.Codigo.Trim()));
            lstParam.Add(new Parametro(":nomeProf", DbType.String, vo.Profissional.Nome.Trim()));
            lstParam.Add(new Parametro(":conselhoProf", DbType.String, vo.Profissional.Codsig.Trim()));
            lstParam.Add(new Parametro(":ufConselhoProf", DbType.String, vo.Profissional.Estado.Trim()));
            lstParam.Add(new Parametro(":nroConselhoProf", DbType.String, vo.Profissional.Numcr.Trim()));

			lstParam.Add(new Parametro(":contrato", DbType.String, vo.NrContrato));
			lstParam.Add(new Parametro(":dataSolicitacao", DbType.Date, vo.DataSolicitacao));
			lstParam.Add(new Parametro(":motivoGlosa", DbType.Int32, vo.IdMotivoGlosa));

			BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);

			DeleteRelated(vo, evdb);

			InsertItems(vo, evdb);

			InsertJustificativas(vo, evdb);
		}

		private static void DeleteRelated(FormNegativaVO vo, EvidaDatabase db) {
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));

			string sql = "DELETE FROM EV_FORM_NEGATIVA_JUST where CD_SOLICITACAO = :id";
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);

			sql = "DELETE FROM EV_FORM_NEGATIVA_ITEM where CD_SOLICITACAO = :id";
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		private static void InsertJustificativas(FormNegativaVO vo, EvidaDatabase evdb) {
			string sql = "INSERT INTO EV_FORM_NEGATIVA_JUST (CD_SOLICITACAO, CD_JUSTIFICATIVA, IN_TIPO_JUSTIFICATIVA, DS_PARAMETROS) " +
				" VALUES (:id, :cdJus, :tipoJus, :param) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));

			lstParams.Add(new ParametroVar(":cdJus", DbType.Int32));
			lstParams.Add(new ParametroVar(":tipoJus", DbType.StringFixedLength));
			lstParams.Add(new ParametroVar(":param", DbType.AnsiString));

			List<ParametroVarRow> lstRows = new List<ParametroVarRow>();
			foreach (FormNegativaJustificativaVO bVO in vo.JustAssistencial) {
				ParametroVarRow row = new ParametroVarRow(lstParams);
				row["cdJus"] = bVO.IdJustificativa;
				row["tipoJus"] = "A";
				row["param"] = bVO.Parametros;
				lstRows.Add(row);
			}
			foreach (FormNegativaJustificativaVO bVO in vo.JustContratual) {
				ParametroVarRow row = new ParametroVarRow(lstParams);
				row["cdJus"] = bVO.IdJustificativa;
				row["tipoJus"] = "C";
				row["param"] = bVO.Parametros;
				lstRows.Add(row);
			}

			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParams, lstRows, evdb);
		}

		private static void InsertItems(FormNegativaVO vo, EvidaDatabase evdb) {

            string sql = "INSERT INTO EV_FORM_NEGATIVA_ITEM (CD_SOLICITACAO, BR8_CODPAD, BR8_CODPSA, BR8_DESCRI, QT_ITEM, DS_OBSERVACAO) " +
                " VALUES (:id, :codpad, :codpsa, :descri, :qtd, :obs) ";       

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));

            lstParams.Add(new ParametroVar(":codpad", DbType.String));
            lstParams.Add(new ParametroVar(":codpsa", DbType.String));
            lstParams.Add(new ParametroVar(":descri", DbType.String));
			lstParams.Add(new ParametroVar(":qtd", DbType.Int32));
			lstParams.Add(new ParametroVar(":obs", DbType.AnsiString));

			List<ParametroVarRow> lstRows = new List<ParametroVarRow>();

			foreach (FormNegativaItemVO bVO in vo.Itens) {
				ParametroVarRow row = new ParametroVarRow(lstParams);
                row["codpad"] = bVO.Codpad.Trim();
                row["codpsa"] = bVO.Codpsa.Trim();
                row["descri"] = bVO.Descri.Trim();
				row["qtd"] = bVO.Quantidade;
				row["obs"] = bVO.Observacao;
				lstRows.Add(row);
			}
			BaseDAO.ExecuteNonQueryMultiRows(sql, lstParams, lstRows, evdb);
		}

		internal static DataTable Pesquisar(string nomeBeneficiario, int cdProtocolo, string protocoloAns, string tipoRede, string cdMascara, string dsServico, 
			FormNegativaStatus? status, FormNegativaReanaliseStatus? statusReanalise, EvidaDatabase db) {

            string sql = "SELECT F.CD_SOLICITACAO, F.IN_ACOMODACAO, F.IN_TIPO_REDE, F.DT_FORMULARIO, NVL(F.ID_USUARIO_UPDATE, F.ID_USUARIO) ID_USUARIO, " +
                " F.CD_STATUS, B.BA1_NOMUSR, F.DS_MOTIVO_CANCELAMENTO, F.NR_PROTOCOLO_ANS, CD_STATUS_REANALISE " +
                " FROM EV_FORM_NEGATIVA F, VW_PR_USUARIO B " +
                " WHERE trim(F.BA1_CODINT) = trim(B.BA1_CODINT) AND trim(F.BA1_CODEMP) = trim(B.BA1_CODEMP) AND trim(F.BA1_MATRIC) = trim(B.BA1_MATRIC) AND trim(F.BA1_TIPREG) = trim(B.BA1_TIPREG) ";

            List<Parametro> lstParam = new List<Parametro>();
            if (!string.IsNullOrEmpty(nomeBeneficiario.Trim()))
            {
                sql += " AND upper(trim(B.BA1_NOMUSR)) LIKE upper(trim(:nmBeneficiario)) ";
                lstParam.Add(new Parametro(":nmBeneficiario", DbType.String, "%" + nomeBeneficiario.ToUpper() + "%"));
            }
            if (cdProtocolo != 0)
            {
                sql += " AND F.CD_SOLICITACAO = :cdProtocolo ";
                lstParam.Add(new Parametro(":cdProtocolo", DbType.Int32, cdProtocolo));
            }
            if (!string.IsNullOrEmpty(tipoRede))
            {
                sql += " AND F.IN_TIPO_REDE = :tipoRede ";
                lstParam.Add(new Parametro(":tipoRede", DbType.String, tipoRede));
            }
            if (!string.IsNullOrEmpty(cdMascara))
            {
                sql += " AND EXISTS (SELECT 1 FROM EV_FORM_NEGATIVA_ITEM FI WHERE FI.CD_SOLICITACAO = F.CD_SOLICITACAO ";
                sql += " AND upper(trim(FI.BR8_CODPSA)) = upper(trim(:cdMascara)) ) ";
                lstParam.Add(new Parametro(":cdMascara", DbType.String, cdMascara));
            }
            if (!string.IsNullOrEmpty(dsServico))
            {
                sql += " AND EXISTS (SELECT 1 FROM EV_FORM_NEGATIVA_ITEM FI WHERE FI.CD_SOLICITACAO = F.CD_SOLICITACAO ";
                sql += " AND upper(trim(FI.BR8_DESCRI)) LIKE upper(trim(:dsServico)) ) ";
                lstParam.Add(new Parametro(":dsServico", DbType.String, "%" + dsServico.ToUpper() + "%"));
            }
            if (!string.IsNullOrEmpty(protocoloAns))
            {
                sql += " AND F.NR_PROTOCOLO_ANS = :protocoloAns ";
                lstParam.Add(new Parametro(":protocoloAns", DbType.String, protocoloAns));
            }

            if (status != null)
            {
                sql += " AND F.CD_STATUS = :status";
                lstParam.Add(new Parametro(":status", DbType.String, status.Value.ToString()));
            }

            if (statusReanalise != null)
            {
                sql += " AND F.CD_STATUS_REANALISE = :statusReanalise";
                lstParam.Add(new Parametro(":statusReanalise", DbType.Int32, (int)statusReanalise.Value));
            }

            sql += " ORDER BY F.CD_SOLICITACAO DESC ";  
			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParam);

			return dt;
		}

        internal static List<FormNegativaVO> ListByBeneficiario(string codint, string codemp, string matric, string tipreg, EvidaDatabase db)
        {
			string sql = "SELECT F.* " +
				" FROM EV_FORM_NEGATIVA F" +
                " WHERE trim(F.BA1_CODINT) = trim(:codint) and trim(F.BA1_CODEMP) = trim(:codemp) and trim(F.BA1_MATRIC) = trim(:matric) and trim(F.BA1_TIPREG) = trim(:tipreg) ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":codint", DbType.String, codint));
            lstParam.Add(new Parametro(":codemp", DbType.String, codemp));
            lstParam.Add(new Parametro(":matric", DbType.String, matric));
            lstParam.Add(new Parametro(":tipreg", DbType.String, tipreg));

			sql += " ORDER BY F.CD_SOLICITACAO DESC ";
			List<FormNegativaVO> dt = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);

			return dt;
		}

		internal static FormNegativaVO GetById(int id, EvidaDatabase db) {
			FormNegativaVO vo = GetFormById(id, db);
			if (vo == null)
				return null;

			vo.Itens = GetItemsById(id, db);
			vo.JustAssistencial = GetJustificativasById(id, 'A', db);
			vo.JustContratual = GetJustificativasById(id, 'C', db);

			return vo;
		}

		private static FormNegativaVO GetFormById(int cdProtocolo, EvidaDatabase db) {
			string sql = " SELECT * " +
				" FROM EV_FORM_NEGATIVA F " +
				" WHERE F.CD_SOLICITACAO = :cdProtocolo ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":cdProtocolo", DbType.Int32, cdProtocolo));

			List<FormNegativaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParam);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static List<FormNegativaItemVO> GetItemsById(int cdProtocolo, EvidaDatabase db) {

            string sql = " SELECT CD_SOLICITACAO, BR8_CODPAD, BR8_CODPSA, BR8_DESCRI, QT_ITEM, DS_OBSERVACAO " +
                " FROM EV_FORM_NEGATIVA_ITEM FI " +
                " WHERE FI.CD_SOLICITACAO = :cdProtocolo ";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":cdProtocolo", DbType.Int32, cdProtocolo));

			List<FormNegativaItemVO> lst = BaseDAO.ExecuteDataSet(db, sql, ItemFromDataRow, lstParam);
			return lst;
		}

		private static List<FormNegativaJustificativaVO> GetJustificativasById(int cdProtocolo, char tipo, EvidaDatabase db) {
			string sql = " SELECT CD_SOLICITACAO, CD_JUSTIFICATIVA, IN_TIPO_JUSTIFICATIVA, DS_PARAMETROS " +
				" FROM EV_FORM_NEGATIVA_JUST FJ " +
				" WHERE FJ.CD_SOLICITACAO = :cdProtocolo AND IN_TIPO_JUSTIFICATIVA = :tipo";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":cdProtocolo", DbType.Int32, cdProtocolo));
			lstParam.Add(new Parametro(":tipo", DbType.StringFixedLength, tipo));

			List<FormNegativaJustificativaVO> lst = BaseDAO.ExecuteDataSet(db, sql, JustFromDataRow, lstParam);
			return lst;
		}

		private static FormNegativaVO FromDataRow(DataRow dr) {
			FormNegativaVO vo = new FormNegativaVO();

			vo.CodSolicitacao = Convert.ToInt32(dr["CD_SOLICITACAO"]);
            vo.Codint = dr.Field<string>("BA1_CODINT");
            vo.Codemp = dr.Field<string>("BA1_CODEMP");
            vo.Matric = dr.Field<string>("BA1_MATRIC");
            vo.Tipreg = dr.Field<string>("BA1_TIPREG");
			vo.DataCriacao = Convert.ToDateTime(dr["DT_CREATE"]);
			vo.DataAlteracao = Convert.ToDateTime(dr["DT_UPDATE"]);
			vo.DataFormulario = Convert.ToDateTime(dr["DT_FORMULARIO"]);
			vo.DescricaoSolicitacao = Convert.ToString(dr["DS_SOLICITACAO"]);
			vo.IdUsuario = Convert.ToInt32(dr["ID_USUARIO"]);
			vo.IdUsuarioUpdate = Convert.ToInt32(dr["ID_USUARIO_UPDATE"]);
			vo.InfoDispositivoLegal = Convert.ToInt32(dr["IN_LEGAL"]);
			vo.MotivoCancelamento = Convert.ToString(dr["DS_MOTIVO_CANCELAMENTO"]);
			vo.PadraoAcomodacao = Convert.ToString(dr["IN_ACOMODACAO"]);
			vo.PrevisaoContratual = Convert.ToString(dr["DS_PREVISAO_CONTRATUAL"]);
			vo.Status = Convert.ToString(dr["CD_STATUS"]);
			vo.TipoRede = Convert.ToString(dr["IN_TIPO_REDE"]);
			vo.ProtocoloAns = dr.Field<string>("NR_PROTOCOLO_ANS");

            vo.Prestador = new PRedeAtendimentoVO()
            {
                Codigo = dr.Field<string>("BAU_CODIGO"),
                Cpfcgc = dr.Field<string>("BAU_CPFCGC"),
                Nome = dr.Field<string>("BAU_NOME")
            };

            vo.Profissional = new PProfissionalSaudeVO()
            {
                Nome = dr.Field<string>("BB0_NOME"),
                Codigo = dr.Field<string>("BB0_CODIGO"),
                Codsig = dr.Field<string>("BB0_CODSIG"),
                Estado = dr.Field<string>("BB0_ESTADO"),
                Numcr = dr.Field<string>("BB0_NUMCR")
            };

			vo.NrContrato = dr.Field<string>("NR_CONTRATO");
			vo.DataSolicitacao = BaseDAO.GetNullableDate(dr, "DT_SOLICITACAO");

			vo.IdMotivoGlosa = BaseDAO.GetNullableInt(dr, "CD_MOTIVO_GLOSA");

			if (!dr.IsNull("NR_REANALISE_PROTOCOLO_ANS"))
				vo.Reanalise = FromDataRowReanalise(dr);

			return vo;
		}

		private static FormNegativaReanaliseVO FromDataRowReanalise(DataRow dr) {
			FormNegativaReanaliseVO vo = new FormNegativaReanaliseVO();
			vo.DataAlteracao = Convert.ToDateTime(dr["DT_REANALISE_UPDATE"]);
			vo.DataCriacao = Convert.ToDateTime(dr["DT_REANALISE_CRIACAO"]);
			vo.DataFormulario = Convert.ToDateTime(dr["DT_REANALISE_SOL"]);
			vo.IdUsuario = Convert.ToInt32(dr["ID_REANALISE_USUARIO"]);
			vo.IdUsuarioUpdate = Convert.ToInt32(dr["ID_REANALISE_USUARIO_UPDATE"]);
			vo.JustificativaNegativa = dr.Field<string>("DS_REANALISE_JUSTIFICATIVA");
			vo.ObservacaoParecer = dr.Field<string>("DS_REANALISE_PARECER");
			vo.Parecer = Convert.ToInt32(dr["IN_REANALISE_POSICIONAMENTO"]);
			vo.ProtocoloAns = Convert.ToString(dr["NR_REANALISE_PROTOCOLO_ANS"]);
			vo.Status = (FormNegativaReanaliseStatus)Convert.ToInt32(dr["CD_STATUS_REANALISE"]);
			vo.ObservacaoDevolucao = dr.Field<string>("DS_REANALISE_DEVOLUCAO");
			return vo;
		}

		private static FormNegativaItemVO ItemFromDataRow(DataRow dr) {
			FormNegativaItemVO vo = new FormNegativaItemVO();

            vo.Codpad = Convert.ToString(dr["BR8_CODPAD"]);
            vo.Codpsa = Convert.ToString(dr["BR8_CODPSA"]);
            vo.Descri = Convert.ToString(dr["BR8_DESCRI"]);
			vo.Observacao = Convert.ToString(dr["DS_OBSERVACAO"]);
			vo.Quantidade = Convert.ToInt32(dr["QT_ITEM"]);
			return vo;
		}

		private static FormNegativaJustificativaVO JustFromDataRow(DataRow dr) {
			FormNegativaJustificativaVO vo = new FormNegativaJustificativaVO();

			vo.IdJustificativa = Convert.ToInt32(dr["CD_JUSTIFICATIVA"]);
			vo.Parametros = Convert.ToString(dr["DS_PARAMETROS"]);
			return vo;
		}

		private static MotivoGlosaVO FromDataRowMotivoGlosa(DataRow dr) {
			MotivoGlosaVO vo = new MotivoGlosaVO();
			vo.Id = Convert.ToInt32(dr["CD_MOTIVO"]);
			vo.Grupo = dr.Field<string>("NM_GRUPO");
			vo.Descricao= dr.Field<string>("DS_MOTIVO");
			return vo;
		}

		internal static void Cancelar(int id, string motivo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_FORM_NEGATIVA SET CD_STATUS = :status, DS_MOTIVO_CANCELAMENTO = :motivo, " +
				" ID_USUARIO_UPDATE = :usuario, DT_UPDATE = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":status", DbType.String, FormNegativaStatus.CANCELADO));
			lstParam.Add(new Parametro(":motivo", DbType.String, motivo));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void DevolverReanalise(int id, int idUsuario, string motivo, EvidaDatabase db) {
			List<Parametro> lstParam = new List<Parametro>();

			string sql = "UPDATE EV_FORM_NEGATIVA SET DS_REANALISE_DEVOLUCAO = :motivo, CD_STATUS_REANALISE = :status, ID_REANALISE_USUARIO_UPDATE = :usuario, DT_REANALISE_UPDATE = LOCALTIMESTAMP " +
					" WHERE CD_SOLICITACAO = :id ";

			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)FormNegativaReanaliseStatus.DEVOLVIDO));
			lstParam.Add(new Parametro(":motivo", DbType.String, motivo));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static void Finalizar(int id, int idUsuario, bool reanalise, EvidaDatabase db) {
			List<Parametro> lstParam = new List<Parametro>();

			string sql = "UPDATE EV_FORM_NEGATIVA SET CD_STATUS = :status, " +
				" ID_USUARIO_UPDATE = :usuario, DT_UPDATE = LOCALTIMESTAMP where CD_SOLICITACAO = :id";
			FormNegativaStatus status = FormNegativaStatus.APROVADO;

			if (reanalise) {
				sql = "UPDATE EV_FORM_NEGATIVA SET DS_REANALISE_DEVOLUCAO = null, CD_STATUS_REANALISE = :status, ID_REANALISE_USUARIO_UPDATE = :usuario, DT_REANALISE_UPDATE = LOCALTIMESTAMP " +
					" WHERE CD_SOLICITACAO = :id ";
				lstParam.Add(new Parametro(":status", DbType.Int32, (int)FormNegativaReanaliseStatus.FINALIZADO));
			} else {
				lstParam.Add(new Parametro(":status", DbType.String, status));
			}

			lstParam.Add(new Parametro(":id", DbType.Int32, id));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}

		internal static List<string> ListarGruposMotivo(EvidaDatabase db) {
			string sql = "SELECT DISTINCT NM_GRUPO FROM EV_MOTIVO_GLOSA B ORDER BY NM_GRUPO";
			return BaseDAO.ExecuteDataSet(db, sql, delegate(DataRow dr) {
				return Convert.ToString(dr["NM_GRUPO"]);
			});
		}

		internal static MotivoGlosaVO GetMotivoById(int idMotivo, EvidaDatabase db) {
			string sql = "SELECT * FROM EV_MOTIVO_GLOSA B WHERE CD_MOTIVO = :id";
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, idMotivo));
			return BaseDAO.ExecuteDataRow(db, sql, FromDataRowMotivoGlosa, lstParam);
		}

		internal static List<MotivoGlosaVO> BuscarMotivosGlosa(int? codigo, string grupo, string descricao, EvidaDatabase db) {
			string sql = "SELECT * " +
				"	FROM EV_MOTIVO_GLOSA B WHERE 1 = 1 ";

			List<Parametro> lstParam = new List<Parametro>();
			if (codigo != null) {
				lstParam.Add(new Parametro(":id", DbType.Int32, codigo));
				sql += " AND CD_MOTIVO = :id ";
			}
			if (!string.IsNullOrEmpty(grupo)) {
				lstParam.Add(new Parametro(":grupo", DbType.String, grupo));
				sql += " AND NM_GRUPO = :grupo ";
			}
			if (!string.IsNullOrEmpty(descricao)) {
				lstParam.Add(new Parametro(":descricao", DbType.String, "%" + descricao.ToUpper() + "%"));
                sql += " AND upper(trim(DS_MOTIVO)) LIKE upper(trim(:descricao)) ";
			}

			sql += " ORDER BY DS_MOTIVO";

			List<MotivoGlosaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowMotivoGlosa, lstParam);
			return lst;
		}

		internal static string GetNroContrato(VO.Protheus.PUsuarioVO usuarioVO, EvidaDatabase db) {

            string sql = "SELECT FN.BA1_CODINT, FN.BA1_CODEMP, FN.BA1_MATRIC, FN.BA1_TIPREG, NR_CONTRATO FROM EV_FORM_NEGATIVA FN " +
                "	WHERE (trim(FN.BA1_CODINT), trim(FN.BA1_CODEMP), trim(FN.BA1_MATRIC), trim(FN.BA1_TIPREG), FN.DT_FORMULARIO) = (SELECT trim(FN2.BA1_CODINT), trim(FN2.BA1_CODEMP), trim(FN2.BA1_MATRIC), trim(FN2.BA1_TIPREG), MAX(DT_FORMULARIO) " +
                "			FROM EV_FORM_NEGATIVA FN2 WHERE trim(FN2.BA1_CODINT) = trim(FN.BA1_CODINT) AND trim(FN2.BA1_CODEMP) = trim(FN.BA1_CODEMP) AND trim(FN2.BA1_MATRIC) = trim(FN.BA1_MATRIC) AND trim(FN2.BA1_TIPREG) = trim(FN.BA1_TIPREG) AND FN2.NR_CONTRATO IS NOT NULL " +
                "			GROUP BY trim(FN2.BA1_CODINT), trim(FN2.BA1_CODEMP), trim(FN2.BA1_MATRIC), trim(FN2.BA1_TIPREG)) " +
                "	AND trim(FN.BA1_CODINT) = trim(:codint) AND trim(FN.BA1_CODEMP) = trim(:codemp) AND trim(FN.BA1_MATRIC) = trim(:matric) AND trim(FN.BA1_TIPREG) = trim(:tipreg) ";

			List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":codint", DbType.String, usuarioVO.Codint));
            lstParam.Add(new Parametro(":codemp", DbType.String, usuarioVO.Codemp));
            lstParam.Add(new Parametro(":matric", DbType.String, usuarioVO.Matric));
            lstParam.Add(new Parametro(":tipreg", DbType.String, usuarioVO.Tipreg));

			return BaseDAO.ExecuteDataRow(db, sql, delegate(DataRow dr) {
				return Convert.ToString(dr["NR_CONTRATO"]);
			}, lstParam);
		}

	}
}
