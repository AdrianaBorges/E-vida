using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO.ControleEmail {
	internal class DeclaracaoAnualDebito : ControleEmailAction {
		public DeclaracaoAnualDebito(EvidaDatabase db) : base(db) { }

		private DeclaracaoAnualDebitoVO GetReference(ControleEmailVO vo) {
			List<string> referencia = vo.Referencia;
			int cdBeneficiario = Int32.Parse(referencia[0]);
			int ano = Int32.Parse(referencia[1]);

			DeclaracaoAnualDebitoVO decVO = DeclaracaoAnualDebitoDAO.GetById(cdBeneficiario, ano, Database);
			return decVO;
		}

		public override void MarcarErro(ControleEmailVO vo, Exception erro) {
			DeclaracaoAnualDebitoVO decVO = GetReference(vo);
			DeclaracaoAnualDebitoDAO.MarcarErro(decVO, erro, Database);
		}

		public override void Finalizar(ControleEmailVO vo) {
			DeclaracaoAnualDebitoVO decVO = GetReference(vo);
			DeclaracaoAnualDebitoDAO.Finalizar(decVO, Database);
		}

		public static Dictionary<string, object> CriarParametros(DeclaracaoAnualDebitoVO vo, HcBeneficiarioVO benefVO, string filePath) {
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["BENEFICIARIO"] = benefVO;
			parameters["REF"] = vo;
			parameters["FILE_PATH"] = filePath;
			return parameters;
		}

		protected override ControleEmailVO Transform(Dictionary<string, object> parameters) {
			HcBeneficiarioVO benefVO = (HcBeneficiarioVO)parameters["BENEFICIARIO"];
			string path = (string)parameters["FILE_PATH"];
			DeclaracaoAnualDebitoVO vo = (DeclaracaoAnualDebitoVO)parameters["REF"];

			ControleEmailVO ctrEmailVO = new ControleEmailVO();
			ctrEmailVO.Anexos = new List<string>();
			ctrEmailVO.Anexos.Add(path);

			ctrEmailVO.Conteudo = EmailUtil.DeclaracaoAnual.CreateBodyDeclaracaoAnualDebito(benefVO);

			ctrEmailVO.DataAgendamento = DateTime.Now;
			ctrEmailVO.Destinatarios = new List<KeyValuePair<string, string>>();
			ctrEmailVO.Destinatarios.Add(new KeyValuePair<string, string>(benefVO.Email, benefVO.NmBeneficiario));

			ctrEmailVO.Referencia = new List<string>();
			ctrEmailVO.Referencia.Add(vo.CodBeneficiario.ToString());
			ctrEmailVO.Referencia.Add(vo.AnoRef.ToString());

			ctrEmailVO.Tipo = TipoControleEmail.DECLARACAO_DEBITO_ANUAL;
			ctrEmailVO.Titulo = EmailUtil.DeclaracaoAnual.GetMailTitleDeclaracaoAnualDebito();

			string sender = EmailUtil.DeclaracaoAnual.GetMailSenderDeclaracaoAnualDebito();
			ctrEmailVO.Sender = new KeyValuePair<string, string>(sender, "");

			return ctrEmailVO;
		}
	}
}
