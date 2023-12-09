using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myChatApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private TcpListener server;

        private void Form2_Load(object sender, EventArgs e)
        {
            // 서버 생성
            server = new TcpListener(IPAddress.Any, 1015);
            server.Start();

            // 수신 메시지를 저장할 버퍼
            Byte[] bytes = new byte[256];
            String data = null;

            // 클라이언트 연결 및 메시지 수신(멀티스레딩)
            Thread thread = new Thread(() =>
            {
                // 클라이언트 메시지 수신 루프
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    data = null;
                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBox2.AppendText("상대방: " + data + "\r\n");
                        });
                    }
                    client.Close();
                }
            });

            thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 송신할 메시지 변수
            String message = textBox1.Text;

            // 클라이언트 연결 및 메시지 송신
            Thread thread = new Thread(() =>
            {
                using (TcpClient client = new TcpClient("localhost", 1014))
                {
                    NetworkStream stream = client.GetStream();

                    byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);

                    string str = System.Text.Encoding.UTF8.GetString(msg);

                    stream.Write(msg, 0, msg.Length);
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBox2.AppendText("나: " + str + "\r\n");
                        textBox1.Text = "";
                    });
                }
            });

            thread.Start();
        }
    }
}
