using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.BO;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.VO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.DAO;

namespace eVidaGeneralLib.BO.TemplateEmail {

	internal abstract class TemplateEmailGerador {
		private EvidaDatabase db;
		protected SortedDictionary<string, string> parametros;
		protected readonly TemplateEmailVO template;

		protected TemplateEmailGerador(TemplateEmailVO template, EvidaDatabase db, SortedDictionary<string, string> parametros) {
			this.template = template;
			this.db = db;
			this.parametros = parametros;
		}

		public string GerarMensagem() {
			SortedDictionary<string, string> values = GetValuesTag();
			string[] tags = TemplateEmailBO.Instance.GetTagsByTipo(template.Tipo);

			string mensagem = template.Texto;

			if (!string.IsNullOrEmpty(mensagem)) {
				foreach (string tag in tags) {
					string replacement = null;
					if (values.ContainsKey(tag))
						replacement = values[tag];
					if (replacement == null)
						replacement = "";
					mensagem = mensagem.Replace("$$" + tag + "$$", replacement);
				}
			}
			mensagem = mensagem.Replace("\r\n", "<BR />").Replace("\n", "<br />").Replace("\"", "'");
			return mensagem;
		}

		public void EnviarMensagem(string mensagem) {
			mensagem = TratativaEspecial(mensagem);
			List<KeyValuePair<string, string>> destinatarios = GetDestinatarios();

			if (destinatarios.Count == 0) return;

			ControleEmailVO controleVO = GetControleEmail();
			controleVO.Conteudo = mensagem;
			controleVO.Destinatarios = destinatarios;
			controleVO.Titulo = template.Nome;
			controleVO.Sender = new KeyValuePair<string,string>("-", "-");
			controleVO.DataAgendamento = DateTime.Now;
						
			ControleEmailDAO.Criar(controleVO, db);

			EmailUtil.Template.SendTemplate(destinatarios, mensagem, template.Nome);

			ControleEmailDAO.Finalizar(controleVO, db);
		}

		protected virtual string TratativaEspecial(string mensagem) {
			return mensagem;
		}

		protected abstract SortedDictionary<string, string> GetValuesTag();

		protected abstract List<KeyValuePair<string, string>> GetDestinatarios();

		protected abstract ControleEmailVO GetControleEmail();
	}

}
