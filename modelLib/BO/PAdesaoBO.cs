using eVidaGeneralLib.BO.Extensions;
using eVidaGeneralLib.BO.Protheus;
using eVidaGeneralLib.DAO.Adesao;
using eVidaGeneralLib.DAO.Protheus;
using eVidaGeneralLib.DAO.Util;
using eVidaGeneralLib.Exceptions;
using eVidaGeneralLib.Util;
using eVidaGeneralLib.VO;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Int64;

namespace eVidaGeneralLib.BO
{

    public class PAdesaoBO
    {
        EVidaLog log = new EVidaLog(typeof(PAdesaoBO));

        private static PAdesaoBO instance = new PAdesaoBO();

        public static PAdesaoBO Instance { get { return instance; } }

        public DataTable BuscarResumo()
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PDeclaracaoDAO.BuscarResumo(db);
            }
        }

        public DataTable Pesquisar(PDados.Empresa? empresa, int? numProposta, long? matricula, PDados.SituacaoDeclaracao? status)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PDeclaracaoDAO.Pesquisar(empresa, numProposta, matricula, status, db);
            }
        }

        public bool IsValidForIntegracao(PDados.Empresa empresa, string produto)
        {
            PDados.Produto plano = PDados.Produto.Find(produto);
            if (plano.Empresa != empresa) return false;
            if (PDados.Produto.PLANOS_INTEGRACAO.Contains(plano)) return true;
            return false;
        }

        public void MarcarRecebida(int id)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();
                try
                {
                    PDeclaracaoDAO.MarcarRecebida(id, transaction, db);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public PDeclaracaoVO GetById(int id)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                return PDeclaracaoDAO.GetById(id, db);
            }
        }

        public void MarcarValidada(int id, bool isValido, string motivo, int idUsuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();
                PDeclaracaoDAO.MarcarValidada(id, isValido, motivo, db);
                PDeclaracaoVO vo = PDeclaracaoDAO.GetById(id, db);
                EmailUtil.PAdesao.SendAdesaoValidacao(vo, isValido, motivo);
                connection.Commit();
            }
        }

        public void FillClienteCorrespondente(PIntegracaoAdesaoVO integracaoVO, PClienteVO cliente)
        {
            PDeclaracaoVO declaracaoVO = integracaoVO.Declaracao;

            PClienteVO clienteVO = null;
            bool flagNovoCliente = true;

            if (cliente != null)
            {
                clienteVO = cliente;
                flagNovoCliente = false;
            }
            else 
            {
                // Caso não tenha sido encontrado, cria um novo cliente
                clienteVO = new PClienteVO()
                {
                    Cgc = declaracaoVO.Titular.Cpf.Trim().ToUpper()
                };
            }

            #region[PREENCHIMENTO/ATUALIZAÇÃO DOS DADOS DO CLIENTE]

            clienteVO.Nome = declaracaoVO.Titular.Nome.Trim().ToUpper();
            clienteVO.Nreduz = declaracaoVO.Titular.Nome.Trim().ToUpper();
            clienteVO.Cep = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());
            clienteVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
            clienteVO.End = declaracaoVO.Endereco.Rua.Trim().ToUpper();
            clienteVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
            clienteVO.Complem = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
            clienteVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
            clienteVO.Est = declaracaoVO.Endereco.Uf.Trim().ToUpper();

            clienteVO.Codmun = "";
            clienteVO.Mun = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
            PCepVO cep = PLocatorDataBO.Instance.BuscarCepProtheus(clienteVO.Cep);
            if(cep != null){
                if(cep.Codmun != null){
                    clienteVO.Codmun = cep.Codmun;
                }
                if(cep.Mun != null){
                    clienteVO.Mun = cep.Mun;           
                }
            }

            string telefone;

            if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    clienteVO.Ddd = telefone.Substring(0, 2);
                    clienteVO.Tel = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    clienteVO.Ddd = telefone.Substring(0, 2);
                    clienteVO.Tel = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    clienteVO.Ddd = telefone.Substring(0, 2);
                    clienteVO.Tel = telefone.Substring(2, telefone.Length - 2);
                }
            }

            #endregion

            // Mapeamento de cliente
            integracaoVO.Cliente = clienteVO;
            integracaoVO.FlagNovoCliente = flagNovoCliente;
        }

        public void FillFornecedorTitular(PIntegracaoAdesaoVO integracaoVO, PFornecedorVO fornecedor)
        {
            PDeclaracaoVO declaracaoVO = integracaoVO.Declaracao;

            PFornecedorVO fornecedorVO = null;
            bool flagNovoFornecedor = true;

            if (fornecedor != null)
            {
                fornecedorVO = fornecedor;
                flagNovoFornecedor = false;
            }
            else
            {
                // Caso não tenha sido encontrado, cria um novo fornecedor
                fornecedorVO = new PFornecedorVO()
                {
                    Cgc = declaracaoVO.Titular.Cpf.Trim().ToUpper()
                };
            }

            #region[PREENCHIMENTO/ATUALIZAÇÃO DOS DADOS DO FORNECEDOR TITULAR]

            fornecedorVO.Nome = declaracaoVO.Titular.Nome.Trim().ToUpper();
            fornecedorVO.Nreduz = declaracaoVO.Titular.Nome.Trim().ToUpper();
            fornecedorVO.Cep = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());
            fornecedorVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
            fornecedorVO.End = declaracaoVO.Endereco.Rua.Trim().ToUpper();
            fornecedorVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
            fornecedorVO.Complem = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
            fornecedorVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
            fornecedorVO.Est = declaracaoVO.Endereco.Uf.Trim().ToUpper();

            fornecedorVO.Codmun = "";
            fornecedorVO.Mun = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
            PCepVO cep = PLocatorDataBO.Instance.BuscarCepProtheus(fornecedorVO.Cep);
            if (cep != null)
            {
                if (cep.Codmun != null)
                {
                    fornecedorVO.Codmun = cep.Codmun;
                }
                if (cep.Mun != null)
                {
                    fornecedorVO.Mun = cep.Mun;
                }
            }

            string telefone;

            if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    fornecedorVO.Ddd = telefone.Substring(0, 2);
                    fornecedorVO.Telres = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    fornecedorVO.Ddd = telefone.Substring(0, 2);
                    fornecedorVO.Tel = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    fornecedorVO.Ddd = telefone.Substring(0, 2);
                    fornecedorVO.Telcom = telefone.Substring(2, telefone.Length - 2);
                }
            }

            if (declaracaoVO.DadosBancarios != null)
            {
                PDadosBancariosVO dBancVO = declaracaoVO.DadosBancarios;
                fornecedorVO.Agencia = ValidateUtil.Nvl(dBancVO.Agencia.Trim().ToUpper() + dBancVO.DVAgencia.Trim().ToUpper(), fornecedorVO.Agencia);
                fornecedorVO.Banco = dBancVO.IdBanco;
                fornecedorVO.Numcon = ValidateUtil.Nvl(dBancVO.Conta.Trim().ToUpper() + dBancVO.DVConta.Trim().ToUpper(), fornecedorVO.Numcon);
            }

            #endregion

            // Mapeamento de cliente
            integracaoVO.FornecedorTitular = fornecedorVO;
            integracaoVO.FlagNovoFornecedorTitular = flagNovoFornecedor;
        }

        public void FillFornecedoresDependentes(PIntegracaoAdesaoVO integracaoVO, List<PFornecedorVO> lstFornecedoresDependentes) 
        {
            PDeclaracaoVO declaracaoVO = integracaoVO.Declaracao;
            List<PDeclaracaoDependenteVO> lstDependentes = declaracaoVO.Dependentes;

            if (lstDependentes != null && lstDependentes.Count > 0)
            {
                // Para cada dependente na declaração atual
                foreach (PDeclaracaoDependenteVO declDepVO in lstDependentes)
                {
                    PFornecedorVO fornecedorDependenteVO = null;
                    bool flagNovoFornecedorDependente = true;

                    if (lstFornecedoresDependentes != null)
                    {
                        // Para cada fornecedor no Protheus
                        foreach (PFornecedorVO fornecedorVO in lstFornecedoresDependentes)
                        {
                            // Se o fornecedor na declaração é o mesmo no Protheus
                            if (IsSameFornecedorDependente(declDepVO, fornecedorVO))
                            {
                                fornecedorDependenteVO = fornecedorVO;
                                flagNovoFornecedorDependente = false;
                                break;
                            }
                        }
                    }

                    if (fornecedorDependenteVO == null)
                    {
                        // Caso não tenha sido encontrado, cria um novo fornecedor
                        fornecedorDependenteVO = new PFornecedorVO()
                        {
                            Cgc = declDepVO.Cpf.Trim().ToUpper()
                        };
                    }

                    #region[PREENCHIMENTO/ATUALIZAÇÃO DOS DADOS DOS FORNECEDORES DEPENDENTES]

                    fornecedorDependenteVO.Nome = declDepVO.Nome.Trim().ToUpper();
                    fornecedorDependenteVO.Nreduz = declDepVO.Nome.Trim().ToUpper();
                    fornecedorDependenteVO.Cep = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());
                    fornecedorDependenteVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
                    fornecedorDependenteVO.End = declaracaoVO.Endereco.Rua.Trim().ToUpper();
                    fornecedorDependenteVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
                    fornecedorDependenteVO.Complem = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
                    fornecedorDependenteVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
                    fornecedorDependenteVO.Est = declaracaoVO.Endereco.Uf.Trim().ToUpper();

                    fornecedorDependenteVO.Codmun = "";
                    fornecedorDependenteVO.Mun = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
                    PCepVO cep = PLocatorDataBO.Instance.BuscarCepProtheus(fornecedorDependenteVO.Cep);
                    if (cep != null)
                    {
                        if (cep.Codmun != null)
                        {
                            fornecedorDependenteVO.Codmun = cep.Codmun;
                        }
                        if (cep.Mun != null)
                        {
                            fornecedorDependenteVO.Mun = cep.Mun;
                        }
                    }

                    string telefone;

                    if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            fornecedorDependenteVO.Ddd = telefone.Substring(0, 2);
                            fornecedorDependenteVO.Telres = telefone.Substring(2, telefone.Length - 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            fornecedorDependenteVO.Ddd = telefone.Substring(0, 2);
                            fornecedorDependenteVO.Tel = telefone.Substring(2, telefone.Length - 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            fornecedorDependenteVO.Ddd = telefone.Substring(0, 2);
                            fornecedorDependenteVO.Telcom = telefone.Substring(2, telefone.Length - 2);
                        }
                    }

                    if (declaracaoVO.DadosBancarios != null)
                    {
                        PDadosBancariosVO dBancVO = declaracaoVO.DadosBancarios;
                        fornecedorDependenteVO.Agencia = ValidateUtil.Nvl(dBancVO.Agencia.Trim().ToUpper() + dBancVO.DVAgencia.Trim().ToUpper(), fornecedorDependenteVO.Agencia);
                        fornecedorDependenteVO.Banco = dBancVO.IdBanco;
                        fornecedorDependenteVO.Numcon = ValidateUtil.Nvl(dBancVO.Conta.Trim().ToUpper() + dBancVO.DVConta.Trim().ToUpper(), fornecedorDependenteVO.Numcon);
                    }

                    #endregion


                    // Mapeamento de usuário e vida do dependente
                    declDepVO.FornecedorDependente = fornecedorDependenteVO;
                    declDepVO.FlagNovoFornecedorDependente = flagNovoFornecedorDependente;
                }
            }            
        }

        public void FillFaixaEtariaFormasFamilia(PIntegracaoAdesaoVO integracaoVO, List<PFaixaEtariaBeneficiarioVO> lstFaixas)
        {
            PDeclaracaoVO declaracaoVO = integracaoVO.Declaracao;
            List<PFaixaEtariaFormasFamiliaVO> lstFaixaEtaria = new List<PFaixaEtariaFormasFamiliaVO>();

            if (lstFaixas != null && lstFaixas.Count > 0)
            {
                // Para cada item
                foreach (PFaixaEtariaBeneficiarioVO item in lstFaixas)
                {
                    PFaixaEtariaFormasFamiliaVO faixaVO = new PFaixaEtariaFormasFamiliaVO()
                    {
                        Codope = item.Codigo.Substring(0, 4),
                        Codemp = item.Codigo.Substring(4, 4),
                        Codfor = item.Codfor,
                        Codfai = item.Codfai,
                        Sexo = item.Sexo,
                        Idaini = item.Idaini,
                        Idafin = item.Idafin,
                        Valfai = item.Valfai,
                        Faifam = item.Faifam,
                        Qtdmin = item.Qtdmin,
                        Qtdmax = item.Qtdmax,
                        Rejapl = item.Rejapl,
                        Automa = item.Automa,
                        Perrej = item.Perrej,
                        Anomes = item.Anomes,
                        Vlrant = item.Vlrant
                    };

                    lstFaixaEtaria.Add(faixaVO);
                }
            }

            integracaoVO.FaixasEtarias = lstFaixaEtaria;
        }

        public void FillTitularCorrespondente(PIntegracaoAdesaoVO integracaoVO, List<PUsuarioVO> lstOldUsuarios, List<PVidaVO> lstOldVidas, List<PVidaVO> lstCnsVidas, bool flagNovaFamilia, DateTime dtInicio, string tpCarencia)
        {
            PDeclaracaoVO declaracaoVO = integracaoVO.Declaracao;

            PUsuarioVO usuarioTitularVO = null;
            PVidaVO vidaTitularVO = null;
            bool flagNovoUsuarioTitular = true;
            bool flagNovaVidaTitular = true;

            if (lstOldVidas != null)
            {
                // Para cada vida da família no Protheus
                foreach (PVidaVO vidaVO in lstOldVidas)
                {
                    // Se o titular na declaração é a vida no Protheus
                    if (!IsSameVidaTitular(declaracaoVO, vidaVO))
                    {
                        vidaTitularVO = vidaVO;
                        flagNovaVidaTitular = false;
                        break;
                    }
                }
            }

            // Se não encontrou a vida do titular, busca pelo CNS
            if (vidaTitularVO == null)
            {
                if (lstCnsVidas != null)
                {
                    // Para cada vida
                    foreach (PVidaVO vidaVO in lstCnsVidas)
                    {
                        // Se o titular na declaração é a vida no Protheus
                        if (IsSameVidaTitularCns(declaracaoVO, vidaVO))
                        {
                            vidaTitularVO = vidaVO;
                            flagNovaVidaTitular = false;
                            break;
                        }
                    }
                }                
            }

            // Se o titular na declaração é a vida no Protheus
            if (vidaTitularVO != null)
            {
                if (flagNovaFamilia == false)
                {
                    if (lstOldUsuarios != null)
                    {
                        // Para cada usuário da família no Protheus
                        foreach (PUsuarioVO usuarioVO in lstOldUsuarios)
                        {
                            // Se a vida no Protheus já foi encontrada 
                            if (usuarioVO.Matvid != null && usuarioVO.Matvid.Trim() == vidaTitularVO.Matvid.Trim())
                            {
                                usuarioTitularVO = usuarioVO;
                                flagNovoUsuarioTitular = false;
                                break;
                            }

                        }
                    }
                }
            }
            else
            {
                // Caso não tenha sido encontrada, cria uma nova vida
                vidaTitularVO = new PVidaVO()
                {
                    Nomusr = declaracaoVO.Titular.Nome
                };
            }

            if (usuarioTitularVO == null)
            {
                // Caso não tenha sido encontrado, cria um novo usuário
                usuarioTitularVO = new PUsuarioVO()
                {
                    Nomusr = declaracaoVO.Titular.Nome,
                    Cpfusr = declaracaoVO.Titular.Cpf.Trim().ToUpper(),
                    Drgusr = declaracaoVO.Titular.Rg.Trim().ToUpper(),
                    //Graupa = grauParentesco.ToString(),
                    Tipusu = PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO,
                    Datinc = dtInicio.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
                };
            }

            #region[PREENCHIMENTO/ATUALIZAÇÃO DOS DADOS DO TITULAR]

            // Preenche os dados do usuário titular

            string telefone;

            if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    usuarioTitularVO.Ddd = telefone.Substring(0, 2);
                    usuarioTitularVO.Telres = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                if (telefone.Length >= 3) 
                {
                    usuarioTitularVO.Ddd = telefone.Substring(0, 2);
                    usuarioTitularVO.Telefo = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                if (telefone.Length >= 3) 
                {
                    usuarioTitularVO.Ddd = telefone.Substring(0, 2);
                    usuarioTitularVO.Telcom = telefone.Substring(2, telefone.Length - 2);
                }
            }

            usuarioTitularVO.Graupa = "01";
            int empresa = (int)declaracaoVO.Empresa;
            usuarioTitularVO.Codemp = empresa.ToString().PadLeft(4, '0');
            usuarioTitularVO.Conemp = integracaoVO.Familia.Conemp;
            usuarioTitularVO.Vercon = integracaoVO.Familia.Vercon;
            usuarioTitularVO.Subcon = integracaoVO.Familia.Subcon;
            usuarioTitularVO.Versub = integracaoVO.Familia.Versub;
            usuarioTitularVO.Nomusr = declaracaoVO.Titular.Nome.Trim().ToUpper();
            usuarioTitularVO.Sexo = ValidateUtil.Nvl(usuarioTitularVO.Sexo, declaracaoVO.Titular.Sexo == PDados.Sexo.MASCULINO ? PConstantes.SEXO_MASCULINO : PConstantes.SEXO_FEMININO);
            if (string.IsNullOrEmpty(usuarioTitularVO.Sexo))
            {
                throw new EvidaException("Erro ao selecionar o sexo: " + declaracaoVO.Titular.Sexo);
            }
            usuarioTitularVO.Datnas = declaracaoVO.Titular.Nascimento.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            usuarioTitularVO.Cpfusr = declaracaoVO.Titular.Cpf.Trim().ToUpper();
            usuarioTitularVO.Drgusr = declaracaoVO.Titular.Rg.Trim().ToUpper();
            usuarioTitularVO.Pai = declaracaoVO.Titular.NomePai.Trim().ToUpper();
            usuarioTitularVO.Mae = declaracaoVO.Titular.NomeMae.Trim().ToUpper();
            usuarioTitularVO.Orgem = declaracaoVO.Titular.OrgaoExpedidor.Trim().ToUpper();
            usuarioTitularVO.Codint = declaracaoVO.Titular.Codint;
            usuarioTitularVO.Codemp = declaracaoVO.Titular.Codemp;
            usuarioTitularVO.Matemp = declaracaoVO.Titular.Matemp;
            usuarioTitularVO.Tipreg = declaracaoVO.Titular.Tipreg;
            if (declaracaoVO.Titular.Admissao != null)
            {
                DateTime admissao = (DateTime)declaracaoVO.Titular.Admissao;
                usuarioTitularVO.Datadm = admissao.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            }
            usuarioTitularVO.Datcar = dtInicio.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            usuarioTitularVO.Estciv = declaracaoVO.Titular.EstadoCivil.ToProtheus();
            if (string.IsNullOrEmpty(usuarioTitularVO.Estciv) || usuarioTitularVO.Estciv.Equals(Constantes.TP_ESTADO_CIVIL_NAO_INFORMADO))
            { // 6- NÃO INFORMADO
                throw new EvidaException("Estado Civil inválido para o titular: " + usuarioTitularVO.Estciv);
            }
            usuarioTitularVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
            usuarioTitularVO.Endere = declaracaoVO.Endereco.Rua.Trim().ToUpper();
            usuarioTitularVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
            usuarioTitularVO.Comend = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
            usuarioTitularVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
            usuarioTitularVO.Munici = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
            usuarioTitularVO.Estado = declaracaoVO.Endereco.Uf.Trim().ToUpper();
            usuarioTitularVO.Cepusr = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());
            usuarioTitularVO.Ycaren = tpCarencia == "NOR" ? "N" : "I";

            if (lstOldUsuarios != null)
            {
                // Para cada usuário da família no Protheus
                foreach (PUsuarioVO usuarioVO in lstOldUsuarios)
                {
                    // Se o titular na declaração é o usuário no Protheus
                    if (IsSameUsuarioTitular(declaracaoVO, usuarioVO))
                    {
                        usuarioTitularVO.Matant = usuarioVO.Matant;
                        usuarioTitularVO.Ycdleg = usuarioVO.Ycdleg;
                        usuarioTitularVO.Codmun = usuarioVO.Codmun;
                    }
                }                
            }

            PCepVO cep = PLocatorDataBO.Instance.BuscarCepProtheus(usuarioTitularVO.Cepusr);
            if (cep != null)
            {
                if (cep.Codmun != null)
                {
                    usuarioTitularVO.Codmun = cep.Codmun;
                }
                if (cep.Mun != null)
                {
                    usuarioTitularVO.Munici = cep.Mun;
                }
            }

            // Preenche os dados da vida titular

            if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    vidaTitularVO.Ddd = telefone.Substring(0, 2);
                    vidaTitularVO.Telres = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    vidaTitularVO.Ddd = telefone.Substring(0, 2);
                    vidaTitularVO.Telefo = telefone.Substring(2, telefone.Length - 2);
                }
            }
            if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
            {
                telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                if (telefone.Length >= 3)
                {
                    vidaTitularVO.Ddd = telefone.Substring(0, 2);
                    vidaTitularVO.Telcom = telefone.Substring(2, telefone.Length - 2);
                }
            }

            vidaTitularVO.Nomusr = declaracaoVO.Titular.Nome.Trim().ToUpper();
            vidaTitularVO.Nomcar = declaracaoVO.Titular.Nome.Trim().ToUpper();
            vidaTitularVO.Sexo = ValidateUtil.Nvl(vidaTitularVO.Sexo, declaracaoVO.Titular.Sexo == PDados.Sexo.MASCULINO ? PConstantes.SEXO_MASCULINO : PConstantes.SEXO_FEMININO);
            if (string.IsNullOrEmpty(vidaTitularVO.Sexo))
            {
                throw new EvidaException("Erro ao selecionar o sexo: " + declaracaoVO.Titular.Sexo);
            }
            vidaTitularVO.Datnas = declaracaoVO.Titular.Nascimento.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            vidaTitularVO.Cpfusr = declaracaoVO.Titular.Cpf.Trim().ToUpper();
            vidaTitularVO.Drgusr = declaracaoVO.Titular.Rg.Trim().ToUpper();
            vidaTitularVO.Rgest = declaracaoVO.Titular.UfOrgaoExpedidor.Trim().ToUpper();
            vidaTitularVO.Pai = declaracaoVO.Titular.NomePai.Trim().ToUpper();
            vidaTitularVO.Mae = declaracaoVO.Titular.NomeMae.Trim().ToUpper();
            vidaTitularVO.Orgem = declaracaoVO.Titular.OrgaoExpedidor.Trim().ToUpper();
            vidaTitularVO.Estciv = declaracaoVO.Titular.EstadoCivil.ToProtheus();
            if (string.IsNullOrEmpty(vidaTitularVO.Estciv) || vidaTitularVO.Estciv.Equals(Constantes.TP_ESTADO_CIVIL_NAO_INFORMADO))
            { // 6- NÃO INFORMADO
                throw new EvidaException("Estado Civil inválido para o titular: " + vidaTitularVO.Estciv);
            }
            vidaTitularVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
            vidaTitularVO.Nrcrna = declaracaoVO.Titular.Cns.Trim().ToUpper();
            vidaTitularVO.Endere = declaracaoVO.Endereco.Rua.Trim().ToUpper();
            vidaTitularVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
            vidaTitularVO.Comend = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
            vidaTitularVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
            vidaTitularVO.Munici = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
            vidaTitularVO.Estado = declaracaoVO.Endereco.Uf.Trim().ToUpper();
            vidaTitularVO.Cepusr = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());

            cep = PLocatorDataBO.Instance.BuscarCepProtheus(vidaTitularVO.Cepusr);
            if (cep != null)
            {
                if (cep.Codmun != null)
                {
                    vidaTitularVO.Codmun = cep.Codmun;
                }
                if (cep.Mun != null)
                {
                    vidaTitularVO.Munici = cep.Mun;
                }
            }

            #endregion

            // Mapeamento de usuário e vida do dependente
            integracaoVO.UsuarioTitular = usuarioTitularVO;
            integracaoVO.VidaTitular = vidaTitularVO;
            integracaoVO.FlagNovoUsuarioTitular = flagNovoUsuarioTitular;
            integracaoVO.FlagNovaVidaTitular = flagNovaVidaTitular;

        }
        
        // Mapeia os beneficiários e dependentes da declaração atual para os do Protheus
        // lstDependentes = Lista de dependentes na declaração atual
        // lstOldUsuarios = Lista de usuários da família no Protheus
        // lstOldVidas = Lista de vidas da família no Protheus
        public void FillDependentesCorrespondentes(PIntegracaoAdesaoVO integracaoVO, List<PUsuarioVO> lstOldUsuarios, List<PVidaVO> lstOldVidas, List<PVidaVO> lstCnsVidas, bool flagNovaFamilia, DateTime dtInicio, string tpCarencia)
        {
            PDeclaracaoVO declaracaoVO = integracaoVO.Declaracao;
            List<PDeclaracaoDependenteVO> lstDependentes = declaracaoVO.Dependentes;

            if (lstDependentes != null && lstDependentes.Count > 0)
            {
                // Para cada dependente na declaração atual
                foreach (PDeclaracaoDependenteVO declDepVO in lstDependentes)
                {
                    PUsuarioVO usuarioDependenteVO = null;
                    PVidaVO vidaDependenteVO = null;
                    bool flagNovoUsuarioDependente = true;
                    bool flagNovaVidaDependente = true;

                    int grauParentesco = declDepVO.Parentesco.ToProtheus();
                    if (grauParentesco == 0)
                    {
                        throw new EvidaException("Não foi possível mapear o parentesco para o Protheus: " + declDepVO.Parentesco);
                    }

                    if (lstOldVidas != null)
                    {
                        // Para cada vida da família no Protheus
                        foreach (PVidaVO vidaVO in lstOldVidas)
                        {
                            // Se o dependente na declaração é o mesmo no Protheus
                            if (IsSameVidaDependente(declDepVO, vidaVO))
                            {
                                vidaDependenteVO = vidaVO;
                                flagNovaVidaDependente = false;
                                break;
                            }
                        }
                    }

                    // Se não encontrou a vida do dependente, busca pelo CNS
                    if (vidaDependenteVO == null)
                    {
                        if (lstCnsVidas != null)
                        {
                            // Para cada vida
                            foreach (PVidaVO vidaVO in lstCnsVidas)
                            {
                                // Se o dependente na declaração é a vida no Protheus
                                if (IsSameVidaDependenteCns(declDepVO, vidaVO))
                                {
                                    vidaDependenteVO = vidaVO;
                                    flagNovaVidaDependente = false;
                                    break;
                                }
                            }
                        }
                    }

                    // Se o dependente na declaração é o mesmo no Protheus
                    if (vidaDependenteVO != null)
                    {
                        if (flagNovaFamilia == false)
                        {
                            if (lstOldUsuarios != null)
                            {
                                // Para cada usuário da família no Protheus
                                foreach (PUsuarioVO usuarioVO in lstOldUsuarios)
                                {

                                    // Se a vida no Protheus já foi encontrada 
                                    if (usuarioVO.Matvid != null && usuarioVO.Matvid.Trim() == vidaDependenteVO.Matvid.Trim())
                                    {
                                        usuarioDependenteVO = usuarioVO;
                                        flagNovoUsuarioDependente = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Caso não tenha sido encontrada, cria uma nova vida
                        vidaDependenteVO = new PVidaVO()
                        {
                            Nomusr = declDepVO.Nome
                        };
                    }

                    if (usuarioDependenteVO == null)
                    {
                        // Caso não tenha sido encontrado, cria um novo usuário
                        usuarioDependenteVO = new PUsuarioVO()
                        {
                            Nomusr = declDepVO.Nome,
                            Graupa = grauParentesco.ToString(),
                            Tipusu = PConstantes.TIPO_BENEFICIARIO_DEPENDENTE,
                            Datinc = dtInicio.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
                        };
                    }

                    #region[PREENCHIMENTO/ATUALIZAÇÃO DOS DADOS DOS DEPENDENTES]

                    // Preenche os dados do usuário dependente

                    string telefone;

                    if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            usuarioDependenteVO.Ddd = telefone.Substring(0, 2);
                            usuarioDependenteVO.Telres = telefone.Substring(2, telefone.Length - 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            usuarioDependenteVO.Ddd = telefone.Substring(0, 2);
                            usuarioDependenteVO.Telefo = telefone.Substring(2, telefone.Length - 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            usuarioDependenteVO.Ddd = telefone.Substring(0, 2);
                            usuarioDependenteVO.Telcom = telefone.Substring(2, telefone.Length - 2);
                        }
                    }

                    usuarioDependenteVO.Graupa = declDepVO.Parentesco.Id.ToString().PadLeft(2, '0');
                    int empresa = (int)declaracaoVO.Empresa;
                    usuarioDependenteVO.Codemp = empresa.ToString().PadLeft(4, '0');
                    usuarioDependenteVO.Conemp = integracaoVO.Familia.Conemp;
                    usuarioDependenteVO.Vercon = integracaoVO.Familia.Vercon;
                    usuarioDependenteVO.Subcon = integracaoVO.Familia.Subcon;
                    usuarioDependenteVO.Versub = integracaoVO.Familia.Versub;
                    usuarioDependenteVO.Nomusr = declDepVO.Nome.Trim().ToUpper();
                    usuarioDependenteVO.Sexo = ValidateUtil.Nvl(usuarioDependenteVO.Sexo, declDepVO.Sexo == PDados.Sexo.MASCULINO ? PConstantes.SEXO_MASCULINO : PConstantes.SEXO_FEMININO);
                    if (string.IsNullOrEmpty(usuarioDependenteVO.Sexo))
                    {
                        throw new EvidaException("Erro ao selecionar o sexo: " + declDepVO.Sexo);
                    }
                    usuarioDependenteVO.Datnas = declDepVO.Nascimento.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    usuarioDependenteVO.Cpfusr = declDepVO.Cpf.Trim().ToUpper();
                    usuarioDependenteVO.Drgusr = declDepVO.Rg.Trim().ToUpper();
                    usuarioDependenteVO.Pai = declDepVO.NomePai.Trim().ToUpper();
                    usuarioDependenteVO.Mae = declDepVO.NomeMae.Trim().ToUpper();
                    usuarioDependenteVO.Orgem = declDepVO.OrgaoExpedidor.Trim().ToUpper();
                    usuarioDependenteVO.Codint = declaracaoVO.Titular.Codint;
                    usuarioDependenteVO.Codemp = declaracaoVO.Titular.Codemp;
                    usuarioDependenteVO.Matemp = declaracaoVO.Titular.Matemp;
                    if (declaracaoVO.Titular.Admissao != null)
                    {
                        DateTime admissao = (DateTime)declaracaoVO.Titular.Admissao;
                        usuarioDependenteVO.Datadm = admissao.ToString("yyyyMMdd", CultureInfo.InvariantCulture);                    
                    }
                    usuarioDependenteVO.Datcar = dtInicio.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    usuarioDependenteVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
                    usuarioDependenteVO.Endere = declaracaoVO.Endereco.Rua.Trim().ToUpper();
                    usuarioDependenteVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
                    usuarioDependenteVO.Comend = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
                    usuarioDependenteVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
                    usuarioDependenteVO.Munici = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
                    usuarioDependenteVO.Estado = declaracaoVO.Endereco.Uf.Trim().ToUpper();
                    usuarioDependenteVO.Cepusr = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());
                    usuarioDependenteVO.Ycaren = tpCarencia == "NOR" ? "N" : "I";

                    if (lstOldUsuarios != null) 
                    {
                        // Para cada usuário da família no Protheus
                        foreach (PUsuarioVO usuarioVO in lstOldUsuarios)
                        {
                            // Se o dependente na declaração é o usuário no Protheus
                            if (IsSameUsuarioDependente(declDepVO, usuarioVO))
                            {
                                usuarioDependenteVO.Matant = usuarioVO.Matant;
                                usuarioDependenteVO.Ycdleg = usuarioVO.Ycdleg;
                                usuarioDependenteVO.Codmun = usuarioVO.Codmun;
                            }
                        }
                    }

                    PCepVO cep = PLocatorDataBO.Instance.BuscarCepProtheus(usuarioDependenteVO.Cepusr);
                    if (cep != null)
                    {
                        if (cep.Codmun != null)
                        {
                            usuarioDependenteVO.Codmun = cep.Codmun;
                        }
                        if (cep.Mun != null)
                        {
                            usuarioDependenteVO.Munici = cep.Mun;
                        }
                    }	

                    // Preenche os dados da vida dependente

                    if (!string.IsNullOrEmpty(declaracaoVO.TelResidencial.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelResidencial.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            vidaDependenteVO.Ddd = telefone.Substring(0, 2);
                            vidaDependenteVO.Telres = telefone.Substring(2, telefone.Length - 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(declaracaoVO.TelCelular.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelCelular.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            vidaDependenteVO.Ddd = telefone.Substring(0, 2);
                            vidaDependenteVO.Telefo = telefone.Substring(2, telefone.Length - 2);
                        }
                    }
                    if (!string.IsNullOrEmpty(declaracaoVO.TelComercial.Trim()))
                    {
                        telefone = apenasNumeros(declaracaoVO.TelComercial.Trim().ToUpper());
                        if (telefone.Length >= 3)
                        {
                            vidaDependenteVO.Ddd = telefone.Substring(0, 2);
                            vidaDependenteVO.Telcom = telefone.Substring(2, telefone.Length - 2);
                        }
                    }

                    vidaDependenteVO.Nomusr = declDepVO.Nome.Trim().ToUpper();
                    vidaDependenteVO.Nomcar = declDepVO.Nome.Trim().ToUpper();
                    vidaDependenteVO.Sexo = ValidateUtil.Nvl(vidaDependenteVO.Sexo, declDepVO.Sexo == PDados.Sexo.MASCULINO ? PConstantes.SEXO_MASCULINO : PConstantes.SEXO_FEMININO);
                    if (string.IsNullOrEmpty(vidaDependenteVO.Sexo))
                    {
                        throw new EvidaException("Erro ao selecionar o sexo: " + declDepVO.Sexo);
                    }
                    vidaDependenteVO.Datnas = declDepVO.Nascimento.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    vidaDependenteVO.Cpfusr = declDepVO.Cpf.Trim().ToUpper();
                    vidaDependenteVO.Drgusr = declDepVO.Rg.Trim().ToUpper();
                    vidaDependenteVO.Rgest = declDepVO.UfOrgaoExpedidor.Trim().ToUpper();
                    vidaDependenteVO.Pai = declDepVO.NomePai.Trim().ToUpper();
                    vidaDependenteVO.Mae = declDepVO.NomeMae.Trim().ToUpper();
                    vidaDependenteVO.Orgem = declDepVO.OrgaoExpedidor.Trim().ToUpper();
                    vidaDependenteVO.Email = declaracaoVO.Titular.Email.Trim().ToUpper();
                    if (declDepVO.Cns != null)
                    {
                        vidaDependenteVO.Nrcrna = declDepVO.Cns.Trim().ToUpper();
                    }
                    vidaDependenteVO.Endere = declaracaoVO.Endereco.Rua.Trim().ToUpper();
                    vidaDependenteVO.Nrend = declaracaoVO.Endereco.NumeroEndereco.Trim().ToUpper();
                    vidaDependenteVO.Comend = declaracaoVO.Endereco.Complemento.Trim().ToUpper();
                    vidaDependenteVO.Bairro = declaracaoVO.Endereco.Bairro.Trim().ToUpper();
                    vidaDependenteVO.Munici = declaracaoVO.Endereco.Cidade.Trim().ToUpper();
                    vidaDependenteVO.Estado = declaracaoVO.Endereco.Uf.Trim().ToUpper();
                    vidaDependenteVO.Cepusr = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());

                    cep = PLocatorDataBO.Instance.BuscarCepProtheus(vidaDependenteVO.Cepusr);
                    if (cep != null)
                    {
                        if (cep.Codmun != null)
                        {
                            vidaDependenteVO.Codmun = cep.Codmun;
                        }
                        if (cep.Mun != null)
                        {
                            vidaDependenteVO.Munici = cep.Mun;
                        }
                    }	

                    #endregion

                    // Mapeamento de usuário e vida do dependente
                    declDepVO.UsuarioDependente = usuarioDependenteVO;
                    declDepVO.VidaDependente = vidaDependenteVO;
                    declDepVO.FlagNovoUsuarioDependente = flagNovoUsuarioDependente;
                    declDepVO.FlagNovaVidaDependente = flagNovaVidaDependente;
                }
            }
        }

        #region Valida Nome+CPF e Nome+Rg do Titular na Declaração e do dependente no Protheus

        public bool IsSameVidaTitular(PDeclaracaoVO declaracaoVo, PVidaVO vidaVo)
        {
            return declaracaoVo.TitularDaDeclaracaoOuUsuarioDuplicado(vidaVo);

        }
        #endregion

        #region Valida Nome+CPF e Nome+Rg do Dependente na Declaração e do usuario no Protheus
        public bool IsSameVidaDependente(PDeclaracaoDependenteVO declDepVo, PVidaVO vidaVo)
        {
            return declDepVo.DependenteDaDeclaracaoOuUsuarioDuplicado(vidaVo);
        }

        #endregion

        private bool IsSameFornecedorDependente(PDeclaracaoDependenteVO declDepVO, PFornecedorVO fornecedorVO)
        {
            if (declDepVO == null) return false;
            if (declDepVO.Nome == null) return false;
            if (declDepVO.Cpf == null) return false;
            if (fornecedorVO == null) return false;
            if (fornecedorVO.Nome == null) return false;
            if (fornecedorVO.Cgc == null) return false;

            if (!declDepVO.Nome.Trim().Equals(fornecedorVO.Nome.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(declDepVO.Cpf.Trim()) && !string.IsNullOrEmpty(fornecedorVO.Cgc.Trim()))
            {
                long cpf_dec = Parse(declDepVO.Cpf.Trim());
                long cpf_prt = Parse(fornecedorVO.Cgc.Trim());

                if (cpf_dec != cpf_prt)
                {
                    return false;
                }
            }
            return true;
        }

        #region Valida Nome+CNS do Titular na Declaração e Protheus

        public bool IsSameVidaTitularCns(PDeclaracaoVO declaracaoVO, PVidaVO vidaVo)
        {
            return declaracaoVO.TitularDaDeclaracaoOuUsuarioDuplicado(vidaVo);

        }

        #endregion

        #region Valida Nome+CNS do Dependente na Declaração e Protheus

        public bool IsSameVidaDependenteCns(PDeclaracaoDependenteVO declDepVo, PVidaVO vidaVo)
        {
            return declDepVo.DependenteDaDeclaracaoOuUsuarioDuplicado(vidaVo);

        }

        #endregion

        #region Valida Nome+CPF e Nome+Rg do Titular na Declaração e do usuario no Protheus

        public bool IsSameUsuarioTitular(PDeclaracaoVO declaracaoVo, PUsuarioVO usuarioVo)
        {
            return declaracaoVo.TitularDaDeclaracaoOuUsuarioDuplicado(usuarioVo);
        }

        #endregion

        #region Valida Nome+CPF e Nome+Rg do Dependente na Declaração e do usuario no Protheus

        public bool IsSameUsuarioDependente(PDeclaracaoDependenteVO declDepVo, PUsuarioVO usuarioVo)
        {
            return declDepVo.DependenteDaDeclaracaoOuUsuarioDuplicado(usuarioVo);
        }

        #endregion

        public bool IsValidPlano(PDados.Empresa empresa, int idPlano)
        {
            switch (empresa)
            {
                case PDados.Empresa.ELETRONORTE: return (idPlano == PConstantes.PLANO_EVIDA_PPRS || idPlano == PConstantes.PLANO_EVIDA_FAMILIA);
                case PDados.Empresa.CEA: return (idPlano == PConstantes.PLANO_MAIS_VIDA_CEA);
                case PDados.Empresa.AMAZONASGT: return (idPlano == PConstantes.PLANO_EVIDA_PPRS_AMAZONASGT || idPlano == PConstantes.PLANO_EVIDA_MELHOR_IDADE || idPlano == PConstantes.PLANO_EVIDA_FAMILIA);
                case PDados.Empresa.AMAZONASD: return idPlano == PConstantes.PLANO_EVIDA_PPRS_AMAZONASD;

                case PDados.Empresa.EVIDA:
                    return (idPlano == PConstantes.PLANO_MAIS_VIDA_EVIDA || idPlano == PConstantes.PLANO_MAIS_VIDA_EVIDA_MASTER);

            }
            return false;
        }

        // Valida e retorna uma nova integração
        public PIntegracaoAdesaoVO ValidarIntegracao(int numProposta, DateTime dtInicio, string cdCategoria, string idPlano, string cdMotivoDesligamentoFamilia, string cdMotivoDesligamentoUsuario, string tpCarencia)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();

                // Nova integração
                PIntegracaoAdesaoVO integracaoVO = new PIntegracaoAdesaoVO();

                #region[ATRIBUIÇÃO DA DECLARAÇÃO]

                // Obtém a declaração atual
                PDeclaracaoVO declaracaoVO = PDeclaracaoDAO.GetById(numProposta, db);

                if (declaracaoVO == null)
                    throw new EvidaException("Declaracao não encontrada!");

                // Atribui a declaração à nova integração
                integracaoVO.Declaracao = declaracaoVO;

                #endregion

                #region[CHECAGEM DO PLANO]

                // Obtém o plano no Protheus
                PProdutoSaudeVO produtoSaude = PLocatorDataBO.Instance.GetProdutoSaude(idPlano.ToString(), db);

                if (produtoSaude == null)
                    throw new EvidaException("Plano inválido para integração: " + declaracaoVO.Plano.Descricao);

                // Se for um dos planos Família
                if(PDados.Produto.Find(declaracaoVO.Produto).Id == "F" ||
                   PDados.Produto.Find(declaracaoVO.Produto).Id == "FAMCEA" ||
                   PDados.Produto.Find(declaracaoVO.Produto).Id == "FAMEVIDA" ||
                   PDados.Produto.Find(declaracaoVO.Produto).Id == "FAMAGT")
                {
                    // O requerente será o titular do plano família
                    declaracaoVO.Titular.Nome = declaracaoVO.NomeRequerente;
                }

                #endregion

                #region[CLIENTE]

                // Cliente no Protheus
                PClienteVO cliente = null;

                // Obtém o cliente para este CGC no Protheus
                cliente = PClienteDAO.GetCliente(declaracaoVO.Titular.Cpf, db);

                FillClienteCorrespondente(integracaoVO, cliente);

                #endregion

                #region[FORNECEDOR TITULAR]

                // Forncededor titular no Protheus
                PFornecedorVO fornecedorTitular = null;

                // Obtém o fornecedor para este CGC no Protheus
                fornecedorTitular = PFornecedorDAO.GetFornecedor(declaracaoVO.Titular.Cpf, db);

                FillFornecedorTitular(integracaoVO, fornecedorTitular);

                #endregion

                #region[FORNECEDORES DEPENDENTES]

                // Lista de fornecedores dependentes no Protheus
                List<PFornecedorVO> lstFornecedoresDependentes = new List<PFornecedorVO>();

                List<PDeclaracaoDependenteVO> lstDependentes = declaracaoVO.Dependentes;

                if (lstDependentes != null && lstDependentes.Count > 0)
                {
                    // Para cada dependente na declaração atual
                    foreach (PDeclaracaoDependenteVO declDepVO in lstDependentes)
                    {
                        // Obtém o fornecedor para este CGC no Protheus
                        PFornecedorVO fornecedorDependente = PFornecedorDAO.GetFornecedor(declDepVO.Cpf, db);

                        if (fornecedorDependente != null)
                        {
                            lstFornecedoresDependentes.Add(fornecedorDependente);
                        }
                    }

                    FillFornecedoresDependentes(integracaoVO, lstFornecedoresDependentes);
                }

                #endregion

                #region[FAMÍLIA]

                #region[VARIÁVEIS]

                // Família no Protheus
                PFamiliaVO familia = null;

                // Lista de usuários para esta família no Protheus
                List<PUsuarioVO> lstOldUsuarios = null;

                // Lista de vidas para esta família no Protheus
                List<PVidaVO> lstOldVidas = null;

                // Usuário titular para esta família no Protheus
                PUsuarioVO usuarioTitular = null;

                // Vida titular para esta família no Protheus
                PVidaVO vidaTitular = null;

                // Regional desta família no Protheus
                string yregio = null;

                // Matrícula Antiga desta família no Protheus
                string matant = null;

                // Flag indicando se será criada uma nova família
                bool flagNovaFamilia = true;

                #endregion

                #region[OBTENÇÃO DA FAMÍLIA]

                if (!string.IsNullOrEmpty(cdCategoria.Trim()))
                {
                    String[] dados_categoria = cdCategoria.Trim().Split('-');
                    String codemp = dados_categoria[0];
                    String conemp = dados_categoria[1];
                    String subcon = dados_categoria[2];

                    // Busca uma Família com este contrato no Protheus
                    familia = PFamiliaBO.Instance.GetByContratoTitular(declaracaoVO.Empresa, declaracaoVO.Titular.Matemp, declaracaoVO.Titular, conemp, subcon);
                }

                // Se encontrou uma família com o mesmo contrato
                if (familia != null)
                {
                    flagNovaFamilia = false;
                }
                else 
                {
                    // Busca a última família deste funcionário no Protheus
                    familia = PFamiliaDAO.GetByMatriculaTitular(declaracaoVO.Empresa, declaracaoVO.Titular.Matemp, declaracaoVO.Titular, db);
                }

                #endregion

                #region[OBTENÇÃO DOS USUÁRIOS E VIDAS EXISTENTES]

                if (familia != null){

                    // Obtém a lista de usuários para esta família no Protheus
                    lstOldUsuarios = PUsuarioDAO.ListarUsuarios(familia.Codint, familia.Codemp, familia.Matric, db);

                    // Obtém a lista de vidas para esta família no Protheus
                    lstOldVidas = PVidaDAO.ListarVidas(familia.Codint, familia.Codemp, familia.Matric, db);

                    if (lstOldUsuarios != null)
                    {
                        // Obtém o usuário titular
                        usuarioTitular = lstOldUsuarios.Find(x => x.Tipusu.Equals(PConstantes.TIPO_BENEFICIARIO_FUNCIONARIO));
                    }

                    if (usuarioTitular != null)
                    {
                        // Obtém a vida titular
                        vidaTitular = lstOldVidas.Find(x => x.Matvid.Equals(usuarioTitular.Matvid));
                    }

                    yregio = familia.Yregio;
                    matant = familia.Matant;
                }

                #endregion

                if (flagNovaFamilia)
                {
                    string versao = String.Empty;
                    string forpag = String.Empty;
                    int diaret = Int32.MinValue;
                    string rotina = String.Empty;
                    string tippag = String.Empty;
                    string portad = String.Empty;
                    string agedep = String.Empty;
                    string ctacor = String.Empty;
                    
                    #region[BUSCA DETALHES DO PLANO]

                    string codpla = PDados.Produto.Find(declaracaoVO.Produto).ToProtheus().ToString().PadLeft(4, '0');
                    PProdutoSaudeVO plano = PLocatorDataBO.Instance.GetProdutoSaude(codpla);
                    if(plano != null){
                        versao = plano.Versao.PadLeft(3, '0');

                        string codint = declaracaoVO.Titular.Codint.PadLeft(4, '0');
                        string codemp = declaracaoVO.Titular.Codemp.PadLeft(4, '0');
                        string[] array_fcategoria = cdCategoria.Split('-');
                        if(array_fcategoria.Length >= 3)
                        {
                            string conemp = array_fcategoria[1];
                            string vercon = "001";
                            string subcon = array_fcategoria[2];
                            string versub = "001";

                            PEmpresaModalidadeCobrancaVO emprModCobr = PLocatorDataBO.Instance.GetEmpresaModalidadeCobranca(codint + codemp, conemp, vercon, subcon, versub, codpla, versao);
                            if (emprModCobr != null)
                            {
                                forpag = emprModCobr.Codfor;

                                PFormaPagamentoVO formaPagamento = PLocatorDataBO.Instance.GetFormaPagamento(emprModCobr.Codfor);
                                if (formaPagamento != null)
                                {
                                    rotina = formaPagamento.Rotina;
                                }

                                diaret = 0;
                            }

                            PSubcontratoVO subcontrato = PLocatorDataBO.Instance.GetSubcontrato(conemp, vercon, subcon, versub);
                            if (subcontrato != null)
                            {
                                tippag = subcontrato.Tippag;
                                portad = subcontrato.Portad;
                                agedep = subcontrato.Agedep;
                                ctacor = subcontrato.Ctacor;
                            }
                        
                        }

                    }

                    #endregion

                    // Se for uma nova família, cria um objeto família
                    familia = new PFamiliaVO()
                    {
                        Matemp = declaracaoVO.Titular.Matemp,
                        Yregio = yregio,
                        Matant = matant,
                        Datbas = dtInicio.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                        Versao = versao,
                        Forpag = forpag,
                        Diaret = diaret,
                        Rotina = rotina,
                        Tippag = tippag,
                        Portad = portad,
                        Agedep = agedep,
                        Ctacor = ctacor
                    };

                }
                else
                {
                    // Se for uma família existente, testa a empresa
                    if (Int32.Parse(familia.Codemp) != (int)declaracaoVO.Empresa)
                    {
                        throw new EvidaException("A empresa do funcionário é diferente da empresa do formulário!");
                    }
                }

                #region[PREENCHIMENTO/ATUALIZAÇÃO DOS DADOS DA FAMÍLIA]

                // Atribui as informações da família

                familia.Codint = declaracaoVO.Titular.Codint;
                familia.Codemp = declaracaoVO.Titular.Codemp;
                familia.Codpla = PDados.Produto.Find(declaracaoVO.Produto).ToProtheus().ToString().PadLeft(4, '0');
                string[] array_categoria = cdCategoria.Split('-');
                familia.Conemp = array_categoria[1];
                familia.Vercon = "001";
                familia.Subcon = array_categoria[2];
                familia.Versub = "001";
                familia.Undorg = declaracaoVO.Titular.Lotacao;
                familia.End = declaracaoVO.Endereco.Rua.ToUpper();
                familia.Numero = declaracaoVO.Endereco.NumeroEndereco.ToUpper();
                familia.Comple = declaracaoVO.Endereco.Complemento.ToUpper();
                familia.Bairro = declaracaoVO.Endereco.Bairro.ToUpper();
                familia.Mun = declaracaoVO.Endereco.Cidade.ToUpper();
                familia.Estado = declaracaoVO.Endereco.Uf;
                familia.Cep = FormatUtil.UnformatCep(declaracaoVO.Endereco.Cep.Trim());

                if (declaracaoVO.DadosBancarios != null)
                {
                    PDadosBancariosVO dBancVO = declaracaoVO.DadosBancarios;
                    familia.Agecli = ValidateUtil.Nvl(dBancVO.Agencia.Trim().ToUpper() + dBancVO.DVAgencia.Trim().ToUpper(), familia.Agecli);
                    familia.Bcocli = dBancVO.IdBanco;
                    familia.Ctacli = ValidateUtil.Nvl(dBancVO.Conta.Trim().ToUpper() + dBancVO.DVConta.Trim().ToUpper(), familia.Ctacli);
                }

                familia.Numcon = numProposta.ToString().PadLeft(5, '0');

                #endregion

                // Atribui a família à nova integração
                integracaoVO.Familia = familia;
                integracaoVO.FlagNovaFamilia = flagNovaFamilia;
                
                #endregion

                #region[FAIXA ETÁRIA FORMAS FAMÍLIA]

                // Obtém as faixas etárias beneficiário no Protheus
                List<PFaixaEtariaBeneficiarioVO> lstFaixas = PFaixaEtariaBeneficiarioDAO.ListarFaixas(familia.Codint + familia.Codemp, familia.Conemp, familia.Subcon, familia.Codpla, db);

                FillFaixaEtariaFormasFamilia(integracaoVO, lstFaixas);

                #endregion

                #region[VIDAS DOS CNS]

                // Lista de vidas com os CNS da proposta
                List<PVidaVO> lstCnsVidas = new List<PVidaVO>();

                // Variável auxiliar
                PVidaVO vidaCns = null;

                // Titular
                if(!string.IsNullOrEmpty(declaracaoVO.Titular.Cns)){

                    vidaCns = PVidaDAO.GetByCns(declaracaoVO.Titular.Cns, db);
                    if (vidaCns != null)
                    {
                        lstCnsVidas.Add(vidaCns);
                    }
                }

                // Dependentes
                if(declaracaoVO.Dependentes != null)
                {
                    foreach (PDeclaracaoDependenteVO dep in declaracaoVO.Dependentes)
                    {
                        if (!string.IsNullOrEmpty(dep.Cns)) 
                        {
                            vidaCns = PVidaDAO.GetByCns(dep.Cns, db);
                            if (vidaCns != null)
                            {
                                lstCnsVidas.Add(vidaCns);
                            }                        
                        }
                    }
                }

                #endregion

                #region[TITULAR]

                FillTitularCorrespondente(integracaoVO, lstOldUsuarios, lstOldVidas, lstCnsVidas, flagNovaFamilia, dtInicio, tpCarencia);

                #endregion

                #region[DEPENDENTES]

                if (declaracaoVO.Dependentes != null && declaracaoVO.Dependentes.Count > 0)
                {
                    // Mapeia os dependentes da declaração atual para os do Protheus
                    FillDependentesCorrespondentes(integracaoVO, lstOldUsuarios, lstOldVidas, lstCnsVidas, flagNovaFamilia, dtInicio, tpCarencia);
                }

                #endregion

                // Verifica se o Cns do titular e dos seus dependentes é duplicado
                // declaracaoVO.Titular = Titular na declaração atual
                // vidaTitular = Vida titular no Protheus
                // declaracaoVO.Dependentes = Lista de dependentes na declaração atual
                CheckCns(declaracaoVO.Titular, vidaTitular, declaracaoVO.Dependentes, db);

                // Mantidos apenas para compatibilidade
                integracaoVO.CdCategoria = cdCategoria;
                integracaoVO.InicioVigencia = dtInicio;
                integracaoVO.CdPlanoVinculado = produtoSaude.Codigo;
                integracaoVO.TpCarencia = tpCarencia;
                integracaoVO.CdMotivoDesligamentoFamilia = cdMotivoDesligamentoFamilia;
                integracaoVO.CdMotivoDesligamentoUsuario = cdMotivoDesligamentoUsuario;

                // Retorna a nova integração
                return integracaoVO;
            }
        }

        // Verifica se o Cns do titular e dos seus dependentes é duplicado
        // decTitular = Titular na declaração atual
        // titularProtheus = Vida titular no Protheus
        // lstDependentes = Lista de dependentes na declaração atual
        private void CheckCns(PPessoaVO titularDeclaracao, PVidaVO titularProtheus, List<PDeclaracaoDependenteVO> lstDependentes, EvidaDatabase db)
        {
            // Verificar se este Cns pertence a esta vida titular
           // ValidateDuplicatedCns(titularDeclaracao.Cns, titularProtheus, db);

            if (lstDependentes != null)
            {
                // Para cada dependente
                foreach (PDeclaracaoDependenteVO dep in lstDependentes)
                {
                    if (!string.IsNullOrEmpty(dep.Cns))
                    {
                        // Verificar se este Cns pertence a esta vida dependente
                        ValidateDuplicatedCns(dep.Cns, dep.VidaDependente, db);
                    }
                }
            }
        }

        // Verificar se este Cns pertence a esta vida
        // cns = Número de Cns
        // vidaProtheus = Vida no Protheus
        private void ValidateDuplicatedCns(string cns, PVidaVO vidaProtheus, EvidaDatabase db)
        {
            PVidaVO vidaCns = null;

            // Busca a vida à qual pertence este Cns
            if (!string.IsNullOrEmpty(cns))
                vidaCns = PVidaDAO.GetByCns(cns, db);

            if (vidaCns != null)
            {
                if (vidaProtheus != null)
                { // vida já existe
                    if (vidaCns.Matvid != vidaProtheus.Matvid)
                    {
                        
                        //    if (!string.IsNullOrEmpty(usuarioTitular.Motblo.Trim()))
                        //{
                        //    // Desbloqueia o usuário existente no Protheus
                        //    PUsuarioDAO.DesbloquearUsuario(usuarioTitular, cdMotivoDesligUsuario, descMotivoDesligUsuario, integracao.InicioVigencia, db);
                        //}

                        //throw new EvidaException("O CNS " + cns + " já é utilizado pelo beneficiario " + vidaCns.Nomusr);
                    }
                }
                /*else
                {
                    throw new EvidaException("O CNS " + cns + " já é utilizado pelo beneficiario " + vidaCns.Nomusr);
                }*/
            }
        }

        // Realiza a integração
        public void RealizarIntegracao(PIntegracaoAdesaoVO integracao, int idUsuario)
        {
            using (EvidaDatabase db = EvidaDatabase.CreateDatabase(null))
            {
                EvidaConnectionHolder connection = db.CreateConnection();
                connection.CreateTransaction();

                string cdMotivoDesligFamilia = !string.IsNullOrEmpty(integracao.CdMotivoDesligamentoFamilia.Trim()) ? integracao.CdMotivoDesligamentoFamilia.Trim() : "999";
                string cdMotivoDesligUsuario = !string.IsNullOrEmpty(integracao.CdMotivoDesligamentoUsuario.Trim()) ? integracao.CdMotivoDesligamentoUsuario.Trim() : "999";
                string descMotivoDesligFamilia = PLocatorDataBO.Instance.ObterDescricaoMotivoDesligamentoFamilia(cdMotivoDesligFamilia);
                string descMotivoDesligUsuario = PLocatorDataBO.Instance.ObterDescricaoMotivoDesligamentoUsuario(cdMotivoDesligUsuario);

                #region[CLIENTE]

                // Obtém o cliente da proposta de integração
                PClienteVO cliente = integracao.Cliente;

                var flagNovoCliente = integracao.FlagNovoCliente;

                string codGerado = "";

                if (flagNovoCliente)
                {
                    // Cria um novo Cliente
                    codGerado = PClienteDAO.CriarCliente(cliente, db);
                }
                else 
                {
                    // Altera os dados do cliente
                    PClienteDAO.AlterarCliente(cliente, db);

                    codGerado = cliente.Cod;                    
                }

                #endregion

                #region[FORNECEDOR TITULAR]

                // Obtém o fornecedor da proposta de integração
                PFornecedorVO fornecedor = integracao.FornecedorTitular;
                bool flagNovoFornecedorTitular = integracao.FlagNovoFornecedorTitular;

                if (flagNovoFornecedorTitular)
                {
                    // Cria um novo Fornecedor
                    PFornecedorDAO.CriarFornecedor(fornecedor, db);
                }
                else
                {
                    // Altera os dados do fornecedor
                    PFornecedorDAO.AlterarFornecedor(fornecedor, db);
                }

                #endregion	

                #region[FORNECEDORES DEPENDENTES]

                // Obtém a lista de dependentes da proposta de integração
                List<PDeclaracaoDependenteVO> lstDependentes = integracao.Declaracao.Dependentes;

                if (lstDependentes != null)
                {
                    // Para cada dependente da proposta de integração
                    foreach (PDeclaracaoDependenteVO declDep in lstDependentes)
                    {

                        // Obtém o fornecedor do dependente na proposta de integração
                        PFornecedorVO fornecedorDependente = declDep.FornecedorDependente;
                        bool flagNovoFornecedorDependente = declDep.FlagNovoFornecedorDependente;

                        if (flagNovoFornecedorDependente)
                        {
                            // Cria um novo Fornecedor
                            PFornecedorDAO.CriarFornecedor(fornecedorDependente, db);
                        }
                        else
                        {
                            // Altera os dados do fornecedor
                            PFornecedorDAO.AlterarFornecedor(fornecedorDependente, db);
                        }

                    }
                }

                #endregion

                #region[FAMÍLIA]

                // Obtém a família da proposta de integração
                PFamiliaVO familia = integracao.Familia;
                bool flagNovaFamilia = integracao.FlagNovaFamilia;
                string matricGerado = "";

                if (flagNovaFamilia)
                {

                    // Busca a Família vigente no Protheus
                    PFamiliaVO famVo = PFamiliaDAO.GetAtualByMatriculaTitular(integracao.Declaracao.Empresa, integracao.Declaracao.Titular.Matemp, integracao.Declaracao.Titular, db);

                    // Se há uma família vigente no Protheus
                    if (famVo != null)
                    {
                        // Bloqueia a família existente no Protheus
                        PFamiliaDAO.BloquearFamilia(famVo, cdMotivoDesligFamilia, descMotivoDesligFamilia, db);

                        // Obtém a lista de usuários para esta família no Protheus
                        List<PUsuarioVO> lstOldUsuarios = PUsuarioDAO.ListarUsuarios(famVo.Codint, famVo.Codemp, famVo.Matric, db);

                        if (lstOldUsuarios != null)
                        {
                            // Bloqueia os usuários existentes
                            foreach (PUsuarioVO usuVO in lstOldUsuarios)
                            {
                                PUsuarioDAO.BloquearUsuario(usuVO, cdMotivoDesligUsuario, descMotivoDesligUsuario, db);
                            }
                        }

                    }

                    familia.Codcli = codGerado;

                    // Cria uma nova familia
                    matricGerado = PFamiliaDAO.CriarFamilia(familia, db);

                }
                else
                {

                    // Busca a Família vigente no Protheus
                    PFamiliaVO famVo = PFamiliaDAO.GetAtualByMatriculaTitular(integracao.Declaracao.Empresa, integracao.Declaracao.Titular.Matemp, integracao.Declaracao.Titular, db);

                    // Se há uma família vigente no Protheus
                    if (famVo != null && (famVo.Codint.Trim() != familia.Codint.Trim() || famVo.Codemp.Trim() != familia.Codemp.Trim() || famVo.Matric.Trim() != familia.Matric.Trim()))
                    {
                        // Bloqueia a família existente no Protheus
                        PFamiliaDAO.BloquearFamilia(famVo, cdMotivoDesligFamilia, descMotivoDesligFamilia, db);

                        // Obtém a lista de usuários para esta família no Protheus
                        List<PUsuarioVO> lstOldUsuarios = PUsuarioDAO.ListarUsuarios(famVo.Codint, famVo.Codemp, famVo.Matric, db);

                        if (lstOldUsuarios != null)
                        {
                            // Bloqueia os usuários existentes
                            foreach (PUsuarioVO usuVO in lstOldUsuarios)
                            {
                                PUsuarioDAO.BloquearUsuario(usuVO, cdMotivoDesligUsuario, descMotivoDesligUsuario, db);
                            }
                        }

                    }

                    // Se a família está bloqueada
                    if(!string.IsNullOrEmpty(familia.Motblo.Trim())){

                        // Desbloqueia a família existente no Protheus
                        PFamiliaDAO.DesbloquearFamilia(familia, cdMotivoDesligFamilia, descMotivoDesligFamilia, integracao.InicioVigencia, db);
                    }

                    // Altera os dados da familia
                    PFamiliaDAO.AlterarFamilia(familia, db);

                    matricGerado = familia.Matric;
                }

                #endregion

                #region[TITULAR]

                // Obtém o usuário e a vida titular da proposta de integração
                PUsuarioVO usuarioTitular = integracao.UsuarioTitular;
                PVidaVO vidaTitular = integracao.VidaTitular;
                bool flagNovoUsuarioTitular = integracao.FlagNovoUsuarioTitular;

                bool flagNovaVidaTitular = integracao.FlagNovaVidaTitular;

                string matvidGerado = "";

                if(flagNovaVidaTitular)
                {
                    // Cria uma nova vida
                    matvidGerado = PVidaDAO.CriarVida(vidaTitular, db);
                }
                else
                {
                    // Altera os dados da vida
                    PVidaDAO.AlterarVida(vidaTitular, db);

                    matvidGerado = vidaTitular.Matvid;
                }

                if (flagNovoUsuarioTitular)
                {
                    usuarioTitular.Matric = matricGerado;
                    usuarioTitular.Matvid = matvidGerado;

                    // Cria um novo usuário
                    PUsuarioDAO.CriarUsuario(usuarioTitular, db);
                    
                    connection.Commit();
                    connection.CreateTransaction();
                }
                else
                {
                    // Se o usuário está bloqueado
                    if (!string.IsNullOrEmpty(usuarioTitular.Motblo.Trim()))
                    {
                        // Desbloqueia o usuário existente no Protheus
                        PUsuarioDAO.DesbloquearUsuario(usuarioTitular, cdMotivoDesligUsuario, descMotivoDesligUsuario, integracao.InicioVigencia, db);
                    }

                    // Altera os dados do usuário
                    PUsuarioDAO.AlterarUsuario(usuarioTitular, db);
                }

                #endregion

                #region[DEPENDENTES]

                // Obtém a lista de dependentes da proposta de integração
                //List<PDeclaracaoDependenteVO> lstDependentes = integracao.Declaracao.Dependentes;

                if (lstDependentes != null)
                {
                    // Para cada dependente da proposta de integração
                    foreach (PDeclaracaoDependenteVO declDep in lstDependentes)
                    {

                        // Obtém o usuário e a vida do dependente na proposta de integração
                        PUsuarioVO usuarioDependente = declDep.UsuarioDependente;
                        PVidaVO vidaDependente = declDep.VidaDependente;
                        bool flagNovoUsuarioDependente = declDep.FlagNovoUsuarioDependente;
                        bool flagNovaVidaDependente = declDep.FlagNovaVidaDependente;

                        if (flagNovaVidaDependente)
                        {
                            // Cria uma nova vida
                            matvidGerado = PVidaDAO.CriarVida(vidaDependente, db);
                        }
                        else
                        {
                            // Altera os dados da vida
                            PVidaDAO.AlterarVida(vidaDependente, db);

                            matvidGerado = vidaDependente.Matvid;
                        }

                        if (flagNovoUsuarioDependente)
                        {
                            usuarioDependente.Matric = matricGerado;
                            usuarioDependente.Matvid = matvidGerado;

                            // Cria um novo usuário
                            PUsuarioDAO.CriarUsuario(usuarioDependente, db);

                            connection.Commit();
                            connection.CreateTransaction();
                        }
                        else
                        {
                            // Se o usuário está bloqueado
                            if (!string.IsNullOrEmpty(usuarioDependente.Motblo.Trim()))
                            {

                                // Desbloqueia o usuário existente no Protheus
                                PUsuarioDAO.DesbloquearUsuario(usuarioDependente, cdMotivoDesligUsuario, descMotivoDesligUsuario, integracao.InicioVigencia, db);
                            }

                            // Altera os dados do usuário
                            PUsuarioDAO.AlterarUsuario(usuarioDependente, db);
                        }

                    }
                }

                #endregion

                #region[FAIXA ETÁRIA FORMAS FAMÍLIA]

                List<PFaixaEtariaFormasFamiliaVO> lstFaixaEtaria = integracao.FaixasEtarias;
                if(lstFaixaEtaria != null && lstFaixaEtaria.Count > 0)
                {
                    foreach(PFaixaEtariaFormasFamiliaVO faixaEtaria in lstFaixaEtaria)
                    {
                        faixaEtaria.Matric = matricGerado;
                        PFaixaEtariaFormasFamiliaDAO.CriarFaixaEtariaFormasFamilia(faixaEtaria, db);
                    }
                }

                #endregion	

                // Marca a declaração como integrada
                PDeclaracaoDAO.MarcarIntegrada(integracao.Declaracao.Numero, idUsuario, familia.Undorg, integracao, db);
                connection.Commit();
            }
        }

        /*// Gera um código para os dependentes
        // lstDeclNew = Lista de dependentes da proposta de integração
        // lstAllOld = Lista de dependentes deste funcionário no ISA
        private void BuildCdDependente(List<DeclaracaoDependenteVO> lstDeclNew, List<HcDependenteVO> lstAllOld)
        {
            List<HcDependenteVO> lstAll = new List<HcDependenteVO>();

            IEnumerable<HcDependenteVO> lstNew = lstDeclNew.Select(x => x.DependenteCorrespondente);

            if (lstNew != null) lstAll.AddRange(lstNew);

            if (lstAllOld != null) lstAll.AddRange(lstAllOld.Where(x => lstNew.FirstOrDefault(y => y.CdDependente == x.CdDependente) == null));

            IEnumerable<HcDependenteVO> enumNew = lstNew.OrderBy(x => (x.DtNascimento != null ? x.DtNascimento.Value : DateTime.MaxValue));
            IEnumerable<HcDependenteVO> enumAll = lstAll.OrderBy(x => (x.DtNascimento != null ? x.DtNascimento.Value : DateTime.MaxValue));
            foreach (HcDependenteVO novo in enumNew)
            {
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

        private int GetSequencialByCdDependente(int cdDependente)
        {
            string strId = cdDependente.ToString("0000");
            string lst2Digitos = strId.Substring(strId.Length - 2);
            return Int32.Parse(lst2Digitos);
        }

        // Gera um código para o beneficiário
        // func = funcionário da proposta de integração
        // cdDependente = código do dependente gerado
        private string BuildCdAlternativo(HcFuncionarioVO func, int? cdDependente)
        {
            //cd_alternativo = cod_empresa-mat-composicao (onde composicao é 2 dígitos Parentesco e 2 sequencial)
            string parteComum = func.CdEmpresa.ToString("00") + "-" + func.CdFuncionario + "-";
            string composicao = "0000";
            if (cdDependente != null)
            {
                composicao = cdDependente.Value.ToString("0000");
            }
            return parteComum + composicao;
        }*/

        private string apenasNumeros(string texto) {

            Regex re = new Regex("[0-9]");
            StringBuilder s = new StringBuilder();

            foreach (Match m in re.Matches(texto.Trim()))
            {
                s.Append(m.Value);
            }

            return s.ToString();
        }

    }
}
