using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.SCL;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.SCL;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class BeneficiarioBO {
		private static BeneficiarioBO instance = new BeneficiarioBO();

		public static BeneficiarioBO Instance { get { return instance; } }

		public HcBeneficiarioVO LogarBeneficiario(string login, string senha) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				SclUsuarioVO uVO = SclUsuarioDAO.Logar(login, senha, db);
				if (uVO == null)
					return null;

				string codAlternativo = SclUsuarioDAO.GetCodAlternativoBeneficiario(uVO.Login, db);

				if (string.IsNullOrEmpty(codAlternativo))
					throw new EvidaException("Número da Carteira do beneficiário não encontrado no DOMINIO");

				HcBeneficiarioVO benefVO = HcVBeneficiarioDAO.GetRowByCartao(codAlternativo, db);

				if (benefVO == null)
					throw new EvidaException("Não há beneficiário não associado ao usuário!");

				return benefVO;
			}
		}

		public DataTable BuscarBeneficiarios(int cdEmpresa, long cdFuncionario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVBeneficiarioDAO.GetBeneficiarios(cdEmpresa, cdFuncionario, db);
			}
		}

		public List<HcBeneficiarioVO> ListarBeneficiarios(int cdEmpresa, long cdFuncionario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVBeneficiarioDAO.ListarBeneficiarios(cdEmpresa, cdFuncionario, db);
			}
		}

		public List<HcDependenteVO> ListarDependentes(HcFuncionarioVO funcionario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcDependenteDAO.ListarDependentes(funcionario, db);
			}
		}

		public HcBeneficiarioVO GetBeneficiario(long cdBeneficiario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVBeneficiarioDAO.GetRowById(cdBeneficiario, db);
			}
		}

		public HcBeneficiarioPlanoVO GetBeneficiarioPlano(int cdBeneficiario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcBeneficiarioPlanoDAO.GetLastBeneficiarioData(cdBeneficiario, null, db);
			}
		}

		public HcBeneficiarioCategoriaVO GetBeneficiarioCategoria(int cdBeneficiario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcBeneficiarioCategoriaDAO.GetLastBeneficiarioData(cdBeneficiario, null, db);
			}
		}

		public HcBeneficiarioVO GetBeneficiarioByCartao(string numCartao, bool useTrata = false) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVBeneficiarioDAO.GetRowByCartao(numCartao, useTrata, db);
			}
		}

		public HcBeneficiarioVO GetTitular(int cdEmpresa, long cdFuncionario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return HcVBeneficiarioDAO.GetTitular(cdEmpresa, cdFuncionario, db);
			}
		}

	}
}
