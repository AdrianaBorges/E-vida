using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;	

namespace eVidaGeneralLib.Reporting {
	public class ReportViagemBinder : IReportBinder {
		SolicitacaoViagemVO vo;

		public ReportViagemBinder(SolicitacaoViagemVO vo) {
			this.vo = vo;
		}

        public ReportBinderParams GetData()
        {
            ReportBinderParams repParams = new ReportBinderParams();

            decimal valorAdiantamento = vo.Solicitacao.ValorAdiantamento != null ? vo.Solicitacao.ValorAdiantamento.Value : 0;

            repParams.Params.Add("NumSolicitacao", vo.Id.ToString());
            repParams.Params.Add("Requisitante", vo.Solicitacao.Nome);
            repParams.Params.Add("Matricula", vo.IsExterno ? "EXTERNO" : vo.Empregado.Matricula.ToString());
            repParams.Params.Add("CPF", FormatUtil.FormatCpf(vo.Solicitacao.Cpf));
            repParams.Params.Add("Cargo", vo.Solicitacao.Cargo.ToReportString());
            repParams.Params.Add("Telefone", vo.Solicitacao.Telefone.ToReportString());

            HcBancoVO banco = LocatorDataBO.Instance.GetBanco(vo.Solicitacao.Banco.Id);
            repParams.Params.Add("Banco", banco.Id.ToString("000") + " - " + banco.Nome);
            repParams.Params.Add("Agencia", vo.Solicitacao.Agencia.ToReportString());
            repParams.Params.Add("ContaCorrente", vo.Solicitacao.ContaCorrente.ToReportString());

            repParams.Params.Add("Objetivo", vo.Solicitacao.Objetivo.ToReportString());
            repParams.Params.Add("ValorAdiantamento", valorAdiantamento != 0 ? valorAdiantamento.ToString("C") : " - ");
            repParams.Params.Add("JustificativaAdiantamento", vo.Solicitacao.JustificativaAdiantamento.ToReportString());

            List<ViagemValorVariavelVO> lstValoresDiaria = ViagemHelper.CalcularDiarias(vo.Compra.Passagens);
            string strValorDiaria = ViagemHelper.GetStringValoresDiaria(lstValoresDiaria);

            Decimal qtdDiarias = ViagemHelper.CalcularQtdTotalDiarias(lstValoresDiaria);
            Decimal valorDiaria = Decimal.Parse(strValorDiaria.Replace("R$ ", ""));
            Decimal totalDiaria = qtdDiarias * valorDiaria;

            repParams.Params.Add("QuantidadeDiaria", ViagemHelper.CalcularQtdTotalDiarias(lstValoresDiaria).ToString());
            repParams.Params.Add("ValorDiaria", strValorDiaria);
            repParams.Params.Add("TotalDiaria", totalDiaria.ToString("C"));
            repParams.Params.Add("DataSolicitacao", vo.DataCriacao.ToShortDateString());
            repParams.Params.Add("SuperiorImediato", vo.AprovSolicitacaoCoordenador.IdUsuario == 0 ? " - " : GetUsuarioEvida(vo.AprovSolicitacaoCoordenador.IdUsuario));
            repParams.Params.Add("DiretorAprovSolicitacao", GetUsuarioEvida(vo.AprovSolicitacaoDiretoria.IdUsuario));
            repParams.Params.Add("MeiosTransporte", vo.Solicitacao.MeioTransporte.Select(x => x.ToString()).Aggregate((x, y) => x + " " + y));
            repParams.DataSources.Add("dsItinerario", CreateItinerario(vo.Solicitacao.Itinerarios));
            repParams.DataSources.Add("dsDespesaEvida", CreateDespEvida(lstValoresDiaria, vo.Compra));
            repParams.DataSources.Add("dsPrestacaoConta", CreatePrestacaoConta(vo.PrestacaoConta));

            decimal valorGasto = vo.PrestacaoConta.DespesasDetalhadas.Sum(x => x.Valor);
            repParams.Params.Add("ValorDespesaFuncionario", valorGasto.ToString("C"));

            string tipoParecer;
            decimal diferenca = valorGasto - valorAdiantamento;
            if (diferenca >= 0)
            {
                tipoParecer = "VALOR A SER COMPLEMENTADO (REEMBOLSO)";
            }
            else
            {
                tipoParecer = "VALOR A SER RESTITUÍDO PARA E-VIDA (DEVOLUÇÃO)";
            }
            repParams.Params.Add("TipoParecer", tipoParecer);
            repParams.Params.Add("ValorParecer", Math.Abs(diferenca).ToString("C"));

            decimal valorPassagens = vo.Compra.Passagens.Sum(x => x.Valor != null ? x.Valor.Value : 0);
            decimal valorHoteis = vo.Compra.Hoteis.Sum(x => x.Valor != null ? x.Valor.Value : 0);
            decimal valorDiarias = ViagemHelper.CalcularVlrTotalDiarias(lstValoresDiaria, vo.Solicitacao.UsaValorDiaria);

            decimal valorEvida = valorPassagens + valorHoteis + valorDiarias;

            decimal valorTotal = valorEvida + (diferenca);
            repParams.Params.Add("ValorTotal", valorTotal.ToString("C"));

            repParams.Params.Add("DataConfSefin", vo.DataSituacao.ToShortDateString());
            repParams.Params.Add("ResumoViagem", vo.PrestacaoConta.ResumoViagem.ToReportString());

            return repParams;
        }

        private string GetUsuarioEvida(int idUsuario)
        {
            UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(idUsuario);
            EmpregadoEvidaVO empregado = EmpregadoEvidaBO.Instance.GetByMatricula((long)usuario.Matricula);
            return empregado.Matricula + " - " + empregado.Nome;
        }	

