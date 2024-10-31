using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;  // Necessário para Task.Delay

namespace MenuAtalhos
{
    class Deletar
    {
        public static void DeleteSelectedShortcuts()
        {
            // Caminho do servidor de arquivos onde estão os atalhos
            string remotePublicDesktopPath;

            // Credenciais para autenticação
            string userName = Menu.UserName;  // Nome de usuário fornecido na tela de login
            string password = Menu.Password;  // Senha fornecida na tela de login

            // Lista de atalhos que você deseja excluir
            List<string> shortcutsToDelete = new List<string>
            {
                "atalhos que voce queira excluir em expecifico"
                
            };

            // Itera sobre cada IP na lista de IPs ativos
            foreach (string remoteIp in Ip.ActiveIps)
            {
                remotePublicDesktopPath = $@"\\{remoteIp}\C$\Users\Public\Desktop";

                try
                {
                    // Use a classe de acesso à rede para conectar-se com as credenciais fornecidas
                    using (NetworkShareAccesser.Access(remotePublicDesktopPath, userName, password))
                    {
                        // Verifique se o diretório da área de trabalho pública remota existe
                        if (Directory.Exists(remotePublicDesktopPath))
                        {
                            // Obtenha todos os arquivos .lnk, .url, .URL e .bat do diretório da área de trabalho pública remota
                            string[] shortcutFiles = Directory.GetFiles(remotePublicDesktopPath, "*.*")
                                                            .Where(file => file.EndsWith(".lnk") || file.EndsWith(".url") || file.EndsWith(".URL") || file.EndsWith(".bat"))
                                                            .ToArray();

                            // Delete apenas os atalhos que estão na lista
                            foreach (string shortcut in shortcutFiles)
                            {
                                string fileName = Path.GetFileName(shortcut);

                                // Verifica se o arquivo está na lista de atalhos a serem excluídos
                                if (shortcutsToDelete.Contains(fileName))
                                {
                                    try
                                    {
                                        File.Delete(shortcut);
                                        Console.WriteLine($"Deleted: {fileName} from {remoteIp}");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Failed to delete {fileName} from {remoteIp}: {ex.Message}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Public desktop path on {remoteIp} does not exist.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Captura e exibe erros relacionados à conexão
                    Console.WriteLine($"Failed to connect or process {remoteIp}: {ex.Message}");
                }
            }

            // Após a execução da função, exibe uma mensagem e aguarda antes de voltar ao menu
            Console.Clear();
            Console.WriteLine("Ação executada. Voltando ao menu...");
            Task.Delay(2000).Wait();  // Pausa de 2 segundos (2000 milissegundos)
        }
    }
}
