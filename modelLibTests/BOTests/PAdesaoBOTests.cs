using eVidaGeneralLib.BO;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.VO.Protheus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace modelLibTests.BOTests
{
    [TestClass]
    public class PAdesaoBOTests
    {
        PAdesaoBO pAdesaoBo = new PAdesaoBO();
        PDeclaracaoDependenteVO pDeclaracaoDependente = new PDeclaracaoDependenteVO();
        PUsuarioVO pUsuario = new PUsuarioVO();
        PDeclaracaoVO pDeclaracao = new PDeclaracaoVO();
        PVidaVO pVida = new PVidaVO();

        [TestMethod]
        public void ValidaIsSameQuandoVazio()
        {
            var sameUsuDep = pAdesaoBo.IsSameUsuarioDependente(pDeclaracaoDependente, pUsuario);

            var sameVidaTit = pAdesaoBo.IsSameVidaTitular(pDeclaracao, pVida);

            var sameVitaDep = pAdesaoBo.IsSameVidaDependente(pDeclaracaoDependente, pVida);

            var sameVitaTitCns = pAdesaoBo.IsSameVidaTitularCns(pDeclaracao, pVida);

            var sameVitaDepCns = pAdesaoBo.IsSameVidaDependenteCns(pDeclaracaoDependente, pVida);

            var sameUsuTit = pAdesaoBo.IsSameUsuarioTitular(pDeclaracao, pUsuario);

            Assert.IsFalse(sameUsuDep);
            Assert.IsFalse(sameVidaTit);
            Assert.IsFalse(sameVitaDep);
            Assert.IsFalse(sameVitaTitCns);
            Assert.IsFalse(sameVitaDepCns);
            Assert.IsFalse(sameUsuTit);

        }

        [TestMethod]
        public void ValidaIsSameUsuarioDependenteQuandoDuplicado()
        {

            var dec = new PDeclaracaoDependenteVO()
            {
                Nome = "Adriana Borges", Cpf = "02547718740", Rg = "100656461" };

            var usu = new PUsuarioVO()
            {
                Nomusr = "Adriana Borges", Cpfusr = "02547718125", Drgusr = "100656461"
            };

            var sameUsuDep = pAdesaoBo.IsSameUsuarioDependente(dec, usu);

            Assert.IsFalse(sameUsuDep);

        }

        [TestMethod]
        public void ValidaIsSameUsuarioDependenteParaInclusao()
        {

            var pessoa = new PPessoaVO()
            {
                    Nome = "Eros Luis", Cpf = "92745318740", Rg = "140756461"
            };

            PDeclaracaoVO declaracao = new PDeclaracaoVO() {Titular = pessoa};

            var usu = new PUsuarioVO()
            {
                Nomusr = "Adriana Borges", Cpfusr = "02547718125", Drgusr = "100656461"
            };

            var sameUsuDep = pAdesaoBo.IsSameUsuarioTitular(declaracao, usu);

            Assert.IsTrue(sameUsuDep);

        }

        [TestMethod]
        public void IsSameVidaDependenteCnsQuandoValida()
        {

            var pessoa = new PPessoaVO()
            {
                Nome = "Eros Luis", Cpf = "92745318740", Cns = "140756461"
            };

            PDeclaracaoVO declaracao = new PDeclaracaoVO() { Titular = pessoa };

            var usu = new PUsuarioVO()
            {
                Nomusr = "Adriana Borges", Cpfusr = "02547718125", Drgusr = "100656461"
            };

            var sameUsuDep = pAdesaoBo.IsSameUsuarioTitular(declaracao, usu);

            Assert.IsTrue(sameUsuDep);

        }

        [TestMethod]
        public void IsSameVidaDependenteCnsQuandoInValida()
        {

            var pessoa = new PPessoaVO()
            {
                Nome = "Adriana Borges",
                Cpf = "92745318740",
                Rg = "140756461"
            };

            PDeclaracaoVO declaracao = new PDeclaracaoVO() { Titular = pessoa };

            var usu = new PUsuarioVO()
            {
                Nomusr = "Adriana Borges",
                Cpfusr = "02547718125",
                Drgusr = "100656461"
            };

            var sameUsuDep = pAdesaoBo.IsSameUsuarioTitular(declaracao, usu);

            Assert.IsFalse(sameUsuDep);

        }

        [TestMethod]
        public void IsSameVidaTitularQuandoValida()
        {

            var pessoa = new PPessoaVO()
            {
                Nome = "Adriana Borges", Cpf = "92745318740"
            };

            PDeclaracaoVO declaracao = new PDeclaracaoVO() { Titular = pessoa };

            var vid = new PVidaVO()
            {
                Nomusr = "Adriana Borges", Cpfusr = "02547718125"
            };

            var sameUsuDep = pAdesaoBo.IsSameVidaTitular(declaracao, vid);

            Assert.IsFalse(sameUsuDep);

        }

        [TestMethod]
        public void IsSameVidaTitularQuandoInValida()
        {

            var pessoa = new PPessoaVO()
            {
                Nome = "Adriana Borges",
                Cpf = "02547718740"
            };

            PDeclaracaoVO declaracao = new PDeclaracaoVO() { Titular = pessoa };

            var vid = new PVidaVO()
            {
                Nomusr = "Adriana Borges",
                Cpfusr = "02547718740"
            };

            var sameUsuDep = pAdesaoBo.IsSameVidaTitular(declaracao, vid);

            Assert.IsFalse(sameUsuDep);

        }
    }
}
