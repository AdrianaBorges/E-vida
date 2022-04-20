using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;

namespace eVidaGeneralLib.DAO.Protheus
{
    internal class PUsuarioDAO
    {
        private static string FIELDS = " ba1_codint, ba1_codemp, ba1_matric, ba1_tipreg, ba1_digito, ba1_matemp, trim(ba1_codint) || '|' || trim(ba1_codemp) || '|' || trim(ba1_matric) || '|' || trim(ba1_tipreg) as cd_beneficiario, ba1_matvid, ba1_conemp, ba1_vercon, ba1_subcon, ba1_versub, ba1_tipusu, ba1_graupa, " +
                " ba1_datadm, ba1_datinc, ba1_datcar, ba1_dtvlcr, trim(ba1_codint) || trim(ba1_codemp) || trim(ba1_matric) || trim(ba1_tipreg) || '.' || trim(ba1_digito) as ba1_matant, ba1_yautpr, ba1_datblo, ba1_motblo, ba1_consid, ba1_nomusr, ba1_estciv, " +
                " ba1_sexo, ba1_datnas, ba1_mae, ba1_pai, ba1_cpfusr, ba1_drgusr, ba1_orgem, ba1_pispas, ba1_endere, ba1_nr_end, ba1_comend, ba1_bairro, " +
                " ba1_codmun, ba1_munici, ba1_estado, ba1_cepusr, ba1_ddd, ba1_telefo, ba1_telres, ba1_telcom, ba1_email, ba1_ycaren, ba1_ycdleg, ba1_image, ba1_codpla ";

        internal static PUsuarioVO GetRowById(string codint, string codemp, string matric, string tipreg, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_USUARIO WHERE upper(trim(BA1_CODINT)) = upper(trim(:codint)) and upper(trim(BA1_CODEMP)) = upper(trim(:codemp)) and upper(trim(BA1_MATRIC)) = upper(trim(:matric)) and upper(trim(BA1_TIPREG)) = upper(trim(:tipreg)) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });
            lstParams.Add(new Parametro() { Name = ":tipreg", Tipo = DbType.String, Value = tipreg.Trim() });

