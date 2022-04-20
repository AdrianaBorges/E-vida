using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using eVida.Web.Security;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using SkyReport.ReportingServices;
using eVidaGeneralLib.Reporting;

namespace eVida.Web.Report
{
    public class ReportIrReembolso : ReportBase<ReportIrReembolsoBinder.ParamsVO, ReportIrReembolsoBinder>
    {

        public ReportIrReembolso(string reportDir, IUsuarioLogado usuario)
            : base(reportDir, usuario)
        {
        }

        internal override ReportIrReembolsoBinder.ParamsVO GerarDados(HttpRequest request)
        {
            int ano;
            if (request["ANO"] == null || !Int32.TryParse(request["ANO"], out ano))
                return null;
            else
            {
                string carteira;
                UsuarioBeneficiarioVO uVO = Usuario as UsuarioBeneficiarioVO;
                //Se a requisição nao foi pelo beneficiário, então deve ter o parametro da matrícula
                if (uVO == null)
                {
                    if (request["CARTAO_TITULAR"] == null)
                    {
                        return null;
                    }
                    else
                    {
                        carteira = request["CARTAO_TITULAR"].ToString();
                    }
                }
                else
                {
                    carteira = uVO.UsuarioTitular.GetCarteira();
                }
                return new ReportIrReembolsoBinder.ParamsVO()
                {
                    AnoRef = ano,
                    Carteira = carteira
                };
            }
        }

        protected override ReportIrReembolsoBinder CreateBinder(ReportIrReembolsoBinder.ParamsVO vo)
        {
            return new ReportIrReembolsoBinder(vo);
        }
    }
}