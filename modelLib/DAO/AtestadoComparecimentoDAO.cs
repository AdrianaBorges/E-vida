using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class AtestadoComparecimentoDAO {
		static EVidaLog log = new EVidaLog(typeof(AtestadoComparecimentoDAO));

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT sq_ev_atestado_comparecimento.nextval FROM DUAL";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		#region Formulario

		internal static void Salvar(AtestadoComparecimentoVO vo, EvidaDatabase db) {
			string sql = null;
			List<Parametro> lstParam = new List<Parametro>();
			
			if (vo.CodSolicitacao == 0) {
				sql = CreateInsert(vo, lstParam);				
				vo.CodSolicitacao = NextId(db);
			} else {
				sql = CreateUpdate(vo);
			}
			lstParam.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));
			lstParam.Add(new Parametro(":codint", DbType.String, vo.Codint.Trim()));
            lstParam.Add(new Parametro(":cdEmpresa", DbType.String, vo.Codemp.Trim()));
            lstParam.Add(new Parametro(":matric", DbType.String, vo.Matric.Trim()));
            lstParam.Add(new Parametro(":tipreg", DbType.String, vo.Tipreg.Trim()));
            lstParam.Add(new Parametro(":nome", DbType.String, vo.Nome.Trim()));
			lstParam.Add(new Parametro(":status", DbType.Int32, vo.IdStatus));
			lstParam.Add(new Parametro(":data", DbType.Date, vo.DataAtendimento));
			lstParam.Add(new Parametro(":horaInicio", DbType.String, vo.HoraInicio));
			lstParam.Add(new Parametro(":horaFim", DbType.String, vo.HoraFim));
			lstParam.Add(new Parametro(":pericia", DbType.Int32, vo.TipoPericia));
            lstParam.Add(new Parametro(":lotacao", DbType.String, vo.Lotacao.Trim()));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
			
		}

		private static string CreateInsert(AtestadoComparecimentoVO vo, List<Parametro> lstParam) {
            string sql = "INSERT INTO ev_atestado_comparecimento (cd_solicitacao, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, " +
                "	BA1_NOMUSR, cd_status, dt_atendimento, hr_inicio, hr_fim, tp_pericia, BBZ_SIGLA, " +
                "	 dt_criacao, cd_usuario_criacao, dt_finalizacao, cd_usuario_finalizacao) " +
                " VALUES (:id, :codint, :cdEmpresa, :matric, :tipreg, :nome, :status, :data, :horaInicio, :horaFim, :pericia, :lotacao," +
                "	LOCALTIMESTAMP, :usuario, null, null)";

			lstParam.Add(new Parametro(":usuario", DbType.Int32, vo.IdUsuarioCriacao));

			return sql;
		}

		private static string CreateUpdate(AtestadoComparecimentoVO vo) {
            string sql = "UPDATE ev_atestado_comparecimento SET BA1_CODINT = :codint, BA1_CODEMP = :cdEmpresa, BA1_MATRIC = :matric, BA1_TIPREG = :tipreg, " +
                "	BA1_NOMUSR = :nome, cd_status = :status, dt_atendimento = :data, " +
                "	hr_inicio = :horaInicio, hr_fim = :horaFim, tp_pericia = :pericia, " +
                "	BBZ_SIGLA = :lotacao " +
                "	WHERE cd_solicitacao = :id";  

			return sql;
		}

		internal static AtestadoComparecimentoVO FromDataRow(DataRow dr) {
			AtestadoComparecimentoVO vo = new AtestadoComparecimentoVO();
			vo.CodSolicitacao = Convert.ToInt32(dr["cd_solicitacao"]);
			vo.Codint = Convert.ToString(dr["BA1_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA1_CODEMP"]);
            vo.Matric = Convert.ToString(dr["BA1_MATRIC"]);
            vo.Tipreg = Convert.ToString(dr["BA1_TIPREG"]);
            vo.Nome = dr.Field<String>("BA1_NOMUSR");
			vo.DataAtendimento = dr.Field<DateTime>("dt_atendimento");
			vo.HoraInicio = dr.Field<String>("hr_inicio");
			vo.HoraFim = dr.Field<String>("hr_fim");
			vo.TipoPericia = Convert.ToInt32(dr["tp_pericia"]);
            vo.Lotacao = dr.Field<string>("BBZ_SIGLA");
			vo.DataCriacao = dr.Field<DateTime>("dt_criacao");
			vo.DataFinalizacao = dr.Field<DateTime?>("dt_finalizacao");
			vo.IdStatus = (int)dr.Field<decimal>("cd_status");
			vo.IdUsuarioCriacao = (int)dr.Field<decimal>("cd_usuario_criacao");
			vo.IdUsuarioFinalizacao = (int?)dr.Field<decimal?>("cd_usuario_finalizacao");
			return vo;
		}

		internal static AtestadoComparecimentoVO GetById(int id, EvidaDatabase db) {
            string sql = "select cd_solicitacao, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BA1_NOMUSR, " +
                            "	cd_status, dt_atendimento, hr_inicio, hr_fim, tp_pericia, BBZ_SIGLA," +
                            "	dt_criacao, cd_usuario_criacao, dt_finalizacao, cd_usuario_finalizacao " +
                            " FROM ev_atestado_comparecimento A " +
                            " WHERE a.cd_solicitacao = :id ";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro(":id", DbType.Int32, id));

			List<AtestadoComparecimentoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstP);

			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		#endregion

		internal static DataTable Pesquisar(string matricula, int? cdProtocolo, StatusAtestadoComparecimento? status, EvidaDatabase db) {

            string sql = "select a.cd_solicitacao, a.BA1_CODINT, a.BA1_CODEMP, a.BA1_MATRIC, a.BA1_TIPREG, benef.BA1_MATEMP, a.BA1_NOMUSR, " +
                "	a.cd_status, a.dt_atendimento, a.hr_inicio, a.hr_fim, a.tp_pericia, a.BBZ_SIGLA, " +
                "	a.dt_criacao, a.cd_usuario_criacao, a.dt_finalizacao, a.cd_usuario_finalizacao, " +
                "	uc.nm_usuario usuario_criacao, uf.nm_usuario usuario_finalizacao, " +
                "	NVL(benef.BA1_NOMUSR, 'TITULAR') nm_beneficiario " +
                " from ev_atestado_comparecimento a, ev_usuario uc, ev_usuario uf, VW_PR_USUARIO benef " +
                " WHERE a.cd_usuario_criacao = uc.id_usuario and a.cd_usuario_finalizacao = uf.id_usuario(+)  " +
                "	AND trim(a.BA1_CODINT) = trim(benef.BA1_CODINT) AND trim(a.BA1_CODEMP) = trim(benef.BA1_CODEMP) AND trim(a.BA1_MATRIC) = trim(benef.BA1_MATRIC) AND trim(a.BA1_TIPREG) = trim(benef.BA1_TIPREG) ";

            List<Parametro> lstP = new List<Parametro>();
            if (cdProtocolo != null)
            {
                sql += " AND a.cd_solicitacao = :id";
                lstP.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
            }
            if (!string.IsNullOrEmpty(matricula.Trim()))
            {
                sql += " AND trim(benef.BA1_MATEMP) = trim(:matricula)";
                lstP.Add(new Parametro(":matricula", DbType.String, matricula));
            }
            if (status != null)
            {
                sql += " AND a.cd_status = :status";
                lstP.Add(new Parametro(":status", DbType.Int32, (int)status));
            }

            sql += " ORDER BY cd_status, cd_solicitacao ";  

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

		internal static void Finalizar(int cdProtocolo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE ev_atestado_comparecimento SET CD_STATUS = :status, " +
				"	CD_USUARIO_FINALIZACAO = :usuario, DT_FINALIZACAO = LOCALTIMESTAMP " +
				"	WHERE CD_SOLICITACAO = :id";

			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			lstParam.Add(new Parametro(":status", DbType.Int32, (int)StatusAtestadoComparecimento.EMITIDO));
			lstParam.Add(new Parametro(":usuario", DbType.Int32, idUsuario));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}
		
	}
}
