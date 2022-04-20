using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVidaGeneralLib.VO.Adesao
{
    public class PDados
    {
        public enum Empresa
        {
            ELETRONORTE = PConstantes.EMPRESA_ELETRONORTE,
            CEA = PConstantes.EMPRESA_CEA,
            EVIDA = PConstantes.EMPRESA_EVIDA,
            AMAZONASGT = PConstantes.EMPRESA_AMAZONASGT,
            AMAZONASD = PConstantes.EMPRESA_AMAZONASD
        }

        public enum Sexo
        {
            MASCULINO = '1',
            FEMININO = '2'
        }

        public enum SituacaoDeclaracao
        {
            PENDENTE = 0,
            RECEBIDA = 1,
            VALIDADA = 2,
            INVALIDA = 3,
            INTEGRADA = 4
        }

        public class EnumTradutor
        {
            public static string TraduzEmpresa(Empresa empresa)
            {
                switch (empresa)
                {
                    case Empresa.ELETRONORTE: return "ELETRONORTE";
                    case Empresa.CEA: return "CEA";
                    case Empresa.EVIDA: return "E-VIDA";
                    case Empresa.AMAZONASGT: return "AMAZONAS GT";
                    case Empresa.AMAZONASD: return "AMAZONAS DISTRIBUIÇÃO";
                    default: return "--";
                }
            }
        }

        [Serializable]
        public class Produto
        {

            public static readonly Produto MAIS_VIDA_EVIDA_MASTER = new Produto("MVEVIDAMASTER",
                "E-VIDA MASTER",
                "rptVidaEvidaMaster",
                "MaisVidaEvidaMaster.aspx",
                PDados.Empresa.EVIDA);

            public static readonly Produto PPRS_ELETRONORTE = new Produto("PE", "PPRS (PARTICIPATIVO)", "rptPPRSEletronorte", "PprsEletronorte.aspx", PDados.Empresa.ELETRONORTE);
            public static readonly Produto MELHOR_IDADE = new Produto("MI", "E-VIDA MELHOR IDADE (APOSENTADOS/EX-EMPREGADOS)", "rptMelhorIdade", "MelhorIdade.aspx", PDados.Empresa.ELETRONORTE);
            public static readonly Produto FAMILIA = new Produto("F", "E-VIDA FAMÍLIA", "rptFamilia", "Familia.aspx", PDados.Empresa.ELETRONORTE);
            public static readonly Produto FAMILIA_CEA = new Produto("FAMCEA", "E-VIDA FAMÍLIA", "rptFamiliaCea", "FamiliaCea.aspx", PDados.Empresa.CEA);
            public static readonly Produto FAMILIA_EVIDA = new Produto("FAMEVIDA", "E-VIDA FAMÍLIA", "rptFamiliaEvida", "FamiliaEvida.aspx", PDados.Empresa.EVIDA);
            public static readonly Produto FAMILIA_AMAZONASGT = new Produto("FAMAGT", "E-VIDA FAMÍLIA", "rptFamiliaAmGt", "FamiliaAmazonasGt.aspx", PDados.Empresa.AMAZONASGT);
            public static readonly Produto PPRS = new Produto("P", "E-VIDA PPRS (CONTRIBUTIVO)", "rptPPRS", "Pprs.aspx", PDados.Empresa.ELETRONORTE);
            public static readonly Produto DEPENDENTE = new Produto("DE", "DEPENDENTE", "rptDependente", "Dependente.aspx", PDados.Empresa.ELETRONORTE);
            public static readonly Produto MELHOR_IDADE_LICENCA = new Produto("ML", "E-VIDA MELHOR IDADE (EMPREGADOS EM LICENÇA SEM VENCIMENTO NA MANTENEDORA)", "rptMelhorIdadeLic", "MelhorIdade.aspx?LIC=Y", PDados.Empresa.ELETRONORTE);
            public static readonly Produto MAIS_VIDA_CEA = new Produto("MVC", "CEA +VIDA", "rptVidaCea", "MaisVidaCea.aspx", PDados.Empresa.CEA);
            public static readonly Produto PPRS_AMAZONASGT = new Produto("PRA", "E-VIDA Amazonas GT", "rptPPRSAmGt", "PprsAmazonasGt.aspx", PDados.Empresa.AMAZONASGT);
            public static readonly Produto PPRS_AMAZONASD = new Produto("PPRSAMD", "E-VIDA PPRS AmD", "rptPPRSAmD", "PprsAmazonasD.aspx", PDados.Empresa.AMAZONASD);

            public static readonly Produto MAIS_VIDA_EVIDA = new Produto("MVEVIDA",
                "+VIDA",
                "rptVidaEvida",
                "MaisVidaEvida.aspx",
                PDados.Empresa.EVIDA);

            public static readonly Produto DEPENDENTE_AMAZONASGT = new Produto("DEAGT", "DEPENDENTE", "rptDependente", "Dependente.aspx", PDados.Empresa.AMAZONASGT);
            public static readonly Produto DEPENDENTE_AMAZONASD = new Produto("DEAMD", "DEPENDENTE", "rptDependente", "Dependente.aspx", PDados.Empresa.AMAZONASD);
            public static readonly Produto DEPENDENTE_CEA = new Produto("DECEA", "DEPENDENTE", "rptDependente", "Dependente.aspx", PDados.Empresa.CEA);
            public static readonly Produto DEPENDENTE_EVIDA = new Produto("DEEVIDA", "DEPENDENTE", "rptDependente", "Dependente.aspx", PDados.Empresa.EVIDA);

            public static readonly Produto MELHOR_IDADE_AMAZONASGT = new Produto("MIAGT", "E-VIDA MELHOR IDADE (APOSENTADOS/EX-EMPREGADOS)", "rptMelhorIdadeAmgt", "MelhorIdadeAmgt.aspx", PDados.Empresa.AMAZONASGT);
            public static readonly Produto MELHOR_IDADE_LICENCA_AMAZONASGT = new Produto("MLAGT", "E-VIDA MELHOR IDADE (EMPREGADOS EM LICENÇA SEM VENCIMENTO NA MANTENEDORA)", "rptMelhorIdadeLicAmgt", "MelhorIdadeAmgt.aspx?LIC=Y", PDados.Empresa.AMAZONASGT);

            public static readonly Produto ALL_PPRS = new Produto("PPRS_ALL", "-", "-", "- ", PDados.Empresa.ELETRONORTE);
            public static readonly Produto[] ALL_PPRS_ARR = new Produto[] { PPRS_ELETRONORTE, PPRS, PPRS_AMAZONASGT, PPRS_AMAZONASD, MAIS_VIDA_EVIDA, MAIS_VIDA_EVIDA_MASTER, MAIS_VIDA_CEA };

            // Careful precisa ser o último, atencao no update
            public static readonly Produto[] PLANOS_INTEGRACAO = new Produto[] { MAIS_VIDA_CEA, PPRS_AMAZONASD, PPRS_AMAZONASGT, MAIS_VIDA_EVIDA, MAIS_VIDA_EVIDA_MASTER, MELHOR_IDADE_AMAZONASGT, MELHOR_IDADE_LICENCA_AMAZONASGT, PPRS, FAMILIA, FAMILIA_AMAZONASGT };

            private Produto(string id, string descricao, string rpt, string pagina, PDados.Empresa empresa)
            {
                this.Id = id;
                this.Descricao = descricao;
                this.ReportFile = rpt;
                this.Empresa = empresa;
                this.Pagina = pagina;
            }

            public string Id { get; private set; }
            public string Descricao { get; private set; }
            public string ReportFile { get; private set; }
            public PDados.Empresa Empresa { get; private set; }
            public string Pagina { get; private set; }

            public static IEnumerable<Produto> Values
            {
                get
                {
                    yield return PPRS_ELETRONORTE;
                    yield return PPRS;
                    yield return MELHOR_IDADE;
                    yield return FAMILIA;
                    yield return FAMILIA_CEA;
                    yield return FAMILIA_EVIDA;
                    yield return FAMILIA_AMAZONASGT;
                    yield return DEPENDENTE;
                    yield return MELHOR_IDADE_LICENCA;
                    yield return MAIS_VIDA_CEA;
                    yield return PPRS_AMAZONASGT;
                    yield return PPRS_AMAZONASD;
                    yield return MAIS_VIDA_EVIDA;
                    yield return MAIS_VIDA_EVIDA_MASTER;
                    yield return MELHOR_IDADE_AMAZONASGT;
                    yield return MELHOR_IDADE_LICENCA_AMAZONASGT;	
                    yield return DEPENDENTE_AMAZONASGT;
                    yield return DEPENDENTE_AMAZONASD;
                    yield return DEPENDENTE_CEA;
                    yield return DEPENDENTE_EVIDA;
                }
            }

            public static Produto Find(string id)
            {
                foreach (Produto p in Values)
                {
                    if (p.Id.Equals(id.Trim()))
                        return p;
                }
                return null;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                Produto p = (Produto)obj;
                return this.Id.Equals(p.Id);
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                return hash;
            }

            public int ToProtheus()
            {
                if (this.Id.Equals(MAIS_VIDA_CEA.Id)) return PConstantes.PLANO_MAIS_VIDA_CEA;
                if (this.Id.Equals(PPRS.Id)) return PConstantes.PLANO_EVIDA_PPRS;
                if (this.Id.Equals(PPRS_ELETRONORTE.Id)) return PConstantes.PLANO_EVIDA_PPRS;
                if (this.Id.Equals(MELHOR_IDADE.Id)) return PConstantes.PLANO_EVIDA_MELHOR_IDADE;
                if (this.Id.Equals(MELHOR_IDADE_LICENCA.Id)) return PConstantes.PLANO_EVIDA_MELHOR_IDADE;
                if (this.Id.Equals(MELHOR_IDADE_AMAZONASGT.Id)) return PConstantes.PLANO_EVIDA_MELHOR_IDADE;
                if (this.Id.Equals(MELHOR_IDADE_LICENCA_AMAZONASGT.Id)) return PConstantes.PLANO_EVIDA_MELHOR_IDADE; 
                if (this.Id.Equals(FAMILIA.Id)) return PConstantes.PLANO_EVIDA_FAMILIA;
                if (this.Id.Equals(FAMILIA_AMAZONASGT.Id)) return PConstantes.PLANO_EVIDA_FAMILIA;
                if (this.Id.Equals(PPRS_AMAZONASGT.Id)) return PConstantes.PLANO_EVIDA_PPRS_AMAZONASGT;
                if (this.Id.Equals(PPRS_AMAZONASD.Id)) return PConstantes.PLANO_EVIDA_PPRS_AMAZONASD;
                if (this.Id.Equals(MAIS_VIDA_EVIDA.Id)) return PConstantes.PLANO_MAIS_VIDA_EVIDA;

                if (Id.Equals(MAIS_VIDA_EVIDA_MASTER.Id))
                    return PConstantes.PLANO_MAIS_VIDA_EVIDA_MASTER;


                throw new InvalidOperationException("Produto " + this.Id + " não mapeado para plano do Protheus");
            }

        }

        [Serializable]
        public class DeclaracaoEstadoCivil
        {
            public static readonly DeclaracaoEstadoCivil CASADO = new DeclaracaoEstadoCivil("C", "CASADO(A)");
            public static readonly DeclaracaoEstadoCivil DIVORCIADO = new DeclaracaoEstadoCivil("D", "DIVORCIADO(A)");
            public static readonly DeclaracaoEstadoCivil MARITAL = new DeclaracaoEstadoCivil("M", "MARITAL");
            public static readonly DeclaracaoEstadoCivil DESQUITADO = new DeclaracaoEstadoCivil("Q", "DESQUITADO(A)");
            public static readonly DeclaracaoEstadoCivil SOLTEIRO = new DeclaracaoEstadoCivil("S", "SOLTEIRO(A)");
            public static readonly DeclaracaoEstadoCivil VIUVO = new DeclaracaoEstadoCivil("V", "VIUVO(A)");

            private readonly string id;
            private readonly string descricao;
            private DeclaracaoEstadoCivil(string id, string descricao)
            {
                this.id = id;
                this.descricao = descricao;
            }

            public string Id { get { return id; } }
            public string Descricao { get { return descricao; } }

            public static IEnumerable<DeclaracaoEstadoCivil> Values
            {
                get
                {
                    yield return CASADO;
                    yield return DIVORCIADO;
                    yield return MARITAL;
                    yield return DESQUITADO;
                    yield return SOLTEIRO;
                    yield return VIUVO;
                }
            }

            public static DeclaracaoEstadoCivil Find(string id)
            {
                foreach (DeclaracaoEstadoCivil p in Values)
                {
                    if (p.Id == id)
                        return p;
                }
                return null;
            }

            public string ToProtheus()
            {
                return ToProtheus(this);
            }

            public static string ToProtheus(PDados.DeclaracaoEstadoCivil ec)
            {

                if (ec == DeclaracaoEstadoCivil.CASADO)
                    return "C";
                if (ec == DeclaracaoEstadoCivil.DIVORCIADO)
                    return "D";
                if (ec == DeclaracaoEstadoCivil.MARITAL)
                    return "M";
                if (ec == DeclaracaoEstadoCivil.DESQUITADO)
                    return "Q";
                if (ec == DeclaracaoEstadoCivil.SOLTEIRO)
                    return "S";
                if (ec == DeclaracaoEstadoCivil.VIUVO)
                    return "V";

                return "";
            }


        }

        [Serializable]
        public class DeclaracaoParentesco
        {
            public static readonly DeclaracaoParentesco BENEFICIARIO_TITULAR = new DeclaracaoParentesco(1, "BENEFICIÁRIO TITULAR", new Produto[] { Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco CONJUGE_COMPANHEIRO = new DeclaracaoParentesco(3, "CÔNJUGE / COMPANHEIRO(A)", new Produto[] { Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco FILHO = new DeclaracaoParentesco(4, "FILHO", new Produto[] { Produto.FAMILIA, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco FILHA = new DeclaracaoParentesco(5, "FILHA", new Produto[] { Produto.FAMILIA, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco ENTEADO = new DeclaracaoParentesco(6, "ENTEADO", new Produto[] { Produto.FAMILIA, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco ENTEADA = new DeclaracaoParentesco(7, "ENTEADA", new Produto[] { Produto.FAMILIA, Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_LICENCA, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco PAI = new DeclaracaoParentesco(8, "PAI", new Produto[] { Produto.FAMILIA, Produto.PPRS_ELETRONORTE, Produto.PPRS, Produto.PPRS_AMAZONASGT, Produto.PPRS_AMAZONASD, Produto.MAIS_VIDA_CEA });
            public static readonly DeclaracaoParentesco MAE = new DeclaracaoParentesco(9, "MÃE", new Produto[] { Produto.FAMILIA, Produto.PPRS_ELETRONORTE, Produto.PPRS, Produto.PPRS_AMAZONASGT, Produto.PPRS_AMAZONASD, Produto.MAIS_VIDA_CEA });
            public static readonly DeclaracaoParentesco AGREGADO_OUTROS = new DeclaracaoParentesco(10, "AGREGADO/OUTROS", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco MENOR_SOB_TUTELA = new DeclaracaoParentesco(11, "MENOR SOB TUTELA", new Produto[] { Produto.MAIS_VIDA_CEA, Produto.PPRS_AMAZONASD });
            public static readonly DeclaracaoParentesco IRMAO = new DeclaracaoParentesco(12, "IRMÃO(Ã)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco MENOR_POBRE = new DeclaracaoParentesco(13, "MENOR POBRE", new Produto[] { Produto.MAIS_VIDA_CEA, Produto.PPRS_AMAZONASD });
            public static readonly DeclaracaoParentesco NETO = new DeclaracaoParentesco(14, "NETO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco BISNETO = new DeclaracaoParentesco(15, "BISNETO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco DESIGNADO = new DeclaracaoParentesco(16, "DESIGNADO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco TIO = new DeclaracaoParentesco(17, "TIO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco MENOR_SOB_GUARDA = new DeclaracaoParentesco(18, "MENOR SOB GUARDA", new Produto[] { Produto.MAIS_VIDA_CEA });
            public static readonly DeclaracaoParentesco SOBRINHO = new DeclaracaoParentesco(19, "SOBRINHO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco AVO = new DeclaracaoParentesco(20, "AVÔ(Ó)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco CONJUGE_COMPANHEIRO_APOS_INVALIDEZ = new DeclaracaoParentesco(21, "CÔNJUGE/COMPANHEIRO(A) APÓS INVALIDEZ", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco GENITORES_PID = new DeclaracaoParentesco(22, "GENITORES (PID)", new Produto[] { Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA });
            public static readonly DeclaracaoParentesco BISAVO = new DeclaracaoParentesco(23, "BISAVÔ(Ó)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco CUNHADO = new DeclaracaoParentesco(24, "CUNHADO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco SOGRO = new DeclaracaoParentesco(25, "SOGRO(A)", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco GENRO = new DeclaracaoParentesco(26, "GENRO / NORA", new Produto[] { Produto.FAMILIA });
            public static readonly DeclaracaoParentesco CURATELADO = new DeclaracaoParentesco(27, "CURATELADO(A)", new Produto[] { Produto.MELHOR_IDADE, Produto.MELHOR_IDADE_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA_AMAZONASGT, Produto.MELHOR_IDADE_LICENCA, Produto.ALL_PPRS });
            public static readonly DeclaracaoParentesco EXCONJUGE_EXCOMPANHEIRO = new DeclaracaoParentesco(28, "EX-CÔNJUGE / EX-COMPANHEIRO(A)", new Produto[] { Produto.FAMILIA });

            private DeclaracaoParentesco(int id, string descricao, Produto[] produtos)
            {
                this.Id = id;
                this.Descricao = descricao;
                this.Produtos = produtos;
            }

            public int Id { get; private set; }

            public string Descricao { get; private set; }

            public Produto[] Produtos { get; private set; }

            public static IEnumerable<DeclaracaoParentesco> ValuesByProduto(Produto prod)
            {
                return Values.Where(p => p.Produtos.Contains(prod));
            }

            private static IEnumerable<DeclaracaoParentesco> Values
            {
                get
                {
                    yield return BENEFICIARIO_TITULAR;
                    yield return CONJUGE_COMPANHEIRO;
                    yield return FILHO;
                    yield return FILHA;
                    yield return ENTEADO;
                    yield return ENTEADA;
                    yield return PAI;
                    yield return MAE;
                    yield return AGREGADO_OUTROS;
                    yield return MENOR_SOB_TUTELA;
                    yield return IRMAO;
                    yield return MENOR_POBRE;
                    yield return NETO;
                    yield return BISNETO;
                    yield return DESIGNADO;
                    yield return TIO;
                    yield return MENOR_SOB_GUARDA;
                    yield return SOBRINHO;
                    yield return AVO;
                    yield return CONJUGE_COMPANHEIRO_APOS_INVALIDEZ;
                    yield return GENITORES_PID;
                    yield return BISAVO;
                    yield return CUNHADO;
                    yield return SOGRO;
                    yield return GENRO;
                    yield return CURATELADO;
                    yield return EXCONJUGE_EXCOMPANHEIRO;
                }
            }

            public static DeclaracaoParentesco Find(int id)
            {
                if (id == 0) return null;

                foreach (DeclaracaoParentesco p in Values)
                {
                    if (p.Id == id)
                        return p;
                }
                return null;
            }

            public int ToProtheus()
            {
                return ToProtheus(this);
            }

            private static int ToProtheus(DeclaracaoParentesco p)
            {

                if (p == BENEFICIARIO_TITULAR) return 1;
                if (p == CONJUGE_COMPANHEIRO) return 3;
                if (p == FILHO) return 4;
                if (p == FILHA) return 5;
                if (p == ENTEADO) return 6;
                if (p == ENTEADA) return 7;
                if (p == PAI) return 8;
                if (p == MAE) return 9;
                if (p == AGREGADO_OUTROS) return 10;
                if (p == MENOR_SOB_TUTELA) return 11;
                if (p == IRMAO) return 12;
                if (p == MENOR_POBRE) return 13;
                if (p == NETO) return 14;
                if (p == BISNETO) return 15;
                if (p == DESIGNADO) return 16;
                if (p == TIO) return 17;
                if (p == MENOR_SOB_GUARDA) return 18;
                if (p == SOBRINHO) return 19;
                if (p == AVO) return 20;
                if (p == CONJUGE_COMPANHEIRO_APOS_INVALIDEZ) return 21;
                if (p == GENITORES_PID) return 22;
                if (p == BISAVO) return 23;
                if (p == CUNHADO) return 24;
                if (p == SOGRO) return 25;
                if (p == GENRO) return 26;
                if (p == CURATELADO) return 27;
                if (p == EXCONJUGE_EXCOMPANHEIRO) return 28;

                return 0;
            }

            internal bool IsConjuge()
            {
                return this == CONJUGE_COMPANHEIRO;
            }

        }

    }
}