            List<PUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static DataTable GetUsuarios(string codint, string codemp, string matric, EvidaDatabase db)
        {
            // Solicitado pelo Sr. Eurico, do Setor de Cadastro:
            // Se o plano da família for o 0001 e o plano do usuário for o 0004, a consulta considera o plano do usuário.
            // Para os demais, a consulta considera o plano da família.

            // Considera o plano da família
            string sql = "select " + FIELDS + ", trim(u.BA1_DATCAR) as BA1_DATCAR, trim(p.BRP_DESCRI) as BRP_DESCRI, trim(s.BI3_CODIGO) as BI3_CODIGO, trim(s.BI3_DESCRI) as BI3_DESCRI, trim(s.BI3_TIPO) as BI3_TIPO " +
                " from VW_PR_USUARIO_ATUAL u, VW_PR_FAMILIA_ATUAL f, VW_PR_GRAU_PARENTESCO p, VW_PR_PRODUTO_SAUDE s " +
                " where upper(trim(u.BA1_CODINT)) = upper(trim(f.BA3_CODINT)) and upper(trim(u.BA1_CODEMP)) = upper(trim(f.BA3_CODEMP)) and upper(trim(u.BA1_MATRIC)) = upper(trim(f.BA3_MATRIC)) " +
                " and upper(trim(u.BA1_GRAUPA)) = upper(trim(p.BRP_CODIGO)) " +
                " and upper(trim(f.BA3_CODINT)) = upper(trim(:codint)) and upper(trim(f.BA3_CODEMP)) = upper(trim(:codemp)) and upper(trim(f.BA3_MATRIC)) = upper(trim(:matric)) " +
                " and (f.BA3_CODPLA <> '0001' or u.BA1_CODPLA <> '0004') " +
                " and upper(trim(f.BA3_CODPLA)) = upper(trim(s.BI3_CODIGO)) " +

                // Considera o plano do usuário
                " union select " + FIELDS + ", trim(u.BA1_DATCAR) as BA1_DATCAR, trim(p.BRP_DESCRI) as BRP_DESCRI, trim(s.BI3_CODIGO) as BI3_CODIGO, trim(s.BI3_DESCRI) as BI3_DESCRI, trim(s.BI3_TIPO) as BI3_TIPO " +
                " from VW_PR_USUARIO_ATUAL u, VW_PR_FAMILIA_ATUAL f, VW_PR_GRAU_PARENTESCO p, VW_PR_PRODUTO_SAUDE s " +
                " where upper(trim(u.BA1_CODINT)) = upper(trim(f.BA3_CODINT)) and upper(trim(u.BA1_CODEMP)) = upper(trim(f.BA3_CODEMP)) and upper(trim(u.BA1_MATRIC)) = upper(trim(f.BA3_MATRIC)) " +
                " and upper(trim(u.BA1_GRAUPA)) = upper(trim(p.BRP_CODIGO)) " +
                " and upper(trim(f.BA3_CODINT)) = upper(trim(:codint)) and upper(trim(f.BA3_CODEMP)) = upper(trim(:codemp)) and upper(trim(f.BA3_MATRIC)) = upper(trim(:matric)) " +
                " and (f.BA3_CODPLA = '0001' and u.BA1_CODPLA = '0004') " +
                " and upper(trim(u.BA1_CODPLA)) = upper(trim(s.BI3_CODIGO)) ";
                
            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });

            DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);
            return dt;
        }

        internal static PUsuarioVO GetRowByCartao(string numCartao, EvidaDatabase db)
        {
            
            StringBuilder sql = new StringBuilder();
            sql.Append(" select " + FIELDS);
            sql.Append(" from VW_PR_USUARIO_FULL ");
            sql.Append(" where (replace(trim(BA1_MATANT), '-', '') = replace(trim(:numCartao), '-', '') or trim(BA1_CODINT) || trim(BA1_CODEMP) || trim(BA1_MATRIC) || trim(BA1_TIPREG) || trim(BA1_DIGITO) = replace(trim(:numCartao), '.', '')) ");
            sql.Append(" and trim(BA1_DATCAR) in (select max(trim(BA1_DATCAR)) ");
            sql.Append(" from VW_PR_USUARIO_FULL ");
            sql.Append(" where (replace(trim(BA1_MATANT), '-', '') = replace(trim(:numCartao), '-', '') or trim(BA1_CODINT) || trim(BA1_CODEMP) || trim(BA1_MATRIC) || trim(BA1_TIPREG) || trim(BA1_DIGITO) = replace(trim(:numCartao), '.', ''))) ");

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":numCartao", DbType.String, numCartao.Trim()));

            List<PUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql.ToString(), FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static PUsuarioVO GetTitular(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_USUARIO WHERE upper(trim(BA1_CODINT)) = upper(trim(:codint)) and upper(trim(BA1_CODEMP)) = upper(trim(:codemp)) and upper(trim(BA1_MATRIC)) = upper(trim(:matric)) and upper(trim(BA1_TIPUSU)) = 'T'";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric });

            List<PUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static List<PUsuarioVO> ListarUsuarios(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_USUARIO " +
                " where upper(trim(BA1_CODINT)) = upper(trim(:codint)) and upper(trim(BA1_CODEMP)) = upper(trim(:codemp)) and upper(trim(BA1_MATRIC)) = upper(trim(:matric))";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });

            List<PUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            return lst;
        }

        internal static List<PUsuarioVO> ListarDependentes(string codint, string codemp, string matric, EvidaDatabase db)
        {
            string sql = "select " + FIELDS +
                " from VW_PR_USUARIO " +
                " where upper(trim(BA1_CODINT)) = upper(trim(:codint)) and upper(trim(BA1_CODEMP)) = upper(trim(:codemp)) and upper(trim(BA1_MATRIC)) = upper(trim(:matric)) " +
                " and BA1_TIPUSU <> 'T'";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });

            List<PUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
            return lst;
        }

        private static PUsuarioVO FromDataRow(DataRow dr)
        {
            PUsuarioVO vo = new PUsuarioVO();

            vo.Codint = dr["ba1_codint"] != DBNull.Value ? dr.Field<string>("ba1_codint") : String.Empty;
            vo.Codemp = dr["ba1_codemp"] != DBNull.Value ? dr.Field<string>("ba1_codemp") : String.Empty;
            vo.Matric = dr["ba1_matric"] != DBNull.Value ? dr.Field<string>("ba1_matric") : String.Empty;
            vo.Tipreg = dr["ba1_tipreg"] != DBNull.Value ? dr.Field<string>("ba1_tipreg") : String.Empty;
            vo.Digito = dr["ba1_digito"] != DBNull.Value ? dr.Field<string>("ba1_digito") : String.Empty;
            vo.Matemp = dr["ba1_matemp"] != DBNull.Value ? dr.Field<string>("ba1_matemp") : String.Empty;
            vo.Matvid = dr["ba1_matvid"] != DBNull.Value ? dr.Field<string>("ba1_matvid") : String.Empty;
            vo.Conemp = dr["ba1_conemp"] != DBNull.Value ? dr.Field<string>("ba1_conemp") : String.Empty;
            vo.Vercon = dr["ba1_vercon"] != DBNull.Value ? dr.Field<string>("ba1_vercon") : String.Empty;
            vo.Subcon = dr["ba1_subcon"] != DBNull.Value ? dr.Field<string>("ba1_subcon") : String.Empty;
            vo.Versub = dr["ba1_versub"] != DBNull.Value ? dr.Field<string>("ba1_versub") : String.Empty;
            vo.Tipusu = dr["ba1_tipusu"] != DBNull.Value ? dr.Field<string>("ba1_tipusu") : String.Empty;
            vo.Graupa = dr["ba1_graupa"] != DBNull.Value ? dr.Field<string>("ba1_graupa") : String.Empty;
            vo.Datadm = dr["ba1_datadm"] != DBNull.Value ? dr.Field<string>("ba1_datadm") : String.Empty;
            vo.Datinc = dr["ba1_datinc"] != DBNull.Value ? dr.Field<string>("ba1_datinc") : String.Empty;
            vo.Datcar = dr["ba1_datcar"] != DBNull.Value ? dr.Field<string>("ba1_datcar") : String.Empty;
            vo.Dtvlcr = dr["ba1_dtvlcr"] != DBNull.Value ? dr.Field<string>("ba1_dtvlcr") : String.Empty;
            vo.Matant = dr["ba1_matant"] != DBNull.Value ? dr.Field<string>("ba1_matant") : String.Empty;
            vo.Yautpr = dr["ba1_yautpr"] != DBNull.Value ? dr.Field<string>("ba1_yautpr") : String.Empty;
            vo.Datblo = dr["ba1_datblo"] != DBNull.Value ? dr.Field<string>("ba1_datblo") : String.Empty;
            vo.Motblo = dr["ba1_motblo"] != DBNull.Value ? dr.Field<string>("ba1_motblo") : String.Empty;
            vo.Consid = dr["ba1_consid"] != DBNull.Value ? dr.Field<string>("ba1_consid") : String.Empty;
            vo.Nomusr = dr["ba1_nomusr"] != DBNull.Value ? dr.Field<string>("ba1_nomusr") : String.Empty;
            vo.Estciv = dr["ba1_estciv"] != DBNull.Value ? dr.Field<string>("ba1_estciv") : String.Empty;
            vo.Sexo = dr["ba1_sexo"] != DBNull.Value ? dr.Field<string>("ba1_sexo") : String.Empty;
            vo.Datnas = dr["ba1_datnas"] != DBNull.Value ? dr.Field<string>("ba1_datnas") : String.Empty;
            vo.Mae = dr["ba1_mae"] != DBNull.Value ? dr.Field<string>("ba1_mae") : String.Empty;
            vo.Pai = dr["ba1_pai"] != DBNull.Value ? dr.Field<string>("ba1_pai") : String.Empty;
            vo.Cpfusr = dr["ba1_cpfusr"] != DBNull.Value ? dr.Field<string>("ba1_cpfusr") : String.Empty;
            vo.Drgusr = dr["ba1_drgusr"] != DBNull.Value ? dr.Field<string>("ba1_drgusr") : String.Empty;
            vo.Orgem = dr["ba1_orgem"] != DBNull.Value ? dr.Field<string>("ba1_orgem") : String.Empty;
            vo.Pispas = dr["ba1_pispas"] != DBNull.Value ? dr.Field<string>("ba1_pispas") : String.Empty;
            vo.Endere = dr["ba1_endere"] != DBNull.Value ? dr.Field<string>("ba1_endere") : String.Empty;
            vo.Nrend = dr["ba1_nr_end"] != DBNull.Value ? dr.Field<string>("ba1_nr_end") : String.Empty;
            vo.Comend = dr["ba1_comend"] != DBNull.Value ? dr.Field<string>("ba1_comend") : String.Empty;
            vo.Bairro = dr["ba1_bairro"] != DBNull.Value ? dr.Field<string>("ba1_bairro") : String.Empty;
            vo.Codmun = dr["ba1_codmun"] != DBNull.Value ? dr.Field<string>("ba1_codmun") : String.Empty;
            vo.Munici = dr["ba1_munici"] != DBNull.Value ? dr.Field<string>("ba1_munici") : String.Empty;
            vo.Estado = dr["ba1_estado"] != DBNull.Value ? dr.Field<string>("ba1_estado") : String.Empty;
            vo.Cepusr = dr["ba1_cepusr"] != DBNull.Value ? dr.Field<string>("ba1_cepusr") : String.Empty;
            vo.Ddd = dr["ba1_ddd"] != DBNull.Value ? dr.Field<string>("ba1_ddd") : String.Empty;
            vo.Telefo = dr["ba1_telefo"] != DBNull.Value ? dr.Field<string>("ba1_telefo") : String.Empty;
            vo.Telres = dr["ba1_telres"] != DBNull.Value ? dr.Field<string>("ba1_telres") : String.Empty;
            vo.Telcom = dr["ba1_telcom"] != DBNull.Value ? dr.Field<string>("ba1_telcom") : String.Empty;
            vo.Email = dr["ba1_email"] != DBNull.Value ? dr.Field<string>("ba1_email") : String.Empty;
            vo.Ycaren = dr["ba1_ycaren"] != DBNull.Value ? dr.Field<string>("ba1_ycaren") : String.Empty;
            vo.Ycdleg = dr["ba1_ycdleg"] != DBNull.Value ? dr.Field<string>("ba1_ycdleg") : String.Empty;
            vo.Image = dr["ba1_image"] != DBNull.Value ? dr.Field<string>("ba1_image") : String.Empty;
            vo.Codpla = dr["ba1_codpla"] != DBNull.Value ? dr.Field<string>("ba1_codpla") : String.Empty;

            vo.Cdusuario = vo.Codint.Trim() + "|" + vo.Codemp.Trim() + "|" + vo.Matric.Trim() + "|" + vo.Tipreg.Trim();

            return vo;
        }

        private static string NextId(string codint, string codemp, string matric, EvidaDatabase evdb)
        {
            string tipreg = "0";

            // Verifica se existe uma empregado para esta família
            string sql = "select count(*) from BA1010 where upper(trim(ba1_codint)) = upper(trim(:codint)) and upper(trim(ba1_codemp)) = upper(trim(:codemp)) and upper(trim(ba1_matric)) = upper(trim(:matric)) ";

            Database db = evdb.Database;
            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":codint", Tipo = DbType.String, Value = codint.Trim() });
            lstParams.Add(new Parametro() { Name = ":codemp", Tipo = DbType.String, Value = codemp.Trim() });
            lstParams.Add(new Parametro() { Name = ":matric", Tipo = DbType.String, Value = matric.Trim() });
            int qtdEmpr = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
            
            if (qtdEmpr > 0)
            {
                // Caso exista um empregado para esta família, pega o próximo número da sequência do empregado
                sql = "select to_number(max(trim(ba1_tipreg))) + 1 from BA1010 where upper(trim(ba1_codint)) = upper(trim(:codint)) and upper(trim(ba1_codemp)) = upper(trim(:codemp)) and upper(trim(ba1_matric)) = upper(trim(:matric))";

                int proxNum = Convert.ToInt32(BaseDAO.ExecuteScalar(db, sql, lstParams));
                tipreg = proxNum.ToString();
            }

            tipreg = tipreg.PadLeft(2, '0');
            return tipreg;
        }

        private static Int64 NextRecno(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from BA1010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }

        private static Int64 NextRecnoBCA(EvidaDatabase db)
        {
            // Pega o próximo número da sequência geral de R_E_C_N_O_
            string sql = "select max(R_E_C_N_O_) + 1 from BCA010";
            Int64 recno = Convert.ToInt64(BaseDAO.ExecuteScalar(db, sql));
            return recno;
        }	

        internal static void CriarUsuario(PUsuarioVO usuario, EvidaDatabase db)
        {
            string sql = "insert into BA1010 (ba1_codint, ba1_codemp, ba1_matric, ba1_tipreg, ba1_matemp, ba1_matant, ba1_matvid, ba1_nomusr, ba1_tipusu, ba1_ddd, ba1_telres, ba1_telefo,  " +
                " ba1_telcom, ba1_graupa, ba1_datadm, ba1_datinc, ba1_datcar, ba1_conemp, ba1_vercon, ba1_subcon, ba1_versub, ba1_image, ba1_sexo, ba1_datnas, ba1_cpfusr, ba1_drgusr, ba1_pai, ba1_mae, ba1_orgem,  " +
                " ba1_estciv, ba1_email, ba1_endere, ba1_nr_end, ba1_comend, ba1_bairro, ba1_codmun, ba1_munici, ba1_estado, ba1_cepusr, ba1_ycaren, ba1_ycdleg, ba1_digito, ba1_locans, ba1_infans, ba1_infsib, ba1_zmotiv, ba1_reeweb, ba1_mudfai, R_E_C_N_O_) " +
                " values (:codint, :codemp, :matric, :tipreg, :matemp, :matant, :matvid, :nomusr, :tipusu, :ddd, :telres, :telefo, :telcom, :graupa, :datadm, :datinc, :datcar, :conemp, :vercon, :subcon, :versub, 'ENABLE', :sexo, :datnas, " +
                " :cpfusr, :drgusr, :pai, :mae, :orgem, :estciv, :email, :endere, :nrend, :comend, :bairro, :codmun, :munici, :estado, :cepusr, :ycaren, :ycdleg, :digito, :locans, :infans, :infsib, :zmotiv, :reeweb, :mudfai, :recno)";

            Int64 recno = NextRecno(db);
            usuario.Tipreg = NextId(usuario.Codint, usuario.Codemp, usuario.Matric, db);

            String calcular = "0001" + usuario.Codemp.Trim().PadRight(4, ' ') + usuario.Matric.Trim().PadRight(6, ' ') + usuario.Tipreg.Trim().PadRight(2, ' ');
            String digito = ValidateUtil.DigitoModulo11(Int64.Parse(calcular)).ToString();

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(new Parametro(":codint", DbType.String, "0001"));
            lstParam.Add(usuario.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, usuario.Codemp.Trim().Length >= 4 ? usuario.Codemp.Trim().Substring(0, 4) : usuario.Codemp.Trim().PadRight(4, ' ')));
            lstParam.Add(usuario.Matric == null ? new Parametro(":matric", DbType.String, "".PadRight(6, ' ')) : new Parametro(":matric", DbType.String, usuario.Matric.Trim().Length >= 6 ? usuario.Matric.Trim().Substring(0, 6) : usuario.Matric.Trim().PadRight(6, ' ')));
            lstParam.Add(usuario.Tipreg == null ? new Parametro(":tipreg", DbType.String, "".PadRight(2, ' ')) : new Parametro(":tipreg", DbType.String, usuario.Tipreg.Trim().Length >= 2 ? usuario.Tipreg.Trim().Substring(0, 2) : usuario.Tipreg.Trim().PadRight(2, ' ')));
            lstParam.Add(usuario.Matemp == null ? new Parametro(":matemp", DbType.String, "".PadRight(30, ' ')) : new Parametro(":matemp", DbType.String, usuario.Matemp.Trim().Length >= 30 ? usuario.Matemp.Trim().Substring(0, 30) : usuario.Matemp.Trim().PadRight(30, ' ')));
            lstParam.Add(usuario.Matant == null ? new Parametro(":matant", DbType.String, "".PadRight(20, ' ')) : new Parametro(":matant", DbType.String, usuario.Matant.Trim().Length >= 20 ? usuario.Matant.Trim().Substring(0, 20) : usuario.Matant.Trim().PadRight(20, ' ')));
            lstParam.Add(usuario.Matvid == null ? new Parametro(":matvid", DbType.String, "".PadRight(8, ' ')) : new Parametro(":matvid", DbType.String, usuario.Matvid.Trim().Length >= 8 ? usuario.Matvid.Trim().Substring(0, 8) : usuario.Matvid.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Nomusr == null ? new Parametro(":nomusr", DbType.String, "".PadRight(70, ' ')) : new Parametro(":nomusr", DbType.String, usuario.Nomusr.Trim().Length >= 70 ? usuario.Nomusr.Trim().Substring(0, 70) : usuario.Nomusr.Trim().PadRight(70, ' ')));
            lstParam.Add(usuario.Tipusu == null ? new Parametro(":tipusu", DbType.String, "".PadRight(1, ' ')) : new Parametro(":tipusu", DbType.String, usuario.Tipusu.Trim().Length >= 1 ? usuario.Tipusu.Trim().Substring(0, 1) : usuario.Tipusu.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, usuario.Ddd.Trim().Length >= 3 ? usuario.Ddd.Trim().Substring(0, 3) : usuario.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(usuario.Telres == null ? new Parametro(":telres", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telres", DbType.String, usuario.Telres.Trim().Length >= 15 ? usuario.Telres.Trim().Substring(0, 15) : usuario.Telres.Trim().PadRight(15, ' ')));
            lstParam.Add(usuario.Telefo == null ? new Parametro(":telefo", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telefo", DbType.String, usuario.Telefo.Trim().Length >= 15 ? usuario.Telefo.Trim().Substring(0, 15) : usuario.Telefo.Trim().PadRight(15, ' ')));
            lstParam.Add(usuario.Telcom == null ? new Parametro(":telcom", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telcom", DbType.String, usuario.Telcom.Trim().Length >= 15 ? usuario.Telcom.Trim().Substring(0, 15) : usuario.Telcom.Trim().PadRight(15, ' ')));
            lstParam.Add(usuario.Graupa == null ? new Parametro(":graupa", DbType.String, "".PadRight(2, ' ')) : new Parametro(":graupa", DbType.String, usuario.Graupa.Trim().Length >= 2 ? usuario.Graupa.Trim().Substring(0, 2) : usuario.Graupa.Trim().PadRight(2, ' ')));
            lstParam.Add(usuario.Datadm == null ? new Parametro(":datadm", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datadm", DbType.String, usuario.Datadm.Trim().Length >= 8 ? usuario.Datadm.Trim().Substring(0, 8) : usuario.Datadm.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Datinc == null ? new Parametro(":datinc", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datinc", DbType.String, usuario.Datinc.Trim().Length >= 8 ? usuario.Datinc.Trim().Substring(0, 8) : usuario.Datinc.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Datcar == null ? new Parametro(":datcar", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datcar", DbType.String, usuario.Datcar.Trim().Length >= 8 ? usuario.Datcar.Trim().Substring(0, 8) : usuario.Datcar.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Conemp == null ? new Parametro(":conemp", DbType.String, "".PadRight(12, ' ')) : new Parametro(":conemp", DbType.String, usuario.Conemp.Trim().Length >= 12 ? usuario.Conemp.Trim().Substring(0, 12) : usuario.Conemp.Trim().PadRight(12, ' ')));
            lstParam.Add(usuario.Vercon == null ? new Parametro(":vercon", DbType.String, "".PadRight(3, ' ')) : new Parametro(":vercon", DbType.String, usuario.Vercon.Trim().Length >= 3 ? usuario.Vercon.Trim().Substring(0, 3) : usuario.Vercon.Trim().PadRight(3, ' ')));
            lstParam.Add(usuario.Subcon == null ? new Parametro(":subcon", DbType.String, "".PadRight(9, ' ')) : new Parametro(":subcon", DbType.String, usuario.Subcon.Trim().Length >= 9 ? usuario.Subcon.Trim().Substring(0, 9) : usuario.Subcon.Trim().PadRight(9, ' ')));
            lstParam.Add(usuario.Versub == null ? new Parametro(":versub", DbType.String, "".PadRight(3, ' ')) : new Parametro(":versub", DbType.String, usuario.Versub.Trim().Length >= 3 ? usuario.Versub.Trim().Substring(0, 3) : usuario.Versub.Trim().PadRight(3, ' ')));
            lstParam.Add(usuario.Sexo == null ? new Parametro(":sexo", DbType.String, "".PadRight(1, ' ')) : new Parametro(":sexo", DbType.String, usuario.Sexo.Trim().Length >= 1 ? usuario.Sexo.Trim().Substring(0, 1) : usuario.Sexo.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Datnas == null ? new Parametro(":datnas", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datnas", DbType.String, usuario.Datnas.Trim().Length >= 8 ? usuario.Datnas.Trim().Substring(0, 8) : usuario.Datnas.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Cpfusr == null ? new Parametro(":cpfusr", DbType.String, "".PadRight(11, ' ')) : new Parametro(":cpfusr", DbType.String, usuario.Cpfusr.Trim().Length >= 11 ? usuario.Cpfusr.Trim().Substring(0, 11) : usuario.Cpfusr.Trim().PadRight(11, ' ')));
            lstParam.Add(usuario.Drgusr == null ? new Parametro(":drgusr", DbType.String, "".PadRight(20, ' ')) : new Parametro(":drgusr", DbType.String, usuario.Drgusr.Trim().Length >= 20 ? usuario.Drgusr.Trim().Substring(0, 20) : usuario.Drgusr.Trim().PadRight(20, ' ')));
            lstParam.Add(usuario.Pai == null ? new Parametro(":pai", DbType.String, "".PadRight(30, ' ')) : new Parametro(":pai", DbType.String, usuario.Pai.Trim().Length >= 30 ? usuario.Pai.Trim().Substring(0, 30) : usuario.Pai.Trim().PadRight(30, ' ')));
            lstParam.Add(usuario.Mae == null ? new Parametro(":mae", DbType.String, "".PadRight(120, ' ')) : new Parametro(":mae", DbType.String, usuario.Mae.Trim().Length >= 120 ? usuario.Mae.Trim().Substring(0, 120) : usuario.Mae.Trim().PadRight(120, ' ')));
            lstParam.Add(usuario.Orgem == null ? new Parametro(":orgem", DbType.String, "".PadRight(10, ' ')) : new Parametro(":orgem", DbType.String, usuario.Orgem.Trim().Length >= 10 ? usuario.Orgem.Trim().Substring(0, 10) : usuario.Orgem.Trim().PadRight(10, ' ')));
            lstParam.Add(usuario.Estciv == null ? new Parametro(":estciv", DbType.String, "".PadRight(1, ' ')) : new Parametro(":estciv", DbType.String, usuario.Estciv.Trim().Length >= 1 ? usuario.Estciv.Trim().Substring(0, 1) : usuario.Estciv.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, usuario.Email.Trim().Length >= 100 ? usuario.Email.Trim().Substring(0, 100) : usuario.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(usuario.Endere == null ? new Parametro(":endere", DbType.String, "".PadRight(40, ' ')) : new Parametro(":endere", DbType.String, usuario.Endere.Trim().Length >= 40 ? usuario.Endere.Trim().Substring(0, 40) : usuario.Endere.Trim().PadRight(40, ' ')));
            lstParam.Add(usuario.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, usuario.Nrend.Trim().Length >= 6 ? usuario.Nrend.Trim().Substring(0, 6) : usuario.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(usuario.Comend == null ? new Parametro(":comend", DbType.String, "".PadRight(20, ' ')) : new Parametro(":comend", DbType.String, usuario.Comend.Trim().Length >= 20 ? usuario.Comend.Trim().Substring(0, 20) : usuario.Comend.Trim().PadRight(20, ' ')));
            lstParam.Add(usuario.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, usuario.Bairro.Trim().Length >= 40 ? usuario.Bairro.Trim().Substring(0, 40) : usuario.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(usuario.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(7, ' ')) : new Parametro(":codmun", DbType.String, usuario.Codmun.Trim().Length >= 7 ? usuario.Codmun.Trim().Substring(0, 7) : usuario.Codmun.Trim().PadRight(7, ' ')));
            lstParam.Add(usuario.Munici == null ? new Parametro(":munici", DbType.String, "".PadRight(30, ' ')) : new Parametro(":munici", DbType.String, usuario.Munici.Trim().Length >= 30 ? usuario.Munici.Trim().Substring(0, 30) : usuario.Munici.Trim().PadRight(30, ' ')));
            lstParam.Add(usuario.Estado == null ? new Parametro(":estado", DbType.String, "".PadRight(2, ' ')) : new Parametro(":estado", DbType.String, usuario.Estado.Trim().Length >= 2 ? usuario.Estado.Trim().Substring(0, 2) : usuario.Estado.Trim().PadRight(2, ' ')));
            lstParam.Add(usuario.Cepusr == null ? new Parametro(":cepusr", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cepusr", DbType.String, usuario.Cepusr.Trim().Length >= 8 ? usuario.Cepusr.Trim().Substring(0, 8) : usuario.Cepusr.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Ycaren == null ? new Parametro(":ycaren", DbType.String, "".PadRight(1, ' ')) : new Parametro(":ycaren", DbType.String, usuario.Ycaren.Trim().Length >= 1 ? usuario.Ycaren.Trim().Substring(0, 1) : usuario.Ycaren.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Ycdleg == null ? new Parametro(":ycdleg", DbType.String, "".PadRight(10, ' ')) : new Parametro(":ycdleg", DbType.String, usuario.Ycdleg.Trim().Length >= 10 ? usuario.Ycdleg.Trim().Substring(0, 10) : usuario.Ycdleg.Trim().PadRight(10, ' ')));
            lstParam.Add(new Parametro(":digito", DbType.String, digito));
            lstParam.Add(new Parametro(":locans", DbType.String, "1"));
            lstParam.Add(new Parametro(":infans", DbType.String, "1"));
            lstParam.Add(new Parametro(":infsib", DbType.String, "1"));
            lstParam.Add(new Parametro(":zmotiv", DbType.String, "1"));
            lstParam.Add(new Parametro(":reeweb", DbType.String, "1"));
            lstParam.Add(new Parametro(":mudfai", DbType.String, "1"));
            lstParam.Add(new Parametro(":recno", DbType.Int64, recno));
            
            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void AlterarUsuario(PUsuarioVO usuario, EvidaDatabase db)
        {
            string sql = "update BA1010 set ba1_matemp = :matemp, ba1_nomusr = :nomusr, ba1_tipusu = :tipusu, ba1_ddd = :ddd, ba1_telres = :telres, ba1_telefo = :telefo, ba1_telcom = :telcom, " +
                " ba1_graupa = :graupa, ba1_datadm = :datadm, ba1_datcar = :datcar, ba1_conemp = :conemp, ba1_vercon = :vercon, ba1_subcon = :subcon, ba1_versub = :versub, ba1_image = 'ENABLE', ba1_sexo = :sexo, ba1_datnas = :datnas, ba1_cpfusr = :cpfusr, ba1_drgusr = :drgusr, ba1_pai = :pai, " +
                " ba1_mae = :mae, ba1_orgem = :orgem, ba1_estciv = :estciv, ba1_email = :email, ba1_endere = :endere, ba1_nr_end = :nrend, " +
                " ba1_comend = :comend, ba1_bairro = :bairro, ba1_codmun = :codmun, ba1_munici = :munici, ba1_estado = :estado, ba1_cepusr = :cepusr, ba1_ycaren = :ycaren, ba1_motblo = '   ', ba1_datblo = '        ', ba1_consid = ' ' " +
                " where upper(trim(ba1_codint)) = upper(trim(:codint)) and upper(trim(ba1_codemp)) = upper(trim(:codemp)) and upper(trim(ba1_matric)) = upper(trim(:matric)) and upper(trim(ba1_tipreg)) = upper(trim(:tipreg)) ";

            List<Parametro> lstParam = new List<Parametro>();
            lstParam.Add(usuario.Codint == null ? new Parametro(":codint", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codint", DbType.String, usuario.Codint.Trim()));
            lstParam.Add(usuario.Codemp == null ? new Parametro(":codemp", DbType.String, "".PadRight(4, ' ')) : new Parametro(":codemp", DbType.String, usuario.Codemp.Trim()));
            lstParam.Add(usuario.Matric == null ? new Parametro(":matric", DbType.String, "".PadRight(6, ' ')) : new Parametro(":matric", DbType.String, usuario.Matric.Trim()));
            lstParam.Add(usuario.Tipreg == null ? new Parametro(":tipreg", DbType.String, "".PadRight(2, ' ')) : new Parametro(":tipreg", DbType.String, usuario.Tipreg.Trim()));
            lstParam.Add(usuario.Matemp == null ? new Parametro(":matemp", DbType.String, "".PadRight(30, ' ')) : new Parametro(":matemp", DbType.String, usuario.Matemp.Trim().Length >= 30 ? usuario.Matemp.Trim().Substring(0, 30) : usuario.Matemp.Trim().PadRight(30, ' ')));
            lstParam.Add(usuario.Nomusr == null ? new Parametro(":nomusr", DbType.String, "".PadRight(70, ' ')) : new Parametro(":nomusr", DbType.String, usuario.Nomusr.Trim().Length >= 70 ? usuario.Nomusr.Trim().Substring(0, 70) : usuario.Nomusr.Trim().PadRight(70, ' ')));
            lstParam.Add(usuario.Tipusu == null ? new Parametro(":tipusu", DbType.String, "".PadRight(1, ' ')) : new Parametro(":tipusu", DbType.String, usuario.Tipusu.Trim().Length >= 1 ? usuario.Tipusu.Trim().Substring(0, 1) : usuario.Tipusu.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Ddd == null ? new Parametro(":ddd", DbType.String, "".PadRight(3, ' ')) : new Parametro(":ddd", DbType.String, usuario.Ddd.Trim().Length >= 3 ? usuario.Ddd.Trim().Substring(0, 3) : usuario.Ddd.Trim().PadRight(3, ' ')));
            lstParam.Add(usuario.Telres == null ? new Parametro(":telres", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telres", DbType.String, usuario.Telres.Trim().Length >= 15 ? usuario.Telres.Trim().Substring(0, 15) : usuario.Telres.Trim().PadRight(15, ' ')));
            lstParam.Add(usuario.Telefo == null ? new Parametro(":telefo", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telefo", DbType.String, usuario.Telefo.Trim().Length >= 15 ? usuario.Telefo.Trim().Substring(0, 15) : usuario.Telefo.Trim().PadRight(15, ' ')));
            lstParam.Add(usuario.Telcom == null ? new Parametro(":telcom", DbType.String, "".PadRight(15, ' ')) : new Parametro(":telcom", DbType.String, usuario.Telcom.Trim().Length >= 15 ? usuario.Telcom.Trim().Substring(0, 15) : usuario.Telcom.Trim().PadRight(15, ' ')));
            lstParam.Add(usuario.Graupa == null ? new Parametro(":graupa", DbType.String, "".PadRight(2, ' ')) : new Parametro(":graupa", DbType.String, usuario.Graupa.Trim().Length >= 2 ? usuario.Graupa.Trim().Substring(0, 2) : usuario.Graupa.Trim().PadRight(2, ' ')));
            lstParam.Add(usuario.Datadm == null ? new Parametro(":datadm", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datadm", DbType.String, usuario.Datadm.Trim().Length >= 8 ? usuario.Datadm.Trim().Substring(0, 8) : usuario.Datadm.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Datcar == null ? new Parametro(":datcar", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datcar", DbType.String, usuario.Datcar.Trim().Length >= 8 ? usuario.Datcar.Trim().Substring(0, 8) : usuario.Datcar.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Conemp == null ? new Parametro(":conemp", DbType.String, "".PadRight(12, ' ')) : new Parametro(":conemp", DbType.String, usuario.Conemp.Trim().Length >= 12 ? usuario.Conemp.Trim().Substring(0, 12) : usuario.Conemp.Trim().PadRight(12, ' ')));
            lstParam.Add(usuario.Vercon == null ? new Parametro(":vercon", DbType.String, "".PadRight(3, ' ')) : new Parametro(":vercon", DbType.String, usuario.Vercon.Trim().Length >= 3 ? usuario.Vercon.Trim().Substring(0, 3) : usuario.Vercon.Trim().PadRight(3, ' ')));
            lstParam.Add(usuario.Subcon == null ? new Parametro(":subcon", DbType.String, "".PadRight(9, ' ')) : new Parametro(":subcon", DbType.String, usuario.Subcon.Trim().Length >= 9 ? usuario.Subcon.Trim().Substring(0, 9) : usuario.Subcon.Trim().PadRight(9, ' ')));
            lstParam.Add(usuario.Versub == null ? new Parametro(":versub", DbType.String, "".PadRight(3, ' ')) : new Parametro(":versub", DbType.String, usuario.Versub.Trim().Length >= 3 ? usuario.Versub.Trim().Substring(0, 3) : usuario.Versub.Trim().PadRight(3, ' ')));
            lstParam.Add(usuario.Sexo == null ? new Parametro(":sexo", DbType.String, "".PadRight(1, ' ')) : new Parametro(":sexo", DbType.String, usuario.Sexo.Trim().Length >= 1 ? usuario.Sexo.Trim().Substring(0, 1) : usuario.Sexo.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Datnas == null ? new Parametro(":datnas", DbType.String, "".PadRight(8, ' ')) : new Parametro(":datnas", DbType.String, usuario.Datnas.Trim().Length >= 8 ? usuario.Datnas.Trim().Substring(0, 8) : usuario.Datnas.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Cpfusr == null ? new Parametro(":cpfusr", DbType.String, "".PadRight(11, ' ')) : new Parametro(":cpfusr", DbType.String, usuario.Cpfusr.Trim().Length >= 11 ? usuario.Cpfusr.Trim().Substring(0, 11) : usuario.Cpfusr.Trim().PadRight(11, ' ')));
            lstParam.Add(usuario.Drgusr == null ? new Parametro(":drgusr", DbType.String, "".PadRight(20, ' ')) : new Parametro(":drgusr", DbType.String, usuario.Drgusr.Trim().Length >= 20 ? usuario.Drgusr.Trim().Substring(0, 20) : usuario.Drgusr.Trim().PadRight(20, ' ')));
            lstParam.Add(usuario.Pai == null ? new Parametro(":pai", DbType.String, "".PadRight(30, ' ')) : new Parametro(":pai", DbType.String, usuario.Pai.Trim().Length >= 30 ? usuario.Pai.Trim().Substring(0, 30) : usuario.Pai.Trim().PadRight(30, ' ')));
            lstParam.Add(usuario.Mae == null ? new Parametro(":mae", DbType.String, "".PadRight(120, ' ')) : new Parametro(":mae", DbType.String, usuario.Mae.Trim().Length >= 120 ? usuario.Mae.Trim().Substring(0, 120) : usuario.Mae.Trim().PadRight(120, ' ')));
            lstParam.Add(usuario.Orgem == null ? new Parametro(":orgem", DbType.String, "".PadRight(10, ' ')) : new Parametro(":orgem", DbType.String, usuario.Orgem.Trim().Length >= 10 ? usuario.Orgem.Trim().Substring(0, 10) : usuario.Orgem.Trim().PadRight(10, ' ')));
            lstParam.Add(usuario.Estciv == null ? new Parametro(":estciv", DbType.String, "".PadRight(1, ' ')) : new Parametro(":estciv", DbType.String, usuario.Estciv.Trim().Length >= 1 ? usuario.Estciv.Trim().Substring(0, 1) : usuario.Estciv.Trim().PadRight(1, ' ')));
            lstParam.Add(usuario.Email == null ? new Parametro(":email", DbType.String, "".PadRight(100, ' ')) : new Parametro(":email", DbType.String, usuario.Email.Trim().Length >= 100 ? usuario.Email.Trim().Substring(0, 100) : usuario.Email.Trim().PadRight(100, ' ')));
            lstParam.Add(usuario.Endere == null ? new Parametro(":endere", DbType.String, "".PadRight(40, ' ')) : new Parametro(":endere", DbType.String, usuario.Endere.Trim().Length >= 40 ? usuario.Endere.Trim().Substring(0, 40) : usuario.Endere.Trim().PadRight(40, ' ')));
            lstParam.Add(usuario.Nrend == null ? new Parametro(":nrend", DbType.String, "".PadRight(6, ' ')) : new Parametro(":nrend", DbType.String, usuario.Nrend.Trim().Length >= 6 ? usuario.Nrend.Trim().Substring(0, 6) : usuario.Nrend.Trim().PadRight(6, ' ')));
            lstParam.Add(usuario.Comend == null ? new Parametro(":comend", DbType.String, "".PadRight(20, ' ')) : new Parametro(":comend", DbType.String, usuario.Comend.Trim().Length >= 20 ? usuario.Comend.Trim().Substring(0, 20) : usuario.Comend.Trim().PadRight(20, ' ')));
            lstParam.Add(usuario.Bairro == null ? new Parametro(":bairro", DbType.String, "".PadRight(40, ' ')) : new Parametro(":bairro", DbType.String, usuario.Bairro.Trim().Length >= 40 ? usuario.Bairro.Trim().Substring(0, 40) : usuario.Bairro.Trim().PadRight(40, ' ')));
            lstParam.Add(usuario.Codmun == null ? new Parametro(":codmun", DbType.String, "".PadRight(7, ' ')) : new Parametro(":codmun", DbType.String, usuario.Codmun.Trim().Length >= 7 ? usuario.Codmun.Trim().Substring(0, 7) : usuario.Codmun.Trim().PadRight(7, ' ')));
            lstParam.Add(usuario.Munici == null ? new Parametro(":munici", DbType.String, "".PadRight(30, ' ')) : new Parametro(":munici", DbType.String, usuario.Munici.Trim().Length >= 30 ? usuario.Munici.Trim().Substring(0, 30) : usuario.Munici.Trim().PadRight(30, ' ')));
            lstParam.Add(usuario.Estado == null ? new Parametro(":estado", DbType.String, "".PadRight(2, ' ')) : new Parametro(":estado", DbType.String, usuario.Estado.Trim().Length >= 2 ? usuario.Estado.Trim().Substring(0, 2) : usuario.Estado.Trim().PadRight(2, ' ')));
            lstParam.Add(usuario.Cepusr == null ? new Parametro(":cepusr", DbType.String, "".PadRight(8, ' ')) : new Parametro(":cepusr", DbType.String, usuario.Cepusr.Trim().Length >= 8 ? usuario.Cepusr.Trim().Substring(0, 8) : usuario.Cepusr.Trim().PadRight(8, ' ')));
            lstParam.Add(usuario.Ycaren == null ? new Parametro(":ycaren", DbType.String, "".PadRight(1, ' ')) : new Parametro(":ycaren", DbType.String, usuario.Ycaren.Trim().Length >= 1 ? usuario.Ycaren.Trim().Substring(0, 1) : usuario.Ycaren.Trim().PadRight(1, ' ')));

            BaseDAO.ExecuteNonQuery(sql, lstParam, db);
        }

        internal static void BloquearUsuario(PUsuarioVO usuario, string cdMotivoDesligUsuario, string descMotivoDesligUsuario, EvidaDatabase db)
        {
            string sql = "update BA1010 set ba1_image = 'DISABLE', ba1_datblo = :hoje, ba1_motblo = :motblo, ba1_consid = 'F' " +
                "	where upper(trim(ba1_codint)) = upper(trim(:codint)) and upper(trim(ba1_codemp)) = upper(trim(:codemp)) and upper(trim(ba1_matric)) = upper(trim(:matric)) and upper(trim(ba1_tipreg)) = upper(trim(:tipreg)) ";

            string hoje = DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string agora = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":hoje", DbType.String, hoje));
            lstParams.Add(cdMotivoDesligUsuario == null ? new Parametro(":motblo", DbType.String, "".PadRight(3, ' ')) : new Parametro(":motblo", DbType.String, cdMotivoDesligUsuario.Trim().Length >= 3 ? cdMotivoDesligUsuario.Trim().Substring(0, 3) : cdMotivoDesligUsuario.Trim().PadRight(3, '0')));
            lstParams.Add(new Parametro(":codint", DbType.String, usuario.Codint.Trim()));
            lstParams.Add(new Parametro(":codemp", DbType.String, usuario.Codemp.Trim()));
            lstParams.Add(new Parametro(":matric", DbType.String, usuario.Matric.Trim()));
            lstParams.Add(new Parametro(":tipreg", DbType.String, usuario.Tipreg.Trim()));

            BaseDAO.ExecuteNonQuery(sql, lstParams, db);

            sql = " insert into BCA010 (bca_matric, bca_tipreg, bca_tipo, bca_data, bca_motblo, bca_obs, bca_usuope, bca_nivblq, bca_matant, bca_blofat, bca_datped, bca_datlan, bca_horlan, R_E_C_N_O_) " +
                  " values (:matric, :tipreg, :tipo, :data, :motblo, :obs, :usuope, :nivblq, :matant, :blofat, :datped, :datlan, :horlan, :recno) ";

            Int64 recno = NextRecnoBCA(db);
            
            List<Parametro> lstParams2 = new List<Parametro>();
            lstParams2.Add(new Parametro(":matric", DbType.String, usuario.Codint.Trim() + usuario.Codemp.Trim() + usuario.Matric.Trim()));
            lstParams2.Add(new Parametro(":tipreg", DbType.String, usuario.Tipreg.PadLeft(2, '0')));
            lstParams2.Add(new Parametro(":tipo", DbType.String, "0"));
            lstParams2.Add(new Parametro(":data", DbType.String, hoje));
            lstParams2.Add(cdMotivoDesligUsuario == null ? new Parametro(":motblo", DbType.String, "".PadRight(3, ' ')) : new Parametro(":motblo", DbType.String, cdMotivoDesligUsuario.Trim().Length >= 3 ? cdMotivoDesligUsuario.Trim().Substring(0, 3) : cdMotivoDesligUsuario.Trim().PadRight(3, '0')));
            lstParams2.Add(new Parametro(":obs", DbType.String, descMotivoDesligUsuario.Trim()));
            lstParams2.Add(new Parametro(":usuope", DbType.String, "Administrador"));
            lstParams2.Add(new Parametro(":nivblq", DbType.String, "S"));
            lstParams2.Add(usuario.Matant == null ? new Parametro(":matant", DbType.String, "".PadRight(20, ' ')) : new Parametro(":matant", DbType.String, usuario.Matant.Trim().Length >= 20 ? usuario.Matant.Trim().Substring(0, 20) : usuario.Matant.Trim().PadRight(20, ' ')));
            lstParams2.Add(new Parametro(":blofat", DbType.String, "1"));
            lstParams2.Add(new Parametro(":datped", DbType.String, hoje));
            lstParams2.Add(new Parametro(":datlan", DbType.String, hoje));
            lstParams2.Add(new Parametro(":horlan", DbType.String, agora));
            lstParams2.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParams2, db);
        }

        internal static void DesbloquearUsuario(PUsuarioVO usuario, string cdMotivoDesligUsuario, string descMotivoDesligUsuario, DateTime inicioVigencia, EvidaDatabase db)
        {
            string sql = " insert into BCA010 (bca_matric, bca_tipreg, bca_tipo, bca_data, bca_motblo, bca_obs, bca_usuope, bca_nivblq, bca_matant, bca_blofat, bca_datlan, bca_horlan, R_E_C_N_O_) " +
                  " values (:matric, :tipreg, :tipo, :data, :motblo, :obs, :usuope, :nivblq, :matant, :blofat, :datlan, :horlan, :recno) ";

            string hoje = DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string agora = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

            Int64 recno = NextRecnoBCA(db);

            List<Parametro> lstParams2 = new List<Parametro>();
            lstParams2.Add(new Parametro(":matric", DbType.String, usuario.Codint.Trim() + usuario.Codemp.Trim() + usuario.Matric.Trim()));
            lstParams2.Add(new Parametro(":tipreg", DbType.String, usuario.Tipreg.PadLeft(2, '0')));
            lstParams2.Add(new Parametro(":tipo", DbType.String, "1"));
            lstParams2.Add(new Parametro(":data", DbType.String, inicioVigencia.ToString("yyyyMMdd", CultureInfo.InvariantCulture)));
            lstParams2.Add(cdMotivoDesligUsuario == null ? new Parametro(":motblo", DbType.String, "".PadRight(3, ' ')) : new Parametro(":motblo", DbType.String, cdMotivoDesligUsuario.Trim().Length >= 3 ? cdMotivoDesligUsuario.Trim().Substring(0, 3) : cdMotivoDesligUsuario.Trim().PadRight(3, '0')));
            lstParams2.Add(new Parametro(":obs", DbType.String, descMotivoDesligUsuario.Trim()));
            lstParams2.Add(new Parametro(":usuope", DbType.String, "Administrador"));
            lstParams2.Add(new Parametro(":nivblq", DbType.String, "S"));
            lstParams2.Add(usuario.Matant == null ? new Parametro(":matant", DbType.String, "".PadRight(20, ' ')) : new Parametro(":matant", DbType.String, usuario.Matant.Trim().Length >= 20 ? usuario.Matant.Trim().Substring(0, 20) : usuario.Matant.Trim().PadRight(20, ' ')));
            lstParams2.Add(new Parametro(":blofat", DbType.String, "1"));
            lstParams2.Add(new Parametro(":datlan", DbType.String, hoje));
            lstParams2.Add(new Parametro(":horlan", DbType.String, agora));
            lstParams2.Add(new Parametro(":recno", DbType.Int64, recno));

            BaseDAO.ExecuteNonQuery(sql, lstParams2, db);
        }

        internal static void AlterarEmailTitular(string codint, string codemp, string matric, string email, EvidaDatabase evdb)
        {
            string sql = "UPDATE BA1010 SET BA1_EMAIL = :email WHERE upper(trim(BA1_CODINT)) = upper(trim(:codint)) and upper(trim(BA1_CODEMP)) = upper(trim(:codemp)) and upper(trim(BA1_MATRIC)) = upper(trim(:matric)) and upper(trim(BA1_TIPUSU)) = 'T'";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":email", DbType.String, email.Trim().Length >= 100 ? email.Trim().Substring(0, 100).ToUpper() : email.Trim().PadRight(100, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":codint", DbType.String, codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, matric.Trim());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

        internal static void SalvarDadosPessoais(PUsuarioVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE BA1010 SET BA1_ENDERE = :endereco, BA1_NR_END = :numero, " +
                "	BA1_COMEND = :complemento, BA1_BAIRRO = :bairro, BA1_CEPUSR =:cep, BA1_CODMUN = :cidade, BA1_MUNICI = :municipio, BA1_ESTADO = :estado, " +
                "	BA1_EMAIL = :email, BA1_DDD = :ddd, BA1_TELRES = :telResidencial, " +
                "	BA1_TELCOM = :telComercial, BA1_TELEFO = :telCelular " +
                "	WHERE upper(trim(BA1_CODINT)) = upper(trim(:codint)) AND upper(trim(BA1_CODEMP)) = upper(trim(:codemp)) AND upper(trim(BA1_MATRIC)) = upper(trim(:matric)) AND upper(trim(BA1_TIPREG)) = upper(trim(:tipreg)) ";    

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":endereco", DbType.String, vo.Endere.Trim().Length >= 40 ? vo.Endere.Trim().Substring(0, 40).ToUpper() : vo.Endere.Trim().PadRight(40, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Nrend.Trim().Length >= 6 ? vo.Nrend.Trim().Substring(0, 6).ToUpper() : vo.Nrend.Trim().PadRight(6, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Comend.Trim().Length >= 20 ? vo.Comend.Trim().Substring(0, 20).ToUpper() : vo.Comend.Trim().PadRight(20, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Bairro.Trim().Length >= 40 ? vo.Bairro.Trim().Substring(0, 40).ToUpper() : vo.Bairro.Trim().PadRight(40, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":cep", DbType.String, vo.Cepusr.Trim().Length >= 8 ? vo.Cepusr.Trim().Substring(0, 8).ToUpper() : vo.Cepusr.Trim().PadRight(8, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Codmun.Trim().Length >= 7 ? vo.Codmun.Trim().Substring(0, 7).ToUpper() : vo.Codmun.Trim().PadRight(7, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":municipio", DbType.String, vo.Munici.Trim().Length >= 30 ? vo.Munici.Trim().Substring(0, 30).ToUpper() : vo.Munici.Trim().PadRight(30, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":estado", DbType.String, vo.Estado.Trim().Length >= 2 ? vo.Estado.Trim().Substring(0, 2).ToUpper() : vo.Estado.Trim().PadRight(2, ' ').ToUpper());
            db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email.Trim().Length >= 100 ? vo.Email.Trim().Substring(0, 100).ToUpper() : vo.Email.Trim().PadRight(100, ' ').ToUpper());

            db.AddInParameter(dbCommand, ":ddd", DbType.String, vo.Ddd.Trim().Length >= 3 ? vo.Ddd.Trim().Substring(0, 3) : vo.Ddd.Trim().PadRight(3, ' '));
            db.AddInParameter(dbCommand, ":telResidencial", DbType.String, vo.Telres.Trim().Length >= 15 ? vo.Telres.Trim().Substring(0, 15) : vo.Telres.Trim().PadRight(15, ' '));
            db.AddInParameter(dbCommand, ":telComercial", DbType.String, vo.Telcom.Trim().Length >= 15 ? vo.Telcom.Trim().Substring(0, 15) : vo.Telcom.Trim().PadRight(15, ' '));
            db.AddInParameter(dbCommand, ":telCelular", DbType.String, vo.Telefo.Trim().Length >= 15 ? vo.Telefo.Trim().Substring(0, 15) : vo.Telefo.Trim().PadRight(15, ' '));

            db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Tipreg.Trim());

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }

    }
}
