using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

namespace Server_test
{
    internal partial class Server : Form
    {
        static TcpListener server;
        public static List<Client> clients;
        Thread T;
        List<Thread> Tt;
        static bool isServerRun;
        static bool isClosing;
        public static List<Client> cops;
        public static List<Client> robbers;
        public static Prison prison;
        Map map;
        List<Galaxy> galaxyList = new List<Galaxy>();
        int k = 0;  // clientRealNumber때문에 만든거
        public List<Client> Robbers
        {
            get { return robbers; }
        }

        public Server()
        {
            InitializeComponent();
            clients = new List<Client>();
            isServerRun = false;
            T = new Thread(() => ServerLoop(1111));
            Tt = new List<Thread>();
            button2.Enabled = false; // 서버 종료
            button4.Enabled = false; // 게임 시작
            button5.Enabled = false; // 게임 종료
            isClosing = false;
            label2.Text = "로컬 IP주소:\n" + GetLocalIPAddress() + "\n외부 IP주소:\n" + GetExternalIPAddress();
            button6.Enabled = false;
            button6.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e) // 서버 시작
        {
            if (int.TryParse(textBox1.Text, out int port) && 0 < port && port < 100000)
            {
                T = new Thread(() => ServerLoop(port));
                T.IsBackground = true;
                T.Start();
                button1.Enabled = false; // 서버 시작
                button2.Enabled = true;  // 서버 종료
                button4.Enabled = true;  // 게임 시작
                isServerRun = true;
                listBox1.Items.Add("Server started");
            }
            else
            {
                MessageBox.Show("포트는 1에서 99999 사이의 정수를 입력해 주세요");
            }
        }

        /* 입력 코드 */

        // 1 : 연결종료
        // 2 : 번호 지정(서버=>클라이언트)
        // 3 : 닉네임 전송(클라이언트=>서버)
        // 4 : 접속한 클라이언트 이름
        // 5 : 접속 종료한 클라이언트 이름
        // 6 : 게임 시작
        // 7 : 게임 종료
        // 8 : 역할 전송 (ex: 8⧫0◊, 0이면 도둑, 1이면 경찰)
        // 9 : 선택한 함선 전송 (클라이언트 => 서버)
        // 10: 모두 함선 선택 완료 (서버 => 클라이언트)


        // 11: 자원 채집
        // 12: 상점 이용
        // 13: 환전
        // 14: 은하 이동
        // 15: 항성계 이동

        /*==================*/
        // 능력 사용 - 총 11개 - 100

        // 경찰 
        // darkUnderTheLamp      등잔 밑이 어둡다 : 16
        // galaxyTravel          은하 탐방 : 17
        // planetTravel          행성 탐방 : 18
        // stun                  스턴 : 19  -- ok
        // handcuff,             수갑 : 20  
        // teamIdentify,         팀 식별 : 21

        // 도둑
        // getFuel,              겟 퓨얼 : 22--ok
        // fuelChanger,          연료 교환권 : 23
        // fuelCompressor,       연료 압축기 : 24--ok
        // stunRemover,          스턴 제거기 : 25--ok

        // 공통
        // storageGrowth         저장량 증가 : 26 -- server_ok

        /*==================*/
        // 능력 구매 - 총 11개 - 200

        // 경찰 
        // darkUnderTheLamp 등잔 밑이 어둡다 : 27
        // galaxyTravel     은하 탐방 : 28
        // planetTravel     행성 탐방 : 29
        // stun             스턴 : 30
        // handcuff,        수갑 : 31
        // teamIdentify,    팀 식별 : 32

        // 도둑
        // getFuel,         겟 퓨얼 : 33
        // fuelChanger,     연료 교환권 : 34
        // fuelCompressor,  연료 압축기 : 35
        // stunRemover,     스턴 제거기 : 36

        // 공통
        // storageGrowth    저장량 증가 : 37

        /*==================*/
        // 농사 지은거 채집: 38
        // 연료 합성 : 39
        // 농사하기 : 40
        // 감옥 넣기 : 41
        // 저장고 확인 : 42

        // Split 문자 : ⧫
        // 송신 Check 문자 : ◊


        public void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);
            while (dateTimeAdd >= dateTimeNow)
            {
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
            return;
        }

