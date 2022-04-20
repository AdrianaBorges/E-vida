using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;


namespace eVidaGeneralLib.DAO.HC {
	internal class HcFuncionarioDAO {
		
		internal static HcFuncionarioVO GetByMatricula(int cdEmpresa, long matricula, EvidaDatabase db) {
			string sql = "SELECT CD_EMPRESA, CD_FUNCIONARIO, DT_ADMISSAO, NM_FUNCIONARIO, DT_NASCIMENTO, NR_CPF, TP_SEXO, DS_EMAIL, TP_ESTADO_CIVIL, NM_MAE, " +
				"	DS_ENDERECO, NR_ENDERECO, DS_COMPLEMENTO, DS_BAIRRO, NR_CEP, NR_RG, DS_ORG_EXP_RG, CD_UF_ORG_EXP_RG, DT_EMISSAO_NR_RG, "+
				"	F.CD_MUNICIPIO, M.CD_ESTADO, " +
				"	NR_CEP_COB, DS_ENDERECO_COB, CD_MUNICIPIO_COB, DS_BAIRRO_COB, CD_EMPRESA_RESPONSAVEL, " +
				"	CD_LOCAL, CD_LOTACAO,  " +
				"	NR_DDD_RESIDENCIAL, NR_TELEFONE_RESIDENCIAL, NR_DDD_COMERCIAL, NR_TELEFONE_COMERCIAL, NR_DDD_CELULAR, NR_TELEFONE_CELULAR " +
                "	CD_BANCO, CD_AGENCIA, NR_CONTA, NR_DV_CONTA " +
				" FROM ISA_HC.HC_FUNCIONARIO F, ISA_HC.HC_MUNICIPIO M " +
				" WHERE F.CD_FUNCIONARIO = :matricula AND F.CD_EMPRESA = : empresa AND F.CD_MUNICIPIO = M.CD_MUNICIPIO(+) ";

			List<Parametro> lstParams = new List<Parametro>();
			lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.Int64, Value = matricula });
			lstParams.Add(new Parametro() { Name = ":empresa", Tipo = DbType.Int32, Value = cdEmpresa });

			List<HcFuncionarioVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