		private DataTable CreatePrestacaoConta(SolicitacaoViagemInfoPrestacaoContaVO prestConta) {
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id"));
			dt.Columns.Add(new DataColumn("Grupo"));
			dt.Columns.Add(new DataColumn("Data"));
			dt.Columns.Add(new DataColumn("Produto"));
			dt.Columns.Add(new DataColumn("Descricao"));
			dt.Columns.Add(new DataColumn("Valor"));

			foreach (SolicitacaoViagemDespesaDetalhadaVO item in prestConta.DespesasDetalhadas) {
				DataRow dr = dt.NewRow();
				dr["Id"] = "D" + item.IdDespesa;
				dr["Grupo"] = SolicitacaoViagemEnumTradutor.TraduzGrupoDespPrestConta(item.GrupoDespesa);

				string produto = SolicitacaoViagemEnumTradutor.TraduzTipoDespPrestConta(item.TipoDespesa);
				if (item.TipoDespesa == TipoDespesaPrestContaViagem.OUTROS) {
					produto += " - " + item.DescricaoTipoDespesa;
				}
				dr["Produto"] = produto;

				string descricao = item.Descricao + " - " + item.Data.ToShortDateString();
				dr["Descricao"] = descricao;
				dr["Valor"] = item.Valor;
				dt.Rows.Add(dr);
			}

			return dt;
		}

		private DataTable CreateDespEvida(List<ViagemValorVariavelVO> valoresDiarias, SolicitacaoViagemInfoCompraVO infoCompra) {
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id"));
			dt.Columns.Add(new DataColumn("Grupo"));
			dt.Columns.Add(new DataColumn("Data"));
			dt.Columns.Add(new DataColumn("Produto"));
			dt.Columns.Add(new DataColumn("Descricao"));
			dt.Columns.Add(new DataColumn("Valor"));

			foreach (SolicitacaoViagemItinerarioVO item in infoCompra.Passagens) {
				DataRow dr = dt.NewRow();
				dr["Id"] = "P" + item.IdItinerario;
				dr["Grupo"] = "TRANSPORTE";

				string[] compls = item.Complemento.Split(new char[] { ';' });
				MeioTransporteViagem meio = (MeioTransporteViagem)Int32.Parse(compls[0]);

				dr["Produto"] = SolicitacaoViagemEnumTradutor.TraduzMeioTransporte(meio);
				//vo.Complemento = ((int)meioTransporte).ToString() + ";" + kmRodado;

				string descricao = item.Origem + " / " + item.Destino + " - " + item.DataPartida.ToShortDateString();
				if (meio == MeioTransporteViagem.CARRO_PROPRIO) {
					descricao += " - " + compls[1] + " KMs";
				}
				dr["Descricao"] = descricao;
				dr["Valor"] = item.Valor.Value;
				dt.Rows.Add(dr);
			}

			foreach (SolicitacaoViagemItinerarioVO item in infoCompra.Hoteis) {
				DataRow dr = dt.NewRow();
				dr["Id"] = "H" + item.IdItinerario;
				dr["Grupo"] = "HOSPEDAGEM, DIÁRIAS E CURSOS";

				dr["Produto"] = "HOTEL";

				string descricao = item.Origem + " - " + item.DataPartida.ToShortDateString() + " / " + item.DataRetorno.ToShortDateString();

				dr["Descricao"] = descricao;
				dr["Valor"] = item.Valor.Value;
				dt.Rows.Add(dr);
			}

			foreach (ViagemValorVariavelVO valorDiaria in valoresDiarias) {
				DataRow dr = dt.NewRow();
				dr["Id"] = "D" + valorDiaria.Parametro.IdLinha;
				dr["Grupo"] = "HOSPEDAGEM, DIÁRIAS E CURSOS";

				dr["Produto"] = "DIÁRIAS";

				string descricao = "TOTAL DE DIÁRIAS: " + valorDiaria.Quantidade + " (" + valorDiaria.Inicio.ToShortDateString() + " - " + valorDiaria.Fim.ToShortDateString() + ")";

				dr["Descricao"] = descricao;
				dr["Valor"] = valorDiaria.ValorFinal;
				dt.Rows.Add(dr);
			}

			return dt;

		}

		private string GetUsuarioStr(int idUsuario) {
			UsuarioVO usuario = UsuarioBO.Instance.GetUsuarioById(idUsuario);
			return usuario.Login + " - " + usuario.Nome;
		}

		private DataTable CreateItinerario(List<SolicitacaoViagemItinerarioVO> lst) {
			DataTable dtItinerario = new DataTable();
			dtItinerario.Columns.Add(new DataColumn("Id"));
			dtItinerario.Columns.Add(new DataColumn("Origem"));
			dtItinerario.Columns.Add(new DataColumn("Destino"));
			dtItinerario.Columns.Add(new DataColumn("DataIda"));
			dtItinerario.Columns.Add(new DataColumn("DataVolta"));

			foreach (SolicitacaoViagemItinerarioVO vo in lst) {
				DataRow dr = dtItinerario.NewRow();
				dr["Id"] = vo.IdItinerario.ToString();
				dr["Origem"] = vo.Origem;
				dr["Destino"] = vo.Destino;
				dr["DataIda"] = vo.DataPartida.ToShortDateString();
				dr["DataVolta"] = vo.DataRetorno.ToShortDateString();
				dtItinerario.Rows.Add(dr);
			}

			return dtItinerario;
		}

		public string GerarNome() {
			return "RELATORIO_VIAGEM_" + vo.Id;
		}

		public string DefaultRpt() {
			return "rptViagem";
		}
	}
}
