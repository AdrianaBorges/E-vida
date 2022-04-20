using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO {
	internal class ConfiguracaoIrDAO {
		internal static ConfiguracaoIrVO GetConfig(EvidaDatabase db) {
			List<KeyValuePair<ParametroUtil.ParametroType, string>> configs = ListarConfigs(db);
			Dictionary<ParametroUtil.ParametroType, string> values = new Dictionary<ParametroUtil.ParametroType,string>();
			if (configs != null) {
				foreach (KeyValuePair<ParametroUtil.ParametroType, string> p in configs) {
					values.Add(p.Key, p.Value);
				}
			}
			ConfiguracaoIrVO vo = new ConfiguracaoIrVO();

			vo.DayIrBeneficiario = values.ContainsKey(ParametroUtil.ParametroType.CONFIGURACAO_IR_DIA_BENEF) ? Int32.Parse(values[ParametroUtil.ParametroType.CONFIGURACAO_IR_DIA_BENEF]) : 28;
			vo.EnableIrBeneficiario = values.ContainsKey(ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_BENEF) ? Boolean.Parse(values[ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_BENEF]) : false;
			vo.EnableIrCredenciado = values.ContainsKey(ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_CRED) ? Boolean.Parse(values[ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_CRED]) : false;
			vo.Anos = values.ContainsKey(ParametroUtil.ParametroType.CONFIGURACAO_IR_ANOS) ? ParseAnos(values[ParametroUtil.ParametroType.CONFIGURACAO_IR_ANOS]) : DefaultAnos();
            vo.EnderecoEVIDA = values.ContainsKey(ParametroUtil.ParametroType.CONFIGURACAO_IR_ENDERECO_EVIDA) ? values[ParametroUtil.ParametroType.CONFIGURACAO_IR_ENDERECO_EVIDA] : "";

			return vo;			
		}

		internal static void SaveConfig(ConfiguracaoIrVO vo, EvidaDatabase db) {
			ParametroDAO.Update(ParametroUtil.ParametroType.CONFIGURACAO_IR_DIA_BENEF, vo.DayIrBeneficiario.ToString(), db);
			ParametroDAO.Update(ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_BENEF, vo.EnableIrBeneficiario.ToString(), db);
			ParametroDAO.Update(ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_CRED, vo.EnableIrCredenciado.ToString(), db);
            ParametroDAO.Update(ParametroUtil.ParametroType.CONFIGURACAO_IR_ENDERECO_EVIDA, vo.EnderecoEVIDA.ToString(), db);
		}

		private static List<int> DefaultAnos() {
			List<int> lst = new List<int>();
			//lst.Add(DateTime.Now.Year - 1);
			return lst;
		}

		private static List<int> ParseAnos(string value) {
			List<string> values = FormatUtil.StringToList(value);
			List<int> lst = values.Select(x => Int32.Parse(x)).ToList();
			lst.Sort();
			return lst;
		}

		private static List<KeyValuePair<ParametroUtil.ParametroType, string>> ListarConfigs(EvidaDatabase db) {
			string sql = "SELECT id_parametro, vl_parametro " +
				" FROM ev_parametro p " +
				" WHERE id_parametro IN (" + 
				(int)ParametroUtil.ParametroType.CONFIGURACAO_IR_ANOS + ", " +
				(int)ParametroUtil.ParametroType.CONFIGURACAO_IR_DIA_BENEF + ", " +
				(int)ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_BENEF + ", " +
                (int)ParametroUtil.ParametroType.CONFIGURACAO_IR_HABILITACAO_CRED + ", " +
                (int)ParametroUtil.ParametroType.CONFIGURACAO_IR_ENDERECO_EVIDA + " ) ";

			List<KeyValuePair<int, string>> lst = BaseDAO.ExecuteDataSet(db, sql, BaseDAO.Converter<int>("id_parametro", "vl_parametro"));
			if (lst != null)
				return lst.Select(x => new KeyValuePair<ParametroUtil.ParametroType, String>((ParametroUtil.ParametroType)x.Key, x.Value)).ToList();
			return null;
		}

		internal static void SaveYearsConfig(List<int> anos, EvidaDatabase db) {
			ParametroDAO.Update(ParametroUtil.ParametroType.CONFIGURACAO_IR_ANOS, FormatUtil.ListToString(anos), db);
		}
	}
}
