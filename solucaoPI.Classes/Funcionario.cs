using System;
using System.Collections.Generic;

namespace ControleEstoque
{

    public class Funcionario
    {
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public Setor Setor { get; set; }


    }
}