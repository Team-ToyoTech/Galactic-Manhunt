using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_test
{
    // 클라이언트 클래스
    public class Client
    {
        public TcpClient client;
        public string nickname;
        public Ship ship;
        public Inventory inventory;
        public Galaxy galaxy;
        public PlanetSystem planetSystem;
        public List<Sailor> sailors;
        public PlanetSystem advancedPlanetsystem; // 행성 탐방 스킬때문에 사용
        public Planet planet;
        public int clientNumber;    // 사용자 realNumber
        public PlayerType playerType;
        public bool isMoving = true; // 행동 가능/불가능 판별 변수
        public int fuelSale = 0;     // 연료 압축기로 줄이는 비율 - 할인 비율 40%

        // TODO : 2턴동안만 못움직이게 하기 -> 턴을 어떻게 가지고 오냐 -> 일단 게임 구현 해야함
        

        public Client(TcpClient client, int n)
        {
            this.client = client;
            nickname = "Client" + n.ToString();
            inventory = new Inventory(140,100);
            clientNumber = n;
        }


        // 겟 퓨얼 찬성 / 반대 함수
        public bool HelpRobber(Resource resource, double mass, string name) // name 은 비교하는 대상이 스킬 사용 대상과 같은지 비교 위해서
        {
            if (name == nickname)
            {
                return false;
            }

            foreach (var item in inventory.Items)
            {
                if (item.GetItemType() == resource)
                {
                    if (item.mass >= mass)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        

        public void Send(string type, string msg)
        {
            client.GetStream().Write(Encoding.UTF8.GetBytes(type + "⧫" + msg + "◊"));
        }

        // 개인의 역할 정하기
        public void TypeSelection(PlayerType type)
        {
            playerType = type;
        }

        public void GalaxySelection(Vector2 loc)
        {
            galaxy = new Galaxy(loc.x,loc.y);
            
        }
        
        public PlayerType GetType()
        {
            return playerType;
        }
        public enum PlayerType
        {
            robber,
            cop
        }
    }
}