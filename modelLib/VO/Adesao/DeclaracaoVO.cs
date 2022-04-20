using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Adesao {


	[Serializable]
	public class PessoaVO {
		public string Nome { get; set; }
		public Dados.Sexo Sexo { get; set; }
		public DateTime Nascimento { get; set; }
		public string Cpf { get; set; }
		public string Rg { get; set; }
		public string Cns { get; set; }
		public string NomePai { get; set; }
		public string NomeMae { get; set; }
		public string OrgaoExpedidor { get; set; }
		public string UfOrgaoExpedidor { get; set; }
		public DateTime? DataEmissaoRg { get; set; }

		public long Matricula { get; set; }
		public string Lotacao { get; set; }
		public DateTime? Admissao { get; set; }
		public Dados.DeclaracaoEstadoCivil EstadoCivil { get; set; }
		public string Email { get; set; }
	}
	public class AdesaoEnderecoVO {
		public string Rua { get; set; }
		public string NumeroEndereco { get; set; }
		public string Complemento { get; set; }
		public string Bairro { get; set; }
		public string Cidade { get; set; }
		public string Uf { get; set; }
		public string Cep { get; set; }
	}
	public class DadosBancariosVO {
		public string IdBanco { get; set; }
		public string NomeBanco { get; set; }
		public string Agencia { get; set; }
		public string DVAgencia { get; set; }
		public string Conta { get; set; }
		public string DVConta { get; set; }
		public int? DiaPagamento { get; set; }
	}
	public class DeclaracaoVO {
		public static string FORMATO_PROTOCOLO = "000000000";
		public Guid Id { get; set; }
		public string Produto { get; set; }
		public PessoaVO Titular { get; set; }
		public string TelResidencial { get; set; }
		public string TelCelular { get; set; }
		public string TelComercial { get; set; }
		public string Email { get; set; }
		public AdesaoEnderecoVO Endereco { get; set; }
		public List<DeclaracaoDependenteVO> Dependentes { get; set; }
		public DadosBancariosVO DadosBancarios { get; set; }
		public DateTime Criacao { get; set; }
		public string Local { get; set; }
		public int Numero { get; set; }
		public string NomeRequerente { get; set; }
		public int? Parentesco { get; set; }
		public decimal? Inss { get; set; }
		public decimal? Previnorte { get; set; }
		public decimal? OutrasFontes { get; set; }
		public ResponsavelFinanceiroVO ResponsavelFinanceiro { get; set; }
		public int OpcaoAutorizacao { get; set; }
		public DateTime InicioPlano { get; set; }
		public Dados.Produto Plano { get; set; }
		public Dados.Empresa Empresa { get; set; }
		public string ObsValidacao { get; set; }

		public DateTime? InicioPlanoIntegracao { get; set; }
		public int? IdUsuarioIntegracao { get; set; }
		public DateTime? DataIntegracao { get; set; }
		public int? CdCategoria { get; set; }
		public string PlanoIntegracao { get; set; }
		public int? CdMotivoDesligamento { get; set; }
		public string CarenciaIntegracao { get; set; }
	}
	public class ResponsavelFinanceiroVO : PessoaVO {
		public string Telefone { get; set; }
	}
	public class DeclaracaoDependenteVO : PessoaVO {
		public Dados.DeclaracaoParentesco Parentesco { get; set; }
		public int? CodDependente { get; set; }
		public HC.HcBeneficiarioVO BeneficiarioCorrespondente { get; set; }
		public HC.HcDependenteVO DependenteCorrespondente { get; set; }
		public HC.HcBeneficiarioPlanoVO BeneficiarioPlanoCorrespondente { get; set; }
	}
}