			if (lst != null && lst.Count > 0) {
				return lst[0];
			}
			return null;
		}

		private static HcFuncionarioVO FromDataRow(DataRow dr) {
			HcFuncionarioVO vo = new HcFuncionarioVO();
			vo.CdEmpresa = (int)dr.Field<decimal>("CD_EMPRESA");
			vo.CdFuncionario = (long)dr.Field<decimal>("CD_FUNCIONARIO");
			vo.Nome = dr.Field<string>("NM_FUNCIONARIO");
			vo.Email = dr.Field<string>("ds_email");
			if (vo.Email == null) vo.Email = string.Empty;
			vo.Admissao = dr.Field<DateTime>("DT_ADMISSAO");
			vo.Nascimento = dr["DT_NASCIMENTO"] != DBNull.Value ? Convert.ToDateTime(dr["DT_NASCIMENTO"]) : DateTime.MinValue;
			vo.Cpf = dr["NR_CPF"] != DBNull.Value ? Convert.ToInt64(dr["NR_CPF"]) : 0;
			vo.Rg = dr.Field<string>("NR_RG");
			vo.OrgaoExpedidorRg = dr.Field<string>("DS_ORG_EXP_RG");
			vo.UfOrgaoExpedidorRg = dr.Field<string>("CD_UF_ORG_EXP_RG");
			vo.DataEmissaoRg = dr["DT_EMISSAO_NR_RG"] != DBNull.Value ? Convert.ToDateTime(dr["DT_EMISSAO_NR_RG"]) : new DateTime?();
			vo.Sexo = Convert.ToString(dr["TP_SEXO"]);
			vo.NomeMae =dr.Field<string>("NM_MAE");
			vo.TipoEstadoCivil = dr.Field<string>("TP_ESTADO_CIVIL");
			vo.CdLocal = (int?)dr.Field<decimal?>("cd_local");
			vo.CdLotacao = dr.Field<string>("cd_lotacao");
			vo.CdEmpresaResponsavel = BaseDAO.GetNullableInt(dr, "CD_EMPRESA_RESPONSAVEL");

			if (dr["NR_CEP"] != DBNull.Value) {
				vo.Endereco = new EnderecoVO();
				vo.Endereco.Cep = Convert.ToInt32(dr["NR_CEP"]);
				vo.Endereco.Bairro = dr.Field<string>("DS_BAIRRO");
				vo.Endereco.IdLocalidade = dr["CD_MUNICIPIO"] != DBNull.Value ? Convert.ToInt32(dr["CD_MUNICIPIO"]) : 0;
				vo.Endereco.Rua = dr.Field<string>("DS_ENDERECO");
				vo.Endereco.Uf = dr.Field<string>("CD_ESTADO");
				vo.Endereco.NumeroEndereco = dr.Field<string>("NR_ENDERECO");
				vo.Endereco.Complemento = dr.Field<string>("DS_COMPLEMENTO");
			}

			if (dr["NR_CEP_COB"] != DBNull.Value) {
				vo.EnderecoCob = new EnderecoVO();
				vo.EnderecoCob.Cep = BaseDAO.GetNullableInt(dr, "NR_CEP_COB").Value;
				vo.EnderecoCob.Bairro = dr.Field<string>("DS_BAIRRO_COB");
				vo.EnderecoCob.IdLocalidade = dr["CD_MUNICIPIO_COB"] != DBNull.Value ? Convert.ToInt32(dr["CD_MUNICIPIO_COB"]) : 0;
				vo.EnderecoCob.Rua = dr.Field<string>("DS_ENDERECO_COB");
			}

			vo.DddTelResidencial = Convert.ToString(dr["NR_DDD_RESIDENCIAL"]);
			vo.TelResidencial = dr.Field<string>("NR_TELEFONE_RESIDENCIAL");
			vo.DddTelComercial = Convert.ToString(dr["NR_DDD_COMERCIAL"]);
			vo.TelComercial = dr.Field<string>("NR_TELEFONE_COMERCIAL");
			vo.DddTelCelular = Convert.ToString(dr["NR_DDD_CELULAR"]);
			vo.TelCelular = dr.Field<string>("NR_TELEFONE_CELULAR");

            vo.CdBanco = dr["CD_BANCO"] != DBNull.Value ? Convert.ToInt32(dr["CD_BANCO"]) : 0;
            vo.CdAgencia = dr.Field<string>("CD_AGENCIA");
            vo.ContaBancaria = dr.Field<string>("NR_CONTA");
            vo.DvContaBancaria = dr.Field<string>("NR_DV_CONTA");

			return vo;
		}

		internal static void AlterarEmail(int cdEmpresa, long cdFuncionario, string email, EvidaDatabase evdb) {
			string sql = "UPDATE ISA_HC.HC_FUNCIONARIO SET DS_EMAIL = :email WHERE CD_EMPRESA = :cdEmpresa AND CD_FUNCIONARIO = :cdFuncionario";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":email", DbType.String, email.ToUpper());
			db.AddInParameter(dbCommand, ":cdEmpresa", DbType.Int32, cdEmpresa);
			db.AddInParameter(dbCommand, ":cdFuncionario", DbType.Int64, cdFuncionario);
			
			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

        internal static void SalvarDadosPessoais(HcFuncionarioVO vo, EvidaDatabase evdb)
        {
            string sql = "UPDATE ISA_HC.HC_FUNCIONARIO SET DS_ENDERECO = :endereco, NR_ENDERECO = :numero, " +
                "	DS_COMPLEMENTO = :complemento, DS_BAIRRO = :bairro, NR_CEP =:cep, CD_MUNICIPIO = :cidade, " +
                "	DS_EMAIL = :email, " +
                "	NR_DDD_RESIDENCIAL = :dddResidencial, NR_TELEFONE_RESIDENCIAL = :telResidencial, NR_DDD_COMERCIAL = :dddComercial, " +
                "	NR_TELEFONE_COMERCIAL = :telComercial, NR_DDD_CELULAR = :dddCelular, NR_TELEFONE_CELULAR = :telCelular, " +
                "	CD_BANCO = :cdBanco, CD_AGENCIA = :cdAgencia, NR_CONTA = :nrConta, NR_DV_CONTA = :dvConta, TP_CONTA = :tpConta " +
                "	WHERE CD_EMPRESA = :cdEmpresa AND CD_FUNCIONARIO = :cdFuncionario";

            Database db = evdb.Database;
            DbCommand dbCommand = db.GetSqlStringCommand(sql);
            db.AddInParameter(dbCommand, ":endereco", DbType.String, vo.Endereco.Rua.Trim().ToUpper());
            db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Endereco.NumeroEndereco.Trim().ToUpper());
            db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Endereco.Complemento.Trim().ToUpper());
            db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Endereco.Bairro.Trim().ToUpper());
            db.AddInParameter(dbCommand, ":cep", DbType.Int32, vo.Endereco.Cep);
            db.AddInParameter(dbCommand, ":cidade", DbType.Int32, vo.Endereco.IdLocalidade);
            db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email);

            db.AddInParameter(dbCommand, ":dddResidencial", DbType.String, vo.DddTelResidencial);
            db.AddInParameter(dbCommand, ":telResidencial", DbType.String, vo.TelResidencial);
            db.AddInParameter(dbCommand, ":dddComercial", DbType.String, vo.DddTelComercial);
            db.AddInParameter(dbCommand, ":telComercial", DbType.String, vo.TelComercial);
            db.AddInParameter(dbCommand, ":dddCelular", DbType.String, vo.DddTelCelular);
            db.AddInParameter(dbCommand, ":telCelular", DbType.String, vo.TelCelular);

            // Dados Bancários
            db.AddInParameter(dbCommand, ":cdBanco", DbType.Int32, vo.CdBanco);
            db.AddInParameter(dbCommand, ":cdAgencia", DbType.String, vo.CdAgencia);
            db.AddInParameter(dbCommand, ":nrConta", DbType.String, vo.ContaBancaria);
            db.AddInParameter(dbCommand, ":dvConta", DbType.String, vo.DvContaBancaria);
            db.AddInParameter(dbCommand, ":tpConta", DbType.String, "COR");

            db.AddInParameter(dbCommand, ":cdEmpresa", DbType.Int32, vo.CdEmpresa);
            db.AddInParameter(dbCommand, ":cdFuncionario", DbType.Int64, vo.CdFuncionario);

            BaseDAO.ExecuteNonQuery(dbCommand, evdb);
        }	

		internal static void CriarFuncionario(HcFuncionarioVO funcionario, EvidaDatabase evdb) {
			funcionario.OrgaoExpedidorRg = CheckOrgaoExpedidor(funcionario.OrgaoExpedidorRg, evdb);

			string sql = "INSERT INTO ISA_HC.HC_FUNCIONARIO (CD_EMPRESA, CD_FUNCIONARIO, DT_ADMISSAO, CD_LOTACAO, CD_LOCAL, NM_FUNCIONARIO, TP_FUNCIONARIO, TP_SEXO, FL_DEFICIENTE_FISICO, " +
				" NR_CPF, NR_RG, DS_ORG_EXP_RG, CD_UF_ORG_EXP_RG, DT_EMISSAO_NR_RG, CD_BANCO, CD_AGENCIA, NR_CONTA, NR_DV_CONTA, NM_PAI, NM_MAE, " +
				" DT_NASCIMENTO, TP_ESTADO_CIVIL, " +
				" NR_DDD_RESIDENCIAL, NR_TELEFONE_RESIDENCIAL, NR_DDD_COMERCIAL, NR_TELEFONE_COMERCIAL, NR_DDD_CELULAR, NR_TELEFONE_CELULAR, " +
				" DS_EMAIL, DS_ENDERECO, NR_ENDERECO, DS_COMPLEMENTO, DS_BAIRRO, NR_CEP, CD_MUNICIPIO, " +
				" NR_CEP_COB, CD_MUNICIPIO_COB, DS_ENDERECO_COB, DS_BAIRRO_COB, CD_EMPRESA_RESPONSAVEL, " +
				" USER_CREATE, DATE_CREATE) " +
				" VALUES " +
				" (:empresa, :matricula, :dtAdmissao, :lotacao, :local, :nome, :tpFuncionario, :sexo, :flDeficiente, " +
				" :cpf, :rg, :orgaoRg, :ufRg, :dtEmissaoRg, :banco, :agencia, :conta, :dvConta, :pai, :mae, " +
				" :nascimento, :estadoCivil, " +
				" :dddResidencial, :telResidencial, :dddComercial, :telComercial, :dddCelular, :telCelular, " +
				" :email, :endereco, :numEndereco, :complEndereco, :bairro, : cep, :cdMunicipio, " + 
				" :cepCob, :cdMunicipioCob, :enderecoCob, :bairroCob, :cdEmpResp, " +
				" :userInt, :dateInt) ";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":empresa", DbType.Int32, funcionario.CdEmpresa);
			db.AddInParameter(dbCommand, ":matricula", DbType.Int64, funcionario.CdFuncionario);
			db.AddInParameter(dbCommand, ":dtAdmissao", DbType.Date, funcionario.Admissao);

			db.AddInParameter(dbCommand, ":lotacao", DbType.String, funcionario.CdLotacao);
			db.AddInParameter(dbCommand, ":local", DbType.Int32, funcionario.CdLocal);

			db.AddInParameter(dbCommand, ":nome", DbType.String, funcionario.Nome);
			db.AddInParameter(dbCommand, ":tpFuncionario", DbType.String, "N");
			db.AddInParameter(dbCommand, ":sexo", DbType.String, funcionario.Sexo.ToString());
			db.AddInParameter(dbCommand, ":flDeficiente", DbType.String, funcionario.IsDeficienteFisico ? "S" : "N");

			db.AddInParameter(dbCommand, ":cpf", DbType.Int64, funcionario.Cpf);
			db.AddInParameter(dbCommand, ":rg", DbType.String, funcionario.Rg);
			db.AddInParameter(dbCommand, ":orgaoRg", DbType.String, funcionario.OrgaoExpedidorRg);
			db.AddInParameter(dbCommand, ":ufRg", DbType.String, funcionario.UfOrgaoExpedidorRg);
			db.AddInParameter(dbCommand, ":dtEmissaoRg", DbType.Date, funcionario.DataEmissaoRg);

			db.AddInParameter(dbCommand, ":banco", DbType.Int32, funcionario.CdBanco);
			db.AddInParameter(dbCommand, ":agencia", DbType.String, funcionario.CdAgencia);
			db.AddInParameter(dbCommand, ":conta", DbType.String, funcionario.ContaBancaria);
			db.AddInParameter(dbCommand, ":dvConta", DbType.String, funcionario.DvContaBancaria);

			db.AddInParameter(dbCommand, ":pai", DbType.String, funcionario.NomePai);
			db.AddInParameter(dbCommand, ":mae", DbType.String, funcionario.NomeMae);

			db.AddInParameter(dbCommand, ":nascimento", DbType.Date, funcionario.Nascimento);
			db.AddInParameter(dbCommand, ":estadoCivil", DbType.String, funcionario.TipoEstadoCivil);

			db.AddInParameter(dbCommand, ":dddResidencial", DbType.String, funcionario.DddTelResidencial);
			db.AddInParameter(dbCommand, ":telResidencial", DbType.String, funcionario.TelResidencial);
			db.AddInParameter(dbCommand, ":dddComercial", DbType.String, funcionario.DddTelComercial);
			db.AddInParameter(dbCommand, ":telComercial", DbType.String, funcionario.TelComercial);
			db.AddInParameter(dbCommand, ":dddCelular", DbType.String, funcionario.DddTelCelular);
			db.AddInParameter(dbCommand, ":telCelular", DbType.String, funcionario.TelCelular);

			db.AddInParameter(dbCommand, ":email", DbType.String, funcionario.Email);

			db.AddInParameter(dbCommand, ":endereco", DbType.String, funcionario.Endereco.Rua);
			db.AddInParameter(dbCommand, ":numEndereco", DbType.String, funcionario.Endereco.NumeroEndereco);
			db.AddInParameter(dbCommand, ":complEndereco", DbType.String, funcionario.Endereco.Complemento);
			db.AddInParameter(dbCommand, ":bairro", DbType.String, funcionario.Endereco.Bairro);
			db.AddInParameter(dbCommand, ":cep", DbType.Int32, funcionario.Endereco.Cep);
			db.AddInParameter(dbCommand, ":cdMunicipio", DbType.Int32, funcionario.Endereco.IdLocalidade);

			db.AddInParameter(dbCommand, ":cepCob", DbType.Int32, funcionario.EnderecoCob.Cep);
			db.AddInParameter(dbCommand, ":cdMunicipioCob", DbType.Int32, funcionario.EnderecoCob.IdLocalidade);
			db.AddInParameter(dbCommand, ":enderecoCob", DbType.String, FormatUtil.EnsureCapacity((funcionario.EnderecoCob.Rua + ", " + funcionario.EnderecoCob.NumeroEndereco + " - " + funcionario.EnderecoCob.Complemento).ToUpper(), 60));
			db.AddInParameter(dbCommand, ":bairroCob", DbType.String, funcionario.EnderecoCob.Bairro.Upper());

			db.AddInParameter(dbCommand, ":cdEmpResp", DbType.Int32, funcionario.CdEmpresaResponsavel);

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO" );
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}

		private static string CheckOrgaoExpedidor(string orgaoExp, EvidaDatabase db) {
			return SCL.SclItemListaDAO.CheckItemLista(Constantes.LISTA_ORGAO_EXPEDIDOR, orgaoExp, db);
		}

		internal static void AlterarFuncionario(HcFuncionarioVO funcionario, EvidaDatabase evdb) {
			funcionario.OrgaoExpedidorRg = CheckOrgaoExpedidor(funcionario.OrgaoExpedidorRg, evdb);

			string sql = "UPDATE ISA_HC.HC_FUNCIONARIO SET TP_SEXO = :sexo, CD_LOTACAO = :lotacao, CD_LOCAL = :local, FL_DEFICIENTE_FISICO = NVL(FL_DEFICIENTE_FISICO, :flDeficiente), " +
				" NR_CPF = :cpf, NR_RG = :rg, DS_ORG_EXP_RG = :orgaoRg, CD_UF_ORG_EXP_RG = :ufRg, DT_EMISSAO_NR_RG = :dtEmissaoRg, " +
				" CD_BANCO = :banco, CD_AGENCIA = :agencia, NR_CONTA = :conta, NR_DV_CONTA = :dvConta, NM_PAI = :pai, NM_MAE = :mae, " +
				" DT_NASCIMENTO = :nascimento, TP_ESTADO_CIVIL = :estadoCivil, " +
				" NR_DDD_RESIDENCIAL = :dddResidencial, NR_TELEFONE_RESIDENCIAL = :telResidencial, NR_DDD_COMERCIAL = :dddComercial, " +
				" NR_TELEFONE_COMERCIAL = :telComercial, NR_DDD_CELULAR = :dddCelular, NR_TELEFONE_CELULAR = :telCelular, " +
				" DS_EMAIL = :email, DS_ENDERECO = :endereco, NR_ENDERECO = :numEndereco, DS_COMPLEMENTO = :complEndereco, DS_BAIRRO = :bairro, " +
				" NR_CEP = :cep, CD_MUNICIPIO = :cdMunicipio, " +
				" NR_CEP_COB = :cepCob, CD_MUNICIPIO_COB = :cdMunicipioCob, DS_ENDERECO_COB = :enderecoCob, DS_BAIRRO_COB = :bairroCob, " +
				" CD_EMPRESA_RESPONSAVEL = :cdEmpResp, " +
				" USER_UPDATE = :userInt, DATE_UPDATE = :dateInt" +
				" WHERE CD_EMPRESA = :empresa AND CD_FUNCIONARIO = :matricula";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":empresa", DbType.Int32, funcionario.CdEmpresa);
			db.AddInParameter(dbCommand, ":matricula", DbType.Int64, funcionario.CdFuncionario);
			db.AddInParameter(dbCommand, ":lotacao", DbType.String, funcionario.CdLotacao);
			db.AddInParameter(dbCommand, ":local", DbType.Int32, funcionario.CdLocal);

			db.AddInParameter(dbCommand, ":sexo", DbType.String, funcionario.Sexo.ToString());
			db.AddInParameter(dbCommand, ":flDeficiente", DbType.String, funcionario.IsDeficienteFisico ? "S" : "N");
			
			db.AddInParameter(dbCommand, ":cpf", DbType.Int64, funcionario.Cpf);
			db.AddInParameter(dbCommand, ":rg", DbType.String, funcionario.Rg);
			db.AddInParameter(dbCommand, ":orgaoRg", DbType.String, funcionario.OrgaoExpedidorRg);
			db.AddInParameter(dbCommand, ":ufRg", DbType.String, funcionario.UfOrgaoExpedidorRg);
			db.AddInParameter(dbCommand, ":dtEmissaoRg", DbType.Date, funcionario.DataEmissaoRg);

			db.AddInParameter(dbCommand, ":banco", DbType.Int32, funcionario.CdBanco);
			db.AddInParameter(dbCommand, ":agencia", DbType.String, funcionario.CdAgencia);
			db.AddInParameter(dbCommand, ":conta", DbType.String, funcionario.ContaBancaria);
			db.AddInParameter(dbCommand, ":dvConta", DbType.String, funcionario.DvContaBancaria);

			db.AddInParameter(dbCommand, ":pai", DbType.String, funcionario.NomePai);
			db.AddInParameter(dbCommand, ":mae", DbType.String, funcionario.NomeMae);

			db.AddInParameter(dbCommand, ":nascimento", DbType.Date, funcionario.Nascimento);
			db.AddInParameter(dbCommand, ":estadoCivil", DbType.String, funcionario.TipoEstadoCivil);

			db.AddInParameter(dbCommand, ":dddResidencial", DbType.String, funcionario.DddTelResidencial);
			db.AddInParameter(dbCommand, ":telResidencial", DbType.String, funcionario.TelResidencial);
			db.AddInParameter(dbCommand, ":dddComercial", DbType.String, funcionario.DddTelComercial);
			db.AddInParameter(dbCommand, ":telComercial", DbType.String, funcionario.TelComercial);
			db.AddInParameter(dbCommand, ":dddCelular", DbType.String, funcionario.DddTelCelular);
			db.AddInParameter(dbCommand, ":telCelular", DbType.String, funcionario.TelCelular);

			db.AddInParameter(dbCommand, ":email", DbType.String, funcionario.Email);

			db.AddInParameter(dbCommand, ":endereco", DbType.String, funcionario.Endereco.Rua.Upper());
			db.AddInParameter(dbCommand, ":numEndereco", DbType.String, funcionario.Endereco.NumeroEndereco);
			db.AddInParameter(dbCommand, ":complEndereco", DbType.String, funcionario.Endereco.Complemento.Upper());
			db.AddInParameter(dbCommand, ":bairro", DbType.String, funcionario.Endereco.Bairro.Upper());
			db.AddInParameter(dbCommand, ":cep", DbType.Int32, funcionario.Endereco.Cep);
			db.AddInParameter(dbCommand, ":cdMunicipio", DbType.Int32, funcionario.Endereco.IdLocalidade);

			db.AddInParameter(dbCommand, ":cepCob", DbType.Int32, funcionario.EnderecoCob.Cep);
			db.AddInParameter(dbCommand, ":cdMunicipioCob", DbType.Int32, funcionario.EnderecoCob.IdLocalidade);
			db.AddInParameter(dbCommand, ":enderecoCob", DbType.String, FormatUtil.EnsureCapacity((funcionario.EnderecoCob.Rua + ", " + funcionario.EnderecoCob.NumeroEndereco + " - " + funcionario.EnderecoCob.Complemento).ToUpper(), 60));
			db.AddInParameter(dbCommand, ":bairroCob", DbType.String, funcionario.EnderecoCob.Bairro.Upper());

			db.AddInParameter(dbCommand, ":cdEmpResp", DbType.Int32, funcionario.CdEmpresaResponsavel);

			db.AddInParameter(dbCommand, ":userInt", DbType.String, "INTRANET - INTEGRAÇÃO");
			db.AddInParameter(dbCommand, ":dateInt", DbType.String, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}

