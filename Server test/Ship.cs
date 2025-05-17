using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server_test
{
    // 함선 클래스
    public class Ship
    {
        public ShipType shipType;    // 함선 타입
        public Inventory inventory;  // 인벤토리
        public List<Sailor> sailors; // 선원들
        string name;
        int galaxyMovingCount = 3;    // 초은하 이동 함선 이동 가능 횟수
        Random rand = new Random(Convert.ToInt16(DateTime.Now.Ticks % 10000));
        Array resources = Enum.GetValues(typeof(Resource));
        
        // 함선 종류별 역할 모음집
        delegate void ResourceShipSkill(ShipType type);
        delegate void NewbieShipSkill(ShipType type);
        delegate void SailorShipSkill(ShipType type);
        delegate void GalaxyTravelingShipSkill(ShipType type);
        delegate void RobberShipSkill(ShipType type);

        public Ship(ShipType shipType)
        {
            this.shipType = shipType;
            
            if (shipType == ShipType.newbieShip)
                name = "초급자 전용 함선";

            else if (shipType == ShipType.resourceShip)
                name = "자원 함선";

            else if (shipType == ShipType.sailorShip)
                name = "선원 함선";

            else if (shipType == ShipType.galaxyTravelingShip)
                name = "초은하 이동 함선";

            else if (shipType == ShipType.robberShip)
                name = "도적 함선";
        }

        public ShipType GetShipType()
        {
            return shipType;
        }

        public string GetShipname()
        {
            return name;
        }

        // 자원 함선 매턴 자원 주는 함수
        // 매개 변수는 함선 종류 분별하기 위해 사용
        // 나중에 delegate 넣으면 필요 없을 수도
        public Item ResourceGive(ShipType type)
        {

            Item items = new Item(Resource.chrono,0);

            if (type != ShipType.resourceShip)
            {
                return items;
            }
            
            int mass = rand.Next(1000, 3000); // 랜덤으로 자원량 줄 것
            int index = rand.Next(0, 12); // 랜덤 자원 
            Resource resource = (Resource)index;    // 여기 틀린거 같으면 다시 구현해주세요.
            items = new Item(resource, mass);
            return items;
        }

        // 자원 증가율 -> FuelSynthesis.cs 에서

        // 초보자용 함선 처음에 아이템 주기
        public List<Item> Newbie_Resource(ShipType type)
        {
            // 초기에 주는 자원량 상수로 만들어 놓기 -> 불편하면 위로 빼도 되고

            const double chrono = 10000;
            const double seed = 10;
            const double food = 500;
            const double water = 500;
            const double epsilon = 500;
            const double hydrazine = 500;
            const double peroxide = 500;
            const double hydrogen = 500;
            const double nitrogen = 500;
            const double oxygen = 500;
            const double epsilonCrystal = 500;

            List<Item> items = new List<Item>();

            if (type != ShipType.newbieShip)
            {
                return items; // 맞는 종류인지 확인
            }

            items.Add(new Item(Resource.chrono, chrono));
            items.Add(new Item(Resource.water, water));
            items.Add(new Item(Resource.epsilon, epsilon));
            items.Add(new Item(Resource.seed, seed));
            items.Add(new Item(Resource.hydrazine, hydrazine));
            items.Add(new Item(Resource.hydrogen, hydrogen));
            items.Add(new Item(Resource.nitrogen, nitrogen));
            items.Add(new Item(Resource.peroxide, peroxide));
            items.Add(new Item(Resource.epsilonCrystal, epsilonCrystal));
            items.Add(new Item(Resource.oxygen, oxygen));
            items.Add(new Item(Resource.food, food));

            return items;
        }

        // 선원 함선

        // 매턴 선원 식량 주는거 - 20턴 동안
        public List<Item> Sailor_Resource(ShipType type)
        {
            List<Item> items = new List<Item>();

            if (type != ShipType.sailorShip)
            {
                return items; // 맞는 종류인지 확인
            }

            items.Add(new Item(Resource.food, 3));
            items.Add(new Item(Resource.water, 2));
            return items;
        }

        // 처음에 선원 3명 주는 함수
        public List<Sailor> SailorGive(ShipType type)
        {
            List<Sailor> sailors = new List<Sailor>();

            for (int i = 0; i < 3; i++)
            {
                sailors.Add(new Sailor(SailorType.normal, 1, 140));
            }
            return sailors;
        }

        // 초은하 이동 함선
        public bool isGalaxyTravelShip(ShipType type)
        {
            if (type != ShipType.galaxyTravelingShip)
            {
                return false;
            }
            if (galaxyMovingCount > 0)
            {
                galaxyMovingCount--;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // TODO : 현재 은하, 현재 행성시스템 추가

    // 함선 타입
    public enum ShipType
    {
        none,                // 선택되지 않음
        newbieShip,          // 초급자 전용 함선
        resourceShip,        // 자원 함선
        sailorShip,          // 선원 함선
        galaxyTravelingShip, // 초은하 이동 함선
        robberShip            // 도적 함선
    }
}