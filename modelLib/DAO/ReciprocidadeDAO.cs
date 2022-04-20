using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.DAO {
	internal class ReciprocidadeDAO {
		static EVidaLog log = new EVidaLog(typeof(ReciprocidadeDAO));

        private static StatusReciprocidade[] STATUS_EM_ANDAMENTO = new StatusReciprocidade[] { StatusReciprocidade.PENDENTE, StatusReciprocidade.ENVIADO };

		private static int NextId(EvidaDatabase db) {
			string sql = "SELECT sq_ev_reciprocidade.nextval FROM DUAL";
			decimal idSolicitacao = (decimal)BaseDAO.ExecuteScalar(db, sql);
			return (int)idSolicitacao;
		}

		#region Empresas

		internal static List<EmpresaReciprocidadeVO> ListarEmpresas(EvidaDatabase db) {
			string sql = "SELECT * FROM EV_EMPRESA_RECIPROCIDADE A ORDER BY A.CD_EMPRESA_RECIPROCIDADE ";
			List<EmpresaReciprocidadeVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromEmpresaDataRow);
			return lst;
		}
		
		internal static EmpresaReciprocidadeVO FromEmpresaDataRow(DataRow dr) {
			EmpresaReciprocidadeVO vo = new EmpresaReciprocidadeVO();
			vo.Codigo = Convert.ToInt32(dr["cd_empresa_reciprocidade"]);
			vo.Nome = Convert.ToString(dr["nm_empresa"]);

			vo.Endereco = new EnderecoVO();
			vo.Endereco.Cep = Convert.ToInt32(dr["nu_cep"]);
			vo.Endereco.Bairro = Convert.ToString(dr["ds_bairro"]);
			vo.Endereco.Cidade = Convert.ToString(dr["ds_cidade"]);
			vo.Endereco.Complemento = Convert.ToString(dr["ds_complemento"]);
			vo.Endereco.NumeroEndereco = Convert.ToString(dr["ds_numero"]);
			vo.Endereco.Rua = Convert.ToString(dr["ds_rua"]);
			vo.Endereco.Uf = Convert.ToString(dr["ds_uf"]);

			vo.Email = new List<string>();
			vo.Email.Add(Convert.ToString(dr["ds_email"]));
			vo.Telefone = new List<string>();
			vo.Telefone.Add(Convert.ToString(dr["ds_telefone"]));
			vo.Fax = new List<string>();
			if (dr["ds_fax"] != DBNull.Value)
				vo.Fax.Add(Convert.ToString(dr["ds_fax"]));
			vo.UrlGuia = Convert.ToString(dr["ds_url_guia"]);
			vo.Contato = Convert.ToString(dr["ds_contato"]);
			vo.AreaContato = Convert.ToString(dr["ds_area_contato"]);
			vo.FuncaoContato = Convert.ToString(dr["ds_funcao_contato"]);

			string str = Convert.ToString(dr["ds_area_atuacao"]);
			List<string> lst = FormatUtil.StringToList(str);

			vo.AreaAtuacao = lst;

			str = Convert.ToString(dr["ds_email_alternativo"]);
			lst = FormatUtil.StringToList(str);
			vo.Email.AddRange(lst);

			str = Convert.ToString(dr["ds_telefone_alternativo"]);
			lst = FormatUtil.StringToList(str);
			vo.Telefone.AddRange(lst);

			str = Convert.ToString(dr["ds_fax_alternativo"]);
			lst = FormatUtil.StringToList(str);
			vo.Fax.AddRange(lst);

			return vo;
		}

		internal static EmpresaReciprocidadeVO GetEmpresaById(int id, EvidaDatabase db) {
			string sql = "select cd_empresa_reciprocidade, nm_empresa, ds_area_atuacao, " +
				" nu_cep, ds_bairro, ds_cidade, ds_complemento, ds_numero, ds_rua, ds_uf, " +
				" ds_email, ds_telefone, ds_fax, ds_url_guia, ds_contato, ds_area_contato, ds_funcao_contato, " +
				" ds_email_alternativo, ds_telefone_alternativo, ds_fax_alternativo " +
				" FROM EV_EMPRESA_RECIPROCIDADE A " +
				" WHERE a.cd_empresa_reciprocidade = :id ";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro(":id", DbType.Int32, id));

			List<EmpresaReciprocidadeVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromEmpresaDataRow, lstP);

			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static void Salvar(EmpresaReciprocidadeVO vo, bool novoRegistro, EvidaDatabase db) {
			if (!novoRegistro) {
				Update(vo, db);
			} else {
				Insert(vo, db);
			}
		}

		private static void Insert(EmpresaReciprocidadeVO vo, EvidaDatabase evdb) {
			string sql = "insert into ev_empresa_reciprocidade " +
				" (cd_empresa_reciprocidade, nm_empresa, ds_area_atuacao, nu_cep, ds_bairro, ds_cidade, " +
				" ds_complemento, ds_numero, ds_rua, ds_uf, ds_email, ds_telefone, ds_fax, ds_url_guia, " +
				" ds_contato, ds_area_contato, ds_funcao_contato, " +
				" ds_email_alternativo, ds_telefone_alternativo, ds_fax_alternativo) " +
				" VALUES (:id, :nome, :area, :cep, :bairro, :cidade, " +
				" :complemento, :numero, :rua, :uf, :email, :telefone, :fax, :url, " +
				" :contato, :areaContato, :funcaoContato, :emailAlt, :telefoneAlt, :faxAlt)";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Codigo);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Nome.ToUpper());

			db.AddInParameter(dbCommand, ":area", DbType.String, FormatUtil.ListToString(vo.AreaAtuacao));

			db.AddInParameter(dbCommand, ":cep", DbType.Int32, vo.Endereco.Cep);
			db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Endereco.Bairro);
			db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Endereco.Cidade);
			db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Endereco.Complemento);
			db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Endereco.NumeroEndereco);
			db.AddInParameter(dbCommand, ":rua", DbType.String, vo.Endereco.Rua);
			db.AddInParameter(dbCommand, ":uf", DbType.String, vo.Endereco.Uf);

			db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email[0]);
			db.AddInParameter(dbCommand, ":telefone", DbType.String, vo.Telefone[0]);
			db.AddInParameter(dbCommand, ":fax", DbType.String, vo.Fax.Count > 0 ? vo.Fax[0] : "");
			db.AddInParameter(dbCommand, ":url", DbType.String, vo.UrlGuia);

			db.AddInParameter(dbCommand, ":contato", DbType.String, vo.Contato);
			db.AddInParameter(dbCommand, ":areaContato", DbType.String, vo.AreaContato);
			db.AddInParameter(dbCommand, ":funcaoContato", DbType.String, vo.FuncaoContato);

			List<string> lst = null;
			string str = null;

			if (vo.Email.Count > 1) {
				lst = vo.Email.GetRange(1, vo.Email.Count - 1);
				str = FormatUtil.ListToString(lst);
			}
			db.AddInParameter(dbCommand, ":emailAlt", DbType.String, str);

			str = null;
			if (vo.Telefone.Count > 1) {
				lst = vo.Telefone.GetRange(1, vo.Telefone.Count - 1);
				str = FormatUtil.ListToString(lst);				
			}
			db.AddInParameter(dbCommand, ":telefoneAlt", DbType.String, str);

			str = null;
			if (vo.Fax.Count > 1) {
				lst = vo.Fax.GetRange(1, vo.Fax.Count - 1);
				str = FormatUtil.ListToString(lst);
			}
			db.AddInParameter(dbCommand, ":faxAlt", DbType.String, str);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static void Update(EmpresaReciprocidadeVO vo, EvidaDatabase evdb) {
			string sql = "UPDATE ev_empresa_reciprocidade SET " +
				" nm_empresa = :nome, ds_area_atuacao = :area, nu_cep = :cep, ds_bairro = :bairro, " +
				" ds_cidade = :cidade, ds_complemento = :complemento, ds_numero = :numero, ds_rua = :rua, " +
				" ds_uf = :uf, ds_email = :email, ds_telefone = :telefone, ds_fax = :fax, ds_url_guia = :url, " +
				" ds_contato = :contato, ds_area_contato = :areaContato, ds_funcao_contato = :funcaoContato, " +
				" ds_email_alternativo = :emailAlt, ds_telefone_alternativo = :telefoneAlt, ds_fax_alternativo = :faxAlt " +
				" WHERE cd_empresa_reciprocidade = :id";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);

			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.Codigo);
			db.AddInParameter(dbCommand, ":nome", DbType.String, vo.Nome.ToUpper());

			db.AddInParameter(dbCommand, ":area", DbType.String, FormatUtil.ListToString(vo.AreaAtuacao));

			db.AddInParameter(dbCommand, ":cep", DbType.Int32, vo.Endereco.Cep);
			db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Endereco.Bairro);
			db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Endereco.Cidade);
			db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Endereco.Complemento);
			db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Endereco.NumeroEndereco);
			db.AddInParameter(dbCommand, ":rua", DbType.String, vo.Endereco.Rua);
			db.AddInParameter(dbCommand, ":uf", DbType.String, vo.Endereco.Uf);

			db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email[0]);
			db.AddInParameter(dbCommand, ":telefone", DbType.String, vo.Telefone[0]);
			db.AddInParameter(dbCommand, ":fax", DbType.String, vo.Fax.Count > 0 ? vo.Fax[0] : "");
			db.AddInParameter(dbCommand, ":url", DbType.String, vo.UrlGuia);

			db.AddInParameter(dbCommand, ":contato", DbType.String, vo.Contato);
			db.AddInParameter(dbCommand, ":areaContato", DbType.String, vo.AreaContato);
			db.AddInParameter(dbCommand, ":funcaoContato", DbType.String, vo.FuncaoContato);

			List<string> lst = null;
			string str = null;


			if (vo.Email.Count > 1) {
				lst = vo.Email.GetRange(1, vo.Email.Count - 1);
				str = FormatUtil.ListToString(lst);
			}
			db.AddInParameter(dbCommand, ":emailAlt", DbType.String, str);

			str = null;
			if (vo.Telefone.Count > 1) {
				lst = vo.Telefone.GetRange(1, vo.Telefone.Count - 1);
				str = FormatUtil.ListToString(lst);
			}
			db.AddInParameter(dbCommand, ":telefoneAlt", DbType.String, str);

			str = null;
			if (vo.Fax.Count > 1) {
				lst = vo.Fax.GetRange(1, vo.Fax.Count - 1);
				str = FormatUtil.ListToString(lst);
			}
			db.AddInParameter(dbCommand, ":faxAlt", DbType.String, str);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		internal static void ExcluirEmpresa(int id, EvidaDatabase db) {
			string sql = "DELETE FROM ev_empresa_reciprocidade WHERE cd_empresa_reciprocidade = :id ";
			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro(":id", DbType.Int32, id));
			BaseDAO.ExecuteNonQuery(sql, lstP, db);
		}

		#endregion

        #region[OPERADORA SAÚDE]

        internal static List<POperadoraSaudeVO> ListarOperadoras(EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_OPERADORA_SAUDE A ORDER BY A.BA0_CODINT ";
            List<POperadoraSaudeVO> lst = BaseDAO.ExecuteDataSet(db, sql, POperadoraSaudeVO.FromDataRow);
            return lst;
        }

        internal static POperadoraSaudeVO GetOperadoraById(string id, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_OPERADORA_SAUDE A " +
                " WHERE A.BA0_CODINT = :id ";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro(":id", DbType.String, id));

            List<POperadoraSaudeVO> lst = BaseDAO.ExecuteDataSet(db, sql, POperadoraSaudeVO.FromDataRow, lstP);

            if (lst != null && lst.Count > 0)
                return lst[0];
            return null;
        }

        internal static List<PContatoOperadoraVO> ListarContatoOperadora(string id, EvidaDatabase db)
        {
            string sql = "SELECT * FROM VW_PR_CONTATO_OPERADORA A " +
                " WHERE A.BIM_CODINT = :id " +
                " ORDER BY A.BIM_CODIGO";

            List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro(":id", DbType.String, id.PadLeft(4, '0')));

            List<PContatoOperadoraVO> lst = BaseDAO.ExecuteDataSet(db, sql, PContatoOperadoraVO.FromDataRow, lstP);
            return lst;
        }

        #endregion

        internal static bool HasLimiteManutencaoOrtodontica(int cdReciprocidade, EvidaDatabase db) {
			string sql = "select nvl(max(mo.is_limite),0) " +
				" FROM ev_reciprocidade_benef rb, VW_PR_MANUTENCAO_ORTODONTICA mo " +
                " WHERE upper(trim(rb.BA1_CODEMP)) = upper(trim(mo.BD6_CODEMP)) " +
                " AND upper(trim(rb.BA1_MATRIC)) = upper(trim(mo.BD6_MATRIC)) " +
                " AND upper(trim(rb.BA1_TIPREG)) = upper(trim(mo.BD6_TIPREG)) " +
				"		AND rb.cd_solicitacao = :id";
			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro(":id", DbType.Int32, cdReciprocidade));

			object value = BaseDAO.ExecuteScalar(db, sql, lstP);

			if (value == null) return false;

			return Convert.ToInt32(value) == 1;
		}

		internal static DataTable RelatorioReciprocidade(long? cdMatricula, string protocoloAns, string nmTitular, string nmDependente, string cdEmpresa, DateTime? dtInicio, DateTime? dtFim, StatusReciprocidade? status, EvidaDatabase db) {

            string sql = "select a.cd_solicitacao, a.BA3_CODINT, a.BA3_CODEMP, a.BA3_MATRIC, f.BA3_MATEMP, a.dt_inicio, a.dt_fim, a.nu_cep, a.ds_bairro, a.ds_cidade, " +
                "	a.ds_complemento, a.ds_numero, a.ds_rua, a.ds_uf, a.ds_local, a.BA0_CODINT, a.ds_assistencia, a.ds_observacao, " +
                "	a.dt_criacao, a.dt_envio, a.cd_status, a.dt_alteracao, a.cd_usuario_alteracao, " +
                "	a.dt_aprovacao, a.cd_usuario_aprovacao, a.cd_usuario_envio, a.nr_protocolo_ans, " +
                "	ue.nm_usuario nm_usuario_envio, ua.nm_usuario nm_usuario_aprovacao, us.nm_usuario nm_usuario_situacao, " +
                "	er.BA0_NOMINT, t.BA1_NOMUSR " +
                " from ev_reciprocidade a, VW_PR_OPERADORA_SAUDE er, VW_PR_FAMILIA f, VW_PR_USUARIO t, ev_usuario ue , ev_usuario ua, ev_usuario us  " +
            " WHERE a.BA0_CODINT = er.BA0_CODINT " +
            "	and trim(a.BA3_CODINT) = trim(f.BA3_CODINT) and trim(a.BA3_CODEMP) = trim(f.BA3_CODEMP) and trim(a.BA3_MATRIC) = trim(f.BA3_MATRIC) " +
            "	and trim(a.BA3_CODINT) = trim(t.BA1_CODINT) and trim(a.BA3_CODEMP) = trim(t.BA1_CODEMP) and trim(a.BA3_MATRIC) = trim(t.BA1_MATRIC) and trim(t.BA1_TIPUSU) = 'T' " +
            "	and a.cd_usuario_envio =ue.id_usuario(+) and a.cd_usuario_aprovacao = ua.id_usuario(+) " +
            "	and a.cd_usuario_alteracao = us.id_usuario (+) ";

            List<Parametro> lstP = new List<Parametro>();
            if (cdMatricula != null)
            {
                sql += " AND trim(f.BA3_MATEMP) = trim(:matricula) ";
                lstP.Add(new Parametro(":matricula", DbType.String, cdMatricula.Value));
            }

            if (!string.IsNullOrEmpty(nmTitular))
            {
                sql += " AND upper(trim(t.BA1_NOMUSR)) LIKE upper(trim(:nmFuncionario)) ";
                lstP.Add(new Parametro(":nmFuncionario", DbType.String, "%" + nmTitular.ToUpper() + "%"));
            }

            if (!string.IsNullOrEmpty(nmDependente))
            {
                sql += " AND EXISTS (SELECT 1 FROM EV_RECIPROCIDADE_BENEF rb, VW_PR_USUARIO b " +
                    " WHERE rb.cd_solicitacao = a.cd_solicitacao AND rb.in_titular = 'N' " +
                    "		AND trim(rb.BA1_CODINT) = trim(b.BA1_CODINT) AND trim(rb.BA1_CODEMP) = trim(b.BA1_CODEMP) AND trim(rb.BA1_MATRIC) = trim(b.BA1_MATRIC) AND trim(rb.BA1_TIPREG) = trim(b.BA1_TIPREG) " +
                    "		and upper(trim(b.BA1_NOMUSR)) LIKE upper(trim(:nmDependente)) )";
                lstP.Add(new Parametro(":nmDependente", DbType.String, "%" + nmDependente.ToUpper() + "%"));
            }

            if (cdEmpresa != null)
            {
                sql += " AND a.BA0_CODINT = :cdEmpresa ";
                lstP.Add(new Parametro(":cdEmpresa", DbType.String, cdEmpresa));
            }

            if (status != null)
            {
                sql += " AND a.cd_status = :status ";
                lstP.Add(new Parametro(":status", DbType.Int32, (int)status.Value));
            }

            if (dtInicio != null)
            {
                sql += " AND a.dt_criacao >= :inicio ";
                lstP.Add(new Parametro(":inicio", DbType.Date, dtInicio.Value));
            }

            if (dtFim != null)
            {
                sql += " AND a.dt_criacao <= :fim ";
                lstP.Add(new Parametro(":fim", DbType.Date, dtFim.Value));
            }

            if (!string.IsNullOrEmpty(protocoloAns))
            {
                sql += " AND a.nr_protocolo_ans = :protocoloAns ";
                lstP.Add(new Parametro(":protocoloAns", DbType.String, protocoloAns));
            }        

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

		#region Formulario

        internal static DataTable ChecarConcorrencia(string codint, string codemp, string matric, List<ReciprocidadeBenefVO> lst, DateTime dtInicio, DateTime dtFim, string cidade, string uf, EvidaDatabase db)
        {

            string sql = "SELECT R.CD_SOLICITACAO, B.BA1_NOMUSR, R.DT_INICIO, R.DT_FIM " +
                "	FROM EV_RECIPROCIDADE R, EV_RECIPROCIDADE_BENEF RB, VW_PR_USUARIO B " +
                "	WHERE R.CD_SOLICITACAO = RB.CD_SOLICITACAO " +
                "		AND trim(R.BA3_CODINT) = trim(B.BA1_CODINT) AND trim(R.BA3_CODEMP) = trim(B.BA1_CODEMP) AND trim(R.BA3_MATRIC) = trim(B.BA1_MATRIC) " +
                "		AND trim(RB.BA1_CODINT) = trim(B.BA1_CODINT) and trim(RB.BA1_CODEMP) = trim(B.BA1_CODEMP) and trim(RB.BA1_MATRIC) = trim(B.BA1_MATRIC) and trim(RB.BA1_TIPREG) = trim(B.BA1_TIPREG) " +
                "		AND R.DS_CIDADE = :cidade AND R.DS_UF = :uf " +
                "		AND trim(R.BA3_CODINT) = trim(:codint) AND trim(R.BA3_CODEMP) = trim(:codemp) AND trim(R.BA3_MATRIC) = trim(:matric) " +
                "		AND (	(:dtInicio between R.DT_INICIO AND R.DT_FIM) " +
                "			OR 	(:dtFim between R.DT_INICIO AND R.DT_FIM) " +
                "			OR  (:dtInicio <= R.DT_INICIO AND :dtFim >= R.DT_FIM) " +
                "		) " +
                "		AND RB.IN_TITULAR = :isTitular " +
                "		AND R.CD_STATUS <> :status ";

			List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro(":codint", DbType.String, codint));
            lstP.Add(new Parametro(":codemp", DbType.String, codemp));
            lstP.Add(new Parametro(":matric", DbType.String, matric));
			lstP.Add(new Parametro(":cidade", DbType.String, cidade));
			lstP.Add(new Parametro(":uf", DbType.String, uf));
			lstP.Add(new Parametro(":dtInicio", DbType.Date, dtInicio));
			lstP.Add(new Parametro(":dtFim", DbType.Date, dtFim));

			lstP.Add(new Parametro(":isTitular", DbType.String, "N"));
			lstP.Add(new Parametro(":status", DbType.Int32, (int)StatusReciprocidade.NEGADO));

            string operador = "";
            if (lst.Count > 0)
            {
                sql += " AND (";
                for (int i = 0; i < lst.Count; i++)
                {
                    if (i > 0) operador = " or ";
                    sql += operador + " (trim(RB.BA1_CODINT) = :a" + i + " and trim(RB.BA1_CODEMP) = :b" + i + " and trim(RB.BA1_MATRIC) = :c" + i + " and trim(RB.BA1_TIPREG) = :d" + i + ") ";
                    lstP.Add(new Parametro(":a" + i, DbType.String, lst[i].Codint));
                    lstP.Add(new Parametro(":b" + i, DbType.String, lst[i].Codemp));
                    lstP.Add(new Parametro(":c" + i, DbType.String, lst[i].Matric));
                    lstP.Add(new Parametro(":d" + i, DbType.String, lst[i].Tipreg));
                }
                sql += ") ";
            }

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

        internal static DataTable BuscarReciprocidade(string codint, string codemp, string matric, EvidaDatabase db)
        {

            string sql = "select a.cd_solicitacao, a.BA3_CODINT, a.BA3_CODEMP, a.BA3_MATRIC, a.dt_inicio, a.dt_fim, a.nu_cep, a.ds_bairro, a.ds_cidade, " +
                "	a.ds_complemento, a.ds_numero, a.ds_rua, a.ds_uf, a.ds_local, a.BA0_CODINT, a.ds_assistencia, a.ds_observacao, " +
                "	a.dt_criacao, a.dt_envio, a.cd_status, a.dt_alteracao, a.cd_usuario_alteracao, " +
                "	a.dt_aprovacao, a.cd_usuario_aprovacao, a.cd_usuario_envio, a.nr_protocolo_ans " +
                " from ev_reciprocidade a " +
            " WHERE trim(a.BA3_CODINT) = trim(:codint) AND trim(a.BA3_CODEMP) = trim(:codemp) AND trim(a.BA3_MATRIC) = trim(:matric) ";   

			List<Parametro> lstP = new List<Parametro>();
            lstP.Add(new Parametro(":codint", DbType.String, codint));
            lstP.Add(new Parametro(":codemp", DbType.String, codemp));
			lstP.Add(new Parametro(":matric", DbType.String, matric));

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

		internal static void Salvar(ReciprocidadeVO vo, List<ReciprocidadeBenefVO> lst, EvidaDatabase evdb) {

            string sql = "INSERT INTO ev_reciprocidade (cd_solicitacao, BA3_CODINT, BA3_CODEMP, BA3_MATRIC, dt_inicio, dt_fim, " +
                "	nu_cep, ds_bairro, ds_cidade, ds_complemento, ds_numero, ds_rua, ds_uf, " +
                "	ds_local, dt_criacao, cd_status, dt_alteracao, cd_usuario_alteracao, nr_protocolo_ans, cd_situacao) " +
                " VALUES (:id, :codint, :codemp, :matric, :dtInicio, :dtFim, " +
                "	:cep, :bairro, :cidade, :complemento, :numero, :rua, :uf, " +
                "	:local, LOCALTIMESTAMP, :status, LOCALTIMESTAMP, null, PKG_PROTOCOLO_ANS.F_NEXT_VALUE(), 0)";

			vo.CodSolicitacao = NextId(evdb);

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CodSolicitacao);
			db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
			db.AddInParameter(dbCommand, ":dtInicio", DbType.Date, vo.Inicio);
			db.AddInParameter(dbCommand, ":dtFim", DbType.Date, vo.Fim);

			db.AddInParameter(dbCommand, ":cep", DbType.Int32, vo.Endereco.Cep != 0 ? (object)vo.Endereco.Cep : DBNull.Value);
			db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Endereco.Bairro);
			db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Endereco.Cidade);
			db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Endereco.Complemento);
			db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Endereco.NumeroEndereco);
			db.AddInParameter(dbCommand, ":rua", DbType.String, vo.Endereco.Rua);
			db.AddInParameter(dbCommand, ":uf", DbType.String, vo.Endereco.Uf);

			db.AddInParameter(dbCommand, ":local", DbType.String, vo.Local);
			db.AddInParameter(dbCommand, ":status", DbType.Int32, (int)vo.Status);

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);

            sql = "INSERT INTO EV_RECIPROCIDADE_BENEF (CD_SOLICITACAO, BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, IN_TITULAR, BI3_TIPO, BA1_DATCAR) " +
                "	VALUES (:id, :codint, :codemp, :matric, :tipreg, :titular, :tipo, :datcar) ";

			dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":id", DbType.Int32, vo.CodSolicitacao);
			db.AddInParameter(dbCommand, ":codint", DbType.String);
            db.AddInParameter(dbCommand, ":codemp", DbType.String);
            db.AddInParameter(dbCommand, ":matric", DbType.String);
            db.AddInParameter(dbCommand, ":tipreg", DbType.String);
			db.AddInParameter(dbCommand, ":titular", DbType.StringFixedLength);
			db.AddInParameter(dbCommand, ":tipo", DbType.String);
            db.AddInParameter(dbCommand, ":datcar", DbType.String);

			foreach (ReciprocidadeBenefVO bVO in lst) {
                db.SetParameterValue(dbCommand, ":codint", bVO.Codint.Trim());
                db.SetParameterValue(dbCommand, ":codemp", bVO.Codemp.Trim());
                db.SetParameterValue(dbCommand, ":matric", bVO.Matric.Trim());
                db.SetParameterValue(dbCommand, ":tipreg", bVO.Tipreg.Trim());
				db.SetParameterValue(dbCommand, ":titular", bVO.IsTitular ? 'S' :'N');
                db.SetParameterValue(dbCommand, ":tipo", bVO.TipoPlano);
                db.SetParameterValue(dbCommand, ":datcar", bVO.InicioPlano);
				BaseDAO.ExecuteNonQuery(dbCommand, evdb);
			}		
		}

		internal static ReciprocidadeVO FromDataRow(DataRow dr) {
			ReciprocidadeVO vo = new ReciprocidadeVO();
			vo.CodSolicitacao = Convert.ToInt32(dr["cd_solicitacao"]);
			vo.Endereco = new EnderecoVO()
			{
				Bairro = Convert.ToString(dr["ds_bairro"]),
				Cep = dr["nu_cep"] != DBNull.Value ? Convert.ToInt32(dr["nu_cep"]) : 0,
				Cidade = Convert.ToString(dr["ds_cidade"]),
				Complemento = Convert.ToString(dr["ds_complemento"]),
				NumeroEndereco = Convert.ToString(dr["ds_numero"]),
				Rua = Convert.ToString(dr["ds_rua"]),
				Uf = Convert.ToString(dr["ds_uf"])
			};
			vo.Codint = Convert.ToString(dr["BA3_CODINT"]);
            vo.Codemp = Convert.ToString(dr["BA3_CODEMP"]);
			vo.Matric = Convert.ToString(dr["BA3_MATRIC"]);
			vo.Inicio = Convert.ToDateTime(dr["dt_inicio"]);
			vo.Fim = Convert.ToDateTime(dr["dt_fim"]);
			vo.Local = Convert.ToString(dr["ds_local"]);

			vo.DataCriacao = Convert.ToDateTime(dr["dt_criacao"]);
			vo.Status = (StatusReciprocidade)Convert.ToInt32(dr["cd_status"]);
			vo.MotivoCancelamento = Convert.ToString(dr["ds_motivo_cancelamento"]);

			if (dr["BA0_CODINT"] != DBNull.Value) {
                if (!string.IsNullOrEmpty(Convert.ToString(dr["BA0_CODINT"]).Trim()))
                {
                    vo.CodintReciprocidade = Convert.ToString(dr["BA0_CODINT"]);
                    vo.DataEnvio = Convert.ToDateTime(dr["dt_envio"]);
                    vo.Observacao = Convert.ToString(dr["ds_observacao"]);
                    vo.Assistencia = ConvertAssistenciaToList(Convert.ToString(dr["ds_assistencia"]));
                    vo.CodUsuarioEnvio = Convert.ToInt32(dr["cd_usuario_envio"]);
                    if (dr["cd_usuario_aprovacao"] != DBNull.Value)
                    {
                        vo.CodUsuarioAprovacao = Convert.ToInt32(dr["cd_usuario_aprovacao"]);
                        vo.DataAprovacao = Convert.ToDateTime(dr["dt_aprovacao"]);
                        vo.ObservacaoAprovacao = Convert.ToString(dr["ds_observacao_aprovacao"]);
                        vo.ArquivoAprovacao = Convert.ToString(dr["ds_arquivo"]);
                    }                
                }
			}
			vo.CodUsuarioAlteracao = dr.IsNull("cd_usuario_alteracao") ? 0 : Convert.ToInt32(dr["cd_usuario_alteracao"]);

			vo.ProtocoloAns = dr.Field<string>("nr_protocolo_ans");
            vo.Situacao = (SituacaoReciprocidade)Convert.ToInt32(dr["cd_situacao"]);
			return vo;
		}

		internal static ReciprocidadeVO GetById(int id, EvidaDatabase db) {
			string sql = "select * " +
				" FROM EV_RECIPROCIDADE A " +
				" WHERE a.cd_solicitacao = :id ";

			List<Parametro> lstP = new List<Parametro>();
			lstP.Add(new Parametro(":id", DbType.Int32, id));

			List<ReciprocidadeVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstP);

			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}

		internal static DataTable BuscarBeneficiarios(int id, EvidaDatabase db) {

            StringBuilder sql = new StringBuilder();

            sql.Append(" select plano.BI3_TIPO, plano.BI3_DESCRI, par.BRP_DESCRI, to_date(bplano.BA1_DATCAR, 'yyyyMMdd') as BA1_DATCAR, b.*, ");
            sql.Append(" to_date(b.BA1_DATNAS, 'yyyyMMdd') as BA1_DATNAS, trim(b.BA1_CODINT) || '|' || trim(b.BA1_CODEMP) || '|' || trim(b.BA1_MATRIC) || '|' || trim(b.BA1_TIPREG) as cd_beneficiario, ");
            sql.Append(" vida.BTS_NRCRNA ");
            sql.Append(" from EV_RECIPROCIDADE_BENEF rb ");
            sql.Append(" inner join VW_PR_FAMILIA_PRODUTO bplano ");
            sql.Append(" on trim(rb.BA1_DATCAR) = trim(bplano.BA1_DATCAR) ");
            sql.Append(" and trim(rb.BI3_TIPO) = trim(bplano.BI3_TIPO) ");
            sql.Append(" and trim(rb.BA1_CODINT) = trim(bplano.BA1_CODINT) ");
            sql.Append(" and trim(rb.BA1_CODEMP) = trim(bplano.BA1_CODEMP) ");
            sql.Append(" and trim(rb.BA1_MATRIC) = trim(bplano.BA1_MATRIC) ");
            sql.Append(" and trim(rb.BA1_TIPREG) = trim(bplano.BA1_TIPREG) ");
            sql.Append(" inner join VW_PR_PRODUTO_SAUDE plano ");
            sql.Append(" on trim(bplano.BA3_CODPLA) = trim(plano.BI3_CODIGO) ");
            sql.Append(" and trim(bplano.BI3_TIPO) = trim(plano.BI3_TIPO) ");
            sql.Append(" inner join VW_PR_USUARIO b ");
            sql.Append(" on trim(b.BA1_CODINT) = trim(rb.BA1_CODINT) ");
            sql.Append(" and trim(b.BA1_CODEMP) = trim(rb.BA1_CODEMP) ");
            sql.Append(" and trim(b.BA1_MATRIC) = trim(rb.BA1_MATRIC) ");
            sql.Append(" and trim(b.BA1_TIPREG) = trim(rb.BA1_TIPREG) ");
            sql.Append(" inner join VW_PR_VIDA vida ");
            sql.Append(" on trim(b.BA1_MATVID) = trim(vida.BTS_MATVID) ");
            sql.Append(" left outer join VW_PR_GRAU_PARENTESCO par ");
            sql.Append(" on b.BA1_GRAUPA = par.BRP_CODIGO ");
            sql.Append(" where rb.cd_solicitacao = :id ");

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":id", Tipo = DbType.Int32, Value = id });

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql.ToString(), lstParams);

			return dt;
		}

		#endregion

		internal static DataTable Pesquisar(string matemp, int? cdProtocolo, string protocoloAns, StatusReciprocidade? status, EvidaDatabase db) {

            string sql = "select a.cd_solicitacao, a.dt_criacao, a.cd_status, a.dt_envio, a.BA3_CODINT, a.BA3_CODEMP, a.BA3_MATRIC, f.BA3_MATEMP, a.ds_motivo_cancelamento, " +
                "	a.cd_usuario_envio, ue.nm_usuario nm_usuario_envio, a.dt_aprovacao, a.cd_usuario_aprovacao, ua.nm_usuario nm_usuario_aprovacao, " +
                "	a.nr_protocolo_ans, a.cd_situacao " +
				" from ev_reciprocidade a, ev_usuario ue, ev_usuario ua, VW_PR_FAMILIA f " +
			" WHERE a.cd_usuario_envio = ue.id_usuario(+) and a.cd_usuario_aprovacao = ua.id_usuario(+) " +
            " and trim(a.BA3_CODINT) = trim(f.BA3_CODINT) and trim(a.BA3_CODEMP) = trim(f.BA3_CODEMP) and trim(a.BA3_MATRIC) = trim(f.BA3_MATRIC) ";

			List<Parametro> lstP = new List<Parametro>();
			if (cdProtocolo != null) {
				sql += " AND a.cd_solicitacao = :id";
				lstP.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			}
            if (!string.IsNullOrEmpty(matemp))
            {
                sql += " AND trim(f.BA3_MATEMP) = trim(:matemp) ";
                lstP.Add(new Parametro(":matemp", DbType.String, matemp));
            }
			if (status != null) {
				sql += " AND a.cd_status = :status";
				lstP.Add(new Parametro(":status", DbType.Int32, (int)status));
			}
			if (!string.IsNullOrEmpty(protocoloAns)) {
				sql += " AND A.NR_PROTOCOLO_ANS = :protocoloAns";
				lstP.Add(new Parametro(":protocoloAns", DbType.String, protocoloAns));
			}

			sql += " ORDER BY cd_status, cd_solicitacao ";

			DataTable dt = BaseDAO.ExecuteDataSet(db, sql, lstP);

			return dt;
		}

		internal static void Aprovar(int cdProtocolo, int idUsuario, string obs, string fileName, EvidaDatabase evdb) {
			string sql = "UPDATE EV_RECIPROCIDADE SET CD_STATUS = :status, DS_MOTIVO_CANCELAMENTO = NULL, " +
				"	DS_OBSERVACAO_APROVACAO = :obs, DS_ARQUIVO = :arquivo, " +
				"	CD_USUARIO_APROVACAO = :usuario, DT_APROVACAO = LOCALTIMESTAMP, " +
				"	CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusReciprocidade.APROVADO));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			lstParams.Add(new Parametro(":obs", DbType.String, obs));
			lstParams.Add(new Parametro(":arquivo", DbType.String, fileName));

			BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);
		}

		internal static void Cancelar(int cdProtocolo, string motivo, int idUsuario, EvidaDatabase evdb) {
			string sql = "UPDATE EV_RECIPROCIDADE SET CD_STATUS = :status, DS_MOTIVO_CANCELAMENTO = :motivo, " +
				" CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP where CD_SOLICITACAO = :id";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)StatusReciprocidade.NEGADO));
			lstParams.Add(new Parametro(":motivo", DbType.String, motivo));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);
		}

		internal static void Enviar(ReciprocidadeVO vo, int idUsuario, EvidaDatabase db) {
			string sql = "UPDATE EV_RECIPROCIDADE SET " +
				"	BA0_CODINT = :empresa_reciprocidade, " +
				"	DS_OBSERVACAO = :observacao, " +
				"	DS_ASSISTENCIA = :assistencia, " +
				"	CD_USUARIO_ENVIO = :usuario, " +
				"	DT_ENVIO = LOCALTIMESTAMP, " +
				"	CD_STATUS = :status, " +
				"	CD_USUARIO_ALTERACAO = :usuario, DT_ALTERACAO = LOCALTIMESTAMP " +
				" WHERE CD_SOLICITACAO = :id";

			string assistencia = ConvertAssistenciaFromList(vo.Assistencia);
			vo.Status = StatusReciprocidade.ENVIADO;
			
			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro(":id", DbType.Int32, vo.CodSolicitacao));
			lstParams.Add(new Parametro(":empresa_reciprocidade", DbType.String, vo.CodintReciprocidade));
			lstParams.Add(new Parametro(":observacao", DbType.String, vo.Observacao));
			lstParams.Add(new Parametro(":assistencia", DbType.String, assistencia));
			lstParams.Add(new Parametro(":status", DbType.Int32, (int)vo.Status));
			lstParams.Add(new Parametro(":usuario", DbType.Int32, idUsuario));
			BaseDAO.ExecuteNonQuery(sql, lstParams, db);
		}

		private static string ConvertAssistenciaFromList(List<int> list) {
			List<string> lst = list.ConvertAll(new Converter<int, string>(x => x.ToString()));
			if (lst.Count > 0)
				return lst.Aggregate("", (a, b) => a + "|" + b, x => x + "|");
			return null;
		}

		private static List<int> ConvertAssistenciaToList(string str) {
			string[] split = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			return split.ToList().ConvertAll(x => Int32.Parse(x));
		}

        internal static DataTable BuscarEmAndamento(EvidaDatabase db)
        {
            string sql = "select * " +
                " FROM EV_RECIPROCIDADE a " +
                " WHERE a.BA3_MATRIC is not null " +
                " AND a.cd_status IN (";

            for (int i = 0; i < STATUS_EM_ANDAMENTO.Length; i++)
            {
                if (i != 0) sql += ",";
                sql += ((int)STATUS_EM_ANDAMENTO[i]).ToString();
            }
            sql += ") ";

            sql += " ORDER BY a.dt_criacao";
            DataTable dt = BaseDAO.ExecuteDataSet(db, sql);
            return dt;
        }

        internal static void AtualizarSituacao(int cdProtocolo, SituacaoReciprocidade situacao, EvidaDatabase evdb)
        {
            string sql = "UPDATE EV_RECIPROCIDADE SET CD_SITUACAO = :situacao " +
                "	where CD_SOLICITACAO = :id";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro(":id", DbType.Int32, cdProtocolo));
            lstParams.Add(new Parametro(":situacao", DbType.Int32, (int)situacao));

            BaseDAO.ExecuteNonQuery(sql, lstParams, evdb);
        }

	}
}
