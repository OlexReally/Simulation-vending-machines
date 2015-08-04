/*
Author - Kutaev O. V.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                int MaxThreadsCount = Environment.ProcessorCount * 4;
                ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
                ThreadPool.SetMinThreads(2, 2);

                Int32 port = 9999;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                int counter = 0;
                server = new TcpListener(localAddr, port);

                server.Start();

                while (true)
                {
                    Console.Write("\n\tWaiting for a connection... ");

                    ThreadPool.QueueUserWorkItem(ConnectFunc, server.AcceptTcpClient());
                    counter++;
                    Console.Write("\nConnection №" + counter.ToString() + "!");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        static void ConnectFunc(object client_obj)
        {
            string[] split;
            byte[] msg;
            Byte[] bytes = new Byte[256];
            String data = null;

            TcpClient client = client_obj as TcpClient;

            data = null;

            NetworkStream stream = client.GetStream();

            Information info = new Information();

            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                split = data.Split(new Char[] { ' ' });

                switch (split[0])
                {
                    case "HELLO":
                        msg = System.Text.Encoding.ASCII.GetBytes("HI");
                        break;

                    case "AUTOMAT_COUNT":
                        info.AutomatCounter = Convert.ToInt32(split[1]);
                        msg = System.Text.Encoding.ASCII.GetBytes(info.AnswerMessageOK());
                        break;

                    case "TRANSACTION":
                        if (Convert.ToInt32(split[1][0].ToString()) == 1)
                        {
                            info.SetTransaction(Convert.ToInt32(split[1][1].ToString()), Convert.ToInt32(split[1][2].ToString()), Convert.ToInt32(split[1][3].ToString()));
                            msg = System.Text.Encoding.ASCII.GetBytes(info.AnswerMessageOK());
                        }
                        else 
                            msg = System.Text.Encoding.ASCII.GetBytes("Unknown Command");
                        break;

                    case "TRANSACTION_COUNT":
                        info.TransactionCounter = Convert.ToInt32(split[1]);
                        msg = System.Text.Encoding.ASCII.GetBytes(info.AnswerMessageOK());
                        break;

                    case "SIMULATE":
                        Simulate sim = new Simulate();
                        sim.StartSimulate(info.AutomatCounter, info.TransactionCounter, info.GetTransaction);
                        msg = System.Text.Encoding.ASCII.GetBytes(info.AnswerMessageOK());
                        break;
                        
                    default:
                        msg = System.Text.Encoding.ASCII.GetBytes("Unknown Command");
                        break;
                }

                stream.Write(msg, 0, msg.Length);
            }
        }
    }

    class Information
    {
        private int automatCount = 0;
        private int transactionCount = 0;
        private int[] transaction = new int[3];
        public Information()
        {

        }

        public int[] GetTransaction
        {
            get
            {
                return this.transaction;
            }
        }
        public void SetTransaction(int a, int b, int c)
        {
            this.transaction[2] = a;//вода
            this.transaction[1] = b;//кава
            this.transaction[0] = c;//чай
        }

        public int TransactionCounter
        {
            get
            {
                return this.transactionCount;
            }
            set
            {
                this.transactionCount = value;
            }
        }

        public int AutomatCounter
        {
            get
            {
                return this.automatCount;
            }
            set
            {
                this.automatCount = value;
            }
        }
        public string AnswerMessageOK()
        {
            return "+OK";
        }
    }

    class Simulate
    {
        private DBMySQL db = new DBMySQL();
        public Simulate()
        {

        }

        public void StartSimulate(int AutCount, int TransCount, int[] TransType)
        {
            string date1 = this.DateConvert(DateTime.Now.ToString());//date without time(HH:MM::SS), only "YYYY-MM-DD"
            Random rnd = new Random();

            for (int id = 1; id <= AutCount; id++)
            {

                for (int trans = 1; trans <= TransCount; trans ++)
                {
                    string HH = rnd.Next(0, 24).ToString();//00 - 24 hours
                    if (Convert.ToInt32(HH.ToString()) >= 0 && Convert.ToInt32(HH.ToString()) <= 9)
                        HH = Fix(HH);

                    string MM = rnd.Next(0, 60).ToString();//00 - 60 minutes
                    if (Convert.ToInt32(MM.ToString()) >= 0 && Convert.ToInt32(MM.ToString()) <= 9)
                        MM = Fix(MM);

                    string SS = rnd.Next(0, 60).ToString();//00 - 60 seconds
                    if (Convert.ToInt32(SS.ToString()) >= 0 && Convert.ToInt32(SS.ToString()) <= 9)
                        SS = Fix(SS);

                    string datetime = date1 + " " + HH + ":" + MM + ":" + SS;

                    int suma = rnd.Next(5, 21);//сума транзакції від 5 до 20

                    int type_trans = GetTransType(TransType);//id транзакції

                    DBMySQL db = new DBMySQL();
                    db.InsertValues(id, datetime, type_trans, suma);
                }
            }
        }

        private int GetTransType(int[] b)
        {
            Random rnd = new Random();
            int type = 0;
            while (true)
            {
                type = rnd.Next(1, 4);//3 типи транзакцій
                if (b[type-1] == 1)
                    break;
            }
            return type;
        }

        private string Fix(string TT)
        {
            return "0" + TT;
        }

        private string DateConvert(string str)//DATE Convert
        {
            string[] split = str.Split(new Char[] { '.', ' ' });
            return split[2] + "-" + split[1] + "-" + split[0];
        }


    }

    class DBMySQL
    {
        public MySqlConnection connection;
        public string server;
        public string database;
        public string uid;
        public string password;

        public DBMySQL()
        {
            Initialize();
        }
        public void Initialize()
        {
            server = "localhost";
            database = "simulate";
            uid = "root";
            password = "121212";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.Write("Cannot connect to server.  Contact administrator;\n");
                        break;

                    case 1045:
                        Console.Write("Invalid username/password, please try again;\n");
                        break;
                }
                return false;
            }
        }
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.Write("Cannot close connection;\n", ex.Message);
                return false;
            }
        }

        public void InsertValues(int id, string datetime, int type, int suma)
        {
            string query = "INSERT INTO result(`id`, time, `type`, `suma`) VALUES (" 
                + id.ToString() + ", '" 
                + datetime + "', " 
                + type.ToString() + ", " 
                + suma.ToString() + ")";

            if (this.OpenConnection() == true)
            {

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                this.CloseConnection();
            }
        }
    }
}