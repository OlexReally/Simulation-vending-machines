using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public partial class Form1 : Form
    {
        private const string server = "127.0.0.1";
        private const int port = 9999;
        private TcpClient client;
        private bool isConnected = false;
        private Byte[] data;
        private Byte[] bytes = new Byte[1024];
        private NetworkStream stream;
        public Form1()
        {
            InitializeComponent();

            toolStripMenuItem3.Enabled = false;

            //к-сть автоматів
            numericUpDown1.Value = 1;
            numericUpDown1.Maximum = 10;
            numericUpDown1.Minimum = 1;

            //к-сть транзакцій
            numericUpDown2.Value = 1;
            numericUpDown2.Maximum = 1000;
            numericUpDown2.Minimum = 1;

            button4.Enabled = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)//Exit
        {
            if (isConnected)
                client.Close();
            Close();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)//Check Connect
        {
            HandShake();
        }
        public void HandShake()
        {
            try
            {
                client = new TcpClient(server, port);

                string hello = "HELLO";

                data = System.Text.Encoding.ASCII.GetBytes(hello);

                stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                int i = stream.Read(bytes, 0, bytes.Length);
                string Data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                if (Data == "HI")
                {
                    //MessageBox.Show("Підключення з сервером встановлено успішно.");
                    isConnected = true;
                    toolStripMenuItem3.Enabled = true;
                    toolStripMenuItem1.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Невдалось підключитись до сервера. Спробуйте пізніше.\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Невдалось підключитись до сервера. Спробуйте пізніше.");
            }
        }

        private void ServerAnswer(int buttonNumber)
        {
            int i = stream.Read(bytes, 0, bytes.Length);
            string Data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

            if (Data == "+OK")
            {
                switch (buttonNumber)
                {
                    case 1:
                        //MessageBox.Show("Дані передані на сервер успішно.");
                        button1.Enabled = false;
                        break;

                    case 2:
                        //MessageBox.Show("Дані передані на сервер успішно.");
                        button2.Enabled = false;
                        break;

                    case 3:
                        //MessageBox.Show("Дані передані на сервер успішно.");
                        button3.Enabled = false;
                        break;

                    case 4:
                        MessageBox.Show("Симуляція завершена. Результати знаходяться в базі даних");
                        button4.Enabled = false;
                        break;

                    default:
                        break;
                }
            }
            else
                MessageBox.Show("Дані не доставлені на сервер. Спробуйте ще раз пізніше.");

            if (!button1.Enabled && !button2.Enabled && !button3.Enabled)
                button4.Enabled = true;
        }

        public void sendToServer(string message, int buttonNumber)
        {
            data = System.Text.Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            ServerAnswer(buttonNumber);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!isConnected)
            {
                MessageBox.Show("Встановіть з'єднання з сервером. Меню -> Встановити з'єднання");
                return;
            }

            sendToServer("AUTOMAT_COUNT " + numericUpDown1.Value.ToString(), 1);

            //button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Встановіть з'єднання з сервером. Меню -> Встановити з'єднання");
                return;
            }

            ushort a = 1000;
            if (checkBox1.Checked == true)//вода
                a += 100;
            if (checkBox2.Checked == true)//кава
                a += 10;
            if (checkBox3.Checked == true)//чай
                a += 1;

            if (a == 1000)
            {
                MessageBox.Show("Потрібно вибрати хоча б один тип транзакції");
                return;
            }

            sendToServer("TRANSACTION " + a.ToString(), 2);

            //button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Встановіть з'єднання з сервером. Меню -> Встановити з'єднання");
                return;
            }

            sendToServer("TRANSACTION_COUNT " + numericUpDown2.Value.ToString(), 3);

            //button3.Enabled = false;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            client.Close();
            toolStripMenuItem1.Enabled = true;
            toolStripMenuItem3.Enabled = false;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;

            isConnected = false;

            //MessageBox.Show("З'єднання з сервером розірвано");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Встановіть з'єднання з сервером. Меню -> Встановити з'єднання");
                return;
            }

            sendToServer("SIMULATE", 4);
        }
    }
}
