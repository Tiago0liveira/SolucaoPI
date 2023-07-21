using System;
using System.Collections.Generic;
using System.IO;

namespace ControleEstoque
{
    public class Estoque
    {
        private List<Produto> produtos;
        private string arquivoMovimentacoes;

        public Estoque(string arquivoMovimentacoes)
        {
            produtos = new List<Produto>();
            this.arquivoMovimentacoes = arquivoMovimentacoes;
        }


        public void AdicionarProduto(Produto produto)
        {
            Produto produtoExistente = produtos.Find(p => p.Equals(produto));

            if (produtoExistente != null)
            {
                produtoExistente.Quantidade += produto.Quantidade;
                produtoExistente.DataEntrada = produto.DataEntrada;
                SalvarMovimentacao(produtoExistente);

            }
            else
            {
                produtos.Add(produto);
                SalvarMovimentacao(produto);

            }


        }


        public void RemoverProduto(int id)
        {
            Produto produto = ConsultarProduto(id);
            if (produto != null)
            {
                produtos.Remove(produto);
                SalvarMovimentacao(produto, true);
            }
            else
            {
                throw new InvalidOperationException("Produto não encontrado no estoque.");
            }
        }


        public Produto ConsultarProduto(int idProduto)
        {
            foreach (Produto produto in produtos)
            {
                if (produto.Id == idProduto)
                {
                    produto.Quantidade = produto.CalcularQuantidadeAtual();
                    return produto;
                }
            }
            try
            {
                string[] linhas = File.ReadAllLines(arquivoMovimentacoes);
                foreach (string linha in linhas)
                {
                    if (linha.Contains($"ID: {idProduto}"))
                    {
                        string nomeProduto = linha.Split(',')[2].Split(':')[1].Trim();
                        Produto produto = new Produto(nomeProduto);
                        produtos.Add(produto);
                        return produto;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar produto: {ex.Message}");
            }

            return null;
        }


        public Produto ConsultarProdutoPorNome(string nomeProduto)
        {
            foreach (Produto produto in produtos)
            {
                if (produto.NomeProduto.Equals(nomeProduto, StringComparison.OrdinalIgnoreCase))
                {
                    return produto;
                }
            }
            return null;
        }

        public List<Produto> ListarProdutos()
        {
            return produtos;
        }

        public List<Produto> ListarProdutosComVolumeMinimoAtingido()
        {
            List<Produto> produtosComVolumeMinimoAtingido = new List<Produto>();
            foreach (Produto produto in produtos)
            {
                if (produto.Quantidade <= produto.VolumeMinimo)
                {
                    produtosComVolumeMinimoAtingido.Add(produto);
                }
            }
            return produtosComVolumeMinimoAtingido;
        }

        public void SalvarMovimentacao(Produto produto, bool isSaida = false)
        {
            string tipoMovimentacao = isSaida ? "Saída" : "Entrada";
            string movimentacao = $"{DateTime.Now}: {tipoMovimentacao} - ID: {produto.Id}, Nome: {produto.NomeProduto}, Quantidade Restante: {produto.Quantidade}, Quantidade Minima: {produto.VolumeMinimo}, Validade: {produto.ValidadeProduto.ToString("dd/MM/yyyy")}";

            try
            {
                List<string> linhas = File.ReadAllLines(arquivoMovimentacoes).ToList();
                int indiceLinha = linhas.FindIndex(l => l.Contains($"ID: {produto.Id}"));
                if (indiceLinha != -1)
                {
                    linhas[indiceLinha] = movimentacao;
                }
                else
                {
                    linhas.Add(movimentacao);
                }
                File.WriteAllLines(arquivoMovimentacoes, linhas);
                // File.AppendAllLines(arquivoMovimentacoes, new List<string> { movimentacao });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar movimentação: {ex.Message}");
            }
        }



    }
}