using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.SCL;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO.SCL {
	internal class SclUsuarioDAO {
		const string FIELDS_USUARIO = "CD_USUARIO, NR_MATRICULA, NM_USUARIO, ID_ATIVO, DT_CADASTRO, DT_EXP_SENHA, NVL(USER_UPDATE, USER_CREATE) USER_UPDATE, NVL(DATE_UPDATE, DATE_CREATE) DATE_UPDATE";
		
        internal static SclUsuarioVO Logar(string login, string senha, EvidaDatabase db) {
			string sql = "SELECT " + FIELDS_USUARIO +
				" FROM SCL_USUARIO U " +
				" WHERE upper(U.CD_USUARIO) = :login AND U.CD_SENHA = EVIDA.CV_ENCRYPT(:senha)";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
			lstParams.Add(new Parametro() { Name = ":senha", Tipo = DbType.String, Value = senha.ToUpper() });

			List<SclUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static SclUsuarioVO FromDataRow(DataRow dr) {
			SclUsuarioVO vo = new SclUsuarioVO();
			vo.Login = dr.Field<string>("cd_usuario");
			vo.Matricula = (long?)dr.Field<decimal?>("nr_matricula");
			vo.Nome = dr.Field<string>("nm_usuario");
			vo.Ativo = Convert.ToInt32(dr["id_ativo"]) == 1;
			vo.DataCadastro = Convert.ToDateTime(dr["dt_cadastro"]);
			vo.DataExpiracaoSenha = BaseDAO.GetNullableDate(dr, "dt_exp_senha");

			vo.UserUpdate = dr.Field<string>("USER_UPDATE");
			vo.DateUpdate = dr.Field<string>("DATE_UPDATE");

			return vo;
		}

		internal static SclUsuarioDominioVO FromDataRowDominio(DataRow dr) {
			SclUsuarioDominioVO vo = new SclUsuarioDominioVO();
			SclUsuarioDominio dominio;

			if (!Enum.TryParse<SclUsuarioDominio>(Convert.ToString(dr["id_dominio"]), out dominio)) {
				return null;
			}

			vo.CdUsuario = dr.Field<string>("cd_usuario");
			vo.CdAutorizacaoI = dr.Field<string>("cd_autorizacao_i");
			
			vo.IdDominio = dominio;
			return vo;
		}
		
		internal static SclPerfilUsuarioVO FromDataRowPerfilUsuario(DataRow dr) {
			SclPerfilUsuarioVO vo = new SclPerfilUsuarioVO();

			vo.CdUsuario = dr.Field<string>("cd_usuario");
			vo.CdGrupo = Convert.ToInt32(dr["cd_grupo"]);
			vo.CdSistema = dr.Field<string>("cd_sistema");
			vo.NmGrupo = dr.Field<string>("nm_grupo");
			return vo;
		}

		internal static string GetCodAlternativoBeneficiario(string cdUsuario, EvidaDatabase db) {
			string sql = "SELECT CD_AUTORIZACAO_I " +
				" FROM SCL_USUARIO_DOMINIO WHERE CD_USUARIO = :cdUsuario AND ID_DOMINIO = 'GAL_NRCARTEIRA_BENEF'";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":cdUsuario", Tipo = DbType.String, Value = cdUsuario });

			Func<DataRow, string> convert = delegate(DataRow dr)
			{
				return Convert.ToString(dr["CD_AUTORIZACAO_I"]);
			};

			List<string> lst = BaseDAO.ExecuteDataSet(db, sql, convert, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static SclUsuarioVO GetUsuarioScl(string login, EvidaDatabase db) {
			string sql = "SELECT " + FIELDS_USUARIO +
				" FROM SCL_USUARIO U " +
				" WHERE U.CD_USUARIO = :login";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });

			List<SclUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static List<SclUsuarioDominioVO> GetDominios(string login, EvidaDatabase db) {
			string sql = "SELECT CD_USUARIO, ID_DOMINIO, CD_AUTORIZACAO_I " +
				" FROM SCL_USUARIO_DOMINIO U " +
				" WHERE U.CD_USUARIO = :login" +
				" ORDER BY ID_DOMINIO, CD_AUTORIZACAO_I";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });

			List<SclUsuarioDominioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowDominio, lstParams);
			if (lst != null) {
				lst.RemoveAll(x => x == null);
			}
			return lst;
		}

		internal static List<SclPerfilUsuarioVO> GetPerfis(string login, EvidaDatabase db) {
			string sql = "SELECT PU.CD_USUARIO, PU.CD_GRUPO, PU.CD_SISTEMA, GA.NM_GRUPO " +
				" FROM SCL_PERFIL_USUARIO PU, SCL_GRUPO_ACESSO GA " +
				" WHERE PU.CD_USUARIO = :login AND " +
				"	PU.CD_SISTEMA = GA.CD_SISTEMA AND PU.CD_GRUPO = GA.CD_GRUPO ";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });

			List<SclPerfilUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRowPerfilUsuario, lstParams);
			return lst;
		}

		internal static DataTable PesquisarUsuariosScl(string login, string nome, EvidaDatabase db) {
			string sql = "SELECT U.CD_USUARIO, U.NM_USUARIO, U.DT_CADASTRO, U.ID_ATIVO " +
				" FROM SCL_USUARIO U " +
				" WHERE 1 = 1 ";
			List<Parametro> lstParams = new List<Parametro>();
			if (!string.IsNullOrEmpty(login)) {
				sql += " AND U.CD_USUARIO = :login ";
				lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
			}
			if (!string.IsNullOrEmpty(nome)) {
                sql += " AND upper(trim(U.NM_USUARIO)) LIKE upper(trim(:nome)) ";
				lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = "%" + nome.ToUpper() + "%" });
			}

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstParams);
			return dt;
		}

		internal static void AlterarUsuario(string login, string senha, DateTime? dtExpiracao, bool isAtivo, UsuarioVO usuarioLogado, EvidaDatabase evdb) {
			string sql = "UPDATE SCL_USUARIO SET DT_EXP_SENHA = :dtExpiracao, ID_ATIVO = :isAtivo, USER_UPDATE = :userInt, DATE_UPDATE = :dateInt " +
				 " WHERE CD_USUARIO = :login";

			login = login.ToUpper();
			bool changeSenha = !string.IsNullOrEmpty(senha);
			List<Parametro> lstParams = new List<Parametro>();		
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login });
			lstParams.Add(new Parametro() { Name = ":dtExpiracao", Tipo = DbType.Date, Value = dtExpiracao });
			lstParams.Add(new Parametro() { Name = ":isAtivo", Tipo = DbType.Int32, Value = isAtivo ? 1 : 0 });
			lstParams.Add(new Parametro() { Name = ":userInt", Tipo = DbType.String, Value = "INTRANET - " + usuarioLogado.Login});
			lstParams.Add(new Parametro() { Name = ":dateInt", Tipo = DbType.String, Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") });

			BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);

			if (changeSenha) {
				AlterarSenha(login, senha, usuarioLogado, evdb);
			}

		}

		private static void AlterarSenha(string login, string senha, UsuarioVO usuarioLogado, EvidaDatabase evdb) {
			const string sqlProcCanc = "BEGIN PC_ALTERA_SENHA_ISA(:login, :senha, :usuario); END;";

			Database db = evdb.Database;
			DbCommand cmd = db.GetSqlStringCommand(sqlProcCanc);
			db.AddInParameter(cmd, ":login", DbType.String, login.ToUpper());
			db.AddInParameter(cmd, ":senha", DbType.String, senha.ToUpper());
			db.AddInParameter(cmd, ":usuario", DbType.String, usuarioLogado.Login);

			BaseDAO.ExecuteNonQuery(cmd, evdb);
		}

		internal static SclUsuarioVO GetUsuarioSclByDominio(SclUsuarioDominio dominio, string vlDominio, EvidaDatabase db) {
			string sql = "SELECT CD_USUARIO, NR_MATRICULA, NM_USUARIO, ID_ATIVO, DT_CADASTRO, DT_EXP_SENHA " +
				" FROM SCL_USUARIO U " +
				" WHERE EXISTS (SELECT 1 FROM SCL_USUARIO_DOMINIO UD WHERE UD.CD_USUARIO = U.CD_USUARIO " +
				"		AND UD.ID_DOMINIO = :dominio AND UD.CD_AUTORIZACAO_I = :vlDominio )";
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":dominio", Tipo = DbType.String, Value = dominio.ToString() });
			lstParams.Add(new Parametro() { Name = ":vlDominio", Tipo = DbType.String, Value = vlDominio });

			List<SclUsuarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static void CriarUsuarioScl(SclUsuarioVO vo, string senha, UsuarioVO usuarioLogado, EvidaDatabase db) {
			string sql = "INSERT INTO SCL_USUARIO (CD_USUARIO, CD_SENHA, NM_USUARIO, DT_CADASTRO, ID_ATIVO, DT_EXP_SENHA, USER_CREATE, DATE_CREATE, USER_UPDATE, DATE_UPDATE) " +
				" VALUES (:login, EVIDA.CV_ENCRYPT(:senha), :nome, :cadastro, :ativo, :expSenha, :userCreate, :dateCreate, :userCreate, :dateCreate) ";

			vo.Login = vo.Login.ToUpper();
			senha = senha.ToUpper();
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = vo.Login });
			lstParams.Add(new Parametro() { Name = ":senha", Tipo = DbType.String, Value = senha });
			lstParams.Add(new Parametro() { Name = ":nome", Tipo = DbType.String, Value = vo.Nome.ToUpper() });
			lstParams.Add(new Parametro() { Name = ":cadastro", Tipo = DbType.Date, Value = vo.DataCadastro.Date });
			lstParams.Add(new Parametro() { Name = ":ativo", Tipo = DbType.Int32, Value = vo.Ativo ? 1 : 0 });
			lstParams.Add(new Parametro() { Name = ":expSenha", Tipo = DbType.DateTime, Value = vo.DataExpiracaoSenha });
			lstParams.Add(new Parametro() { Name = ":userCreate", Tipo = DbType.String, Value = "INTRANET - " + usuarioLogado.Login });
			lstParams.Add(new Parametro() { Name = ":dateCreate", Tipo = DbType.String, Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") });

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);

			AssociarPerfilUsuario(vo.Login, vo.Perfis[0], usuarioLogado, db);
			AssociarDominioUsuario(vo.Login, vo.Dominios[0], usuarioLogado, db);

			AlterarSenha(vo.Login, senha, usuarioLogado, db);
		}

		private static void AssociarPerfilUsuario(string login, SclPerfilUsuarioVO perfil, UsuarioVO usuarioLogado, EvidaDatabase db) {
			string sql = "INSERT INTO SCL_PERFIL_USUARIO (CD_USUARIO, CD_GRUPO, CD_SISTEMA, USER_CREATE, DATE_CREATE) " +
				" VALUES (:login, :grupo, :sistema, :userCreate, :dateCreate) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
			lstParams.Add(new Parametro() { Name = ":grupo", Tipo = DbType.Int32, Value = perfil.CdGrupo });
			lstParams.Add(new Parametro() { Name = ":sistema", Tipo = DbType.String, Value = perfil.CdSistema });
			lstParams.Add(new Parametro() { Name = ":userCreate", Tipo = DbType.String, Value = "INTRANET - " + usuarioLogado.Login });
			lstParams.Add(new Parametro() { Name = ":dateCreate", Tipo = DbType.String, Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") });

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		private static void AssociarDominioUsuario(string login, SclUsuarioDominioVO dominio, UsuarioVO usuarioLogado, EvidaDatabase db) {
			string sql = "INSERT INTO SCL_USUARIO_DOMINIO (CD_USUARIO, ID_DOMINIO, CD_AUTORIZACAO_I, USER_CREATE, DATE_CREATE) " +
				" VALUES (:login, :idDominio, :vlDominio, :userCreate, :dateCreate) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":login", Tipo = DbType.String, Value = login.ToUpper() });
			lstParams.Add(new Parametro() { Name = ":idDominio", Tipo = DbType.String, Value = dominio.IdDominio.ToString() });
			lstParams.Add(new Parametro() { Name = ":vlDominio", Tipo = DbType.String, Value = dominio.CdAutorizacaoI });
			lstParams.Add(new Parametro() { Name = ":userCreate", Tipo = DbType.String, Value = "INTRANET - " + usuarioLogado.Login });
			lstParams.Add(new Parametro() { Name = ":dateCreate", Tipo = DbType.String, Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") });

			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}
	}
}
