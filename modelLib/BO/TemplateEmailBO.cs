using eVidaGeneralLib.BO.TemplateEmail;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class TemplateEmailBO {
		EVidaLog log = new EVidaLog(typeof(TemplateEmailBO));

		private static TemplateEmailBO instance = new TemplateEmailBO();

		public static TemplateEmailBO Instance { get { return instance; } }

		public TemplateEmailVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return TemplateEmailDAO.GetById(id, db);
			}
		}

		public List<TemplateEmailVO> ListarTemplates() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return TemplateEmailDAO.ListarTemplates(db);
			}
		}
		public List<TemplateEmailVO> ListarTemplatesByTipo(TipoTemplateEmail tipo) {
			List<TemplateEmailVO> lstTemplates = ListarTemplates();
			if (lstTemplates != null) {
				lstTemplates = lstTemplates.FindAll(x => x.Tipo == tipo);
			}
			return lstTemplates;
		}

		public void Salvar(TemplateEmailVO vo, int cdUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Salvando template email " + vo.Id + " - " + vo.Nome);

				TemplateEmailDAO.Salvar(vo, cdUsuario, db);

				connection.Commit();
			}
		}
		public void Excluir(TemplateEmailVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				TemplateEmailDAO.Excluir(vo, db);

				connection.Commit();
			}
		}

		public string[] GetTagsByTipo(TipoTemplateEmail tipo) {
			return TemplateEmailGeradorFacade.GetTagsByTipo(tipo);
		}

		public string GerarEmail(int idTemplate, SortedDictionary<string,string> parametros,bool enviar){
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				TemplateEmailVO vo = TemplateEmailDAO.GetById(idTemplate, db);

				TemplateEmailGerador gerador = GetGerador(vo, db, parametros);

				string mensagem = gerador.GerarMensagem();

				if (enviar)
					gerador.EnviarMensagem(mensagem);

				connection.Commit();
				return mensagem;
			}
		}

		internal TemplateEmailGerador GetGerador(TemplateEmailVO vo, EvidaDatabase db, SortedDictionary<string,string> parametros) {
			return TemplateEmailGeradorFacade.GetGerador(vo, db, parametros);
		}
	}

}
