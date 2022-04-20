using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class ParametroVariavelBO {
		EVidaLog log = new EVidaLog(typeof(ParametroVariavelBO));
		private static ParametroVariavelBO instance = new ParametroVariavelBO();

		public static ParametroVariavelBO Instance { get { return instance; } }

		public List<ParametroVariavelVO> GetParametroRange(ParametroUtil.ParametroVariavelType param, DateTime inicio, DateTime fim) {
			List<ParametroVariavelVO> lstParametros = GetParametroAllInternal(param);
			if (lstParametros != null) {
				return lstParametros.FindAll(x => 
					(x.Inicio <= fim && x.Fim >= fim) ||
					(x.Inicio <= inicio && x.Fim >= inicio) ||
					(x.Inicio >= inicio && x.Fim <= fim)
					);

				/*
				 * ((dt_inicio_vigencia <= :fim and dt_fim_vigencia >= :fim) OR "+
				"		(dt_inicio_vigencia <= :inicio and dt_fim_vigencia >= :inicio) OR " +
				"		(dt_inicio_vigencia >= :inicio and dt_fim_vigencia <= :fim)) " +*/
			}
			return null;
		}

		public ParametroVariavelVO GetParametro(ParametroUtil.ParametroVariavelType param, int idSeq) {
			List<ParametroVariavelVO> lstParametros = GetParametroAllInternal(param);
			if (lstParametros != null) {
				return lstParametros.Find(x => x.IdLinha == idSeq);
			}
			return null;
		}
		public ParametroVariavelVO GetParametro(ParametroUtil.ParametroVariavelType param, DateTime dataRef) {
			List<ParametroVariavelVO> lstParametros = GetParametroAllInternal(param);
			if (lstParametros != null) {
				return lstParametros.Find(x => x.Inicio <= dataRef && x.Fim >= dataRef);
			}
			return null;
		}

		public List<ParametroVariavelVO> GetParametroAll(ParametroUtil.ParametroVariavelType param) {
			return GetParametroAllInternal(param);
		}

		private List<ParametroVariavelVO> GetParametroAllInternal(ParametroUtil.ParametroVariavelType param) {
			List<ParametroVariavelVO> lstValues = CacheHelper.GetFromCache<List<ParametroVariavelVO>>("PV_" + param);
			if (lstValues == null) {
				using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
					EvidaConnectionHolder connection = db.CreateConnection();

					lstValues = ParametroDAO.GetParametroAll(param, db);
					CacheHelper.AddOnCache("PV_" + param, lstValues, 30);
				}
			}
			return lstValues;
		}

		public ParametroVariavelVO CheckConcorrencia(ParametroVariavelVO vo) {
			List<ParametroVariavelVO> lst = GetParametroRange(vo.Id, vo.Inicio, vo.Fim);
			if (lst == null) return null;

			foreach (ParametroVariavelVO item in lst) {
				if (item.IdLinha != vo.IdLinha) {
					return item;
				}
			}
			return null;
		}

		public void Salvar(ParametroVariavelVO vo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				ParametroDAO.Salvar(vo, idUsuario, db);
				CacheHelper.RemoveFromCache("PV_" + vo.Id);

				connection.Commit();
			}
		}
	}
}
