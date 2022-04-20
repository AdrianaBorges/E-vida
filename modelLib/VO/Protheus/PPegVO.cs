using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class PPegVO
    {
        public string Filial { get; set; }
        public string Codope { get; set; }
        public string Codldp { get; set; }
        public string Codpeg { get; set; }
        public string Operda { get; set; }
        public string Codrda { get; set; }
        public string Nomrda { get; set; }
        public string Tipser { get; set; }
        public string Tipgui { get; set; }
        public Double Qtdeve { get; set; }
        public Double Qtdevd { get; set; }
        public Double Qtddig { get; set; }
        public Double Vlrgui { get; set; }
        public string Dtdigi { get; set; }
        public string Datrec { get; set; }
        public string Status { get; set; }
        public string Mes { get; set; }
        public string Nrimum { get; set; }
        public string Operes { get; set; }
        public string Ano { get; set; }
        public string Codcor { get; set; }
        public string Fase { get; set; }
        public Double Yvlcon { get; set; }
        public string Situac { get; set; }
        public Double Yvlpag { get; set; }
        public string Tipo { get; set; }
        public Double Yvlglo { get; set; }
        public string Tippre { get; set; }
        public string Arquiv { get; set; }
        public string Protoc { get; set; }
        public string Guipap { get; set; }
        public string Datgui { get; set; }
        public string Coddat { get; set; }
        public string Datpag { get; set; }
        public string Lotgui { get; set; }
        public string Proces { get; set; }
        public string Origem { get; set; }
        public string Ytpdoc { get; set; }
        public string Ydoc { get; set; }
        public string Yserie { get; set; }
        public Double Yvldoc { get; set; }
        public string Yemiss { get; set; }
        public string Yredoc { get; set; }
        public string Yvedoc { get; set; }
        public string Usergi { get; set; }
        public string Userga { get; set; }
        public string Seqnfs { get; set; }
        public string Cgcrda { get; set; }
        public string Codglo { get; set; }
        public Double Vlrglo { get; set; }
        public string Sitrg { get; set; }
        public string Digoff { get; set; }
        public string Sttiss { get; set; }
        public string Ynfant { get; set; }
        public string Zflgpr { get; set; }
        public string Usrlib { get; set; }
        public string Dthrlb { get; set; }
        public string Idxml { get; set; }
        public Double Vlrapr { get; set; }
        public Double Valori { get; set; }

        public static PPegVO FromDataRow(DataRow dr)
        {
            PPegVO vo = new PPegVO();

            vo.Filial = dr["BCI_FILIAL"] != DBNull.Value ? dr.Field<string>("BCI_FILIAL") : String.Empty;
            vo.Codope = dr["BCI_CODOPE"] != DBNull.Value ? dr.Field<string>("BCI_CODOPE") : String.Empty;
            vo.Codldp = dr["BCI_CODLDP"] != DBNull.Value ? dr.Field<string>("BCI_CODLDP") : String.Empty;
            vo.Codpeg = dr["BCI_CODPEG"] != DBNull.Value ? dr.Field<string>("BCI_CODPEG") : String.Empty;
            vo.Operda = dr["BCI_OPERDA"] != DBNull.Value ? dr.Field<string>("BCI_OPERDA") : String.Empty;
            vo.Codrda = dr["BCI_CODRDA"] != DBNull.Value ? dr.Field<string>("BCI_CODRDA") : String.Empty;
            vo.Nomrda = dr["BCI_NOMRDA"] != DBNull.Value ? dr.Field<string>("BCI_NOMRDA") : String.Empty;
            vo.Tipser = dr["BCI_TIPSER"] != DBNull.Value ? dr.Field<string>("BCI_TIPSER") : String.Empty;
            vo.Tipgui = dr["BCI_TIPGUI"] != DBNull.Value ? dr.Field<string>("BCI_TIPGUI") : String.Empty;
            vo.Qtdeve = dr["BCI_QTDEVE"] != DBNull.Value ? Convert.ToDouble(dr["BCI_QTDEVE"]) : Double.MinValue;
            vo.Qtdevd = dr["BCI_QTDEVD"] != DBNull.Value ? Convert.ToDouble(dr["BCI_QTDEVD"]) : Double.MinValue;
            vo.Qtddig = dr["BCI_QTDDIG"] != DBNull.Value ? Convert.ToDouble(dr["BCI_QTDDIG"]) : Double.MinValue;
            vo.Vlrgui = dr["BCI_VLRGUI"] != DBNull.Value ? Convert.ToDouble(dr["BCI_VLRGUI"]) : Double.MinValue;
            vo.Dtdigi = dr["BCI_DTDIGI"] != DBNull.Value ? dr.Field<string>("BCI_DTDIGI") : String.Empty;
            vo.Datrec = dr["BCI_DATREC"] != DBNull.Value ? dr.Field<string>("BCI_DATREC") : String.Empty;
            vo.Status = dr["BCI_STATUS"] != DBNull.Value ? dr.Field<string>("BCI_STATUS") : String.Empty;
            vo.Mes = dr["BCI_MES"] != DBNull.Value ? dr.Field<string>("BCI_MES") : String.Empty;
            vo.Nrimum = dr["BCI_NRIMUN"] != DBNull.Value ? dr.Field<string>("BCI_NRIMUN") : String.Empty;
            vo.Operes = dr["BCI_OPERES"] != DBNull.Value ? dr.Field<string>("BCI_OPERES") : String.Empty;
            vo.Ano = dr["BCI_ANO"] != DBNull.Value ? dr.Field<string>("BCI_ANO") : String.Empty;
            vo.Codcor = dr["BCI_CODCOR"] != DBNull.Value ? dr.Field<string>("BCI_CODCOR") : String.Empty;
            vo.Fase = dr["BCI_FASE"] != DBNull.Value ? dr.Field<string>("BCI_FASE") : String.Empty;
            vo.Yvlcon = dr["BCI_YVLCON"] != DBNull.Value ? Convert.ToDouble(dr["BCI_YVLCON"]) : Double.MinValue;
            vo.Situac = dr["BCI_SITUAC"] != DBNull.Value ? dr.Field<string>("BCI_SITUAC") : String.Empty;
            vo.Yvlpag = dr["BCI_YVLPAG"] != DBNull.Value ? Convert.ToDouble(dr["BCI_YVLPAG"]) : Double.MinValue;
            vo.Tipo = dr["BCI_TIPO"] != DBNull.Value ? dr.Field<string>("BCI_TIPO") : String.Empty;
            vo.Yvlglo = dr["BCI_YVLGLO"] != DBNull.Value ? Convert.ToDouble(dr["BCI_YVLGLO"]) : Double.MinValue;
            vo.Tippre = dr["BCI_TIPPRE"] != DBNull.Value ? dr.Field<string>("BCI_TIPPRE") : String.Empty;
            vo.Arquiv = dr["BCI_ARQUIV"] != DBNull.Value ? dr.Field<string>("BCI_ARQUIV") : String.Empty;
            vo.Protoc = dr["BCI_PROTOC"] != DBNull.Value ? dr.Field<string>("BCI_PROTOC") : String.Empty;
            vo.Guipap = dr["BCI_GUIPAP"] != DBNull.Value ? dr.Field<string>("BCI_GUIPAP") : String.Empty;
            vo.Datgui = dr["BCI_DATGUI"] != DBNull.Value ? dr.Field<string>("BCI_DATGUI") : String.Empty;
            vo.Coddat = dr["BCI_CODDAT"] != DBNull.Value ? dr.Field<string>("BCI_CODDAT") : String.Empty;
            vo.Datpag = dr["BCI_DATPAG"] != DBNull.Value ? dr.Field<string>("BCI_DATPAG") : String.Empty;
            vo.Lotgui = dr["BCI_LOTGUI"] != DBNull.Value ? dr.Field<string>("BCI_LOTGUI") : String.Empty;
            vo.Proces = dr["BCI_PROCES"] != DBNull.Value ? dr.Field<string>("BCI_PROCES") : String.Empty;
            vo.Origem = dr["BCI_ORIGEM"] != DBNull.Value ? dr.Field<string>("BCI_ORIGEM") : String.Empty;
            vo.Ytpdoc = dr["BCI_YTPDOC"] != DBNull.Value ? dr.Field<string>("BCI_YTPDOC") : String.Empty;
            vo.Ydoc = dr["BCI_YDOC"] != DBNull.Value ? dr.Field<string>("BCI_YDOC") : String.Empty;
            vo.Yserie = dr["BCI_YSERIE"] != DBNull.Value ? dr.Field<string>("BCI_YSERIE") : String.Empty;
            vo.Yvldoc = dr["BCI_YVLDOC"] != DBNull.Value ? Convert.ToDouble(dr["BCI_YVLDOC"]) : Double.MinValue;
            vo.Yemiss = dr["BCI_YEMISS"] != DBNull.Value ? dr.Field<string>("BCI_YEMISS") : String.Empty;
            vo.Yredoc = dr["BCI_YREDOC"] != DBNull.Value ? dr.Field<string>("BCI_YREDOC") : String.Empty;
            vo.Yvedoc = dr["BCI_YVEDOC"] != DBNull.Value ? dr.Field<string>("BCI_YVEDOC") : String.Empty;
            vo.Usergi = dr["BCI_USERGI"] != DBNull.Value ? dr.Field<string>("BCI_USERGI") : String.Empty;
            vo.Userga = dr["BCI_USERGA"] != DBNull.Value ? dr.Field<string>("BCI_USERGA") : String.Empty;
            vo.Seqnfs = dr["BCI_SEQNFS"] != DBNull.Value ? dr.Field<string>("BCI_SEQNFS") : String.Empty;
            vo.Cgcrda = dr["BCI_CGCRDA"] != DBNull.Value ? dr.Field<string>("BCI_CGCRDA") : String.Empty;
            vo.Codglo = dr["BCI_CODGLO"] != DBNull.Value ? dr.Field<string>("BCI_CODGLO") : String.Empty;
            vo.Vlrglo = dr["BCI_VLRGLO"] != DBNull.Value ? Convert.ToDouble(dr["BCI_VLRGLO"]) : Double.MinValue;
            vo.Sitrg = dr["BCI_SITRG"] != DBNull.Value ? dr.Field<string>("BCI_SITRG") : String.Empty;
            vo.Digoff = dr["BCI_DIGOFF"] != DBNull.Value ? dr.Field<string>("BCI_DIGOFF") : String.Empty;
            vo.Sttiss = dr["BCI_STTISS"] != DBNull.Value ? dr.Field<string>("BCI_STTISS") : String.Empty;
            vo.Ynfant = dr["BCI_YNFANT"] != DBNull.Value ? dr.Field<string>("BCI_YNFANT") : String.Empty;
            vo.Zflgpr = dr["BCI_ZFLGPR"] != DBNull.Value ? dr.Field<string>("BCI_ZFLGPR") : String.Empty;
            vo.Usrlib = dr["BCI_USRLIB"] != DBNull.Value ? dr.Field<string>("BCI_USRLIB") : String.Empty;
            vo.Dthrlb = dr["BCI_DTHRLB"] != DBNull.Value ? dr.Field<string>("BCI_DTHRLB") : String.Empty;
            vo.Idxml = dr["BCI_IDXML"] != DBNull.Value ? dr.Field<string>("BCI_IDXML") : String.Empty;
            vo.Vlrapr = dr["BCI_VLRAPR"] != DBNull.Value ? Convert.ToDouble(dr["BCI_VLRAPR"]) : Double.MinValue;
            vo.Valori = dr["BCI_VALORI"] != DBNull.Value ? Convert.ToDouble(dr["BCI_VALORI"]) : Double.MinValue;

            return vo;
        }

    }
}
