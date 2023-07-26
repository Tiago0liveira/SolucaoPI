using System;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;



namespace ControleEstoque
{

    public class Program
    {
        private static Estoque estoque;
        private static List<Fornecedor> fornecedores;
        private static List<Funcionario> funcionarios;
        private static List<Setor> setores = new List<Setor>();

        private static CrudEstoque crudEstoque;

        private static string arquivoFuncionarios = "../BancoDados/BdFuncionarios.txt";
        private static string arquivoEstoque = "../BancoDados/BdEstoque.txt";

        public static void Main(string[] args)
        {
            estoque = VerificarArquivoEstoque();
            estoque.CarregarProdutostxt();
            fornecedores = new List<Fornecedor>();
            funcionarios = new List<Funcionario>();
            setores = new List<Setor>();
            crudEstoque = new CrudEstoque(arquivoEstoque);

            // Verifica se o arquivo de funcionários existe e cria-o, se necessário
            if (!File.Exists(arquivoFuncionarios))
            {
                File.Create(arquivoFuncionarios).Close();
            }

            //LimparDadosArquivo();
            CarregarFuncionariosDeArquivo();
            SalvarFuncionariosEmArquivo();
            MostrarMenuPrincipal();

        }

        public static void MostrarMenuPrincipal()
        {

            Console.WriteLine();
            Console.WriteLine("===== Controle de Estoque Integrador  =====");
            Console.WriteLine("=====                                 =====");
            Console.WriteLine("===== 1. Cadastrar Fornecedor         =====");
            Console.WriteLine("===== 2. Cadastrar Funcionário        =====");
            Console.WriteLine("===== 3. Recebimento de Produtos      =====");
            Console.WriteLine("===== 4. Entrega de Produtos          =====");
            Console.WriteLine("===== 5. Inventário                   =====");
            Console.WriteLine("===== 6. Sair                         =====");
            Console.WriteLine("===========================================");

            Console.Write("Digite a opção desejada: ");
            int opcao = int.Parse(Console.ReadLine());

            Console.WriteLine();

            switch (opcao)
            {
                case 1:
                    CadastrarFornecedor();
                    break;
                case 2:
                    CadastrarFuncionario();
                    break;
                case 3:
                    RealizarRecebimentoProdutos();
                    break;
                case 4:
                    RealizarEntregaProdutos();
                    break;
                case 5:
                    GerarInventario();
                    break;
                case 6:
                    Console.WriteLine("===== Saindo do Sistema Integrador...                         =====");
                    Console.WriteLine("===== Obrigado por utilizar o Controle de Estoque Integrador! =====");
                    Console.WriteLine("===== Esperamos que sua experiência tenha sido satisfatória.  =====");
                    Console.WriteLine("===== Até a próxima! Volte sempre!                            =====");
                    Console.WriteLine("===================================================================");
                    Console.WriteLine();
                    Environment.Exit(0);

                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }


            Console.WriteLine();
            MostrarMenuPrincipal();
        }

        public static void CadastrarFornecedor()
        {
            Console.WriteLine("===== [ALERTA!] VOCÊ ESTA PRESTES A CADASTRAR UM NOVO FORNECEDOR =====");
            Thread.Sleep(2000); ;
            Console.WriteLine();
            Console.WriteLine("===== Cadastro de Fornecedor =====");
            Console.WriteLine(" ");
            Console.Write("Digite o nome do fornecedor: ");
            string nome = Console.ReadLine();
            Console.Write("Digite o endereço do fornecedor: ");
            string endereco = Console.ReadLine();
            Console.Write("Digite o contato do fornecedor: ");
            string contato = Console.ReadLine();

            Fornecedor fornecedor = new Fornecedor
            {
                Nome = nome,
                Endereco = endereco,
                Contato = contato
            };

            fornecedores.Add(fornecedor);
            Console.WriteLine();
            Console.WriteLine($"Fornecedor: {nome}, Endereço: {endereco}, Contato: {contato}");
            Thread.Sleep(2000);
            Console.WriteLine("");
            Console.WriteLine("Fornecedor cadastrado com sucesso!");
        }

