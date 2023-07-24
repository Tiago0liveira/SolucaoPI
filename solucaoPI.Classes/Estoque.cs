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
    
    bool idExisteNoArquivo = false;
    try
    {
        string[] linhas = File.ReadAllLines(arquivoMovimentacoes);
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
        int novoId = produtos.Max(p => p.Id) + 1;
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

    
    Produto produtoExistente = produtos.Find(p => p.Equals(produto));

    if (produtoExistente != null)
    {
        SalvarMovimentacao(produtoExistente);
    }
    else
    {
        produtos.Add(produto);
        AdicionarMovimentacao(produto);
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

        public string ConsultarProdutoNoArquivo(int idProduto)
{
        if (File.Exists(arquivoMovimentacoes))
        {
            string[] linhas = File.ReadAllLines(arquivoMovimentacoes);
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

        
        
        public void CarregarProdutostxt()
{
    if (File.Exists(arquivoMovimentacoes))
    {
        string[] linhas = File.ReadAllLines(arquivoMovimentacoes);
        foreach (string linha in linhas)
        {
            string[] partes = linha.Split(',');
            int id = int.Parse(partes[0].Split(new string[] { "ID: " }, StringSplitOptions.None)[1].Trim());
            string nomeProduto = partes[1].Split(':')[1].Trim();
            int quantidade = int.Parse(partes[2].Split(':')[1].Trim());
            int volumeMinimo = int.Parse(partes[3].Split(':')[1].Trim());
            DateTime validadeProduto = DateTime.ParseExact(partes[4].Split(':')[1].Trim(), "dd/MM/yyyy", null);

            Produto produto = new Produto(nomeProduto, id)
            {
                Quantidade = quantidade,
                VolumeMinimo = volumeMinimo,
                ValidadeProduto = validadeProduto
            };
            produtos.Add(produto);
        }

    
        if (produtos.Count > 0)
        {
            int maxId = produtos.Max(p => p.Id);
            Produto.AtualizarProximoId(maxId + 1);
        }
    }
}

public void AdicionarMovimentacao(Produto produto, bool isSaida = false)
{
    string tipoMovimentacao = isSaida ? "Saída" : "Entrada";
    string movimentacao = $"{DateTime.Now}: {tipoMovimentacao} - ID: {produto.Id}, Nome: {produto.NomeProduto}, Quantidade Restante: {produto.Quantidade}, Quantidade Minima: {produto.VolumeMinimo}, Validade: {produto.ValidadeProduto.ToString("dd/MM/yyyy")}";

    try
    {
        List<string> linhas = File.ReadAllLines(arquivoMovimentacoes).ToList();
        linhas.Add(movimentacao);
        File.WriteAllLines(arquivoMovimentacoes, linhas);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao salvar movimentação: {ex.Message}");
    }
}
        public void ImprimirArquivo()
{
        if (File.Exists(arquivoMovimentacoes))
    {
        string[] linhas = File.ReadAllLines(arquivoMovimentacoes);
        foreach (string linha in linhas)
        {
            Console.WriteLine(linha);
        }
    }
     else
    {
        Console.WriteLine("Não há produtos no Estoque");
    }
}












    }
}