using eVidaGeneralLib.BO.ControleEmail;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class DeclaracaoAnualDebitoBO {
		EVidaLog log = new EVidaLog(typeof(DeclaracaoAnualDebitoBO));

		private static DeclaracaoAnualDebitoBO instance = new DeclaracaoAnualDebitoBO();

		public static DeclaracaoAnualDebitoBO Instance { get { return instance; } }


		public DataTable Pesquisar(int ano, string cdPlano, int? empresa, long? matricula, int? status, bool? apenasQuitados) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return DeclaracaoAnualDebitoDAO.Pesquisar(ano, cdPlano, empresa, matricula, status, apenasQuitados, db);
			}
		}

		public void Enviar(List<int> lstBeneficiario, int ano, int cdUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Enviando decl deb anual em lote.");

				List<DeclaracaoAnualDebitoVO> lstOld = DeclaracaoAnualDebitoDAO.ListById(lstBeneficiario, ano, db);
				if (lstOld == null)
					lstOld = new List<DeclaracaoAnualDebitoVO>();
				List<int> lstPendenciaAno = DeclaracaoAnualDebitoDAO.ListPendenciaAno(lstBeneficiario, ano, db);
				if (lstPendenciaAno == null)
					lstPendenciaAno = new List<int>();

				foreach (int codBeneficiario in lstBeneficiario) {
					if (lstPendenciaAno.Contains(codBeneficiario)) continue;

					DeclaracaoAnualDebitoVO old = lstOld.FirstOrDefault(x => x.CodBeneficiario == codBeneficiario && x.AnoRef == ano);

					if (old == null) {
						DeclaracaoAnualDebitoVO vo = new DeclaracaoAnualDebitoVO();
						vo.CodBeneficiario = codBeneficiario;
						vo.AnoRef = ano;
						vo.CodUsuarioSolicitacao = cdUsuario;

						DeclaracaoAnualDebitoDAO.Criar(vo, db);
					} else {
						DeclaracaoAnualDebitoDAO.Ressolicitar(old, db);
					}

				}

				connection.Commit();
			}
		}
		
		public void Enviar(DeclaracaoAnualDebitoVO vo, int? cdUsuario, byte[] pdfBytes) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Enviando decl deb anual. Benef: " + vo.CodBeneficiario + " ano: " + vo.AnoRef + " Usuario: " + cdUsuario);

				DeclaracaoAnualDebitoVO old = DeclaracaoAnualDebitoDAO.GetById(vo.CodBeneficiario, vo.AnoRef, db);
				vo.CodUsuarioSolicitacao = cdUsuario;

				if (!DeclaracaoAnualDebitoDAO.CheckDebitoAno(vo.CodBeneficiario, vo.AnoRef, db)) {
					throw new EvidaException("O beneficiário possui pendências no ano " + vo.AnoRef);
				}

				if (old == null) {
					DeclaracaoAnualDebitoDAO.Criar(vo, db);
				}

				HcBeneficiarioVO benefVO = HcVBeneficiarioDAO.GetRowById(vo.CodBeneficiario, db);

				DeclaracaoAnualDebitoDAO.Finalizar(vo, db);

				EmailUtil.DeclaracaoAnual.SendDeclaracaoAnualDebito(vo, benefVO, pdfBytes);
				connection.Commit();
			}
		}

		public bool CheckDebitoAno(int cdBeneficiario, int ano) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return DeclaracaoAnualDebitoDAO.CheckDebitoAno(cdBeneficiario, ano, db);
			}
		}

		public void FillInfo(DeclaracaoAnualDebitoInfoVO info) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				HcBeneficiarioVO benef = HcVBeneficiarioDAO.GetRowById(info.CdBeneficiario, db);
				HcBeneficiarioPlanoVO benefPlanoVO = HcBeneficiarioPlanoDAO.GetLastBeneficiarioData(info.CdBeneficiario, null, db);
				HcPlanoVO planoVO = LocatorDataBO.Instance.GetPlano(benefPlanoVO.CdPlanoVinculado, db);

				info.Cpf = benef.NrCpf;
				info.NomeBeneficiario = benef.NmBeneficiario;
				info.NomePlano = planoVO.DsPlano;
			}
		}

		public string BuildConteudo(DeclaracaoAnualDebitoInfoVO vo) {
			string templatePath = FileUtil.GetFilePath(FileUtil.FileDir.DECLARACAO_ANUAL_DEBITO, null, null, "template.htm");
			string conteudo = System.IO.File.ReadAllText(templatePath);
			conteudo = conteudo.Replace("$$NOME_BENEFICIARIO$$", vo.NomeBeneficiario);
			conteudo = conteudo.Replace("$$ANO$$", vo.AnoRef.ToString());
            conteudo = conteudo.Replace("$$CPF$$", vo.Cpf != null ? FormatUtil.FormatCpf(vo.Cpf) : "");
			conteudo = conteudo.Replace("$$PLANO$$", vo.NomePlano);
			return conteudo;
		}


		public List<DeclaracaoAnualDebitoVO> ListarSolPendente(int maxRecords) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();

				try {
					List<DeclaracaoAnualDebitoVO> lst = DeclaracaoAnualDebitoDAO.ListarSolPendente(maxRecords, db);
					return lst;
				} finally {
					connection.Close();
				}
			}
		}

		public void RegistrarErro(DeclaracaoAnualDebitoVO vo, Exception ex) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				log.Error("Registrando ERRO " + vo.CodBeneficiario + " - " + vo.AnoRef, ex);

				DeclaracaoAnualDebitoDAO.MarcarErro(vo, ex, db);
				connection.Commit();
			}
		}

		public void RegistarGerado(DeclaracaoAnualDebitoVO vo, byte[] pdfBytes) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Gerando PDF " + vo.CodBeneficiario + " - " + vo.AnoRef);

				bool debitoOK = DeclaracaoAnualDebitoDAO.CheckDebitoAno(vo.CodBeneficiario, vo.AnoRef, db);
				if (!debitoOK) {
					throw new EvidaException("O beneficiário possui pendências no ano " + vo.AnoRef);
				}
				HcBeneficiarioVO benefVO = HcVBeneficiarioDAO.GetRowById(vo.CodBeneficiario, db);
				if (string.IsNullOrEmpty(benefVO.Email)) {
					throw new EvidaException("Beneficiário não possui email cadastrado!");
				}

				string dir = FileUtil.GetRepositoryDir(FileUtil.FileDir.DECLARACAO_ANUAL_DEBITO, vo.AnoRef.ToString(), true);
				string path = FileUtil.SaveFile(dir, "DECLARACAO_ANUAL_DEBITO_" + vo.AnoRef + "_" + vo.CodBeneficiario + ".pdf", pdfBytes);

				DeclaracaoAnualDebitoDAO.RegistrarGerado(vo, db);

				Dictionary<string, object> paramEmail = DeclaracaoAnualDebito.CriarParametros(vo, benefVO, path);
				ControleEmailAction.Criar(paramEmail, TipoControleEmail.DECLARACAO_DEBITO_ANUAL, db);

				connection.Commit();
			}
		}
	}
}
