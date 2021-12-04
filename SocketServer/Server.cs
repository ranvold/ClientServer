using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace SocketServer
{
    class Program
    {
        private static List<string> directory = new(){ 
            "lab1_AOC_Volodymyr_Shafran_K25.cs",
            "Lab1_OOP",
            "list_of_debts.txt",
            "hw6_OOP.cs",
            "record12_math_log.mp4",
            "essay_OOP.docx",
            "lab2_AOC_test.docx",
            "hw7_OOP.cs"
        };
        static void Main(string[] args)
        {
            // Встановлюємо для сокету локальну кінцеву точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 1031);

            // Створюэмо сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Призначаємо сокет локальної кінцевої точки та слухаємо вхідні сокети
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Починаємо слухати з'єднання
                while (true)
                {
                    Console.WriteLine("Waiting for a connection through the port {0}", ipEndPoint);

                    // Програма припиняється, очікуючи на вхідне з'єднання
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Ми дочекалися клієнта, який намагається з нами з'єднатися
                    byte[] bytes = new byte[256];
                    int bytesRec = handler.Receive(bytes);

                    data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    SaveServerLog(data);

                    // Показуємо дані на консолі
                    Console.Write("Received text: " + data + "\n\n");

                    // Надсилаємо відповідь клієнту
                    string reply = "";
                    if (data == "Who")
                    {
                        reply = "Shafran Volodymyr K25 Var-6";
                    }
                    else if (data == "0")
                    {
                        reply = GetAll();
                    }
                    else if (data == "1")
                    {
                        bytesRec = handler.Receive(bytes);
                        data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        SaveServerLog(data);
                        reply = GetWithFilter(data);
                    }
                    else
                    {
                        reply = "Unknown command!";
                    }

                    SaveServerLog(reply);
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);


                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("The server has ended the connection with the client.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
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
        public static string GetAll()
        {
            string reply = "";
            foreach (var item in directory)
            {
                reply += "\n" + item;
            }
            return reply;
        }

        // Використовуэмо регулярні вирази для фільтрації
        public static string GetWithFilter(string filter)
        {
            string reply = "";
            Regex rgx = new Regex(filter);
            foreach (var item in directory)
            {
                if (rgx.IsMatch(item))
                {
                    reply += "\n" + item;
                }
            }
            if (reply == "")
            {
                reply = "Not found";
            }
            return reply;
        }
        public static void SaveServerLog(string socket)
        {
            string writePath = @"C:\Users\rovald\source\repos\ClientServer\SocketServer\ServerLog.txt";

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
