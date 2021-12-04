using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SendMessageFromSocket(1031);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void SendMessageFromSocket(int port)
        {
            // Буфер для вхідних даних
            byte[] bytes = new byte[256];

            // З'єднуємося з віддаленим пристроєм

            // Встановлюємо віддалену точку для сокету
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // З'єднуємо сокет із віддаленою точкою
            sender.Connect(ipEndPoint);

            Console.Write("Enter command: \n\"Who\" - to get the author" +
                " \n\"0\" - to get the entire list of server directories" +
                " \n\"1\" - get a filtered list, you will be asked to enter a filter\n");
            string message = Console.ReadLine();
            SaveClientLog(message);

            Console.WriteLine("The socket connects to {0} ", sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Надсилаємо дані через сокет
            int bytesSent = sender.Send(msg);

            if (message == "1")
            {
                Console.Write("Enter filter: ");
                message = Console.ReadLine();
                SaveClientLog(message);
                msg = Encoding.UTF8.GetBytes(message);
                bytesSent = sender.Send(msg);
            }

            // Отримуємо відповідь від сервера
            int bytesRec = sender.Receive(bytes);

            message = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            SaveClientLog(message);
            Console.WriteLine("\nServer response: {0}\n\n", message);

            // Використовуємо рекурсію для неодноразового виклику SendMessageFromSocket()
            if (message.IndexOf("<TheEnd>") == -1)
                SendMessageFromSocket(port);

            // Звільняємо сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        public static void SaveClientLog(string socket)
        {
            string writePath = @"C:\Users\rovald\source\repos\ClientServer\SocketClient\ClientLog.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(DateTime.Now + $" {socket}");
                    sw.WriteLine();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