        // Thread func
        void ServerLoop(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isServerRun = true;

            int count = 0;

            while (true)
            {
                try
                {
                    clients.Add(new Client(server.AcceptTcpClient(), count));
                    Invoke(new Action(() => listBox2.Items.Add(clients[clients.Count - 1].nickname)));
                    count++;

                    Tt.Add(new Thread(() => ClientCheck(clients.Count - 1, count)));
                    Delay(100);
                    clients[clients.Count - 1].client.GetStream().Write(Encoding.UTF8.GetBytes($"2⧫{count}◊"));
                    Tt[Tt.Count - 1].IsBackground = true;
                    Tt[Tt.Count - 1].Start();
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        void ClientCheck(int clientRealNumber, int clientN)
        {
            Client client = clients[clientRealNumber];
            NetworkStream stream = clients[clientRealNumber].client.GetStream();
            byte[] buffer = new byte[102400];
            buffer[102399] = 255;
            bool error = false;
            string msg = "";
            while (isServerRun)
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
                    if (message[0] == "0")
                    {
                        Invoke(new Action(() => listBox1.Items.Add(message[1])));

                        foreach (var c in clients)
                        {
                            if (c != client)
                            {
                                NetworkStream cStream = c.client.GetStream();
                                byte[] responseBytes = Encoding.UTF8.GetBytes("0⧫" + message[1] + '◊');
                                cStream.Write(responseBytes, 0, responseBytes.Length);
                            }
                        }
                    }
                    else if (message[0] == "1")
                    {
                        Invoke(new Action(() => listBox1.Items.Add($"{client.nickname} disconnected...")));
                        Invoke(new Action(() => listBox2.Items.Remove(client.nickname)));
                        foreach (var c in clients)
                        {
                            NetworkStream cStream = c.client.GetStream();
                            byte[] responseBytes = buffer;
                            if (c != client)
                            {
                                cStream.Write(Encoding.UTF8.GetBytes($"0⧫{client.nickname} disconnected...◊"));
                                cStream.Flush();
                                Delay(100);
                                cStream.Write(Encoding.UTF8.GetBytes($"5⧫{client.nickname}◊"));
                                cStream.Flush();
                            }

                        }
                        break;
                    }
                    else if (message[0] == "3")
                    {
                        foreach (var c in clients)
                        {
                            if (c.nickname == message[1])
                            {
                                string nickname = "";
                                foreach (var c2 in clients)
                                {
                                    if (c2 != client)
                                        nickname += c2.nickname + ", ";
                                }
                                client.client.GetStream().Write(Encoding.UTF8.GetBytes("1⧫닉네임은 다음과 같을 수 없습니다: " + nickname + '◊'));
                                clients.Remove(client);
                                Invoke(new Action(() => listBox2.Items.Remove(client.nickname)));
                                int b = 0;
                                error = true;
                                int a = 10 / b;
                            }
                        }
                        clients.Remove(client);
                        Invoke(new Action(() => listBox2.Items.Remove(client.nickname)));
                        client.nickname = message[1];
                        foreach (var c in clients)
                        {
                            client.client.GetStream().Write(Encoding.UTF8.GetBytes("4⧫" + c.nickname + '◊'));
                            client.client.GetStream().Flush();
                            Delay(100);
                        }
                        clients.Add(client);
                        foreach (var c in clients)
                        {
                            c.client.GetStream().Write(Encoding.UTF8.GetBytes("4⧫" + client.nickname + '◊'));
                        }
                        Invoke(new Action(() => listBox2.Items.Add(client.nickname)));
                        Invoke(new Action(() => listBox1.Items.Add($"{message[1]} joined")));
                        buffer = Encoding.UTF8.GetBytes($"0⧫{client.nickname} joined◊");
                        foreach (var c in clients)
                        {
                            NetworkStream s = c.client.GetStream();
                            s.Write(buffer, 0, buffer.Length);
                        }
                    }
                    else if (message[0] == "9")
                    {
                        client.ship.shipType = (ShipType)int.Parse(message[1]);
                        bool isAllSelected = true;
                        foreach (var c in clients)
                        {
                            if (c.ship.shipType == ShipType.none)
                            {
                                isAllSelected = false;
                            }
                        }
                        if (isAllSelected)
                        {
                            client.client.GetStream().Write(Encoding.UTF8.GetBytes("10⧫◊"));
                        }
                    }

                    else if (message[0] == "100")   // 능력 사용
                    {
                        ClientUse(clientRealNumber, message);
                        client.client.GetStream().Write(Encoding.UTF8.GetBytes(message[1] + "⧫◊")); // 사용 성공 함수
                    }
                    else if (message[0] == "200")   // 아이템 구매
                    {
                        ClientItemPurchase(clientRealNumber, message);
                        client.client.GetStream().Write(Encoding.UTF8.GetBytes(message[1] + "⧫◊"));
                    }
                    else if (message[0] == "300")   // 능력 구매
                    {
                        ClientAbilityPurchase(clientRealNumber, message);
                        client.client.GetStream().Write(Encoding.UTF8.GetBytes(message[1] + "⧫◊"));
                    }
                    Invoke(new Action(() => listBox1.TopIndex = listBox1.Items.Count - 1));
                }
                catch (Exception e)
                {
                    break;
                }
            }
            client.client.Close();
            if (!isClosing)
            {
                Invoke(new Action(() => listBox1.Items.Remove(client.nickname)));
                clients.Remove(client);
            }
        }

        // 구매 통신 함수
        void ClientItemPurchase(int clientRealNumber, string[] msg)
        {
            
        }

        void ClientAbilityPurchase(int clientRealNumber, string[] msg)
        {

        }

        // 사용 통신 함수
        void ClientUse(int clientRealNumber, string[] msg)
        {

            Work work = new Work(WorkType.itemUse);
            // 스턴
            if (msg[1] == "19")
            {
                Vector2 locate = clients[clientRealNumber].galaxy.location;
                foreach (var client in clients)
                {
                    if (client.clientNumber != clientRealNumber)
                    {
                        if (client.galaxy.Location == locate && client.playerType == Client.PlayerType.robber)
                        {
                            client.isMoving = false;
                        }
                    }
                }
            }
            // 수갑
            if (msg[1] == "20")
            {
                Vector2 locates = clients[clientRealNumber].planetSystem.location;
                Vector2 locate = clients[clientRealNumber].galaxy.Location;
                foreach (var client in clients)
                {
                    if (client.playerType == Client.PlayerType.robber && client.galaxy.location == locate && client.planetSystem.location == locates)
                    {
                        client.galaxy = prison.galaxy;
                        prison.AddRobber();
                        if (prison.IsFinish())
                        {
                            // TODO : 게임 끝내는 함수 만들고 호출하기.
                        }
                    }
                }
            }
            // 겟 퓨얼
            if (msg[1] == "22")
            {
                Resource resource = new Resource();
                if (msg[2] == " hydrogen") resource = Resource.hydrogen;
                else if (msg[2] == "nitrogen") resource = Resource.nitrogen;
                else if (msg[2] == "oxygen") resource = Resource.oxygen;
                else if (msg[2] == "epsilonCrystal") resource = Resource.epsilonCrystal;
                else if (msg[2] == "peroxide") resource = Resource.peroxide;
                else if (msg[2] == "hydrazine") resource = Resource.hydrazine;
                else if (msg[2] == "epsilon") resource = Resource.epsilon;
                else if (msg[2] == "water") resource = Resource.water;
                else if (msg[2] == "food") resource = Resource.food;
                else if (msg[2] == "seed") resource = Resource.seed;
                else if (msg[2] == "chrono") resource = Resource.chrono;

                clients[clientRealNumber].inventory.AddItem(new Item(resource, work.GetFuel(resource).mass));
            }
            // 연료 압축기
            if (msg[1] == "24")
            {
                Resource resource = new Resource();
                if (msg[2] == " hydrogen") resource = Resource.hydrogen;
                else if (msg[2] == "nitrogen") resource = Resource.nitrogen;
                else if (msg[2] == "oxygen") resource = Resource.oxygen;
                else if (msg[2] == "epsilonCrystal") resource = Resource.epsilonCrystal;
                else if (msg[2] == "peroxide") resource = Resource.peroxide;
                else if (msg[2] == "hydrazine") resource = Resource.hydrazine;
                else if (msg[2] == "epsilon") resource = Resource.epsilon;
                else if (msg[2] == "water") resource = Resource.water;
                else if (msg[2] == "food") resource = Resource.food;
                else if (msg[2] == "seed") resource = Resource.seed;
                else if (msg[2] == "chrono") resource = Resource.chrono;
                clients[clientRealNumber].inventory.AddItem(new Item(resource, work.ReturnSaleFuel()));
            }
            // 스턴 제거기
            if (msg[1] == "25")
            {
                clients[clientRealNumber].isMoving = true;
            }

            // 저장량 증가
            if (msg[1] == "26")
            {
                int level = clients[clientRealNumber].inventory.inventoryLevel;
                if (level == 0)
                {
                    clients[clientRealNumber].inventory.SetItemMax(9000);
                    clients[clientRealNumber].inventory.inventoryLevel++;
                }
                else if (level == 1)
                {
                    clients[clientRealNumber].inventory.SetItemMax(13000);
                    clients[clientRealNumber].inventory.inventoryLevel++;
                }
                else if (level == 2)
                {
                    clients[clientRealNumber].inventory.SetItemMax(17000);
                    clients[clientRealNumber].inventory.inventoryLevel++;
                }
            }
            


            // client 번호 반환 함수
            int returnClientNumber(int number)
            {
                int num = 0;
                foreach (var client in clients)
                {

                    if (client.clientNumber == number) return num;
                    num++;
                }
                return num;
            }

            void ClientWork(int clientRealNumber)
            {
                clientRealNumber = returnClientNumber(clientRealNumber);
                Client client = clients[clientRealNumber];
                NetworkStream stream = client.client.GetStream();
                byte[] buffer = new byte[102400];
                buffer[102399] = 255;
                bool error = false;
                string msg = "";
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // 프로그램 종료
        {
            isClosing = true;
            foreach (var c in clients)
            {
                NetworkStream n = c.client.GetStream();
                n.Write(Encoding.UTF8.GetBytes("1⧫◊"));
                c.client.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) // 서버 종료
        {
            foreach (var c in clients)
            {
                c.client.GetStream().Write(Encoding.UTF8.GetBytes("1⧫◊"));
                c.client.Close();
            }
            button1.Enabled = true;  // 서버 시작
            button2.Enabled = false; // 서버 종료
            isServerRun = false;
            listBox1.Items.Add("Server stopped");
            server.Stop();
            listBox2.Items.Clear();
        }

        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("로컬 IP 주소를 찾을 수 없습니다.");
        }

        static string GetExternalIPAddress()
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString("https://api.ipify.org");
                return response;
            }
        }

        private void button4_Click(object sender, EventArgs e) // 게임 시작
        {
            if (clients.Count >= 2)
            {
                button4.Enabled = false;
                button5.Enabled = true;
                Thread t = new Thread(Game);
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                MessageBox.Show("최소 2명의 플레이어가 필요합니다.");
            }
        }

        int CopsCount(int players)
        {
            return players / 2;
        }

        void Game()
        {
            // TODO: 게임 구현하기
            // TODO: 서버에 이미지 파일 하나 만들어서 전체 지도 표시할거임. 평범한 상태는 검은색, 도둑이 있는 은하는 빨간색, 경찰이 있는 은하는 파란색, 둘 다 있는 은하는 보라색 - 완
            Random rand = new Random(Convert.ToInt16(DateTime.Now.Ticks % 10000));

            foreach (var c in clients)
            {
                c.Send("6", "");
                Delay(10);
            }

            int copsCount = CopsCount(clients.Count);
            cops = new List<Client>();
            robbers = new List<Client>();
            Random random = new Random();

            List<int> selectedIndices = new List<int>();

            // 팀 가르기
            while (cops.Count < copsCount)
            {
                int index = random.Next(clients.Count);
                if (!selectedIndices.Contains(index))
                {
                    selectedIndices.Add(index);
                    cops.Add(clients[index]);
                    clients[index].TypeSelection(Client.PlayerType.cop);
                }
            }

            for (int i = 0; i < clients.Count; i++)
            {
                if (!selectedIndices.Contains(i))
                {
                    robbers.Add(clients[i]);
                    clients[i].TypeSelection(Client.PlayerType.robber);
                }
            }

            foreach (var c in cops)
            {
                c.Send("8", "1");
                Delay(10);
            }

            foreach (var r in robbers)
            {
                r.Send("8", "0");
                Delay(10);
            }

            // 랜덤 지도 생성
            bool[,] visited = new bool[760, 460];
            int maxGalaxy = (clients.Count() > 40) ? 20 : (clients.Count() < 20 ? 11 : clients.Count() / 2); // 최대는 11 ~ 20인데 20 - > 인원 / 2로
            int galaxySize = rand.Next(10, maxGalaxy);
            int prisonLocationGalaxy = rand.Next(0, galaxySize);

            for (int i = 0; i < galaxySize; i++)
            {
                int x = rand.Next(-350, 351);
                int y = rand.Next(-224, 225);
                while (visited[x + 350, y + 224] || visited[x + 3 + 350, y + 3 + 224] || visited[x + 2 + 350, y + 2 + 224] ||
                    visited[x + 1 + 350, y + 1 + 224] || visited[x - 1 + 350, y - 1 + 224] || visited[x - 2 + 350, y - 2 + 224] || visited[x - 3 + 350, y - 3 + 224])
                {
                    x = rand.Next(-350, 351);
                    y = rand.Next(-224, 225);
                }
                for (int j = -3; j <= 3; j++)
                {
                    visited[x + j + 1000, y + j + 1000] = true;
                }

                galaxyList.Add(new Galaxy(x, y));
            }
            prison = new Prison(galaxyList[prisonLocationGalaxy]);

            // client 위치 설정

            foreach (var client in clients)
            {
                int galaxyNum = rand.Next(0, galaxySize);
                client.GalaxySelection(galaxyList[galaxyNum].location);
            }

            map = new Map(clients, galaxyList);
            button6.Visible = true;
            button6.Enabled = true;
            // 순서 섞기

            clients = clients.OrderBy(_ => rand.Next()).ToList();
            // 게임 구현 시작

            int turn = 200;

            while(turn-- >= 0)
            {
                foreach(var c in clients)
                {
                    
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) // 포트 텍스트 박스에서 엔터
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick(); // 서버 시작 버튼
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            map = new Map(clients, galaxyList); // 모두 구현한 후에 게임 시작 후 버튼 활성화, 이 줄 삭제
            map.Show();
        }
    }
}