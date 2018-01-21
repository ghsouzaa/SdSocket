using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ClientA
{
    public static void Main(String[] args)
    {
        #region Variaveis

        byte[] bytes = new byte[1024];
        IPAddress ipServidor = IPAddress.Parse("192.168.56.1");
        int portaServidor = 11000;
        string tipoClient = "A";

        #endregion

        try
        {
            // Cria EndPoint que será usado pelo socket.
            IPEndPoint endPointServidor = new IPEndPoint(ipServidor, portaServidor);

            // Cria socket. 
            Socket sender = new Socket(ipServidor.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Conecta o socket com o servidor.
            sender.Connect(endPointServidor);

            Console.WriteLine("Conectado ao servidor {0}", sender.RemoteEndPoint.ToString());

            // Envia o tipo do cliente.
            sender.Send(Encoding.ASCII.GetBytes(tipoClient));

            // Laço para ficar requisitando o servidor.
            while (true)
            {
                Console.Write("Digite um número ou aperte Enter para sair: ");
                var op = Console.ReadLine();

                // Condição para saber se deseja desconectar.
                if (op != "")
                {
                    // Envia mensagem para o servidor.
                    byte[] msg = Encoding.ASCII.GetBytes(op);
                    int bytesSent = sender.Send(msg);

                    // Recebe o resultado e mostra na tela.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Resultado = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                }
                else
                {
                    // Envia mensagem para o servidor avisando que quer se desconectar.
                    byte[] msg = Encoding.ASCII.GetBytes(op);
                    int bytesSent = sender.Send(msg);
                    break;
                }
            }

            // Desconecta do servidor.
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.ReadKey();
        }
    }
}