using System;
using System.Collections.Generic;

namespace ControleEstoque
{
    public class Produto
    {
        private static int proximoId = 1;

        public Produto(string nomeProduto)
        {
            NomeProduto = nomeProduto;
            Id = proximoId++;
            DataEntrada = new List<DateTime>();
            ValidadeProduto = DateTime.MinValue;
        }

        public int Id { get; }
        public string NomeProduto { get; set; }
        public List<DateTime> DataEntrada { get; set; }
        public int Quantidade { get; set; }
        public int VolumeMinimo { get; set; }
        public int VolumeMaximo { get; set; }

        public DateTime ValidadeProduto { get; set; }



        public bool Equals(Produto other)
        {
            if (other == null)
                return false;

            return NomeProduto == other.NomeProduto;
        }


        public int CalcularQuantidadeAtual()
        {
            int quantidadeAtual = Quantidade;
            foreach (DateTime dataEntrada in DataEntrada)
            {
                if (dataEntrada <= DateTime.Now)
                {
                    quantidadeAtual += 0;
                }
            }
            return quantidadeAtual;
        }


    }
}