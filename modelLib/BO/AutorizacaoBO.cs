using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;

namespace eVidaGeneralLib.BO {
	public class AutorizacaoBO {
		EVidaLog log = new EVidaLog(typeof(AutorizacaoBO));

		private static AutorizacaoBO instance = new AutorizacaoBO();

		public static AutorizacaoBO Instance { get { return instance; } }

		#region Pesquisas

		public AutorizacaoVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AutorizacaoDAO.GetById(id, db);
			}
		}

		public bool PodeRevalidar(AutorizacaoVO vo) {
			if (vo.Status == StatusAutorizacao.APROVADA) {
				if (vo.Internacao) return true;
				DateTime dtAprovacao = vo.DataAutorizacao.Value.Date;
				if (vo.DataAprovRevalidacao != null)
					dtAprovacao = vo.DataAprovRevalidacao.Value.Date;
				if (dtAprovacao.AddDays(90) < DateTime.Now.Date) return true;
			}
			return false;
		}

        public DataTable BuscarAutorizacaoByBeneficiario(string codintTitular, string codempTitular, string matricTitular, string tipregTitular)
        {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = AutorizacaoDAO.BuscarAutorizacaoByBeneficiario(codintTitular, codempTitular, matricTitular, tipregTitular, db);
				dt = RebuildPesquisaTable(dt);
				return dt;
			}
		}

		public DataTable BuscarAutorizacaoByCredenciado(string codCredenciado) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = AutorizacaoDAO.BuscarAutorizacaoByCredenciado(codCredenciado, db);
				dt = RebuildPesquisaTable(dt);
				return dt;
			}
		}

		public DataTable Pesquisar(VO.Filter.FilterAutorizacaoVO filtro) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = AutorizacaoDAO.Pesquisar(filtro, db);
				dt = RebuildPesquisaTable(dt);
				return dt;
			}
		}

		public DataTable BuscarEmAndamento() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				DataTable dt = AutorizacaoDAO.BuscarEmAndamento(db);
				dt = RebuildPesquisaTable(dt);
				return dt;
			}
		}

		private DataTable RebuildPesquisaTable(DataTable dt) {
			if (dt != null) {
				DataColumn colObj = new DataColumn("OBJ", typeof(AutorizacaoVO));
				DataColumn colPrazo = new DataColumn("PRAZO", typeof(PrazoAutorizacaoVO));
				dt.Columns.Add(colObj);
				dt.Columns.Add(colPrazo);

				foreach (DataRow dr in dt.Rows) {
					AutorizacaoVO vo = AutorizacaoDAO.FromDataRow(dr);
					PrazoAutorizacaoVO prazo = AutorizacaoBO.Instance.CalcularPrazo(vo);
					dr["OBJ"] = vo;
					dr["PRAZO"] = prazo;
				}
			}
			return dt;
		}

		public DataTable ListarHistorico(int cdProtocolo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AutorizacaoDAO.ListarHistorico(cdProtocolo, db);
			}
		}

		private AutorizacaoVO ConvertPesquisaRow(DataRow dr) {
			return AutorizacaoDAO.FromDataRow(dr);
		}

		public List<AutorizacaoProcedimentoVO> ListProcedimentos(int idAutorizacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();				
				return AutorizacaoDAO.ListProcedimentos(idAutorizacao, db);
			}
		}

		public List<AutorizacaoArquivoVO> ListArquivos(int idAutorizacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AutorizacaoDAO.ListArquivos(idAutorizacao, db);
			}
		}

		public List<AutorizacaoSolDocVO> ListSolicitacoesDoc(int idAutorizacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AutorizacaoDAO.ListSolDoc(idAutorizacao, db);
			}
		}

		public List<AutorizacaoTissVO> ListTiss(int idAutorizacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AutorizacaoDAO.ListTiss(idAutorizacao, db);
			}
		}

		public List<UsuarioVO> ListarUsuariosGestao() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return AutorizacaoDAO.ListarUsuariosGestao(db);
			}
		}

		#endregion

		#region Atualizacoes

		public void Salvar(AutorizacaoVO vo, List<AutorizacaoArquivoVO> lstArquivos, PUsuarioVO hcBeneficiarioVO) {
			if (log.IsDebugEnabled)
                log.Debug("Salvando solicitação. Codint: " + vo.UsuarioTitular.Codint + ", Codemp: " + vo.UsuarioTitular.Codemp + ", Matric: " + vo.UsuarioTitular.Matric + ", Tipreg: " + vo.UsuarioTitular.Tipreg);

			if (vo.Id == 0) {
				List<AutorizacaoProcedimentoVO> lstProcedimentos = new List<AutorizacaoProcedimentoVO>();
				Criar(OrigemAutorizacao.BENEF, vo, lstProcedimentos, lstArquivos);
			} else {
				List<AutorizacaoProcedimentoVO> lstProcedimentos = ListProcedimentos(vo.Id);
				if (lstProcedimentos == null)
					lstProcedimentos = new List<AutorizacaoProcedimentoVO>();
				Alterar(OrigemAutorizacao.BENEF, vo, lstProcedimentos, lstArquivos);
			}
		}

		public void Salvar(AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, List<AutorizacaoArquivoVO> lstArquivos, UsuarioVO usuarioVO) {
			if (log.IsDebugEnabled)
				log.Debug("Salvando solicitação. Usuario: " + usuarioVO.Id);

			vo.CodUsuarioAlteracao = usuarioVO.Id;
			if (vo.Id == 0) {
				vo.CodUsuarioCriacao = usuarioVO.Id;
				if (lstProcedimentos == null)
					lstProcedimentos = new List<AutorizacaoProcedimentoVO>();
				Criar(OrigemAutorizacao.GESTOR, vo, lstProcedimentos, lstArquivos);
			} else {
				if (lstProcedimentos == null)
					lstProcedimentos = ListProcedimentos(vo.Id);
				if (lstProcedimentos == null)
					lstProcedimentos = new List<AutorizacaoProcedimentoVO>();
				Alterar(OrigemAutorizacao.GESTOR, vo, lstProcedimentos, lstArquivos);
			}
		}

		public void Salvar(AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, List<AutorizacaoArquivoVO> lstArquivos, PRedeAtendimentoVO credVO) {
			if (log.IsDebugEnabled)
				log.Debug("Salvando solicitação. Credenciado: " + credVO.Codigo);

			if (vo.Id == 0) {
				vo.Hospital = vo.RedeAtendimento;
				if (lstProcedimentos == null)
					lstProcedimentos = new List<AutorizacaoProcedimentoVO>();
				Criar(OrigemAutorizacao.CRED, vo, lstProcedimentos, lstArquivos);
			} else {
				if (lstProcedimentos == null)
					lstProcedimentos = ListProcedimentos(vo.Id);
				if (lstProcedimentos == null)
					lstProcedimentos = new List<AutorizacaoProcedimentoVO>();
				Alterar(OrigemAutorizacao.CRED, vo, lstProcedimentos, lstArquivos);
			}
		}

		private void Criar(OrigemAutorizacao origemOperacao, AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, List<AutorizacaoArquivoVO> lstArquivos) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				vo.DataSolicitacao = new DateTime();
				vo.Origem = origemOperacao;

				AutorizacaoDAO.Criar(vo, lstProcedimentos, db);

				List<AutorizacaoArquivoVO> lstNewFiles;
				Dictionary<string, AutorizacaoArquivoVO> mapArquivos;

				lstNewFiles = PrepareFiles(origemOperacao, vo, lstArquivos, out mapArquivos);

				AutorizacaoDAO.SalvarArquivos(vo, lstNewFiles, db);

				MoveFiles(vo, mapArquivos);

				if (ParametroUtil.EmailEnabled) {
					try {
						PUsuarioVO benef;
						PRedeAtendimentoVO cred;
						UsuarioVO usuarioCriacao;
						UsuarioVO gestor;
						FillEmailData(vo.Id, out vo, out benef, out cred, out usuarioCriacao, out gestor, db);
						EmailUtil.Autorizacao.SendAutorizacaoCriacao(vo, benef, cred, usuarioCriacao);
					} catch (Exception ex) {
						log.Error("Erro ao enviar email de criação", ex);
					}
				}

				connection.Commit();
			}
		}
		
		private void Alterar(OrigemAutorizacao origemOperacao, AutorizacaoVO vo, List<AutorizacaoProcedimentoVO> lstProcedimentos, List<AutorizacaoArquivoVO> lstArquivos) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				vo.OrigemAlteracao = origemOperacao;
				AutorizacaoDAO.Alterar(vo, lstProcedimentos, db);

				List<AutorizacaoArquivoVO> lstNewFiles;
				Dictionary<string, AutorizacaoArquivoVO> mapArquivos;

				lstNewFiles = PrepareFiles(origemOperacao, vo, lstArquivos, out mapArquivos);

				List<AutorizacaoArquivoVO> lstDel = AutorizacaoDAO.SalvarArquivos(vo, lstNewFiles, db);

				MoveFiles(vo, mapArquivos);

				if (lstDel != null && lstDel.Count > 0) {
					DeleteFiles(vo, lstDel);
				}

				EnviarEmailAlteracao(vo, vo.CodUsuarioAlteracao);

				connection.Commit();
			}
		}

		private void EnviarEmailAlteracao(AutorizacaoVO vo, int? idUsuario) {
			try {
				UsuarioVO usuario = null;
				OrigemAutorizacao origemOperacao = vo.OrigemAlteracao;
				if (origemOperacao == OrigemAutorizacao.GESTOR)
					usuario = UsuarioBO.Instance.GetUsuarioById(idUsuario.Value);

                PUsuarioVO benef = PUsuarioBO.Instance.GetUsuario(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);
				PRedeAtendimentoVO cred = null;
				if (vo.RedeAtendimento != null && vo.RedeAtendimento.Codigo != "")
					cred = PRedeAtendimentoBO.Instance.GetById(vo.RedeAtendimento.Codigo);

				EnviarEmailAlteracao(vo, benef, cred, usuario);
			}
			catch (Exception ex) {
				log.Error("Erro ao enviar email de alteracao", ex);
			}
		}

		private void EnviarEmailAlteracao(AutorizacaoVO vo, PUsuarioVO benef, PRedeAtendimentoVO cred, UsuarioVO usuario) {
			EmailUtil.Autorizacao.SendAutorizacaoAlteracao(vo, benef, cred, usuario);
		}

		public void EnviarDocumentos(OrigemAutorizacao origemOperacao, AutorizacaoVO vo, int? idUsuario, List<AutorizacaoArquivoVO> lstArqs) {
			List<AutorizacaoArquivoVO> lstNewFiles;
			Dictionary<string, AutorizacaoArquivoVO> mapArquivos;
			vo.OrigemAlteracao = origemOperacao;
			if (origemOperacao == OrigemAutorizacao.GESTOR)
				vo.CodUsuarioAlteracao = idUsuario;
			lstNewFiles = PrepareFiles(origemOperacao, vo, lstArqs, out mapArquivos);

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoDAO.EnviarDocumentos(origemOperacao, vo.Id, idUsuario, lstNewFiles, db);

				MoveFiles(vo, mapArquivos);

				EnviarEmailAlteracao(vo, idUsuario);

				connection.Commit();
			}
		}

		#endregion

		#region Files

		private List<AutorizacaoArquivoVO> PrepareFiles(OrigemAutorizacao origemOperacao, AutorizacaoVO vo, List<AutorizacaoArquivoVO> lstArquivos,  out Dictionary<string, AutorizacaoArquivoVO> mapArquivos) {
			Sistema sistema = Sistema.INTRANET;
            string uid = "";

			if (origemOperacao == OrigemAutorizacao.CRED) {
				sistema = Sistema.CREDENCIADO;
				uid = vo.RedeAtendimento.Codigo;
			} else if (origemOperacao == OrigemAutorizacao.BENEF) {
				sistema = Sistema.BENEFICIARIO;
                uid = vo.UsuarioTitular.Codint.Trim() + vo.UsuarioTitular.Codemp.Trim() + vo.UsuarioTitular.Matric.Trim() + vo.UsuarioTitular.Tipreg.Trim();
			} else {
				sistema = Sistema.INTRANET;
				uid = vo.CodUsuarioAlteracao.Value.ToString();
			}

			string prefix = UploadConfigManager.GetPrefix(UploadFilePrefix.AUTORIZACAO, sistema, uid);

			List<AutorizacaoArquivoVO> lstNewFiles = new List<AutorizacaoArquivoVO>();
			mapArquivos = new Dictionary<string, AutorizacaoArquivoVO>();
			foreach (AutorizacaoArquivoVO arq in lstArquivos) {
				if (arq.CodAutorizacao == 0) {
					AutorizacaoArquivoVO newFile = new AutorizacaoArquivoVO();
					newFile.CodAutorizacao = vo.Id;
					newFile.DataEnvio = DateTime.Now;
					newFile.NomeArquivo = arq.NomeArquivo;

					string diskFile = prefix + "_" + arq.NomeArquivo;
					if (!FileUtil.HasTempFile(diskFile)) {
						throw new Exception("Arquivo enviado [" + arq.NomeArquivo + "] não existe em disco (" + diskFile + ")!");
					}
					lstNewFiles.Add(newFile);
					mapArquivos.Add(diskFile, arq);
				} else {
					lstNewFiles.Add(arq);
				}
			}
			return lstNewFiles;
		}

		private void MoveFiles(AutorizacaoVO vo, Dictionary<string, AutorizacaoArquivoVO> mapArquivos) {
			String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.AUTORIZACAO);
			foreach (string diskFile in mapArquivos.Keys) {
				AutorizacaoArquivoVO arq = mapArquivos[diskFile];
				FileUtil.MoverArquivo(vo.Id.ToString(), null, diskFile, dirDestino, arq.NomeArquivo);
			}
		}

		private void DeleteFiles(AutorizacaoVO vo, List<AutorizacaoArquivoVO> lstDel) {
			String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.AUTORIZACAO);
			foreach (AutorizacaoArquivoVO arq in lstDel) {
				FileUtil.RemoverArquivo(vo.Id.ToString(), dirDestino, arq.NomeArquivo);
			}
		}

		#endregion

		#region Alt Status

		private void FillEmailData(int idAutorizacao, out AutorizacaoVO vo, out PUsuarioVO benef, out PRedeAtendimentoVO cred, 
			 out UsuarioVO usuarioCriacao, out UsuarioVO gestor, EvidaDatabase db) {
			vo = AutorizacaoDAO.GetById(idAutorizacao, db);
            benef = PUsuarioDAO.GetRowById(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg, db);
			cred = null;			
			gestor = null;
			usuarioCriacao = null;

			if (vo.CodUsuarioAlteracao != null)
				gestor = UsuarioDAO.GetUsuarioById(vo.CodUsuarioAlteracao.Value, db);

			if (vo.Origem == OrigemAutorizacao.CRED) {
				if (vo.RedeAtendimento != null && vo.RedeAtendimento.Codigo != "")
					cred = PRedeAtendimentoDAO.GetById(vo.RedeAtendimento.Codigo, db);
			} else if (vo.Origem == OrigemAutorizacao.GESTOR) {
				if (vo.CodUsuarioCriacao != null)
					usuarioCriacao = UsuarioDAO.GetUsuarioById(vo.CodUsuarioCriacao.Value, db);
			}
		}

		public AutorizacaoVO IniciarCotacao(int idAutorizacao, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoVO autVO = AutorizacaoDAO.GetById(idAutorizacao, db);
				if (AutorizacaoTradutorHelper.IsStatusFim(autVO.Status)) {
					throw new InvalidOperationException("A autorização está no status '" +
						AutorizacaoTradutorHelper.TraduzStatus(autVO.Status) +
						"' e não pode iniciar cotação!");
				}
				AutorizacaoDAO.IniciarCotacao(idAutorizacao, idUsuario, db);

				if (ParametroUtil.EmailEnabled) {
					PUsuarioVO benef;
					PRedeAtendimentoVO cred;
					UsuarioVO usuarioCriacao;
					UsuarioVO gestor;
					FillEmailData(idAutorizacao, out autVO, out benef, out cred, out usuarioCriacao, out gestor, db);
					EmailUtil.Autorizacao.SendAutorizacaoCotacao(autVO, benef, cred, usuarioCriacao, gestor);

					EnviarEmailAlteracao(autVO, benef, cred, gestor);
				} else {
					autVO = AutorizacaoDAO.GetById(idAutorizacao, db);
				}
				connection.Commit();
				return autVO;
			}
		}

		public AutorizacaoVO IniciarAnalise(int idAutorizacao, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoVO vo = AutorizacaoDAO.GetById(idAutorizacao, db);
				if (vo.Status != StatusAutorizacao.ENVIADA && vo.Status != StatusAutorizacao.REVALIDACAO) {
					throw new Exception("Outro usuário já iniciou a análise desta autorização.");
				}
				AutorizacaoDAO.IniciarAnalise(idAutorizacao, idUsuario, db);

				if (ParametroUtil.EmailEnabled) {
					PUsuarioVO benef;
					PRedeAtendimentoVO cred;
					UsuarioVO usuarioCriacao;
					UsuarioVO gestor;
					FillEmailData(idAutorizacao, out vo, out benef, out cred, out usuarioCriacao, out gestor, db);
					EmailUtil.Autorizacao.SendAutorizacaoAnalise(vo, benef, cred, usuarioCriacao, gestor);
					EnviarEmailAlteracao(vo, benef, cred, gestor);
				} else {
					vo = AutorizacaoDAO.GetById(idAutorizacao, db);
				}
				connection.Commit();
				return vo;
			}
		}

		public List<string> CheckAntesAprovar(int cdAutorizacao) {
			AutorizacaoVO aut = GetById(cdAutorizacao);

			if (aut == null)
				throw new Exception("Autorização não encontrada! [" + cdAutorizacao + "]");

			List<string> msg = new List<string>();
			PRedeAtendimentoVO hosp = aut.Hospital;
			if (hosp != null) {
                if (hosp.Codigo == "" || hosp.Codigo == null)
                {
					msg.Add("O hospital/clínica associado não faz parte do Protheus. Favor selecionar pelo sistema de busca no campo do hospital.");
				} else {
					hosp = PRedeAtendimentoBO.Instance.GetById(hosp.Codigo);
				}
			}
			if (hosp == null)
                msg.Add("O hospital/clínica associado não faz parte do Protheus. Favor selecionar pelo sistema de busca no campo do hospital.");

			PProfissionalSaudeVO prof = PLocatorDataBO.Instance.GetProfissional(aut.Profissional.Numcr, aut.Profissional.Estado, aut.Profissional.Codsig);
			if (prof == null)
                msg.Add("O profissional associado não faz parte do Protheus. [" + aut.Profissional.Numcr + " - " + aut.Profissional.Codsig + " - " + aut.Profissional.Estado + "]");

			List<AutorizacaoProcedimentoVO> lstProcs = AutorizacaoBO.Instance.ListProcedimentos(cdAutorizacao);
			if (lstProcs == null || lstProcs.Count == 0)
				msg.Add("É necessário ter pelo menos um procedimento!");

			List<AutorizacaoArquivoVO> lstArqs = AutorizacaoBO.Instance.ListArquivos(cdAutorizacao);
			if (lstArqs == null || lstArqs.Count == 0)
				msg.Add("É necessário ter pelo menos uma solicitação médica!");

			if (aut.Status == StatusAutorizacao.APROVADA) {
				UsuarioVO usuarioAlt = UsuarioBO.Instance.GetUsuarioById(aut.CodUsuarioAlteracao.Value);
				msg.Add("Autorização " + aut.Id + " já está aprovada por outro usuário [" + usuarioAlt.Login + " - " + aut.DataAlteracao.ToString("dd/MM/yyyy HH:mm:ss") + "] !");
			}
			/*
			if (aut.NrAutorizacaoTiss == null || aut.NrAutorizacaoTiss.Value == 0)
				msg.Add("Para aprovação, a solicitação deve constar o número da autorização TISS.");

			if (string.IsNullOrEmpty(aut.ArquivoTiss))
				msg.Add("É necessário anexar o PDF da autorização emitida pelo sistema ISA HC.");
			*/
			return msg;
		}

		public void SolicitarDocumento(int idAutorizacao, string strConteudo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoVO autVO = AutorizacaoDAO.GetById(idAutorizacao, db);
				if (AutorizacaoTradutorHelper.IsStatusFim(autVO.Status)) {
					throw new InvalidOperationException("A autorização está no status '" +
						AutorizacaoTradutorHelper.TraduzStatus(autVO.Status) +
						"' e não pode ter documentação solicitada!");
				}
				AutorizacaoDAO.SolicitarDocumento(idAutorizacao, strConteudo, idUsuario, db);
				if (ParametroUtil.EmailEnabled) {
					AutorizacaoVO vo;
					PUsuarioVO benef;
					PRedeAtendimentoVO cred;
					UsuarioVO usuarioCriacao;
					UsuarioVO gestor;
					FillEmailData(idAutorizacao, out vo, out benef, out cred, out usuarioCriacao, out gestor, db);

					EmailUtil.Autorizacao.SendAutorizacaoSolDocumento(vo, strConteudo, benef, cred, usuarioCriacao, gestor);
					EnviarEmailAlteracao(vo, benef, cred, gestor);
				}
				connection.Commit();
			}
		}

		public void Cancelar(int idAutorizacao, string motivo, int? idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoVO vo = AutorizacaoDAO.GetById(idAutorizacao, db);
				if (idUsuario != null) {
					if (AutorizacaoTradutorHelper.IsStatusFim(vo.Status)) {
						throw new InvalidOperationException("A autorização está no status '" +
							AutorizacaoTradutorHelper.TraduzStatus(vo.Status) +
							"' e não pode ser cancelada!");
					}
				} else if (vo.Status != StatusAutorizacao.ENVIADA) {
					throw new InvalidOperationException("A autorização está no status '" +
							AutorizacaoTradutorHelper.TraduzStatus(vo.Status) + "'." +
							" Apenas autorizações no status ENVIADA podem ser canceladas por seu usuário!");
				}

				AutorizacaoDAO.Cancelar(idAutorizacao, motivo, idUsuario, db);
				if (ParametroUtil.EmailEnabled) {
					if (idUsuario != null) {
						PUsuarioVO benef;
						PRedeAtendimentoVO cred;
						UsuarioVO usuarioCriacao;
						UsuarioVO gestor;
						FillEmailData(idAutorizacao, out vo, out benef, out cred, out usuarioCriacao, out gestor, db);
						EmailUtil.Autorizacao.SendAutorizacaoCancelar(vo, motivo, benef, cred, usuarioCriacao, gestor);
						EnviarEmailAlteracao(vo, benef, cred, gestor);
					}
				}
				connection.Commit();
			}
		}

		public void Negar(int idAutorizacao, int? idNeg, int idUsuario, byte[] anexoNeg) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoVO autVO = AutorizacaoDAO.GetById(idAutorizacao, db);
				if (AutorizacaoTradutorHelper.IsStatusFim(autVO.Status)) {
					throw new InvalidOperationException("A autorização está no status '" +
						AutorizacaoTradutorHelper.TraduzStatus(autVO.Status) +
						"' e não pode ser negada!");
				}
				AutorizacaoDAO.Negar(idAutorizacao, idNeg, idUsuario, db);
				if (ParametroUtil.EmailEnabled) {
					AutorizacaoVO vo;
					PUsuarioVO benef;
					PRedeAtendimentoVO cred;
					UsuarioVO usuarioCriacao;
					UsuarioVO gestor;
					FillEmailData(idAutorizacao, out vo, out benef, out cred, out usuarioCriacao, out gestor, db);

					EmailUtil.Autorizacao.SendAutorizacaoNegar(vo, idNeg, benef, cred, usuarioCriacao, gestor, anexoNeg);
					EnviarEmailAlteracao(vo, benef, cred, gestor);
				}
				connection.Commit();
			}
		}

		public void Aprovar(int idAutorizacao, List<AutorizacaoTissVO> lstArquivo, string obs, int idUsuario) {
			List<string> lstErroCheck = CheckAntesAprovar(idAutorizacao);
			if (lstErroCheck != null && lstErroCheck.Count > 0)
				throw new InvalidOperationException("Existem pendências para a aprovação da autorização!");

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Aprovando solicitação. Solicitacao: " + idAutorizacao);
				String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.AUTORIZACAO);

				List<string> lstOriginalFileName = new List<string>();

				for (int i = 0; i < lstArquivo.Count; i++) {
					AutorizacaoTissVO arquivo = lstArquivo[i];
					string fileSave = idAutorizacao + "_ARQ_ISA_" + i + FileUtil.GetFileExtension(arquivo.NomeArquivo);
					lstOriginalFileName.Add(arquivo.NomeArquivo);
					arquivo.NomeArquivo = fileSave;
				}

				AutorizacaoVO oldVO = AutorizacaoDAO.GetById(idAutorizacao, db);
				if (oldVO.Status == StatusAutorizacao.APROVADA) {
					throw new InvalidOperationException("A autorização " + oldVO.Id  + " já foi aprovada em outra operação!");
				}

				AutorizacaoDAO.Aprovar(idAutorizacao, lstArquivo, obs, idUsuario, db);

				AutorizacaoVO vo;
				PUsuarioVO benef;
				PRedeAtendimentoVO cred;
				UsuarioVO usuarioCriacao;
				UsuarioVO gestor;
				FillEmailData(idAutorizacao, out vo, out benef, out cred, out usuarioCriacao, out gestor, db);

				for (int i = 0; i < lstOriginalFileName.Count; i++) {
					AutorizacaoTissVO arquivo = lstArquivo[i];
					FileUtil.MoverArquivo(idAutorizacao.ToString(), null, lstOriginalFileName[i], dirDestino, arquivo.NomeArquivo);
				}

				if (ParametroUtil.EmailEnabled) {
					EmailUtil.Autorizacao.SendAutorizacaoAprov(vo, benef, cred, usuarioCriacao, gestor, dirDestino, lstArquivo);
					EnviarEmailAlteracao(vo, benef, cred, gestor);
				}
				connection.Commit();
			}
		}

		public void Revalidar(AutorizacaoVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				
				AutorizacaoDAO.Revalidar(vo, db);

				vo = AutorizacaoDAO.GetById(vo.Id, db);
				EnviarEmailAlteracao(vo, vo.CodUsuarioAlteracao);

				connection.Commit();
			}
		}

		public void EnviarComentarios(int id, string comentario, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				AutorizacaoVO autVO = AutorizacaoDAO.GetById(id, db);
				if (AutorizacaoTradutorHelper.IsStatusFim(autVO.Status)) {
					throw new InvalidOperationException("A autorização está no status '" +
						AutorizacaoTradutorHelper.TraduzStatus(autVO.Status) +
						"' e não pode enviar comentários!");
				}
				AutorizacaoDAO.EnviarComentarios(id, comentario, idUsuario, db);
				if (ParametroUtil.EmailEnabled) {
					PUsuarioVO benef;
					PRedeAtendimentoVO cred;
					UsuarioVO usuarioCriacao;
					UsuarioVO gestor;
					FillEmailData(id, out autVO, out benef, out cred, out usuarioCriacao, out gestor, db);

					EnviarEmailAlteracao(autVO, benef, cred, gestor);
				}
				connection.Commit();
			}
		}

		public void SolicitarPericia(AutorizacaoVO autVO, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				autVO = AutorizacaoDAO.GetById(autVO.Id, db);
				if (AutorizacaoTradutorHelper.IsStatusFim(autVO.Status)) {
					throw new InvalidOperationException("A autorização está no status '" +
						AutorizacaoTradutorHelper.TraduzStatus(autVO.Status) +
						"' e não pode solicitar perícia!");
				}
				AutorizacaoDAO.SolicitarPericia(autVO.Id, idUsuario, db);
				if (ParametroUtil.EmailEnabled) {
					PUsuarioVO benef;
					PRedeAtendimentoVO cred;
					UsuarioVO usuarioCriacao;
					UsuarioVO gestor;
					FillEmailData(autVO.Id, out autVO, out benef, out cred, out usuarioCriacao, out gestor, db);

					EnviarEmailAlteracao(autVO, benef, cred, gestor);
					EmailUtil.Autorizacao.SendAutorizacaoPericia(autVO);
				}
				connection.Commit();
			}
		}

		#endregion

		#region Prazo

		private PrazoAutorizacaoVO CalcularPrazo(double horas, StatusAutorizacao status, DateTime dtStatus, bool opme) {
			if (AutorizacaoTradutorHelper.NecessitaCalculoPrazo(status)) {
				DateTime ultData = dtStatus;
				horas += DateUtil.CalcularHorasDiasUteis(ultData, DateTime.Now);
			}
			return MontarPrazo(horas, opme);
		}

		public PrazoAutorizacaoVO CalcularPrazo(AutorizacaoVO vo) {
			return CalcularPrazo(vo.HorasPrazo, vo.Status, vo.DataStatus, vo.Opme);
		}

		private PrazoAutorizacaoVO MontarPrazo(double horas, bool isOpme) {
			PrazoAutorizacao prazo = PrazoAutorizacao.DENTRO_PRAZO;
			double prazoMax = 5 * 24;
			double alerta = prazoMax * 0.9;
			double diff = 0;
			if (horas >= prazoMax) {
				prazo = PrazoAutorizacao.FORA_PRAZO;
				diff = horas - prazoMax;
			} else if (horas >= alerta) {
				prazo = PrazoAutorizacao.ALERTA;
				diff = horas - alerta;
			}

			return new PrazoAutorizacaoVO()
			{
				Prazo = prazo,
				Horas = horas,
				Diff = diff
			};
		}

		#endregion

		public void EnviarEmailAlerta(List<AutorizacaoVO> lstProtocoloNovo, List<AutorizacaoVO> lstProtocoloAlerta, List<AutorizacaoVO> lstProtocoloFora) {
			EmailUtil.Autorizacao.SendAutorizacaoAlerta(lstProtocoloNovo, lstProtocoloAlerta, lstProtocoloFora, false);
			EmailUtil.Autorizacao.SendAutorizacaoAlerta(lstProtocoloNovo, lstProtocoloAlerta, lstProtocoloFora, true);
		}

	}
}
