using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO
{
    internal class SolicitacaoSegViaDAO
    {
        private static int NextId(EvidaDatabase db)
        {
            string sql = "SELECT EVIDA.SQ_EV_SOLICITACAO_SVIA.nextval FROM DUAL";
            decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
            return (int)idSolicitacao;
        }

        internal static DataTable ListarDadosSegViaCarteira(PUsuarioVO beneficiario, EvidaDatabase db)
        {
              string sql = "SELECT benef.BA1_CODINT, benef.BA1_CODEMP, benef.BA1_MATRIC, benef.BA1_TIPREG, benef.BA1_MATEMP, trim(benef.BA1_CODINT) || '|' || trim(benef.BA1_CODEMP) || '|' || trim(benef.BA1_MATRIC) || '|' || trim(benef.BA1_TIPREG) as cd_beneficiario, func.BA1_NOMUSR nm_funcionario, benef.BA1_MATVID, benef.BA1_NOMUSR nm_beneficiario, " +
              " parentesco.BRP_CODIGO, parentesco.BRP_DESCRI, p.BI3_DESCRI, benef.BA1_MATANT, benef.BA1_DTVLCR " +
              " FROM VW_PR_USUARIO benef," +
              " VW_PR_FAMILIA fam, VW_PR_GRAU_PARENTESCO parentesco, " +
              " VW_PR_PRODUTO_SAUDE p, VW_PR_USUARIO func " +
              " WHERE trim(benef.BA1_CODINT) = trim(:codint) AND trim(benef.BA1_CODEMP) = trim(:codemp) AND trim(benef.BA1_MATRIC) = trim(:matric) " +
              " and trim(benef.BA1_CODINT) = trim(fam.BA3_CODINT) AND trim(benef.BA1_CODEMP) = trim(fam.BA3_CODEMP) AND trim(benef.BA1_MATRIC) = trim(fam.BA3_MATRIC) " +
              " and trim(p.BI3_CODIGO) = trim(fam.BA3_CODPLA) " +
              " and parentesco.BRP_CODIGO (+) = benef.BA1_GRAUPA " +
              " and (to_date(trim(benef.BA1_DATCAR), 'yyyymmdd') <= sysdate and (to_date(trim(benef.BA1_DATBLO), 'yyyymmdd') > sysdate or benef.BA1_DATBLO = '        ' or benef.BA1_DATBLO is null)) " +
              " and trim(benef.BA1_CODINT) = trim(func.BA1_CODINT) and trim(benef.BA1_CODEMP) = trim(func.BA1_CODEMP) and trim(benef.BA1_MATRIC) = trim(func.BA1_MATRIC) and trim(func.BA1_TIPUSU) = 'T' " +
              " ORDER BY trim(benef.BA1_MATVID)";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = beneficiario.Codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = beneficiario.Codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = beneficiario.Matric });

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static void Salvar(SolicitacaoSegViaCarteiraVO vo, List<SolicitacaoSegViaCarteiraBenefVO> lst, EvidaDatabase evdb)
        {
            string sql = "INSERT INTO EV_SOLICITACAO_SVIA (CD_SOLICITACAO, BA3_CODINT, BA3_CODEMP, BA3_MATRIC, DS_LOCAL, DT_CRIACAO, TP_STATUS, DT_ALTERACAO, CD_USUARIO_ALTERACAO, NR_PROTOCOLO_ANS) " +
                " VALUES (:id, :codint, :codemp, :matric, :local, LOCALTIMESTAMP, :status, null, null, PKG_PROTOCOLO_ANS.F_NEXT_VALUE())";

            vo.CdSolicitacao = NextId(evdb);

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CdSolicitacao);
            db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":local", DbType.String, vo.Local);
            db.AddInParameter(dbCommand, ":status", DbType.StringFixedLength, vo.Status);

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);

            sql = "INSERT INTO EV_SOLICITACAO_SVIA_BENEF (CD_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, TP_MOTIVO) VALUES (:id, :codint, :codemp, :matric, :tipreg, :motivo) ";    

            dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CdSolicitacao);
            db.AddInParameter(dbCommand, ":codint", DbType.String);
            db.AddInParameter(dbCommand, ":codemp", DbType.String);
            db.AddInParameter(dbCommand, ":matric", DbType.String);
            db.AddInParameter(dbCommand, ":tipreg", DbType.String);
            db.AddInParameter(dbCommand, ":motivo", DbType.StringFixedLength);

            foreach (SolicitacaoSegViaCarteiraBenefVO bVO in lst)
            {
                db.SetParameterValue(dbCommand, ":codint", bVO.Codint.Trim());
                db.SetParameterValue(dbCommand, ":codemp", bVO.Codemp.Trim());
                db.SetParameterValue(dbCommand, ":matric", bVO.Matric.Trim());
                db.SetParameterValue(dbCommand, ":tipreg", bVO.Tipreg.Trim());
                db.SetParameterValue(dbCommand, ":motivo", bVO.Motivo);
                BaseDAO.ExecuteNonQuery(dbCommand, evdb);
            }

        }

        internal static SolicitacaoSegViaCarteiraVO GetById(int id, EvidaDatabase db)
        {
            string sql = "SELECT * " +
                " FROM  EVIDA.EV_SOLICITACAO_SVIA " +
                " WHERE CD_SOLICITACAO = :id";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

            List<SolicitacaoSegViaCarteiraVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        private static SolicitacaoSegViaCarteiraVO FromDataRow(DataRow dr)
        {
            SolicitacaoSegViaCarteiraVO vo = new SolicitacaoSegViaCarteiraVO();
            vo.CdSolicitacao = Convert.ToInt32(dr["CD_SOLICITACAO"]);
            vo.Codint = Convert.ToString(dr["BA3_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA3_CODEMP"]);
            vo.Matric = Convert.ToString(dr["BA3_MATRIC"]);
            vo.Alteracao = dr["DT_ALTERACAO"] != DBNull.Value ? Convert.ToDateTime(dr["DT_ALTERACAO"]) : new DateTime?();
            vo.Criacao = Convert.ToDateTime(dr["DT_CRIACAO"]);
            vo.Local = Convert.ToString(dr["DS_LOCAL"]);
            vo.Status = Convert.ToChar(dr["TP_STATUS"]);
            vo.UsuarioAlteracao = dr["CD_USUARIO_ALTERACAO"] != DBNull.Value ? Convert.ToInt32(dr["CD_USUARIO_ALTERACAO"]) : new int?();
            vo.MotivoCancelamento = dr.Field<string>("DS_MOTIVO_CANCELAMENTO");
            vo.ProtocoloAns = dr.Field<string>("NR_PROTOCOLO_ANS");
            return vo;
        }

        internal static DataTable BuscarBeneficiarios(int id, EvidaDatabase db)
        {
            string sql = " SELECT benef.BA1_CODINT, benef.BA1_CODEMP, benef.BA1_MATRIC, benef.BA1_TIPREG, func.BA1_NOMUSR nm_funcionario, benef.BA1_CODINT || '|' || benef.BA1_CODEMP || '|' || benef.BA1_MATRIC || '|' || benef.BA1_TIPREG as cd_beneficiario, benef.BA1_MATVID, benef.BA1_NOMUSR nm_beneficiario, " +
                "		parentesco.BRP_CODIGO, parentesco.BRP_DESCRI, p.BI3_DESCRI, benef.BA1_MATANT, benef.BA1_DTVLCR, " +
                "		sol.cd_solicitacao, sol.dt_criacao, solben.tp_motivo " +
                " FROM EV_SOLICITACAO_SVIA sol, EV_SOLICITACAO_SVIA_BENEF solben, VW_PR_USUARIO benef," +
                "		VW_PR_FAMILIA fam, VW_PR_GRAU_PARENTESCO parentesco, " +
                "		VW_PR_PRODUTO_SAUDE p, VW_PR_USUARIO func " +
                "	WHERE sol.cd_solicitacao = solben.cd_solicitacao " +
                "		AND trim(sol.BA3_CODINT) = trim(func.BA1_CODINT) and trim(sol.BA3_CODEMP) = trim(func.BA1_CODEMP) and trim(sol.BA3_MATRIC) = trim(func.BA1_MATRIC) and trim(func.BA1_TIPUSU) = 'T' " +
                "		AND trim(solben.BA1_CODINT) = trim(benef.BA1_CODINT) AND trim(solben.BA1_CODEMP) = trim(benef.BA1_CODEMP) AND trim(solben.BA1_MATRIC) = trim(benef.BA1_MATRIC) AND trim(solben.BA1_TIPREG) = trim(benef.BA1_TIPREG) " +
                "		and trim(benef.BA1_CODINT) = trim(fam.BA3_CODINT) AND trim(benef.BA1_CODEMP) = trim(fam.BA3_CODEMP) AND trim(benef.BA1_MATRIC) = trim(fam.BA3_MATRIC) " +
                "		and trim(p.BI3_CODIGO) = trim(fam.BA3_CODPLA) " +
                "		and parentesco.BRP_CODIGO (+) = benef.BA1_GRAUPA " +
                "		and (to_date(trim(benef.BA1_DATCAR), 'yyyymmdd') <= sol.dt_criacao and (to_date(trim(benef.BA1_DATBLO), 'yyyymmdd') > sol.dt_criacao or benef.BA1_DATBLO = '        ' or benef.BA1_DATBLO is null)) " +
                "		and trim(benef.BA1_CODINT) = trim(func.BA1_CODINT) and trim(benef.BA1_CODEMP) = trim(func.BA1_CODEMP) and trim(benef.BA1_MATRIC) = trim(func.BA1_MATRIC) " +
                "		and sol.cd_solicitacao = :id " +
                " ORDER BY benef.BA1_MATVID"; 

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarSolicitacoes(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "SELECT sol.* " +
                " FROM EV_SOLICITACAO_SVIA sol " +
                " WHERE trim(sol.BA3_CODINT) = trim(:codint) AND trim(sol.BA3_CODEMP) = trim(:codemp) AND trim(sol.BA3_MATRIC) = trim(:matric) " +
                " ORDER BY cd_solicitacao DESC ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable BuscarBeneficiariosPendentes(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "SELECT SOL.cd_solicitacao, solben.BA1_CODINT, solben.BA1_CODEMP, solben.BA1_MATRIC, solben.BA1_TIPREG, trim(solben.BA1_CODINT) || '|' || trim(solben.BA1_CODEMP) || '|' || trim(solben.BA1_MATRIC) || '|' || trim(solben.BA1_TIPREG) as cd_beneficiario, " +
            "		func.BA1_NOMUSR, dep.BTS_NOMUSR, nvl(dep.BTS_NOMUSR, func.BA1_NOMUSR) nm_beneficiario " +
            " FROM EV_SOLICITACAO_SVIA sol, EV_SOLICITACAO_SVIA_BENEF solben, " +
            "		VW_PR_USUARIO benef, VW_PR_VIDA dep, VW_PR_USUARIO func " +
            " WHERE trim(sol.BA3_CODINT) = trim(:codint) AND trim(sol.BA3_CODEMP) = trim(:codemp) AND trim(sol.BA3_MATRIC) = trim(:matric) " +
            "		AND trim(sol.BA3_CODINT) = trim(func.BA1_CODINT) and trim(sol.BA3_CODEMP) = trim(func.BA1_CODEMP) and trim(sol.BA3_MATRIC) = trim(func.BA1_MATRIC) and trim(func.BA1_TIPUSU) = 'T' " +
            "		AND sol.cd_solicitacao = solben.cd_solicitacao " +
            "		AND trim(solben.BA1_CODINT) = trim(benef.BA1_CODINT) AND trim(solben.BA1_CODEMP) = trim(benef.BA1_CODEMP) AND trim(solben.BA1_MATRIC) = trim(benef.BA1_MATRIC) AND trim(solben.BA1_TIPREG) = trim(benef.BA1_TIPREG) " +
            "		and benef.BA1_MATVID = dep.BTS_MATVID (+) " +
            "		and trim(benef.BA1_CODINT) = trim(func.BA1_CODINT) and trim(benef.BA1_CODEMP) = trim(func.BA1_CODEMP) and trim(benef.BA1_MATRIC) = trim(func.BA1_MATRIC) " +
            "		AND sol.TP_STATUS = :status " +
            " ORDER BY cd_solicitacao DESC ";  

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });
            lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.StringFixedLength, Value = (char)StatusSegVia.PENDENTE });

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static DataTable PesquisarSegViaCarteira(int? cdProtocolo, string cdFuncionario, string protocoloAns, DateTime? dtInicial, DateTime? dtFinal, StatusSegVia? status, EvidaDatabase db)
        {
            string sql = "SELECT SOL.cd_solicitacao, SOL.BA3_CODINT, SOL.BA3_CODEMP, SOL.BA3_MATRIC, func.BA1_MATEMP, SOL.dt_criacao, SOL.tp_status, func.BA1_NOMUSR, SOL.DT_ALTERACAO, SOL.CD_USUARIO_ALTERACAO, " +
                " SOL.DS_MOTIVO_CANCELAMENTO, SOL.NR_PROTOCOLO_ANS " +
                " FROM EV_SOLICITACAO_SVIA sol, VW_PR_USUARIO func " +
                " WHERE trim(sol.BA3_CODINT) = trim(func.BA1_CODINT) and trim(sol.BA3_CODEMP) = trim(func.BA1_CODEMP) and trim(sol.BA3_MATRIC) = trim(func.BA1_MATRIC) and trim(func.BA1_TIPUSU) = 'T' ";

            List<Parametro> lstParams = new List<Parametro>();
            if (cdProtocolo != null)
            {
                lstParams.Add(new Parametro() { Name = ":cdProtocolo", Tipo = DbType.Int32, Value = cdProtocolo.Value });
                sql += " AND sol.cd_solicitacao = :cdProtocolo ";
            }
            if (!string.IsNullOrEmpty(cdFuncionario))
            {
                lstParams.Add(new Parametro() { Name = ":cdFuncionario", Tipo = DbType.String, Value = cdFuncionario });
                sql += " AND trim(func.BA1_MATEMP) = trim(:cdFuncionario) ";
            }
            if (status != null)
            {
                lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.StringFixedLength, Value = (char)status.Value });
                sql += " AND sol.TP_STATUS = :status ";
            }
            if (dtInicial != null)
            {
                lstParams.Add(new Parametro() { Name = ":dtInicial", Tipo = DbType.Date, Value = dtInicial.Value });
                sql += " AND sol.DT_CRIACAO >= :dtInicial ";
            }
            if (dtFinal != null)
            {
                lstParams.Add(new Parametro() { Name = ":dtFinal", Tipo = DbType.Date, Value = dtFinal.Value });
                sql += " AND sol.DT_CRIACAO <= :dtFinal ";
            }
            if (!string.IsNullOrEmpty(protocoloAns))
            {
                lstParams.Add(new Parametro(":protocoloAns", DbType.String, protocoloAns));
                sql += " AND sol.NR_PROTOCOLO_ANS = :protocoloAns";
            }
            sql += " ORDER BY cd_solicitacao DESC ";    
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static void Finalizar(int cdProtocolo, int cdUsuario, EvidaDatabase db)
        {
            string sql = "UPDATE EVIDA.EV_SOLICITACAO_SVIA SET TP_STATUS = :status, DT_ALTERACAO = LOCALTIMESTAMP, CD_USUARIO_ALTERACAO = :idUsuario " +
                " WHERE CD_SOLICITACAO = :id ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
            lstParams.Add(new Parametro(":idUsuario", DbType.Int32, cdUsuario));
            lstParams.Add(new Parametro(":status", DbType.StringFixedLength, (char)StatusSegVia.FINALIZADO));

            BaseDAO.ExecuteNonQuery(sql, lstParams, db);
        }

        internal static void Cancelar(int cdProtocolo, string motivo, int cdUsuario, EvidaDatabase db)
        {
            string sql = "UPDATE EVIDA.EV_SOLICITACAO_SVIA SET TP_STATUS = :status, DT_ALTERACAO = LOCALTIMESTAMP, CD_USUARIO_ALTERACAO = :idUsuario, " +
                "	DS_MOTIVO_CANCELAMENTO = :motivo " +
                " WHERE CD_SOLICITACAO = :id ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
            lstParams.Add(new Parametro(":idUsuario", DbType.Int32, cdUsuario));
            lstParams.Add(new Parametro(":motivo", DbType.String, motivo));
            lstParams.Add(new Parametro(":status", DbType.StringFixedLength, (char)StatusSegVia.CANCELADO));

            BaseDAO.ExecuteNonQuery(sql, lstParams, db);
        }

        #region[ARQUIVOS]

        internal static void CriarArquivos(int idSegViaCarteira, List<SolicitacaoSegViaCarteiraArquivoVO> lstNewFiles, EvidaDatabase evdb)
        {
            Database db = evdb.Database;
            DbCommand dbCommand = CreateInsertArquivo(db);

            db.AddInParameter(dbCommand, ":id", DbType.Int32, idSegViaCarteira);
            db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32);
            db.AddInParameter(dbCommand, ":tipo", DbType.Int32);
            db.AddInParameter(dbCommand, ":nome", DbType.String);

            int idNextArquivo = GetNextArquivoId(idSegViaCarteira, evdb);

            foreach (SolicitacaoSegViaCarteiraArquivoVO arq in lstNewFiles)
            {
                arq.IdArquivo = idNextArquivo++;

                db.SetParameterValue(dbCommand, ":idArquivo", arq.IdArquivo);
                db.SetParameterValue(dbCommand, ":tipo", (int)arq.TipoArquivo);
                db.SetParameterValue(dbCommand, ":nome", arq.NomeArquivo);

                BaseDAO.ExecuteNonQuery(dbCommand, evdb);
            }

        }

        private static int GetNextArquivoId(int idSegViaCarteira, EvidaDatabase evdb)
        {
            string sql = "SELECT NVL(MAX(CD_ARQUIVO),0)+1 FROM EVIDA.EV_SOLICITACAO_SVIA_ARQUIVO WHERE CD_SOLICITACAO = :idSegVia";
            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idSegVia", DbType.Int32, idSegViaCarteira));
            decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, sql, lstParam);

            return (int)idSolicitacao;
        }

        private static DbCommand CreateInsertArquivo(Database db)
        {
            string sql = "INSERT INTO EVIDA.EV_SOLICITACAO_SVIA_ARQUIVO (CD_SOLICITACAO, CD_ARQUIVO, TP_ARQUIVO, NM_ARQUIVO, DT_ENVIO) " +
                "	VALUES (:id, :idArquivo, :tipo, :nome, LOCALTIMESTAMP) ";

            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            return dbCommand;
        }

        internal static List<SolicitacaoSegViaCarteiraArquivoVO> ListarArquivos(int idSegViaCarteira, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EVIDA.EV_SOLICITACAO_SVIA_ARQUIVO" +
                " WHERE CD_SOLICITACAO = :id ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idSegViaCarteira));

            List<SolicitacaoSegViaCarteiraArquivoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParam);
            return lst;
        }

        private static SolicitacaoSegViaCarteiraArquivoVO FromDataRowArquivo(DataRow dr)
        {
            SolicitacaoSegViaCarteiraArquivoVO vo = new SolicitacaoSegViaCarteiraArquivoVO();
            vo.IdArquivo = Convert.ToInt32(dr["CD_ARQUIVO"]);
            vo.IdSegViaCarteira = Convert.ToInt32(dr["CD_SOLICITACAO"]);
            vo.DataEnvio = Convert.ToDateTime(dr["dt_envio"]);
            vo.NomeArquivo = Convert.ToString(dr["nm_arquivo"]);
            vo.TipoArquivo = (TipoArquivoSolicitacaoSegViaCarteira)Convert.ToInt32(dr["tp_arquivo"]);
            return vo;
        }

        internal static void ExcluirArquivo(SolicitacaoSegViaCarteiraArquivoVO vo, EvidaDatabase evdb)
        {
            string sql = "DELETE FROM EVIDA.EV_SOLICITACAO_SVIA_ARQUIVO " +
                "	WHERE CD_SOLICITACAO = :idSegViaCarteira AND CD_ARQUIVO = :idArquivo";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idSegViaCarteira", DbType.Int32, vo.IdSegViaCarteira));
            lstParam.Add(new Parametro(":idArquivo", DbType.Int32, vo.IdArquivo));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        #endregion
    }
}
