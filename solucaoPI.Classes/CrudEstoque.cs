using System;
using System.Collections.Generic;
using System.IO;

namespace ControleEstoque
{
    public class CrudEstoque
    {
        private Estoque estoque;
        private string arquivoEstoque;
        private string arquivoMovimentacoes;

        public CrudEstoque(string arquivoEstoque)
        {
            estoque = new Estoque(arquivoEstoque);
            this.arquivoEstoque = arquivoEstoque;
            this.arquivoMovimentacoes = arquivoMovimentacoes;
        }


        //public List<Produto> ListarProdutos()
        //{
        //return estoque.GetProdutos(); 
        //}

        public void AdicionarProduto(Produto produto)
        {
            bool idExisteNoArquivo = false;
            try
            {
                string[] linhas = File.ReadAllLines(arquivoEstoque);
                foreach (string linha in linhas)
                {
                    if (linha.Contains($"ID: {produto.Id}"))
                    {
                        idExisteNoArquivo = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar ID do produto no arquivo: {ex.Message}");
            }

            if (idExisteNoArquivo)
            {
                int novoId = estoque.GetProdutos().Max(p => p.Id) + 1;
                Produto novoProduto = new Produto(produto.NomeProduto, novoId)
                {
                    Quantidade = produto.Quantidade,
                    VolumeMinimo = produto.VolumeMinimo,
                    VolumeMaximo = produto.VolumeMaximo,
                    ValidadeProduto = produto.ValidadeProduto
                };
                produto = novoProduto;

                Produto.AtualizarProximoId(novoId + 1);
            }

            Produto produtoExistente = estoque.GetProdutos().Find(p => p.Equals(produto));

            if (produtoExistente != null)
            {
                estoque.SalvarMovimentacao(produtoExistente);
            }
            else
            {
                estoque.GetProdutos().Add(produto);
                AdicionarMovimentacao(produto);
            }
        }


        public void RemoverProduto(int id)
        {
            Produto produto = ConsultarProdutoCrud(id);
            if (produto != null)
            {
                estoque.GetProdutos().Remove(produto);
                AdicionarMovimentacao(produto, true);
            }
            else
            {
                throw new InvalidOperationException("Produto não encontrado no estoque.");
            }
        }


        public Produto ConsultarProdutoCrud(int idProduto)
        {
            Produto produto = estoque.ConsultarProduto(idProduto);
            if (produto != null)
            {
                produto.Quantidade = produto.CalcularQuantidadeAtual();
                return produto;
            }

            return null;
        }

        public string ConsultarProdutoNoArquivo(int idProduto)
        {
            if (File.Exists(arquivoEstoque))
            {
                string[] linhas = File.ReadAllLines(arquivoEstoque);
                foreach (string linha in linhas)
                {
                    if (linha.Contains($"ID: {idProduto}"))
                    {
                        return linha;
                    }
                }
            }
            return null;
        }



        public Produto ConsultarProdutoPorNome(string nomeProduto)
        {
            foreach (Produto produto in estoque.GetProdutos())
            {
                if (produto.NomeProduto.Equals(nomeProduto, StringComparison.OrdinalIgnoreCase))
                {
                    return produto;
                }
            }
            return null;
        }


        public void AdicionarMovimentacao(Produto produto, bool isSaida = false)
        {
            string tipoMovimentacao = isSaida ? "Saída" : "Entrada";
            string movimentacao = $"{DateTime.Now}: {tipoMovimentacao} - ID: {produto.Id}, Nome: {produto.NomeProduto}, Quantidade Restante: {produto.Quantidade}, Quantidade Minima: {produto.VolumeMinimo}, Validade: {produto.ValidadeProduto.ToString("dd/MM/yyyy")}";

            try
            {
                List<string> linhas = File.ReadAllLines(arquivoEstoque).ToList();
                linhas.Add(movimentacao);
                File.WriteAllLines(arquivoEstoque, linhas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar movimentação: {ex.Message}");
            }
        }

    }
}