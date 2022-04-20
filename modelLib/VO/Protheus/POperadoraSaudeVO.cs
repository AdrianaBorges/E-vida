using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Protheus
{
    [Serializable]
    public class POperadoraSaudeVO
    {
        public string Filial { get; set; }
        public string Codide { get; set; }
        public string Codint { get; set; }
        public string Nomint { get; set; }
        public string Claint { get; set; }
        public string Gruope { get; set; }
        public string Camcom { get; set; }
        public string Susep { get; set; }
        public string Cgc { get; set; }
        public string Incest { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public string Datfun { get; set; }
        public string Cep { get; set; }
        public string End { get; set; }
        public string Numend { get; set; }
        public string Compen { get; set; }
        public string Bairro { get; set; }
        public string Codmun { get; set; }
        public string Cidade { get; set; }
        public string Est { get; set; }
        public string Telef1 { get; set; }
        public string Acopul { get; set; }
        public string Telef2 { get; set; }
        public string Ddd { get; set; }
        public string Ramal1 { get; set; }
        public string Telef3 { get; set; }
        public Double Totaco { get; set; }
        public string Fax1 { get; set; }
        public string Fax3 { get; set; }
        public string Ramal2 { get; set; }
        public string Fax2 { get; set; }
        public string Ramal3 { get; set; }
        public string Matfil { get; set; }
        public string Modope { get; set; }
        public string Codfor { get; set; }
        public string Loja { get; set; }
        public string Tbrfre { get; set; }
        public string Codtab { get; set; }
        public string Senqui { get; set; }
        public string Senrad { get; set; }
        public string Codcli { get; set; }        
        public string Senopm { get; set; }
        public string Lojcli { get; set; }
        public string Nature { get; set; }
        public string Tipope { get; set; }
        public string Expide { get; set; }
        public string Nomcar { get; set; }
        public Double Vl2bol { get; set; }
        public Double Vlcsop { get; set; }
        public Double Vencto { get; set; }
        public string Tipven { get; set; }
        public Double Vencus { get; set; }
        public string Tipcus { get; set; }
        public string Envptu { get; set; }
        public string Recptu { get; set; }
        public string Emaptu { get; set; }
        public string Resptu { get; set; }
        public string Codrda { get; set; }
        public string Online { get; set; }
        public Double Diaret { get; set; }
        public string A100 { get; set; }        
        public string A300 { get; set; }
        public string A600 { get; set; }
        public string A700 { get; set; }
        public Double Limch { get; set; }
        public string Tiplim { get; set; }
        public string Nivval { get; set; }
        public string Gnt { get; set; }
        public string Bloino { get; set; }
        public string Tippag { get; set; }
        public string Bcocli { get; set; }
        public string Agecli { get; set; }
        public string Ctacli { get; set; }
        public string Portad { get; set; }
        public string Agedep { get; set; }
        public string Ctacor { get; set; }
        public string Empori { get; set; }
        public string Bascop { get; set; }
        public string Abrang { get; set; }
        public string Natjur { get; set; }
        public string Modali { get; set; }        
        public string Segmen { get; set; }
        public string Codreg { get; set; }
        public string Tpoped { get; set; }
        public string Tisver { get; set; }
        public Double Maxrg { get; set; }
        public Double Przrec { get; set; }
        public string Criprz { get; set; }
        public string Tppag { get; set; }
        public string Tabprz { get; set; }
        public string Crilim { get; set; }
        public string Tablim { get; set; }
        public string Autges { get; set; }
        public string Liecon { get; set; }
        public string Digane { get; set; }
        public string Zverfr { get; set; }
        public string Vlrapr { get; set; }

        public static POperadoraSaudeVO FromDataRow(DataRow dr)
        {
            POperadoraSaudeVO vo = new POperadoraSaudeVO();

            vo.Filial = dr["BA0_FILIAL"] != DBNull.Value ? dr.Field<string>("BA0_FILIAL") : String.Empty;
            vo.Codide = dr["BA0_CODIDE"] != DBNull.Value ? dr.Field<string>("BA0_CODIDE") : String.Empty;
            vo.Codint = dr["BA0_CODINT"] != DBNull.Value ? dr.Field<string>("BA0_CODINT") : String.Empty;
            vo.Nomint = dr["BA0_NOMINT"] != DBNull.Value ? dr.Field<string>("BA0_NOMINT") : String.Empty;
            vo.Claint = dr["BA0_CLAINT"] != DBNull.Value ? dr.Field<string>("BA0_CLAINT") : String.Empty;
            vo.Gruope = dr["BA0_GRUOPE"] != DBNull.Value ? dr.Field<string>("BA0_GRUOPE") : String.Empty;
            vo.Camcom = dr["BA0_CAMCOM"] != DBNull.Value ? dr.Field<string>("BA0_CAMCOM") : String.Empty;
            vo.Susep = dr["BA0_SUSEP"] != DBNull.Value ? dr.Field<string>("BA0_SUSEP") : String.Empty;
            vo.Cgc = dr["BA0_CGC"] != DBNull.Value ? dr.Field<string>("BA0_CGC") : String.Empty;
            vo.Incest = dr["BA0_INCEST"] != DBNull.Value ? dr.Field<string>("BA0_INCEST") : String.Empty;
            vo.Email = dr["BA0_EMAIL"] != DBNull.Value ? dr.Field<string>("BA0_EMAIL") : String.Empty;
            vo.Site = dr["BA0_SITE"] != DBNull.Value ? dr.Field<string>("BA0_SITE") : String.Empty;
            vo.Datfun = dr["BA0_DATFUN"] != DBNull.Value ? dr.Field<string>("BA0_DATFUN") : String.Empty;
            vo.Cep = dr["BA0_CEP"] != DBNull.Value ? dr.Field<string>("BA0_CEP") : String.Empty;
            vo.End = dr["BA0_END"] != DBNull.Value ? dr.Field<string>("BA0_END") : String.Empty;
            vo.Numend = dr["BA0_NUMEND"] != DBNull.Value ? dr.Field<string>("BA0_NUMEND") : String.Empty;
            vo.Compen = dr["BA0_COMPEN"] != DBNull.Value ? dr.Field<string>("BA0_COMPEN") : String.Empty;
            vo.Bairro = dr["BA0_BAIRRO"] != DBNull.Value ? dr.Field<string>("BA0_BAIRRO") : String.Empty;
            vo.Codmun = dr["BA0_CODMUN"] != DBNull.Value ? dr.Field<string>("BA0_CODMUN") : String.Empty;
            vo.Cidade = dr["BA0_CIDADE"] != DBNull.Value ? dr.Field<string>("BA0_CIDADE") : String.Empty;
            vo.Est = dr["BA0_EST"] != DBNull.Value ? dr.Field<string>("BA0_EST") : String.Empty;
            vo.Telef1 = dr["BA0_TELEF1"] != DBNull.Value ? dr.Field<string>("BA0_TELEF1") : String.Empty;
            vo.Acopul = dr["BA0_ACOPUL"] != DBNull.Value ? dr.Field<string>("BA0_ACOPUL") : String.Empty;
            vo.Telef2 = dr["BA0_TELEF2"] != DBNull.Value ? dr.Field<string>("BA0_TELEF2") : String.Empty;
            vo.Ddd = dr["BA0_DDD"] != DBNull.Value ? dr.Field<string>("BA0_DDD") : String.Empty;
            vo.Ramal1 = dr["BA0_RAMAL1"] != DBNull.Value ? dr.Field<string>("BA0_RAMAL1") : String.Empty;
            vo.Telef3 = dr["BA0_TELEF3"] != DBNull.Value ? dr.Field<string>("BA0_TELEF3") : String.Empty;
            vo.Totaco = dr["BA0_TOTACO"] != DBNull.Value ? Convert.ToDouble(dr["BA0_TOTACO"]) : 0.0;
            vo.Fax1 = dr["BA0_FAX1"] != DBNull.Value ? dr.Field<string>("BA0_FAX1") : String.Empty;
            vo.Fax3 = dr["BA0_FAX3"] != DBNull.Value ? dr.Field<string>("BA0_FAX3") : String.Empty;
            vo.Ramal2 = dr["BA0_RAMAL2"] != DBNull.Value ? dr.Field<string>("BA0_RAMAL2") : String.Empty;
            vo.Fax2 = dr["BA0_FAX2"] != DBNull.Value ? dr.Field<string>("BA0_FAX2") : String.Empty;
            vo.Ramal3 = dr["BA0_RAMAL3"] != DBNull.Value ? dr.Field<string>("BA0_RAMAL3") : String.Empty;
            vo.Matfil = dr["BA0_MATFIL"] != DBNull.Value ? dr.Field<string>("BA0_MATFIL") : String.Empty;
            vo.Modope = dr["BA0_MODOPE"] != DBNull.Value ? dr.Field<string>("BA0_MODOPE") : String.Empty;
            vo.Codfor = dr["BA0_CODFOR"] != DBNull.Value ? dr.Field<string>("BA0_CODFOR") : String.Empty;
            vo.Loja = dr["BA0_LOJA"] != DBNull.Value ? dr.Field<string>("BA0_LOJA") : String.Empty;
            vo.Tbrfre = dr["BA0_TBRFRE"] != DBNull.Value ? dr.Field<string>("BA0_TBRFRE") : String.Empty;
            vo.Codtab = dr["BA0_CODTAB"] != DBNull.Value ? dr.Field<string>("BA0_CODTAB") : String.Empty;
            vo.Senqui = dr["BA0_SENQUI"] != DBNull.Value ? dr.Field<string>("BA0_SENQUI") : String.Empty;
            vo.Senrad = dr["BA0_SENRAD"] != DBNull.Value ? dr.Field<string>("BA0_SENRAD") : String.Empty;
            vo.Codcli = dr["BA0_CODCLI"] != DBNull.Value ? dr.Field<string>("BA0_CODCLI") : String.Empty;
            vo.Senopm = dr["BA0_SENOPM"] != DBNull.Value ? dr.Field<string>("BA0_SENOPM") : String.Empty;
            vo.Lojcli = dr["BA0_LOJCLI"] != DBNull.Value ? dr.Field<string>("BA0_LOJCLI") : String.Empty;
            vo.Nature = dr["BA0_NATURE"] != DBNull.Value ? dr.Field<string>("BA0_NATURE") : String.Empty;
            vo.Tipope = dr["BA0_TIPOPE"] != DBNull.Value ? dr.Field<string>("BA0_TIPOPE") : String.Empty;
            vo.Expide = dr["BA0_EXPIDE"] != DBNull.Value ? dr.Field<string>("BA0_EXPIDE") : String.Empty;
            vo.Nomcar = dr["BA0_NOMCAR"] != DBNull.Value ? dr.Field<string>("BA0_NOMCAR") : String.Empty;
            vo.Vl2bol = dr["BA0_VL2BOL"] != DBNull.Value ? Convert.ToDouble(dr["BA0_VL2BOL"]) : 0.0;
            vo.Vlcsop = dr["BA0_VLCSOP"] != DBNull.Value ? Convert.ToDouble(dr["BA0_VLCSOP"]) : 0.0;
            vo.Vencto = dr["BA0_VENCTO"] != DBNull.Value ? Convert.ToDouble(dr["BA0_VENCTO"]) : 0.0;
            vo.Tipven = dr["BA0_TIPVEN"] != DBNull.Value ? dr.Field<string>("BA0_TIPVEN") : String.Empty;
            vo.Vencus = dr["BA0_VENCUS"] != DBNull.Value ? Convert.ToDouble(dr["BA0_VENCUS"]) : 0.0;
            vo.Tipcus = dr["BA0_TIPCUS"] != DBNull.Value ? dr.Field<string>("BA0_TIPCUS") : String.Empty;
            vo.Envptu = dr["BA0_ENVPTU"] != DBNull.Value ? dr.Field<string>("BA0_ENVPTU") : String.Empty;
            vo.Recptu = dr["BA0_RECPTU"] != DBNull.Value ? dr.Field<string>("BA0_RECPTU") : String.Empty;
            vo.Emaptu = dr["BA0_EMAPTU"] != DBNull.Value ? dr.Field<string>("BA0_EMAPTU") : String.Empty;
            vo.Resptu = dr["BA0_RESPTU"] != DBNull.Value ? dr.Field<string>("BA0_RESPTU") : String.Empty;
            vo.Codrda = dr["BA0_CODRDA"] != DBNull.Value ? dr.Field<string>("BA0_CODRDA") : String.Empty;
            vo.Online = dr["BA0_ONLINE"] != DBNull.Value ? dr.Field<string>("BA0_ONLINE") : String.Empty;
            vo.Diaret = dr["BA0_DIARET"] != DBNull.Value ? Convert.ToDouble(dr["BA0_DIARET"]) : 0.0;
            vo.A100 = dr["BA0_A100"] != DBNull.Value ? dr.Field<string>("BA0_A100") : String.Empty;
            vo.A300 = dr["BA0_A300"] != DBNull.Value ? dr.Field<string>("BA0_A300") : String.Empty;
            vo.A600 = dr["BA0_A600"] != DBNull.Value ? dr.Field<string>("BA0_A600") : String.Empty;
            vo.A700 = dr["BA0_A700"] != DBNull.Value ? dr.Field<string>("BA0_A700") : String.Empty;
            vo.Limch = dr["BA0_LIMCH"] != DBNull.Value ? Convert.ToDouble(dr["BA0_LIMCH"]) : 0.0;
            vo.Tiplim = dr["BA0_TIPLIM"] != DBNull.Value ? dr.Field<string>("BA0_TIPLIM") : String.Empty;
            vo.Nivval = dr["BA0_NIVVAL"] != DBNull.Value ? dr.Field<string>("BA0_NIVVAL") : String.Empty;
            vo.Gnt = dr["BA0_GNT"] != DBNull.Value ? dr.Field<string>("BA0_GNT") : String.Empty;
            vo.Bloino = dr["BA0_BLOINO"] != DBNull.Value ? dr.Field<string>("BA0_BLOINO") : String.Empty;
            vo.Tippag = dr["BA0_TIPPAG"] != DBNull.Value ? dr.Field<string>("BA0_TIPPAG") : String.Empty;
            vo.Bcocli = dr["BA0_BCOCLI"] != DBNull.Value ? dr.Field<string>("BA0_BCOCLI") : String.Empty;
            vo.Agecli = dr["BA0_AGECLI"] != DBNull.Value ? dr.Field<string>("BA0_AGECLI") : String.Empty;
            vo.Ctacli = dr["BA0_CTACLI"] != DBNull.Value ? dr.Field<string>("BA0_CTACLI") : String.Empty;
            vo.Portad = dr["BA0_PORTAD"] != DBNull.Value ? dr.Field<string>("BA0_PORTAD") : String.Empty;
            vo.Agedep = dr["BA0_AGEDEP"] != DBNull.Value ? dr.Field<string>("BA0_AGEDEP") : String.Empty;
            vo.Ctacor = dr["BA0_CTACOR"] != DBNull.Value ? dr.Field<string>("BA0_CTACOR") : String.Empty;
            vo.Empori = dr["BA0_EMPORI"] != DBNull.Value ? dr.Field<string>("BA0_EMPORI") : String.Empty;
            vo.Bascop = dr["BA0_BASCOP"] != DBNull.Value ? dr.Field<string>("BA0_BASCOP") : String.Empty;
            vo.Abrang = dr["BA0_ABRANG"] != DBNull.Value ? dr.Field<string>("BA0_ABRANG") : String.Empty;
            vo.Natjur = dr["BA0_NATJUR"] != DBNull.Value ? dr.Field<string>("BA0_NATJUR") : String.Empty;
            vo.Modali = dr["BA0_MODALI"] != DBNull.Value ? dr.Field<string>("BA0_MODALI") : String.Empty;
            vo.Segmen = dr["BA0_SEGMEN"] != DBNull.Value ? dr.Field<string>("BA0_SEGMEN") : String.Empty;
            vo.Codreg = dr["BA0_CODREG"] != DBNull.Value ? dr.Field<string>("BA0_CODREG") : String.Empty;
            vo.Tpoped = dr["BA0_TPOPED"] != DBNull.Value ? dr.Field<string>("BA0_TPOPED") : String.Empty;
            vo.Tisver = dr["BA0_TISVER"] != DBNull.Value ? dr.Field<string>("BA0_TISVER") : String.Empty;
            vo.Maxrg = dr["BA0_MAXRG"] != DBNull.Value ? Convert.ToDouble(dr["BA0_MAXRG"]) : 0.0;
            vo.Przrec = dr["BA0_PRZREC"] != DBNull.Value ? Convert.ToDouble(dr["BA0_PRZREC"]) : 0.0;
            vo.Criprz = dr["BA0_CRIPRZ"] != DBNull.Value ? dr.Field<string>("BA0_CRIPRZ") : String.Empty;
            vo.Tppag = dr["BA0_TPPAG"] != DBNull.Value ? dr.Field<string>("BA0_TPPAG") : String.Empty;
            vo.Tabprz = dr["BA0_TABPRZ"] != DBNull.Value ? dr.Field<string>("BA0_TABPRZ") : String.Empty;
            vo.Crilim = dr["BA0_CRILIM"] != DBNull.Value ? dr.Field<string>("BA0_CRILIM") : String.Empty;
            vo.Tablim = dr["BA0_TABLIM"] != DBNull.Value ? dr.Field<string>("BA0_TABLIM") : String.Empty;
            vo.Autges = dr["BA0_AUTGES"] != DBNull.Value ? dr.Field<string>("BA0_AUTGES") : String.Empty;
            vo.Liecon = dr["BA0_LIECON"] != DBNull.Value ? dr.Field<string>("BA0_LIECON") : String.Empty;
            vo.Digane = dr["BA0_DIGANE"] != DBNull.Value ? dr.Field<string>("BA0_DIGANE") : String.Empty;
            vo.Zverfr = dr["BA0_ZVERFR"] != DBNull.Value ? dr.Field<string>("BA0_ZVERFR") : String.Empty;
            vo.Vlrapr = dr["BA0_VLRAPR"] != DBNull.Value ? dr.Field<string>("BA0_VLRAPR") : String.Empty;

            return vo;
        }
    }
}
