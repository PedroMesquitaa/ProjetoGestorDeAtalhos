using System;                                // Namespace que contém classes básicas, como Console.
using System.Collections.Generic;            // Para utilizar a classe List<T>.
using System.IO;                             // Para manipulação de arquivos.
using System.Linq;                           // Para funções como Reverse().
using System.Net;                            // Para trabalhar com rede, IPs, DNS.
using System.Net.NetworkInformation;         // Para usar Ping e NetworkInformation.
using System.Threading.Tasks;                // Para utilizar Task.Delay (aguardar de forma assíncrona).

namespace MenuAtalhos                        // Define um namespace para agrupar o código relacionado.
{
    public static class Ip                   // Declara uma classe estática chamada Ip.
    {
        // Lista que armazenará os IPs ativos encontrados.
        public static List<string> ActiveIps { get; private set; } = new List<string>();

        // Variáveis para rastrear se as opções foram executadas.
        private static bool deletarAtalhosExecutado = false;
        private static bool adicionarAtalhosExecutado = false;

        // Método principal que inicia o scanner de IPs.
        public static void IpScanner()
        {
            Console.Clear();  // Limpa a tela do console.

            // Solicita ao usuário o IP inicial e final.
            Console.Write("Digite o endereço IP inicial: ");
            string startIp = Console.ReadLine();

            Console.Write("Digite o endereço IP final: ");
            string endIp = Console.ReadLine();

            // Define o caminho do arquivo de log que será salvo na área de trabalho.
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ActiveIPs.log");

            // Realiza o escaneamento de IPs no intervalo fornecido.
            int activeComputers = ScanIpRange(startIp, endIp, logFilePath);

            // Exibe o número de computadores ativos encontrados.
            Console.WriteLine($"\nNúmero de computadores ativos no intervalo de IPs: {activeComputers}");

            // Exibe o menu para ações adicionais.
            Console.WriteLine("\nO que você deseja fazer?");
            SecondMenu();  // Chama o menu secundário.
        }

