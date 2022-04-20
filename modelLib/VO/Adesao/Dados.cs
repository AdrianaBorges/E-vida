using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Adesao {
	public class Dados {

		public enum Empresa {
			ELETRONORTE = Constantes.EMPRESA_ELETRONOTE,
			CEA = Constantes.EMPRESA_CEA,
			AMAZONASGT = Constantes.EMPRESA_AMAZONASGT,
			AMAZONASD = Constantes.EMPRESA_AMAZONASD,
			EVIDA = Constantes.EMPRESA_EVIDA
		}
		public enum Sexo {
			INDEFINIDO = 'I',
			MASCULINO = 'M',
			FEMININO = 'F'
		}
		public enum SituacaoDeclaracao {
			PENDENTE = 0,
			RECEBIDA = 1,
			VALIDADA = 2,
			INVALIDA = 3,
			INTEGRADA = 4
		}
		public class EnumTradutor {
			public static string TraduzEmpresa(Empresa empresa) {
				switch (empresa) {
					case Empresa.ELETRONORTE: return "ELETRONORTE";
					case Empresa.CEA: return "CEA";
					case Empresa.AMAZONASGT: return "AMAZONAS GT";
					case Empresa.AMAZONASD: return "AMAZONAS DISTRIBUIÇÃO";
					case Empresa.EVIDA: return "E-VIDA";
					default: return "--";
				}
			}
		}

		[Serializable]
		public class Produto {

            public static readonly Produto MAIS_VIDA_EVIDA_MASTER = new Produto("MVEVIDAMASTER",
                "E-VIDA MASTER",
                "rptVidaEvidaMaster",
                "MaisVidaEvidaMaster.aspx",
                Dados.Empresa.EVIDA);

            public static readonly Produto MAIS_VIDA_EVIDA = new Produto("MVEVIDA",
                "+VIDA",
                "rptVidaEvida",
                "MaisVidaEvida.aspx",
                Dados.Empresa.EVIDA);

            public static readonly Produto PPRS_ELETRONORTE = new Produto("PE", "PPRS (PARTICIPATIVO)", "rptPPRSEletronorte", "PprsEletronorte.aspx", Dados.Empresa.ELETRONORTE);
			public static readonly Produto MELHOR_IDADE = new Produto("MI", "E-VIDA MELHOR IDADE (APOSENTADOS/EX-EMPREGADOS)", "rptMelhorIdade", "MelhorIdade.aspx", Dados.Empresa.ELETRONORTE);
			public static readonly Produto FAMILIA = new Produto("F", "E-VIDA FAMÍLIA", "rptFamilia", "Familia.aspx", Dados.Empresa.ELETRONORTE);
			public static readonly Produto PPRS = new Produto("P", "E-VIDA PPRS (CONTRIBUTIVO)", "rptPPRS", "Pprs.aspx", Dados.Empresa.ELETRONORTE);
			public static readonly Produto DEPENDENTE = new Produto("DE", "DEPENDENTE", "rptDependente", "Dependente.aspx", Dados.Empresa.ELETRONORTE);
			public static readonly Produto MELHOR_IDADE_LICENCA = new Produto("ML", "E-VIDA MELHOR IDADE (EMPREGADOS EM LICENÇA SEM VENCIMENTO NA MANTENEDORA)", "MelhorIdade.aspx?LIC=Y", "rptMelhorIdadeLic", Dados.Empresa.ELETRONORTE);
			public static readonly Produto MAIS_VIDA_CEA = new Produto("MVC", "CEA +VIDA", "rptVidaCea", "MaisVidaCea.aspx", Dados.Empresa.CEA);
			public static readonly Produto PPRS_AMAZONASGT = new Produto("PRA", "E-VIDA PPRS AmGT", "rptPPRSAmGt", "PprsAmazonasGt.aspx", Dados.Empresa.AMAZONASGT);
			public static readonly Produto PPRS_AMAZONASD = new Produto("PPRSAMD", "E-VIDA PPRS AmD", "rptPPRSAmD", "PprsAmazonasD.aspx", Dados.Empresa.AMAZONASD);

			public static readonly Produto DEPENDENTE_AMAZONASGT = new Produto("DEAGT", "DEPENDENTE", "rptDependente", "Dependente.aspx", Dados.Empresa.AMAZONASGT);
			public static readonly Produto DEPENDENTE_AMAZONASD = new Produto("DEAMD", "DEPENDENTE", "rptDependente", "Dependente.aspx", Dados.Empresa.AMAZONASD);
			public static readonly Produto DEPENDENTE_CEA = new Produto("DECEA", "DEPENDENTE", "rptDependente", "Dependente.aspx", Dados.Empresa.CEA);
			public static readonly Produto DEPENDENTE_EVIDA = new Produto("DEEVIDA", "DEPENDENTE", "rptDependente", "Dependente.aspx", Dados.Empresa.EVIDA);

			public static readonly Produto ALL_PPRS = new Produto("PPRS_ALL", "-", "-", "- ", Empresa.ELETRONORTE);
			public static readonly Produto[] ALL_PPRS_ARR = new Produto[] { PPRS_ELETRONORTE, PPRS, PPRS_AMAZONASGT, PPRS_AMAZONASD, MAIS_VIDA_EVIDA, MAIS_VIDA_EVIDA_MASTER, MAIS_VIDA_CEA };

			/** Careful precisa ser o último, atencao no update */
			public static readonly Produto[] PLANOS_INTEGRACAO = new Produto[] { MAIS_VIDA_CEA, PPRS_AMAZONASD, PPRS_AMAZONASGT, MAIS_VIDA_EVIDA, MAIS_VIDA_EVIDA_MASTER };

			private Produto(string id, string descricao, string rpt, string pagina, Dados.Empresa empresa) {
				this.Id = id;
				this.Descricao = descricao;
				this.ReportFile = rpt;
				this.Empresa = empresa;
				this.Pagina = pagina;
			}

			public string Id { get; private set; }
			public string Descricao { get; private set; }
			public string ReportFile { get; private set; }
			public Dados.Empresa Empresa { get; private set; }
			public string Pagina { get; private set; }

			public static IEnumerable<Produto> Values {
				get {
					yield return PPRS_ELETRONORTE;
					yield return PPRS;
					yield return MELHOR_IDADE;
					yield return FAMILIA;
					yield return DEPENDENTE;
					yield return MELHOR_IDADE_LICENCA;
					yield return MAIS_VIDA_CEA;
					yield return PPRS_AMAZONASGT;
					yield return PPRS_AMAZONASD;
					yield return MAIS_VIDA_EVIDA;
                    yield return MAIS_VIDA_EVIDA_MASTER;
                    yield return DEPENDENTE_AMAZONASGT;
					yield return DEPENDENTE_AMAZONASD;
					yield return DEPENDENTE_CEA;
					yield return DEPENDENTE_EVIDA;
				}
			}

			public static Produto Find(string id) {
				foreach (Produto p in Values) {
					if (p.Id.Equals(id.Trim()))
						return p;
				}
				return null;
			}

			public override bool Equals(object obj) {
				if (obj == null) return false;
				Produto p = (Produto)obj;
				return this.Id.Equals(p.Id);
			}

			public override int GetHashCode() {
				int hash = 17;
				hash = hash * 23 + Id.GetHashCode();
				return hash;
			}

			public int ToISA() {
				if (this.Id.Equals(MAIS_VIDA_CEA.Id)) return Constantes.PLANO_MAIS_VIDA_CEA;
				if (this.Id.Equals(PPRS.Id)) return Constantes.PLANO_EVIDA_PPRS;
				if (this.Id.Equals(PPRS_ELETRONORTE.Id)) return Constantes.PLANO_EVIDA_PPRS;
				if (this.Id.Equals(MELHOR_IDADE.Id)) return Constantes.PLANO_EVIDA_MELHOR_IDADE;
				if (this.Id.Equals(MELHOR_IDADE_LICENCA.Id)) return Constantes.PLANO_EVIDA_MELHOR_IDADE;
				if (this.Id.Equals(FAMILIA.Id)) return Constantes.PLANO_EVIDA_FAMILIA;
				if (this.Id.Equals(PPRS_AMAZONASGT.Id)) return Constantes.PLANO_EVIDA_PPRS_AMAZONASGT;
				if (this.Id.Equals(PPRS_AMAZONASD.Id)) return Constantes.PLANO_EVIDA_PPRS_AMAZONASD;
				if (this.Id.Equals(MAIS_VIDA_EVIDA.Id)) return Constantes.PLANO_MAIS_VIDA_EVIDA;

                if (Id.Equals(MAIS_VIDA_EVIDA_MASTER.Id))
                    return Constantes.PLANO_MAIS_VIDA_EVIDA_MASTER;


                throw new InvalidOperationException("Produto " + this.Id + " não mapeado para plano do ISA");
			}

		}

		[Serializable]
		public class DeclaracaoEstadoCivil {
			public static readonly DeclaracaoEstadoCivil SOLTEIRO = new DeclaracaoEstadoCivil(1, "SOLTEIRO");
			public static readonly DeclaracaoEstadoCivil CASADO = new DeclaracaoEstadoCivil(2, "CASADO");
			public static readonly DeclaracaoEstadoCivil DIVORCIADO = new DeclaracaoEstadoCivil(3, "DIVORCIADO");
			public static readonly DeclaracaoEstadoCivil SEPARADO = new DeclaracaoEstadoCivil(4, "SEPARADO");
			public static readonly DeclaracaoEstadoCivil VIUVO = new DeclaracaoEstadoCivil(5, "VIUVO");
			public static readonly DeclaracaoEstadoCivil OUTROS = new DeclaracaoEstadoCivil(6, "OUTROS");

			private readonly int id;
			private readonly string descricao;
			private DeclaracaoEstadoCivil(int id, string descricao) {
				this.id = id;
				this.descricao = descricao;
			}

			public int Id { get { return id; } }
			public string Descricao { get { return descricao; } }

			public static IEnumerable<DeclaracaoEstadoCivil> Values {
				get {
					yield return SOLTEIRO;
					yield return CASADO;
					yield return SEPARADO;
					yield return DIVORCIADO;
					yield return VIUVO;
					yield return OUTROS;
				}
			}


			public static DeclaracaoEstadoCivil Find(int id) {
				foreach (DeclaracaoEstadoCivil p in Values) {
					if (p.Id == id)
						return p;
				}
				return null;
			}

			public string ToISA() {
				return ToISA(this);
			}

			public static string ToISA(Dados.DeclaracaoEstadoCivil ec) {
				/*
				 * 1 - CASADO
					2 - DESQUITADO/SEPARADO
					3 - OUTROS
					4 - SOLTEIRO
					5 - VIÚVO
					6 - NÃO INFORMADO
					7 - DIVORCIADO
					A - LISTA/ITEM INVALIDO!
				*/

				if (ec == DeclaracaoEstadoCivil.CASADO)
					return "1";
				if (ec == DeclaracaoEstadoCivil.SEPARADO)
					return "2";
				if (ec == DeclaracaoEstadoCivil.OUTROS)
					return "3";
				if (ec == DeclaracaoEstadoCivil.SOLTEIRO)
					return "4";
				if (ec == DeclaracaoEstadoCivil.VIUVO)
					return "5";
				if (ec == DeclaracaoEstadoCivil.DIVORCIADO)
					return "7";

				return "6";
			}
		}

		[Serializable]
		public class DeclaracaoParentesco {
			public static readonly DeclaracaoParentesco CONJUGE = new DeclaracaoParentesco(1, "CÔNJUGE", new Produto[] { Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MAIS_VIDA_CEA });
			public static readonly DeclaracaoParentesco COMPANHEIRO = new DeclaracaoParentesco(2, "COMPANHEIRO(A)", new Produto[] { Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MAIS_VIDA_CEA });
			public static readonly DeclaracaoParentesco FILHO = new DeclaracaoParentesco(3, "FILHO(A)", new Produto[] { Produto.FAMILIA, Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MAIS_VIDA_CEA });
			public static readonly DeclaracaoParentesco ENTEADO = new DeclaracaoParentesco(4, "ENTEADO(A)", new Produto[] { Produto.FAMILIA, Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MAIS_VIDA_CEA });
			public static readonly DeclaracaoParentesco CURATELADO = new DeclaracaoParentesco(5, "CURATELADO(A)", new Produto[] { Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MAIS_VIDA_CEA });
			public static readonly DeclaracaoParentesco MENOR_ADOCAO = new DeclaracaoParentesco(6, "MENOR EM ADOÇÃO", new Produto[] { Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA });
			public static readonly DeclaracaoParentesco DEPENDENTE_ESPECIAL = new DeclaracaoParentesco(7, "DEPENDENTE ESPECIAL", new Produto[] { Produto.PPRS, Produto.PPRS_ELETRONORTE, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA });
			public static readonly DeclaracaoParentesco GENITOR = new DeclaracaoParentesco(8, "GENITOR(A)", new Produto[] { Produto.FAMILIA, Produto.PPRS, Produto.PPRS_ELETRONORTE });
			public static readonly DeclaracaoParentesco CUNHADO = new DeclaracaoParentesco(9, "CUNHADO(A)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco IRMAO = new DeclaracaoParentesco(10, "IRMÃO(Ã)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco AVO = new DeclaracaoParentesco(11, "AVÔ(Ó)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco SOGRO = new DeclaracaoParentesco(12, "SOGRO(A)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco TIO = new DeclaracaoParentesco(13, "TIO(A)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco BISAVO = new DeclaracaoParentesco(14, "BISAVÔ(Ó)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco NETO = new DeclaracaoParentesco(15, "NETO(A)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco SOBRINHO = new DeclaracaoParentesco(16, "SOBRINHO(A)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco GENRO = new DeclaracaoParentesco(17, "GENRO / NORA", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco BISNETO = new DeclaracaoParentesco(18, "BISNETO(A)", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco GENITOR_PID_PPRS = new DeclaracaoParentesco(19, "GENITOR(A) (Exclusivo para o PID-Inscrito no PPRS)", new Produto[] { Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA });
			public static readonly DeclaracaoParentesco CONJUGE_APOS_INVALIDEZ = new DeclaracaoParentesco(20, "CÔNJUGE – Exclusiv. para Aposentado por Invalidez", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco COMPANHEIRO_APOS_INVALIDEZ = new DeclaracaoParentesco(21, "COMPANHEIRO(A) – Exclusiv. para Aposentado por Invalidez", new Produto[] { Produto.FAMILIA });
			public static readonly DeclaracaoParentesco TUTELADO = new DeclaracaoParentesco(22, "TUTELADO(A)", new Produto[] { Produto.MAIS_VIDA_CEA });
			public static readonly DeclaracaoParentesco MENOR_GUARDA_JUDICIAL = new DeclaracaoParentesco(23, "MENOR SOB GUARDA JUDICIAL", new Produto[] { Produto.MAIS_VIDA_CEA });

			private DeclaracaoParentesco(int id, string descricao, Produto[] produtos) {
				this.Id = id;
				this.Descricao = descricao;
				this.Produtos = produtos;
			}

			public int Id { get; private set; }
			public string Descricao { get; private set; }
			public Produto[] Produtos { get; private set; }

			public static IEnumerable<DeclaracaoParentesco> ValuesByProduto(Produto prod) {
				return Values.Where(p => p.Produtos.Contains(prod));
			}

			private static IEnumerable<DeclaracaoParentesco> Values {
				get {
					yield return CONJUGE;
					yield return COMPANHEIRO;
					yield return FILHO;
					yield return ENTEADO;
					yield return CURATELADO;
					yield return MENOR_ADOCAO;
					yield return DEPENDENTE_ESPECIAL;
					yield return GENITOR;
					yield return CUNHADO;
					yield return IRMAO;
					yield return AVO;
					yield return SOGRO;
					yield return TIO;
					yield return BISAVO;
					yield return NETO;
					yield return SOBRINHO;
					yield return GENRO;
					yield return BISNETO;
					yield return GENITOR_PID_PPRS;
					yield return CONJUGE_APOS_INVALIDEZ;
					yield return COMPANHEIRO_APOS_INVALIDEZ;
					yield return TUTELADO;
					yield return MENOR_GUARDA_JUDICIAL;
				}
			}

			public static DeclaracaoParentesco Find(int id) {
				foreach (DeclaracaoParentesco p in Values) {
					if (p.Id == id)
						return p;
				}
				return null;
			}

			public string ToISA(Sexo sexo) {
				return ToISA(this, sexo);
			}

			private static string ToISA(DeclaracaoParentesco p, Sexo sexo) {
				/*
					 CD_PARENTESCO 	 DS_PARENTESCO 
					11	Pai
					12	Mae
					1	Conjuge
					2	Filho(a)
					5	Menor sob tutela
					3	Companheiro(a)
					4	Irmao(a)
					8	Menor pobre
					7	Neto(a)
					9	Bisneto(o)
					6	Enteado
					19	Designado(o)
					14	Tio(a)
					15	Menor Sob Guarda
					16	Sobrinho
					10	Ex-conjuge
					17	Ex-companheiro(a)
					18	Ex-enteado
					99	Outros
					9901	Pensionista
					9902	Pensionista 2
					98	Outros - PPRS
					13	Avo(o)
					26	CURATELADO (A)
					24	Cônjuge - Exclusiv. para Aposentado por Invalidez
					25	Companheiro(a) - Exclusiv. para Aposentado por Invalidez
					9903	GENITOR (A) (EXCLUSIVO PID - INSCRITO NO PPRS)
					20	Bisavô(ó)
					21	Cunhado(a)
					22	Sogro(a)
					23	Genro/Nora
				*/
				if (p == GENITOR) {
					if (sexo == Sexo.FEMININO) return "12";
					return "11";
				}

				if (p == CONJUGE) return "1";
				if (p == FILHO) return "2";
				if (p == COMPANHEIRO) return "3";
				if (p == IRMAO) return "4";
				if (p == NETO) return "7";
				if (p == BISNETO) return "9";
				if (p == ENTEADO) return "6";
				if (p == TIO) return "14";
				if (p == CURATELADO) return "26";
				if (p == SOBRINHO) return "16";
				if (p == AVO) return "13";
				if (p == CONJUGE_APOS_INVALIDEZ) return "24";
				if (p == GENITOR_PID_PPRS) return "9903";


				if (p == MENOR_ADOCAO) return "26";
				if (p == DEPENDENTE_ESPECIAL) return "99";
				if (p == CUNHADO) return "21";
				if (p == SOGRO) return "22";
				if (p == BISAVO) return "20";
				if (p == GENRO) return "23";
				if (p == COMPANHEIRO_APOS_INVALIDEZ) return "25";
				if (p == TUTELADO) return "5";
				if (p == MENOR_GUARDA_JUDICIAL) return "15";

				return null;
			}

			internal bool IsConjuge() {
				return this == CONJUGE || this == COMPANHEIRO;
			}
		}
	}
}
