using System; // Importa o namespace System, que contém classes fundamentais, como Console.
using System.IO; // Importa o namespace System.IO, que permite manipulação de arquivos e diretórios.

namespace MenuAtalhos // Define um namespace chamado MenuAtalhos para organizar o código.
{
    class Program // Declara uma classe chamada Program, que será o ponto de entrada do programa.
    {
        static void Main(string[] args) // Define o método Main, que é o ponto de entrada do programa.
        {
            Menu.Show(); // Chama o método Show da classe Menu, que provavelmente exibe o menu principal da aplicação.
            Console.ReadKey(); // Espera que o usuário pressione uma tecla antes de encerrar o programa.
        }
    }
}
