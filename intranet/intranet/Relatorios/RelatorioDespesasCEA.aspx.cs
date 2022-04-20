using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eVidaIntranet.Classes;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Util;
using SkyReport.ExcelExporter;

namespace eVidaIntranet.Relatorios
{
    public partial class RelatorioDespesasCEA : RelatorioExcelPageBase
    {
        protected override void PageLoad(object sender, EventArgs e)
        {
        }

        protected override eVidaGeneralLib.VO.Modulo Modulo
        {
            get { return eVidaGeneralLib.VO.Modulo.RELATORIO_DESPESAS_CEA; }
        }

    }
}