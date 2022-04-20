using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;

namespace eVidaGeneralLib.DAO
{
    internal class HistoricoDadoPessoalDAO {
        
		internal static void SalvarHistorico(PUsuarioVO vo, EvidaDatabase evdb) {
            string sql = "INSERT INTO EV_DADO_PESSOAL_HISTORICO (BA1_CODINT, BA1_CODEMP, BA1_MATRIC, BA1_TIPREG, DT_ALTERACAO, DS_EMAIL, " +
            "	NU_CEP, DS_RUA, NR_ENDERECO, DS_COMPLEMENTO, DS_BAIRRO, SG_UF, DS_MUNICIPIO, CD_MUNICIPIO, " +
            "	NR_DDD_RESIDENCIAL, NR_TELEFONE_RESIDENCIAL, NR_DDD_COMERCIAL, NR_TELEFONE_COMERCIAL, NR_DDD_CELULAR, NR_TELEFONE_CELULAR, " +
            "	CD_BANCO, CD_AGENCIA, NR_CONTA, NR_DV_CONTA, TP_CONTA) " +
            "	VALUES " +
            "	(:codint, :codemp, :matric, :tipreg, LOCALTIMESTAMP, :email, :cep, :endereco, :numero, :complemento, :bairro, :uf, :cidade, :idCidade, " +
            "	:dddResidencial, :telResidencial, :dddComercial, :telComercial, :dddCelular, :telCelular, :cdBanco, :cdAgencia, :nrConta, :dvConta, :tpConta)";

			Database db = evdb.Database;
			DbCommand dbCommand = db.GetSqlStringCommand(sql);
			db.AddInParameter(dbCommand, ":email", DbType.String, vo.Email.Trim().ToUpper());
			db.AddInParameter(dbCommand, ":endereco", DbType.String, vo.Endere.Trim().ToUpper());
			db.AddInParameter(dbCommand, ":numero", DbType.String, vo.Nrend.Trim().ToUpper());
			db.AddInParameter(dbCommand, ":complemento", DbType.String, vo.Comend.Trim().ToUpper());
			db.AddInParameter(dbCommand, ":bairro", DbType.String, vo.Bairro.Trim().ToUpper());
			db.AddInParameter(dbCommand, ":cep", DbType.String, vo.Cepusr);
			db.AddInParameter(dbCommand, ":uf", DbType.String, vo.Estado);
			db.AddInParameter(dbCommand, ":cidade", DbType.String, vo.Munici);
            db.AddInParameter(dbCommand, ":idCidade", DbType.String, vo.Codmun);

            db.AddInParameter(dbCommand, ":dddResidencial", DbType.String, vo.Ddd.Trim());
            db.AddInParameter(dbCommand, ":telResidencial", DbType.String, vo.Telres.Trim());
            db.AddInParameter(dbCommand, ":dddComercial", DbType.String, vo.Ddd.Trim());
            db.AddInParameter(dbCommand, ":telComercial", DbType.String, vo.Telcom.Trim());
            db.AddInParameter(dbCommand, ":dddCelular", DbType.String, vo.Ddd.Trim());
            db.AddInParameter(dbCommand, ":telCelular", DbType.String, vo.Telefo.Trim());

            // Dados Bancários
            PFamiliaVO familia = PFamiliaBO.Instance.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric);

            if (int.TryParse(familia.Bcocli.Trim(), out _))
                db.AddInParameter(dbCommand, ":cdBanco", DbType.Int32, int.Parse(familia.Bcocli.Trim()));
            else
                db.AddInParameter(dbCommand, ":cdBanco", DbType.Int32, "0");

            db.AddInParameter(dbCommand, ":cdAgencia", DbType.String, familia.Agecli.Trim());
            db.AddInParameter(dbCommand, ":nrConta", DbType.String, familia.Ctacli.Trim());
            db.AddInParameter(dbCommand, ":dvConta", DbType.String, "");
            db.AddInParameter(dbCommand, ":tpConta", DbType.String, "COR");

            db.AddInParameter(dbCommand, ":codint", DbType.String, vo.Codint.Trim());
            db.AddInParameter(dbCommand, ":codemp", DbType.String, vo.Codemp.Trim());
            db.AddInParameter(dbCommand, ":matric", DbType.String, vo.Matric.Trim());
            db.AddInParameter(dbCommand, ":tipreg", DbType.String, vo.Tipreg.Trim());

			BaseDAO.ExecuteNonQuery(dbCommand, evdb);
		}
	}
}
