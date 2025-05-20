using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server_test
{
    class Work
    {
        WorkType type;
        string name;

        FuelSynthesis fuelSynthesis;
        public Work(WorkType workType)
        {
            type = workType;
            if (type == WorkType.shipControl)
                name = "함선 조종"; // complete

            else if (type == WorkType.farming)
                name = "농사";

            else if (type == WorkType.collection)
                name = "채집";

            else if(type == WorkType.itemUse)
                name = "아이템 사용"; // 90% complete - except 등잔밑이 어둡다

            else if(type == WorkType.itemSynthesis)
                name = "아이템 합성";

            else if(type == WorkType.store)
                name = "상점";

            else if (type == WorkType.none)
                name = "없음";
        }

        public WorkType GetWorkType()
        {
            return type;
        }

        public string GetWorkName()
        {
            return name;
        }

        // 함선 조종
        
        // 함선 다음 위치 반환
        public Vector2 ShipMoving(Planet planet)
        { 
            return planet.location;
        }

        public Vector2 ShipMoving(PlanetSystem system)
        {
            return system.Location;
            
        }

        public Vector2 ShipMoving(Galaxy galaxy)
        {
            return galaxy.Location;
        }

        // 퍼옥사이드하고 하이드라진 이용하는 행성계 간 이동
        double ReturnFuel(double fuel, Resource fuelType, Vector2 location, PlanetSystem system, int sale)
        {
            if (fuelType == Resource.epsilon)
            {
                return -1;
            }
            double distance = Math.Sqrt((location.y - system.Location.y) * (location.y - system.Location.y) 
                + (location.x - system.Location.y) * (location.x - system.Location.y));
            double fuels = fuel - distance * (fuelType == Resource.peroxide ? 10.2 : 8) / (sale > 0 ? 4 : 20) * 4;
            if (fuels < 0)
            {
                return -1; // -1 이면 이동 못한다는 의미로 이해하고 못가게 하기
            }
            return fuels; // 아니면 이동하는거지
        }

        // 엑실론 사용하는 은하간 이동
        double ReturnFuel(double fuel, Resource fuelType, Vector2 location, Galaxy galaxy)
        {
            if (fuelType != Resource.epsilon)
            {
                return -1;
            }
            double distance = Math.Sqrt((location.y - galaxy.Location.y) * (location.y - galaxy.Location.y) 
                + (location.x - galaxy.Location.y) * (location.x - galaxy.Location.y));
            double fuels = fuel - distance * 4;
            if (fuels < 0)
            {
                return -1; // 똑같아요 위에하고
            }
            return fuels;
        }

        // 아이템 사용 == 능력 사용

        // 은하 탐방
        List<Vector2> UsingGalaxyMove(Ability ability)
        {
            if (ability.GetAbilityType() == AbilityType.galaxyTravel)
            {
                List<Vector2> locationList = new List<Vector2>();
                foreach (var robber in Server.robbers)
                {
                    locationList.Add(robber.galaxy.Location);
                }
                return locationList;
            }
            return null;
        }

        // 행성 탐방

       List<Vector2> UsingPlanetTravel(Ability ability)
        {
            List<Vector2> location_list = new List<Vector2>();
            if (ability.GetAbilityType() == AbilityType.planetTravel)
            {
                foreach(var robber in Server.robbers)
                {
                    location_list.Add(robber.advancedPlanetsystem.Location);
                }
                return location_list;
            }
            return location_list;
        }

        // 경찰 기본스킬 - 행성계 들어가면 도둑들 스턴        -- 얘는 그냥 움직일 때마다 사용하는거라서 매개변수 필요 x

        public void Stun_Robber(Vector2 location)
        {
            foreach(var robber in Server.robbers)
            {
                if(robber.planetSystem.Location == location)
                {
                    robber.isMoving = false;
                }
            }
        }

        // 팀 식별 - 전체 공용
        public List<Client.PlayerType> Team_Identify(PlanetSystem planetSystem)
        {
            List<Client.PlayerType> returnList= new List<Client.PlayerType>();
            foreach(var client in Server.clients)
            {
                if(planetSystem.Location == client.planetSystem.Location)
                {
                    returnList.Add(client.playerType);
                }
            }
            return returnList;
        }
        
        // 겟 퓨얼 - 도둑

        public Item GetFuel(Resource resource)
        {
            Random rand = new Random();
            Item returnItem = new Item(resource, rand.Next(20, 80));
            return returnItem;
        }

        // 연료 교환권 - 도둑

        // List에서 0번째는 본인이 요청한 연료, 1번째는 본인이 줄 연료 == 0 : 추가, 1 : 차감
        public List<Item> FuelChanged(Resource resource, double mass, Resource resource2, double mass2, string name)
        {
            List<Item> returnItem = new List<Item>();
            foreach(var robber in Server.robbers)
            {
                if (robber.HelpRobber(resource, mass, name))
                {
                    returnItem.Add(new Item(resource, mass));
                    returnItem.Add(new Item(resource2, mass2));
                    robber.inventory.AddItem(new Item(resource2, mass2));
                    robber.inventory.AddItem(new Item(resource, -mass));
                    break;
                }
            }
            return returnItem;
        }

        // 수갑
        public void Handcuff(PlanetSystem planetSystem, Planet planet)
        {
            foreach (var robber in Server.robbers)
            {
                if (robber.planetSystem.Location == planetSystem.Location)
                {
                    if (robber.planet.location == planet.location)
                    {
                        //  잡힌 상태 - 감옥에 가둬놓는 형태
                        robber.galaxy = Server.prison.galaxy;
                        Server.prison.AddRobber();
                        if (Server.prison.IsFinish())
                        {
                            // TODO : 게임 끝내는 기능 추가
                        }
                    }
                }
            }
        }

        // 스턴 && 스턴 제거기
        public void Stun(Vector2 location)
        {
            foreach (var robber in Server.robbers)
            {
                if (robber.galaxy.Location == location)
                {
                    robber.isMoving = false;

                    // TODO : 게임 구현 후 2턴 만들어서 그거로 스턴 해제 하고 그러기
                }
            }
        }

        public void StunRemover(string nick)
        {
            foreach (var robber in Server.robbers)
            {
                if (robber.nickname == nick)
                {
                    robber.isMoving = true;
                }
            }
        }

        // 연료 압축기
        public int ReturnSaleFuel()
        {
            return 40;
        }



        // 채집

        // 씨앗 심기
        // 이 친구는 씨앗 수집 이후 진행되어야함
        public Farm Plant(int seedCount, Farm farm)
        {
            if (farm.day1 == 0 || farm.water == 0)
            {
                farm = new Farm(farm.day1 + seedCount, farm.day2, farm.day3);
                return farm;
            }
            else return null;
        }

        // 음식 채집
        public double Collection(Farm farm, double water)
        {
            farm.Water(water);          // 씨앗 쑥쑥 키우기
            return farm.ReturnFood();       // 완료된 음식 반환
        }

        // 상점
        public Tuple<List<Item>,List<Ability>> Store(double chrono, Client.PlayerType playerType,Planet planet, List<Item> itemBuyList, List<Ability>abilityBuyList)
        {
            Tuple<List<Item>, List<Ability>> returnTuple;
            List<Item> itemList = new List<Item>();
            List<Ability> abilityList = new List<Ability>();
            if(planet.storeType == StoreType.sailorStore || planet.storeType == StoreType.publicStore)
            {
                
            }
            else if(planet.storeType == StoreType.copsStore && playerType == Client.PlayerType.cop)
            {
                foreach (var it in itemBuyList)
                {
                    Tuple<Item, double> tuple = planet.store.ReturnItem(it, chrono);
                    if (chrono == -1)
                    {
                        // -1이면 돈이 부족
                        break;
                    }
                    else
                    {
                        chrono = tuple.Item2;
                        itemList.Add(tuple.Item1);
                    }
                }
                foreach(var ab in abilityBuyList)
                {
                    Tuple<Ability, double> tuple = planet.store.ReturnAbility(ab, chrono);
                    if(chrono == -1)
                    {
                        // -1이면 돈 부족
                        break;
                    }
                    else
                    {
                        chrono = tuple.Item2;
                        abilityList.Add(tuple.Item1);
                    }
                }
            }
            else if(planet.storeType == StoreType.blackMarket && playerType == Client.PlayerType.robber)
            {
                foreach (var it in itemBuyList)
                {
                    Tuple<Item, double> tuple = planet.store.ReturnItem(it, chrono);
                    if (chrono == -1)
                    {
                        // -1이면 돈이 부족
                        break;
                    }
                    else
                    {
                        chrono = tuple.Item2;
                        itemList.Add(tuple.Item1);
                    }
                }
                foreach (var ab in abilityBuyList)
                {
                    Tuple<Ability, double> tuple = planet.store.ReturnAbility(ab, chrono);
                    if (chrono == -1)
                    {
                        // -1이면 돈 부족
                        break;
                    }
                    else
                    {
                        chrono = tuple.Item2;
                        abilityList.Add(tuple.Item1);
                    }
                }
            }
            else
            {
                
            }
            returnTuple = new Tuple<List<Item>,List<Ability>>(itemList,abilityList);
            return returnTuple;
        }


        // 아이템(연료) 합성

        
        public List<Item> ItemSynthesis(Dictionary<Resource,double>synthesisItemList, ShipType type)
        {
            fuelSynthesis.Remove();
            foreach (var item in synthesisItemList)
            {
                fuelSynthesis = new FuelSynthesis(new Item(item.Key, item.Value), type);
            }
            return fuelSynthesis.ReturnFuel();
        }

        
    }

    // 작업 타입
    enum WorkType
    {
        none,          // 없음
        shipControl,   // 함선 조종 - complete
        farming,       // 농사 - complete?
        collection,    // 채집 - complete?
        itemUse,       // 아이템 사용 - complete
        itemSynthesis, // 아이템 합성 - complete
        store          // 상점 - complete
    }
}