using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class ViagemHelper {

		public static string GetStringValoresDiaria(List<ViagemValorVariavelVO> lstValoresDiaria) {
			if (lstValoresDiaria == null || lstValoresDiaria.Count == 0)
				return string.Empty;
			string strValorDiaria = lstValoresDiaria.Min(x => Convert.ToDecimal(x.Parametro.Value)).ToString("C");
			if (lstValoresDiaria.Count > 1) {
				strValorDiaria += " / " + lstValoresDiaria.Max(x => Convert.ToDecimal(x.Parametro.Value)).ToString("C");
			}
			return strValorDiaria;
		}

		public static List<ViagemValorVariavelVO> CalcularDiarias(List<SolicitacaoViagemItinerarioVO> passagens) {
			decimal qtdDiarias = 0;
			if (passagens != null && passagens.Count > 0) {
				DateTime partida = passagens.Min(x => x.DataPartida);
				DateTime chegada = passagens.Max(x => x.DataRetorno.AddHours(1));

				List<ParametroVariavelVO> lstParams = ParametroVariavelBO.Instance.GetParametroRange(ParametroUtil.ParametroVariavelType.VIAGEM_VALOR_DIARIA, partida, chegada);
				if (lstParams == null || lstParams.Count == 0) {
					throw new EvidaException("Não existem valores de diária configurados para o período de " + partida.ToShortDateString() + " - " + chegada.ToShortDateString());
				} else if (lstParams.Count > 2) {
					throw new EvidaException("O sistema não suporta mais de 2 vigências de diárias para a mesma viagem");
				}
				Dictionary<int, ViagemValorVariavelVO> mapVariavel = new Dictionary<int, ViagemValorVariavelVO>();
				for (DateTime dtContar = partida.Date; dtContar <= chegada.Date; dtContar = dtContar.AddDays(1)) {
					ParametroVariavelVO param = lstParams.Find(x => x.Inicio <= dtContar && x.Fim >= dtContar);
					if (param == null) {
						throw new EvidaException("Não existe valor de diária configurado para o dia " + dtContar.ToShortDateString());
					}
					ViagemValorVariavelVO valorVariavelVO = new ViagemValorVariavelVO();
					if (mapVariavel.ContainsKey(param.IdLinha)) {
						valorVariavelVO = mapVariavel[param.IdLinha];
						valorVariavelVO.Fim = dtContar;
						valorVariavelVO.Quantidade += 1;
					} else {
						valorVariavelVO.Parametro = param;
						valorVariavelVO.Quantidade = 1;
						valorVariavelVO.Inicio = dtContar;
						valorVariavelVO.Fim = dtContar;
						mapVariavel.Add(param.IdLinha, valorVariavelVO);
					}
				}

				qtdDiarias = chegada.Subtract(partida).Days;

				DateTime voltaNada = DateTime.Parse(chegada.ToString("dd/MM/yyyy") + " " + "12:00");
				DateTime voltaMeia = DateTime.Parse(chegada.ToString("dd/MM/yyyy") + " " + "20:00");
				decimal adicional = 0;
				if (chegada < voltaNada) {
					// não adiciona
				} else if (chegada < voltaMeia) {
					adicional += (decimal)0.5;
				} else {
					adicional += 1;
				}
				qtdDiarias += adicional;

				if (adicional != 0) {
					ParametroVariavelVO param = lstParams.Find(x => x.Inicio <= chegada.Date && x.Fim >= chegada.Date);
					ViagemValorVariavelVO valorVariavelVO = mapVariavel[param.IdLinha];
					valorVariavelVO.Quantidade += adicional;
				}

				return new List<ViagemValorVariavelVO>(mapVariavel.Values);
			}
			return new List<ViagemValorVariavelVO>();
		}

		internal static decimal CalcularQtdTotalDiarias(List<SolicitacaoViagemItinerarioVO> passagens) {
			List<ViagemValorVariavelVO> lst = CalcularDiarias(passagens);
			return CalcularQtdTotalDiarias(lst);
		}

		public static decimal CalcularQtdTotalDiarias(List<ViagemValorVariavelVO> lstDiarias) {
			if (lstDiarias != null && lstDiarias.Count > 0) {
				return lstDiarias.Sum(x => x.Quantidade);
			}
			return 0;
		}

		public static decimal CalcularVlrTotalDiarias(List<ViagemValorVariavelVO> lstDiarias, bool usarValor) {
			if (!usarValor) return 0;
			if (lstDiarias != null && lstDiarias.Count > 0) {
				return lstDiarias.Sum(x => x.ValorFinal);
			}
			return 0;
		}
	}
}