        public static void CadastrarFuncionario()
        {
            Console.WriteLine("===== [ALERTA!] VOCÊ ESTA PRESTES A CADASTRAR UM NOVO FUNCIONARIO =====");
            Thread.Sleep(2000); ;
            Console.WriteLine();
            Console.WriteLine("===== Cadastro de Funcionário =====");
            Console.WriteLine("");
            Console.Write("Digite o nome do funcionário: ");
            string nome = Console.ReadLine();
            Console.Write("Digite o cargo do funcionário: ");
            string cargo = Console.ReadLine();
            Console.Write("Digite o login do funcionário: ");
            string login = Console.ReadLine();
            Console.Write("Digite a senha do funcionário: ");
            string senha = Console.ReadLine();
            Console.Write("Digite o Setor do funcionário: ");
            string nomeSetor = Console.ReadLine();

            Setor setor = setores.Find(s => s.Nome == nomeSetor);
            if (setor == null)
            {
                setor = new Setor { Nome = nomeSetor };
                setores.Add(setor);
            }

            Funcionario funcionario = new Funcionario
            {
                Nome = nome,
                Cargo = cargo,
                Login = login,
                Senha = senha,
                Setor = setor
            };

            setor.Funcionarios.Add(funcionario);
            funcionarios.Add(funcionario);
            SalvarFuncionariosEmArquivo();

            Console.WriteLine("");
            Console.WriteLine("Funcionário cadastrado com sucesso!");
        }

        public static void CadastrarProduto()
        {
            Console.WriteLine("===== [ALERTA!] VOCÊ ESTÁ PRESTES A CADASTRAR UM NOVO PRODUTO =====");
            Thread.Sleep(2000); ;
            Console.WriteLine();
            Console.WriteLine("===== Cadastro de Produto =====");
            Console.WriteLine();
            Console.Write("Digite o nome do produto: ");
            string nomeProduto = Console.ReadLine();
            Console.Write("Digite a quantidade inicial: ");
            int quantidade = int.Parse(Console.ReadLine());
            Console.Write("Digite o volume mínimo: ");
            int volumeMinimo = int.Parse(Console.ReadLine());
            Console.Write("Digite o volume máximo: ");
            int volumeMaximo = int.Parse(Console.ReadLine());
            Console.Write("Digite a data de validade (dd/mm/aaaa): ");
            string dataValidadeString = Console.ReadLine();

            Produto produto = new Produto(nomeProduto)
            {
                Quantidade = quantidade,
                VolumeMinimo = volumeMinimo,
                VolumeMaximo = volumeMaximo
            };

            if (DateTime.TryParseExact(dataValidadeString, new[] { "dd/MM/yyyy", "ddMMyyyy" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataValidade))
            {
                produto.ValidadeProduto = dataValidade;
                crudEstoque.AdicionarProduto(produto);

                Console.WriteLine();
                Console.WriteLine($"Produto cadastrado com sucesso! ID: {produto.Id}, Nome: {produto.NomeProduto}, Quantidade: {produto.Quantidade}, Data Validade: {produto.ValidadeProduto:dd/MM/yyyy}");
            }
            else
            {
                Console.WriteLine("Data de validade inválida. O produto não será cadastrado sem data de validade.");
            }

            Console.WriteLine();
            Console.WriteLine("Deseja cadastrar um novo Produto? (S/N)");
            string cadastrarNovo = Console.ReadLine().ToLower();

            if (cadastrarNovo == "s")
            {
                CadastrarProduto();
            }
            else if (cadastrarNovo == "n")
            {
                estoque.CarregarProdutostxt();
                Console.WriteLine("===== [ALERTA!] VOCÊ VAI SER REDIRECIONADO AO MENU ANTERIOR =====");
                Thread.Sleep(1000);
                Console.WriteLine();

            }

            {
                RealizarRecebimentoProdutos();
            }

        }


        public static void RealizarRecebimentoProdutos()
        {
            int opcao = -1;

            while (opcao != 0 && opcao != 6)
            {
                Console.WriteLine(" ===== BEM VINDO AO RECEBIMENTO DE PRODUTOS =====");
                Thread.Sleep(2000); ;
                Console.WriteLine();
                Console.WriteLine("===== Recebimento de Produtos =====");
                Console.WriteLine("1. Cadastrar novo produto");
                Console.WriteLine("2. Cadastrar Um produto existente");
                Console.WriteLine("0. Retornar ao Menu Principal");
                Console.WriteLine("===================================");

                Console.Write("Digite a opção desejada: ");
                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.WriteLine("[ALERTA!]Opção inválida. Tente novamente.");
                    continue;
                }

                Console.WriteLine();

                switch (opcao)
                {
                    case 1:
                        CadastrarProduto();
                        estoque.CarregarProdutostxt();
                        break;
                    case 2:
                        Console.Write("Digite o ID do produto: ");
                        if (!int.TryParse(Console.ReadLine(), out int idProduto))
                        {
                            Console.WriteLine("ID inválido. Tente novamente.");
                            continue;
                        }

                        Produto produtoExistente = estoque.GetProdutoPorId(idProduto);

                        if (produtoExistente != null)
                        {
                            Console.Write("Digite a quantidade recebida: ");
                            if (!int.TryParse(Console.ReadLine(), out int quantidadeRecebida))
                            {
                                Console.WriteLine("Quantidade inválida. Tente novamente.");
                                continue;
                            }

                            produtoExistente.Quantidade += quantidadeRecebida;
                            produtoExistente.DataEntrada.Add(DateTime.Now);
                            Console.WriteLine("");
                            Console.WriteLine($"Produto atualizado com sucesso! ID: {produtoExistente.Id}, Nome: {produtoExistente.NomeProduto}, Quantidade: {produtoExistente.Quantidade}");
                            Console.WriteLine("");
                            Console.WriteLine("===== [ALERTA!] VOCÊ VAI SER REDIRECIONADO AO MENU ANTERIOR =====");
                            Thread.Sleep(2000); ;
                            Console.WriteLine("");



                            estoque.SalvarMovimentacao(produtoExistente);
                            estoque.CarregarProdutostxt();

                            break;
                        }

                        else
                        {
                            Console.WriteLine("Produto não encontrado. Recebimento de produtos não registrado.");
                        }
                        break;

                    case 0:
                        estoque.CarregarProdutostxt();
                        Thread.Sleep(1000);
                        MostrarMenuPrincipal();
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }

                Console.WriteLine();
            }

            Console.WriteLine("Recebimento de produtos concluído.");
        }


