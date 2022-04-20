using eVidaGeneralLib.DAO;
using eVidaGeneralLib.DAO.Adesao;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Adesao;
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
	public class AdesaoBO {
		EVidaLog log = new EVidaLog(typeof(AdesaoBO));

		private static AdesaoBO instance = new AdesaoBO();

		public static AdesaoBO Instance { get { return instance; } }

		public DeclaracaoVO GetById(int id) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return DeclaracaoDAO.GetById(id, db);
			}
		}

		public DataTable Pesquisar(Dados.Empresa? empresa, int? numProposta, long? matricula, Dados.SituacaoDeclaracao? status) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return DeclaracaoDAO.Pesquisar(empresa, numProposta, matricula, status, db);
			}
		}

		public DataTable BuscarResumo() {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				return DeclaracaoDAO.BuscarResumo(db);
			}
		}

		public void MarcarRecebida(int id) {
			Database db = DatabaseFactory.CreateDatabase();
			using (DbConnection connection = db.CreateConnection()) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();
				try {
					DeclaracaoDAO.MarcarRecebida(id, transaction, db);
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				} finally {
					connection.Close();
				}
			}
		}

		public void MarcarValidada(int id, bool isValido, string motivo, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();
				DeclaracaoDAO.MarcarValidada(id, isValido, motivo, db);

				DeclaracaoVO vo = DeclaracaoDAO.GetById(id, db);

				EmailUtil.Adesao.SendAdesaoValidacao(vo, isValido, motivo);
				connection.Commit();
			}
		}

		private bool IsSameDependente(DeclaracaoDependenteVO declDepVO, HcDependenteVO depVO) {
			if (!declDepVO.Nome.Equals(depVO.NmDependente, StringComparison.InvariantCultureIgnoreCase)) {
				return false;
			}
			if (!string.IsNullOrEmpty(declDepVO.Cpf) && depVO.NrCpf != null) {
				long cpf = Int64.Parse(declDepVO.Cpf);
				if (cpf != depVO.NrCpf.Value) {
					return false;
				}
			}
			if (!string.IsNullOrEmpty(declDepVO.Rg) && !string.IsNullOrEmpty(depVO.NrRg)) {
				if (!declDepVO.Rg.Trim().Equals(depVO.NrRg.Trim()))
					return false;
			}
			return true;
		}
		
		public void FillBenefCorrespondente(List<DeclaracaoDependenteVO> lstDependentes, List<HcBeneficiarioVO> lstBenefs, List<HcDependenteVO> lstDeps) {
			if (lstDependentes != null && lstDependentes.Count > 0) {
				foreach (DeclaracaoDependenteVO declDepVO in lstDependentes) {
					HcBeneficiarioVO benefCorrespondenteVO = null;
					HcDependenteVO depCorrespondenteVO = null;

					string grauParentesco = declDepVO.Parentesco.ToISA(declDepVO.Sexo);

					if (string.IsNullOrEmpty(grauParentesco)) {
						throw new EvidaException("Não foi possível mapear o parentesco para o ISA: " + declDepVO.Parentesco);
					}

					if (lstDeps != null) {
						foreach (HcDependenteVO depVO in lstDeps) {
							if (IsSameDependente(declDepVO, depVO)) {
								depCorrespondenteVO = depVO;
								break;
							}
						}
					}
					if (depCorrespondenteVO != null) {
						if (lstBenefs != null) {
							foreach (HcBeneficiarioVO benefVO in lstBenefs) {
								if (benefVO.CdDependente != null && benefVO.CdDependente.Value == depCorrespondenteVO.CdDependente) {
									benefCorrespondenteVO = benefVO;
									break;
								}
							}
						}
					} else {
						depCorrespondenteVO = new HcDependenteVO() {
							NmDependente = declDepVO.Nome,
							CdGrauParentesco = grauParentesco,
							TpDependente = "N"
						};
					}
					
					if (benefCorrespondenteVO == null) {
						benefCorrespondenteVO = new HcBeneficiarioVO() {
							NmBeneficiario = declDepVO.Nome,
							CdGrauParentesco = grauParentesco,
							TpBeneficiario = Constantes.TIPO_BENEFICIARIO_DEPENDENTE							
						};
					}
					/*
					if (!grauParentesco.Equals(benefCorrespondenteVO.CdGrauParentesco, StringComparison.InvariantCultureIgnoreCase)) {
						throw new EvidaException("O parentesco do beneficiário " + benefCorrespondenteVO.NmBeneficiario + " não corresponde no ISA!");
					}
					if (!grauParentesco.Equals(depCorrespondenteVO.CdGrauParentesco, StringComparison.InvariantCultureIgnoreCase)) {
						throw new EvidaException("O parentesco do dependente " + depCorrespondenteVO.NmDependente + " não corresponde no ISA!");
					}
					*/
					benefCorrespondenteVO.NrCns = declDepVO.Cns;
					
					//sexo, data de nascimento, estado civil, nome pai, mae, cpf, rg
					depCorrespondenteVO.DtNascimento = benefCorrespondenteVO.DtNascimento = declDepVO.Nascimento;
					depCorrespondenteVO.TpSexo = benefCorrespondenteVO.TpSexo = declDepVO.Sexo == Dados.Sexo.FEMININO ? Constantes.SEXO_FEMININO : Constantes.SEXO_MASCULINO;
					depCorrespondenteVO.NmMae = benefCorrespondenteVO.NmMae = declDepVO.NomeMae;
					depCorrespondenteVO.TpEstadoCivil = declDepVO.Parentesco.IsConjuge() ? Dados.DeclaracaoEstadoCivil.CASADO.ToISA() : Constantes.TP_ESTADO_CIVIL_NAO_INFORMADO;

					depCorrespondenteVO.NmPai = ValidateUtil.Nvl(declDepVO.NomePai, depCorrespondenteVO.NmPai);
					depCorrespondenteVO.NrRg = ValidateUtil.Nvl(declDepVO.Rg, depCorrespondenteVO.NrRg);
					depCorrespondenteVO.CdUfOrgExpRg = ValidateUtil.Nvl(declDepVO.UfOrgaoExpedidor, depCorrespondenteVO.CdUfOrgExpRg);
					depCorrespondenteVO.DsOrgExpRg = ValidateUtil.Nvl(declDepVO.OrgaoExpedidor, depCorrespondenteVO.DsOrgExpRg);
					depCorrespondenteVO.DataEmissaoRg = ValidateUtil.Nvl(declDepVO.DataEmissaoRg, depCorrespondenteVO.DataEmissaoRg);

					if (!string.IsNullOrEmpty(declDepVO.Cpf)) {
						depCorrespondenteVO.NrCpf = benefCorrespondenteVO.NrCpf = Int64.Parse(declDepVO.Cpf);
					}
					declDepVO.BeneficiarioCorrespondente = benefCorrespondenteVO;
					declDepVO.DependenteCorrespondente = depCorrespondenteVO;
				}
			}
		}

		public IntegracaoAdesaoVO ValidarIntegracao(int numProposta, DateTime dtInicio, int cdCategoria, string idPlano, int? cdMotivoDesligamento, string lotacao, string tpCarencia) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				IntegracaoAdesaoVO retornoVO = new IntegracaoAdesaoVO();
				DeclaracaoVO declaracaoVO = DeclaracaoDAO.GetById(numProposta, db);

				if (declaracaoVO == null)
					throw new EvidaException("Declaracao não encontrada!");

				retornoVO.Declaracao = declaracaoVO;

				HcPlanoVO plano = LocatorDataBO.Instance.GetPlano(idPlano, db);

				if (plano == null) {
					throw new EvidaException("Plano inválido para integração: " + declaracaoVO.Plano.Descricao);
				}

				HcFuncionarioVO funcionario = DAO.HC.HcFuncionarioDAO.GetByMatricula((int)declaracaoVO.Empresa, declaracaoVO.Titular.Matricula, db);
				bool newFunc = funcionario == null;
				List<HcBeneficiarioVO> lstOldBenefs = null;
				List<HcDependenteVO> lstOldDeps = new List<HcDependenteVO>();
				HcBeneficiarioVO benefTitular = null;

				if (newFunc) {
					// novo funcionario
					funcionario = new HcFuncionarioVO() {
						Admissao = declaracaoVO.Titular.Admissao.Value,
						CdEmpresa = (int)declaracaoVO.Empresa,
						CdFuncionario = declaracaoVO.Titular.Matricula
					};

				} else {
					// funcionario existente
					if (funcionario.CdEmpresa != (int)declaracaoVO.Empresa) {
						throw new EvidaException("A empresa do funcionário é diferente da empresa do formulário!");
					}

					lstOldBenefs = DAO.HC.HcVBeneficiarioDAO.ListarBeneficiarios(funcionario.CdEmpresa, funcionario.CdFuncionario, db);
					lstOldDeps = DAO.HC.HcDependenteDAO.ListarDependentes(funcionario, db);

					if (lstOldBenefs != null)
						benefTitular = lstOldBenefs.Find(x => x.TpBeneficiario.Equals(Constantes.TIPO_BENEFICIARIO_FUNCIONARIO));

				}
				if (funcionario.CdEmpresaResponsavel == null || funcionario.CdEmpresaResponsavel.Value == 0) {
					if (Constantes.EMPRESAS_ADESAO_ESP.Contains(funcionario.CdEmpresa))
						funcionario.CdEmpresaResponsavel = funcionario.CdEmpresa;
					else
						funcionario.CdEmpresaResponsavel = Constantes.EMPRESA_ELETRONOTE;
				}


				retornoVO.Funcionario = funcionario;
				funcionario.Sexo = ValidateUtil.Nvl(funcionario.Sexo, declaracaoVO.Titular.Sexo == Dados.Sexo.MASCULINO ? Constantes.SEXO_MASCULINO : Constantes.SEXO_FEMININO);
				funcionario.Cpf = Int64.Parse(declaracaoVO.Titular.Cpf);
				funcionario.Email = declaracaoVO.Titular.Email;
				funcionario.Nascimento = declaracaoVO.Titular.Nascimento;
				funcionario.Nome = string.IsNullOrEmpty(funcionario.Nome) ? declaracaoVO.Titular.Nome : funcionario.Nome;
				funcionario.NomeMae = declaracaoVO.Titular.NomeMae;
				funcionario.NomePai = ValidateUtil.Nvl(declaracaoVO.Titular.NomePai, funcionario.NomePai);
				funcionario.Rg = ValidateUtil.Nvl(declaracaoVO.Titular.Rg, funcionario.Rg);
				funcionario.OrgaoExpedidorRg = declaracaoVO.Titular.OrgaoExpedidor;
				funcionario.UfOrgaoExpedidorRg = declaracaoVO.Titular.UfOrgaoExpedidor;
				funcionario.DataEmissaoRg = declaracaoVO.Titular.DataEmissaoRg;

				funcionario.TipoEstadoCivil = declaracaoVO.Titular.EstadoCivil.ToISA();
				if (string.IsNullOrEmpty(funcionario.TipoEstadoCivil) || funcionario.TipoEstadoCivil.Equals(Constantes.TP_ESTADO_CIVIL_NAO_INFORMADO)) { // 6- NÃO INFORMADO
					throw new EvidaException("Estado Civil inválido para o titular: " + funcionario.TipoEstadoCivil);
				}
				if (string.IsNullOrEmpty(funcionario.Sexo)) {
					throw new EvidaException("Erro ao selecionar o sexo: " + declaracaoVO.Titular.Sexo);
				}
				funcionario.CdLotacao = lotacao;
				if (string.IsNullOrEmpty(plano.LocalAtendPadrao)) {
					//throw new EvidaException("O plano " + plano.CdPlano + " não possui local de atendimento padrão configurado!");
				} else
					funcionario.CdLocal = Convert.ToInt32(plano.LocalAtendPadrao);

				string[] telefone = null;
				if (!string.IsNullOrEmpty(declaracaoVO.TelCelular)) {
					telefone = ValidateUtil.TrySplitTelefone(declaracaoVO.TelCelular);
					funcionario.DddTelCelular = telefone[0];
					funcionario.TelCelular = telefone[1];
				}
				if (!string.IsNullOrEmpty(declaracaoVO.TelComercial)) {
					telefone = ValidateUtil.TrySplitTelefone(declaracaoVO.TelComercial);
					funcionario.DddTelComercial = telefone[0];
					funcionario.TelComercial = telefone[1];
				}
				if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial)) {
					telefone = ValidateUtil.TrySplitTelefone(declaracaoVO.TelResidencial);
					funcionario.DddTelResidencial = telefone[0];
					funcionario.TelResidencial = telefone[1];
				}

				if (declaracaoVO.DadosBancarios != null) {
					DadosBancariosVO dBancVO = declaracaoVO.DadosBancarios;
					funcionario.CdAgencia = ValidateUtil.Nvl(dBancVO.Agencia + dBancVO.DVAgencia, funcionario.CdAgencia);
					funcionario.CdBanco = FormatUtil.TryParseNullableInt(dBancVO.IdBanco);
					funcionario.ContaBancaria = dBancVO.Conta;
					funcionario.DvContaBancaria = dBancVO.DVConta;
					if (funcionario.CdBanco != null && !string.IsNullOrEmpty(dBancVO.Agencia)) {
						HcBancoVO bancoVO = LocatorDAO.GetBancoISA(funcionario.CdBanco.Value, db);
						if (bancoVO == null) {
							throw new EvidaException("Banco de código " + funcionario.CdBanco + " não encontrado no ISA.");
						}
						HcAgenciaBancariaVO agencia = LocatorDAO.GetAgenciaBancoISA(funcionario.CdBanco.Value, dBancVO.Agencia, db);
						if (agencia == null) {
							throw new EvidaException("Não foi possível encontrar a agência (CD_AGENCIA) " + dBancVO.Agencia + " do banco (" + bancoVO.Id + ") " + bancoVO.Nome + " no ISA!");
						}
						funcionario.CdAgencia = agencia.CdAgencia + agencia.DvAgencia;
					}
				}

				AdesaoEnderecoVO enderecoVO = declaracaoVO.Endereco;
				funcionario.Endereco = ConvertEndereco(enderecoVO, db);
				funcionario.EnderecoCob = funcionario.Endereco;

				if (declaracaoVO.Dependentes != null && declaracaoVO.Dependentes.Count > 0) {
					FillBenefCorrespondente(declaracaoVO.Dependentes, lstOldBenefs, lstOldDeps);
				}
				if (benefTitular != null) {
					HcBeneficiarioCategoriaVO benefCatTitularVO = DAO.HC.HcBeneficiarioCategoriaDAO.GetLastBeneficiarioData(benefTitular.CdBeneficiario, null, db);
					if (benefCatTitularVO != null && benefCatTitularVO.InicioVigencia >= dtInicio) {
						throw new EvidaException("O titular " + benefTitular.NmBeneficiario + " possui uma categoria com data futura!");
					}
					HcBeneficiarioPlanoVO benefPlanoTitularVO = DAO.HC.HcBeneficiarioPlanoDAO.GetLastBeneficiarioData(benefTitular.CdBeneficiario, null, db);
					if (benefPlanoTitularVO != null && benefPlanoTitularVO.InicioVigencia >= dtInicio) {
						throw new EvidaException("O titular " + benefTitular.NmBeneficiario + " possui um plano com data futura!");
					}
					retornoVO.BeneficiarioPlanoCorrespondente = benefPlanoTitularVO;

					if (declaracaoVO.Dependentes != null) {
						foreach (DeclaracaoDependenteVO declDepVO in declaracaoVO.Dependentes) {
							HcBeneficiarioVO benefVO = declDepVO.BeneficiarioCorrespondente;
							if (benefVO.CdBeneficiario != 0) {
								HcBeneficiarioCategoriaVO benefCatVO = DAO.HC.HcBeneficiarioCategoriaDAO.GetLastBeneficiarioData(benefVO.CdBeneficiario, null, db);
								if (benefCatVO.InicioVigencia >= dtInicio) {
									throw new EvidaException("O beneficiário " + benefVO.NmBeneficiario + " possui uma categoria com data futura!");
								}
								HcBeneficiarioPlanoVO benefPlanoVO = DAO.HC.HcBeneficiarioPlanoDAO.GetLastBeneficiarioData(benefVO.CdBeneficiario, null, db);
								if (benefPlanoVO != null && benefPlanoVO.InicioVigencia >= dtInicio) {
									throw new EvidaException("O beneficiário " + benefVO.NmBeneficiario + " possui um plano com data futura!");
								}
								declDepVO.BeneficiarioPlanoCorrespondente = benefPlanoVO;
							}
						}
					}
				}

				CheckCns(declaracaoVO.Titular, benefTitular, declaracaoVO.Dependentes, db);

				HcBeneficiarioCategoriaVO categoriaForm = new HcBeneficiarioCategoriaVO() {
					CdCategoria = cdCategoria,
					InicioVigencia = dtInicio
				};
				HcBeneficiarioPlanoVO planoForm = new HcBeneficiarioPlanoVO() {
					CdPlanoVinculado = plano.CdPlano,
					InicioVigencia = dtInicio,
					TpPlano = plano.TpPlano,
					TpSistemaAtend = HcBeneficiarioPlanoVO.SIST_ATEND_CREDREEMB,
					TpFundoReserva = HcBeneficiarioPlanoVO.FUNDO_RESERVA_ISENTO,
					FlTemSubsidio = "N",
					CdMotivoDesligamento = cdMotivoDesligamento,
					CdPlanoEmpresa = plano.CdPlano
				};

				if (tpCarencia.Equals(HcBeneficiarioPlanoVO.CARENCIA_NORMAL)) {
					planoForm.TpCarencia = HcBeneficiarioPlanoVO.CARENCIA_NORMAL;
					planoForm.Observacao = "INÍCIO DO PLANO: " + dtInicio.ToShortDateString() +
						" COBERTURA ROL ANS - TÉRMINO CARÊNCIA: URG/EMERGÊNCIA: " + dtInicio.AddDays(1).ToShortDateString() +
						" PARTO: " + dtInicio.AddDays(360).ToShortDateString() + " DEMAIS PROCEDIMENTOS: " + dtInicio.AddDays(180).ToShortDateString();
				} else {
					planoForm.TpCarencia = HcBeneficiarioPlanoVO.CARENCIA_ISENTO;
					planoForm.Observacao = "COBERTURA - ROL DE PROCEDIMENTOS E EVENTOS DA ANS";
				}

				retornoVO.PlanoIntegracao = planoForm;
				retornoVO.CategoriaIntegracao = categoriaForm;

				return retornoVO;

			}
		}

		private void CheckCns(PessoaVO decTitular, HcBeneficiarioVO titularIsa, List<DeclaracaoDependenteVO> lstDependentes, EvidaDatabase db) {
			ValidateDuplicatedCns(decTitular.Cns, titularIsa, db);
			if (lstDependentes != null) {
				foreach (DeclaracaoDependenteVO dep in lstDependentes) {
					ValidateDuplicatedCns(dep.Cns, dep.BeneficiarioCorrespondente, db);
				}
			}
		}

		private void ValidateDuplicatedCns(string cns, HcBeneficiarioVO benefIsa, EvidaDatabase db) {
			HcBeneficiarioVO benefCns = null;
			if (!string.IsNullOrEmpty(cns))
				benefCns = DAO.HC.HcVBeneficiarioDAO.GetByCns(cns, db);

			if (benefCns != null) {
				if (benefIsa != null && benefIsa.CdBeneficiario != 0) { // benef já existe
					if (benefCns.CdBeneficiario != benefIsa.CdBeneficiario) {
						throw new EvidaException("O CNS " + cns + " já é utilizado pelo beneficiario " + benefCns.CdAlternativo);
					}
				} else {
					throw new EvidaException("O CNS " + cns + " já é utilizado pelo beneficiario " + benefCns.CdAlternativo);
				}
			}
		}

		private EnderecoVO ConvertEndereco(AdesaoEnderecoVO enderecoVO, EvidaDatabase db) {
			EnderecoVO vo = new EnderecoVO();
			vo.Bairro = enderecoVO.Bairro;
			vo.Cep = Int32.Parse(FormatUtil.UnformatCep(enderecoVO.Cep));
			vo.Cidade = enderecoVO.Cidade;
			vo.Complemento = enderecoVO.Complemento;
			vo.NumeroEndereco = enderecoVO.NumeroEndereco;
			vo.Rua = enderecoVO.Rua;
			vo.Uf = enderecoVO.Uf;

			int? idLocalidade = LocatorDAO.GetCdMunicipioFromAdesao(enderecoVO.Cidade, enderecoVO.Uf, db);
			if (idLocalidade == null)
				throw new EvidaException("Não foi possivel encontrar a localidade no ISA para cidade " + enderecoVO.Cidade + " - " + enderecoVO.Uf);
			vo.IdLocalidade = idLocalidade.Value;
			return vo;
		}
		
		public void RealizarIntegracao(IntegracaoAdesaoVO integracao, int idUsuario) {
			using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null)) {
				EvidaConnectionHolder connection = db.CreateConnection();
				connection.CreateTransaction();

				HcFuncionarioVO funcionario = integracao.Funcionario;
				HcFuncionarioVO oldFunc = DAO.HC.HcFuncionarioDAO.GetByMatricula(funcionario.CdEmpresa, funcionario.CdFuncionario, db);

				// Se o funcionario não existe, então cria
				bool newFuncionario = oldFunc == null;

				if (newFuncionario) {
					DAO.HC.HcFuncionarioDAO.CriarFuncionario(funcionario, db);
				} else {
					DAO.HC.HcFuncionarioDAO.AlterarFuncionario(funcionario, db);
				}

				List<HcBeneficiarioVO> lstOldBenef = DAO.HC.HcVBeneficiarioDAO.ListarBeneficiarios(funcionario.CdEmpresa, funcionario.CdFuncionario, db);
				List<HcDependenteVO> lstOldDependentes = DAO.HC.HcDependenteDAO.ListarDependentes(funcionario, db);

				integracao.Titular = null;
				if (newFuncionario) {


				} else {
					if (lstOldBenef != null)
						integracao.Titular = lstOldBenef.Find(x => x.TpBeneficiario == Constantes.TIPO_BENEFICIARIO_FUNCIONARIO);
				}

				if (integracao.Titular == null) {
					integracao.Titular = new HcBeneficiarioVO() {
						TpBeneficiario = Constantes.TIPO_BENEFICIARIO_FUNCIONARIO,
						CdEmpresa = funcionario.CdEmpresa,
						CdFuncionario = funcionario.CdFuncionario,
						DtAdmissao = funcionario.Admissao,
						CdSituacaoBenef = Constantes.SITUACAO_BENEFICIARIO_ATIVO,
						CdAlternativo = BuildCdAlternativo(funcionario, null),
						CdDependente = null,
					};
				}
				integracao.Titular.NrCns = ValidateUtil.Nvl(integracao.Declaracao.Titular.Cns, integracao.Titular.NrCns);
				bool usePlanoEmpresa = integracao.CategoriaIntegracao.CdCategoria != Constantes.CATEGORIA_COMISSIONADO;

				CriarBeneficiario(integracao.Titular, integracao.CategoriaIntegracao, integracao.PlanoIntegracao, usePlanoEmpresa, db);

				// Se não há beneficiários para o funcionario, então cria
				List<DeclaracaoDependenteVO> lstDependentes = integracao.Declaracao.Dependentes;
				if (lstDependentes != null) {
					BuildCdDependente(lstDependentes, lstOldDependentes);
					foreach (DeclaracaoDependenteVO declDep in lstDependentes) {
						HcDependenteVO dep = declDep.DependenteCorrespondente;
						if (dep.IsNew) {
							dep.CdEmpresa = funcionario.CdEmpresa;
							dep.CdFuncionario = funcionario.CdFuncionario;
							dep.DtAdmissao = funcionario.Admissao;
							DAO.HC.HcDependenteDAO.CriarDependente(dep, db);

						} else {
							DAO.HC.HcDependenteDAO.AtualizarDependente(dep, db);
						}

						HcBeneficiarioVO benef = declDep.BeneficiarioCorrespondente;
						benef.CdDependente = dep.CdDependente;

						benef.CdEmpresa = funcionario.CdEmpresa;
						benef.CdFuncionario = funcionario.CdFuncionario;
						benef.DtAdmissao = funcionario.Admissao;

						benef.CdBeneficiarioTitular = integracao.Titular.CdBeneficiario;
						benef.CdAlternativo = BuildCdAlternativo(funcionario, dep.CdDependente);
						benef.CdSituacaoBenef = Constantes.SITUACAO_BENEFICIARIO_ATIVO;
						benef.TpBeneficiario = Constantes.TIPO_BENEFICIARIO_DEPENDENTE;

						bool isEnteado = Dados.DeclaracaoParentesco.ENTEADO.ToISA(dep.TpSexo.Equals(Constantes.SEXO_MASCULINO) ? Dados.Sexo.MASCULINO : Dados.Sexo.FEMININO).Equals(dep.CdGrauParentesco);

						CriarBeneficiario(benef, integracao.CategoriaIntegracao, integracao.PlanoIntegracao, usePlanoEmpresa && !isEnteado, db);
					}
				}
				DeclaracaoDAO.MarcarIntegrada(integracao.Declaracao.Numero, idUsuario, funcionario.CdLotacao, integracao.CategoriaIntegracao, integracao.PlanoIntegracao, db);
				connection.Commit();
			}
		}

		private void CriarBeneficiario(HcBeneficiarioVO benef, HcBeneficiarioCategoriaVO categoriaIntegracao, HcBeneficiarioPlanoVO planoIntegracao, bool usePlanoEmpresa, EvidaDatabase db) {
			if (benef.CdBeneficiario == 0)
				DAO.HC.HcVBeneficiarioDAO.CriarBeneficiario(benef, db);

			DAO.HC.HcVBeneficiarioDAO.CriarBenefRespFinanceiro(benef, categoriaIntegracao.InicioVigencia, db);
			DAO.HC.HcVBeneficiarioDAO.CriarBenefHistTit(benef, categoriaIntegracao.InicioVigencia, db);

			HcBeneficiarioCategoriaVO benefCategoriaVO = new HcBeneficiarioCategoriaVO();
			benefCategoriaVO.CdBeneficiario = benef.CdBeneficiario;
			benefCategoriaVO.CdCategoria = categoriaIntegracao.CdCategoria;
			benefCategoriaVO.InicioVigencia = categoriaIntegracao.InicioVigencia;

			DAO.HC.HcBeneficiarioCategoriaDAO.CriarBeneficiarioCategoria(benefCategoriaVO, db);

			HcBeneficiarioPlanoVO benefPlanoVO = new HcBeneficiarioPlanoVO();
			benefPlanoVO.CdBeneficiario = benef.CdBeneficiario;
			benefPlanoVO.CdPlanoVinculado = planoIntegracao.CdPlanoVinculado;
			benefPlanoVO.CdPlanoEmpresa = usePlanoEmpresa ? planoIntegracao.CdPlanoEmpresa : null;
			benefPlanoVO.TpPlano = planoIntegracao.TpPlano;
			benefPlanoVO.FlTemSubsidio = planoIntegracao.FlTemSubsidio;
			benefPlanoVO.InicioVigencia = planoIntegracao.InicioVigencia;
			benefPlanoVO.Observacao = planoIntegracao.Observacao;
			benefPlanoVO.TpCarencia = planoIntegracao.TpCarencia;
			benefPlanoVO.TpFundoReserva = planoIntegracao.TpFundoReserva;
			benefPlanoVO.TpSistemaAtend = planoIntegracao.TpSistemaAtend;
			benefPlanoVO.CdMotivoDesligamento = planoIntegracao.CdMotivoDesligamento;

			DAO.HC.HcBeneficiarioPlanoDAO.CriarBeneficiarioPlano(benefPlanoVO, db);
		}

		private void BuildCdDependente(List<DeclaracaoDependenteVO> lstDeclNew, List<HcDependenteVO> lstAllOld) {
			List<HcDependenteVO> lstAll = new List<HcDependenteVO>();
			IEnumerable<HcDependenteVO> lstNew = lstDeclNew.Select(x => x.DependenteCorrespondente);
			if (lstNew != null) lstAll.AddRange(lstNew);
			if (lstAllOld != null) lstAll.AddRange(lstAllOld.Where(x => lstNew.FirstOrDefault(y => y.CdDependente == x.CdDependente) == null));

			IEnumerable<HcDependenteVO> enumNew = lstNew.OrderBy(x => (x.DtNascimento != null ? x.DtNascimento.Value : DateTime.MaxValue));
			IEnumerable<HcDependenteVO> enumAll = lstAll.OrderBy(x => (x.DtNascimento != null ? x.DtNascimento.Value : DateTime.MaxValue));
			foreach (HcDependenteVO novo in enumNew) {
				if (novo.CdDependente != 0) continue;

				IEnumerable<HcDependenteVO> enumParentesco = enumAll.Where(x => x.CdDependente != 0 && x.CdGrauParentesco.Equals(novo.CdGrauParentesco));
				int maxId = 0;
				if (enumParentesco.Count() > 0)
					maxId = enumParentesco.Max(x => x.CdDependente);
				int nextId = GetSequencialByCdDependente(maxId + 1);
				string strNextId = novo.CdGrauParentesco + nextId.ToString("00");
				novo.CdDependente = Int32.Parse(strNextId);
				novo.IsNew = true;
			}
		}

		private int GetSequencialByCdDependente(int cdDependente) {
			string strId = cdDependente.ToString("0000");
			string lst2Digitos = strId.Substring(strId.Length - 2);
			return Int32.Parse(lst2Digitos);
		}

		private string BuildCdAlternativo(HcFuncionarioVO func, int? cdDependente) {
			//cd_alternativo = cod_empresa-mat-composicao (onde composicao é 2 dígitos Parentesco e 2 sequencial)
			string parteComum = func.CdEmpresa.ToString("00") + "-" + func.CdFuncionario + "-";
			string composicao = "0000";
			if (cdDependente != null) {
				composicao = cdDependente.Value.ToString("0000");
			}
			return parteComum + composicao;
		}

		public bool IsValidPlano(Dados.Empresa empresa, int idPlano) {			
			switch(empresa) {
				case Dados.Empresa.CEA: return (idPlano == Constantes.PLANO_MAIS_VIDA_CEA);
				case Dados.Empresa.AMAZONASGT: return idPlano == Constantes.PLANO_EVIDA_PPRS_AMAZONASGT;
				case Dados.Empresa.AMAZONASD: return idPlano == Constantes.PLANO_EVIDA_PPRS_AMAZONASD;

                case Dados.Empresa.EVIDA:
                    return (idPlano == Constantes.PLANO_MAIS_VIDA_EVIDA || idPlano == Constantes.PLANO_MAIS_VIDA_EVIDA_MASTER);

            }
            return false;
		}

		public bool IsValidForIntegracao(Dados.Empresa empresa, string produto) {
			Dados.Produto plano = Dados.Produto.Find(produto);
			if (plano.Empresa != empresa) return false;
			if (Dados.Produto.PLANOS_INTEGRACAO.Contains(plano)) return true;
			return false;
		}
	}
}
