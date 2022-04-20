using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.VO.Adesao
{
    [Serializable]
    public class PPessoaVO
    {
        public string Nome { get; set; }
        public PDados.Sexo Sexo { get; set; }
        public DateTime Nascimento { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public string Cns { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string OrgaoExpedidor { get; set; }
        public string UfOrgaoExpedidor { get; set; }
        public DateTime? DataEmissaoRg { get; set; }

        public string Codint { get; set; }
        public string Codemp { get; set; }
        public string Matric { get; set; }
        public string Tipreg { get; set; }
        public string Matemp { get; set; }
        public string Lotacao { get; set; }
        public DateTime? Admissao { get; set; }
        public PDados.DeclaracaoEstadoCivil EstadoCivil { get; set; }
        public string Email { get; set; }
    }

    public class PAdesaoEnderecoVO
    {
        public string Rua { get; set; }
        public string NumeroEndereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
    }

    public class PDadosBancariosVO
    {
        public string IdBanco { get; set; }
        public string NomeBanco { get; set; }
        public string Agencia { get; set; }
        public string DVAgencia { get; set; }
        public string Conta { get; set; }
        public string DVConta { get; set; }
        public int? DiaPagamento { get; set; }
    }

    public class PDeclaracaoVO
    {
        public static string FORMATO_PROTOCOLO = "000000000";
        public Guid Id { get; set; }
        public string Produto { get; set; }
        public PPessoaVO Titular { get; set; }
        public string TelResidencial { get; set; }
        public string TelCelular { get; set; }
        public string TelComercial { get; set; }
        public string Email { get; set; }
        public PAdesaoEnderecoVO Endereco { get; set; }
        public List<PDeclaracaoDependenteVO> Dependentes { get; set; }
        public PDadosBancariosVO DadosBancarios { get; set; }
        public DateTime Criacao { get; set; }
        public string Local { get; set; }
        public int Numero { get; set; }
        public string NomeRequerente { get; set; }
        public int? Parentesco { get; set; }
        public decimal? Inss { get; set; }
        public decimal? Previnorte { get; set; }
        public decimal? OutrasFontes { get; set; }
        public int OpcaoAutorizacao { get; set; }
        public DateTime InicioPlano { get; set; }
        public PDados.Produto Plano { get; set; }
        public PDados.Empresa Empresa { get; set; }
        public string ObsValidacao { get; set; }

        public DateTime? InicioPlanoIntegracao { get; set; }
        public int? IdUsuarioIntegracao { get; set; }
        public DateTime? DataIntegracao { get; set; }
        public int? CdCategoria { get; set; }
        public string PlanoIntegracao { get; set; }
        public string CdMotivoDesligamentoFamilia { get; set; }
        public string CdMotivoDesligamentoUsuario { get; set; }
        public string CarenciaIntegracao { get; set; }
    }

    public class PDeclaracaoDependenteVO : PPessoaVO
    {
        public PDados.DeclaracaoParentesco Parentesco { get; set; }
        public PUsuarioVO UsuarioDependente { get; set; }
        public PVidaVO VidaDependente { get; set; }
        public PFornecedorVO FornecedorDependente { get; set; }
        public bool FlagNovoUsuarioDependente { get; set; }
        public bool FlagNovaVidaDependente { get; set; }
        public bool FlagNovoFornecedorDependente { get; set; }
        public object DependenteDaDeclaracao { get; internal set; }
    }

}
