using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.DAO.Protheus
{
    internal class PUsuarioPortalDAO
    {
        const string FIELDS_USUARIO = " BSW_CODUSR, BSW_LOGUSR, BSW_NOMUSR, BSW_SENHA, BSW_EMAIL, BSW_CODACE, BSW_TIPCAR, BSW_BIOMET, BSW_TPPOR, BSW_DTSEN, BSW_YCPF, BSW_YEXPPS, BSW_YHASH, BSW_BEINTR, BSW_YATUCD, BSW_YPRIAC, BSW_PERACE, BSW_PRIACE, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, BAU_CODIGO ";

        internal static PUsuarioPortalVO LogarBeneficiario(string login, string senha, EvidaDatabase db)
        {
            string sql = "SELECT " + FIELDS_USUARIO +
                " FROM VW_PR_USUARIO_PORTAL_BENEF U " +
                " WHERE upper(trim(U.BSW_LOGUSR)) = upper(trim(:login)) AND upper(trim(U.BSW_SENHA)) = upper(trim(EVIDA.MD5(:senha)))";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.Trim() });
            lstParams.Add(new Parametro() { Name = ":senha", Tipo = DbType.String, Value = senha.Trim() });

            List<PUsuarioPortalVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PUsuarioPortalVO LogarCredenciado(string login, string senha, EvidaDatabase db)
        {
            string sql = "SELECT " + FIELDS_USUARIO +
                " FROM VW_PR_USUARIO_PORTAL_CRED U " +
                " WHERE upper(trim(U.BSW_LOGUSR)) = upper(trim(:login)) AND upper(trim(U.BSW_SENHA)) = upper(trim(EVIDA.MD5(:senha)))";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.Trim() });
            lstParams.Add(new Parametro() { Name = ":senha", Tipo = DbType.String, Value = senha.Trim() });

            List<PUsuarioPortalVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PUsuarioPortalVO FromDataRow(DataRow dr)
        {
            PUsuarioPortalVO vo = new PUsuarioPortalVO();

            vo.Codusr = dr["BSW_CODUSR"] != DBNull.Value ? dr.Field<string>("BSW_CODUSR") : String.Empty;
            vo.Logusr = dr["BSW_LOGUSR"] != DBNull.Value ? dr.Field<string>("BSW_LOGUSR") : String.Empty;
            vo.Nomusr = dr["BSW_NOMUSR"] != DBNull.Value ? dr.Field<string>("BSW_NOMUSR") : String.Empty;
            vo.Senha = dr["BSW_SENHA"] != DBNull.Value ? dr.Field<string>("BSW_SENHA") : String.Empty;
            vo.Email = dr["BSW_EMAIL"] != DBNull.Value ? dr.Field<string>("BSW_EMAIL") : String.Empty;
            vo.Codace = dr["BSW_CODACE"] != DBNull.Value ? dr.Field<string>("BSW_CODACE") : String.Empty;
            vo.Tipcar = dr["BSW_TIPCAR"] != DBNull.Value ? dr.Field<string>("BSW_TIPCAR") : String.Empty;
            vo.Biomet = dr["BSW_BIOMET"] != DBNull.Value ? dr.Field<string>("BSW_BIOMET") : String.Empty;
            vo.Tppor = dr["BSW_TPPOR"] != DBNull.Value ? dr.Field<string>("BSW_TPPOR") : String.Empty;
            vo.Dtsen = dr["BSW_DTSEN"] != DBNull.Value ? dr.Field<string>("BSW_DTSEN") : String.Empty;
            vo.Ycpf = dr["BSW_YCPF"] != DBNull.Value ? dr.Field<string>("BSW_YCPF") : String.Empty;
            vo.Yexpps = dr["BSW_YEXPPS"] != DBNull.Value ? dr.Field<string>("BSW_YEXPPS") : String.Empty;
            vo.Yhash = dr["BSW_YHASH"] != DBNull.Value ? dr.Field<string>("BSW_YHASH") : String.Empty;
            vo.Beintr = dr["BSW_BEINTR"] != DBNull.Value ? dr.Field<string>("BSW_BEINTR") : String.Empty;
            vo.Yatucd = dr["BSW_YATUCD"] != DBNull.Value ? dr.Field<string>("BSW_YATUCD") : String.Empty;
            vo.Ypriac = dr["BSW_YPRIAC"] != DBNull.Value ? dr.Field<string>("BSW_YPRIAC") : String.Empty;
            vo.Perace = dr["BSW_PERACE"] != DBNull.Value ? dr.Field<string>("BSW_PERACE") : String.Empty;
            vo.Priace = dr["BSW_PRIACE"] != DBNull.Value ? dr.Field<string>("BSW_PRIACE") : String.Empty;
            vo.Codint = dr["BA1_CODINT"] != DBNull.Value ? dr.Field<string>("BA1_CODINT") : String.Empty;
            vo.Codemp = dr["BA1_CODEMP"] != DBNull.Value ? dr.Field<string>("BA1_CODEMP") : String.Empty;
            vo.Matric = dr["BA1_MATRIC"] != DBNull.Value ? dr.Field<string>("BA1_MATRIC") : String.Empty;
            vo.Tipreg = dr["BA1_TIPREG"] != DBNull.Value ? dr.Field<string>("BA1_TIPREG") : String.Empty;
            vo.Codigo = dr["BAU_CODIGO"] != DBNull.Value ? dr.Field<string>("BAU_CODIGO") : String.Empty;

            return vo;
        }

        internal static void SalvarDadosContato(PUsuarioPortalVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE BSW010 SET BSW_EMAIL = :email " +
                "	WHERE upper(trim(BSW_LOGUSR)) = upper(trim(:logusr)) ";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email.Trim().Length >= 60 ? vo.Email.Trim().Substring(0, 60).ToUpper() : vo.Email.Trim().PadRight(60, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":logusr", DbType.String, vo.Logusr.Trim());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

    }
}