        public static void RealizarEntregaProdutos()
        {
            Console.WriteLine("===== Entrega de Produtos =====");
            Console.Write("Digite o ID do produto: ");
            int idProduto = int.Parse(Console.ReadLine());
            Console.Write("Digite a quantidade a ser entregue: ");
            int quantidadeEntregue = int.Parse(Console.ReadLine());

            Produto produto = estoque.GetProdutoPorId(idProduto);

            if (produto != null)
            {
                if (produto.Quantidade >= quantidadeEntregue)
                {
                    Console.Write("Digite o nome do setor que está retirando o material: ");
                    string nomeSetor = Console.ReadLine();

                    Setor setor = setores.Find(s => s.Nome == nomeSetor);
                    if (setor != null)
                    {
                        Console.Write("Digite o login do funcionário responsável: ");
                        string loginFuncionario = Console.ReadLine();

                        // Verifica se o funcionário e setor existem
                        Funcionario funcionario = setor.Funcionarios.Find(f => f.Login == loginFuncionario);
                        if (funcionario != null)
                        {
                            // Remover o produto do estoque
                            estoque.GetProdutos().Remove(produto);

                            // Atualizar a quantidade
                            produto.Quantidade -= quantidadeEntregue;
                            Console.WriteLine($"Quantidade antes da entrega: {produto.Quantidade + quantidadeEntregue}");
                            Console.WriteLine($"Quantidade depois da entrega: {produto.Quantidade}");

                            // Salvar movimentação
                            estoque.SalvarMovimentacao(produto, true);
                            Console.WriteLine();
                            Console.WriteLine("Entrega de produtos registrada com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Funcionário não encontrado.");
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Setor não encontrado.");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Quantidade insuficiente para realizar a entrega.");
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Produto não encontrado.");
            }
            Thread.Sleep(2000);
        }




        public static void GerarInventario()
        {
            Console.WriteLine(" ===== BEM VINDO AO INVENTARIO =====");
            Thread.Sleep(1000);
            Console.WriteLine();
            Console.WriteLine("========== Inventário ==========");
            Console.WriteLine("===== 1. Produtos no Estoque");
            Console.WriteLine("===== 2. Consultar Produto por ID");
            Console.WriteLine("===== 3. Consultar Produto Com limite Minimo ");
            Console.WriteLine("===== 0. Retornar");
            Console.WriteLine("=================================");

            Console.Write("Digite a opção desejada: ");

            int opcao;
            while (!int.TryParse(Console.ReadLine(), out opcao) || opcao < 0 || opcao > 3)
            {
                Console.WriteLine();
                Console.WriteLine("Opção inválida. Tente novamente.");
                Thread.Sleep(2000);
                Console.WriteLine();
                GerarInventario();
            }

            Console.WriteLine();

            if (opcao == 1)
            {
                estoque.ImprimirArquivo();
                Thread.Sleep(1000);
                Console.WriteLine();
                Console.WriteLine("[ALERTA!] VOCÊ SERA REDIRECIONADO AO MENU ANTERIOR");
                Console.WriteLine();
                Thread.Sleep(1000);
                GerarInventario();
            }
            else if (opcao == 2)
            {
                Console.Write("Digite o ID do produto: ");
                int idparametro = int.Parse(Console.ReadLine());
                Console.WriteLine();
                var existe = crudEstoque.ConsultarProdutoNoArquivo(idparametro);
                if (existe == null)
                {
                    Console.WriteLine("ID não encontrado, tente novamente");
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine($"ID Pesquisado: {idparametro}");
                    Console.WriteLine();
                    Console.WriteLine(existe);
                    Thread.Sleep(3000);
                }
            }
            else if (opcao == 3)
            {
                List<Produto> produtosComVolumeMinimoAtingido = estoque.ListarProdutosComVolumeMinimoAtingido();
                if (produtosComVolumeMinimoAtingido.Count > 0)
                {
                    Console.WriteLine("===== Produtos com volume mínimo atingido =====");
                    foreach (Produto produto in produtosComVolumeMinimoAtingido)
                    {
                        Console.WriteLine($"ID: {produto.Id}, Nome: {produto.NomeProduto}, Quantidade: {produto.Quantidade}, Volume Mínimo: {produto.VolumeMinimo}");
                    }
                }
                else
                {
                    Console.WriteLine("Não há produtos com volume mínimo atingido.");
                    Console.WriteLine();
                }


                Console.WriteLine("[ALERTA!] VOCÊ SERA REDIRECIONADO AO MENU ANTERIOR");
                Console.WriteLine();
                Thread.Sleep(2000);
                GerarInventario();

            }
            else if (opcao == 0)
            {
                Console.WriteLine();
                Console.WriteLine("[ALERTA!] VOCÊ SERA REDIRECIONADO AO MENU PRINCIPAL");
                Console.WriteLine();
                Thread.Sleep(1000);
                MostrarMenuPrincipal();
            }

            Console.WriteLine();
            Console.WriteLine("[ALERTA!] VOCÊ SERA REDIRECIONADO AO MENU ANTERIOR");
            Console.WriteLine();
            Thread.Sleep(1000);
            GerarInventario();
        }

        private static void CarregarFuncionariosDeArquivo()
        {
            try
            {
                if (File.Exists(arquivoFuncionarios))
                {
                    string[] linhas = File.ReadAllLines(arquivoFuncionarios);
                    foreach (string linha in linhas)
                    {
                        string[] dadosFuncionario = linha.Split(',');
                        if (dadosFuncionario.Length == 5)
                        {
                            string nome = dadosFuncionario[0];
                            string cargo = dadosFuncionario[1];
                            string login = dadosFuncionario[2];
                            string senha = dadosFuncionario[3];
                            string nomeSetor = dadosFuncionario[4];

                            Setor setor = setores.Find(s => s.Nome == nomeSetor);
                            if (setor == null)
                            {
                                setor = new Setor { Nome = nomeSetor };
                                setores.Add(setor);
                            }

                            Funcionario funcionario = new Funcionario
                            {
                                Nome = nome,
                                Cargo = cargo,
                                Login = login,
                                Senha = senha,
                                Setor = setor
                            };

                            setor.Funcionarios.Add(funcionario);
                            funcionarios.Add(funcionario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar funcionários: {ex.Message}");
            }
        }

        private static void SalvarFuncionariosEmArquivo()
        {
            try
            {
                List<string> linhas = new List<string>();
                foreach (Funcionario funcionario in funcionarios)
                {

                    string senhaAsteriscos = new string('*', funcionario.Senha.Length);
                    string loginAsterisco = new string('*', funcionario.Login.Length);

                    string linhaFuncionario = $" Funcionario:{funcionario.Nome}, Cargo:{funcionario.Cargo}, Login:{loginAsterisco}, Senha:{senhaAsteriscos}, Setor:{funcionario.Setor.Nome}, Data do Registro: {DateTime.Now}";

                    linhas.Add(linhaFuncionario);
                }

                File.WriteAllLines(arquivoFuncionarios, linhas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar funcionários: {ex.Message}");
            }
        }

        private static Estoque VerificarArquivoEstoque()
        {
            // Verifica se o arquivo de estoque existe e cria-o, se necessário
            if (!File.Exists(arquivoEstoque))
            {
                File.Create(arquivoEstoque).Close();
            }

            return new Estoque(arquivoEstoque);
        }

        //     public static void LimparDadosArquivo()
        // {
        //     try
        //     {
        //         // Cria um novo arquivo vazio com o mesmo nome do arquivo existente, substituindo-o
        //         File.WriteAllText(arquivoFuncionarios, string.Empty);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Erro ao limpar dados do arquivo: {ex.Message}");
        //     }
        // }
    }
}




