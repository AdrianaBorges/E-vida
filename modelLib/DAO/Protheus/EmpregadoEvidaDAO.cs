using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.DAO.Protheus {
    internal class EmpregadoEvidaDAO
    {
        internal static EmpregadoEvidaVO GetByMatricula(long matricula, EvidaDatabase db)
        {
            string sql = "SELECT * " +
                " FROM EVIDA.VW_PR_FUNCIONARIO " +
                " WHERE trim(ZF1_MAT) = trim(:matricula) ";

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.String, Value = matricula.ToString("000000") });

            List<EmpregadoEvidaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        private static EmpregadoEvidaVO FromDataRow(DataRow dr)
        {
            EmpregadoEvidaVO vo = new EmpregadoEvidaVO();
            vo.Matricula = Int64.Parse(dr.Field<string>("ZF1_MAT"));
            vo.Nome = dr.Field<string>("ZF1_NOME").Trim();
            vo.Cpf = dr.Field<string>("ZF1_CPF").Trim();
            vo.Email = dr.Field<string>("ZF1_EMAIL");
            vo.Rg = dr.Field<string>("ZF1_RG").Trim();
            vo.RgOrg = dr.Field<string>("ZF1_RGORG").Trim();
            vo.RgUf = dr.Field<string>("ZF1_RGUF").Trim();
            vo.CodFuncao = dr.Field<string>("ZF1_CODFUN").Trim();
            vo.Funcao = dr.Field<string>("ZF1_DESCFU").Trim();
            vo.CodCentroCusto = dr.Field<string>("ZF1_CC").Trim();
            vo.CentroCusto = dr.Field<string>("ZF1_DESCCC").Trim();
            vo.CodDepartamento = dr.Field<string>("ZF1_DEPTO").Trim();
            vo.Departamento = dr.Field<string>("ZF1_DDEPTO").Trim();
            vo.RecNo = (int)dr.Field<decimal>("R_E_C_N_O_");

            string strNasc = dr.Field<string>("ZF1_NASC").Trim();
            DateTime dt;
            if (DateTime.TryParseExact(strNasc, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                vo.DataNascimento = dt;
            }
            else
            {
                vo.DataNascimento = DateTime.Now.Date;
            }

            return vo;
        }

        internal static EmpregadoEvidaVO GetCoordenador(long matricula, string funcao, EvidaDatabase db)
        {
            string sql = "";

            // Se for um diretor
            if (funcao.Equals("00025"))
            {
                sql = " SELECT * " +
                      " FROM EVIDA.VW_PR_FUNCIONARIO " +
                      " WHERE trim(ZF1_MAT) = trim(:matricula) ";

            }
            // Se for um assessor ou coordenador
            else if (funcao.Equals("00014") || funcao.Equals("00015") || funcao.Equals("00018"))
            {
                sql = " SELECT * " +
                      " FROM EVIDA.VW_PR_FUNCIONARIO " +
                      " WHERE trim(ZF1_MAT) IN (SELECT trim(DEP.QB_MATRESP) FROM EVIDA.VW_PR_SRA010 S, EVIDA.VW_PR_SQB010 DEP " +
                                        " WHERE trim(DEP.QB_DEPTO) = trim(S.RA_DEPTO) AND trim(S.RA_MAT) = trim(:matricula)) ";

            }
            // Se for qualquer outro usuário
            else
            {
                sql = " SELECT * " +
                      " FROM EVIDA.VW_PR_FUNCIONARIO " +
                      " WHERE trim(ZF1_MAT) IN (SELECT trim(DEP.QB_MATRESP) FROM EVIDA.VW_PR_SRA010 S, EVIDA.VW_PR_SQB010 DEP " +
                                        " WHERE trim(DEP.QB_DEPTO) = trim(S.RA_DEPTO) AND trim(S.RA_MAT) = trim(:matricula)) ";
            }

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.String, Value = matricula.ToString("000000") });

            List<EmpregadoEvidaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static EmpregadoEvidaVO GetDiretor(long matricula, string funcao, EvidaDatabase db)
        {
            string sql = "";

            // Se for um diretor
            if (funcao.Equals("00025"))
            {
                sql = " SELECT * " +
                      " FROM EVIDA.VW_PR_FUNCIONARIO " +
                      " WHERE trim(ZF1_MAT) IN (SELECT trim(DEP.QB_ZMAT) FROM EVIDA.VW_PR_ZF1010 F, EVIDA.VW_PR_SQB010 DEP " +
                                        " WHERE trim(DEP.QB_DEPTO) = trim(F.ZF1_DEPTO) AND trim(F.ZF1_MAT) = trim(:matricula)) ";

            }
            // Se for um assessor ou coordenador
            else if (funcao.Equals("00014") || funcao.Equals("00015") || funcao.Equals("00018"))
            {
                sql = " SELECT * " +
                      " FROM EVIDA.VW_PR_FUNCIONARIO " +
                      " WHERE trim(ZF1_MAT) IN (SELECT trim(DEP.QB_ZMAT) FROM EVIDA.VW_PR_SRA010 S, EVIDA.VW_PR_SQB010 DEP " +
                                        " WHERE trim(DEP.QB_DEPTO) = trim(S.RA_DEPTO) AND trim(S.RA_MAT) = trim(:matricula)) ";

            }
            // Se for qualquer outro usuário
            else
            {
                sql = " SELECT * " +
                      " FROM EVIDA.VW_PR_FUNCIONARIO " +
                      " WHERE trim(ZF1_MAT) IN (SELECT trim(DEP.QB_ZMAT) FROM EVIDA.VW_PR_SRA010 S, EVIDA.VW_PR_SQB010 DEP " +
                                        " WHERE trim(DEP.QB_DEPTO) = trim(S.RA_DEPTO) AND trim(S.RA_MAT) = trim(:matricula)) ";
            }

            List<Parametro> lstParams = new List<Parametro>();
            lstParams.Add(new Parametro() { Name = ":matricula", Tipo = DbType.String, Value = matricula.ToString("000000") });

            List<EmpregadoEvidaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow, lstParams);

            if (lst != null && lst.Count > 0)
            {
                return lst[0];
            }
            return null;
        }

        internal static List<EmpregadoEvidaVO> ListarDiretores(EvidaDatabase db)
        {
            string sql = "SELECT * " +
                " FROM EVIDA.VW_PR_FUNCIONARIO " +
                " WHERE trim(ZF1_MAT) IN (SELECT trim(DEP.QB_ZMAT) FROM EVIDA.VW_PR_SQB010 DEP) ";

            List<EmpregadoEvidaVO> lst = BaseDAO.ExecuteDataSet(db, sql, FromDataRow);
            return lst;
        }

    }
}
