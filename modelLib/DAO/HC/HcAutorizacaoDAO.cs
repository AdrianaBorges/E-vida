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

namespace eVidaGeneralLib.DAO.HC {
	internal class HcAutorizacaoDAO {

		private const string FIELDS = "nr_autorizacao, dt_registro, tp_autorizacao, st_autorizacao, " +
			" tp_sistema_atend, cd_beneficiario, cd_credenciado, dt_inicio_autorizacao, dt_termino_autorizacao, " +
			" ds_observacao, user_update, date_update, nr_dias_autorizados";

		internal static HcAutorizacaoVO GetRowById(int nroAutorizacao, EvidaDatabase db) {
			string sql = "select " + FIELDS +
				" from isa_hc.hc_autorizacao WHERE nr_autorizacao = :nroAutorizacao";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":nroAutorizacao", DbType.Int32, nroAutorizacao));

			List<HcAutorizacaoVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		private static HcAutorizacaoVO FromDataRow(DataRow dr) {
			HcAutorizacaoVO vo = new HcAutorizacaoVO();
			vo.CdBeneficiario = BaseDAO.GetNullableInt(dr, "cd_beneficiario").Value;
			vo.DateUpdate = dr.Field<string>("date_update");
			vo.DtInicioAutorizacao = dr.Field<DateTime?>("dt_inicio_autorizacao");
			vo.DtRegistro = dr.Field<DateTime>("dt_registro");
			vo.DtTerminoAutorizacao = dr.Field<DateTime?>("dt_termino_autorizacao");
			vo.NrAutorizacao = BaseDAO.GetNullableInt(dr, "nr_autorizacao").Value;
			vo.NrDiasAutorizados = BaseDAO.GetNullableInt(dr, "nr_dias_autorizados");
			vo.Observacao = dr.Field<string>("ds_observacao");
			vo.StAutorizacao = dr.Field<string>("st_autorizacao");
			vo.TpAutorizacao = dr.Field<string>("tp_autorizacao");
			vo.UserUpdate = dr.Field<string>("user_update");
			return vo;
		}

		internal static void AjustarDatas(int nroAutorizacao, DateTime dtInicio, DateTime dtFim, VO.UsuarioVO usuarioVO, EvidaDatabase db) {
			HcAutorizacaoVO oldVO = GetRowById(nroAutorizacao, db);
			AltAutorizacaoIsaVO auditVO = new AltAutorizacaoIsaVO();
			auditVO.IdUsuario = usuarioVO.Id;
			auditVO.InicioAnt = oldVO.DtInicioAutorizacao;
			auditVO.InicioNovo = dtInicio;
			auditVO.NrAutorizacao = nroAutorizacao;
			auditVO.TerminoAnt = oldVO.DtTerminoAutorizacao;
			auditVO.TerminoNovo = dtFim;

			GerarAuditoria(auditVO, db);

			SalvarAjuste(oldVO, auditVO, usuarioVO, db);
		}

		private static void SalvarAjuste(HcAutorizacaoVO oldVO, AltAutorizacaoIsaVO auditVO, UsuarioVO usuarioVO, EvidaDatabase db) {
			string sql = "UPDATE ISA_HC.HC_AUTORIZACAO SET " +
				"	DT_INICIO_AUTORIZACAO = :dtIniNovo, DT_TERMINO_AUTORIZACAO = :dtTerminoNovo, " +
				"	NR_DIAS_AUTORIZADOS = :nroDias, USER_UPDATE = :login, DATE_UPDATE = TO_CHAR(SYSDATE, 'DD/MM/RRRR HH24:MI:SS') " +
				" WHERE NR_AUTORIZACAO = :id ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, oldVO.NrAutorizacao));
			lstParams.Add(new Parametro(":login", DbType.String, usuarioVO.Login + "_INTRANET"));
			lstParams.Add(new Parametro(":dtIniNovo", DbType.Date, auditVO.InicioNovo));
			lstParams.Add(new Parametro(":dtTerminoNovo", DbType.Date, auditVO.TerminoNovo));
			lstParams.Add(new Parametro(":nroDias", DbType.Int32, DateUtil.DayDiff(auditVO.TerminoNovo, auditVO.InicioNovo, 1)));

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		private static void GerarAuditoria(AltAutorizacaoIsaVO auditoria, EvidaDatabase db) {
			string sql = "INSERT INTO EV_ALT_AUTORIZACAO_ISA (NR_AUTORIZACAO, DT_ALTERACAO, ID_USUARIO, " +
				"	DT_INICIO_ANT, DT_INICIO_NOVO, DT_TERMINO_ANT, DT_TERMINO_NOVO) " +
				" VALUES (:id, LOCALTIMESTAMP, :idUsuario, " +
				"	:dtIniAnt, :dtIniNovo, :dtTerminoAnt, :dtTerminoNovo) ";
			
			List<Parametro> lstParam = new List<Parametro>();
			lstParam.Add(new Parametro(":id", DbType.Int32, auditoria.NrAutorizacao));
			lstParam.Add(new Parametro(":idUsuario", DbType.Int32, auditoria.IdUsuario));
			lstParam.Add(new Parametro(":dtIniAnt", DbType.Date, auditoria.InicioAnt));
			lstParam.Add(new Parametro(":dtIniNovo", DbType.Date, auditoria.InicioNovo));
			lstParam.Add(new Parametro(":dtTerminoAnt", DbType.Date, auditoria.TerminoAnt));
			lstParam.Add(new Parametro(":dtTerminoNovo", DbType.Date, auditoria.TerminoNovo));

			BaseDAO.ExecuteNonQuery(sql, lstParam, db);
		}
	}
}
