using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using eVidaGeneralLib.BO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.BO {
	public class IndisponibilidadeRedeBO {
		EVidaLog log = new EVidaLog(typeof(IndisponibilidadeRedeBO));

		private static IndisponibilidadeRedeBO instance = new IndisponibilidadeRedeBO();

		public static IndisponibilidadeRedeBO Instance { get { return instance; } }

		public DataTable BuscarIndisponibilidadeRede(string codint, string codemp, string matric) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return IndisponibilidadeRedeDAO.BuscarIndisponibilidadeRede(codint, codemp, matric, db);
			}
		}

        public DataTable Pesquisar(long? matricula, int? cdProtocolo, string protocoloAns, List<StatusIndisponibilidadeRede> filtroStatus, List<EncaminhamentoIndisponibilidadeRede> filtroSetor, string uf, int? idMunicipio, int? pendencia, string procedencia, bool? acompanhante)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return IndisponibilidadeRedeDAO.Pesquisar(matricula, cdProtocolo, protocoloAns, filtroStatus, filtroSetor, uf, idMunicipio, pendencia, procedencia, acompanhante, db);
            }
        }	

		public IndisponibilidadeRedeVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				IndisponibilidadeRedeVO vo = IndisponibilidadeRedeDAO.GetById(id, db);
				vo.Usuario = PUsuarioDAO.GetRowById(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg, db);
				return vo;
			}
		}

		public List<EspecialidadeVO> ListarEspecialidades() {
			List<EspecialidadeVO> lst = CacheHelper.GetFromCache<List<EspecialidadeVO>>("LST_ESPECIALIDADE");
			if (lst != null)
				return lst;

			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				lst = IndisponibilidadeRedeDAO.ListarEspecialidades(db);
			}
			CacheHelper.AddOnCache("LST_ESPECIALIDADE", lst);
			return lst;
		}

		public EspecialidadeVO GetEspecialidadeById(int idEspecialidade) {
			List<EspecialidadeVO> lst = ListarEspecialidades();
			if (lst != null)
				return lst.FirstOrDefault(x => x.Id == idEspecialidade);
			return null;
		}

		public List<PrioridadeIndisponibilidadeRede> GetPrioridadesEspecialidade(EspecialidadeVO vo) {
			List<PrioridadeIndisponibilidadeRede> lst = new List<PrioridadeIndisponibilidadeRede>();
			if (vo.PrazoConsulta != null)
				lst.Add(PrioridadeIndisponibilidadeRede.CONSULTA); 
			if (vo.PrazoExame != null)
				lst.Add(PrioridadeIndisponibilidadeRede.EXAMES);
			if (vo.PrazoAltaComplexidade != null)
				lst.Add(PrioridadeIndisponibilidadeRede.ALTA_COMPLEXIDADE);
			if (vo.PrazoAtendimentoHosp != null)
				lst.Add(PrioridadeIndisponibilidadeRede.REGIME_HOSPITALAR);
			if (vo.PrazoUrgenciaEmergencia != null)
				lst.Add(PrioridadeIndisponibilidadeRede.URGENCIA_EMERGENCIA);
			if (vo.PrazoOutros != null)
				lst.Add(PrioridadeIndisponibilidadeRede.OUTROS);
			return lst;
		}

		public int GetPrazoEspecialidade(EspecialidadeVO vo, PrioridadeIndisponibilidadeRede prioridade) {
			switch (prioridade) {
				case PrioridadeIndisponibilidadeRede.CONSULTA: return vo.PrazoConsulta.Value;
				case PrioridadeIndisponibilidadeRede.EXAMES: return vo.PrazoExame.Value;
				case PrioridadeIndisponibilidadeRede.ALTA_COMPLEXIDADE: return vo.PrazoAltaComplexidade.Value;
				case PrioridadeIndisponibilidadeRede.REGIME_HOSPITALAR: return vo.PrazoAtendimentoHosp.Value;
				case PrioridadeIndisponibilidadeRede.URGENCIA_EMERGENCIA: return vo.PrazoUrgenciaEmergencia.Value;
				default: return 0;
			}
		}

		public void CriarSolicitacao(IndisponibilidadeRedeVO vo, string obs, string origem, List<ArquivoTelaVO> lstArquivos) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				vo.DataCriacao = DateTime.Now;

				EspecialidadeVO espVO = GetEspecialidadeById(vo.IdEspecialidade);
				vo.DiasPrazo = GetPrazoEspecialidade(espVO, vo.Prioridade);

				IndisponibilidadeRedeDAO.Criar(vo, db);

				IndisponibilidadeRedeDAO.InsertStatusChange(vo.Id, null, StatusIndisponibilidadeRede.ABERTO, EncaminhamentoIndisponibilidadeRede.CREDENCIAMENTO, null, db);

				if (!string.IsNullOrEmpty(obs)) {
					IndisponibilidadeRedeObsVO voObs = new IndisponibilidadeRedeObsVO();
					voObs.IdIndisponibilidade = vo.Id;
					voObs.Observacao = obs;
					voObs.Origem = origem;
					voObs.TipoObs = IndisponibilidadeRedeObsVO.TIPO_EXTERNO;
					IndisponibilidadeRedeDAO.IncluirObs(voObs, db);
				}

				if (lstArquivos != null)
					SalvarArquivos(vo.Id, lstArquivos, db);

				if (!string.IsNullOrEmpty(vo.EmailContato)) {
                    PUsuarioVO usuarioVO = PUsuarioBO.Instance.GetUsuario(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);
                    EmailUtil.IndisponibilidadeRede.SendIndisponibilidadeRedeCriada(vo, usuarioVO, vo.EmailContato);
				}
				connection.Commit();
			}
		}

		public void SalvarSolicitacao(IndisponibilidadeRedeVO vo, List<IndisponibilidadeRedeOrcamentoVO> lstOrcamento) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				EspecialidadeVO espVO = GetEspecialidadeById(vo.IdEspecialidade);
				vo.DiasPrazo = GetPrazoEspecialidade(espVO, vo.Prioridade);

				IndisponibilidadeRedeDAO.Alterar(vo, db);

				if (lstOrcamento != null) {
					IndisponibilidadeRedeDAO.SalvarOrcamentos(vo.Id, lstOrcamento, db);
				}
				connection.Commit();
			}
		}

		public void IncluirObs(IndisponibilidadeRedeObsVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.IncluirObs(vo, db);

				if (vo.Pendencia != null) {
					IndisponibilidadeRedeVO voIndisp = IndisponibilidadeRedeDAO.GetById(vo.IdIndisponibilidade, db);
					if (voIndisp.Pendencia != vo.Pendencia) {
						IndisponibilidadeRedeDAO.AlterarPendencia(vo.IdIndisponibilidade, vo.Pendencia.Value, db);
					}
				}

				connection.Commit();
			}
		}

		public List<IndisponibilidadeRedeObsVO> ListarObs(int idIndisponibilidade) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				List<IndisponibilidadeRedeObsVO> lst = IndisponibilidadeRedeDAO.ListarObs(idIndisponibilidade, db);
				return lst;
			}
		}

		public List<IndisponibilidadeRedeHistoricoVO> ListarHistorico(int idIndisponibilidade) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				List<IndisponibilidadeRedeHistoricoVO> lst = IndisponibilidadeRedeDAO.ListarHistorico(idIndisponibilidade, db);
				return lst;
			}
		}

		public List<IndisponibilidadeRedeOrcamentoVO> ListarOrcamento(int idIndisponibilidade) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				List<IndisponibilidadeRedeOrcamentoVO> lst = IndisponibilidadeRedeDAO.ListarOrcamento(idIndisponibilidade, db);
				return lst;
			}
		}

		#region Files

		public List<IndisponibilidadeRedeArquivoVO> ListarArquivos(int idIndisponibilidade) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				List<IndisponibilidadeRedeArquivoVO> lst = IndisponibilidadeRedeDAO.ListarArquivos(idIndisponibilidade, db);
				return lst;
			}
		}

		private void SalvarArquivos(int idIndisponibilidade, List<ArquivoTelaVO> lstArquivos, EvidaDatabase db) {

			List<IndisponibilidadeRedeArquivoVO> lstNewFiles = PrepareFiles(idIndisponibilidade, lstArquivos);

			IndisponibilidadeRedeDAO.CriarArquivos(idIndisponibilidade, lstNewFiles, db);

			MoveFiles(idIndisponibilidade, lstArquivos);
		}

		private List<IndisponibilidadeRedeArquivoVO> PrepareFiles(int idIndisponibilidade, List<ArquivoTelaVO> lstArquivos) {			
			List<IndisponibilidadeRedeArquivoVO> lstNewFiles = new List<IndisponibilidadeRedeArquivoVO>();
			foreach (ArquivoTelaVO arq in lstArquivos) {
				IndisponibilidadeRedeArquivoVO fileVO = new IndisponibilidadeRedeArquivoVO();
				fileVO.IdIndisponibilidade = idIndisponibilidade;
				fileVO.NomeArquivo = arq.NomeTela;
				fileVO.TipoArquivo = (TipoArquivoIndisponibilidadeRede)Convert.ToInt32(arq.Parameters["TP_ARQUIVO"]);
				if (arq.IsNew) {
					fileVO.DataEnvio = DateTime.Now;

					string diskFile = arq.NomeFisico;
					if (!FileUtil.HasTempFile(diskFile)) {
						throw new Exception("Arquivo enviado [" + arq.NomeTela + "] não existe em disco (" + diskFile + ")!");
					}
					lstNewFiles.Add(fileVO);
				}
			}
			return lstNewFiles;
		}

		private void MoveFiles(int idIndisponibilidade, List<ArquivoTelaVO> lstArquivos) {
			String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.INDISPONIBILIDADE_REDE);
			foreach (ArquivoTelaVO arq in lstArquivos) {
				FileUtil.MoverArquivo(idIndisponibilidade.ToString(), null, arq.NomeFisico, dirDestino, arq.NomeTela);
			}
		}

		private void DeleteFiles(IEnumerable<IndisponibilidadeRedeArquivoVO> lstDel) {
			if (lstDel != null && lstDel.Count() > 0) {
				String dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.INDISPONIBILIDADE_REDE);
				foreach (IndisponibilidadeRedeArquivoVO arq in lstDel) {
					FileUtil.RemoverArquivo(arq.IdIndisponibilidade.ToString(), dirDestino, arq.NomeArquivo);
				}
			}
		}

		public void SalvarArquivo(int idIndisponibilidade, ArquivoTelaVO arq) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Salvando arquivo " + idIndisponibilidade + " - " + arq.NomeTela + " - " + arq.NomeFisico);

				List<ArquivoTelaVO> lst = new List<ArquivoTelaVO>();
				lst.Add(arq);

				SalvarArquivos(idIndisponibilidade, lst, db);

				connection.Commit();
			}
		}

		public void RemoverArquivo(IndisponibilidadeRedeArquivoVO arq) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Removendo arquivo " + arq.IdIndisponibilidade + " - " + arq.NomeArquivo);

				IndisponibilidadeRedeDAO.ExcluirArquivo(arq, db);

				List<IndisponibilidadeRedeArquivoVO> lst = new List<IndisponibilidadeRedeArquivoVO>();
				lst.Add(arq);
				DeleteFiles(lst);

				connection.Commit();
			}
		}

		public void SalvarComprovante(int idIndisponibilidade, ArquivoTelaVO arq) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Salvando comprovante " + idIndisponibilidade + " - " + arq.NomeTela + " - " + arq.NomeFisico);

				List<ArquivoTelaVO> lstNew = new List<ArquivoTelaVO>();
				lstNew.Add(arq);

				IEnumerable<IndisponibilidadeRedeArquivoVO> lstOld = IndisponibilidadeRedeDAO.ListarArquivos(idIndisponibilidade, db);
				if (lstOld != null) {
					lstOld = lstOld.Where(x => x.TipoArquivo == TipoArquivoIndisponibilidadeRede.COMPROVANTE);
					foreach (IndisponibilidadeRedeArquivoVO arqOld in lstOld) {
						IndisponibilidadeRedeDAO.ExcluirArquivo(arqOld, db);
					}
					DeleteFiles(lstOld);
				}

				SalvarArquivos(idIndisponibilidade, lstNew, db);

				connection.Commit();
			}
		}

		public void RemoverComprovante(int idIndisponibilidade) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (log.IsDebugEnabled)
					log.Debug("Removendo comprovante " + idIndisponibilidade);

				IEnumerable<IndisponibilidadeRedeArquivoVO> lstOld = IndisponibilidadeRedeDAO.ListarArquivos(idIndisponibilidade, db);
				if (lstOld != null) {
					lstOld = lstOld.Where(x => x.TipoArquivo == TipoArquivoIndisponibilidadeRede.COMPROVANTE);
					foreach (IndisponibilidadeRedeArquivoVO arqOld in lstOld) {
						IndisponibilidadeRedeDAO.ExcluirArquivo(arqOld, db);
					}
					DeleteFiles(lstOld);
				}

				connection.Commit();
			}
		}
		
        #endregion

		#region MiniForms

		public void SalvarCredenciamento(IndisponibilidadeRedeVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.SalvarCredenciamento(vo, db);

				connection.Commit();
			}
		}

		public void SalvarFinanceiro(IndisponibilidadeRedeVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.SalvarFinanceiro(vo, db);

				connection.Commit();
			}
		}

		public void SalvarDiretoria(IndisponibilidadeRedeVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.SalvarDiretoria(vo, db);

				connection.Commit();
			}
		}

		public void SalvarFaturamento(IndisponibilidadeRedeVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.SalvarFaturamento(vo, db);

				connection.Commit();
			}
		}

		public void SalvarAutorizacao(IndisponibilidadeRedeVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.SalvarAutorizacao(vo, db);

				connection.Commit();
			}
		}

		#endregion

		#region Assumir/Encaminhar

		public void AssumirSolicitacao(IndisponibilidadeRedeVO vo, bool forcado, UsuarioVO usuarioVO) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				if (forcado) {
					IndisponibilidadeRedeDAO.EncaminharSolicitacao(vo, db);
				}
				IndisponibilidadeRedeDAO.AssumirSolicitacao(vo, usuarioVO, db);

				IndisponibilidadeRedeDAO.InsertStatusChange(vo.Id, vo.Situacao, StatusIndisponibilidadeRede.EM_ATENDIMENTO, vo.SetorEncaminhamento, usuarioVO.Id, db);

				connection.Commit();
			}
		}

		public void EncaminharSolicitacao(IndisponibilidadeRedeVO vo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				EncaminharSolicitacao(vo, db);

				connection.Commit();
			}
		}

		private void EncaminharSolicitacao(IndisponibilidadeRedeVO vo, EvidaDatabase db) {
			IndisponibilidadeRedeDAO.EncaminharSolicitacao(vo, db);

			IndisponibilidadeRedeDAO.InsertStatusChange(vo.Id, vo.Situacao, StatusIndisponibilidadeRede.PENDENTE, vo.SetorEncaminhamento, null, db);

			EmailUtil.IndisponibilidadeRede.SendIndisponibilidadeRedeEncaminhar(vo);
		}

		public void Encerrar(IndisponibilidadeRedeVO vo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.Encerrar(vo, db);

				IndisponibilidadeRedeDAO.InsertStatusChange(vo.Id, vo.Situacao, StatusIndisponibilidadeRede.ENCERRADO, vo.SetorEncaminhamento, idUsuario, db);

				if (!string.IsNullOrEmpty(vo.EmailContato)) {
                    PUsuarioVO usuarioVO = PUsuarioBO.Instance.GetUsuario(vo.Usuario.Codint, vo.Usuario.Codemp, vo.Usuario.Matric, vo.Usuario.Tipreg);
                    EmailUtil.IndisponibilidadeRede.SendIndisponibilidadeRedeEncerrada(vo, usuarioVO, vo.EmailContato, vo.MotivoEncerramento);

                }
				connection.Commit();
			}
		}

		#endregion

		public bool PodeExecutarCobranca(StatusIndisponibilidadeRede status, TipoPendenciaIndisponibilidadeRede? pendencia, DateTime? dtPendencia, bool isSameUsuario) {
			if (status == StatusIndisponibilidadeRede.PENDENTE || (status == StatusIndisponibilidadeRede.EM_ATENDIMENTO && isSameUsuario)) {
				if (pendencia != null && pendencia.Value == TipoPendenciaIndisponibilidadeRede.NF_RECIBO_FINANCEIRO) {
					int diasPendencia = DateTime.Now.Date.Subtract(dtPendencia.Value.Date).Days;
					if (diasPendencia >= 10) {
						return true;
					}
				}
			}
			return false;
		}

		public void ExecutarCobranca(int id, UsuarioVO usuarioVO) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				IndisponibilidadeRedeVO vo = IndisponibilidadeRedeDAO.GetById(id, db);

				bool isSameUsuario = vo.CodUsuarioAtuante != null && vo.CodUsuarioAtuante.Value == usuarioVO.Id;
				if (!PodeExecutarCobranca(vo.Situacao, vo.Pendencia, vo.DataPendencia, isSameUsuario)) {
					throw new InvalidOperationException("A solicitação não atende às condições para enviar execução de cobrança! " +
					vo.Situacao + " - " + vo.Pendencia + " - " + vo.DataPendencia);
				}

                IndisponibilidadeRedeObsVO obsVO = new IndisponibilidadeRedeObsVO();
                obsVO.IdIndisponibilidade = id;
                obsVO.CodUsuario = usuarioVO.Id;
                obsVO.Observacao = "Favor executar cobrança integral conforme termo de compromiso em anexo devido à falta de Nota fiscal/Recibo.";
                obsVO.Origem = IndisponibilidadeRedeObsVO.ORIGEM_INTRANET;
                obsVO.Pendencia = TipoPendenciaIndisponibilidadeRede.RESOLVIDO;
                obsVO.TipoObs = IndisponibilidadeRedeObsVO.TIPO_INTERNO;

				if (vo.Situacao == StatusIndisponibilidadeRede.PENDENTE)
					IndisponibilidadeRedeDAO.AssumirSolicitacao(vo, usuarioVO, db);

				IndisponibilidadeRedeDAO.IncluirObs(obsVO, db);
				IndisponibilidadeRedeDAO.AlterarPendencia(id, obsVO.Pendencia.Value, db);
				vo.SetorEncaminhamento = EncaminhamentoIndisponibilidadeRede.FINANCEIRO;
				vo.SituacaoFinanceiro = SituacaoFinanceiroIndisponibilidadeRede.EXECUTAR_COBRANCA;
				EncaminharSolicitacao(vo, db);

				connection.Commit();
			}
		}

		public void ExecutarCobranca(int idIndisponibilidade, DateTime dtExecucao, decimal vlExecucao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				IndisponibilidadeRedeDAO.ExecutarCobranca(idIndisponibilidade, dtExecucao, vlExecucao, db);
				connection.Commit();
			}
		}

        public void SalvarCadastro(IndisponibilidadeRedeVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                IndisponibilidadeRedeDAO.SalvarCadastro(vo, db);

                connection.Commit();
            }
        }

        public void ApagarEncerramento(IndisponibilidadeRedeVO vo)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                IndisponibilidadeRedeDAO.ApagarEncerramento(vo, db);
                connection.Commit();
            }
        }

        public void AtualizarPrazos()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                List<Int32> lstProtocolo = IndisponibilidadeRedeDAO.ListarUltimosProtocolos(db);

                if (lstProtocolo.Count > 0)
                {
                    // Para cada protocolo
                    foreach (Int32 id in lstProtocolo)
                    {
                        // Limpa a tabela de horas por setor
                        IndisponibilidadeRedeDAO.ExcluirHorasSetor(id, db);

                        // Dicionário que será preenchido
                        var myDictionary = new Dictionary<int, double>();

                        // Obtém a lista de eventos
                        DataTable dtEventos = IndisponibilidadeRedeDAO.ListarEventos(id, db);

                        if (dtEventos.Rows.Count > 0)
                        {
                            // Variáveis controle
                            DateTime data_pendencia = DateTime.MinValue;
                            DateTime data_anterior = DateTime.MinValue;
                            Int32 setor_anterior = 0;

                            Boolean FlagAtivo = false;
                            Boolean FlagPendente = false;

                            // Para cada evento
                            foreach (DataRow linha in dtEventos.Rows)
                            {
                                // Obtém os dados do evento
                                DateTime data_atual = linha["DATA"] != DBNull.Value ? Convert.ToDateTime(linha["DATA"]) : DateTime.MinValue;
                                String evento_atual = linha.Field<string>("EVENTO");
                                Int32 setor_atual = linha["SETOR"] != DBNull.Value ? Convert.ToInt32(linha["SETOR"]) : 0;


                                if (evento_atual == "REGISTRADO")
                                {
                                    FlagAtivo = true;

                                    data_anterior = data_atual;
                                    setor_anterior = setor_atual;
                                }
                                else if (evento_atual == "ENCAMINHADO")
                                {
                                    if (FlagAtivo == true)
                                    {
                                        if (FlagPendente == false)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_anterior, data_atual);

                                            if (myDictionary.ContainsKey(setor_anterior)) myDictionary[setor_anterior] += horas_decorridas;
                                            else myDictionary.Add(setor_anterior, horas_decorridas);
                                        }

                                        data_anterior = data_atual;
                                        setor_anterior = setor_atual;
                                    }

                                }
                                else if (evento_atual == "PENDENTE")
                                {
                                    if (FlagAtivo == true)
                                    {

                                        if (FlagPendente == false)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_anterior, data_atual);

                                            if (myDictionary.ContainsKey(setor_anterior)) myDictionary[setor_anterior] += horas_decorridas;
                                            else myDictionary.Add(setor_anterior, horas_decorridas);

                                            data_pendencia = data_atual;
                                            FlagPendente = true;
                                        }

                                    }

                                }
                                else if (evento_atual == "RESOLVIDO")
                                {
                                    if (FlagAtivo == true)
                                    {

                                        if (FlagPendente == true)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_pendencia, data_atual);

                                            if (myDictionary.ContainsKey((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO)) myDictionary[(Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO] += horas_decorridas;
                                            else myDictionary.Add((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO, horas_decorridas);

                                            FlagPendente = false;
                                        }

                                        data_anterior = data_atual;

                                    }

                                }
                                else if (evento_atual == "ATENDIDO")
                                {
                                    if (FlagAtivo == true)
                                    {
                                        if (FlagPendente == false)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_anterior, data_atual);

                                            if (myDictionary.ContainsKey(setor_anterior)) myDictionary[setor_anterior] += horas_decorridas;
                                            else myDictionary.Add(setor_anterior, horas_decorridas);
                                        }
                                        else if (FlagPendente == true)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_pendencia, data_atual);

                                            if (myDictionary.ContainsKey((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO)) myDictionary[(Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO] += horas_decorridas;
                                            else myDictionary.Add((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO, horas_decorridas);

                                            FlagPendente = false;
                                        }

                                        FlagAtivo = false;
                                    }

                                }
                                else if (evento_atual == "ENCERRADO")
                                {
                                    if (FlagAtivo == true)
                                    {
                                        if (FlagPendente == false)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_anterior, data_atual);

                                            if (myDictionary.ContainsKey(setor_anterior)) myDictionary[setor_anterior] += horas_decorridas;
                                            else myDictionary.Add(setor_anterior, horas_decorridas);
                                        }
                                        else if (FlagPendente == true)
                                        {
                                            double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_pendencia, data_atual);

                                            if (myDictionary.ContainsKey((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO)) myDictionary[(Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO] += horas_decorridas;
                                            else myDictionary.Add((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO, horas_decorridas);

                                            FlagPendente = false;
                                        }

                                        FlagAtivo = false;
                                    }

                                }

                            }

                            if (FlagAtivo == true)
                            {
                                if (FlagPendente == false)
                                {
                                    double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_anterior, DateTime.Now);

                                    if (myDictionary.ContainsKey(setor_anterior)) myDictionary[setor_anterior] += horas_decorridas;
                                    else myDictionary.Add(setor_anterior, horas_decorridas);

                                }
                                else if (FlagPendente == true)
                                {
                                    double horas_decorridas = DateUtil.CalcularHorasDiasUteis(data_pendencia, DateTime.Now);

                                    if (myDictionary.ContainsKey((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO)) myDictionary[(Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO] += horas_decorridas;
                                    else myDictionary.Add((Int32)EncaminhamentoIndisponibilidadeRede.BENEFICIARIO, horas_decorridas);
                                }
                            }


                        }

                        foreach (KeyValuePair<int, double> kvp in myDictionary)
                        {
                            IndisponibilidadeRedeHorasSetorVO voHrs = new IndisponibilidadeRedeHorasSetorVO();
                            voHrs.IdIndisponibilidade = id;
                            voHrs.Setor = (EncaminhamentoIndisponibilidadeRede)kvp.Key;
                            voHrs.Horas = kvp.Value;

                            // Preenche a tabela de horas por setor
                            IndisponibilidadeRedeDAO.IncluirHorasSetor(voHrs, db);
                        }

                    }

                }

                connection.Commit();
            }
        }

        public List<IndisponibilidadeRedeHorasSetorVO> ListarHorasSetor(int idIndisponibilidade)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                List<IndisponibilidadeRedeHorasSetorVO> lst = IndisponibilidadeRedeDAO.ListarHorasSetor(idIndisponibilidade, db);
                return lst;
            }
        }

        public DataTable ListarProtocolosPendentes()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return IndisponibilidadeRedeDAO.ListarProtocolosPendentes(db);
            }
        }

        public Double ObterHorasUsadas(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return IndisponibilidadeRedeDAO.ObterHorasUsadas(id, db);
            }
        }

        public DataTable ListarPendencias(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return IndisponibilidadeRedeDAO.ListarPendencias(id, db);
            }
        }

        public void EnviarEmailAlerta(EncaminhamentoIndisponibilidadeRede setor, List<IndisponibilidadeRedeVO> lstProtocoloAlerta, List<IndisponibilidadeRedeVO> lstProtocoloFora)
        {
            EmailUtil.IndisponibilidadeRede.SendIndisponibilidadeRedeAlerta(setor, lstProtocoloAlerta, lstProtocoloFora);
        }
	}
}
