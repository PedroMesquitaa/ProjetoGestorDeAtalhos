using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MenuAtalhos
{
    // Classe para gerenciar a conexão com compartilhamentos de rede
    public class NetworkShareAccesser : IDisposable
    {
        private string _networkName;

        // Método estático para criar uma nova instância da classe e conectar ao compartilhamento de rede
        public static NetworkShareAccesser Access(string networkName, string userName, string password)
        {
            return new NetworkShareAccesser(networkName, userName, password);
        }

        // Construtor privado que configura a conexão com o compartilhamento de rede
        private NetworkShareAccesser(string networkName, string userName, string password)
        {
            _networkName = networkName;

            // Cria um objeto NetResource para especificar o compartilhamento de rede
            var netResource = new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            // Tenta conectar ao compartilhamento de rede usando as credenciais fornecidas
            var result = WNetAddConnection2(netResource, password, userName, 0);

            // Se a conexão falhar, lança uma exceção com o código de erro
            if (result != 0)
            {
                throw new Exception($"Failed to connect to network path. Error code: {result}");
            }
        }

        // Método para desconectar do compartilhamento de rede e liberar recursos
        public void Dispose()
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        // Destruidor para garantir que os recursos sejam liberados se Dispose não for chamado
        ~NetworkShareAccesser()
        {
            Dispose();
        }

        // Importa a função WNetAddConnection2 da DLL mpr.dll para conectar a um recurso de rede
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        // Importa a função WNetCancelConnection2 da DLL mpr.dll para desconectar de um recurso de rede
        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        // Estrutura para especificar o recurso de rede
        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        // Enumeração para especificar o escopo do recurso de rede
        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        }

        // Enumeração para especificar o tipo de recurso de rede
        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        // Enumeração para especificar o tipo de exibição do recurso de rede
        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }
    }
}
