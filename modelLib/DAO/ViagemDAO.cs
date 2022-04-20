using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO
{
    internal class ViagemDAO
    {
        private static int NextId(EvidaDatabase evdb)
        {
            string sql = "SELECT SQ_EV_SOL_VIAGEM.nextval FROM DUAL";
            decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, sql);
            return (int)idSolicitacao;
        }

        internal static DataTable Pesquisar(string matricula, int? id, StatusSolicitacaoViagem? status, int? usuarioCriacao, EvidaDatabase db)
        {
            string sql = "SELECT CD_VIAGEM, DECODE( VG.Fl_Externo, 'S', 'EXTERNO', 'INTERNO') ISEXTERNO, DT_SOLICITACAO, VG.ID_SITUACAO, VG.CD_MAT_EMPREGADO,  nvl(emp.ZF1_NOME, VG.NM_VIAJANTE) NM_EMPREGADO " +
                " FROM EV_SOL_VIAGEM VG, VW_PR_FUNCIONARIO EMP " +
                " WHERE EMP.ZF1_MAT (+)= TO_CHAR(VG.CD_MAT_EMPREGADO,'FM000000') ";

            List<Parametro> lstParams = new List<Parametro>();
            if (!string.IsNullOrEmpty(matricula))
            {
                sql += " AND VG.CD_MAT_EMPREGADO = :matricula ";
                lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.String, Value = matricula });
            }
            if (id != null)
            {
                sql += " AND CD_VIAGEM = :id ";
                lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id.Value });
            }
            if (status != null)
            {
                sql += " AND ID_SITUACAO = :status ";
                lstParams.Add(new Parametro() { Name = ":status", Tipo = DbType.Int32, Value = (int)status.Value });
            }

            if (usuarioCriacao != null)
            {
                sql += " AND CD_USUARIO_CRIACAO = :usuarioCriacao ";
                lstParams.Add(new Parametro() { Name = ":usuarioCriacao", Tipo = DbType.Int32, Value = usuarioCriacao.Value });
            }

            sql += " ORDER BY CD_VIAGEM DESC ";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);

            return dt;
        }

        internal static SolicitacaoViagemVO GetById(int id, EvidaDatabase evdb)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM A" +
                "		WHERE a.cd_viagem = :id ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, id));

            List<SolicitacaoViagemVO> lst = BaseDAO.ExecuteDataSet(evdb, sql, FromDataRow, lstParam);
            if (lst != null && lst.Count > 0)
            {
                SolicitacaoViagemVO vo = lst[0];
                List<SolicitacaoViagemItinerarioVO> lstItinerarios = ListarItinerarios(vo.Id, evdb);
                if (lstItinerarios != null)
                {
                    vo.Compra.Passagens = lstItinerarios.FindAll(x => x.TipoRegistro == TipoItinerarioSolicitacaoViagem.PASSAGEM);
                    vo.Compra.Hoteis = lstItinerarios.FindAll(x => x.TipoRegistro == TipoItinerarioSolicitacaoViagem.HOTEL);

                    vo.Solicitacao.Itinerarios = lstItinerarios.FindAll(x => x.TipoRegistro == TipoItinerarioSolicitacaoViagem.ITINERARIO);

                    vo.PrestacaoConta.DespesasDetalhadas = ListarDespesasDetalhadas(vo.Id, evdb);
                }
                else
                {
                    vo.Compra.Passagens = new List<SolicitacaoViagemItinerarioVO>();
                    vo.Compra.Hoteis = new List<SolicitacaoViagemItinerarioVO>();
                    vo.Solicitacao.Itinerarios = new List<SolicitacaoViagemItinerarioVO>();
                }
                return vo;
            }
            return null;
        }

        internal static List<SolicitacaoViagemItinerarioVO> ListarItinerarios(int idViagem, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_ITINERARIO A" +
                "		WHERE a.cd_viagem = :id ORDER BY DT_PARTIDA ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idViagem));
            List<SolicitacaoViagemItinerarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowItinerario, lstParam);
            return lst;
        }

        internal static SolicitacaoViagemItinerarioVO GetItinerarioById(int idViagem, TipoItinerarioSolicitacaoViagem tipo, int id, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_ITINERARIO A" +
                "		WHERE a.cd_viagem = :idViagem AND TP_ITINERARIO = :tipo AND ID_ITINERARIO = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, (int)tipo));
            lstParam.Add(new Parametro(":id", DbType.Int32, id));
            return BaseDAO.ExecuteDataRow(db, sql, FromDataRowItinerario, lstParam);
        }

        internal static List<SolicitacaoViagemItinerarioVO> ListarItinerarios(int idViagem, TipoItinerarioSolicitacaoViagem tipo, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_ITINERARIO A" +
                "		WHERE a.cd_viagem = :id AND TP_ITINERARIO = :tipo";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, (int)tipo));
            List<SolicitacaoViagemItinerarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowItinerario, lstParam);
            return lst;
        }

        internal static List<SolicitacaoViagemDespesaDetalhadaVO> ListarDespesasDetalhadas(int idViagem, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_DESP_DET A" +
                "		WHERE a.cd_viagem = :id ORDER BY DT_DESPESA, CD_GRUPO_DESPESA, CD_TIPO_DESPESA, DS_TIPO_DESPESA, VL_DESPESA ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idViagem));
            List<SolicitacaoViagemDespesaDetalhadaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowDespDet, lstParam);
            return lst;
        }

        private static SolicitacaoViagemVO FromDataRow(DataRow dr)
        {
            SolicitacaoViagemVO vo = new SolicitacaoViagemVO();
            vo.Id = Convert.ToInt32(dr["CD_VIAGEM"]);
            vo.DataCriacao = Convert.ToDateTime(dr["DT_SOLICITACAO"]);

            vo.IsExterno = BaseDAO.GetNullableBoolean(dr, "FL_EXTERNO", "S").Value;

            if (!vo.IsExterno)
            {
                vo.Empregado = new VO.Protheus.EmpregadoEvidaVO();
                vo.Empregado.Matricula = dr.IsNull("CD_MAT_EMPREGADO") ? 0 : Convert.ToInt64(dr["CD_MAT_EMPREGADO"]);

                if (vo.Empregado.Matricula != 0)
                {
                    vo.Empregado = BO.EmpregadoEvidaBO.Instance.GetByMatricula(vo.Empregado.Matricula);
                }

            }

            vo.Solicitacao = new SolicitacaoViagemInfoSolicitacaoVO();
            FillSolicitacao(vo.Solicitacao, dr);

            bool? solAprovCoordenador = BaseDAO.GetNullableBoolean(dr, "FL_SOL_APROVADO_COORDENADOR", "S");
            if (solAprovCoordenador != null)
            {
                vo.AprovSolicitacaoCoordenador = new SolicitacaoViagemAprovacaoVO();
                vo.AprovSolicitacaoCoordenador.Aprovado = solAprovCoordenador.Value;
                vo.AprovSolicitacaoCoordenador.Justificativa = Convert.ToString(dr["DS_SOL_JUST_COORDENADOR"]);
                vo.AprovSolicitacaoCoordenador.IdUsuario = Convert.ToInt32(dr["ID_USR_COORD"]);
            }

            bool? solAprovDiretoria = BaseDAO.GetNullableBoolean(dr, "FL_SOL_APROVADO_DIRETORIA", "S");
            if (solAprovDiretoria != null)
            {
                vo.AprovSolicitacaoDiretoria = new SolicitacaoViagemAprovacaoVO();
                vo.AprovSolicitacaoDiretoria.Aprovado = solAprovDiretoria.Value;
                vo.AprovSolicitacaoDiretoria.Justificativa = Convert.ToString(dr["DS_SOL_JUST_DIRETORIA"]);
                vo.AprovSolicitacaoDiretoria.IdUsuario = Convert.ToInt32(dr["ID_USR_DIR_SOL"]);
            }


            bool? pcAprovFinanceiro = BaseDAO.GetNullableBoolean(dr, "FL_PC_APROVADO_FINANCEIRO", "S");
            if (pcAprovFinanceiro != null)
            {
                vo.AprovPrestacaoFinanceiro = new SolicitacaoViagemAprovacaoVO();
                vo.AprovPrestacaoFinanceiro.Aprovado = pcAprovFinanceiro.Value;
                vo.AprovPrestacaoFinanceiro.Justificativa = Convert.ToString(dr["DS_PC_JUST_FINANCEIRO"]);
                vo.AprovPrestacaoFinanceiro.IdUsuario = Convert.ToInt32(dr["ID_USR_FIN_PC"]);
            }

            bool? pcAprovDiretoria = BaseDAO.GetNullableBoolean(dr, "FL_PC_APROVADO_DIRETORIA", "S");
            if (pcAprovDiretoria != null)
            {
                vo.AprovPrestacaoDiretoria = new SolicitacaoViagemAprovacaoVO();
                vo.AprovPrestacaoDiretoria.Aprovado = pcAprovDiretoria.Value;
                vo.AprovPrestacaoDiretoria.Justificativa = Convert.ToString(dr["DS_PC_JUST_DIRETORIA"]);
                vo.AprovPrestacaoDiretoria.IdUsuario = Convert.ToInt32(dr["ID_USR_DIR_PC"]);
            }

            vo.Compra = new SolicitacaoViagemInfoCompraVO();
            vo.Compra.ValorPago = BaseDAO.GetNullableDecimal(dr, "VL_PAGO");
            vo.Compra.RecebimentoConfirmado = "S".Equals(Convert.ToString(dr["FL_PAGAMENTO_RECEBIDO"]));
            vo.Compra.ValorCurso = BaseDAO.GetNullableDecimal(dr, "VL_CURSO");

            vo.PrestacaoConta = new SolicitacaoViagemInfoPrestacaoContaVO();
            vo.PrestacaoConta.ValorHospedagem = BaseDAO.GetNullableDecimal(dr, "VL_DESPESA_HOSPEDAGEM");
            vo.PrestacaoConta.ValorPassagens = BaseDAO.GetNullableDecimal(dr, "VL_DESPESA_PASSAGEM");
            vo.PrestacaoConta.ResumoViagem = dr.Field<string>("DS_RESUMO_VIAGEM");

            vo.CodUsuarioSolicitante = Convert.ToInt32(dr["CD_USUARIO_SOLICITANTE"]);

            vo.Situacao = (StatusSolicitacaoViagem)Convert.ToInt32(dr["ID_SITUACAO"]);
            vo.DataSituacao = BaseDAO.GetNullableDate(dr, "DT_SITUACAO").Value;

            vo.TipoViagem = dr.IsNull("TP_VIAGEM") ? new TipoViagem?() : (TipoViagem)BaseDAO.GetNullableInt(dr, "TP_VIAGEM").Value;

            vo.Solicitacao.UsaValorDiaria = "S".Equals(dr.Field<string>("FL_USA_DIARIA"));

            return vo;
        }

        private static SolicitacaoViagemItinerarioVO FromDataRowItinerario(DataRow dr)
        {
            SolicitacaoViagemItinerarioVO vo = new SolicitacaoViagemItinerarioVO();
            vo.IdViagem = Convert.ToInt32(dr["CD_VIAGEM"]);
            vo.TipoRegistro = (TipoItinerarioSolicitacaoViagem)Convert.ToInt32(dr["TP_ITINERARIO"]);
            vo.IdItinerario = Convert.ToInt32(dr["ID_ITINERARIO"]);
            vo.Origem = Convert.ToString(dr["DS_ORIGEM"]);
            vo.Destino = Convert.ToString(dr["DS_DESTINO"]);
            vo.DataPartida = Convert.ToDateTime(dr["DT_PARTIDA"]);
            vo.DataRetorno = Convert.ToDateTime(dr["DT_RETORNO"]);
            vo.Valor = BaseDAO.GetNullableDecimal(dr, "VL_ITINERARIO");
            vo.Complemento = Convert.ToString(dr["DS_COMPLEMENTO"]);
            vo.IdArquivo = BaseDAO.GetNullableInt(dr, "CD_ARQUIVO");
            vo.TipoArquivo = BaseDAO.GetNullableEnum<TipoArquivoViagem>(dr, "TP_ARQUIVO");
            return vo;
        }

        private static SolicitacaoViagemDespesaDetalhadaVO FromDataRowDespDet(DataRow dr)
        {
            SolicitacaoViagemDespesaDetalhadaVO vo = new SolicitacaoViagemDespesaDetalhadaVO();
            // CD_VIAGEM, ID_DESPESA, DT_DESPESA  DS_DESPESA  CD_GRUPO_DESPESA  CD_TIPO_DESPESA DS_TIPO_DESPESA DS_IDENTIFICADOR
            // VL_DESPESA DT_CONFERIDO
            vo.IdViagem = Convert.ToInt32(dr["CD_VIAGEM"]);
            vo.IdDespesa = Convert.ToInt32(dr["ID_DESPESA"]);
            vo.Descricao = Convert.ToString(dr["DS_DESPESA"]);
            vo.Data = Convert.ToDateTime(dr["DT_DESPESA"]);
            vo.Valor = BaseDAO.GetNullableDecimal(dr, "VL_DESPESA").Value;

            vo.GrupoDespesa = (GrupoDespesaPrestContaViagem)Convert.ToInt32(dr["CD_GRUPO_DESPESA"]);
            vo.TipoDespesa = (TipoDespesaPrestContaViagem)Convert.ToInt32(dr["CD_TIPO_DESPESA"]);
            vo.DescricaoTipoDespesa = Convert.ToString(dr["ds_tipo_despesa"]);
            vo.Identificador = Convert.ToString(dr["DS_IDENTIFICADOR"]);

            vo.DataConferido = BaseDAO.GetNullableDate(dr, "DT_CONFERIDO");

            return vo;
        }

        private static void FillSolicitacao(SolicitacaoViagemInfoSolicitacaoVO vo, DataRow dr)
        {
            vo.Nome = Convert.ToString(dr["NM_VIAJANTE"]);
            vo.Cpf = Convert.ToInt64(dr["NR_CPF"]);
            vo.Rg = Convert.ToString(dr["NR_RG"]);
            vo.DataNascimento = Convert.ToDateTime(dr["DT_NASCIMENTO"]);
            vo.Cargo = Convert.ToString(dr["DS_CARGO"]);
            vo.Ramal = Convert.ToString(dr["DS_RAMAL"]);
            vo.Telefone = Convert.ToString(dr["DS_TELEFONE"]);

            vo.Agencia = dr.Field<string>("DS_AGENCIA");
            vo.Banco = dr.IsNull("DS_BANCO") ? null : new HcBancoVO()
            {
                Id = Convert.ToInt32(dr["DS_BANCO"])
            };
            vo.ContaCorrente = dr.Field<string>("DS_CONTA_CORRENTE");

            vo.MeioTransporte = FormatUtil.StringToList(Convert.ToString(dr["DS_MEIO_TRANSPORTE"])).Select(x => (MeioTransporteViagem)(Int32.Parse(x))).ToList();
            vo.Objetivo = Convert.ToString(dr["DS_OBJETIVO_VIAGEM"]);

            vo.ValorAdiantamento = BaseDAO.GetNullableDecimal(dr, "VL_SOL_ADIANTAMENTO");
            vo.JustificativaAdiantamento = dr.Field<string>("DS_JUST_ADIANTAMENTO");
        }

        internal static void Criar(SolicitacaoViagemVO solVO, EvidaDatabase evdb)
        {
            string sql = "INSERT INTO EV_SOL_VIAGEM (CD_VIAGEM, DT_SOLICITACAO, CD_MAT_EMPREGADO, NM_VIAJANTE, FL_EXTERNO, TP_VIAGEM, " +
                "	NR_CPF, NR_RG, DT_NASCIMENTO, DS_CARGO, DS_RAMAL, DS_TELEFONE, DS_AGENCIA, DS_BANCO, DS_CONTA_CORRENTE, " +
                "	VL_SOL_ADIANTAMENTO, DS_JUST_ADIANTAMENTO, " +
                "	FL_USA_DIARIA, " +
                "	DS_MEIO_TRANSPORTE, DS_OBJETIVO_VIAGEM, ID_SITUACAO, CD_USUARIO_SOLICITANTE, DT_SITUACAO) " +
                " VALUES (:id, LOCALTIMESTAMP, :cdFuncionario, :nmViajante, :flExterno, :tpViagem, " +
                "	:cpf, :rg, :nascimento, :cargo, :ramal, :telefone, :agencia, :banco, :conta, " +
                "	:vlAdiantamento, :justAdiantamento, " +
                "	:flDiaria, " +
                "	:meioTransporte, :objetivoViagem, :status, :cdUsuario, LOCALTIMESTAMP) ";

            Database db = evdb.Database;
            solVO.Id = NextId(evdb);
            SolicitacaoViagemInfoSolicitacaoVO vo = solVO.Solicitacao;

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, solVO.Id));
            lstParams.Add(new Parametro(":cdFuncionario", DbType.Int64, solVO.Empregado != null ? solVO.Empregado.Matricula : new long?()));
            lstParams.Add(new Parametro(":flExterno", DbType.String, solVO.IsExterno ? "S" : "N"));
            lstParams.Add(new Parametro(":tpViagem", DbType.Int32, solVO.TipoViagem != null ? (int)solVO.TipoViagem : new int?()));
            lstParams.Add(new Parametro(":nmViajante", DbType.String, vo.Nome));
            lstParams.Add(new Parametro(":cpf", DbType.Int64, vo.Cpf));
            lstParams.Add(new Parametro(":rg", DbType.String, vo.Rg));
            lstParams.Add(new Parametro(":nascimento", DbType.Date, vo.DataNascimento));
            lstParams.Add(new Parametro(":cargo", DbType.String, vo.Cargo));
            lstParams.Add(new Parametro(":ramal", DbType.String, vo.Ramal));
            lstParams.Add(new Parametro(":telefone", DbType.String, vo.Telefone));

            lstParams.Add(new Parametro(":agencia", DbType.String, vo.Agencia));
            lstParams.Add(new Parametro(":banco", DbType.String, vo.Banco != null ? vo.Banco.Id.ToString() : null));
            lstParams.Add(new Parametro(":conta", DbType.String, vo.ContaCorrente));
            lstParams.Add(new Parametro(":vlAdiantamento", DbType.Decimal, vo.ValorAdiantamento));
            lstParams.Add(new Parametro(":justAdiantamento", DbType.String, vo.JustificativaAdiantamento));

            lstParams.Add(new Parametro(":meioTransporte", DbType.String, FormatUtil.ListToString(vo.MeioTransporte.Select(x => ((int)x).ToString()).ToList())));
            lstParams.Add(new Parametro(":objetivoViagem", DbType.String, vo.Objetivo));

            lstParams.Add(new Parametro(":flDiaria", DbType.String, vo.UsaValorDiaria ? "S" : "N"));

            lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusSolicitacaoViagem.SOLICITACAO_PENDENTE));
            lstParams.Add(new Parametro(":cdUsuario", DbType.Int32, solVO.CodUsuarioSolicitante));

            BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);

            SalvarItinerarios(solVO.Id, solVO.Solicitacao.Itinerarios, evdb);
        }

        internal static void SalvarSolicitacao(SolicitacaoViagemVO solVO, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET CD_MAT_EMPREGADO = :cdFuncionario, NM_VIAJANTE = :nmViajante, FL_EXTERNO = :flExterno, TP_VIAGEM = :tpViagem, " +
                "	NR_CPF = :cpf, NR_RG = :rg, DT_NASCIMENTO = :nascimento, DS_CARGO = :cargo, DS_RAMAL = :ramal, DS_TELEFONE = :telefone, " +
                "	DS_AGENCIA = :agencia, DS_BANCO = :banco, DS_CONTA_CORRENTE = :conta, FL_USA_DIARIA = :flDiaria, " +
                "	VL_SOL_ADIANTAMENTO = :vlAdiantamento, DS_JUST_ADIANTAMENTO = :justAdiantamento, " +
                "	DS_MEIO_TRANSPORTE = :meioTransporte, DS_OBJETIVO_VIAGEM = :objetivoViagem, ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP " +
                " WHERE CD_VIAGEM = :id ";

            SolicitacaoViagemInfoSolicitacaoVO vo = solVO.Solicitacao;

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, solVO.Id));
            lstParams.Add(new Parametro(":cdFuncionario", DbType.Int64, solVO.Empregado != null ? solVO.Empregado.Matricula : new long?()));
            lstParams.Add(new Parametro(":flExterno", DbType.String, solVO.IsExterno ? "S" : "N"));
            lstParams.Add(new Parametro(":tpViagem", DbType.Int32, solVO.TipoViagem != null ? (int)solVO.TipoViagem : new int?()));
            lstParams.Add(new Parametro(":nmViajante", DbType.String, vo.Nome));
            lstParams.Add(new Parametro(":cpf", DbType.Int64, vo.Cpf));
            lstParams.Add(new Parametro(":rg", DbType.String, vo.Rg));
            lstParams.Add(new Parametro(":nascimento", DbType.Date, vo.DataNascimento));
            lstParams.Add(new Parametro(":cargo", DbType.String, vo.Cargo));
            lstParams.Add(new Parametro(":ramal", DbType.String, vo.Ramal));
            lstParams.Add(new Parametro(":telefone", DbType.String, vo.Telefone));

            lstParams.Add(new Parametro(":agencia", DbType.String, vo.Agencia));
            lstParams.Add(new Parametro(":banco", DbType.String, vo.Banco != null ? vo.Banco.Id.ToString() : null));
            lstParams.Add(new Parametro(":conta", DbType.String, vo.ContaCorrente));
            lstParams.Add(new Parametro(":vlAdiantamento", DbType.Decimal, vo.ValorAdiantamento));
            lstParams.Add(new Parametro(":justAdiantamento", DbType.String, vo.JustificativaAdiantamento));
            lstParams.Add(new Parametro(":flDiaria", DbType.String, vo.UsaValorDiaria ? "S" : "N"));

            lstParams.Add(new Parametro(":meioTransporte", DbType.String, FormatUtil.ListToString(vo.MeioTransporte.Select(x => ((int)x).ToString()).ToList())));
            lstParams.Add(new Parametro(":objetivoViagem", DbType.String, vo.Objetivo));

            lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusSolicitacaoViagem.SOLICITACAO_PENDENTE));

            BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);

            ClearItinerarios(solVO.Id, TipoItinerarioSolicitacaoViagem.ITINERARIO, evdb);
            SalvarItinerarios(solVO.Id, solVO.Solicitacao.Itinerarios, evdb);
        }

        private static void SalvarItinerarios(int idViagem, List<SolicitacaoViagemItinerarioVO> lst, EvidaDatabase evdb)
        {
            string sql = "INSERT INTO EV_SOL_VIAGEM_ITINERARIO (CD_VIAGEM, TP_ITINERARIO, ID_ITINERARIO, DS_ORIGEM, DS_DESTINO, DT_PARTIDA, DT_RETORNO, VL_ITINERARIO, DS_COMPLEMENTO) " +
                " VALUES (:idViagem, :tipo, :idItinerario, :origem, :destino, :dtPartida, :dtRetorno, :valor, :compl) ";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);

            db.AddInParameter(dbCommand, ":idViagem", DbType.Int32, idViagem);
            db.AddInParameter(dbCommand, ":tipo", DbType.Int32);
            db.AddInParameter(dbCommand, ":idItinerario", DbType.Int32);
            db.AddInParameter(dbCommand, ":origem", DbType.String);
            db.AddInParameter(dbCommand, ":destino", DbType.String);
            db.AddInParameter(dbCommand, ":dtPartida", DbType.DateTime);
            db.AddInParameter(dbCommand, ":dtRetorno", DbType.DateTime);
            db.AddInParameter(dbCommand, ":valor", DbType.Decimal);
            db.AddInParameter(dbCommand, ":compl", DbType.String);

            int id = 1;
            foreach (SolicitacaoViagemItinerarioVO item in lst)
            {
                db.SetParameterValue(dbCommand, ":tipo", item.TipoRegistro);
                db.SetParameterValue(dbCommand, ":idItinerario", id);
                db.SetParameterValue(dbCommand, ":origem", item.Origem);
                db.SetParameterValue(dbCommand, ":destino", item.Destino);
                db.SetParameterValue(dbCommand, ":dtPartida", item.DataPartida);
                db.SetParameterValue(dbCommand, ":dtRetorno", item.DataRetorno);
                db.SetParameterValue(dbCommand, ":valor", item.Valor);
                db.SetParameterValue(dbCommand, ":compl", item.Complemento);
                id++;

                BaseDAO.ExecuteNonQuery(dbCommand, evdb);
            }
        }

        private static void ClearItinerarios(int idViagem, TipoItinerarioSolicitacaoViagem tipo, EvidaDatabase evdb)
        {
            string sql = "DELETE FROM EV_SOL_VIAGEM_ITINERARIO WHERE CD_VIAGEM = :id AND TP_ITINERARIO = :tipo";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, idViagem);
            db.AddInParameter(dbCommand, ":tipo", DbType.Int32, (int)tipo);

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void CancelarSolicitacao(int id, string motivo, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET DS_MOTIVO_CANCELAMENTO = :motivo, ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP WHERE CD_VIAGEM = :id";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":motivo", DbType.String, motivo);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(StatusSolicitacaoViagem.CANCELADO));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }


        #region Aprovacao/Reprovacao

        internal static void SalvarSolicitacaoAprovCoordenador(int id, bool aprovado, string justificativa, int idUsuario, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET FL_SOL_APROVADO_COORDENADOR = :aprovado, DS_SOL_JUST_COORDENADOR = :justificativa, " +
                "	ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, ID_USR_COORD = :idUsuario " +
                "	WHERE CD_VIAGEM = :id";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":aprovado", DbType.String, aprovado ? "S" : "N");
            db.AddInParameter(dbCommand, ":justificativa", DbType.String, justificativa);
            db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(aprovado ? StatusSolicitacaoViagem.SOLICITACAO_APROVADO_COORDENADOR : StatusSolicitacaoViagem.SOLICITACAO_REPROVADO_COORDENADOR));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void SalvarSolicitacaoAprovDiretoria(int id, bool aprovado, string justificativa, int idUsuario, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET FL_SOL_APROVADO_DIRETORIA = :aprovado, DS_SOL_JUST_DIRETORIA = :justificativa, " +
                "	ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, ID_USR_DIR_SOL = :idUsuario " +
                "	WHERE CD_VIAGEM = :id";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":aprovado", DbType.String, aprovado ? "S" : "N");
            db.AddInParameter(dbCommand, ":justificativa", DbType.String, justificativa);
            db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(aprovado ? StatusSolicitacaoViagem.SOLICITACAO_APROVADO_DIRETORIA : StatusSolicitacaoViagem.SOLICITACAO_REPROVADO_DIRETORIA));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void SalvarPrestContaAprovFinanceiro(int id, bool aprovado, string justificativa, int idUsuario, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET FL_PC_APROVADO_FINANCEIRO = :aprovado, DS_PC_JUST_FINANCEIRO = :justificativa, " +
                "	ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, ID_USR_FIN_PC = :idUsuario " +
                "	WHERE CD_VIAGEM = :id";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":aprovado", DbType.String, aprovado ? "S" : "N");
            db.AddInParameter(dbCommand, ":justificativa", DbType.String, justificativa);
            db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(aprovado ? StatusSolicitacaoViagem.PRESTACAO_CONTA_CONFERIDA : StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_FINANCEIRO));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void SalvarPrestContaAprovDiretoria(int id, bool aprovado, string justificativa, int idUsuario, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET FL_PC_APROVADO_DIRETORIA = :aprovado, DS_PC_JUST_DIRETORIA = :justificativa, " +
                "	ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, ID_USR_DIR_PC = :idUsuario " +
                "	WHERE CD_VIAGEM = :id";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, id);
            db.AddInParameter(dbCommand, ":aprovado", DbType.String, aprovado ? "S" : "N");
            db.AddInParameter(dbCommand, ":justificativa", DbType.String, justificativa);
            db.AddInParameter(dbCommand, ":idUsuario", DbType.Int32, idUsuario);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(aprovado ? StatusSolicitacaoViagem.PRESTACAO_CONTA_APROVADA : StatusSolicitacaoViagem.PRESTACAO_CONTA_REPROVADO_DIRETORIA));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        #endregion

        #region Compra Passagem/Hotel

        internal static void SalvarInfoCompra(SolicitacaoViagemInfoCompraVO vo, decimal qtdDiarias, bool changeStatus, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET " +
                "		QT_TOTAL_DIARIAS = :totalDiaria, VL_CURSO = :vlCurso " +
                (changeStatus ? ", ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP" : "") +
                "	WHERE CD_VIAGEM = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, vo.IdViagem));
            lstParam.Add(new Parametro(":totalDiaria", DbType.Decimal, qtdDiarias));
            lstParam.Add(new Parametro(":vlCurso", DbType.Decimal, vo.ValorCurso));

            if (changeStatus)
                lstParam.Add(new Parametro(":status", DbType.Int32, (int)(StatusSolicitacaoViagem.COMPRA_EFETUADA)));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void SalvarInfoCompra(int idViagem, decimal qtdDiarias, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET " +
                "		QT_TOTAL_DIARIAS = :totalDiaria " +
                "	WHERE CD_VIAGEM = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":totalDiaria", DbType.Decimal, qtdDiarias));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        private static int GetNextItinerarioId(int idViagem, TipoItinerarioSolicitacaoViagem tipo, EvidaDatabase db)
        {
            string sql = "SELECT NVL(MAX(ID_ITINERARIO),0)+1 FROM EV_SOL_VIAGEM_ITINERARIO WHERE CD_VIAGEM = :idViagem AND TP_ITINERARIO = :tipo";
            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, tipo));

            return (int)((decimal)BaseDAO.ExecuteScalar(db, sql, lstParam));
        }

        internal static void IncluirItinerario(SolicitacaoViagemItinerarioVO vo, EvidaDatabase db)
        {
            string sql = "INSERT INTO EV_SOL_VIAGEM_ITINERARIO (CD_VIAGEM, TP_ITINERARIO, ID_ITINERARIO, DS_ORIGEM, DS_DESTINO, DT_PARTIDA, DT_RETORNO, VL_ITINERARIO, DS_COMPLEMENTO) " +
                " VALUES (:idViagem, :tipo, :idItinerario, :origem, :destino, :dtPartida, :dtRetorno, :valor, :compl) ";

            int id = GetNextItinerarioId(vo.IdViagem, vo.TipoRegistro, db);
            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, vo.IdViagem));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, vo.TipoRegistro));
            lstParam.Add(new Parametro(":idItinerario", DbType.Int32, id));
            lstParam.Add(new Parametro(":origem", DbType.String, vo.Origem));
            lstParam.Add(new Parametro(":destino", DbType.String, vo.Destino));
            lstParam.Add(new Parametro(":dtPartida", DbType.DateTime, vo.DataPartida));
            lstParam.Add(new Parametro(":dtRetorno", DbType.DateTime, vo.DataRetorno));
            lstParam.Add(new Parametro(":valor", DbType.Decimal, vo.Valor));
            lstParam.Add(new Parametro(":compl", DbType.String, vo.Complemento));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void RemoverItinerario(int idViagem, TipoItinerarioSolicitacaoViagem tipo, int idItinerario, EvidaDatabase db)
        {
            string sql = "DELETE FROM EV_SOL_VIAGEM_ITINERARIO WHERE CD_VIAGEM = :idViagem AND TP_ITINERARIO = :tipo AND ID_ITINERARIO = :idItinerario";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, tipo));
            lstParam.Add(new Parametro(":idItinerario", DbType.Int32, idItinerario));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void VincularArquivoItinerario(SolicitacaoViagemItinerarioVO itinerarioVO, SolicitacaoViagemArquivoVO solicitacaoViagemArquivoVO, EvidaDatabase db)
        {
            string sql = "UPDATE EV_SOL_VIAGEM_ITINERARIO SET TP_ARQUIVO = :tpArq, CD_ARQUIVO = :idArquivo WHERE CD_VIAGEM = :idViagem AND ID_ITINERARIO = :idItinerario AND TP_ITINERARIO = :tipo";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, itinerarioVO.IdViagem));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, (int)itinerarioVO.TipoRegistro));
            lstParam.Add(new Parametro(":idItinerario", DbType.Int32, itinerarioVO.IdItinerario));

            lstParam.Add(new Parametro(":idArquivo", DbType.Int32, solicitacaoViagemArquivoVO.IdArquivo));
            lstParam.Add(new Parametro(":tpArq", DbType.Int32, (int)solicitacaoViagemArquivoVO.TipoArquivo));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        #endregion

        internal static void SalvarFinanceiroPagamento(int idViagem, decimal valorPago, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET VL_PAGO = :valor WHERE CD_VIAGEM = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":valor", DbType.Decimal, valorPago));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void SalvarFinanceiroPagamento(SolicitacaoViagemInfoCompraVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP WHERE CD_VIAGEM = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, vo.IdViagem));
            lstParam.Add(new Parametro(":status", DbType.Int32, (int)(StatusSolicitacaoViagem.PAGAMENTO_ADIANTAMENTO_EFETUADO)));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void ConfirmarPagamentoRecebido(int id, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, FL_PAGAMENTO_RECEBIDO = 'S' WHERE CD_VIAGEM = :id";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, id));
            lstParam.Add(new Parametro(":status", DbType.Int32, (int)(StatusSolicitacaoViagem.PAGAMENTO_ADIANTAMENTO_CONFERIDO)));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void SalvarPrestacaoConta(SolicitacaoViagemInfoPrestacaoContaVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_SOL_VIAGEM SET VL_DESPESA_PASSAGEM = :vlPrestContaPassagens, VL_DESPESA_HOSPEDAGEM = :vlPrestContaHospedagem, " +
                "		ID_SITUACAO = :status, DT_SITUACAO = LOCALTIMESTAMP, DS_RESUMO_VIAGEM = :resumo " +
                "	WHERE CD_VIAGEM = :id";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.IdViagem);
            db.AddInParameter(dbCommand, ":vlPrestContaPassagens", DbType.Decimal, vo.ValorPassagens);
            db.AddInParameter(dbCommand, ":vlPrestContaHospedagem", DbType.Decimal, vo.ValorHospedagem);
            db.AddInParameter(dbCommand, ":resumo", DbType.String, vo.ResumoViagem);
            db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)(StatusSolicitacaoViagem.PRESTACAO_CONTA_PENDENTE));

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void IncluirDespesaDetalhada(SolicitacaoViagemDespesaDetalhadaVO vo, EvidaDatabase evdb)
        {
            string sql = "SELECT NVL(MAX(ID_DESPESA),0)+1 FROM EV_SOL_VIAGEM_DESP_DET" +
                " WHERE CD_VIAGEM = :idViagem ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, vo.IdViagem));
            decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, sql, lstParam);

            sql = "INSERT INTO EV_SOL_VIAGEM_DESP_DET (CD_VIAGEM, ID_DESPESA, DS_DESPESA, DT_DESPESA, VL_DESPESA, " +
                "	CD_GRUPO_DESPESA, CD_TIPO_DESPESA, DS_TIPO_DESPESA, DS_IDENTIFICADOR, DT_CONFERIDO) " +
                " VALUES (:idViagem, :idDesp, :despesa, :data, :valor, " +
                "	:grupo, :tipo, :desTipo, :ident, NULL) ";

            lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, vo.IdViagem));
            lstParam.Add(new Parametro(":idDesp", DbType.Int32, (int)idSolicitacao));
            lstParam.Add(new Parametro(":despesa", DbType.String, vo.Descricao));
            lstParam.Add(new Parametro(":data", DbType.DateTime, vo.Data));
            lstParam.Add(new Parametro(":valor", DbType.Decimal, vo.Valor));

            lstParam.Add(new Parametro(":grupo", DbType.Int32, (int)vo.GrupoDespesa));
            lstParam.Add(new Parametro(":tipo", DbType.Int32, (int)vo.TipoDespesa));
            lstParam.Add(new Parametro(":desTipo", DbType.String, vo.DescricaoTipoDespesa));
            lstParam.Add(new Parametro(":ident", DbType.String, vo.Identificador));

            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void RemoverDespesaDetalhada(int idViagem, int idDespesa, EvidaDatabase evdb)
        {
            string sql = "DELETE FROM EV_SOL_VIAGEM_DESP_DET WHERE CD_VIAGEM = :idViagem AND ID_DESPESA = :idDesp ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":idDesp", DbType.Int32, idDespesa));
            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        internal static void MarcarDespesaDetalhadaConferido(int idViagem, int idDespesa, bool ok, EvidaDatabase db)
        {
            string sqlOK = "UPDATE EV_SOL_VIAGEM_DESP_DET SET DT_CONFERIDO = LOCALTIMESTAMP WHERE CD_VIAGEM = :idViagem AND ID_DESPESA = :idDesp ";
            string sqlRev = "UPDATE EV_SOL_VIAGEM_DESP_DET SET DT_CONFERIDO = NULL WHERE CD_VIAGEM = :idViagem AND ID_DESPESA = :idDesp ";

            string sql = ok ? sqlOK : sqlRev;

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":idDesp", DbType.Int32, idDespesa));
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        #region Arquivo

        private static SolicitacaoViagemArquivoVO FromDataRowArquivo(DataRow dr)
        {
            SolicitacaoViagemArquivoVO vo = new SolicitacaoViagemArquivoVO();
            vo.TipoArquivo = (TipoArquivoViagem)Convert.ToInt32(dr["tp_arquivo"]);
            vo.IdArquivo = Convert.ToInt32(dr["CD_ARQUIVO"]);
            vo.IdViagem = Convert.ToInt32(dr["CD_VIAGEM"]);
            vo.DataEnvio = Convert.ToDateTime(dr["dt_envio"]);
            vo.NomeArquivo = Convert.ToString(dr["nm_arquivo"]);
            return vo;
        }

        internal static void CriarArquivos(int idViagem, TipoArquivoViagem tipoArquivo, List<SolicitacaoViagemArquivoVO> lstNewFiles, EvidaDatabase evdb)
        {
            Database db = evdb.Database;
            DbCommand dbCommand = CreateInsertArquivo(db);

            db.AddInParameter(dbCommand, ":id", DbType.Int32, idViagem);
            db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32);
            db.AddInParameter(dbCommand, ":tipo", DbType.Int32, (int)tipoArquivo);
            db.AddInParameter(dbCommand, ":nome", DbType.String);

            int idNextArquivo = GetNextArquivoId(idViagem, tipoArquivo, evdb);

            foreach (SolicitacaoViagemArquivoVO arq in lstNewFiles)
            {
                if (arq.IdArquivo == 0)
                {
                    arq.IdArquivo = idNextArquivo++;

                    db.SetParameterValue(dbCommand, ":idArquivo", arq.IdArquivo);
                    db.SetParameterValue(dbCommand, ":nome", arq.NomeArquivo);

                    BaseDAO.ExecuteNonQuery(dbCommand, evdb);
                }
            }
            dbCommand = CreateUpdateArquivo(db);

            db.AddInParameter(dbCommand, ":id", DbType.Int32, idViagem);
            db.AddInParameter(dbCommand, ":idArquivo", DbType.Int32);
            db.AddInParameter(dbCommand, ":tipo", DbType.Int32, (int)tipoArquivo);
            db.AddInParameter(dbCommand, ":nome", DbType.String);

            foreach (SolicitacaoViagemArquivoVO arq in lstNewFiles)
            {
                db.SetParameterValue(dbCommand, ":idArquivo", arq.IdArquivo);
                db.SetParameterValue(dbCommand, ":nome", arq.NomeArquivo);

                BaseDAO.ExecuteNonQuery(dbCommand, evdb);
            }
        }

        internal static SolicitacaoViagemArquivoVO GetArquivoById(int idViagem, TipoArquivoViagem tipoArq, int idArquivo, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_ARQUIVO A" +
                "		WHERE a.cd_viagem = :idViagem AND TP_ARQUIVO = :tipoArq AND CD_ARQUIVO = :idArquivo";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":tipoArq", DbType.Int32, (int)tipoArq));
            lstParam.Add(new Parametro(":idArquivo", DbType.Int32, idArquivo));

            return BaseDAO.ExecuteDataRow(db, sql, FromDataRowArquivo, lstParam);
        }

        internal static List<SolicitacaoViagemArquivoVO> ListarArquivos(int id, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_ARQUIVO A" +
                "		WHERE a.cd_viagem = :id ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, id));

            List<SolicitacaoViagemArquivoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParam);
            return lst;
        }

        internal static List<SolicitacaoViagemArquivoVO> ListarArquivos(int id, TipoArquivoViagem tipoArq, EvidaDatabase db)
        {
            string sql = "SELECT * FROM EV_SOL_VIAGEM_ARQUIVO A" +
                "		WHERE a.cd_viagem = :id AND TP_ARQUIVO = :tipoArq";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":id", DbType.Int32, id));
            lstParam.Add(new Parametro(":tipoArq", DbType.Int32, (int)tipoArq));

            List<SolicitacaoViagemArquivoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowArquivo, lstParam);
            return lst;
        }

        private static int GetNextArquivoId(int idViagem, TipoArquivoViagem tipoArq, EvidaDatabase evdb)
        {
            string sql = "SELECT NVL(MAX(CD_ARQUIVO),0)+1 FROM EV_SOL_VIAGEM_ARQUIVO WHERE CD_VIAGEM = :idViagem AND TP_ARQUIVO = :tipoArq";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, idViagem));
            lstParam.Add(new Parametro(":tipoArq", DbType.Int32, (int)tipoArq));

            decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(evdb, sql, lstParam);

            return (int)idSolicitacao;
        }

        private static DbCommand CreateInsertArquivo(Database db)
        {
            string sql = "INSERT INTO EV_SOL_VIAGEM_ARQUIVO (CD_VIAGEM, CD_ARQUIVO, TP_ARQUIVO, NM_ARQUIVO, DT_ENVIO) " +
                "	VALUES (:id, :idArquivo, :tipo, :nome, LOCALTIMESTAMP) ";

            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            return dbCommand;
        }

        private static DbCommand CreateUpdateArquivo(Database db)
        {
            string sql = "UPDATE EV_SOL_VIAGEM_ARQUIVO SET NM_ARQUIVO = :nome, DT_ENVIO = LOCALTIMESTAMP WHERE CD_VIAGEM = :id AND CD_ARQUIVO = :idArquivo AND TP_ARQUIVO = :tipo";

            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            return dbCommand;
        }

        internal static void ExcluirArquivo(SolicitacaoViagemArquivoVO vo, EvidaDatabase evdb)
        {
            string sql = "DELETE FROM EV_SOL_VIAGEM_ARQUIVO " +
                "	WHERE CD_VIAGEM = :idViagem AND CD_ARQUIVO = :idArquivo AND TP_ARQUIVO = :tipoArq";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":idViagem", DbType.Int32, vo.IdViagem));
            lstParam.Add(new Parametro(":idArquivo", DbType.Int32, vo.IdArquivo));
            lstParam.Add(new Parametro(":tipoArq", DbType.Int32, (int)vo.TipoArquivo));
            BaseDAO.ExecuteNonQuery(sql, lstParam, evdb);
        }

        #endregion


    }
}
