using Converte_Object_Json.Teste;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Converte_Object_Json
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnObjectJson_Click(object sender, EventArgs e)
        {
            //cria objeto com valores
            Produto produto = new Produto
            {
                ProdutoID = 1001,
                ProdutoNome = "Samsung Galaxy III",
                Categoria = "Celular",
                Preco = 699.00
            };

            //lista de objetos
            List<Produto> produtos = new List<Produto>
            {
                new Produto(){ ProdutoID = 1002, ProdutoNome= "IPhone", Categoria="Celular",  Preco=1999.00 },
                new Produto(){ ProdutoID = 1003, ProdutoNome= "IPad Apple", Categoria="Tablet",  Preco=2999.00 },
                new Produto(){ ProdutoID = 1004, ProdutoNome= "SamSung Galaxy 5S", Categoria="Celular",  Preco=2599.00 }
            };

            JsonConversao jsonconv = new JsonConversao();
            txtResultado.Text = jsonconv.ConverteObjectParaJSon(produto);
            //txtResultado.Text = txtResultado.Text +  jsonconv.ConverteObjectParaJSon(produtos);
        }

        private void btnJsonObject_Click(object sender, EventArgs e)
        {
            JsonConversao jsonconv = new JsonConversao();

            //lista de objetos
            List<Aluno> alunos = new List<Aluno>
            {
                new Aluno(){ Nome = "Jose Carlos", Sobrenome="Macoratti" },
                new Aluno(){ Nome = "Paulo", Sobrenome="Silveira" }
            };

            string strAlunos = jsonconv.ConverteObjectParaJSon<List<Aluno>>(alunos);
            alunos = jsonconv.ConverteJSonParaObject<List<Aluno>>(strAlunos);
            foreach (var aluno in alunos)
            {
                txtResultado2.Text += aluno.Nome + " " + aluno.Sobrenome + Environment.NewLine;
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
