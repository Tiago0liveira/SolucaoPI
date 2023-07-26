using System;
using System.Collections.Generic;
using System.IO;

namespace ControleEstoque
{
    public class CrudFuncionario
    {
        private static string arquivoFuncionarios;
        private List<Funcionario> Funcionarios { get; set; }

        public void AdicionarFuncionario(Funcionario funcionario)
        {
            Funcionarios.Add(funcionario);
        }

    }
}