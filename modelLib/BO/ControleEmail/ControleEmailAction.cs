using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO.ControleEmail {
	abstract class ControleEmailAction {
		protected EvidaDatabase Database;

		protected ControleEmailAction(EvidaDatabase db) {
			this.Database = db;
		}

		public abstract void MarcarErro(ControleEmailVO vo, Exception ex);

		public abstract void Finalizar(ControleEmailVO vo);

		protected abstract ControleEmailVO Transform(Dictionary<string, object> parameters);

		public void Gerar(Dictionary<string, object> parameters) {
			ControleEmailVO ctrEmailVO = Transform(parameters);
			ControleEmailDAO.Criar(ctrEmailVO, Database);
		}

		public static ControleEmailAction GetAction(TipoControleEmail tipo, EvidaDatabase db) {
			if (tipo == TipoControleEmail.DECLARACAO_DEBITO_ANUAL) {
				return new DeclaracaoAnualDebito(db);
			}
			return null;
		}

		internal static void Criar(Dictionary<string, object> paramEmail, TipoControleEmail tipo, EvidaDatabase db) {
			ControleEmailAction action = GetAction(tipo, db);
			action.Gerar(paramEmail);
		}
	}
}
