using System.Net.Sockets;
using System.Text;

namespace Client_test
{
    public partial class ChatClient : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        int mynum;
        bool isConnected;
        string nickname;
        static string str;
        static bool isGameStarted;
        internal Job job;
        internal ShipType ship;
        internal Inventory storage;

        internal Dictionary<Job, string> jobDisplay = new Dictionary<Job, string> // enum Job에 따른 한글 표시
        {
            { Job.robber, "도둑" },
            { Job.cop, "경찰" }
        };

        internal Dictionary<ShipType, string> shipDisplay = new Dictionary<ShipType, string> // enum ShipType에 따른 한글 표시
        {
            { ShipType.newbieShip, "초급자 전용 함선" },
            { ShipType.resourceShip, "자원 함선" },
            { ShipType.sailorShip, "선원 함선" },
            { ShipType.galaxyTravelingShip, "초은하 이동 함선" },
            { ShipType.robberShip, "도적 함선" }
        };

        public ChatClient()
        {
            InitializeComponent();
            button2.Enabled = false;
            isConnected = false;
            isGameStarted = false;

            storage = new Inventory(0, 0); // 아이템 최대량, 능력 최대량
        }

        public void Send(string type, string msg)
        {
            stream.Write(Encoding.UTF8.GetBytes(type + "⧫" + msg + "◊"));
        }

        public void GetShip()
        {
            Invoke(new Action(() => label4.Text = "직업: " + jobDisplay[job] + "\n" + "함선: " + shipDisplay[ship]));
            Send("9", (int)ship + "");
            //stream.Write(Encoding.UTF8.GetBytes("9⧫" + (int)ship + '◊'));
            // TODO: Server에서 함선 선택 끝나기 전까지 로딩 화면 구현
        }

        private void button1_Click(object sender, EventArgs e) // 연결
        {
            try
            {
                if (int.TryParse(textBox3.Text, out int port))
                {
                    client = new TcpClient(textBox2.Text, port);
                    stream = client.GetStream();
                    receiveThread = new Thread(ReceiveMessages);
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                    listBox1.Items.Add("Connected to Server...");
                    isConnected = true;
                    button1.Enabled = false;  // 연결
                    button2.Enabled = true;   // 연결 해제
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[102400];
            string msg = "";

            while (true)
            {
                try
                {
                    buffer = new byte[102400];
                    if (msg != "")
                    {
                        buffer = Encoding.UTF8.GetBytes(msg);
                    }
                    while (true)
                    {
                        byte[] data = new byte[256];
                        int bytesRead = stream.Read(data, 0, data.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        data = data.Where(x => x != 0).ToArray();

                        if (buffer.Length == 102400)
                        {
                            buffer = data;
                        }
                        else
                        {
                            buffer = buffer.Concat(data).ToArray();
                        }

                        msg = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        if (msg.Contains('◊'))
                        {
                            break;
                        }
                    }
                    if (Encoding.UTF8.GetString(buffer, 0, buffer.Length).Split("◊").Length == 1)
                    {
                        msg = "";
                    }
                    else
                    {
                        msg = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Split("◊")[1];
                    }
                    string[] message = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Split("◊")[0].Split('⧫');

                    if (message[0] == "1") // 연결 종료
                    {
                        client.Close();
                        if (message[1] != "")
                        {
                            MessageBox.Show(message[1]);
                        }

                        Invoke(new Action(() =>
                        {
                            listBox1.Items.Add("DisConnected from server...");
                            isConnected = false;
                            button1.Enabled = true;    // 연결
                            button2.Enabled = false;   // 연결 해제
                            textBox4.Enabled = true;   // 닉네임
                            listBox2.Items.Clear();
                        }));

                        break;
                    }

                    else if (message[0] == "2") // 번호 지정
                    {
                        mynum = int.Parse(message[1]);
                        Invoke(new Action(() => str = textBox4.Text));
                        if (str == "")
                        {
                            nickname = "Client" + mynum.ToString();
                            Invoke(new Action(() => textBox4.Text = nickname));
                        }
                        else if (!str.Contains('⧫'))
                        {
                            nickname = str;
                        }
                        else
                        {
                            MessageBox.Show("이름에 다음 문자가 포함되어서는 안됩니다: ⧫\n기본 이름으로 진행합니다.");
                            nickname = "Client" + mynum.ToString();
                            Invoke(new Action(() => textBox4.Text = nickname));
                        }
                        Send("3", nickname);
                        //stream.Write(Encoding.UTF8.GetBytes("3⧫" + nickname + '◊'));
                        stream.Flush();
                    }
                    else if (message[0] == "4") // 접속한 클라이언트 이름
                    {
                        Invoke(new Action(() => listBox2.Items.Add(message[1])));
                    }
                    else if (message[0] == "5") // 접속 종료한 클라이언트 이름
                    {
                        Invoke(new Action(() => listBox2.Items.Remove(message[1])));
                    }
                    else if (message[0] == "6") // 게임 시작
                    {
                        isGameStarted = true;
                        Invoke(new Action(() =>
                        {
                            ShipSelection shipSelection = new ShipSelection(this);
                            shipSelection.Show();
                        }));
                        button4.Enabled = true;
                        button5.Enabled = true;
                    }
                    else if (message[0] == "7") // 게임 종료
                    {
                        isGameStarted = false;
                    }
                    else if (message[0] == "8") // 역할 전송 받기
                    {
                        job = (Job)int.Parse(message[1]);
                        Invoke(new Action(() => label4.Text = "직업: " + jobDisplay[job] + "\n"));
                    }
                    else if (message[0] == "10") // 모두 선택 확인
                    {
                        // TODO: 게임 시작 구현
                    }
                        Invoke(new Action(() => listBox1.TopIndex = listBox1.Items.Count - 1));
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) // 연결 해제
        {
            Send("1", "");
            //stream.Write(Encoding.UTF8.GetBytes("1⧫◊"));
            stream.Flush();
            stream.Close();
            client.Close();
            listBox1.Items.Add("DisConnected from Server...");
            isConnected = false;
            button1.Enabled = true;    // 연결
            button2.Enabled = false;   // 연결 해제
            textBox4.Enabled = true;   // 닉네임
            listBox2.Items.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // 프로그램 종료
        {
            if (isConnected)
            {
                Send("1", "");
                //stream.Write(Encoding.UTF8.GetBytes("1⧫◊"));
                stream.Flush();
                stream.Close();
                client.Close();
                listBox1.Items.Add("DisConnected from server...");
                isConnected = false;
                button1.Enabled = true;  // 연결
                button2.Enabled = false; // 연결 해제
                textBox4.Enabled = true; // 닉네임
            }
        }

        private void button5_Click(object sender, EventArgs e) // 저장고 확인
        {
            Storage storage = new Storage(this);
            storage.Show();
        }

        private void button4_Click(object sender, EventArgs e) // 할 일 선택
        {
            // TODO: TaskSelection 본인 턴 확인 후 실행
            // TODO: TaskSelection 혹은 다른 Task창이 열러있는지 확인 후 아무것도 열려있지 않을 때만 열기

            //if (본인 턴)
            //{
            TaskSelection taskSelection = new TaskSelection(this, false);
            taskSelection.Show();
            //}
            //else
            //{
            //    TaskSelection taskSelection = new TaskSelection(this, true);
            //    taskSelection.Show();
            //}
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // 엔터 누르면 연결
            {
                button1.PerformClick(); // 연결 버튼
            }
        }
    }

    // 직업 타입
    enum Job
    {
        robber, // 도둑
        cop     // 경찰
    };
}