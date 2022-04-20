using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class ControleEmailDAO {

		private static int NextId(EvidaDatabase db) {
			string sql = ("SELECT SQ_EV_CONTROLE_EMAIL.nextval FROM DUAL");

			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);

			return (int)idSolicitacao;
		}

		internal static ControleEmailVO GetById(int codEmail, DbTransaction transaction, Database db) {
			string sql = "SELECT * FROM EV_CONTROLE_EMAIL WHERE CD_CONTROLE = :id ";
			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = codEmail });

			List<ControleEmailVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams, transaction);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static ControleEmailVO FromDataRow(DataRow dr) {
			ControleEmailVO vo = new ControleEmailVO();
			vo.Id = BaseDAO.GetNullableInt(dr, "CD_CONTROLE").Value;
			vo.Tipo = (TipoControleEmail)BaseDAO.GetNullableInt(dr, "TP_EMAIL").Value;
						
			vo.Conteudo = Convert.ToString(dr["DS_CONTEUDO_EMAIL"]);
			vo.Titulo = Convert.ToString(dr["DS_TITULO_EMAIL"]);
			vo.DataCriacao = BaseDAO.GetNullableDate(dr, "DT_CRIACAO").Value;

			vo.DataEnvio = BaseDAO.GetNullableDate(dr, "DT_ENVIO");
			vo.DataAgendamento = BaseDAO.GetNullableDate(dr, "DT_AGENDAMENTO").Value;
			vo.QtdTentativa = BaseDAO.GetNullableInt(dr, "QT_TENTATIVA").Value;

			vo.Situacao = (StatusControleEmail)BaseDAO.GetNullableInt(dr, "CD_STATUS").Value;
			vo.Erro = Convert.ToString(dr["DS_ERRO"]);

			List<string> lstAnexos = FormatUtil.StringToList(Convert.ToString(dr["DS_ARQUIVO_ANEXO"]));
			vo.Anexos = new List<string>();
			string rootDir = System.IO.Path.GetFullPath(ParametroUtil.FileRepository).ToLower();
			foreach (string anexo in lstAnexos) {
				string newAnexo = anexo;
				if (newAnexo.StartsWith("$$ROOT$$")) {
					newAnexo = newAnexo.Replace("$$ROOT$$", rootDir);
				}
				vo.Anexos.Add(newAnexo);
			}

			vo.Referencia = FormatUtil.StringToList(Convert.ToString(dr["DS_REFERENCIA"]));
			vo.Destinatarios = ParseEmail(dr);

			string sender = Convert.ToString(dr["DS_SENDER"]);
			if (!string.IsNullOrEmpty(sender)) {
				string[] senderArr = sender.Split(new char[] { ';' });
				string mSender = senderArr[0];
				string nSender = "";
				if (senderArr.Length > 1)
					nSender = senderArr[1];
				vo.Sender = new KeyValuePair<string, string>(mSender, nSender);
			} else {
				vo.Sender = new KeyValuePair<string, string>();
			}

			return vo;
		}

		internal static void Criar(ControleEmailVO vo, EvidaDatabase evdb) {

			vo.Id = NextId(evdb);
			string sql = "INSERT INTO EV_CONTROLE_EMAIL(CD_CONTROLE, TP_EMAIL, DS_REFERENCIA, DS_SENDER, DS_DESTINATARIO_EMAIL, DS_DESTINATARIO_NOME, DS_CONTEUDO_EMAIL, " +
				"	DS_ARQUIVO_ANEXO, DS_TITULO_EMAIL, DT_CRIACAO, DT_AGENDAMENTO, QT_TENTATIVA, CD_STATUS) " +
				" VALUES (:id, :tipo, :referencia, :sender, :email, :nome, :conteudo, :anexo, :titulo, LOCALTIMESTAMP, :agendamento, 0, :status)";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":tipo", DbType.Int32, (int)vo.Tipo);
			db.AddInParameter(dbCommand, ":referencia", DbType.String, FormatUtil.ListToString(vo.Referencia));
			db.AddInParameter(dbCommand, ":conteudo", DbType.AnsiString, vo.Conteudo);
			db.AddInParameter(dbCommand, ":titulo", DbType.String, vo.Titulo);
			db.AddInParameter(dbCommand, ":sender", DbType.String, vo.Sender.Key + ";" + vo.Sender.Value);

			List<string> lstAnexos = new List<string>();
			if (vo.Anexos != null) {
				string rootDir = System.IO.Path.GetFullPath(ParametroUtil.FileRepository).ToLower();
				foreach (string anexo in vo.Anexos) {
					string newAnexo = anexo;
					if (newAnexo.StartsWith(rootDir)) {
						newAnexo = newAnexo.Replace(rootDir, "$$ROOT$$");
					}
					lstAnexos.Add(newAnexo);
				}
			}
			db.AddInParameter(dbCommand, ":anexo", DbType.AnsiString, FormatUtil.ListToString(lstAnexos));

			db.AddInParameter(dbCommand, ":agendamento", DbType.DateTime, vo.DataAgendamento);
			db.AddInParameter(dbCommand, ":status", DbType.AnsiString, (int)StatusControleEmail.PENDENTE);

			List<string> email = vo.Destinatarios.Select(x => x.Key).ToList();
			List<string> nome = vo.Destinatarios.Select(x => x.Value).ToList();

			db.AddInParameter(dbCommand, ":email", DbType.String, FormatUtil.ListToString(email));
			db.AddInParameter(dbCommand, ":nome", DbType.String, FormatUtil.ListToString(nome));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static bool MarcarErro(ControleEmailVO vo, Exception erro, EvidaDatabase evdb) {
			string sql = "UPDATE EV_CONTROLE_EMAIL SET CD_STATUS = :status, DT_ENVIO = LOCALTIMESTAMP, QT_TENTATIVA = QT_TENTATIVA+1, " +
				" DT_AGENDAMENTO = :agendamento, DS_ERRO = :erro " +
				" WHERE CD_CONTROLE = :id ";

			vo.QtdTentativa++;
			vo.Erro = erro.ToString();
			if (vo.QtdTentativa >= 10) {
				vo.Situacao = StatusControleEmail.ERRO;
			} else {
				vo.DataAgendamento = vo.DataAgendamento.AddHours(1);
				vo.Situacao = StatusControleEmail.RETENTAR;
			}
			Database db = evdb.Database;

			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)vo.Situacao);
			db.AddInParameter(dbCommand, ":agendamento", DbType.DateTime, vo.DataAgendamento);
			db.AddInParameter(dbCommand, ":erro", DbType.AnsiString, vo.Erro);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			return vo.Situacao == StatusControleEmail.ERRO;
		}

		internal static void Finalizar(ControleEmailVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE EV_CONTROLE_EMAIL SET CD_STATUS = :status, DT_ENVIO = LOCALTIMESTAMP, DS_ERRO = NULL " +
				" WHERE CD_CONTROLE = :id ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Id);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)StatusControleEmail.ENVIADO);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static List<KeyValuePair<string, string>> ParseEmail(DataRow dr) {
			List<string> lstEmail = FormatUtil.StringToList(Convert.ToString(dr["DS_DESTINATARIO_EMAIL"]));
			List<string> lstNomes = FormatUtil.StringToList(Convert.ToString(dr["DS_DESTINATARIO_NOME"]));
			List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
			for (int i = 0; i < lstEmail.Count; ++i) {
				string key = lstEmail[i];
				string nome = key;
				if (i < lstNomes.Count) {
					nome = lstNomes[i];
				}
				KeyValuePair<string, string> email = new KeyValuePair<string, string>(key, nome);
				lst.Add(email);
			}

			return lst;
		}

		internal static List<ControleEmailVO> ListarSolPendente(int maxRecords, DbTransaction transaction, Database db) {
			string sql = "SELECT * FROM EV_CONTROLE_EMAIL WHERE CD_STATUS IN (:status1,:status2) AND DT_AGENDAMENTO < SYSDATE AND ROWNUM <= :maxRecords order by DT_AGENDAMENTO ";
			List<Parametro> lstParams = new List<Parametro>();

			lstParams.Add(new Parametro() { Name = ":status1", Tipo = DbType.Int32, Value = (int)StatusControleEmail.RETENTAR });
			lstParams.Add(new Parametro() { Name = ":status2", Tipo = DbType.Int32, Value = (int)StatusControleEmail.PENDENTE });
			lstParams.Add(new Parametro() { Name = ":maxRecords", Tipo = DbType.Int32, Value = maxRecords });

			List<ControleEmailVO> lst = BaseDAO.ExecuteDataSet(maxRecords, db, sql, FromDataRow, lstParams, transaction);
			return lst;
		}

	}
}