        // Método que escaneia o intervalo de IPs fornecido.
        static int ScanIpRange(string startIp, string endIp, string logFilePath)
        {
            Console.WriteLine("Scanneando...");

            int activeCount = 0;  // Contador para IPs ativos encontrados.

            // Converte os IPs inicial e final em objetos IPAddress.
            IPAddress startAddress = IPAddress.Parse(startIp);
            IPAddress endAddress = IPAddress.Parse(endIp);

            // Obtém os bytes dos endereços IPs.
            byte[] startBytes = startAddress.GetAddressBytes();
            byte[] endBytes = endAddress.GetAddressBytes();

            // Obtém o IP local da máquina que está executando o programa.
            string localIp = GetLocalIPAddress();

            // Verifica se o IP inicial é maior que o IP final.
            if (BitConverter.ToUInt32(startBytes.Reverse().ToArray(), 0) > BitConverter.ToUInt32(endBytes.Reverse().ToArray(), 0))
            {
                Console.WriteLine("Erro: O endereço IP inicial é maior que o endereço IP final.");
                return activeCount;
            }

            // Calcula o número total de IPs no intervalo fornecido.
            int totalIps = (endBytes[0] - startBytes[0] + 1) *
                           (endBytes[1] - startBytes[1] + 1) *
                           (endBytes[2] - startBytes[2] + 1) *
                           (endBytes[3] - startBytes[3] + 1);

            if (totalIps <= 0)
            {
                Console.WriteLine("Erro: Intervalo de IPs inválido.");
                return activeCount;
            }

            int ipsScanned = 0;  // Contador de IPs já escaneados.

            // Abre o arquivo de log para registrar os IPs ativos.
            using (StreamWriter log = new StreamWriter(logFilePath, true))
            {
                // Percorre todos os IPs dentro do intervalo.
                for (byte a = startBytes[0]; a <= endBytes[0]; a++)
                {
                    for (byte b = startBytes[1]; b <= endBytes[1]; b++)
                    {
                        for (byte c = startBytes[2]; c <= endBytes[2]; c++)
                        {
                            for (byte d = startBytes[3]; d <= endBytes[3]; d++)
                            {
                                string ip = $"{a}.{b}.{c}.{d}";  // Constrói o endereço IP a partir dos bytes.

                                if (ip == localIp)
                                {
                                    continue;  // Ignora o próprio IP da máquina.
                                }

                                if (ip == endIp)  // Quando chega ao último IP do intervalo.
                                {
                                    try
                                    {
                                        if (PingHost(ip))  // Tenta realizar um ping no IP.
                                        {
                                            ActiveIps.Add(ip);  // Adiciona o IP à lista de ativos se for bem-sucedido.
                                            string hostname = GetHostName(ip);  // Obtém o nome do host associado.
                                            log.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {hostname} ({ip}) está ativo.");
                                            activeCount++;  // Incrementa o número de IPs ativos.
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        log.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Erro ao processar IP {ip}: {ex.Message}");
                                    }
                                    ipsScanned++;
                                    UpdateProgressBar(ipsScanned, totalIps);  // Atualiza a barra de progresso.
                                    break;  // Sai do loop ao atingir o último IP.
                                }

                                try
                                {
                                    if (PingHost(ip))  // Realiza ping no IP.
                                    {
                                        ActiveIps.Add(ip);
                                        string hostname = GetHostName(ip);
                                        log.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {hostname} ({ip}) está ativo.");
                                        activeCount++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Erro ao processar IP {ip}: {ex.Message}");
                                }

                                ipsScanned++;
                                UpdateProgressBar(ipsScanned, totalIps);  // Atualiza a barra de progresso.
                            }
                        }
                    }
                }
            }

            UpdateProgressBar(totalIps, totalIps);  // Barra de progresso completa.
            return activeCount;  // Retorna o número total de IPs ativos.
        }

        // Método que obtém o IP local da máquina.
        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());  // Obtém informações sobre o host atual.
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();  // Retorna o primeiro IP IPv4 encontrado.
                }
            }
            throw new Exception("Nenhum IP da rede local encontrado!");  // Lança exceção se nenhum IP for encontrado.
        }

        // Método que obtém o nome do host associado a um IP.
        static string GetHostName(string ip)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                return hostEntry.HostName;  // Retorna o nome do host associado ao IP.
            }
            catch (Exception)
            {
                return "Hostname não encontrado";  // Caso o hostname não seja encontrado, retorna uma mensagem padrão.
            }
        }

        // Método que realiza o ping em um IP para verificar se ele está ativo.
        static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();

            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;  // Verifica se o ping foi bem-sucedido.
            }
            catch (PingException ex)
            {
                Console.WriteLine($"Erro ao pingar {nameOrAddress}: {ex.Message}");
            }

            return pingable;  // Retorna true se o ping foi bem-sucedido, false caso contrário.
        }

        // Método para atualizar a barra de progresso no console.
        static void UpdateProgressBar(int current, int total)
        {
            if (total <= 0) return;

            int progressBarWidth = 50;  // Define a largura da barra de progresso.
            double progressPercentage = (double)current / total;
            int filledBar = (int)(progressPercentage * progressBarWidth);

            Console.SetCursorPosition(0, Console.CursorTop);  // Move o cursor para o início da linha no console.
            Console.Write("[");
            Console.Write(new string('#', filledBar));  // Preenche a barra de progresso com #.
            Console.Write(new string('-', progressBarWidth - filledBar));  // Preenche o restante com -.
            Console.Write($"] {progressPercentage * 100:0.00}%");  // Exibe a porcentagem de progresso.
        }

        // Menu secundário após o escaneamento de IPs.
        public static void SecondMenu()
        {
            bool isMenuActive = true;

            while (isMenuActive)  // Mantém o menu ativo até que o usuário escolha sair.
            {
                Console.Clear();  // Limpa a tela do console.

                // Lista de ações disponíveis.
                List<string> listActions = new List<string>
                {
                    "Digite Apenas o número \r\n",
                    "1 - Deletar Atalhos \r\n",
                    "2 - Adicionar Atalhos\r\n",
                    "3 - Sair\r\n"
                };

                listActions.ForEach(f => Console.WriteLine(f));  // Exibe as opções de ação.

                var selectedAction = Console.ReadKey(true);  // Lê a opção escolhida pelo usuário.

                switch (selectedAction.KeyChar.ToString())
                {
                    case "1":
                        Console.Clear();
                        if (deletarAtalhosExecutado)  // Verifica se a ação já foi executada.
                        {
                            Console.WriteLine("Aviso: Você já executou a opção 'Deletar Atalhos'. Voltando ao menu...");
                            Task.Delay(2000).Wait();  // Pausa de 2 segundos.
                        }
                        else
                        {
                            Deletar.DeleteSelectedShortcuts();  // Chama o método para deletar atalhos.
                            deletarAtalhosExecutado = true;  // Marca como executado.
                        }
                        break;

                    case "2":
                        Console.Clear();
                        if (adicionarAtalhosExecutado)  // Verifica se a ação já foi executada.
                        {
                            Console.WriteLine("Aviso: Você já executou a opção 'Adicionar Atalhos'. Voltando ao menu...");
                            Task.Delay(2000).Wait();  // Pausa de 2 segundos.
                        }
                        else
                        {
                            Adicionar.CopyAllShortcut();  // Chama o método para adicionar atalhos.
                            adicionarAtalhosExecutado = true;  // Marca como executado.
                        }
                        break;

                    case "3":
                        isMenuActive = false;  // Sai do loop do menu.
                        Environment.Exit(0);  // Encerra o programa.
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Opção inválida! Pressione qualquer tecla para tentar novamente.");
                        Console.ReadKey(true);  // Aguarda a tecla pressionada pelo usuário.
                        continue;  // Retorna ao início do loop para exibir o menu novamente.
                }
            }
        }

    }
}
