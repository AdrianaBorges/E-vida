using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class CartaPositivaCraBO {
		EVidaLog log = new EVidaLog(typeof(CartaPositivaCraBO));

		private static CartaPositivaCraBO instance = new CartaPositivaCraBO();

		public static CartaPositivaCraBO Instance { get { return instance; } }

		public DataTable Pesquisar(int? id, string protocoloCra, string matricula, string carteira, CartaPositivaCraStatus? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return CartaPositivaCraDAO.Pesquisar(id, protocoloCra, matricula, carteira, status, db);
			}
		}

		public CartaPositivaCraVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				CartaPositivaCraVO vo = CartaPositivaCraDAO.GetById(id, db);
                vo.Beneficiario = DAO.Protheus.PUsuarioDAO.GetRowById(vo.Beneficiario.Codint, vo.Beneficiario.Codemp, vo.Beneficiario.Matric, vo.Beneficiario.Tipreg, db);
				vo.Credenciado = DAO.Protheus.PRedeAtendimentoDAO.GetById(vo.Credenciado.Codigo, db);
				return vo;
			}
		}

		public void SalvarSolicitacao(CartaPositivaCraVO vo, UsuarioVO usuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				vo.IdUsuarioAlteracao = usuario.Id;

				CartaPositivaCraDAO.Salvar(vo, db);

				connection.Commit();
			}
		}

		public void AprovarSolicitacao(int id, UsuarioVO usuario) {
			string assinatura = UsuarioBO.Instance.GetAssinatura(usuario.Id);
			if (string.IsNullOrEmpty(assinatura)) {
				throw new EvidaException("O usuário " + usuario.Login + " não possui assinatura cadastrada!");
			}
			if (string.IsNullOrEmpty(usuario.Cargo)) {
				throw new EvidaException("O seu usuário não possui cargo cadastrado na intranet! Entre em contato com o suporte!");
			}

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				CartaPositivaCraDAO.Aprovar(id, usuario.Id, db);

				connection.Commit();
			}
		}

		public void CancelarSolicitacao(int id, string motivoCancelamento, UsuarioVO usuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				
				CartaPositivaCraDAO.Cancelar(id, motivoCancelamento, usuario.Id, db);

				connection.Commit();
			}
		}
	}
}
