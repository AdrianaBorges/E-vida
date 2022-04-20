using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO 
{
	public class Constantes 
    {
		public const int AUTORIZACAO_PRAZO_OPME = 5 * 24;
		public const int AUTORIZACAO_PRAZO_NORMAL = 3 * 24;

		public const int PLANO_EVIDA_FAMILIA = 22;
		public const int PLANO_MAIS_VIDA_CEA = 23;
		public const int PLANO_EVIDA_PPRS = 20;
		public const int PLANO_EVIDA_MELHOR_IDADE = 21;
		public const int PLANO_EVIDA_PPRS_AMAZONASGT = 24;
		public const int PLANO_EVIDA_PPRS_AMAZONASD = 25;
		public const int PLANO_MAIS_VIDA_EVIDA = 26;

        public const int PLANO_MAIS_VIDA_EVIDA_MASTER = 27;

        public const int CATEGORIA_COMISSIONADO = 41;

		public const string TIPO_BENEFICIARIO_FUNCIONARIO = "F";
		public const string TIPO_BENEFICIARIO_DEPENDENTE = "D";

		public const string LISTA_ABRANGENCIA = "TP_ABRANGENCIA";
		public const string LISTA_ESTADO_CIVIL = "ESTADO_CIVIL";
		public const string LISTA_ORGAO_EXPEDIDOR = "TIPO_ORGAO_EXP";

		public const string TP_ESTADO_CIVIL_NAO_INFORMADO = "6";

		public const string PESSOA_FISICA = "F";
		public const string PESSOA_JURIDICA = "J";

		public const string GRAU_PARENTESCO_FILHO = "2";
		public const string GRAU_PARENTESCO_ENTEADO = "6";

		public const string SEXO_FEMININO = "F";
		public const string SEXO_MASCULINO = "M";

		public const string SEXO_INDEFINIDO = "";

		public const string SITUACAO_BENEFICIARIO_ATIVO = "A";
		public const string SITUACAO_BENEFICIARIO_INATIVO = "I";

		public const int EMPRESA_ELETRONOTE = 60;
		public const int EMPRESA_CEA = 62;
		public const int EMPRESA_AMAZONASGT = 63;
		public const int EMPRESA_AMAZONASD = 64;
		public const int EMPRESA_EVIDA = 65;

		public static readonly int[] EMPRESAS_ADESAO_ESP = new int[] { EMPRESA_CEA, EMPRESA_AMAZONASGT, EMPRESA_AMAZONASD, EMPRESA_EVIDA };
		
		public class Uf {
			public static readonly Uf AC = new Uf("AC", "Acre");
			public static readonly Uf AL = new Uf("AL", "Alagoas");
			public static readonly Uf AP = new Uf("AP", "Amapá");
			public static readonly Uf AM = new Uf("AM", "Amazonas");
			public static readonly Uf BA = new Uf("BA", "Bahia");
			public static readonly Uf CE = new Uf("CE", "Ceará");
			public static readonly Uf DF = new Uf("DF", "Distrito Federal");
			public static readonly Uf ES = new Uf("ES", "Espírito Santo");
			public static readonly Uf GO = new Uf("GO", "Goiás");
			public static readonly Uf MA = new Uf("MA", "Maranhão");
			public static readonly Uf MT = new Uf("MT", "Mato Grosso");
			public static readonly Uf MS = new Uf("MS", "Mato Grosso do Sul");
			public static readonly Uf MG = new Uf("MG", "Minas Gerais");
			public static readonly Uf PA = new Uf("PA", "Pará");
			public static readonly Uf PB = new Uf("PB", "Paraíba");
			public static readonly Uf PR = new Uf("PR", "Paraná");
			public static readonly Uf PE = new Uf("PE", "Pernambuco");
			public static readonly Uf PI = new Uf("PI", "Piauí");
			public static readonly Uf RJ = new Uf("RJ", "Rio de Janeiro");
			public static readonly Uf RN = new Uf("RN", "Rio Grande do Norte");
			public static readonly Uf RS = new Uf("RS", "Rio Grande do Sul");
			public static readonly Uf RO = new Uf("RO", "Rondônia");
			public static readonly Uf RR = new Uf("RR", "Roraima");
			public static readonly Uf SC = new Uf("SC", "Santa Catarina");
			public static readonly Uf SP = new Uf("SP", "São Paulo");
			public static readonly Uf SE = new Uf("SE", "Sergipe");
			public static readonly Uf TO = new Uf("TO", "Tocantins");

			private Uf(string sigla, string nome) {
				this.Sigla = sigla;
				this.Nome = nome;
			}

			public string Sigla { get; private set; }
			public string Nome { get; private set; }

			public static IEnumerable<Uf> Values {
				get {
					yield return AC;
					yield return AL;
					yield return AP;
					yield return AM;
					yield return BA;
					yield return CE;
					yield return DF;
					yield return ES;
					yield return GO;
					yield return MA;
					yield return MT;
					yield return MS;
					yield return MG;
					yield return PA;
					yield return PB;
					yield return PR;
					yield return PE;
					yield return PI;
					yield return RJ;
					yield return RN;
					yield return RS;
					yield return RO;
					yield return RR;
					yield return SC;
					yield return SP;
					yield return SE;
					yield return TO;
				}
			}
		}
	}

}
