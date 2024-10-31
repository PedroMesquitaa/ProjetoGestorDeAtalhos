using System;                // Importa o namespace System, que contém classes fundamentais como Console.
using System.Threading;      // Importa o namespace System.Threading, usado para gerenciar threads, como a função Sleep.

namespace MenuAtalhos       // Define um namespace chamado MenuAtalhos para organizar o código e evitar conflitos de nome.
{
    public static class Menu  // Declara uma classe estática chamada Menu, onde os métodos e variáveis são estáticos.
    {
        // Variáveis para armazenar o nome de usuário e a senha inseridos pelo usuário.
        public static string UserName { get; private set; }  // Propriedade pública para o nome de usuário, apenas leitura pública.
        public static string Password { get; private set; }  // Propriedade pública para a senha, apenas leitura pública.

        public static void Show()  // Método público que exibe o menu principal.
        {
            ShowWelcomeScreen();    // Chama o método que exibe a tela de boas-vindas.
            Thread.Sleep(5000);     // Aguarda 5 segundos (5000 milissegundos) antes de prosseguir.
            Console.Clear();        // Limpa o conteúdo do console (remove a tela de boas-vindas).
            
            ShowLoginScreen();      // Chama o método que exibe a tela de login.
            
            Ip.IpScanner();         // Chama o método IpScanner da classe Ip (não mostrado aqui) para realizar a função principal do programa.
        }

        private static void ShowWelcomeScreen()  // Método privado para exibir a tela de boas-vindas.
        {
            // Define a largura e altura da janela do console.
            Console.SetWindowSize(80, 20);  

            // Define a cor de fundo do console para azul escuro e a cor do texto para branco.
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();  // Limpa o console com as novas cores definidas.

            // Exibe a mensagem de boas-vindas no centro da tela.
            Console.WriteLine("                                                                                                  ");
            Console.WriteLine("                                                                                                  ");
            Console.WriteLine("                              BEM-VINDO AO GERENCIADOR DE ATALHOS                                 ");
            Console.WriteLine("                                                                                                  ");
            Console.WriteLine("                                  Desenvolvido por: Pedro Mesquita                                      ");
            Console.WriteLine("                                         Versão: 1.0                                              ");
            Console.WriteLine("                                                                                                  ");
            Console.WriteLine("                            A inicialização começará em breve...                                  ");
            Console.WriteLine("                                                                                                  ");
            Console.WriteLine("                                                                                                  ");

            // Restaura as cores padrões do console (geralmente fundo preto e texto branco).
            Console.ResetColor();
        }

        private static void ShowLoginScreen()  // Método privado para exibir a tela de login.
        {
            Console.WriteLine("=== Tela de Login ===");  // Exibe o cabeçalho da tela de login.
            
            // Solicita ao usuário o nome de usuário e armazena na variável UserName.
            Console.Write("Digite seu nome de usuário: ");
            UserName = Console.ReadLine();  // Lê a entrada do usuário e armazena em UserName.
            
            // Solicita ao usuário a senha e armazena na variável Password.
            Console.Write("Digite sua senha: ");
            Password = ReadPassword();  // Chama o método ReadPassword() para ler a senha sem exibi-la na tela.

            Console.WriteLine("\nLogin realizado com sucesso!");  // Exibe uma mensagem confirmando o login.
        }

        // Método para ler a senha sem exibi-la na tela, substituindo os caracteres digitados por asteriscos.
        private static string ReadPassword()  
        {
            string password = "";  // Variável para armazenar a senha digitada.
            ConsoleKeyInfo key;    // Armazena informações sobre a tecla pressionada.

            do
            {
                // Lê uma tecla pressionada sem exibi-la no console.
                key = Console.ReadKey(true);

                // Verifica se a tecla pressionada não é Backspace (apagar) nem Enter (concluir).
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;  // Adiciona o caractere pressionado à senha.
                    Console.Write("*");       // Exibe um asterisco no console para cada caractere digitado.
                }
                // Se a tecla pressionada for Backspace e a senha tiver pelo menos um caractere, o último caractere da senha é removido.
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    // Remove o último caractere da senha.
                    password = password.Substring(0, password.Length - 1); // Move o cursor uma posição para trás, apaga o caractere na tela e move o cursor de volta.
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter); // Continua capturando teclas até que o usuário pressione Enter.
            // Retorna a senha digitada pelo usuário.
            return password;
        }
    }
}
