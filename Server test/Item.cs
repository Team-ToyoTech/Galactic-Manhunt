using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_test
{
    // 아이템 클래스
    public class Item
    {
        Resource itemType;
        string name;
        public double mass; // 질량, 단위: kg
        public static int resourceCount = 10;
        public static int airCount = 3;
        public static int compountCount = 4;
        public static int mineralCount = 2;
        public static int organicMatterCount = 1;

        public Item(Resource itemType, double mass)
        {
            this.itemType = itemType;
            this.mass = mass;
            if (itemType == Resource.hydrogen)
                name = "수소";

            else if (itemType == Resource.nitrogen)
                name = "질소";

            else if (itemType == Resource.oxygen)
                name = "산소";

            else if (itemType == Resource.peroxide)
                name = "퍼옥사이드";

            else if (itemType == Resource.hydrazine)
                name = "하이드라진";

            else if (itemType == Resource.epsilon)
                name = "엑실론";

            else if (itemType == Resource.food)
                name = "식량";

            else if (itemType == Resource.epsilonCrystal)
                name = "엑실론 크리스탈";

            else if (itemType == Resource.water)
                name = "물";

            else if (itemType == Resource.seed)
                name = "씨앗";

            else if (itemType == Resource.chrono)
                name = "크로노";
        }

        public Resource GetItemType()
        {
            return itemType;
        }

        public string GetItemname()
        {
            return name;
        }
    }

    // 자원 종류
    // 자원 추가할때마다 Item의 전역변수 Count 수정하기
    public enum Resource
    {
        hydrogen = 1,         // 수소
        nitrogen = 2,         // 질소
        oxygen = 3,           // 산소
        epsilonCrystal = 4,   // 엑실론-크리스탈

        peroxide = 5,         // 퍼옥사이드
        hydrazine = 6,        // 하이드라진
        epsilon = 7 ,         // 엑실론

        water = 8,            // 물
        food = 9,             // 식량

        seed = 10,            // 씨앗
                             
        chrono = 11           // 크로노
    }
}