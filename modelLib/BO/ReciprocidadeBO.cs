using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.DAO;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.HC;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.DAO.HC;
using eVidaGeneralLib.DAO.Protheus;

namespace eVidaGeneralLib.BO {
	public class ReciprocidadeBO {
		EVidaLog log = new EVidaLog(typeof(ReciprocidadeBO));

		private static ReciprocidadeBO instance = new ReciprocidadeBO();

		public static ReciprocidadeBO Instance { get { return instance; } }

		public List<EmpresaReciprocidadeVO> ListarEmpresas() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.ListarEmpresas(db);
			}
		}

		public EmpresaReciprocidadeVO GetEmpresaById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.GetEmpresaById(id, db);
			}
		}

		public void SalvarEmpresa(EmpresaReciprocidadeVO vo, bool novoRegistro) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Salvando empresa. Codigo: " + vo.Codigo + " Nome: " + vo.Nome);

				ReciprocidadeDAO.Salvar(vo, novoRegistro, db);

				connection.Commit();
			}
		}

		public void ExcluirEmpresa(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Excluindo empresa. Codigo: " + id);

				ReciprocidadeDAO.ExcluirEmpresa(id, db);

				connection.Commit();
			}
		}

        public List<POperadoraSaudeVO> ListarOperadoras()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return ReciprocidadeDAO.ListarOperadoras(db);
            }
        }

        public POperadoraSaudeVO GetOperadoraById(string id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return ReciprocidadeDAO.GetOperadoraById(id, db);
            }
        }

        public List<PContatoOperadoraVO> ListarContatoOperadora(string id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return ReciprocidadeDAO.ListarContatoOperadora(id, db);
            }
        }

		public DataTable RelatorioReciprocidade(long? cdMatricula, string protocoloAns, string nmTitular, string nmDependente, string cdEmpresa, DateTime? dtInicio, DateTime? dtFim, StatusReciprocidade? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.RelatorioReciprocidade(cdMatricula, protocoloAns, nmTitular, nmDependente, cdEmpresa, dtInicio, dtFim, status, db);
			}
		}

		public DataTable BuscarReciprocidade(string codint, string codemp, string matric) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.BuscarReciprocidade(codint, codemp, matric, db);
			}
		}

		public DataTable ChecarConcorrencia(string codint, string codemp, string matric, List<ReciprocidadeBenefVO> lst, DateTime dtInicio, DateTime dtFim, string cidade, string uf) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
                return ReciprocidadeDAO.ChecarConcorrencia(codint, codemp, matric, lst, dtInicio, dtFim, cidade, uf, db);
			}
		}

		public void Salvar(ReciprocidadeVO vo, List<ReciprocidadeBenefVO> lst, string email) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
                    log.Debug("Salvando solicitação " + vo.Codint + " - " + vo.Codemp + " - " + vo.Matric + " - " + email);

                VO.Protheus.PUsuarioVO titular = DAO.Protheus.PUsuarioDAO.GetTitular(vo.Codint, vo.Codemp, vo.Matric, db);
				if (titular == null)
					throw new InvalidOperationException("Funcionário inexistente");
				if (!string.IsNullOrEmpty(email)) {
					if (!email.Equals(titular.Email, StringComparison.InvariantCultureIgnoreCase)) {
                        DAO.Protheus.PUsuarioDAO.AlterarEmailTitular(vo.Codint, vo.Codemp, vo.Matric, email, db);
					}
				}

				ReciprocidadeDAO.Salvar(vo, lst, db);

				connection.Commit();
			}
		}

		public ReciprocidadeVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				ReciprocidadeVO vo = ReciprocidadeDAO.GetById(id, db);
				return vo;
			}
		}

		public DataTable BuscarBeneficiarios(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.BuscarBeneficiarios(id, db);
			}
		}

		public void Aprovar(int cdProtocolo, string observacao, string fileName, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Aprovando solicitação. Solicitacao: " + cdProtocolo);

				string dirDestino = FileUtil.GetRepositoryDir(FileUtil.FileDir.RECIPROCIDADE);
				if (!string.IsNullOrEmpty(fileName)) {
					fileName = FileUtil.MoverArquivo(cdProtocolo.ToString(), null, fileName, dirDestino, fileName);
				}

				ReciprocidadeDAO.Aprovar(cdProtocolo, idUsuario, observacao, fileName, db);

				if (ParametroUtil.EmailEnabled) {
					ReciprocidadeVO vo = ReciprocidadeDAO.GetById(cdProtocolo, db);
                    PUsuarioVO funcVO = DAO.Protheus.PUsuarioDAO.GetTitular(vo.Codint, vo.Codemp, vo.Matric, db);
                    POperadoraSaudeVO empresa = ReciprocidadeDAO.GetOperadoraById(vo.CodintReciprocidade, db);
					EmailUtil.Reciprocidade.SendEmailReciprocidadeAprovacaoFuncionario(vo, funcVO, empresa, dirDestino);
				}
				connection.Commit();
			}
		}

		public void Cancelar(int cdProtocolo, string motivo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Cancelando solicitação. Solicitacao: " + cdProtocolo);

				ReciprocidadeDAO.Cancelar(cdProtocolo, motivo, idUsuario, db);

				connection.Commit();
			}
		}

		public DataTable Pesquisar(string matemp, int? cdProtocolo, string protocoloAns, StatusReciprocidade? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.Pesquisar(matemp, cdProtocolo, protocoloAns, status, db);
			}
		}

		public void Enviar(ReciprocidadeVO vo, int idUsuario, byte[] anexo) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				if (log.IsDebugEnabled)
					log.Debug("Enviando solicitação " + vo.CodSolicitacao);

				ReciprocidadeDAO.Enviar(vo, idUsuario, db);

				if (anexo != null) {
                    POperadoraSaudeVO empresa = ReciprocidadeDAO.GetOperadoraById(vo.CodintReciprocidade, db);
					vo = ReciprocidadeDAO.GetById(vo.CodSolicitacao, db);
                    PUsuarioVO titular = PUsuarioDAO.GetTitular(vo.Codint, vo.Codemp, vo.Matric, db);
                    PFamiliaVO famVO = PFamiliaDAO.GetByMatricula(vo.Codint, vo.Codemp, vo.Matric, db);
					EnviarEmailsReciprocidade(vo, titular, famVO, empresa, anexo);
				}
				connection.Commit();
			}
		}

        private void EnviarEmailsReciprocidade(ReciprocidadeVO vo, PUsuarioVO titular, PFamiliaVO famVO, POperadoraSaudeVO empresa, byte[] anexo)
        {
			EmailUtil.Reciprocidade.SendEmailReciprocidadeEmpresa(vo, titular, empresa, anexo);
            EmailUtil.Reciprocidade.SendEmailReciprocidadeFuncionario(vo, titular, empresa);
		}

		public bool HasLimiteOrtodontico(int cdSolicitacao) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return ReciprocidadeDAO.HasLimiteManutencaoOrtodontica(cdSolicitacao, db);
			}
		}

        public void EnviarEmailAlerta(UsuarioVO usuario, List<ReciprocidadeVO> lstReciprocidadeAlerta, List<ReciprocidadeVO> lstReciprocidadeCritica)
        {
            EmailUtil.Reciprocidade.SendReciprocidadeAlerta(usuario, lstReciprocidadeAlerta, lstReciprocidadeCritica);
        }

        public DataTable BuscarEmAndamento()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                DataTable dt = ReciprocidadeDAO.BuscarEmAndamento(db);
                dt = RebuildPesquisaTable(dt);
                return dt;
            }
        }

        private DataTable RebuildPesquisaTable(DataTable dt)
        {
            if (dt != null)
            {
                DataColumn colObj = new DataColumn("OBJ", typeof(ReciprocidadeVO));
                dt.Columns.Add(colObj);

                foreach (DataRow dr in dt.Rows)
                {
                    ReciprocidadeVO vo = ReciprocidadeDAO.FromDataRow(dr);
                    dr["OBJ"] = vo;
                }
            }
            return dt;
        }

        public void AtualizarSituacao(int cdProtocolo, SituacaoReciprocidade situacao)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                ReciprocidadeDAO.AtualizarSituacao(cdProtocolo, situacao, db);
                connection.Commit();
            }
        }	
	
	}
}

