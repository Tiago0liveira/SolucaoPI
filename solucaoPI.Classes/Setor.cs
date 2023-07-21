using System;
using System.Collections.Generic;

namespace ControleEstoque
{

    public class Setor
    {
        public string Nome { get; set; }
        public List<Funcionario> Funcionarios { get; set; }

        public Setor()
        {
            Funcionarios = new List<Funcionario>();
        }
    }
}