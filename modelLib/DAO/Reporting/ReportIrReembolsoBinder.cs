using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO.Protheus;

namespace eVidaGeneralLib.Reporting
{
    public class ReportIrReembolsoBinder : IReportBinder
    {

        public class ParamsVO
        {
            public string Carteira { get; set; }
            public int AnoRef { get; set; }
        }

        private ParamsVO vo;

        public ReportIrReembolsoBinder(ParamsVO vo)
        {
            this.vo = vo;
        }

        public ReportBinderParams GetData()
        {
            ReportBinderParams repParam = new ReportBinderParams();

            string codint = vo.Carteira.Substring(0, 4);
            string codemp = vo.Carteira.Substring(4, 4);
            string matric = vo.Carteira.Substring(8, 6);

            PFamiliaVO func = PFamiliaBO.Instance.GetByMatricula(codint, codemp, matric);
            if (func == null)
            {
                throw new Exception("Funcionário inválido:" + codint + '/' + codemp + '/' + matric);
            }

            string local = "-";
            string lotacao = "-";

            if (!string.IsNullOrEmpty(func.Yregio.Trim()))
            {
                local = func.Yregio + " - " + PLocatorDataBO.Instance.GetRegiao(func.Yregio);
            }

            //DateTime dataManifesto = new DateTime(vo.AnoRef + 1, 3, 1);
            //dataManifesto = DateUtil.PreviousBusinessDay(dataManifesto);
            //if (dataManifesto.Year == 2017) {
            //	dataManifesto = DateUtil.PreviousBusinessDay(dataManifesto);
            //}
            VO.ConfiguracaoIrVO configIR = ConfiguracaoIrBO.Instance.GetConfiguracao();
            DateTime dataManifesto = new DateTime(vo.AnoRef + 1, 2, configIR.DayIrBeneficiario);

            PUsuarioVO titular = PUsuarioBO.Instance.GetTitular(func.Codint, func.Codemp, func.Matric);
            string nomeTitular = "";
            string cpfTitular = "";
            string carteiraTitular = "";
            if (titular != null)
            {
                nomeTitular = titular.Nomusr;
                cpfTitular = FormatUtil.FormatCpf(titular.Cpfusr);
                carteiraTitular = titular.GetCarteira();
            }

            repParam.Params.Add("AnoRef", vo.AnoRef + "");
            repParam.Params.Add("NomeTitular", nomeTitular);
            repParam.Params.Add("Cpf", cpfTitular);
            repParam.Params.Add("Matricula", carteiraTitular);
            repParam.Params.Add("DataCriacao", FormatUtil.FormatDataExtenso(dataManifesto));

            repParam.Params.Add("Inicio", (new DateTime(vo.AnoRef, 1, 1)).ToShortDateString());
            repParam.Params.Add("Fim", (new DateTime(vo.AnoRef, 12, 31)).ToShortDateString());
            repParam.Params.Add("Local", local);
            repParam.Params.Add("Lotacao", lotacao);

            DataTable dt = ExtratoIrBeneficiarioBO.Instance.RelatorioReembolsoIrTable(vo.Carteira, vo.AnoRef);
            repParam.DataSources.Add("dsMensalidade", dt);
            return repParam;
        }	

        public string GerarNome()
        {
            return "REEMBOLSO_" + vo.Carteira.Trim() + "_" + vo.AnoRef;
        }

        public string DefaultRpt()
        {
            return "rptIrReembolso";
        }
    }
}
