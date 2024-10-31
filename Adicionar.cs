using System;
using System.IO;
using System.Linq;
using System.Net;

namespace MenuAtalhos
{
    class Adicionar
    {
        public static void CopyAllShortcut()
        {
            // Caminho do servidor de arquivos onde estão os atalhos padrão a serem copiados
            string serverPath = @"\\server\path\to\shortcuts\";

            // Credenciais para autenticação no servidor remoto (usando as credenciais fornecidas na tela de login)
            string userName = Menu.UserName;  // Nome de usuário fornecido na tela de login
            string password = Menu.Password;  // Senha fornecida na tela de login

            // Caminho do arquivo CSV de log na área de trabalho do usuário atual
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AdicionarAtalhosErrors.csv");

            // Se o arquivo CSV não existir, cria o cabeçalho
            if (!File.Exists(logFilePath))
            {
                File.AppendAllText(logFilePath, "Data/Hora;IP Remoto;Hostname;Caminho de Destino;Mensagem de Erro\n");
            }

            // Verifica se o diretório do servidor de arquivos onde estão os atalhos existe
            if (!Directory.Exists(serverPath))
            {
                // Se o diretório não existir, escreve uma mensagem no arquivo CSV e exibe no console
                File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}; ; ;{serverPath};Server path does not exist\n");
                Console.WriteLine("Server path does not exist.");
                return;  // Interrompe a execução se o diretório não for encontrado
            }

            // Obtém todos os arquivos .lnk, .url e .bat do diretório do servidor de arquivos
            string[] shortcutFiles = Directory.GetFiles(serverPath, "*.*")
                                                .Where(file => file.EndsWith(".lnk") || file.EndsWith(".url") || file.EndsWith(".URL") || file.EndsWith(".bat"))
                                                .ToArray();

            // Itera sobre cada IP ativo na lista de IPs (Ip.ActiveIps contém os IPs ativos)
            foreach (string remoteIp in Ip.ActiveIps)
            {
                // Obtém o hostname associado ao IP remoto
                string hostName = GetHostName(remoteIp);

                // Define o caminho para a área de trabalho pública no computador remoto
                string publicDesktopPath = $@"\\{remoteIp}\C$\Users\Public\Desktop";

                try
                {
                    // Conecta-se à área de trabalho pública do computador remoto usando as credenciais fornecidas
                    using (NetworkShareAccesser.Access(publicDesktopPath, userName, password))
                    {
                        // Copia cada atalho do servidor para a área de trabalho pública do computador remoto
                        foreach (string shortcut in shortcutFiles)
                        {
                            // Obtém o nome do arquivo de atalho
                            string fileName = Path.GetFileName(shortcut);

                            // Define o caminho completo do arquivo de destino no computador remoto
                            string destFile = Path.Combine(publicDesktopPath, fileName);

                            try
                            {
                                // Copia o arquivo de atalho para o destino, sobrescrevendo o arquivo existente, se necessário
                                File.Copy(shortcut, destFile, true); // O terceiro parâmetro 'true' permite sobrescrever arquivos existentes
                                Console.WriteLine($"Copied and replaced: {fileName} on {remoteIp} ({hostName})");
                            }
                            catch (Exception ex)
                            {
                                // Se ocorrer um erro durante a cópia, registra o erro no arquivo CSV e exibe no console
                                File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss};{remoteIp};{hostName};{destFile};Failed to copy {fileName}: {ex.Message}\n");
                                Console.WriteLine($"Failed to copy {fileName} to {remoteIp} ({hostName}): {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Se ocorrer um erro ao acessar o caminho da área de trabalho pública no computador remoto, registra o erro no CSV
                    File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss};{remoteIp};{hostName};{publicDesktopPath};Failed to access public desktop: {ex.Message}\n");
                    Console.WriteLine($"Failed to access {publicDesktopPath} on {remoteIp} ({hostName}): {ex.Message}");
                }
            }

            // Após a execução da função, exibe uma mensagem e aguarda antes de voltar ao menu
            Console.Clear();
            Console.WriteLine("Ação executada. Voltando ao menu...");
            Task.Delay(2000).Wait();  // Pausa de 2 segundos (2000 milissegundos)
        }

        // Método para obter o hostname associado a um endereço IP
        static string GetHostName(string ip)
        {
            try
            {
                // Obtém as informações de DNS (hostname) associadas ao IP fornecido
                IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                return hostEntry.HostName;  // Retorna o nome do host associado ao IP
            }
            catch (Exception)
            {
                // Se não for possível obter o hostname, retorna uma mensagem padrão
                return "Hostname não encontrado";
            }
        }
    }
}
