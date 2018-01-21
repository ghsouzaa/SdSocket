using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    public static void StartListening()
    {
        #region Variaveis

        string mensagem = null;
        byte[] bytesMensagem = new Byte[1024];

        string tipoCliente = null;
        byte[] bytesTipoCliente = new Byte[1024];

        IPAddress ipLocal = LocalIPAddress();
        int portaServidor = 11000;

        #endregion

        // Cria EndPoint local que será usado pelo socket.
        IPEndPoint endPoint = new IPEndPoint(ipLocal, portaServidor);

        // Cria Socket.
        Socket listener = new Socket(ipLocal.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Vincula o Socket ao Endpoint do servidor.
            listener.Bind(endPoint);
            // Limite da fila de clientes conectados.
            listener.Listen(10);

            // Laço para ficar aguardando conexões dos clientes.
            while (true)
            {
                Console.WriteLine("Esperando conexão de um cliente...");
                // Servidor fica parado nesse ponto esperando a conexão de um novo cliente.  
                Socket handler = listener.Accept();
                
                // Reseta o tipo do cliente pra cada nova conexão receber um novo tipo.
                bytesTipoCliente = new byte[1024];

                // Recebe os bytes do tipo do client, transforma em string e salva na variavel tipoCliente.
                int bytesRecTipo = handler.Receive(bytesTipoCliente);
                tipoCliente = Encoding.ASCII.GetString(bytesTipoCliente, 0, bytesRecTipo);
                Console.WriteLine("Cliente tipo " + tipoCliente.ToUpper() + " conectado.");

                // Loop que fica aguardando as requisições do cliente conectado até que decida desconectar.
                while (true)
                {
                    // Reseta a mensagem para cada nova requisição receber uma nova mensagem.
                    bytesMensagem = new byte[1024];

                    // Recebe os bytes da mensagem, transforma em string e salva na variavel mensagem.
                    int bytesRecMensagem = handler.Receive(bytesMensagem);
                    mensagem = Encoding.ASCII.GetString(bytesMensagem, 0, bytesRecMensagem);

                    // Condição pra saber se o cliente deseja desconectar.
                    if (mensagem == "")
                    {
                        // Desconecta do cliente e para esse laço pra voltar ao laço de aguardando conexão.
                        Console.WriteLine("Cliente desconectou.");
                        Console.WriteLine("");
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        break;
                    }
                    else
                    {
                        byte[] byteResultado = null;
                        Console.WriteLine("Mensagem recebida: {0}", mensagem);

                        if(tipoCliente == "A")
                        {
                            // Converte a mensagem de String para Inteiro, aplica a operação e guarda o resultado na variavel de retorno, em bytes.
                            int convert;
                            int.TryParse(mensagem, out convert);
                            int resultado = convert * convert;
                            byteResultado = Encoding.ASCII.GetBytes(resultado.ToString());
                        }
                        else
                        {
                            // Converte a mensagem de String para Double, aplica a operação e guarda o resultado na variavel de retorno, em bytes.
                            double convert;
                            double.TryParse(mensagem, out convert);
                            double resultado = Math.Sqrt(convert);
                            byteResultado = Encoding.ASCII.GetBytes(resultado.ToString());
                        }

                        // Envia o resultado da operação para o cliente.
                        handler.Send(byteResultado);
                    }
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nOcorreu um erro...");
        Console.Read();
    }

    /// <summary>
    /// Retorna a classe IPAdress que contém o IP da máquina que está executando o método
    /// </summary>
    /// <returns></returns>
    private static IPAddress LocalIPAddress()
    {
        if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            return null;

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}